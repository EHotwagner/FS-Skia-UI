# FSI transcript — public `Layout.evaluateIncremental` (T022 / Principle I)

evidence-kind=fsi-transcript
status=pass
authoritative=true
command=LD_LIBRARY_PATH=tests/Layout.Tests/bin/Debug/net10.0 dotnet fsi /tmp/fsi097.fsx
library=src/Layout (FS.Skia.UI.Layout.dll, built) — the real public surface a consumer reaches.
failure-class=product-defect

## Script

```fsharp
#r ".../Yoga.Net.dll"
#r ".../FS.Skia.UI.Scene.dll"
#r ".../FS.Skia.UI.Layout.dll"
open FS.Skia.UI.Layout
let avail = { Width = 400.0; WidthMode = Exactly; Height = 300.0; HeightMode = Exactly }
let leaf id w h : LayoutNode = { Defaults.layoutNode id with Intent = { Defaults.layoutIntent with Size = { Width = Some w; Height = Some h } } }
let fixedBox id w h kids : LayoutNode = { Defaults.layoutNode id with Intent = { Defaults.layoutIntent with Size = { Width = Some w; Height = Some h } }; Children = kids }
let auto id kids : LayoutNode = { Defaults.layoutNode id with Children = kids }
let frame1 = auto "0" [ fixedBox "0.0" 200.0 120.0 [ leaf "0.0.0" 50.0 20.0; leaf "0.0.1" 50.0 20.0 ] ; leaf "0.1" 100.0 30.0 ]
let frame2 = auto "0" [ fixedBox "0.0" 200.0 120.0 [ leaf "0.0.0" 70.0 35.0; leaf "0.0.1" 50.0 20.0 ] ; leaf "0.1" 100.0 30.0 ]
let prev = Layout.evaluate avail frame1
let inc  = Layout.evaluateIncremental prev [ "0.0.0" ] avail frame2
let full = Layout.evaluate avail frame2
```

## Output (captured)

```
RESULT byte-identical-bounds = true
RESULT invalidated = ["0.0"; "0.0.0"; "0.0.1"]
RESULT root-not-remeasured = true
RESULT revision-advanced = true (1 -> 2)
```

## Interpretation

- `byte-identical-bounds = true` — incremental `Bounds` exactly equal a full `evaluate` (INV-1 / SC-002).
- `invalidated = ["0.0"; "0.0.0"; "0.0.1"]` — the honest post-propagation re-measured set: the requested
  `["0.0.0"]` widened to its fixed-size boundary `"0.0"` and that boundary's whole subtree, NOT the
  verbatim input and NOT the whole tree (FR-001a / SC-008).
- `root-not-remeasured = true` — the root and the `"0.1"` sibling reused their cached bounds (partial
  re-measure; SC-001).
- `revision-advanced = true (1 -> 2)` — `Revision = previous.Revision + 1`.
