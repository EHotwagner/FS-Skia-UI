# Real-image evidence — NOT APPLICABLE (feature 098, R3)

evidence-kind=structural-and-dispatch
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=not-applicable
proves-desktop-visibility=not-applicable

R3 is a framework-internal **id-derivation and recovery** correction. The `Scene`, `Layout`, and computed
`Bounds` **rectangles** are byte-identical — only the `ControlId` **labels** on unkeyed bounds change
(`Kind → path`, FR-007). There is no rendered-output / geometry change, so this is **not** a persistent
graphical viewer feature and carries no persistent-launch / screenshot / `real-image` obligation (recorded
as a visible decision in T003).

Proof is the live-adapter routing seam (`routeInteractivePointer`) driving a real recovered dispatch plus
structural / property tests — not a decodable image, and not a pixel-readback (pixel-readback alone cannot
prove desktop visibility, which R3 does not assert). The actionable signal is the existing responds-vs-renders
proof primitive (E1): an inert / un-fixed build cannot produce the `us1-unkeyed-dispatch` artifact.
