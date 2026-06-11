# Real-image evidence — NOT APPLICABLE (feature 097, R2)

evidence-kind=structural-equality
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=not-applicable
proves-desktop-visibility=not-applicable

R2 makes no real-image / screenshot claim. It is performance-and-metric-only; the visible output never
changes (FR-008). Proof is structural `Bounds`/`Scene` equality and the FsCheck equivalence invariant on
the deterministic evaluators — not a decodable image, and not a pixel-readback (pixel-readback alone
cannot prove desktop visibility, which R2 does not assert).
