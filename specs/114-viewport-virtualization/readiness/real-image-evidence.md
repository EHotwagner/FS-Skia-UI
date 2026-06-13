# real-image-evidence — applicability (feature 114, T002)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

Feature 114 is not a new persistent graphical-viewer feature (a visible decision recorded in T001): it is
a viewport-virtualization contract + two additive `FrameMetrics` fields + an additive overscan/a11y
surface + offscreen addressability on the logical model. Its stories are proven by bounded/non-scaling
materialization, default-0 byte-identity, opt-in overscan correctness, offscreen focus/selection +
boundary-crossing relocation, a11y total/position, and the deterministic
`VirtualItemsMaterialized`/`VirtualItemsTotal` goldens through Controls.Tests / Elmish.Tests, not by a
captured image. No new window, host-launch, or screenshot is introduced. artifact-decodable=not-applicable
— no image is produced. proves-scene-rendering=false — parity asserts structural `Scene` equality, no
pixel. proves-desktop-visibility=false — no desktop-visibility claim.
