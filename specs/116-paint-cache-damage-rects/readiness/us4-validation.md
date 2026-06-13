# US4 independent validation — controls using expensive offscreen effects are flagged

**Story**: A control whose paint requires offscreen composition surfaces an advisory diagnostic; a plain
control does not; output is byte-identical either way.

## Path

Apply the pure `RetainedRender.offscreenEffect` detector to hand-built scenes (drop-shadow, `PathClip`,
non-opaque paint over a multi-node group, plain, `RectClip`) and assert the per-effect verdict. Drive a
real offscreen-forcing control (a line-chart with a translucent area fill over its line + dots) and a
plain control through `RetainedRender.step` and assert the advisory `OffscreenComposition` diagnostic
fires / does not fire on the `Diagnostics` channel, with byte-identical output in both.

## Evidence

- `tests/Controls.Tests/Feature116OffscreenDiagTests.fs` — the detector returns `drop-shadow` /
  `path clip` / `opacity group` for the three offscreen-forcing scenes and `None` for a plain opaque
  scene; a `RectClip` (the cheap ubiquitous label clip, lowered to `canvas.ClipRect` with no layer) is
  NOT flagged; a line-chart with data surfaces an advisory `OffscreenComposition` (`Severity = Info`)
  diagnostic via `step.Diagnostics` and renders byte-identically to a fresh rebuild; a plain control
  surfaces none and renders identically.

Detection boundary (pinned in [picture-cache-authority.md](./picture-cache-authority.md) §offscreen): the
genuinely offscreen-forcing effects in this renderer are drop-shadow / image-filter, `PathClip`, and a
non-opaque paint over a multi-node group (which a layered backend composites via `SaveLayer`); `RectClip`
and baked-per-paint opacity are deliberately excluded.

Result: PASS — advisory only; fires when offscreen-forcing, silent otherwise, never alters output
(SC-005).
