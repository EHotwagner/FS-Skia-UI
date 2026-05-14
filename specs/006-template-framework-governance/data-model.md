# Data Model: Template Framework Governance

## Command Target

Represents one named workflow entry point in the canonical build graph.

**Fields**

- `name`: Stable target name, unique within the build graph.
- `purpose`: Human-readable reason to run the target.
- `dependencies`: Other targets that must run first.
- `inputs`: Files, projects, scripts, or feature directories consumed by the target.
- `outputs`: Logs, baselines, packages, reports, or console verdicts produced by the target.
- `passCriteria`: Conditions that make the target successful.
- `deferred`: Whether the target is outside v1 scope.

**Validation Rules**

- V1 targets must have documented inputs, outputs, and pass criteria.
- Deferred roadmap targets must not be required by `Dev`, `Verify`, or `Ci`.
- `Verify` must include all required v1 evidence artifact classes.

## V1 Evidence Artifact

Represents a reproducible file or report produced by the v1 workflow.

**Fields**

- `artifactClass`: One of build/test/package logs, public contract transcript, package surface baseline, sample smoke output, task graph output, or evidence audit output.
- `path`: Stable repository-relative output path.
- `producerTarget`: Target responsible for producing or checking the artifact.
- `requiredForVerify`: Whether absence fails `Verify`.
- `retentionOwner`: Current owner of the artifact location.

**Validation Rules**

- Every required artifact class must have one or more documented paths.
- Missing required artifacts must fail `Verify` with actionable output.
- Feature-specific evidence may be copied into feature readiness folders, but current baselines must come from root-level stable paths.

## Stable Package Surface Baseline

Represents the current public package surface reference for one packable package.

**Fields**

- `packageId`: Package identifier such as `FS.Skia.UI`.
- `baselinePath`: Root-level stable baseline file path.
- `legacyPath`: Historical feature-readiness path, if compatibility copying is retained.
- `refreshCommand`: Canonical target, normally `RefreshSurfaceBaselines`, that regenerates the baseline.
- `checkCommand`: Canonical target that compares current package surface to the baseline.

**Validation Rules**

- Baseline paths must not live exclusively under historical feature directories.
- Refresh and check commands must agree on the same stable current path.
- Package surface tests must fail when an expected public contract name is missing.

## Repository Automation Entry

Represents repository-level automation that verifies the project or runs Spec Kit workflows.

**Fields**

- `name`: Stable automation entry name.
- `location`: Repository-relative file path.
- `invokedTarget`: Canonical target used by the automation.
- `duplicatedCommands`: Any direct restore/build/test/package/evidence sequence that must be removed.
- `scope`: Local, CI, Spec Kit workflow, or hook guidance.

**Validation Rules**

- Touched automation must invoke or reference canonical workflow entries.
- Automation must not duplicate command order owned by `build.fsx`.
- If no CI workflow exists, docs must state that future CI should call `Ci`.

## Generated Task Guidance

Represents task-generation guidance that tells future tasks how to verify work.

**Fields**

- `templatePath`: Path to the task template or command guidance.
- `canonicalTargetReferences`: Target names that generated tasks should call.
- `deferredGuidance`: Spec/plan template hardening that is explicitly out of scope for v1.

**Validation Rules**

- Updated task guidance must point to canonical workflow entries.
- Task guidance must still require `tasks.deps.yml` and evidence graph validation.
- Full generated spec/plan template hardening must remain documented as deferred.

## Deferred Roadmap Item

Represents a template-framework capability visible in docs but excluded from v1 pass/fail criteria.

**Fields**

- `name`: Deferred capability name.
- `reasonDeferred`: Scope boundary or dependency on v1 foundation.
- `futureTarget`: Expected future target or document, if known.
- `blockedFromV1Verify`: Whether the item is explicitly excluded from v1 verification.

**Validation Rules**

- Deferred items must not appear as required v1 artifacts.
- Docs must name package consumer smoke as deferred.
- Template packaging, dependency governance, generated spec/plan hardening, layout evidence, visual evidence, and release validation must be excluded from v1 pass/fail criteria.

## Relationships

- `Verify` depends on `Dev`, `PackLocal`, `PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`, `EvidenceGraph`, and `EvidenceAudit`.
- `Ci` aliases or wraps `Verify` for non-interactive automation.
- `PackLocal` depends on `Build` and produces local packages only.
- `RefreshSurfaceBaselines` regenerates `Stable Package Surface Baseline` records in `readiness/surface-baselines/`.
- `PackageSurfaceCheck` consumes `Stable Package Surface Baseline` records.
- `Repository Automation Entry` invokes `Command Target`.
- `Generated Task Guidance` references `Command Target`.
- `Deferred Roadmap Item` may become a future `Command Target`, but is not part of v1 `Verify`.
