# Contract: Controls Public API

## Purpose

Define the public API obligations for the refactored Controls boundary.
Implementation must start from `.fsi` signatures, then semantic tests, then
`.fs` bodies.

## Package Surface

- Package id: `FS.Skia.UI.Controls`
- Namespace: `FS.Skia.UI.Controls`
- Required public contracts:
  - `src/Controls/Types.fsi`
  - `src/Controls/Control.fsi`
  - `src/Controls/ControlRuntime.fsi`
  - `src/Controls/Attributes.fsi`
  - `src/Controls/Theme.fsi`
  - `src/Controls/Accessibility.fsi`
  - `src/Controls/Diagnostics.fsi`
  - `src/Controls/Catalog.fsi`
  - `src/Controls/RichText.fsi`
  - `src/Controls/TextInput.fsi`
  - `src/Controls/Collections.fsi`
  - `src/Controls/Charts.fsi`
  - `src/Controls/DataGrid.fsi`
  - `src/Controls/CustomControl.fsi`

`RichText.fsi`, `DataGrid.fsi`, and `ControlRuntime.fsi` may be folded into
existing files only if the final surface remains clearly documented and surface
baselines expose equivalent contracts.

## Required API Groups

- Stable control records for ordinary form, display, navigation, feedback,
  collection, chart, graph, and DataGrid controls
- Generic message-producing event attributes
- Explicit Skia escape hatches for advanced/custom rendering
- Control runtime model, messages, update function, effects, diagnostics, and
  recovery helpers
- Rich text or rich rendering declarations that expose Skia-specific concepts
- Catalog metadata access for supported Controls-owned controls
- Diagnostics for stale targets, missing metadata, dependency leaks, and
  unsupported environment conditions

## Public API Rules

- Base control declarations are generic over product messages.
- Ordinary control declarations do not expose Elmish `Cmd`, `Program`, or host
  loop concepts.
- Persistent business values are supplied by the product model and returned
  through product messages/effects.
- Transient interaction state is represented by product-owned
  `ControlRuntime`.
- Advanced Skia rendering is explicit and documented as Skia-specific.
- DataGrid is a data or collection control.
- Chart and graph controls are Controls-owned and do not require
  `FS.Skia.UI.Charts`.

## Validation

- Package surface baseline:
  `readiness/surface-baselines/FS.Skia.UI.Controls.txt`
- FSI transcripts construct:
  - one ordinary stable-record form flow
  - one rich text or rich rendering escape-hatch flow
  - one chart flow
  - one graph or DataGrid flow
  - one control runtime update/recovery flow
- Failing diagnostics identify the public module, control id, catalog row, or
  missing evidence path.
