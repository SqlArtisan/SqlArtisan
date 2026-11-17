using System.Numerics;
using System.Reflection;
using System.Text;
using static SqlArtisan.Sql;

namespace SqlArtisan.Tests;

public partial class FunctionTests
{
    [Fact]
    public void Bind_NumericValue_CorrectSql()
    {
        SqlStatement sql =
            Select(Bind(5))
            .Build();

        StringBuilder expected = new();
        expected.Append("SELECT ");
        expected.Append(":0");

        Assert.Equal(expected.ToString(), sql.Text);
        Assert.Equal(1, sql.Parameters.Count);
        Assert.Equal(5, sql.Parameters.Get<int>(":0"));
    }

    [Fact]
    public void Bind_MySql_CorrectSql()
    {
        SqlStatement sql =
            Select(Bind(5))
            .Build(Dbms.MySql);

        StringBuilder expected = new();
        expected.Append("SELECT ");
        expected.Append("?0");

        Assert.Equal(expected.ToString(), sql.Text);
        Assert.Equal(5, sql.Parameters.Get<int>("?0"));
    }

    [Fact]
    public void Bind_SqlServer_CorrectSql()
    {
        SqlStatement sql =
            Select(Bind(5))
            .Build(Dbms.SqlServer);

        StringBuilder expected = new();
        expected.Append("SELECT ");
        expected.Append("@0");

        Assert.Equal(expected.ToString(), sql.Text);
        Assert.Equal(5, sql.Parameters.Get<int>("@0"));
    }

    // Two separate Bind(...) calls, even with an equal value, mint distinct
    // markers — reuse requires sharing the same handle instance (#241).
    [Fact]
    public void Bind_CalledTwiceWithEqualValue_CreatesDistinctMarkers()
    {
        SqlStatement sql =
            Select(Bind(1), Bind(1))
            .Build();

        StringBuilder expected = new();
        expected.Append("SELECT ");
        expected.Append(":0, :1");

        Assert.Equal(expected.ToString(), sql.Text);
        Assert.Equal(2, sql.Parameters.Count);
    }

    // Sharing a Bind(...) handle across clauses reuses its marker in each — the
    // #241 mechanism this factory exposes directly.
    [Fact]
    public void Bind_SharedHandleInSelectAndGroupBy_CorrectSql()
    {
        BindValue p10 = Bind(10);
        BindValue low = Bind("Low");
        BindValue other = Bind("Other");

        SqlStatement sql =
            Select(Case(_t.Code, When(p10).Then(low), Else(other)))
            .From(_t)
            .GroupBy(Case(_t.Code, When(p10).Then(low), Else(other)))
            .Build();

        StringBuilder expected = new();
        expected.Append("SELECT ");
        expected.Append("CASE \"t\".code WHEN :0 THEN :1 ELSE :2 END ");
        expected.Append("FROM ");
        expected.Append("test_table \"t\" ");
        expected.Append("GROUP BY ");
        expected.Append("CASE \"t\".code WHEN :0 THEN :1 ELSE :2 END");

        Assert.Equal(expected.ToString(), sql.Text);
        Assert.Equal(3, sql.Parameters.Count);
        Assert.Equal(10, sql.Parameters.Get<int>(":0"));
        Assert.Equal("Low", sql.Parameters.Get<string>(":1"));
        Assert.Equal("Other", sql.Parameters.Get<string>(":2"));
    }

    [Fact]
    public void Bind_NullValue_ThrowsArgumentNullException()
    {
        ArgumentNullException ex =
            Assert.Throws<ArgumentNullException>(() => Bind(null!));

        Assert.Equal(
            "A C# null cannot be bound; use Sql.BindNull to bind SQL NULL. (Parameter 'value')",
            ex.Message);
    }

    [Fact]
    public void BindValue_NullValue_ThrowsArgumentNullException()
    {
        ArgumentNullException ex =
            Assert.Throws<ArgumentNullException>(() => new BindValue(null!));

        Assert.Equal(
            "A C# null cannot be bound; use Sql.BindNull to bind SQL NULL. (Parameter 'value')",
            ex.Message);
    }

    [Fact]
    public void BindValue_SqlExpressionValue_ThrowsArgumentException()
    {
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() => new BindValue(_t.Code + 1));

        Assert.Equal(
            "A SqlArtisan object cannot be bound; pass a .NET value, "
                + "or write an expression in place of the bind. (Parameter 'value')",
            ex.Message);
    }

    [Fact]
    public void BindValue_SubqueryValue_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => new BindValue(Select(_t.Code).From(_t)));

        Assert.Equal(
            "A SqlArtisan object cannot be bound; pass a .NET value, "
                + "or write an expression in place of the bind. (Parameter 'value')",
            ex.Message);
    }

    // Pending nodes and the built statement are SqlArtisan objects without being
    // SqlPart, and bound the object itself just the same.
    [Fact]
    public void BindValue_NonPartSqlArtisanObject_ThrowsArgumentException()
    {
        object[] values =
        [
            PercentileCont(0.5),
            Select(_t.Code).From(_t).Build(),
            Sequence("s"),
        ];

        foreach (object value in values)
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(() => new BindValue(value));

            Assert.Equal(
                "A SqlArtisan object cannot be bound; pass a .NET value, "
                    + "or write an expression in place of the bind. (Parameter 'value')",
                ex.Message);
        }
    }

    // The constructor passes a directly bindable value without its SqlArtisan-object
    // test, so a type of this assembly on that list would bind as the object itself.
    [Fact]
    public void BindValue_NoSqlArtisanTypeButAnEnum_IsDirectlyBindable()
    {
        Assembly assembly = typeof(BindValue).Assembly;
        MethodInfo isDirectlyBindableType = assembly
            .GetType("SqlArtisan.Internal.ExpressionResolver", throwOnError: true)!
            .GetMethod("IsDirectlyBindableType", BindingFlags.Static | BindingFlags.NonPublic)!;

        Type[] bindable = [.. assembly.GetTypes()
            .Where(type => !type.IsEnum && !type.ContainsGenericParameters)
            .Where(type => (bool)isDirectlyBindableType.Invoke(null, [type])!)];

        Assert.Empty(bindable);
    }

    [Fact]
    public void BindValue_DirectlyBindableValue_KeepsTheValue()
    {
        object[] values =
        [
            true, 'c', "s", new DateTime(2024, 1, 2), DateTimeOffset.UnixEpoch,
            new DateOnly(2024, 1, 2), new TimeOnly(1, 2), TimeSpan.FromSeconds(3),
            (sbyte)1, (byte)2, (short)3, (ushort)4, 5, 6u, (nint)7, (nuint)8, 9L, 10UL,
            1.5f, 2.5d, 3.5m, new Complex(1, 2), DayOfWeek.Monday, Dbms.Oracle,
            DateTimePart.Day, Guid.Empty,
        ];

        foreach (object value in values)
        {
            Assert.Same(value, new BindValue(value).Value);
        }
    }

    // An array reports its element type's assembly; a SqlArtisan enum element
    // binds as the enum itself does.
    [Fact]
    public void BindArray_SqlArtisanEnumElements_Binds()
    {
        DateTimePart[] parts = [DateTimePart.Day, DateTimePart.Month];

        SqlStatement sql =
            Select(_t.Code)
            .From(_t)
            .Where(_t.Code == Any(BindArray(parts)))
            .Build(Dbms.PostgreSql);

        Assert.Equal(
            "SELECT \"t\".code FROM test_table \"t\" WHERE \"t\".code = ANY (:0)", sql.Text);
        Assert.Equal(parts, sql.Parameters.Get<DateTimePart[]>(":0"));
    }

    [Fact]
    public void Bind_DbNullValue_ThrowsArgumentException()
    {
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() => Bind(DBNull.Value));

        Assert.Equal(
            "DBNull.Value cannot be bound; use Sql.BindNull to bind SQL NULL.", ex.Message);
    }

    [Fact]
    public void Bind_NonBindableValue_ThrowsArgumentException()
    {
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() => Bind(new object()));

        Assert.Equal("Invalid type for Bind: System.Object", ex.Message);
    }

    [Fact]
    public void Bind_SqlExpressionValue_ThrowsArgumentException()
    {
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() => Bind(_t.Code));

        Assert.Equal("Invalid type for Bind: SqlArtisan.DbColumn", ex.Message);
    }

    [Fact]
    public void BindNull_NoArguments_CorrectSql()
    {
        SqlStatement sql =
            Select(BindNull())
            .Build();

        StringBuilder expected = new();
        expected.Append("SELECT ");
        expected.Append(":0");

        Assert.Equal(expected.ToString(), sql.Text);
        Assert.Equal(1, sql.Parameters.Count);
        Assert.Equal(DBNull.Value, sql.Parameters.Get<object>(":0"));
    }

    [Fact]
    public void BindNull_WithDbType_SetsDbType()
    {
        BindValue bind = BindNull(System.Data.DbType.Int32);

        Assert.Equal(DBNull.Value, bind.Value);
        Assert.Equal(System.Data.DbType.Int32, bind.DbType);
    }
}
