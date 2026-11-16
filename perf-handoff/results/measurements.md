# 生の計測値

すべて同じクラウドコンテナ(Linux、4 コア、.NET SDK 10.0.115 / ランタイム 10.0.15)で、
コミット `f428588`(#845 マージ直後の `main`)を基準に測った。絶対値は他の環境に持ち出せない。
同じ表の中の相対比較だけが意味を持つ。

変種の名前:

| 名前 | 中身 |
|---|---|
| `base` | `f428588` そのまま |
| `e1` | `BuildCore` から `ThrowIfDuplicateClauseInBlock()` の呼び出しを消した(切り分け専用) |
| `e2` | `BuildCore` から `Validate(dbms)` の呼び出しを消した(切り分け専用) |
| `e6a` | 重複句チェックの 2 つの `TryGetValue` ブロックだけ消した(`is` の連鎖は残す) |
| `e6c` | 重複句チェックの `is` の連鎖だけ消した(`TryGetValue` は残す) |
| `p1` | パッチ 01 |
| `p2` | パッチ 01〜02 |
| `p3` | パッチ 01〜03 |
| `p4` | パッチ 01〜04 |
| `p5` | パッチ 01〜05 |
| `p7` | パッチ 01〜06 |
| `p8` | パッチ 01〜07 |
| `p9` | パッチ 01〜08 |
| `p10` | パッチ 01〜09 |

`e*` は挙動を壊す切り分け用で、パッチとしては残していない。

## 1. ベンチマーククエリだけを回すハーネス(3 シナリオ)

`tables` / `chain` / `full` の 3 つだけを 1 プロセスで回した初期版のハーネス。
1 プロセス内で 30 万回 × 9 ラウンドの最小値を取り、それを 4 プロセス分集めた最小値と中央値。
ハーネスは結果をボックス化したタプルで返すので、公式ベンチマークより 32 B 多く出る
(2,352 B ↔ 公式の 2.27 KB)。

```
base     chain: min   614.4 med   619.8  1552B | full: min  1396.0 med  1411.5  2352B |
e1       chain: min   614.3 med   621.3  1552B | full: min  1204.5 med  1210.4  2352B |
p1       chain: min   612.1 med   615.8  1552B | full: min  1237.2 med  1271.0  2352B |
p2       chain: min   622.7 med   630.3  1552B | full: min  1230.3 med  1242.3  2352B |
```

```
p2       chain: min   609.2 med   618.2  1552B | full: min  1217.8 med  1276.8  2352B |
p3       chain: min   618.9 med   622.0  1552B | full: min  1197.5 med  1245.0  2352B |
e2       chain: min   613.0 med   620.7  1552B | full: min  1332.8 med  1348.5  2352B |
```

```
p3       chain: min   613.8 med   620.9  1552B | full: min  1207.0 med  1244.2  2352B |
p4       chain: min   585.1 med   603.2  1552B | full: min  1170.3 med  1191.2  2352B |
```

```
p4       chain: min   588.8 med   594.8  1552B | full: min  1180.4 med  1183.1  2352B |
p5       chain: min   580.8 med   588.5  1552B | full: min  1158.6 med  1167.6  2272B |
```

基準のフェーズ内訳(同じハーネス、1 プロセス):

```
tables        109.6 ns     384 B
chain         615.0 ns    1552 B
full         1407.8 ns    2352 B
```

`tables` はテーブル 2 つの生成、`chain` は `Build()` 直前まで、`full` は `Build()` 込み。
`chain` と `full` は `tables` を含む。

### 切り分け(1 プロセスずつ、`full` の値、ns)

```
base: 1415.4  1422.0  1436.2  1376.5  1409.6  1432.5
e1:   1264.8  1179.1  1270.1  1227.4  1200.9  1203.2
e2:   1346.0  1348.6  1399.7  1353.5  1362.8  1358.5
```

```
base: 1411.6  1392.6  1388.3
e1:   1347.1  1210.9  1189.1
e6a:  1249.1  1243.0  1266.3
e6c:  1327.0  1315.2  1326.9
```

`e1` の 1347.1 のように、同じバイナリでもプロセスによって 100 ns 以上遅い回が出る。
1 回の値で判断せず、複数プロセスの最小値か中央値で比べること。

## 2. 全シナリオを 1 プロセスで回すハーネス(現在の `harness/Program.cs`)

13 シナリオを同じプロセスで回すと `Format` の仮想呼び出し先が多様になり、
ベンチマーククエリの値も上の表より遅く出る(`base` の `full` が 1,400 → 1,600 ns 前後)。
30 万回 × 7 ラウンドの最小値、1 プロセスずつ。

```
== base
wide30        953.9 ns    2328 B
wide30lazy    633.2 ns    1288 B
wide30static    512.5 ns     856 B
resolve        37.7 ns      24 B
simple        517.2 ns     832 B
simplecnn     541.4 ns     832 B
in20         2843.8 ns    3496 B
insert10     1873.5 ns    2064 B
and10        2101.8 ns    3336 B
subquery     1390.9 ns    1912 B
tables        111.9 ns     384 B
chain         717.0 ns    1552 B
full         1615.6 ns    2352 B
== p7
wide30        854.4 ns    2248 B
wide30lazy    504.6 ns    1208 B
wide30static    406.7 ns     776 B
resolve        34.3 ns      24 B
simple        397.9 ns     752 B
simplecnn     425.7 ns     752 B
in20         2536.2 ns    3416 B
insert10     1543.6 ns    1984 B
and10        1759.4 ns    3256 B
subquery     1117.4 ns    1832 B
tables        109.5 ns     384 B
chain         656.2 ns    1552 B
full         1257.4 ns    2272 B
== p8
wide30        813.9 ns    2248 B
wide30lazy    523.1 ns    1208 B
wide30static    411.7 ns     776 B
resolve        16.1 ns      24 B
simple        420.4 ns     752 B
simplecnn     420.7 ns     752 B
in20         2794.3 ns    3416 B
insert10     1519.7 ns    1984 B
and10        1832.4 ns    3256 B
subquery     1092.7 ns    1832 B
tables        110.1 ns     384 B
chain         650.5 ns    1552 B
full         1268.6 ns    2272 B
== p9
wide30        845.3 ns    2280 B
wide30lazy    518.2 ns    1240 B
wide30static    424.1 ns     808 B
resolve        19.1 ns      24 B
simple        438.0 ns     784 B
simplecnn     409.9 ns     784 B
in20         2542.0 ns    3448 B
insert10     1516.1 ns    2016 B
and10        1826.5 ns    3288 B
subquery     1083.7 ns    1808 B
tables        109.7 ns     384 B
chain         601.2 ns    1496 B
full         1212.6 ns    2216 B
== p10
wide30        861.4 ns    2280 B
wide30lazy    527.1 ns    1240 B
wide30static    442.5 ns     808 B
resolve        13.3 ns      24 B
simple        415.7 ns     784 B
simplecnn     422.0 ns     784 B
in20         2217.8 ns    2736 B
insert10     1407.6 ns    1680 B
and10        1780.9 ns    3288 B
subquery     1072.2 ns    1808 B
tables        109.3 ns     384 B
chain         595.0 ns    1496 B
full         1197.6 ns    2216 B
```

`resolve` の 24 B は、ハーネスが `Dbms` をボックス化して返すぶんで、ライブラリの確保ではない。
`wide30` / `wide30lazy` / `wide30static` はハーネス内に手書きした 30 列のテーブルクラスで、
ライブラリ側の変更ではない(`wide30lazy` はカラムを `??=` で遅延生成する版)。

## 3. パラメータの多い文(`H.dll 7 big`)

2,000 回 × 7 ラウンドの最小値。

```
base   in1000       448842.1 ns    153736 B
base   bulk100x10   464482.3 ns    160928 B
p5     in1000       445732.1 ns    153656 B
p5     bulk100x10   440742.2 ns    160848 B
p5     in1000       438350.3 ns    153656 B   (別プロセスでの再計測)
p5     bulk100x10   435149.3 ns    160848 B
p7     in1000       103676.1 ns    153656 B
p7     bulk100x10   108718.1 ns    160848 B
```

`in1000` は 1,000 値の `IN`、`bulk100x10` は 100 行 × 10 列の `INSERT ... VALUES`。

## 4. 公式ベンチマーク(BenchmarkDotNet、`--inProcess`)

`dotnet run -c Release --project tests/SqlArtisan.Benchmark -- --filter '*SqlArtisan_*' '*Sqlify*' --inProcess`
を、基準 → 試作(パッチ 01〜06)→ 基準 → 試作 の順に回した。

```
基準 1 回目
| Method                         | Mean     | Error     | StdDev    | Gen0   | Allocated |
| Sqlify_SpecificParams          | 1.608 us | 0.0403 us | 0.1170 us | 0.0229 |   3.13 KB |
| SqlArtisan_SpecificParams      | 1.581 us | 0.0342 us | 0.0997 us | 0.0153 |   2.27 KB |
| SqlArtisan_DapperDynamicParams | 1.785 us | 0.0358 us | 0.0871 us | 0.0210 |   2.88 KB |

試作 1 回目
| Sqlify_SpecificParams          | 1.528 us | 0.0307 us | 0.0741 us | 0.0229 |   3.13 KB |
| SqlArtisan_SpecificParams      | 1.214 us | 0.0239 us | 0.0454 us | 0.0153 |   2.19 KB |
| SqlArtisan_DapperDynamicParams | 1.469 us | 0.0287 us | 0.0762 us | 0.0210 |    2.8 KB |

基準 2 回目
| Sqlify_SpecificParams          | 1.524 us | 0.0305 us | 0.0820 us | 0.0229 |   3.13 KB |
| SqlArtisan_SpecificParams      | 1.545 us | 0.0359 us | 0.1054 us | 0.0153 |   2.27 KB |
| SqlArtisan_DapperDynamicParams | 1.763 us | 0.0545 us | 0.1573 us | 0.0191 |   2.88 KB |

試作 2 回目
| Method                         | Mean     | Error     | StdDev    | Median   | Gen0   | Allocated |
| Sqlify_SpecificParams          | 1.527 us | 0.0306 us | 0.0638 us | 1.529 us | 0.0229 |   3.13 KB |
| SqlArtisan_SpecificParams      | 1.270 us | 0.0270 us | 0.0775 us | 1.243 us | 0.0153 |   2.19 KB |
| SqlArtisan_DapperDynamicParams | 1.475 us | 0.0295 us | 0.0814 us | 1.463 us | 0.0210 |    2.8 KB |
```

パッチ 07〜09 を入れた状態では公式ベンチマークを回していない。

## 5. 挙動の確認

- 単体テスト(`dotnet test tests/SqlArtisan.Tests -c Release`)をパッチ 01〜06 の状態と
  01〜09 の状態で実行し、どちらも 2,262 件中 2,261 件が成功した。失敗は
  `LineLengthSweepTests.NoLineOver100Columns` の 1 件で、試作コードの 5 行が 100 桁を超えている:

  ```
  src/SqlArtisan/Internal/SqlBuilder/SqlBuilderBase.cs:349 (107 columns)
  src/SqlArtisan/Internal/SqlBuilder/SqlBuilderBase.cs:350 (115 columns)
  src/SqlArtisan/Internal/SqlBuilder/SqlBuilderBase.cs:456 (105 columns)
  src/SqlArtisan/Internal/SqlBuilder/SqlBuilderBase.cs:483 (126 columns)
  src/SqlArtisan/Internal/SqlBuilder/Select/SelectBuilder.cs:449 (106 columns)
  ```

- `H.dll probe`: 同じ `BindValue` インスタンスを繰り返し含む 300 要素の `IN`(パラメータ 240 個)
  を組み、SQL 文・パラメータ数・マーカー名の並び・値のチェックサムを出力する。
  `base` と `p10` の出力はバイト単位で一致した。
- `SqlArtisan.Analyzers.Tests`、`SqlArtisan.TableClassGen.Tests`、結合テスト、
  `dotnet format --verify-no-changes` は試作に対して実行していない。
