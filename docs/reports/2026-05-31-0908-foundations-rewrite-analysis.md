---
title: Foundations Re-examination - Process, Build, Language, and Governance
---

# Foundations Re-examination: Process, Build, Language, and Governance

- **Date:** 2026-05-31 09:08 CEST
- **Author:** Claude Code (analysis requested by maintainer)
- **Status:** Analysis / recommendation. Not a decision; not in-flight work.
- **Scope:** Four foundational questions raised by the maintainer, plus adjacent issues found during investigation.

## Reading this in context

This report deliberately builds on, and does not repeat, prior analyses already in
`docs/reports/`:

- `V2Analysis.md` — set the "deterministic shell around nondeterministic AI" direction; already recommended moving governance behind machine-readable schemas and shrinking `build.fsx`.
- `v3Design.md` — modular package distribution + lean generated projects (future direction).
- `2026-05-28-1557-agent-consumer-framework-analysis.md` — reframed the governance harness as an *agent contract*; recommended `validation.contract.yml` tiers, native FAKE targets, a consolidated `agent-verdict.json`.
- `template-framework-analysis.md` — the governed-template proposal (comprehensive; phased).
- `2026-05-27-2204-refactoring-analysis.md` — targeted refactor, not rewrite; flagged `build.fsx` (then 4,071, now 4,688 lines) as the top decomposition target.

Where those reports said "modularize the script" and "use schemas," this report pushes the
argument one level further and ties the four questions together under a single thesis. The
prior reports treated build, governance, and process as separate clean-up items. They are
not separate. They are one mistake repeated in four places.

---

## Thesis: the project turned its own determinism engine inward and it became bureaucracy

FS.Skia.UI's central, correct idea is a **deterministic shell around nondeterministic AI**:
`.fsi` signatures, surface baselines, FAKE gates, and evidence audits exist so that an AI
agent building an *application* on top of the framework can be held to checkable invariants
instead of trusted prose.

The four problems the maintainer raises are all the same problem: **that determinism
discipline was applied to the product's public surface but never to the project's own
development process or tooling.** The result is that framework development is run as a
*nondeterministic-agent-reads-prose-and-is-trusted-to-comply* loop — exactly the thing the
framework was built to abolish — while the genuinely deterministic machinery (the build, the
graph algorithms, the rule checks) is scattered across a 4,688-line script, two Python files,
a 1,284-line Bash orchestrator, and ~23,000 lines of governance Markdown.

Every one of the maintainer's four instincts is correct. The rest of this report establishes
the evidence, weighs the trade-offs honestly, and gives a verdict on each, then proposes a
single convergent direction rather than four independent fixes.

---

## 1. Framework development vs. application development are different jobs

### What is actually happening

The repository dogfoods its full consumer-grade governance during framework development:

- **38 numbered spec directories**, **~58,880 lines** of `spec.md`/`plan.md`/`tasks.md`/
  research/contracts, **773 spec Markdown files**.
- Per-feature ceremony measured on the current feature (`038-authoring-guidance-consistency`,
  39 tasks): mandatory post-generation skill evaluation, Synthetic-Evidence Inventory tables
  (up to 9 columns/row), Constitution Check gate (11 decision areas), per-task skill-loading
  evidence, evidence graph + audit refresh. Estimated **~12–14 hours of process overhead per
  feature**, most of it attention spent satisfying gates rather than changing the framework.
- The serialized validation order in `CLAUDE.md`/`AGENTS.md` (six FAKE targets, run
  sequentially because they share `.fake` state) is the *minimum* a framework change is
  expected to clear.

This governance was designed for the consumer — an AI agent building an app on the framework
who genuinely needs guardrails, evidence paths, and reproducibility. Applying it unchanged to
the people *building the framework* is a category error. Framework development needs the
opposite affordances: fast iteration on the rendering boundary, freedom to break `.fsi`
contracts and refactor across packages, cheap experimentation. Running every such change
through consumer-grade ceremony is the "suffocation" the maintainer describes.

### The nuance that keeps it from being a clean cut

There is one legitimate reason to dogfood: the framework needs to *prove its own governance
harness works* before shipping it to consumers. If no framework feature ever runs the full
Spec Kit + evidence pipeline, the pipeline rots. So the answer is not "framework development
gets no governance" — it is "governance becomes opt-in and sampled instead of mandatory and
total."

### Trade-offs

| Approach | Pros | Cons |
|---|---|---|
| **Status quo** (full governance on all framework work) | Harness is continuously exercised; uniform process; nothing is "untracked" | Suffocating; ~12–14h/feature overhead; attention spent on ceremony not design; discourages refactoring and breaking-change cleanup |
| **Two explicit tiers** (lightweight framework-author loop + full consumer-agent governance, with a small dogfooding sample) | Matches effort to risk; keeps framework iteration fast; still exercises the harness on chosen features | Requires defining and enforcing the tier boundary; risk of the "fast" tier silently becoming the only tier and the harness rotting |
| **Drop dogfooding entirely** | Maximum framework velocity | Harness ships to consumers untested against real work; regressions discovered by consumers |

### Verdict — **Strongly agree. Split the process into two tiers.**

1. Define a **framework-author inner loop** that is the real default for framework work:
   `Restore → Build → Test → .fsi surface check`. No spec directory, no skillist, no
   synthetic-evidence inventory, no constitution gate. This is `./fake.sh build -t Dev` plus a
   surface check, and nothing else.
2. Reserve the **full Spec Kit + evidence governance** for two things only: (a) the generated
   consumer projects (where it belongs), and (b) a small, explicitly chosen set of dogfooding
   features that exist precisely to keep the harness honest. Mark those features; everything
   else uses the inner loop.
3. The agent-consumer analysis already proposed `validation.contract.yml` with tiers
   (`inner-loop`, `focused-authority`, `agent-ready`, `maintainer-verify`, `automation-final`).
   Extend that idea so the tier is selected by **who is developing what**, not only by which
   paths changed. Framework-internal changes default to `inner-loop`; consumer-template and
   governance changes escalate.

This is the highest-leverage change in the report because it directly reclaims the maintainer's
time and attention, and it requires no rewrite — only a policy and a thin gate.

---

## 2. `build.fsx` (script-run FAKE) vs. a dedicated build project

### Correcting the premise (it matters)

The build does **not** run via `dotnet fsi build.fsx`. `fake.sh`/`fake.cmd` run the
**FAKE 6.1.4 CLI tool** (`dotnet fake`), which compiles `build.fsx` — with its inline
`#r "paket: ..."` dependency header — into a content-hashed DLL cached under
`.fake/build.fsx/` and re-runs it. So the real choice the maintainer is pointing at is:

> **script-based FAKE** (a `build.fsx` compiled by the `fake` runner) **vs. a dedicated,
> normally-compiled build project** (a `Build.fsproj` referencing the `Fake.*` libraries,
> run with `dotnet run`), as the FAKE docs' "dedicated build project" guide recommends.

This distinction is real and the maintainer's instinct is right, but it is the *smaller* of
two findings. The larger finding is below.

### The script is not really a build script

`build.fsx` is **4,688 lines**. Only ~400 of those are build orchestration (a target list, a
dependency graph wired with `==>`, a `runTarget` dispatcher). The other **~2,000+ lines are a
governance/validation engine**:

- A hand-rolled YAML parser for the capability catalog (~60 lines, no schema, fragile to
  indentation).
- ~500 lines of capability validation, ~800 lines of generated-product validation, ~480 lines
  of target-metadata validation/drift, ~270 lines of process-health/bootstrap checks, plus
  guidance/skill scanners.
- A full Model-Effect-interpreter (MEL) architecture (`BuildMsg`/`BuildEffect`/`BuildModel` +
  `update` + `interpret`) — a reasonable design, implemented inside a script where it cannot
  be unit-tested, cannot be referenced from anywhere else, and is recompiled as one unit on
  every edit.

The pain this causes:

- **No incremental compilation, no IDE-grade tooling** on a 207 KB script. Every edit
  re-compiles the world.
- **No unit tests** for the most consequential logic in the repository (the rules that decide
  whether a feature is "ready").
- **Stringly-typed everything**: `StartTarget "TemplatePack"`, `RequireFiles("...", ...)`.
  Target renames are grep-and-pray.
- **Second sources of truth**: target metadata is pattern-matched from target-name strings,
  separate from the `update` function it describes; the build then *checks for drift* between
  them rather than deriving one from the other.
- **It shells out to Bash/Python for the evidence graph** (item 3), so the build has no typed
  view of its own most important gate — only "ok"/"failed" and a re-parsed JSON blob.

### Trade-offs

| Option | Pros | Cons |
|---|---|---|
| **Keep `build.fsx` as-is** | Zero migration cost; familiar; `#r "paket:"` keeps deps inline | All pain above persists; the most important logic stays untested and unreferenceable |
| **Dedicated build project** (`Build.fsproj`, `dotnet run`) | Real compilation, incremental builds, full IDE tooling, can split into modules, can unit-test | Slightly heavier than a script; one more project in the solution; still couples orchestration and governance if you stop here |
| **Extract a tested governance *library* + thin build front-end** (recommended) | The 2,000 lines of rules become a normal, unit-tested F# library (`FS.Skia.Governance`) with real types; the build (FAKE project *or* a thin `.fsx`) just calls it; subsumes items 3 and 4 | Largest up-front effort; requires designing the library API |

### Verdict — **Agree the script must go, but the dedicated-build-project framing undersells the win.**

The dedicated build project is correct and worth doing, but on its own it just relocates the
problem. The decisive move is to recognize that **most of `build.fsx` is not a build — it is a
governance library wearing a build's clothes.** Extract it:

1. Create `FS.Skia.Governance` (or similar) as a normal compiled F# library: typed `Target`
   union instead of strings, typed catalog model (parsed once, validated with proper errors),
   the MEL engine, and *unit tests* for the validation rules.
2. Make the build a **thin front-end** over that library. Whether the front-end is a FAKE
   *dedicated build project* or a small `build.fsx` that `#load`s the compiled library is a
   secondary decision — both are fine once the logic is out. A dedicated project is the
   cleaner default because it gets first-class tooling and references the library directly.
3. This is the keystone refactor: it is also the home for the ported Python (item 3) and the
   codified Markdown rules (item 4). Do this once and three of the four problems collapse into
   it.

Keep `paket`/`#r` inline deps only if you keep a script front-end; a dedicated project uses
normal `PackageReference` and Central Package Management, which you already run elsewhere.

---

## 3. Bash and Python → F#

### What the scripts actually contain

This is the most clear-cut item, because the investigation found that the Python is **not
glue — it is core domain logic**, and it is *duplicated in spirit* with `build.fsx`:

- **`compute-task-graph.py` — 1,310 lines.** Parses `tasks.md` and `tasks.deps.yml` (custom
  minimal YAML parser), discovers the skill registry, runs **multi-pass validation**, **cycle
  detection (3-colour DFS)**, **topological sort (Kahn)**, and **synthetic-evidence
  propagation** (`done` task with a synthetic dependency becomes `auto-synthetic`), then renders
  JSON + Mermaid + Markdown. This is the evidence graph — the project's single most important
  governance computation — written in an untyped language with no tests.
- **`audit-status-scan.py` — 150 lines.** Structured-only `audit-status` region scanner with
  deterministic first-region-wins and duplicate-key rules; small but safety-critical.
- **`run-audit.sh` — 1,284 lines of Bash with nine embedded Python blocks** orchestrating the
  above plus diff-scan. The build shells out to *this* for `EvidenceGraph`/`EvidenceAudit`.

So the project's flagship gate currently crosses **three languages and a process boundary**
(F# build → Bash orchestrator → Python algorithms → JSON → re-parsed in F#), and the F# side
has no typed view of any of it. `build.fsx` even re-implements feature-directory resolution
(`activeFeatureId`) that already exists in `common.sh` — duplication across the boundary.

The remaining Bash is genuinely mixed:

- **Pure OS glue** (keep in Bash, or absorb later): `fake.sh`/`fake.cmd` launchers, container
  entrypoints, `us1-vulkan-smoke.sh`, the hook validators.
- **Substantial logic that happens to be in Bash**: `common.sh` (656 lines: repo-root
  detection, feature resolution, template composition), the Spec Kit git scripts. These are
  upstream Spec Kit's shape, not glue, but porting them is a bigger lift.

### Trade-offs

| Approach | Pros | Cons |
|---|---|---|
| **Keep Python/Bash** | Lightweight; matches upstream Spec Kit; no port cost | Core graph/propagation logic untyped and untested; tri-language process boundary; duplicated logic; build is blind to its own gate |
| **Port the two Python files to F#** (into the governance library) | Type-safe DAG + propagation; unit-testable; build calls it in-process, no shell/JSON round-trip; collapses a language; removes duplication | Forks from upstream Spec Kit's Python; you own the algorithm |
| **Port Python *and* the heavy Bash** (`run-audit.sh`, `common.sh`, git scripts) | Spec Kit toolchain becomes a single F#/dotnet tool; one language | Large effort; deepens the fork from upstream Spec Kit |

### Verdict — **Agree, with a sharp priority order.**

1. **Port `compute-task-graph.py` and `audit-status-scan.py` to F#** as modules of the
   governance library (item 2). This is high-value and well-bounded: the algorithms are
   standard, the inputs/outputs are clear, and once ported the build computes the evidence
   graph **in-process** with typed results instead of shelling to Bash and re-parsing JSON.
   It also eliminates the `activeFeatureId`/`common.sh` duplication.
2. **Reduce `run-audit.sh` to a thin shim or delete it** once the Python is gone — its job was
   to orchestrate the Python.
3. **Keep the OS-glue Bash** (`fake.sh`, container entrypoints, smoke runner). There is no
   benefit to F#-ifying a three-line launcher.
4. **Defer the heavy Spec Kit Bash** (`common.sh`, git scripts). Only port these if you decide
   to own the whole Spec Kit toolchain as a dotnet tool (a strategic decision, not a clean-up).

**The honest cost:** porting the Python forks you from upstream Spec Kit. But the repo has
already heavily customized Spec Kit (`extensions/`, `presets/`, custom evidence rules) — the
fork has effectively already happened. Owning the algorithm in typed, tested F# is the right
trade given how central it is.

---

## 4. Agent-facing Markdown bureaucracy → deterministic F# code

### The scale of the problem

- **~23,065 lines of governance Markdown** the agent is instructed to read and obey, against
  roughly **~1,100 lines of F# validation** — a **~21:1 prose-to-code ratio** for rules that
  are mostly mechanical.
- **`.claude/skills/` and `.agents/skills/` are 19 byte-for-byte identical `SKILL.md` files,
  ~2,927 lines each (~5,854 duplicated lines).** `CLAUDE.md` calls them "synchronized peers"
  — but **no build check enforces the sync.** A change to one silently diverges from the other.
- Many "rules" are enforced only by asking an agent to read prose and comply: skillist
  presence and ordering, the `[SEH]` design-phase-only timing rule, synthetic propagation,
  Constitution Check completeness, section-presence in skills. Several have *tests* but no
  *production gate* (e.g. nothing prevents a late `[SEH]` tag in real code; only a test fixture
  proves the checker would catch it).

### The principle the project already believes but didn't apply here

The framework's own thesis is: *a checkable invariant should be enforced by code that fails,
not by prose a nondeterministic agent is trusted to honour.* They applied this to the product's
`.fsi` surface. They did not apply it to their own governance. So the governance loop is the
exact anti-pattern the framework exists to replace — and it is expensive in the three ways the
maintainer named: **attention** (every agent invocation burns context reading rules),
**reliability** (prose compliance is probabilistic), and **money** (those tokens cost real
dollars on every run).

### The three buckets (this is the key distinction)

Not all 23,000 lines should become code. Sort them:

- **(a) Deterministic checks → F# validators.** Skillist presence/structure, skill-id
  resolution, `.claude`/`.agents` sync, `[SEH]` timing, synthetic propagation, evidence-audit
  verdict, section-presence. These should be **code that fails the build**. The agent then does
  not need to read the rule *at all* — the rule enforces itself, for free, deterministically.
  Estimated compression: ~570 lines of prose rules → ~150–200 lines of typed validators.
- **(b) Genuine guidance the agent reads to act well.** Architecture rationale, when/why to use
  a feature, design intent ("Elmish makes the hard part observable"). **Keep as prose**, but
  minimize and de-duplicate. ~350 lines.
- **(c) Pure duplication.** The `.claude`/`.agents` mirror, the constitution echoed across
  templates, `CLAUDE.md`/`AGENTS.md` overlap. **Eliminate via single-source + generation.**
  ~6,000 lines.

### Trade-offs

| Approach | Pros | Cons |
|---|---|---|
| **Keep prose governance** | Flexible; no code to maintain; agents can "interpret" | Expensive per-invocation (attention/$); probabilistic compliance; silent drift (the unvalidated `.claude`/`.agents` mirror); rules and reality diverge |
| **Codify bucket (a), generate bucket (c), keep (b)** | Rules enforce themselves for free; agent context shrinks dramatically; drift becomes impossible, not merely detected | Up-front work to write validators; some "judgement" rules resist full codification and need a prose fallback |
| **Try to codify everything** | Maximal determinism | Bucket (b) genuinely needs natural language; over-codifying turns guidance into brittle keyword-matching |

### Verdict — **Strongly agree, with the bucket discipline above.**

1. Move every **bucket (a)** rule into the governance library (item 2) as a validator that
   **fails the build**. The agent stops reading these rules; the build enforces them.
2. **Generate, don't hand-sync, bucket (c).** Pick one source of truth for skills and emit
   `.claude/` and `.agents/` from it (or symlink / share a single directory). Same for the
   constitution fragments echoed into templates. The build currently *checks drift* — generating
   from one source makes drift structurally impossible, which is strictly better than detecting
   it after the fact. As an immediate stopgap, add the missing `.claude`↔`.agents` sync check;
   it is a one-line risk today.
3. Keep **bucket (b)** as prose, trimmed. Target a governance Markdown footprint in the low
   hundreds of lines, not 23,000.

This item and item 1 are the same fix from two angles: item 1 stops applying the ceremony to
framework work; item 4 converts the ceremony that remains from "prose an agent obeys" into
"code that enforces itself." Do both and the bureaucracy stops costing attention and money on
every run.

---

## 5. Adjacent findings worth flagging

These surfaced during investigation and reinforce the thesis:

- **Generation beats drift-checking, everywhere.** Target metadata vs. the `update` function,
  `.claude` vs. `.agents`, skillist in `tasks.md` vs. `tasks.deps.yml`, constitution echoed in
  templates — all are *hand-maintained duplicates the build checks for drift.* Every one of
  these should be **derived from a single source**. A drift check is a confession that you have
  two sources of truth; generation removes the second.
- **Committed evidence is modest — earlier "~38 GB" was the working tree, not the repo.** The
  *tracked* repo is only ~24 MB (~15 MB git history); the ~38 GB working tree is almost entirely
  gitignored build output (`.fake/`, `bin/obj/`, `artifacts/`). Committed evidence is ~5 MB of
  `readiness.zip` archives plus ~3 MB of logs. So this is a non-problem: the right move is just to
  `.gitignore` *future* regenerable logs/zips, not to clean up or rewrite history. Noted here so
  the figure is not mistaken for a pressing concern.
- **No versioning on the generated-product contract.** The ~800 lines of generated-product
  validation hard-code exact structural expectations with no deprecation path. When the
  governance moves into a library (item 2), **version that contract** so template changes have a
  migration window instead of a hard break.
- **The MEL engine is worth keeping — just relocate it.** The Model-Effect-interpreter design
  inside `build.fsx` is over-engineered for a build script but is a sound seed for a real,
  tested governance engine. Don't throw it away in a rewrite; extract and test it.

---

## 6. Configuration representation: compiled F# over YAML and over runtime-loaded FSX

A natural question follows from the move to a typed governance library: if the rules become F#,
should the *configuration* that drives them (the `validation.contract.yml` tiers and routing, the
capability catalog, the diff-scan patterns) also stop being YAML? And if so, should it be F#
script (`.fsx`) loaded and compiled at startup via FSharp Compiler Services (FCS) so it is
type-checked? Security — arbitrary code execution — is correctly *not* a concern for
framework-internal config, since the project already runs `build.fsx` as code.

This is a false binary. There are three options, and the strongest one is neither YAML nor
FCS-loaded FSX:

1. **YAML / data parsed at runtime** — today's `validation.contract.yml`. Inert and
   language-agnostic, but stringly-typed and needs a parser plus a validator.
2. **`.fsx` loaded and compiled at runtime via FCS** — type-checked, but *at load time*.
3. **Plain `.fs` compiled into the governance library** — config is F# values and functions in a
   normal compiled module (`let tiers = [ … ]`; routing rules as predicates).

| | YAML (1) | FSX via FCS (2) | Compiled F# (3) |
|---|---|---|---|
| When errors surface | runtime parse/validate | **runtime load/compile** | **build time — the build fails** |
| Startup cost | trivial parse | **FCS compile on every run** | none (already compiled) |
| References the `Target` DU / domain types | no (strings) | yes | yes |
| Rules as *functions*, not patterns | no | yes | yes |
| IDE / refactor / unit-test | weak | awkward | full |
| Determinism | inert data | can do arbitrary IO at load | pure values |

**Verdict — compiled F# (3) for framework-owned config; keep a data format only for high-churn,
agent-authored, logic-free instance data; do not adopt FCS-loaded FSX.**

The prize the FSX idea reaches for — "type-checked config" — is really *"rules that are
executable predicates sharing the domain types"* (a routing rule as `Diff -> Tier` instead of a
path-glob string). **That power comes from F# being compiled, not from FCS specifically.** FCS
only adds the ability to load that F# from outside the compiled host at runtime, and the cost is
steep and points the wrong way:

- **It re-walks the exact trap this report is trying to remove.** `build.fsx` already *is*
  config-as-code evaluated by a script compiler at runtime; the whole thesis is to stop
  dynamically compiling logic and instead compile and test it. For a build/governance *tool*,
  "application start" is *every invocation* (`./fake.sh build -t Route`), so FCS reintroduces the
  per-run compile tax the project is paying down. (FAKE content-hash-caches compiled scripts
  precisely because this hurts.)
- **It loses the build-time guarantee.** An FSX type error fails when the tool *runs and loads
  it*, not when the tool is built. Compiled `.fs` config fails the build — strictly stronger
  enforcement.
- **FCS is a heavy, churny dependency** to carry just to read config.
- **Determinism, not security, is the live concern.** An FSX config can read files/env at load
  time, so "config" can become non-deterministic — bad for an engine whose purpose is
  reproducible verdicts. Inert data and pure compiled values are easier to reason about.

By artifact:

- **Framework-owned config that changes with the code** (`validation.contract.yml` tiers +
  routing, the target graph, the capability catalog, audit-scan patterns) → **compiled F# in the
  governance library.** Routing rules become predicates over a diff; a typo'd target name fails
  to compile. This is a small extension of the typed `Target` model the keystone already creates.
- **The consumer boundary** (generated projects need governance too) → once the consumer has a
  real build project (see the keystone), its config is a `.fs` calling the packaged engine's
  typed API — type-checked at the consumer's build time, no FCS, no file loading.
- **High-churn, agent-authored, logic-free instance data** (`tasks.deps.yml`: id → deps,
  skillist) → **keep as data**, validated by the ported F# parser. Making it FSX adds
  type-check friction to the most frequently edited file for a payoff (dangling skill/target
  refs) a validator over the data already provides.

So FCS-loaded FSX is the right tool only for "external, runtime-loaded, must-be-typed,
genuinely can't-be-compiled-into-the-build-that-reads-it" config — and once the framework and
its generated consumers both have compiled build projects, no such case remains in this repo.

---

## Consolidated verdict and direction

All four instincts are correct, and they converge on **one keystone and one policy**, not four
separate projects:

**Keystone (addresses items 2, 3, 4):** Extract a tested F# **governance library** out of
`build.fsx`. Into it go: the typed target model and MEL engine, the validation rules currently
living as prose (bucket (a) of item 4), and the ported Python graph/propagation algorithms
(item 3). The build becomes a thin front-end over it — preferably a **dedicated FAKE build
project** rather than a 4,700-line script. This single move detangles the build, kills the
tri-language evidence boundary, and converts prose rules into self-enforcing code.

**Policy (addresses item 1):** Split the development process into a **lightweight
framework-author inner loop** (build/test/surface-check, no ceremony) and the **full
consumer-grade governance**, reserving the latter for generated consumer projects and a small,
explicit dogfooding sample. Select the tier by who-is-doing-what, extending the
`validation.contract.yml` tiering the agent-consumer report already proposed.

**Plus:** generate-don't-sync the duplicated artifacts (item 5), add the missing
`.claude`↔`.agents` check today as a stopgap, and represent framework-owned config as **compiled
F# in the governance library** rather than YAML or runtime-loaded FSX (item 6) — keeping a data
format only for high-churn, agent-authored instance data.

### Is this a rewrite or a refactor?

It is a **large refactor with a rewrite at its core** (the governance library), not a
ground-up rewrite of the framework. The runtime architecture (Scene → SkiaViewer → Elmish, the
declarative boundary) is sound and every prior report agrees — leave it alone. The V3 modular
package split remains the right *future* direction but is orthogonal to these four problems and
should not be coupled to them.

### Suggested sequencing (lowest risk first)

1. **Policy split (item 1)** — pure policy + a thin tier gate. No code rewrite. Immediate relief.
2. **`.claude`↔`.agents` sync check** — one-line risk closed today.
3. **Stand up the governance library skeleton** and move the *cheapest, highest-value*
   validators (bucket (a)) into it first; build calls them in-process.
4. **Port the two Python files** into the library; reduce `run-audit.sh` to a shim.
5. **Migrate the build front-end** to a dedicated project once the library is the real home of
   the logic.
6. **Generate the duplicated artifacts** from single sources; delete the drift checks they
   replace.

Each step is independently valuable and independently revertible. None requires touching the
framework's runtime.

---

## Suggested next step

This report is analysis only. The natural follow-up — if the direction is accepted — is a
proper Spec Kit feature for step 1 (the process tier split), since that is the change that most
directly relieves the maintainer and unblocks faster iteration on everything else. The keystone
library (steps 3–5) deserves its own feature with the contract-versioning decision made up
front.
