# Generated guidance validation (feature 077, SC-005)

- **Authoritative command**: `./fake.sh build -t GeneratedGuidanceCheck`.
- **Artifact**: `readiness/generated-guidance-validation.md` (this file).
- **Failure class**: template / generated-guidance (required Constitution-Check governance
  areas empty or boilerplate in generated guidance).
- **Next action**: confirm the gate passes; the feature adds no generated-guidance area, only
  corrected vendored phase-skill text that ships through the copy globs.

## Result

PASS — `./fake.sh build -t GeneratedGuidanceCheck` finished `Status: Ok`. This feature adds no
generated-guidance Constitution-Check area; the corrected vendored phase-skill text ships
through the existing `.agents/skills/**/*` / `.claude/skills/**` copy globs without any
`template.json` change. `TargetMetadataDrift` also reports no contract drift after the
regenerated `validation.contract.yml` added the `PhaseHookParityCheck` gate to the
`skill-quality` rule.
