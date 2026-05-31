# Feature Specification: Foundations Governance Library — First Real Validators

**Feature Branch**: `041-foundations-library-validators`
**Created**: 2026-05-31
**Status**: Draft
**Input**: User description: "@docs/reports/2026-05-31-0908-foundations-rewrite-analysis.md @docs/reports/2026-05-31-1049-foundations-implementation-plan.md lets work on the next part. research what already has been done last few features and continue." → resolved with the maintainer to **Stage 3** of the foundations implementation plan: extract the first real validators out of `build.fsx` into the `FS.Skia.UI.Build` governance library.

## Overview

The foundations programme (companion reports
[`2026-05-31-0908-foundations-rewrite-analysis.md`](../../docs/reports/2026-05-31-0908-foundations-rewrite-analysis.md)
and [`2026-05-31-1049-foundations-implementation-plan.md`](../../docs/reports/2026-05-31-1049-foundations-implementation-plan.md))
converges on one keystone: a tested F# **governance library** extracted from the 4,839-line
`build.fsx`. Stage 0 (feature `039-foundations-baseline-spike`) stood up the library skeleton
(`build/Governance/FS.Skia.UI.Build.fsproj`) and the dedicated FAKE front-end (`build/Build.fsproj`),
captured the quantitative baseline, and committed **golden evidence fixtures** as the parity oracle.
The capability-skills feature (`040-foundations-capability-skills`) added the six F# capability
cookbooks plus the first two real library modules (`SkillSync`, `SkillExamples`) and their gates.

This feature is **Stage 3 of the plan**: the keystone's first substantive brick. It moves the
*cheapest, highest-value* validators out of `build.fsx` into the library, proving the
extraction-and-in-process-call pattern end-to-end on low-risk logic before the bigger Stage 4
Python port and Stage 5 front-end migration. Specifically it introduces the typed `Target` model
(so target metadata is **derived, not duplicated**, and the drift the build currently *checks for*
becomes structurally impossible) and represents the capability catalog as typed values (retiring the
hand-rolled YAML parser). Every extracted validator must produce reports **byte-identical** to the
Stage 0 golden baseline.

This is a **build-tooling** change only. It does not touch the runtime
(`Scene → SkiaViewer → Elmish`, the declarative boundary, the public `.fsi` product surface), does
not port any Bash/Python script (that is Stage 4), does not change the build front-end form (that is
Stage 5), and ships no package into any generated product.

## Clarifications

### Session 2026-05-31

- Q: Which foundations stage is this feature? → A: **Stage 3** — extract the first real validators
  into the governance library (resolved with the maintainer before specifying).
- Q: Which validators move first? → A: The two named in plan Stage 3.3 for low coupling + high
  duplication payoff: **target-metadata drift** (`build.fsx` `targetMetadata` / `ValidateTargetMetadataDrift`
  / `validateTargetMetadataAgainstRepo`, ~`837`–`1006` and the `requiredTargets`/`targetDependencyRows`
  registries) and the **capability catalog** (`readCapabilityCatalog` hand-rolled YAML parser +
  `validateCapabilityRows`, ~`2241`–`2360`).
- Q: How is "no behaviour change" proven? → A: **Golden-diff parity** — the `CapabilityCheck`,
  `TargetMetadata`, and `TargetMetadataDrift` reports the build emits after extraction are
  byte-identical to the Stage 0 golden fixtures committed under
  `tests/Governance.Tests/fixtures/`; the extraction does not merge until the diff is empty.
- Q: What replaces the hand-rolled capability YAML parser? → A: Per ADR D6 (compiled-F# config),
  the catalog becomes a **typed model** owned by the library; the bespoke line-by-line parser is
  retired. (Source form resolved below.)
- Q: How far does the typed `Target` model go in this feature? → A: **Full `Target` discriminated
  union now** — convert all `StartTarget "..."` dispatch arms to the typed union in this feature
  (not just enough to support the two validators). The library owns the `Target` DU and the typed
  dependency graph; `build.fsx` dispatches on the DU and derives `requiredTargets` /
  `targetDependencyRows` / metadata from it. The MVU `update`/effect-interpreter **engine** and the
  build front-end **form** (dedicated project / `build.fsx` retirement) remain Stage 5.
- Q: What is the capability catalog's source form? → A: **YAML behind the typed model.** Keep
  `template/capabilities.yml` as the source-of-truth data file (it is consumed by template
  generation — default-app selection, per-profile `.fsi` contract selection, generated guidance —
  so it is not build-internal), and read it through the library's typed model via the already-present
  `YamlDotNet`. Only the bespoke parser is retired; no new dependency; no YAML file deleted or
  regenerated this stage.
- Q: How is the golden-diff parity enforced? → A: **Governance.Tests assertion.** A Governance.Tests
  case reads each produced report (`CapabilityCheck`, `TargetMetadata`, `TargetMetadataDrift`) and
  asserts byte-equality against its committed Stage 0 golden fixture, running under the existing
  `Dev`/test gate. No new FAKE target is added.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Target metadata can no longer drift, because it is derived (Priority: P1)

A maintainer renames or reconfigures a build target. Today, target *identity*, *dependencies*, and
*metadata* live in three stringly-typed places (`requiredTargets`, `targetDependencyRows`,
`targetMetadata`) and the build runs a `TargetMetadataDrift` check to detect when they disagree — a
confession of multiple sources of truth. After this feature, the typed `Target` model in the
governance library is the single source: metadata is computed from it, so a mismatch is a compile
error or cannot be expressed, not a runtime drift to be reported.

**Independent test**: In a scratch branch, introduce a target reference that previously would have
produced a `TargetMetadataDrift` diagnostic (e.g. a runnable target with no metadata row). With the
typed model, the inconsistency is unrepresentable (fails to compile) or is caught by a library unit
test asserting the typed error — without running the full build.

### User Story 2 - The build computes its validators in-process, with the same output (Priority: P1)

The build invokes `CapabilityCheck`, `TargetMetadata`, and `TargetMetadataDrift`. After this
feature those targets call `FS.Skia.UI.Build` functions in-process instead of inline `build.fsx`
logic, and emit reports byte-for-byte identical to the Stage 0 golden fixtures. A maintainer cannot
tell, from the produced artifacts, that the logic moved — only that `build.fsx` is smaller and the
logic is now unit-tested.

**Independent test**: Run `CapabilityCheck`, `TargetMetadata`, and `TargetMetadataDrift` on the
pinned baseline state; diff each produced report against its Stage 0 golden fixture. The diff is
empty.

### User Story 3 - The moved rules are unit-tested against typed errors (Priority: P1)

A contributor changing capability-catalog or target-metadata logic gets fast, precise feedback from
unit tests that assert **typed** validation outcomes (a specific `MissingMetadata target` /
catalog-error case), not brittle string matches. The tests reference the real library functions
directly — the upgrade the existing `tests/Governance.Tests` files could not make while the logic
was trapped in a script.

**Independent test**: A new Governance.Tests suite calls the extracted validators with crafted
inputs and asserts the exact typed finding for each violation class; the tests fail if a validator
stops emitting that finding.

### User Story 4 - The bespoke capability YAML parser is gone (Priority: P2)

The ~60-line hand-rolled, indentation-fragile capability-catalog parser in `build.fsx` is retired in
favour of a typed model owned by the library. The catalog's validation rules become a pure function
over typed values with real error types.

**Independent test**: Grep proves the bespoke parser (`readCapabilityCatalog`'s line-splitting
state machine) no longer exists in `build.fsx`; the capability catalog is read through the library's
typed model; `CapabilityCheck` still produces the golden-identical report.

### Edge Cases

- A capability-catalog input that the old bespoke parser tolerated through an indentation quirk MUST
  produce the same parsed model (and therefore the same report) under the typed model, or the
  divergence MUST be surfaced before the old parser is deleted (golden-diff gate).
- A target present in `validation.contract.yml` or in `docs/reports/*.md` but absent from metadata
  MUST still be reported exactly as today (`validateTargetMetadataAgainstRepo`'s contract-drift and
  docs-drift diagnostics are preserved, not dropped, by the extraction).
- If the library DLL cannot be referenced from the build front-end at extraction time, the work
  MUST surface that explicitly (it is the Stage-5 trigger) rather than silently falling back to
  inline logic.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST introduce a typed `Target` **discriminated union** in
  `FS.Skia.UI.Build` (replacing the stringly-typed `requiredTargets` and `targetDependencyRows`
  registries) such that a target's identity, direct prerequisites, and metadata derive from **one**
  source, making the metadata "second source of truth" that `TargetMetadataDrift` currently checks
  structurally impossible to introduce. **All** `StartTarget "..."` dispatch arms in `build.fsx` MUST
  be converted to dispatch on the typed `Target` union (a renamed/mistyped target becomes a compile
  error, not a runtime drift), and `requiredTargets` / `targetDependencyRows` / metadata MUST be
  derived from the union rather than maintained alongside it.
- **FR-001a**: The MVU `update`/effect-interpreter **engine** and the build front-end **form** (a
  dedicated build project, or retirement of `build.fsx`) are **out of scope** for this feature and
  remain Stage 5. This feature converts dispatch *keys* to the typed union and relocates the two
  named validators; it does not relocate the engine or change how the build is launched.
- **FR-002**: The system MUST extract the target-metadata-drift validation
  (`ValidateTargetMetadataDrift` / `validateTargetMetadataAgainstRepo`, including the
  contract-reference and docs-reference drift checks) into the library as pure functions over the
  typed model, preserving every existing diagnostic category and message.
- **FR-003**: The system MUST represent the capability catalog as a typed model owned by the
  library and retire the hand-rolled, line-by-line YAML parser (`readCapabilityCatalog`). The
  source-of-truth file `template/capabilities.yml` MUST be retained (it is consumed by template
  generation) and read **behind** the typed model via the already-present `YamlDotNet` (no new
  dependency; the YAML file is neither deleted nor regenerated this stage). The catalog's validation
  (`validateCapabilityRows`) MUST become a pure function over typed values that reports **typed**
  error cases rather than ad-hoc strings.
- **FR-004**: The library MUST expose a uniform structured finding type so every extracted validator
  returns structured results; existing report text is reconstructed from those structured results
  for output parity.
- **FR-005**: The build MUST call the library **in-process** for the `CapabilityCheck`,
  `TargetMetadata`, and `TargetMetadataDrift` targets, replacing the inline `build.fsx` logic for
  those targets.
- **FR-006**: The `CapabilityCheck`, `TargetMetadata`, and `TargetMetadataDrift` reports produced
  after extraction MUST be byte-identical to the Stage 0 golden fixtures; the extraction MUST NOT
  merge until the parity diff is empty. (Plan Invariant 6 — output parity.)
- **FR-007**: Every new public F# module in `build/Governance` MUST carry a curated `.fsi` companion
  (Principle II) even though it is build-tooling; no access modifiers in `.fs`.
- **FR-008**: `tests/Governance.Tests` MUST gain unit tests that call the extracted validators
  directly and assert the typed error cases (not string matching); moved-logic tests that previously
  asserted strings/behaviours MUST be re-pointed at the real library functions.
- **FR-008a**: The golden-diff parity required by FR-006 MUST be enforced by a `Governance.Tests`
  assertion that reads each produced report (`CapabilityCheck`, `TargetMetadata`,
  `TargetMetadataDrift`) and asserts byte-equality against its committed Stage 0 golden fixture,
  running under the existing `Dev`/test gate. No new FAKE target is introduced for parity.
- **FR-009**: `build.fsx` MUST shrink by at least 800 lines versus the 041 pre-extraction
  baseline (4,839 lines at post-040 HEAD) as the moved logic is removed; the delta MUST be
  recorded. (The 039 Stage-0 count was 4,688; feature 040 grew the file, so the
  authoritative before-count for this feature is the current 4,839.)
- **FR-010**: The library MUST build clean under the repository conventions (`net10.0`,
  `TreatWarningsAsErrors`, `FS0078`-as-error, Central Package Management); no new `PackageVersion`
  outside `Directory.Packages.props`.
- **FR-011**: No product public surface MUST change — `PackageSurfaceCheck` and `FsiTranscripts`
  show no baseline diff, and nothing under the runtime `src/**` directories is edited.
- **FR-012**: No package MUST be shipped into any generated product by this feature; any new
  dependency is build-tooling-only, pinned in `Directory.Packages.props`, with a row added to
  `docs/reports/dependencies.md`. (The catalog work SHOULD reuse the already-present `YamlDotNet` if
  a YAML reader is retained behind the typed model, adding no new dependency.)
- **FR-013**: FAKE-backed validation MUST run in the repository's deterministic serialized order;
  the extracted targets MUST keep their existing positions in `requiredTargets` /
  `targetDependencyRows` and their existing meaning within `Dev`/`Verify`.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, contents, version, or generated-package consumer changes.
  `FS.Skia.UI.Build` is build-tooling only and is not shipped into any generated product by this
  feature. No controls/chart/graph/DataGrid authoring change.
- **Public contract impact**: No *product* `.fsi`, documented public API, sample contract, or
  surface baseline changes. New build-tooling modules in `build/Governance` REQUIRE curated `.fsi`
  companions (Principle II); the "contract" surfaces here are those internal build-tooling `.fsi`
  files plus the unchanged report formats validated by golden-diff.
- **State workflow impact**: The `build.fsx` MVU `update`/effect-interpreter boundary is preserved.
  The extracted validators are **pure** over their inputs; I/O (reading catalog/registry inputs,
  writing reports) stays at the interpreter edge. No new long-lived stateful workflow.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshots, Vulkan,
  Skia, or visual-output change.
- **Evidence obligations**: Real evidence only — `readiness/` parity reports showing the
  `CapabilityCheck` / `TargetMetadata` / `TargetMetadataDrift` golden-diff is empty; the recorded
  `build.fsx` line-count delta; the new Governance.Tests results; the serialized FAKE gate logs. No
  synthetic evidence anticipated (no `[S]`/`[SEH]` tasks).
- **Unsupported scope**: No Bash/Python port (Stage 4), no build front-end migration or `build.fsx`
  retirement (Stage 5), no two-tier `Route` process (Stage 1), no single-source generation beyond
  what already shipped (Stage 2), no runtime/visual/release/platform/distribution change.
- **Build-target impact**: `Dev`/`Verify` keep their meaning. The dispatch *mechanism* changes for
  every target (string `StartTarget "..."` → typed `Target`-union dispatch, FR-001), but **no
  target's name, dependencies, outputs, or position in the graph changes**. The `CapabilityCheck`,
  `TargetMetadata`, and `TargetMetadataDrift` target **bodies** additionally call the library
  in-process. **No new FAKE target is added** (parity is a Governance.Tests assertion, FR-008a).
  `DependencyReport` must continue to recognise any new build-tooling dependency (none expected —
  `YamlDotNet` is already present). `TemplateCheck`, `GeneratedProductCheck`,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit` are unchanged in
  meaning.

## Key Entities

- **Target (typed)**: the discriminated-union identity of a runnable build target, carrying its
  direct prerequisites and the data from which its metadata row is derived. Single source of truth
  replacing `requiredTargets` + `targetDependencyRows` + the `targetMetadata` derivation.
- **TargetMetadata**: the per-target descriptor (expected outputs, timeout class, cost, authority,
  failure owner, command, stale assumptions) — now computed from `Target`, not maintained alongside
  it.
- **ValidationFinding**: the uniform structured result type a validator returns (e.g. missing
  metadata, missing expected output, dependency divergence, contract/docs drift, catalog error),
  from which report text is rendered.
- **CapabilityRow / CapabilityCatalog**: the typed model of the capability catalog
  (`template/capabilities.yml`) replacing the hand-rolled parser; the subject of
  `validateCapabilityRows`.
- **GoldenFixture / ParityReport**: the Stage 0 committed expected outputs and the empty-diff
  comparison that gates the extraction.

## Success Criteria *(mandatory)*

- **SC-001**: `build.fsx` is at least 800 lines smaller than the 041 pre-extraction baseline
  (4,839 at post-040 HEAD) — final ≤ 4,039 — and the before/after counts are recorded in the
  feature's readiness evidence.
- **SC-002**: The `CapabilityCheck`, `TargetMetadata`, and `TargetMetadataDrift` reports produced
  by the post-extraction build are byte-identical to their Stage 0 golden fixtures (parity diff =
  0 bytes for all three).
- **SC-003**: A previously-possible target-metadata inconsistency (a renamed/mistyped target, a
  runnable target without a metadata row, or a metadata row without a runnable target) is
  unrepresentable once dispatch is on the typed `Target` union — it **fails to compile** — or, for
  the residual derived checks, is caught by a library unit test. The persistent SC-003 evidence is
  the committed `TargetMetadataTests` case (T009) asserting the typed
  `MissingMetadata`/`MissingRunnableTarget` finding; the compile-error half is demonstrated
  transiently on a scratch branch (T012).
- **SC-004**: At least 6 new Governance.Tests cases call the extracted validators directly and
  assert typed findings (≥3 for capability-catalog error classes, ≥3 for target-metadata drift
  classes); all pass.
- **SC-005**: The hand-rolled capability-catalog YAML parser no longer exists in `build.fsx`
  (grep returns nothing), and `CapabilityCheck` still produces the golden-identical report through
  the typed model.
- **SC-006**: The full serialized FAKE gate sequence (`Dev`, `GeneratedGuidanceCheck`,
  `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`) is green (modulo the
  documented pre-existing `FsiTranscripts`/`SkiaViewer.Tests` environment flakes recorded in feature
  039), and `PackageSurfaceCheck` shows no baseline diff.
- **SC-007**: `git diff` over `src/**` is empty (runtime untouched), and no new `PackageVersion`
  exists outside `Directory.Packages.props`.

## Assumptions

- The Stage 0 golden fixtures committed in `039-foundations-baseline-spike`
  (`tests/Governance.Tests/fixtures/`) are the authoritative parity oracle for the three extracted
  targets and remain reproducible at the pinned baseline SHA.
- The dedicated build front-end can reference the compiled `FS.Skia.UI.Build` library in-process —
  confirmed by the Stage 0 D2 spike (`SpikeHello` drove a target from the compiled exe). This
  feature targets that same reference path; if the *current* `build.fsx`/FAKE-runner path cannot
  `#r` the DLL cleanly, that is the documented Stage-5 trigger and is surfaced explicitly rather than
  worked around.
- Representing the capability catalog as a typed model is consistent with ADR D6 (compiled-F#
  config). For this stage the source form is **`YamlDotNet`-behind-the-typed-model** with
  `template/capabilities.yml` retained as the source file (resolved in clarification); moving the
  catalog to inline compiled F# values or a generated-YAML view is deferred to a later stage.
- Converting **all** `StartTarget` dispatch arms to the typed `Target` union (resolved in
  clarification) intentionally overlaps the *target-typing* portion of Stage 5; the Stage-5
  remainder — relocating the MVU `update`/effect-interpreter engine and changing the build
  front-end form — stays out of scope here, so this feature does not retire `build.fsx` or change
  how the build is launched.
- Per the programme meta-process, this is a framework-author feature; it is **not** one of the named
  dogfood features (Stage 1, Stage 4), so it runs the focused library/build gates plus the parity
  checks rather than re-running the full consumer pipeline for its own sake — while still holding
  Invariants 1–6.

## Dependencies

- Builds on `039-foundations-baseline-spike` (library skeleton, dedicated front-end, golden
  fixtures, ADRs D1/D2/D6) and `040-foundations-capability-skills` (the established
  extract-into-`build/Governance` + curated-`.fsi` + Governance.Tests pattern).
- Independent of Stage 1 (two-tier `Route` process) and Stage 2 (single-source generation beyond the
  shipped sync check). De-risks and precedes Stage 4 (Python evidence port). Pulls the
  *target-typing* portion of Stage 5 forward (the typed `Target` union + dispatch conversion);
  Stage 5's remainder — MVU engine relocation and the dedicated build front-end / `build.fsx`
  retirement — still follows.
