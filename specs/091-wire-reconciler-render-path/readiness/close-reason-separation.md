# Close-Reason Separation (091)

close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

## Render-only posture

Feature 091 launched **no** live window in this run (internal render-path wiring; render-only
evidence — see `window-visibility.md` / `interactive-visible-window.md`), so there is no close
event to classify: `close-reason=not-applicable`. Neither a **user** close
(`user-close-observed=false`) nor an **evidence-mode** close (`evidence-close-observed=false`) is
claimed, and the two are kept distinct — no evidence-close is reported as a user-close.
