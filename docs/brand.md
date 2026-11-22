# SqlArtisan Brand Brief (Draft)

**Status:** Draft, under discussion. Not a reference page: it is not linked from
the reference home or `llms.txt`, and nothing here is a promise to users until
it reaches the surfaces listed under [Touchpoints](#touchpoints).

This page is the single source the name, the words and the visuals are derived
from. A visual or verbal choice is settled only when it can be traced back to a
line here; one that cannot is taste, and taste is what this page replaces.

## Contents

- [Audience](#audience)
- [Positioning](#positioning)
- [Promise and pillars](#promise-and-pillars)
- [Personality](#personality)
- [Voice](#voice)
- [Name and tagline](#name-and-tagline)
- [Visual direction](#visual-direction)
- [Touchpoints](#touchpoints)
- [Open questions](#open-questions)

---

## Audience

In priority order. The order decides which message leads when a surface has
room for one.

1. **C# developers who know their database and want to write its SQL
   themselves.** They have hit the ORM's ceiling, or never wanted an ORM, and
   are concatenating SQL strings and hand-writing `DbParameter`s today. This is
   who the library was designed for.
2. **Teams whose SQL is written alongside AI coding assistants.** They need the
   assistant's plausible-but-wrong SQL caught before it reaches a database —
   the deterministic guard rail is the mission, so this audience is the growth
   story.
3. **Oracle shops.** Japanese enterprise systems in particular — an
   unmeasured hypothesis. Array bind (one round trip per batch) and live
   verification on two Oracle versions are the hook.

**Not the audience**, stated so no message courts them: teams that need one
query to run unchanged on several engines, want change tracking and
migrations, or do not want to write SQL. The comparison guide's
*When NOT to Choose SqlArtisan* names the better fit for each.

## Positioning

> For C# developers who know their database and want to write its SQL
> themselves, SqlArtisan is the query builder that emits exactly the SQL you
> wrote — and checks it before it runs. Unlike ORMs and portability-focused
> builders, it never rewrites your query: faithful SQL is its foundation and
> portability a deliberate non-goal, and the type system, the analyzer,
> exact-SQL tests and a live-engine matrix are the proof that the checks hold.

The category is **query builder**, not ORM and not data-access layer. The
difference claimed is **fidelity plus verification**; either alone exists
elsewhere.

## Promise and pillars

**Promise: the SQL you write is the SQL that runs — and it is checked before
it does.**

Three pillars carry the promise. Each has reasons to believe that are facts
about the product, not adjectives, and a message that claims a pillar cites
one of them.

| Pillar | Means | Reasons to believe |
|---|---|---|
| **Faithful** | The output is the SQL you wrote; nothing is rewritten, nothing hidden. | Exact-SQL tests across the suite; no dialect compiler; DBMS differences handled only where the syntax itself differs. |
| **Verified** | Mistakes fail at build time, deterministically, not in production. | Compile-checked columns and statement shapes; the Roslyn analyzer's dialect, version and schema rules; analyzer entries run against live engines. |
| **Lean** | Building a statement costs little. | Allocation-light build path; published benchmarks against other builders. |

## Personality

If SqlArtisan were a person, it would be a craftsman: one who shows you the
material, tells you plainly what the tool will and will not do, and takes
pride in exact work. In archetype terms it is the **Creator** (the maker,
the craftsman), not the Magician (transformation you cannot see) or the
Everyman (one size for all).

| It is | It is not |
|---|---|
| **Precise** — exact, measured, specific | pedantic or fussy |
| **Honest** — says what it does not do, and why | apologetic, or blunt for show |
| **Crafted** — deliberate, finished, made with care | ornate or decorative |

The tension between *precise* (cool, technical) and *crafted* (warm, human) is
the brand's character; the visuals carry both.

## Voice

- **Plain and declarative.** State what happens: "`Build()` emits …", not
  "SqlArtisan empowers you to …".
- **Show the SQL.** A claim about output comes with the output.
- **Measured claims only.** No superlative without a measurement behind it;
  "fast" points at the benchmarks.
- **No magic words.** Avoid *magic*, *effortless*, *seamless*, *simply*,
  *powerful* — they promise what a faithful tool deliberately does not do:
  hide the work.
- **Name the limits.** An honest "not for you if …" earns the rest of the
  page its credibility.

The terminology and punctuation rules in `.claude/rules/docs-style.md` apply
unchanged; this section adds the register, not the spelling.

## Name and tagline

**Name.** *SqlArtisan*: SQL, crafted by hand with care. Written as one word in
PascalCase — never *SQL Artisan*, *SQLArtisan* or *Sqlartisan* — matching the
package ID and the namespace.

**Tagline.** *Write SQL, in C#.* It names the act (writing SQL), the medium
(C#) and, by what it leaves out, the non-goal (no translation layer).

**Supporting lines**, one per pillar:

- Faithful — *The SQL you write is the SQL that runs.*
- Verified — *A deterministic guard rail for AI-assisted SQL.*
- Lean — *Allocation-light, fast, and automatically parameterized.*

## Visual direction

Derived from the pillars and personality above; each rule names its source.

- **Concept: a craftsman's tool meeting SQL.** The mark shows *Artisan* (a
  hand tool — chisel, pen nib) acting on *SQL* (the letters, or a code
  token). A database cylinder alone is generic and says neither half.
  *(Name, Personality: crafted)*
- **Few, exact shapes.** Clean geometry, crisp edges, no 3D, gloss or texture
  that hides the form. *(Personality: precise; Pillar: faithful — nothing
  hidden)*
- **A cool base with one warm accent.** A deep technical base (ink navy or
  deep teal) for precision, and a single warm material accent (amber, copper,
  wood) for craft. One accent, not a palette. *(Personality: precise ×
  crafted)*
- **Legible small.** Readable at 32 px (IDE package lists) and recognizable at
  64 px (nuget.org search), on light and dark backgrounds alike. A detail
  that disappears at 32 px is cut. *(Touchpoints)*
- **Type.** A geometric sans for the wordmark; monospace only where code is
  shown.
- **Avoid.** The look of other products' marks, the .NET brand's own colors
  as the primary color, and stock database clip art.

## Touchpoints

Where the brand is applied, and the gaps found while drafting this page.

| Surface | Carries | Current gap |
|---|---|---|
| GitHub repository | Description, topics, homepage, social preview | Topics empty; homepage empty; social preview unverified |
| NuGet packages (4) | Icon, description, tags | No icon on any package; tags shared across three packages; `SqlArtisan.Dapper`'s description says *effortless* (see [Voice](#voice)) |
| README | Tagline, promise, pillars | — |
| docs/ | Voice | — |
| Articles and announcements | Positioning, audience order | None yet; 1.0.0 is the planned launch point |

NuGet metadata ships only with a new package version, and any change under
`src/` requires a new release candidate before 1.0.0.

## Open questions

1. **Audience order.** Is the AI-assistant audience second (growth story) or
   first (mission)? It decides whether the README and the launch lead with
   *Write SQL, in C#* or with the guard rail.
2. **Is *Lean* a pillar?** Performance is real and measured, but it is a
   reason to stay more than a reason to come; it could move to a supporting
   line under *Faithful*.
3. **Oracle as a named audience.** Naming it sharpens the Japanese-market
   story; it may also read as narrowing a five-engine library.
4. **The personality adjectives.** *Precise, honest, crafted* fixes the visual
   direction above; a different three changes the icon.
