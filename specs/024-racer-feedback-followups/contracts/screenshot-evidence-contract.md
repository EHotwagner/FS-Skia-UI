# Screenshot Evidence Contract

## Scope

Applies to live screenshot evidence produced by SkiaViewer/generated app
evidence commands and to validators that consume screenshot evidence reports.

## Successful Screenshot Result

A successful live screenshot result MUST include:

- `status=ok`
- `evidence-kind=screenshot`
- PNG artifact path
- positive width and height
- first-frame presentation occurred before capture
- capture source identifies the live viewer window

Deterministic scene rendering MUST remain a separate fallback or diagnostic
evidence kind and MUST NOT be relabeled as screenshot proof.

## Unsupported or Failed Result

Unsupported and failed screenshot results MUST preserve distinct fields for:

- whether the viewer could be opened
- whether first-frame presentation occurred, if known
- whether screenshot capture is available
- unsupported or failure reason
- deterministic fallback evidence kind, when fallback exists

When the viewer opens but live capture is unavailable, the result MUST say so
without implying renderer or app failure. When the viewer cannot open, the
result MUST expose launch/open failure separately from capture capability.

## Evidence

- `specs/024-racer-feedback-followups/readiness/screenshot-success-artifact.md`
  records successful PNG proof on at least one supported Windows or Linux
  desktop host.
- `specs/024-racer-feedback-followups/readiness/screenshot-capability-detail.md`
  records unsupported/capability details and any deferral for an unavailable
  supported OS validation host.
