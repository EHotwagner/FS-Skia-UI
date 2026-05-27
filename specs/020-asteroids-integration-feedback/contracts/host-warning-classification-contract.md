# Contract: Host Warning Classification

Readiness output must separate benign desktop host warnings from actionable
failures.

## Classes

- `BenignEnvironmentWarning`: known non-fatal host warning while the app remains
  usable and required evidence passes.
- `LaunchFailure`: app or viewer did not produce a usable launch.
- `RenderingFailure`: rendering or first-frame evidence failed.
- `LayoutFailure`: HUD/layout readability validation failed.
- `PackageFailure`: restore, package drift, or generated package validation
  failed.
- `UnknownWarning`: unclassified warning that remains visible for review.

## Non-Fatal Rule

A warning may be non-fatal only when:

- The warning is known and documented by message class or normalized code.
- Usable launch evidence is present or the host is explicitly unsupported.
- Rendering/package checks have not failed.
- Layout readability checks have passed or are explicitly unsupported without a
  readability claim.

## Report Fields

- `warning-class`
- `fatal`
- `raw-message` or normalized code
- `evidence-path`
- `supporting-checks`
- `diagnostics`

Real launch, rendering, layout, package, and missing-evidence failures must
remain fatal even if benign warnings are also present.
