# Contract: Process Health And Verification Verdicts

## Purpose

Define how broad verification reports runner health, environment failures, and
authoritative product evidence.

## Process-Health Preflight

Broad `Verify` and `Ci` aggregate paths must record a process-health preflight
before high-pressure work starts. The preflight report must include:

- target name and stage
- timestamp and platform
- available memory when measurable
- process count and zombie process count where measurable
- thread and file descriptor limits/headroom where measurable
- dotnet/CoreCLR startup smoke result
- FAKE/bootstrap dependency status
- unsupported health signals with reason
- threshold decisions, including defaults and explicit overrides

If preflight determines that runner health is clearly insufficient, the
aggregate must stop before launching high-pressure validation work.

## Threshold Overrides

Every fail-fast threshold has a repository-owned default. An override is valid
only when the report includes:

- rule id
- default value
- override value
- override source
- human-readable reason

Malformed overrides fail preflight with an environment-failure verdict.

## Verdict Categories

Verification reports use these categories:

- `success`: all required checks ran and passed
- `product-failure`: a product or governance check ran and failed
- `environment-failure`: runner health, startup, process creation, bootstrap,
  CoreCLR, VSTest, socket/thread, or unsupported-environment conditions
  prevented authoritative product validation
- `degraded`: checks completed with caveats that require explicit readiness
  review but do not by themselves prove product failure

Environment failures fail the aggregate command. They are marked
non-authoritative for product behavior unless a product check actually ran and
failed.

## Required Broad Aggregate Output

`Verify` and `Ci` must write:

- process-health report path
- concise verdict report path
- failing stage
- health diagnostics that caused degradation or failure
- product checks that did or did not run
- recommended rerun environment, such as fresh shell, fresh container, or CI
  runner

## Validation

Command-contract and governance tests must prove:

- broad preflight effects are emitted before broad aggregate work
- preflight fail-fast produces an `environment-failure` verdict
- startup failures are not reported as product failures unless product checks
  ran and failed
- threshold overrides include value and reason
- final readiness remains blocked after broad environment failure until a
  later healthy broad pass is recorded
