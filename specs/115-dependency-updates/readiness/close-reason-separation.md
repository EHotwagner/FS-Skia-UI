# close-reason-separation — applicability (feature 115, T003)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

Feature 115 opens no new window, so there is no new window close to classify. The evidence path is
deterministic and headless (the dependency-pin + governance-asset gate runs); neither a user close nor an
evidence close occurs in this feature's evidence capture. Evidence close and user close stay separated —
an evidence-close is never reported as a user-close — but neither occurs here.
