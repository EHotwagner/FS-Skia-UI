# Foundations Port: F# Capabilities & Library Selection

- **Date:** 2026-05-31 17:14 CEST
- **Author:** Claude Code (analysis requested by maintainer)
- **Status:** Analysis / recommendation + generated capability skills. Not a decision; not in-flight work.
- **Baseline SHA:** `838127a` (master).
- **Scope:** The Bash/Python → F# port described in the companion reports requires a set of
  concrete F# *capabilities* (parsing, code/artifact generation, IO, shell/process wrapping, graph
  algorithms, diffing, testing). This report enumerates every capability the port needs — grounded
  in the actual scripts — and selects, per capability, the right F# library (or "hand-roll"),
  with an adopt/consider/reject verdict and the rationale tied to the project's standing principles
  (compiled-F#, build-time enforcement, determinism, no FCS at runtime).

## Companion reports (read alongside)

- [`2026-05-31-0908-foundations-rewrite-analysis.md`](./2026-05-31-0908-foundations-rewrite-analysis.md) — the four-problem thesis; §6 config-as-compiled-F#.
- [`2026-05-31-1049-foundations-implementation-plan.md`](./2026-05-31-1049-foundations-implementation-plan.md) — the staged programme; Stage 4 (Python port) and Stage 5 (build front-end) are the consumers of this report.
- Stage-0 baseline: [`_baselines/2026-05-31-foundations.md`](./_baselines/2026-05-31-foundations.md); D2 spike: [`_baselines/2026-05-31-spike-d2-outcome.md`](./_baselines/2026-05-31-spike-d2-outcome.md).

This report deliberately stops short of writing the port. It de-risks the *library choices* that
Stages 3–6 depend on, and ships a set of **local capability skills** (see §10) so the agents doing
those stages have grounded, reusable guidance instead of re-deriving the landscape each time.

---

## 1. Method

Two passes:

1. **Grounding** — read the actual artifacts to be ported, not the prose about them:
   `compute-task-graph.py` (1,310 LOC), `audit-status-scan.py` (151 LOC), `run-audit.sh`
   (1,285 LOC), `common.sh` (657 LOC), the YAML/JSON parsing and MEL engine inside `build.fsx`,
   and the input formats (`tasks.md`, `tasks.deps.yml`, `capabilities.yml`,
   `validation.contract.yml`, `audit-patterns.yml`, `.specify/feature.json`).
2. **Library survey** — the FAKE module reference, XParsec, Myriad, Fabulous.AST/Fantomas, code
   quotations, YamlDotNet/Legivel, FSharp.Formatting/Markdig, System.Text.Json/FSharp.SystemTextJson/
   Thoth, QuikGraph, Microsoft.Extensions.FileSystemGlobbing, Fake.Core.Process/Fake.Tools.Git/
   CliWrap/Fli, DiffPlex, FsCheck/Expecto — plus the awesome-fsharp index.

What is **already on hand** (central package management, `Directory.Packages.props`): `YamlDotNet
17.1.0`, `Fake.Core.Target 6.1.4` (build-tooling), `Expecto 10.2.2`, `FSharp.Core 10.1.300`. The
compiled build front-end (`build/Build.fsproj` → `build/Governance/FS.Skia.UI.Build.fsproj`)
already exists from the D2 spike and is the home for everything below.

---

## 2. Capability inventory (what the scripts actually do)

Every responsibility currently in Bash/Python/inline-`build.fsx`, mapped to the capability it
requires. This is the demand side; §3–§9 are the supply side.

| # | Current owner | Responsibility | Capability needed |
|---|---|---|---|
| C1 | `compute-task-graph.py` 327–421; `build.fsx` 2178–2260 | Parse `tasks.deps.yml` and `capabilities.yml` (hand-rolled, no lib) | **YAML parsing** (typed) |
| C2 | `compute-task-graph.py` 145–307 | Parse `tasks.md` task lines: box `[ X S F - *]`, annotations `[P]`/`[US\d+]`/`[T[12]]`/`[SEH]`/`[skillist: …]`, phase/checkpoint headers, Synthetic-Evidence Inventory tables | **Line-oriented / Markdown parsing** |
| C3 | `audit-status-scan.py` 47–104 | Scan fenced ` ```audit-status ` regions; key=value with first-region-wins, dup-key = error | **Structured region parsing** |
| C4 | `common.sh` 254–347; `build.fsx` 284–330 | Read `.specify/feature.json` robustly (jq→python→grep fallback chain) | **JSON reading** |
| C5 | `compute-task-graph.py` rendering | Emit `task-graph.json` (schema 1.0) | **JSON writing** |
| C6 | `compute-task-graph.py` 849–878 | Cycle detection (WHITE/GRAY/BLACK DFS) | **Graph: cycle detection** |
| C7 | `compute-task-graph.py` 891–915 | Topological sort (Kahn) | **Graph: topo sort** |
| C8 | `compute-task-graph.py` 918–948 | Synthetic propagation + root-cause map (custom rule) | **Graph: custom propagation** |
| C9 | `compute-task-graph.py` 290–305 | Phase-checkpoint implicit edge injection | **Graph: edge synthesis** |
| C10 | `compute-task-graph.py` rendering | Render `task-graph.md`: Mermaid `graph TD`, ASCII tree, count tables | **Document generation (text)** |
| C11 | Stage 2 (`GenerateAgentSkills`) | Generate `.claude/skills/**` from `.agents/skills/**`; render `tasks.md` skillist from `tasks.deps.yml` | **Document generation + currency check** |
| C12 | Stage 3 migration | (One-shot) generate typed `Config.fs` from `capabilities.yml` | **F# source generation** |
| C13 | `compute-task-graph.py` 460–484 | Discover skills: `.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, `template/fragments/*/skill/SKILL.md` | **File discovery / globbing** |
| C14 | `run-audit.sh`; `audit-patterns.yml` whitelist | Match files against `fnmatch` globs (`docs/**`, `**/*.md`) | **Glob matching (fnmatch semantics)** |
| C15 | `run-audit.sh` 961–999 | Git: resolve base ref (main/master/HEAD~1), `merge-base`, `diff --unified=0` | **Git wrapping** |
| C16 | `run-audit.sh` diff-scan | Apply `audit-patterns.yml` regexes to the diff | **Regex over diff** |
| C17 | `run-audit.sh` whole file; `build.fsx` 1266,1273 | Orchestrate graph→audit→diff-scan; temp files; exit-code contract | **Process orchestration** |
| C18 | `build.fsx` MEL engine 233–269; `requiredTargets` | Target list, dependency graph, dispatch | **Build orchestration / CLI** |
| C19 | Stage 4/5 exit gates | Prove ported output byte-identical to golden fixtures | **Text diffing (golden parity)** |
| C20 | `tests/Governance.Tests` | Unit + property test the ported algorithms | **Property/unit testing** |
| C21 | `build.fsx` 3273–3338; `audit-status-scan.py` | Regex: package-version rewrite, key=value extraction, fence detection | **Regex (general)** |

Eight distinct capability families fall out of these 21: **parsing** (C1–C4, C16, C21),
**JSON I/O** (C4–C5), **graph algorithms** (C6–C9), **document/artifact generation** (C10–C11),
**F# source generation** (C12), **file discovery & globbing** (C13–C14), **shell/process/git
wrapping** (C15, C17), **build orchestration/CLI** (C18), **diffing** (C19), **testing** (C20).

---

## 3. Parsing

### 3.1 YAML (C1) — **Adopt YamlDotNet (already present), typed; consider Legivel for pure-F#**

The bespoke YAML parsers (one in Python, one ~80 lines in `build.fsx`) are the single biggest
parity-and-fragility liability. `YamlDotNet 17.1.0` is **already** in `Directory.Packages.props`
(today scoped runtime; the governance library adds a `PackageReference`). Deserialize into typed
F# models behind the public API:

- Define `[<CLIMutable>]` records mirroring `tasks.deps.yml` / `capabilities.yml`, or — better —
  use the low-level `YamlStream`/node model and project into immutable F# records + DUs so the rest
  of the library never sees a mutable bag. The hand-rolled parsers retire entirely (analysis §3.3,
  plan Stage 3.3).
- **Parity caution:** `tasks.deps.yml` has *two* shapes the Python tolerates — object form
  (`{deps, skillist}`) and legacy bare-list. The typed reader must accept both; cover both in the
  DepsParser fixture suite (plan 4.1) before deleting the Python.

**Legivel** (fjoppe) is a pure-F#, YAML-1.2-conformant parser with a discriminated-union result —
attractive for determinism and Fable, but it adds a dependency where one already exists and is
stricter than the lax minimal-YAML the files use. **Verdict: YamlDotNet now; Legivel only if a
spec-conformance or Fable need appears.** Do not keep either bespoke parser.

### 3.2 `tasks.md` line grammar & SEH tables (C2) — **Regex port first (parity), then XParsec**

`tasks.md` is **line-oriented**, parsed in Python with `System.Text.RegularExpressions`-equivalent
regexes (e.g. `^\s*-\s*\[(?P<box>[ X\-FS*])\]\s+(?P<id>T\d{3,4})\b(?P<rest>.*)$`, then annotation
sub-matches). Two routes:

- **(a) Faithful regex port** with `System.Text.RegularExpressions`. Lowest parity risk against the
  Stage-0 golden fixtures (Invariant 6 demands byte-identical output), because it reproduces the
  Python's exact match semantics. **Do this first for Stage 4.**
- **(b) XParsec** (roboz0r) — a pure-F# parser-combinator library, **v1.0.0, MIT, May 2026**, Fable-
  compilable, faster than FParsec in its JSON benchmark (~30 ms vs ~61 ms) with lower allocation,
  operating uniformly over `string`/`'T[]`/`ImmutableArray`/`ReadOnlyMemory`. The annotation grammar
  (ordered optional `[…]` tags + free-text tail) and the SEH inventory **table** parsing are a clean
  fit, yielding typed results instead of brittle capture-group indexing. Note one semantic
  difference from FParsec: *alternatives always backtrack; there is no `attempt`* — design grammars
  accordingly.

**Verdict:** regex port to clear the parity gate, then migrate the grammar to **XParsec** for
robustness once parity is signed off. XParsec is the recommended long-term home for C2/C3/C16. A
full CommonMark AST (FSharp.Formatting.Markdown or Markdig) is **overkill** here — the inputs are a
constrained line grammar, not arbitrary Markdown — so **reject** pulling a document parser for the
task grammar. Keep FSharp.Formatting/Markdig in mind only if a future need to walk arbitrary
prose tables appears.

### 3.3 `audit-status` fenced regions (C3) — **XParsec or small hand parser**

`audit-status-scan.py` has exacting, safety-critical semantics: first ` ```audit-status ` region
wins, unclosed-region detection, `key=value` with `#` comments, **duplicate key = hard error**
(never last-wins), key normalization `.lower().strip()`. Small enough to hand-roll faithfully;
XParsec expresses it cleanly. Port semantics *exactly* and pin them with the audit golden fixture.
**Verdict: XParsec (preferred) or a ≤60-line hand parser; faithful port + fixture is mandatory.**

### 3.4 JSON read/write (C4, C5) — **Adopt System.Text.Json + FSharp.SystemTextJson**

`.specify/feature.json` is trivial (one key) — read it with `System.Text.Json` and stop the
three-language jq→python→grep fallback chain. For `task-graph.json` *emission* the schema must stay
byte-compatible (schema 1.0), so drive it with explicit `Utf8JsonWriter` or a typed model. Because
the models involve F# records/DUs, add **FSharp.SystemTextJson** (Tarmil) — `System.Text.Json`
alone cannot round-trip DUs. **Reject Thoth.Json** (Fable-first, 2–10× slower, no benefit off the
browser) and **Newtonsoft** (legacy; STJ is the modern default). **Verdict: System.Text.Json +
FSharp.SystemTextJson.**

---

## 4. Graph algorithms (C6–C9) — **Hand-roll in F#, property-test with FsCheck**

Cycle detection (3-colour DFS), Kahn topological sort, and especially the **custom synthetic
propagation** (`declared=synthetic → synthetic`; `declared=done ∧ any dep (auto-)synthetic →
auto-synthetic`, except accepted-`[SEH]`; plus the upstream root-cause map) are small, standard, and
*central*. Hand-rolling them as **pure functions** over a typed `Task`/`Dep` model:

- guarantees output parity with the Python (you control every tie-break and ordering),
- makes them unit- and **property-testable** — propagation is monotone; a graph with no synthetic
  roots has no auto-synthetic nodes; topo order respects every edge — exactly the invariants
  **FsCheck** (v3, integrates with the in-repo Expecto) shrinks counterexamples for,
- carries zero new runtime dependency.

**QuikGraph** (KeRNeLith) offers `TopologicalSortAlgorithm`, `SourceFirstTopologicalSort`, DFS, and
DAG cycle checks and would be a fine backstop — but it brings a C# dependency for ~40 lines of
standard code, *and* the propagation rule is bespoke and would sit outside it anyway. **Verdict:
hand-roll C6–C9; FsCheck property tests; QuikGraph only if the graph work later grows beyond these
primitives.** Phase-checkpoint edge synthesis (C9) is pure list manipulation — no library.

---

## 5. Code & artifact generation

A critical distinction the word "code generation" blurs — two *different* capabilities:

### 5.1 Document/artifact generation (C10, C11) — **No library; typed rendering**

`task-graph.md` (Mermaid `graph TD`, ASCII `└──` tree, count tables), `task-graph.json`, and the
generated `.claude/skills/**` are **text/document** outputs, not F# source. Build them with plain
typed rendering (string builders, `Utf8JsonWriter`) so the output is deterministic and
byte-comparable to the golden fixtures. Mermaid and ASCII tree are a few dozen lines each. The
`.claude`-from-`.agents` generation (plan Stage 2) is a file copy + a **currency check** (regenerate
to temp, diff, fail if stale) — strictly better than the current unguarded drift. **Verdict: no
dependency; structured rendering + DiffPlex (§8) for the currency/parity diff.** Code quotations are
**the wrong tool** here (see 5.3).

### 5.2 F# source generation (C12) — **Fabulous.AST/Fantomas (one-shot) or Myriad (recurring)**

Only one task is genuinely *F#-source* generation: turning `capabilities.yml` into a typed
`Config.fs` of compiled values during the YAML→compiled-F# migration (analysis §6, plan 3.3/5.5).
Two real options:

- **Fabulous.AST** (edgarfgp) — a DSL over Fantomas's Oak AST; you describe the F# you want as a
  node tree and Fantomas pretty-prints style-correct source. Best for a **one-shot/occasional**
  generator run by a person or a build step. Pair with **Fantomas** for formatting.
- **Myriad** (MoiraeSoftware, v0.85) — a **pre-build, plugin-based** generator wired into the
  `.fsproj` (`<MyriadFile>`); regenerates on every build from annotated input or `myriad.toml`.
  Best when the typed config must be **continuously** re-derived from the YAML and you want the
  generation to be part of compilation.

**Verdict:** if `capabilities.yml` is migrated *once* into hand-owned compiled values, prefer a
**Fabulous.AST + Fantomas** one-shot (no permanent build dependency). If the team wants the catalog
to *stay* as data and be compiled each build, adopt **Myriad**. Given the config-representation ADR
(D6) points at hand-owned compiled F#, the **one-shot Fabulous.AST path is the lighter fit**; record
the choice in the Stage-5 ADR.

### 5.3 Code quotations — **Reject for this work (scope note)**

F# **code quotations** (`<@ … @>`) are *runtime* metaprogramming — they produce `Expr` trees
evaluated/inspected at run time. They are not a source-emission tool and do not write build
artifacts; using them here would re-introduce exactly the runtime-evaluation tax (and the FCS-style
"logic compiled at run time" anti-pattern) that the foundations programme is removing (analysis §6,
plan D6). **Verdict: do not use quotations for the governance port.** They are noted only to
prevent the common conflation with source generation.

---

## 6. File discovery & globbing (C13, C14)

- **Skill discovery (C13)** — enumerate `.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`,
  `template/fragments/*/skill/SKILL.md`. `System.IO.Directory.EnumerateFiles` /
  **`Fake.IO.Globbing`** (the FAKE family already in-tree via the front-end) both do this; prefer
  **Fake.IO.Globbing** to keep one IO idiom across the build. **Adopt Fake.IO.Globbing.**
- **Whitelist glob matching (C14)** — `audit-patterns.yml` whitelists use **fnmatch** globs
  (`docs/**`, `**/*.md`). **`Microsoft.Extensions.FileSystemGlobbing.Matcher`** implements `*`/`**`
  include/exclude semantics natively and is a first-party package. **Parity caution:** .NET glob
  semantics differ from Python `fnmatch` at the edges (e.g. a single `*` crossing directory
  separators, leading `**/`); fixture-test every whitelist entry against the Python before cutover.
  **Verdict: adopt `Microsoft.Extensions.FileSystemGlobbing` for whitelist matching; golden-test the
  semantics.**

---

## 7. Shell, process & git wrapping (C15, C17)

The strategic win is **in-process-first**: once the graph/audit/diff-scan live in the library, most
of `run-audit.sh`'s orchestration *disappears* — the build calls F# functions directly instead of
shelling to Bash→Python→JSON→re-parse (analysis §3, plan 4.5–4.6). What remains:

- **Git (C15)** — base-ref resolution, `merge-base`, `diff --unified=0`. Use **`Fake.Tools.Git`**
  (FAKE family) for ergonomic, tested git wrapping, or `Fake.Core.Process` for raw `git` calls.
  **Adopt Fake.Tools.Git.**
- **Process (C17 residue)** — the few genuine external invocations (e.g. `dotnet`, smoke runners).
  **`Fake.Core.Process`** is already transitively present via `Fake.Core.Target` and keeps the
  dependency family consistent. **CliWrap** (Tyrrrz) + **CliWrap.FSharp**, or **Fli** (CaptnCodr,
  F#-native CE) are more ergonomic for piping/async, but add a dependency where Fake.Core.Process
  suffices. **Verdict: Fake.Core.Process for residual shelling; CliWrap/Fli only if rich piping is
  needed.** Keep true OS-glue (`fake.sh`/`fake.cmd` launchers, container entrypoints) in Bash —
  F#-ifying a three-line launcher has no payoff (analysis §3 verdict 3).

---

## 8. Build orchestration, CLI, diffing, testing

- **Orchestration / CLI (C18)** — **`Fake.Core.Target`** is already adopted and the D2 spike proved
  it drives targets from the compiled exe with no FSX runner and **no `FSharp.Compiler.*`**
  transitive (FR-012). Dispatch via `Target.runOrDefaultWithArguments`. For a richer `Route` CLI,
  **Argu** is the idiomatic typed arg-parser; **Spectre.Console** for tables/colour output. Both
  **optional** — adopt only if the front-end's CLI grows. **Verdict: Fake.Core.Target (done); Argu/
  Spectre.Console optional.**
- **Diffing (C19)** — the Stage-4/5 parity gates need readable golden diffs. **DiffPlex** (mmanela,
  v1.9.0) generates unified/side-by-side text diffs; **Verify.DiffPlex** integrates with snapshot
  testing. **Adopt DiffPlex** for the parity gate and the Stage-2 generation-currency check.
- **Testing (C20)** — **Expecto 10.2.2** is already in-tree. Add **FsCheck (v3)** via
  `Expecto.FsCheck` for the graph property tests (§4). **Adopt FsCheck 3.**

---

## 9. Consolidated dependency decision

All new packages are **build-tooling scope** (referenced by `build/Governance/**` and/or
`tests/Governance.Tests`), **never shipped in a generated product**, and every version goes in
`Directory.Packages.props` per Central Package Management.

| Capability | Package | Status | Verdict |
|---|---|---|---|
| YAML parse (C1) | `YamlDotNet` 17.1.0 | **present** (add ref) | **Adopt** |
| JSON DU read/write (C4,C5) | `FSharp.SystemTextJson` | add | **Adopt** (STJ base is built-in) |
| Line/region/diff parsing (C2,C3,C16) | `XParsec` | add | **Adopt** (after regex-port parity) |
| Glob whitelist (C14) | `Microsoft.Extensions.FileSystemGlobbing` | add | **Adopt** |
| File discovery (C13) | `Fake.IO.FileSystem` / `Fake.IO.Globbing` | add (FAKE family) | **Adopt** |
| Git wrapping (C15) | `Fake.Tools.Git` | add (FAKE family) | **Adopt** |
| Residual process (C17) | `Fake.Core.Process` | transitive (present) | **Adopt** |
| Orchestration (C18) | `Fake.Core.Target` 6.1.4 | **present** | **Adopt** (done) |
| Golden diff (C19) | `DiffPlex` | add | **Adopt** |
| Property tests (C20) | `FsCheck` 3 (`Expecto.FsCheck`) | add (Expecto present) | **Adopt** |
| F# source gen (C12) | `Fabulous.AST` + `Fantomas` | add (one-shot) | **Consider** (Myriad if recurring) |
| Richer CLI (C18) | `Argu`, `Spectre.Console` | add | **Consider** |
| Graph algorithms (C6–C9) | `QuikGraph` | — | **Reject** (hand-roll + FsCheck) |
| YAML alt | `Legivel` | — | **Reject** (YamlDotNet present) |
| JSON alt | `Thoth.Json`, `Newtonsoft.Json` | — | **Reject** (STJ + FSharp.SystemTextJson) |
| Markdown AST | `FSharp.Formatting`, `Markdig` | — | **Reject** for task grammar (line-oriented) |
| Source emission via `<@ @>` | code quotations | — | **Reject** (runtime metaprogramming, wrong tool) |
| Rich process | `CliWrap`/`Fli` | — | **Reject** unless piping needed |

**Minimal adopt set (8):** YamlDotNet, FSharp.SystemTextJson, XParsec, Microsoft.Extensions.
FileSystemGlobbing, Fake.IO.*, Fake.Tools.Git, DiffPlex, FsCheck. All are MIT/permissive,
actively maintained, net10-compatible, and build-tooling-only. None re-introduces FCS or
runtime-script compilation; all are compiled into the governance library — consistent with D2/D6.

---

## 10. Mapping to plan stages

| Plan stage | Capabilities | Libraries this report selects |
|---|---|---|
| Stage 2 (single-source generation) | C11, C14, C19 | document rendering + Microsoft.Extensions.FileSystemGlobbing + DiffPlex (currency check) |
| Stage 3 (library skeleton + cheap validators) | C1, C4, C13, C21 | YamlDotNet (typed catalog), System.Text.Json, Fake.IO.Globbing |
| Stage 3.3 / 5.5 (config → compiled F#) | C12 | Fabulous.AST + Fantomas (one-shot) *or* Myriad |
| Stage 4 (Python port — flagship) | C2,C3,C5,C6–C10,C16 | regex-port → XParsec; FSharp.SystemTextJson; hand-rolled graph + FsCheck; DiffPlex parity gate |
| Stage 4.5/4.6 (rewire, retire run-audit.sh) | C15, C17 | Fake.Tools.Git, Fake.Core.Process |
| Stage 5 (build front-end + Routing.fs) | C18 | Fake.Core.Target (done); Argu/Spectre.Console optional |
| Stage 6 (codify rules, version contract) | C20 | Expecto + FsCheck property/unit gates |

---

## 11. Local capability skills generated

Per the request, the capability knowledge above is captured as **six local skills**, written
byte-identically into both `.claude/skills/` and `.agents/skills/` (the synchronized-peer
requirement; the future `SkillSyncCheck` will verify identity). Each encodes the verdicts, the
concrete F# snippets, the exact input grammars, and the parity/determinism cautions, so the agents
executing Stages 2–6 invoke the skill instead of re-deriving the landscape.

| Skill | Covers capabilities | Used by stages |
|---|---|---|
| `fsharp-parsing` | C1–C4, C16, C21 (YAML/line-grammar/region/JSON/regex) | 3, 4 |
| `fsharp-graph-algorithms` | C6–C9 (cycle/topo/propagation + FsCheck) | 4 |
| `fsharp-code-generation` | C10–C12 (doc rendering, F# source gen, quotations caveat) | 2, 3, 5 |
| `fsharp-io-globbing` | C13–C14 (discovery, fnmatch glob, currency diff) | 2, 3 |
| `fsharp-shell-process` | C15, C17 (git/process; in-process-first) | 4 |
| `fsharp-build-orchestration` | C18–C20 (Fake.Core.Target, DiffPlex, Expecto/FsCheck) | 5, 6 |

These are **capability/reference** skills (not Spec Kit command skills); they are not referenced by
any task `skillist` and therefore do not alter the evidence graph. They become discoverable to any
agent and are cited from the stage workflows above.

---

## 12. Risks & cautions

- **Parity is the hard gate, not library choice.** Every ported parser/algorithm must produce
  byte-identical output to the Stage-0 golden fixtures (Invariant 6) before the Python is deleted.
  The regex-port-first recommendation for C2 and the "faithful port + fixture" rule for C3 exist to
  protect that gate; library elegance is secondary.
- **Glob/fnmatch semantic drift (C14)** and **two YAML shapes (C1)** are the two most likely silent
  divergences — both are called out with a "golden-test before cutover" mitigation.
- **Scope discipline:** adopt the 8-package minimal set; treat Fabulous.AST/Myriad, Argu/Spectre,
  CliWrap/Fli, QuikGraph as deferred "consider/reject" so the dependency surface stays small and
  every addition is build-tooling-only and FCS-free.

## 13. Next step

This is analysis + skills only; no build code changed and no FAKE target run. The natural follow-up
is **Stage 3** (library skeleton), which is the first consumer of §9's adopt set and of the
`fsharp-parsing` / `fsharp-io-globbing` skills — done as a Spec Kit feature under the framework-
author light tier per the plan's meta-process.

---

## Sources

- FAKE module reference — https://fake.build/reference/index.html ; process guide — https://fake.build/guide/core-process.html
- XParsec — https://github.com/roboz0r/XParsec
- Myriad — https://github.com/MoiraeSoftware/myriad ; docs — https://moiraesoftware.github.io/Myriad/
- Fabulous.AST — https://github.com/edgarfgp/Fabulous.AST ; Fantomas code gen — https://fsprojects.github.io/fantomas/docs/end-users/GeneratingCode.html
- Code quotations — https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/code-quotations
- YamlDotNet — https://github.com/aaubry/YamlDotNet ; Legivel — https://github.com/fjoppe/Legivel
- FSharp.Formatting (Markdown) — https://fsprojects.github.io/FSharp.Formatting/markdown.html ; Markdig — https://github.com/mmanela/diffplex (diff) / https://github.com/xoofx/markdig
- FSharp.SystemTextJson — https://github.com/Tarmil/FSharp.SystemTextJson ; Thoth.Json — https://github.com/thoth-org/Thoth.Json
- QuikGraph — https://github.com/KeRNeLith/QuikGraph
- Microsoft.Extensions.FileSystemGlobbing — https://learn.microsoft.com/en-us/dotnet/core/extensions/file-globbing
- CliWrap — https://github.com/Tyrrrz/CliWrap ; Fli — https://github.com/CaptnCodr/Fli
- DiffPlex — https://github.com/mmanela/diffplex ; Verify.DiffPlex — https://github.com/VerifyTests/Verify.DiffPlex
- FsCheck — https://fscheck.github.io/FsCheck/ ; Expecto — https://github.com/haf/expecto
- awesome-fsharp — https://github.com/fsprojects/awesome-fsharp
