# real-image-evidence — applicability (feature 121)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

Feature 121 changes the live-loop **cadence** and an idle-tick allocation, plus published docs — it does
**not** change rendered PIXELS and produces no new image/screenshot artifact. At-rest output is
byte-identical (the frame-cap bounds how often a frame is presented, not what it draws; the idle-tick
short-circuit changes cost, not pixels). There is therefore no real-image artifact to decode, and none
is claimed — proves-scene-rendering=false / proves-desktop-visibility=false. Behaviour is proven by the
deterministic unit tests (`Feature121LiveHostPacingTests`, `Feature121IdleTickTests`) and the standing
green suites under `Dev`. Pixel-readback alone could not prove desktop visibility here, and no desktop
visibility is asserted.
</content>
