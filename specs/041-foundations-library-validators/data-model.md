# Phase 1 Data Model — Foundations Library Validators (041)

Entities owned by `FS.Skia.UI.Build` after extraction. All types are build-tooling; visibility is
declared in the companion `.fsi` (Principle II). Records reproduce the existing `build.fsx` shapes
exactly so reports stay byte-identical.

## Findings module (`Findings.fs/.fsi`) — FR-004

### `ValidationFinding`
The uniform structured result every extracted validator returns (moved verbatim from `build.fsx`
~54–58 so report text is reconstructed identically).

| Field | Type | Notes |
|---|---|---|
| `ArtifactClass` | `string` | e.g. `"capability-catalog"` |
| `Path` | `string` | offending path or capability id |
| `Rule` | `string` | rule id (e.g. `displayName`, `dependency`) |
| `Message` | `string` | human-readable detail |

- `finding : artifactClass:string -> path:string -> rule:string -> message:string -> ValidationFinding`
  — the constructor used pervasively (`build.fsx` ~2310).
- Rendering helper(s) reproducing `writeFindingsOrPass`'s detail-line format
  (`` - `{Path}` [{Rule}]: {Message} ``) so failure messages match byte-for-byte.

## Targets module (`Targets.fs/.fsi`) — FR-001

### `Target` (discriminated union)
One nullary case per runnable target currently in `requiredTargets` (`Clean`, `Restore`, `Build`,
`Test`, `Dev`, `CapabilityCheck`, `SkillCheck`, `TargetMetadata`, `TargetMetadataDrift`,
`SkillSyncCheck`, `SkillExamplesCheck`, … through the full ~45-case list). Closed union ⇒ exhaustive
matching ⇒ a mistyped/renamed target is a compile error (SC-003).

### `TargetSpec`
The single source from which identity + dependencies + metadata-shape derive (R3).

| Field | Type | Derives |
|---|---|---|
| `Target` | `Target` | identity |
| `Name` | `string` | the canonical runnable-target string (for FAKE registration / report text) |
| `DirectPrerequisites` | `Target list` | replaces `targetDependencyRows` |
| `TimeoutClass` | `string` | `"broad"`/`"medium"`/`"focused"` (was the `match target` at ~846) |
| `Cost` | `string` | `"high"`/`"medium"`/`"low"` (was ~855) |
| `FailureOwner` | `string` | `"template"`/`"product"`/`"governance"` (was ~868) |

- `allTargets : Target list` — the ordered list (replaces `requiredTargets`; preserves order/positions, FR-013).
- `spec : Target -> TargetSpec` — **total**; the single source of truth.
- `name : Target -> string` and `directPrerequisites : Target -> Target list` — convenience projections.
- Derived views for `build.fsx` compatibility: `requiredTargetNames : string list`,
  `targetDependencyRows : (string * string list) list` (computed from `spec`, not maintained).

**Invariant:** there is no way to express a runnable target without a spec (every `Target` matches in
`spec`) nor a spec without a runnable target (specs are produced only by mapping `spec` over
`allTargets`). This is the structural elimination of the drift `TargetMetadataDrift` checks for.

## TargetMetadata module (`TargetMetadata.fs/.fsi`) — FR-002

### `TargetMetadata`
Per-target descriptor (moved from `build.fsx` ~170–184), now **computed from** `TargetSpec` plus the
runtime path inputs injected at the edge (R3).

| Field | Type |
|---|---|
| `RunnableTargetName` | `string` |
| `DirectPrerequisites` | `string list` |
| `ExpectedOutputs` | `string list` |
| `StaleAssumptions` | `string list` |
| `TimeoutClass` / `Cost` / `Authority` / `FailureOwner` / `Command` | `string` |

### `TargetMetadataDrift` (discriminated union)
Moved verbatim (`build.fsx` ~186). Cases: `MissingRunnableTarget`, `MissingMetadata`,
`MissingExpectedOutput`, `MissingFailureOwner`, `DependencyDivergence` (each `of string`). These are
the **typed** findings unit tests assert (SC-004).

**Functions (pure):**
- `validateMetadataDrift : runnableTargets:string list -> metadata:TargetMetadata list -> TargetMetadataDrift list`
  (was `ValidateTargetMetadataDrift`).
- `validateAgainstRepo : contractReferences:string list -> docReferences:string list -> runnableTargets:string list -> metadata:TargetMetadata list -> string list`
  — drift + contract-drift + docs-drift diagnostics (was `validateTargetMetadataAgainstRepo`; the file
  reads that produce `contractReferences`/`docReferences` stay at the `build.fsx` edge, passed in).
- `driftDiagnostic : TargetMetadataDrift -> string` — exact message strings (~960–966).
- `metadataJson : generatedAtUtc:string -> diagnostics:string list -> metadata:TargetMetadata list -> string`
  — the JSON renderer with the timestamp as an **explicit parameter** (R2).
- `driftMarkdown : diagnostics:string list -> string` (was `targetMetadataDriftMarkdown`).

### Validation/state rules (preserved exactly)
- runnable target ∉ metadata ⇒ `MissingMetadata`; metadata row ∉ runnable ⇒ `MissingRunnableTarget`;
  empty `ExpectedOutputs` ⇒ `MissingExpectedOutput`; blank `FailureOwner` ⇒ `MissingFailureOwner`;
  blank prerequisite ⇒ `DependencyDivergence`.
- contract reference without metadata ⇒ `"validation contract references target without metadata: …"`;
  doc reference without metadata ⇒ `"docs reference target without metadata: …"`.
- Report markdown: `PASS:` line when no diagnostics; else `FAIL:` + `- {diagnostic}` lines.

## Capabilities module (`Capabilities.fs/.fsi`) — FR-003

### `CapabilityRow`
Moved verbatim from `build.fsx` ~37–52 (15 fields: `Id`, `DisplayName`, `PackageId`, `Project`,
`Contracts`, `Tests`, `Skill`, `TemplateFragment`, `Dependencies`, `Profiles`, `DefaultApp`,
`Evidence`, `SurfaceBaseline`, `Docs`, `NonRuntime`).

### `CapabilityCatalog`
`CapabilityRow list` (alias or wrapper) read from `template/capabilities.yml` via `YamlDotNet`
**behind** the typed model (R4), replacing the bespoke `readCapabilityCatalog` state machine.

**Functions:**
- `readCatalog : yamlPath:string -> CapabilityRow list` — YamlDotNet deserialize → project to
  `CapabilityRow` (I/O at the edge; pure projection separable for tests).
- `validateRows : surfaceBaselineExists:(string -> bool) -> capabilities:CapabilityRow list -> ValidationFinding list`
  — pure; the `File.Exists` surface-baseline check is injected so the validator is testable without
  disk (was inline at ~2356). Reports the existing typed rule ids.
- `renderReport : capabilities:CapabilityRow list -> string` — the `# Capability Catalog` PASS table
  (was the `rows` block at ~2382). Failure path reuses `Findings` rendering.

### Validation rules (preserved exactly)
default-app set must equal `{Scene;SkiaViewer;Elmish;KeyboardInput;Layout;Controls}`; per-row:
non-blank `DisplayName`; runtime row needs `Project`; non-empty `Contracts`/`Tests`/`Profiles`/
`Evidence`; present `Skill`/`TemplateFragment`; `SurfaceBaseline` present and existing (or
`no-public-surface`); every `Dependency` resolves to a known capability id.

## Test-only entities (Governance.Tests)

### `GoldenFixture` / parity (FR-006, FR-008a, R1, R2)
Committed pre-extraction snapshots under `tests/Governance.Tests/fixtures/reports-golden/`:
`capability-catalog.md`, `target-metadata.json`, `target-metadata-drift.md`. The parity test renders
each report via the library (capability/drift fully; metadata with a fixed `generatedAtUtc`) and
asserts byte-equality against the fixture — for `target-metadata.json`, every line except the
`generated_at_utc` value (asserted present + well-formed) (R2).
