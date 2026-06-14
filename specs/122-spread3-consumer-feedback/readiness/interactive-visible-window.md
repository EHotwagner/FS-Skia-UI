# interactive-visible-window — applicability (feature 122)

status=not-applicable
mode=deterministic-present-decision-evidence
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 122 changes the **live** `DirectToSwapchain` present path (bounded re-present of the cached last
good frame so no swapchain buffer is left undrawn, US1) and threads window behavior into the live
controls launch (US2), plus a `CustomControl` null guard and documentation. It ships an additive
`runInteractiveAppWithWindowBehavior` overload but drives **no interactive window** in this evidence. The
Wayland windowed-fullscreen visual blink is not reproducible in the headless / no
windowed-fullscreen-compositor CI environment (recorded in `runtime-limitations.md`); the present-path
change is proven on the pure `GlHost.planPresent` decision + buffer-fill state machine (unit-tested in
isolation) and the byte-identical offscreen/readback goldens. The `runInteractiveApp` launch contract is
unchanged (the overload is additive). No interactive-window pass is claimed.
