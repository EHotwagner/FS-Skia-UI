# FSI transcript — the unified `Key ?? path` scheme over the packed library (T019 / Principle I)

evidence-kind=fsi-transcript
status=pass
authoritative=true
command=LD_LIBRARY_PATH=tests/Controls.Tests/bin/Debug/net10.0 dotnet fsi /tmp/fsi098.fsx
library=src/Controls (FS.Skia.UI.Controls.dll, built) — the real public surface a consumer reaches.
failure-class=product-defect

## Script

```fsharp
#r ".../FS.Skia.UI.Scene.dll"
#r ".../FS.Skia.UI.Layout.dll"
#r ".../FS.Skia.UI.Controls.dll"
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
type Msg = ClickedA | ClickedB
let theme = Theme.light
let size: Size = { Width = 320; Height = 200 }
// an UNKEYED Button.onClick plus a same-kind sibling, authored the documented way (no withKey):
let view =
    Stack.create
        [ Stack.orientation "horizontal"
          Stack.children
              [ Button.create [ Button.text "A"; Button.onClick ClickedA ]
                Button.create [ Button.text "B"; Button.onClick ClickedB ] ] ]
let rendered = Control.renderTree theme size view
let preview  = Control.render theme view
```

## Output (captured)

```
RESULT boundIds = ["0.0"; "0.1"]
RESULT eventBinding-ids = ["0.0"; "0.1"]
RESULT boundIds-match-eventBindings = true
RESULT siblings-distinct = true
RESULT nearestAuthored 0.0 = Some "0.0"
RESULT nearestAuthored 0.1 = Some "0.1"
RESULT preview.Bounds-empty = true
RESULT preview.BoundIds-populated = true
RESULT preview.BoundIds = ["0.0"; "0.1"]
```

## Interpretation

- `boundIds == eventBinding-ids` — `renderTree` emits `BoundIds` in the **same** `Key ?? path` scheme as
  `EventBindings` (SC-003 / FR-002), so a recovered id is a direct lookup key.
- `siblings-distinct = true` — the two unkeyed same-kind Buttons mint **distinct** path ids `"0.0"` /
  `"0.1"`, not a shared `Kind` id (SC-004 / FR-007).
- `nearestAuthored 0.0 = Some "0.0"` (and `0.1`) — recovery now treats an unkeyed-**bound** node as
  authored and returns its own path id (FR-003) — the previously-dead button is alive.
- `preview.Bounds-empty = true` **and** `preview.BoundIds-populated = true` — `render` keeps `Bounds = []`
  but **does** populate `BoundIds` from its bound nodes (FR-002, D3/D6).
