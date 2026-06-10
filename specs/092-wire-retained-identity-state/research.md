# Phase 0 Research: Wire Retained Identity Into Live Interactive State

All NEEDS CLARIFICATION are resolved (the three either/or decisions were settled in
`/speckit-clarify`, recorded in spec.md § Clarifications). This file records the design decisions
that flow from them and from reading the current code.

## R1 — FR-004: resolve a click to a stable per-node identity via the retained tree

**Decision**: Add an internal `RetainedRender.retainedHitTest : point -> RetainedRender<'msg> ->
RetainedId option` that walks the retained tree and returns the **deepest** `RetainedNode` whose
`Fragment.Box` contains the point. Focus-on-click resolves through this, not through
`Control.hitTest`/`nearestAuthored`.

**Rationale**: The live adapter today does `Control.hitTest rendered x y |> Option.bind
(Control.nearestAuthored rendered)`. `Control.hitTest` keys on `ControlId = Key |> defaultValue
Kind`, so two unkeyed same-kind siblings collapse to the same id, and `nearestAuthored` uses a
*different* (path-derived) scheme — the two disagree for unkeyed/wrapped controls (the 090 review
finding). The retained tree already mints a **distinct `RetainedId` per node** (even unkeyed) and
stores each node's evaluated `Fragment.Box`, so a box hit-test over it yields a per-node identity
with no collision — directly satisfying FR-004's "fully disambiguate unkeyed siblings" decision.
Reusing the boxes already computed by `step`/`init` avoids a second layout pass.

**Alternatives considered**: (a) keep `Control.hitTest` and make `nearestAuthored` agree — still
collapses unkeyed same-kind siblings, so it cannot satisfy the clarified FR-004; (b) add keys
implicitly — rejected by the clarification (no key may be required).

## R2 — FR-001/2/3: thread live focus/text/clock state on `RetainedId`

**Decision**: Replace the closure refs `focusedText : ControlId option` and `textModels :
Map<ControlId, TextInputModel>` with focus keyed on `RetainedId` and the text/animation state held
in the retained structure's `StateByIdentity : Map<RetainedId, RetainedUiState>` (its `Text` field
already exists). On focus-acquisition the adapter writes a `RetainedUiState` for the focused
`RetainedId` into the current retained structure; each keystroke updates that entry; `step` already
carries `StateByIdentity` across frames keyed by identity (filtering to live ids), so a positional
shift preserves it and a `Replace`/removal drops it (FR-003).

**Rationale**: This makes the *running host* exercise the survival mechanism that 091 built and
only tested by hand-seeding. Because `step` carries `StateByIdentity` for matched (`Keep`/`Update`/
moved) nodes and drops it for `Replace`/removed nodes, FR-003 falls out of the existing diff with
no new carry logic. `RetainedRender` becomes the single home of per-control UI state (no parallel
`ControlId` map), satisfying FR-002.

**Note**: focus itself stays a small `RetainedId option` ref at the edge (the
`RetainedRender.fsi` comment already says focus "stays in the consumer model's
`ControlRuntime.FocusedControl`; 091 only remaps the lookup to `RetainedId`"); the adapter's edge
ref is the lookup key, consistent with that contract.

## R3 — FR-005: seed the edit buffer from the field's value and honor line mode

**Decision**: On focus-acquisition, seed `TextInput.init` with the **control's current value**
(read from the focused authored control's text attribute / content) and the **line mode derived
from its kind** (`text-area` → `MultiLine`, otherwise `SingleLine`) — not the current hard-coded
`TextInput.init authored SingleLine ""`.

**Conflict resolution (spec FR-005 vs FR-001/3)**: the carried draft is authoritative while the
control stays focused — the model value re-seeds the draft **only on the focus-acquisition
transition**, never on an ordinary re-render — so a same-frame model change cannot silently
overwrite in-progress typing, and the first keystroke after focus appends to the existing value
rather than erasing it.

**Rationale**: Fixes the 090 defects (empty seed wipes a pre-filled field on first keystroke;
`SingleLine` hard-coded even for `TextArea`) on the same path being rewired.

## R4 — FR-006: dispatch every matched change-binding (public seam widening)

**Decision**: Widen `InteractiveViewerHost.MapKey` from `ViewerKey -> bool -> 'msg option` to
`ViewerKey -> bool -> 'msg list` (empty list = unhandled, replacing `None`), and have the viewer
fold every returned message through `update`. The `ControlsElmish` text seam then returns all
matched `onChanged` messages instead of `List.tryHead`.

**Rationale**: The text routing already computes a `'msg list`; the single-`'msg option` seam is
the only reason all-but-the-first are dropped. There is no other channel from a key event into the
update loop, so carrying the list is the minimal honest fix. This is a **Tier 1 public-surface
change** to `SkiaViewer.fsi` (and the `ControlsElmish` `mapKey`), justified because no edge-side
workaround can dispatch N messages through a 1-message seam, and the change is backward-shaped
(`Some m` → `[m]`, `None` → `[]`). A compatibility/migration note ships in contracts/.

**Alternatives considered**: keep `'msg option` and document FR-006 as unmet — rejected, it is a
stated requirement; batch into one synthetic message — rejected, not generically possible for
arbitrary `'msg`.

## R5 — FR-007: distinct shifted-work counter + corrected documentation

**Decision**: Add `ShiftedNodeCount` to `WorkReductionRecord`. `RetainedRender.step`'s `carry`
path (which recomputes a shifted-but-unchanged subtree) increments a `shifted` counter instead of
leaving the work uncounted; `changed` work (`Replace`/own-change/`ChildInsert`) keeps incrementing
`ChangedSubtreeBound`. The `.fsi` documentation is corrected to state the relationship as
`RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount ≤ BaselineNodeCount`, with
`RecomputedNodeCount < BaselineNodeCount` for any localized change.

**Rationale**: The current doc claims `RecomputedNodeCount ≤ ChangedSubtreeBound`, which a
sibling-shifting change violates (the 091 review finding: `[editor]→[banner,editor]` gives
recomputed=2, bound=1). Separating *changed* from *shifted* makes the measure honest and keeps the
SC-003 evidence trustworthy. This is a measurement change only — produced output is unchanged
(FR-010 wins any conflict).

## R6 — FR-008: theme in the fragment reuse key

**Decision**: Store the per-loop `Theme` on `RetainedRender<'msg>`. In `step`, if `prev.Theme` ≠
the new `theme`, no fragment may be reused (every node repaints under the new theme); otherwise the
existing box-based reuse stands. (Theme is uniform across the tree per frame, so a single top-level
theme comparison suffices — no per-fragment theme storage needed.)

**Rationale**: Fragments cache paint produced under a specific theme; reusing them across a theme
change would show stale-theme paint, contradicting the round-trip byte-identity guarantee. The
clarification chose folding theme into the reuse key (future-proofing E-series theme switching)
over a constant-theme precondition. A whole-tree theme comparison is the cheapest correct form
because theme is not per-node.

## R7 — FR-009: single first-frame paint + first-frame diagnostics

**Decision**: `RetainedRender.init` returns the first-frame `Diagnostics` (detecting duplicate
sibling keys via the same check `Reconcile.diff` uses) and the adapter's `renderRetained` `None`
branch uses `init`'s already-painted fragments for the scene instead of calling `Control.renderTree`
a second time. Frame-0 diagnostics are surfaced through the same de-duped channel as later frames.

**Rationale**: Today the `None` branch calls both `RetainedRender.init` and `Control.renderTree`
(two full paints), and `init` never diffs, so a duplicate-key collision present in the first tree
is not reported until frame 1. Returning diagnostics from `init` and reusing its scene closes both
gaps with no output change. `init`'s signature changes (now returns a small first-frame result) —
internal `.fsi` only.

## R8 — Testable seam so SC-001 drives the real adapter without a window

**Decision**: Keep the focus/text routing as named seam functions on the `ControlsElmish` surface
that operate over `(RetainedRender, focused RetainedId, input)` and return the next retained state +
product messages — the same functions the `runInteractiveApp` closure calls. Tests drive
init → focus(click) → keystroke → step(shift) → keystroke directly against these seams.

**Rationale**: SC-001 requires proving survival "through the real adapter path, with no manual
seeding of the identity-keyed state map." The full closure needs a live viewer; extracting the
exact routing the closure uses lets the test exercise the production code path deterministically
(render-only, no Vulkan window — `fs-skia-evidence-mode`), while the closure remains a thin wiring
of those seams. This mirrors how feature 090's `routeFocusedText`/`routeInteractivePointer` are
already structured and tested.

## Open risks

- **MapKey widening blast radius**: every `InteractiveViewerHost` constructor (in `SkiaViewer`
  consumers + the `ControlsElmish` adapter) must update `MapKey`. Mechanical (`Some`→singleton,
  `None`→`[]`); covered by the compiler and the contract tests. Recorded as the one Tier-1
  complexity in the Constitution Check.
- **Reading a control's current text value (R3)** depends on how `TextBox`/`TextArea` store their
  value (attribute vs content); confirm the accessor during data-model/implementation and keep the
  seam tolerant of an absent value (treat as empty).
