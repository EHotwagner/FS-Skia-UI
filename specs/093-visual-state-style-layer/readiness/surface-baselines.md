# Surface-area baselines (Tier 1) — feature 093

The public surface moved, so the controls-public-surface / per-package /
cross-package baselines were recaptured via
`./fake.sh build -t RefreshSurfaceBaselines` (regenerates the aggregate +
per-package snapshots, the bundled `template/base/docs/api-surface/Controls/*`
mirrors, and `DesignTokens.fs` from the DTCG source).

## Recaptured baselines + gate verdicts

- `readiness/surface-baselines/FS.Skia.UI.Controls.txt` (aggregate) — **updated**.
- `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` (per-package) — **updated**.
- `PackageSurfaceCheck` → `Status: Ok`.
- `PerPackageSurfaceDiff` → `Status: Ok`.

## Additive-only deltas (no removals)

Aggregate baseline additions:

```
FS.Skia.UI.Controls.StyleVariant
FS.Skia.UI.Controls.StyleClass            (+ Variant / + Custom)
FS.Skia.UI.Controls.ResolvedStyle
FS.Skia.UI.Controls.Style                 (module — resolve)
FS.Skia.UI.Controls.AttrValue`1+StyleClassesValue
FS.Skia.UI.Controls.AttrValue`1+VisualStateValue
```

Per-package `.fsi` additions:

```
Attributes:  val styleClasses : StyleClass list -> Attr<'msg>
             val visualState  : VisualState -> Attr<'msg>
Style:       type ResolvedStyle ; module Style.resolve
DesignTokens: val success : Color ; val warning : Color   (Light & Dark)
Control (internal): val styleClassesOf ; val visualStateOf ; val faithfulContent
Primitives:  ButtonProps.Classes / CheckBoxProps.Classes
```

All deltas are **additive** — no public name was removed or changed. The
`view : 'model -> Control<'msg>` contract is unchanged; a consumer attaching no
class sees byte-identical lowering.
