# Contract: Internal targeted runtime visual-state stamp

**Module**: `FS.Skia.UI.Controls` — `ControlRuntime` (internal seam). No public
signature change (the public `deriveVisualState` and the internal full-tree
`applyRuntimeVisualState` are unchanged). The new surface is the internal
`RuntimeStampResult` type + `val internal applyRuntimeVisualStateTargeted`, reached by
`Controls.Elmish` within the assembly and by `Controls.Tests` via `InternalsVisibleTo`.

## Signature (post-feature)

```fsharp
/// Feature 112 (FR-007): the targeted-stamp result — the stamped tree + the number of
/// nodes the targeted walk rebuilt this frame (the changed-state paths). Internal.
type internal RuntimeStampResult<'msg> =
    { Stamped: Control<'msg>
      RuntimeStateTouchedNodeCount: int }

/// Feature 112 (FR-001..FR-005): re-stamp only the controls whose FINAL visual state
/// changed between `prev` and `cur`, reusing every unchanged subtree from `prevStamped`.
/// `finalState M node` = consumer-set state if non-Normal, else `deriveVisualState M id`.
/// Byte-identical to `applyRuntimeVisualState cur fresh` (the full oracle). Internal.
val internal applyRuntimeVisualStateTargeted:
    prev: ControlRuntimeModel ->
    cur: ControlRuntimeModel ->
    prevStamped: Control<'msg> ->
    fresh: Control<'msg> ->
        RuntimeStampResult<'msg>
```

XML-doc on the new type + val is **required** (doc-preservation gate). The full-tree
`applyRuntimeVisualState` keeps its doc + signature (oracle/fallback).

## Behavioural contract

Given the previous + current runtime models, the previous frame's **stamped** tree, and
the current **fresh** un-stamped view tree (same structure on the model-unchanged path),
the targeted stamp MUST, for each node (zipped):

1. Compute `finalCur = finalState cur node` and `finalPrev = finalState prev node`,
   where `finalState M node = if visualStateOf node.Attributes <> Normal then that state
   else deriveVisualState M (node.Key ?? node.Kind)`.
2. If `finalCur = finalPrev` AND no descendant changed → **reuse** the `prevStamped`
   node instance untouched (contributes `0` to the count).
3. Else → **rebuild** from the `fresh` node: stamp `finalCur` (via `setVisualState`,
   or leave NO `visualState` attribute when `finalCur = Normal`), recurse children;
   contribute `+1` to the count.
4. Return `{ Stamped; RuntimeStateTouchedNodeCount }`.

## Parity obligation (FR-005 / SC-002)

For every tree and every hover-move / focus-move / press-toggle transition, the
targeted stamp's `Stamped` MUST render (via `Control.renderTree`) byte-identically to
`applyRuntimeVisualState cur fresh` (the full oracle), and resolve the same per-control
visual state. Proven by a direct two-path comparison test
(`Feature112TargetedStampParityTests.fs`) over: keyed controls, nested containers,
unkeyed same-kind siblings, and consumer-set controls. Controls have no general value
equality → compare the rendered `Scene` (which has equality) + the resolved visual
states.

## Precedence rule (FR-003 / SC-004)

A consumer-set non-`Normal` visual state (`Disabled`, `Selected`, …) wins over a derived
hover/focus/press: its `finalState` is the consumer state under BOTH models, so it never
changes and is never re-stamped by a derived transition. A derived `Normal` emits NO
`visualState` attribute (byte-identity at rest, FR-008).

## No-change rule (FR-004 / SC-003)

When `cur` and `prev` derive the same final state for every node (e.g. hover persists on
the same control, or a fully at-rest frame), `RuntimeStateTouchedNodeCount = 0` and the
returned `Stamped` reuses every subtree instance from `prevStamped` (no rebuild).

## Fallback rule (FR-006)

The targeted stamp is wired only on the live model-unchanged path. A model-changing /
first / structurally-misaligned frame uses the full `applyRuntimeVisualState` oracle;
the resulting scene still equals the oracle's (it IS the oracle). Normal hover/focus/
press frames never fall back.

## Wiring

- `renderRetained` `Some prev` branch (`ControlsElmish.fs:912-920`): on a model-unchanged
  frame, call `applyRuntimeVisualStateTargeted lastRuntimeModel curModel prev.Root.Control
  (viewFor size model)`; use `.Stamped` as `next` for `RetainedRender.step`; surface
  `.RuntimeStateTouchedNodeCount` best-effort. On a model-changing frame, use the full
  oracle as today.
- `renderRetained` `None` first-frame branch (`:905-911`): full oracle (no prior stamp).
- Store the current runtime model into `lastRuntimeModel` after each stamp.
