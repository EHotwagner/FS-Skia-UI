# Real-image / render evidence — feature 093

This feature is **deterministic render-only** (the spec's "no live window
required"). Parity and distinctness are proven by structural `Scene` /
`ResolvedStyle` equality — `SceneEvidence`'s render functions are deterministic
capability-hash functions, not pixel encoders — so a live windowed pixel-PNG
capture path is explicitly out of scope here.

| Field | Value |
|-------|-------|
| evidence-kind | structural-scene-equality (deterministic render-only; not a pixel screenshot) |
| status | not-applicable — render-only feature, no live desktop window claimed |
| artifact-decodable | yes — the captured `readiness/parity/*.scene.txt` baselines are plain-text decodable structural `Scene` dumps |
| proves-scene-rendering | yes — the migrated Button/CheckBox paint is structurally-`Scene`-equal to the frozen procedural baseline (`Feature093ParityTests`) |
| proves-desktop-visibility | no — a render-only structural artifact (and pixel-readback alone) cannot prove desktop visibility; no live window is asserted |

Decodable structural-`Scene` evidence proves the resolver-driven scene; it does
not — and does not claim to — prove desktop visibility. That is consistent with
the feature's render-only scope (E4 focus/keyboard delivery and a live windowed
pixel-PNG path are deferred follow-ups).
