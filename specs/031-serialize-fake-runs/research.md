# Research: Serialize FAKE Runs

## Decision: Treat all `fake.sh`, `fake.cmd`, and `dotnet fake` invocations as FAKE-backed and serialize them

**Rationale**: The repository assumption is that FAKE-backed tests and targets share `.fake` state and can race when launched concurrently. A command-name based rule is easy for humans, agents, generated project guidance, and validation scanners to apply. It covers direct targets such as `./fake.sh build -t Verify`, generated product `./fake.sh build -t Test`, Windows `fake.cmd`, and direct `dotnet fake` usage.

**Alternatives considered**: Serializing only `Verify` and `Ci` was rejected because the spec names both FAKE-backed tests and FAKE targets, and generated products expose `Dev`, `Test`, `Verify`, `EvidenceGraph`, and `EvidenceAudit`. Detecting actual `.fake` writers dynamically was rejected for this planning phase because the guidance must be deterministic and understandable before execution.

## Decision: Preserve non-FAKE parallelism when otherwise safe

**Rationale**: The race risk is scoped to shared `.fake` state. File reads, `rg`, docs inspection, non-FAKE buildless checks, and independent analysis can still be parallelized when they do not invoke FAKE or depend on `.fake`. This preserves agent efficiency without hiding the specific unsafe operation class.

**Alternatives considered**: Banning all parallel commands was rejected because it exceeds FR-006 and would slow unrelated repository work. Allowing agents to decide case-by-case without a named FAKE rule was rejected because the failure mode is intermittent and easy to miss.

## Decision: Record command order in readiness evidence whenever more than one FAKE-backed command supports a claim

**Rationale**: Readiness reviewers need to distinguish a product failure from a suspected `.fake` race. A small ordered table with command, purpose, start/end timestamp or relative order, exit code, and log path is enough to prove serialization and support failure triage.

**Alternatives considered**: Keeping only individual logs was rejected because separate logs do not prove ordering. Requiring a full process monitor was rejected as unnecessary for a guidance/governance feature.

## Decision: Validate guidance through focused text checks and generated artifact checks

**Rationale**: The feature changes instructions rather than runtime behavior. Focused governance tests can fail when any updated FAKE-backed instruction omits the terms `FAKE-backed`, `.fake`, or sequential execution, while generated artifact checks prove the template carries the same guidance to products.

**Alternatives considered**: Manual review alone was rejected because SC-001 requires complete coverage of updated agent-facing validation instructions. Changing FAKE target internals to enforce a repository-wide lock was rejected as broader build-target redesign outside the spec.

## Decision: Race-like failure triage starts with a sequential rerun

**Rationale**: A suspected `.fake` race is an environmental/workflow failure until reproduced by a sequential run. Guidance should instruct contributors to rerun the affected FAKE-backed commands one at a time, record the order, and only investigate product defects if the sequential rerun fails.

**Alternatives considered**: Treating every FAKE-backed failure as a product regression was rejected because it wastes debugging time on known workflow races. Automatically deleting `.fake` was rejected because it may mask state and does not prove concurrency was avoided.
