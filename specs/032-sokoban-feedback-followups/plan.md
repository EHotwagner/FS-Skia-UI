# Implementation Plan: Sokoban Feedback Follow-ups

**Branch**: `032-sokoban-feedback-followups` | **Date**: 2026-05-29 | **Spec**: `specs/032-sokoban-feedback-followups/spec.md`
**Input**: Feature specification from `specs/032-sokoban-feedback-followups/spec.md`

## Summary

Address the Sokoban demo feedback by making default text readable in screenshot evidence, proving persistent interactive close evidence through the real generated host, and improving generated consumer guidance for API shape, readiness contracts, and task graph pitfalls. The implementation should reuse existing FS.Skia.UI package boundaries and generated-product validation, with public surface changes only if implementation discovers that existing close/input/text primitives cannot satisfy the contracts.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for framework packages, generated templates, validation helpers, and tests; Markdown/YAML for Spec Kit and generated-app guidance.
**Primary Dependencies**: Existing FS.Skia.UI Scene, SkiaViewer, Testing, KeyboardInput, Controls/Elmish packages; SkiaSharp 4 preview packages; Silk.NET; Expecto; FAKE; Spec Kit evidence scripts. No new dependency is planned.
**Testing**: Failing-first Expecto tests in package and governance suites, screenshot capability evidence, generated product validation, and sequential FAKE-backed targets (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`, with `Verify` as a single broad final pass when needed).
**Target Platform**: Windows and Linux generated graphical app workflows. Default text screenshot acceptance is required on supported Linux desktop hosts with common Latin fonts; unsupported host limitations must be reported explicitly. Browser, mobile, macOS support expansion, release publishing, and new gameplay are out of scope.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Generated product docs, generated local skills, template guidance fragments, and evidence command documentation are in scope. Review `.template.config/template.json` only if new generated files are introduced; otherwise preserve template identity and inclusion policy.
- **Dependency impact**: No dependency change is planned. If implementation requires a new font/rendering/package dependency, stop and update `Directory.Packages.props`, `docs/dependencies.md`, template package pins, and `DependencyReport` coverage before implementation continues.
- **Command-surface impact**: `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, and `EvidenceAudit` are in scope for guidance and readiness validation. `Dev` may be used for generated persistent launch proof. `Verify` remains a broad validation entry and must not be run concurrently with other FAKE-backed commands.
- **Generated project impact**: Generated app docs, product notes, local skills, evidence command guidance, generated tests, and readiness instructions are in scope. Generated default launch should remain a real persistent viewer-backed interactive window; bounded evidence-only commands must stay separate.
- **Evidence paths**: Required readiness paths are `specs/032-sokoban-feedback-followups/readiness/default-text-glyph-capture.md`, `specs/032-sokoban-feedback-followups/readiness/interactive-window-close-evidence.md`, `specs/032-sokoban-feedback-followups/readiness/consumer-guidance-scan.md`, `specs/032-sokoban-feedback-followups/readiness/readiness-contract-scan.md`, and `specs/032-sokoban-feedback-followups/readiness/task-guidance-scan.md`. Implementation should also refresh task graph and evidence audit outputs under the same readiness directory.
- **`.fsi` / contract impact**: Public APIs are not expected to change for the baseline plan. If screenshot glyph capability, app-requested close, key/input mapping, or validation helpers require a new public contract, start in the relevant `.fsi` file, add semantic tests and surface baselines, then implement.
- **MVU/effect boundary**: Generated app close remains app-owned state and message flow. Pure reducers emit app commands or close-request state; viewer/window close, input dispatch, screenshot capture, process launch, and artifact writes stay at generated host or validation interpreter edges.
- **Synthetic evidence**: Synthetic fixtures are allowed only for negative malformed-output or scanner error-path tests with Principle V disclosure. Passing readiness requires real screenshot capability checks and real generated persistent-window launch/close evidence.
- **Test evidence**: Add failing-first package/guidance tests proving default text in screenshot captures renders glyph-shaped coverage, generated host close evidence records accepted persistent launch and clean exit, consumer guidance names key/API surfaces, readiness guidance names required files/terms, and task guidance catches known graph pitfalls.
- **Observability**: Evidence artifacts must include command, mode, host support facts, screenshot artifact path or unsupported classification, first-frame/window-opened status, close request source, exit path, required-term scan results, failure classification, and next action.
- **Deferred scope**: Mobile/browser/macOS support expansion, release publishing, broad renderer redesign, new Sokoban gameplay, replacing Spec Kit governance, and unrelated Controls/chart/DataGrid work are deferred.

**Gate result before Phase 0**: PASS. The plan names package, template, command, generated-product, MVU/effect, synthetic-evidence, observability, and readiness obligations with no unresolved clarification.

## Project Structure

### Feature Artifacts

```text
specs/032-sokoban-feedback-followups/
|-- spec.md
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- default-text-glyph-capture.md
|   |-- persistent-close-evidence.md
|   `-- guidance-readiness-task-contracts.md
`-- readiness/
    |-- default-text-glyph-capture.md
    |-- interactive-window-close-evidence.md
    |-- consumer-guidance-scan.md
    |-- readiness-contract-scan.md
    `-- task-guidance-scan.md
```

### Source And Test Touch Points

```text
src/Lib/Library.fsi
src/Lib/Library.fs
src/Scene/Scene.fsi
src/Scene/Scene.fs
src/SkiaViewer/SkiaViewer.fsi
src/SkiaViewer/SkiaViewer.fs
src/Testing/Testing.fsi
src/Testing/Testing.fs
src/KeyboardInput/KeyboardInput.fsi
src/Controls.Elmish/ControlsElmish.fsi
template/base/docs/product.md
template/base/README.md
template/base/src/Product/EvidenceCommands.fs
template/base/src/Product/Program.fs
template/base/.agents/skills/fs-skia-project/SKILL.md
template/base/.claude/skills/fs-skia-project/SKILL.md
template/fragments/*/README.md
.specify/templates/tasks-template.md
.specify/presets/fsharp-opinionated/templates/tasks-template.md
.agents/skills/speckit-tasks/SKILL.md
docs/generated-apps.md
docs/evidence.md
docs/testing.md
tests/Lib.Tests/
tests/Scene.Tests/
tests/SkiaViewer.Tests/
tests/Testing.Tests/
tests/Governance.Tests/
build.fsx
```

## Phase 0 Research

Research is captured in `specs/032-sokoban-feedback-followups/research.md`.

## Phase 1 Design

Design entities are captured in `specs/032-sokoban-feedback-followups/data-model.md`.

Contracts are captured in:

- `specs/032-sokoban-feedback-followups/contracts/default-text-glyph-capture.md`
- `specs/032-sokoban-feedback-followups/contracts/persistent-close-evidence.md`
- `specs/032-sokoban-feedback-followups/contracts/guidance-readiness-task-contracts.md`

Quickstart validation is captured in `specs/032-sokoban-feedback-followups/quickstart.md`.

## Constitution Check Post-Design

- **Spec -> FSI -> semantic tests -> implementation**: PASS. No public API addition is assumed; contracts require `.fsi`, semantic tests, surface baselines, and docs before implementation if public surface changes become necessary.
- **Visibility lives in `.fsi`**: PASS. Any new public Scene, SkiaViewer, Testing, KeyboardInput, or adapter symbol must be declared through its `.fsi`.
- **Idiomatic simplicity**: PASS. The planned work uses existing records, discriminated unions, generated guidance, and validation checks; no complex F# feature is required.
- **MVU/effect boundary**: PASS. Product close remains app-owned state/message data and host effects are interpreted at the edge.
- **Synthetic evidence disclosure**: PASS. Synthetic negative scanner/parser fixtures cannot satisfy readiness for screenshot glyphs or persistent launch.
- **Test evidence mandatory**: PASS. Each story has failing-first package/governance/generated validation and named real readiness evidence.
- **Observability and safe failure**: PASS. Contracts require actionable status fields, host facts, failure classifications, artifact paths, and next commands.
