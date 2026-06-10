# Close-Reason Separation (094)

status=not-applicable
close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

## Why not-applicable

This feature opens no window and runs no interactive session of its own (the focus reducers and
the `routeFocusedKey` route-probe are deterministic/offscreen), so there is no window close
transition to classify. The deterministic test suites run to completion and exit; no user-close and
no evidence-close is observed. No evidence-close is reported as a user close, and no user close is
fabricated.
