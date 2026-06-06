# Contract: Typed-authoring gallery panel, coverage & evidence (US2)

**Surface kind**: repo sample value + test coverage + readiness evidence (no
shipped public `.fsi`). Tier 2 / additive.

## Provider

`samples/ControlsGallery/Program.fs` — `typedAuthoringPanel : Control<Msg>`,
extended from {TextBlock, Button, CheckBox} to ≥1 control per mechanic group,
authored only through `FS.Skia.UI.Controls.Typed.*` `view` functions.

## Panel authoring contract

| # | Given | When | Then |
| --- | --- | --- | --- |
| G1 | the extended panel | inspected | covers ≥1 control from each mechanic group: display, input, stateful input, layout container, navigation/composite, overlay, selection collection, charts/graph — resolved against the catalog `Category` crosswalk below (FR-007/SC-004, AS1) |
| G2 | the panel source | scanned | contains **no** `Attr` and no `*.create` call — every control is a `FS.Skia.UI.Controls.Typed.*` `view` (SC-004) |
| G3 | the persistent `ControlsGallery` sample | launched | the typed panel appears alongside the existing panels as a render/interaction smoke (FR-007, AS2) |
| G4 | stateful controls in the panel | composed | reuse the already-shipped `070` MVU models; no new `Model`/`Msg`/`Effect` (plan MVU boundary) |

## Mechanic-group → catalog `Category` crosswalk (SC-004)

The 8 gallery mechanic groups in G1 resolve onto the catalog `Category` taxonomy
(11 values) as below, so "≥1 per group" is mechanically auditable: a group is
satisfied by ≥1 typed-authored control whose catalog `Category` is in its row.
Categories `data`, `feedback`, and `custom` are **not** required gallery groups —
the panel is representative, not exhaustive (edge case "gallery panel breadth vs
launch cost"); a control from them MAY appear but does not by itself satisfy a
required group.

| Gallery mechanic group | Catalog `Category` | Example satisfying id(s) |
| --- | --- | --- |
| display | `display` | `text-block`, `label`, `badge` |
| input | `input` (stateless action) | `button`, `icon-button` |
| stateful input | `input` (edit-state) | `text-box`, `numeric-input`, `slider` |
| layout container | `layout` | `stack`, `grid`, `border` |
| navigation/composite | `navigation` | `tabs`, `toolbar` |
| overlay | `overlay` | `dialog`, `tooltip` |
| selection collection | `selection` (optionally `data`) | `combo-box`, `list-box` (or `list-view`) |
| charts/graph | `chart` + `graph` | `line-chart` + `graph-view` |

Not a required group (optional, representative only): `data` (`list-view`,
`tree-view`, `data-grid`), `feedback` (`toast`, `progress-bar`, `spinner`,
`validation-message`), `custom` (`custom-control`).

## Coverage contract (`RenderingTests.fs` / `AccessibilityTests.fs`)

| # | Given | When | Then |
| --- | --- | --- | --- |
| G5 | the typed gallery panel | rendered at ≥2 viewports through the existing render path | rendering suite passes, no diagnostics (FR-008/SC-005, AS1) |
| G6 | the typed gallery panel | accessibility suite at ≥2 viewports | passes; expected accessibility roles present (FR-008/SC-005) |
| G7 | the suites before the panel/coverage exist | run | fail (failing-first), then pass after the panel + coverage land (Principle VI) |

## Evidence contract

| # | Given | When | Then |
| --- | --- | --- | --- |
| G8 | the deterministic render path | typed-gallery viewport evidence captured to `readiness/controls-rendering.md` | evidence is real, render-only, carries **no** `[S]`/`[S*]` disclosure (FR-009/SC-006, AS3) |
| G9 | the same panel + model state | re-captured | byte-identical output (determinism; lowering is pure — no wall-clock/random/I/O) (FR-009/SC-006) |
| G10 | the captured evidence | classified | a render/interaction smoke, **not** a substitute for the persistent gallery launch (edge "render evidence is render-only") |
