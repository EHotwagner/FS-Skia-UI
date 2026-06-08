# Interactive Visible Window Evidence (080)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=false

## Why not-applicable

Feature 080 is a **renderer + governance** feature (faithful per-control preview geometry, a
decoded-content fidelity gate, and a new build target/routing rule). It adds no default
executable, no persistent interactive viewer, and no desktop window. The window-visibility
evidence class is triggered only because the feature text names `real-image-evidence.md`; this
record honestly declares `mode=render-only` with no window claim.

The only images this feature produces are committed **control preview** PNGs generated through
the deterministic render-only evidence path (`ViewerRenderTargetPng`, off-window raster) — no
window. Their decoded honesty is recorded in `control-fidelity.md` and `real-image-evidence.md`,
not here. No taskbar-only or process-only substitution is claimed.
