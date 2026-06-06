# Runtime Limitations & Unsupported Scope — Add Animations (073)

## Unsupported scope (deferred)

This feature ships a **representative motion slice**, not a full motion system. Out
of scope and deferred: general physics/spring simulation as a system,
gesture-/input-driven interactive scrubbing, sequenced/chained timelines and
keyframe tracks, particle systems, video playback, GPU/shader visual effects, and any
layout-reflowing animation beyond the bounded transform/opacity/color set. Rewriting
the host's internal Vulkan present loop for fine-grained per-widget redraw regions is
also deferred — this feature gates redraws at the framework-request (subscription)
level only.

## Unsupported-scope handling / failure diagnostics (FR-010)

- **No new failure mode.** Sampling is total: non-positive `Duration` resolves
  immediately to the end value (no divide-by-zero) and out-of-range time samples
  clamp to the start/end endpoints. No animation path throws, hangs, or requests a
  perpetual redraw.
- **Render evidence is render-only.** Sampled frames render headlessly through the
  existing `SceneEvidence.render` `deterministic-scene` path — **no GPU window**. The
  kind-hash is value-insensitive by design, so the value-aware progression oracle
  (`ParityAnimationOutput.encodeFrame`) complements it; both are byte-identical on
  re-capture and in a fresh process.
- **No hidden state.** The author holds `AnimationState` in their own model; the
  framework owns no mutable animation registry. A removed animating widget simply
  drops its state ⇒ the tick subscription self-suspends.

## Platform runtime boundary

The render/evidence path runs on the supported runtime only:

- **.NET 10 desktop** host (Windows and Linux desktop).
- **Vulkan**-backed GPU path for the windowed host; the headless deterministic-scene
  path used here exercises the same scene vocabulary without a window.
- **SkiaSharp preview** is the pinned rendering dependency (unchanged — no new
  dependency is added by this feature).
- **unsupported macOS/mobile/browser**: these targets are out of runtime scope.
- **no software-renderer fallback**: there is no software rasterizer substitute;
  unsupported hosts are reported as unsupported, not silently downgraded.

## Non-authoritative aggregate

`GeneratedProductCheck` fails locally for an environment reason (no template
`feature.json` resolution + `Map.empty` env), independent of this change. It is a
**non-authoritative environment failure**, not a product regression; the
authoritative per-gate surface/parity checks all pass.
