# Foundations Implementation Plan: Two-Tier Process + Governance Library

- **Date:** 2026-05-31 10:49 CEST
- **Author:** Claude Code (planning, requested by maintainer)
- **Status:** Proposed plan. Not yet a feature; not in-flight.
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
Stage 0  Foundations, baselines, decisions
   │
   ├──> Stage 1  Two-tier process (policy + enforcement)        [independent, ship first]
   │
   ├──> Stage 2  Single-source generation (.claude/.agents, constitution, skillist)
   │
   └──> Stage 3  Governance library skeleton + cheap validators
                    │
                    ├──> Stage 4  Port Python graph/audit into the library
                    │
                    └──> Stage 5  Dedicated build front-end + extract MEL engine + typed targets
                                     │
                                     └──> Stage 6  Codify remaining rules, trim prose, evidence hygiene, version the contract
                                                      │
                                                      └──> Stage 7  Decommission, measure, document the new normal
```

Stages 1 and 2 are independent of the library track (3→4→5→6) and can proceed in parallel by
different sessions. Stage 1 is the recommended first ship: it is pure policy + a thin gate and
delivers the maintainer's biggest immediate relief.

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
   - **ADR: Governance library placement and distribution** — decide whether the extracted
     library is (a) a build-only project under `build/`, (b) a non-packable `src/` project, or
     (c) a published package `FS.Skia.UI.Build`. **Recommendation: (c)** because generated
     consumer projects run `EvidenceGraph`/`EvidenceAudit` too and will need the same engine; a
     package referenced by both the repo's build front-end and generated build front-ends avoids
     a second port. (See Open Decision D1.)
   - **ADR: Build front-end form** — dedicated FAKE build project (`dotnet run`) vs thin
     `build.fsx` that `#load`s the compiled library. **Recommendation: dedicated project** for
     tooling/incremental compilation; revisit if it complicates the FAKE tool-restore flow.
   - **ADR: Contract versioning** — the generated-product contract (the ~800 lines of structural
     checks) gets a `schema_version` and a deprecation window. Define the policy now.
   - **ADR: Spec Kit fork stance** — porting the Python forks us from upstream Spec Kit. Record
     that we accept ownership (the `extensions/`+`presets/` customisation already forked us in
     practice).

0.3 **Establish the meta-process** (this is the dogfooding fix applied to the plan itself):
   - This programme's own features run under the **framework-author inner loop** (Stage 1's
     light tier): `Dev` + surface check only, *except* Stage 1 and Stage 6 which touch
     governance/consumer contracts and therefore escalate to the full gate set.
   - Designate Stage 1 and Stage 4 as the **named dogfood features** that intentionally exercise
     the full Spec Kit + evidence pipeline, keeping the harness honest.

### Exit criteria

- Baseline file committed with all counts and the golden evidence fixtures.
- All four ADRs written and the open decisions (below) either resolved or explicitly deferred
  with an owner.
- No code changed (Stage 0 is read + document only). `Dev` still green.

### Risks & mitigations

- *Risk:* baseline drifts before later stages run. *Mitigation:* baseline is a point-in-time
  snapshot with the git SHA recorded; later stages compare against it, not against a moving tree.

**Effort:** ~0.5–1 day. **Revert:** delete the docs; nothing else touched.

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

1.1 **Define the framework-author inner loop** explicitly in `validation.contract.yml`:
   - Add a `developer_class` axis (`framework-author` | `consumer-agent`).
   - `framework-author` default tier = `inner-loop` (gates: `Dev` + a new lightweight
     `SurfaceCheck` composite = `PackageSurfaceCheck` only when `.fsi` changed).
   - `consumer-agent` and any change matching `template/**`, `.specify/**`, `validation.contract.yml`,
     governance paths, or `src/**/*.fsi` **escalate** to the existing focused/agent-ready tiers
     via the current `routing_rules`.

1.2 **Add a tier-selection gate** — a small, deterministic command that, given the working-tree
   diff, prints the required tier and gate list and (in `--enforce` mode) fails if a
   higher-tier change is being shipped without its gates' evidence artifacts present. Two
   implementation options:
   - *Interim (Stage 1):* a `scripts/build/select-tier.fsx` (`dotnet fsi`) that reads
     `validation.contract.yml` + `git diff --name-only` and emits the tier + gates as JSON/text.
   - *Final (folded into Stage 3+):* the same logic as a typed function in the governance library
     exposed via a `Route` target. Stage 1 ships the fsx; Stage 6 retires it into the library.

1.3 **Add a `Route` FAKE target** (`./fake.sh build -t Route`) that runs the selector and prints
   the minimal gate set for the current diff. This becomes the agent's and maintainer's entry
   point: "what must I run for this change?" replaces "read the prose and guess."

1.4 **Update agent-facing guidance** (`CLAUDE.md`, `AGENTS.md`) to say: *run `Route` first; run
   only the gates it prints.* Replace the blanket "serialized six-target order" instruction with
   "that order is the `maintainer-verify`/escalated path; routine framework work uses `Route`."

1.5 **Mark dogfood features.** Add a `dogfood: true` marker convention for the small set of
   features that must run the full pipeline regardless of tier, so the harness stays exercised.

### New / changed artifacts

- `validation.contract.yml` (extended with `developer_class`, `inner-loop` gate definition).
- `scripts/build/select-tier.fsx` (interim selector).
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

3.3 **Move the first validators** (chosen for low coupling + high duplication payoff):
   - **Target-metadata drift** (`ValidateTargetMetadataDrift`, build.fsx ~865–879 and ~548–1030).
     Becomes a pure function over the typed `Target` graph; the drift *disappears* because
     metadata is derived, not duplicated. ~480 lines of build.fsx logic → tested module.
   - **Capability-catalog parse + validate** (build.fsx ~2244–2361). Replace the hand-rolled YAML
     parser with `YamlDotNet` (already a managed dependency) + a typed `CapabilityRow` model and
     a `validateCapabilityRows` function with real error types. ~500 lines → tested module.
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
  `CapabilityCatalog.fs`, `TargetMetadata.fs`.
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

5.3 **Create the dedicated build front-end** `build/Build.fsproj` (FAKE dedicated build project
   pattern): references `FS.Skia.UI.Build`, registers targets from the typed `Target` graph,
   delegates each target body to the library. `fake.sh`/`fake.cmd` updated to `dotnet run
   --project build/Build.fsproj -- <target>` (or keep `dotnet fake` if the ADR chose a thin
   `build.fsx` that `#load`s the DLL).

5.4 **Retire `build.fsx`** to either a deleted file (dedicated-project path) or a <200-line
   `#load` shim (script path). The 4,688-line monolith is gone.

5.5 **Fold Stage 1's `select-tier.fsx` into the library** as a typed `Routing.fs`; the `Route`
   target now calls it in-process.

### New / changed artifacts

- `build/Build.fsproj` (dedicated front-end) **or** trimmed `build.fsx` shim.
- `build/Governance/Engine/*.fs`, `GeneratedProduct.fs`, `Guidance.fs`, `Preflight.fs`,
  `Routing.fs`.
- `fake.sh`, `fake.cmd` updated.
- `.config/dotnet-tools.json` (drop `fake-cli` if moving fully to `dotnet run`; keep if thin-fsx
  path).

### Exit criteria

- `build.fsx` either deleted or ≤ 200 lines; recorded vs 4,688 baseline.
- Every target produces baseline-identical reports/artifacts (golden diff across all 36 targets).
- `update` has direct unit tests for representative targets (typed effect-list assertions).
- Tier selection (`Route`) runs in-process; `select-tier.fsx` deleted.
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

6.5 **Evidence-artifact hygiene.** Address the committed bloat (35 `.zip`, 142 `.log`, 174 `.txt`,
   916 `.md`; ~38 GB tree): decide per-class whether evidence is regenerable output (gitignore +
   regenerate on demand) or durable history; move regenerable evidence out of committed history.
   This is lower-risk now that the evidence engine is in-process and fast.

### New / changed artifacts

- Library: production gates for `[SEH]` timing, constitution-check, skill resolution, surface
  baselines.
- Trimmed `.agents/skills/**` (and generated `.claude/skills/**`), `.specify/memory/constitution.md`,
  templates.
- `validation.contract.yml` + generated-product contract: `schema_version`, changelog.
- `.gitignore` updates for regenerable evidence; a migration commit moving bloat out of history
  (or a documented decision to keep it).

### Exit criteria

- Governance Markdown line count reduced from the Stage-0 baseline (~23,000) to a recorded target
  (goal: low hundreds for *rules*; rationale prose retained).
- Every bucket-(a) rule has a failing-build gate + a unit test; prove each by introducing a
  violation in a scratch branch and observing the gate fail.
- Per-invocation agent context (skill bytes loaded) reduced; recorded vs baseline.
- Generated-product contract carries a version and a deprecation path.
- Committed-evidence size reduced per the hygiene decision; recorded.
- Invariants 1–6 hold.

### Risks & mitigations

- *Risk:* deleting prose an agent silently relied on. *Mitigation:* only delete a prose rule once
  its code gate exists and is proven to fail on violation (6.1 before 6.2, rule by rule).
- *Risk:* rewriting git history for bloat is disruptive. *Mitigation:* prefer gitignore-going-
  forward + a single archival commit over history rewrite; only rewrite history if the maintainer
  explicitly opts in (Open Decision D3).

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
| Tier selection | implicit / "run everything" | `./fake.sh build -t Route`, enforced from `validation.contract.yml` |
| Generated-product contract | unversioned, hard-break | versioned with deprecation window |
| Runtime architecture | sound | **unchanged** |

---

## Open decisions requiring maintainer input

- **D1 — Governance library distribution.** Build-only project, non-packable `src/` project, or
  published `FS.Skia.UI.Build` package? (Recommendation: published package, because generated
  consumers also run the evidence engine.) *Blocks: Stage 4.7, Stage 5.3.*
- **D2 — Build front-end form.** Dedicated FAKE build project (`dotnet run`) vs thin `build.fsx`
  that `#load`s the compiled library. (Recommendation: dedicated project.) *Blocks: Stage 5.*
- **D3 — Evidence bloat handling.** Gitignore-going-forward + single archival commit (safe) vs
  history rewrite (disruptive, needs explicit opt-in). *Blocks: Stage 6.5.*
- **D4 — Spec Kit fork stance.** Confirm acceptance that porting the Python permanently forks us
  from upstream Spec Kit. *Blocks: Stage 4.*
- **D5 — Sequencing & parallelism.** Stage 1 and Stage 2 can run in parallel with the library
  track. Confirm whether to ship Stage 1 alone first (recommended) for immediate relief.

---

## Suggested entry point

Ship **Stage 1** first as its own Spec Kit feature (it is a designated dogfood feature, so it
runs the full pipeline and exercises the harness). It delivers the maintainer's largest immediate
relief with zero rewrite, and it makes every subsequent stage cheaper by giving framework work a
light default tier. Begin the library track (Stage 3) in parallel once D1/D2 are decided; the
library skeleton spike (3.1) should be done early because it validates the one technical
unknown — whether the FAKE front-end can reference a compiled library — that gates Stage 5's form.
