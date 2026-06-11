# Interactive Visible Window Evidence (100, R5)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=false

## Why not-applicable

Feature 100 (R5) generalizes the focused-control navigation path in the **existing**
`runInteractiveApp` host loop: it widens `Focus.route` to a closed role-derived `NavIntent` and
replaces the slider-only `Navigate` arm with a uniform per-intent resolver. It opens **no new desktop
window** and adds **no** default-executable / persistent-launch entry point. The selection-move /
declared-step / grid-move proofs are exercised through the production retained render path
(`RetainedRender.init`/`step` + `ControlsElmish.routeFocusedKey`) with deterministic dispatch
assertions and no live Vulkan window.

The window-visibility evidence class is triggered only because the feature text names
`real-image-evidence.md`; this record honestly declares `mode=render-only` with no window claim. The
live desktop window that surfaces this behavior is `runInteractiveApp`, whose visibility was
established by the earlier interactive-host features (085/092/096) and is unchanged here — a generated
project consuming `runInteractiveApp` gains general navigation automatically with no scaffold change.
No taskbar-only or process-only substitution is claimed.
