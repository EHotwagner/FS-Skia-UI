# FS Skia UI Claude Code Instructions

@AGENTS.md

Claude Code should use the project-local skills in `.claude/skills/` for Spec Kit workflows. The matching Codex source artifacts live under `.agents/skills/`; validation treats these as synchronized peers.

Project settings live in `.claude/settings.json`. User-local settings such as `.claude/settings.local.json` or `~/.claude/settings.json` are optional personal preferences and are not required for repository or generated-project readiness.

FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`) share
repository `.fake` state and are not safe to run concurrently. Claude Code may
parallelize safe non-FAKE reads and checks, but must run multiple FAKE-backed
tests or targets sequentially in deterministic order. If a failure looks
race-like or concurrent FAKE context is unknown, rerun the affected
FAKE-backed commands sequentially before product debugging.

Default serialized order when multiple FAKE-backed validation commands are
needed:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`
