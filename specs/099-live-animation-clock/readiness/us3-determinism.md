# Animation clock determinism — identical injected-delta sequence ⇒ identical output (feature 099, SC-004/FR-006)

evidence-kind=determinism
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=RetainedRender.advance (the pure clock core the host Tick wrapper calls)
wall-clock-consulted=false
time-source=injected per-frame TimeSpan delta only (no Date.now / no System clock)
fscheck-cases=1000
fixed-sequence-frames=12
run-a-elapsed-ms=150.000000
run-b-elapsed-ms=150.000000
two-runs-identical=true
edge-non-positive-delta=no-op (never rewinds)
edge-very-large-delta=clamps to duration, sample settles at End (no overshoot)
edge-retarget-mid-flight=re-aims from current sampled value (no snap to start)
edge-return-to-normal-settled=dropped to None (byte-identical at rest restored)
edge-multi-clock=each RetainedId advances its own clock independently
authoritative-test=Feature099AnimationClockTests/099 US3 determinism + edges (pure clock core)
