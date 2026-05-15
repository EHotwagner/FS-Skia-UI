# Contract: Layout Diagnostics and Record Invariants

## Yoga Fallback Diagnostics

When recoverable Yoga execution fails and pure layout fallback is used:

- layout still returns safe deterministic bounds
- `LayoutResult.Diagnostics` includes an observable fallback diagnostic if the existing public diagnostic surface can carry it
- the diagnostic uses existing public fields only
- the diagnostic identifies Yoga fallback use through code, severity, constraint, fallback flag, message, and available node/context

Planned diagnostic encoding:

- `Code = FallbackBoundsApplied`
- `Severity = Warning`
- `Constraint = Some "yoga"`
- `FallbackApplied = true`
- `Message` names recoverable Yoga execution failure and pure fallback layout
- `NodeId` is populated when the failure can be attributed to a node

If this encoding is insufficient without changing `.fsi`, the implementation must record a follow-up API proposal and avoid public surface changes.

## Public Record Invariant Review

The readiness inventory must include every public record exposed by the library packages.

Each entry includes:

- package and record name
- relevant fields
- invariant or "free construction intended"
- current construction stance
- decision
- rationale
- follow-up ID when helper constructors or validation-first public APIs are recommended

## Failure Conditions

- Yoga failure falls back silently when existing fields can carry a diagnostic.
- Yoga fallback diagnostic requires a new public field, union case, or helper in this feature.
- Safe fallback bounds regress.
- Any public record is absent from the readiness inventory.
- A recommendation for public helper constructors or validation APIs lacks a follow-up ID.

## Evidence

- `specs/008-targeted-refactor-governance/readiness/yoga-fallback-diagnostics.txt`
- `specs/008-targeted-refactor-governance/readiness/record-invariants.md`
- `specs/008-targeted-refactor-governance/readiness/follow-ups.md`
