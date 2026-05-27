# Contract: Layout Evidence

Layout evidence is a public framework-facing contract, separate from
deterministic render metadata.

## Proof Levels

- `ReadableLayout`: includes HUD region, gameplay region, text bounds, gameplay
  bounds, and non-overlap diagnostics.
- `DeterministicRenderOnly`: includes deterministic scene metadata, hashes, or
  render readback but does not prove HUD readability.
- `UnsupportedLayoutInspection`: reports that required layout facts cannot be
  produced on the current host or with current public APIs.

## Evidence Report Fields

- `scene`
- `output-size`
- `proof-level`
- `hud-region`
- `gameplay-region`
- `text-bounds`
- `gameplay-bounds`
- `overlap-status`
- `measurement-mode`
- `unsupported-reason`
- `diagnostics`

## Compatibility Requirements

- Existing deterministic render evidence remains valid for render consistency.
- Existing metadata/hash evidence must not be relabeled as readability proof
  unless layout facts are present.
- Unsupported facts must be explicit and actionable.
- Public `.fsi` signatures and surface baselines must be updated before
  implementation exposes new types or helpers.
