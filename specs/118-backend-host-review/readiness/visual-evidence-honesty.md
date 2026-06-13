# visual-evidence-honesty — feature 118 (US1)

status=applicable

Feature 118 produces real visual evidence and is explicit about what it does and does not prove:

- The two captures (`smoke/direct-frame.png`, `smoke/offscreen-frame.png`) are real, decodable,
  byte-identical PNGs of the production scene rendered through the on-demand offscreen readback
  routine (FR-004). They **prove scene rendering** and present-mode-independent visual output.
- They are **pixel-readback** captures, so per the evidence contract they do **not** prove
  desktop visibility on their own (`real-image-evidence.md`: proves-desktop-visibility=false).
  Desktop presentation is evidenced separately by the windowed run presenting 40 frames on the
  real backend.
- The benign/blocking host-warning classification is honest: the single `DirectToSwapchain`
  `Warning` is a **benign** binding-limitation degradation (safe fallback to readback), not a
  blocking product defect. The `OffscreenReadback` run emits no warnings.
- No screenshot is presented as proof of the readback-free direct present path — that path is
  blocked upstream and not exercised (`audit/present-path-audit.md`). No visual claim overstates
  what the artifact shows.
