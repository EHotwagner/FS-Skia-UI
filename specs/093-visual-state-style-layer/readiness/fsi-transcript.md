# FSI transcript — exercising the public style surface (Principle I)

The public resolver + attach-class front door are exercised from FSI against the
**built** `FS.Skia.UI.Controls` assembly (the packed-equivalent surface), proving
the consumer-reachable path works end-to-end — not just internal helpers.

## Script

```fsharp
#r "FS.Skia.UI.Scene.dll" ; #r "FS.Skia.UI.Layout.dll"
#r "FS.Skia.UI.KeyboardInput.dll" ; #r "FS.Skia.UI.Controls.dll"
open FS.Skia.UI.Controls
let theme = Theme.light
let baseStyle : ResolvedStyle =
    { Foreground = theme.Foreground; Fill = theme.Background; Stroke = theme.Foreground
      StrokeWidth = 1.0; FontFamily = theme.FontFamily; FontSize = 14.0; FontWeight = None }
// variants resolve to token-derived fills:
Style.resolve theme baseStyle [ Variant StyleVariant.Primary ] Normal       // Fill = accent
Style.resolve theme baseStyle [ Variant StyleVariant.Danger ]  Normal       // Fill = danger
Style.resolve theme baseStyle [ Variant StyleVariant.Success ] Normal       // Fill = success token
// Custom flows through the same fold; unknown is identity:
(Style.resolve theme baseStyle [ Custom "primary" ] Normal) = (Style.resolve theme baseStyle [ Variant StyleVariant.Primary ] Normal)
(Style.resolve theme baseStyle [ Custom "zzz" ] Normal) = baseStyle
// precedence: state over class, later class over earlier:
Style.resolve theme baseStyle [ Variant StyleVariant.Danger ] Disabled                                  // Fill = muted (state wins)
Style.resolve theme baseStyle [ Variant StyleVariant.Primary; Variant StyleVariant.Danger ] Normal      // Fill = danger (later wins)
(Style.resolve theme baseStyle [] Normal) = baseStyle                                                   // base fidelity
// typed front door attach-class affordance lowers to a styleClasses attribute:
let btn : Widget<int> = Typed.Button.view { Typed.Button.defaults with Text = "Pay"; Classes = [ Variant StyleVariant.Danger ] }
```

## Captured output

```text
Primary.Fill        = { Red = 37uy ... }     (= DesignTokens.Light.accent)
Danger.Fill         = { Red = 185uy ... }    (= DesignTokens.Light.danger)
Success.Fill        = { Red = 21uy ... }     (= DesignTokens.Light.success)
Custom primary==Variant Primary = true
Unknown custom == base          = true
Disabled over Danger Fill = { Red = 100uy ... }   (= theme.Muted — state wins)
Later class wins Fill     = { Red = 185uy ... }   (= theme.Danger — later class wins)
resolve base [] Normal == base  = true
Typed Button Classes lowered    = Some [Variant Danger]
Default (Classes=[]) emits no styleClasses attr = true
```

## Result

PASS — `Style.resolve`, the `Variant`/`Custom` `StyleClass` surface, the fixed
precedence, base fidelity, and the typed `Button.Classes` lowering all work
through the public surface; `Classes = []` lowers to no style attribute (additive).
