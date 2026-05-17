# Rich Rendering Evidence

Status: setup placeholder, awaiting US1 implementation.

## Required Evidence

- Skia-specific rich text, measurement, custom drawing, clipping, effects, hit
  testing, diagnostics, and accessibility metadata through Controls.
- Deterministic render/readback or screenshot evidence where supported.
- Unsupported environment diagnostics reported separately from implementation
  failures.

## Current Assets

- Custom control surface: `src/Controls/CustomControl.fsi`, `src/Controls/CustomControl.fs`
- Rendering tests: `tests/Controls.Tests/RenderingTests.fs`

## US1 Evidence

- `readiness/logs/t028-us1-controls-records.txt`: custom controls expose
  measurement, drawing, clipping, effect, and diagnostic hooks.
- `readiness/logs/t029-rich-rendering.txt`: rich text measurement respects
  max width, reports unsupported Skia effects, and produces deterministic
  render/readback evidence.
- `readiness/logs/t034-controlsgallery-contract-smoke.txt`: ControlsGallery
  public sample renders `RichText` and a custom Skia escape hatch through the
  Controls package surface.

## US1 Readiness Capture

| Evidence | Path | Verdict |
|----------|------|---------|
| Rich text measurement/readback test | `readiness/logs/t029-rich-rendering.txt` | PASS |
| Controls FSI render output | `readiness/logs/t033-controls-fsi.txt` | PASS |
| ControlsGallery rich text/custom control sample | `readiness/logs/t034-controlsgallery-contract-smoke.txt` | PASS |

The US1 sample exercises Skia-specific rich text and a custom Skia escape hatch
through Controls records; unsupported rich-text effects continue to be reported
by `RichText.measure`.

## T076 Rendering Gate

| Evidence | Path | Verdict | Duration |
|----------|------|---------|----------|
| `./fake.sh build -t ControlsRenderingCheck` | `readiness/logs/t076-controls-rendering-check.txt` | PASS | 3s |
| Rendering test detail | `readiness/logs/controls-rendering-check.txt` | PASS | recorded by target |
| Direct serial rendering slice | `readiness/logs/t076-controls-rendering-direct.txt` | PASS | 2s |

The rendering gate exercises deterministic scene readback for three viewport
sizes and two density scale factors, graph/chart controls, rich text
diagnostics, and 10,000-item visible-range behavior. An earlier
FAKE-plus-broad-build attempt hit local VSTest out-of-memory startup pressure;
the final target graph runs the rendering slice directly with
`dotnet test -m:1 --no-restore --filter Rendering` and passes.
