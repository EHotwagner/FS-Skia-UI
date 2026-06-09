# Interactive Visible Window Evidence (084)

status=deferred
mode=render-only
window-visible=deferred
accessible-window=deferred
first-frame-presented=deferred
self-closed-for-evidence=false

## Scope of this record

Feature 084 is a **framework + governance + template** change: it adds the
`WindowedFullscreen` startup state, makes it the default, reclassifies fullscreen /
windowed fullscreen as **honored**, and wires the generated launcher to honor the
parsed window-behavior request (`runAppWithWindowBehavior`). The framework repo
ships **libraries + a template**, not a runnable windowed product, so no visible
desktop window is opened from this repo's own validation.

The new states and the new default are exercised here through the **public surface**
(real evidence): `readiness/fsi-session.txt` confirms
`defaultWindowBehavior.StartupState = WindowedFullscreen` and that
`validateWindowBehavior`/`validateWindowLaunchBehavior` report `Honored` for
`Fullscreen` and `WindowedFullscreen` and `UnsupportedOption` for `Minimized`
(SC-001/SC-002), and `tests/SkiaViewer.Tests` asserts the same against the built
library.

The **real visible-window launch** (SC-001/SC-002 launch evidence for the
windowed-fullscreen default and each supported state) is captured from the
**generated default executable** on a **display-capable host** — the same path the
project documents as locally **non-authoritative** for `GeneratedProductCheck`
(see `aggregate-hang-diagnostics.md`). On this framework dev host that launch is
**deferred** (`mode=render-only`); no taskbar-only or process-only substitution is
claimed and no false visible-window is asserted (spec Edge case: headless degrades
to honest render-only).
