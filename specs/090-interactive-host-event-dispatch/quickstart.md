# Quickstart: Verify Live Interactive Responsiveness (090)

The author→host→click→see-it-change loop the feature makes work, and the proof an inert build fails.

## 1. Author controls the documented, obvious way (no `MapPointer` clauses)

```fsharp
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish

// A leaf-keyed button and a container-keyed composite, both with authored bindings.
let view _ model =
    Stack.vertical [
        Button.create [ Attr.key "inc"; Button.text "+1"; Button.onClick Increment ]      // leaf-keyed
        ColorPicker.create [ Attr.key "picker"; ColorPicker.onChanged PickColor ]          // container-keyed
        TextBox.create   [ Attr.key "name"; TextBox.onChanged SetName ]                    // focus-aware text
    ]

let host : InteractiveAppHost<Model, Msg> =
    { Init = init; Update = update; View = view; Theme = Theme.light
      MapKey = (fun _ _ -> None)                 // no hand-routing
      MapPointer = (fun _ -> None)               // no hand-routing — bindings carry it now
      Tick = (fun _ -> None); Diagnostics = ViewerDiagnosticsOptions.silent }
```

## 2. Drive the host through the real adapter path (headless — no window needed)

```fsharp
// Synthesize a press+release over the button's bounds; routeInteractivePointer is exactly what
// runInteractiveApp wires (research D6).
let state0 = Pointer.init ()
let state1, msgs = ControlsElmish.routeInteractivePointer host state0 size model0 pressInput
let state2, msgs' = ControlsElmish.routeInteractivePointer host state1 size model0 releaseInput
// msgs' contains `Increment` — the AUTHORED binding fired, with zero MapPointer clauses.   (US1)
```

- **Container-keyed:** a click anywhere inside `ColorPicker` resolves via `nearestAuthored` to the
  `"picker"` id and dispatches `PickColor` — not an opaque inner `"0.1"` (US2).
- **Text:** click the `TextBox` to focus it, deliver a keystroke through the focus-aware seam, and the
  character reaches the focused control's `TextInput` model (US4).

## 3. Capture the responds-proof (distinguishes renders from responds)

```fsharp
let proof = ControlsElmish.captureRespondsProof host (Pointer.init ()) size model0 clickInput
// proof.Verdict = Responsive   (before ≠ after — the click changed the rendered output)   (US3)
// An inert build (binding dropped) ⇒ proof.Verdict = Inert ⇒ FAILS — the dead-window gate.
```

## 4. Validate (escalated / maintainer-verify — serialized six-target order)

FAKE-backed commands share `.fake` state — run **sequentially**:

```
./fake.sh build -t Route            # expect: escalates (public .fsi in Controls + Controls.Elmish)
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck    # env-failure here is non-authoritative ([[generated-product-check-env-failure]])
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Then recapture surfaces (per-package `.fsi.txt` is NOT covered by `RefreshSurfaceBaselines` —
[[per-package-baseline-not-in-refresh-target]]) and regenerate any touched `.claude` skill:

```
# PerPackageSurface.captureCurrent for FS.Skia.UI.Controls + FS.Skia.UI.Controls.Elmish
./fake.sh build -t RefreshSurfaceBaselines   # api-surface tree + .claude evidence-mode skill mirror
```

## Success signals

- `routeInteractivePointer` returns the **authored** binding's message with no `MapPointer` clause (SC-001).
- The `ControlsElmish.fsi` host-contract doc matches the implementation — no false dispatch claim (SC-002).
- Container-keyed click routes via `nearestAuthored`; leaf still resolves to itself (SC-003).
- The responds-proof is `Responsive` for a live app and `Inert` (fails) for a dead one (SC-004).
- A keystroke reaches the focused text control, documented in the contract (SC-005).
- Serialized six-target order green; baselines current; `.claude`↔`.agents` byte-identical (SC-006).
</content>
