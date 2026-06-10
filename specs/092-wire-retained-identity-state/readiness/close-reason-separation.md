# Close-Reason Separation (092)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

## Render-only posture

Feature 092 launched **no** live window in this run (internal interactive-state wiring; render-only
evidence — see `window-visibility.md` / `interactive-visible-window.md`), so there is no close event
to classify: `close-reason=not-applicable`. Neither a **user** close (`user-close-observed=false`)
nor an **evidence-mode** close (`evidence-close-observed=false`) is claimed, and the two are kept
distinct — no evidence-close is reported as a user-close.
