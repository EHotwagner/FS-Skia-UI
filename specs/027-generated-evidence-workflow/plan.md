# Implementation Plan: Generated Evidence Workflow Authority

**Branch**: `027-generated-evidence-workflow` | **Date**: 2026-05-28 | **Spec**: `specs/027-generated-evidence-workflow/spec.md`
**Input**: Feature specification from `/specs/027-generated-evidence-workflow/spec.md`

## Summary

Make generated project evidence workflows authoritative instead of completion-only placeholders. The implementation will connect generated evidence graph and audit targets to the governed Spec Kit validation semantics, add skill-loading evidence generation/validation for one row per task and skill pairing, improve readiness audit diagnostics with missing file and missing term details, make audit-enforced readiness files discoverable before implementation, and refresh generated guidance for common FS.Skia.UI generated game evidence patterns.

This is a Tier 1 contracted governance and generated-consumer behavior change. It changes generated command behavior, audit diagnostics, generated guidance, template validation, and readiness obligations. No default public `.fsi` API change is planned.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for build targets, governance tests, generated product tests, and generated app code. Existing Spec Kit extension scripts use Bash and Python 3 and remain script-level governance assets.

**Primary Dependencies**: Existing FAKE build script, Expecto, generated template assets under `template/base`, Spec Kit evidence scripts under `.specify/extensions/evidence`, and FS.Skia.UI generated consumer packages. No new external package dependency is planned.

**Testing**: Expecto governance tests, generated product tests, template validation, focused FAKE targets (`GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, `EvidenceAudit`, `Verify`, `Ci` aggregation as applicable), direct negative fixtures for evidence graph/audit failure behavior, and readiness artifacts that record command output.

**Target Platform**: Windows and Linux development hosts for repository and generated project validation. Host-specific visual screenshot support is not expanded by this feature; generated evidence wording must classify unsupported screenshot or desktop visibility claims precisely.

**Public Surface**: No planned `.fsi` surface change. Publicly observable surfaces are generated project command behavior, generated template files, build-target contracts, evidence script diagnostics, docs, and readiness contracts. Surface baselines change only if implementation discovers an unavoidable public library helper.

**Evidence Requirement**: Required readiness paths are:

- `specs/027-generated-evidence-workflow/readiness/generated-validation-authority.md`
- `specs/027-generated-evidence-workflow/readiness/skill-loading-evidence-workflow.md`
- `specs/027-generated-evidence-workflow/readiness/audit-diagnostics.md`
- `specs/027-generated-evidence-workflow/readiness/readiness-contract-discovery.md`
- `specs/027-generated-evidence-workflow/readiness/framework-guidance.md`
- `specs/027-generated-evidence-workflow/readiness/evidence-vocabulary.md`
- `specs/027-generated-evidence-workflow/readiness/evidence-graph.md`
- `specs/027-generated-evidence-workflow/readiness/evidence-audit.md`

**Synthetic Evidence**: Synthetic malformed graph/audit/readiness fixtures may be used only for rejection and diagnostic tests with `[SEH]` task labeling and disclosure. Successful generated evidence graph/audit proof must come from real commands over generated or fixture feature directories and must not be represented by success-only placeholder logs.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Generated project template source changes are expected in `template/base/build.fsx`, `template/base/docs/product.md`, `template/base/src/Product/EvidenceCommands.fs`, generated tests under `template/base/tests/Product.Tests`, and possibly full-governance fragment guidance. Update `.template.config/template.json` only if template file membership changes.
- **Dependency impact**: PASS. No new package dependency is planned. If implementation adds a package, it must update `Directory.Packages.props`, generated package pins, `docs/dependencies.md`, and `DependencyReport`.
- **Command-surface impact**: `EvidenceGraph`, `EvidenceAudit`, `GeneratedGuidanceCheck`, and `TemplateCheck` must change. `Verify` and `Ci` must include the updated checks where their existing aggregation expects generated governance evidence. `Dev`, `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if command aggregation, packaging, dependency, or template-drift coverage requires it.
- **Generated project impact**: Generated governed apps must no longer expose success-only evidence graph/audit stubs. Generated guidance must make readiness contracts, skill-loading evidence, and screenshot vocabulary discoverable. Default interactive launch remains unchanged.
- **Evidence paths**: Required readiness paths are listed in Technical Context. Target-level logs should be captured under `readiness/logs/` when tasks are generated.
- **`.fsi` / contract impact**: PASS with no planned `.fsi` changes. This feature changes generated command and script contracts. If a public helper is added to `src/Testing` or another package, the `.fsi`-first chain, FSI transcript, semantic tests, and surface baseline refresh become mandatory.
- **MVU/effect boundary**: PASS. Normal generated app state and viewer workflows remain unchanged. Evidence command execution is I/O-bearing but lives at build/command edges; if generated app runtime bookkeeping grows stateful, it must expose pure decisions and edge execution clearly.
- **Synthetic evidence**: PASS with restrictions. Negative fixtures for malformed graph/audit/readiness states are allowed only as synthetic error-handling evidence. Success criteria require real command outputs, not canned completion text.
- **Test evidence**: Add failing-first governance tests for generated `EvidenceGraph`/`EvidenceAudit` authority, skill-loading row generation/validation, audit missing-term diagnostics, readiness contract discovery, generated guidance wording, and normal launch separation.
- **Observability**: Failure reports must include command, feature/generated app identity, failed validation area, missing readiness path, missing terms where applicable, exit code, and whether validation was authoritative or skipped.
- **Deferred scope**: New game mechanics, renderer redesign, package publishing, new platform support, browser/mobile screenshots, semantic scene annotation public API, and replacement of the screenshot capture contract are out of scope.

**Pre-design gate result**: PASS. The plan preserves no-new-dependency intent, normal launch separation, no synthetic success path, and explicit diagnostics for generated governance evidence.

## Project Structure

```text
build.fsx
  # Root command-surface contracts, target dependencies, readiness outputs

.specify/extensions/evidence/
  scripts/python/compute-task-graph.py  # graph, skillist, skill-loading validation
  scripts/bash/run-audit.sh             # audit diagnostics and readiness contract scan

template/base/
  build.fsx                             # generated project evidence targets
  docs/product.md                       # generated app governance and evidence guidance
  src/Product/EvidenceCommands.fs       # generated evidence command wording/records
  tests/Product.Tests/Tests.fs          # generated product guidance/command tests

template/fragments/
  full-governance/README.md
  scene/README.md
  skiaviewer/README.md
  testing/README.md
  layout/README.md

docs/
  build.md
  evidence.md
  generated-apps.md
  speckit.md
  testing.md

tests/Governance.Tests/
  CommandContractTests.fs
  GeneratedProjectValidationTests.fs
  GovernanceEvidenceTests.fs
  PersistentViewerEvidenceTests.fs
  SkillValidationTests.fs
  TemplateWorkflowTests.fs
  GeneratedGuidanceTests.fs

specs/027-generated-evidence-workflow/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    generated-evidence-command-contract.md
    skill-loading-evidence-contract.md
    audit-diagnostics-contract.md
    generated-guidance-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/027-generated-evidence-workflow/research.md`.
Key decisions:

- Generated evidence graph/audit targets must invoke or exactly delegate to the Spec Kit evidence extension semantics, not emit standalone success reports.
- Skill-loading evidence should be derived from `tasks.deps.yml` `skillist` metadata and rendered as one task/skill row per required pairing; collapsed ranges remain invalid.
- Audit readiness diagnostics should expose missing files and missing terms directly in console output and structured JSON/Markdown artifacts.
- Readiness contract discovery should be represented both as task-generation obligations and as generated placeholder/checklist guidance so implementers see audit-enforced files before work starts.
- Generated guidance should keep semantic scene facts separate from screenshot proof and should use explicit fallback fields for non-screenshot visual evidence.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/027-generated-evidence-workflow/research.md`
- `specs/027-generated-evidence-workflow/data-model.md`
- `specs/027-generated-evidence-workflow/contracts/generated-evidence-command-contract.md`
- `specs/027-generated-evidence-workflow/contracts/skill-loading-evidence-contract.md`
- `specs/027-generated-evidence-workflow/contracts/audit-diagnostics-contract.md`
- `specs/027-generated-evidence-workflow/contracts/generated-guidance-contract.md`
- `specs/027-generated-evidence-workflow/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. No public `.fsi` surface is planned. Command/script contracts are captured in `contracts/`; any later public helper addition triggers `.fsi`-first work.
- **Visibility in `.fsi`**: PASS. Planned changes are build scripts, extension scripts, generated template files, docs, tests, and readiness artifacts.
- **Idiomatic simplicity**: PASS. Existing F#, Bash, and Python assets are sufficient; no complex F# feature is planned.
- **MVU/effect boundary**: PASS. Normal app MVU remains unchanged. Evidence validation is command-edge I/O with observable reports.
- **Synthetic disclosure**: PASS with restrictions. Negative fixtures must be `[SEH]`; authoritative success evidence must be real command output.
- **Test evidence**: PASS. Quickstart names failing-first governance tests, generated product tests, template checks, command checks, and readiness outputs.
- **Observability and safe failure**: PASS. Contracts require failure details, missing terms, authoritative/skipped state, and no success-only completion logs.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks with `skillist` metadata, failing-first governance tests, generated template updates, evidence script diagnostics, readiness placeholder/discovery work, docs/guidance updates, focused target runs, evidence graph validation, and final evidence audit.
