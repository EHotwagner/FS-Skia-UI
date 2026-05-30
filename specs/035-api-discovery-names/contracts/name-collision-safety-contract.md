# Contract: Name Collision Safety

## Purpose

Consumers that combine `FS.Skia.UI.Scene` and `FS.Skia.UI.Controls` can write
stable code without relying on namespace open order.

## Collision Inventory

Each collision-prone group must record:

- `collisionName`
- owning packages and namespaces/modules
- symbol kinds involved
- risk level
- observed or plausible authoring failure
- public contract decision
- generated guidance decision
- validation scenario

## Allowed Resolutions

- Add or preserve contract-level qualification such as
  `[<RequireQualifiedAccess>]` for compatible discriminated unions.
- Add explicit safer front-door APIs where a package owns the ambiguity.
- Require explicit namespace or module qualification in generated examples and
  guidance.
- Mark as accepted non-issue only when validation proves the overlap cannot
  affect consumer authoring.

## Acceptance

- Every collision-prone name identified by validation has a decision.
- Mixed Scene/Controls generated examples compile with multiple namespace open
  orders or avoid open-order dependence through explicit qualification.
- Guidance names the required qualification pattern.
