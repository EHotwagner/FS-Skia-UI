# Deterministic sampling — Add Animations (073)

Story 3 (FR-009, SC-003): animated output is captured as deterministic evidence by
sampling the animation at explicit `TimeSpan` points and rendering each frame through
the **existing** `SceneEvidence.render` `deterministic-scene` path — no new renderer,
no GPU.

## Two complementary oracles

`SceneEvidence.render`'s `DeterministicHash` is derived from element **kinds** + size
only — it is value-insensitive by design (it proves *structure*, e.g. settled≡static).
Two frames that differ only in opacity / transform **values** therefore share a
kind-hash. The value-aware progression oracle `ParityAnimationOutput.encodeFrame`
fills that gap: it walks the sampled `SceneNode` and emits the folded opacity / alpha
and the lowered transform matrix at fixed `%.4f` (InvariantCulture) precision with
stable ordering and no environment-dependent fields.

## Start / midpoint / end distinct-hash progression

Reference entrance (opacity 0→1, translateY 24→0, ease-out, 300 ms) sampled at
`0 / 150 / 300 ms` (`tests/Parity.Tests/fixtures/v3-host-golden/scene-output/`):

| sample | matrix M23 (translateY) | painted-rect opacity | text alpha |
|--------|-------------------------|----------------------|------------|
| `animation-entrance-start.txt`   | 24.0000 | 0.0000 | 0   |
| `animation-entrance-mid.txt`     | 3.0000  | 0.8750 | 223 |
| `animation-entrance-settled.txt` | (unwrapped — identity, no PerspectiveNode) | 1.0000 | 255 |

The three encodings are **distinct** and the underlying property values move
**monotonically** (opacity strictly increasing 0→1; translateY strictly decreasing
24→0) — asserted in `AnimationOutputTests` ("underlying property values move
monotonically", "start / mid / settled frames are distinct"). Each frame also renders
through `SceneEvidence.render` (`RendererMode = "deterministic-scene"`) successfully.

## Same-process + fresh-process byte-identical re-capture (SC-003)

- **Same process**: the "re-derivation … is byte-identical (same process)" test
  encodes each sample twice and asserts equality.
- **Fresh process**: goldens were first written with `FS_SKIA_CAPTURE_GOLDEN=1
  dotnet test tests/Parity.Tests`, then re-asserted by a second `dotnet test`
  invocation (a fresh process) with the env var unset — all goldens re-derived with a
  0-byte diff. Determinism holds because sampling is pure arithmetic over an explicit
  `TimeSpan` with no wall-clock, RNG, or environment input.

## Concurrent independent animations (SC-007)

The `entrance` and `glide` reference animations sampled at the same elapsed time
produce **independent** frames (asserted "concurrent independent animations sample
without interference"): each animation is an independent value sampled against the
shared time, with no shared mutable state.

## US3 independent validation path

1. `Animation.sampleFrames [t0; tMid; tEnd] animation target` → one `Scene` per time.
2. Render each through the existing `deterministic-scene` path / encode with the
   value-aware oracle.
3. Re-render twice and on a fresh process → identical bytes.
