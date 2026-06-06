---
name: fs-skia-typed-controls
description: Migrate an FS.Skia.UI catalog control to the typed Props/MVU front door under FS.Skia.UI.Controls.Typed — an immutable Props record, defaults, and a view returning Widget that lowers structurally equal to the legacy builder, proven by a per-control parity test.
compatibility: F# net10.0; FS.Skia.UI.Controls package, additive-only public .fsi surface.
metadata:
  author: fs-skia-ui
  source: specs/070-typed-controls-migration/plan.md
---

# fs-skia-typed-controls

Cookbook for moving a catalog control from the legacy string-keyed `*.create`/`Attr`
authoring API to the **typed front door** started in `065`. The migration is uniform:
"run this skill once per control id." Every control gains an additive,
compiler-checked authoring surface in the `FS.Skia.UI.Controls.Typed` namespace
without changing or removing the legacy peer (it stays byte-frozen).

## Scope / when to use

Use this skill when adding (or reviewing) a typed `FS.Skia.UI.Controls.Typed.<Control>`
module for a catalog control: a pure display/input/container control, a stateful
control that reuses an existing MVU model (`TextInput`, `Collections`, `DataGrid`,
chart/graph), or the `custom-control` escape hatch. Do **not** use it to invent new
catalog controls, new dependencies, or new MVU models — the typed façade only
re-expresses an existing legacy builder.

## Driven-library API and the per-id recipe

The typed modules live under `src/Controls/Widgets/*.fsi`/`*.fs` and re-export through
`FS.Skia.UI.Controls.Typed`. Each control id gets its own module named by the
PascalCase of its catalog id. Four steps:

1. **Pick taxonomy fields.** Each catalog `requiredAttribute` (PascalCased) becomes a
   **non-optional** `Props` field; every other value is optional and resolves through
   `defaults`; every catalog event is an **optional callback** field that lowers to
   **no binding** when `None`. No field is `obj`, untyped, or a string-named event.
2. **Write `Props` + `defaults` + `view`.** `view : Props<'msg> -> Widget<'msg>` builds
   the control by calling the **exact same legacy `*.create`/`Attr` builders** (or
   `Control.standard (Custom <id>)` where no dedicated `*.create` exists), then
   `Widget.ofControl`. Because it calls the legacy builder, the lowered IR is equal to
   the legacy authoring call by construction.
3. **Add the mandatory lowering-parity test.** Assert `view props |> Widget.toControl`
   is structurally equal to the hand-written legacy builder output (order-normalized,
   events canonicalized to the message they produce). This is the keystone proof.
4. **For stateful controls, reuse the existing MVU model.** `init`/`update` delegate to
   the existing pure model (`TextInput`/`Collections`/`DataGrid`/chart) and return the
   existing `Model`/`Msg`/`Effect` types — never a fork. A delegation test asserts the
   typed `update` result equals the reused model's `update` for the same input.

Keep the surface additive: only the new typed modules/records appear on the public
`.fsi`; visibility lives in the signature file (Principle II). `custom-control` has
**no `Props` schema** — its typed affordance is the existing `Widget.ofControl` bridge.

## Runnable example — a pure control and a stateful control

```fsharp
namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

// Pure display control: one required field, lowers to the dedicated legacy builder.
type BadgeProps<'msg> = { Id: ControlId option; Text: string }

module Badge =
    let defaults: BadgeProps<'msg> = { Id = None; Text = "" }

    let view (props: BadgeProps<'msg>) : Widget<'msg> =
        FS.Skia.UI.Controls.Badge.create [ FS.Skia.UI.Controls.Badge.text props.Text ]
        |> Widget.ofControl   // lowers ≡ legacy Badge.create
```

```fsharp
// Stateful control: delegate init/update to the existing TextInput model (no fork).
type TextAreaProps<'msg> =
    { Id: ControlId; Value: string; ReadOnly: bool
      Validation: ValidationState; OnChanged: (string -> 'msg) option }

module TextArea =
    let init (props: TextAreaProps<'msg>) = TextInput.init props.Id MultiLine props.Value
    let update msg model = TextInput.update msg model      // pure, no I/O
    let view (props: TextAreaProps<'msg>) (model: TextInputModel) : Widget<'msg> =
        FS.Skia.UI.Controls.TextArea.create
            [ yield FS.Skia.UI.Controls.TextArea.value model.DraftText
              match props.OnChanged with
              | Some map -> yield FS.Skia.UI.Controls.TextArea.onChanged map  // None => no binding
              | None -> () ]
        |> Control.withKey props.Id
        |> Widget.ofControl
```

The parity test (the keystone) for the pure case:

```fsharp
let typed = Badge.view { Badge.defaults with Text = "beta" }
let legacy = FS.Skia.UI.Controls.Badge.create [ FS.Skia.UI.Controls.Badge.text "beta" ]
Expect.equal (show (Widget.toControl typed)) (show legacy) "Badge lowers to legacy IR"
```

## Build and test commands

Run `./fake.sh build -t Dev` for the inner loop. When the public `.fsi` changes,
run `./fake.sh build -t Route` first and run only the gates it prints — typically
`PackageSurfaceCheck`/`PerPackageSurfaceDiff` (additive-only), `ControlsCatalogGenerationCheck`
(catalog currency), and the escalated six-target order. Regenerate generated artifacts
(the `.claude` skill peer, the catalog) with `./fake.sh build -t RefreshSurfaceBaselines`;
never hand-edit them. Focused tests: `dotnet test tests/Controls.Tests/Controls.Tests.fsproj`.

## Persistent problems

When a lowering will not reach parity after reasonable in-repo attempts, **official
online docs first** (the F#/.NET docs and the SkiaSharp API reference), then community
sources (forums, issue trackers, changelogs). Record findings and resolving links in
`specs/<feature>/feedback/` and, for durable lessons, in this skill's **Sources** line.
Offline, record "research blocked — <why>" rather than hard-failing the phase.

## Related

- [[fs-skia-ui-widgets]] is the legacy string-keyed Controls authoring surface this
  typed front door re-expresses and lowers to.
- [[fs-skia-layout]] is the runtime layout engine the lowered controls compose over.
- [[fs-skia-scene]] is the primitive surface the controls ultimately render into.

## Sources / links

- F#/.NET docs: https://learn.microsoft.com/en-us/dotnet/fsharp/
- SkiaSharp (the driven Skia rendering library): https://github.com/mono/SkiaSharp
