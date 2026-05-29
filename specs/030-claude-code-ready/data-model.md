# Data Model: Claude Code Ready Spec Kit

## Agent Instruction Artifact

- **Fields**: `id`, `agentKind`, `path`, `sourceId`, `imports`, `generatedBy`, `scope`, `profileApplicability`.
- **Relationships**: Rendered from a Synchronization Source. Participates in Drift Reports. Included in Repository Agent Inventory and Generated Project Agent Bundle.
- **Validation rules**: Claude project instructions must include `CLAUDE.md` at the repository/product root and import `AGENTS.md` rather than duplicate it. Generated instruction paths must be project-relative and committed when shared.

## Workflow Artifact

- **Fields**: `workflowId`, `agentKind`, `canonicalKind`, `path`, `frontmatter`, `bodySourceId`, `allowedTools`, `commandAliasPath`, `extensionHookSemantics`.
- **Relationships**: Generated from a Synchronization Source for Codex `.agents/skills` and Claude `.claude/skills`. May have an optional Claude command alias.
- **Validation rules**: Claude canonical artifacts live at `.claude/skills/<workflow>/SKILL.md`; each has valid YAML frontmatter with name and description. Command aliases may exist only when generated from the same workflow source. Core lifecycle workflows cover specify, clarify, plan, tasks, implement, checklist, analyze, task-to-issues, git extension commands, and evidence extension commands.

## Project Settings Artifact

- **Fields**: `path`, `permissions`, `hooks`, `environment`, `scope`, `shareable`, `sourceId`.
- **Relationships**: Generated into repository and generated product `.claude/settings.json`; references Hook Artifacts when hooks are supported.
- **Validation rules**: JSON must parse. Shared project settings must not contain secrets, personal paths, credentials, host-specific approval assumptions, or `.claude/settings.local.json` requirements.

## Hook Artifact

- **Fields**: `hookId`, `event`, `matcher`, `type`, `command`, `scriptPath`, `timeout`, `supportedWorkflowIds`, `sourceId`.
- **Relationships**: Referenced by Project Settings Artifact and validated by generated product checks.
- **Validation rules**: Hook commands must use project-local paths, preferably `${CLAUDE_PROJECT_DIR}`. Each referenced script must exist in the generated repository/product and be portable across Windows/Linux or documented as shell-specific with an alternate path.

## Synchronization Source

- **Fields**: `sourceId`, `artifactClass`, `workflowId`, `codexRenderer`, `claudeRenderer`, `templateProfiles`, `hash`, `owner`.
- **Relationships**: Produces Agent Instruction Artifacts, Workflow Artifacts, Project Settings Artifacts, and command aliases.
- **Validation rules**: Every Codex artifact with Claude parity must map to exactly one source id and one Claude artifact. Source changes must either regenerate outputs or produce a Drift Report.

## Drift Report

- **Fields**: `status`, `scope`, `sourceId`, `artifactPair`, `workflowId`, `profile`, `expectedPath`, `actualPath`, `differenceSummary`, `repairAction`, `command`.
- **Relationships**: Emitted by validation targets and referenced from readiness evidence.
- **Validation rules**: Failing drift reports must identify mismatched artifact, affected workflow or instruction, expected source of truth, and repair command/action.

## Generated Project Agent Bundle

- **Fields**: `profile`, `artifactSource`, `codexFiles`, `claudeFiles`, `settingsFiles`, `hookFiles`, `selectedSkills`, `validationEvidence`.
- **Relationships**: Built from template profiles and scanned by generated product validation.
- **Validation rules**: Every profile that emits Codex Spec Kit artifacts must emit equivalent Claude Code artifacts. Generated projects must exclude framework history, user-local Claude files, and source-only active feature state.

## Claude Code Compatibility Evidence

- **Fields**: `concept`, `officialSourceUrl`, `retrievedAt`, `implementedArtifacts`, `limitations`, `validationCommand`, `evidencePath`.
- **Relationships**: Stored under `readiness/claude-code-research.md` and linked from implementation evidence.
- **Validation rules**: Each supported Claude concept has an official source, retrieval date, implemented artifact path, and validation result.
