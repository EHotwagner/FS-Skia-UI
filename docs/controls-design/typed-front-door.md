---
title: Typed Control Front Door
category: Controls & design tokens
categoryindex: 4
index: 20
description: The typed Props/MVU authoring surface under FS.Skia.UI.Controls.Typed — an immutable Props record plus defaults and a view returning Widget that lowers structurally equal to the legacy builder, proven by per-control parity tests.
---

# Typed Control Front Door

`FS.Skia.UI.Controls` ships two authoring surfaces for the same control catalog.
The original surface is the **legacy string-keyed builder**: you assemble a control
from a heterogeneous `Attr<'msg> list` keyed by string names, where a misspelled
attribute name, a wrong value type, or a missing required attribute is caught at
*runtime* by catalog diagnostics rather than by the compiler. The **typed front
door** — the modules under the `FS.Skia.UI.Controls.Typed` namespace introduced in
feature 065 and extended across the whole catalog in features 070–072 — re-expresses
each control as an immutable, compiler-checked `Props` record with a `defaults`
value and a `view` function that returns a `Widget<'msg>`. The typed surface is
**additive**: the legacy builders are byte-frozen and still compile, and every typed
`view` lowers to the *exact same* `Control<'msg>` IR the legacy builder produces —
a property proven, control by control, by a mandatory lowering-parity test. This
page explains that design, why the lowering seam exists, and how to author against
it.

This page is part of the controls deep-dive set; see also the
[design-token flow](design-tokens-penpot.html) that feeds the same typed surface,
the [controls architecture](../architecture/controls.html) page for the renderer
and Elmish adapter beneath it, and the [API reference](../reference/index.html) for
the generated per-member signatures.

## Why a typed front door

The control catalog was always broad — 52 supported controls as of feature 072 —
but the authoring contract was weak. The core IR, `Control<'msg>`, carries a
string `Kind` and an `Attr<'msg> list` whose value union includes an
`UntypedValue of obj` escape hatch. That is the right shape for an internal
intermediate representation that the renderer, layout engine, diagnostics, and
accessibility passes all consume uniformly — but it is the wrong shape for a public
authoring surface, because nothing in the type system enforces the per-control
contract. The typed front door moves the string keys off the public surface (where
they are unsafe) and onto an internal lowering seam (where they are an
implementation detail), satisfying the repo's "visibility lives in the `.fsi`"
principle while leaving the entire downstream pipeline untouched.

## The two-axis model

Every typed control is the product of two axes:

```
Control = (Props : immutable typed record)  ×  (optional MVU : Model × Msg × update)
```

- **`Props<'msg>`** is the "well-defined variable values" for the control: a closed
  record that is the public authoring surface and the compile-time contract.
  Defaults come from a `defaults` value; you modify them with ordinary F# record
  `with` syntax. Pure display, input, and container controls are *only* `Props`.
- **MVU** is present **only** for controls that own ephemeral UI state. These reuse
  the existing per-control models (`TextInput`, `DataGrid`, `Collections`, charts)
  rather than forking new ones — the typed module is a thin typed façade whose
  `init`/`update` delegate to the already-shipped pure model.
- **`Widget<'msg>`** is the return type of every `view`: a thin, opaque wrapper over
  the lowered `Control<'msg>` IR.

### Props fields follow a fixed taxonomy

Each `Props` record draws its fields from a fixed taxonomy so that the records are
consistent and a new control follows the template: Identity (`Id: ControlId option`),
Content (`Text`, `Children`), Data (`Rows`, `Columns`), Behavior (`Enabled`,
`ReadOnly`), Variant (`Intent`), Layout (`Orientation`, `Spacing`), Accessibility,
and Events (Elmish message callbacks). The rule is uniform: every **required** value
is a non-optional field; every optional value resolves through `defaults`; every
event is an **optional callback** that lowers to *no binding* when `None`. No field
is `obj`, untyped, or a string-named event.

You can see this concretely in `src/Controls/Widgets/Primitives.fsi`:

```fsharp
namespace FS.Skia.UI.Controls.Typed

type ButtonIntent = Primary | Secondary | Danger | Ghost

type ButtonProps<'msg> =
    { Id: ControlId option
      Text: string
      Enabled: bool
      Intent: ButtonIntent
      OnClick: 'msg option }      // None => no event binding, never a default message

module Button =
    val defaults : ButtonProps<'msg>
    val view     : ButtonProps<'msg> -> Widget<'msg>
```

Authoring stays terse and compiler-checked. A typo in a field name, a wrong value
type, or a missing required field is now a compile error rather than a runtime
diagnostic:

```fsharp
Stack.view
  { Stack.defaults with
      Orientation = Vertical
      Children =
        [ TextBlock.view { TextBlock.defaults with Text = "Sign in" }
          Button.view    { Button.defaults with Text = "Submit"
                                                Intent = Primary
                                                OnClick = Some Save } ] }
```

## The `Widget<'msg>` wrapper and the lowering seam

`Widget<'msg>` is declared `[<Sealed>]` in `src/Controls/Widget.fsi` with its
representation — `{ Lowered: Control<'msg> }` — kept entirely in the implementation
file. The public module exposes exactly three functions:

```fsharp
[<Sealed>]
type Widget<'msg>

module Widget =
    val ofControl : Control<'msg> -> Widget<'msg>                       // bridge in
    val toControl : Widget<'msg> -> Control<'msg>                       // lowering accessor
    val render    : Theme -> Widget<'msg> -> ControlRenderResult<'msg>  // = render (toControl w)
```

The implementation is deliberately trivial — `ofControl` wraps, `toControl`
unwraps, `render` delegates to `Control.render` — which is what makes the lowering
seam cheap and explicit:

```fsharp
type Widget<'msg> = { Lowered: Control<'msg> }

module Widget =
    let ofControl control = { Lowered = control }
    let toControl widget  = widget.Lowered
    let render theme widget = Control.render theme widget.Lowered
```

A sealed wrapper was chosen over a bare alias (`type Widget<'msg> = Control<'msg>`)
for three reasons: it keeps the door open for keyed-reconciliation metadata (feature
067) without another public-surface break; it forces consumers through
`Widget.toControl`, so the single lowering seam is explicit and greppable; and it
lets `Widget` and the untouched legacy `Control<'msg>` API coexist during the
preview window. `Widget.ofControl` also serves as the migration bridge: you can drop
a legacy `Control<'msg>` straight into a typed `Stack.Children` list, and the
`custom-control` escape hatch (`src/Controls/Widgets/CustomControlWidget.fsi`) has
**no fabricated `Props` schema** — its typed affordance is exactly this
`Widget.ofControl` bridge.

## How a typed `view` lowers to the legacy builder

The lowering is correct *by construction*: a typed `view` calls the **exact same**
legacy `*.create`/`Attr` builders the legacy authoring path calls, then wraps the
result with `Widget.ofControl`. Nothing re-implements attribute assembly. From
`src/Controls/Widgets/Primitives.fs`:

```fsharp
module Button =
    let view (props: ButtonProps<'msg>) : Widget<'msg> =
        let attrs =
            [ yield FS.Skia.UI.Controls.Button.text props.Text
              yield FS.Skia.UI.Controls.Button.enabled props.Enabled
              yield Attr.style (LegacyControls.intentStyle props.Intent)
              match props.OnClick with
              | Some msg -> yield FS.Skia.UI.Controls.Button.onClick msg
              | None -> () ]                                 // None => no binding
        FS.Skia.UI.Controls.Button.create attrs
        |> LegacyControls.withKeyOpt props.Id
        |> Widget.ofControl
```

Because the typed `view` and the legacy builder construct the same
`Attr<'msg> list`, the resulting `Control<'msg>` is structurally equal. That is the
keystone proof: a per-control **lowering-parity test** asserts that
`view props |> Widget.toControl` is structurally equal to the hand-written legacy
builder output, with attributes order-normalized and events canonicalized to the
message they produce. The pure-control parity test reads, in essence:

```fsharp
let typed  = Button.view { Button.defaults with Text = "Submit"; OnClick = Some Save }
let legacy = Button.create [ Button.text "Submit"; Button.onClick Save ]
Expect.equal (normalize (Widget.toControl typed)) (normalize legacy)
             "typed Button lowers to legacy IR"
```

This single proof protects every downstream test — render, accessibility,
diagnostics, evidence — without duplicating them, because once the lowered IR is
proven equal, everything that consumes the IR behaves identically.

## Stateful controls reuse the existing MVU model

For controls that own ephemeral UI state, the typed module is a thin façade over an
existing pure model — it never forks one. `TextBox`
(`src/Controls/Widgets/TextBoxWidget.fs`) is representative: `init` and `update`
delegate straight to `TextInput`, returning the existing `TextInputModel`/
`TextInputMsg`/`TextInputEffect` types, and `view` lowers the current model state
through the legacy `TextBox` builder:

```fsharp
module TextBox =
    let init (props: TextBoxProps<'msg>) = TextInput.init props.Id props.Mode props.Value
    let update msg model = TextInput.update msg model         // pure, no I/O
    let view (props: TextBoxProps<'msg>) (model: TextInputModel) : Widget<'msg> =
        [ yield FS.Skia.UI.Controls.TextBox.value model.DraftText
          yield FS.Skia.UI.Controls.TextBox.readOnly props.ReadOnly
          yield FS.Skia.UI.Controls.TextBox.validation model.Validation
          match props.OnChanged with
          | Some map -> yield FS.Skia.UI.Controls.TextBox.onChanged map
          | None -> () ]
        |> FS.Skia.UI.Controls.TextBox.create
        |> Control.withKey props.Id
        |> Widget.ofControl
```

A delegation test asserts the typed `update` result equals the reused model's
`update` for the same input, keeping the MVU layer additive too. Product/business
state stays in the product's own Elmish model; the per-control model holds only
ephemeral UI state (draft text, caret, selection, focus), exactly as Constitution
Principle IV requires for stateful controls.

## How to use it across the Spec Kit process

The typed front door is the **preferred** authoring path for new product code, and
the legacy builders remain a permanent peer (the deprecation decision, open question
Q1 in the plan, resolved to "keep as peer"). When a feature adds or reviews a typed
control, that work happens during `speckit-implement` and is driven by the
`fs-skia-typed-controls` skill. Because the typed modules live under
`src/Controls/**` and change a public `.fsi`, the change routes through the
`controls-public-surface` rule (`build/Governance/Routing.fs`), which escalates to
the focused-authority gate set — including `PackageSurfaceCheck` (additive-only) and
`ControlsCatalogGenerationCheck` (catalog currency). Run `./fake.sh build -t Route`
first and run only the gates it prints; see the
[Spec Kit process](../speckit/process.html) page for where this sits in the wider
spec → clarify → plan → tasks → implement → analyze flow.

## Analysis

### Implementation strengths

- The lowering is correct *by construction* — `view` calls the identical legacy
  `*.create`/`Attr` builders (`src/Controls/Widgets/Primitives.fs`) and only wraps
  the result — so a typed control cannot silently diverge from its legacy peer in
  attribute assembly, and the per-control parity test makes that guarantee
  executable rather than aspirational.
- The change is genuinely additive: `Widget<'msg>` and the typed modules are new
  declarations on the `.fsi`, the legacy `Control.create`/`Attr` API is byte-frozen,
  and no new dependency (notably not `Fable.Elmish`) enters `FS.Skia.UI.Controls`,
  so existing consumers keep compiling unchanged.

### Implementation weaknesses

- The `Widget<'msg>` representation is the trivial `{ Lowered: Control<'msg> }` with
  pass-through accessors, so today it carries no diff/identity metadata; the sealed
  wrapper reserves space for keyed reconciliation, but feature 067's diff is
  internal and deliberately unwired, so the wrapper currently buys indirection
  without yet delivering the reconciliation it was shaped for.
- Parity is proven by structural equality of the lowered IR with attributes
  order-normalized and events canonicalized — a strong but indirect proof; a bug
  that is invisible at the IR level (for example a stale `normalize` helper or an
  attribute the normalizer drops) could let a typed-only divergence through, since
  the test never renders pixels.

### Design pros

- Moving string keys from the public surface to an internal lowering seam gives
  consumers compile-time enforcement of each control's contract while leaving the
  renderer, layout, diagnostics, accessibility, and evidence pipelines completely
  unchanged — a large safety win for a small, contained blast radius.
- Reusing the existing `TextInput`/`DataGrid`/`Collections` models for stateful
  controls (rather than minting parallel models) keeps the MVU layer additive and
  honours the one-model-per-control boundary, so the typed façade adds ergonomics
  without adding a second source of truth for control state.

### Design cons

- Two coexisting authoring surfaces (typed and legacy) is a real, ongoing cognitive
  and maintenance cost: every catalog control now has two ways to author it, the
  parity test must be kept green for both, and the "preferred but not enforced"
  status means consumers can mix the two and lose the typed guarantees at the seam.
- The opaque `Widget<'msg>` plus the mandatory `Widget.toControl` hop adds a layer
  of indirection that a bare type alias would not, and the `custom-control` escape
  hatch has no `Props` schema at all — so the most open-ended control reverts to the
  untyped `Widget.ofControl` bridge, leaving a typed-surface gap exactly where
  authors are most likely to need help.
