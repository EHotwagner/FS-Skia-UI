# Tasks: Focus, Keyboard Traversal & Input Routing

**Feature branch**: `094-focus-keyboard-traversal`
**Spec**: `specs/094-focus-keyboard-traversal/spec.md`
**Plan**: `specs/094-focus-keyboard-traversal/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` is an annotation for design-approved synthetic error-handling work; it
remains `[S]` when completed. **None planned for this feature** — the focus
reducers are pure and total (an unmatched key resolves to `Fallthrough`, a
defined no-op, never an exception; a removed focused control reuses E2's
stale-target recovery, never a throw), and all traversal/routing/stability
evidence is real (deterministic reducer + route-probe results, the live retained
path, the reused E1 responds-proof). Any `[S]` that appears triggers the full
Principle V disclosure regime.

## Tier & MVU posture

This is a **Tier 1** change (public surface moves: a new public
`src/Controls/Focus.fsi`, and the internal host key-routing contract +
`runInteractiveApp` doc on `src/Controls.Elmish/ControlsElmish.fsi`) — every
phase is Tier 1, so per-task `[T1]` marks are omitted. **MVU/Elmish IS
applicable**: focus is stateful, satisfied through the **existing**
`ControlRuntime` boundary — `Model` = `ControlRuntimeModel.FocusedControl`,
`Msg` = `ControlRuntimeMsg.FocusControl`, pure `update` = the existing
`ControlRuntime.update`. E4 adds **pure reducers** (`Focus.order` /
`Focus.traverse` / `Focus.route` — functions of tree + metadata + current focus
+ key, no `Effect`/`Cmd`/subscription model) and the **interpreter-edge** key
routing at the host (`routeFocusedKey`, wired by `runInteractiveApp`). Traversal
emits `FocusControl` messages the existing `update` consumes; the engine reads —
never duplicates — `FocusedControl`.

## Vertical-slice rule (US phases)

A `[US*]` task is `[X]` only when the user-reachable surface (the public `Focus`
functions through the packed library, or the live key path through
`runInteractiveApp` / the real `routeFocusedKey` adapter route-probe) was
actually exercised — passing unit tests on internal helpers alone do not satisfy
`[X]`. For these stateful stories, `[X]` also requires the MVU evidence: the
`FocusControl` transition was exercised through the existing `ControlRuntime`
boundary and emitted effects asserted.

## Success-criterion → assertion mapping

- **SC-001** (traversal order = FocusOrder-then-layout, cyclic wrap, skips
  non-focusable) → T011 tab-order test + T012 traversal test + T015 host wiring.
- **SC-002** (focused Button activates once / pointer-equivalent; focused Slider
  navigates on ArrowLeft/Right) → T017 `routeFocusedKey` adapter route-probe.
- **SC-003** (focused text control zero regression) → T018 E1 text-seam test.
- **SC-004** (focus stable across sibling-shift via live retained path) → T023
  live-retained test + T025 identity wiring (not a hand-seeded map).
- **SC-005** (focus indicator via E3 `Focused`, moves with focus, no procedural
  branch) → T026 indicator wiring + evidence.
- **SC-006** (purity/totality/determinism over ≥1000; unmatched key no-op never
  throws) → T028 FsCheck property.
- **SC-007** (computed order passes `Accessibility.validate`; metadata-only, no
  parallel table; `view` contract unchanged for keyboard-free consumers) → T024
  validate-order test + T022 representative metadata.

## Non-SC requirement traceability

- **FR-006** (pointer↔keyboard focus compose; click on a non-focusable region
  leaves focus unchanged) — the one functional requirement with no dedicated
  success criterion → T025 pointer-composition assertion (click sets focus to the
  hit focusable control / nearest focusable keyed ancestor; subsequent traversal
  continues from that position; non-focusable click is a no-op).

## Governance risk levels

- **Small** — pure `Focus.fs` reducer logic + the R1 `Accessibility.fs`
  correction: focused validation is `Dev` + the targeted `Controls.Tests` suites.
- **Medium** — `routeFocusedKey` host wiring + representative widget metadata +
  retained-identity focus binding: `Dev` + the adapter route-probe + E1 text-seam
  regression + the live-retained stability test.
- **Broad** — the public `Focus.fsi` + `Controls.Elmish` `.fsi` surface move
  escalates to controls-public-surface / package-surface: the full serialized
  `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck →
  EvidenceGraph → EvidenceAudit` path is required. FAKE-backed targets run
  **sequentially** (shared `.fake` state); aggregate results are recorded as
  **non-authoritative** unless re-confirmed sequentially.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- Every task has a matching `tasks.deps.yml` entry; every line mirrors the
  structured `skillist` via `[skillist: ...]`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory artifacts are present and linked (spec, plan, research, data-model, quickstart, `contracts/focus-model.md`, `contracts/key-routing-surface.md`, `checklists/requirements.md`)
- [X] T002 [skillist: []] Record feature Tier 1, affected layers (`FS.Skia.UI.Controls` new `Focus`; `FS.Skia.UI.Controls.Elmish` host seam), public-API impact (new `Focus.fsi`; internal `routeFocusedKey` + `runInteractiveApp` doc), MVU applicability (existing `ControlRuntime` boundary + pure reducers + host interpreter edge), and the evidence obligations from the plan
- [X] T003 [P] [skillist: []] Scaffold audit-discoverable readiness placeholders under `readiness/`: `us1-tab-traversal.md`, `us2-focused-key-delivery.md`, `us2-text-seam-preserved.md`, `us3-focus-stability.md`, `us3-focus-indicator.md`, `sc006-determinism-property.md`, `sc007-validate-order.md`, `responds-proof.md`, `fsi-transcript.md`, `surface-baselines.md`, plus `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `real-image-evidence.md`, `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action
- [X] T004 [P] [skillist: []] Run `./fake.sh build -t Route`; confirm the controls-public-surface + Controls.Elmish package-surface escalation and record the authoritative gate list plus the small/medium/broad governance risk levels for this Tier-1 surface move into `readiness/governance-risk-levels.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-ui-widgets, fs-skia-keyboard-input] Draft the public `src/Controls/Focus.fsi` surface — `FocusStop`, `TabOrder`, `FocusMove`, `KeyRouting`, and the pure totals `Focus.order` / `Focus.traverse` / `Focus.route` per `contracts/focus-model.md`; keep `RetainedId` out of the surface (it binds at the host)
- [X] T006 [skillist: fs-skia-elmish, fs-skia-viewer-host] Draft the internal `routeFocusedKey` contract on `src/Controls.Elmish/ControlsElmish.fsi` and update the `runInteractiveApp` `.fsi` doc to honestly describe the key path (text seam → `Focus.route` → traversal → `host.MapKey`) per `contracts/key-routing-surface.md`
- [X] T007 [P] [skillist: fs-skia-ui-widgets, fs-skia-keyboard-input, fs-skia-testing] Apply Research **R1** to `src/Controls/Accessibility.fs` (signatures unchanged): stop seeding every focusable control's `NavigationKeys` with `["Tab"; "Shift+Tab"]` in `defaultFor` (seed intra-control arrows per role instead); relax `validate` so an activation-only focusable control (e.g. `Button`) is valid — paired with a **failing-first** test asserting a focusable Button validates and a default control does not consume Tab
- [X] T008 [skillist: fs-skia-ui-widgets] Exercise the draft `Focus.fsi` from FSI against the packed library (`order` / `traverse` / `route`), capturing the session transcript to `readiness/fsi-transcript.md`
- [X] T009 [P] [skillist: []] Record the initial surface-area baseline expectations for the new/changed public modules (`Focus.fsi`, `ControlsElmish.fsi`); the authoritative recapture happens in Polish (T029)
- [X] T010 [P] [skillist: []] Record unsupported-scope handling and failure diagnostics into `readiness/runtime-limitations.md` (the `Fallthrough` no-op falls through to `host.MapKey`; a removed focused control reuses E2 `StaleTarget`/`RecoverStaleTarget`; no new accessibility primitive)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — keyboard traversal in a predictable order

### Tests First (Principle I, Principle VI)

- [X] T011 [P] [US1] [skillist: fs-skia-ui-widgets, fs-skia-testing] Add a failing-first tab-order test: `Focus.order` over a tree of mixed `FocusOrder` yields focusable-only stops ordered `FocusOrder` ascending with `None` in document order, and excludes non-focusable controls (SC-001 / US1.3)
- [X] T012 [P] [US1] [skillist: fs-skia-ui-widgets, fs-skia-testing] Add a failing-first traversal test: `Focus.traverse` advances on Tab / reverses on Shift+Tab, wraps cyclically at both ends, `None + Next` → first / `None + Previous` → last, **and an empty `TabOrder` (no focusable controls) is a no-op — `Next`/`Previous` both yield `None`, never throw** (edge case "No focusable controls", SC-001)

### Implementation

- [X] T013 [US1] [skillist: fs-skia-ui-widgets] Implement `Focus.order` in `src/Controls/Focus.fs` — pre-order tree walk → keep `Keyboard.Focusable = true` → stable sort by `(FocusOrder ?? +∞, docIndex)`; composites are a single stop (clarified)
- [X] T014 [US1] [skillist: fs-skia-ui-widgets] Implement `Focus.traverse` — index ± 1 mod n with cyclic wrap, `None` → first/last, and stale-target recovery (a current id absent from the order resolves to the next stop at its former position, or `None`)
- [X] T015 [US1] [skillist: fs-skia-elmish, fs-skia-viewer-host] Wire traversal at the host: `runInteractiveApp` routes an unconsumed Tab / Shift+Tab to `ControlRuntimeMsg.FocusControl (Focus.traverse (Focus.order view) focused move)`; capture the FocusControl transition + traversal evidence to `readiness/us1-tab-traversal.md`

**Checkpoint**: User Story 1 is functional and testable independently.

---

## Phase 4: User Story 2 (US2) — focused control responds to its activation/navigation keys

### Tests First

- [X] T016 [P] [US2] [skillist: fs-skia-ui-widgets, fs-skia-keyboard-input, fs-skia-testing] Add a failing-first `Focus.route` classification test: `ActivationKeys` → `Activate`, `NavigationKeys` → `Navigate`, unconsumed Tab → `Traverse`, else `Fallthrough`; a key in both a control's keys and the traversal set is consumed (never `Traverse`) (SC-002 / FR-007)
- [X] T017 [P] [US2] [skillist: fs-skia-elmish, fs-skia-reconciliation, fs-skia-testing] Add a failing-first `routeFocusedKey` adapter route-probe (via `InternalsVisibleTo`, no hand-seeded map): a focused `Button` + an `ActivationKey` produces exactly the pointer-equivalent message once (no double-dispatch); a focused `Slider` + ArrowLeft/Right produces its value-change message (SC-002)
- [X] T018 [P] [US2] [skillist: fs-skia-elmish, fs-skia-testing] Add an E1 text-seam regression test: a focused text control still receives typed/committed/composed text through the unchanged `routeFocusedText` path (SC-003)

### Implementation

- [X] T019 [US2] [skillist: fs-skia-ui-widgets, fs-skia-keyboard-input] Implement `Focus.route` in `src/Controls/Focus.fs` — membership tests (`ActivationKeys` then `NavigationKeys`) before the Tab test, returning the closed `KeyRouting` verdict; pure and total
- [X] T020 [US2] [skillist: fs-skia-elmish, fs-skia-reconciliation, fs-skia-keyboard-input] Implement `routeFocusedKey` in `src/Controls.Elmish/ControlsElmish.fs` — resolve the focused control over the retained tree (E2 `RetainedId`), normalize `ViewerKey`, run the E1 `routeFocusedText` first, then `Focus.route`, emitting authored activation/value-change messages, a `FocusControl` traversal message, or fall-through
- [X] T021 [US2] [skillist: fs-skia-elmish, fs-skia-viewer-host] Wire `routeFocusedKey` into `runInteractiveApp`'s key path ahead of the existing `host.MapKey` fallback; capture evidence to `readiness/us2-focused-key-delivery.md` and `readiness/us2-text-seam-preserved.md`
- [X] T022 [US2] [skillist: fs-skia-typed-controls, fs-skia-ui-widgets] Confirm/expose the representative controls' `KeyboardOperation` via the corrected `Accessibility.defaultFor` — `Button` (Enter/Space activation), `Slider` (ArrowLeft/Right navigation), a text control (E1 path) — touching `Widgets/*.fsi` only if a default is missing

**Checkpoint**: User Story 2 is functional and testable independently.

---

## Phase 5: User Story 3 (US3) — focus survives re-renders and is metadata-driven

### Tests First

- [X] T023 [P] [US3] [skillist: fs-skia-reconciliation, fs-skia-testing] Add a focus-stability test over the **live** retained path: after a sibling-shifting `RetainedRender.step`, the focused control still resolves to the same `RetainedId` — not a hand-seeded `StateByIdentity` map (SC-004)
- [X] T024 [P] [US3] [skillist: fs-skia-ui-widgets, fs-skia-testing] Add a validate-order test: the computed `Focus.order` for the representative view passes `Accessibility.validate`, and the order + key semantics derive solely from `AccessibilityMetadata` with no parallel hand-rolled table (SC-007)

### Implementation

- [X] T025 [US3] [skillist: fs-skia-reconciliation, fs-skia-elmish] Bind focus identity over the retained tree: `routeFocusedKey` / focus resolution consume E2's `RetainedId` (via `retainedHitTest` / `resolveFocus`) so `FocusedControl` survives an unrelated re-render, and a removed focused control reuses stale-target recovery; **also assert pointer↔keyboard focus composition (FR-006): a pointer click sets focus to the hit focusable control or its nearest focusable keyed ancestor (`FocusMovedByPointer`) and subsequent `Focus.traverse` continues from that control's position in the order, while a click on a non-focusable region leaves the current `FocusedControl` unchanged (does not silently clear it)**; capture to `readiness/us3-focus-stability.md` (SC-004, FR-006)
- [X] T026 [US3] [skillist: fs-skia-ui-widgets, fs-skia-evidence-mode] Drive the focused control's `Focused` visual-state through E3's resolver (no procedural per-kind focus-paint branch); the indicator moves with focus and is removed from the previously-focused control; capture to `readiness/us3-focus-indicator.md` (SC-005). **E3 (feature 093) dependency — confirm 093 has landed before asserting the E3-resolver path; if E3 is unlanded at implementation time, resolve the `Focused` state through whatever path renders it then (still no parallel procedural branch, per plan Assumptions) and mark the E3-resolver-specific assertion `[-]` with that written rationale rather than synthesizing evidence**
- [X] T027 [US3] [skillist: fs-skia-elmish, fs-skia-evidence-mode] Capture an input→visible-change responds-proof for a key-driven focus change via the reused E1 `captureRespondsProof` (an inert host yields identical frames + `Inert`); record to `readiness/responds-proof.md`

**Checkpoint**: User Story 3 is functional and testable independently.

---

## Phase 6: Integration & Polish

- [X] T028 [skillist: fs-skia-ui-widgets, fs-skia-testing] Add the FsCheck property over `Focus.order` / `traverse` / `route`: purity / totality / determinism over ≥1000 generated combinations, and an unmatched key is a defined no-op that never throws; record to `readiness/sc006-determinism-property.md` (SC-006)
- [X] T029 [skillist: fs-skia-ui-widgets, fs-skia-elmish] Recapture Tier-1 surface baselines (`./fake.sh build -t RefreshSurfaceBaselines` + `PerPackageSurface.captureCurrent`): controls-public-surface + Controls.Elmish package-surface + per-package + cross-package; record diffs to `readiness/surface-baselines.md`
- [X] T030 [skillist: fs-skia-template-update] Run the serialized escalated non-concurrent gate prefix sequentially — `./fake.sh build -t Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` — recording aggregate results as non-authoritative into `readiness/generated-guidance-validation.md`
- [X] T031 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; record to `readiness/evidence-graph.md`
- [X] T032 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS or document every `--accept-synthetic` override; record to `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. For `[SEH]`
rows, include the approval label, design-phase source, synthetic input class,
expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
