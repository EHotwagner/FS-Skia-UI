# Data Model: Serialize FAKE Runs

## FAKE-Backed Command

Represents any validation or build command that invokes FAKE and may touch shared `.fake` state.

- **Fields**: command text, working directory, target name when present, platform wrapper (`fake.sh`, `fake.cmd`, or `dotnet fake`), purpose, log path, start order, exit code.
- **Relationships**: Appears in a Validation Sequence and may be referenced by Failure Triage Note.
- **Validation rules**: Must not be documented as parallel with another FAKE-backed command. When multiple FAKE-backed commands are required, each must have a deterministic order.

## Validation Sequence

Represents a maintainer, agent, generated product, or readiness workflow that runs one or more commands.

- **Fields**: sequence id, scope, ordered command list, allowed non-FAKE parallel work, evidence path, owner guidance path.
- **Relationships**: Contains zero or more FAKE-Backed Commands and may produce Sequential Evidence Record.
- **Validation rules**: If the sequence contains more than one FAKE-backed command, those commands must be ordered and accompanied by a statement that FAKE-backed commands are unsafe to run concurrently because they can race on `.fake`.

## Guidance Surface

Represents repository or generated artifact text that instructs a user or agent to run validation.

- **Fields**: path, audience, generated or source-owned status, command snippets, required serialization text, last validation status.
- **Relationships**: References Validation Sequences and may be included in generated product guidance.
- **Validation rules**: Updated agent-facing guidance that mentions FAKE-backed tests or targets must also name `.fake` race risk and sequential execution. Non-FAKE checks may still be described as parallelizable only when clearly distinguished from FAKE-backed work.

## Sequential Evidence Record

Represents readiness proof that FAKE-backed commands ran one at a time.

- **Fields**: feature id, ordered commands, log paths, exit codes, timestamps or relative order, rerun status, notes on non-FAKE work.
- **Relationships**: Produced by a Validation Sequence and stored under `specs/031-serialize-fake-runs/readiness/`.
- **Validation rules**: Must include command order whenever more than one FAKE-backed command supports the readiness claim. Must not claim merge readiness from concurrent FAKE-backed logs.

## Failure Triage Note

Represents guidance attached to failed FAKE-backed validation evidence.

- **Fields**: failed command, observed failure, whether another FAKE-backed command was running, suspected `.fake` race classification, sequential rerun command order, next investigation step.
- **Relationships**: References one or more FAKE-Backed Commands and may be included in Sequential Evidence Record.
- **Validation rules**: If concurrency is suspected or unknown, the first next step is a sequential rerun. Product regression investigation starts only after a sequential rerun reproduces the failure.

## State Transitions

```text
Planned sequence
  -> Running sequential FAKE-backed commands
  -> Passed sequential evidence
  -> Ready for normal defect/readiness review

Planned sequence
  -> FAKE-backed failure with concurrent/unknown context
  -> Sequential rerun required
  -> Passed rerun: classify original as suspected workflow race
  -> Failed rerun: continue normal product or validation defect investigation
```
