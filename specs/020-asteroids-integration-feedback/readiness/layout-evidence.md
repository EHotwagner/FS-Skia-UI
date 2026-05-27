# Layout Evidence Readiness

## Status

Setup scope recorded for T004.

## Commands

- `./fake.sh build -t EvidenceGraph` after T001, T002, and T003 status changes: passed.

## Evidence

## Tier 1 Scope

This feature is a Tier 1 contracted framework and governance change. It affects
public layout-evidence contracts, generated product validation, generated app
guidance, task skill governance, readiness reports, and evidence audit
behavior.

## Public API Impact

Public layout proof and generated validation types must start in `.fsi`
signatures before implementation. Expected changed surfaces are
`src/Scene/Scene.fsi` and/or `src/Testing/Testing.fsi`, with package surface
baseline review before the integration task is complete.

## Initial Package Surface Review Expectations

T010 records the intentional surface delta before implementation:

- `FS.Skia.UI.Scene` adds public layout evidence records and unions for proof
  level, measurement mode, HUD/gameplay/text bounds, overlap diagnostics,
  unsupported reasons, and `LayoutEvidenceReport`.
- `FS.Skia.UI.Testing` adds generated layout validation request/result records
  and validation failure classes that consume the Scene layout evidence report.
- `FS.Skia.UI.Testing` now references `FS.Skia.UI.Scene`, matching the existing
  capability catalog dependency.
- `readiness/surface-baselines/FS.Skia.UI.Scene.txt` and
  `readiness/surface-baselines/FS.Skia.UI.Testing.txt` must be refreshed only
  after the implementation helpers and validators are complete.
- `PackageSurfaceCheck` evidence is deferred to T032, after T025 and T026
  settle the public surface.

## Generated Product Impact

Generated game samples must reserve a named HUD/status region and a named
gameplay region. Score, lives, wave, status, movement, wrapping, spawning,
clamping, collisions, and active entity bounds must be validated against those
regions at 1280x720 and at the documented constrained size of 640x480.

## MVU And Effect Boundary Applicability

Pure geometry classification and evidence validation do not need a separate
Elmish shell. Generated app contracts and warning/evidence collection remain
subject to the MVU/effect-boundary rule when they introduce state or I/O:
models and messages describe workflow state, pure updates request effects, and
host, filesystem, process, package, font, or window-system work stays at the
edge.

## Evidence Limitations

Deterministic scene hashes and render metadata are valid render consistency
evidence, but they are not HUD readability proof. Approximate text measurement
is allowed only when deterministic, conservative, and disclosed as approximate.
Unsupported layout inspection must remain explicit and must not be converted
into readable-layout proof.

## Required Evidence Obligations

- `readiness/hud-layout-readability.md`: generated HUD/gameplay readability at
  default and constrained sizes.
- `readiness/public-contract-guidance.md`: public FSI or generated-product
  evidence for `Product.Program.view`, `Product.Program.generatedHost`, and
  `Product.Program.update`.
- `readiness/layout-evidence.md`: public layout evidence records, proof levels,
  unsupported diagnostics, and overlap diagnostics.
- `readiness/host-warning-classification.md`: benign warning classification
  without hiding launch, rendering, layout, package, or missing-evidence
  failures.
- `readiness/generated-validation.md`: generated validation commands, evidence
  paths, and duration.
- `readiness/evidence-audit.md`: final graph/audit result and any unsupported
  conditions.

## T027 Layout Evidence Results

Commands:

- `dotnet test tests/Scene.Tests/Scene.Tests.fsproj --logger "console;verbosity=minimal"`: passed 9 tests.
- `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --logger "console;verbosity=minimal"`: passed 23 tests.

Real evidence:

- Complete layout reports classify as `ReadableLayout` only when HUD region,
  gameplay region, text bounds, gameplay bounds, and no-overlap diagnostics are
  present.
- Deterministic render metadata remains `DeterministicRenderOnly`.
- Unsupported layout inspection preserves unsupported reasons and does not claim
  readability.

Invalid and unsupported evidence coverage:

- `generated layout validation rejects missing unsupported overlapping and deterministic-only reports`
  exercises public `LayoutEvidence.fromRenderEvidence`,
  `LayoutEvidence.unsupported`, `LayoutEvidence.classify`, and
  `GeneratedLayoutValidation.validate`.
