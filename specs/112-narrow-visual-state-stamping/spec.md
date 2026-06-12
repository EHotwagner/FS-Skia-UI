# Feature Specification: Narrow Runtime Visual-State Updates (Targeted Hover/Focus/Press Stamping)

**Feature Branch**: `112-narrow-visual-state-stamping`
**Created**: 2026-06-12
**Status**: Draft
**Input**: User description: "do the next part."

**Source report** (local in-repo report, not a remote URL — no `source-spec.md`
snapshot per the specify FR-016 no-op rule):
`docs/reports/2026-06-12-1422-controls-performance-framework-research.md`. This
feature implements the **next part** of that report's staged plan after feature 111
(which delivered Phase 3: the frame scheduler & phase-invalidation model) — namely
**Phase 4: Narrow Runtime Visual-State Updates**, also the report's "Do next"
priority **#2**. Everything from Phase 5 onward (view memoization, viewport
virtualization, paint/damage caches, layout caches, backend review) remains **out of
scope** — see *Unsupported scope*.

## Why this feature (context)

Feature 111 stopped the live host re-running `host.View` on a model-unchanged frame,
but the host still **stamps runtime visual state across the entire control tree**
every frame it paints. On each hover/focus/press change the live loop calls
`ControlRuntime.applyRuntimeVisualState` (`ControlsElmish.fs:907/914`), which walks
**every** node of the lowered tree and reconstructs it to stamp the derived
`VisualState` — even though only the control that gained or lost hover/focus/press
actually changes. The report names this directly (Phase 4 goal): "Hover/focus/press
changes should not require stamping the entire tree."

That whole-tree stamp is O(node-count) allocation + walk on every pointer move that
crosses a control boundary, riding on top of an otherwise-retained pipeline. This
feature replaces it with a **targeted** stamp that re-stamps only the control
identities whose runtime state actually changed — the previous and current hover,
the previous and current focus, and the pressed identities — plus the ancestor paths
needed to rebuild them, leaving every unaffected subtree reused as-is. The full-tree
stamp is **preserved** as a parity oracle and fallback. The result must produce the
**byte-identical** final rendered scene, while touching far fewer nodes, made
observable by a new `RuntimeStateTouchedNodeCount`.

## Clarifications

### Session 2026-06-12

- Q: Where should `RuntimeStateTouchedNodeCount` be surfaced, given the runtime
  visual-state stamp runs only on the live host (the deterministic `Perf.runScript`
  corpus stamps visual state inline via the model, not via the runtime bridge)? → A:
  **Internal count only** — the count is returned by the internal targeted-stamp
  result and asserted deterministically in `Controls.Tests` (the authoritative
  evidence); the live host surfaces it best-effort (diagnostic/log). It is **NOT** a
  public `FrameMetrics` field — so there is no breaking `ControlsElmish.fsi`
  `FrameMetrics` change, no corpus-golden churn, and no permanently-`0` golden column.
  (`Route` still escalates to controls-public-surface because the internal Controls
  `ControlRuntime.fsi` surface changes.)

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A hover/focus/press change re-stamps only the affected controls (Priority: P1)

A framework maintainer moves the pointer so hover leaves one control and enters
another. Today every node of the tree is re-stamped to apply the visual-state change.
After this feature, only the old-hover and new-hover control identities (and the
ancestor paths needed to rebuild them) are re-stamped; the rest of the tree is reused
untouched.

**Why this priority**: This is the report's Phase 4 headline and the last broad
per-frame whole-tree walk on the live hot path. It is independently valuable: even
with no other change, narrowing the stamp makes a hover sweep cost work proportional
to the affected controls, not the control count.

**Independent Test**: Drive a hover transition between two controls in a tree of many
controls and assert, via `RuntimeStateTouchedNodeCount`, that the number of nodes the
runtime stamp touched is far below the total node count — and equals the affected
identities plus their ancestor paths.

**Acceptance Scenarios**:

1. **Given** a tree of many controls with hover on control A, **When** hover moves to
   control B, **Then** the runtime stamp touches only A, B, and their ancestor paths
   — not every node.
2. **Given** focus on control A, **When** focus moves to control B, **Then** the stamp
   touches only A, B, and their ancestor paths.
3. **Given** a control with no runtime state change this frame, **When** the frame is
   stamped, **Then** that control's subtree is reused untouched (zero nodes touched in
   it).

---

### User Story 2 - Targeted stamping is rendered-scene-identical to the full-tree stamp (Priority: P1)

A maintainer must trust that narrowing the stamp changed *nothing* observable: the
final rendered scene after a targeted stamp must be byte-identical to the scene the
preserved full-tree stamp would produce — for hover-move, focus-move, and press
transitions — and the consumer-set / disabled visual-state precedence must be
unchanged.

**Why this priority**: A faster-but-wrong stamp is unacceptable; scene parity is the
gate that lets the optimization land. P1 because the optimization (US1) cannot be
accepted without it.

**Independent Test**: For a representative set of trees and hover/focus/press
transitions, stamp via the targeted path and via the preserved full-tree oracle and
assert the resulting rendered scenes (and the resolved per-control visual states) are
equal — including a control whose consumer-set `Disabled`/`Selected` state must win
over a derived hover/focus.

**Acceptance Scenarios**:

1. **Given** any hover/focus/press transition over any tree, **When** the scene is
   produced via targeted stamping and via the full-tree oracle, **Then** the rendered
   scenes are byte-identical.
2. **Given** a control whose consumer set a non-`Normal` visual state, **When** a
   derived hover/focus would otherwise apply, **Then** the consumer-set state still
   wins under targeted stamping (precedence unchanged).
3. **Given** a tree at rest (no hover/focus/press), **When** it is stamped, **Then**
   the output is byte-identical to the un-stamped/at-rest build.

---

### User Story 3 - The whole-tree stamp work is observable (Priority: P2)

A maintainer needs to know, in a deterministic count, how many nodes the runtime
visual-state stamp touched, so a regression that reintroduces the whole-tree stamp is
visible rather than silent.

**Why this priority**: The report requires tracking `RuntimeStateTouchedNodeCount`.
P2 because it hardens and proves US1/US2 rather than delivering the mechanism itself.

**Independent Test**: Assert `RuntimeStateTouchedNodeCount` reports the touched-node
count for a hover transition (far below the total) and `0` for a frame with no
runtime-state change; a regression that re-stamps the whole tree makes the count jump
to the node count and fails the assertion.

**Acceptance Scenarios**:

1. **Given** a hover/focus/press transition, **When** the frame is stamped, **Then**
   `RuntimeStateTouchedNodeCount` equals the touched-node count (affected identities +
   ancestor paths), which is far below the total node count.
2. **Given** a frame with no runtime-state change, **When** it is stamped, **Then**
   `RuntimeStateTouchedNodeCount` is `0`.

---

## Requirements *(mandatory)*

### Functional Requirements

**Targeted stamping (Phase 4 core)**

- **FR-001**: The framework MUST provide a **targeted** runtime visual-state stamp
  that re-stamps only the control identities whose derived runtime state could change
  this frame — the **previous and current hover**, the **previous and current focus**,
  and the **pressed** identities — plus the ancestor paths needed to rebuild those
  nodes, leaving every unaffected subtree reused unchanged.
- **FR-002**: The live host hot path MUST use the targeted stamp instead of the
  whole-tree `applyRuntimeVisualState` walk for an ordinary hover/focus/press frame.
- **FR-003**: The targeted stamp MUST honour the existing visual-state **precedence**:
  a consumer-set non-`Normal` state (e.g. `Disabled`, `Selected`) wins over a derived
  hover/focus/press; a derived `Normal` emits nothing (byte-identity at rest).
- **FR-004**: A control whose runtime state did **not** change this frame MUST have
  its subtree reused untouched (it contributes `0` to the touched-node count).

**Parity & fallback (Phase 4 correctness)**

- **FR-005**: The full-tree `applyRuntimeVisualState` stamp MUST be **preserved** as a
  parity oracle and a fallback; it MUST NOT be the normal live path. For every
  hover/focus/press transition the targeted stamp's final rendered scene MUST be
  **byte-identical** to the full-tree oracle's, proven by tests comparing the two.
- **FR-006**: When the set of changed identities cannot be resolved targeted (an
  unexpected case), the host MAY fall back to the full-tree stamp to preserve
  correctness; the resulting scene MUST still equal the oracle's.

**Observability (Phase 4 metric)**

- **FR-007**: The framework MUST expose a deterministic `RuntimeStateTouchedNodeCount`
  counting how many nodes the runtime visual-state stamp touched this frame (affected
  identities + ancestor paths). It MUST be `0` for a frame with no runtime-state
  change, and far below the node count for a localized hover/focus/press change. The
  count is **returned by the internal targeted-stamp result** (an internal
  `ControlRuntime` seam) and asserted deterministically in `Controls.Tests` — the
  authoritative evidence — and the live host surfaces it best-effort (diagnostic/log).
  It is **NOT** a public `FrameMetrics` field (clarified 2026-06-12): the runtime-state
  stamp runs only on the live host, so a golden-asserted `FrameMetrics` field would be
  a permanently-`0` corpus column; keeping the count internal avoids that and adds no
  corpus-golden churn.

**Behaviour preservation (cross-cutting)**

- **FR-008**: This feature is a **hot-path mechanism change only**. At-rest rendered
  output, control geometry, focus/keyboard routing semantics, and every dispatch
  outcome MUST remain **byte-identical** to the pre-feature state. The only intended
  observable changes are (a) fewer nodes touched by the runtime stamp and (b) the new
  `RuntimeStateTouchedNodeCount`.
- **FR-009**: Feature 111's frame scheduler / view-skip, feature 110's retained
  routing, and the retained render pipeline are unchanged; this feature only replaces
  the whole-tree runtime-state stamp with the targeted stamp at the same seam.

> Interacting / conflicting requirements:
> - **FR-001 (touch only changed identities) vs FR-005 (byte-identical scene)** —
>   resolution: a non-affected control derives the SAME visual state it had last frame,
>   and the full-tree stamp leaves a derived-`Normal`/unchanged node structurally
>   equal anyway; so re-stamping only the changed identities yields the same final tree
>   the full walk would, by construction. The targeted set is exactly
>   `{prev-hover, cur-hover, prev-focus, cur-focus, pressed}`; any identity outside it
>   is provably unchanged, so omitting it cannot change the scene.
> - **FR-007 (`RuntimeStateTouchedNodeCount`) determinism vs the live-only stamp** —
>   resolution: the runtime-state bridge runs only on the live host (the deterministic
>   `Perf.runScript` corpus stamps visual state inline via the model, not via the
>   runtime bridge), so the authoritative deterministic evidence for the count is a
>   direct test of the targeted-stamp function/result (a pure, byte-stable count); the
>   live `OnFrameMetrics` sink reports it best-effort. The count is therefore proven
>   deterministically without depending on a live window.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Touches `FS.Skia.UI.Controls` — `ControlRuntime`
  (`applyRuntimeVisualState` gains a targeted companion that returns the stamped tree +
  the internal touched-node count; the existing full-tree stamp stays as oracle) — and
  `FS.Skia.UI.Controls.Elmish` — the live `renderRetained` stamping seam
  (`ControlsElmish.fs:907/914`), which calls the targeted stamp and surfaces its
  internal count best-effort (no public `FrameMetrics` change, clarified 2026-06-12).
  The targeted stamp is an **internal** seam (`ControlRuntime` is internal; tests reach
  it via `InternalsVisibleTo`). No package identity changes; package **contents** change
  and versions bump on merge.
- **Public contract impact**: A new **internal** targeted-stamp `val internal` in
  `ControlRuntime.fsi` (consuming the internal `ControlRuntimeModel`/`Control`),
  returning the stamped tree + the `RuntimeStateTouchedNodeCount` (also internal,
  clarified 2026-06-12). There is **no** public `FrameMetrics` change and **no**
  public function signature change — the public surface delta is the internal Controls
  `ControlRuntime.fsi` seam only. `Route` is nonetheless expected to escalate to the
  **controls-public-surface** tier because the Controls package `.fsi` surface changes
  (an internal-val add still moves the per-package Controls surface baseline). Run
  `Route` first and obey its printed list.
- **State workflow impact**: None to MVU semantics — `Update`, effects, subscriptions,
  commands, and interpreter behaviour are unchanged. Dispatch *outcomes* are
  byte-identical (FR-008); only the *mechanism* that stamps the per-frame visual state
  changes.
- **Layout/rendering impact**: None to rendered output — at-rest scene, geometry, and
  the retained step are byte-identical (FR-008). The targeted stamp produces the same
  stamped tree the full walk would; it changes *how many nodes are rebuilt to stamp*,
  not *what is drawn*. No Vulkan/Skia/visual-output change; no unsupported-environment
  diagnostic change.
- **Evidence obligations**: scene-parity evidence comparing the targeted stamp to the
  full-tree oracle over hover/focus/press transitions (FR-005); touched-node-count
  evidence for a localized change vs a no-change frame (FR-007); precedence evidence
  (consumer-set/`Disabled` wins, FR-003); at-rest byte-identity (the standing
  Scene-parity golden suite under `Dev`); skill-loading evidence; the window-visibility
  not-applicable set; `readiness/evidence-audit.md` with a verdict token; the
  generated-validation package-resolution tokens. The escalated `maintainer-verify`
  readiness set applies because of the Controls `.fsi` change.
- **Unsupported scope**: This feature is **Phase 4 only**. Explicitly OUT: view/control
  **memoization** + stable-dependency diagnostics (Phase 5); viewport **virtualization**
  (Phase 6); damage rectangles / Skia picture / paint caches (Phase 7); text /
  layout-boundary caches (Phase 8); `SkiaViewer` backend / render-thread / compositor
  review (Phase 9). The full-tree stamp is **not removed** (preserved as oracle/
  fallback). No renderer rewrite, no Avalonia/WPF redesign, no platform/release/
  distribution scope. Feature 110's retained routing and feature 111's scheduler/
  view-skip are unchanged.
- **Build-target impact**: Escalated to the controls-public-surface set is expected
  because the Controls package `.fsi` surface changes; run `Route` first and obey its
  printed minimal list (`Dev`, the package/per-package surface diffs, `FsiTranscripts`,
  the controls catalog/doc/interaction/rendering checks, `GeneratedGuidanceCheck`,
  `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`). `RefreshSurfaceBaselines` must
  regenerate the surface + per-package baselines after the additions. No new gate.

## Success Criteria *(mandatory)*

- **SC-001**: A hover/focus/press transition over a tree of N controls touches only
  the affected identities + their ancestor paths — `RuntimeStateTouchedNodeCount` «
  N (it does not scale with the control count).
- **SC-002**: For **100%** of tested hover/focus/press transitions over representative
  trees, the targeted stamp's final rendered scene equals the preserved full-tree
  oracle's.
- **SC-003**: A frame with no runtime-state change reports
  `RuntimeStateTouchedNodeCount = 0` and reuses every subtree untouched.
- **SC-004**: Consumer-set / `Disabled` visual-state precedence is unchanged: a
  consumer-set non-`Normal` state still wins over a derived hover/focus/press under
  targeted stamping.
- **SC-005**: At-rest rendered output, control geometry, focus/keyboard routing
  semantics, and all dispatch outcomes are byte-identical to the pre-feature state.
- **SC-006**: Moving hover repeatedly across a large tree produces stamp work
  proportional to the affected controls, not the control count (proven by the
  touched-node counts across the transition sequence).

## Key Entities

- **Targeted runtime visual-state stamp** (internal, `ControlRuntime`): re-stamps only
  the changed runtime identities (`prev/cur hover`, `prev/cur focus`, `pressed`) +
  their ancestor paths, reusing unaffected subtrees; returns the stamped tree and the
  touched-node count. The mechanism replacing the whole-tree walk on the hot path.
- **Full-tree stamp oracle** (internal, preserved): the existing
  `ControlRuntime.applyRuntimeVisualState` whole-tree walk, kept as the parity oracle
  and fallback — never the normal live path.
- **RuntimeStateTouchedNodeCount**: the deterministic count of nodes the runtime stamp
  touched this frame (affected identities + ancestor paths); `0` on a no-change frame.
  Returned by the internal targeted-stamp result and asserted in `Controls.Tests`; an
  **internal** count (not a public `FrameMetrics` field, clarified 2026-06-12), surfaced
  best-effort by the live host. The observability surface this feature adds.
- **ControlRuntimeModel**: the existing read-only hover/press/focus/selection model the
  stamp derives visual state from (unchanged); the targeted stamp additionally consumes
  the **previous** frame's model to know which identities left a state.

## Assumptions

- **"Next part" = Phase 4** (Narrow Runtime Visual-State Updates). Feature 111
  delivered Phase 3; the report stages Phase 4 next and lists it as "Do next" #2.
  Phase 5+ is out of scope.
- Features 108–111's `FrameMetrics`, `Perf.runScript`, retained routing, the frame
  scheduler/view-skip, and the retained render pipeline are merged and are the
  foundation this feature extends — not rebuilt.
- The set of identities whose derived runtime state can change in a frame is exactly
  `{previous hover, current hover, previous focus, current focus, pressed}` — every
  other identity derives the same state it had last frame, so omitting it from the
  re-stamp cannot change the rendered scene (the basis of FR-001/FR-005 parity).
- The runtime-state bridge runs only on the live host; the deterministic
  `Perf.runScript` corpus stamps visual state inline via the model, so the
  authoritative deterministic evidence for the targeted stamp + its count is a direct
  test of the stamp result (not a live window).
- Scene/precedence parity is asserted by structural scene equality + resolved
  per-control visual-state comparison (controls have no general value equality), using
  the techniques established in features 092/096/103/111.
