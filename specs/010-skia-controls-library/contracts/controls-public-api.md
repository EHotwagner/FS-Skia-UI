# Contract: Controls Public API

## Purpose

The Controls public API is the contracted F# surface for describing reusable
Skia controls inside an Elmish-style view function.

## Required Package

```text
src/Controls/Controls.fsproj
PackageId: FS.Skia.UI.Controls
TargetFramework: net10.0
```

## Required Signature Files

```text
src/Controls/Types.fsi
src/Controls/Control.fsi
src/Controls/Attributes.fsi
src/Controls/Theme.fsi
src/Controls/Accessibility.fsi
src/Controls/Diagnostics.fsi
src/Controls/Catalog.fsi
src/Controls/TextInput.fsi
src/Controls/Collections.fsi
src/Controls/Charts.fsi
src/Controls/CustomControl.fsi
```

The implementation may split additional internal files, but every public module
must be represented in `.fsi` and in the package surface baseline.

## Core Shape

The public surface must support this authoring pattern:

```fsharp
module Example

open FS.Skia.UI.Controls

let view model =
    Stack.create [
        Stack.children [
            TextBlock.create [
                TextBlock.text model.Title
            ]
            Button.create [
                Button.text "Save"
                Button.enabled model.CanSave
                Button.onClick SaveRequested
            ]
            TextBox.create [
                TextBox.value model.Name
                TextBox.onChanged NameChanged
                TextBox.validation model.NameValidation
            ]
        ]
    ]
```

Required API concepts:

```fsharp
type Control<'msg>
type Attr<'msg>
type ControlId = string
type ControlEvent
type ControlDiagnostic
type AccessibilityMetadata
type Theme

module Control =
    val withKey: key: ControlId -> control: Control<'msg> -> Control<'msg>
    val render: theme: Theme -> control: Control<'msg> -> ControlRenderResult<'msg>
    val diagnostics: control: Control<'msg> -> ControlDiagnostic list
```

Control modules must expose `create : Attr<'msg> list -> Control<'msg>` and
consistent attribute naming for:

- content or text value where applicable
- child or children composition where applicable
- layout attributes
- style/theme attributes
- visual state attributes
- validation attributes
- accessibility attributes
- message-oriented event attributes

## Message-Oriented Events

Interactive controls must expose event attributes that produce application
messages:

```fsharp
module Button =
    val create: Attr<'msg> list -> Control<'msg>
    val text: value: string -> Attr<'msg>
    val onClick: msg: 'msg -> Attr<'msg>
    val onClickWith: map: (ControlEvent -> 'msg) -> Attr<'msg>
```

Validation rules:

- an enabled click, key activation, or equivalent tested action dispatches the
  expected message once
- disabled or read-only states suppress disallowed actions
- event mapping must use the current view description after model updates
- duplicate event attributes must either have documented precedence or produce
  diagnostics

## State Ownership

Persistent values are model-owned and passed as attributes:

- committed text
- selected values
- checked/toggled values
- numeric/slider values
- validation result
- loading/enabled/visible/read-only state

Control-owned state is limited to transient interaction state:

- hover
- pressed
- focus
- caret and active text selection interaction
- active drag
- in-progress composition

Any public API that stores durable application values inside the control runtime
violates this contract.

## Public Surface Baseline

Required baseline path:

```text
readiness/surface-baselines/FS.Skia.UI.Controls.txt
```

`PackageSurfaceCheck` must fail when:

- a public module has no `.fsi`
- an implementation exports members omitted from `.fsi`
- the baseline differs without feature evidence
- removed chart members have no compatibility-impact record
- the Controls package accidentally exposes implementation-only diagnostics or
  mutable runtime internals
