# Contract: Target Metadata and Native Registration

## Artifact

Target metadata is exposed as a repository-owned structured value consumable by `build.fsx` and governance tests. The implementation may store it in F# records, YAML, or JSON, but it must be externally discoverable through a build target or generated report.

## Required Fields

Each runnable validation target metadata entry must include:

- `name`
- `description`
- `tier`
- `dependencies`
- `direct_prerequisites`
- `expected_outputs`
- `stale_assumptions`
- `timeout_class`
- `cost`
- `authority_level`
- `default_failure_owner`
- `command`

## Native Registration

In-scope validation targets must be registered through native FAKE target registration while preserving existing stable command names:

- `GeneratedGuidanceCheck`
- `TemplateCheck`
- `PackageSurfaceCheck`
- `FsiTranscripts`
- `GeneratedProductCheck`
- `EvidenceGraph`
- `EvidenceAudit`
- `Verify`
- `Ci`
- `AgentReady`

## Drift Checks

Validation fails when:

- metadata names a runnable target that native FAKE registration does not expose
- native FAKE registration exposes a validation target without metadata
- `validation.contract.yml` references a target missing metadata
- docs list a validation target that metadata does not list
- metadata lacks expected outputs or default failure owner for a focused/broad gate
- dependencies in metadata and runnable target registration diverge for in-scope validation targets

## Compatibility

Existing stable command names must continue to work during migration. If command behavior changes, docs and verdict metadata must identify the compatibility transition.
