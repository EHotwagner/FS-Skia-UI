# Evidence Obligations

Date: 2026-05-16

## Tier 1 Scope

This feature changes package identity, public contracts, generated template
composition, command governance, selected local skills, dependency ownership,
and package surface baselines. It is a Tier 1 contracted change.

## Public API Impact

V3 introduces or retargets package-specific public contracts for Scene,
SkiaViewer, Elmish, KeyboardInput, Layout, Charts, and Testing. Public surface
ownership lives in `.fsi` files and package-specific baselines.

## MVU Applicability

Build and generated-product command workflows are stateful and I/O-bearing.
They must use `BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`, and an
edge interpreter. Runtime workflow capabilities that expose state transitions
or effects must document `Model`, `Msg`, `Effect` or `Cmd<Msg>`, `init`,
`update`, subscriptions, and interpreter boundaries where applicable.

## Unsupported Scope

V2 migration implementation is out of scope. The feature must record
compatibility impact and explicitly state that no V2 migration path is provided
by this work.

## Evidence Required

| Evidence class | Required readiness path |
|----------------|-------------------------|
| Capability catalog | `specs/009-v3-modular-framework/readiness/capability-catalog.md` |
| Generated file lists | `specs/009-v3-modular-framework/readiness/generated-file-lists/` |
| Generated product verify logs | `specs/009-v3-modular-framework/readiness/generated-product-verify/` |
| Selected skills | `specs/009-v3-modular-framework/readiness/selected-skills.md` |
| Package surfaces | `specs/009-v3-modular-framework/readiness/package-surfaces/` |
| Dependency report | `specs/009-v3-modular-framework/readiness/dependency-report.md` |
| Generated guidance | `specs/009-v3-modular-framework/readiness/generated-guidance.md` |
| Template drift | `specs/009-v3-modular-framework/readiness/template-drift.md` |
| Compatibility impact | `specs/009-v3-modular-framework/readiness/compatibility-impact.md` |
| Evidence graph and audit | `specs/009-v3-modular-framework/readiness/task-graph.*`, `diff-scan-hits.json`, and audit logs |

## Synthetic Evidence Policy

No synthetic evidence is planned as the primary proof. If placeholders,
in-memory substitutes, canned native evidence, or unconnected generated-product
fixtures are introduced, the task must be marked `[S]`, code/test disclosures
must use the `SYNTHETIC` token, and this file plus `tasks.md` must explain the
real-evidence path.
