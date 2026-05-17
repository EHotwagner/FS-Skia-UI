# Contract: Focused Gate Independence

## Purpose

Define the command contract for focused validation gates so maintainers can
diagnose product and governance failures when broad aggregate verification is
under runner pressure.

## Required Focused Gates

The following gates must remain directly invocable:

- `PackageSurfaceCheck`
- `FsiTranscripts`
- `ControlsCatalogCheck`
- `ControlsInteractionCheck`
- `ControlsRenderingCheck`
- `DependencyReport`
- `TemplateCheck`
- `GeneratedProductCheck`
- `GeneratedGuidanceCheck`
- `TemplateDrift`
- `EvidenceGraph`
- `EvidenceAudit`

Additional focused gates may be added for process health, bootstrap runner
validation, or verification verdicts when that keeps target contracts clearer.

## Dependency Rules

- A focused gate must not depend on `Verify`, `Ci`, or another broad aggregate.
- A focused gate may depend on a small direct prerequisite such as restore,
  build, template pack, capability check, or evidence graph only when the
  prerequisite is documented in the target contract and tested.
- Stale `--no-build` or `--no-restore` assumptions must produce diagnostics
  naming the affected gate and the action needed to obtain valid evidence.

## Required Gate Output

Each focused gate must report:

- command or target name
- direct prerequisites
- duration or timestamp evidence
- log path
- readiness output path where applicable
- verdict category
- failure rule or affected artifact when failing

## Validation

Command-contract tests must fail if:

- a focused gate is recoupled to `Verify`, `Ci`, or broad build work without a
  documented/tested reason
- a focused gate loses its log or readiness output
- a gate relies on stale build/restore assumptions without diagnostics
- target membership changes without updating docs and readiness contracts
