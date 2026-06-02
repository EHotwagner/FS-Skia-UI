# Feature Specification: V3 Stage 0 — Monolith-Retirement Baseline, Per-Package Surface Baselines & Parity Oracle

**Feature Branch**: `048-v3-retirement-baseline`
**Created**: 2026-06-02
**Status**: Draft
**Input**: User description: "stage 0" (V3 implementation plan, `docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md` §Stage 0)

## Context

The V3 modular-distribution programme retires the legacy `FS.Skia.UI` monolith (`src/Lib`). Unlike
every prior foundations feature, that programme will **edit the runtime `src/**`** — moving the
Vulkan/Skia host out of `src/Lib` into the `FS.Skia.UI.SkiaViewer` package and deleting the legacy
core. Before any runtime code moves (Stage 1 onward), the programme needs a captured, point-in-time
**baseline** and a **parity oracle** so that "output unchanged" and "public surface controlled" become
*provable* rather than asserted.

This feature (Stage 0) is **record-and-oracle only**: it changes **no runtime code** and moves no
library code between packages. It produces measurement artifacts, captured golden fixtures, per-package
surface baselines, the per-package surface-diff capability that consumes them, and the architecture
decision records that lock the retirement's shape.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Capture a verifiable before-state and parity oracle (Priority: P1)

As the maintainer/agent about to move the Vulkan/Skia host between packages, I need a committed,
SHA-pinned baseline of the monolith and a captured set of golden outputs from the **current** host, so
that when Stage 1 relocates the host I can prove the rendered output is unchanged instead of trusting
that it is.

**Independent test**: From a clean checkout at the pinned SHA, the baseline report reproduces its
headline numbers via the recorded commands, and the captured parity fixtures (deterministic
scene-output) re-derive byte-identically from the current host. A reviewer can confirm the leak
(`FS.Skia.UI.SkiaViewer` transitively pulling the `FS.Skia.UI` monolith) is documented with a
reproducible dependency dump.

### User Story 2 - Per-package surface baselines exist and are diffable (Priority: P2)

As an agent making later-stage package moves, I need each public package's API surface captured as an
independent baseline plus a check that diffs a package's current surface against its baseline, so that
an unintended public-API change in any single package is detectable rather than hidden inside the
aggregate surface check.

**Independent test**: The per-package surface-diff check runs over every public split package, reports
zero drift against the freshly-captured baselines at the pinned SHA, and — when a public signature is
experimentally altered in a scratch edit — reports drift for exactly that one package.

### User Story 3 - Retirement decisions are recorded as ADRs (Priority: P3)

As a contributor joining the V3 programme mid-flight, I need the shaping decisions (host ownership,
scene-vocabulary single source, governance-parser placement, legacy-sample policy, parity-oracle
method) written as ADRs, so that later stages execute against locked decisions rather than re-litigating
them.

**Independent test**: ADRs 0007–0011 exist under `docs/adr/`, each stating decision / alternatives /
rationale / which stages it shapes, and each is referenced by the implementation plan.

### Edge Cases

- **Headless-render flake**: reference screenshots may be non-deterministic in the known headless
  environment (the `SkiaViewer.Tests` libdecor-gtk crash). The parity oracle MUST therefore treat
  deterministic scene-output as the authoritative signal and screenshots as corroboration only, and
  MUST record the capture environment so a mismatch is attributable to environment, not regression.
- **No stable per-package baseline today**: only an aggregate surface baseline exists; the feature MUST
  create per-package baselines from scratch without weakening the existing aggregate check.
- **Monolith excluded from per-package baselines**: the package being retired (`FS.Skia.UI`) and the
  build-tooling library (`FS.Skia.UI.Build`) are out of scope for the *runtime* per-package baselines.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The feature MUST produce a SHA-pinned baseline report (under
  `docs/reports/_baselines/`) recording `src/Lib` line counts per file, the runtime package
  dependency graph, and the count of types defined in **both** the `Scene` package surface and the
  monolith surface (the duplicate scene vocabulary). Each headline metric MUST name the command that
  reproduces it.
- **FR-002**: The baseline report MUST include a reproducible **leak proof** — a dependency dump
  showing that the `FS.Skia.UI.SkiaViewer` package transitively depends on the `FS.Skia.UI` monolith
  and that the default generated `app` profile therefore pulls the monolith.
- **FR-003**: The baseline report MUST include a complete inventory of every consumer of the monolith
  (runtime packages, samples, test projects, and the governance build front-end) as the work-list later
  stages must clear. The inventory MUST classify **all** sample projects present at the pin
  (monolith-consumer vs split-package-only), derived from the recorded reproduction command, rather
  than pre-narrowing to a subset.
- **FR-004**: The feature MUST capture a **parity oracle** from the *current* monolith host:
  deterministic scene-output golden fixtures committed under the test fixtures tree, captured before any
  host code is moved. These become the Stage-1 parity gate.
- **FR-005**: The feature MUST capture reference rendered-frame artifacts (screenshots) from the
  current host for the visual samples **where the capture environment permits**, recorded together with
  the exact capture environment, and MUST document that scene-output is the authoritative oracle and
  screenshots are corroboration. If the known headless render flake (the `SkiaViewer.Tests`
  libdecor-gtk crash) prevents capture, the feature MUST record the infeasibility per Principle V
  (capture environment + failure class + the GPU-passthrough host needed) rather than fabricate or
  silently omit the frames; scene-output (FR-004) remains the authoritative gate regardless.
- **FR-006**: The feature MUST capture a **per-package public-surface baseline** for each public split
  package (`Scene`, `SkiaViewer`, `Elmish`, `KeyboardInput`, `Layout`, `Controls`, `Controls.Elmish`,
  `Testing`), excluding the retiring monolith and the build-tooling library.
- **FR-007**: The feature MUST provide a per-package surface-diff capability that compares each
  package's current public surface against its baseline and reports per-package drift. At the pinned
  baseline SHA it MUST report zero drift for every package.
- **FR-008**: The per-package surface-diff capability MUST report drift for exactly the affected
  package (not the whole repository) when a single package's public signature changes.
- **FR-009**: The feature MUST record ADRs 0007–0011 under `docs/adr/` covering: host ownership
  (`SkiaViewer` owns the Vulkan/Skia host), scene-vocabulary single source (`FS.Skia.UI.Scene` types
  canonical; monolith duplicates deleted, no permanent conversion shim), `AgentValidation` placement
  (moves to the governance library), legacy-sample policy, and the parity-oracle method.
- **FR-010**: The feature MUST NOT change any runtime behaviour or move any library code between
  packages. The monolith, the split packages, the host, and `SceneConversion` remain exactly as they
  are at the pinned SHA; only measurement artifacts, captured fixtures/baselines, the surface-diff
  capability, and documents are added.
- **FR-011**: The existing aggregate public-surface check and the generated-consumer contract checks
  MUST remain green; the new per-package capability is additive and MUST NOT weaken or replace them in
  this feature (merge-gate enforcement of per-package drift is deferred to a later stage).

### Framework Governance Prompts *(mandatory)*

- **Package impact**: **No package identity, contents, or version changes.** No package is added,
  renamed, repacked, or re-versioned. Per-package *surface baselines* (descriptive artifacts) are
  introduced for the existing public split packages; the monolith (`FS.Skia.UI`) and build-tooling
  library (`FS.Skia.UI.Build`) are excluded from those runtime baselines. No Charts/Controls authoring
  change.
- **Public contract impact**: **No `.fsi` signature changes.** Public surfaces are *captured* as
  baselines, not modified. No documented API or sample contract changes.
- **State workflow impact**: **None.** No stateful workflow, I/O, command, effect, subscription, or
  interpreter behaviour changes. (The per-package surface-diff capability is a pure comparison over
  captured surface text plus file reads at the interpreter edge.)
- **Layout/rendering impact**: **No rendering behaviour change.** Rendering output is *captured* as the
  parity oracle (scene-output fixtures + reference screenshots) but not altered. The capture
  environment is recorded; headless-flake risk is mitigated by treating scene-output as authoritative.
- **Evidence obligations**: SHA-pinned baseline report with reproduction commands; committed parity
  golden fixtures re-derivable from the current host; committed per-package surface baselines with a
  zero-drift run; a seeded-violation demonstration that the per-package diff flags exactly one package;
  ADRs 0007–0011. Evidence graph/audit PASS with real (non-synthetic) evidence.
- **Unsupported scope**: No host move (Stage 1), no governance-parser relocation (Stage 2), no
  sample/test repointing (Stages 3–4), no monolith deletion or unpublish (Stage 5). No separate
  `FS.Skia.UI.Charts` package split. No template-profile expansion. No history rewrite. No new
  rendering architecture.
- **Build-target impact**: A per-package surface-diff capability is added (consuming the new
  baselines) and runs green at baseline; it is additive. `EvidenceGraph`/`EvidenceAudit` run as the
  evidence gate. No change to the *behaviour* of `Dev`, `Verify`, `Ci`, `PackLocal`, `TemplateCheck`,
  `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, or the existing aggregate surface
  check. As a change touching governance/build paths, `Route` escalates this feature to the full
  serialized gate set (dogfood).

## Success Criteria *(mandatory)*

- **SC-001**: From a clean checkout at the pinned SHA, 100% of the baseline report's headline metrics
  reproduce via their recorded commands.
- **SC-002**: The leak proof reproduces: the dependency dump shows `FS.Skia.UI.SkiaViewer →
  FS.Skia.UI` and a generated default `app` resolving the monolith — verifiable by re-running the
  recorded command.
- **SC-003**: The captured scene-output parity fixtures re-derive **byte-identically** from the current
  host (0-byte diff) at the pinned SHA.
- **SC-004**: Per-package surface baselines exist for all 8 public split packages and the per-package
  diff reports **zero** drift for every one of them at the pinned SHA.
- **SC-005**: A single experimental public-signature change in one package causes the per-package diff
  to report drift for **exactly that one package** and no other.
- **SC-006**: ADRs 0007–0011 are present, each with decision/alternatives/rationale/affected-stages,
  and are linked from the implementation plan.
- **SC-007**: `git diff` over runtime `src/**` shows **zero** changes (no runtime code touched); the
  monolith, split packages, host, and `SceneConversion.fs` are unchanged at byte level.
- **SC-008**: The full serialized gate set (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`) is green, with `EvidenceAudit` returning
  PASS on real (zero-synthetic) evidence.

## Key Entities

- **Baseline report**: SHA-pinned measurement document (`docs/reports/_baselines/2026-06-02-v3-before.md`)
  with monolith LOC, dependency graph, duplicate-type inventory, leak proof, and consumer inventory.
- **Parity oracle**: committed golden artifacts — deterministic scene-output fixtures (authoritative)
  and reference screenshots (corroboration) — captured from the current host.
- **Per-package surface baselines**: one captured public-surface artifact per public split package.
- **Per-package surface-diff capability**: the comparison that diffs a package's current surface against
  its baseline and reports per-package drift.
- **ADRs 0007–0011**: the decision records that shape Stages 1–5.

## Assumptions

- "Public split packages" for per-package baselines = `Scene`, `SkiaViewer`, `Elmish`, `KeyboardInput`,
  `Layout`, `Controls`, `Controls.Elmish`, `Testing`. The retiring monolith (`FS.Skia.UI`) and the
  build-tooling library (`FS.Skia.UI.Build`) are excluded.
- Deterministic scene-output is the authoritative parity signal; screenshots corroborate, given the
  known headless-render flake.
- The per-package surface-diff capability is introduced as additive and **green** at baseline; turning
  per-package drift into a hard merge gate is deferred to Stage 5 of the plan.
- ADR numbering continues the existing series (foundations ended at ADR 0006), so this feature adds
  0007–0011.
- Baseline pin SHA = `031e56072779c736adf6dd8b0345e17b58a62e73` unless the feature branch advances it,
  in which case the branch-point SHA is recorded in the report.
