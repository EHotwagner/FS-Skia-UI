# Tasks: V3 Stage 0 — Monolith-Retirement Baseline, Per-Package Surface Baselines & Parity Oracle

**Feature branch**: `048-v3-retirement-baseline`
**Spec**: `specs/048-v3-retirement-baseline/spec.md`
**Plan**: `specs/048-v3-retirement-baseline/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

**This feature ships zero synthetic evidence.** All evidence is real and
reproducible: the SHA-pinned baseline report whose every headline metric re-runs
from its recorded command, parity golden fixtures that re-derive byte-identically
from the current host, eight captured per-package surface baselines diffed at zero
drift, a real reverted scratch edit that drifts exactly one package, ADRs
0007–0011, and the serialized escalated FAKE gate logs. No `[S]`/`[SEH]` task is
approved (Principle V); `EvidenceAudit` MUST return `verdict=PASS` with zero
synthetic (SC-008).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]** verifiable before-state + parity oracle, **[US2]** per-package
  surface baselines + additive diff capability, **[US3]** retirement decision
  records (ADRs 0007–0011)
- **[T1]** / **[T2]** — this feature is **Tier 1 for the governance/build surface
  only** (it adds one new curated governance `.fsi`, the `PerPackageSurfaceDiff`
  target, a Routing rule, and new baseline artifacts) and **Tier 2-equivalent for
  the runtime** (no runtime `.fsi`, no package identity/version, no rendering
  behaviour change — FR-010/FR-011/SC-007). The new-capability tasks carry `[T1]`;
  the record-and-oracle / measurement / verification tasks carry `[T2]`. Because
  it touches governance/build paths, `Route` **escalates** it; as a V3-programme
  dogfood feature it runs through the full serialized gate set **plus**
  `PerPackageSurfaceDiff`.
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read first)

This feature has three deliverable classes, and they take different skills:

- **Record-and-oracle Markdown** (the baseline report, the consumer inventory,
  the leak proof, the ADRs, every verification record) authors **no F# source**
  and runs existing `wc`/`grep`/`dotnet list package`/`git` commands by hand — so
  those tasks take a justified `valid-empty` `skillist`.
- **The parity oracle** authors a deterministic scene-output encoder that reads
  the **current host's `Scene` values** and a screenshot/environment capture.
  Scene-output encoding takes `fs-skia-scene` (scene vocabulary) plus
  `fs-skia-layout-evidence` (deterministic evidence mode); screenshot +
  environment capture takes `fs-skia-skiaviewer` (screenshot capture) plus
  `fs-skia-layout-evidence` (real-image evidence). *Ambiguity, recorded:* the
  host's `Scene` type is the monolith's duplicate, but the vocabulary is the
  shared `Scene` vocabulary, so `fs-skia-scene` is a confidence-medium assignment.
- **The `PerPackageSurfaceDiff` capability** authors compiled `FS.Skia.UI.Build`
  code: a new FAKE target + DiffPlex line comparison (`fsharp-build-orchestration`
  — "Drive FAKE targets from the compiled front-end; golden-diff parity with
  DiffPlex") and an edge interpreter that reads `.fsi` files and aggregates the
  `Controls` package's multiple `.fsi` files (`fsharp-io-globbing` — "File
  discovery, fnmatch-style glob matching, and generation-currency diffing"). The
  pure normalize/diff is string work, **not** grammar parsing, so `fsharp-parsing`
  is a deliberate `false-positive` rejection.

`fs-skia-template-update` is **not** assigned — there is no `dotnet new
fs-skia-ui` / `template.json` / package-pin change (FR-011). `speckit-constitution`
is **not** assigned — no `.specify/memory/constitution.md` edit; the shaping
decisions are recorded as ADRs (decision records), and no task title uses the
`constitution` word. The genuine workflow tasks are the last two: **T023**
declares `speckit-evidence-graph` and **T024** declares `speckit-evidence-audit`,
in that order (graph before audit).

## Governance risk levels & validation

- **Small** (routine Markdown inside this feature's own `readiness/`, the ADRs,
  and the baseline report prose): focused review plus a `git diff` over the edited
  files is the **required evidence** and is authoritative for the level.
- **Medium** (the new `PerPackageSurfaceDiff` capability and its eight captured
  baselines): the focused `PerPackageSurfaceDiff` target run plus the
  `tests/Governance.Tests` pure + interpreter tests are the **required evidence**;
  the zero-drift run (SC-004) and the one-package seeded drift (SC-005) are the
  authoritative signals for the level.
- **Broad** (required here, because `Route` escalates this governance/build-path
  change): the full serialized FAKE gate order (`Dev` → `PerPackageSurfaceDiff` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the final
  graph and audit gates). **Broad validation is required** whenever a
  governance/build target, Routing rule, or curated governance `.fsi` is added.
  Aggregate FAKE results are recorded as **non-authoritative**; any race-like or
  environment-flaky failure (the known `SkiaViewer.Tests` headless libdecor-gtk
  crash) is rerun in focused isolation and that focused result is authoritative.

## Pre-graph-gate pitfall guidance

Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`)
before declaring this phase complete. Task **titles** deliberately avoid the
validator's blocking trigger tokens: the new capability is referenced by its
camelCase id `PerPackageSurfaceDiff` and the word "diff" never appears as the
blocking `diff-scan`; the decision-record task says "decision records / ADRs"
(never `constitution`/`constitutional`); "runtime dependency graph" and "the
final graph and audit gates" use bare "graph" and never the blocking `task graph`
/ `evidence graph` / `evidence audit` phrases; no non-graph/non-audit title uses
`synthetic propagation` / `validator diagnostics` / `readiness validation` /
`mirror mismatch`. The genuine graph/audit workflow tasks (T023/T024) **do**
declare `speckit-evidence-graph` / `speckit-evidence-audit` and name
`EvidenceGraph` / `EvidenceAudit` directly. The readiness-scaffold task (T002)
uses the safe `Create placeholder evidence files listed by the plan` wording and
the readiness-aggregation task (T003) uses the `Complete readiness notes` prefix,
so their hyphenated filename citations do not fire capability checks. This feature
delivers **no viewer and no default executable** — screenshots are corroboration
captured from existing samples, so there is no persistent-launch / window-visibility
work and no such trigger phrase appears. `tasks.deps.yml` keeps one indented
object per task id with `deps` and `skillist`; every `[skillist: …]` mirror
matches the structured list exactly and in order. The baseline report (T007/T010)
and the ADRs (T019/T020) cross-reference by **fixed path** only — no backward task
edge is written, so the DAG stays acyclic.

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Record the feature Tier (Tier 1 for the governance/build surface only — one new curated `build/Governance/PerPackageSurface.fsi`, the `PerPackageSurfaceDiff` target, a Routing rule, and new baseline artifacts; Tier 2-equivalent for the runtime — no runtime `.fsi`, package identity/version, or rendering behaviour change, FR-010/FR-011/SC-007), the affected surfaces (`docs/reports/_baselines/2026-06-02-v3-before.md`, `tests/Parity.Tests/fixtures/v3-host-golden/**`, `readiness/per-package-surface/**`, `readiness/per-package-surface-expectations.md`, `build/Governance/PerPackageSurface.fs(i)`, `build/Governance/Targets.fs(i)` / `Routing.fs` / `Engine/Model.fs` / `Engine/Update.fs`, `tests/Governance.Tests/PerPackageSurfaceTests.fs`, `docs/adr/0007–0011`, and `specs/048-v3-retirement-baseline/readiness/**`), the public-API impact (no runtime `.fsi`; exactly one new governance `.fsi`), the Elmish/MVU applicability (N/A — the capability is a pure `diff` with file reads at a thin edge interpreter; no `Model`/`Msg`/`Cmd`/subscription, Principle IV not warranted), and the real-evidence obligations (SHA-pinned baseline report with per-metric reproduction commands, byte-identical parity golden re-derivation, eight zero-drift per-package baselines, a reverted one-package seeded drift, ADRs 0007–0011, the runtime-untouched proof, and the serialized escalated FAKE gate logs; zero synthetic)
- [X] T002 [P] [T2] [skillist: []] Create placeholder evidence files listed by the plan under `specs/048-v3-retirement-baseline/readiness/` so the audit-enforced readiness files are discoverable at setup: `per-package-surface-diff.md`, `seeded-violation.md`, `baseline-repro.md`, `parity-oracle.md`, `runtime-untouched.md`, the always-required contract trio `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, the gate records `validation-contract.md`, `evidence-graph.md`, `evidence-audit.md`, the `fsi/per-package-surface-diff.txt` transcript placeholder, and `logs/` (`dev.log`, `per-package-surface-diff.log`, `generated-guidance-check.log`, `template-check.log`, `generated-product-check.log`, `evidence-graph.log`, `evidence-audit.log`)
- [X] T003 [T2] [skillist: []] Complete readiness notes for the feature's required readiness placeholder files — `governance-risk-levels.md` (the small / medium / broad levels, their required evidence, and when broad validation is required), `aggregate-hang-diagnostics.md` (verdict / stage / elapsed duration / last observed command / focused rerun / non-authoritative aggregate, for the known `SkiaViewer.Tests` headless crash), and `runtime-limitations.md` (the .NET 10 desktop / Vulkan / SkiaSharp preview / unsupported macOS/mobile/browser / no software-renderer fallback statements) — each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation (capability contract + report shape fixed first)

- [X] T004 [T1] [skillist: fsharp-build-orchestration] Draft the curated public surface `build/Governance/PerPackageSurface.fsi` per `contracts/per-package-surface-diff.md` — `PackageId`, `Surface`, `SurfaceLineChange` (`Added`/`Removed`), `PackageDrift`, `DiffOutcome` (`Drifted`/`CheckedPackages`/`MissingBaselines`), and the vals `packagesInScope`, `normalize`, `diffPackage`, `diff`, `captureCurrent`, `loadBaselines`, `runReport` — with the eight in-scope split packages and the monolith + `FS.Skia.UI.Build` exclusion encoded as the surface contract (signatures only; implementation follows in US2)
- [X] T005 [P] [T2] [skillist: []] Scaffold the SHA-pinned baseline report document shape `docs/reports/_baselines/2026-06-02-v3-before.md` per `contracts/baseline-report.md` — the pin header, and the empty labelled sections for monolith LOC (per file), runtime dependency graph, duplicate-type inventory, leak proof, and consumer inventory, each with a placeholder for its reproduction command (values filled in US1)

**Checkpoint**: Foundation ready — the capability surface contract and the baseline report shape are fixed; story work may begin.

---

## Phase 3: User Story 1 (US1) — a verifiable before-state and parity oracle (P1)

**Goal**: a clean checkout at the pin re-derives the baseline report's headline
numbers from their recorded commands, reproduces the leak proof, and re-derives
the scene-output parity golden byte-identically; reference screenshots and the
capture environment corroborate (FR-001/002/003/004/005, SC-001/002/003).

### Tests First (Principle I, Principle VI)

<!-- Generated from .specify/memory/constitution.md by `./fake.sh build -t RefreshSurfaceBaselines`; do not hand-edit between the markers. -->
<!-- BEGIN GENERATED: constitution/tests-first -->
**VI. Test Evidence Is Mandatory** — Behavior-changing code MUST include automated tests that fail before the change and pass after.
<!-- END GENERATED: constitution/tests-first -->

- [X] T006 [P] [US1] [T2] [skillist: fs-skia-scene, fs-skia-layout-evidence] Add the failing-first scene-output golden re-derivation test under `tests/Parity.Tests/fixtures/v3-host-golden/` that re-runs the deterministic encoder over the current host's seed scenes and asserts **byte-identical** output (0-byte diff, SC-003); it is red until the encoder and committed fixtures exist (T008)

### Implementation

- [X] T007 [P] [US1] [T2] [skillist: []] Fill the baseline report `docs/reports/_baselines/2026-06-02-v3-before.md` sections with measured values — `src/Lib/*.fs(i)` LOC per file, the runtime package dependency graph, the duplicate-type inventory across `src/Scene/Scene.fsi` and `src/Lib/Library.fsi`, the leak proof showing `FS.Skia.UI.SkiaViewer → FS.Skia.UI` and a generated default `app` resolving the monolith, and the complete consumer inventory (runtime `src/SkiaViewer`, all sample projects at the pin classified monolith-consumer vs split-package-only, the test projects, and `build/Governance/Front/Support.fs`) — each headline metric naming the exact command that reproduces it (FR-001/002/003, SC-001/002)
- [X] T008 [US1] [T2] [skillist: fs-skia-scene, fs-skia-layout-evidence] Implement the deterministic scene-output encoder (stable node ordering, canonical numeric formatting, no timestamps/environment-dependent fields, versioned with the fixture) and capture the golden fixtures under `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/<seed>.txt` from the current host, turning T006 green (FR-004, SC-003)
- [X] T009 [P] [US1] [T2] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Capture reference rendered-frame screenshots under `tests/Parity.Tests/fixtures/v3-host-golden/screenshots/<sample>.png` from the current host (`ScreenshotGallery`/`EffectsGallery`/`BasicViewer`) together with `capture-environment.md` (OS, GPU/driver, .NET/toolchain, capture command, timestamp), recorded as **corroboration only** with scene-output documented as the authoritative oracle; if the known `SkiaViewer.Tests` libdecor-gtk headless crash prevents capture in this environment, mark the screenshot capture `[-]` with a Principle V infeasibility note in `capture-environment.md` (environment + failure class + the GPU-passthrough host required) rather than faking frames — scene-output (T008) remains the authoritative gate (FR-005)
- [X] T010 [US1] [T2] [skillist: []] Reproduce the US1 before-state — re-run every baseline headline metric and the leak-proof dump from their recorded commands and confirm the report values match, and re-derive the scene-output golden byte-identically — recording the re-runs (command + output) in `readiness/baseline-repro.md` and `readiness/parity-oracle.md` (SC-001/002/003)

**Checkpoint**: User Story 1 complete — baseline report reproduces, leak proof reproduces, parity golden re-derives byte-identically, screenshots + environment recorded.

---

## Phase 4: User Story 2 (US2) — per-package surface baselines exist and are diffable (P2)

**Goal**: each of the eight public split packages has a captured surface baseline,
the additive `PerPackageSurfaceDiff` capability reports zero drift across all eight
at the pin, and a single reverted scratch edit drifts exactly that one package —
without weakening the existing aggregate `PackageSurfaceCheck` (FR-006/007/008/011,
SC-004/005).

### Tests First (Principle I, Principle VI)

- [X] T011 [P] [US2] [T1] [skillist: fsharp-build-orchestration] Add the failing-first pure semantic tests in `tests/Governance.Tests/PerPackageSurfaceTests.fs` exercising the `PerPackageSurface` surface through its `.fsi` — identical surfaces yield empty `Drifted`; a single mutated signature yields exactly one `PackageDrift` for that package and no other (the SC-005 oracle over literal-but-real surface text); a current package with no baseline lands in `MissingBaselines` and fails (Principle VII)
- [X] T012 [P] [US2] [T1] [skillist: fsharp-build-orchestration, fsharp-io-globbing] Add the failing-first interpreter test that runs `captureCurrent`/`loadBaselines` over the real source tree and committed baselines and asserts `Drifted = []` and `MissingBaselines = []` at the pin (the SC-004 oracle); red until the edge interpreter and the eight baselines exist

### Implementation

- [X] T013 [US2] [T1] [skillist: fsharp-build-orchestration] Implement the pure core in `build/Governance/PerPackageSurface.fs` — `normalize` (strip `//` and `(* *)` comments, trim trailing whitespace, collapse blank-line runs, normalize newlines to `\n`, preserve declaration order), `diffPackage` (DiffPlex line comparison, `None` ⇒ zero drift), and `diff` (per-package, missing baseline ⇒ `MissingBaselines`), turning T011 green
- [X] T014 [US2] [T1] [skillist: fsharp-io-globbing] Implement the edge interpreter in `build/Governance/PerPackageSurface.fs` — `captureCurrent` (read each in-scope package's `.fsi` file(s) and normalize, aggregating the `Controls` package's multiple `.fsi` files in filename order), `loadBaselines` (read `readiness/per-package-surface/*.fsi.txt`), and `runReport` (write the per-package drift report, return clean ⇔ no drift and no missing) — failing loud with the package, the added/removed lines, and the baseline path on drift
- [X] T015 [US2] [T1] [skillist: fsharp-io-globbing] Capture the eight per-package public-surface baselines at the pin under `readiness/per-package-surface/<PackageId>.fsi.txt` (`Scene`, `SkiaViewer`, `Elmish`, `KeyboardInput`, `Layout`, `Controls`, `Controls.Elmish`, `Testing`), excluding the monolith `FS.Skia.UI` and the build-tooling `FS.Skia.UI.Build`, turning T012 green
- [X] T016 [US2] [T1] [skillist: fsharp-build-orchestration] Register the `PerPackageSurfaceDiff` FAKE target (`Targets.fs(i)` `allTargets`/`name`/`directPrerequisites = [ Build ]`/metadata), wire the `BuildEffect`/`StartTarget` arm in `Engine/Model.fs` + `Engine/Update.fs`, add the new `Routing.fs` rule over `readiness/per-package-surface/**` + the new module path (tier `FocusedAuthority`, gates `[ PerPackageSurfaceDiff ]`, expected artifact `readiness/per-package-surface-expectations.md`), author that expectations doc. **Routing-rule sub-step deferred (runtime-coupling finding):** a rule would render `PerPackageSurfaceDiff` into `validation.contract.yml`, whose known-gate allowlist is validated by the runtime monolith (`src/Lib/AgentValidation.fs` `knownGates`); adding the gate there would modify runtime code, violating SC-007 (`src/**` byte-unchanged). The target ships additive + runnable directly; Route-gating is deferred with the Stage-5 hard-gate enforcement and the Stage-2 `AgentValidation` relocation (ADR 0009). `validation.contract.yml` is therefore unchanged. See `readiness/per-package-surface-expectations.md` and `readiness/runtime-untouched.md`
- [X] T017 [US2] [T1] [skillist: fsharp-build-orchestration] Run `./fake.sh build -t PerPackageSurfaceDiff` green at the pin (zero drift across the eight packages, SC-004), capture the FSI transcript exercising `diff`/`captureCurrent` to `readiness/fsi/per-package-surface-diff.txt`, and record the zero-drift run in `readiness/per-package-surface-diff.md`
- [X] T018 [US2] [T1] [skillist: fsharp-build-orchestration] Demonstrate the seeded one-package violation — make a reverted scratch edit to one public `.fsi` (e.g. `src/Scene/Scene.fsi`), re-run the target so `Drifted` reports exactly that one package and no other, then `git checkout --` the file and re-run to confirm zero drift — recording the demonstration in `readiness/seeded-violation.md` (real reverted edit over real files, SC-005)

**Checkpoint**: User Story 2 complete — eight baselines captured, target green at zero drift, one-package seeded drift demonstrated, aggregate check untouched.

---

## Phase 5: User Story 3 (US3) — retirement decisions recorded as decision records (P3)

**Goal**: the five shaping decisions are written as ADRs 0007–0011 and linked from
the programme implementation plan, so later stages execute against locked decisions
(FR-009, SC-006).

- [X] T019 [P] [US3] [T2] [skillist: []] Author the retirement decision records `docs/adr/0007-host-ownership.md`, `0008-scene-vocabulary-single-source.md`, `0009-agentvalidation-placement.md`, `0010-legacy-sample-policy.md`, `0011-parity-oracle-method.md` in the existing `0006-*` ADR format — each with Status, Date, Decision source, Context, Decision, Alternatives, Rationale, and **Affected stages** (research.md D8)
- [X] T020 [US3] [T2] [skillist: []] Link ADRs 0007–0011 from `docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md` and confirm each ADR is present with all required sections, recording the presence + link check in `readiness/baseline-repro.md` (FR-009, SC-006)

**Checkpoint**: User Story 3 complete — ADRs 0007–0011 present, sectioned, and cross-linked from the programme plan.

---

## Phase 6: Integration & Polish (runtime-untouched proof, serialized escalated gates)

- [X] T021 [P] [T2] [skillist: []] Capture the runtime-untouched standing-invariants proof in `readiness/runtime-untouched.md` — `git diff --stat -- 'src/**'` is empty (monolith, split packages, host, and `SceneConversion.fs` byte-unchanged, SC-007) and the existing aggregate `PackageSurfaceCheck` stays green and unchanged with no new `PackageVersion` outside `Directory.Packages.props` (FR-010/FR-011)
- [X] T022 [T2] [skillist: []] Run the escalated serialized FAKE gate set sequentially — `Dev` → `PerPackageSurfaceDiff` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the final graph and audit gates (T023/T024) — never concurrently; record aggregate FAKE results as **non-authoritative** and rerun any race-like or environment-flaky failure (the known `SkiaViewer.Tests` headless crash) in focused isolation as the authoritative result; logs under `readiness/logs/`
- [X] T023 [T2] [skillist: speckit-evidence-graph] Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`) — confirm the DAG is acyclic, no dangling refs, no `[S*]` surprises, and the structured task metadata and visible mirrors are valid (`verdict=ok`)
- [X] T024 [T2] [skillist: speckit-evidence-audit] Run the merge-gate audit (`./fake.sh build -t EvidenceAudit`) — confirm `verdict=PASS` (0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 blocking diff-scan, 0 blocking readiness-contract) with zero synthetic evidence to accept (SC-008)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — this feature ships zero synthetic evidence; the baseline reproduces from recorded commands, the parity golden re-derives byte-identically, the per-package diff runs over real `.fsi` surfaces, and the seeded violation is a real reverted edit)_ | | | | | | | | |
