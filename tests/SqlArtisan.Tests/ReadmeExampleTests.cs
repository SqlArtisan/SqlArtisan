using System.Text.RegularExpressions;
using static SqlArtisan.Sql;

namespace SqlArtisan.Tests;

// The root README's snippets share one table class and reach nuget.org and llms-full.txt,
// but verify_sql_examples.py sweeps docs/ only, so a column the class lacked once shipped.
public class ReadmeExampleTests
{
    private static readonly Regex CSharpBlockPattern = new(
        @"^[ \t]*```csharp[ \t]*\r?\n(?<body>.*?)^[ \t]*```",
        RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

    [Fact]
    public void ReadmeExamples_TableClassMembers_AreDeclaredByTheReadmeTableClass()
    {
        string readme = ReadReadme();
        List<string> blocks =
            [.. CSharpBlockPattern.Matches(readme).Select(m => m.Groups["body"].Value)];
        Dictionary<string, HashSet<string>> columnsByClass = [];

        foreach (string block in blocks)
        {
            Match declaration = Regex.Match(block, @"class (\w+) : DbTableBase");

            if (declaration.Success)
            {
                MatchCollection columns = Regex.Matches(block, @"public DbColumn (\w+)");
                columnsByClass[declaration.Groups[1].Value] =
                    [.. columns.Select(m => m.Groups[1].Value)];
            }
        }

        Assert.NotEmpty(columnsByClass);

        // A later snippet reuses a local an earlier one declared (Manual Execution's `u`), so
        // the locals are resolved across the whole README, not block by block.
        Dictionary<string, string> classByLocal = [];

        foreach (Match local in Regex.Matches(readme, @"\b(\w+) (\w+) = new\("))
        {
            if (columnsByClass.ContainsKey(local.Groups[1].Value))
            {
                classByLocal[local.Groups[2].Value] = local.Groups[1].Value;
            }
        }

        int checkedReferences = 0;

        foreach (string block in blocks)
        {
            foreach ((string name, string type) in classByLocal)
            {
                foreach (Match reference in Regex.Matches(block, $@"\b{name}\.(\w+)"))
                {
                    string member = reference.Groups[1].Value;
                    Assert.True(
                        columnsByClass[type].Contains(member),
                        $"README.md uses {name}.{member}, which its {type} class lacks.");
                    checkedReferences++;
                }
            }
        }

        Assert.True(checkedReferences > 0, "README.md's snippets reference no table class column.");
    }

    // The SQL tests below build with the mirror class, so it must say what README.md's says.
    [Fact]
    public void ReadmeTableClass_MatchesTheTestMirror()
    {
        string readme = ReadReadme();

        Assert.Contains("base(\"users\", tableAlias)", readme, StringComparison.Ordinal);

        foreach ((string property, string column) in UsersTable.Columns)
        {
            Assert.Contains(
                $"{property} = new DbColumn(this, \"{column}\");",
                readme,
                StringComparison.Ordinal);
        }

        HashSet<string> declared =
            [.. Regex.Matches(readme, @"public DbColumn (\w+)").Select(m => m.Groups[1].Value)];
        HashSet<string> mirrored = [.. UsersTable.Columns.Select(c => c.Property)];
        Assert.Equal(mirrored, declared);
    }

    [Fact]
    public void ReadmeWhyExample_CorrectSql()
    {
        bool filterByDate = true;
        UsersTable u = new();

        SqlStatement sql =
            Select(u.Id, u.Name, u.CreatedAt)
            .From(u)
            .Where(
                u.Name.Like("A%")
                & ConditionIf(filterByDate, u.CreatedAt >= new DateTime(2026, 1, 1)))
            .Build();

        Assert.Equal(
            "SELECT id, name, created_at FROM users WHERE (name LIKE :0) AND (created_at >= :1)",
            sql.Text);
        Assert.Equal("A%", sql.Parameters.Get<string>(":0"));
        Assert.Equal(new DateTime(2026, 1, 1), sql.Parameters.Get<DateTime>(":1"));

        // The chain built above, whitespace removed: README.md's must be the same code.
        string readme = ReadReadme();
        MatchCollection flags = Regex.Matches(readme, @"bool filterByDate = (\w+);");
        Assert.NotEmpty(flags);
        Assert.All(flags, flag => Assert.Equal("true", flag.Groups[1].Value));
        Assert.Equal(
            "Select(u.Id,u.Name,u.CreatedAt).From(u).Where(u.Name.Like(\"A%\")"
                + "&ConditionIf(filterByDate,u.CreatedAt>=newDateTime(2026,1,1)))",
            Chain(readme, "SqlStatement sql =", ".Build();"));
        Assert.Contains($"// {sql.Text}\n", readme, StringComparison.Ordinal);
    }

    // Quick Start runs the query Why SqlArtisan? shows; two near-copies read as differing on
    // purpose, and they drifted apart once already.
    [Fact]
    public void ReadmeQuickStartQuery_MatchesTheWhyExample()
    {
        string readme = ReadReadme();
        string why = Chain(readme, "SqlStatement sql =", ".Build();");
        string quickStart = Chain(readme, "ISqlBuilder sql =", ";");

        Assert.Equal(why, quickStart);
    }

    [Fact]
    public void ReadmeManualExecutionExample_CorrectSql()
    {
        UsersTable u = new();

        SqlStatement sql = Select(u.Id, u.Name).From(u).Where(u.Id == 10).Build();

        Assert.Equal("SELECT id, name FROM users WHERE id = :0", sql.Text);
        Assert.Equal(10, sql.Parameters.Get<int>(":0"));
        string readme = ReadReadme();
        Assert.Contains(
            "SqlStatement sql = Select(u.Id, u.Name).From(u).Where(u.Id == 10).Build();",
            readme,
            StringComparison.Ordinal);
        Assert.Contains($"// sql.Text       => \"{sql.Text}\"", readme, StringComparison.Ordinal);
    }

    // The first multi-line chain after `declaration`, whitespace collapsed, up to `end`.
    private static string Chain(string readme, string declaration, string end)
    {
        int start = readme.IndexOf(declaration + "\n", StringComparison.Ordinal);
        Assert.True(start >= 0, $"README.md has no `{declaration}` chain.");
        start += declaration.Length;
        int stop = readme.IndexOf(end, start, StringComparison.Ordinal);
        Assert.True(stop > start, $"README.md's `{declaration}` chain has no `{end}`.");

        return Regex.Replace(readme[start..stop], @"\s+", "");
    }

    private static string ReadReadme() =>
        File.ReadAllText(Path.Combine(CommentCapRatchetTests.FindRepoRoot(), "README.md"))
            .ReplaceLineEndings("\n");

    // Mirrors README.md's Quick Start table class; ReadmeTableClass_MatchesTheTestMirror holds
    // the two equal.
    private sealed class UsersTable : DbTableBase
    {
        public static readonly (string Property, string Column)[] Columns =
            [("Id", "id"), ("Name", "name"), ("CreatedAt", "created_at")];

        public UsersTable(string tableAlias = "") : base("users", tableAlias)
        {
            Id = new DbColumn(this, "id");
            Name = new DbColumn(this, "name");
            CreatedAt = new DbColumn(this, "created_at");
        }

        public DbColumn Id { get; }

        public DbColumn Name { get; }

        public DbColumn CreatedAt { get; }
    }
}
