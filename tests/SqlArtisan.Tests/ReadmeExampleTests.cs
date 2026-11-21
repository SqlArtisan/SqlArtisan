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
        int checkedReferences = 0;

        foreach (string block in blocks)
        {
            foreach (Match local in Regex.Matches(block, @"\b(\w+) (\w+) = new\("))
            {
                string type = local.Groups[1].Value;

                if (!columnsByClass.TryGetValue(type, out HashSet<string>? columns))
                {
                    continue;
                }

                string name = local.Groups[2].Value;

                foreach (Match reference in Regex.Matches(block, $@"\b{name}\.(\w+)"))
                {
                    string member = reference.Groups[1].Value;
                    Assert.True(
                        columns.Contains(member),
                        $"README.md uses {name}.{member}, which its {type} class lacks.");
                    checkedReferences++;
                }
            }
        }

        Assert.True(checkedReferences > 0, "README.md's snippets reference no table class column.");
    }

    [Fact]
    public void ReadmeWhyExample_CorrectSql()
    {
        bool onlyActive = true;
        UsersTable u = new();

        SqlStatement sql =
            Select(u.Id, u.Name, u.CreatedAt)
            .From(u)
            .Where(u.Id > 0 & u.Name.Like("A%") & ConditionIf(onlyActive, u.StatusId == 1))
            .Build();

        Assert.Equal(
            "SELECT id, name, created_at FROM users "
                + "WHERE (id > :0) AND (name LIKE :1) AND (status_id = :2)",
            sql.Text);
        Assert.Equal(0, sql.Parameters.Get<int>(":0"));
        Assert.Equal("A%", sql.Parameters.Get<string>(":1"));
        Assert.Equal(1, sql.Parameters.Get<int>(":2"));
        Assert.Contains($"// {sql.Text}\n", ReadReadme(), StringComparison.Ordinal);
    }

    [Fact]
    public void ReadmeManualExecutionExample_CorrectSql()
    {
        UsersTable u = new();

        SqlStatement sql = Select(u.Id, u.Name).From(u).Where(u.Id == 10).Build();

        Assert.Equal("SELECT id, name FROM users WHERE id = :0", sql.Text);
        Assert.Equal(10, sql.Parameters.Get<int>(":0"));
        Assert.Contains(
            $"// sql.Text       => \"{sql.Text}\"",
            ReadReadme(),
            StringComparison.Ordinal);
    }

    private static string ReadReadme() =>
        File.ReadAllText(Path.Combine(CommentCapRatchetTests.FindRepoRoot(), "README.md"))
            .ReplaceLineEndings("\n");

    // Mirrors README.md's Quick Start table class, which the first test holds the snippets to.
    private sealed class UsersTable : DbTableBase
    {
        public UsersTable(string tableAlias = "") : base("users", tableAlias)
        {
            Id = new DbColumn(this, "id");
            Name = new DbColumn(this, "name");
            CreatedAt = new DbColumn(this, "created_at");
            StatusId = new DbColumn(this, "status_id");
        }

        public DbColumn Id { get; }

        public DbColumn Name { get; }

        public DbColumn CreatedAt { get; }

        public DbColumn StatusId { get; }
    }
}
