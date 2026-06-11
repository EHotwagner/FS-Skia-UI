# Interactive Visible Window Evidence (099, R4)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=false

## Why not-applicable

Feature 099 (R4) wires a per-identity animation clock into the **existing** `runInteractiveApp` host
loop: the wrapped `Tick` advances live clocks by the injected delta and `RetainedRender.step` samples
them on paint. It opens **no new desktop window** and adds **no** default-executable / persistent-launch
entry point. The animates-vs-snaps / survival / GC / scoped-repaint proofs are exercised through the
production retained render path (`RetainedRender.advance`/`step` + `ControlRuntime.applyRuntimeVisualState`)
with deterministic `Scene` equality.

The window-visibility evidence class is triggered only because the feature text names
`real-image-evidence.md`; this record honestly declares `mode=render-only` with no window claim. The
live desktop window that surfaces this behavior is `runInteractiveApp`, whose visibility was
established by the earlier interactive-host features (085/092/096) and is unchanged here — a generated
project consuming `runInteractiveApp` gains live animation automatically with no scaffold change. No
taskbar-only or process-only substitution is claimed.
