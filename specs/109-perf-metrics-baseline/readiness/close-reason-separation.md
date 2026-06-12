# Close-reason separation — applicability (feature 109, T001/T028)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

Feature 109 opens no new window, so there is no new window close to classify. The evidence path is
render-only deterministic (`Perf.runScript` counts goldens + non-golden timing baselines); neither a
user close nor an evidence close occurs in this feature's evidence capture.
