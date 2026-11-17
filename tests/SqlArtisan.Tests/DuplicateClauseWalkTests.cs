using System.Collections;
using System.Reflection;

namespace SqlArtisan.Tests;

// The duplicate-clause walk reads each part's role and kinds from a table keyed by exact
// type, where it used to test `is`; a clause missing from the table passes the walk unread.
public class DuplicateClauseWalkTests
{
    private static readonly Assembly Core = typeof(SqlPart).Assembly;

    private static readonly Dictionary<Type, string> RoleByType = ReadRoles();

    [Fact]
    public void TraitsByType_DerivedClause_IsListedWithItsBaseRole()
    {
        string[] missing = [.. RoleByType
            .SelectMany(listed => ConcreteParts()
                .Where(t => t != listed.Key && listed.Key.IsAssignableFrom(t))
                .Where(t => RoleByType.GetValueOrDefault(t) != listed.Value)
                .Select(t => $"{t.Name} (derives {listed.Key.Name})"))];

        Assert.True(missing.Length == 0, string.Join(", ", missing));
    }

    [Theory]
    [InlineData("ISelectItemsClause", "SelectItems", "SelectItems")]
    [InlineData("IJoinedRelation", "ConditionedJoin", "ConditionFreeJoin")]
    public void TraitsByType_ClauseBehindARoleInterface_IsListed(
        string interfaceName,
        string role,
        string otherRole)
    {
        Type marker = Core.GetTypes().Single(t => t.IsInterface && t.Name == interfaceName);
        Type[] implementers = [.. ConcreteParts().Where(marker.IsAssignableFrom)];

        Assert.NotEmpty(implementers);

        string[] unlisted = [.. implementers
            .Where(t => RoleByType.GetValueOrDefault(t) is not { } listed
                || (listed != role && listed != otherRole))
            .Select(t => t.Name)];

        Assert.True(unlisted.Length == 0, string.Join(", ", unlisted));
    }

    private static IEnumerable<Type> ConcreteParts() =>
        Core.GetTypes().Where(t => typeof(SqlPart).IsAssignableFrom(t) && !t.IsAbstract);

    private static Dictionary<Type, string> ReadRoles()
    {
        const BindingFlags NonPublic =
            BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        object table = Core.GetType("SqlArtisan.Internal.SqlBuilderBase", throwOnError: true)!
            .GetField("TraitsByType", NonPublic)!
            .GetValue(null)!;

        Dictionary<Type, string> roles = [];
        foreach (object entry in (IEnumerable)table)
        {
            Type type = (Type)entry.GetType().GetProperty("Key")!.GetValue(entry)!;
            object traits = entry.GetType().GetProperty("Value")!.GetValue(entry)!;
            roles[type] = traits.GetType().GetProperty("Role", NonPublic)!.GetValue(traits)!
                .ToString()!;
        }

        return roles;
    }
}
