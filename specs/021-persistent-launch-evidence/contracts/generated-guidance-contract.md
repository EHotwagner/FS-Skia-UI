# Contract: Generated Guidance

## Purpose

Keep generated app examples unambiguous and keep deterministic layout evidence
separate from persistent-window launch evidence.

## Required Names

Generated docs, tests, and samples must use:

- `Product.Program.view` for the app-owned scene-returning function.
- `Product.Program.generatedHost` for the generated viewer host value.
- `Product.Program.update` for reducer examples and tests.

This is required when generated code opens framework capability namespaces such
as keyboard input, scene, testing, or viewer modules.

## Evidence Separation

Generated guidance must state through command structure and artifacts that:

- Layout evidence proves structural layout/readability facts.
- Deterministic render hashes prove deterministic rendering only.
- Persistent-launch evidence proves real interactive window launch facts.
- Screenshot or visible-window proof requires actual visual/window facts.

## Generated Readiness Command

Generated graphical apps must expose a readiness workflow or target that writes
the persistent-launch artifact without changing normal default launch behavior.

## Validation

`GeneratedGuidanceCheck`, `GeneratedProductCheck`, `TemplateCheck`, and
`EvidenceAudit` must fail when generated guidance:

- Uses ambiguous unqualified app-owned names in collision-prone examples.
- Presents layout evidence as visible-window proof.
- Omits the persistent-launch artifact path from readiness instructions.
- Fails to document known benign warning treatment.
