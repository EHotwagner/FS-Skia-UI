# Contract: Validation Routing

## Artifact

The repository exposes `validation.contract.yml` at the repository root.

## Required Shape

```yaml
version: 1
defaults:
  broad_fallback_command: ./fake.sh build -t Verify
  final_gates:
    - EvidenceGraph
    - EvidenceAudit
tiers:
  - id: inner-loop
    authority: non-authoritative
  - id: focused-authority
    authority: focused-authoritative
  - id: agent-ready
    authority: focused-authoritative
  - id: maintainer-verify
    authority: broad-authoritative
  - id: automation-final
    authority: broad-authoritative
rules:
  - id: controls-public-surface
    paths:
      - src/Controls/**/*.fsi
      - src/Controls/**/*.fs
    feature_concerns:
      - controls-public-surface
    required_gates:
      - ControlsCatalogCheck
      - ControlsInteractionCheck
      - ControlsRenderingCheck
      - PackageSurfaceCheck
      - FsiTranscripts
      - GeneratedProductCheck
    expected_artifacts:
      - readiness/control-catalog.md
      - readiness/interaction-tests.md
      - readiness/layout-rendering.md
    timeout_class: focused
    failure_owner: product
```

## Required Rules

The initial contract must include rules for:

- controls public surface changes
- generated template changes
- evidence-governance changes
- generated app guidance changes
- documentation-only changes
- package-surface changes
- build-target contract changes

## Selection Semantics

1. Determine changed paths from active feature metadata.
2. If unavailable, determine changed paths from git merge-base diff.
3. Select every rule with matching paths, feature concerns, or risk categories.
4. Union required gates without duplicate execution claims.
5. Add `EvidenceGraph` and `EvidenceAudit` for `agent-ready` authority.
6. If changed-path context is unavailable or ambiguous, emit a degraded verdict with the broad fallback command.

## Drift Validation

Validation fails when:

- a required gate has no runnable FAKE target
- a required gate has no `TargetMetadata`
- a focused or broad gate lacks outputs or failure owner
- a documented validation target is missing from metadata
- a runnable validation target is missing from metadata
- seeded representative changed-path scenarios route to the wrong gates
