# Live restyle — zero consumer code (feature 096, T016, SC-001)

evidence-kind=live-restyle
renderer-mode=DeterministicRenderOnly
status=pass

US1: a migrated control whose id the `ControlRuntimeModel` reports hovered / pressed / selected
resolves to the matching `Hover` / `Pressed` / `Selected` style with a **no-attribute** consumer view
— the consumer wrote zero styling. The bridge (`ControlRuntime.applyRuntimeVisualState`) stamps the
derived `VisualState` onto the lowered tree in the `ControlId` domain; `Style.resolve` (E3) then drives
the paint.

Host wiring: `renderRetained` (`src/Controls.Elmish/ControlsElmish.fs`) assembles a read-only
`ControlRuntimeModel` from the live `pointerState` (`Hover`/`Presses`, already `ControlId`-keyed) and
`focused` (`RetainedId` resolved back to its `ControlId` via the prior retained tree) and applies the
bridge to `host.View size model` **before** `RetainedRender.init`/`step` (pre-reconcile).

Observed:
- a no-attribute button the model reports `Pressed` resolves Fill = Muted — visibly distinct from its
  Normal Accent fill (the runtime state actually drove the resolved paint).
- a no-attribute control the model reports `Hover`/`Selected` is stamped `Hover`/`Selected`.
- a non-interacted sibling resolves `Normal` and is returned structurally unchanged (no attribute
  added).

result=pass — a running control restyles on interaction with zero consumer code.
authoritative-test=Feature096RuntimeBridgeTests/Feature 096 runtime visual-state bridge (T011)
host-call-site=src/Controls.Elmish/ControlsElmish.fs renderRetained (assembleRuntimeModel + applyRuntimeVisualState pre-step)
