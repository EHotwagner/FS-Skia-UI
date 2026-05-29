# Data Model: Agent Validation Framework

## ValidationContract

Repository-owned machine-readable document that maps changes and feature concerns to validation obligations.

**Fields**: `version`, `defaults`, `tiers`, `rules`, `targetMetadataRef`.

**Relationships**: Owns many `ValidationRule`; references many `TargetMetadata` entries.

**Validation rules**:

- `version` must be supported by the current build.
- Every referenced gate must have runnable target registration and target metadata.
- Defaults must include broad fallback command and final evidence obligations.

## ValidationRule

One routable validation obligation selected by paths, feature concerns, or risk categories.

**Fields**: `id`, `description`, `paths`, `featureConcerns`, `riskCategories`, `requiredGates`, `expectedArtifacts`, `timeoutClass`, `authorityLevel`, `failureOwner`, `stalePrerequisites`.

**Relationships**: References one or more `TargetMetadata` records and zero or more required readiness artifacts.

**Validation rules**:

- `id` is stable, unique, and appears in verdicts.
- `requiredGates` must not contain unknown targets.
- `expectedArtifacts` must be relative paths and must be covered by evidence graph/audit where final readiness depends on them.

## ValidationTier

Named authority and cost level for validation.

**Fields**: `id`, `authorityLevel`, `intendedUse`, `defaultGates`, `cost`, `timeoutClass`.

**Known values**: `inner-loop`, `focused-authority`, `agent-ready`, `maintainer-verify`, `automation-final`.

**Validation rules**:

- `agent-ready` includes selected focused gates plus `EvidenceGraph` and `EvidenceAudit`.
- `maintainer-verify` maps to `Verify`.
- `automation-final` maps to `Ci`.

## ChangedPathSource

Source used to select validation rules.

**Fields**: `kind`, `paths`, `feature`, `mergeBase`, `diagnostics`.

**Known values**: `active-feature-metadata`, `git-merge-base-diff`, `unavailable`.

**State transitions**:

- Start with active feature metadata.
- Fall back to git merge-base diff when metadata is absent.
- Degrade when neither source produces confident paths.

## AgentVerdict

Single validation outcome emitted by agent-ready and broad validation paths.

**Fields**: `status`, `authority`, `changedPathSource`, `selectedRuleIds`, `requiredGates`, `completedGates`, `missingGates`, `skippedGates`, `failureOwner`, `failureClass`, `nextCommand`, `artifacts`, `diagnostics`, `timestampUtc`.

**Known statuses**: `passed`, `failed`, `unsupported`, `degraded`.

**Known failure classes**: `environment`, `unsupported-host`, `stale-prerequisite`, `product`, `template`, `governance`, `missing-evidence`, `unknown`.

**Validation rules**:

- A degraded verdict must include `nextCommand`.
- A passed verdict must have no missing required gates.
- Every selected rule id must exist in `ValidationContract`.

## TargetMetadata

Discoverable planning metadata for a runnable validation target.

**Fields**: `name`, `description`, `tier`, `dependencies`, `directPrerequisites`, `expectedOutputs`, `staleAssumptions`, `timeoutClass`, `cost`, `authorityLevel`, `defaultFailureOwner`, `command`.

**Relationships**: Must correspond to exactly one runnable FAKE target name unless explicitly marked `metadataOnly`.

**Validation rules**:

- Runnable validation targets must have metadata.
- Metadata targets that claim runnable authority must exist in native FAKE registration.
- `expectedOutputs` and `defaultFailureOwner` are required for focused and broad validation gates.

## EvidencePolicyWorkflow

Explicit generated or repository workflow that produces governed evidence separately from normal product launch.

**Fields**: `command`, `productFacts`, `policyActions`, `reportPath`, `authorityLevel`, `failureClasses`.

**Relationships**: Consumes product-owned facts and emits `AgentVerdict` or evidence reports.

**Validation rules**:

- Normal launch must not run audits or write evidence artifacts.
- Report wording must not claim stronger authority than completed gates support.

## TypedControlFrontDoor

Public standard-control authoring path that prevents or rejects misspelled known framework contracts before lowering to generic controls.

**Fields**: `controlKind`, `events`, `attributes`, `requiredAttributes`, `dataAttributes`, `customAllowed`.

**Relationships**: References `ControlSchema`, lowers to `Control<'msg>` and `Attr<'msg>`.

**Validation rules**:

- Every existing standard controls module must have a typed path.
- Custom controls/events/values must use visibly custom APIs.
- Typed standard paths must reject seeded misspellings for known kinds, events, and chart/grid data attributes.

## ControlSchema

Shared known-control registry used by diagnostics, generated guidance, catalogs, and rendering validation.

**Fields**: `kind`, `requiredAttributes`, `supportedEvents`, `supportedAttributes`, `accessibilityExpectations`, `diagnosticCodes`.

**Relationships**: Supports `TypedControlFrontDoor` and `ControlDiagnostic`.

**Validation rules**:

- Required attributes and supported events are defined once per known standard control.
- Diagnostics for missing or unsupported required attributes must name schema-owned terms.
