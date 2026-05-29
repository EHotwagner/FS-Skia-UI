# Contract: Agent Artifact Synchronization

## Purpose

Codex and Claude Code artifacts must be generated from one authoritative workflow source and validated as matching that source.

## Required Source Records

Each shared workflow source record must provide:

- `sourceId`: stable id such as `speckit-plan`.
- `artifactClass`: `instruction`, `workflow`, `settings`, `hook`, or `command-alias`.
- `codexOutputs`: project-relative generated Codex paths.
- `claudeOutputs`: project-relative generated Claude Code paths.
- `templateProfiles`: profiles that must receive the artifact.
- `repairAction`: command or build target that regenerates outputs.

## Required Parity

- `AGENTS.md` and `CLAUDE.md` are instruction peers. `CLAUDE.md` imports `AGENTS.md`.
- `.agents/skills/<id>/SKILL.md` and `.claude/skills/<id>/SKILL.md` are workflow peers.
- `.claude/commands/<id>.md` aliases are optional and must point back to the same `sourceId`.
- `.claude/settings.json` is the only committed Claude settings file unless a future plan explicitly adds managed policy support.

## Drift Failure Requirements

Validation must fail when:

- A source id renders different user-facing workflow semantics across agents.
- A Codex artifact exists without the required Claude peer.
- A Claude artifact exists without a source id or Codex peer, except Claude-only glue such as `CLAUDE.md` import text.
- Generated template output omits a Claude artifact for a profile that includes the Codex equivalent.

## Report Fields

Every failure report must include `scope`, `sourceId`, `workflowId`, `expectedPath`, `actualPath`, `differenceSummary`, and `repairAction`.
