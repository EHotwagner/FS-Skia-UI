# real-image-evidence — feature 120 (US3)

evidence-kind=real-image-evidence
status=applicable
artifact-decodable=true
proves-scene-rendering=true
proves-desktop-visibility=false

Feature 120 does not change the rendered PIXELS — the `CachedSubtree` replay boundary is transparent
(the backend replays the SAME recorded draw commands, or recurses directly when disabled). The
production-painter pixels are proven real and byte-identical by `Feature120ReplayCacheTests`'s
cache-on/off parity: a scene of replay boundaries is rendered through `SceneRenderer.paintNode` to a
real `SKSurface`, PNG-encoded (decodable), and compared byte-for-byte across direct-walk, disabled
oracle, and warmed-replay renders — all identical.

artifact-decodable=true — the compared snapshots are valid PNG-encoded surfaces.
proves-scene-rendering=true — the bytes are the production `SceneRenderer` pixels, identical across
replay on/off, so replay preserves the rendered scene exactly (SC-003).
proves-desktop-visibility=false — these are surface-snapshot captures; desktop presentation is
evidenced separately by the live windowed run on display `:1` (`sample-smoke/live-host-evidence.txt`)
and by feature 119's verified GL present of the identical render path.
