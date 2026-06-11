# Widened kinds — restyle + focus on the representative set (feature 096, T015/T023, SC-006, FR-006)

evidence-kind=widened-kinds
renderer-mode=DeterministicRenderOnly
status=pass

The migrated geometry set is widened from `button`/`check-box` (E3/093) to add `slider`/`text-box`/
`radio-group`/`switch`. Each widened kind routes its paint through `Style.resolve theme baseStyle
classes state`; at `classes = []`, `state = Normal` the base reproduces the prior procedural colours
exactly, so the at-rest render is **byte-identical** to today (FR-006).

Per-kind the visible response channel differs (each kind draws the channel its chrome naturally owns):

| kind         | base reproduces            | visible runtime response                         |
|--------------|----------------------------|--------------------------------------------------|
| button       | accent fill / outline (E3) | Pressed → Muted fill                             |
| check-box    | accent box / tick (E3)     | state-driven fill/stroke                         |
| slider       | accent filled-track + thumb| Pressed/Hover/Selected/Disabled restyle the fill |
| switch       | accent/muted track         | Pressed/Hover/Selected/Disabled restyle the fill |
| radio-group  | accent ring (selected)     | Pressed/Hover/Selected/Disabled restyle the ring |
| text-box     | foreground border + label  | Focused → accent border (a visible focus indicator) |

Observed:
- each widened kind: attaching `Normal` is byte-identical to the unset (at-rest) render, and a runtime
  visual state restyles its resolved paint (`notEqual`).
- unmigrated kinds (`progress-bar`, `numeric-input`, and by extension `toggle-button`/`list-box`/
  `multi-select-list`/`combo-box`) show **no render-output delta** when a runtime state is stamped —
  the widening is additive and scoped to the representative set.

result=pass — the representative focusable set restyles/focus-indicates; unmigrated kinds are unchanged.
authoritative-test=Feature096RuntimeBridgeTests/Feature 096 runtime visual-state bridge (T015/T023)
