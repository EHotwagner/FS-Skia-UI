# Build Workflow

The repository command surface is the FAKE target graph invoked through the
repo-local wrappers:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t Verify`
3. `./fake.sh build -t Ci`

On Windows command prompts, use:

1. `fake.cmd build -t Dev`
2. `fake.cmd build -t Verify`
3. `fake.cmd build -t Ci`

FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`) share
repository `.fake` state and are not safe to run concurrently. When a workflow
needs more than one FAKE-backed target, run one command to completion before
starting the next and preserve that order in readiness evidence. Non-FAKE
checks may still run in parallel when they do not invoke FAKE or depend on
`.fake`.

The wrappers restore the local `fake-cli` tool from `.config/dotnet-tools.json`
and run the shared `build.fsx` target graph. Automation should call
`./fake.sh build -t Ci`.
Targets are registered through FAKE's native target graph. `./fake.sh build
--list` is the supported discovery surface for runnable target names, and
`TargetMetadataDrift` verifies that discovered targets, metadata, docs, and
`validation.contract.yml` stay aligned.

## Targets

| Target | Responsibility | Output |
|--------|----------------|--------|
| `Clean` | Removes target-owned generated readiness outputs. | Clean logs, FSI transcripts, smoke output, and package notes. |
| `Restore` | Restores local tools and `FS-Skia-UI.sln`. | Active feature `readiness/logs/restore.txt` |
| `Build` | Builds the solution with the repository warning policy. | Active feature `readiness/logs/build.txt` |
| `Test` | Runs the default non-visual test set. | Active feature `readiness/logs/test.txt` |
| `Dev` | Fast local verification: `Restore`, `Build`, `Test`. | Active feature `readiness/logs/dev-verdict.txt` |
| `PackLocal` | Packs the current public capability packages: Scene, SkiaViewer, Elmish, KeyboardInput, Controls.Elmish, Testing, the compatibility core package, Layout, and Controls; writes generated-consumer package snippets and setup-drift guidance. | `~/.local/share/nuget-local/*.nupkg`, active feature `readiness/package/local-packages.md`, and the task-specific PackLocal log under `readiness/logs/` |
| `RefreshSurfaceBaselines` | Regenerates current package surface baselines. | `readiness/surface-baselines/*.txt` |
| `PackageSurfaceCheck` | Checks the current package surface against stable baselines. | `readiness/logs/package-surface-check.txt` |
| `FsiTranscripts` | Runs public prelude scripts through FSI. | `readiness/fsi/*.txt` under this feature |
| `SampleContractSmoke` | Runs non-visual sample `--contract-smoke` paths. | `readiness/sample-smoke/*.txt` under this feature |
| `EvidenceGraph` | Runs Spec Kit task graph and task `skillist` metadata validation. | `readiness/task-graph.json` and `.md` under this feature |
| `EvidenceAudit` | Runs the synthetic evidence audit after a valid task graph. | `readiness/logs/evidence-audit.txt` and diff-scan output |
| `TargetMetadata` | Writes machine-readable target metadata for external tooling and reviewers. | Active feature `readiness/target-metadata.json` |
| `TargetMetadataDrift` | Validates native FAKE targets, metadata, docs, and validation contract references. | Active feature `readiness/target-metadata-drift.md` |
| `VerifyPreflight` | Records process-health and bootstrap evidence for `Verify` before broad work starts. | Active feature `readiness/process-health.md`, `readiness/bootstrap-runner.md`, and `readiness/verification-verdicts.md` |
| `CiPreflight` | Records process-health and bootstrap evidence for `Ci` before delegating to `Verify`. | Active feature `readiness/process-health.md`, `readiness/bootstrap-runner.md`, and `readiness/verification-verdicts.md` |
| `StaleBoundaryScan` | Records stale active ownership scan status for removed package/boundary evidence. | Active feature `readiness/stale-boundary-scan.md` |
| `FinalReadiness` | Summarizes final readiness status and broad aggregate authority. | Active feature `readiness/evidence-audit.md` |
| `Verify` | Full repository verification requiring process-health preflight, bootstrap, package, template, generated-product, guidance, drift, dependency, and evidence audit artifact classes. | `readiness/logs/verify-verdict.txt` and `readiness/verification-verdicts.md` |
| `Ci` | Non-interactive automation entry running `CiPreflight` before delegating to `Verify`. | `readiness/logs/ci-verdict.txt` and `readiness/verification-verdicts.md` |

## Template And Governance Targets

| Target | Responsibility | Output |
|--------|----------------|--------|
| `TemplatePack` | Builds `FS.Skia.UI.Template.*.nupkg` from `.template.package/FS.Skia.UI.Template.fsproj`. | `artifacts/templates/` and active feature `readiness/template/template-pack.log` |
| `TemplateInstallSource` | Installs the template from the source directory. | Active feature `readiness/template/source-install.log` |
| `TemplateInstallPackage` | Installs the local packaged template artifact. | Active feature `readiness/template/package-install.log` |
| `TemplateInstantiate` | Creates source and package generated projects for the V3 `app`, `headless-scene`, `governed`, and `sample-pack` profiles. | `artifacts/template-check/<active-feature>/` |
| `TemplateSmoke` | Scans generated projects and runs their `./fake.sh build -t Dev` workflow. | Active feature `readiness/template/generated-project-scans.md` |
| `TemplateCheck` | Requires the full template validation artifact class. | Active feature `readiness/template/verdict.md` |
| `CapabilityCheck` | Validates `template/capabilities.yml`, capability ownership metadata, package contracts, tests, skills, fragments, evidence classes, and default app membership. | Active feature `readiness/capability-catalog.md` |
| `SkillCheck` | Validates package-owned local skills and selected default app skill destinations. | Active feature `readiness/selected-skills.md` |
| `GeneratedProductCheck` | Generates and verifies the V3 app, packaged app, headless-scene, governed, and sample-pack product rows; for generated graphical consumers it also restores from local packages, runs semantic tests, bounded smoke, and scene evidence. | Active feature `readiness/generated-file-lists/summary.md`, `readiness/generated-product-verify/**`, and `readiness/generated-product-validation.md` |
| `DependencyReport` | Verifies Central Package Management and dependency metadata. | Active feature `readiness/dependencies.md` |
| `GeneratedGuidanceCheck` | Verifies active and preset-owned spec/plan prompts, task `skillist` templates, task-generation guidance, implementation skill-loading guidance, constitution guidance, deferred-scope boundaries, and active/preset parity. | Active feature `readiness/generated-guidance.md` |
| `TemplateDrift` | Classifies template-owned path changes and checks required alignment classes plus active feature evidence or accepted deferrals. | Active feature `readiness/template-drift.md` |

## Generated Graphical Integration

Feature `013-tetris-demo-integration` adds contracted graphical integration
evidence before story implementation may be declared complete. The build
surface may extend `GeneratedProductCheck`, `GeneratedGuidanceCheck`,
`TemplateCheck`, `TemplateDrift`, `PackLocal`, `Verify`, and `Ci` so generated
apps can prove normalized viewer input, bounded real-viewer smoke,
deterministic scene evidence, local package setup, and unsupported-host
diagnostics.

The generated graphical app command surface is:

```bash
dotnet run --project src/Product/Product.fsproj
dotnet run --project src/Product/Product.fsproj -- --bounded-smoke <path>
dotnet run --project src/Product/Product.fsproj -- --scene-evidence <path>
```

Bounded real-viewer evidence and deterministic scene-level evidence are
separate artifact classes. A headless scene hash or PNG does not replace
first-frame viewer startup evidence; an unsupported desktop host must produce
an explicit unsupported-environment diagnostic.

Persistent generated graphical launch evidence uses the default executable
path and reports `mode=persistent-window`. Bounded smoke, first-frame,
frame-count, scene metadata, and unsupported-host diagnostics remain explicit
helper evidence and do not substitute for supported-host persistent launch
readiness.

`Dev` remains the fast local restore/build/test path and is independent of
template packaging. `Verify` includes source tests, local package packing,
package surface checks, public FSI transcripts, sample contract smoke,
template validation, capability validation, selected skill validation,
generated product validation, dependency governance, generated guidance,
template drift, and the evidence audit. `VerifyPreflight` and `CiPreflight`
run process-health and bootstrap checks before high-pressure broad work. A
preflight or bootstrap failure is an `environment-failure`: it fails the broad
aggregate, records non-authoritative product evidence, and recommends rerunning
in a fresh shell, fresh container, or CI runner. `Ci` runs its own preflight and
then delegates to `Verify`.

If a FAKE-backed command fails with a race-like or unexplained setup symptom,
record whether another FAKE-backed command was running. When the concurrent
context is suspected or unknown, rerun the affected FAKE-backed commands
sequentially before classifying the result as a product regression.

Focused gates remain direct entry points. Each focused gate writes or appends a
row to `readiness/focused-gates.md` with its direct prerequisites, command, log
path, readiness path, verdict category, and stale build/restore assumption
diagnostics. Targets that intentionally use `--no-restore` or `--no-build`
must name the affected gate and remediation command when the assumed artifact
is stale.

## Focused Gate Matrix

| Gate | Direct prerequisites | Primary output |
|------|----------------------|----------------|
| `PackageSurfaceCheck` | none | `readiness/logs/package-surface-check.txt` and package surface report |
| `FsiTranscripts` | none | `readiness/fsi/*.txt` |
| `ControlsCatalogCheck` | restored `tests/Controls.Tests` assets | `readiness/control-catalog.md` |
| `ControlsInteractionCheck` | restored `tests/Controls.Tests` assets | `readiness/interaction-tests.md` |
| `ControlsRenderingCheck` | restored `tests/Controls.Tests` assets | `readiness/layout-rendering.md` |
| `DependencyReport` | none | `readiness/dependency-report.md` and dependency log |
| `TemplateCheck` | template pack/install/instantiate/smoke targets | `readiness/template/verdict.md` |
| `GeneratedProductCheck` | `CapabilityCheck`, `SkillCheck` | `readiness/generated-file-lists/summary.md` |
| `GeneratedGuidanceCheck` | none | `readiness/generated-guidance.md` |
| `TemplateDrift` | none | `readiness/template-drift.md` |
| `TargetMetadataDrift` | `TargetMetadata` | `readiness/target-metadata.json` and `readiness/target-metadata-drift.md` |
| `EvidenceGraph` | none | `readiness/task-graph.md` and `.json`, including task `skillist` diagnostics |
| `EvidenceAudit` | `EvidenceGraph` | `readiness/logs/evidence-audit.txt` |

No focused gate may depend on `Verify` or `Ci`. Adding a broad prerequisite is
a command-contract change and must be documented with a test before it lands.

`Verify` checks existing baselines; it does not silently refresh them. Use
`RefreshSurfaceBaselines` when an intentional public surface change has already
been reviewed.

## Deferred

The following roadmap items remain outside repository pass/fail: full visual evidence,
release validation, an external template repository split, and broader
distribution automation.
Package consumer smoke remains available as an explicit deferred target:

```bash
./fake.sh build -t PackageSmoke
```
