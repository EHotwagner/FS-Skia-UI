# Data Model: Fix Refactor Process Reliability

## Process Health Snapshot

- **Fields**: timestamp, target name, platform, available memory, process
  count, zombie process count, thread limit/headroom, file descriptor
  limit/headroom, dotnet startup smoke result, FAKE bootstrap status, warning
  summary, unsupported signal list
- **Relationships**: Produced before broad aggregate work; referenced by
  verification verdicts, process-health readiness evidence, and final
  readiness guidance
- **Validation Rules**: Must be written before high-pressure aggregate work
  starts. Unsupported signals must be reported explicitly rather than omitted
  silently. Fail-fast decisions must name the failed default threshold or
  explicit override.

## Process Health Threshold

- **Fields**: rule id, signal name, default value, comparison, override value,
  override source, override reason, platform applicability
- **Relationships**: Evaluated against a process health snapshot; reported in
  environment-failure verdicts and readiness evidence
- **Validation Rules**: Repository defaults are required. Overrides are valid
  only when both value and reason are recorded. Unknown or malformed overrides
  fail bootstrap/preflight with actionable diagnostics.

## Verification Verdict

- **Fields**: verdict category, target, stage, exit code, product checks run,
  product failures, environment failures, health snapshot path, log path,
  recommended rerun environment, authoritative product evidence flag
- **Relationships**: Produced by broad aggregates and focused gates; consumed
  by readiness reports and final readiness blocking rules
- **Validation Rules**: Environment failures fail the aggregate but are marked
  non-authoritative for product behavior unless a product check actually ran
  and failed. A later healthy broad pass is required after any broad aggregate
  environment failure.

## Bootstrap Validation

- **Fields**: dotnet SDK status, FAKE tool restore status, required package
  cache/restore status, wrapper availability, repeated warning classification,
  failure recommendation, log path
- **Relationships**: Runs before feature gates depend on local runner
  dependencies; feeds process-health and verification verdict evidence
- **Validation Rules**: Missing required runner dependencies fail bootstrap.
  Known warnings are classified separately from target failures and must not
  hide later actionable failures.

## Focused Gate

- **Fields**: target name, purpose, direct prerequisites, allowed broad
  prerequisites, command, expected logs, readiness outputs, duration, verdict,
  stale build/restore assumptions
- **Relationships**: Implemented as FAKE targets; tested by command-contract
  tests; referenced by focused-gates readiness evidence
- **Validation Rules**: Must be directly invocable. Must not depend on broad
  aggregate work unless the dependency is explicitly documented and tested.
  Diagnostics must name missing build/restore prerequisites.

## Command Contract Rule

- **Fields**: rule id, target, required dependency shape, forbidden dependency
  shape, required effect, required log/report path, fixture scenario
- **Relationships**: Enforced by governance tests over `build.fsx`, scripts,
  docs, and readiness artifacts
- **Validation Rules**: Fails when focused targets are recoupled to broad
  aggregates, verdict effects are missing, process-health effects are hidden,
  or required artifact paths drift.

## Governance Scanner Rule

- **Fields**: rule id, scanner name, input paths, parse mode, allowed contexts,
  forbidden contexts, profile applicability, diagnostic format, evidence path
- **Relationships**: Covers dependency reports, generated product checks,
  generated guidance, template drift, stale boundary scans, and evidence audit
- **Validation Rules**: Must inspect structured declarations or anchored
  syntax. Failures name the affected file, profile, package reference,
  capability id, stale term, or readiness path.

## Generated Product Profile

- **Fields**: profile id, artifact kind, allowed packages, forbidden packages,
  allowed generated content, forbidden framework content, expected source
  markers, expected test markers, evidence logs
- **Relationships**: Produced by template validation; consumed by generated
  product scanner rules and inventory reports
- **Validation Rules**: `sample-pack` may contain intended generated sample
  content. Ordinary profiles reject copied framework samples, implementation
  projects, historical specs, readiness evidence, and stale package
  references. Public guidance claims require source and test evidence.

## Stale Boundary Reference

- **Fields**: term, file path, line or match context, active ownership status,
  allowed historical/migration classification, owning rule, remediation
  action, evidence path
- **Relationships**: Found by stale boundary scans across docs, memory, source,
  tests, metadata, templates, generated guidance, and readiness evidence
- **Validation Rules**: Active ownership references to removed packages fail.
  Historical or migration references pass only when the context clearly states
  replacement or removal guidance.

## Removed Package Evidence

- **Fields**: package id, source deletion status, test deletion status, package
  reference status, capability entry status, public ownership claim status,
  migration guidance status, evidence paths
- **Relationships**: Aggregates stale boundary scan results and generated
  product validation for removed packages
- **Validation Rules**: Must identify deletion or update status for source,
  tests, package references, capability entries, and active ownership docs in
  one readiness report.

## Readiness Evidence Record

- **Fields**: evidence path, producer command, covered story, target verdict,
  authoritative flag, environment failure links, synthetic marker, stale/diff
  scan status, last healthy broad pass reference
- **Relationships**: Written under `specs/012-fix-refactor-process/readiness/`
  and referenced by tasks, contracts, quickstart, evidence graph, and evidence
  audit
- **Validation Rules**: Must not claim final readiness when any broad aggregate
  environment failure lacks a later healthy broad pass. Synthetic evidence and
  diff-scan hits remain visible to evidence audit.
