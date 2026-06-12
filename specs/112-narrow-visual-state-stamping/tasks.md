# Tasks: Narrow Runtime Visual-State Updates (Targeted Hover/Focus/Press Stamping)

**Feature branch**: `112-narrow-visual-state-stamping`
**Spec**: `specs/112-narrow-visual-state-stamping/spec.md`
**Plan**: `specs/112-narrow-visual-state-stamping/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]` or
`[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the evidence audit.
See `readiness/task-graph.md` for the propagated view.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- This whole feature is **Tier 1** (a new internal `ControlRuntime.fsi` seam moves the
  Controls per-package surface baseline); per-task `[T1]/[T2]` annotations are omitted
  because every phase matches the feature tier.

## Elmish/MVU applicability

Principle IV's dedicated `Model`/`Msg`/`Effect`/`init`/`update`/interpreter tasks are
**N/A** for this feature: it is a per-frame visual-state *stamp mechanism* change inside
an existing host. `Update`, effects, subscriptions, commands, and the interpreter are
unchanged; dispatch *outcomes* stay byte-identical (FR-008). The interactive-UI
run-and-use gate is also not applicable — the feature delivers an internal stamp
optimization observable via the internal `RuntimeStateTouchedNodeCount` and the
preserved live render path, not a new interactive surface. Recorded in the
evidence-obligations task (T003 / T007).

## Governance risk level

**Medium** governance risk: a new internal `ControlRuntime.fsi` seam escalates `Route`
to the **controls-public-surface** tier, but there is no new gate, no dependency change,
no template-content change, and no public function-signature change. Focused validation
= the escalated gate set Route prints (T019–T021). Broad validation (full `Verify`) is
not required because the change set is two packages' internal contents plus the Controls
per-package baseline. Non-authoritative aggregate results are recorded as "focused
rerun" notes in `readiness/aggregate-hang-diagnostics.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold `specs/112-narrow-visual-state-stamping/` and confirm spec + plan + research + data-model + contracts + quickstart are linked and current
- [X] T002 [P] [skillist: []] Create the `specs/112-narrow-visual-state-stamping/readiness/` scaffolds discoverable before implementation — `evidence-audit.md`, `evidence-graph.md`, `skill-loading-evidence.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, `byte-identity-authority.md`, `touched-node-delta.md`, and the window-visibility not-applicable set — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: []] Record feature Tier (Tier 1), affected packages (`FS.Skia.UI.Controls` `ControlRuntime` + `FS.Skia.UI.Controls.Elmish` live seam), public-API impact (internal `RuntimeStampResult` + `applyRuntimeVisualStateTargeted`; no public signature change; `RuntimeStateTouchedNodeCount` internal), Elmish/MVU + interactive-UI applicability (both N/A with the rationale above), and the required evidence obligations (parity vs oracle, touched count, precedence, baselines, XML-doc)

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-ui-widgets] Add the internal `RuntimeStampResult<'msg>` record (`Stamped` + `RuntimeStateTouchedNodeCount`) and `val internal applyRuntimeVisualStateTargeted: prev -> cur -> prevStamped -> fresh -> RuntimeStampResult<'msg>` to `ControlRuntime.fsi` (XML-doc each), and implement the parallel-walk in `ControlRuntime.fs`: zip `prevStamped` and `fresh`, compute `finalState M node = consumer-set state if non-Normal else deriveVisualState M id`, REUSE the `prevStamped` node when `finalCur = finalPrev` and no descendant changed, else REBUILD from the `fresh` node with `finalCur` stamped (via `setVisualState`, or no `visualState` attr at `Normal`), counting rebuilt nodes (FR-001/FR-007). **Also** add a pure, internal, deterministically-testable **path-selection** helper `val internal runtimeStampFor: prior: (ControlRuntimeModel * Control<'msg>) option -> cur: ControlRuntimeModel -> fresh: Control<'msg> -> RuntimeStampResult<'msg>` that returns the **targeted** result when `prior = Some(prevModel, prevStamped)` and the structures align, else the **full-oracle** result over `fresh` (`applyRuntimeVisualState cur fresh`, count = node count) — this encapsulates the live route choice so FR-002's selection is testable without driving the live loop (FR-002/FR-006). Build compiles
- [X] T005 [skillist: fs-skia-ui-widgets] Exercise the drafted targeted-stamp shape from FSI (build a tree + prev/cur models, call `applyRuntimeVisualStateTargeted`, print the touched count), capturing the session transcript to `readiness/fsi-session.txt`
- [X] T006 [skillist: fs-skia-ui-widgets] Capture the intended per-package Controls surface baseline shape for the new internal `ControlRuntime` seam (the authoritative regen happens in T017) and note it in `readiness/`
- [X] T007 [skillist: []] Record unsupported-scope handling and failure diagnostics: Phase 5+ is OUT; the full-tree `applyRuntimeVisualState` oracle is preserved (FR-005); narrowing the reconciler DIFF (vs the stamp) is OUT; features 110/111 (retained routing, scheduler/view-skip) are unchanged (FR-009); the targeted path degrades to the full oracle on a model-change/first/misaligned frame (never a stale render, FR-006); Principle IV + interactive-UI gate N/A

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — A hover/focus/press change re-stamps only the affected controls

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fs-skia-ui-widgets, fs-skia-evidence-mode] Add a failing-first `Feature112TouchedCountTests` in `tests/Controls.Tests`: over a tree of many controls, a hover move A→B and a focus move A→B each report `RuntimeStateTouchedNodeCount` equal to the A + B + ancestor-path node count (far below the total); a no-change frame (hover persists on the same control, and a fully at-rest frame) reports `0` and reuses every subtree (SC-001/SC-003/SC-006/FR-004)
- [X] T009 [US1] [skillist: fs-skia-ui-widgets] Make the reuse-and-count in `applyRuntimeVisualStateTargeted` correct: a node whose `finalState` is unchanged AND whose descendants are unchanged returns the `prevStamped` instance untouched (contributes `0`); a changed node rebuilds its path and counts `+1` per rebuilt node. Make T008 pass (FR-001/FR-004/FR-007)
- [X] T010 [US1] [skillist: fs-skia-controls-host] Wire the live host: `renderRetained` (`ControlsElmish.fs:912-920`) calls the pure `ControlRuntime.runtimeStampFor` helper (T004) — passing `Some(lastRuntimeModel, prev.Root.Control)` only on a model-unchanged frame (`viewFor` cache hit + `retained` present), else `None` (model-change / first frame → full oracle) — using `.Stamped` as `next` for `RetainedRender.step` and surfacing `.RuntimeStateTouchedNodeCount` best-effort; add a `lastRuntimeModel` ref updated each frame; confirm the live loop still renders (Dev / standing Scene-parity suite). Routing the live decision through the pure helper makes the model-unchanged-vs-oracle selection deterministically testable (FR-002/FR-006)
- [X] T011 [US1] [skillist: []] Document the US1 independent validation path (build a tree, move hover/focus, assert the touched count « N and no-change = 0) in `readiness/`

**Checkpoint**: User Story 1 is functional and independently testable.

---

## Phase 4: User Story 2 (US2) — Targeted stamping is rendered-scene-identical to the full-tree stamp

### Tests First

- [X] T012 [P] [US2] [skillist: fs-skia-ui-widgets, fs-skia-evidence-mode] Add a failing-first `Feature112TargetedStampParityTests`: for keyed / nested / unkeyed-same-kind-sibling / consumer-set trees and hover-move / focus-move / press-toggle transitions, the targeted stamp's `Stamped` rendered scene (via `Control.renderTree`) and the resolved per-control visual states equal the preserved full-tree `applyRuntimeVisualState` oracle's (structural `Scene` equality; controls have no value equality) (SC-002/FR-005). **Also** assert the live **path-selection** deterministically via `ControlRuntime.runtimeStampFor` (T004): `prior = Some(prevModel, prevStamped)` over an aligned structure takes the targeted route (its scene equals the oracle), and `prior = None` (first/model-change frame) takes the full-oracle route — so FR-002's route choice is covered without driving the live loop (FR-002/FR-006)

### Implementation

- [X] T013 [US2] [skillist: fs-skia-ui-widgets] Ensure `Stamped` is byte-identical to `applyRuntimeVisualState cur fresh` for every node (a reused node already carries `finalCur`; a rebuilt node is `fresh + finalCur`; `Normal` emits NO `visualState` attribute — byte-identity at rest). Make T012 pass (FR-005/FR-008/SC-002)
- [X] T014 [US2] [skillist: fs-skia-ui-widgets] Add `Feature112PrecedenceTests`: a consumer-set `Disabled`/`Selected` control keeps its state under targeting (its `finalState` is the consumer state under both models, so it is never re-stamped by a derived hover/focus), and a derived `Normal` emits nothing (FR-003/SC-004)

**Checkpoint**: User Story 2 is functional and independently testable.

---

## Phase 5: User Story 3 (US3) — The whole-tree stamp work is observable

### Tests First

- [X] T015 [P] [US3] [skillist: fs-skia-ui-widgets, fs-skia-evidence-mode] Add a `Feature112` assertion that across a hover-sweep sequence over a large tree the touched-node counts are proportional to the affected controls, not the control count (SC-006), and that the count is the regression guard — a (temporary, in-test) whole-tree stamp makes the count jump to the node count, proving the metric detects the regression (FR-007)

### Implementation

- [X] T016 [US3] [skillist: []] Document the internal-count surface decision (no public `FrameMetrics` field, clarified 2026-06-12) and the touched-node before/after delta (whole-tree N → affected-paths count) in `readiness/touched-node-delta.md`

**Checkpoint**: User Story 3 is functional and independently testable.

---

## Phase 6: Integration & Polish

- [X] T017 [skillist: fs-skia-ui-widgets] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate the per-package Controls surface (gains the internal `RuntimeStampResult` type + `applyRuntimeVisualStateTargeted` val) and confirm the top-level public Controls surface baseline is unchanged (the seam is `internal`); update any remaining sites it flags
- [X] T018 [skillist: fs-skia-ui-widgets] Confirm the new `RuntimeStampResult` + `applyRuntimeVisualStateTargeted` XML-doc satisfies the doc-preservation gate, the full-tree `applyRuntimeVisualState` oracle's signature/doc are unchanged, and no public function signature changed
- [X] T019 [skillist: fs-skia-template-update, fs-skia-controls-host] Run the escalated controls-public-surface gates sequentially as `Route` prints them — `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, the package/per-package surface diffs, and the controls catalog/doc/interaction/rendering checks — and record the focused governance risk level + non-authoritative aggregate notes in `readiness/`
- [X] T020 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and the echoed `feature-directory`/`tasks=<n>` match this feature
- [X] T021 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with no remaining `[S]`/`[S*]` and no diff-scan hits, or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the source
for the PR description's synthetic-evidence section. For `[SEH]` rows, include the
approval label, design-phase source, synthetic input class, expected error behavior, and
reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
