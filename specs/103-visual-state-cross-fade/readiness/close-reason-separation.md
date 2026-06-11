# Close-reason separation — applicability (feature 103, R6, T002/T003)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

R6 opens no window, so there is no window close to classify. Evidence close and user close stay
separated trivially: neither occurs (no live host launch in this feature). This is a render-only,
GPU-free deterministic-assembly feature.
