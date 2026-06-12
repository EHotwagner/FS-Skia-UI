# Close-reason separation — applicability (feature 111, T002)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

Feature 111 opens no new window, so there is no new window close to classify. The evidence path is
deterministic and headless (`Perf.runScript` cause/phase goldens + the `RetainedRender` step tests);
neither a user close nor an evidence close occurs in this feature's evidence capture.
