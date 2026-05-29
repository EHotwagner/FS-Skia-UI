# Contract: Typed Controls Front Door

## Scope

Every existing standard controls module must expose typed or otherwise constrained authoring paths while preserving deliberate custom extension APIs.

Covered standard modules include:

- base controls in `Control.fsi`
- event helpers in `Attributes.fsi`
- chart controls in `Charts.fsi`
- grid controls in `DataGrid.fsi`
- schema/catalog/diagnostic support in `Catalog.fsi` and `Diagnostics.fsi`

## Required Public Concepts

The implementation must expose typed or constrained equivalents for:

- known standard control kinds
- known standard event kinds
- known standard attribute names
- chart series and chart point data
- grid columns, rows, visible range, selected rows, and focused cell data
- custom control kinds, custom events, custom attributes, and custom values

## Compatibility

Existing flexible lowered forms remain available:

- `Control<'msg>`
- `Attr<'msg>`
- existing standard module constructors
- deliberate custom extension path

Compatibility APIs may delegate to typed/schema-backed paths, but custom usage must be visibly named and must not masquerade as a known standard contract.

## Diagnostics

Schema-backed diagnostics must report:

- missing required standard attributes
- unsupported standard attributes
- unsupported standard events
- custom usage where custom extension paths were used

Diagnostic vocabulary must come from the shared control schema rather than scattered string literals.

## Acceptance

Seeded misspellings for standard control kinds, standard event kinds, standard chart data attributes, and standard grid data attributes must be prevented at compile time or rejected by typed/schema validation. Deliberate custom controls/events/values must remain possible and visibly classified as custom.
