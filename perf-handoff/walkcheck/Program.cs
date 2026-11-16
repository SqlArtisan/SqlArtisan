using System.Reflection;
using System.Runtime.CompilerServices;
using SqlArtisan;

Assembly asm = typeof(SqlPart).Assembly;
Type T(string n) => asm.GetTypes().Single(t => t.Name == n && !t.IsNested);
Type sbb = asm.GetType("SqlArtisan.Internal.SqlBuilderBase")!;
(string Name, Type[] Types)[] Table(string f) =>
    ((string, Type[])[])sbb.GetField(f, BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
var block = Table("OncePerBlockClauses");
var branch = Table("OncePerBranchClauses");

// The original method's `is` lists, in the order its ifs read them.
Dictionary<int, string[]> roles = new()
{
    [1] = ["OnClause", "JoinUsingClause"],
    [2] = ["UnionOperator", "ExceptOperator", "IntersectOperator", "MinusOperator"],
    [3] = ["ISelectItemsClause"],
    [4] = ["WhenMatchedClause", "WhenNotMatchedClause", "WhenNotMatchedBySourceClause"],
    [5] = ["InsertIntoClause", "InsertIgnoreIntoClause"],
    [6] = ["InsertValuesClause", "InsertSetClause"],
    [7] = ["InnerJoinClause", "LeftJoinClause", "RightJoinClause", "FullJoinClause", "JoinLateralClause"],
    [8] = ["LeftJoinLateralClause", "CrossJoinLateralClause", "CrossJoinClause", "NaturalJoinClause",
           "NaturalLeftJoinClause", "NaturalRightJoinClause", "NaturalFullJoinClause", "CrossApplyClause",
           "OuterApplyClause"],
};
var roleTypes = roles.ToDictionary(kv => kv.Key, kv => kv.Value.Select(T).ToArray());
PropertyInfo walk = typeof(SqlPart).GetProperty("WalkCode", BindingFlags.NonPublic | BindingFlags.Instance)!;

int checkedTypes = 0, nonZero = 0, bad = 0;
foreach (Type t in asm.GetTypes().Where(t => typeof(SqlPart).IsAssignableFrom(t) && !t.IsAbstract))
{
    if (t.ContainsGenericParameters) { Console.WriteLine($"GENERIC (skipped): {t}"); continue; }
    int[] matched = roleTypes.Where(kv => kv.Value.Any(r => r.IsAssignableFrom(t))).Select(kv => kv.Key).ToArray();
    if (matched.Length > 1) { Console.WriteLine($"MULTI-ROLE {t}: {string.Join(",", matched)}"); bad++; }
    int role = matched.Length == 0 ? 0 : matched[0];
    int b = Array.FindIndex(block, e => Array.IndexOf(e.Types, t) >= 0);
    int br = Array.FindIndex(branch, e => Array.IndexOf(e.Types, t) >= 0);
    int expected = role | ((b + 1) << 8) | ((br + 1) << 16);
    int actual = (int)walk.GetValue(RuntimeHelpers.GetUninitializedObject(t))!;
    checkedTypes++;
    if (actual != 0) nonZero++;
    if (actual != expected) { bad++; Console.WriteLine($"MISMATCH {t}: expected {expected:X} actual {actual:X}"); }
}

// A type listed under two kinds of one table, or in both tables.
foreach (var tab in new[] { block, branch })
    foreach (var g in tab.SelectMany((e, i) => e.Types.Select(x => (x, i))).GroupBy(p => p.x).Where(g => g.Count() > 1))
    { bad++; Console.WriteLine($"DUP IN TABLE {g.Key}"); }
Console.WriteLine($"both tables: {string.Join(",", block.SelectMany(e => e.Types).Intersect(branch.SelectMany(e => e.Types)).Select(x => x.Name))}");
Console.WriteLine($"non-sealed table types: {string.Join(",", block.Concat(branch).SelectMany(e => e.Types).Where(x => !x.IsSealed).Select(x => x.Name))}");
Console.WriteLine($"concrete SqlPart types checked: {checkedTypes}, with a non-zero code: {nonZero}, problems: {bad}");
