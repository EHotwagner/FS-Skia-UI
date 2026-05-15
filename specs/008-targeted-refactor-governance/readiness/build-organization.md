# Build Organization Evidence

## Concern Inventory

| Concern | Current Location | Accepted Strategy |
|---------|------------------|-------------------|
| Path model and feature readiness resolution | `build.fsx` top section | Single canonical script with named concern sections. |
| MVU workflow model | `BuildModel`, `BuildMsg`, `BuildEffect`, `init`, `update` | Preserved. |
| Interpreter edge | `interpret`, process helpers, file helpers | Preserved at the script edge. |
| Validation and test targets | target graph in `build.fsx` | Preserved. |
| Generated guidance | `GeneratedGuidanceScan` interpreter branch | Hardened with Markdown section validation. |
| Template drift | `scripts/template-drift.fsx` | Hardened with path-class and alignment-class diagnostics. |

## Physical Split Attempt

This feature keeps one canonical `build.fsx`. A physical split was rejected at
design time for this pass because the script is invoked directly by FAKE and the
feature can deliver the requested reviewability through named concern sections,
semantic validators, and target self-check coverage without introducing script
load risk. The acceptance evidence is therefore `Dev`, `Verify`, and `Ci`
loading from the canonical script.

## Evidence

Verification output will be appended by the focused and final build runs:

- `specs/008-targeted-refactor-governance/readiness/logs/dev-verdict.txt`
- `specs/008-targeted-refactor-governance/readiness/logs/verify-verdict.txt`
- `specs/008-targeted-refactor-governance/readiness/logs/ci-verdict.txt`

Current focused evidence:

- `./fake.sh build -t BuildWorkflowCheck`: PASS.
- `./fake.sh build -t GeneratedGuidanceCheck`: PASS.
- `./fake.sh build -t TemplateDrift`: PASS.
- `./fake.sh build -t PackageSurfaceCheck`: PASS.
