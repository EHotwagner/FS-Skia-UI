---
title: Migrating from V2 (`FS.Skia.UI` monolith) to V3 (split packages)
category: Guides
categoryindex: 10
---

# Migrating from V2 (`FS.Skia.UI` monolith) to V3 (split packages)

V3 retires the broad `FS.Skia.UI` package. The monolith's runtime was relocated into
focused, single-responsibility packages across programme Stages 1–4 (ADRs 0007–0011),
and Stage 5 (feature `053-v3-monolith-retirement`) deletes the monolith and stops
publishing it. A V2 app that referenced `FS.Skia.UI` migrates by replacing that one
package reference with the focused packages it actually uses.

## Surface map

| V2 monolith namespace / surface (`FS.Skia.UI.*`) | V3 package | V3 namespace |
|---|---|---|
| Scene vocabulary — `Scene`, `SceneNode`, `Paint`, `Path`, `Colors`, `Point`/`Rect`/`Size`, `BlendMode`, shaders/filters, diagnostics | `FS.Skia.UI.Scene` | `FS.Skia.UI.Scene` |
| Vulkan/Skia desktop host + persistent viewer (`Viewer.runApp`, swapchain presenter, window hosting) | `FS.Skia.UI.SkiaViewer` | `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.SkiaViewer.Host` |
| Elmish program/command/subscription wiring over the viewer | `FS.Skia.UI.Elmish` | `FS.Skia.UI.Elmish` |
| Keyboard input contracts (key model/mapping) | `FS.Skia.UI.KeyboardInput` | `FS.Skia.UI.KeyboardInput` |
| Rich keyboard-input runtime (reducer/effect, diagnostics, YAML config) | `FS.Skia.UI.Input` | `FS.Skia.UI.Input` |
| Layout / graph definitions and validation | `FS.Skia.UI.Layout` | `FS.Skia.UI.Layout` |
| Form controls, rich text, DataGrid, chart controls, graph views, `ControlRuntime` | `FS.Skia.UI.Controls` | `FS.Skia.UI.Controls` |

`FS.Skia.UI.Controls.Elmish` (the Controls↔Elmish program adapter) and
`FS.Skia.UI.Testing` (deterministic scene-output testing helpers) have **no V2 monolith
public-surface predecessor** — they are V3-native packages and are intentionally absent
from the surface map above.

## Moving an app's package references

Replace the single broad reference:

```xml
<!-- V2 -->
<PackageReference Include="FS.Skia.UI" />
```

with the focused packages the app uses, for a typical Elmish desktop viewer app:

```xml
<!-- V3 -->
<PackageReference Include="FS.Skia.UI.Scene" />
<PackageReference Include="FS.Skia.UI.SkiaViewer" />
<PackageReference Include="FS.Skia.UI.Elmish" />
<!-- add as needed: FS.Skia.UI.Controls, FS.Skia.UI.Controls.Elmish,
     FS.Skia.UI.KeyboardInput, FS.Skia.UI.Input, FS.Skia.UI.Layout, FS.Skia.UI.Testing -->
```

A generated `dotnet new fs-skia-ui` app already references the split packages only — the
template never pinned the monolith — so newly generated products need no migration.

## Removed `SceneConversion`

V2 bridged the monolith's opaque `Scene` to the split `Scene = { Nodes: SceneNode list }`
via `src/SkiaViewer/SceneConversion.fs`. V3 has a **single** scene vocabulary
(`FS.Skia.UI.Scene`, ADR 0008), so the `SceneConversion` shim is gone. Apps that built
scenes through the monolith's scene API now build them directly against
`FS.Skia.UI.Scene` — the same structured `Scene`/`SceneNode`/`Paint`/`Path` vocabulary,
with no conversion step. The viewer host consumes `FS.Skia.UI.Scene` values directly.

## Rich keyboard input → `FS.Skia.UI.Input`

The rich keyboard-input runtime (the stateful reducer/effect model, diagnostics, and YAML
key-binding configuration) moved out of the monolith into `FS.Skia.UI.Input` (feature
052). Apps that drove rich keyboard handling through `FS.Skia.UI` now reference
`FS.Skia.UI.Input` and open `FS.Skia.UI.Input`; the lighter key model/mapping contracts
remain in `FS.Skia.UI.KeyboardInput`.

## See also

- After-measurement baseline: `docs/reports/_baselines/2026-06-02-v3-after.md`
- Programme closeout: `docs/adr/0012-monolith-retirement-closeout.md`
- Stage ADRs: `docs/adr/0007-host-ownership.md` … `0011-parity-oracle-method.md`
