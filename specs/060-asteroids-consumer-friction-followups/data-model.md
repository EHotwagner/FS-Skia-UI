# Phase 1 Data Model: Governance Entities

This feature adds governance/generation entities (compiled F# in `FS.Skia.UI.Build`).
No runtime/domain model changes. Entities are derived from existing single sources
(`template/capabilities.yml`, the packable `.fsproj` set, the skill tree) — none are
hand-authored state.

## ApiSurfaceEntry (FR-003)

Represents one emitted contract file in the generated `docs/api-surface/` tree.

| Field          | Type     | Source / Rule                                              |
|----------------|----------|------------------------------------------------------------|
| `packageId`    | string   | `capabilities[].packageId` (e.g. `FS.Skia.UI.Scene`)       |
| `pkgLeaf`      | string   | last dotted segment (`Scene`) → directory name             |
| `sourceFsi`    | path     | one of `capabilities[].contracts` (`src/Scene/Scene.fsi`)  |
| `emittedPath`  | path     | `template/base/docs/api-surface/<pkgLeaf>/<file>.fsi`      |
| `profiles`     | string[] | `capabilities[].profiles` — gates which profile emits it   |

**Validation rules**
- `emittedPath` content is **byte-identical** to `sourceFsi` (generated, currency-checked).
- Every `capabilities[].contracts` entry for an in-profile capability has a matching
  `ApiSurfaceEntry`. No orphan emitted files without a source.
- Generated only via `RefreshSurfaceBaselines`; drift fails the currency gate.

## SkillContractClaim (FR-004)

Represents a `docs/api-surface/...fsi` path a capability/product skill names as its
contract source.

| Field          | Type   | Source / Rule                                              |
|----------------|--------|------------------------------------------------------------|
| `skillPath`    | path   | `template/product-skills/*/SKILL.md` (or `src/*/skill/...`)|
| `claimedPath`  | path   | the `docs/api-surface/<Pkg>/<file>.fsi` string in the skill|
| `claimsNoReflection` | bool | whether the skill asserts "no DLL reflection needed"    |

**Validation rules**
- Every `claimedPath` MUST equal some `ApiSurfaceEntry.emittedPath` (path equality).
- A skill with `claimsNoReflection = true` MUST have its `claimedPath` exist in the
  emitted tree.
- Diagnostic on failure names the skill and the missing/extra path.

## PackableProject (FR-009 / SC-006)

Represents a repo project that produces a NuGet package.

| Field        | Type   | Source / Rule                                                |
|--------------|--------|-------------------------------------------------------------|
| `projectPath`| path   | `*.fsproj` with `<IsPackable>true</IsPackable>` / `<PackageId>` in `src/**` + `build/Governance/**` |
| `packageId`  | string | derived `FS.Skia.UI.<leaf>` (Build lives in build/Governance) |
| `feedNupkg`  | string | `FS.Skia.UI.<leaf>.<version>.nupkg` expected in local feed   |

**Validation rules**
- The packable set is currently: `FS.Skia.UI.Build`, `.Scene`, `.SkiaViewer`,
  `.Elmish`, `.KeyboardInput`, `.Input`, `.Layout`, `.Controls`, `.Controls.Elmish`,
  `.Testing`, `.SkillSupport` (11 projects).
- The `fs-skia-template-update` skill's enumerated package set MUST equal this set:
  **zero phantom** (no bare-Lib `FS.Skia.UI`), **zero missing** (`SkillSupport`,
  `Input` present). `TemplateUpdateSkillPackageCheck` diffs the two and fails on any
  asymmetry.
- Distinguish the **feed loop** enumeration (all packable, including non-pinned `Input`)
  from the **props-pin** enumeration (only template-pinned packages).

## GeneratedTestFile (FR-005)

Represents the split generated test compilation units.

| Field        | Type   | Rule                                                          |
|--------------|--------|--------------------------------------------------------------|
| `governance` | path   | `tests/Product.Tests/GovernanceTests.fs` — durable, model-agnostic source/structure/visual-evidence scans |
| `behavior`   | path   | `tests/Product.Tests/BehaviorTests.fs` — replaceable scaffold `view`/`update`/scene-text tests |

**Validation rules**
- `Product.Tests.fsproj` compiles `governance` before `behavior`.
- Swapping the scaffold model must leave `governance` compiling/running; only
  `behavior` requires rewriting (SC-003).
- Both files retain `//#if (profile == ...)` conditionals from the original `Tests.fs`.
