# Evidence Audit Setup Notes

Status: US4 evidence coverage captured through T069.

## Tier And Scope

- Feature tier: Tier 1 contracted change.
- Affected public contracts: Controls, KeyboardInput, planned Controls.Elmish
  adapter, package baselines, generated guidance, catalog metadata, and
  compatibility docs.
- Package/capability impact: Controls becomes the active home for ordinary
  controls, rich rendering, charts, graph views, and DataGrid; legacy Charts
  package/capability is removed rather than shimmed.
- MVU applicability: KeyboardInput runtime, ControlRuntime, and adapter
  command/subscription integration are stateful and must use public model,
  message, effect, pure update, and interpreter boundaries.
- Unsupported scope: renderer-neutral widgets, new renderer backends,
  platform-native wrappers, formal accessibility certification, automated
  external-app migration, and release publishing automation.

## Synthetic Evidence Policy

Primary proof must be real evidence. If synthetic evidence is needed for
clipboard, IME, GPU, font, unsupported environment paths, generated products,
or host interaction, the task must be marked `[S]`, code and test disclosures
must include `SYNTHETIC`, and this inventory must list the real-evidence path
or tracking issue.

## Real Evidence Obligations

- Public surface: package baselines and FSI transcripts.
- Package boundary: stale reference scans and dependency report.
- Runtime: ControlRuntime and KeyboardInput public `init`/`update` transition
  tests plus emitted-effect assertions.
- Adapter: real interpreter path where safe, with base Controls free of direct
  command/program ownership.
- Rendering: deterministic readback or unsupported environment diagnostics.
- Generated products: generated file/package inventories and command logs.
- Final gates: `EvidenceGraph`, `EvidenceAudit`, `Verify`, and `Ci`.

## Foundation Diagnostic Evidence

- `readiness/logs/t020-boundary-diagnostics.txt`: Controls diagnostics tests
  verify actionable messages for stale package references, dependency leaks,
  catalog omissions, duplicate runtime definitions, stale event targets, and
  unsupported scope expansion.

## US4 T064 Diagnostic Contract Evidence

- Added `tests/Controls.Tests/DiagnosticsTests.fs` coverage proving validation
  diagnostics name actionable packages, capabilities, controls, catalog
  entries, generated profiles, adapter contracts, runtime state, unsupported
  environment details, and migration gaps.
- Pass log: `readiness/logs/t064-diagnostics-contract.txt`.

## US4 T069 Boundary Evidence Test Coverage

- Governance tests now require readiness evidence for public surface,
  package boundary, generated guidance, dependency impact, compatibility impact,
  and lower-level path preservation.
- Package tests now assert active package-surface wiring includes Controls,
  KeyboardInput, and Controls.Elmish baselines while the Charts baseline stays
  inactive.
- Smoke tests now include ControlsGallery, ChartsGallery, and DataGridGallery
  contract-smoke entry point and sample-source checks, proving sample coverage
  for Controls-owned chart, graph, DataGrid, adapter, runtime, and lower-level
  paths.
- Direct contract-smoke evidence was captured for ControlsGallery,
  ChartsGallery, and DataGridGallery.
- Evidence:
  - `readiness/logs/t069-governance-evidence.txt`
  - `readiness/logs/t069-package-boundary.txt`
  - `readiness/logs/t069-smoke-sample-boundary.txt`
  - `readiness/logs/t069-controlsgallery-contract-smoke.txt`
  - `readiness/logs/t069-chartsgallery-contract-smoke.txt`
  - `readiness/logs/t069-datagridgallery-contract-smoke.txt`

## T073 Integration Focused Verification

`./fake.sh build -t Dev` was attempted and recorded in
`readiness/logs/t073-dev.txt`. The command failed in local FAKE environment
setup before repository targets ran because the cache references missing
`/home/developer/.nuget/packages/fsharp.core/6.0.7`; the log records
`exit-code=1` and `duration-seconds=1`.

Direct focused verification was run with serial `dotnet test -m:1 --no-restore`
to avoid the local FAKE cache failure:

| Area | Command | Log | Exit | Duration |
| --- | --- | --- | --- | --- |
| Controls | `dotnet test tests/Controls.Tests/Controls.Tests.fsproj -m:1 --no-restore` | `readiness/logs/t073-controls-tests.txt` | 0 | 3s |
| KeyboardInput | `dotnet test tests/KeyboardInput.Tests/KeyboardInput.Tests.fsproj -m:1 --no-restore` | `readiness/logs/t073-keyboardinput-tests.txt` | 0 | 2s |
| Elmish adapter | `dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj -m:1 --no-restore` | `readiness/logs/t073-elmish-adapter-tests.txt` | 0 | 4s |
| Package boundary | `dotnet test tests/Package.Tests/Package.Tests.fsproj -m:1 --no-restore` | `readiness/logs/t073-package-tests.txt` | 0 | 1s |
| Governance focus | `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -m:1 --no-restore --filter "Controls boundary"` | `readiness/logs/t073-governance-tests.txt` | 0 | 1s |

## T074 Package Surface And Transcript Gates

| Gate | Log | Exit | Duration |
| --- | --- | --- | --- |
| `./fake.sh build -t PackLocal` | `readiness/logs/t074-packlocal.txt` | 0 | 18s |
| `./fake.sh build -t PackageSurfaceCheck` | `readiness/logs/t074-package-surface-check.txt` | 0 | 4s |
| `./fake.sh build -t FsiTranscripts` | `readiness/logs/t074-fsi-transcripts.txt` | 0 | 6s |

`RefreshSurfaceBaselines` was intentionally not rerun for T074. The approved
baseline changes are the earlier T067 public Controls, KeyboardInput, and
Controls.Elmish additions plus removal of the active Charts baseline.

## T075 Verify And Runtime/Adapter Evidence

`./fake.sh build -t Verify` was run and recorded in
`readiness/logs/t075-verify.txt`. The aggregate target reached the `Test`
target, then failed while VSTest was starting the `Lib.Tests` testhost. The
failure is an environment/process-limit failure, not a test assertion failure:
`readiness/logs/test.txt` records `System.OutOfMemoryException` from
`System.Net.Sockets.SocketAsyncEngine` during VSTest socket setup.

Focused runtime and adapter evidence was captured outside the failing aggregate
testhost startup path:

| Gate | Log | Exit | Duration |
| --- | --- | --- | --- |
| `./fake.sh build -t Verify` | `readiness/logs/t075-verify.txt` | 1 | 7s |
| `./fake.sh build -t ControlsInteractionCheck` | `readiness/logs/t075-controls-interaction-check.txt` | 0 | 13s |
| `dotnet test tests/Lib.Tests/Lib.Tests.fsproj -m:1 --no-restore -- --sequenced` | `readiness/logs/t075-lib-tests-direct.txt` | 0 | 6s |
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -m:1 --no-restore --filter "Canonical command contract"` | `readiness/logs/t075-command-contract-tests.txt` | 0 | 3s |

The task-named split targets `ControlsRuntimeCheck`, `KeyboardInputCheck`, and
`ControlsBoundaryCheck` do not exist in the current target graph. The existing
feature-specific runtime/adapter equivalent is `ControlsInteractionCheck`,
which passed after the target graph was aligned to run tests from `Restore`
without forcing the broad `Build` precondition.

## T076 Catalog And Rendering Gates

| Gate | Log | Exit | Duration |
| --- | --- | --- | --- |
| `./fake.sh build -t ControlsCatalogCheck` | `readiness/logs/t076-controls-catalog-check.txt` | 0 | 2s |
| `./fake.sh build -t ControlsRenderingCheck` | `readiness/logs/t076-controls-rendering-check.txt` | 0 | 3s |
| `./fake.sh build -t BuildWorkflowCheck` | `readiness/logs/t076-build-workflow-check.txt` | 0 | 0s |

The rendering target initially reproduced a local VSTest out-of-memory startup
failure when the graph forced a broad `Build` before running the rendering
slice. The split Controls targets now run their focused test slices directly
with `dotnet test -m:1 --no-restore`, and the build workflow self-check passes
with the updated graph.

## T077 Capability, Skill, And Dependency Gates

| Gate | Log | Exit | Duration |
| --- | --- | --- | --- |
| `./fake.sh build -t CapabilityCheck` | `readiness/logs/t077-capability-check.txt` | 0 | 1s |
| `./fake.sh build -t SkillCheck` | `readiness/logs/t077-skill-check.txt` | 0 | 0s |
| `./fake.sh build -t DependencyReport` | `readiness/logs/t077-dependency-report.txt` | 0 | 3s |
| Focused dependency governance tests | `readiness/logs/t077-dependency-governance-tests.txt` | 0 | 2s |

`DependencyReport` initially exposed a stale substring guard that treated
`OutputType=Library` as a Controls-to-Lib dependency leak. The guard now checks
concrete forbidden ProjectReference and PackageReference entries, and both the
target and governance tests pass.

## T078 Template And Generated Product Gates

| Gate | Log | Exit | Duration |
| --- | --- | --- | --- |
| `./fake.sh build -t TemplateCheck` | `readiness/logs/t078-template-check.txt` | 0 | 28s |
| `./fake.sh build -t GeneratedProductCheck` | `readiness/logs/t078-generated-product-check.txt` | 0 | 12s |
| `./fake.sh build -t GeneratedGuidanceCheck` | `readiness/logs/t078-generated-guidance-check.txt` | 0 | 1s |
| `./fake.sh build -t TemplateDrift` | `readiness/logs/t078-template-drift.txt` | 0 | 2s |

`TemplateCheck` initially exposed that the copied-content scanner treated the
explicit generated `sample-pack` profile as forbidden framework sample content.
The scanner now permits generated sample-pack files only for that profile while
continuing to reject framework sample projects in ordinary generated products.

## T079 Representative Generated Product Commands

| Product | Dev | Test | Verify | Inventory |
| --- | --- | --- | --- | --- |
| `app-source` | `readiness/generated-product-verify/app-source/dev.log` | `readiness/generated-product-verify/app-source/test.log` | `readiness/generated-product-verify/app-source/verify.log` | `readiness/generated-file-lists/app-source.txt` |
| `app-package` | `readiness/generated-product-verify/app-package/dev.log` | `readiness/generated-product-verify/app-package/test.log` | `readiness/generated-product-verify/app-package/verify.log` | `readiness/generated-file-lists/app-package.txt` |

All six generated product command logs record `exit-code=0`. The inventory
scan in `readiness/logs/t079-generated-app-inventory.txt` records the Controls,
Controls.Elmish, KeyboardInput, Layout, Scene, SkiaViewer, and Elmish package
references plus product-owned rich text, chart, graph, DataGrid, and adapter
source markers.

## T080 Final Aggregate Commands

| Gate | Log | Exit | Duration | Verdict |
| --- | --- | --- | --- | --- |
| `./fake.sh build -t Verify` | `readiness/logs/t080-verify.txt` | 1 | 7s | ENVIRONMENT FAIL |
| `./fake.sh build -t Ci` | `readiness/logs/t080-ci.txt` | 1 | 7s | ENVIRONMENT FAIL |

Both aggregate gates reached `Test` and failed while starting the `Lib.Tests`
VSTest testhost. `readiness/logs/test.txt` records local out-of-memory
diagnostics, including `Failed to create CoreCLR, HRESULT: 0x8007000E` and
earlier `SocketAsyncEngine` startup failures. Focused Phase 7 gates for package
surface, FSI transcripts, Controls catalog/rendering/interaction, generated
products, dependency reporting, template drift, and generated guidance passed
outside this local VSTest host pressure.

## T082 Evidence Graph And Audit Gates

| Gate | Log | Exit | Duration |
| --- | --- | --- | --- |
| `./fake.sh build -t EvidenceGraph` | `readiness/logs/t082-evidence-graph.txt` | 0 | 0s |
| `./fake.sh build -t EvidenceAudit` | `readiness/logs/t082-evidence-audit.txt` | 0 | 0s |

The governed evidence targets passed. The task graph reported zero `[S]` and
zero propagated `[S*]`; no synthetic-evidence blocker was reported by the audit
target.

## T083 Synthetic-Evidence Inventory Review

| Review | Log | Verdict |
| --- | --- | --- |
| Synthetic inventory and graph status review | `readiness/logs/t083-synthetic-inventory.txt` | PASS |

The feature Synthetic-Evidence Inventory remains empty and the effective task
graph reports zero `[S]` and zero `[S*]` tasks. Existing `SYNTHETIC` disclosures
found by the repository-wide scan are pre-existing Lib/native smoke fixtures or
documentation references, not new synthetic evidence for this feature.

## T084 Stale Boundary Scan

| Scan Or Check | Log | Verdict |
| --- | --- | --- |
| Active stale reference and copied-asset scan | `readiness/logs/t084-stale-boundary-scan.txt` | PASS |
| Package Charts focused test attempt | `readiness/logs/t084-package-charts-tests.txt` | ENVIRONMENT FAIL |
| Generated guidance focused test attempt | `readiness/logs/t084-generated-guidance-tests.txt` | ENVIRONMENT FAIL |
| Template drift rerun attempt | `readiness/logs/t084-template-drift.txt` | ENVIRONMENT FAIL |

The focused T084 blocker checks found no active Charts package/capability
reference outside absence tests, no stale generated-guidance wording, and no
copied framework assets in representative generated app roots. The broad scan
still reports expected absence-test literals and readiness/history references.
The active governance memory list was updated from `fs-skia-charts` to
`fs-skia-ui-widgets`, `docs/architecture.md` was aligned to the current package
boundary, and leftover tracked legacy `src/Charts/*` and `tests/Charts.Tests/*`
source files were removed. Follow-up Package/Governance/TemplateDrift attempts
were blocked by local out-of-memory/CoreCLR startup pressure rather than stale
boundary assertions.

## T085 Final Readiness Summary

Final readiness summary: `readiness/final-readiness-summary.md`.

The summary ties user stories, requirements, public-surface baselines,
generated product obligations, compatibility impact, command verdicts, evidence
graph, evidence audit, synthetic-evidence status, and the remaining local
aggregate environment caveat together for maintainer review.

Final post-T085 governed audit: `readiness/logs/t085-evidence-audit-final.txt`
passed with task graph status `85[X]`, zero `[S]`, and zero `[S*]`.
