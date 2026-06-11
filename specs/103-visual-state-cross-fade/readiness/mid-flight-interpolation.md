# Mid-flight cross-fade — the displayed colour is strictly between the endpoints (feature 103, SC-001/INV-3)

evidence-kind=mid-flight-interpolation
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=ControlRuntime.applyRuntimeVisualState + RetainedRender.advance (Tick) + RetainedRender.step (the real runInteractiveApp seam)
representative-kind=Switch (off) — track FILL restyles Muted→Accent on Hover via Style.resolve
default-transition-duration-ms=150
easing=EaseOut
wall-clock-consulted=false
time-source=injected per-frame TimeSpan delta only
prior-endpoint-rgb=(100uy, 116uy, 139uy)
next-endpoint-rgb=(37uy, 99uy, 235uy)
sampled-elapsed-ms=75.000000
op-next=0.875000
op-prior=0.125000
prior-colour-present-mid-flight=true
next-colour-present-mid-flight=true
displayed-red=38.105263 (between 37 and 100)
lerpColor-reference-rgb=(45uy, 101uy, 223uy)
counterfactual=the pre-R6 code overlays ONLY the next own-scene fading in from transparent; the prior colour is absent mid-flight, so this prior-colour-present assertion is RED before the snapshot-composite and GREEN after.
authoritative-test=Feature103CrossFadeTests/103 US1 a visual-state transition genuinely cross-fades its colours (not a fade-in from transparent)
