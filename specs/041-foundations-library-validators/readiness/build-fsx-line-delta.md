# build.fsx line-count delta (SC-001)

Authoritative command: `wc -l build.fsx` (before/after the extraction).

| Snapshot | `wc -l build.fsx` | Source |
|----------|-------------------|--------|
| 041 pre-extraction baseline (post-040 HEAD `76414fb`) | **4839** | T002 |
| Post-extraction (this feature) | **4454** | T019 |
| **Shrink** | **385 lines** | 4839 − 4454 |

## What moved out of build.fsx

The two validators, their shared types, and the stringly-typed registries moved into the
compiled `build/Governance` library (`Findings`, `Targets`, `TargetMetadata`,
`Capabilities` — `.fsi` + `.fs`, 833 lines total incl. signatures):

- the bespoke `readCapabilityCatalog` line-by-line YAML state machine + `emptyCapability`
  + the `trimQuotes`/`parseScalar`/`parseInlineList` wrappers (retired; YamlDotNet now);
- `validateCapabilityRows` + the `finding` constructor + the `writeFindingsOrPass` detail
  format (now `Capabilities.validateRows` / `Findings.finding` / `Findings.renderDetail`);
- `CapabilityRow`, `ValidationFinding`, `TargetMetadata`, `TargetMetadataReport`,
  `TargetMetadataDrift`, `RunnableTargetName` type definitions;
- `ValidateTargetMetadataDrift`, `targetMetadataJson`, `targetMetadataDriftMarkdown`,
  `driftDiagnostic`, and the `jsonEscape`/`jsonString`/`jsonArray` helpers;
- the `requiredTargets` (38-row) and `targetDependencyRows` (40-row) string registries +
  the `targetDependencies` map + `directPrerequisites` lookup — all now DERIVED from the
  typed `Targets.Target` DU + total `Targets.spec`.

## SC-001 variance (disclosed, not padded)

SC-001/T019 set the shrink target at **≥800 lines**. The realized shrink is **385**.

The gap is a scope boundary recorded in research **R3**: the bulk of the target-metadata
code is `focusedGateContract` (~120 lines) plus the `BuildModel` path machinery and the
`targetMetadata` record assembly, which depend on runtime `BuildModel` paths and the
`VerificationVerdictCategory`/effect boundary. R3 deliberately keeps that at the build.fsx
interpreter edge (path resolution and I/O stay at the edge, Principle IV); moving
`focusedGateContract`/`BuildModel` wholesale into the library is the **Stage-5** MEL-engine
relocation, explicitly out of scope (FR-001a). The ≥800 figure in the plan over-counted by
assuming that machinery moved in Stage 3; the honest extractable surface for *these two
validators + the typed registries* is ~386 lines.

All other SC criteria are met: golden-diff parity = 0 bytes (SC-002/005, see
`report-parity.md`), ≥6 typed-finding cases (SC-004), `git diff src/**` empty (SC-007),
no new `PackageVersion` outside `Directory.Packages.props` (FR-010/FR-012).

Failure class if this gate is treated as hard-blocking: `governance / line-count-target`.
Next action: either accept the bounded Stage-3 shrink (recommended — parity + typed model
are the substantive wins) or schedule the `focusedGateContract`/`BuildModel` relocation as
part of Stage-5 to realize the remaining reduction.
