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
#pragma warning disable CS8767
        public string ConnectionString { get; set; } = "";
#pragma warning restore CS8767
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

        int rounds = args.Length > 0 ? int.Parse(args[0]) : 15;
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
