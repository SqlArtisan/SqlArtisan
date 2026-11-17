using System.Diagnostics;
using SqlArtisan;
using static SqlArtisan.Sql;

internal sealed class Orders : DbTableBase
{
    public Orders(string alias) : base("orders", alias)
    {
        Id = new DbColumn(this, "id");
        UserId = new DbColumn(this, "user_id");
        OrderDate = new DbColumn(this, "order_date");
    }
    public DbColumn Id { get; }
    public DbColumn UserId { get; }
    public DbColumn OrderDate { get; }
}

internal sealed class Users : DbTableBase
{
    public Users(string alias) : base("users", alias)
    {
        Id = new DbColumn(this, "id");
        Name = new DbColumn(this, "name");
        CreatedAt = new DbColumn(this, "created_at");
    }
    public DbColumn Id { get; }
    public DbColumn Name { get; }
    public DbColumn CreatedAt { get; }
}

internal sealed class Wide : DbTableBase
{
    public Wide(string alias) : base("wide", alias)
    {
        C0 = new DbColumn(this, "c0");
        C1 = new DbColumn(this, "c1");
        C2 = new DbColumn(this, "c2");
        C3 = new DbColumn(this, "c3");
        C4 = new DbColumn(this, "c4");
        C5 = new DbColumn(this, "c5");
        C6 = new DbColumn(this, "c6");
        C7 = new DbColumn(this, "c7");
        C8 = new DbColumn(this, "c8");
        C9 = new DbColumn(this, "c9");
        C10 = new DbColumn(this, "c10");
        C11 = new DbColumn(this, "c11");
        C12 = new DbColumn(this, "c12");
        C13 = new DbColumn(this, "c13");
        C14 = new DbColumn(this, "c14");
        C15 = new DbColumn(this, "c15");
        C16 = new DbColumn(this, "c16");
        C17 = new DbColumn(this, "c17");
        C18 = new DbColumn(this, "c18");
        C19 = new DbColumn(this, "c19");
        C20 = new DbColumn(this, "c20");
        C21 = new DbColumn(this, "c21");
        C22 = new DbColumn(this, "c22");
        C23 = new DbColumn(this, "c23");
        C24 = new DbColumn(this, "c24");
        C25 = new DbColumn(this, "c25");
        C26 = new DbColumn(this, "c26");
        C27 = new DbColumn(this, "c27");
        C28 = new DbColumn(this, "c28");
        C29 = new DbColumn(this, "c29");
    }
    public DbColumn C0 { get; }
    public DbColumn C1 { get; }
    public DbColumn C2 { get; }
    public DbColumn C3 { get; }
    public DbColumn C4 { get; }
    public DbColumn C5 { get; }
    public DbColumn C6 { get; }
    public DbColumn C7 { get; }
    public DbColumn C8 { get; }
    public DbColumn C9 { get; }
    public DbColumn C10 { get; }
    public DbColumn C11 { get; }
    public DbColumn C12 { get; }
    public DbColumn C13 { get; }
    public DbColumn C14 { get; }
    public DbColumn C15 { get; }
    public DbColumn C16 { get; }
    public DbColumn C17 { get; }
    public DbColumn C18 { get; }
    public DbColumn C19 { get; }
    public DbColumn C20 { get; }
    public DbColumn C21 { get; }
    public DbColumn C22 { get; }
    public DbColumn C23 { get; }
    public DbColumn C24 { get; }
    public DbColumn C25 { get; }
    public DbColumn C26 { get; }
    public DbColumn C27 { get; }
    public DbColumn C28 { get; }
    public DbColumn C29 { get; }
}

internal sealed class WideLazy : DbTableBase
{
    public WideLazy(string alias) : base("wide", alias)
    {
    }
    private DbColumn? _c0;
    public DbColumn C0 => _c0 ??= new DbColumn(this, "c0");
    private DbColumn? _c1;
    public DbColumn C1 => _c1 ??= new DbColumn(this, "c1");
    private DbColumn? _c2;
    public DbColumn C2 => _c2 ??= new DbColumn(this, "c2");
    private DbColumn? _c3;
    public DbColumn C3 => _c3 ??= new DbColumn(this, "c3");
    private DbColumn? _c4;
    public DbColumn C4 => _c4 ??= new DbColumn(this, "c4");
    private DbColumn? _c5;
    public DbColumn C5 => _c5 ??= new DbColumn(this, "c5");
    private DbColumn? _c6;
    public DbColumn C6 => _c6 ??= new DbColumn(this, "c6");
    private DbColumn? _c7;
    public DbColumn C7 => _c7 ??= new DbColumn(this, "c7");
    private DbColumn? _c8;
    public DbColumn C8 => _c8 ??= new DbColumn(this, "c8");
    private DbColumn? _c9;
    public DbColumn C9 => _c9 ??= new DbColumn(this, "c9");
    private DbColumn? _c10;
    public DbColumn C10 => _c10 ??= new DbColumn(this, "c10");
    private DbColumn? _c11;
    public DbColumn C11 => _c11 ??= new DbColumn(this, "c11");
    private DbColumn? _c12;
    public DbColumn C12 => _c12 ??= new DbColumn(this, "c12");
    private DbColumn? _c13;
    public DbColumn C13 => _c13 ??= new DbColumn(this, "c13");
    private DbColumn? _c14;
    public DbColumn C14 => _c14 ??= new DbColumn(this, "c14");
    private DbColumn? _c15;
    public DbColumn C15 => _c15 ??= new DbColumn(this, "c15");
    private DbColumn? _c16;
    public DbColumn C16 => _c16 ??= new DbColumn(this, "c16");
    private DbColumn? _c17;
    public DbColumn C17 => _c17 ??= new DbColumn(this, "c17");
    private DbColumn? _c18;
    public DbColumn C18 => _c18 ??= new DbColumn(this, "c18");
    private DbColumn? _c19;
    public DbColumn C19 => _c19 ??= new DbColumn(this, "c19");
    private DbColumn? _c20;
    public DbColumn C20 => _c20 ??= new DbColumn(this, "c20");
    private DbColumn? _c21;
    public DbColumn C21 => _c21 ??= new DbColumn(this, "c21");
    private DbColumn? _c22;
    public DbColumn C22 => _c22 ??= new DbColumn(this, "c22");
    private DbColumn? _c23;
    public DbColumn C23 => _c23 ??= new DbColumn(this, "c23");
    private DbColumn? _c24;
    public DbColumn C24 => _c24 ??= new DbColumn(this, "c24");
    private DbColumn? _c25;
    public DbColumn C25 => _c25 ??= new DbColumn(this, "c25");
    private DbColumn? _c26;
    public DbColumn C26 => _c26 ??= new DbColumn(this, "c26");
    private DbColumn? _c27;
    public DbColumn C27 => _c27 ??= new DbColumn(this, "c27");
    private DbColumn? _c28;
    public DbColumn C28 => _c28 ??= new DbColumn(this, "c28");
    private DbColumn? _c29;
    public DbColumn C29 => _c29 ??= new DbColumn(this, "c29");
}

internal static class Program
{
    static readonly Wide s_wide = new("w");

    static object Wide30()
    {
        Wide w = new("w");
        SqlStatement sql = Select(w.C0, w.C1, w.C2).From(w).Where(w.C3 == 1).Build();
        return sql.Text;
    }

    static object Wide30Lazy()
    {
        WideLazy w = new("w");
        SqlStatement sql = Select(w.C0, w.C1, w.C2).From(w).Where(w.C3 == 1).Build();
        return sql.Text;
    }

    static object Wide30Static()
    {
        Wide w = s_wide;
        SqlStatement sql = Select(w.C0, w.C1, w.C2).From(w).Where(w.C3 == 1).Build();
        return sql.Text;
    }

    static object Tables()
    {
        Users u = new("u");
        Orders o = new("o");
        return (u, o);
    }

    static object Chain()
    {
        Users u = new("u");
        Orders o = new("o");
        return
            Select(
                u.Id.As("user_id"),
                u.Name.As("user_name"),
                Count(o.Id).As("order_count"))
            .From(u)
            .InnerJoin(o)
            .On(u.Id == o.UserId)
            .Where(
                o.OrderDate >= new DateTime(2024, 1, 1)
                & o.OrderDate < new DateTime(2025, 1, 1))
            .GroupBy(u.Id, u.Name)
            .OrderBy(Count(o.Id).As("order_count").Desc);
    }

    static object Full()
    {
        Users u = new("u");
        Orders o = new("o");
        SqlStatement sql =
            Select(
                u.Id.As("user_id"),
                u.Name.As("user_name"),
                Count(o.Id).As("order_count"))
            .From(u)
            .InnerJoin(o)
            .On(u.Id == o.UserId)
            .Where(
                o.OrderDate >= new DateTime(2024, 1, 1)
                & o.OrderDate < new DateTime(2025, 1, 1))
            .GroupBy(u.Id, u.Name)
            .OrderBy(Count(o.Id).As("order_count").Desc)
            .Build();
        return (sql.Text, sql.Parameters.Count);
    }

    sealed class FakeConnection : System.Data.IDbConnection
    {
        public string ConnectionString { get; set; } = "";
        public int ConnectionTimeout => 0;
        public string Database => "";
        public System.Data.ConnectionState State => System.Data.ConnectionState.Closed;
        public System.Data.IDbTransaction BeginTransaction() => null!;
        public System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il) => null!;
        public void ChangeDatabase(string databaseName) { }
        public void Close() { }
        public System.Data.IDbCommand CreateCommand() => null!;
        public void Dispose() { }
        public void Open() { }
    }

    static readonly FakeConnection s_cnn = new();
    static readonly Users s_u = new("u");
    static readonly int[] s_ids = Enumerable.Range(1, 20).ToArray();
    static readonly DbColumn[] s_cols = Enumerable.Range(0, 10).Select(i => new DbColumn(s_u, "c" + i)).ToArray();

    static object Resolve() => DbmsResolver.Resolve(s_cnn);

    static object Simple()
    {
        Users u = s_u;
        SqlStatement sql = Select(u.Id, u.Name).From(u).Where(u.Id == 1).Build();
        return sql.Text;
    }

    static object SimpleCnn()
    {
        Users u = s_u;
        SqlStatement sql = Select(u.Id, u.Name).From(u).Where(u.Id == 1).Build(s_cnn);
        return sql.Text;
    }

    static object In20()
    {
        Users u = s_u;
        SqlStatement sql = Select(u.Id).From(u).Where(u.Id.In(s_ids)).Build();
        return sql.Text;
    }

    static object Insert10()
    {
        Users u = s_u;
        SqlStatement sql = InsertInto(u, s_cols).Values(1, 2, 3, "a", "b", "c", 4.5, 6L, true, "z").Build();
        return sql.Text;
    }

    static object And10()
    {
        Users u = s_u;
        SqlCondition c = u.Id == 1;
        for (int i = 0; i < 9; i++) c &= s_cols[i] == i;
        SqlStatement sql = Select(u.Id).From(u).Where(c).Build();
        return sql.Text;
    }

    static object Subquery()
    {
        Users u = s_u;
        Orders o = new("o");
        SqlStatement sql = Select(u.Id, u.Name).From(u)
            .Where(Exists(Select(o.Id).From(o).Where(o.UserId == u.Id & o.OrderDate >= new DateTime(2024, 1, 1))))
            .OrderBy(u.Id).Limit(10).Build();
        return sql.Text;
    }

    static readonly object[] s_ids1000 = Enumerable.Range(1, 1000).Select(i => (object)i).ToArray();
    static readonly object?[][] s_rows = Enumerable.Range(0, 100).Select(r => Enumerable.Range(0, 10).Select(c => (object?)(r * 10 + c)).ToArray()).ToArray();

    static object In1000()
    {
        Users u = s_u;
        SqlStatement sql = Select(u.Id).From(u).Where(u.Id.In(s_ids1000)).Build();
        return sql.Text.Length;
    }

    static object Bulk100x10()
    {
        Users u = s_u;
        SqlStatement sql = InsertInto(u, s_cols).Values(s_rows).Build();
        return sql.Text.Length;
    }

    static readonly int[][] s_idsN = new[] { 8, 16, 24, 31, 33, 40, 48, 64, 100, 200 }.Select(n => Enumerable.Range(1, n).ToArray()).ToArray();
    static readonly object[] s_cols20 = Enumerable.Range(0, 20).Select(i => (object)new DbColumn(s_u, "k" + i)).ToArray();

    static Func<object> InN(int n)
    {
        int[] ids = s_idsN.Single(a => a.Length == n);
        return () =>
        {
            Users u = s_u;
            return Select(u.Id).From(u).Where(u.Id.In(ids)).Build().Text;
        };
    }

    static readonly BindValue s_same = Bind(7);
    static readonly object[] s_same300 = Enumerable.Repeat((object)s_same, 300).ToArray();
    static readonly object[] s_same5 = Enumerable.Repeat((object)s_same, 5).ToArray();
    static readonly BindValue[] s_tenant = Enumerable.Range(0, 4).Select(k => Bind("tenant" + k)).ToArray();
    static readonly object?[][] s_sharedRows = Enumerable.Range(0, 1000).Select(i =>
        Enumerable.Range(0, 5).Select(k => k < 4 ? s_tenant[k] : (object?)(i * 10 + k)).ToArray()).ToArray();
    static readonly DbColumn[] s_cols5 = Enumerable.Range(0, 5).Select(i => new DbColumn(s_u, "k" + i)).ToArray();
    static readonly SqlCondition s_reused = s_u.Id.In(Enumerable.Range(0, 100).ToArray());
    static readonly OutputParameter[] s_outs = Enumerable.Range(0, 300).Select(i => new OutputParameter("v" + i, System.Data.DbType.Int32)).ToArray();
    static readonly object[] s_outItems = Enumerable.Range(0, 300).Select(i => (object)s_cols[i % 10]).ToArray();
    static readonly byte[] s_bytes = new byte[4];

    static object InSame300() => Select(s_u.Id).From(s_u).Where(s_u.Id.In(s_same300)).Build().Text;

    static object InSame5() => Select(s_u.Id).From(s_u).Where(s_u.Id.In(s_same5)).Build().Text;

    static object ValuesShared() => InsertInto(s_u, s_cols5).Values(s_sharedRows).Build().Text.Length;

    static object CondTwice() => Select(s_u.Id).From(s_u).Where(s_reused | s_reused).Build().Text;

    static object And129()
    {
        SqlCondition c = s_u.Id == 0;
        for (int i = 1; i < 129; i++) c &= s_u.Id != i;
        return Select(s_u.Id).From(s_u).Where(c).Build().Text;
    }

    static Func<object> AndN(int n) => () =>
    {
        SqlCondition c = s_u.Id == 0;
        for (int i = 1; i < n; i++) c &= s_u.Id != i;
        return Select(s_u.Id).From(s_u).Where(c).Build().Text;
    };

    // n IN lists of k values each: every list asks for a reservation the list already has room for.
    static Func<object> ManyIn(int n, int k) => () =>
    {
        int next = 0;
        SqlCondition? c = null;
        for (int i = 0; i < n; i++)
        {
            object[] v = new object[k];
            for (int j = 0; j < k; j++) v[j] = next++;
            SqlCondition one = s_u.Id.In(v);
            c = c is null ? one : c & one;
        }
        return Select(s_u.Id).From(s_u).Where(c!).Build().Text;
    };

    // The same shape over one condition tree made beforehand, so the chain and Build() are timed.
    static Func<object> ManyInBuild(int n, int k)
    {
        int next = 0;
        SqlCondition? c = null;
        for (int i = 0; i < n; i++)
        {
            object[] v = new object[k];
            for (int j = 0; j < k; j++) v[j] = next++;
            SqlCondition one = s_u.Id.In(v);
            c = c is null ? one : c & one;
        }
        SqlCondition all = c!;
        return () => Select(s_u.Id).From(s_u).Where(all).Build().Text;
    }

    // n pairs of "x = i AND y IN (i)": a bind added alone between every two short lists.
    static Func<object> EqIn(int n)
    {
        SqlCondition? c = null;
        for (int i = 0; i < n; i++)
        {
            SqlCondition one = s_u.Id == i & s_u.Id.In(new object[] { i + 100_000 });
            c = c is null ? one : c & one;
        }
        SqlCondition all = c!;
        return () => Select(s_u.Id).From(s_u).Where(all).Build().Text;
    }

    // 64 binds added one at a time, then a list of 64: the reservation takes the list to 128.
    static object InAfter64()
    {
        SqlCondition c = s_u.Id != -1;
        for (int i = 1; i < 64; i++) c &= s_u.Id != -1 - i;
        object[] v = new object[64];
        for (int i = 0; i < 64; i++) v[i] = i;
        return Select(s_u.Id).From(s_u).Where(c & s_u.Id.In(v)).Build().Text;
    }

    static Func<object> HeldN(int n) => () =>
    {
        object[] v = new object[n];
        for (int i = 0; i < n; i++) v[i] = Bind(i);
        return Select(s_u.Id).From(s_u).Where(s_u.Id.In(v)).Build().Text;
    };

    static Func<object> HeldThenDirect(int held, int direct) => () =>
    {
        SqlCondition c = s_u.Id == Bind(-1);
        for (int i = 1; i < held; i++) c |= s_u.Id == Bind(i);
        return Select(s_u.Id).From(s_u).Where(c & s_u.Id.In(Enumerable.Range(0, direct).ToArray())).Build().Text;
    };

    static object RetInto300() =>
        Update(s_u).Set(s_cols[0] == 1).Returning(s_outItems).Into(s_outs).Build(Dbms.Oracle).Text.Length;

    static object BindBytes() => new BindValue(s_bytes);

    static object BindInt() => new BindValue(5);

    static object BindNullValue() => BindNull();

    static object In20Cols()
    {
        Users u = s_u;
        SqlStatement sql = Select(u.Id).From(u).Where(u.Id.In(s_cols20)).Build();
        return sql.Text;
    }

    static object InsertNull10()
    {
        Users u = s_u;
        SqlStatement sql = InsertInto(u, s_cols).Values(Null, Null, Null, Null, Null, Null, Null, Null, Null, Null).Build();
        return sql.Text;
    }

    static Func<object> FullOn(Dbms dbms) => () =>
    {
        Users u = new("u");
        Orders o = new("o");
        SqlStatement sql =
            Select(
                u.Id.As("user_id"),
                u.Name.As("user_name"),
                Count(o.Id).As("order_count"))
            .From(u)
            .InnerJoin(o)
            .On(u.Id == o.UserId)
            .Where(
                o.OrderDate >= new DateTime(2024, 1, 1)
                & o.OrderDate < new DateTime(2025, 1, 1))
            .GroupBy(u.Id, u.Name)
            .OrderBy(Count(o.Id).As("order_count").Desc)
            .Build(dbms);
        return sql.Text;
    };

    static void Stress()
    {
        // Many threads, statements of different sizes and dialects: every output must equal the
        // one the same thread-free build gives.
        Func<int, string> make = i =>
        {
            Users u = new("u" + (i % 7));
            Orders o = new("o");
            int n = 1 + (i * 37 % 150);
            BindValue shared = Bind(i);
            object[] items = Enumerable.Range(0, n).Select(k => k % 5 == 0 ? (object)shared : k + i).ToArray();
            Dbms d = (Dbms)(1 + i % 5);
            SqlStatement s = Select(u.Id, shared).From(u).InnerJoin(o).On(u.Id == o.UserId)
                .Where(u.Id.In(items) & o.UserId == shared & Exists(Select(o.Id).From(o).Where(o.Id == i)))
                .OrderBy(u.Id).Build(d);
            long sum = 0; int k2 = 0;
            s.Parameters.ForEach((name, v) => sum += (int)v.Value * ++k2 + name.Length);
            return s.Text + "|" + s.Parameters.Count + "|" + sum;
        };
        const int N = 20000;
        string[] expected = new string[N];
        for (int i = 0; i < N; i++) expected[i] = make(i);
        int bad = 0;
        for (int round = 0; round < 3; round++)
        {
            Parallel.For(0, N, new ParallelOptions { MaxDegreeOfParallelism = 16 }, i =>
            {
                if (make(i) != expected[i]) Interlocked.Increment(ref bad);
            });
        }
        string digest = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(string.Concat(expected))));
        Console.WriteLine($"stress: {N} statements x 3 parallel rounds, mismatches {bad}, digest {digest}");
    }

    // Drives the internal buffer directly: random runs of new binds, repeats of earlier binds,
    // output parameters and (where the build has it) reservations that over- or under-count.
    static void Fuzz(int sequences, int maxOps)
    {
        const System.Reflection.BindingFlags F = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
        var asm = typeof(SqlStatement).Assembly;
        Type bufT = asm.GetType("SqlArtisan.Internal.SqlBuildingBuffer")!;
        var add = bufT.GetMethod("AddParameter", F)!;
        var addOut = bufT.GetMethod("AddOutParameter", F)!;
        var toStmt = bufT.GetMethod("ToSqlStatement", F)!;
        var reserve = bufT.GetMethod("ReserveParameters", F);
        Type? implT = asm.GetType("SqlArtisan.Internal.ImplicitBindValue");
        Func<object, BindValue> make = implT is null
            ? v => new BindValue(v)
            : v => (BindValue)Activator.CreateInstance(implT, F, null, new object[] { v }, null)!;
        using var sha = System.Security.Cryptography.IncrementalHash.CreateHash(System.Security.Cryptography.HashAlgorithmName.SHA256);
        long ops = 0, maxParams = 0;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int seq = 0; seq < sequences; seq++)
        {
            var rnd = new Random(seq);
            Dbms dbms = new[] { Dbms.MySql, Dbms.Oracle, Dbms.PostgreSql, Dbms.Sqlite, Dbms.SqlServer }[seq % 5];
            object buf = Activator.CreateInstance(bufT, F, null, new object[] { dbms }, null)!;
            var bound = new List<BindValue>();
            int n = rnd.Next(1, maxOps), outs = 0;
            // Bias the run toward one kind of step so long runs of each occur.
            int pNew = rnd.Next(1, 10), pRep = rnd.Next(0, 6), pOut = rnd.Next(0, 6), pRes = rnd.Next(0, 3);
            int total = pNew + pRep + pOut + pRes;
            int k = 0, run = 0;
            for (int i = 0; i < n; i++)
            {
                // Steps come in runs of one kind, some of them long: hundreds of output
                // parameters between two binds is the shape that filled the prototype's table.
                if (run-- <= 0)
                {
                    k = rnd.Next(total);
                    run = rnd.Next(4) == 0 ? rnd.Next(1, 700) : rnd.Next(1, 8);
                }
                ops++;
                if (k < pNew)
                {
                    BindValue b = rnd.Next(2) == 0 ? make(i) : new BindValue(i);
                    bound.Add(b);
                    add.Invoke(buf, new object[] { b });
                }
                else if (k < pNew + pRep)
                {
                    if (bound.Count > 0) add.Invoke(buf, new object[] { bound[rnd.Next(bound.Count)] });
                }
                else if (k < pNew + pRep + pOut)
                {
                    addOut.Invoke(buf, new object[] { new OutputParameter("o" + outs++, System.Data.DbType.Int32) });
                }
                else
                {
                    // A reservation for binds that may or may not follow, first bound or not.
                    int claimed = rnd.Next(0, 300);
                    bool reuse = rnd.Next(4) == 0 && bound.Count > 0;
                    BindValue first = reuse ? bound[rnd.Next(bound.Count)] : make(-i);
                    if (reserve is not null && implT is not null && implT.IsInstanceOfType(first))
                        reserve.Invoke(buf, new object?[] { first, claimed });
                    int arrive = rnd.Next(4) == 0 ? 0 : rnd.Next(0, claimed + 1);
                    for (int j = 0; j < arrive; j++)
                    {
                        BindValue b = j == 0 ? first : make(j);
                        if (j > 0 || !reuse) bound.Add(b);
                        add.Invoke(buf, new object[] { b });
                    }
                }
            }
            var st = (SqlStatement)toStmt.Invoke(buf, null)!;
            ((IDisposable)buf).Dispose();
            maxParams = Math.Max(maxParams, st.Parameters.Count);
            sha.AppendData(System.Text.Encoding.UTF8.GetBytes(st.Text));
            st.Parameters.ForEach((name, v) => sha.AppendData(System.Text.Encoding.UTF8.GetBytes(name + "=" + v.Value + ";")));
        }
        Console.WriteLine($"fuzz: {sequences} sequences, {ops} steps, largest statement {maxParams} parameters, {watch.Elapsed.TotalSeconds:F1} s, digest {Convert.ToHexString(sha.GetHashAndReset())}");
    }

    static (double ns, long bytes) Measure(Func<object> f, int n, int rounds, int warm = 200_000)
    {
        for (int i = 0; i < warm; i++) GC.KeepAlive(f());
        Thread.Sleep(200); // let tier-1 land
        for (int i = 0; i < warm; i++) GC.KeepAlive(f());
        long b = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1000; i++) GC.KeepAlive(f());
        long bytes = (GC.GetAllocatedBytesForCurrentThread() - b) / 1000;
        double best = double.MaxValue;
        for (int r = 0; r < rounds; r++)
        {
            long t = Stopwatch.GetTimestamp();
            for (int i = 0; i < n; i++) GC.KeepAlive(f());
            double ns = Stopwatch.GetElapsedTime(t).TotalNanoseconds / n;
            if (ns < best) best = ns;
        }
        return (best, bytes);
    }

    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "probe")
        {
            // The same BindValue instance must reuse its marker, below and above the hash threshold.
            Users pu = s_u;
            BindValue[] held = Enumerable.Range(0, 40).Select(i => Bind(i)).ToArray();
            object[] items = Enumerable.Range(0, 300).Select(i => i % 3 == 0 ? (object)held[i % 40] : i).ToArray();
            SqlStatement p = Select(held[5], held[39]).From(pu).Where(pu.Id.In(items) & pu.Id == held[0] & pu.Name == held[39]).Build();
            Console.WriteLine(p.Text);
            Console.WriteLine(p.Parameters.Count);
            Console.WriteLine(string.Join(",", p.Parameters.ParameterNames));
            int k = 0; long sum = 0;
            p.Parameters.ForEach((n, v) => sum += (int)v.Value * ++k);
            Console.WriteLine(sum);
            return;
        }

        if (args.Length > 0 && args[0] == "outloop")
        {
            // A held UPDATE stage: SET binds, then RETURNING ... INTO, then a WHERE appended after it.
            int binds = int.Parse(args[1]);
            int outs = int.Parse(args[2]);
            Users t = new("u");
            EqualityCondition[] sets = Enumerable.Range(0, binds).Select(i => new DbColumn(t, "x" + i) == i).ToArray();
            var held = Update(t).Set(sets);
            object[] items = Enumerable.Range(0, outs).Select(i => (object)new DbColumn(t, "x" + (i % binds))).ToArray();
            OutputParameter[] vars = Enumerable.Range(0, outs).Select(i => new OutputParameter("v" + i, System.Data.DbType.Int32)).ToArray();
            held.Returning(items).Into(vars);
            try
            {
                SqlStatement st = held.Where(t.Id == 1 & t.Name == "a").Build(Dbms.Oracle);
                Console.WriteLine($"built: {st.Text.Length} chars, {st.Parameters.Count} parameters, tail: {st.Text[^40..]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"threw {ex.GetType().Name}: {ex.Message}");
            }
            return;
        }

        if (args.Length > 0 && args[0] == "mergemsg")
        {
            // A held MERGE branch given UPDATE SET ... WHERE and then DELETE: DELETE has two partners there.
            Users t = new("t");
            Users s = new("s");
            var held = MergeInto(t).Using(s).On(t.Id == s.Id).WhenMatched();
            held.ThenUpdateSet(t.Name == s.Name).Where(t.Id == 1);
            try
            {
                held.ThenDelete().Build(Dbms.Oracle);
                Console.WriteLine("built");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }
            return;
        }

        if (args.Length > 0 && args[0] == "fuzz")
        {
            Fuzz(int.Parse(args[1]), int.Parse(args[2]));
            return;
        }

        if (args.Length > 0 && args[0] == "stress")
        {
            Stress();
            return;
        }

        int rounds = args.Length > 0 ? int.Parse(args[0]) : 15;
        if (args.Length > 1 && args[1].StartsWith("only="))
        {
            var all = new List<(string, Func<object>)>
            {
                ("inSame300", InSame300), ("inSame5", InSame5), ("valuesShared", ValuesShared), ("condTwice", CondTwice),
                ("and129", And129), ("retInto300", RetInto300), ("bindBytes", BindBytes), ("bindInt", BindInt), ("bindNull", BindNullValue),
                ("in20cols", In20Cols), ("insertnull10", InsertNull10), ("simple", Simple), ("in20", In20), ("insert10", Insert10),
                ("full", Full), ("fullOracle", FullOn(Dbms.Oracle)), ("fullMySql", FullOn(Dbms.MySql)),
                ("fullSqlite", FullOn(Dbms.Sqlite)), ("fullSqlServer", FullOn(Dbms.SqlServer)), ("fullPg", FullOn(Dbms.PostgreSql)),
            };
            foreach (int[] a in s_idsN) all.Add(("in" + a.Length, InN(a.Length)));
            foreach (int n in new[] { 100, 120, 127, 128, 129, 132, 136, 140, 145, 150, 160, 180, 200, 256, 257, 300 })
            {
                all.Add(("andN" + n, AndN(n)));
                all.Add(("heldN" + n, HeldN(n)));
            }
            foreach (int n in new[] { 10, 20, 30, 40, 60, 100, 128, 140, 192, 257 })
            {
                all.Add(("manyIn1_" + n, ManyIn(n, 1)));
                all.Add(("manyIn1b_" + n, ManyInBuild(n, 1)));
                all.Add(("manyIn2b_" + n, ManyInBuild(n, 2)));
                all.Add(("manyIn5b_" + n, ManyInBuild(n, 5)));
                all.Add(("manyIn2_" + n, ManyIn(n, 2)));
                all.Add(("manyIn5_" + n, ManyIn(n, 5)));
            }
            foreach (int n in new[] { 20, 40, 64, 80, 100, 128 }) all.Add(("eqIn" + n, EqIn(n)));
            all.Add(("inAfter64", InAfter64));
            all.Add(("htd120_10", HeldThenDirect(120, 10)));
            all.Add(("htd127_3", HeldThenDirect(127, 3)));
            all.Add(("htd60_70", HeldThenDirect(60, 70)));
            all.Add(("htd100_100", HeldThenDirect(100, 100)));
            string[] want = args[1][5..].Split(',');
            foreach (string w in want)
            {
                var (name, f) = all.Single(c => c.Item1 == w);
                bool heavy = w.StartsWith("htd") || w.StartsWith("eqIn") || w == "inAfter64" || w.StartsWith("manyIn") || w.StartsWith("andN") || w.StartsWith("heldN") || w is "valuesShared" or "retInto300" or "and129" or "inSame300" or "condTwice";
                var (ns, bytes) = heavy ? Measure(f, 2000, rounds, 3000) : Measure(f, 200_000, rounds, 100_000);
                Console.WriteLine($"{name,-13} {ns,8:F1} ns  {bytes,6} B");
            }
            return;
        }

        Console.WriteLine(((ValueTuple<string,int>)Full()).Item1);
        DbmsResolver.RegisterProvider(typeof(FakeConnection).FullName!, Dbms.PostgreSql);
        var cases = new (string, Func<object>)[] { ("wide30", Wide30), ("wide30lazy", Wide30Lazy), ("wide30static", Wide30Static), ("resolve", Resolve), ("simple", Simple), ("simplecnn", SimpleCnn), ("in20", In20), ("insert10", Insert10), ("and10", And10), ("subquery", Subquery), ("tables", Tables), ("chain", Chain), ("full", Full) };
        foreach (var (_, f) in cases) Console.WriteLine(f());
        if (args.Length > 1)
        {
            foreach (var (name, f) in new (string, Func<object>)[] { ("in1000", In1000), ("bulk100x10", Bulk100x10) })
            {
                var (ns, bytes) = Measure(f, 2000, rounds, 3000);
                Console.WriteLine($"{name,-10} {ns,10:F1} ns  {bytes,8} B");
            }
            return;
        }

        foreach (var (name, f) in cases)
        {
            var (ns, bytes) = Measure(f, 300_000, rounds);
            Console.WriteLine($"{name,-10} {ns,8:F1} ns  {bytes,6} B");
        }
    }
}
