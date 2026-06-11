# Quickstart: Runtime Visual-State Bridge (R1)

## What you get

On the built-in retained host (`runInteractiveApp`), interacting with a migrated
control now restyles it **with zero consumer code** — no `Attr.visualState`, nothing
read from `ControlRuntime`. Hover lightens, press darkens, focus shows an indicator,
selected/disabled render distinctly. Migrated kinds: `button`, `check-box`,
`slider`, `text-box`, `radio-group`, `switch`.

## Consumer view: nothing changes

```fsharp
// Your view stays a plain pure function — no focus/hover bookkeeping.
let view (model: Model) : Control<'msg> =
    Controls.stack [
        Controls.button "Save"  SaveClicked
        Controls.slider model.Volume VolumeChanged
        Controls.textBox model.Name NameChanged
    ]
```

Run it under `runInteractiveApp`; hover/press/focus restyle and focus indication are
automatic. A consumer who interacts with nothing observes no behavior change.

## Direct use of the public projection

`ControlRuntime.deriveVisualState` is public and reusable (e.g. for a custom host or
a test):

```fsharp
open FS.Skia.UI.Controls

let m =
    { fst (ControlRuntime.init ()) with
        FocusedControl  = Some "name-field"
        HoveredControl  = Some "save-btn" }

ControlRuntime.deriveVisualState m "save-btn"    // Hover
ControlRuntime.deriveVisualState m "name-field"  // Focused
ControlRuntime.deriveVisualState m "missing"     // Normal

// Closed precedence — Pressed out-ranks Focused/Hover:
let pressed = { m with PressedControls = Set.ofList [ "save-btn" ] }
ControlRuntime.deriveVisualState pressed "save-btn" // Pressed
```

## Precedence, in one line

```
Pressed > Selected > Focused > Hover > Normal     (runtime-derivable order)
```

…and a **consumer-set** non-`Normal` state (`Disabled`/`Validation`/`Loading`/
`Selected`, set from your `'model`) **always wins** over any derived interaction
state — derived states only fill a slot you left at `Normal`. A disabled control
stays disabled-looking even while hovered.

## Verifying locally

```bash
# 1. Route prints the exact gates for this change (public .fsi escalates):
./fake.sh build -t Route

# 2. Serialized escalated path (run FAKE targets sequentially):
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
# plus ContrastCheck (Route prints it for the migrated-styling change)
```

Surface baselines are recaptured for the new `deriveVisualState` projection
(`RefreshSurfaceBaselines` + `PerPackageSurface.captureCurrent`).

## Evidence artifacts (under `readiness/`)

`derive-precedence.md` (property: totality/determinism/order, ≥1000 combos),
`live-restyle.md` (US1), `focus-survives-reshuffle.md` (US2, real identity),
`byte-identity-at-rest.md` (FR-005), `partial-repaint.md` (SC-005),
`widened-kinds.md` (SC-006), `responds-proof.md` (input→restyle), `contrast.md`
(SC-007).
