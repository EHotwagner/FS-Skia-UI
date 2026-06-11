# Interactive visible window — applicability (feature 103, R6, T002/T003)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

R6 ships **no** persistent/interactive graphical entry point. The cross-fade is GPU-free deterministic
scene assembly proven through `RetainedRender.step` with injected deltas — there is no live window, no
desktop-visibility claim, and no screenshot. The story is an internal framework behavior change reached
in-assembly via `InternalsVisibleTo`, not a host a user drives with pointer/keyboard.
