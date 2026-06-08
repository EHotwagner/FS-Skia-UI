# Close-Reason Separation (080)

status=not-applicable
close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

## Why not-applicable

This renderer/governance feature opens no window and runs no interactive session, so there is
no window close transition to classify. The render-only preview generator and the `--fidelity`
decode gate run to completion and exit; no user-close and no evidence-close is observed. No
evidence-close is reported as a user close, and no user close is fabricated.
