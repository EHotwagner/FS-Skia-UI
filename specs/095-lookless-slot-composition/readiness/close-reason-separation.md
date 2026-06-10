# Close-reason separation (feature 095)

No live window is launched by this render-only feature, so neither an evidence-close nor a user-close
is observed. The fields are recorded as not-applicable, keeping the evidence-close and user-close
channels separated (an evidence close is never reported as a user close).

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

Note: deterministic render-only; no host window lifecycle to separate ([[fs-skia-evidence-mode]]).
