# Quickstart: Claude Code Ready Spec Kit Validation

## Prerequisites

- .NET SDK for `net10.0`.
- Bash for `fake.sh` on Linux/WSL/Git Bash.
- Network access only for refreshing Claude Code research evidence; normal validation must run from local files.

## Validation Flow

1. Run repository verification:

   ```bash
   ./fake.sh build -t Verify
   ```

2. Run focused generated-template validation:

   ```bash
   ./fake.sh build -t TemplateCheck
   ./fake.sh build -t GeneratedGuidanceCheck
   ./fake.sh build -t TemplateDrift
   ```

3. Prove evidence graph and audit still understand agent configuration files:

   ```bash
   ./fake.sh build -t EvidenceGraph
   ./fake.sh build -t EvidenceAudit
   ```

4. Inspect generated rows under the template validation output and confirm each Codex `.agents` artifact has a Claude Code peer in `.claude`.

5. Introduce a controlled one-line mismatch in a generated Claude skill or Codex skill fixture, run the sync validation target, and record the failing diagnostic in `readiness/config-sync-validation.md`.

6. Restore generated artifacts through the documented repair action and re-run `Verify`.

## Acceptance Evidence

The feature is ready for task generation when the plan artifacts exist, all Claude Code concepts are mapped to official documentation, and the contracts define how implementation will prove:

- repository Claude Code readiness,
- generated product Claude Code readiness,
- Codex/Claude drift failure,
- template profile coverage,
- evidence audit pattern coverage.
