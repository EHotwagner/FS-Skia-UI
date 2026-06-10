# Real image evidence (feature 095)

Feature 095 is a **deterministic render-only** structural-lowering feature. Its parity proof is pure
**structural `Scene` / lowered-`Control<'msg>` equality** — not a decoded screenshot — because the
`SceneEvidence` render functions are deterministic capability-hash functions, not pixel encoders
([[fs-skia-evidence-mode]]). No desktop image is produced, so image-decodability and desktop
visibility are recorded as not-applicable; the scene-rendering proof is the structural equality
asserted by the test suite.

evidence-kind=structural-scene-equality
status=deferred
artifact-decodable=not-applicable
proves-scene-rendering=true
proves-desktop-visibility=not-applicable
requested-image-evidence=false
artifact-kind=structural-equality

Note: the authoritative artifact is the structural Scene equality in Feature095SlotCompositionTests
(an unfilled slot-bearing control equals the captured pre-slot oracle; a wired retained frame equals
a full rebuild). A live screenshot is out of scope for this render-only feature.
