# Contract: Deterministic animation evidence

This contract pins how animated output is captured as render evidence, reusing the existing
deterministic-scene path. It binds Story 3 (FR-009) and the parity guarantees (FR-006/FR-007).

## Mechanism (no new evidence renderer)

```fsharp
// Sample the animation at explicit time points → distinct Scenes:
let frames =
    Animation.sampleFrames
        [ TimeSpan.Zero; TimeSpan.FromMilliseconds 150.0; TimeSpan.FromMilliseconds 300.0 ]
        animation target          // 0 = start, mid, end (= Duration)

// Render each through the EXISTING deterministic-scene evidence path:
let evidence =
    frames
    |> List.map (fun scene ->
        SceneEvidence.render
            { Scene = scene
              OutputSize = { Width = 320; Height = 240 }
              Format = Hash
              RendererMode = "deterministic-scene"
              EvidencePath = None })
```

## Contract guarantees

1. **Existing mechanism only.** Evidence is produced by `SceneEvidence.render` with
   `RendererMode = "deterministic-scene"` — no new renderer, no GPU. The
   `docs/scaffold-map.md` must-survive vocabulary (`--scene-evidence`, `SceneEvidence.render`,
   `deterministic-scene`) is unchanged.
2. **Progression.** The start, midpoint, and end samples of the Story 1 / Story 2 reference
   animations produce **distinct** hashes whose underlying property values move monotonically
   *(deterministic-sampling.md)*.
3. **Reproducibility.** Re-rendering the same animation at the same time samples — in the same
   process and in a **fresh** process — yields byte-identical evidence (SC-003)
   *(AnimationOutputTests, captured goldens under
   `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-*.txt`)*.
4. **Settled ≡ static.** The end sample's evidence equals the evidence of the static render of
   the same widget at its final value — guaranteed structurally by the identity-at-rest rule
   when the final value is the identity (FR-006/SC-004) *(settled-static-parity.md)*.
5. **Un-animated unchanged.** A scene rendered with `Animation.empty` (or with no animation at
   all) is byte-identical to today's static evidence for that scene (FR-007/SC-005).
6. **No new failure mode.** In unsupported/headless environments the sampled-frame path falls
   through the existing benign/blocking/deferred host-warning classification; it never
   introduces an uncaught failure (FR-010).
