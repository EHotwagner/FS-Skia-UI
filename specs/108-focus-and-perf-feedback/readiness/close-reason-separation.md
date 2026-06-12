# Close-reason separation — applicability (feature 108, T002/T040)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

Feature 108 opens no new window, so there is no new window close to classify. The evidence path is
render-only deterministic (`Perf.runScript`, structural Scene) plus a responds-proof; neither a user
close nor an evidence close occurs in this feature's evidence capture.
