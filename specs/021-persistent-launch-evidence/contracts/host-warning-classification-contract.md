# Contract: Host Warning Classification

## Purpose

Record common non-fatal desktop warning noise without failing readiness when the
real launch evidence passes.

## Public Surface

Review `src/Testing/Testing.fsi` for host warning classification helpers. The
existing `HostWarningClassification` types may be extended if they cannot
represent persistent-launch facts.

## Benign Warning Examples

Known optional desktop module warnings include messages equivalent to:

```text
Failed to load module "colorreload-gtk-module"
Failed to load module "window-decorations-gtk-module"
```

These are benign only when launch, first-frame/render, and required exit facts
pass.

## Fatal Class Preservation

Warnings or diagnostics must remain fatal when paired with:

- Launch failure.
- Rendering failure.
- Layout failure.
- Package failure.
- Artifact write failure.

## Required Output

Each classification result must include:

- Raw message.
- Warning class.
- Fatal flag.
- Evidence path.
- Supporting facts used for classification.
- Diagnostics.

## Audit Rule

EvidenceAudit must not block a passing persistent-launch artifact solely because
benign host warnings were present, but it must preserve the warnings in the
readiness output.
