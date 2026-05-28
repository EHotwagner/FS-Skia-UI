# Tier 1 Scope And Evidence Obligations

Recorded at: 2026-05-28T08:04:20+02:00

## Scope

This is a Tier 1 contracted framework/governance follow-up for the top-down
racer feedback. The feature affects public framework contracts, generated
template guidance, generated product evidence output, governance tests, and
readiness evidence. It does not redesign generated game mechanics, renderer
architecture, broad platform support, state workflow semantics, or release
automation.

## Public API Impact

Potential public surface changes are additive and belong in signature files
first:

- `src/SkiaViewer/SkiaViewer.fsi` for screenshot capability detail,
  live-window capture source, viewer-open status, capture availability, first
  frame presentation, and viewer/evidence workflow request-result contracts.
- `src/Testing/Testing.fsi` for report validators, screenshot evidence
  validators, host warning classification helpers, report schemas, and pure
  evidence workflow transitions if Testing owns those helpers.

Any public surface change requires semantic tests through the public signature
and a later package surface baseline refresh.

## Generated Product Impact

Generated samples, template docs, fragment READMEs, and public generated-app
docs must:

- Avoid app-domain geometry recommendations named only `Rect`, `Point`, or
  `Size` when scene/layout primitives are in scope.
- Prefer domain-specific names such as `WorldRect`, `WorldPoint`,
  `TrackBounds`, `CarPose`, and `CheckpointBounds`.
- Preserve existing interactive launch, bounded first-frame, deterministic
  render, screenshot, and unsupported screenshot paths.
- Recommend a Linux detached-session launch pattern that preserves logs,
  redirects stderr, and detaches standard input from `/dev/null`.

## MVU / Effect Boundary Applicability

Generated gameplay MVU workflows remain unchanged. Principle IV applies to
new or clarified screenshot, launch, warning, and evidence workflows:

- Owned state belongs in `EvidenceWorkflowModel`.
- User actions, host responses, and transitions belong in `EvidenceWorkflowMsg`.
- Viewer launch, screenshot capture, process output collection, file writes,
  and generated guidance validation belong in `EvidenceWorkflowEffect`.
- `init` and `update` must stay pure; filesystem, process, window-system,
  network, wall-clock, and mutable global work stays at interpreter edges.

## Synthetic Limitations

Synthetic screenshot success is not acceptable. Deterministic render evidence
is real render evidence, but it is only fallback or diagnostic evidence and
must not be claimed as live screenshot proof.

Synthetic malformed readiness report fixtures are allowed only for the
design-approved `[SEH]` error-handling task T008. Any implementation-time
synthetic evidence must use `[S]` status and Principle V code/test/inventory
disclosures.

## Risk Levels

- Small: one focused docs, guidance, or validator wording change. Verify with
  touched tests and the named readiness artifact.
- Medium: generated template, public guidance, or validation workflow changes.
  Verify with affected package tests, `GeneratedGuidanceCheck`,
  `GeneratedProductCheck`, and `TemplateCheck`.
- Broad: public `.fsi`, package surface, generated product behavior, live
  viewer capture, or readiness/audit semantics. Verify with `Verify`,
  `PackageSurfaceCheck`, `EvidenceGraph`, and `EvidenceAudit`, while recording
  focused failures separately from aggregate results.

## Required Evidence

- `readiness/baseline-status.md`
- `readiness/generated-guidance-validation.md`
- `readiness/screenshot-capability-detail.md`
- `readiness/screenshot-success-artifact.md`
- `readiness/host-warning-classification.md`
- `readiness/detached-launch-guidance.md`
