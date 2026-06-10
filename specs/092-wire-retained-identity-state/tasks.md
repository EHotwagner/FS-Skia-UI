# Tasks: Wire Retained Identity Into Live Interactive State (092 / E2)

**Feature branch**: `092-wire-retained-identity-state`
**Spec**: `specs/092-wire-retained-identity-state/spec.md`
**Plan**: `specs/092-wire-retained-identity-state/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. It still remains `[S]` when completed
with synthetic-only malformed-input or explicit error-path evidence. The
classification is assigned here (task generation); implementation-time
relabeling is forbidden.

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when the behavior is reachable and was
actually exercised through the **real adapter seam** (an FSI session, a captured
`readiness/` artifact, a real test run over the wired focus/text/render path) —
**never** by hand-seeding the identity-keyed state map (the exact gap this
feature closes). For this feature all US evidence is capturable
**headless/offscreen** (structural `Scene`/identity equality, before/after state
diff) — **no live Vulkan window is required** ([[fs-skia-evidence-mode]],
render-only honesty). Internal-module/core changes alone do not satisfy `[X]`
for a `[US*]` task; the wired path must actually be driven end to end.

This feature is **Tier 1 (contracted)**: public surfaces move — `SkiaViewer.fsi`
(`InteractiveViewerHost.MapKey` widens to `'msg list`, FR-006),
`ControlsElmish.fsi` (focus-routing seam re-keyed onto the retained structure),
and the internal `RetainedRender.fsi` (work-reduction + theme + first-frame
contract). The consumer `view`/`update` stay pure; the only mutation is the
interpreter-edge focus/text/clock state, re-keyed from `ControlId` to
`RetainedId` (constitution III).

## Success-criterion → assertion mapping

Each headline SC is paired with a concrete enforcing assertion so it cannot be
silently violated while gates stay green: **SC-001** → live-survival test driving
the real focus→keystroke→shift→keystroke seam (no `StateByIdentity` seeding) with
a rebuild-every-frame baseline that **fails** the same proof; **SC-002** →
keyed/unkeyed/keyed-container-wrapped focus-resolution + pre-filled multi-line
first-keystroke append + every-matching-change-binding dispatched (FR-006),
verified for 100% of cases; **SC-003** → measured
`RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount < BaselineNodeCount`
under a sibling-shifting change; **SC-004** → wired round-trip byte-identity over
≥1,000 generated frame pairs **and** a chained 3+-frame sequence; **SC-005** →
frame-0 `KeyCollision` reaches the `ControlDiagnostic` channel (de-duped) and the
first frame paints exactly once; **SC-006** → second-frame output byte-identical
to a full rebuild under the new theme (theme in the reuse key, no stale fragment);
**SC-007** → the four 067/091 invariants (totality, determinism, identity-at-rest,
round-trip) still pass on the wired path.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- **[T1]** — the whole feature is **Tier 1 (contracted)** by routing (public `.fsi`
  deltas); per-task tier annotations are omitted because none differs from the
  feature tier
- **[SEH]** — design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml`. Every line mirrors its
structured `skillist` as `[skillist: ...]` (`[skillist: []]` when none).

## Governance risk levels

- **Small (framework-internal):** the default routing for a pure `src/Controls/**/*.fs`
  edit — focused `Dev` only.
- **Medium:** `src/Controls/**`, `src/Controls.Elmish/**`, `src/SkiaViewer/**`
  content changes — escalate to the controls-public-surface / package-surface
  rules even with zero public-surface delta.
- **Broad (maintainer-verify):** this feature, because public `.fsi` signatures
  move (`SkiaViewer.MapKey`, `ControlsElmish` focus seam) — `Route` is expected to
  **escalate** to the consumer-contract tier, so the serialized FAKE-backed order
  (`Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck →
  EvidenceGraph → EvidenceAudit`) is required, run sequentially (shared `.fake`
  state — never concurrent). Broad validation is required whenever `Route` prints
  the escalation. Aggregate results that cannot be authoritatively reproduced
  locally (e.g. a `GeneratedProductCheck` environment failure) are recorded
  **non-authoritatively** with the environment cause, per
  `readiness/aggregate-hang-diagnostics.md` / `readiness/runtime-limitations.md`.

## Canonical Verification Targets

Generated tasks call repository targets rather than raw command order. Run
`./fake.sh build -t Route` first and run **only** the gates it prints. The
serialized FAKE-backed order (when `Route` escalates): 1. `Dev`,
2. `GeneratedGuidanceCheck`, 3. `TemplateCheck`, 4. `GeneratedProductCheck`,
5. `EvidenceGraph`, 6. `EvidenceAudit`. FAKE-backed commands share `.fake` state
and must run sequentially. `RefreshSurfaceBaselines` / `PerPackageSurface` for
intentional baseline refreshes.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory (`specs/092-wire-retained-identity-state/`) links spec + plan and that `.specify/feature.json` pins to it
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Scaffold the audit-enforced readiness files discoverable before implementation — `readiness/visual-evidence-honesty.md`, `readiness/window-visibility.md` (honest "deferred — render-only offscreen, no live Vulkan window required"), `readiness/real-image-evidence.md`, `readiness/governance-risk-levels.md`, `readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`, `readiness/generated-guidance-validation.md`, plus the feature-specific `readiness/live-survival/`, `readiness/focus-resolution/`, `readiness/work-reduction/`, `readiness/theme-reuse/`, and `readiness/multi-frame/` placeholders — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [skillist: []] Record feature Tier (1 / contracted), affected layers (`src/Controls/RetainedRender.fs`, `src/Controls.Elmish/ControlsElmish.fs`, `src/SkiaViewer/SkiaViewer.fs`), public-API impact (`MapKey` widening + `ControlsElmish` focus seam re-key + internal `RetainedRender` work-reduction/theme/first-frame), Elmish/MVU applicability (consumer `view`/`update` unchanged; interpreter-edge focus/text/clock state re-keyed `ControlId`→`RetainedId`), and the real-evidence obligations (live-survival through the real seam, focus-resolution, work-reduction, theme-reuse, multi-frame, surface-baseline diffs)

---

## Phase 2: Foundation

- [X] T004 [P] [skillist: fs-skia-reconciliation] Draft the internal `RetainedRender.fsi` deltas (`src/Controls/RetainedRender.fsi`, stays `module internal`) per `contracts/contracts.md` §1 — `WorkReductionRecord` gains `ShiftedNodeCount`; `RetainedRender<'msg>` gains `Theme`; add `RetainedInit<'msg>` (init returns retained + render + first-frame diagnostics); `init` return type changes; add `retainedHitTest: x -> y -> RetainedRender<'msg> -> RetainedId option`; correct the work-reduction doc to `RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount < BaselineNodeCount`
- [X] T005 [P] [skillist: fs-skia-skiaviewer] Draft the `SkiaViewer.fsi` public seam widening (`src/SkiaViewer/SkiaViewer.fsi`) per `contracts/contracts.md` §2 — `InteractiveViewerHost.MapKey : ViewerKey -> bool -> 'msg list` (was `'msg option`; `[]` = unhandled, non-empty dispatches every message in order); **enumerate every `ViewerKey -> bool -> 'msg option` field across the viewer host records and either widen each identically or record in this task note that no sibling field exists** (resolve the widening scope at contract time, not later); author the compatibility/migration note (`Some m → [ m ]`, `None → []`) for the public release notes
- [X] T006 [P] [skillist: fs-skia-elmish] Draft the `ControlsElmish.fsi` package-surface focus-routing seam (`src/Controls.Elmish/ControlsElmish.fsi`) per `contracts/contracts.md` §3 — `resolveFocus: retained -> x -> y -> RetainedId option` (replaces the `ControlId` `hitTest |> nearestAuthored` path) and `routeFocusedText: retained -> focused:RetainedId option -> TextInputMsg -> RetainedRender<'msg> * 'msg list` (seeds from value + line mode on first focus, returns ALL matched `onChanged` messages); note the 090 `ControlId`-keyed `routeFocusedText` is **replaced** (breaking within the package surface, covered by the recaptured baseline + migration note)
- [X] T007 [skillist: fs-skia-evidence-mode] Record the surface-baseline posture (`SkiaViewer` + `Controls.Elmish` public per-package + cross-package baselines move; `Controls` internal per-package baseline moves; all regenerated via `RefreshSurfaceBaselines` / `PerPackageSurface`, never hand-edited) and the unsupported-scope handling — correctness-wins fallback (output byte-identical to a full rebuild; FR-007 measurement never alters the scene), frame-0 `KeyCollision` surfacing through the existing `ControlDiagnostic` channel, render-only honesty — in `readiness/governance-risk-levels.md` / `readiness/runtime-limitations.md`

**Checkpoint**: Foundation ready — `.fsi` contracts drafted, baselines posture recorded; story implementation may begin.

---

## Phase 3: User Story 1 — focus and in-progress text survive a position change in the running app (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fs-skia-reconciliation, fs-skia-testing] Failing-first **live-survival** test driving the **real adapter seam** (no manual `StateByIdentity` seeding): focus the editor → keystroke `x` (`draft="hix"`) → an unrelated insert shifts the editor down → keystroke `y` ⇒ focus is still on the editor and `draft="hixy"` (continued, not reset), **and the editor's per-control animation clock (`RetainedUiState.Animation`) is the carried value, not a freshly-reset clock** (FR-001 clock element); assert a rebuild-every-frame baseline (re-`init` each frame, minting a fresh id) **fails** the same proof for focus, draft, and clock (SC-001, quickstart steps 1–5 + baseline)
- [X] T009 [P] [US1] [skillist: fs-skia-reconciliation, fs-skia-testing] Failing-first carry/drop test: when `step` matches a control across a shift (`ChildKeep`/`Update`, not `Replace`), its `StateByIdentity` entry is carried to the matched `RetainedId`; when the diff `Replace`s (kind/key change) or removes it, the prior entry is dropped — no false identity carry; a focused control removed entirely clears focus (FR-003 + edge case)

### Implementation

- [X] T010 [US1] [skillist: fs-skia-reconciliation] Make `RetainedRender.step` (`src/Controls/RetainedRender.fs`) **populate and read** `StateByIdentity`: carry each matched node's `RetainedUiState` to its carried `RetainedId`, drop on `Replace`/remove, filter entries whose identity left the live set (FR-001/FR-002/FR-003) — 091 carried the map but the host never consumed it; this closes that half
- [X] T011 [US1] [skillist: fs-skia-elmish, fs-skia-keyboard-input] Re-key the `ControlsElmish` interpreter-edge closure (`src/Controls.Elmish/ControlsElmish.fs`): `focusedText` ref `ControlId option → RetainedId option`; remove the separate `textModels : Map<ControlId, TextInputModel>` (state now lives in `RetainedRender.StateByIdentity[id].Text`); the carried draft is authoritative while a control is focused, and the model value re-seeds the draft **only on initial focus acquisition** (not every re-render), so a same-frame model change never overwrites in-progress typing (FR-001/FR-002 + the FR-005-vs-draft conflict resolution)
- [X] T012 [US1] [skillist: fs-skia-evidence-mode] Capture `readiness/live-survival/` — `survival.txt` (focus + draft text + per-control animation clock survive the shift through the live seam) and `baseline-fails.txt` (rebuild-every-frame loses all three under the identical sequence), authoritative as structural `Scene`/identity equality; document US1's independent validation path (quickstart §) and confirm an existing MVU consumer needs zero `view`/`update` changes to benefit (SC-001)

**Checkpoint**: User Story 1 — survival is real in the running host, proven without seeding.

---

## Phase 4: User Story 2 — any focusable field accepts focus and preserves its current value (US2, P2)

### Tests First

- [X] T013 [P] [US2] [skillist: fs-skia-reconciliation, fs-skia-testing] Failing-first **focus-resolution** test: click-to-focus resolves to the correct control for a directly-keyed field, an unkeyed field, and an unkeyed field nested under a keyed container; two unkeyed same-kind siblings resolve to **distinct** `RetainedId`s (independently focusable, no shared-id collapse); in a **pre-filled multi-line** field the first keystroke yields prior value + the new character (zero characters lost); a control with more than one change binding dispatches **every** matching binding (SC-002, FR-004/FR-005/FR-006)

### Implementation

- [X] T014 [US2] [skillist: fs-skia-reconciliation] Implement `RetainedRender.retainedHitTest` (`src/Controls/RetainedRender.fs`): return the deepest retained node whose `Fragment.Box` contains the point, else `None` (true gap / outside root); per-node distinct so unkeyed same-kind siblings resolve to different ids — one identity scheme shared between hit-testing and focus resolution (FR-004)
- [X] T015 [US2] [skillist: fs-skia-elmish, fs-skia-keyboard-input] Wire focus acquisition in `ControlsElmish` to `resolveFocus`/`retainedHitTest` (replacing the `ControlId` `hitTest |> nearestAuthored` path) and seed the focused control's `TextInput` from its **current value** + **kind-derived line mode** (single vs multi-line) on first focus, so the first keystroke appends rather than discards; fix the 090 `TextInput.init` value-discard / hardcoded-`SingleLine` defects on this path (FR-004/FR-005)
- [X] T016 [US2] [skillist: fs-skia-skiaviewer] Widen the host `mapKey` closure and `InteractiveViewerHost.MapKey` to `'msg list` and dispatch **all** matched `onChanged` product messages in order (replacing the 090 `mapKey |> List.tryHead` first-only path) (FR-006)
- [X] T017 [US2] [skillist: fs-skia-evidence-mode] Capture `readiness/focus-resolution/` — `focus-resolution.txt` (keyed / unkeyed / keyed-container-wrapped each resolve to a distinct id) and `prefilled-append.txt` (pre-filled multi-line first keystroke appends), structural-equality authoritative (SC-002)

**Checkpoint**: User Story 2 — every focusable field focuses and preserves its value; US1's survival now reaches unkeyed, wrapped, and pre-filled fields.

---

## Phase 5: User Story 3 — work-reduction reporting is honest under layout shifts (US3, P3)

### Tests First

- [X] T018 [P] [US3] [skillist: fs-skia-reconciliation, fs-skia-testing] Failing-first **work-reduction** test exercising a **sibling-shifting** change (insert a sibling above a fixed-size leaf): assert `RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount` and `RecomputedNodeCount < BaselineNodeCount` — the prior 091 suite only covered the no-geometry-shift case that the 091 `RecomputedNodeCount ≤ ChangedSubtreeBound` doc could not survive (SC-003)

### Implementation

- [X] T019 [US3] [skillist: fs-skia-reconciliation] Add `ShiftedNodeCount` to `WorkReductionRecord` (`src/Controls/RetainedRender.fs`), counting nodes recomputed **only** because an upstream change relaid them out, distinct from `ChangedSubtreeBound` (now genuinely-changed work only); bring the `.fsi` doc into agreement (`changed + shifted` relationship, FR-007). Adding the counters MUST NOT alter the produced render output (FR-010 wins if forced)
- [X] T020 [US3] [skillist: fs-skia-evidence-mode] Capture `readiness/work-reduction/work-reduction.txt` (`BaselineNodeCount`, `RecomputedNodeCount`, `ChangedSubtreeBound`, `ShiftedNodeCount` under the sibling-shifting change satisfying the documented relationship) (SC-003)

**Checkpoint**: User Story 3 — the work-reduction measure is self-consistent under a layout shift.

---

## Phase 6: User Story 4 — render-path hygiene: theme changes, first frame, and standing collisions (US4, P3)

### Tests First

- [X] T021 [P] [US4] [skillist: fs-skia-reconciliation, fs-skia-testing] Failing-first hygiene tests: (a) two consecutive frames with **different themes** ⇒ the second frame's output is byte-identical to a full rebuild under the new theme, with no fragment painted under the old theme reused (SC-006); (b) the **first frame** measures/paints its node set exactly **once**, not twice (SC-005)
- [S] T022 [P] [US4] [SEH] synthetic-error-handling-approved [skillist: fs-skia-reconciliation, fs-skia-testing] Frame-0 duplicate-key diagnostic test: a tree with **duplicate sibling keys from its first appearance** ⇒ the `KeyCollision` diagnostic is surfaced on that first frame (de-duped to once per standing collision) through the `ControlDiagnostic` channel, and the path stays total (no throw) (SC-005) — deliberately-malformed duplicate-keyed literal tree is the only synthetic element; the diagnostic is produced by the real wired path (mirrors 091's KeyCollision `[SEH]`); remains `[S]` when completed (see Synthetic-Evidence Inventory)

### Implementation

- [X] T023 [US4] [skillist: fs-skia-reconciliation] Fold `Theme` into `RetainedRender<'msg>` and into the fragment reuse decision in `step` (`src/Controls/RetainedRender.fs`): a fragment painted under one theme is **not** reused unchanged under a different theme — a theme change invalidates the affected fragments and they repaint; the path no longer relies on a constant-per-host-loop theme precondition (FR-008/SC-006)
- [X] T024 [US4] [skillist: fs-skia-reconciliation] Change `RetainedRender.init` to measure/paint the first frame **once** and return first-frame `Diagnostics` (duplicate-key `KeyCollision` detected on the first tree) via `RetainedInit<'msg>`; surface those diagnostics through the `ControlsElmish` adapter's existing de-dup `Set` and paint the returned scene once (no frame-0 double render, no deferred collision) (FR-009/SC-005)
- [X] T025 [US4] [skillist: fs-skia-evidence-mode] Capture `readiness/theme-reuse/theme-reuse.txt` (frame-1 byte-identity to a full rebuild under the new theme) and `readiness/multi-frame/first-frame.txt` (single first-frame paint + frame-0 `KeyCollision` surfaced once) (SC-005/SC-006)

**Checkpoint**: User Story 4 — the wired path's edges (theme, first frame, standing collisions) are predictable.

---

## Phase 7: Invariants & multi-frame parity

- [X] T026 [P] [skillist: fs-skia-reconciliation, fs-skia-testing] Confirm all four 067/091 invariants (totality, determinism, identity-at-rest, round-trip) still hold on the wired path (SC-007); assert wired round-trip byte-identity (`step.Render.Scene ≡ Control.renderTree theme size next`) over **≥1,000** generated `(prev, next)` frame pairs **and** across a chained sequence of **3 or more** consecutive frames (multi-frame reconciliation, not only a single transition); capture `readiness/multi-frame/round-trip.txt` (SC-004/SC-007)

**Checkpoint**: Invariants and multi-frame byte-identity proven on the wired path.

---

## Phase 8: Integration & Polish (surface baselines + gates)

- [X] T027 [skillist: fs-skia-skiaviewer] Recapture surface baselines — per-package for `FS.Skia.UI.SkiaViewer` and `FS.Skia.UI.Controls.Elmish` (public deltas) and `FS.Skia.UI.Controls` (internal `.fsi`, no public delta), plus the cross-package baseline (`MapKey` + `ControlsElmish` seam), via `RefreshSurfaceBaselines` / `PerPackageSurface.captureCurrent` (never hand-edited); add the `MapKey` widening compatibility/migration note to the public docs/release notes — **DONE:** `RefreshSurfaceBaselines` regenerated 11 per-package baselines (Controls/Controls.Elmish/SkiaViewer moved); emitted `template/base/docs/api-surface/SkiaViewer/SkiaViewer.fsi` updated; `PackageSurfaceCheck` + `PerPackageSurfaceDiff` PASS; migration note in `contracts/contracts.md` §2 + `governance-risk-levels.md`.
- [X] T028 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Route` over the branch diff, confirm the expected escalation (public `.fsi` deltas → consumer-contract tier), then run the gate order **sequentially** (`Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck`); record any non-authoritative aggregate (e.g. a `GeneratedProductCheck` environment failure) with its cause in `readiness/runtime-limitations.md` — **DONE:** `Route` escalated to `agent-ready`; all printed gates PASS — `Dev`, `PackageSurfaceCheck`, `PerPackageSurfaceDiff`, `FsiTranscripts`, `GeneratedProductCheck` (full template instantiate + consumer validation + smoke, no env failure this run), `ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`, `DesignTokenDrift`, `ContrastCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`.
- [X] T029 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory`/`tasks=<n>` match this feature, no cycles, no dangling refs, no `[S*]` surprises — **DONE:** graph valid; 27 `[X]`, 1 accepted-`[SEH]` (`[S]` T022), 0 `[S*]` (accepted-seh stopped the cascade to T024/T025), no cycles/dangling refs.
- [X] T030 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS, or document the `[SEH]` T022 `accepted-seh` row against its Synthetic-Evidence Inventory entry — **DONE:** `verdict=PASS`, real-tasks=27, accepted-seh-tasks=1, unaccepted-synthetic-tasks=0, auto-synthetic-tasks=0, diff-scan-hits=0, window-visibility-hits=0, total-blockers=0 (T022's `accepted-seh` row recognized; no override needed).

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. For `[SEH]` rows,
include the approval label, design-phase source, synthetic input class, expected
error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T022 | Frame-0 duplicate-key diagnostic requires a deliberately-malformed duplicate-keyed literal tree; the diagnostic itself is produced by the real wired `init` path (only the malformed input is a literal) | `readiness/multi-frame/first-frame.txt` | — | `synthetic-error-handling-approved` | spec FR-009 / SC-005; plan.md "Synthetic evidence" (mirrors 091 KeyCollision `[SEH]`) | Malformed duplicate-keyed sibling literal tree (first frame) | `KeyCollision` surfaced once via `ControlDiagnostic`; `init`/`step` stays total (no throw) | accepted-seh |
