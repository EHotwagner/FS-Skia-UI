# Contract: Sequential FAKE Readiness Evidence

Readiness evidence that supports this feature must prove FAKE-backed validation ran one command at a time.

## Required Artifact

`specs/031-serialize-fake-runs/readiness/sequential-fake-validation.md`

## Required Fields

For every FAKE-backed command in the evidence sequence:

- Order number
- Command
- Working directory
- Purpose
- Start/end timestamp or clear relative ordering
- Exit code
- Log path or note explaining why the command produced no separate log

## Required Failure Triage Fields

When a FAKE-backed command fails:

- Failed command
- Whether another FAKE-backed command was running at the same time
- Suspected `.fake` race status: `suspected`, `not-suspected`, or `unknown`
- Required rerun order when status is `suspected` or `unknown`
- Follow-up classification after sequential rerun

## Pass Conditions

- More than one FAKE-backed command is never presented as concurrent evidence.
- Evidence records the order for all FAKE-backed commands needed by the readiness claim.
- If a race-like failure occurred, the evidence includes the sequential rerun result before product regression claims.
