# Research: Archive Readiness And API Docs

## Decision: Archive historical readiness in place

Keep historical readiness material in its original feature directory by
default. Add archive inventory rows, archival markers, and active guidance
that explains the material is historical audit context, not current pass/fail
evidence.

**Rationale**: Historical readiness files contain provenance, prior command
results, unsupported-host classifications, and synthetic-evidence disclosures.
Moving or deleting broad historical trees risks breaking audit trails and
links. In-place archival gives reviewers a current-vs-historical map without
destroying context.

**Alternatives considered**:

- Move historical readiness to a central archive directory. Rejected for this
  feature because it would churn many paths and make old specs harder to
  audit.
- Delete obsolete generated outputs. Allowed only for clearly obsolete files
  whose paths are not needed for historical audit, and only with an inventory
  row naming the reason.

## Decision: Block stale references only from active surfaces

The stale-reference scan blocks active docs, templates, generated guidance,
build reports, and the active feature when they cite archived readiness as
current evidence. Historical specs and readiness folders are informational scan
inputs unless an active surface cites them as current gate evidence.

**Rationale**: The repository intentionally keeps historical specs. Blocking
all old references inside historical material would turn archival into a
large rewrite. The actual risk is an active reviewer or generated-product
agent treating archived material as current evidence.

**Alternatives considered**:

- Block every stale-looking reference anywhere under `specs/`. Rejected as too
  noisy and counter to the clarification that historical specs are
  informational.
- Do not scan historical material at all. Rejected because informational
  findings help maintain the archive inventory and catch active citations.

## Decision: Keep curated `.fsi` API reference authoritative for agents

The current `scripts/generate-package-api-reference.fsx` workflow remains the
authoritative package API reference for agent authoring. It reads curated
`.fsi` files, emits package-specific Markdown, preserves source-shaped F#
signatures, reports symbol/XML summary counts, and declares
`assembly-reflection: false` plus
`repository-source-authoring-fallback: false`.

**Rationale**: The user need is package-consumer authoring fidelity: record
fields, union cases, module functions, parameter names, and mixed
Scene/Controls qualification guidance must be discoverable without reflection
or repository source inspection. The existing tests in
`tests/Package.Tests/PackageApiReferenceTests.fs` already encode that contract
for Scene, Controls, SkiaViewer, KeyboardInput, and other packages.

**Alternatives considered**:

- Replace the generator with reflection over compiled assemblies. Rejected
  because compiled names can differ from F# authoring forms and prior feature
  requirements explicitly rejected reflection as the authoring strategy.
- Replace the generator immediately with fsdocs. Rejected until a spike proves
  fsdocs output preserves every agent-facing source-shaped contract.

## Decision: Evaluate FSharp.Formatting/fsdocs as secondary or hybrid output

Use FSharp.Formatting/fsdocs for a representative spike and decision record,
not as an immediate authoritative replacement. Compare `Scene`, `Controls`,
and `SkiaViewer` or `Controls.Elmish` output against the current curated
reference for source-shaped fidelity, XML documentation, package-adjacent
discoverability, Markdown/HTML suitability, dependency impact, generated
guidance compatibility, and LLM-oriented output.

**Rationale**: FSharp.Formatting is the standard F# documentation tooling and
its docs state that `fsdocs build` generates API documentation for F#
libraries with XML comments, selects projects with `GenerateDocumentationFile`,
and can produce searchable HTML plus `llms.txt`/`llms-full.txt`. It is a good
candidate for browsable or companion docs. However, its API docs are generated
from compiled projects/assemblies and XML docs, and the repository's
authoring-agent contract is stricter than general browsable API docs.

**Sources**:

- FSharp.Formatting API docs: https://fsprojects.github.io/FSharp.Formatting/apidocs.html
- FSharp.Formatting command line: https://fsprojects.github.io/FSharp.Formatting/commandline.html
- F# XML documentation: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/xml-documentation

**Alternatives considered**:

- Do not evaluate fsdocs. Rejected because the feature explicitly asks whether
  reference API material would be better created by FSharp.Formatting.
- Adopt fsdocs only for external hosted documentation. Deferred because
  external hosting is out of scope.

## Decision: Avoid new runtime or public API changes

This feature changes repository guidance, archive/scanner tooling, optional
documentation generation evaluation, and readiness reports. It should not
change runtime behavior, public `.fsi` signatures, package identities, or
surface baselines unless the fsdocs evaluation reveals an unavoidable package
reference-material gap.

**Rationale**: The spec scopes public API contracts and runtime behavior out
unless documentation-only changes cannot meet package consumer needs. The
existing source-shaped reference path already satisfies the prior package
discovery contract.

**Alternatives considered**:

- Add fsdocs output to packages immediately. Deferred until the spike proves
  package contents should change and dependency/package governance is updated.
