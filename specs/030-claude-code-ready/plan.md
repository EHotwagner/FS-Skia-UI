# Implementation Plan: Claude Code Ready Spec Kit

**Branch**: `030-claude-code-ready` | **Date**: 2026-05-29 | **Spec**: `specs/030-claude-code-ready/spec.md`
**Input**: Feature specification from `specs/030-claude-code-ready/spec.md`

## Summary

Make the framework repository and every generated Spec Kit product usable from Claude Code without degrading the existing Codex workflow. The approach is to introduce a single source for agent-facing workflow content, generate both Codex `.agents`/`AGENTS.md` artifacts and Claude Code `CLAUDE.md`/`.claude` artifacts from it, and extend validation so repository and generated-template drift fails with actionable reports.

## Technical Context

**Language/Version**: F# on .NET `net10.0`
**Primary Dependencies**: Existing FS.Skia.UI packages, Expecto, FAKE, template packaging, Spec Kit extension scripts. No new runtime package dependency is planned. Online research sources are official Claude Code documentation for project memory, skills, settings, commands, and hooks.
**Testing**: Expecto governance tests, FAKE targets (`Verify`, `TemplateCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`), generated product validation, drift fixtures, and readiness evidence.
**Target Platform**: Windows and Linux. Claude Code user-local setup, release publishing, product UI/runtime behavior, browser/mobile support, and package identity/version changes are out of scope unless template package content validation proves a packaging metadata update is required.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: `.template.config/template.json`, `.template.config/generated/AGENTS.md`, `.template.package/FS.Skia.UI.Template.fsproj`, `.specify/templates/*`, `.agents/skills/*`, `template/base/.agents/skills/fs-skia-project/SKILL.md`, generated product instructions, and `build.fsx` template inclusion checks are in scope. The generator must emit Claude Code artifacts for every profile that emits Codex artifacts.
- **Dependency impact**: No new package dependency is planned. If implementation adds a parser/generator dependency, it must update `Directory.Packages.props`, `docs/dependencies.md`, generated template package references when applicable, and `DependencyReport`.
- **Command-surface impact**: `Verify`, `TemplateCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` are in scope. `Ci` must continue to aggregate the updated verification. New focused validation may be added only if it is wired into `Verify`.
- **Generated project impact**: Generated projects must include `CLAUDE.md`, `.claude/settings.json`, `.claude/skills/*/SKILL.md`, hook scripts/settings when validated, and optional `.claude/commands/*` aliases only when generated from the same source as skills. Generated products must not require `.claude/settings.local.json` or user-level `~/.claude` files.
- **Evidence paths**: Required readiness paths are `specs/030-claude-code-ready/readiness/claude-code-research.md`, `specs/030-claude-code-ready/readiness/repository-agent-inventory.md`, `specs/030-claude-code-ready/readiness/config-sync-validation.md`, `specs/030-claude-code-ready/readiness/generated-template-agent-artifacts.md`, and `specs/030-claude-code-ready/readiness/generated-project-claude-code-ready.md`. Implementation should also refresh `readiness/task-graph.md`, `readiness/task-graph.json`, `readiness/evidence-audit.md`, and generated guidance reports.
- **`.fsi` / contract impact**: Public F# APIs are not expected. Implementation MUST keep shared generation and validation helpers in build-script/template scope unless a reusable `src/*` module is deliberately introduced. If any reusable public F# module is introduced, work must stop until a `.fsi`, semantic FSI tests, surface-area baseline updates, and compatibility notes are added before `.fs` implementation continues. Build-script-only helpers do not create public framework API.
- **MVU/effect boundary**: This is a stateful/I/O workflow change. Model the shared artifact generation and validation as explicit inputs, outputs, reports, and file-system effects. Keep parsing/rendering decisions testable without mutating the repository; run file writes and process execution at build/generator edges.
- **Synthetic evidence**: Successful readiness cannot rely on synthetic generated projects, synthetic Claude docs, or fake drift output. Synthetic fixtures are allowed only for deliberate malformed/mismatched drift tests and must use the required synthetic disclosure if represented as task evidence.
- **Test evidence**: Add failing-first governance tests for shared-source generation, Claude artifact presence, settings validity, hook locality, generated profile coverage, and drift diagnostics. Add generated-product validation proving source and package template outputs include the Claude artifacts.
- **Observability**: Drift reports must name artifact pair, workflow/instruction id, expected source, observed mismatch, profile or repository scope, and repair command/action. Hook/settings validation must report invalid JSON, non-project-local paths, missing scripts, disabled unsupported hooks, and user-local dependency leaks.
- **Deferred scope**: New product UI behavior, gameplay, renderer changes, broad package publishing, managed enterprise Claude deployment, personal Claude preferences, and non-Claude agents beyond preserving Codex are deferred.

**Gate result before Phase 0**: PASS. The plan names template, command, generated-product, drift, evidence, and Claude Code compatibility obligations with no unresolved clarifications.

## Project Structure

### Feature Artifacts

```text
specs/030-claude-code-ready/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── agent-artifact-sync.md
│   ├── claude-code-project-artifacts.md
│   └── validation-reports.md
└── readiness/
    ├── claude-code-research.md
    ├── repository-agent-inventory.md
    ├── config-sync-validation.md
    ├── generated-template-agent-artifacts.md
    └── generated-project-claude-code-ready.md
```

### Source And Test Touch Points

```text
AGENTS.md
CLAUDE.md
.agents/skills/*/SKILL.md
.claude/settings.json
.claude/skills/*/SKILL.md
.claude/commands/*.md
.claude/hooks/*
.template.config/template.json
.template.config/generated/AGENTS.md
.template.config/generated/CLAUDE.md
.specify/templates/*.md
.specify/extensions/*/commands/*.md
template/base/.agents/skills/fs-skia-project/SKILL.md
template/base/.claude/skills/fs-skia-project/SKILL.md
template/base/.claude/settings.json
template/base/CLAUDE.md
template/profiles/*.yml
template/capabilities.yml
build.fsx
tests/Governance.Tests/
tests/Package.Tests/
```

## Phase 0 Research

Research is captured in `specs/030-claude-code-ready/research.md`.

## Phase 1 Design

Design entities are captured in `specs/030-claude-code-ready/data-model.md`.

Contracts are captured in:

- `specs/030-claude-code-ready/contracts/agent-artifact-sync.md`
- `specs/030-claude-code-ready/contracts/claude-code-project-artifacts.md`
- `specs/030-claude-code-ready/contracts/validation-reports.md`

Quickstart validation is captured in `specs/030-claude-code-ready/quickstart.md`.

## Constitution Check Post-Design

- **Spec -> FSI -> semantic tests -> implementation**: PASS. No public F# API is planned; if one appears, contracts require `.fsi` first, semantic tests, and baseline updates.
- **Visibility lives in `.fsi`**: PASS. Planned work is repository/template/build validation. Any new public module must use `.fsi` as the only public surface.
- **Idiomatic simplicity**: PASS. Use existing file-generation and validation patterns in `build.fsx`; no custom operators, SRTP, reflection, type providers, or non-trivial computation expressions are planned.
- **MVU/effect boundary**: PASS. Generation and drift validation are specified as explicit input/source/output/report entities with file-system effects at the edge.
- **Synthetic evidence disclosure**: PASS. Synthetic fixtures are limited to negative drift/error cases and cannot satisfy successful generated-project readiness.
- **Test evidence mandatory**: PASS. Contracts and quickstart require governance tests, generated source/package template checks, and deliberate drift failure proof.
- **Observability and safe failure**: PASS. Validation report contracts require actionable mismatch, missing artifact, malformed settings, and unsupported hook diagnostics.
