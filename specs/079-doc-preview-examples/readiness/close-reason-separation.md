# Close-Reason Separation (079)

status=not-applicable
close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

## Why not-applicable

This docs feature opens no window and runs no interactive session, so there is no window
close transition to classify. The render-only preview generator runs to completion and exits;
no user-close and no evidence-close is observed. No evidence-close is reported as a user
close, and no user close is fabricated.
