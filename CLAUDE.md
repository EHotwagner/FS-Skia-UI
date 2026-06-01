# FS Skia UI Claude Code Instructions

@AGENTS.md

Claude Code should use the project-local skills in `.claude/skills/` for Spec Kit workflows. The matching Codex source artifacts live under `.agents/skills/`; validation treats these as synchronized peers.

Project settings live in `.claude/settings.json`. User-local settings such as `.claude/settings.local.json` or `~/.claude/settings.json` are optional personal preferences and are not required for repository or generated-project readiness.

## Run `Route` first; run only the gates it prints

Before validating a change, run `./fake.sh build -t Route`. It reads the
working-tree diff and prints the authoritative **tier** and the **minimal gate
list** for *this* change; run only the gates it prints. A routine
framework-internal change routes to the light **inner-loop** tier (`Dev` only);
consumer-contract changes (`template/**`, `.specify/**`, public `src/**/*.fsi`,
`build.fsx`, governance paths) **escalate** automatically. `./fake.sh build -t
Route --enforce` fails when an escalated change is missing a required evidence
artifact. The selector is compiled F# in `FS.Skia.UI.Build` (`Routing`); a
mistyped gate is a compile error, and `validation.contract.yml` is generated
from `Routing.fs`.

## The serialized six-target order (escalated / maintainer-verify path)

The serialized order below is the **escalated `maintainer-verify` path**,
reserved for consumer-contract changes and **dogfood** features (such as `042`) —
no longer the unconditional default; run it only when `Route` escalates to it.

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
