# Contract: Regenerated typed starter (`template/base/src/Product/View.fs`)

The generated starter demonstrates the compiler-guided typed front door as the primary
authoring path (US1 / FR-001..FR-003).

## Required shape

- Controls are authored via `FS.Skia.UI.Controls.Typed` modules using the
  `{ Module.defaults with Field = ... } |> Module.view` (or equivalent record-literal)
  pattern — **not** `Module.create [ Module.attr ... ]` legacy attr lists.
- **Not every typed module uses the bare `defaults`/`view` shape.** The starter MUST
  use each module's actual signature:
  - `TextBlock`/`Button` (`Primitives.fsi`): `defaults: …Props<'msg>`,
    `view: props -> Widget<'msg>` — the uniform `{ defaults with … } |> view` form.
  - `TextBox` (`TextBoxWidget.fsi`): `defaults: ControlId -> TextBoxProps<'msg>` **and**
    `view: props -> model: TextInputModel -> Widget<'msg>` — so the interactive input is
    authored as `TextBox.view { TextBox.defaults "<id>" with Value = …; OnChanged = … }
    <textModel>`, where `<textModel>` is the retained per-identity `TextInputModel` the
    live host (`ControlsElmish`) already tracks. The starter MUST show where that model
    comes from, not invent a literal.
- The typed `view` returns `Widget<'msg>`; the rewritten `controlsExampleView` MUST
  remain a valid view for `ControlsElmish.program` (lower/compose the `Widget` tree the
  way `ControlsElmish.program` consumes today's legacy tree). `GeneratedProductCheck`
  confirms this wiring compiles and renders.
- Demonstrates at minimum (FR-002):
  - a **display** control (e.g. `TextBlock`),
  - an **interactive input** control (e.g. `TextBox` with an `OnChanged` message),
  - a **button with an event handler** (`Button` with `OnClick = Some msg`).
- The `OnClick = None` / optional-binding behavior is visible or commented so the
  consumer learns the "omit to bind nothing" idiom from the starter (edge case).
- Any control the starter uses that is NOT yet in the typed front door stays on the
  legacy builder, with a one-line comment pointing at the typed path for the rest
  (keeps non-migrated controls reachable — edge case / FR-005 documents them anyway).

## Behavioral obligation

- **Parity**: every typed control the starter uses is covered by
  `tests/Controls.Tests/TypedLoweringTests.fs` (lowers structurally equal to the legacy
  builder). If the starter introduces a typed control not yet in the parity suite, add
  a parity case for it.
- **Renders unchanged**: `GeneratedProductCheck` shows the regenerated product compiles
  and renders the same controls (no visual/behavioral regression vs. today's starter).
- **Bundle currency**: `template/base/docs/api-surface/Controls/*.fsi` (already
  containing the typed `Widgets/*.fsi`) passes `ApiSurfaceGen.currency` after
  `RefreshSurfaceBaselines`.

## README pointer obligation (FR-010/FR-013)

`template/base/README.md`'s existing "do not use reflection / use the source-shaped API
reference" guidance MUST resolve to concrete, populated targets:
- the typed front door demonstrated in `View.fs`,
- the `docs/api-surface/Controls/*.fsi` bundle,
- the documented `Catalog.*` discovery API,
- the bundled consumer-visible catalog reference under `docs/`.

No "do not reflect" instruction may be left without a usable, named alternative
(SC-005).
