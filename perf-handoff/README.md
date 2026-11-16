# #845 の後続: Build 経路の性能改善候補(引き継ぎ)

#845(重複句チェックの辞書化)のあとに残っている改善余地を、試作と実測で調べた結果。
このフォルダだけで別のセッションが再現と実装に入れるようにしてある。

- 基準コミット: `f428588`(#845 マージ直後の `main`)。
- `src/` と `tests/` は変更していない。試作は `patches/` の差分としてだけ存在する。
- このフォルダは引き継ぎ用で、`main` に入れるものではない。実装が済んだら消す。

| パス | 中身 |
|---|---|
| `README.md` | この文書。結論、施策ごとの中身と実装時の注意 |
| `results/measurements.md` | 生の計測値と、変種の名前(`base` / `e1` / `p1` …)の対応 |
| `patches/01`〜`09` | 試作のステップ別差分。番号順に積む前提(下記) |
| `harness/` | 時間とアロケーションを測るコンソール。`SqlArtisan.sln` には入っていない |

## 結論

公式ベンチマークのクエリは、パッチ 01〜06 で 1.56 → 1.24 µs(約 −20%)、2.27 → 2.19 KB になった
(BenchmarkDotNet `--inProcess`、基準と試作を交互に 2 回ずつ。同じ回の Sqlify は 1.52〜1.61 µs)。
クラウドコンテナ上の値なので、相対比較としてだけ読むこと。

| パッチ | 施策 | 実測 | 判断 |
|---|---|---|---|
| 01 + 02 | 重複句チェックで句の種別を各ノードの仮想プロパティから取る。排他判定はビットマスク | ベンチマーククエリで約 −170 ns(−12%)、確保バイト数は不変 | 推奨。全ての文に効く |
| 06 | パラメータ重複検出を、32 個を超えたら借り物のハッシュ索引に切り替える | 1,000 値の `IN`: 438 → 104 µs。100 行 × 10 列 `INSERT`: 435 → 109 µs。確保バイト数は不変 | 推奨。現状は O(n²) |
| 09 | `IN` と `VALUES` の描画前にパラメータリストの容量を確保する | 20 値の `IN`: 3,448 → 2,736 B、10 列 `INSERT`: 2,016 → 1,680 B | 推奨。試作は過剰確保する場合がある |
| 04 | 暗黙バインドで `BindValue` コンストラクタの型検査を省く | 2 バインドで −20〜50 ns | 推奨。バインド数に比例 |
| 05 | `SqlBuildingBuffer` をスレッド単位で再利用する | −80 B、−15〜20 ns | 任意。状態の持ち越しリスクあり |
| 03 | `SelectBuilder.Validate` を 1 回の走査にする | −20〜30 ns | 任意。単独では小さい |
| 07 | 接続の型から DBMS を引く処理を `Type` キーでキャッシュする | `DbmsResolver.Resolve`: 38 → 13〜19 ns | 任意。小さい |
| 08 | 句リストの初期容量を 4 → 8 にする | 7 句のクエリで −56 B、4 句以下のクエリで +32 B | ユーザー判断。得失が分かれる |

ベンチマークに現れないが大きいもの(ライブラリの変更ではなく、ハーネス内の手書きクラスで測った):
30 列のテーブルクラスを毎回 `new` すると、4 列しか使わないクエリでも 954 ns / 2,328 B かかる。
カラムを遅延生成すると 633 ns / 1,288 B、インスタンスを `static readonly` で使い回すと
513 ns / 856 B。詳細は「テーブルクラス」の節。

## 計測の方法

- 環境: クラウドコンテナ(Linux、4 コア)、.NET SDK 10.0.115、ランタイム 10.0.15。
  ライブラリは `net8.0` ターゲットのまま、ハーネスは `net10.0` で実行。
- CPU プロファイラは使えていない(`dotnet-trace` の導入が権限で拒否された)。
  代わりに、処理を 1 つずつ消すか差し替えた変種を別ツリーに作り、同じハーネスで比べた。
  関数単位の内訳は取れていないので、ここに挙げた以外のホットスポットが残っている可能性はある。
- ハーネスは各シナリオを 40 万回ウォームアップしたあと、30 万回 × N ラウンドの最小値を出す。
  確保バイト数は `GC.GetAllocatedBytesForCurrentThread` の 1,000 回平均で、毎回同じ値になる。
- 同じバイナリでもプロセスによって 100 ns 以上遅い回がある。時間は複数プロセスの最小値か
  中央値で比べること。20 ns 未満の差は測り分けられていないと考えてよい。
- シナリオを 3 つだけ回したときと 13 個回したときで、同じクエリの絶対値が変わる
  (基準の `full` が 1,400 ns 前後 → 1,600 ns 前後)。`results/measurements.md` の 1 と 2 は
  別の表として読むこと。

### 再現手順

```bash
# 基準(このリポジトリの src/ を参照)
cd perf-handoff/harness
dotnet build -c Release -o out/base
dotnet out/base/H.dll 7          # 13 シナリオ。7 はラウンド数
dotnet out/base/H.dll 7 big      # in1000 / bulk100x10
dotnet out/base/H.dll probe      # 同一 BindValue の再利用を含む文の出力

# 変種(別ツリーにパッチを当てて、そのツリーを参照してビルド)
git worktree add /tmp/sa-variant f428588
cd /tmp/sa-variant
for p in /path/to/perf-handoff/patches/0[1-6]-*.diff; do git apply "$p"; done
cd /path/to/perf-handoff/harness
dotnet build -c Release -o out/variant \
    -p:SaPath=/tmp/sa-variant/src/SqlArtisan/SqlArtisan.csproj
dotnet out/variant/H.dll 7
```

公式ベンチマークは `sa-run-benchmark` スキルの手順どおり。今回は
`--filter '*SqlArtisan_*' '*Sqlify*' --inProcess` で回した。別のツリーと同時に回すと
値が乱れるので、1 本ずつ交互に回す。

## パッチの読み方

- 9 本とも `f428588` に対する差分を、番号順に積んだもの。`git apply` で順に当たることと、
  9 本を当てた結果が計測に使ったツリーと一致することは確認済み。
- 一部だけ使う場合、01 → 02 は依存する。06 は 05 の `Dispose` の形を文脈にしているので、
  05 なしで当てるなら `Dispose` の差分を手で合わせる。09 は 06 のあとの `AddParameter` 周辺を
  文脈にしている。ロジック上の依存は 01 → 02 だけ。
- どれも計測用の試作で、そのままは出せない。行長ゲートに落ちる行があり、コメントも
  テストも付けていない。各節の「本実装で要ること」を満たす形で書き直す前提。

## 01 + 02: 重複句チェックの種別判定

### 測ったこと

`SqlBuilderBase.ThrowIfDuplicateClauseInBlock` は #845 のあとも、ベンチマーククエリの
`Build()` 込み 1,400 ns のうち約 200 ns を使っている(`e1`)。内訳は 2 つの
`FrozenDictionary<Type, int>` の検索が約 150 ns(`e6a`)、`part is X or Y …` の連鎖が
約 80 ns(`e6c`)。句は 7 個なので、検索 1 回あたり約 10 ns になる。

パッチ 01 + 02 で `full` は中央値 1,411 → 1,242 ns。チェックを丸ごと消した `e1` が 1,210 ns
なので、チェックの費用の約 85% が消えている。

### 試作の中身

- `SqlPart` に `internal virtual int WalkCode => 0;` を足し、64 個の句クラスで上書きする。
  値は 3 つを詰めた整数: 下位 8 ビットが役割、次の 8 ビットが `OncePerBlockClauses` の添字 + 1、
  その次の 8 ビットが `OncePerBranchClauses` の添字 + 1。
- 役割の番号: 1 = `ON` / 結合の `USING`、2 = 集合演算子、3 = `SELECT` 句 6 種、
  4 = MERGE の `WHEN` 3 種、5 = `INSERT INTO` / `INSERT IGNORE INTO`、
  6 = `InsertValuesClause` / `InsertSetClause`、7 = 条件を要する結合 5 種、
  8 = 条件を取らない結合 9 種。
- チェックのループは `part.WalkCode` を 1 回読み、役割と種別を整数比較で判定する。
  例外メッセージと受理・拒否の判断は元のまま。
- パッチ 02 は、種別ごとの排他相手をビットマスクにして、相手が 1 つも見えていないときは
  `ExclusiveKinds` の走査に入らないようにする。

### 元の判定と同じになる根拠

- 辞書は実行時の型そのもので引いていた。表に載っている型は全て `sealed` なので、
  上書きを継承する派生型はない。
- `is` は派生型にも合う。該当するのは `RequirementOnlyOnClause : OnClause` だけで、
  `OnClause` の上書き(役割 1)を継承するので `part is OnClause` と同じ結果になる。
  `OnClause` はどちらの表にも載っていない。
- `part is ISelectItemsClause` は役割 3 に置き換えた。このインターフェースを実装するのは
  `SelectClause*` の 6 クラスだけ。

### 本実装で要ること

- 整数リテラルをやめ、役割と種別を型(列挙など)で表す。種別の名前表
  (`OncePerBlockClauses` の `Name`)と各クラスの上書きがずれない置き場を決める。
  #845 の「同じ型を 2 つの種別に載せたら `Dictionary.Add` が投げる」という保証は、
  1 クラス 1 上書きなら構造上成り立つ。
- 試作は `OncePerBlockKindByType` / `OncePerBranchKindByType` と `IndexKindsByType` を
  未使用のまま残している。消すこと。
- パッチ 02 のループは `for` の継続条件にマスク判定を押し込んだ計測用の書き方。
  マスクに当たったときだけ相手の名前を探して投げる形に書き直す。
- 新しい句クラスを足したときに上書きを忘れると、チェックを黙って素通りする。
  それを落とすゲートが要る(辞書方式でも表への追加忘れは同じだが、いまは表が 1 か所にある)。
- `SqlBuilderBase.cs` の「Each part's kind in one probe … about 15%」のコメントは内容が変わる。
- CHANGELOG の `[Unreleased]` にある #845 の項目を書き換える(同じ未リリース内の変更なので、
  項目を足さずに直す。`docs-style.md`)。
- `.claude/rules/guards-and-empty-states.md` § "When to throw" は、チェックを `Build()` 時に
  残す理由として「`AddPart` ごとに走らせると build 時間の 0.3〜2.4 倍」という見積もりを
  挙げている。チェックが軽くなるとこの数字は変わる。チェックの位置を動かす話ではないので
  決定は変えないが、記録の数字を直すかはユーザー判断。

### 測っていない代案

辞書を 1 つにまとめ、値に役割と種別を詰める(`RequirementOnlyOnClause` も明示的に載せる)。
変更は `SqlBuilderBase.cs` だけで済む。検索が 2 回から 1 回になり `is` の連鎖も消えるので
効果は出るはずだが、測っていない。

## 06: パラメータ重複検出のハッシュ索引

### 測ったこと

`SqlBuildingBuffer.AddParameter` は、同じ `BindValue` インスタンスに同じマーカーを返すために
既存パラメータを毎回線形に走査する。1,000 パラメータで 438 µs、そのうち約 330 µs が
この走査になる。20 パラメータでは差が出ない。

既存コメントの「37 ms at 2,100 binds」は再現していない(2,100 個では測っていない)。

### 試作の中身

- パラメータが 32 個に達したら `ArrayPool<int>.Shared` から表を借り、既存分を登録する。
  表はオープンアドレス法で、要素は「パラメータの添字 + 1」、ハッシュは
  `RuntimeHelpers.GetHashCode`、照合は `ReferenceEquals`。
- 表の大きさは 128 から始め、パラメータ数の 2 倍を超えるたびに倍にして詰め直す。
  借りた配列は要求より大きいことがあるので、要求した大きさのマスクだけを使う。
- `Dispose` で表を返す。32 個未満の文は元の線形走査のままで、追加の確保も借用もない。

### 本実装で要ること

- 既存の単体テストがハッシュ経路(32 個超で同じインスタンスが再登場)を通っているかは
  確認していない。`H.dll probe` で基準と出力が一致することは見たが、テストを足すこと。
- `AddOutParameter` は表に登録しない。出力パラメータの `BindValue` はその場で作られ、
  再び `AddParameter` に来ないので実害はないが、意図として書くか登録するかを決める。
- しきい値 32 と初期サイズ 128 は決め打ち。しきい値を変えた計測はしていない。
- 「Linear by design (ADR 0006's best effort)」のコメントを書き換える。

## 09: パラメータリストの容量確保

`List<KeyValuePair<string, BindValue>>` が 4 → 8 → 16 → 32 と伸びるたびに配列を捨てている。
試作は `SqlBuildingBuffer.ReserveParameters(int)` を足し、`InCondition.Format` と
`InsertValuesClause.Format` の先頭で呼ぶ(5 個以上のときだけ)。

本実装で要ること: 試作は要素数をそのまま確保するので、`IN` の要素がカラムや
同じ `BindValue` の繰り返しだと余分に確保する。バインドの数を数えるか、過剰確保を
許すかを決める。`Sql.Values(...)` の派生テーブルなど、他の多値の位置は見ていない。

## 04: 暗黙バインドの型検査

`BindValue` の公開コンストラクタは、値が SqlArtisan のオブジェクトでないことを
`value.GetType()` の `GetElementType()` / `IsEnum` / `Assembly` で確かめる。
`ExpressionResolver.BindImplicitly` の最後の分岐に来る値は `IsDirectlyBindable` が真で、
関数引数の列挙でもないことが確認済みなので、この検査は必ず通る
(`bool`、文字、日時、数値、`Guid`、列挙のどれか)。

試作は `internal BindValue(object value, byte trusted)` という仮の署名のコンストラクタを足し、
その 1 か所だけ差し替えた。本実装では署名を決め直す。同じ理屈が成り立つ呼び出し元
(`Sql.Bind` の直接バインド分岐、`Limit` / `Offset` / `Fetch` / `Top` の `int`)は
試作では触っていない。

## 05: ビルドバッファのスレッド単位再利用

`[ThreadStatic]` に `SqlBuildingBuffer` を 1 つ持ち、`Build()` のたびに借りて返す。
バッファ本体(80 B)の確保と `ArrayPool<char>` の貸し借りが消える。

本実装で要ること:

- 返すときに全ての状態を初期値に戻す。試作は当時のフィールドを手で並べている。
  フィールドを足したときに戻し忘れると、前の文の状態が次の文に漏れる。
- 同じスレッドでの入れ子(借りている最中の別の `Build()`)は新しいバッファを作って
  しのいでいる。入れ子が実際に起きる経路があるかは調べていない。
- 文字配列が 8,192 文字を超えたバッファは保持せずプールに返す。保持するバッファは
  スレッドごとに 1 つ(初期 2,048 文字)残り続ける。
- `Dbms` プロパティと `_dialect` を書き換え可能にしている。

得られるのは 80 B(公式ベンチマークで 2.27 → 2.19 KB)と 15〜20 ns で、
漏れたときの害(別の文のガード状態で描画する)に対して小さい。入れるかはユーザー判断。

## 03: `SelectBuilder.Validate` の 1 回走査

`Validate` 全体はベンチマーククエリで約 60 ns(`e2`)。`FindPart<T>()` を 3〜4 回呼ぶのを、
型の `switch` 1 回の走査に置き換えて −20〜30 ns。試作は `SelectBuilder` だけで、
`ITopSelectClause` の判定を実装クラス 2 つの型判定に変えている。

`InsertBuilder.Validate` は `FindPart` を十数回と、句リストを受け取って走査するガードを
4 つ呼び、さらに `SelectBuilder.Validate` も呼ぶが、試作していない。01 の走査が「どの種別があったか」の
ビット集合を返せば、ない種別の `FindPart` を省ける。これも測っていない。

## 07: `DbmsResolver` の型キャッシュ

`Resolve` は接続の型の `FullName` を大文字小文字無視の文字列辞書で引く(38 ns)。
`ConcurrentDictionary<Type, Dbms>` を前段に置くと 13〜19 ns。解決できた結果だけを覚える。
`RegisterProvider` は先勝ちで登録を消さないので、覚えた結果が古くなることはない。
未解決(`Unknown`)は覚えないので、あとから登録した型は解決されるようになる。

`Build(cnn)` 1 回あたり約 20 ns で、単純な `SELECT` の計測では差が出なかった。

## 08: 句リストの初期容量

`SqlBuilderBase.ExpectedClauseCount` は 4。5 句以上のクエリは配列を 1 回捨てる。
8 にすると 7 句のベンチマーククエリは −56 B・`chain` が約 −50 ns、3 句の単純な `SELECT` は
+32 B になる。どちらのクエリ形を優先するかで決まる。

## テーブルクラス(ライブラリ外で測ったもの)

生成されるテーブルクラスも README の手書き例も、コンストラクタで全カラムの `DbColumn` を作る。
README とクイックスタートはクエリごとに `new` する書き方になっている。

| 30 列のテーブル、4 列を使う `SELECT ... WHERE` | 時間 | 確保 |
|---|---|---|
| クエリごとに `new`(現状の書き方) | 954 ns | 2,328 B |
| カラムを `??=` で遅延生成 | 633 ns | 1,288 B |
| `static readonly` のインスタンスを使い回す | 513 ns | 856 B |

- 使い回しはコード変更なしでできる。テーブルインスタンスは生成後に変わらない。
  ただしドキュメントにその案内がなく、別名の違うインスタンスが要る場面
  (自己結合)との書き分けも説明が要る。
- 遅延生成は TableClassGen の出力形の変更になる。生成ファイルを読む側
  (`--check` / `--fix` の `CommittedFile`、アナライザのスキーマ規則)への影響は調べていない。

## 試作も計測もしていない候補

- `ToDynamicParameters`(`SqlArtisan.Dapper`): `ForEach` に渡すラムダがクロージャと
  デリゲートを確保する。計算上 88 B(公式ベンチマークの Dapper 版 2.88 KB のうち)。
  なくすには `SqlParameters` に状態を渡せる列挙の公開 API が要る。
- `SqlStatement` → `SqlParameters` → `List` → 配列の 4 段。`SqlParameters` が配列と個数を
  直接持てば 24〜32 B 減る見込み。
- `a & b & c & …` の長い連結は、3 つ目以降の追加のたびに配列を作り直す
  (`LogicalConditionCore`)。10 連結の `and10` は 3,336 B。
- `params object[]` の配列(ベンチマーククエリで計 152 B)。`params ReadOnlySpan<object>` の
  オーバーロードで消せるが、公開 API が倍になる。
- `EncloseInAliasQuotes` は引用符を方言インターフェース越しに 2 回読む。
- 上の「測っていない代案」(辞書 1 つにまとめる)と、`InsertBuilder.Validate` の走査削減。

## 実装時に守るリポジトリの決まり

- 重複句チェックを `Build()` から `AddPart` に移す案は決定済みで不採用
  (`guards-and-empty-states.md` § "When to throw")。軽くするのは構わない。
- 呼び出し側の配列を複製しない・書き換えない(`public-api-design.md`
  § "A node's fluent member returns a new node" の末尾)。
- DBMS による分岐は `DbmsDialect` の中だけ。
- README の性能表と `docs/comparison.md` の記述は、ユーザーの計測機
  (#845 の記載では i5-1135G7 / Windows 11)で取った値に基づく。クラウドで取った時間を
  ドキュメントに書かない。確保バイト数は環境に依らず再現するが、05 や 08 を入れると
  表の *Allocated* 列が変わり、README と `llms-full.txt` の更新が要る
  (`tools/regen-llms-full.sh`、`LlmsFullTests`)。
- 性能に関する主張は公式ベンチマークを根拠にする(ADR 0006)。このフォルダのハーネスは
  切り分け用で、ドキュメントの根拠にはしない。
- コメントは理由だけ、行は 100 桁まで、`dotnet format SqlArtisan.sln` と
  `dotnet test` 3 本を通す(`CLAUDE.md`)。

## 調べていないこと

- アナライザの速度(ビルド時、IDE 上)。呼び出し 1 件に 10 以上のコールバックが
  登録されているのは見たが、遅いかどうかは測っていない。
- `SqlArtisan.ArrayBind` の実行経路。
- 試作に対する `SqlArtisan.Analyzers.Tests`、`SqlArtisan.TableClassGen.Tests`、結合テスト、
  `dotnet format --verify-no-changes`。
- ユーザーの計測機での公式ベンチマーク。
