# Capability Guidance

Recorded at: 2026-05-28T08:04:00+02:00

## T003 - fs-skia-layout-evidence

Resolved skill: `.agents/skills/fs-skia-layout-evidence/SKILL.md`

Applied guidance for this feature:

- Public evidence contracts must start in `.fsi` files before implementation.
- Screenshot and readability proof must not be inferred from deterministic
  render hashes, scene metadata, or readback-only diagnostics.
- Unsupported host or capture facts must remain explicit and actionable.
- Warning classification must preserve real launch, rendering, layout, and
  package diagnostics even when known benign host warnings are present.
- Generated and public guidance should use app-owned names and avoid collisions
  with scene/layout primitives. For this feature, domain geometry examples must
  prefer names such as `WorldRect`, `WorldPoint`, `TrackBounds`, `CarPose`, and
  `CheckpointBounds` instead of generic `Rect`, `Point`, or `Size`.
- Repository validation should prefer focused package/governance targets such
  as `GeneratedGuidanceCheck`, `GeneratedProductCheck`, `TemplateCheck`,
  `PackageSurfaceCheck`, `EvidenceGraph`, `EvidenceAudit`, and `Verify`.

Screenshot proof restrictions from the feature plan remain controlling:

- Real screenshot success requires a live viewer-window capture after first
  frame presentation, a PNG artifact, positive dimensions, and screenshot
  evidence kind.
- Deterministic render evidence remains valid only as fallback or diagnostic
  evidence and cannot be relabeled as live screenshot proof.
- Synthetic screenshot success is not acceptable.
