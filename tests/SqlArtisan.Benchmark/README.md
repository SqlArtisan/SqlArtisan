# SqlArtisan.Benchmark

The [BenchmarkDotNet](https://benchmarkdotnet.org/) suite behind the numbers in
the root [README's Performance section](https://github.com/h-tacayama/SqlArtisan/blob/main/README.md#performance).
The numbers live there and only there; this page is how you reproduce them.

## Running it

From the repo root. No database is needed — every entrant generates SQL offline,
so nothing here opens a connection.

```bash
P=tests/SqlArtisan.Benchmark
dotnet run --project $P -c Release -- validate                        # not a measurement
dotnet run --project $P -c Release -- --filter '*SqlBuilderBenchmarks*'
dotnet run --project $P -c Release                                    # pick a suite
```

Keep the filter quoted. Run from the project directory instead and the unquoted
pattern matches `SqlBuilderBenchmarks.cs`, so the shell hands BenchmarkDotNet a
filename that matches no benchmark.

`validate` asserts that each entrant built the shared query: exactly two bind
parameters, `SELECT`/`FROM`/`JOIN`/`WHERE`/`GROUP BY`/`ORDER BY` present, in
order, and none of them fused onto the text before it, an aggregate outside the
sort, and two `GROUP BY` keys. It checks that
shape rather than the text, because dialects and alias generation spell the
same query differently — and it checks nothing else, so a wrong join or filter
would still pass. The EF Core reference is printed but not checked. Run it after
touching any entrant; a change that quietly stops parameterizing, stops
aggregating, or glues a template clause onto its neighbor, would otherwise look
like a win.

**Release configuration is required.** BenchmarkDotNet refuses to run a Debug
build, and a measurement taken on a shared or virtualized host is not comparable
to the root README's figures.

## What is measured

Every entrant builds the SQL string **and** its bind-parameter collection for the
same logical query, so the comparison is like-for-like:

```csharp
Select(u.Id.As("user_id"), u.Name.As("user_name"), Count(o.Id).As("order_count"))
    .From(u)
    .InnerJoin(o).On(u.Id == o.UserId)
    .Where(o.OrderDate >= new DateTime(2024, 1, 1)
        & o.OrderDate < new DateTime(2025, 1, 1))
    .GroupBy(u.Id, u.Name)
    .OrderBy(Count(o.Id).As("order_count").Desc)
    .Build();
```

An `INNER JOIN` plus a `GROUP BY` aggregate, filtered by two date parameters, on
the PostgreSQL dialect. Every entrant in `SqlBuilderBenchmarks` returns the
produced `(Sql, ParameterCount)` tuple so BenchmarkDotNet consumes both outputs
and cannot dead-code-eliminate the work.

Entrants fall into three categories, and only one of them is a comparison:

| Category | Entrants | Why |
|---|---|---|
| `Builders` | SqlArtisan (twice — specific and Dapper dynamic parameters), Dapper.SqlBuilder, InterpolatedSql, linq2db, Sqlify, SqlKata | The like-for-like set |
| `Baseline` | A hand-written `StringBuilder` + Dapper `DynamicParameters` | The floor — no type safety, no dialect handling |
| `ORM reference` | EF Core | Materially different work; shown only for scale |

linq2db and EF Core reuse a connection object and a `DbContext` created once in
`[GlobalSetup]` — neither is ever opened — and EF Core caches the compiled query
plan, so the loop measures warm steady state rather than first-call cost.

`ValueConverterBenchmarks` measures what a registered value converter costs SqlArtisan
itself, so it is no cross-builder comparison: one statement binding `id.Value` against
the same statement binding a type of your own the converter turns into that value
(`dotnet run --project $P -c Release -- --filter '*ValueConverterBenchmarks*'`).

## Pinned library versions

The comparison is only meaningful against the versions it was run with. The
direct ones are pinned in `SqlArtisan.Benchmark.csproj`; update this table when
they move. The last two rows are transitive — listed because their versions
float free of any direct pin, so a restore can move them without touching the
csproj. Other transitive packages in the measured path (EF Core's caching
assemblies) move in lockstep with the direct pin above them. EF Core's
`Relational` assembly is pinned directly: left to the PostgreSQL provider, it
resolves an older 10.0 patch than EF Core's own.

| Package | Version | |
|---|---|---|
| BenchmarkDotNet | 0.15.8 | |
| Dapper.SqlBuilder | 2.1.89 | |
| InterpolatedSql | 2.5.1 | |
| linq2db | 6.6.0 | |
| Microsoft.EntityFrameworkCore | 10.0.15 | |
| Microsoft.EntityFrameworkCore.Relational | 10.0.15 | |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 | |
| Sqlify | 0.3.14 | |
| SqlKata | 4.0.1 | |
| Dapper | 2.1.89 | transitive; `DynamicParameters` is measured by the baseline, Dapper.SqlBuilder and `SqlArtisan+Dapper` |
| Npgsql | 10.0.3 | transitive; under the EF Core reference |

The runtime is part of this list too: the project targets `net10.0`, and the
root README's figures were taken on .NET 10.0.15 with SDK 10.0.109. The
lightweight entrants allocate the same bytes on every run, so on the same
runtime a fresh run reproduces the *Allocated* column. Times do not travel:
across three runs on the measuring machine an entrant's time varied by up to
about 40%, enough to reorder entrants whose times are close. Compare a fresh
run's times against another fresh run on the same machine, not against the
published table.

## Reading the results

The allocation column is the firm one — the lightweight builders allocate the
same bytes every run. Treat the timing order as directional: run-to-run variance
grows for the heavier entrants, and it grows further on any host you do not
control.
