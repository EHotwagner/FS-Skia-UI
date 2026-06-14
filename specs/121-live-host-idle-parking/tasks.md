# Tasks: Live Host Pacing, Surface Honesty & Viewer Ergonomics

**Feature branch**: `121-live-host-idle-parking`
**Spec**: `specs/121-live-host-idle-parking/spec.md`
**Plan**: `specs/121-live-host-idle-parking/plan.md`

## Status Legend

- `[ ]` pending · `[X]` done (real evidence) · `[S]` synthetic-only (disclose) ·
  `[F]` failed · `[-]` skipped (with rationale). `[S*]` is computed by the audit, never
  written by hand.

## Scope note (reconciled to shipped truth)

Live unchanged-frame paint-skip (feature 120) and graceful quit via the `CloseWindow`
`ViewerEffect` are **already shipped**; this feature documents/reconciles them and does
**not** re-implement them. The build work is: a consumer frame-cap on `ViewerOptions`
that gates render cadence (US1), an allocation-free idle clock tick (US2), and published
pointer surface + viewer-host guidance (US3). The persistent window is not drivable in
headless CI — loop changes are proven on extracted pure decisions + reasoning and the
free-run is recorded as an environment limitation, never an interactive pass.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold `specs/121-live-host-idle-parking/readiness/` placeholders discoverable before implementation (`runtime-limitations.md`, `evidence-graph.md`, `evidence-audit.md`, `generated-validation.md`), each naming its authoritative command, artifact path, failure class, and next action; link spec + plan
- [X] T002 [skillist: []] Record feature Tier (T1 — public `ViewerOptions` `.fsi` escalates), affected layers (`src/SkiaViewer`, `src/Controls.Elmish`, `docs/api-surface`, `.agents/skills`), public-API impact (additive defaulted field), and that MVU contract is unchanged (the `CloseWindow` quit path is already wired)

---

## Phase 2: Foundation

- [X] T003 [skillist: fs-skia-skiaviewer] Draft the `ViewerOptions` `.fsi` change in `src/SkiaViewer/SkiaViewer.fsi` — additive defaulted `FrameRateCap: int option` plus the defaulting construction path — per `contracts/viewer-options.md`, and the signature of the extracted pure pacing decision `shouldAdvanceFrame`
- [X] T004 [skillist: fs-skia-skiaviewer] Exercise the drafted `ViewerOptions` surface from FSI (prelude or ad-hoc), including a `FrameRateCap = Some n` / `None` construction, and capture the transcript to `readiness/fsi-session.txt`
- [X] T005 [skillist: []] Record/refresh the surface-area baseline expectation for the changed `SkiaViewer` public module so the intentional additive delta is captured (no behavior baseline change)

**Checkpoint**: Foundation ready — stories may proceed.

---

## Phase 3: User Story 1 — consumer frame-cap (US1, P1)

### Tests First

- [X] T006 [P] [US1] [skillist: fs-skia-skiaviewer] Failing-first unit tests for the pure `shouldAdvanceFrame` pacing decision: cap `n` bounds advances/second (cadence ≤ cap), a larger interval yields strictly fewer advances, the first frame always advances (SC-001); and `validateOptions` rejects `FrameRateCap = Some n, n <= 0` with a startup diagnostic (SC-005)

### Implementation

- [X] T007 [US1] [skillist: fs-skia-skiaviewer] Add `FrameRateCap: int option` to `ViewerOptions` (`.fsi` + `.fs`) with the defaulting path; thread it into `ViewerConfiguration.TargetFrameRate` at `SkiaViewer.fs:1232-1236` (replacing literal `Some 60`); extend `validateOptions` to reject non-positive caps (FR-001/FR-003)
- [X] T008 [US1] [skillist: fs-skia-skiaviewer] In `src/SkiaViewer/Host/OpenGl.fs` `runEventLoop`, extract `shouldAdvanceFrame` and gate **both** `DoUpdate()` and `DoRender()` by it so the cap bounds render cadence (FR-002), preserving `Thread.Sleep(1)` and feature-120 paint-skip
- [X] T009 [P] [US1] [skillist: []] Update every `ViewerOptions` construction site (samples, `scripts/*-prelude.fsx`, tests) for the new field so the repo compiles; confirm omitting the cap is byte-identical (FR-008/SC-002)
- [X] T010 [US1] [skillist: []] Document the US1 independent validation path (the pacing-decision unit test + the headless-undrivable-window caveat) in `readiness/runtime-limitations.md`

**Checkpoint**: US1 functional and independently testable.

---

## Phase 4: User Story 2 — allocation-free idle tick (US2, P2)

### Tests First

- [X] T011 [P] [US2] [skillist: fs-skia-controls-host] Failing-first unit tests for the clock-advance guard: no active clock ⇒ result is reference-equal to input (no allocation, SC-003); ≥1 active clock ⇒ each active clock advances by the delta exactly as today (features 099/103 unchanged)

### Implementation

- [X] T012 [US2] [skillist: fs-skia-controls-host] In `src/Controls.Elmish/ControlsElmish.fs` `wrappedTick`, guard the `StateByIdentity |> Map.map (advance)` with a `Map.exists (clock active)` check, leaving `retained.Value` unchanged when no clock is active (FR-004) — internal, no `.fsi` impact

**Checkpoint**: US2 functional and independently testable.

---

## Phase 5: User Story 3 — surface honesty (US3, P3)

- [X] T013 [P] [US3] [skillist: fs-skia-skiaviewer] Publish `PointerInteraction`, `PointerButton`, and `ViewerPointerPhaseKind` (and the `MapPointer`/`MapKeyChord` folding note) under `docs/api-surface/` per `contracts/api-surface.md`, and wire/extend a drift check that fails if the published shape diverges from the `.fsi` (FR-005)
- [X] T014 [P] [US3] [skillist: fs-skia-viewer-host] Extend the canonical `.agents/skills/fs-skia-viewer-host` skill with present-mode selection (`DirectToSwapchain` live vs `OffscreenReadback` evidence + don't-reuse-evidence-options warning), the new frame-cap lever, the no-compositor free-run environment limit, and the reconciliation facts (live paint-skip + quit-via-`CloseWindow` already shipped); regenerate the `.claude` peer via `RefreshSurfaceBaselines` (FR-006/FR-007)
- [X] T015 [US3] [skillist: fs-skia-evidence-mode] Finalize `readiness/runtime-limitations.md`: the persistent live window free-runs on a no-compositor host (environment limitation, not a defect), with the frame-cap as the consumer mitigation; record that no interactive-window pass is claimed (FR-008)

**Checkpoint**: US3 functional and independently verifiable.

---

## Phase 6: Integration & Polish

- [X] T016 [skillist: fs-skia-skiaviewer] Run `Route` first; then the serialized FAKE-backed gate set it prints (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`) plus `RefreshSurfaceBaselines`, sequentially in deterministic order; record the non-authoritative aggregate result
- [X] T017 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; capture before/after graph paths
- [X] T018 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with 0 synthetic (no `--accept-synthetic` expected); write `readiness/evidence-audit.md` verdict token

---

## Synthetic-Evidence Inventory

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none expected — all evidence is real unit tests + docs; the live window is honest non-coverage recorded in runtime-limitations, not synthetic)_ | | | | | | | | |
</content>
