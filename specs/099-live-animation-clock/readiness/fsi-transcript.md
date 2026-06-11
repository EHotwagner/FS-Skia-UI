# FSI transcript — the reused feature-073 sampling that drives the live transition (feature 099, R4, T020)

evidence-kind=fsi-transcript
renderer-mode=DeterministicRenderOnly
status=pass

R4 adds **no** new public function — the seam is internal host wiring (`RetainedRender.advance`/`step`,
reached by the test assemblies via `InternalsVisibleTo`; the in-assembly `Feature099AnimationSeamTests`
/ `Feature099AnimationClockTests` are the user-reachable surface for these internal stories). What the
consumer **observes** — a gradual, non-snapping transition with zero animation code — is produced by
the **reused, public** feature-073 `Animation` sampling. This transcript exercises that public
primitive against the built `FS.Skia.UI.Scene.dll` with the exact single framework default R4 uses (a
150 ms `EaseOut` opacity fade), showing the gradual per-frame sampled opacity and the settle (which is
byte-identical at rest).

```fsharp
open System
open FS.Skia.UI.Scene
// The single framework default R4 starts on a visual-state flip: a 150 ms EaseOut opacity fade 0 -> 1.
let fade =
    { Animation.empty with
        Opacity = Some { Start = 0.0; End = 1.0; Duration = TimeSpan.FromMilliseconds 150.0; Easing = EaseOut } }
let sampleOpacity (e: TimeSpan) =
    match fade.Opacity with Some t -> Tween.sample Animation.lerpFloat e t | None -> 1.0
for f in 0..9 do
    let e = TimeSpan.FromMilliseconds(float f * 16.0)
    printfn "  %3d ms -> %.4f  settled=%b" (int e.TotalMilliseconds) (sampleOpacity e) (Animation.isSettled e fade)
```

Output (deterministic; injected `TimeSpan` samples only, no wall-clock):

```
elapsed(ms) -> sampled opacity (the gradual transition the host samples each frame):
    0 ms -> 0.0000  settled=false
   16 ms -> 0.2871  settled=false
   32 ms -> 0.5132  settled=false
   48 ms -> 0.6856  settled=false
   64 ms -> 0.8115  settled=false
   80 ms -> 0.8984  settled=false
   96 ms -> 0.9533  settled=false
  112 ms -> 0.9837  settled=false
  128 ms -> 0.9968  settled=false
  144 ms -> 0.9999  settled=false
settled-at-200ms=true
```

- The opacity rises monotonically 0 → ~1 across consecutive 16 ms frames — the **intermediate sampled
  appearances** the host paints (animates, not snaps). A no-seam build would show only the final value.
- `Animation.applyAt` at a settled sample returns the target node **unwrapped** (identity-at-rest
  lowering), so once `settled=true` the host drops the clock and the frame is byte-identical to the
  pre-R4 static render (FR-005).
- The live seam wiring this primitive into per-identity paint is exercised end-to-end (Tick advance →
  sample on paint → converge to the snapped target) in `readiness/us1-animates-vs-snaps.md`.
