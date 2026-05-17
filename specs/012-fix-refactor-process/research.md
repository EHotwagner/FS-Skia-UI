# Research: Fix Refactor Process Reliability

No unresolved clarifications remain from the specification. The decisions below
resolve the planning unknowns and set the implementation boundary.

## Broad Process-Health Preflight

**Decision**: Add a repository-owned preflight before broad `Verify` and `Ci`
aggregate work. The preflight records available memory where measurable,
process count, zombie process count, process/thread/file descriptor limits,
open file descriptor headroom where measurable, dotnet/FAKE bootstrap
availability, and a lightweight process/runtime startup smoke signal. Defaults
live in repository code and can be overridden explicitly with reported rule,
value, and reason.

**Rationale**: The process report showed broad aggregates failing before useful
product tests executed. A fast preflight makes non-authoritative local runner
conditions visible before the aggregate adds more pressure.

**Alternatives considered**: Relying on post-failure log interpretation was
rejected because it is too late and easy to confuse with product failure.
Requiring CI-only broad verification was rejected because maintainers still
need local diagnostics and focused isolation paths.

## Environment-Failure Verdicts

**Decision**: Introduce explicit verification verdict categories:
`success`, `product-failure`, `environment-failure`, and `degraded`. Startup,
process creation, CoreCLR creation, VSTest socket/thread setup, missing
runner/bootstrap dependencies, or fail-fast preflight failures produce a
failing `environment-failure` aggregate unless a product check actually ran and
failed.

**Rationale**: The aggregate result must fail when evidence is not
authoritative, but the failure must not be presented as a product regression.
This lets focused product evidence remain useful while final broad readiness
stays blocked until a healthy broad pass exists.

**Alternatives considered**: Treating environment failures as skipped was
rejected because final readiness could pass without broad evidence. Treating
all nonzero aggregate exits as product failures was rejected because it
misstates runner exhaustion as product behavior.

## Default Threshold Ownership

**Decision**: Keep fail-fast threshold defaults in repository-owned workflow
code or config, with command-contract tests proving defaults and override
reporting exist. Thresholds are intentionally conservative and based on
headroom classes rather than machine-specific promises: minimum available
memory when measurable, maximum zombie process count, minimum process/thread
headroom, minimum file descriptor headroom, and required startup smoke success.

**Rationale**: The spec requires defaults plus explicit overrides, but the
repository must run on multiple developer and CI environments. Headroom classes
make decisions auditable without hard-coding one developer machine as the
standard.

**Alternatives considered**: No defaults was rejected by clarification.
Machine-specific constants only in docs were rejected because tests and logs
would not prove enforcement.

## Focused Gate Independence

**Decision**: Preserve focused gates as first-class targets for package
surface, FSI transcripts, catalog, interaction, rendering, dependency,
template, generated product, generated guidance, template drift, evidence
graph, and evidence audit checks. A focused gate may perform a small direct
restore/build prerequisite only when the target contract names and tests that
dependency.

**Rationale**: During the Controls boundary refactor, focused gates produced
actionable product and governance evidence even when the broad runner was
degraded. Accidental recoupling to broad build work would recreate the failure
mode.

**Alternatives considered**: Running only `Verify` for all checks was rejected
because it hides individual failure causes. Letting each target define
implicit prerequisites was rejected because stale `--no-build` and
`--no-restore` assumptions become invisible.

## Bootstrap Warning Classification

**Decision**: Add a lightweight bootstrap validation and classify repeated
runner/tooling warnings separately from target failures. Missing restorable
runner dependencies fail bootstrap. Repeated FAKE script-load warnings are
reported as warnings unless they accompany a nonzero target exit or blocked
startup.

**Rationale**: The process report identified noisy `netstandard` script-load
messages and missing `FSharp.Core/6.0.7` cache state. Maintainers need those
signals separated from product failures.

**Alternatives considered**: Suppressing all warnings was rejected because
bootstrap drift matters. Failing every warning was rejected because successful
target execution can still be authoritative.

## Dependency Scanner Parsing

**Decision**: Dependency governance must inspect structured project XML and
anchored dependency syntax instead of arbitrary substring matching. Package and
project names such as `Lib`, `Charts`, and `Scene` only count when they appear
in known dependency declarations or governed metadata fields.

**Rationale**: The prior `Lib` false positive came from matching inside
`<OutputType>Library</OutputType>`. Structured parsing removes that class of
false positive while preserving real dependency leak detection.

**Alternatives considered**: Maintaining a growing ignore list was rejected
because it would make scanner behavior harder to reason about. Whole-file
substring scanning was rejected as the source of the bug.

## Generated Product Profile Rules

**Decision**: Generated product scanners are profile-aware. The `sample-pack`
profile may intentionally include generated sample content under `samples/`,
while ordinary generated products still reject copied framework samples,
framework implementation projects, historical specs, readiness evidence, and
active stale package references.

**Rationale**: The process report found that intended sample-pack content was
misclassified as copied framework content. Profiles are already part of the
generated product matrix, so validation should use them.

**Alternatives considered**: Allowing samples everywhere was rejected because
ordinary generated products must remain product-owned. Rejecting all sample
content was rejected because it invalidates the sample-pack profile.

## Generated Inventory Completeness

**Decision**: Generated inventories must include product source and tests for
each public behavior claimed by generated guidance, not only outer file lists.
Reports must tie guidance claims such as `RichText.create`,
`LineChart.create`, `GraphView.create`, `DataGrid.create`, and
`ControlsElmish.program` to generated source/test evidence.

**Rationale**: Guidance validation is only useful if the generated product
actually exercises the public behavior being reported.

**Alternatives considered**: File-list-only inventories were rejected because
they can prove package presence without proving product usage. Manual
readiness prose was rejected as insufficient without scanner-backed evidence.

## Stale Boundary Scan Scope

**Decision**: Stale boundary scans cover active-tree ownership documentation,
governance memory, architecture docs, active source, active tests, package
metadata, capability metadata, generated guidance, template fragments, and
readiness evidence before final audit completion is accepted. Historical specs
and migration guidance may mention removed packages only when clearly framed as
historical or migration context.

**Rationale**: The final stale scan in the previous refactor found active
ownership references outside source code. Earlier scan coverage prevents late
audit surprises.

**Alternatives considered**: Scanning source only was rejected because the
known misses were in governance memory and architecture docs. Blocking all
historical mentions was rejected because migration guidance needs to name the
removed package.

## Final Readiness After Environment Failure

**Decision**: After any broad aggregate environment failure, final readiness
requires a subsequent healthy broad aggregate pass. Focused passing evidence
may support diagnosis and product confidence, but it does not replace the
final broad signal.

**Rationale**: A degraded local run can preserve useful focused evidence while
still failing to prove aggregate health. Readiness must make that distinction
hard to miss.

**Alternatives considered**: Allowing focused evidence to override the broad
failure was rejected by clarification. Permanently blocking readiness after a
single local environment failure was rejected because a fresh healthy runner
can provide the missing aggregate evidence.

## MVU Workflow Boundary

**Decision**: Process-health collection, bootstrap validation, target
execution, scanner runs, and verdict writing remain represented as
`BuildEffect` values emitted by pure `update` transitions and interpreted at
the edge. New workflow state is added to `BuildModel` only as durable command
state, not as hidden mutable globals.

**Rationale**: The constitution requires stateful and I/O-bearing work to keep
I/O explicit and testable. The existing build workflow already follows this
shape.

**Alternatives considered**: Running preflight directly inside target graph
construction was rejected because it would mix planning and I/O. Shell-only
preflight outside `build.fsx` was rejected because it would evade command
contract tests and Windows parity.
