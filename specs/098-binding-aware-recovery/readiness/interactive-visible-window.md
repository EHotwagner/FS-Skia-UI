# Interactive Visible Window Evidence (098, R3)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=false

## Why not-applicable

Feature 098 (R3) is a framework-internal **id-derivation and recovery** correction: it unifies the canonical
`ControlId` to `Key ?? structural-path`, adds a pure `BoundIds : Set<ControlId>` field + `boundIdsOf`
derivation, and widens the pure `nearestAuthored` predicate. It opens **no new desktop window**: the
recovery/dispatch is exercised through the production live-adapter routing seam
(`ControlsElmish.routeInteractivePointer` — the exact seam `runInteractiveApp` wires) and the
deterministic `Control.renderTree` / property suites.

The window-visibility evidence class is triggered only because the feature text names
`real-image-evidence.md`; this record honestly declares `mode=render-only` with no window claim. The live
desktop window that surfaces this behavior is `runInteractiveApp`, whose visibility was established by the
earlier interactive-host features (085/092) and is unchanged here — a generated project consuming
`runInteractiveApp` gains correct unkeyed-button dispatch automatically with no scaffold change. No
taskbar-only or process-only substitution is claimed.
