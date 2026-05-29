# Validation Contract Evidence

Status: PASS for US1 routing selection evidence.

## Public Entry Point

The public FSI transcript `readiness/logs/t024-validation-interpreter-fsi.txt`
drives `FS.Skia.UI.AgentValidation.ValidationSelectionInterpreter` through the
real repository path:

- active feature metadata read from `.specify/feature.json`
- explicit fallback to real `git merge-base master HEAD` because active
  metadata does not yet include changed paths
- real `validation.contract.yml` load and parse
- public rule selection through `ValidationSelection.update`
- readiness report write to `readiness/validation-selection-report.md`

Observed result:

- changed-path source: `GitMergeBaseDiff`
- selected rules: `controls-public-surface`, `generated-template`,
  `evidence-governance`, `generated-guidance`, `docs-only`,
  `package-surface`, `build-target-contract`
- required gates: `ControlsCatalogCheck`, `ControlsInteractionCheck`,
  `ControlsRenderingCheck`, `PackageSurfaceCheck`, `FsiTranscripts`,
  `GeneratedProductCheck`, `TemplateCheck`, `EvidenceGraph`,
  `EvidenceAudit`, `GeneratedGuidanceCheck`, `TemplateDrift`, `AgentReady`,
  `Verify`, `Ci`
- authority: `AgentReadyAuthority`
- degraded: `False`

The metadata fallback diagnostic is expected for this repository state:
`.specify/feature.json` identifies the active feature directory but does not
yet carry a changed-path list.

## Representative Scenarios

| Scenario | Representative changed path | Selected rule | Focused gates | Failure owner |
|----------|-----------------------------|---------------|---------------|---------------|
| Controls public surface | `src/Controls/Types.fsi` | `controls-public-surface` | `ControlsCatalogCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`, `PackageSurfaceCheck`, `FsiTranscripts`, `GeneratedProductCheck` | product |
| Generated template | `template/base/src/Product/Program.fs` | `generated-template` | `TemplateCheck`, `GeneratedProductCheck` | template |
| Evidence governance | `specs/028-agent-validation-framework/tasks.md` | `evidence-governance` | `EvidenceGraph`, `EvidenceAudit` | governance |
| Generated guidance | `template/fragments/controls/skill/SKILL.md` | `generated-guidance` | `GeneratedGuidanceCheck`, `TemplateDrift` | governance |
| Documentation-only | `docs/testing.md` | `docs-only` | `EvidenceGraph` | governance |
| Package surface | `src/Lib/AgentValidation.fsi` | `package-surface` | `PackageSurfaceCheck`, `FsiTranscripts` | product |
| Build target contract | `build.fsx` | `build-target-contract` | `AgentReady`, `EvidenceGraph`, `EvidenceAudit`, `Verify`, `Ci` | governance |

## Verification

- `dotnet build src/Lib/Lib.fsproj --no-restore`: PASS.
- `dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj --no-restore -- --filter-test-case "ValidationSelectionInterpreter"`: PASS, 2 tests.
- `dotnet fsi specs/028-agent-validation-framework/readiness/t024-validation-interpreter.fsx`: PASS.

The focused interpreter tests use real temp filesystem writes, real git
repository commands, real contract loading, and real report output. The FSI
transcript exercises the public module against the current repository state.
