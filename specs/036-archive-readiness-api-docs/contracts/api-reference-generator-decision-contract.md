# Contract: API Reference Generator Decision

The generator comparison writes:

`specs/036-archive-readiness-api-docs/readiness/api-reference-generator-evaluation.md`

The fsdocs spike writes:

`specs/036-archive-readiness-api-docs/readiness/fsharp-formatting-spike.md`

## Required Packages

- `FS.Skia.UI.Scene`
- `FS.Skia.UI.Controls`
- one host/adapter package: prefer `FS.Skia.UI.SkiaViewer`; use
  `FS.Skia.UI.Controls.Elmish` if it exposes a more relevant comparison risk.

## Required Comparison Dimensions

- F# authoring spelling fidelity
- record-field and union-case visibility
- parameter names and labels
- XML documentation preservation
- package-adjacent discoverability
- Markdown or HTML output suitability
- dependency and build impact
- generated product guidance compatibility
- mixed Scene/Controls qualification guidance
- clean package-consumer discovery without repository source inspection or
  reflection as the authoring strategy

## Required Decision Values

- `authoritative`: accepted as the agent-facing reference
- `secondary`: useful companion docs but not authoritative
- `hybrid`: generated browsable docs plus curated `.fsi` authoritative
  reference
- `rejected`: does not meet required guarantees
- `deferred`: blocked pending documented next action

## Rules

- The curated `.fsi` workflow remains authoritative unless fsdocs passes every
  required dimension for all required package samples.
- fsdocs output may be accepted as secondary/hybrid when it improves
  browsability without weakening the source-shaped contract.
- A committed fsdocs dependency requires dependency governance updates.

## Failure Conditions

- Missing required package sample.
- Missing comparison dimension.
- Replacement decision without proof that source-shaped authoring names remain
  discoverable.
- fsdocs blocker lacks command, log path, and next action.
