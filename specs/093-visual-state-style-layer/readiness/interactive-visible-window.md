# Interactive Visible Window Evidence (093)

status=deferred
mode=render-only-offscreen
window-visible=deferred
accessible-window=deferred
first-frame-presented=deferred
self-closed-for-evidence=false

## Render-only / offscreen posture (no live window required)

Feature 093 (E3) adds a **pure state→style resolver** plus a declarative
style-class/variant authoring surface, and migrates Button/CheckBox paint onto
the resolver. It introduces **no** new interactive surface — the consumer
`view : 'model -> Control<'msg>` contract is unchanged and a consumer who
attaches no class renders byte-identically. So the interactive-UI run-and-use
gate does **not** apply (no new host app the user drives).

Per the plan and [[fs-skia-evidence-mode]], every success criterion is proven
**render-only / offscreen** with **no live Vulkan window**:

- SC-001/SC-002 — variant/state distinctness + fixed precedence are pure
  `ResolvedStyle` comparisons (`Feature093StyleResolverTests`).
- SC-003 — migrated-kind parity is structural-`Scene` equality vs the frozen
  procedural baseline (`Feature093ParityTests`).
- SC-004 — purity/determinism + precedence over ≥1000 FsCheck inputs.
- SC-005 — the state-driven look survives a sibling shift through the **live
  retained path** (`RetainedRender.init`/`step`) — exercised offscreen via the
  real production code path, not a live window.

A live persistent-window launch is **deferred** (not performed — this feature is
a pure styling layer with no new interactive surface) and is **not required**.
The environment HAS a GPU and a live Vulkan/Skia window can open (via a compiled
exe on the X11 path); the deferral is by scope, not a hardware limitation. No
taskbar-only / process-only success is claimed.
