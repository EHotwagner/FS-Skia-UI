# Build Workflow

The repository command surface is the FAKE target graph invoked through the
repo-local wrappers:

```bash
./fake.sh build -t Dev
./fake.sh build -t Verify
./fake.sh build -t Ci
```

On Windows command prompts, use:

```cmd
fake.cmd build -t Dev
fake.cmd build -t Verify
fake.cmd build -t Ci
```

The wrappers restore the local `fake-cli` tool from `.config/dotnet-tools.json`
and run the shared `build.fsx` target graph. Automation should call
`./fake.sh build -t Ci`.

## Targets

| Target | Responsibility | Output |
|--------|----------------|--------|
| `Clean` | Removes target-owned generated readiness outputs. | Clean logs, FSI transcripts, smoke output, and package notes. |
| `Restore` | Restores local tools and `FS-Skia-UI.sln`. | `specs/006-template-framework-governance/readiness/logs/restore.txt` |
| `Build` | Builds the solution with the repository warning policy. | `specs/006-template-framework-governance/readiness/logs/build.txt` |
| `Test` | Runs the default non-visual test set. | `specs/006-template-framework-governance/readiness/logs/test.txt` |
| `Dev` | Fast local verification: `Restore`, `Build`, `Test`. | `specs/006-template-framework-governance/readiness/logs/dev-verdict.txt` |
| `PackLocal` | Packs `FS.Skia.UI`, `FS.Skia.UI.Charts`, and `FS.Skia.UI.Layout`. | `~/.local/share/nuget-local/*.nupkg` and `readiness/logs/pack-local.txt` |
| `RefreshSurfaceBaselines` | Regenerates current package surface baselines. | `readiness/surface-baselines/*.txt` |
| `PackageSurfaceCheck` | Checks the current package surface against stable baselines. | `readiness/logs/package-surface-check.txt` |
| `FsiTranscripts` | Runs public prelude scripts through FSI. | `readiness/fsi/*.txt` under this feature |
| `SampleContractSmoke` | Runs non-visual sample `--contract-smoke` paths. | `readiness/sample-smoke/*.txt` under this feature |
| `EvidenceGraph` | Runs the Spec Kit task graph validation. | `readiness/task-graph.json` and `.md` under this feature |
| `EvidenceAudit` | Runs the synthetic evidence audit. | `readiness/logs/evidence-audit.txt` and diff-scan output |
| `Verify` | Full v1 verification requiring all v1 artifact classes. | `readiness/logs/verify-verdict.txt` |
| `Ci` | Non-interactive automation entry delegating to `Verify`. | `readiness/logs/ci-verdict.txt` |

`Verify` checks existing baselines; it does not silently refresh them. Use
`RefreshSurfaceBaselines` when an intentional public surface change has already
been reviewed.

## Deferred

The following roadmap items are outside v1 `Dev`, `Verify`, and `Ci`: template
packaging, dependency governance, generated spec/plan hardening, layout
evidence, visual evidence, package consumer smoke, and release validation.
Package consumer smoke remains available as an explicit deferred target:

```bash
./fake.sh build -t PackageSmoke
```
