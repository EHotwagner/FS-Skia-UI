# Implementation Plan: Fixture (N/A with rationale)

## Constitution Check

### Repository Governance Decisions

- **Template ownership**: No template.json change; the consumer contract gains a schema version.
- **Dependency impact**: No package or dependency-report change.
- **Command-surface impact**: No new FAKE target; the validator folds into an existing gate.
- **Generated project impact**: A current generated project still validates green.
- **Evidence paths**: Unit tests and live gate logs under the feature readiness directory.
- **`.fsi` / contract impact**: No product signature change; build-tooling surfaces only.
- **MVU/effect boundary**: N/A — this change has no stateful workflow or I/O; pure functions only.
- **Synthetic evidence**: None planned; all evidence is real typed results.
- **Test evidence**: Failing-first typed Expecto tests over the real parser.
- **Observability**: Each finding names the area and the plan path it was expected in.
- **Deferred scope**: Stage 7 work is out of scope.

## Project Structure

(omitted — MVU/effect boundary is explicitly N/A with a rationale and counts as filled)
