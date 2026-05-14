# Tasks: Yoga.Net Layout for UI Elements and Widgets

**Feature branch**: `005-add-yoga-net-layout`
**Spec**: `specs/005-add-yoga-net-layout/spec.md`
**Plan**: `specs/005-add-yoga-net-layout/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a user-facing entry point and that path was actually exercised: FSI
against the built layout package, an automated semantic test through the
public `FS.Skia.UI.Layout` surface, a sample smoke transcript, or another
checked-in readiness artifact under `specs/005-add-yoga-net-layout/readiness/`.

For this feature, the automatic layout evaluator is pure library behavior, so
Principle IV does not require a public `Model` / `Msg` / `Effect` contract for
the evaluator itself. Stateful host resize, widget updates, and measurement
invalidation are covered at the interpreter boundary by sample smoke evidence,
semantic tests, and readiness transcripts.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- **[T1]** / **[T2]** — Tier 1 vs Tier 2 annotation, omitted here because the feature is Tier 1 overall

Every task has a matching entry in `tasks.deps.yml`.

---

## Phase 1: Setup

- [X] T001 Confirm the current `src/Layout` compile order, public `.fsi` boundaries, test projects, and sample entry points affected by automatic layout.
- [X] T002 [P] Add the pinned `Yoga.Net` `3.2.3` package reference to `src/Layout/Layout.fsproj` without exposing Yoga.Net types in public signatures.
- [X] T003 [P] Create readiness scaffolding under `specs/005-add-yoga-net-layout/readiness/` for FSI, logs, performance, sample smoke, and surface baselines.
- [X] T004 Record Tier 1 evidence obligations, dependency pinning rationale, MVU applicability, and unsupported v1 scope in `specs/005-add-yoga-net-layout/readiness/evidence-obligations.md`.

**Checkpoint**: Setup ready.

---

## Phase 2: Foundation

- [X] T005 Draft automatic layout geometry, sizing, visibility, measurement, diagnostics, result, and pixel snapping types in `src/Layout/Types.fsi`.
- [X] T006 Draft default constructors and public evaluator functions in `src/Layout/Layout.fsi` or `src/Layout/YogaLayout.fsi`, including `evaluate`, `evaluateIncremental`, `renderComputed`, `snapBounds`, and `hitTestComputed`.
- [X] T007 [P] Add matching implementation placeholders and compile-order entries in `src/Layout/Types.fs`, `src/Layout/Layout.fs`, and any new Yoga layout implementation files.
- [X] T008 [P] Add semantic test helpers in `tests/Layout.Tests/Tests.fs` for reading computed bounds, asserting non-overlap, deterministic results, diagnostics, and visibility.
- [X] T009 [P] Add `scripts/layout-prelude.fsx` coverage for the intended public automatic layout API and expected readiness transcript path.
- [X] T010 [P] Add or update package surface baseline generation notes for `FS.Skia.UI.Layout` under `specs/005-add-yoga-net-layout/readiness/surface-baselines/`.
- [X] T011 Define validation rules for duplicate node ids, invalid available space, invalid numeric style values, invalid measurement output, min/max conflicts, and v1 rejection of absolute or overlay intent.
- [X] T012 Define the Yoga.Net adapter boundary so Yoga nodes are allocated, styled, measured, evaluated, read back, and disposed without leaking Yoga.Net types through `.fsi`.
- [X] T013 Define the deterministic fallback geometry policy for recoverable failures, including bounded root and child rectangles plus `LayoutDiagnostic` records.
- [X] T014 Define the invalidation model for node intent, visibility, child structure, parent size, and content measurement changes, including stable unchanged sibling bounds.
- [X] T015 Build the foundation surface and capture an initial FSI or build transcript proving the public signatures are reachable from the package.
- [X] T052 Draft the stateful host/sample layout workflow contract for resize, widget updates, and content-measurement invalidation, including `Model`, `Msg`, `Effect` or `Cmd<Msg>`, `init`, `update`, and interpreter boundary.
- [X] T053 Add pure transition tests for the host/sample layout workflow covering resize, visibility changes, layout-intent changes, and content-measurement changes.
- [X] T054 Add emitted-effect assertions and real interpreter evidence for the host/sample layout workflow where safe, saving transcript output under `readiness/fsi/`.

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) - Arrange Child Elements Automatically

### Tests First

- [X] T016 [P] [US1] Add semantic tests for row, column, and wrap containers with non-overlapping child bounds inside the parent.
- [X] T017 [P] [US1] Add semantic tests for parent padding, child margin, row and column gaps, and main/cross-axis alignment.
- [X] T018 [P] [US1] Add semantic tests for fixed, min, max, flex grow, flex shrink, flex basis, and deterministic repeated evaluation.
- [X] T019 [P] [US1] Add public API tests for custom content measurement callbacks that return preferred logical sizes and diagnostics.

### Implementation

- [X] T020 [P] [US1] Implement default automatic layout records and helper constructors for layout intents, nodes, available space, and pixel snap policies.
- [X] T021 [US1] Implement Yoga.Net style mapping for direction, wrap, alignment, justification, padding, margin, gap, fixed/min/max size, grow, shrink, and basis.
- [X] T022 [US1] Implement recursive layout tree evaluation through Yoga.Net and read back one logical `ComputedBounds` entry per participating node.
- [X] T023 [US1] Implement custom leaf measurement callback bridging between public `MeasureRequest` / `MeasureResponse` and Yoga.Net measure callbacks.
- [X] T024 [US1] Implement deterministic result ordering, finite logical bounds normalization, and valid-layout non-overlap guarantees.
- [X] T025 [US1] Capture US1 readiness evidence through `dotnet test --filter` or equivalent and the FSI prelude transcript for nested element layout.

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2) - Compose Standard Widgets

### Tests First

- [X] T026 [P] [US2] Add semantic tests for mixed standard widgets and custom elements participating in one automatic layout tree.
- [X] T027 [P] [US2] Add resize and incremental evaluation tests proving changed parent size or child measurement updates bounds without stale overlap.
- [X] T028 [P] [US2] Add invalidation locality tests proving unaffected sibling subtrees keep byte-for-byte equivalent computed bounds after unrelated changes.
- [X] T029 [P] [US2] Add render and hit-test tests proving computed logical bounds are consumed without independent layout recalculation.

### Implementation

- [X] T030 [P] [US2] Add fixtures or adapters that let existing element/widget scene content attach to `LayoutNode.Content` while keeping automatic layout opt-in.
- [X] T031 [US2] Implement `evaluateIncremental` revision handling, invalidated node reporting, and changed-node ancestor propagation.
- [X] T032 [US2] Implement `renderComputed` so scene content is positioned from computed bounds while existing manual stack, dock, graph, absolute, and overlay composition keep working.
- [X] T033 [US2] Implement `snapBounds` and `hitTestComputed` with one deterministic `PixelSnapPolicy` shared by rendering and hit testing.
- [X] T055 [US2] Document and test keyboard focus region alignment with computed visual bounds and pointer hit-test bounds after pixel snapping.
- [X] T034 [US2] Add or update `samples/LayoutGraphGallery` and `samples/DemoReel` automatic layout examples for nested elements, mixed widgets, resizing, flexible sizing, and hidden elements.
- [X] T035 [US2] Capture US2 readiness evidence through widget/invalidation tests and sample smoke transcript under `readiness/sample-smoke/`.

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 (US3) - Diagnose Layout Problems

### Tests First

- [X] T036 [P] [US3] Add semantic tests for invalid available space, invalid style values, min/max conflicts, and size requests larger than the parent.
- [X] T037 [P] [US3] Add semantic tests for unmeasurable content, invalid measurement callback output, hidden nodes, and collapsed nodes.
- [X] T038 [P] [US3] Add tests proving recoverable failures return structured diagnostics and bounded fallback geometry without terminating render flow.

### Implementation

- [X] T039 [P] [US3] Implement structured diagnostic creation with node id, code, severity, message, constraint, and fallback flags.
- [X] T040 [US3] Implement validation and normalization for available space, layout intent values, measurement output, and unsupported automatic-layout scope.
- [X] T041 [US3] Implement safe fallback bounds for recoverable constraints and propagate diagnostics through `LayoutResult.Diagnostics`.
- [X] T042 [US3] Implement hidden and collapsed node behavior so visibility diagnostics are distinguishable from layout failures and visible siblings stay stable.
- [X] T043 [US3] Capture US3 readiness evidence for invalid/conflicting layouts and diagnostic samples.

**Checkpoint**: User Story 3 is fully functional and testable independently.

---

## Phase 6: Integration & Polish

- [X] T044 Refresh `FS.Skia.UI.Layout` surface-area baseline and package tests for the new public contract.
- [X] T045 [P] Update `specs/005-add-yoga-net-layout/quickstart.md` and layout interaction docs for final public names, pointer hit testing, keyboard focus regions, visual bounds, and pixel snapping behavior.
- [X] T046 [P] Add performance evidence for representative 200-node resize/re-layout under `readiness/performance/yoga-layout-200-node-resize.txt`, including command, hardware/runtime profile, iteration count, median/p95 timing, and pass/fail against the SC-004 threshold.
- [X] T047 Run `dotnet restore`, `dotnet build`, and `dotnet test`, saving final logs under `readiness/logs/`.
- [X] T048 Run the layout FSI prelude and save `readiness/fsi/yoga-layout-prelude.txt`.
- [X] T049 Run automatic layout sample smoke checks and save `readiness/sample-smoke/automatic-layout-gallery.txt`.
- [X] T050 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/005-add-yoga-net-layout --graph-only` and confirm no dangling refs, cycles, or unexpected `[S*]` propagation.
- [X] T051 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/005-add-yoga-net-layout` and document PASS or every explicit synthetic-evidence override.

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |
