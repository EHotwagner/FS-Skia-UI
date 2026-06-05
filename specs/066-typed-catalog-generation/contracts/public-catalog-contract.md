# Contract: Public Catalog Surface (UNCHANGED)

This feature is non-behavioral for the shipped `FS.Skia.UI.Controls` package.
This file records the surface that MUST remain stable, so any movement is caught
as a regression rather than accepted as a delta.

## Stable public surface — `src/Controls/Catalog.fsi` (no edit)

```fsharp
type CatalogAccessibility =
    { Role: string; NameSource: string; StateMetadata: string list
      FocusBehavior: string; KeyboardOperation: string; ContrastEvidence: string }

type ControlDefinition =
    { Id: string; DisplayName: string; Category: string; Module: string
      Purpose: string; RequiredAttributes: string list; CommonAttributes: string list
      Events: string list; VisualStates: string list; Accessibility: CatalogAccessibility
      Examples: string list; Tests: string list; Evidence: string list
      SupportStatus: string; Owner: string }

module Catalog =
    val supportedControls: ControlDefinition list
    val standardSchema: ControlSchema list
    val knownControlKinds: unit -> StandardControlKind list
    val requiredAttributes: kind: StandardControlKind -> StandardAttributeName list
    val supportedAttributes: kind: StandardControlKind -> StandardAttributeName list
    val supportedEvents: kind: StandardControlKind -> StandardEventKind list
    val validateStandardControl: control: Control<'msg> -> ControlDiagnostic list
    val supportedCount: unit -> int
    val categories: unit -> string list
    val validate: unit -> ControlDiagnostic list
    val markdownSummary: unit -> string
```

## Invariants the migration must preserve (asserted by `CatalogTests`)

- `Catalog.supportedControls` has **47** entries, in the **same order**, with the
  same `ControlDefinition` values as before the migration.
- `Catalog.supportedCount () = 47`; `Catalog.categories ()` = the same 10 categories.
- `catalog.yml` `summary.supportedCount: 47`, `categories`, and `defaults:` block
  are unchanged; the 41 non-typed rows are **byte-for-byte** unchanged (SC-005).
- The six generated rows (`text-block`, `button`, `check-box`, `stack`, `text-box`,
  `data-grid`) are **structurally identical** to their pre-migration rows (FR-004,
  SC-002), in `catalog.yml` and `Catalog.fs` alike.
- No `.fsi` signature changes; surface baselines do not move
  (`PackageSurfaceCheck` passes unchanged). A baseline delta is a regression to
  investigate (spec: Public contract impact).
- `Catalog.validate ()` still returns no errors (its data-grid / rich-text / owner
  invariants, `Catalog.fs:292-313`, continue to hold).

## Consumers that must observe no change

`ControlsCatalogCheck`, the existing `CatalogTests` assertions,
`GeneratedGuidanceCheck`/`GeneratedProductCheck` catalog consumers, and the
`samples/ControlsGallery` examples references — all pass without assertion changes
(FR-007), except the intentional **extension** of `CatalogTests` with the new
generated-vs-source parity, drift, and typed-registry correspondence tests.
