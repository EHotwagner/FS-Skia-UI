# Implementation Plan: Fixture (unresolved placeholder)

## Constitution Check

### Repository Governance Decisions

- **Template ownership**: No template.json change; the consumer contract gains a schema version.
- **Dependency impact**: NEEDS CLARIFICATION — is a dependency-report change required?
- **Command-surface impact**: No new FAKE target; the validator folds into an existing gate.
- **Generated project impact**: A current generated project still validates green.
- **Evidence paths**: Unit tests and live gate logs under the feature readiness directory.
- **`.fsi` / contract impact**: No product signature change; build-tooling surfaces only.
- **MVU/effect boundary**: Pure validators; file I/O confined to the interpreter edge.
- **Synthetic evidence**: None planned; all evidence is real typed results.
- **Test evidence**: Failing-first typed Expecto tests over the real parser.
- **Observability**: Each finding names the area and the plan path it was expected in.
- **Deferred scope**: Stage 7 work is out of scope.

## Project Structure

(omitted — Dependency impact carries a NEEDS CLARIFICATION placeholder)
