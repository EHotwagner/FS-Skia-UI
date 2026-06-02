# Feature Specification: V3 Stage 3–4 Residual — Decouple Remaining Consumers from `src/Lib`

**Feature Branch**: `052-v3-lib-decoupling`  
**Created**: 2026-06-02  
**Status**: Draft  
**Input**: User description: "@docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md implement the next part and update the plan afterwards."

## Context

The V3 modular-distribution programme retires the legacy `FS.Skia.UI` monolith
(`src/Lib`). Stages 0–2 shipped: the parity oracle and per-package surface
baselines exist (048), the host was extracted into `FS.Skia.UI.SkiaViewer` and
the modularity leak closed (050), and `AgentValidation` was relocated into the
governance library (051). The mechanical sample/test repointing was pulled
forward into Stage 1.

This feature is the plan's bundled **Stages 3–4 residual** — the last work
before the Stage 5 closeout. After Stages 1–2, `src/Lib` retains only two things:

- the **rich `KeyboardInput`** module (`src/Lib/KeyboardInput.fs(i)`, namespace
  `FS.Skia.UI`, retyped in Stage 1 onto `FS.Skia.UI.Scene` +
  `FS.Skia.UI.SkiaViewer.Host`), and
- the **`Parity` evidence helper** (`src/Lib/Library.fs(i)`).

Four consumers therefore still reference `src/Lib` by `ProjectReference`:
`samples/InteractiveViewer` and `tests/Lib.Tests` (for the rich keyboard input),
`tests/Parity.Tests` (the old-vs-new bridge, which uses the `Parity` helper), and
`tests/Package.Tests` (conditional). `samples/ParityGallery` was already
repointed onto `Scene` + `SkiaViewer` in Stage 1 and no longer references the
monolith.

The goal of this feature is to remove **every** remaining consumer reference to
`src/Lib`, so the Stage 5 deletion of `src/Lib` and unpublishing of the
`FS.Skia.UI` package is a no-consumer operation. The actual deletion and
unpublishing remain Stage 5 (out of scope here).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Monolith reference-free so Stage 5 can delete it (Priority: P1)

As the maintainer retiring the monolith, I need every sample and test moved off
`src/Lib` so that the Stage 5 deletion of `src/Lib` (and unpublishing of
`FS.Skia.UI`) breaks nothing — the build, the template, and all tests stay green
with no project referencing `Lib`.

**Independent test:** A repository-wide search for references to `Lib.fsproj`
and the `FS.Skia.UI` monolith package (by `ProjectReference` / `PackageReference`)
returns zero hits across `samples/**`, `tests/**`, and `src/**` (excluding
`src/Lib` itself). The full escalated gate set is green.

### User Story 2 - Rich keyboard input available from a focused split package (Priority: P2)

As a consumer building an interactive app, I get the rich, host-coupled keyboard
input capability (modes, bindings, sequences, command intents) from a focused
split package rather than from the broad `FS.Skia.UI` monolith, so my app's
dependency footprint stays light and the V3 dependency-light promise holds for
the keyboard-input path too.

**Independent test:** A sample/app exercising the rich keyboard input restores,
builds, and runs referencing only split packages (no `FS.Skia.UI` monolith); the
migrated keyboard-input test suite is green and asserts behaviour identical to the
pre-move module.

### User Story 3 - Parity scaffolding retired after sign-off (Priority: P3)

As the maintainer, once Stage-1 parity is signed off there is no longer an "old"
host to compare against, so the `Parity.Tests` old-vs-new bridge and the dead
`Parity` helper are retired — with any still-valuable assertions folded into the
split-package test suites first — removing the last piece of old-vs-new
scaffolding while keeping the deterministic scene-output parity oracle.

**Independent test:** `tests/Parity.Tests` is removed (assertions migrated where
valuable); the deterministic scene-output parity check that remains in the
split-package suites is byte-identical to the Stage-0 golden; no test references
`src/Lib`.

### Edge Cases

- **Acyclic-graph hazard when rehoming keyboard input.** The rich keyboard input
  depends on both `FS.Skia.UI.Scene` and `FS.Skia.UI.SkiaViewer.Host`, while
  `SkiaViewer` already depends on the lean `FS.Skia.UI.KeyboardInput` package.
  Its new home MUST be chosen so no back-edge / cycle is introduced (e.g. it may
  not land in the lean `KeyboardInput` package if that would force
  `KeyboardInput → SkiaViewer → KeyboardInput`).
- **Name/namespace collision.** The lean split `FS.Skia.UI.KeyboardInput` package
  and the rich `FS.Skia.UI` keyboard input module carry overlapping concepts; the
  rehome MUST resolve identity so the two do not collide and a consumer can tell
  which it depends on.
- **`Lib.Tests` assertions must travel with the module** to its new home (or be
  retired with justification), not be silently dropped.
- **`ParityGallery` already repointed.** It builds on `Scene` + `SkiaViewer`; this
  feature decides whether it is kept as a supported-capability demo or retired with
  the parity bridge (ADR 0010), and records that decision.
- **`src/Lib` becomes an unreferenced husk** (only `InternalsVisibleTo.fs` plus
  emptied modules). This feature does **not** delete it — Stage 5 does — preserving
  staged revertibility.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The rich keyboard input capability currently in
  `src/Lib/KeyboardInput.fs(i)` MUST be relocated to a split package such that no
  sample or test needs `src/Lib` to consume it.
- **FR-002**: The relocated keyboard input MUST preserve its observable behaviour
  (public function shapes, parsing, mode/binding/sequence semantics, diagnostics):
  no behaviour change, only a change of home.
- **FR-003**: `samples/InteractiveViewer` MUST be repointed off `src/Lib` (both its
  `ProjectReference` to `Lib.fsproj` and its `PackageReference` to `FS.Skia.UI`)
  onto the split package(s) that now host the rich keyboard input.
- **FR-004**: `tests/Lib.Tests` MUST be repointed (or its assertions migrated) so it
  no longer references `src/Lib`; keyboard-input assertions travel with the module.
- **FR-005**: The `Parity.Tests` old-vs-new bridge MUST be retired once Stage-1
  parity is signed off, with still-valuable assertions folded into
  `SkiaViewer.Tests` / `Scene.Tests` first.
- **FR-006**: `tests/Package.Tests` MUST no longer reference `src/Lib` (its
  conditional `Lib.fsproj` reference is dropped).
- **FR-007**: The legacy-sample / `ParityGallery` policy MUST be settled per ADR
  0010 (kept as a supported-capability demo, or retired), the decision recorded;
  after this feature no sample references the `FS.Skia.UI` monolith.
- **FR-008**: The package dependency graph MUST remain acyclic and `FS.Skia.UI.Scene`
  MUST remain FSharp.Core-only; the rehome MUST NOT introduce a back-edge or a new
  heavy dependency into a base package.
- **FR-009**: The per-package public surface of the receiving package MUST be
  recorded in its per-package surface baseline, and `PerPackageSurfaceDiff` MUST be
  clean (no unrecorded `.fsi` drift).
- **FR-010**: After this feature, a repository-wide reference search MUST show
  `src/Lib` has **zero** remaining consumers (no `ProjectReference`/`PackageReference`
  from `samples/**`, `tests/**`, or `src/**` outside `src/Lib`).
- **FR-011**: `src/Lib` MUST NOT be deleted and `FS.Skia.UI` MUST NOT be unpublished
  in this feature; both remain for the Stage 5 closeout (staged revertibility).
- **FR-012**: The generated-consumer contract MUST stay green — `TemplateCheck`,
  `GeneratedProductCheck`, and `GeneratedGuidanceCheck` pass; a generated default
  `app` continues to restore/build/run and does not pull the monolith transitively.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: The package **receiving** the rich keyboard input gains that
  surface (package contents + public surface grow); the `FS.Skia.UI` monolith loses
  its last consumers (it is **not** unpublished here — Stage 5). No new package
  identity is required unless planning concludes a new home package is the only
  acyclic option. The standard post-merge two-commit version-bump + template-pin
  flow applies to any package whose contents change. No `Charts` package migration
  is in scope.
- **Public contract impact**: The receiving package's `.fsi` grows with the rich
  keyboard input types/modules; its **per-package surface baseline** is updated. The
  monolith (`FS.Skia.UI`) per-package/aggregate baseline shrinks as its modules empty
  out. `validation.contract.yml` is unchanged (no `Routing.fs` rule changes here —
  the per-package Route-gating rule is Stage 5).
- **State workflow impact**: The rich keyboard input is a stateful input runtime
  (modes, bindings, sequences, command intents, recovery diagnostics). Its
  state-workflow behaviour MUST be preserved exactly across the relocation; this
  feature changes its home, not its semantics.
- **Layout/rendering impact**: No rendering-architecture change. The rich keyboard
  input couples to `FS.Skia.UI.SkiaViewer.Host` (`ViewerKey`/`ViewerKeyEvent`); the
  relocation MUST keep that coupling without cycles. The deterministic scene-output
  parity oracle MUST remain byte-identical to the Stage-0 golden, which is the
  precondition for retiring `Parity.Tests`. Screenshot/visual re-capture remains
  headless-GPU-infeasible (disclosed; scene-output is authoritative).
- **Evidence obligations**: Parity sign-off evidence (scene-output byte-identical to
  the Stage-0 golden) before `Parity.Tests` retires; `PerPackageSurfaceDiff` baseline
  diff for the receiving package; the migrated keyboard-input test suite green; the
  standard repo-root governance readiness docs and per-feature readiness notes for
  `Route --enforce`; `EvidenceGraph` valid and `EvidenceAudit` PASS with zero
  synthetic.
- **Unsupported scope**: Deleting `src/Lib` and unpublishing `FS.Skia.UI` (Stage 5);
  adding the `PerPackageSurfaceDiff` `Routing.fs` rule + hard-gate enforcement
  (Stage 5); the generated-project cleanliness gate (Stage 5); V2→V3 migration docs
  and after-measurement (Stage 5); the `Charts`/`DataGrid` package split; new template
  profiles; any new rendering architecture or dynamic/plugin loader.
- **Build-target impact**: No new build targets. `PerPackageSurfaceDiff` records the
  receiving package's delta; `TemplateCheck`, `GeneratedProductCheck`,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` must
  remain green. `Route` selects the escalated gate set (the change touches
  `src/**/*.fsi`). `Dev` must be green (full test suite, including the migrated
  keyboard-input tests).

## Success Criteria *(mandatory)*

- **SC-001**: No sample project references `src/Lib` (`Lib.fsproj`) or the
  `FS.Skia.UI` monolith package — a named repo-wide reference search over
  `samples/**` returns zero hits.
- **SC-002**: No test project references `src/Lib`; `tests/Parity.Tests` is removed
  (valuable assertions migrated to `SkiaViewer.Tests`/`Scene.Tests`).
- **SC-003**: A sample/app exercising the rich keyboard input restores, builds, and
  runs against split packages only (no `FS.Skia.UI` monolith reference).
- **SC-004**: The relocated keyboard input behaves identically to the pre-move module
  — its migrated test suite is green — and the deterministic scene-output parity check
  remains byte-identical to the Stage-0 golden (justifying the `Parity.Tests`
  retirement).
- **SC-005**: The package dependency graph is acyclic, `FS.Skia.UI.Scene` is still
  FSharp.Core-only, and no back-edge was introduced by the rehome (verified from
  project references).
- **SC-006**: The receiving package's per-package surface baseline is updated and
  `PerPackageSurfaceDiff` is clean; `validation.contract.yml` is unchanged.
- **SC-007**: `src/Lib` has **zero** remaining consumers repo-wide, yet `src/Lib` is
  still present and `FS.Skia.UI` is still packable (deletion/unpublish deferred to
  Stage 5).
- **SC-008**: The full escalated gate sequence is green and `EvidenceAudit` returns
  PASS with zero synthetic tasks.

## Assumptions

- The deterministic scene-output parity oracle established in Stage 0 and retained in
  Stage 1 is the authoritative parity sign-off mechanism; reference-screenshot
  re-capture is headless-GPU-infeasible and remains corroboration-only (carried from
  Stages 0–1).
- The lean `FS.Skia.UI.KeyboardInput` package and the rich keyboard input module are
  distinct capabilities; the rich module's permanent home is decided in planning under
  the acyclic-graph constraint (the spec requires *a* split-package home, not a
  specific one).
- `samples/ParityGallery` already builds on `Scene` + `SkiaViewer`; the only open
  question is keep-vs-retire policy, not repointing.
- Stage 5 owns the deletion of `src/Lib`, the unpublishing of `FS.Skia.UI`, the
  per-package Route-gating rule + enforcement, the generated-project cleanliness gate,
  and the migration docs / after-measurement.
- The post-merge version-bump + template-pin flow (per Stage 2's outcome) applies on
  merge for any package whose contents change.

## Dependencies

- **Stage 1 (feature 050)** — host extracted, parity oracle repointed onto `Scene`,
  rich keyboard input retyped onto `Scene` + `SkiaViewer.Host`. Done.
- **Stage 2 (feature 051)** — `AgentValidation` relocated. Done.
- **Stage 1 parity sign-off** must hold before `Parity.Tests` retires (FR-005).
- **Blocks Stage 5** — the Stage 5 deletion requires `src/Lib` reference-free (this
  feature's FR-010 / SC-007).

## Implementation outcome (2026-06-02) — scope deviation

Implemented; all `Route` gates green (`agent-ready`: `Dev`, `PackageSurfaceCheck`, `FsiTranscripts`,
`GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit` **verdict=PASS**, plus
`PerPackageSurfaceDiff`); zero synthetic. The rich input runtime rehomed to the new
`FS.Skia.UI.Input` package (99% rename, namespace line only); `Input.Tests` 12/12; `InteractiveViewer`
+ `Lib.Tests` off `Lib`; the `Parity.Tests` old-vs-new bridge (`Tests.fs`) retired with the
Scene-only scene-output oracle preserved (4/4 byte-identical to the Stage-0 golden).

**Deviation (maintainer-confirmed): the monolith-decommission cluster defers to Stage 5.** Discovery
during implementation: `tests/Package.Tests` is a deliberate *packaging-contract* consumer that
asserts the still-published `FS.Skia.UI` surface via `typeof<FS.Skia.UI.ParityReport>.Assembly` (the
`Parity` helper) and the `PackLocal` wiring. Removing the `Parity` helper (original FR-005 tail) would
break it, and dropping its `Lib` reference would gut contract tests that must pass while `FS.Skia.UI`
is published (FR-011). These form one coupled unit retired at Stage 5. Net amendments:

- **FR-005 (amended):** retire only the `Parity.Tests` *bridge* (`Tests.fs`) + the `Lib` reference;
  **keep** the `Parity` helper (`src/Lib/Library.fs(i)`) — it retires with the monolith at Stage 5.
- **FR-006 (amended):** `Package.Tests` retains its `Lib` reference (the packaging contract); only
  `Lib.Tests` is decoupled this feature.
- **FR-010 / SC-007 (amended):** every keyboard-input + parity-bridge consumer is off `Lib`; the
  **only** remaining `src/Lib` consumer is `Package.Tests`. "Fully reference-free" is therefore a
  Stage 5 outcome — it cannot hold while `FS.Skia.UI` is a published package under packaging tests.

See `readiness/no-consumer-grep.md` for the residual-consumer record.
