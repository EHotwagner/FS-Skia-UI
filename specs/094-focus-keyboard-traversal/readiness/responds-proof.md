# Responds-proof — input→visible-change for a key-driven focus change

The reused E1 `respondsProofOf` input→visible-change primitive applied to a key-driven focus change:
a `Responsive` verdict when a real focus change produces a visible difference in the rendered output,
an `Inert` verdict when the frames are identical — so "renders" cannot be passed off as "responds".

- evidence-kind=responds-proof
- renderer-mode=DeterministicRenderOnly
- status=pass
- driven-through=`Control.renderTree` (the production render path) + `ControlsElmish.respondsProofOf`

## Results

- BEFORE: focus on `btn`; AFTER: a `Tab` moved focus to `sld` — the focused control carries E3's
  `Focused` visual-state and the header names the focused control. `respondsProofOf before after` →
  `Responsive` (the key-driven focus change produced a visible change through the production
  `Control.renderTree` path).
- An identical-frame capture → `Inert`: an inert host (one that renders but does not reflect the
  focus change) yields identical frames and cannot be passed off as a responds-proof.

## Authoritative tests

- `Feature094FocusRoutingTests` / `094 US3 responds-proof for a key-driven focus change`
