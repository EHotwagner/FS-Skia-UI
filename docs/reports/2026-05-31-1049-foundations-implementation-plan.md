# Foundations Implementation Plan: Two-Tier Process + Governance Library

- **Date:** 2026-05-31 10:49 CEST
- **Author:** Claude Code (planning, requested by maintainer)
- **Status:** Proposed plan, partially implemented.
  - **Stage 0 IMPLEMENTED** as feature `039-foundations-baseline-spike` (all 26 tasks complete; evidence graph `ok`, evidence audit `PASS`; D2 spike **confirmed**). See [Stage 0 — Implementation status](#stage-0--implementation-status-2026-05-31-feature-039) below.
  - **Stage 2.1 IMPLEMENTED** (the `.claude`↔`.agents` byte-identity stopgap) as feature `040-foundations-capability-skills`, which shipped `SkillSyncCheck` (in-process SHA-256 sync) + `SkillExamplesCheck` (compile-verified cookbooks). The rest of Stage 2 (2.2–2.5) is not yet started. See [Stage 2 — Implementation status](#stage-2--implementation-status-2026-05-31-feature-040).
  - **Stage 3 IMPLEMENTED** as feature `041-foundations-library-validators` (the first two real validators extracted into the compiled `FS.Skia.UI.Build` library + the typed `Target` single-source DU; squash-merged to `master`, evidence audit `PASS`). See [Stage 3 — Implementation status](#stage-3--implementation-status-2026-05-31-feature-041).
  - **Stage 1 IMPLEMENTED** as feature `042-foundations-two-tier-process` (the two-tier process made authoritative and enforced via a compiled `Routing.fs` selector + a new `Route` target; the interim `select-tier.fsx` was **skipped** and Stage 5.5's `Routing.fs` pulled forward, mirroring how 041 pulled the typed `Target` work forward; evidence audit `PASS`, dogfood serialized pipeline green). See [Stage 1 — Implementation status](#stage-1--implementation-status-2026-06-01-feature-042).
  - **Stage 4 IMPLEMENTED** as feature `043-foundations-evidence-engine` (the tri-language evidence gate — `build.fsx → run-audit.sh → compute-task-graph.py + audit-status-scan.py → JSON` — replaced by compiled, unit/property-tested F# in `FS.Skia.UI.Build.Evidence`, computing graph + merge-gate audit **in-process**; byte-for-byte parity against the Stage-0 golden fixtures proven *before* the Python and `run-audit.sh` were deleted; `FS.Skia.UI.Build` packed/published and consumed by generated projects per D1; squash-merged to `main`, all 34 tasks complete, evidence audit `PASS`, zero synthetic). See [Stage 4 — Implementation status](#stage-4--implementation-status-2026-06-01-feature-043).
  - **Stages 2.2–2.5, 5–7 not yet started.** (Stage 5.5's routing migration was already pulled forward into Stage 1/feature 042.)
- **Companion analysis:** [`2026-05-31-0908-foundations-rewrite-analysis.md`](./2026-05-31-0908-foundations-rewrite-analysis.md)
- **Baseline at authoring time:** `build.fsx` = 4,688 lines; `tests/Governance.Tests/` = 34 test files; `validation.contract.yml` already defines tiers + path routing; evidence graph/audit computed by `.specify/extensions/evidence/scripts/python/*` orchestrated by `run-audit.sh` (1,284 lines) and shelled to from `build.fsx:1266,1273`.

---

## How to read this plan

The companion analysis concluded that the four foundational problems converge on **one keystone**
(a tested F# governance library extracted from `build.fsx`) and **one policy** (a two-tier
development process). This plan turns that into an executable, staged programme.

It is deliberately **incremental and reversible**. Each stage:

- delivers standalone value and can be shipped on its own,
- preserves the runtime architecture and public `.fsi` surface (never touched),
- is gated by checkable exit criteria, and
- can be reverted without unwinding later stages.

The plan does **not** rewrite the framework. It rewrites the *tooling and process around* the
framework. The runtime (`Scene → SkiaViewer → Elmish`, the declarative boundary, the eight
packages) is sound per every prior report and is explicitly out of scope.

### What already exists that we build on (do not reinvent)

| Asset | State today | This plan's use |
|---|---|---|
| `validation.contract.yml` | Already defines tiers (`inner-loop`, `focused-authority`, `agent-ready`, `maintainer-verify`, `automation-final`, `tier1`, `tier2`) and `routing_rules` by path | Stage 1 makes it *authoritative and enforced*; adds the framework-author vs consumer-agent distinction |
| `tests/Governance.Tests/` | 34 test files (~6,400+ LOC) but referencing only `src/Lib`; tests behaviours/strings, not the build's own functions | Stages 3–5 give these tests a real library to assert against |
| `scripts/build/` directory | Exists (referenced by `validation.contract.yml`) | Host for the thin build front-end if a script front-end is kept |
| MEL engine in `build.fsx` (`BuildMsg`/`BuildEffect`/`BuildModel`/`update`/`interpret`) | Sound design, untestable in a script | Stage 5 extracts it intact into the library |
| Python graph/audit (`compute-task-graph.py` 1,310 LOC, `audit-status-scan.py` 150 LOC) | Core domain logic, untyped, untested | Stage 4 ports into the library |

---

## Invariants every stage must preserve (acceptance gates that never change)

These are the standing acceptance criteria checked at the end of **every** stage. A stage is not
"done" if any regress:

1. **Public surface unchanged.** `./fake.sh build -t PackageSurfaceCheck` and `FsiTranscripts`
   pass with no baseline diff (unless a stage explicitly and separately records one).
2. **Runtime untouched.** No edits under `src/Scene`, `src/SkiaViewer`, `src/Elmish`,
   `src/KeyboardInput`, `src/Layout`, `src/Controls`, `src/Controls.Elmish`, `src/Lib` except
   where a stage names them. (None do.)
3. **Generated consumers still pass full governance.** `TemplateCheck` + `GeneratedProductCheck`
   + `GeneratedGuidanceCheck` green. The point of the plan is to lighten *framework-author*
   process, never to weaken the *consumer* contract.
4. **net10 conventions honoured.** New projects inherit `Directory.Build.props`
   (`net10.0`, `TreatWarningsAsErrors`, `FS0078`-as-error, Central Package Management). No new
   `PackageVersion` outside `Directory.Packages.props`.
5. **FAKE sequencing respected.** FAKE-backed validation runs in the deterministic serialized
   order from `CLAUDE.md`/`AGENTS.md`; never concurrently.
6. **Output parity for evidence.** Any ported gate (Stage 4) produces the *same status
   vocabulary and counts* (`accepted-seh-tasks`, `unaccepted-synthetic-tasks`,
   `auto-synthetic-tasks`, `late-seh-tasks`) as the Python it replaces, proven by a golden-output
   diff before the Python is deleted.

### The standard per-stage validation command sequence

Unless a stage overrides it, the exit-gate command sequence is the canonical serialized order:

```
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

---

## Stage dependency overview

```
Stage 0  Foundations, baselines, decisions  ── + ── Stage 3.1 spike (library reference)   [GATE: do these first — D5]
   │
   │  (spike outcome confirms D2 dedicated-project form, or triggers the thin-fsx fallback)
   │
   ├──> Stage 1  Two-tier process (policy + enforcement)        [independent; ship early, in parallel]
   │
   ├──> Stage 2  Single-source generation (.claude/.agents, constitution, skillist)
   │
   └──> Stage 3  Governance library skeleton + cheap validators (continues from the 3.1 spike)
                    │
                    ├──> Stage 4  Port Python graph/audit into the library
                    │
                    └──> Stage 5  Dedicated build front-end + extract MEL engine + typed targets
                                     │
                                     └──> Stage 6  Codify remaining rules, trim prose, evidence hygiene, version the contract
                                                      │
                                                      └──> Stage 7  Decommission, measure, document the new normal
```

**Sequencing (D5 resolved): Stage 0 + the Stage-3.1 spike go first**, before committing effort to
the rest. The spike de-risks the one technical unknown — whether the dedicated FAKE build project
(D2) can cleanly reference the compiled library — and either confirms D2 or activates the thin-fsx
fallback. Once that is settled, Stage 1 and Stage 2 (independent of the library track) ship early
and in parallel with the continuing library track (3→4→5→6).

---

## Stage 0 — Foundations, baselines, and decisions

**Goal:** Make the programme measurable and safe before changing anything. Capture the "before"
numbers, lock the decisions that shape later stages, and establish the meta-process (this work is
itself framework development and must run under the new light tier, not the old ceremony).

**Why now:** Every later stage claims a reduction (lines, languages, prose). Without a captured
baseline those claims are unverifiable, and "did we regress evidence output?" is unanswerable.

**Dependencies:** none.

### Work items

0.1 **Capture quantitative baseline** into `docs/reports/_baselines/2026-05-31-foundations.md`:
   - `build.fsx` line count (4,688) and a function-level breakdown (orchestration vs validation).
   - Governance Markdown line counts: `.claude/skills` + `.agents/skills` (byte-identical today),
     `.specify/memory/constitution.md`, templates, total `specs/**` lines.
   - Language LOC: F# vs Bash vs Python (use `git ls-files | grep` + `wc`).
   - Per-feature ceremony time estimate (current: ~12–14h/feature).
   - Golden evidence outputs: run `EvidenceGraph` + `EvidenceAudit` on the current feature and on
     two historical features; archive `task-graph.json`, `task-graph.md`, and the audit count
     block as fixtures under `tests/Governance.Tests/fixtures/evidence-golden/`. **These become
     the Stage 4 parity oracle.**

0.2 **Record architecture decisions** as ADRs under `docs/adr/` (the template-framework report
   already anticipated `docs/adr/`):
   - **ADR: Governance library placement and distribution — DECIDED (D1): published library
     package `FS.Skia.UI.Build`.** The repo's build front-end project-references it in-solution;
     generated consumers package-reference the published version. One port serves both and keeps
     consumers lean (no source copies). Cost: one more package to pack/version in the release flow.
   - **ADR: Build front-end form — DECIDED (D2): dedicated FAKE build project (`build/Build.fsproj`,
     `dotnet run`).** Compiled, IDE-grade tooling, references the library directly, and `dotnet
     run` builds the whole project graph (no DLL bootstrap-order wrinkle). The Stage-3.1 spike must
     confirm the FAKE Target API works cleanly from a normal compiled exe; the thin-`build.fsx`-shim
     remains the documented fallback **only if** the spike surfaces a blocker.
   - **ADR: Contract versioning** — the generated-product contract (the ~800 lines of structural
     checks) gets a `schema_version` and a deprecation window. Define the policy now.
   - **ADR: Spec Kit fork stance — DECIDED (D4): accept full F# ownership.** Porting the Python
     forks us from upstream Spec Kit; the `extensions/`+`presets/` customisation already did so in
     practice. We own the evidence toolchain in typed, tested, in-process F# and forgo an upstream
     merge path.
   - **ADR: Configuration representation** — framework-owned config (the `validation.contract.yml`
     tiers + routing, the capability catalog, audit-scan patterns) is represented as **compiled F#
     values and functions in the governance library**, *not* YAML and *not* runtime-loaded `.fsx`
     via FSharp Compiler Services. Compiled F# fails at *build* time, has zero per-run compile
     cost, can reference the typed `Target` model (a mistyped target name won't compile), and lets
     routing rules be predicates (`Diff -> Tier`) rather than path-glob strings. FCS-loaded FSX is
     explicitly rejected: it pushes errors to load time, re-introduces the per-invocation compile
     tax this whole programme removes, and can do non-deterministic IO at load. A **data** format
     (YAML/JSON) is retained *only* for high-churn, agent-authored, logic-free instance data
     (`tasks.deps.yml`), validated by the ported F# parser. (See companion analysis §6 and Open
     Decision D6.)

0.3 **Establish the meta-process** (this is the dogfooding fix applied to the plan itself):
   - This programme's own features run under the **framework-author inner loop** (Stage 1's
     light tier): `Dev` + surface check only, *except* Stage 1 and Stage 6 which touch
     governance/consumer contracts and therefore escalate to the full gate set.
   - Designate Stage 1 and Stage 4 as the **named dogfood features** that intentionally exercise
     the full Spec Kit + evidence pipeline, keeping the harness honest.

### Exit criteria

- Baseline file committed with all counts and the golden evidence fixtures.
- All five ADRs written, recording the decisions already resolved with the maintainer
  (D1, D2, D4, D6) and the contract-versioning policy. (See the resolved-decisions section below.)
- No code changed (Stage 0 is read + document only). `Dev` still green.

### Risks & mitigations

- *Risk:* baseline drifts before later stages run. *Mitigation:* baseline is a point-in-time
  snapshot with the git SHA recorded; later stages compare against it, not against a moving tree.

**Effort:** ~0.5–1 day. **Revert:** delete the docs; nothing else touched.

### Stage 0 — Implementation status (2026-05-31, feature 039)

**Status: COMPLETE.** Stage 0 was implemented as feature
`039-foundations-baseline-spike` (branch `039-foundations-baseline-spike`,
pinned commit `34faf1ed61ec0ec2a8a2a81168517cb5ccf499d1`). All 26 tasks are
`[X]`; `speckit.evidence.graph` returns `verdict=ok` (26 tasks, 0 errors/cycles)
and `speckit.evidence.audit` returns `verdict=PASS` (0 unaccepted-synthetic, 0
auto-synthetic, 0 late-seh, 0 diff-scan, 0 readiness-contract blocking). No
synthetic evidence was used. Toolchain: dotnet `10.0.300`, FAKE `6.1.4`, python
`3.14.5`.

**0.1 Baseline captured** → `docs/reports/_baselines/2026-05-31-foundations.md`
(SHA-pinned, each metric paired with its reproduction command):

| Metric | Value (committed corpus at pinned SHA) |
|---|---|
| `build.fsx` total | **4,688** lines |
| `build.fsx` orchestration vs validation | 45 `StartTarget` dispatch cases (orchestration) · 22 `Validate*` functions (validation), reported as marker counts; per-line attribution deferred to Stage 4 |
| `.claude/skills` ↔ `.agents/skills` mirror | 19 ↔ 19 files, **5,854** combined lines |
| `.specify/memory/constitution.md` | 336 lines |
| Templates (`.specify/templates` + preset templates) | 1,508 lines |
| `specs/**/*.md` | 773 files, **58,880** lines |
| F# (`.fs`/`.fsi`/`.fsx`) | 191 files, **44,398** LOC |
| Bash (`.sh`) | 17 files, 611 LOC |
| Python (`.py`) | 2 files, **1,460** LOC (the evidence engine — the Stage 4 port target) |
| Feature dirs under `specs/` | 40 |
| Per-feature ceremony estimate | ~12–14 h (carried from this plan; labelled an estimate, exempt from the measurement-command rule) |

   - **Golden fixtures** committed under
     `tests/Governance.Tests/fixtures/evidence-golden/<feature>/`
     (`task-graph.json` + `task-graph.md` + `audit-counts.txt`), proven
     byte-for-byte reproducible (SHA-1) from the existing Python engine —
     **the Stage 4 parity oracle**.
   - **Substitution (recorded per FR-003):** the plan named three sources
     (current 038, plus two historical). `017-synthetic-error-evidence` does
     **not** produce a stable evidence output at the pinned SHA — its graph
     compute fails (`exit 3`, `verdict=error`) because its skilled tasks lack a
     committed `readiness/skill-loading-evidence.md`, so the audit halts before
     a count block. Per the substitution rule it was replaced by
     **`036-archive-readiness-api-docs`**, which passes graph compute *and*
     carries an accepted-`[SEH]` task (`accepted-seh-tasks=1`, T005), preserving
     the synthetic-propagation coverage 017 was chosen for. Final source set:
     `038-authoring-guidance-consistency` (current), `037-authoring-audit-robustness`,
     `036-archive-readiness-api-docs`. (Coverage gap noted in the fixtures
     README: no stable source exercises `[S*]` auto-synthetic / unaccepted
     counts — a follow-up.)

**0.2 ADRs written** → `docs/adr/0001..0005-*.md` (D1 governance-library
placement & distribution, D2 build front-end form, contract-versioning policy,
D4 Spec Kit fork stance, D6 configuration representation), each with
decision / alternatives / rationale / stages-shaped.

   - **D2 spike — confirmed.** The de-risking spike stood up the two compiled
     projects that Stage 5 builds on, and recorded the outcome in
     `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md`:
     - `build/Governance/FS.Skia.UI.Build.fsproj` — governance-library skeleton
       with a curated `Spike.fsi` (`val run : unit -> string`) per Principle II.
     - `build/Build.fsproj` (Exe) — dedicated FAKE front-end whose `Program.fs`
       registers one `SpikeHello` target via `Fake.Core.Target` delegating
       **only** to `FS.Skia.UI.Build.Spike.run` (no inlined logic), dispatched
       via `Target.runOrDefaultWithArguments`.
     - Both compile clean under `net10.0`/`TreatWarningsAsErrors` (0/0);
       `dotnet run --project build/Build.fsproj -- SpikeHello` prints the
       library's success line; and `dotnet list build/Build.fsproj package
       --include-transitive` shows **no `FSharp.Compiler.*`** (FR-012 satisfied
       — the FAKE Target API works from a normal compiled exe with no FSX
       runner / FCS). Reproduced by the committed `build/spike-verify.sh`
       (`SPIKE-VERIFY PASS: D2 confirmed`). **The thin-`build.fsx` fallback is
       not needed.**
     - Dependency wiring: `Fake.Core.Target 6.1.4` added centrally to
       `Directory.Packages.props` (build-tooling only; transitively brings the
       minimal `Fake.Core.*` companions; **not** shipped in any generated
       product) with a matching row in `docs/reports/dependencies.md`. Both
       projects added to `FS-Skia-UI.sln` additively (32 → 36 project entries).

**0.3 Meta-process established** — recorded in
`specs/039-foundations-baseline-spike/plan.md §Programme Meta-Process` (the
single discoverable place, cross-linked from the baseline): default
framework-author light tier for foundations features, with Stage 1 and Stage 4
named as the full-pipeline dogfood features.

**Invariants held.** Runtime untouched: `git diff` over `src/**` = 0 changes,
no runtime `.fsi` changed, `PackageSurfaceCheck` shows no baseline diff (SC-006).

**No-regression caveat (honest disclosure).** In the serialized FAKE gate run,
`Dev`, `GeneratedGuidanceCheck`, `GeneratedProductCheck`, `DependencyReport`,
`TemplateDrift`, and `PackageSurfaceCheck` are **green**; the two readiness
gates (`EvidenceGraph`/`EvidenceAudit`) PASS. **Two gates are RED for
pre-existing, feature-independent reasons**, proven via a stash control (they
fail identically with all of feature 039's edits stashed): `FsiTranscripts`
(`scripts/controls-prelude.fsx` exits 1 on this toolchain) and `TemplateCheck`
(its `Test` target hits the known `SkiaViewer.Tests` headless flake). Both are
runtime/environment-side, out of scope per the runtime-untouched invariant; full
detail in `specs/039-foundations-baseline-spike/readiness/logs/no-regression.md`.
A one-time FAKE-runner paket-cache gap was resolved by restoring the
`build.fsx.lock` "Main" group into the NuGet cache and clearing `.fake` (no
target behaviour changed).

**Stage 0 exit criteria: met** — baseline + golden fixtures committed, all five
ADRs written, runtime unchanged, evidence graph/audit green.

---

## Stage 1 — Two-tier development process (policy + enforcement)

**Goal:** Stop applying consumer-grade governance to routine framework changes. Make
`validation.contract.yml`'s already-present tiers *authoritative and enforced*, and add the
missing **framework-author vs consumer-agent** axis so the tier is chosen by *who is changing
what*, not only by changed paths.

**Why now / first:** Highest immediate relief, zero rewrite. It is pure policy plus a thin gate
that reads a file that already exists.

**Dependencies:** Stage 0 (ADRs + baseline). Independent of the library track.

### The problem precisely

`validation.contract.yml` already lists `inner-loop` and the escalation tiers, and `routing_rules`
already map paths → required gates. But nothing *selects* a tier and *enforces only that tier's
gates* for a given change; today the de-facto default is "run the full serialized six-target
order on everything," which is the suffocation. The framework-author path is undefined.

### Work items

1.1 **Define the framework-author inner loop.** Add a `developer_class` axis
   (`framework-author` | `consumer-agent`):
   - `framework-author` default tier = `inner-loop` (gates: `Dev` + a new lightweight
     `SurfaceCheck` composite = `PackageSurfaceCheck` only when `.fsi` changed).
   - `consumer-agent` and any change matching `template/**`, `.specify/**`, `validation.contract.yml`,
     governance paths, or `src/**/*.fsi` **escalate** to the existing focused/agent-ready tiers.
   - **Config-representation note (per the Stage-0 ADR / analysis §6):** the end-state for this
     contract is **compiled F#** in the governance library, not YAML. Because Stage 1 ships before
     the library exists, it reads the *existing* `validation.contract.yml` as a pragmatic interim
     — but **do not grow the YAML schema**. Keep the `developer_class`/`inner-loop` additions
     minimal; they are scaffolding to be superseded by `Routing.fs` (Stage 5), where tiers and
     routing rules become typed values and predicates. Treat any temptation to add rich YAML
     parsing as a signal to pull the library work forward instead.

1.2 **Add a tier-selection gate** — a small, deterministic command that, given the working-tree
   diff, prints the required tier and gate list and (in `--enforce` mode) fails if a
   higher-tier change is being shipped without its gates' evidence artifacts present. Two
   implementation options:
   - *Interim (Stage 1):* a `scripts/build/select-tier.fsx` (`dotnet fsi`) that reads the existing
     `validation.contract.yml` + `git diff --name-only` and emits the tier + gates. Deliberately
     thin — it is throwaway scaffolding.
   - *Final (Stage 5):* the contract *is* compiled F# (`Routing.fs`); the same logic becomes a
     typed function in the governance library exposed via the `Route` target, and
     `select-tier.fsx` is deleted. No FCS / runtime script loading at any point — the interim uses
     `dotnet fsi` only as a stopgap reader, and it is removed, not promoted.

1.3 **Add a `Route` FAKE target** (`./fake.sh build -t Route`) that runs the selector and prints
   the minimal gate set for the current diff. This becomes the agent's and maintainer's entry
   point: "what must I run for this change?" replaces "read the prose and guess."

1.4 **Update agent-facing guidance** (`CLAUDE.md`, `AGENTS.md`) to say: *run `Route` first; run
   only the gates it prints.* Replace the blanket "serialized six-target order" instruction with
   "that order is the `maintainer-verify`/escalated path; routine framework work uses `Route`."

1.5 **Mark dogfood features.** Add a `dogfood: true` marker convention for the small set of
   features that must run the full pipeline regardless of tier, so the harness stays exercised.

### New / changed artifacts

- `validation.contract.yml` (minimally extended with `developer_class` + `inner-loop`; interim
  only — superseded by compiled F# in Stage 5).
- `scripts/build/select-tier.fsx` (interim selector; deleted in Stage 5).
- `build.fsx` (+ a `Route` target wired into the existing dynamic target list at
  `requiredTargets`, lines 548–584, and metadata at `targetMetadata`).
- `CLAUDE.md`, `AGENTS.md` (guidance rewrite).
- `docs/reports/build.md` and `docs/reports/speckit.md` (document the tiered process).

### Exit criteria

- `./fake.sh build -t Route` on a pure `src/Scene/*.fs` change prints `inner-loop` → `Dev`
  (+ surface check only), **not** the full six-target set.
- `./fake.sh build -t Route` on a `template/base/**` change prints the escalated gate set
  (`TemplateCheck`, `GeneratedProductCheck`, …).
- `Route --enforce` fails a simulated `.fsi` change that lacks `readiness/package-surface-expectations.md`.
- A Governance.Tests test asserts tier-selection for ≥6 representative diffs (this is a string/IO
  test today; Stage 3 upgrades it to call the typed selector).
- Invariants 1–6 hold.

### Risks & mitigations

- *Risk:* the light tier silently becomes the only tier and the harness rots. *Mitigation:*
  named dogfood features (1.5) + a CI scheduled run of the full pipeline on the dogfood set.
- *Risk:* tier mis-selection lets a consumer-affecting change ship under-validated. *Mitigation:*
  `routing_rules` already err toward escalation for `template/**`, `.specify/**`, `src/**/*.fsi`;
  default-deny for unknown paths (the contract already has `unknown_gate_rejection`).

**Effort:** ~2–3 days. **Revert:** drop the `Route` target + selector; revert the two guidance
files; the contract additions are inert if unused.

### Stage 1 — Implementation status (2026-06-01, feature 042)

**Status: IMPLEMENTED.** Stage 1 shipped as feature
`042-foundations-two-tier-process` (branch `042-foundations-two-tier-process`).
All 30 tasks are `[X]` with **real evidence** (zero synthetic);
`speckit.evidence.graph` returns `verdict=ok` and `speckit.evidence.audit`
returns **verdict=PASS** (0 unaccepted-synthetic, 0 auto-synthetic, 0 diff-scan,
0 readiness-contract blocking). As a designated **dogfood** feature (FR-015) it
ran the full serialized FAKE gate sequence for itself — `Dev` →
`GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
`EvidenceGraph` → `EvidenceAudit`, all green — plus the Governance.Tests suite
(312/312). Runtime untouched: `git diff` over `src/**` = 0 changes; no product
`.fsi`, surface baseline, or `PackageVersion` change (SC-009/FR-012/FR-013).

**Key deviation from the staged plan (the central design decision, resolved with
the maintainer):** because `FS.Skia.UI.Build` now exists (039/040/041), the
interim `scripts/build/select-tier.fsx` + grown-YAML path in work items 1.1–1.2
was **skipped entirely**. Stage 5.5's `Routing.fs` was **pulled forward** — the
tier/routing policy is **compiled F#** from the start (mirroring how 041 pulled
the typed `Target` work forward). No `select-tier.fsx`, no `dotnet fsi` selector,
and no `FSharp.Compiler.*` dependency was ever introduced (SC-006, verified by
grep). This means the bulk of what Stage 5.5 prescribed is **already done**;
Stage 5's remaining scope is the MEL-engine relocation / `build.fsx` retirement,
not the routing migration.

What landed against the Stage-1 work items:

- **1.1 framework-author inner loop — done, as compiled F#.** `Routing.fs` ships
  typed `DeveloperClass` (`FrameworkAuthor` default | `ConsumerAgent`), `Tier`
  (the five authoritative tiers + retained `Tier1`/`Tier2` aliases) with a total
  `tierRank`, and the rule table as glob predicates over a `Diff`. `inner-loop`
  gates = `[Dev]` only; a public `src/**/*.fsi` edit **escalates** via the
  `package-surface` rule rather than adding a check to inner-loop. The
  `ConsumerAgent` floor raises the base tier to `focused-authority`;
  consumer-contract **paths** escalate regardless of class. `template/**` and
  `.specify/**` were broadened (F2) for full consumer-contract coverage. The YAML
  schema was **not** grown — the contract became typed F# as the ADR intended.
- **1.2 tier-selection gate — done, compiled (no interim FSX).** `select`
  (default-deny unmatched → `Verify`; `maxBy tierRank` escalation;
  registry-order gate de-dup) and `selectForFeature` (dogfood override) are
  **pure**; `unmetArtifacts`/`enforceDiagnostic` back `--enforce`. Git union-diff
  (`merge-base HEAD master`…`HEAD` ∪ `status --porcelain --untracked-files=all`),
  `File.Exists`, and printing stay at the `build.fsx` interpreter edge
  (Principle IV), so the selector is unit-testable without git.
- **1.3 `Route` FAKE target — done.** An additive `Targets.Route` case (DU +
  derived metadata + dispatch wiring; no existing target moved) runs the selector
  in-process and prints `developer-class` / `tier` / `gates`. `--enforce` exits
  non-zero naming each missing artifact and the requiring tier.
- **1.4 guidance rewrite — done.** `CLAUDE.md` and `AGENTS.md` now lead with "run
  `Route` first; run only the gates it prints" and reframe the serialized
  six-target order as the escalated `maintainer-verify` path (no longer the
  unconditional default); `SequentialFakeGuidanceTests.fs` asserts both.
  `docs/reports/build.md` and `docs/reports/speckit.md` document the tiers, the
  developer-class axis, how `Route` selects, and `--enforce` (FR-009).
- **1.5 dogfood marker — done, as typed policy.** The dogfood set is a typed
  `dogfoodFeatureIds` list in `Routing.fs` (ADR D6), including `"042"`;
  `isDogfood` matches the leading numeric segment of the active feature slug, so
  `Route` forces the full pipeline for feature 042 (`dogfood-forced=true`,
  gate set = `fullPipelineGates`) even on a would-be inner-loop diff.
- **Single source of truth (work the plan deferred to 5.5, delivered now).**
  `validation.contract.yml` is **retained but generated** from `Routing.fs` via
  `ContractView.render` (the single emitter), so its existing consumers
  (`build.fsx`, the `TargetMetadataDrift` reference check, `AgentReady`, and the
  feature-028 `src/Lib/AgentValidation.fs` parser) keep reading a coherent file.
  Regeneration folds into `RefreshSurfaceBaselines`; the pure
  `ContractView.currencyDrift` currency check folds into `TargetMetadataDrift`,
  so drift is structurally impossible (a hand-edit fails with a "regenerate from
  `Routing.fs`" diagnostic). Demonstrated live: accept(0) → hand-edit reject(1) →
  regenerate(0) → re-accept(0).

**Tests (SC-004 / FR-010).** `RoutingTests.fs` adds 14 typed-selector cases
(inner-loop, empty-diff default, `.fsi` escalation, `template/base/**`,
`.specify/**`, mixed-diff highest-tier, unknown-path default-deny, the
`ConsumerAgent` floor, the F2 broadened coverage, the dogfood override, and the
`--enforce` core); `ContractViewTests.fs` adds the `currencyDrift` None/Some
cases. All assert typed `Selection` values, not strings.

**Invariants held.** 1 (public surface unchanged), 2 (runtime untouched), 3
(generated consumers still pass `TemplateCheck`/`GeneratedProductCheck`/
`GeneratedGuidanceCheck`), 4 (net10 conventions; no new `PackageVersion`), 5
(FAKE sequencing — gates run serially, never concurrently), 6 (evidence output
parity — graph/audit vocabulary and counts unchanged) all hold.

**Stage 1 exit criteria: met** — `Route` routes a routine framework change light
(typed-selector evidence; the live `Route` is dogfood-forced on this very
feature, so the controlled inner-loop result is proven through the same pure
`Routing.select` the spec's Independent Test names), escalates consumer-contract
changes, `--enforce` blocks an under-evidenced escalated change, ≥6
Governance.Tests cases assert tier selection (delivered as typed assertions, the
Stage-3 upgrade the plan anticipated — already realized since the library
exists), and invariants 1–6 hold. The Stage-1 exit criterion that the
Governance.Tests selector test would be "string/IO today, Stage 3 upgrades it to
typed" is satisfied directly: the test is typed from the start.

---

## Stage 2 — Single-source generation (kill the hand-synced duplicates)

**Goal:** Replace every "two copies the build checks for drift" with "one source the build
generates from." Eliminates the largest duplication class and the silent-drift risk.

**Why now:** Independent of the library; removes ~6,000 lines of duplication and closes the
unguarded `.claude`↔`.agents` drift hole (a live risk today — no check enforces their identity).

**Dependencies:** Stage 0. Parallelizable with Stage 1.

### Work items

2.1 **Immediate stopgap — `.claude`↔`.agents` sync check.** Add a FAKE-callable check (and a
   `Governance.Tests` test) asserting the 19 `SKILL.md` files are byte-identical across
   `.claude/skills/` and `.agents/skills/`. This is a one-day risk closed in minutes; do it
   before the generation work lands.

2.2 **Pick the single source for skills.** Decide canonical = `.agents/skills/` (the Codex source
   per `CLAUDE.md`) and **generate** `.claude/skills/` from it (copy-with-manifest, or a
   symlink-tree where the platform allows). Add a `GenerateAgentSkills` target; the sync check
   (2.1) becomes a verification that generation is current.

2.3 **De-duplicate the constitution echo.** `.specify/memory/constitution.md` content is
   paraphrased into `plan-template.md`, `tasks-template.md`, and generated plans. Replace the
   echoes with **fragment includes** generated from the single constitution source, or reduce the
   templates to *reference* the constitution rather than restate it.

2.4 **Single-source the skillist.** Today each task's skillist lives in both `tasks.md` and
   `tasks.deps.yml` and the build checks they match. Make `tasks.deps.yml` canonical and
   **render** the `tasks.md` `[skillist: …]` annotations from it (or vice-versa). This pairs with
   Stage 4 (the graph tool already parses both).

2.5 **Replace drift-checks with generation-currency checks.** Wherever a target previously
   asserted "A matches B," change it to "B is freshly generated from A" (regenerate to a temp,
   diff, fail if stale). A stale-generation failure is actionable ("run `GenerateX`"); a
   drift failure is not.

### New / changed artifacts

- New targets: `SkillSyncCheck` (interim), `GenerateAgentSkills`, possibly `GenerateTaskMarkup`.
- Generation provenance header in generated files (`<!-- generated from … ; run GenerateAgentSkills -->`).
- Governance.Tests: identity/currency tests.

### Exit criteria

- Editing one skill source and *not* regenerating fails `SkillSyncCheck` with a "run
  `GenerateAgentSkills`" diagnostic.
- `.claude/skills` is reproducible from `.agents/skills` (bit-identical after generation).
- Constitution text exists in exactly one place; templates reference or include it.
- Baseline duplication count (Stage 0) drops by the eliminated lines; record the delta.
- Invariants 1–6 hold.

### Risks & mitigations

- *Risk:* generation breaks an agent that reads the now-derived file mid-edit. *Mitigation:*
  generated files are committed (not gitignored) so the working tree is always coherent; the
  check enforces currency at gate time.
- *Risk:* symlinks unsupported on a contributor's platform. *Mitigation:* default to
  copy-generation, not symlinks; cross-platform by construction.

**Effort:** ~3–4 days. **Revert:** generation targets are additive; the sync check can be removed;
generated files were already present so nothing is lost.

### Stage 2 — Implementation status (2026-05-31, feature 040)

**Status: 2.1 COMPLETE; 2.2–2.5 not started.** Feature
`040-foundations-capability-skills` shipped the Stage-2.1 stopgap and the
capability-skill cookbooks Stage 3 consumes:

- **2.1 `.claude`↔`.agents` sync check — done.** `SkillSyncCheck` compares the
  two skill trees by in-process SHA-256 byte-identity (no `diff`/`cmp`/`sha256sum`
  shelling) and fails on drift; `SkillExamplesCheck` tangles every ` ```fsharp `
  block out of the six capability skills and compiles them against the pinned
  adopt-set. Both are compiled `build/Governance` modules (`SkillSync.fs`,
  `SkillExamples.fs`, each with a curated `.fsi`) `#load`'d into `build.fsx` —
  the same in-process pattern Stage 3 reuses.
- **Not yet done:** 2.2 `GenerateAgentSkills` (the trees are still hand-synced and
  only *checked*, not generated from one source), 2.3 constitution de-dup,
  2.4 skillist single-source, 2.5 drift-checks → generation-currency checks.

---

## Stage 3 — Governance library skeleton + cheapest high-value validators

**Goal:** Stand up `FS.Skia.UI.Build` (name per ADR D1) as a real, compiled, unit-tested F#
library and move the *cheapest, highest-value* validators out of `build.fsx` into it. Prove the
extraction pattern end-to-end on a small slice before committing to the big moves.

**Why now:** This is the keystone's first brick. It de-risks Stages 4–5 by establishing the
project wiring, the test harness, and the build-calls-library-in-process pattern on low-risk
logic first.

**Dependencies:** Stage 0 (placement ADR). Independent of Stages 1–2.

### Work items

3.1 **Create the library project.** `build/Governance/FS.Skia.UI.Build.fsproj` (or `src/` per ADR),
   `net10.0`, inheriting `Directory.Build.props`. Add to `FS-Skia-UI.sln`. Reference from
   `tests/Governance.Tests/Governance.Tests.fsproj` (which today references only `Lib`).

3.2 **Define the typed core** (modules, each ≤ a few hundred lines, all unit-tested):
   - `Targets.fs` — a `Target` discriminated union replacing the stringly-typed target names
     (`requiredTargets`, lines 548–584) and a typed dependency graph replacing
     `targetDependencyRows` (lines 586–644). One source of truth for target identity + deps +
     metadata, eliminating the metadata "second source of truth" the build currently
     drift-checks.
   - `Paths.fs` — the path model from `BuildModel` (lines 361–398) as a typed record builder.
   - `Findings.fs` — the `ValidationFinding` type + a uniform finding/result type so every
     validator returns structured results instead of ad-hoc strings.
   - `Config.fs` — the home for framework-owned config **as compiled F# values** (per the
     Stage-0 config-representation ADR / analysis §6): the capability catalog and audit-scan
     patterns become typed values here rather than parsed YAML. The `validation.contract.yml`
     tiers/routing follow in Stage 5 (`Routing.fs`). No FCS, no runtime script loading — config
     is data-typed-as-code, checked by the compiler and unit-testable like any other module.

3.3 **Move the first validators** (chosen for low coupling + high duplication payoff):
   - **Target-metadata drift** (`ValidateTargetMetadataDrift`, build.fsx ~865–879 and ~548–1030).
     Becomes a pure function over the typed `Target` graph; the drift *disappears* because
     metadata is derived, not duplicated. ~480 lines of build.fsx logic → tested module.
   - **Capability catalog** (build.fsx ~2244–2361). Per the config-representation ADR, represent
     the catalog as **compiled F# values** in `Config.fs` (a typed `CapabilityRow` list) and
     retire the hand-rolled YAML parser entirely; `validateCapabilityRows` becomes a pure
     function over those values with real error types. If a YAML form must persist briefly for an
     external reader, parse it with `YamlDotNet` (already a managed dependency) *behind* the typed
     model and schedule its retirement — do not keep the bespoke parser. ~500 lines → tested
     module + typed config.
   - **`.claude`/`.agents` sync + skillist presence** (folds in Stage 2's checks as typed
     functions).

3.4 **Wire the build to call the library in-process.** The relevant `interpret` cases (build.fsx
   ~4627–4667: `CapabilityCatalogCheck`, target-metadata cases) now call
   `FS.Skia.UI.Build.<module>.<fn>` instead of inline code. Net effect: build.fsx shrinks; logic
   is tested.

3.5 **Upgrade Governance.Tests.** The 34 existing files test behaviours/strings because they
   could not reference build internals. Re-point the moved-logic tests
   (`SkillValidationTests.fs`, `TemplateDriftTests.fs`, target-metadata assertions) at the real
   library functions, adding direct unit tests with fixtures.

### New / changed artifacts

- `build/Governance/FS.Skia.UI.Build.fsproj` + `Targets.fs`, `Paths.fs`, `Findings.fs`,
  `Config.fs`, `TargetMetadata.fs`.
- `FS-Skia-UI.sln` (+1 project), `Governance.Tests.fsproj` (+ project reference).
- `build.fsx` shrinks by the moved logic (target: −~900 lines this stage).

### Exit criteria

- `dotnet build build/Governance/FS.Skia.UI.Build.fsproj` clean under `TreatWarningsAsErrors`.
- New unit tests for the three moved validators pass; they assert *typed* error cases, not
  string-matching.
- `CapabilityCheck`, `TargetMetadata`, `TargetMetadataDrift` targets produce **identical**
  reports to baseline (Stage 0 golden) — proven by report diff.
- `build.fsx` line count reduced ≥ 800 vs baseline; recorded.
- Invariants 1–6 hold; the full serialized gate sequence is green.

### Risks & mitigations

- *Risk:* FAKE script can't reference the compiled library (script-runner classpath).
  *Mitigation:* `build.fsx` `#r`s the built DLL (or, if brittle, this is the trigger to bring
  Stage 5's dedicated front-end forward). Validate this path *first* in 3.1 as a spike.
- *Risk:* YAML parser swap changes behaviour on edge-case catalogs. *Mitigation:* golden-diff the
  parsed model against the hand-rolled parser on all existing catalogs before deleting the old
  parser.

**Effort:** ~5–7 days. **Revert:** the library is additive; `interpret` cases can fall back to the
inline code (kept behind a flag until the golden diff is clean, then deleted).

### Stage 3 — Implementation status (2026-05-31, feature 041)

**Status: IMPLEMENTED** (squash-merged to `master` as `ca0f5b5`; version bump
`5ddcdc7`). Feature `041-foundations-library-validators` moved the two
cheapest/highest-value validators into the compiled `FS.Skia.UI.Build` library
and introduced the typed `Target` single-source. `speckit.evidence.audit`
returns **verdict=PASS** (0 unaccepted-synthetic, 0 auto-synthetic, 0 diff-scan,
0 readiness-contract blocking); the serialized gate set (`Dev`,
`GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
`EvidenceGraph`, `EvidenceAudit`) is green. **Zero synthetic evidence.**

What landed against the Stage-3 work items:

- **3.1 library reference — done** (the `build/Governance/FS.Skia.UI.Build.fsproj`
  skeleton from the 039 spike / 040 is now `#load`'d into `build.fsx` and
  project-referenced by `Governance.Tests`).
- **3.2 typed core — partial (as scoped):** `Targets.fs` (a 40-case `Target` DU +
  total `spec` from which `requiredTargetNames`/`targetDependencyRows` are
  **derived**, replacing the stringly-typed registries) and `Findings.fs` (the
  uniform `ValidationFinding` + `finding`/`renderDetail`) shipped. **`Paths.fs`
  deferred:** the `BuildModel` path machinery and `focusedGateContract` stay at
  the `build.fsx` interpreter edge per research R3 (Principle IV; relocation is
  Stage 5). **`Config.fs`-as-compiled-F#-values deviated (recorded):** per the
  041 clarification/R4 the capability catalog is read through **`YamlDotNet`
  behind the typed model** (`Capabilities.fs`) — the YAML file is retained as the
  data source consumed by template generation — rather than inlined as compiled
  F# values. The bespoke hand-rolled YAML parser is deleted; D6's
  compiled-F#-config end-state for the catalog is left for a later stage.
- **3.3 first validators — done:** target-metadata drift (`TargetMetadata.fs`,
  pure `validateMetadataDrift`/`validateAgainstRepo`) and the capability catalog
  (`Capabilities.fs`, pure `validateRows` with the surface-baseline probe
  injected). The drift becomes structurally unrepresentable because metadata is
  derived from the closed `Target` DU. The `.claude`/`.agents` sync folded in
  earlier via 040.
- **3.4 in-process wiring — done:** the `CapabilityCheck` / `TargetMetadata` /
  `TargetMetadataDrift` interpret cases call the library; **all** `StartTarget`
  arms now dispatch on the typed `Targets.Target` and FAKE registration is driven
  off `Targets.dispatchTargets` (a renamed target is a compile error, SC-003).
- **3.5 Governance.Tests upgraded — done:** 3 new suites (`TargetMetadataTests`,
  `CapabilityCatalogTests`, `ReportParityTests`) assert **typed** findings (8
  cases) and byte-identical report parity; the source-scan command/dependency
  contract tests were re-pointed at the typed `Targets` values.

**Exit-criterion variance (honest disclosure):** the "`build.fsx` line count
reduced ≥ 800" criterion was **not met** — realized shrink is **385** (4,839 →
4,454). The bulk of the target-metadata code is `focusedGateContract` +
`BuildModel` path machinery, which R3 deliberately keeps at the edge (moving it
is the out-of-scope Stage-5 MEL-engine relocation); the ≥800 figure over-counted
Stage-3's extractable surface. Recorded as `[F]` on task T019 with diagnostics in
`specs/041-foundations-library-validators/readiness/build-fsx-line-delta.md`; not
padded. All other exit criteria (clean `TreatWarningsAsErrors` build, typed
unit tests, golden-diff report parity = 0 bytes, invariants 1–6) hold. The
remaining Stage-3 reduction is realized when Stage 5 relocates the engine.

---

## Stage 4 — Port the Python graph/audit into the library

**Goal:** Replace `compute-task-graph.py` (1,310 LOC) and `audit-status-scan.py` (150 LOC) with
typed, unit-tested F# in `FS.Skia.UI.Build`, and compute the evidence graph/audit **in-process**
instead of shelling `build.fsx → run-audit.sh → python → JSON → re-parse`.

**Why now:** The library skeleton (Stage 3) proved the pattern; this is the highest-value port
(the flagship gate) and collapses the tri-language boundary.

**Dependencies:** Stage 3 (library + test harness). Stage 0 golden evidence fixtures are the
parity oracle. This is a designated **dogfood feature** (runs full Spec Kit pipeline).

### Work items

4.1 **Port the data model + parsers** into `Evidence/` modules:
   - `TaskParser.fs` — parse `tasks.md` (task ids, status boxes `[ X S F - *]`, `[P]`/`[US]`/
     tier/`[SEH]` annotations, phase-checkpoint edges, Synthetic-Evidence Inventory tables).
   - `DepsParser.fs` — parse `tasks.deps.yml` (use `YamlDotNet`; supports both legacy bare-list
     and object `{deps, skillist}` forms).
   - `SkillRegistry.fs` — discover skills across `.agents/skills`, `src/*/skill`,
     `template/fragments/*/skill`.

4.2 **Port the algorithms** into `Evidence/Graph.fs`:
   - Cycle detection (3-colour DFS), topological sort (Kahn) — standard, fully unit-tested with
     hand-built DAG fixtures.
   - Synthetic propagation: `declared=synthetic → synthetic`; `declared=done ∧ any dep
     synthetic/auto → auto-synthetic`; else `declared`. Encode as a pure function; property-test
     it (propagation is monotone; a graph with no synthetic roots has no auto-synthetic nodes).

4.3 **Port validation + audit** into `Evidence/Audit.fs` and `Evidence/StatusRegion.fs`:
   - Cross-file consistency (every task in `tasks.md` ↔ `tasks.deps.yml`), skill-id resolution,
     skill-ordering (`evidence-audit` not before `evidence-graph`), `[SEH]` design-phase-only
     timing, audit verdict (`PASS`/`FAIL`/`BLOCKED`).
   - `audit-status` structured-region scanner (port `audit-status-scan.py` faithfully: first-
     region-wins, duplicate-key = error, no prose interpretation).

4.4 **Port rendering** into `Evidence/Render.fs`: JSON (`task-graph.json`), Markdown
   (`task-graph.md`), Mermaid, ASCII tree — **byte-compatible** with the Python output schema so
   downstream consumers and the Stage-0 golden fixtures match.

4.5 **Rewire the build.** `build.fsx:1263–1276` (`EvidenceGraph`/`EvidenceAudit` `StartTarget`
   cases) stop emitting a `processEffect` to `run-audit.sh` and instead call
   `FS.Skia.UI.Build.Evidence.*` directly. The diff-scan portion of `run-audit.sh` (git pattern
   matching against `audit-patterns.yml`) is ported into `Evidence/DiffScan.fs`.

4.6 **Reduce or delete `run-audit.sh`.** Once the F# path is parity-clean, `run-audit.sh` becomes
   a thin shim that calls the build target (kept for backward-compat callers) or is deleted.
   Delete the two Python files and their embedded duplicates.

4.7 **Distribute to generated consumers.** Per ADR D1: generated projects' `EvidenceGraph`/
   `EvidenceAudit` now reference `FS.Skia.UI.Build` (packaged) instead of carrying the Python +
   `run-audit.sh`. Update the template (`template/base/.specify/...`) accordingly. This is itself
   a `template/**` change → escalates gates.

### New / changed artifacts

- `build/Governance/Evidence/*.fs` (TaskParser, DepsParser, SkillRegistry, Graph, Audit,
  StatusRegion, DiffScan, Render).
- Deleted: `.specify/extensions/evidence/scripts/python/compute-task-graph.py`,
  `audit-status-scan.py`; `run-audit.sh` reduced/removed.
- `template/base/**` updated to consume the packaged engine.
- `build.fsx:1263–1276` rewired.

### Exit criteria

- **Parity proof:** F# `EvidenceGraph`/`EvidenceAudit` output (`task-graph.json`,
  `task-graph.md`, audit count block) is byte-identical to the Stage-0 golden fixtures on the
  current feature and the two historical features. (Invariant 6.)
- Unit + property tests for cycle detection, topo sort, propagation, status-region scanning pass.
- No `python3` invocation remains in the evidence path (grep proves it).
- Generated consumer projects still pass `EvidenceGraph`/`EvidenceAudit` using the packaged
  engine (`GeneratedProductCheck` green).
- Languages-in-evidence-path reduced from {F#, Bash, Python} to {F#}; recorded vs baseline.
- Invariants 1–6 hold.

### Risks & mitigations

- *Risk:* subtle parser divergence (Markdown table edge cases, indentation). *Mitigation:* the
  golden-fixture parity gate blocks the merge until byte-identical; keep Python runnable in
  parallel behind a `--legacy-evidence` flag until parity is signed off, then delete.
- *Risk:* generated-consumer distribution complexity (packaging the engine). *Mitigation:* ADR D1
  resolves this in Stage 0; if packaging slips, ship the engine as source into the template as an
  interim, package later.
- *Risk:* `YamlDotNet` parses the custom minimal YAML differently than the bespoke Python parser.
  *Mitigation:* parity gate + a dedicated DepsParser fixture suite covering both YAML forms.

**Effort:** ~7–10 days (the largest single port). **Revert:** `--legacy-evidence` flag restores
the Python path until the flag and Python are removed at sign-off.

### Stage 4 — Implementation status (2026-06-01, feature 043)

**Status: IMPLEMENTED** (squash-merged to `main`; the rename master→main landed
alongside). Feature `043-foundations-evidence-engine` replaced the tri-language
evidence gate with typed, unit- and property-tested **compiled F#** inside
`FS.Skia.UI.Build`, computing the evidence graph and the full merge-gate audit
**in-process**. All **34 tasks** are `[X]` with **real evidence** (zero
synthetic); `speckit.evidence.audit` returns **verdict=PASS** and
`speckit.evidence.graph` returns **verdict=ok**. As a designated **dogfood** +
consumer-contract change it ran the full serialized FAKE gate sequence for itself
(`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
`EvidenceGraph` → `EvidenceAudit`). Runtime untouched: `git diff` over product
`src/**` = 0; no product `.fsi`, surface baseline, or `PackageVersion` change.

What landed against the Stage-4 work items:

- **4.1 data model + parsers — done.** `Evidence/TaskParser.fs` (tasks.md grammar
  → typed `TaskRecord` list, including the `[skillist: …]` mirror), `DepsParser.fs`
  (`tasks.deps.yml` bare-list + `{deps, skillist}` forms via **`YamlDotNet`**
  behind a typed model — no bespoke parser, FR-002), `SkillRegistry.fs`
  (`.agents/skills`, `src/*/skill`, `template/fragments/*/skill`).
- **4.2 algorithms — done.** `Evidence/Graph.fs`: 3-colour-DFS cycle detection,
  Kahn topological sort, and synthetic propagation as a **pure** function,
  **property-tested** with FsCheck (propagation monotonicity; a graph with no
  synthetic roots has no auto-synthetic nodes).
- **4.3 validation + audit — done.** `Evidence/Audit.fs` (cross-file consistency,
  skill-id resolution, skill-ordering, `[SEH]` timing, `PASS`/`FAIL`/`BLOCKED`
  verdict), `Evidence/StatusRegion.fs` (the `audit-status` structured-region
  scanner ported faithfully — first-region-wins, duplicate-key = error, no prose
  interpretation), plus `Evidence/Scans.fs` for the readiness-contract,
  persistent-launch, persistent-GUI-runtime, and window-visibility scans.
- **4.4 rendering — done, byte-compatible.** `Evidence/Render.fs` emits
  `task-graph.json` (schema_version 1.0, id-sorted, fixed field order), the
  `task-graph.md` block (verdict, skill-assessment table, status counts, SEH
  classification, Mermaid `classDef`, ASCII tree, propagation report), and the
  audit count block with the exact indentation/trailing-newline the Python wrote.
- **4.5 rewire the build — done.** `build.fsx`'s two evidence arms emit new
  in-process `EvidenceGraphCheck` / `EvidenceAuditCheck` `BuildEffect` cases
  (`update` stays pure; all file/`git`/write I/O lives in `interpret`) that call
  `FS.Skia.UI.Build.Evidence.Engine.runGraph`/`runAudit` — **no `processEffect` to
  `run-audit.sh`**. The diff-scan moved into `Evidence/DiffScan.fs`.
- **4.6 delete the Python and `run-audit.sh` — done.** Both Python files and
  `run-audit.sh` are **gone from the tracked tree** (the `--legacy-evidence`
  selector and the Python path were removed *in this feature* at parity sign-off,
  not deferred). Grep proofs (`logs/no-python-grep.txt`) confirm zero
  `python3`/`run-audit.sh`/`compute-task-graph.py`/`audit-status-scan.py` in the
  steady-state evidence path; the only residual copies are gitignored
  `artifacts/` build output. No `FSharp.Compiler.*` was introduced
  (`logs/no-fcs-grep.txt`, SC-004).
- **4.7 distribute to generated consumers — done (D1).** `FS.Skia.UI.Build`
  flipped `IsPackable false → true`, joined `PackLocal` + the pack/version flow +
  `docs/reports/dependencies.md`; `.template.config/template.json` stops copying
  the Python/bash scripts; `template/base/build.fsx` calls the **packaged** engine
  in-process; generated projects add a `FS.Skia.UI.Build` package pin. Every
  generated-consumer evidence gate passes through the packaged engine
  (`package/generated-evidence-reports/*`: graph `verdict=ok`, audit
  `verdict=PASS`, SC-006).

**Parity proof (Invariant 6).** Byte-for-byte (**0 bytes**) against the Stage-0
golden fixtures on **036/037/038** for `task-graph.json`, `task-graph.md`, and the
audit count block — *plus* five **newly-captured** golden fixtures for the scan
outputs that had no Stage-0 oracle (`readiness-contract-hits.json`,
`persistent-launch-hits.json`, `persistent-gui-runtime-hits.json`,
`window-visibility-hits.json`, `diff-scan-hits.json`, FR-017), captured from the
*then-current* Python engine before any deletion. Diffs recorded under
`readiness/parity/{036,037,038}/` and `readiness/parity/scans/{036,037,038}/`.

**Tests.** Expecto unit tests + FsCheck property tests assert **typed** results
(cycle/topo/propagation/status-region); golden byte-diff via DiffPlex is the
parity oracle. The previously-shelling suites (`AuditStatusRegionTests`,
`PersistentViewerEvidenceTests`, `SyntheticErrorEvidenceTests`) were re-pointed
from `python3`/`bash run-audit.sh` to the typed library (FR-014).

**Language reduction (SC-005).** The evidence path went from {F#, Bash, Python} →
**{F#}**, recorded vs the Stage-0 baseline in `logs/language-reduction.md`.

**Addition beyond the staged plan (recorded).** The plan's module list named
TaskParser/DepsParser/SkillRegistry/Graph/Audit/StatusRegion/DiffScan/Render; the
implementation also adds `Evidence/Engine.fs` (the orchestrator that wires
parse → merge → graph → render) and `Evidence/Scans.fs` (the four
readiness/launch/GUI/visibility scans), and captured the five extra scan golden
fixtures (FR-017) the Stage-0 oracle did not cover. No work item was dropped.

**Invariants held.** 1 (product surface unchanged — `PackageSurfaceCheck`/
`FsiTranscripts` no product baseline diff; only new *curated* governance-library
`.fsi` modules per Principle II), 2 (runtime untouched), 3 (generated consumers
still fully governed via the packaged engine), 4 (net10; `YamlDotNet`/
`Fake.Core.Target` already central, no new `PackageVersion` outside
`Directory.Packages.props`), 5 (FAKE sequencing), 6 (evidence output parity —
proven byte-identical) all hold.

**Stage 4 exit criteria: met** — byte-identical parity on all fixtures, typed
unit/property tests, no `python3` in the evidence path, generated consumers pass
on the packaged engine, evidence-path languages reduced to {F#}, invariants 1–6
hold.

---

## Stage 5 — Dedicated build front-end + MEL engine extraction + typed targets

**Goal:** Finish the keystone. Move the orchestration core (the MEL engine + remaining
validators) into the library, and reduce `build.fsx` to a thin front-end — ideally a dedicated,
compiled FAKE build project per the Stage-0 ADR.

**Why now:** Stages 3–4 emptied most domain logic out of `build.fsx`; what remains is the
orchestration skeleton, which is the cleanest thing to relocate last.

**Dependencies:** Stages 3, 4.

### Work items

5.1 **Extract the MEL engine** (`BuildMsg`/`BuildEffect`/`BuildModel`/`update`/`interpret`,
   build.fsx ~16–281, 1031–1450, 4627–4667) into `Engine/` modules of the library, now with the
   typed `Target` union (Stage 3) replacing string `StartTarget "…"`. Unit-test `update` as a
   pure function: given a `Target`, assert the emitted `BuildEffect` list.

5.2 **Move the remaining heavy validators** still in `build.fsx`:
   - Generated-product validation (~800 lines, build.fsx ~2700–3500) → `GeneratedProduct.fs`,
     with the **versioned contract** (ADR) so template changes get a deprecation window instead
     of a hard break.
   - Generated-guidance / skill-section scanners (~200 lines) → `Guidance.fs`.
   - Process-health/bootstrap (~267 lines, build.fsx ~1534–1800) → `Preflight.fs`.

5.3 **Create the dedicated build front-end** `build/Build.fsproj` (D2 decided): references
   `FS.Skia.UI.Build`, registers targets from the typed `Target` graph, delegates each target body
   to the library. `fake.sh`/`fake.cmd` updated to `dotnet run --project build/Build.fsproj --
   <target>`, dropping the `dotnet fake` CLI invocation. (Fallback, only if the Stage-3.1 spike
   blocked the dedicated project: a <200-line `build.fsx` that `#r`s the DLL, still via `dotnet
   fake`.)

5.4 **Retire `build.fsx`** — deleted under the dedicated-project path (the 4,688-line monolith is
   gone), or reduced to the <200-line shim only under the spike-fallback path.

5.5 **Migrate the validation contract from YAML to compiled F#** (per the config-representation
   ADR / analysis §6). **DONE EARLY in feature 042 (Stage 1) — pulled forward.** Because the
   library already existed, the interim `select-tier.fsx` was skipped and this item was delivered
   as part of Stage 1: tiers and routing rules are typed values/predicates in `Routing.fs` sharing
   the `Target` union (a mistyped gate is a compile error); the `Route` target calls it in-process;
   `validation.contract.yml` is **retained but generated** from `Routing.fs` (via
   `ContractView.render`), currency-checked by `TargetMetadataDrift`, never a hand-maintained
   source; no `select-tier.fsx` was ever created and no FCS / runtime script loading was
   introduced. See [Stage 1 — Implementation status](#stage-1--implementation-status-2026-06-01-feature-042).
   Nothing remains for Stage 5 here beyond consuming `Routing.fs` from the relocated front-end.

### New / changed artifacts

- `build/Build.fsproj` (dedicated front-end; D2).
- `build/Governance/Engine/*.fs`, `GeneratedProduct.fs`, `Guidance.fs`, `Preflight.fs`,
  `Routing.fs`.
- `fake.sh`, `fake.cmd` updated to `dotnet run --project build/Build.fsproj`.
- `validation.contract.yml` retired or demoted to a generated documentation view; `Routing.fs`
  becomes the source of truth. `scripts/build/select-tier.fsx` deleted.
- `.config/dotnet-tools.json`: drop `fake-cli` (moving to `dotnet run`), unless the spike-fallback
  thin-fsx path is taken.

### Exit criteria

- `build.fsx` either deleted or ≤ 200 lines; recorded vs 4,688 baseline.
- Every target produces baseline-identical reports/artifacts (golden diff across all 36 targets).
- `update` has direct unit tests for representative targets (typed effect-list assertions).
- Tier selection (`Route`) runs in-process; `select-tier.fsx` deleted; `validation.contract.yml`
  retired or demoted to a generated view, with `Routing.fs` as the compiled-F# source of truth.
- No FCS / runtime-script-loading dependency was introduced anywhere; config is compiled, not
  loaded (grep proves no `FSharp.Compiler.Service` reference).
- Cold-build and warm-build wall-clock recorded vs baseline (expect warm builds faster: compiled
  library replaces 207 KB script recompilation).
- Invariants 1–6 hold; full serialized gate sequence green.

### Risks & mitigations

- *Risk:* FAKE dedicated-project flow interacts badly with `dotnet tool restore`/`.fake` state
  caching. *Mitigation:* the Stage-3 spike (3.1) already validated library reference from the
  build; if the dedicated project is problematic, the ADR fallback (thin `build.fsx` `#load`)
  ships instead — same library, smaller front-end change.
- *Risk:* a target's report changes format subtly when moved. *Mitigation:* golden diff across
  *all* targets is the merge gate.

**Effort:** ~7–10 days. **Revert:** keep `build.fsx` alongside the new front-end behind a chooser
until the golden diff across all 36 targets is clean, then remove.

---

## Stage 6 — Codify remaining rules, trim prose, evidence hygiene, version the contract

**Goal:** Convert the remaining **bucket-(a)** prose rules into self-enforcing library checks,
trim **bucket-(b)** guidance to the minimum, and clean up committed-evidence bloat. This is where
the 21:1 prose-to-code ratio collapses.

**Why now:** The library now exists and is the natural home; the build front-end is thin; the
remaining work is converting "prose an agent obeys" into "code that fails."

**Dependencies:** Stages 3–5. This stage touches governance + consumer contracts → escalated
gates (full pipeline).

### Work items

6.1 **Codify the remaining bucket-(a) rules** as library validators that fail the build (these
   currently exist only as prose or as test fixtures, not production gates):
   - `[SEH]` design-phase-only timing as a *production* gate (today only `SyntheticErrorEvidenceTests.fs`
     proves the checker; make it run in `EvidenceAudit`).
   - Constitution-Check completeness (the 11-decision plan-template section) → a structured,
     validated checklist; reject a plan with missing required decisions.
   - Skill-id resolution / no-dangling-ids as a hard gate (build.fsx had ~150 lines of this; now a
     typed `SkillRegistry` function).
   - Surface-baseline presence per public module.

6.2 **Trim bucket-(b) prose.** Reduce the ~23,000 governance Markdown lines toward the low
   hundreds: delete rules now enforced by code (the agent no longer reads them — the build
   enforces them); keep only genuine rationale/intent/when-to-use. Record the new line count.

6.3 **Shrink agent context cost.** With deterministic rules gone from prose, the `SKILL.md`
   files shrink to capability guidance + the one canonical `fsharp` usage snippet. Measure the
   per-invocation context reduction (tokens) vs baseline.

6.4 **Version the generated-product contract.** Apply the ADR: `schema_version` on the contract,
   a deprecation window for structural checks, machine-readable change log.

6.5 **Evidence-artifact hygiene (minimal — D3 resolved).** The earlier "~38 GB" figure was the
   *working tree* (gitignored `.fake/`, `bin/obj/`, `artifacts/`). The **tracked** repo is only
   **~24 MB with ~15 MB of git history**; committed evidence is ~5 MB of `readiness.zip` archives
   and ~3 MB of logs — minor. **Decision: leave existing committed evidence as-is; add `.gitignore`
   rules so *future* regenerable logs/zips are not committed going forward.** No active tree
   cleanup, no history rewrite (negligible gain, would break clones). This item shrinks to a small
   `.gitignore` edit.

### New / changed artifacts

- Library: production gates for `[SEH]` timing, constitution-check, skill resolution, surface
  baselines.
- Trimmed `.agents/skills/**` (and generated `.claude/skills/**`), `.specify/memory/constitution.md`,
  templates.
- `validation.contract.yml` + generated-product contract: `schema_version`, changelog.
- `.gitignore` updates so future regenerable logs/zips aren't committed (no history rewrite,
  no tree cleanup — D3 resolved minimal).

### Exit criteria

- Governance Markdown line count reduced from the Stage-0 baseline (~23,000) to a recorded target
  (goal: low hundreds for *rules*; rationale prose retained).
- Every bucket-(a) rule has a failing-build gate + a unit test; prove each by introducing a
  violation in a scratch branch and observing the gate fail.
- Per-invocation agent context (skill bytes loaded) reduced; recorded vs baseline.
- Generated-product contract carries a version and a deprecation path.
- `.gitignore` prevents future regenerable logs/zips from being committed (D3 minimal).
- Invariants 1–6 hold.

### Risks & mitigations

- *Risk:* deleting prose an agent silently relied on. *Mitigation:* only delete a prose rule once
  its code gate exists and is proven to fail on violation (6.1 before 6.2, rule by rule).

**Effort:** ~5–8 days. **Revert:** prose deletions are git-revertible; gates are additive and can
be downgraded to warnings.

---

## Stage 7 — Decommission, measure, document the new normal

**Goal:** Confirm the programme's promises against the Stage-0 baseline, remove all interim
scaffolding, and document the new development model so it sticks.

**Dependencies:** Stages 1–6.

### Work items

7.1 **Remove interim scaffolding:** any `--legacy-evidence` flag, the old `build.fsx` chooser,
   `select-tier.fsx` (folded into the library in Stage 5), residual `run-audit.sh` shim.

7.2 **Final measurement report** `docs/reports/_baselines/<date>-foundations-after.md` comparing
   before/after: `build.fsx` lines (4,688 → target), governance Markdown lines, languages
   (3 → fewer in the evidence/build path), per-feature ceremony time, agent context bytes, warm-
   build time, duplication lines eliminated.

7.3 **Document the new normal:** update `README.md`, `docs/reports/build.md`,
   `docs/reports/speckit.md`, `CLAUDE.md`, `AGENTS.md` to describe: the two-tier process, the
   `Route` entry point, the governance library as the home of all rules, and the
   "generate-don't-sync" principle. Add an ADR closing the programme.

7.4 **Retrospective on dogfooding:** confirm the named dogfood features kept the harness honest;
   schedule a recurring full-pipeline run so the consumer governance path cannot rot.

### Exit criteria

- No interim flags or shims remain (grep proves it).
- After-baseline shows the targeted reductions; any miss is explained.
- All docs describe the new model; a new contributor can run `Route` and proceed without reading
  23,000 lines of prose.
- Full serialized gate sequence green; generated consumers fully governed.

**Effort:** ~2–3 days.

---

## Whole-programme definition of done

| Dimension | Baseline (2026-05-31) | Target |
|---|---|---|
| `build.fsx` size | 4,688 lines | deleted or ≤ 200-line shim |
| Domain logic location | inline in build script, untested | `FS.Skia.UI.Build` library, unit + property tested |
| Evidence-path languages | F# + Bash + Python | F# only |
| `compute-task-graph.py` / `audit-status-scan.py` / `run-audit.sh` | 1,310 + 150 + 1,284 LOC | removed (logic in library) |
| Governance Markdown (rules) | ~23,000 lines, 21:1 prose:code | low hundreds; rules enforced by code |
| `.claude`/`.agents` duplication | ~5,854 lines, unguarded | single source + generation |
| Framework-author process | full consumer ceremony (~12–14h/feature) | `inner-loop` (`Dev` + surface check); full pipeline reserved for consumers + dogfood |
| Tier selection | implicit / "run everything" | `./fake.sh build -t Route`, enforced by compiled `Routing.fs` |
| Framework-owned config | YAML, stringly-typed, runtime-parsed | compiled F# values/predicates in the library; build-time checked; no FCS |
| Generated-product contract | unversioned, hard-break | versioned with deprecation window |
| Runtime architecture | sound | **unchanged** |

---

## Decisions (resolved 2026-05-31)

All six open decisions were resolved with the maintainer. Recorded here and reflected in the
stages above.

- **D1 — Governance library distribution → published library package `FS.Skia.UI.Build`.** Repo
  build front-end project-references it in-solution; generated consumers package-reference the
  published version. One port serves both; consumers stay lean. *Shapes: Stage 0 ADR, 4.7, 5.3.*
- **D2 — Build front-end form → dedicated FAKE build project (`build/Build.fsproj`, `dotnet run`).**
  Compiled, IDE-grade, references the library directly, no DLL bootstrap wrinkle. Thin-`build.fsx`
  shim retained only as a fallback if the Stage-3.1 spike surfaces a blocker. **Spike CONFIRMED in
  feature 039 (2026-05-31): `Fake.Core.Target` drives a target from a compiled exe with no FSX
  runner and no `FSharp.Compiler.*` transitive dependency; the fallback is not needed.** *Shapes:
  Stage 5.* See `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md`.
- **D3 — Evidence artifacts → minimal.** Tracked repo is ~24 MB (~15 MB history), not 38 GB; the
  bloat was overstated. Leave existing committed evidence as-is; only `.gitignore` future
  regenerable logs/zips. No tree cleanup, no history rewrite. *Shapes: Stage 6.5 (now a one-line
  `.gitignore` edit).*
- **D4 — Spec Kit fork stance → accept full F# ownership.** Port the evidence engine to typed,
  tested, in-process F#; forgo an upstream merge path (already forked via `extensions/`+`presets/`
  in practice). *Shapes: Stage 0 ADR, Stage 4.*
- **D5 — Sequencing → Stage 0 + the Stage-3.1 spike first**, then ship Stage 1 / Stage 2 early and
  in parallel with the continuing library track. De-risk the dedicated-project unknown before
  committing broad effort. *Shapes: Stage dependency overview, Suggested entry point.*
- **D6 — Configuration representation → compiled F# as stated.** Framework-owned config becomes
  compiled F# values/predicates in the library (build-time enforcement, no FCS, no per-run
  compile); a data format is retained only for high-churn, agent-authored `tasks.deps.yml`.
  *Shapes: Stage 1.1, Stage 3.2/3.3, Stage 5.5.*

---

## Suggested entry point (D5 resolved)

**Start with Stage 0 + the Stage-3.1 spike, run together.** Stage 0 is now mostly a recording
exercise (the four shaping ADRs — D1, D2, D4, D6 — are decided; it captures baselines and golden
evidence fixtures). The Stage-3.1 spike stands up `build/Governance/FS.Skia.UI.Build.fsproj` and
the dedicated `build/Build.fsproj`, then proves the one technical unknown: that the dedicated FAKE
build project can reference and drive the compiled library. That spike either confirms D2 or
activates the thin-fsx fallback, which is what gates Stage 5's form.

Once the spike is green, **ship Stage 1 (two-tier process) early** — it is a designated dogfood
feature, delivers the largest immediate relief with no rewrite, and makes every later stage
cheaper by giving framework work a light default tier — and run **Stage 2** and the continuing
**library track (3→4→5→6)** in parallel. The highest-risk single step remains Stage 4 (the Python
port), gated by the Stage-0 golden-evidence parity fixtures.
