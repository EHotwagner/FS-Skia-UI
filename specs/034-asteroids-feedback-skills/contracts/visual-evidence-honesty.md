# Contract: Visual Evidence Honesty

## Scope

Evidence guidance prevents visual proof claims from overstating screenshots, scene rasterization, or layout reports.

## Required Behavior

- Screenshot proof requires a decodable image artifact at the claimed artifact path.
- The artifact must report expected dimensions and non-trivial content before guidance may call it complete visual proof.
- Textual reports, metadata-only key/value files, and command output cannot substitute for the image artifact.
- Rasterized scene evidence and layout-bounds evidence are separate proof classes.
- Fallback or placeholder images are unsupported or incomplete proof unless they satisfy the same decodable-image, dimension, and content checks as real captures.

## Acceptance Cues

- Guidance rejects a screenshot path that contains only an ASCII report, even if the report says `proves-screenshot=True`.
- Guidance rejects a 1x1 fallback PNG as scene-rendering proof for a 640x480 expected frame.
- Layout readability evidence may support bounds, overlap, and HUD-region claims, but cannot prove visible pixels for stroke-only entities or glyphs.
