# Close-reason separation — applicability (feature 110, T002)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

Feature 110 opens no new window, so there is no new window close to classify. The evidence path is
deterministic and headless (`Perf.runScript` count goldens + the parity/fallback seam tests); neither a
user close nor an evidence close occurs in this feature's evidence capture.
