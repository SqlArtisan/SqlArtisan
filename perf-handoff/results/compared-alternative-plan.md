# 重複句チェックの1回引き化（実装案）

**狙い:** `ThrowIfDuplicateClauseInBlock` では、句1つにつき辞書引きが2回と型判定が最大約25回走っている。これを辞書引き1回とビット演算だけにする。変更するのは `src/SqlArtisan/Internal/SqlBuilder/SqlBuilderBase.cs` の1ファイルで、エラーメッセージと判定の順序は今のまま残す。

## 1. 句ごとの「性質」を1つの表にまとめる

```csharp
// 句の役割（ループ内の `is` 判定を置き換える）
[Flags]
private enum PartRole : byte
{
    None = 0,
    SetOperator = 1,        // UNION / EXCEPT / INTERSECT / MINUS
    MergeWhen = 2,          // WHEN MATCHED / NOT MATCHED / NOT MATCHED BY SOURCE
    InsertHead = 4,         // INSERT INTO / INSERT IGNORE INTO
    InsertSource = 8,       // VALUES / SET / SELECT
    SelectItems = 16,       // 集合演算子の後に来るべき SELECT
    ConditionedJoin = 32,   // ON/USING が必要な結合（5種）
    ConditionFreeJoin = 64, // 条件を取らない結合（9種、ON/USING の枠を埋める）
    JoinCondition = 128,    // ON / USING
}

private readonly record struct PartTraits(PartRole Roles, sbyte Kind, sbyte BranchKind);

private static readonly FrozenDictionary<Type, PartTraits> TraitsByType = BuildTraits();
```

- 既存の `OncePerBlockClauses` と `OncePerBranchClauses` はそのまま残す。メッセージ中の句の名前と、排他ペアの定義に使っているため。
- 役割ごとの型の一覧を1つ足し、構築するときに3つの一覧を1つの表へ合成する。
- SELECT の6型は、既存の `"SELECT"` エントリの `Types` を参照する。同じ一覧を2か所で持たないため。
- 1つの型が2つの種類に載っていたら、今と同じく構築時に例外にする。

## 2. ループの書き換え

```csharp
PartTraits t = TraitsByType.TryGetValue(part.GetType(), out PartTraits found)
    ? found : PartTraits.None;   // None = (None, -1, -1)
PartRole roles = t.Roles;

if (joinConditionPending && (roles & PartRole.JoinCondition) == 0) ...
if (setOperatorPending && (roles & PartRole.SelectItems) == 0) ...
if ((roles & PartRole.SetOperator) != 0) { ...; continue; }
...
if (t.Kind >= 0) { /* 既存の once-per-block 判定 */ }
if (t.BranchKind >= 0) { /* 既存の once-per-branch 判定 */ }
```

判定の順序、`continue` の位置、例外メッセージはすべて今のまま。この順序を変えると、違反が2つ重なった場合にどちらのメッセージが出るかが変わってしまうため。

## 3. 挙動を変えないための注意点

**`is` と辞書の違い:** `is` は派生クラスやインターフェースの実装にも当たるが、辞書は型が完全に一致したときしか当たらない。該当する型を調べた結果は次のとおり。

- `OnClause` は sealed ではなく、`RequirementOnlyOnClause` が派生している。このため両方を登録する。
- `ISelectItemsClause` を実装しているのは SELECT の6型だけ。
- 結合句、集合演算子、WHEN 句、INSERT 句は、すべて sealed。

**登録漏れの防止:** 今は `is ISelectItemsClause` などが新しい型にも自動で当たる。表に変えると、今後 SELECT 句や結合句を足したときに登録を忘れれば、チェックが黙って効かなくなる。これを防ぐゲートテストを1本足す。

- リフレクションで、次の具象型がすべて正しい役割で表に載っていることを確かめる。
  - `IJoinedRelation` を実装している型（`ConditionedJoin` か `ConditionFreeJoin` のどちらか一方）
  - `ISelectItemsClause` を実装している型
  - `OnClause` の派生クラス
- コアライブラリには `InternalsVisibleTo` がない。そこで、`BindParameterTypeContractTests` などと同じく `NonPublic` のリフレクションで private の表を読む。
- 起動時にアセンブリを走査して自動登録する方法は採らない。AOT 互換やトリミングと相性が悪いため。

## 4. 検証

- **既存テスト:** `BuilderReuseTests` と `MergeTests` にある重複句のメッセージのテストが、そのまま通ることを確かめる。
- **ゲート一式:** 警告0件でのビルド、3つのテストプロジェクト、`dotnet format`。
- **細かい計測:** 前回と同じ、リフレクションで内部メソッドを直接呼ぶ計測。7句のクエリで 227 ns → 110 ns 前後になる見込み。
- **公式ベンチマーク:** `--inProcess` で main と交互に測る。割り当てバイト（2.27 KB）は変わらない見込み。

## 5. 文書

- **CHANGELOG:** `[Unreleased]` には、前回の「約15%速くなった」というエントリがすでにある。未リリースの変更を改訂する場合は新しいエントリを足さない決まりなので、そのエントリを測定結果に合わせて書き換える。
- **README の表:** 測り直して更新する。README と `docs/comparison.md` には「Sqlify は毎回より速く、Dapper.SqlBuilder は4回中3回より速かった」という順位の記述があり、これが変わった場合はそちらも直す。

## 見込みと影響範囲

| 項目 | 内容 |
|---|---|
| 見込み | 約 110〜130 ns の短縮（ベンチマーククエリ全体の約1割） |
| 割り当て | 変化なし |
| 公開 API | 変化なし（private の内部実装だけ） |
| 主なリスク | 型の登録漏れ（ゲートテストで防ぐ） |

コミットメッセージ案: `perf: Find each clause's traits with one lookup in the duplicate-clause check`
