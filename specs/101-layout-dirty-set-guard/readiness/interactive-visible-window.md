# Interactive visible window — applicability (feature 101, R7)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

R7 adds no interactive, persistent, or graphical entry point. It is a build/test-time anti-drift guard
over the existing pure `evaluateLayout` / `layoutDirtySet` / `layoutAffectingAttrNames`, exercised by
in-process Expecto tests with no window, GPU, or wall-clock dependency. There is no window to make
visible, accessible, present a first frame, or self-close for evidence. Rendering output is
byte-identical to pre-R7 (R2 INV-1), so no window-visibility obligation arises (see
[window-visibility.md](./window-visibility.md)).
