# Quickstart: Lookless Slot Composition (E5)

E5 lets you re-skin a control's **shape** — put an icon before a button's label, give a panel
a custom header — by filling the control's **named slots** with your own `Control<'msg>`
sub-tree. A slot fill is a static value your own `view` computes; there is no data binding,
`DataContext`, or template engine. Filling no slot is byte-identical to today.

## Fill a button's leading slot

```fsharp
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed

type Msg = Save

let view model =
    Button.view
        { Button.defaults with
            Text     = "Save"
            Leading  = Some (TextBlock.view { TextBlock.defaults with Text = "💾" })  // any Widget<'msg>
            OnClick  = Some Save }
```

The button renders the leading content ahead of its label. The leading content is a real
control: if it carried an `onClick`, that binding dispatches through your single `update`; if
it were focusable, it would join the tab order. Leave `Leading`/`Trailing` as `None` (the
default) and the button is **byte-identical** to a plain button.

## Give a panel a custom header

```fsharp
let card model =
    Panel.view
        { Panel.defaults with
            Header   = Some (TextBlock.view { TextBlock.defaults with Text = "Account" })
            Children = [ /* your content widgets */ ] }
```

`Header`/`Footer` default to `None` (no region), so a panel with no header is unchanged. Fill
`Header` to add a header region above the content.

## What you can and cannot do

| You can | You cannot |
|---|---|
| Fill any **declared** slot (`Button.Leading`/`Trailing`, `Panel.Header`/`Footer`) with any `Widget<'msg>` | Invent a slot a kind does not declare — there is no field, so it is a **compile error** |
| Put an interactive, styled, focusable sub-tree in a slot — it composes with E1–E4 | Bind a slot to model data — slots take a *static* value your `view` already computed (no `DataContext`/binding) |
| Nest slot-bearing controls inside a slot — it lowers recursively | Use a free-form string slot name — there is no public string-keyed builder |
| Fill a slot with empty content to render an empty region by choice | Expect an *unfilled* slot to be empty — unfilled falls back to the kind's default chrome |

## Why filling no slot is safe

An unfilled slot renders the kind's **default content** — exactly today's chrome — and the
peripheral defaults (`Leading`/`Trailing`/`Header`/`Footer`) contribute zero geometry, so your
label/content does not shift. A maintainer can add slots to a kind without changing any
existing consumer's render.

## Verifying a change (maintainer)

Run `Route` first and run only the gates it prints:

```bash
./fake.sh build -t Route
```

Public `src/Controls/*.fsi` / `src/Controls/Widgets/*.fsi` edits escalate to the serialized
order:

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Recapture surface baselines after the `.fsi` change (do **not** hand-edit them):

```bash
./fake.sh build -t RefreshSurfaceBaselines
# per-package snapshots are NOT covered by RefreshSurfaceBaselines —
# regenerate them via PerPackageSurface.captureCurrent
```

Evidence lands under `specs/095-lookless-slot-composition/readiness/` — slot placement
(SC-001), unfilled byte-identity (SC-002), E1–E4 composition (SC-003), live retained identity
(SC-004), determinism property (SC-005), typed-closed / no-template-engine (SC-006).
