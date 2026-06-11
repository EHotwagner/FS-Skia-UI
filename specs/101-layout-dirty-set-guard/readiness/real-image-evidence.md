# Real-image evidence — applicability (feature 101, R7, T002/T003)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

R7 is **not** a persistent graphical viewer feature (recorded as a visible decision in T003): it adds a
build/test-time anti-drift guard over the existing pure `evaluateLayout` / `layoutDirtySet` /
`layoutAffectingAttrNames`. It introduces **no** new window, host-launch, or user-driven interactive
surface, and changes **no** observable rendering output — the rendered scene is **byte-identical** to
the pre-R7 path (R2 INV-1 preserved; the byte-identity is itself pinned by the unchanged feature-097
≥1000-case property, see `r2-preservation.md`).

Therefore the window-visibility / desktop-screenshot obligations do not apply and no image artifact is
produced:

- artifact-decodable=not-applicable — no image/screenshot is produced; there is nothing to decode.
- proves-scene-rendering=false — R7 makes no rendering claim; it changes no render output. (The
  existing R2 render evidence is preserved unchanged, not re-asserted by this feature.)
- proves-desktop-visibility=false — pixel-readback alone cannot prove desktop visibility, and R7 makes
  no desktop-visibility claim. The user-reachable surface is the failing build/test gate, not a window.
