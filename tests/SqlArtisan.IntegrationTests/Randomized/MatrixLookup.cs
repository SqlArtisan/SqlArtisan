using SqlArtisan.Analyzers;

namespace SqlArtisan.IntegrationTests.Randomized;

/// <summary>A verdict a model level depends on: a <see cref="DialectMatrix"/> entry, a DML
/// shape <see cref="DialectUsageAnalyzer"/> reports for the dialects that reject it, or a
/// <c>Build()</c> guard (<see cref="GuardKey"/>).</summary>
internal abstract record LevelKey
{
    public static implicit operator LevelKey(string member) => new MemberKey(member, null);

    public static implicit operator LevelKey((string Member, int Arity) key) =>
        new MemberKey(key.Member, key.Arity);

    public static implicit operator LevelKey(ContextRules.DmlShape shape) => new ShapeKey(shape);
}

/// <summary>A matrix entry by member name, at <paramref name="Arity"/> where same-named
/// entries differ by arity (<c>Into</c>, <c>("Into", 2)</c>).</summary>
internal sealed record MemberKey(string Member, int? Arity) : LevelKey
{
    public override string ToString() => Arity is null ? Member : $"{Member}/{Arity}";
}

internal sealed record ShapeKey(ContextRules.DmlShape Shape) : LevelKey
{
    public override string ToString() => Shape.ToString();
}

/// <summary>A dialect-scoped <c>Build()</c> guard (ADR 0011) the analyzer does not report, named
/// by <paramref name="Guard"/> and proved by its <paramref name="Probes"/>: each must throw
/// <paramref name="Message"/> on exactly <paramref name="RejectedOn"/> (PictModelTests).</summary>
internal sealed record GuardKey(
    string Guard,
    Dbms[] RejectedOn,
    string Message,
    Func<ISqlBuilder>[] Probes) : LevelKey
{
    public override string ToString() => Guard;
}

/// <summary>Reads the analyzer's verdicts for one engine at the baseline of its oldest
/// integration lane, the version every lane of that engine is at least, or for a row of the
/// lane above it at that lane's version (<see cref="NewerLane"/>).</summary>
internal static class MatrixLookup
{
    /// <summary>The version of each engine's lane above its baseline, as its fixture's image
    /// pins it: Oracle23aiFixture, PostgreSql17Fixture and SqlServer2025Fixture.</summary>
    public static readonly IReadOnlyDictionary<Dbms, EngineVersion> NewerLane =
        new Dictionary<Dbms, EngineVersion>
        {
            [Dbms.Oracle] = DialectMatrix.V("23"),
            [Dbms.PostgreSql] = DialectMatrix.V("17"),
            [Dbms.SqlServer] = DialectMatrix.V("2025"),
        };

    /// <summary>The <c>Dbms</c> level a DML model names a lane above its engine's baseline by,
    /// so PICT covers that lane's pairs apart from the baseline lane's.</summary>
    public static readonly IReadOnlyDictionary<Dbms, string> NewerLaneName =
        new Dictionary<Dbms, string>
        {
            [Dbms.Oracle] = "Oracle23ai",
            [Dbms.PostgreSql] = "PostgreSql17",
            [Dbms.SqlServer] = "SqlServer2025",
        };

    /// <summary>The engine and lane a DML model's <c>Dbms</c> level names.</summary>
    public static (Dbms Dbms, bool Newer) ParseLane(string level)
    {
        foreach ((Dbms dbms, string name) in NewerLaneName)
        {
            if (name == level)
            {
                return (dbms, true);
            }
        }

        return (CaseRow.Level<Dbms>(level), false);
    }

    public static bool Supports(Dbms dbms, LevelKey key, bool newer = false) =>
        Verdict(dbms, key, newer) is null;

    public static bool SupportsAll(Dbms dbms, IEnumerable<LevelKey> keys, bool newer = false) =>
        keys.All(key => Supports(dbms, key, newer));

    /// <summary>Why <paramref name="key"/> does not run on <paramref name="dbms"/>, or
    /// <see langword="null"/> when it does.</summary>
    public static string? Verdict(Dbms dbms, LevelKey key, bool newer = false) => key switch
    {
        MemberKey member => MemberVerdict(ToTarget(dbms), dbms, member, At(dbms, newer)),
        ShapeKey shape => ShapeVerdict(ToTarget(dbms), dbms, shape.Shape, At(dbms, newer)),
        GuardKey guard => guard.RejectedOn.Contains(dbms)
            ? $"Build() rejects it on {dbms} ({guard.Guard})"
            : null,
        _ => throw new ArgumentOutOfRangeException(nameof(key), key, message: null),
    };

    // The version a verdict reads, and how its message names it.
    private readonly record struct LaneVersion(EngineVersion Version, string Name);

    private static LaneVersion At(Dbms dbms, bool newer)
    {
        EngineVersion version = newer
            ? NewerLane[dbms]
            : DialectMatrix.BaselineVersion[ToTarget(dbms)];
        return new(version, $"the {version} {(newer ? "lane" : "baseline")}");
    }

    // An arity with no row of its own falls back to the member's row, as the analyzer reads it.
    private static string? MemberVerdict(
        TargetDbms target,
        Dbms dbms,
        MemberKey key,
        LaneVersion at)
    {
        if (!DialectMatrix.TryGetEntry(
            key.Member, key.Arity, out DbmsSupport support, out bool wasArityMatch))
        {
            return $"{key} has no DialectMatrix entry";
        }

        // As DialectSupportResolver.Evaluate reads a declared version: a floor decides alone,
        // so a cell the baseline leaves false opens at the lane above it.
        MatrixKey matched = new(key.Member, wasArityMatch ? key.Arity : null);
        bool bounded = DialectMatrix.TryGetMinVersion(matched, target, out EngineVersion min);
        if (bounded && at.Version.CompareTo(min) >= 0)
        {
            return null;
        }

        return bounded ? $"{key} needs {dbms} {min}, above {at.Name}"
            : support.IsSupported(target) ? null
            : $"DialectMatrix marks {key} unsupported on {dbms}";
    }

    // Past its floor the analyzer still reports a DELETE lead that re-lists its target, the
    // shape this key names, so only the lead that does not re-list it passes there.
    private static string? ShapeVerdict(
        TargetDbms target,
        Dbms dbms,
        ContextRules.DmlShape shape,
        LaneVersion at)
    {
        if (shape == ContextRules.DmlShape.None)
        {
            throw new ArgumentOutOfRangeException(nameof(shape), shape, message: null);
        }

        if (!DialectUsageAnalyzer.RejectingDialects(shape).Contains(target))
        {
            return null;
        }

        foreach ((ContextRules.DmlShape Shape, TargetDbms Dbms, EngineVersion From) floor
            in DialectUsageAnalyzer.DmlShapeFloors)
        {
            if (floor.Shape == shape && floor.Dbms == target)
            {
                if (at.Version.CompareTo(floor.From) < 0)
                {
                    return $"{shape} needs {dbms} {floor.From}, above {at.Name}";
                }

                return shape == ContextRules.DmlShape.JoinedDeleteLead
                    ? $"DialectUsageAnalyzer still reports a re-listed {shape} at {at.Name}"
                    : null;
            }
        }

        return $"DialectUsageAnalyzer rejects {shape} on {dbms}";
    }

    private static TargetDbms ToTarget(Dbms dbms) => dbms switch
    {
        Dbms.MySql => TargetDbms.MySql,
        Dbms.Oracle => TargetDbms.Oracle,
        Dbms.PostgreSql => TargetDbms.PostgreSql,
        Dbms.Sqlite => TargetDbms.Sqlite,
        Dbms.SqlServer => TargetDbms.SqlServer,
        _ => throw new ArgumentOutOfRangeException(nameof(dbms), dbms, message: null),
    };
}
