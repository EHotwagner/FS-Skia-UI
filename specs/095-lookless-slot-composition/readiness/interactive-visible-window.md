# Interactive visible window (feature 095)

Feature 095 is a **deterministic render-only** structural-lowering feature ([[fs-skia-evidence-mode]]).
It ships **no** interactive host, no live window, and no new render path — slot fills lower into the
control's `Children` and are exercised through structural `Scene` / lowered-`Control<'msg>` equality.
There is therefore **no** persistent graphical launch obligation (a visible decision recorded in
`tasks.md` T003). The fields below are recorded as deferred / not-applicable so the contract is
satisfied honestly rather than with a substitute-visibility claim.

status=deferred
mode=deterministic-render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable
process-running=not-applicable
process-only=false
taskbar-entry=false

Note: no live window is opened by this feature; parity is structural Scene equality, not a desktop
screenshot. The authoritative proof is the Feature095SlotCompositionTests suite.
