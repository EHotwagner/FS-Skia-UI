# Tasks: Spread3 Consumer Feedback Remediation

**Feature branch**: `122-spread3-consumer-feedback`
**Spec**: `specs/122-spread3-consumer-feedback/spec.md`
**Plan**: `specs/122-spread3-consumer-feedback/plan.md`

## Status Legend

- `[ ]` pending · `[X]` done (real evidence) · `[S]` synthetic-only (disclose) ·
  `[F]` failed · `[-]` skipped (with rationale). `[S*]` is computed by the audit, never
  written by hand.

## Scope note (reconciled to shipped truth — see research.md)

The dogfood-verify pass narrowed scope. **Already-shipped / refuted, NOT rebuilt:**
window startup-state selection exists at the viewer layer (`ViewerWindowBehaviorRequest.StartupState`
+ `runInteractiveViewerWithWindowBehavior`); the "live vs screenshot divergence" was refuted
(both use the same `paintNode`). **Deferred:** FR-004 public present-sync/buffer-count knobs (FR-001
removes the need; an internal `bufferFillDepth` covers it); behavioral CustomControl painting.
**Built (genuine gaps):** FR-001/002 present-path buffer-fill; FR-005 controls/template
window-behavior threading (FR-003 satisfied through it); FR-006/007 CustomControl guard + honesty;
FR-008/009/010/011/012 doc/governance/skill edits.

The Wayland windowed-fullscreen **visual** blink is not reproducible in headless/Mesa CI; it is a
disclosed `[-]` observation (T016) with rationale, NOT `[S]` synthetic. Real evidence is the pure
`planPresent` golden + the present-action host log (no undrawn buffer) + offscreen byte-identical
goldens.

## Task Annotations

`[P]` parallel-safe · `[US*]` story scope · `[T1]`/`[T2]` tier · `[SEH]` synthetic error-handling.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold the feature directory and link spec + plan (done in specify/plan; confirm `research.md`/`data-model.md`/`contracts/`/`quickstart.md` present)
- [X] T002 [P] [skillist: []] Record adoption note: no dependency change — `Directory.Packages.props`/`DependencyReport` untouched; CustomControl property-style test uses hand-rolled deterministic loops (no FsCheck add)
- [X] T003 [P] [skillist: []] Scaffold `specs/122-spread3-consumer-feedback/readiness/` audit-enforced placeholders (governance-risk-levels, aggregate-hang-diagnostics, runtime-limitations, generated-validation, skill-loading-evidence, interactive-visible-window, window-state-diagnostics, real-image-evidence, evidence-graph, evidence-audit) discoverable before implementation
- [X] T004 [skillist: []] Record feature Tier (T1 additive `.fsi`), affected layers (SkiaViewer host, Controls.Elmish, Controls, template, governance docs, skills), public-API impact (one additive overload + `PresentAction`/`planPresent` test seam), MVU applicability (host-loop/launch — `update` contract unchanged), and evidence obligations

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-controls-host, fs-skia-viewer-host] Draft the public `.fsi`: `ControlsElmish.runInteractiveAppWithWindowBehavior` (ControlsElmish.fsi, XML-doc) and `PresentAction` DU + `GlHost.planPresent` (OpenGl.fsi test seam, attr→doc→type order)
- [X] T006 [P] [skillist: []] Author `readiness/governance-risk-levels.md` — small/medium/broad levels; this change is **broad** (template + public `.fsi` + governance docs + skills) → maintainer-verify focused validation; record non-authoritative aggregate handling
- [X] T007 [P] [skillist: fs-skia-evidence-mode] Author `readiness/runtime-limitations.md` + `readiness/aggregate-hang-diagnostics.md`: Wayland windowed-fullscreen visual blink not reproducible headless; FR-004 knobs deferred; routed-gate sequential order (FAKE not concurrency-safe)
- [X] T008 [skillist: fs-skia-viewer-host] Exercise the draft `.fsi` from FSI (`planPresent` truth table; overload signature shape) and capture the transcript to `readiness/fsi-session.txt`
- [X] T009 [skillist: []] Record surface-area baselines for the changed public modules (Controls.Elmish top-level + SkiaViewer.Host) pre-change for the Tier-1 diff
- [X] T010 [skillist: fs-skia-evidence-mode] Record unsupported-scope handling and failure diagnostics: present-path observability (`representedCount`/`skippedPresentCount`); offscreen path untouched

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 — present path, no black buffer (US1, FR-001/002)

### Tests First

- [X] T011 [P] [US1] [skillist: fs-skia-viewer-host] Failing-first `planPresent` golden: for `bufferFillDepth=3` a (change, then static…) sequence yields `[PaintAndPresent; RepresentLastGood; RepresentLastGood; SkipPresent; SkipPresent; …]` (SC-001)
- [X] T012 [P] [US1] [skillist: fs-skia-viewer-host, fs-skia-evidence-mode] Failing-first present-action host-log test: a static scene presents a populated buffer every frame (never an undrawn/black buffer) AND steady-state reaches `SkipPresent` (idle preserved); plus an offscreen byte-identical golden (readback path untouched) (SC-004)

### Implementation

- [X] T013 [P] [US1] [skillist: []] Present-sequence fixtures (scene-change cadences: static, single-change-then-static, alternating) for T011/T012
- [X] T014 [US1] [skillist: fs-skia-viewer-host] Implement `PresentAction` + pure `GlHost.planPresent` in `OpenGl.fs`/`OpenGl.fsi` (reusing `shouldPresent`)
- [X] T015 [US1] [skillist: fs-skia-viewer-host] Wire the bounded re-present in `GlHost.run`: cache `lastGoodFrame` (`surface.Snapshot()`, dispose prior) after each paint; on `RepresentLastGood` blit the cached frame + Flush + SwapBuffers (no scene walk); track `idleRepresentsRemaining`/`representedCount`/`bufferFillDepth`; `SkipPresent` stays full idle (FR-002)
- [-] T016 [US1] [skillist: fs-skia-skiaviewer] (disclosed skip — Wayland windowed-fullscreen visual not reproducible headless; rationale in readiness/runtime-limitations.md + real-image-evidence.md) Persistent graphical launch: confirm the live `DirectToSwapchain` host is reachable from the default executable path and exercises the new present plan; disclosed `[-]` Wayland windowed-fullscreen visual no-blink observation (not reproducible in headless/Mesa CI — rationale in `readiness/real-image-evidence.md`) (SC-001)
- [X] T017 [US1] [skillist: fs-skia-evidence-mode] Author `readiness/interactive-visible-window.md`, `readiness/window-state-diagnostics.md`, `readiness/real-image-evidence.md` for the present-path change (key=value token form per FR-008)

**Checkpoint**: US1 present path verified by pure golden + host log; live visual disclosed.

---

## Phase 4: User Story 2 — window-behavior threading (US2, FR-003/005)

### Tests First

- [X] T018 [P] [US2] [skillist: fs-skia-controls-host] Failing-first parity test: `runInteractiveAppWithWindowBehavior options Viewer.defaultWindowBehavior host` is byte-identical to `runInteractiveApp options host` (default path unchanged) (SC-004)
- [X] T019 [P] [US2] [skillist: fs-skia-template-update] Failing-first generated `Program.fs` threading verification: with a window flag supplied, the app-profile launch routes through `runInteractiveAppWithWindowBehavior` (flag reaches the live launch, not only `manualWindowOptionResults`) (SC-003)

### Implementation

- [X] T020 [P] [US2] [skillist: []] Window-behavior threading fixtures (parsed `--window-startup normal` → `ViewerWindowBehaviorRequest`)
- [X] T021 [US2] [skillist: fs-skia-controls-host] Implement `ControlsElmish.runInteractiveAppWithWindowBehavior` (delegates to `Viewer.runInteractiveViewerWithWindowBehavior`; `runInteractiveApp` unchanged)
- [X] T022 [US2] [skillist: fs-skia-template-update] Update template `Program.fs` app profile to call `runInteractiveAppWithWindowBehavior viewerOptions windowBehaviorRequest interactiveHost` when `windowFlagSupplied args`, else `runInteractiveApp` (mirrors game branch; no-flag default byte-identical)

**Checkpoint**: US2 — `--window-startup normal` applies to the live controls window.

---

## Phase 5: User Story 3 — CustomControl honesty + NRE guard (US3, FR-006/007)

- [X] T023 [P] [US3] [skillist: fs-skia-ui-widgets] Failing-first test: `CustomControl.validate`/`create` with a real null `Id` and null `Effects` entries returns a validation diagnostic and does NOT throw (NRE) (SC-005). Real evidence — actual null values through the real functions, no mocks/fakes; reclassified from the task-gen `[SEH]` because the null input is real, representative, and feasible (so not synthetic).
- [X] T024 [US3] [skillist: fs-skia-ui-widgets] Implement null guards in `CustomControl.fs` (`String.IsNullOrWhiteSpace` for `Id`/effects; guard the `Accessibility.defaultFor … Id` argument)
- [X] T025 [US3] [skillist: fs-skia-ui-widgets] Correct `Catalog.fs` `custom-control` purpose to the honest statement (renderTree/preview paints a labeled placeholder; build must-show geometry from primitive controls); regenerate `docs/controls-catalog.md`; update any stale test asserting the old string (SC-005)

**Checkpoint**: US3 — CustomControl never NREs; catalog/docs are honest about placeholder rendering.

---

## Phase 6: User Stories 4 & 5 — readiness token + scaffold/skill traps (US4/US5, FR-008/009/010/011/012)

- [X] T026 [P] [US4] [skillist: fs-skia-evidence-mode] Update `template/base/docs/evidence-formats.md`: render the required tokens for `interactive-visible-window.md` (`status=…  mode=…  window-visible=…  accessible-window=…  first-frame-presented=…  self-closed-for-evidence=…`) and `generated-validation.md` (`exact-package-match=…  generated-tests-ran=…  authoritative=…  failure-class=…`) in explicit `key=value` form, noting these files are key/value-parsed (SC-006)
- [X] T027 [P] [US5] [skillist: []] Add the additive-files note to `template/base/docs/scaffold-map.md`: new source files may be added provided the six scanned files (`Model.fs → View.fs → LayoutEvidence.fs → WindowOptions.fs → EvidenceCommands.fs → Program.fs`) keep their relative compile order (SC-007)
- [X] T028 [P] [US5] [skillist: []] Update `.specify/templates/tasks-template.md` widgets hint: the directory is `fs-skia-ui-widgets` but the resolved `name:` in a generated product is the project-prefixed form (e.g. `<project>-widgets`) — use the resolved `name:` in `skillist` ids (SC-007)
- [X] T029 [P] [US5] [skillist: fs-skia-viewer-host] Add the interleaved-black-frame section to `.agents/skills/fs-skia-viewer-host/SKILL.md` (Wayland `DirectToSwapchain`): framework now keeps swapchain buffers populated (FR-001); `--window-startup normal` now applies to controls apps (FR-005); mark the prior "size-aware view" advice as a **blur** fix only and warn the full-extent grid is an O(cells) ANR trap
- [X] T030 [P] [US5] [skillist: fs-skia-ui-widgets] Add the CustomControl placeholder note (FR-007) and the no-new-dependency property-test pattern note (FR-012) to `.agents/skills/fs-skia-ui-widgets/SKILL.md` and mirror into `template/product-skills/fs-skia-ui-widgets/SKILL.md`

**Checkpoint**: US4/US5 — readiness token shape documented; scaffold/skill authoring traps removed.

---

## Phase 7: Integration & Polish

- [X] T031 [skillist: fs-skia-template-update] Regenerate the `.claude/**` skill mirrors (`./fake.sh build -t RefreshSurfaceBaselines`) and confirm `SkillSyncCheck` green
- [X] T032 [skillist: []] Surface-area baseline refresh (Tier 1): per-package + top-level baselines for the new `runInteractiveAppWithWindowBehavior` and `PresentAction`/`planPresent`
- [X] T033 [skillist: []] Author `readiness/skill-loading-evidence.md` (one row per task,skill) + `readiness/selected-skills.md`; `readiness/generated-validation.md` (key=value form) and `readiness/evidence-graph.md`
- [X] T034 [skillist: fs-skia-template-update] Run the routed gate set sequentially: `./fake.sh build -t Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` (FAKE not concurrency-safe) + the controls/package-surface gates `Route` prints
- [X] T035 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; record before/after `readiness/evidence-graph.md`
- [X] T036 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS, 0 synthetic; record `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |

> This feature ships **zero synthetic evidence**. T023 was reclassified from the task-gen `[SEH]`
> to real `[X]`: it drives actual null values through the real `CustomControl.validate`/`create`
> (no mocks/fakes), and a real null is representative/feasible input, so it is not synthetic.
> T016's Wayland windowed-fullscreen visual observation is `[-]` (disclosed skip — not reproducible
> in headless CI), not `[S]`.
