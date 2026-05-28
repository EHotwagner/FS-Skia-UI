# Data Model: Phased Refactor Cleanup

## Refactor Phase

- **Fields**: phase id, name, priority, scope, touched areas, baseline checks,
  final checks, readiness path, acceptance result.
- **Relationships**: Owns one or more responsibility areas and writes one
  readiness artifact.
- **Validation rules**: A phase cannot begin until current status and
  pre-existing failures are recorded. A phase cannot be accepted until its
  checks prove behavior preservation for touched areas.
- **State transitions**: planned -> baseline recorded -> implementation in
  progress -> checks run -> accepted or blocked.

## Behavior Contract

- **Fields**: command names, FAKE target names, generated profile names, report
  field names, status vocabulary, output paths, exit-code meanings, package
  identities, public signatures, surface baselines.
- **Relationships**: Referenced by generated template, build governance,
  package tests, viewer tests, and readiness files.
- **Validation rules**: Values remain unchanged unless a separate approved Tier
  1 feature authorizes a contract change.
- **State transitions**: baseline captured -> compared after phase -> preserved
  or violation reported.

## Duplication Classification

- **Fields**: helper family, locations, classification, consolidation decision,
  boundary rationale, verification coverage.
- **Relationships**: Applies to process execution, report writing, scalar/list
  parsing, generated scanning, package resolution, image checks, geometry
  checks, and process-health policy.
- **Validation rules**: Intentional template and package-boundary copies may
  remain; repository-local and drift-prone semantic copies should consolidate
  when no boundary is crossed.
- **State transitions**: discovered -> classified -> consolidated, retained, or
  deferred.

## Generated Product Module

- **Fields**: module name, responsibility, compile order, profile conditions,
  dependencies, generated file path.
- **Relationships**: Belongs to `template/base/src/Product/Product.fsproj` and
  contributes to generated profiles.
- **Validation rules**: Compile order must satisfy F# dependencies. Each
  generated profile must include only needed files and remain standalone.
- **State transitions**: single-file responsibility -> extracted file ->
  generated matrix validated.

## Evidence Report Writer

- **Fields**: ordered key-value fields, status, command, output path,
  diagnostics, unsupported reason, fallback, exit code.
- **Relationships**: Used by generated product evidence commands and validated
  by generated/guidance tests.
- **Validation rules**: Field names, ordering expectations, normalized status
  vocabulary, stdout echo behavior, parent directory creation, and exit-code
  meanings remain stable.
- **State transitions**: specialized writer -> shared local generated writer ->
  command output verified.

## Build Governance Module

- **Fields**: script path, extracted responsibility, loaded order, target users,
  report outputs, failure messages.
- **Relationships**: Loaded by `build.fsx`; supports existing FAKE targets.
- **Validation rules**: Target names, dependencies, readiness paths, report
  outputs, and failure messages remain stable after extraction.
- **State transitions**: inline helper -> loaded script helper -> focused
  target verified.

## Viewer Internal Boundary

- **Fields**: boundary name, internal module path, public facade call sites,
  diagnostics emitted, host classification outcomes, evidence artifacts.
- **Relationships**: Lives behind unchanged `src/SkiaViewer/SkiaViewer.fsi`.
- **Validation rules**: Viewer diagnostics, window behavior validation,
  desktop/unsupported classification, visual evidence, and screenshot evidence
  remain behaviorally unchanged.
- **State transitions**: co-located implementation -> internal module ->
  facade-preserved runtime behavior.
