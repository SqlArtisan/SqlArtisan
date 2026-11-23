![SqlArtisan — Write SQL, in C#.](https://raw.githubusercontent.com/h-tacayama/SqlArtisan/main/assets/banner.png)

# SqlArtisan.Dapper

Dapper integration for [SqlArtisan](https://github.com/h-tacayama/SqlArtisan): call
Dapper's verbs on an `IDbConnection` with a SqlArtisan builder in place of a SQL
string. The extension builds the statement for the connection's dialect, resolved
from its provider type, and passes the text and bind parameters to Dapper.

## Installation

Packages are pre-release, so pass `--prerelease`:

```bash
dotnet add package SqlArtisan --prerelease
dotnet add package SqlArtisan.Dapper --prerelease
```

`SqlArtisan.Dapper` pulls in Dapper itself; your ADO.NET provider (Npgsql,
MySqlConnector, Microsoft.Data.SqlClient, …) is whatever you already use to open
connections.

## Usage

`UsersTable` is a [table class](https://github.com/h-tacayama/SqlArtisan/blob/main/README.md#quick-start),
and `UserDto` a plain class with settable properties.

```csharp
using SqlArtisan;
using SqlArtisan.Dapper;
using static SqlArtisan.Sql;

// Dapper maps snake_case columns to PascalCase members with this on:
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

UsersTable u = new("u");

IEnumerable<UserDto> users = await connection.QueryAsync<UserDto>(
    Select(u.Id, u.Name, u.CreatedAt)
    .From(u)
    .Where(u.Name.Like("A%"))
    .OrderBy(u.Id));

UsersTable w = new();
int affected = await connection.ExecuteAsync(
    Update(w).Set(w.Name == "renamed").Where(w.Id == 1));
```

## Documentation

- [Guide: Dapper + SqlArtisan from Scratch](https://github.com/h-tacayama/SqlArtisan/blob/main/docs/guides/dapper-quickstart.md)
  — the verbs, their `transaction` / `commandTimeout` / cancellation arguments,
  and mapping columns to members
- [SqlArtisan README](https://github.com/h-tacayama/SqlArtisan/blob/main/README.md)
  — the query builder itself

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/h-tacayama/SqlArtisan/blob/main/LICENSE) file for the full license text.
