# Contract: Canonical Workflow

This contract defines the v1 command surface that implementation must expose. The concrete implementation is expected to be a FAKE target graph invoked through repo-local wrappers.

## Command Invocation

```bash
./fake.sh build -t <Target>
fake.cmd build -t <Target>
```

Both wrappers MUST run the same target graph. Target names are case-sensitive in documentation and should be treated as stable.

The target graph is the operator-facing command surface. Process and filesystem
effects behind that surface MUST be modeled in `build.fsx` through a local
workflow effect algebra: `BuildModel`, `BuildMsg`, `BuildEffect`, `init`, pure
`update`, and an interpreter that executes effects at the edge.

## Required Targets

| Target | Purpose | Required Inputs | Required Outputs | Pass Criteria |
|--------|---------|-----------------|------------------|---------------|
| `Clean` | Remove generated build and readiness outputs owned by the target graph. | Repository root. | Cleaned target-owned outputs. | Completes without deleting source, specs, docs, or historical readiness evidence. |
| `Restore` | Restore the .NET solution and local tools. | `FS-Skia-UI.sln`, `.config/dotnet-tools.json`. | Restore log. | Tool and solution restore succeed. |
| `Build` | Build the solution. | Restored solution. | Build log. | Solution builds with warnings-as-errors policy intact. |
| `Test` | Run default non-visual tests for v1. | Built solution and test projects. | Test log. | Required tests pass; deferred package consumer smoke is not required by this target. |
| `Dev` | Fast local verification. | `Restore`, `Build`, `Test`. | Console verdict and logs. | Completes in 10 minutes or less on a supported development machine. |
| `PackLocal` | Produce local packages for packable projects. | Built packable projects. | `.nupkg` files under `~/.local/share/nuget-local/` and package log. | All packable packages are produced. |
| `RefreshSurfaceBaselines` | Regenerate stable current package surface baselines. | Built assemblies and surface extraction script. | Updated `readiness/surface-baselines/*.txt` and refresh log. | Baseline files are written to the stable current location, not a historical feature readiness folder. |
| `PackageSurfaceCheck` | Verify stable current package surface baselines. | Built packages/assemblies and `readiness/surface-baselines/*.txt`. | Package surface status log. | Expected public contract names are exported; stale baselines fail. |
| `FsiTranscripts` | Run existing public contract prelude scripts. | Built packages/assemblies and `scripts/*prelude*.fsx`. | FSI transcript files under feature readiness. | Required scripts complete and transcripts are captured. |
| `SampleContractSmoke` | Run existing non-visual sample smoke evidence. | Sample projects with `--contract-smoke`. | Sample smoke logs under feature readiness. | Required sample smoke commands exit successfully or fail with actionable output. |
| `EvidenceGraph` | Validate task graph. | `tasks.md` and `tasks.deps.yml` for the active feature. | `readiness/task-graph.json` and `readiness/task-graph.md`. | Graph has no cycles or dangling refs. |
| `EvidenceAudit` | Run synthetic evidence audit. | Active feature directory and evidence extension. | Audit output and diff-scan reports. | PASS, or failure clearly names unresolved evidence. |
| `Verify` | Full v1 verification. | `Dev`, `PackLocal`, `PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`, `EvidenceGraph`, and `EvidenceAudit`. | Build/test/package logs, transcripts, baselines, sample smoke output, task graph output, and audit verdict. | All required v1 artifact classes exist and pass. |
| `Ci` | Non-interactive automation entry. | Same as `Verify`. | CI-compatible logs. | Calls `Verify` without duplicating command order. |

## Deferred Targets

The following names may be added later but MUST NOT be required by v1 `Dev`, `Verify`, or `Ci`:

- `PackageSmoke`
- `TemplateCheck`
- `DependencyReport`
- `LayoutEvidence`
- `Visual`
- `ReleasePack`

`RefreshSurfaceBaselines` is a v1 maintenance target, but `Verify` MUST check baselines rather than silently refreshing them.

## Artifact Path Contract

| Artifact Class | Stable Path |
|----------------|-------------|
| Package surface baselines | `readiness/surface-baselines/*.txt` |
| Feature build/test/package logs | `specs/006-template-framework-governance/readiness/logs/*.txt` |
| FSI transcripts | `specs/006-template-framework-governance/readiness/fsi/*.txt` |
| Sample smoke output | `specs/006-template-framework-governance/readiness/sample-smoke/*.txt` |
| Task graph output | `specs/006-template-framework-governance/readiness/task-graph.json` and `.md` |
| Evidence audit output | `specs/006-template-framework-governance/readiness/` audit files produced by the evidence extension |
| Local packages | `~/.local/share/nuget-local/*.nupkg` |

## Automation Contract

- Any repository automation touched in v1 MUST invoke `Ci`, `Verify`, or a named target instead of duplicating restore/build/test/package/evidence command order.
- If no CI workflow exists, `docs/build.md` MUST state that future CI should call `./fake.sh build -t Ci`.
- Generated task guidance updated in v1 MUST instruct tasks to run canonical targets, not raw command sequences.
