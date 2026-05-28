# Governance Risk Levels

This feature uses the risk-level evidence model from `tasks.md`.

## Small

Small risk changes are localized helper moves with no compile-order, generated
profile, FAKE target, public facade, or report-field changes. Required evidence
is the focused unit or target check that owns the moved helper family, recorded
in the phase readiness file.

## Medium

Medium risk changes include generated source ownership, report writer
consolidation, loaded build script extraction, and viewer internal boundary
movement. Required evidence is the phase-specific focused target set named in
`quickstart.md`, with command, exit code, changed ownership area, and
pre-existing failure attribution recorded in the phase readiness file.

## Broad

Broad risk covers generated profile inclusion, FAKE target dependency wiring,
public `.fsi` shape, surface baselines, package IDs, or readiness paths. Broad
validation requires the focused phase checks plus advisory aggregate runs such
as `Verify`, `PackageSurfaceCheck`, `EvidenceGraph`, and `EvidenceAudit` when
the phase touches those contracts.

## Required Evidence

Each implementation batch records command, exit code, risk level, changed
ownership area, pre-existing failure attribution, and verdict before a task is
marked `[X]`.

## Broad Validation

Broad aggregate commands are treated as non-authoritative unless the focused
phase evidence also passed. Any public contract change remains a Tier 1 stop
condition rather than a cleanup task.
