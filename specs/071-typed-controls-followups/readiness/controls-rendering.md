# Typed gallery panel render evidence (071) — T017

**Proof level**: `DeterministicRenderOnly`. This is a render/interaction smoke through
the existing `Widget.toControl` → `Control.render` IR path — **not** a readable-layout
proof and **not** a substitute for the persistent `ControlsGallery` launch (T015,
`readiness/sample-smoke/controls-gallery-launch.txt`). **No `[S]`/`[S*]` disclosure**: the
output is real render readback over the migrated typed surface (FR-009 / SC-006).

## Captured viewports (deterministic readback hash)

Panel: `ControlsTypedGalleryPanel.panel` — the exact value the US2 rendering and
accessibility suites render, authored only through `FS.Skia.UI.Controls.Typed.*` `view`
functions and lowered with `Widget.toControl`. Captured via
`scripts/capture-typed-gallery-evidence.fsx`:

| Viewport | Deterministic hash | Diagnostics | Node count |
| --- | --- | --- | --- |
| 320×240 | `73d2b19423dbfef1a00f27b9835a6c1d28dd859be5861025082e2ca6169cd937` | 0 | 10 |
| 1024×768 | `b1d2401e079536d97f62bd6d6d4fd2cb18a34481a44ee77f841c336c0e9c6a63` | 0 | 10 |

## Determinism (SC-006 / G9)

The lowering is pure (no wall-clock / random / I/O), so re-rendering the identical panel
and model state yields byte-identical readback. Two consecutive captures were
**byte-identical** (`diff` of the two runs is empty). Re-run with
`dotnet fsi scripts/capture-typed-gallery-evidence.fsx`.

## Per-group coverage (SC-004, Category crosswalk)

Covered catalog categories (from the rendered tree):
`chart, display, graph, input, layout, navigation, overlay, selection` — every required
mechanic group. The satisfying typed-authored control per group:

| Gallery mechanic group | Catalog `Category` | Satisfying control id |
| --- | --- | --- |
| display | `display` | `text-block` |
| input | `input` (action) | `button` |
| stateful input | `input` (edit-state) | `text-area` (stateful, reuses `TextInput` model) |
| layout container | `layout` | `stack` |
| navigation/composite | `navigation` | `tabs` |
| overlay | `overlay` | `tooltip` |
| selection collection | `selection` | `check-box`, `list-box` (stateful, reuses `Collections` model) |
| charts/graph | `chart` + `graph` | `line-chart` + `graph-view` |

Control kinds present: `button, check-box, graph-view, line-chart, list-box, stack, tabs,
text-area, text-block, tooltip`. `data`/`feedback`/`custom` are not required groups.

## Enforcement

`tests/Controls.Tests/RenderingTests.fs` ("Feature 071 typed gallery rendering (US2)") and
`AccessibilityTests.fs` ("Feature 071 typed gallery accessibility (US2)") render this panel
at the two viewports above, assert no diagnostics + a non-empty deterministic hash, assert
the covered category set ⊇ the 8 required groups, and assert the expected accessibility
roles (`Button`, `TextBox`, `CheckBox`, `List`, `Tab`, `Chart`, `Graph`). Both went RED on
the 3-control stub before T014 (see `readiness/logs/us2-red-green.md`).
