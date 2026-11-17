# Build 経路の性能改善: 実装案 3 つ(別セッション向け)

このファイルだけで、別のセッションが手元のチェックアウトに 3 つの修正を入れられるようにしてある。
背景と実測は `README.md`、`results/adopted-walk-table.md`、`results/exploration-2.md` にある。

- 基準は `main` の `f428588`(#845 マージ直後)。3 つとも `f428588` の上に、下の順で積む。
- クラウド側で実装済みの参照ブランチがある: `perf/build-path-walk-table-and-bind-check`
  (コミット 3 つ。PR は作っていない)。手元で作り直す代わりに、これを取り込んでもよい。
- 3 つとも、SQL、パラメータ、例外とメッセージは現行と変わらない。公開 API も変わらない。

| # | 修正 | 変更の大きさ | 効果(クラウド上の相対値) | 確保 |
|---|---|---|---|---|
| 1 | 重複句チェックの表の一本化 | `SqlBuilderBase.cs` + テスト 2 ファイル | ベンチマークのクエリ −6〜9% | 不変 |
| 2 | ビルダーのルート句を `params` 配列で渡さない | 6 ファイル、+26 / −12 行 | 単純な SELECT −7%、ほかは −1〜3% | 文 1 つ(とサブクエリ 1 つ)ごとに −32 B |
| 3 | `BindValue` の検査で直接バインドできる型を先に通す | `BindValue.cs` + テスト 2 件 + CHANGELOG | 10 列 INSERT −18〜23%、20 値の `IN` −9〜25%、単純な SELECT −4〜7% | 不変 |

どれも 1 文あたり 0.05〜0.4 µs 程度の短縮で、DB の往復時間に対しては見えない。意味があるのは
README の比較表と、確保量の首位の余裕に対してだけ。

## 入れ方

どちらか一方でよい。

```bash
# (a) 参照ブランチをそのまま使う
git fetch origin perf/build-path-walk-table-and-bind-check
git checkout -b <作業ブランチ> origin/perf/build-path-walk-table-and-bind-check

# (b) main から作業ブランチを切って、パッチを順に当てる
git fetch origin main claude/performance-improvement-review-2c83xn
git checkout -b <作業ブランチ> origin/main
for p in adopted-walk-table.diff impl-02-root-parts-without-array.diff impl-03-bindvalue-direct-check.diff; do
  git show origin/claude/performance-improvement-review-2c83xn:perf-handoff/patches/$p | git apply --index
  git commit   # メッセージは下の各節
done
```

`main` が `f428588` より進んでいてパッチが当たらないときは `git apply --3way` を使うか、
下の説明のとおりに手で書く。

## 1. 重複句チェックの表の一本化

- パッチ: `patches/adopted-walk-table.diff`(コミット文は
  `patches/adopted-walk-table-commit-message.txt`)。参照ブランチでは `ddca118`。
- 中身: `SqlBuilderBase.ThrowIfDuplicateClauseInBlock` が、句 1 つにつき `FrozenDictionary` を
  2 回引いて型判定を 30 回ほど行っていたのを、厳密な型をキーにした表 1 つ(`TraitsByType`)から
  役割と 2 種類の種別を 1 回で引く形にする。排他ペアは種別ごとのビットマスクで判定する。
  元の表(`OncePerBlockClauses` など)と例外メッセージはそのまま。
- SELECT の役割は、種別の表の `"SELECT"` エントリの型一覧を参照する(一覧を 2 か所に書かない)。
- テスト: `DuplicateClauseWalkTests`(表の網羅)と `BuilderReuseTests` の 2 件。
- 詳細、変異テスト、別案との比較は `results/adopted-walk-table.md`。
- **検討済みで、やり直さないこと**: 役割を `[Flags]` にする、表にない型を `PartTraits.None` で返す、
  排他ペアの判定を元のループのまま残す形、句クラスごとの仮想プロパティ、チェックを `AddPart` に移す。

## 2. ビルダーのルート句を `params` 配列で渡さない

- パッチ: `patches/impl-02-root-parts-without-array.diff`。参照ブランチでは `48264b9`。
- 中身: すべての文が `new SelectBuilder(params SqlPart[] rootParts)` の形で 1〜2 要素の配列を作り、
  基底クラスが `List.AddRange` で写している。ルート句は 1 個か 2 個(`WITH` つきが 2 個)しか
  ないので、引数 2 つで受ける。

```csharp
// SqlBuilderBase
protected SqlBuilderBase(SqlPart rootPart, SqlPart? secondRootPart)
{
    _parts = new List<SqlPart>(ExpectedClauseCount);
    _parts.Add(rootPart);
    if (secondRootPart is not null)
    {
        _parts.Add(secondRootPart);
    }
}

// SelectBuilder(InsertBuilder / UpdateBuilder / DeleteBuilder / MergeBuilder も同じ形)
internal SelectBuilder(SqlPart rootPart, SqlPart? secondRootPart = null)
    : base(rootPart, secondRootPart)
```

- 呼び出し側(`Sql.S.cs`、`Sql.I.cs`、`Sql.U.cs`、`Sql.D.cs`、`Sql.M.cs`、`WithBuilder.cs`)は
  引数の並びが同じなので書き換え不要。
- テストの追加はない(挙動が変わらないので、既存の単体テストと出力比較が守る)。

コミット文:

```
perf: Start a builder from its root clauses without an array

Every statement passed its one or two root clauses as a params array that the
base constructor copied into the clause list. The constructors take the two
clauses themselves, so each builder, a subquery's included, allocates 32 bytes
less. The clause order and the emitted SQL are unchanged.
```

## 3. `BindValue` の検査で直接バインドできる型を先に通す

- パッチ: `patches/impl-03-bindvalue-direct-check.diff`(テストと CHANGELOG を含む)。
  参照ブランチでは `2099d8b`。
- 中身: 公開コンストラクタは値が SqlArtisan のオブジェクトでないことを
  `value is SqlPart or ISqlBuilder or ISubquery` と、型の `GetElementType()` / `IsEnum` /
  `Assembly` で確かめる。`int` や `string` は多数のインターフェースを実装しているので、外れる
  インターフェース判定 2 回が重い。`ExpressionResolver.IsDirectlyBindable(value)` が真の値
  (`bool`、文字、日時、数値、`Complex`、列挙、`Guid`)は SqlArtisan のオブジェクトでありえないので、
  先に通す。検査は残る。

```csharp
private static readonly Assembly s_assembly = typeof(BindValue).Assembly;

// コンストラクタ内
if (IsSqlArtisanObject(value))
{
    throw new ArgumentException(...);   // メッセージは現行のまま
}

private static bool IsSqlArtisanObject(object value)
{
    // No directly bindable type is one, and the tests below cost an int or a string
    // most of its bind: each implements dozens of interfaces to miss (ADR 0006).
    if (ExpressionResolver.IsDirectlyBindable(value))
    {
        return false;
    }

    if (value is SqlPart or ISqlBuilder or ISubquery)
    {
        return true;
    }

    // An array reports its element type's assembly, so BindArray's element is what
    // is tested: an array of a SqlArtisan enum binds like the enum itself.
    Type type = value.GetType();
    Type tested = type.GetElementType() ?? type;
    return !tested.IsEnum && tested.Assembly == s_assembly;
}
```

- テスト(`tests/SqlArtisan.Tests/FunctionTests/FunctionTests.B.cs` に 2 件):
  - `BindValue_NoSqlArtisanTypeButAnEnum_IsDirectlyBindable`: SqlArtisan のアセンブリの全型について、
    列挙以外で `IsDirectlyBindableType` が真になる型がないこと。直接バインドできる型の一覧に
    このアセンブリの型が足されると、検査を通らずにバインドされるので、それを落とすゲート。
  - `BindValue_DirectlyBindableValue_KeepsTheValue`: 直接バインドできる全型の値が、そのまま保持されること。
- CHANGELOG: `[Unreleased]` の `### Changed` に、数字を入れない 1 項目を足してある(パッチに含む)。
- **不採用の案との違い**: 不採用になった暗黙バインド(`ImplicitBindValue`)は、検査を通らない別の
  経路を足す案だった。これは公開コンストラクタ自体を軽くするので、経路も型も増えない。
- **やらないこと**: リゾルバ側で直接バインド型をサブクエリ判定より先に見る変更(`explore-02`)は、
  スカラーサブクエリのオペランドが +10〜17 ns 遅くなるので入れない。

コミット文:

```
perf: Pass a directly bindable value before BindValue's object test

The BindValue constructor tested every value against SqlPart, ISqlBuilder and
ISubquery and then read its type's element type and assembly. An int or a
string misses both interface tests across dozens of interfaces, which cost
most of a bind. A directly bindable value is never a SqlArtisan object, so it
now passes first; every other value is tested as before, and what the
constructor accepts, rejects and reports is unchanged. A test fails if a
non-enum type of this assembly ever becomes directly bindable.
```

## 確認(手元で回すもの)

```bash
dotnet build SqlArtisan.sln -c Release
dotnet test tests/SqlArtisan.Tests -c Release              # 2,269 件
dotnet test tests/SqlArtisan.Analyzers.Tests -c Release    # 2,166 件
dotnet test tests/SqlArtisan.TableClassGen.Tests -c Release   # 336 件
dotnet format SqlArtisan.sln --verify-no-changes
```

クラウド側では上の 5 つが通り、ハーネスの `bindguard`(値 59 種 × 経路 9 通りの受理・拒否・
メッセージ)、`probe`、`stress`、`mergemsg`、`outloop` の出力が変更前と一致した。確保は
ベンチマークのクエリで 2,352 → 2,320 B、単純な SELECT で 832 → 800 B(ハーネスの値。公式
ベンチマークより 32 B 多く出る)。

クラウド側で回していないもの: 3 つを積んだ状態での DB 不要スライスと SQLite の結合テスト、
コンテナを使う結合テスト(MySQL、Oracle、PostgreSQL、SQL Server)、基準機での公式ベンチマーク、
.NET 8 ランタイム、独立したレビュー。

## 計測と、そのあとに決めること

```bash
dotnet run -c Release --project tests/SqlArtisan.Benchmark -- --filter '*SqlBuilderBenchmarks*'
```

- 基準(`f428588`)と作業ブランチを 1 本ずつ交互に複数回走らせ、同時には走らせない。
- CHANGELOG の「about 15% faster」は #845 時点の数字のまま。計測値で差し替えるか、数字を消す。
- README のベンチマーク表は触っていない。*Mean* は計測値で差し替える。*Allocated* は
  `SqlArtisan_SpecificParams` と `SqlArtisan_DapperDynamicParams` がそれぞれ 32 B 減るはずなので、
  計測結果の表示で確かめる。README を変えたら `bash tools/regen-llms-full.sh` で `llms-full.txt` も
  更新する(`LlmsFullTests` が突き合わせる)。
- `docs/comparison.md` と ベンチマークプロジェクトの README に同じ数字があれば合わせる
  (あるかどうかは確かめていない)。
- `perf-handoff/` は引き継ぎ用で、`main` には入れない。
