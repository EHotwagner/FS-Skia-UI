# Quickstart: Accessible Color Contrast & Palettes

FSI-first per Principle I: sketch and exercise the public surface before the
`.fs` body exists, then run the gate.

## 1. Measure contrast for any two colors (US3, SC-002/SC-004)

```fsharp
#r "nuget: FS.Skia.UI.Scene, 0.1.86-preview.1"   // or #load the local Scene.fs
#r "nuget: FS.Skia.UI.Color, 0.1.86-preview.1"
open FS.Skia.UI.Scene
open FS.Skia.UI.Color

// Reference pairs (WCAG): black-on-white = 21:1, white-on-white = 1:1.
Contrast.ratio Colors.black Colors.white      // ~21.0  (within 0.01)
Contrast.ratio Colors.white Colors.white      // 1.0

// Ratio + verdict in one call for a body-text pairing:
Contrast.check Text Colors.white Colors.black
// { Ratio = 21.0; Role = Text; Verdict = Aaa }

// A UI graphic (focus ring) needs only 3:1:
Contrast.check GraphicOrUi (Colors.rgba 248uy 250uy 252uy 255uy) (Colors.rgba 37uy 99uy 235uy 255uy)
```

## 2. Pick legible values from a ready-made ramp (US2, SC-003)

```fsharp
// A matched light + dark ramp exists for each family.
let blueLight = Palettes.ramp "blue" Palettes.Light |> Option.get
let blueDark  = Palettes.ramp "blue" Palettes.Dark  |> Option.get

// A documented text step over a documented background step meets AA body text:
let textStep = blueLight.Steps |> List.find (fun s -> s.Role = Palettes.Text)
let bgStep   = blueLight.Steps |> List.find (fun s -> s.Role = Palettes.AppBackground)
Contrast.ratio textStep.Color bgStep.Color >= 4.5   // true
```

## 3. Run the gate on the shipped themes (US1, SC-001)

```bash
# Validates every documented foreground/background pairing in BOTH themes
# against the theme's declared contrastRequiredRatio (text) / 3:1 (graphic-UI).
./fake.sh build -t ContrastCheck
# PASS => readiness/color-contrast-evidence.md lists per-pairing measured vs required.
```

## 4. Demonstrate regression protection (SC-005)

```bash
# Edit the DTCG single source only — drop a validated pairing below threshold:
#   src/Controls/design-tokens.tokens.json  (e.g. set light.danger near light.background)
./fake.sh build -t RefreshSurfaceBaselines   # regenerate DesignTokens.fs
./fake.sh build -t ContrastCheck             # FAILS, naming pairing + measured + required
# Restore the accessible value (from a ramp) and re-run => PASS.
```

## 5. Fix a failing shipped color (US1, FR-010)

1. Edit only the failing `$value` in `src/Controls/design-tokens.tokens.json`,
   choosing a replacement from a ramp (step 2). Leave conforming tokens
   untouched (minimize churn).
2. `./fake.sh build -t RefreshSurfaceBaselines` — regenerates `DesignTokens.fs`.
3. `./fake.sh build -t DesignTokenDrift` — confirms currency (PASS).
4. `./fake.sh build -t ContrastCheck` — confirms conformance (PASS).

## 6. Escalated validation order (maintainer-verify)

This is a consumer-contract change; run the serialized six-target path
sequentially (FAKE state is not concurrency-safe):

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

First, confirm `Route` agrees: `./fake.sh build -t Route` should list
`ContrastCheck` among the gates for this change.
