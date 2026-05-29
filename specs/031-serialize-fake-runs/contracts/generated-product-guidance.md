# Contract: Generated Product Sequential FAKE Guidance

Generated projects that include FAKE-backed validation commands must carry the same serialization rule as the source repository.

## Affected Generated Surfaces

- `README.md`
- `docs/product.md`
- Local agent skill instructions under `.agents/skills/`
- Claude Code skill instructions under `.claude/skills/`
- Any generated command/readiness guidance that lists `Dev`, `Test`, `Verify`, `EvidenceGraph`, or `EvidenceAudit`

## Required Behavior

- Generated docs list FAKE-backed validation commands in order.
- Generated docs state that `./fake.sh`, `fake.cmd`, and `dotnet fake` commands are not safe to run concurrently because they can race on `.fake`.
- Generated docs say non-FAKE checks may be parallelized only when they do not invoke FAKE or depend on `.fake`.
- Generated readiness capture guidance uses a sequential command order and asks contributors to record that order when more than one FAKE-backed command is used.

## Validation

Template source validation and package validation must inspect generated output, not only source templates. A valid report names each profile or generated artifact checked and whether sequential FAKE guidance was present.
