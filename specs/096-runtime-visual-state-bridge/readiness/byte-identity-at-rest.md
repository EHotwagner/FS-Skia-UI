# Byte-identity at rest (feature 096, T012/T016, FR-005, SC-003, SC-008)

evidence-kind=byte-identity-at-rest
renderer-mode=DeterministicRenderOnly
status=pass

A `Normal`-and-unset control is returned from `applyRuntimeVisualState` **unchanged** — the bridge
emits nothing at `Normal`, so the un-bridged build stays structurally identical.

Observed:
- `applyRuntimeVisualState emptyModel tree` is structurally equal (`%A` projection) to `tree` — no
  attribute is added anywhere when nothing is hovered/pressed/focused/selected.
- the at-rest bridged tree renders **Scene-byte-identical** to the un-bridged build
  (`Control.renderTree theme size bridged).Scene = (Control.renderTree theme size tree).Scene`).
- the live retained step recomputes **0 nodes** at rest
  (`RetainedRender.step` `WorkReduction.RecomputedNodeCount = 0`): a `Normal`-and-unset frame is an
  identity-at-rest `Keep` no-op, so `RecomputedNodeCount` is unchanged and the E2/E3 fast paths are
  untouched.

The `view : 'model -> Control<'msg>` consumer contract is unchanged — the addition is purely additive
(SC-008): a single new public `deriveVisualState` projection plus an internal bridge; no new
binding/observable/dependency-property/selector/template surface.

result=pass — `Normal`+unset is byte-identical to the un-bridged render and adds no recompute at rest.
authoritative-test=Feature096RuntimeBridgeTests/Feature 096 runtime visual-state bridge (T012)
