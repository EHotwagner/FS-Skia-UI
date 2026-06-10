# Interactive Visible Window Evidence (094)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=false

## Why not-applicable

Feature 094 (E4) adds a **pure focus model** (`Focus.order` / `traverse` / `route`) and an
**internal host key-routing seam** (`routeFocusedKey`) wired into the already-existing
`ControlsElmish.runInteractiveApp`. It opens **no new desktop window**: the focus reducers are
pure, the `routeFocusedKey` route-probe is offscreen (through the real adapter path via
`InternalsVisibleTo`, no hand-seeded map), and the focus-indicator + responds-proof are exercised
through the production `Control.renderTree` path with deterministic `Scene` equality.

The window-visibility evidence class is triggered only because the feature text names
`real-image-evidence.md`; this record honestly declares `mode=render-only` with no window claim.
The live desktop window that surfaces this behavior is `runInteractiveApp`, whose visibility was
established by the earlier interactive-host features (085/092) and is unchanged here. No
taskbar-only or process-only substitution is claimed.
