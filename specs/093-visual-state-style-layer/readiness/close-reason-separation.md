# Close-Reason Separation (093)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

## Render-only posture

Feature 093 launched **no** live window in this run (it is a pure styling-layer
change; all evidence is render-only / structural — see
`interactive-visible-window.md`), so there is no close event to classify:
`close-reason=not-applicable`. Neither a **user** close
(`user-close-observed=false`) nor an **evidence-mode** close
(`evidence-close-observed=false`) is claimed, and the two are kept distinct — no
evidence-close is reported as a user-close.
