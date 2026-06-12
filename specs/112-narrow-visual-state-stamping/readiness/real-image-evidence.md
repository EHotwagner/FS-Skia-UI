# real-image-evidence — applicability (feature 112, T002)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

Feature 112 is not a new persistent graphical-viewer feature (a visible decision recorded in T001): it
is a per-frame stamp mechanism change. Its stories are proven by deterministic targeted-vs-oracle scene
parity + the touched-node count through Controls.Tests, not by a captured image. No new window,
host-launch, or screenshot is introduced. artifact-decodable=not-applicable — no image is produced.
proves-scene-rendering=false — parity asserts structural Scene equality, no pixel. proves-desktop-visibility=false
— no desktop-visibility claim.
