# Phase 1 Data Model — V3 Stage 5 Closeout

This stage deletes code and adds governance enforcement; it introduces **no new
runtime domain types**. The "entities" here are governance/config records and the
contract artifacts they drive.

## Governance entities (existing — touched this stage)

### RoutingRule (existing, `build/Governance/Routing.fs`)
```
type RoutingRule =
  { Id: string
    Paths: string list              // glob patterns matched against the diff
    Matches: Diff -> bool
    Tier: Tier
    RequiredGates: Targets.Target list
    ExpectedArtifacts: string list
    TimeoutClass: string
    FailureOwner: string }
```
- **Change:** the `package-surface` rule (Id `"package-surface"`, Paths
  `["src/**/*.fsi"; "readiness/surface-baselines/**"]`, Tier `FocusedAuthority`) gains
  `Targets.PerPackageSurfaceDiff` in `RequiredGates`.
- **Invariant:** every gate in any rule's `RequiredGates` MUST be on the `knownGates`
  allowlist; `validation.contract.yml` MUST stay current vs `Routing.fs`
  (`TargetMetadataDrift`).

### knownGates (existing, `build/Governance/AgentValidation.fs`)
- A `string list` allowlist of recognised gate names (16 entries today).
- **Change:** add `"PerPackageSurfaceDiff"`.
- **Validation rule:** the contract validator rejects a `required_gates` entry whose
  name is not in `knownGates`; adding the routing rule without this entry fails the
  contract.

### packProjects (existing, `build/Governance/Front/Helpers.fs`)
- `(projectPath * packageId) list` consumed by `PackLocal` and the version-bump flow.
- **Change:** remove `("src/Lib/Lib.fsproj", "FS.Skia.UI")`. Resulting list = the nine
  split packages + `FS.Skia.UI.Build` (10 entries).
- **Invariant:** every path in the list MUST exist on disk (the deleted `Lib.fsproj`
  would otherwise fail pack); `Package.Tests` asserts the list shape (rewritten).

### packagesInScope (existing, `build/Governance/PerPackageSurface.fs`)
- The nine in-scope split-package ids; the monolith is excluded by construction.
- **Change:** none to the list; only the stale comment (`:29`) is corrected to stop
  describing the monolith as "retiring" (it is retired).

## Contract artifacts (generated / authored this stage)

### validation.contract.yml (regenerated)
- Rendered from `Routing.fs`. After the rule change its `package-surface`
  `required_gates` includes `PerPackageSurfaceDiff`. Regenerated, not hand-edited.

### Per-package surface baselines (`readiness/per-package-surface/<PackageId>.fsi.txt`)
- Nine files, one per in-scope package. **Unchanged** this stage (no runtime `.fsi`
  moves). The enforcement proof drifts exactly one of them temporarily (reverted).

### Library.fsi public types (deleted)
- `ParityStatus`, `EvidenceType`, `ParityEvidenceItem`, `ParityReport`, and the
  `Parity` module — **removed** with `src/Lib`. The aggregate `PackageSurfaceCheck`
  baseline sheds the corresponding `FS.Skia.UI.*` lines (recorded).

## Document artifacts (authored this stage)

| Artifact | Path | Required fields |
|---|---|---|
| After-baseline | `docs/reports/_baselines/2026-06-02-v3-after.md` | pin SHA; `src/Lib` LOC→0; monolith transitive-pull→none; duplicate-type count→0; package count; per-package baselines present; generated-`app` cleanliness asserted; **each with reproduction command** |
| V2→V3 migration | `docs/migration/v2-to-v3.md` | surface map (`FS.Skia.UI` → `.Scene`/`.SkiaViewer`/`.Elmish`/`.KeyboardInput`/`.Input`/`.Layout`/`.Controls`); package-reference move steps; removed-`SceneConversion` note; rich keyboard-input → `FS.Skia.UI.Input` mapping |
| ADR 0012 | `docs/adr/0012-monolith-retirement-closeout.md` | status Accepted; records completed retirement; links ADRs 0007–0011 |
| No-consumer grep proof | `specs/053-v3-monolith-retirement/readiness/…` | zero `Lib`/`FS.Skia.UI`-monolith refs across `src/** samples/** tests/** template/** build/**`; the grep command + its empty output |

## State transitions

The only "state machine" is the **enforcement proof** for `PerPackageSurfaceDiff`:

```
baseline recorded  --(edit one package .fsi, no baseline update)-->  PerPackageSurfaceDiff FAILS
PerPackageSurfaceDiff FAILS  --(update that package's baseline)-->   PASS
PASS  --(revert both edit + baseline)-->  baseline recorded (clean)
```
This is real evidence (SC-004), not synthetic.
