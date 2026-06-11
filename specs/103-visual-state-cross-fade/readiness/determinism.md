# Cross-fade determinism — identical injected-delta sequences ⇒ identical frames (feature 103, SC-004/INV-4)

evidence-kind=determinism
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=RetainedRender.advance (Tick) + RetainedRender.step over a replayed injected-delta sequence
wall-clock-consulted=false
time-source=injected per-frame TimeSpan delta only
fscheck-cases=60
fixed-sequence-frames=7
two-runs-identical=true
edge-non-positive-delta=no-op (never rewinds)
edge-past-duration-delta=settles canonically (no overshoot in any channel)
authoritative-test=Feature103CrossFadeTests/103 US2 the cross-fade is deterministic under injected deltas
