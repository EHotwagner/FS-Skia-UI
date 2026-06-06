# Quickstart — Catalog Expansion (New Typed Controls)

## For a product author (consuming the new controls)

Author the new controls through the typed front door like any other `Widget<'msg>`:

```fsharp
open System
open FS.Skia.UI.Controls.Typed
open FS.Skia.UI.Scene   // Color, for ColorPicker

type Msg =
    | DarkModeToggled of bool
    | DueDateChosen of DateOnly
    | ReminderTimeChosen of TimeOnly
    | AccentChosen of ColorSwatch
    | ExportAs of string

let settingsPanel model =
    Stack.view
        { Stack.defaults with
            Orientation = Vertical
            Children =
              [ ToggleButton.view
                  { ToggleButton.defaults with
                      Text = "Dark mode"; IsOn = model.DarkMode
                      OnToggle = Some DarkModeToggled }
                DatePicker.view
                  { DatePicker.defaults with
                      Value = model.DueDate; IsOpen = model.CalendarOpen
                      OnChange = Some DueDateChosen }
                TimePicker.view
                  { TimePicker.defaults with
                      Value = model.Reminder; OnChange = Some ReminderTimeChosen }
                ColorPicker.view
                  { ColorPicker.defaults with
                      Swatches = model.Palette; Selected = model.Accent
                      OnSelected = Some AccentChosen }
                SplitButton.view
                  { SplitButton.defaults with
                      Text = "Export"; IsOpen = model.ExportMenuOpen
                      Items = [ { Key = "pdf"; Label = "PDF" }; { Key = "png"; Label = "PNG" } ]
                      OnClick = Some (ExportAs "pdf"); OnSelected = Some ExportAs } ] }
    |> Widget.toControl   // finish the view; the adapter consumes Control<'msg>
```

Values (toggle on/off, chosen date/time/color, popup open) live in **your** model and flow
in through `Props`; the control is a pure projection — no per-control MVU model to wire.

## For a framework maintainer (adding a new control — the recipe)

1. **Author the typed module** under `src/Controls/Widgets/Buttons.*` or `Pickers.*`:
   `Props` record + `defaults` + `view` that composes **existing** legacy builders, then
   `Widget.ofControl`. No new `ControlKind`, no new dependency, no new MVU model.
2. **Add the lowering-parity test** (red-first): `view props |> Widget.toControl` ≡ the
   explicit legacy composition (order-normalized, events canonicalized) — the keystone.
3. **Add the catalog fact** to `CatalogGen.catalogFacts`, place the
   `BEGIN/END GENERATED: typed-catalog/<id>` marker pairs in `catalog.yml`/`Catalog.fs`, then
   regenerate: `./fake.sh build -t RefreshSurfaceBaselines`. Bump `supportedCount` 47→52 and
   the `CatalogTests.fs` assertion; capture the per-id parity fixtures.
4. **Extend** the `typedPropsById` cross-check, interaction/rendering/accessibility tests
   (≥2 viewports), and the `samples/ControlsGallery` typed panel.

## Validate

```bash
# Inner loop
./fake.sh build -t Dev
dotnet test tests/Controls.Tests/Controls.Tests.fsproj

# Authoritative — run Route on the implementation diff, then only the gates it prints.
./fake.sh build -t Route            # expect escalation: controls-public-surface
./fake.sh build -t Route --enforce  # fails if a required evidence artifact is missing
```

Run FAKE-backed targets **sequentially** (shared `.fake` state). On the escalated path use
the serialized six-target order (`Dev → GeneratedGuidanceCheck → TemplateCheck →
GeneratedProductCheck → EvidenceGraph → EvidenceAudit`) plus the `Route`-printed controls
gates and `ControlsCatalogGenerationCheck`. See [[fs-skia-typed-controls]] for the per-control
recipe and [[fsharp-code-generation]] for the catalog single-source pattern.
