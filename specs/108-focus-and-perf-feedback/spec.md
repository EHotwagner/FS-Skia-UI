# Feature Specification: Focus Visibility, Performance Instrumentation, and ControlsShowcase3 Feedback Follow-ups

**Feature Branch**: `108-focus-and-perf-feedback`
**Created**: 2026-06-12
**Status**: Draft
**Input**: User description: "create specs from the feedback of the sibling repo controlsshowcase3. also add show focus mechanism for all controls. performance logging and testing mechanism."

**Feedback source** (local sibling repo, not a remote URL — no `source-spec.md`
snapshot per the specify FR-016 no-op rule): `../ControlsShowcase3/specs/001-controls-showcase/feedback/`
— `specify-2026-06-12.md`, `clarify-2026-06-12.md`, `plan-2026-06-12.md`,
`implement-2026-06-12.md` (severity: major). The implement-phase note is the
primary signal: it records three classes of live-host problems found by running
the real persistent window that the unit-test + offscreen-evidence layer did not
catch — invisible focus, unreliable modifier chords, and pointer-input stalling —
plus a 23-source survey of how other UI frameworks solve the pointer-move repaint
problem.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Every focusable control shows where focus is (Priority: P1)

A consumer drives an interactive FS.Skia.UI app from the keyboard. They press
Tab / arrow keys to move focus between controls. Today the consumer's focus model
updates (a status strip can print it), but **the rendered tree shows no focus
indicator** — the host's runtime visual state stamps focus from the host's *own*
retained focus, not from a consumer focus field, so arrow keys appear to "do
nothing." The consumer wants focus to be visibly painted on **whichever control
holds it**, for every focusable control kind, without hand-walking the tree and
hand-stamping `VisualState.Focused` on individually keyed nodes.

**Why this priority**: This is the explicit user request ("show focus mechanism
for all controls") and the single most visible correctness gap from the
feedback — an interactive UI whose focus is invisible fails the most basic
keyboard-accessibility expectation. It is independently valuable and testable on
its own.

**Independent Test**: Build a small multi-control app (button, slider, text box,
radio group, switch, plus unkeyed scaffolding such as labels/tiles). Move focus
across all focusable controls via the framework focus order. Assert via a
render-diff / structural-scene test that the focused control — and only the
focused control — carries the focus visual state and paints a focus ring, for
each control kind, including controls the consumer did not individually key.

**Acceptance Scenarios**:

1. **Given** an app with several focusable controls, **When** focus moves to a
   control, **Then** that control renders with a visible focus indicator and all
   others render without one.
2. **Given** a focusable control that the consumer did not assign an explicit key,
   **When** focus traversal reaches it, **Then** it still receives the focus
   indicator (traversal does not skip unkeyed focusable controls).
3. **Given** focus on a control, **When** focus moves away, **Then** the previously
   focused control returns to its non-focused appearance with no residual ring.
4. **Given** a non-focusable / structural element (e.g. a stack/grid/panel
   container or a static label), **When** focus traversal runs, **Then** it is not
   given a focus indicator and is skipped in the order.

---

### User Story 2 - Per-frame performance is observable from the host loop (Priority: P1)

A consumer (or the framework's own evidence tooling) wants to *see* what the
interactive host is doing each frame: how many nodes were re-measured, how many
pointer samples were processed, whether a frame triggered a full view rebuild,
and how long the frame's work took. Today this is invisible — the only way the
ControlsShowcase3 author discovered that pointer moves flooded
update→reconcile→repaint and that a 60 fps tick was rebuilding the whole tree was
by *running the app and feeling the stall*. There is no structured per-frame
work/timing signal to log, assert on, or attach as evidence.

**Why this priority**: This is the "performance logging" half of the explicit
request, and it is the prerequisite that makes the performance problems from the
feedback measurable rather than felt. It is independently valuable: even before
any host-loop optimization lands, exposing the metric turns an invisible
regression into an observable one.

**Independent Test**: Run a deterministic input script through the host's pure
update path and read back a structured per-frame metrics record. Assert the
record reports the expected counts (remeasured nodes, pointer samples consumed,
view-rebuild yes/no) for a known sequence, and that the metric is byte-stable
across runs (timings excluded from the determinism check; counts included).

**Acceptance Scenarios**:

1. **Given** a frame in which nothing changed, **When** the host advances, **Then**
   the metrics record reports zero re-measured nodes and no view rebuild.
2. **Given** a frame that mutated one control's geometry, **When** the host
   advances, **Then** the metrics record reports a bounded (non-whole-tree)
   re-measure count.
3. **Given** a burst of pointer-move samples within one frame, **When** the host
   advances, **Then** the metrics record reports how many raw samples arrived and
   how many were actually processed.

---

### User Story 3 - Performance behavior is locked by deterministic tests (Priority: P2)

A maintainer wants the performance characteristics that the feedback identified to
be *guarded by tests*, so a future change that reintroduces a full-tree rebuild on
hover, or un-coalesces pointer moves, fails CI rather than shipping and being
re-discovered by a consumer running the app. The "testing mechanism" is a
deterministic, frame-counted driver that folds an ordered input script over the
pure host update and asserts on the per-frame metrics from User Story 2.

**Why this priority**: This is the "performance testing mechanism" half of the
explicit request. It depends on the metrics surface (US2) existing first, hence
P2. It converts the feedback's hard-won runtime lessons into regressions that the
offscreen test layer *can* catch.

**Independent Test**: A test fixture runs a scripted interaction (hover across
controls, click, drag, navigate) and asserts: a pure-hover frame does not trigger
a whole-tree rebuild; N pointer-move samples in one frame yield at most one
processed move; an idle frame does zero re-measure work.

**Acceptance Scenarios**:

1. **Given** the deterministic input driver, **When** a script of M steps runs,
   **Then** it produces a byte-stable structured outcome (counts/metrics) suitable
   for a golden assertion.
2. **Given** a scripted hover over a control, **When** the frame is evaluated,
   **Then** the test asserts no full-tree rebuild occurred.
3. **Given** a scripted burst of K pointer-move samples in a single frame, **When**
   the frame is evaluated, **Then** the test asserts at most one move was
   processed.

---

### User Story 4 - Pointer input no longer stalls under continuous movement (Priority: P2)

A consumer moves the cursor continuously over the app (hover, drag). Today the
host re-runs the full view rebuild + reconcile + repaint on *every* pointer sample,
and over a remote/nested session the samples arrive in bursts, so clicks "stall
for seconds." The consumer wants continuous pointer movement to be decoupled from
render rate: at most one pointer-move processed per frame, with the latest
position kept (and the path retained for drags).

**Why this priority**: This is the highest-payoff framework fix in the feedback's
pointer-repaint survey (its P1) and is host-side (a consumer's `MapPointer`
returning `None` cannot stop the host's own per-sample hit-test + repaint). It is
self-contained — coalescing alone is expected to remove the buffering — and is the
behavior US3's tests assert on. Deeper repaint optimizations (damage-rect repaint,
hover-as-local-invalidation, backend motion-event compression) are explicitly
deferred (see Out of Scope) so this story stays shippable.

**Independent Test**: Feed a burst of pointer-move samples spanning one frame into
the host and assert (via the US2 metrics) that exactly one move was processed and
the processed position equals the most recent sample; for a drag, assert the
coalesced path is preserved.

**Acceptance Scenarios**:

1. **Given** several pointer-move samples between two frame boundaries, **When** the
   frame is processed, **Then** only the latest move position drives the frame and
   only one hit-test/visual-state update results.
2. **Given** a drag gesture spanning multiple samples in a frame, **When** the
   frame is processed, **Then** the drag still observes the full movement path
   (coalescing preserves drag fidelity).
3. **Given** an event-driven interactive tick (no animation loop), **When** no
   input arrives, **Then** no frame work is scheduled, and animation clocks still
   advance from the injected delta when they exist.

---

### User Story 5 - Composition and input ergonomics gaps are closed (Priority: P3)

A consumer building a multi-page app hits three sharp edges the feedback called
out: (a) pages cannot return their own `Control<PageMsg>` and be folded into the
shell's `Control<Msg>` because there is no `Control.map` / `Widget.map`; (b)
`DataGrid` sort is bi-state (asc ↔ desc) but a three-state asc → desc → none cycle
must be hand-coded in the product update; (c) at the key boundary the host hands a
*normalized* key with no modifier flag, so `Ctrl`/`Alt`/`Shift` chord shortcuts are
unreliable while plain digit/arrow keys route cleanly.

**Why this priority**: These are real ergonomics defects but each has a working
consumer-side workaround today, so they are lower urgency than invisible focus and
input stalls. They are grouped because they share the "make the documented
consumer seam match what consumers actually need" theme.

**Independent Test**: (a) Map a `Control<'a>` to `Control<'b>` and assert the
lowered structure is equivalent to authoring it directly in `'b`. (b) Cycle a
DataGrid column sort three times and assert asc → desc → cleared. (c) Deliver a
modified key at the host boundary and assert the consumer can observe the modifier
state (or receive a distinct chord event) deterministically.

**Acceptance Scenarios**:

1. **Given** a `Control<PageMsg>` and a `PageMsg -> Msg` wrapper, **When** mapped,
   **Then** the result is a `Control<Msg>` structurally equal to the directly
   authored control.
2. **Given** a sorted DataGrid column, **When** the sort toggle is invoked a third
   time, **Then** the column returns to unsorted (no product-side special-casing
   required).
3. **Given** a key pressed with a modifier held, **When** it reaches the consumer
   key boundary, **Then** the modifier is observable (no silent loss of the
   chord), so modifier shortcuts are as dependable as unmodified keys.

---

### User Story 6 - Live theming has a supported render-path pattern and helpers (Priority: P3)

A consumer wants live theme/accent switching where the captured/painted palette is
exact while the host's fragment-reuse key stays stable. The ControlsShowcase3
author solved it by building a `Theme` from a model-derived palette and passing it
to the render path while `host.Theme` stayed static for the reuse key — a subtle
split worth a documented, supported pattern. They also re-derived
`Palette.resolve` (theme+accent → palette), `Palette.contrastRatio` (WCAG relative
luminance), and `Palette.toTheme` (project a role palette onto the framework
`Theme`) by hand; these recur across consumers and belong in the skill-support
surface.

**Why this priority**: A documented pattern plus small reusable helpers; valuable
but not blocking, and orthogonal to the focus/perf core.

**Independent Test**: Resolve a palette for a theme+accent, assert
`contrastRatio` matches the WCAG AA reference for known color pairs (≥4.5:1 normal,
≥3:1 large), project it to a `Theme`, and assert the render-path-vs-reuse-key split
is documented and demonstrated by a runnable example.

**Acceptance Scenarios**:

1. **Given** a theme mode and accent, **When** the palette is resolved and
   projected to a `Theme`, **Then** the painted output uses the exact model-derived
   palette while the reuse key remains stable.
2. **Given** two colors, **When** `contrastRatio` is computed, **Then** it equals
   the WCAG relative-luminance ratio and the AA thresholds are checkable.

---

### User Story 7 - Interactive-feature readiness and host-seam authority are discoverable (Priority: P3)

A consumer or research agent planning an interactive feature is misled twice: (a)
`docs/api-surface/` exposes only the legacy builder surface plus the lower
`Viewer.runInteractiveViewer` seam, so an agent reading only that concludes the
`runInteractiveApp` / `InteractiveAppHost` / `PointerInteraction` consumer seam
"isn't in the public API" — when it lives in `Controls.Elmish` (the
`fs-skia-controls-host` skill + `ControlsElmish.fsi` are authority); (b) the
window-visibility-class readiness files the `EvidenceAudit` requires for an
interactive feature are discoverable only by reading `docs/evidence-formats.md`
and then failing the audit once. The consumer wants both gaps closed by
discoverable, in-repo guidance.

**Why this priority**: Pure documentation/governance discoverability — no runtime
behavior change. Real friction, but lowest urgency.

**Independent Test**: From `docs/scaffold-map.md` alone, a reader can identify the
`Controls.Elmish` host seam as "present in package, not in `docs/api-surface/` —
authority is the skill + `ControlsElmish.fsi`"; and from a single discoverable
checklist, a reader can enumerate the exact window-visibility readiness files +
required tokens an interactive feature's `EvidenceAudit` will demand, before the
first audit run.

**Acceptance Scenarios**:

1. **Given** `docs/scaffold-map.md`, **When** a reader looks up the host seam,
   **Then** it names the `Controls.Elmish` `runInteractiveApp` / `InteractiveAppHost`
   / `PointerInteraction` seam as package-present-but-absent-from-`api-surface`,
   alongside the existing typed-front-door note.
2. **Given** the interactive-feature readiness checklist, **When** a reader
   consults it before running the audit, **Then** they can list every required
   window-visibility readiness file and its `key=value` tokens without first
   failing `EvidenceAudit`.

---

### Edge Cases

- A control that is focusable but currently disabled — does the focus ring apply?
  (Assumption: a control carrying a consumer-set non-`Normal` state such as `Disabled`
  keeps that state — `markFocused` does not override it with `Focused`, so a disabled
  control receives no ring. State precedence, not traversal removal, is the mechanism.)
- Focus on a control inside a slot/lookless-composed subtree — the ring must paint
  on the actual focused leaf, not the carrier.
- Pointer coalescing must not drop a press/release/click that is interleaved with
  moves — only *moves* coalesce; discrete interactions are never dropped.
- A drag whose samples span a frame boundary must not lose intermediate path when
  the consumer needs it (e.g. freehand) — coalescing keeps the path for drags.
- Performance metrics during an animation cross-fade — re-measure/rebuild counts
  must reflect the overlay assembly path, not report a false full rebuild.
- `Control.map` over a control carrying keys / focus identity must preserve those
  identities (mapping changes only the message type, never structure or identity).
- Theme reuse-key vs paint-theme split must not cause a stale fragment to be reused
  when only the palette changed.

## Requirements *(mandatory)*

### Functional Requirements

**Focus visibility (US1)**

- **FR-001**: The framework MUST paint a visible focus indicator on the control
  that holds focus, for every focusable control kind, driven from the framework's
  own focus order/traversal rather than requiring the consumer to hand-walk the
  tree and stamp visual state.
- **FR-002**: Focus traversal MUST NOT skip a focusable control merely because the
  consumer did not assign it an explicit key; the focus indicator MUST be reachable
  on all focusable controls, including otherwise-unkeyed ones.
- **FR-003**: Exactly one control MUST carry the focus indicator at a time; moving
  focus MUST clear the indicator from the previously focused control with no
  residual ring.
- **FR-004**: Structural/non-focusable elements (containers, static labels) MUST
  NOT receive a focus indicator and MUST be skipped in the focus order.
- **FR-005**: A consumer-held focus model MUST have a supported way to be reflected
  into the rendered tree (the consumer's notion of "focused control" drives the
  ring), without depending on the host's own internal retained focus. *(FR-001 and
  FR-005 compose rather than conflict: the consumer supplies **which** control id is
  focused; the framework focus order/traversal and shared identity scheme make that
  control **reachable and paintable** — neither requires hand-walking the tree.)*

**Performance logging (US2)**

- **FR-006**: The interactive host MUST expose a structured per-frame metrics
  record reporting at minimum: re-measured node count, pointer samples received,
  pointer moves processed, and whether the frame performed a full view rebuild.
- **FR-007**: The metrics record's count fields MUST be deterministic / byte-stable
  for a given scripted input sequence (timing fields are excluded from determinism
  guarantees and may be reported separately).
- **FR-008**: A frame in which nothing changed MUST report zero re-measured nodes
  and no view rebuild.

**Performance testing mechanism (US3)**

- **FR-009**: The framework MUST provide a deterministic, frame-counted input
  driver that folds an ordered input script over the pure host update, advancing
  one frame per step and accumulating a byte-stable structured outcome including
  the per-frame metrics.
- **FR-010**: The test surface MUST make these assertions expressible: a pure-hover
  frame performs no full-tree rebuild; K pointer-move samples in one frame yield at
  most one processed move; an idle frame performs zero re-measure work.

**Pointer-move coalescing (US4)**

- **FR-011**: The host MUST coalesce continuous pointer-move samples to at most one
  processed move per frame, keeping the most recent position; discrete pointer
  interactions (press, release, click, drag begin/end/cancel, scroll, secondary)
  MUST NOT be coalesced or dropped.
- **FR-012**: Coalescing MUST preserve drag-path fidelity where a consumer needs the
  intermediate path (the coalesced move retains the path for drags).
- **FR-013**: An event-driven interactive tick (no animation loop) MUST be a
  supported and documented default for a controls UI; when no input arrives, no
  frame work is scheduled, and animation clocks MUST still advance from the injected
  delta when present. *(Resolution of the interacting requirement: "do work only on
  input" vs "advance animation every frame" — animation advance is driven by the
  injected delta independent of input, so an idle event-driven tick still animates
  active clocks without rebuilding the view.)*

**Composition & input ergonomics (US5)**

- **FR-014**: The public surface MUST provide a `Control.map` (and/or `Widget.map`)
  `('a -> 'b) -> Control<'a> -> Control<'b>` that changes only the message type,
  preserving structure, keys, and focus identity, so a page can be a self-contained
  sub-model + msg + update + view module.
- **FR-015**: `DataGrid` column sort MUST support a three-state cycle asc → desc →
  none (or an explicit clear-sort message) so the third toggle clears the sort
  without product-side special-casing.
- **FR-016**: The consumer key boundary MUST make modifier state observable (or
  deliver a distinct modifier-chord event) so `Ctrl`/`Alt`/`Shift` shortcuts are as
  dependable as unmodified keys; the loss of the modifier flag at the normalized-key
  boundary MUST be addressed.

**Live theming (US6)**

- **FR-017**: The framework MUST provide reusable theming helpers: resolve a
  palette from theme mode + accent, compute WCAG relative-luminance contrast ratio,
  and project a role palette onto the framework `Theme`. *(Placement: these live in
  the `FS.Skia.UI.Controls` package beside `Theme` — not the skill-support surface —
  because `toTheme` projects onto the Controls `Theme` type; see plan research
  decision D8. The earlier "skill-support surface" wording was tentative.)*
- **FR-018**: The framework MUST document the supported live-theming pattern that
  keeps the painted palette exact (model-derived theme on the render path) while the
  fragment-reuse key stays stable, so the reuse cache is not invalidated spuriously
  and never reuses a stale fragment when only the palette changed.

**Discoverability & governance (US7)**

- **FR-019**: `docs/scaffold-map.md` MUST call out the `Controls.Elmish`
  `runInteractiveApp` / `InteractiveAppHost` / `PointerInteraction` host seam as
  "present in package, not in `docs/api-surface/` — authority is the
  `fs-skia-controls-host` skill + `ControlsElmish.fsi`," alongside the existing
  typed-front-door absence note.
- **FR-020**: There MUST be a discoverable interactive-feature readiness checklist
  (in `docs/` and/or a skill) enumerating the window-visibility-class readiness
  files the `EvidenceAudit` requires for an interactive feature and their required
  `key=value` tokens, so a consumer can satisfy them before the first audit run
  rather than by failing it once.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Touches `FS.Skia.UI.Controls` (focus order/traversal, the
  visual-state stamp that paints the ring, `Control.map`/`Widget.map`, DataGrid
  sort, and the `Theming` helpers — placed in Controls beside `Theme` per plan D8),
  `FS.Skia.UI.Controls.Elmish` (the `runInteractiveApp` host loop: per-frame
  metrics, pointer-move coalescing, event-driven tick default, key
  boundary/modifier state), and the skill-support surface
  (`FS.Skia.UI.SkillSupport`: the deterministic evidence/perf tour driver). No
  legacy Charts migration. Active controls package path is the
  authoring surface; package versions bump on merge per the standard post-merge
  pack.
- **Public contract impact**: New/changed `.fsi` signatures are expected —
  `Control.map`/`Widget.map`, a consumer-focus-into-tree stamp (generalizing the
  consumer's `View.markFocused` into a framework-supported entry, ideally
  `Focus.traverse`-driven), a per-frame metrics record on the host seam, a
  modifier-aware key boundary (e.g. a `MapKeyChord` seam or a modifier accessor),
  DataGrid clear/tri-state sort, and `SkillSupport` theming helpers. These changes
  escalate `Route` to the **controls-public-surface** (maintainer-verify) gate set;
  surface baselines and per-package baselines will move and must be recaptured
  (`RefreshSurfaceBaselines`).
- **State workflow impact**: The interactive host update/effect loop changes —
  pointer-sample coalescing, event-driven tick scheduling, and a metrics
  accumulator are stateful host-loop behavior. Animation-clock advancement from the
  injected delta is preserved.
- **Layout/rendering impact**: Focus-ring painting is a rendering change (must be
  byte-identical where focus is absent / `VisualState.Normal`). Pointer coalescing
  changes when hit-test/visual-state/repaint runs but MUST keep at-rest output
  byte-identical. Damage-rect / dirty-region repaint and hover-as-local-invalidation
  are explicitly **out of scope** here (deferred follow-ups).
- **Evidence obligations**: Real evidence required — a render-diff/structural-scene
  proof that the focus ring paints on each focusable kind and on otherwise-unkeyed
  controls (US1); a deterministic per-frame metrics golden + the perf-test driver
  outcomes (US2/US3/US4); responds-proof for interactive behavior
  (`ControlsElmish.respondsProofOf` / `captureRespondsProof`) where applicable; and
  the full window-visibility-class readiness set (interactive feature) plus
  skill-loading and readiness-contract tokens. `readiness/` artifacts under the
  feature directory; `evidence-audit.md` must carry its verdict token.
- **Unsupported scope**: No damage-rect/dirty-region repaint, no
  hover-as-local-invalidation re-stamp, no X11/Wayland backend motion-event
  compression, no `speckit.snapshot-source-tree` tooling, no consumer-side
  ListView visible-window slicing — all deferred (see Out of Scope). No new
  platform/distribution targets; no live-Vulkan-window requirement (offscreen +
  responds-proof evidence is sufficient).
- **Build-target impact**: Escalated `maintainer-verify` path. Run the serialized
  six-target order: `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` →
  `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`. `RefreshSurfaceBaselines`
  must regenerate aggregate + per-package surface baselines (and skillist/skill
  tree if a new skill is added). `Route --enforce` must pass with required evidence
  present.

## Success Criteria *(mandatory)*

- **SC-001**: For every focusable control kind in a representative app, moving
  focus to it produces a visible focus indicator on that control and on no other,
  verified by an automated render-diff test (US1).
- **SC-002**: Keyboard focus traversal reaches 100% of focusable controls in the
  representative app, including controls the consumer did not explicitly key — zero
  focusable controls are skipped (US1).
- **SC-003**: A consumer can read a structured per-frame work/metrics record from
  the host without modifying the framework; the record's count fields are identical
  across repeated runs of the same scripted input (US2).
- **SC-004**: A burst of N continuous pointer-move samples occurring within a single
  frame results in at most 1 processed move and at most 1 hit-test/visual-state
  update for that frame (US4), asserted by the deterministic driver.
- **SC-005**: A pure-hover frame performs no full-tree rebuild, and an idle frame
  performs zero re-measure work, both locked by tests that fail if the behavior
  regresses (US3).
- **SC-006**: Continuous cursor movement over the representative app no longer
  stalls discrete clicks — a click issued during continuous movement is processed
  within one frame of its arrival (US4).
- **SC-007**: A page authored as a self-contained `Control<PageMsg>` module can be
  folded into a shell `Control<Msg>` via a single map call with structurally
  identical lowering (US5).
- **SC-008**: A DataGrid column sort cycles asc → desc → none with no product-side
  special-case code (US5).
- **SC-009**: A modifier-chord shortcut (`Ctrl`/`Alt`/`Shift` + key) is delivered to
  the consumer as dependably as an unmodified key — zero silent modifier loss across
  a scripted chord sequence (US5).
- **SC-010**: `contrastRatio` for known color pairs matches the WCAG relative-
  luminance reference, and the AA thresholds (≥4.5:1 normal text, ≥3:1 large) are
  checkable from the shipped helper (US6).
- **SC-011**: A reader can identify the `Controls.Elmish` host seam's authority and
  enumerate the required interactive-feature readiness files + tokens from in-repo
  docs alone, before running `EvidenceAudit` (US7).
- **SC-012**: At-rest rendering (no focus, `VisualState.Normal`, no pending input)
  is byte-identical to the pre-feature output — focus/perf changes add no visual
  drift when inactive.

## Assumptions

- The focus indicator is the existing `VisualState.Focused` style path (the style
  resolver already paints a ring for it); this feature drives that state from the
  framework focus order rather than introducing a new visual primitive.
- "All controls" means all **focusable** control kinds; structural containers and
  static decorations remain non-focusable (consistent with feature 094's
  structural-non-focusability rule).
- Offscreen deterministic evidence + responds-proof is acceptable proof for
  interactive behavior; a live Vulkan window is not required (consistent with prior
  features).
- Per-frame timing is reported but excluded from byte-stable/golden assertions;
  only counts/structural metrics are golden.
- Pointer coalescing is the in-scope performance fix; the deeper repaint
  optimizations from the feedback survey (damage-rect repaint, hover-local
  invalidation, backend motion compression) are separate future features.
- The deterministic perf-test driver generalizes the consumer's frame-counted
  "tour" into a `SkillSupport`-level reusable combinator (matching both plan- and
  implement-phase feedback).
- Modifier-state exposure can be satisfied either by surfacing the modifier on the
  normalized key or by a distinct chord boundary; the spec requires the *capability*
  (no silent loss), not a specific API shape.

## Out of Scope (deferred follow-ups)

- **Damage-rect / dirty-region repaint** (feedback P2): repaint only the changed
  region instead of the whole window. Large `RetainedRender` change; separate
  feature.
- **Hover as localized per-node visual-state toggle** (feedback P3): re-stamp +
  invalidate only entered/left nodes with no `host.View` rebuild. Depends on the
  damage-rect work.
- **X11/Wayland backend motion-event compression** (feedback P5): drop all but the
  latest queued motion event at the SkiaViewer backend. Backend change; separate
  feature.
- **`speckit.snapshot-source-tree` tooling** (specify-phase feedback): a speckit
  subcommand to ingest a remote multi-file spec tree into one provenance-stamped
  snapshot. Tooling/process improvement, not an FS.Skia.UI runtime change.
- **Consumer-side ListView visible-window slicing** (feedback P6): pass only the
  visible slice into the view. This is a consumer-side mitigation, not a framework
  change.

## Dependencies

- Builds on feature 094 (Focus.fsi order/traverse/route) and feature 096+
  (runtime visual-state bridge / `applyRuntimeVisualState`) for the focus-ring
  path, and on the wired retained render path (091/092/097/099/103) for per-frame
  metrics and animation-clock advancement.
- The `fs-skia-controls-host`, `fs-skia-viewer-host`, `fs-skia-evidence-mode`, and
  `fs-skia-design-tokens` skills are the documentation homes for the host-loop,
  evidence-tour, and theming additions.
