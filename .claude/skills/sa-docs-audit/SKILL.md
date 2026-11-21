---
name: sa-docs-audit
description: Audit SqlArtisan's user-facing documentation corpus (README, docs/, llms.txt, CHANGELOG) for structure, completeness, accuracy, consistency, links, persuasiveness, conciseness, example integrity, whether a reader can finish from the page, placement, and release readiness, each page held to its page type's bar. Corpus-wide by design, not diff-scoped — its bundled scripts sweep every page regardless of what changed, so it catches the coverage and link defects a diff review structurally cannot. Use when asked to review/audit the docs, check the README, validate a docs change before pushing, or confirm docs match the code; for docs changes inside a branch diff, sa-diff-review already covers them. Bundles scripts that verify links, API coverage, terminology, and emitted SQL empirically. Ends with an adversarial verification pass (one independent subagent, not skippable) that attempts to refute the docs' factual claims against primary sources.
---

# Audit the SqlArtisan documentation corpus

This audits the **docs**, not the code (use `sa-diff-review` / `code-review` for
code). The highest-value moves are **empirical and bidirectional**: verify the
emitted SQL by running the real builder (never from memory), and check coverage
both ways (every public API documented; every documented API real).

Surface map: **README** = landing + capability-map index (shipped in the NuGet
package, rendered on nuget.org); **`docs/`** = reference (`README.md` home +
`query-statements.md` / `expressions.md` / `functions.md`); **`llms.txt`** = AI
index. Conventions live in `.claude/rules/docs-style.md`.

## Run the bundled checks first

From the repo root (`dotnet` SDK required for the last one):

```bash
S=.claude/skills/sa-docs-audit/scripts
python3 $S/check_links.py          # anchor/link integrity across README + docs + llms.txt
python3 $S/check_api_coverage.py   # deficiency (undocumented API) + surplus (phantom API)
python3 $S/check_terms.py          # terminology + em-dash + whitespace lint vs docs-style.md
python3 $S/verify_sql_examples.py  # build + run the reference pages' examples; compare emitted SQL
```

Each exits non-zero on findings. They are heuristics — read each hit and judge;
do not blind-fix. `verify_sql_examples.py` embeds the table classes the examples
use; if a new example needs a new table/column, extend `SCAFFOLD` there.

## Page types set the bar

Each page is held to its own type's standard. A candidate that holds only
under another type's bar is dropped, not reported:

| Type | Pages | Examples must |
|---|---|---|
| Landing | `README.md` | model a query someone would write; lead a reader to a first run in as few words as the facts allow |
| Index | `docs/README.md` | carry no example; link every page with its main sections |
| Guide / cookbook | `docs/guides/*`, `docs/cookbook.md`, `docs/comparison.md` | model a query someone would write, and complete the task the page names from the page alone |
| Reference | `docs/query-statements.md`, `docs/expressions.md`, `docs/functions.md`, `docs/analyzer.md` | isolate one construct with minimal operands (`u.Id < 10`); a realistic filter beside it hides what the construct changes |
| Policy | `docs/versioning.md` | carry no example; every rule it states matches the release procedure and the gates |
| ADR | `docs/adr/*` | — ADRs are immutable once Accepted: audit only the index, statuses and links; a decision the code has outgrown is a proposal for a superseding ADR, never an edit |

## Review dimensions

Score the docs against these. The scripts cover 2–5 and parts of 6/10.

1. **Structure / information architecture.** README section order follows the
   evaluation funnel — *discover → use → deepen → meta* (Why → Getting
   Started / Configuration → Design Philosophy / Performance / Documentation →
   Contributing / Changelog / License). Why carries the pitch a feature list
   would repeat, and Performance is evidence Why links to, so a reader reaches
   installation without passing either. README is a lean
   landing + capability map; `docs/` holds the reference. Index/TOC present and
   right-sized; file split neither too coarse nor too fine.

2. **Completeness — no deficiency (code → doc).** Every public `Sql.*` factory,
   builder step, and function is documented. (`check_api_coverage.py`; this is
   how the undocumented `GREATEST`/`LEAST` were caught.)

3. **Completeness — no surplus (doc → code).** No documented API is missing from
   source — no phantom names. Prose mentions and every code block must resolve
   to real APIs. (`check_api_coverage.py` + `verify_sql_examples.py`, which
   compiles the three reference pages' examples — `docs/cookbook.md` is pinned
   separately by the hand-maintained `CookbookTests`, the root README by
   `ReadmeExampleTests`, and the guides' snippets are unswept; compile those by
   hand when they change.)

4. **Accuracy — output matches the code.** Documented emitted SQL equals the
   real `sql.Text`. Run it. (`verify_sql_examples.py`.) Remember `sql.Text` is a
   single line and a single condition has no wrapping parens; the `//` comments
   are line-wrapped for readability — don't claim "verbatim".
   - **Cross-check a table's prose against its own data columns.** A "Why"/notes
     cell that contradicts the row's own Construct/Dialect/Version columns is a
     defect — the Oracle set-operator row claimed plain `INTERSECT` landed in
     21c while its Construct column correctly listed only the `ALL` forms.
   - **Dialect-support claims resolve at the *arity-specific* matrix entry**
     (`[MatrixKey(name, arity)]`), not just the member — check it before
     trusting a per-overload remark (`ToNumber` arity-1 is Oracle-only though
     the member entry is Oracle+PostgreSQL).
   - **Version / vendor-spec facts** ("landed in PostgreSQL 15", "MySQL rejects
     `NOT IN` + `LIMIT`") are verified against the vendor's own docs via a
     WebSearch scoped to the official domain — WebFetch to vendor sites is often
     egress-blocked (403), so WebSearch is the access path, not memory.

5. **Consistency — terminology & formatting.** Conform to
   `.claude/rules/docs-style.md`: *table class* (not "table schema class"),
   *type-safe* (adj) / *type safety* (noun), *bind parameter* (noun) /
   *bind-parameter* (modifier), *UPSERT* in caps, DBMS display names in prose vs
   `Dbms` enum identifiers in code, DBMS listed in enum order, spaced em dash.
   (`check_terms.py`.)

6. **Links & cross-references.** Anchors resolve; README→docs and docs↔docs use
   **absolute** `blob/main` GitHub URLs (nuget.org does not resolve relative
   links); `llms.txt` uses `raw.githubusercontent.com`; in-page anchors stay
   relative. (`check_links.py`.)

7. **Persuasiveness & honesty (landing copy).** The Why hook lands the arc
   *pain → solution → proof → payoff* and states the promise ("the SQL you write
   is the SQL that runs"). Claims are **scoped and backed** — no overclaim
   ("minimal overhead", not "no overhead"; "fastest builder", scoped to
   builders, not "fastest"); performance claims cite the benchmark suite with
   machine + versions (ADR 0006).

8. **Conciseness — no redundancy.** Section volume is proportionate (watch the
   longest, usually Quick Start). No near-duplicate sentences across sections
   (e.g. the Why close vs the Performance lead) — differentiate the angle.
   Before flagging redundancy or an inconsistency, check whether a nearby
   note/column already reconciles it: a version gap an adjacent parenthetical
   explains is not an inconsistency, and an explanatory column that carries the
   SQL-token spelling the code-name column hides (`WithRecursive` →
   `WITH RECURSIVE`) is additive, not a restatement.

9. **AI-consumption readiness.** `llms.txt` exists and its links track the docs;
   each `docs/` page leads with a design-constraints block; examples are
   deterministic C# → emitted-SQL pairs.

10. **Multi-surface rendering & mechanical hygiene.** Renders cleanly on **both**
    GitHub and **nuget.org** (relative links, no auto-TOC on nuget, `<details>`/
    `<sub>` may be stripped). No trailing whitespace, double blank lines, or
    mojibake. (`check_terms.py` covers whitespace/blank lines.)

11. **Precision boundary (ADR 0020).** Every claim has a source of truth in the
    repo. Flag result semantics (duplicate handling, `NULL` matching,
    multiplicity, collation), an equivalence between two constructs, a
    hand-written SQL rewrite, and a version floor outside `docs/analyzer.md`'s
    register — whether or not it currently reads as true, since nothing keeps it
    true. A substitution names a sibling SqlArtisan API or nothing at all; a
    hazard in SqlArtisan's own emitted SQL is in boundary and stays.

12. **Example integrity across a page.** Snippets that share a fixture (a table
    class, a local) agree on it: every member any snippet uses is declared
    where the page declares the fixture — the README's opening example once
    filtered on a `StatusId` its Quick Start class lacked. Two examples that
    are nearly the same are made identical or made visibly different in
    purpose; a stray difference (a dropped filter, an added `ORDER BY`) reads
    as deliberate and sends the reader looking for the reason. Every changed
    example runs as shown on an engine (`docs-style.md`), and meets its page
    type's realism bar (above).

13. **The variant a reader writes next.** For each example, write the
    neighbouring form a reader would reach for — a nullable or empty input, the
    excluded branch of a flag, a second call on the same builder, another
    dialect — and run it. A variant that throws or changes meaning while the
    page is silent about it is an omission: `ConditionIf(x.HasValue, col >= x)`
    throws for a null `x` although the flag excludes it (#859), because C#
    builds the argument first.

14. **The reader can finish from the page.** Every tool or package the page
    tells the reader to use has its install step on the page or one link away;
    an opt-in feature the page advertises shows how to turn it on — the
    analyzer reports nothing until a dialect is configured, so a page that
    sells it without the enabling line sells a feature that stays off; a
    "prerequisite" is something the reader must have before starting, not a
    usage tip.

15. **Placement.** Content sits where its reader looks for it. Flag a
    version-specific operational note on a landing page, an index that mirrors
    another index, and a section whose every item restates another surface.
    Before proposing a cut, list what *only* that content says and give each
    such fact a new home in the proposal. A list that appears on two surfaces
    keeps one order, chosen by how many readers need each item, not alphabet
    (`docs-style.md` § Package order).

16. **Release readiness** (a pre-release audit). Prerelease wording,
    `--prerelease` flags, `nuget/vpre` badges and version strings match the
    release about to ship, and each one the release must change is named by a
    step of CLAUDE.md's release procedure.

## Process safeguards (hard-won)

- **Verify, don't recall.** Emitted SQL, dialect behaviour, and allocations come
  from running the builder, not from memory or the doc's own text.
- **Edit UTF-8-safely.** A `perl -i` substitution corrupted em-dashes here. Use
  Python (`encoding="utf-8"`) or the editing tools; re-scan for mojibake after
  any bulk text rewrite.
- **Lossless moves.** When relocating content, byte-compare the code-fence
  bodies before/after (`git show OLD:file` vs new) so no example is dropped or
  altered; only headings/links should change.
- **Measure, don't estimate.** Word counts, link counts and "words before
  installation" come from the file, counted by a script, and a measurement
  that contradicts an earlier estimate in the conversation corrects it in the
  report.
- **Find the record before proposing a change.** A section, an index or a
  sentence may be there on purpose: a release-procedure step that requires it,
  an issue or ADR that chose it (#340 mirrored the README map), a
  `.claude/rules/` clause, a gate. A proposal that reverses one names it and is
  put as a decision for the author, never as a plain fix. A proposal is also
  checked against the page-type table above before it is made.
- **Land the class, and gate the real text.** A fix covers every surface the
  finding's wording appears on: grep the docs, the rules, the CHANGELOG and
  test comments for the same overclaim ("full index" sat in four places). A
  test that pins an example reads the page's own code, not a copy typed into
  the test, and is mutation-probed: break the page in each way the finding
  implies and confirm the test fails. `ReadmeExampleTests` first passed with a
  stale `// SELECT` comment because it built from a copy.
- **Say what was not verified.** A claim the environment cannot check (an
  external badge's rendering, a vendor page behind a blocked host) is reported
  as unverified, and any wording written for it claims only what was checked.
- **Report by severity; confirm editorial calls.** Group findings
  (broken/inaccurate > inconsistent > stylistic). Apply clear mechanical fixes;
  for wording/ordering/trims that are judgment calls, propose options and
  confirm before applying.

## Adversarial verification (final pass)

The dimensions above confirm; this pass refutes. A neutral pass accepted
"every analyzer entry is executed against a live engine" (#267/#319) — the
adversarial pass read `MatrixSweepCatalog.cs` and found the two excluded
entries. Run this pass — an independent-context refutation catches
overclaims your own drafting pass cannot see past.

Spawn **one** independent subagent (`sa-reviewer`) with an explicitly
adversarial mission — "try to refute this", never "check this is right" —
and, in a release audit, the audit framing: the corpus as it stands, never a
diff, with no "mergeable" verdict in the report.
One subagent covering all findings is the right shape here; do not spawn
several to split the work or to re-verify each other. Prime refutation
targets in docs:

- **Quantifiers** — every / all / always / never / only. One real exception
  turns the sentence into an OVERREACH.
- **Superlatives and rankings** — fastest / lowest-allocation; the benchmark
  suite output is the primary source, not the README's own table.
- **The audit's own proposals** — for each structural or editorial proposal,
  try to show it conflicts with a deliberate decision, a required fact, or the
  page type's bar; a proposal that survives only by ignoring one falls.
- **Counts, version bounds, capability claims** — "verified against live
  engines", "N engines", "SQLite 3.44+"; the primary source is the test
  catalog (e.g. `MatrixSweepCatalog.cs`), the pinned image list, or a live
  probe.
- **The subagent has no web access** (Read/Grep/Glob/Bash), so it checks
  in-repo primary sources but **cannot verify a version / vendor-spec claim**
  against the vendor's docs — keep those with the orchestrator's WebSearch, and
  don't read the subagent's silence on them as confirmation.

Mission rules: every factual claim is traced to a primary source (the code,
a test catalog, an ADR, a live builder/harness run) — never the doc's own
text or memory. Surviving findings carry severity (High/Medium/Low) plus the
evidence that survived refutation (verbatim probe output or the primary
source's `file:line`). Fallen claims are classified **DEFECT** (factually
wrong) / **OVERREACH** (true but misleading) / **INCONSISTENCY** (contradicts
another surface). Tell the subagent that "nothing falls" is a complete
answer and that it reports only what it would change — a refuting pass is
under more pressure than any other to return *something*, which is how
non-actionable "worth knowing" items reach a report. Drop anything it hands
back that you would not act on; do not forward it.

Recursion and fallback: if you *are* the adversarial subagent, skip this
section — no recursion. If the Agent tool is unavailable — or the subagent
errors, times out, or never returns — run the pass yourself as a distinct
final phase, re-deriving each claim from primary sources rather than rereading
your draft. Never report the review as complete with the adversarial pass
silently skipped — a launched-but-non-returning agent is not a pass.

**Detecting a stalled/vanished subagent.** There is no reliable "is it still
running" query, and a background agent can disappear with zero
notification — no completion, no error. Don't sleep-poll for it; but don't
wait on it indefinitely either. If work has stalled on the adversarial pass
with no notification, `TaskStop` on the agent's ID doubles as a status probe:
an error (e.g. "no task found") confirms it already ended without notifying,
and is the cue to run the fallback pass yourself rather than continue waiting.

**Do not add more self-verification than this.** The single independent-context
pass above is the complete *findings*-verification step (separate from the
bundled scripts and empirical SQL checks above, which still apply). If you
are aware you are running as a model with strong built-in self-checking (e.g. Claude
Opus 5 or later), do not additionally re-verify your own findings before or
after spawning the subagent, and do not spawn extra subagents "just to be
sure". If you cannot tell which model you are, run exactly the one pass
described above; that is correct for every model.

## Output

**Report only what you are asking to change.** A finding earns its place by
having a fix worth making. If your own conclusion is "this is fine", "already
covered elsewhere", or "worth knowing but needs no action", it does not go in
the report — not as a caveat, an observation, or a passing mention. **Zero
findings is a normal, good result**, not evidence the review was shallow: say
the docs are clean and stop. Being asked for a review does not oblige you to
return a finding, and padding one out with non-actionable items buries the ones
that need action. The test is one question — **am I asking for a change?** No
means it does not appear.

List concrete fixes with file:line, and state what was verified empirically
(links resolve, N/N examples match, coverage clean) — that evidence is the
report's substance when there are no findings. Give the per-dimension rundown
only where a dimension produced a fix; a dimension that passed needs no
paragraph. Name the DEFECT / OVERREACH / INCONSISTENCY classification of
whatever the adversarial pass knocked down, and cover its survivals in one
line rather than itemising them. Re-run the relevant script after fixes to
confirm green.

Match this report's length to what the findings need: cover every finding at
the detail level above, but do not pad with filler recap sections, restated
summaries, or boilerplate closing paragraphs.
