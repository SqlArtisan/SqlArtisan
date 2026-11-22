# SqlArtisan Brand Brief (Draft)

**Status:** Draft; audience order, pillars and personality agreed. Not a
reference page: it is not linked from the reference home or `llms.txt`, and
nothing here is a promise to users until it reaches the surfaces listed under
[Touchpoints](#touchpoints).

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
- [The mark](#the-mark)
- [Touchpoints](#touchpoints)

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
   verification on two Oracle versions are the hook. Named here and in
   Japanese-language articles, never on the README or a package description:
   there it would read as narrowing a five-engine library.

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

Three traits, one per pillar, so the character and the promise share a
structure: a trait no pillar backs would be a claim without proof.

| It is | It is not | Pillar |
|---|---|---|
| **Honest** — shows the SQL it runs, and says what it does not do | apologetic, or blunt for show | Faithful |
| **Robust** — holds under load and under mistakes; fails at build time, not in production | heavy or rigid | Verified |
| **Nimble** — light to adopt and cheap to run | flimsy or casual | Lean |

The tension between *robust* (solid, load-bearing) and *nimble* (light, quick)
is the brand's character; the visuals carry both. Craft is not a trait here:
the name carries it, so the mark draws its craftsman's tool from the name.

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
  hand tool) acting on *SQL* (a database or a code token, never letters: the
  name sits beside the mark wherever it is shown small). A database cylinder
  alone is generic and says neither half. *(Name)*
- **Few, exact shapes.** Clean geometry, crisp edges, no 3D, gloss or texture
  that hides the form. *(Personality: honest — nothing hidden)*
- **A solid footing with one element in motion.** A heavy, stable mass (thick
  strokes, a squared base) carries robustness; one element set at an angle or
  mid-stroke carries nimbleness. *(Personality: robust × nimble)*
- **A deep base with one bright accent.** A deep, saturated base (ink navy or
  deep teal) for the solid footing, and a single bright warm accent (amber or
  copper) for the moving element. One accent, not a palette.
  *(Personality: robust × nimble)*
- **Legible small.** Readable at 32 px (IDE package lists) and recognizable at
  64 px (nuget.org search), on light and dark backgrounds alike. A detail
  that disappears at 32 px is cut. *(Touchpoints)*
- **Type.** A geometric sans for the wordmark (Inter Display ExtraBold,
  outlined in the logo files); monospace only where code is shown.
- **Avoid.** The look of other products' marks, the .NET brand's own colors
  as the primary color, and stock database clip art.

## The mark

A database cylinder sharpened into a fountain-pen nib, held at 45°, with the
asterisk it has just written ahead of its tip. Each part reads as one idea:

| Part | Reads as |
|---|---|
| The white lid and the two bands | the database |
| The nib's point, slit and breather hole | writing it by hand — the craft |
| The `*` ahead of the tip | SQL (`SELECT *`), the thing written |

It carries no letters, so it has to be learned; until it is, it appears with
the wordmark wherever the name is not already beside it.

| Color | Hex | Used for |
|---|---|---|
| Navy | `#12263F` | The base: background and the nib's cut lines |
| Amber | `#F59E0B` | The tool — the one accent |
| White | `#F8FAFC` | The lid and the ink |
| Muted | `#9FB0C6` | Secondary text on navy |

**Files** (`assets/`):

| File | Use |
|---|---|
| `icon.svg` | The source of every other file |
| `icon.png` | 128 px, packed as every NuGet package's icon |
| `logo.svg`, `logo-dark.svg` | Mark beside the wordmark, for light and dark backgrounds |
| `social-preview.png` | 1280 × 640, the GitHub repository's social preview |

**Rules.** The asterisk stays clear of the tip: touching, it reads as hanging
from the nib, and placed on the nib face it reads as something else entirely.
The 45° angle is the mark's one moving element, so it is not straightened. The
colors are not swapped or recolored, and the mark is not shown below 32 px.

## Touchpoints

Where the brand is applied, and the gaps found while drafting this page.

| Surface | Carries | Current gap |
|---|---|---|
| GitHub repository | Description, topics, homepage, social preview | Topics empty; homepage empty; `social-preview.png` not yet uploaded (repository settings) |
| NuGet packages (4) | Icon, description, tags | Icon, tags and `SqlArtisan.Dapper`'s description land in the next package version |
| README | Tagline, promise, pillars | — |
| docs/ | Voice | — |
| Articles and announcements | Positioning, audience order | None yet; 1.0.0 is the planned launch point |

NuGet metadata ships only with a new package version, and any change under
`src/` requires a new release candidate before 1.0.0.
