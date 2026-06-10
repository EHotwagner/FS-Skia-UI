# Phase 1 Data Model: Wire Retained Identity Into Live Interactive State

Entities are mostly *existing* types from feature 091; this feature changes a few fields and adds
one hit-test result. No new persisted data; all state is in-memory at the interpreter edge.

## RetainedId (unchanged)

`RetainedId of uint64` — the stable, monotonic, diff-conferred per-node identity. Distinct per
node, including unkeyed siblings (this is what FR-004 hit-testing returns). Survives positional
shifts; not reminted for carried nodes.

## RenderFragment (unchanged shape)

`{ OwnScene; SubtreeScene; Box : Rect option }` — cached measure/paint for one node. Reused only
when its paint inputs are provably unchanged. **New reuse precondition (FR-008)**: reuse is
additionally gated on the per-loop theme being unchanged (enforced at the `RetainedRender` level,
not by adding a field here — see below).

## RetainedNode<'msg> (unchanged)

`{ Identity : RetainedId; Control : Control<'msg>; Fragment; Children }`. The `Fragment.Box` +
`Identity` of each node are the inputs to the new `retainedHitTest`.

## RetainedUiState (unchanged shape; now actually populated in production)

`{ Animation : AnimationState<Transform> option; Text : TextInputModel option }`. Per-control UI
state keyed by `RetainedId`. **Change**: the live adapter now writes and reads this (focus seeds
`Text` from the control's value; keystrokes advance it), where 091 left it populated only by tests.

## RetainedRender<'msg> (one new field)

| Field | 091 | 092 |
|-------|-----|-----|
| `Root : RetainedNode<'msg>` | ✓ | ✓ |
| `NextId : uint64` | ✓ | ✓ |
| `StateByIdentity : Map<RetainedId, RetainedUiState>` | ✓ (carried, unused by host) | ✓ (carried **and consumed by the host**) |
| `Theme : Theme` | — | **new** — the theme this structure was painted under; compared in `step` to gate fragment reuse (FR-008) |

## WorkReductionRecord (one new field; corrected invariant)

| Field | 091 | 092 |
|-------|-----|-----|
| `BaselineNodeCount : int` | ✓ (= N) | ✓ |
| `RecomputedNodeCount : int` | ✓ | ✓ |
| `ChangedSubtreeBound : int` | ✓ | ✓ (now *only* genuinely-changed work) |
| `ShiftedNodeCount : int` | — | **new** — nodes recomputed only because an upstream change relaid them out |

**Invariant (corrected, FR-007)**: `RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount`
and `RecomputedNodeCount < BaselineNodeCount` for any localized change. (091 documented
`RecomputedNodeCount ≤ ChangedSubtreeBound`, which a sibling-shifting change violates.)

## RetainedRenderStep<'msg> (unchanged shape)

`{ Retained; Render; Diagnostics; WorkReduction }`. `WorkReduction` now carries the shifted count.

## First-frame result (init return shape change — FR-009)

`RetainedRender.init` changes from returning `RetainedRender<'msg>` to returning the initial
retained structure **plus first-frame `Diagnostics`** (duplicate-key `KeyCollision` detected on the
first tree) and exposing the painted scene so the adapter paints once. Shape captured in
`contracts/RetainedRender.fsi.md`.

## RetainedHitResult (new, FR-004)

`retainedHitTest : x:float -> y:float -> RetainedRender<'msg> -> RetainedId option` — the deepest
retained node whose `Fragment.Box` contains the point, else `None` (point in a true gap / outside
the root). Per-node distinct, so unkeyed same-kind siblings resolve to different ids.

## Interpreter-edge focus state (re-keyed)

| Closure ref | 091/090 | 092 |
|-------------|---------|-----|
| focused control | `focusedText : ControlId option` | `RetainedId option` |
| text models | `textModels : Map<ControlId, TextInputModel>` | removed — lives in `RetainedRender.StateByIdentity[id].Text` |
| retained structure | `retained : RetainedRender option` | unchanged (now the home of UI state) |
| surfaced diagnostics dedup | `Set<string>` | unchanged (also fed by frame-0 diagnostics) |

## Validation / state-transition rules

- **Focus acquisition** (pointer Pressed over a control with an `onChanged` binding): resolve
  `RetainedId` via `retainedHitTest`; set focus ref; if no `StateByIdentity` entry exists, seed
  `Text` from the control's current value + kind-derived line mode (R3).
- **Keystroke** (printable key-down, focus set): advance `StateByIdentity[focused].Text` via
  `TextInput.update`; dispatch **all** matched `onChanged` product messages (R4).
- **Frame transition** (`step`): `StateByIdentity` carried to matched identities, dropped for
  `Replace`/removed; if theme changed, no fragment reused (R6); shifted work counted (R5).
- **Focused control removed**: its identity leaves the live set → its `StateByIdentity` entry is
  filtered out and focus clears (edge case).
