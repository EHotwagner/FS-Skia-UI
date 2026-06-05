# Contract: Catalog Generation (build front, `FS.Skia.UI.Build`)

Build-tooling scope (`build/Governance`, `net10.0`). **Not** part of the tracked
runtime surface baselines (same status as `SkillTreeGen.fsi`). Pure generation +
currency over in-memory text; all filesystem read/write stays at the
`Engine/Interpret.fs` edge (Principle IV).

## 1. `CatalogGen` module — `build/Governance/CatalogGen.fsi`

```fsharp
module FS.Skia.UI.Build.CatalogGen

/// The single-source catalog-relevant facts for one 065 typed control.
type TypedCatalogFact =
    { Id: string
      DisplayName: string
      Category: string
      Module: string
      Purpose: string
      RequiredAttributes: string list
      Events: string list
      AccessibilityRole: string }

/// Status of one generated region relative to a fresh render.
type RegionStatus = Current | Stale | Missing

/// Currency result for one (control, file) region.
type CatalogCurrency =
    { ControlId: string
      FilePath: string
      Status: RegionStatus }

/// The authoritative six-control fact table — the single source (FR-001).
/// Covers exactly { text-block, button, check-box, stack, text-box, data-grid }.
val catalogFacts: TypedCatalogFact list

/// Repo-relative home files the six rows are spliced into.
val catalogYmlRel: string      // "src/Controls/catalog.yml"
val catalogFsRel: string       // "src/Controls/Catalog.fs"

/// Render one fact to its on-disk row text for each target (byte-identical to
/// today's hand-authored row — FR-004). Pure.
val renderFSharpRow: fact: TypedCatalogFact -> string
val renderYamlRow: fact: TypedCatalogFact -> string

/// Splice every fact's rendered row into its `typed-catalog/<id>` marked region
/// in the given file text, leaving everything outside the markers untouched
/// (FR-003). Pure.
val spliceFSharp: fileText: string -> string
val spliceYaml: fileText: string -> string

/// Compare each on-disk region (both files) against a fresh render. Clean iff
/// every returned status is Current. Pure.
val currency: catalogYmlText: string -> catalogFsText: string -> CatalogCurrency list

/// True when every region is current.
val isCurrent: currency: CatalogCurrency list -> bool

/// Actionable drift diagnostics — one per stale/missing region, naming the
/// divergent control, the file, and `./fake.sh build -t RefreshSurfaceBaselines`
/// (FR-005). Empty when current.
val currencyDrift: currency: CatalogCurrency list -> string list
```

### Marker contract (per-control, non-contiguous rows — R2)

- `catalog.yml` (YAML `#` comments):
  `# BEGIN GENERATED: typed-catalog/<id>` … row … `# END GENERATED: typed-catalog/<id>`
- `Catalog.fs` (F# `//` comments):
  `// BEGIN GENERATED: typed-catalog/<id>` … `definition …` row … `// END GENERATED: typed-catalog/<id>`

The 41 hand-authored rows carry **no** markers and are never read or written by
generation. Regeneration is deterministic: stable ordering (rows stay in place)
and stable formatting (no incidental whitespace churn — edge case "Ordering /
formatting churn").

## 2. New gate: `ControlsCatalogGenerationCheck`

- **Registration** (`build/Governance/Targets.fs`): add the enum case; add to
  `allTargets`; `name = "ControlsCatalogGenerationCheck"`;
  `directPrerequisites = []`; `timeoutClass = "focused"`; `cost = "low"`;
  `failureOwner = "product"`.
- **Allowlist** (`AgentValidation.fs`): add `"ControlsCatalogGenerationCheck"` to
  `ValidationContract.knownGates`.
- **Behavior** (`Engine/Update.fs` gate arm): read both catalog files at the edge;
  call `CatalogGen.currency`; `WriteStructuredReport` a PASS/FAIL readiness report;
  `FailWith (currencyDrift …)` listing the divergent control(s) when stale/missing;
  emit the focused-gate assumption check + summary like sibling gates.
- **PASS report** states it is a current, byte-identical regeneration of the six
  typed rows from `CatalogGen.catalogFacts`, naming the count (6) and both files.

## 3. Routing delta — `controls-public-surface`

Add `Targets.ControlsCatalogGenerationCheck` to the rule's `RequiredGates`
(`Routing.fs`, the `src/Controls/**` rule). Result:

```
./fake.sh build -t Route   # over a src/Controls/** (registry) diff
→ tier: FocusedAuthority
→ gates: ControlsCatalogCheck, ControlsInteractionCheck, ControlsRenderingCheck,
         ControlsCatalogGenerationCheck,           # ← new, listed (FR-006/US3/SC-004)
         PackageSurfaceCheck, FsiTranscripts, GeneratedProductCheck
```

`validation.contract.yml` is regenerated from `Routing.fs` (via
`RefreshSurfaceBaselines` → `ContractView.render`) so its
`controls-public-surface.required_gates` includes the new gate; its currency is
enforced by `TargetMetadataDrift` (so the contract can never silently omit it).
`Route --enforce` blocks on a stale generated catalog as a failed obligation.

## 4. Regeneration — `RegenerateCatalog` effect

- **Effect** (`Engine/Interpret.fs(/.fsi)`): add `RegenerateCatalog`; the handler
  reads both files, applies `CatalogGen.spliceYaml`/`spliceFSharp`, and writes both
  back in one operation (FR-002; partial-regeneration edge case cannot occur).
- **Wiring** (`Engine/Update.fs`): emit `RegenerateCatalog` from the
  `RefreshSurfaceBaselines` arm, alongside `RegenerateGovernedBlocks` etc., so one
  `./fake.sh build -t RefreshSurfaceBaselines` makes the catalog current.

## 5. Compile order — `FS.Skia.UI.Build.fsproj`

Insert `CatalogGen.fsi` then `CatalogGen.fs` after `GovernedBlocks.fs` (CatalogGen
may reuse `GovernedBlocks` splice/currency primitives) and before the engine files
that reference it.
