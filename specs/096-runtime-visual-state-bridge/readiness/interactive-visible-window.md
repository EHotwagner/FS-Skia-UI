# Interactive Visible Window Evidence (096)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=false

## Why not-applicable

Feature 096 (R1) adds a **pure projection** (`ControlRuntime.deriveVisualState`), a **pure internal
host bridge** (`applyRuntimeVisualState`), a `renderRetained` call site, and four widened geometry
functions. It opens **no new desktop window**: the projection and bridge are pure, and the
live-restyle / focus-survival / responds-proof are exercised through the production retained render
path (`RetainedRender.init`/`step`) and `Control.renderTree` with deterministic `Scene` / resolved-style
equality.

The window-visibility evidence class is triggered only because the feature text names
`real-image-evidence.md`; this record honestly declares `mode=render-only` with no window claim. The
live desktop window that surfaces this behavior is `runInteractiveApp`, whose visibility was
established by the earlier interactive-host features (085/092) and is unchanged here — a generated
project consuming `runInteractiveApp` gains live restyle/focus automatically with no scaffold change.
No taskbar-only or process-only substitution is claimed.
