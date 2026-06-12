# Quickstart — feature 108

Consumer-facing walkthrough (focus ring, live theming, metrics) and the maintainer
evidence walkthrough.

## Consumer: make focus visible

```fsharp
open FS.Skia.UI.Controls

// In your `view`, reflect your own focus model into the tree. Drives the ring on the
// focused control — keyed OR unkeyed — and nothing else (byte-identical when None).
let view (model: Model) (size: Size) : Control<Msg> =
    page model
    |> Focus.markFocused model.FocusedControl   // <- the supported entry (FR-005)
```

Move focus with the framework order; `markFocused` paints exactly one ring:

```fsharp
// e.g. on Tab / arrow, advance focus via Focus.order/traverse, store in the model,
// and the next `view` stamps the ring. No hand-walking, no per-control withKey needed.
let order = Focus.order (page model)
let next  = Focus.traverse order model.FocusedControl FocusMove.Next
```

## Consumer: compose pages with `Control.map`

```fsharp
// Each page is a self-contained module: PageModel, PageMsg, update, view -> Control<PageMsg>.
let shellView model size : Control<Msg> =
    Layout.stack [
        SettingsPage.view model.Settings size |> Control.map Msg.Settings
        DataPage.view     model.Data     size |> Control.map Msg.Data
    ]
```

## Consumer: live theming (render-path vs reuse-key split)

```fsharp
open FS.Skia.UI.Controls
open FS.Skia.UI.Color

let palette = Theming.resolve Palettes.RampVariant.Dark model.Accent
let paintTheme = Theming.toTheme palette          // EXACT model-derived palette
// Pass `paintTheme` to the render path (Control.renderTree) so the capture is exact…
// …while `host.Theme` stays a STATIC value for the fragment-reuse key (FR-018):
//   the reuse cache is not invalidated spuriously, and never reuses a stale fragment
//   when only the palette changed.

let aa = Contrast.ratio paintTheme.Background paintTheme.Foreground >= 4.5  // WCAG AA
```

## Consumer: dependable modifier chords

```fsharp
// Provide MapKeyChord to handle Ctrl/Alt/Shift/Meta shortcuts; plain keys still go to MapKey.
{ host with
    MapKeyChord = fun key mods ->
        match key, mods with
        | ViewerKey.Letter 'S', m when m.Ctrl -> Some SaveMsg
        | _ -> None }
```

## Consumer: read per-frame metrics

```fsharp
{ host with OnFrameMetrics = fun m ->
              if m.ViewRebuilt then log $"rebuild; remeasured={m.RemeasuredNodeCount}" }
```

## Maintainer: deterministic perf evidence

```fsharp
open FS.Skia.UI.Controls.Elmish

let script =
    [ FrameInput.Idle                                   // zero re-measure, no rebuild
      FrameInput.Pointer (HoverEnter (id, 10., 10.))    // one hover…
      FrameInput.Pointer (HoverEnter (id, 11., 11.))    // …burst within one frame
      FrameInput.Pointer (HoverEnter (id, 12., 12.))    // -> PointerMovesProcessed <= 1
      FrameInput.Pointer (Click (id, Primary, 12., 12.)) ] // click NOT dropped
let frames = Perf.runScript host size script   // FrameMetrics list — byte-stable counts
```

Assert (US2/US3/US4): the idle frame has `RemeasuredNodeCount = 0` and
`ViewRebuilt = false`; the hover-burst frame has `PointerSamplesReceived = 3`,
`PointerMovesProcessed <= 1`; the click frame processes the click within one frame.

## Maintainer: evidence + gates

Run `./fake.sh build -t Route` against the real diff (escalates to
controls-public-surface). Serialized order: `Dev` → `GeneratedGuidanceCheck` →
`TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`.
Recapture aggregate + per-package surface baselines (`RefreshSurfaceBaselines` +
explicit `PerPackageSurface.captureCurrent`). Author the full window-visibility
readiness set + the new FR-020 checklist (`readiness/` under this feature dir;
`evidence-audit.md` carries its verdict token). See [research.md](./research.md) D10.
