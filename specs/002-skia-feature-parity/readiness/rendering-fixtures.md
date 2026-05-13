# Deterministic Rendering Fixtures

Feature: `002-skia-feature-parity`

## Screenshot Tolerance

The deterministic rendering tests use the following metadata until per-gallery baselines are captured:

| Fixture | Width | Height | Max channel delta | Max differing pixels |
|---------|-------|--------|-------------------|----------------------|
| `core-primitives` | 640 | 480 | 2 | 64 |
| `charts-large-data` | 800 | 480 | 2 | 128 |
| `layout-graph` | 800 | 600 | 2 | 128 |

## Large Data Generators

Generators are deterministic and parameterized by count:

- `sin-wave`: `sin(index / 100.0) * 100.0`
- `linear`: `float index`
- `ohlc`: open/high/low/close values derived from `sin-wave`
- `grid-row`: stable row maps with `Name`, `Score`, and `Active` columns

## Sample Assets

`readiness/sample-assets/checkerboard.ppm` is a plain-text image fixture suitable for file-read tests without binary churn. Real image decode/render coverage is added in US1.
