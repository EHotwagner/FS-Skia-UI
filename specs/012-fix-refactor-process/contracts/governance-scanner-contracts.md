# Contract: Governance Scanner Accuracy

## Purpose

Define scanner rules that reduce false positives while preserving real
boundary, dependency, template, and stale-reference failures.

## Dependency Scanner Rules

Dependency checks must inspect structured declarations or anchored syntax:

- `ProjectReference` entries from project XML
- `PackageReference` entries from project XML
- central package version entries from `Directory.Packages.props`
- governed metadata fields in capability or template files

Package/project names must not be matched by arbitrary substring search.
Names such as `Lib`, `Charts`, and `Scene` count only in known declaration or
governed metadata contexts.

## Template And Generated Product Rules

Generated scanners must be profile-aware:

- ordinary product profiles reject copied framework implementation projects,
  framework samples, historical specs, readiness evidence, framework docs, and
  stale package references
- `sample-pack` profiles may include intended generated sample content under
  generated `samples/` paths
- generated products must reference public packages rather than framework
  implementation source

## Generated Guidance Rules

Generated guidance validation must connect public guidance claims to generated
source and test evidence. When a report claims generated products exercise
`RichText.create`, `LineChart.create`, `GraphView.create`, `DataGrid.create`,
or `ControlsElmish.program`, the generated inventory must include matching
source and test markers.

## Stale Boundary Rules

Stale boundary scans must cover:

- active-tree ownership documentation
- `.specify/memory/constitution.md`
- architecture documentation
- active source files
- active tests
- package metadata
- capability metadata
- generated guidance
- template fragments
- readiness evidence

Historical specs and migration guidance may mention removed packages only when
the context clearly describes history, replacement, or deletion.

## Diagnostics

Every scanner failure must identify the actionable item:

- file path
- rule id
- generated profile where applicable
- package or project reference
- capability id
- stale term and classification
- readiness evidence path
- remediation hint

## Validation

Governance tests must include seeded scenarios for:

- dependency substring false positives
- real dependency violations
- sample-pack allowed content
- ordinary generated product copied-framework violations
- generated inventory missing source/test markers
- stale active ownership references
- allowed historical or migration references
