# Real-image evidence — applicability (feature 102, R8, T002/T003)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

R8 is **not** a persistent graphical viewer feature (recorded as a visible decision in T003): it is a
documentation/internal-comment honesty pass that reconciles roadmap report prose and adds descriptive
source comments. It introduces **no** new window, host-launch, or user-driven interactive surface, and
changes **no** observable rendering output — the rendered scene is **byte-identical** to the pre-R8
path.

Therefore the window-visibility / desktop-screenshot obligations do not apply and no image artifact is
produced:

- artifact-decodable=not-applicable — no image/screenshot is produced; there is nothing to decode.
- proves-scene-rendering=false — R8 makes no rendering claim; it changes no render output.
- proves-desktop-visibility=false — pixel-readback alone cannot prove desktop visibility, and R8 makes
  no desktop-visibility claim. The user-reachable surface is the reconciled prose and comments, not a
  window.
