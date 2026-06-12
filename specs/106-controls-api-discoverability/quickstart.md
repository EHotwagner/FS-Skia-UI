# Quickstart: Author a control without reflection

The consumer-facing outcome of this feature. After it lands, a developer (or their
agent) in a `dotnet new fs-skia-ui` project authors controls entirely from
demonstrated code + IntelliSense + shipped docs — no DLL reflection, no framework
source reads.

## 1. Start from the demonstrated typed front door

The generated `src/Product/View.fs` shows the typed `Props`/`view` pattern:

```fsharp
open FS.Skia.UI.Controls.Typed

let view model =
    Stack.view
        { Stack.defaults with
            Children =
                [ TextBlock.view { TextBlock.defaults with Text = "Product controls" }
                  TextBox.view   { TextBox.defaults with Value = model.Name
                                                         OnChanged = Some NameChanged }
                  Button.view    { Button.defaults with Text = "Save"
                                                        Enabled = model.CanSave
                                                        OnClick = Some SaveRequested } ] }
```

To add a new control, type `<ControlName>.defaults with ` and let the compiler/
IntelliSense enumerate the record fields — each field is the typed, checked authoring
surface. `OnClick = None` binds nothing; `Some msg` binds the handler.

## 2. Hover for substantive guidance

Hovering any member — a typed `Props` field, `Button.view`, a legacy `Attr.width`, or a
`Catalog` function — shows a real description (what it does, value meaning, accepting
control kinds), shipped in `FS.Skia.UI.Controls.xml`. No member shows the old
"Public contract function exposed…" placeholder.

## 3. Discover a control's contract two ways

**Programmatically** (from IntelliSense, documented):

```fsharp
open FS.Skia.UI.Controls
Catalog.requiredAttributes  StandardControlKind.Button   // attributes the control must have
Catalog.supportedAttributes StandardControlKind.Button   // everything it accepts
Catalog.supportedEvents     StandardControlKind.Button   // bindable events
Catalog.markdownSummary ()                               // a full catalog as markdown
```

**Statically** — open the bundled catalog reference under `docs/` (linked from the
project README), or the source-shaped signatures under `docs/api-surface/Controls/`.

## 4. The README points you here, not at reflection

The generated `README.md` resolves every "do not reflect" line to a concrete target:
the typed starter, the `docs/api-surface/Controls/*.fsi` bundle, the `Catalog.*` API,
and the catalog reference — so reflection is never the most reliable path.

## Acceptance walkthrough (maps to Success Criteria)

1. From a fresh generated project, add a control kind not shown in the starter using
   only `defaults with` + IntelliSense → it compiles and renders. **(SC-001)**
2. Hover 10 random Controls members → 0 show the placeholder sentence. **(SC-002)**
3. `grep "create \[" src/Product/View.fs` over the demonstrated view → 0 legacy attr
   lists. **(SC-003)**
4. From the README, reach the `Catalog` API and the catalog reference; read Button's
   full supported-attribute set without reflection. **(SC-004/SC-005)**
5. Confirm `docs/api-surface/Controls/` contains the typed `Widgets` signatures.
   **(SC-006)**
