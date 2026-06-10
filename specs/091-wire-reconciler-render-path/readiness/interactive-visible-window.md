# Interactive Visible Window Evidence (091)

status=deferred
mode=render-only-offscreen
window-visible=deferred
accessible-window=deferred
first-frame-presented=deferred
self-closed-for-evidence=false

## Render-only / offscreen posture (no live window required)

Feature 091 wires the parked keyed reconciler (067) onto the production render path via
`module internal RetainedRender`, consumed by `ControlsElmish.runInteractiveApp`'s `View`
closure. The behavioral change is **internal** (each frame is produced by diffing the next tree
against a retained previous tree and reusing unchanged subtrees) and is **byte-for-byte identical**
to the prior full rebuild (FR-005). It adds **no** new interactive surface, so the interactive-UI
run-and-use gate does **not** apply to a new surface here.

Per the plan and [[fs-skia-evidence-mode]], every success criterion is proven **render-only /
offscreen** with **no live Vulkan window**:

- SC-004 golden parity — pure structural equality of `ControlRenderResult.Scene` (wired ==
  `Control.renderTree next`), asserted by `Feature091RetainedRenderTests`.
- SC-002 focus/animation survives — the stable `RetainedId` is carried across an unrelated
  re-render (`RetainedRender.StateByIdentity`); a rebuild-every-frame baseline fails the same
  proof.
- SC-003 work reduction — measured `RecomputedNodeCount` vs `BaselineNodeCount`.

A live persistent-window launch is **deferred** (not performed in this run — this feature is
internal render-path wiring with no new interactive surface) and is **not required** for this
feature's evidence. The environment **HAS** a GPU and a live Vulkan/Skia window can open (via a
compiled exe on the X11 path); the deferral is by scope, not a hardware limitation. No
taskbar-only / process-only success is claimed.
