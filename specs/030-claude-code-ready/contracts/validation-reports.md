# Contract: Validation Reports

## Repository Agent Inventory

Path: `specs/030-claude-code-ready/readiness/repository-agent-inventory.md`

Must list repository Codex and Claude artifacts by class, source id, path, and validation status.

## Config Sync Validation

Path: `specs/030-claude-code-ready/readiness/config-sync-validation.md`

Must include:

- Command run.
- Passing sync result.
- At least one deliberate one-line drift fixture or controlled mutation result.
- Failure diagnostic showing mismatched artifact, source id, and repair action.

## Generated Template Agent Artifacts

Path: `specs/030-claude-code-ready/readiness/generated-template-agent-artifacts.md`

Must list source and package template validation rows for every supported profile and the Claude artifacts found in each generated output.

## Generated Project Claude Code Ready

Path: `specs/030-claude-code-ready/readiness/generated-project-claude-code-ready.md`

Must prove a generated project contains project instructions, skills, settings, hooks when supported, and no user-local Claude configuration dependency.

## Claude Code Research

Path: `specs/030-claude-code-ready/readiness/claude-code-research.md`

Must cite official Claude Code documentation with retrieval dates and map each source to implemented project artifacts.
