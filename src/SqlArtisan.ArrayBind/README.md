![SqlArtisan — Write SQL, in C#.](https://raw.githubusercontent.com/h-tacayama/SqlArtisan/main/assets/banner.png)

# SqlArtisan.ArrayBind

Oracle array-bind execution for [SqlArtisan](https://github.com/h-tacayama/SqlArtisan):
build N statements of identical shape (typically one `INSERT`/`UPDATE`/`DELETE` per
row) the normal way, through the query builder, and run them together in **one
round trip** via ODP.NET array binding (`OracleCommand.ArrayBindCount`). It builds
no SQL itself — every statement comes from `SqlArtisan`.

## Installation

The package is pre-release, so pass `--prerelease`. It pulls
`Oracle.ManagedDataAccess.Core` (ODP.NET) transitively.

```bash
dotnet add package SqlArtisan.ArrayBind --prerelease
```

## Usage

Build one statement per row, keeping the same columns and clause shape across
rows. `UsersTable` is a [table class](https://github.com/h-tacayama/SqlArtisan/blob/main/README.md#quick-start).

```csharp
using SqlArtisan;
using static SqlArtisan.Sql;

UsersTable users = new();
UserRow[] rows = LoadRows(); // thousands of rows

IReadOnlyCollection<ISqlBuilder> statements = rows.Select(row =>
    InsertInto(users, users.Id, users.Name, users.CreatedAt)
        .Values(
            row.Id,
            row.Name is null ? BindNull() : row.Name,
            row.CreatedAt is null ? BindNull() : row.CreatedAt))
    .ToList();
```

Then execute them in one round trip:

```csharp
using Oracle.ManagedDataAccess.Client;
using SqlArtisan.ArrayBind;

using OracleConnection connection = new(connectionString);
connection.Open();
using OracleTransaction transaction = connection.BeginTransaction();

int inserted = connection.ExecuteArrayBind(statements, transaction);
// or: await connection.ExecuteArrayBindAsync(statements, transaction, cancellationToken);

transaction.Commit();
```

## Documentation

- [Guide: Oracle Array-Bind Execution](https://github.com/h-tacayama/SqlArtisan/blob/main/docs/guides/oracle-array-bind.md)
  — null values and their types, `UPDATE`/`DELETE` batches, and the misuse that
  fails before anything reaches the database
- [SqlArtisan README](https://github.com/h-tacayama/SqlArtisan/blob/main/README.md)
  — the query builder itself

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/h-tacayama/SqlArtisan/blob/main/LICENSE) file for the full license text.
