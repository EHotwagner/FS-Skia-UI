# real-image-evidence — applicability (feature 122)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

Feature 122 changes **which swapchain buffer is presented** on idle frames (re-presenting the cached
last good frame instead of skipping the buffer swap), not **what is drawn**. The offscreen / readback
render path is untouched, so at-rest pixel output is byte-identical and there is no new image/screenshot
artifact to decode — proves-scene-rendering=false / proves-desktop-visibility=false. The visible effect
(no interleaved-black blink) only manifests on a real Wayland windowed-fullscreen compositor with a 3+
buffer swapchain, which is not available in this headless CI (see `runtime-limitations.md`); that
end-to-end visual observation is a disclosed `[-]` item, not a claimed image pass. Behaviour is proven by
the deterministic unit tests (`Feature122PresentPathTests`) and the standing green offscreen golden
suites under `Dev`. Pixel-readback alone could not prove desktop visibility here, and none is asserted.
