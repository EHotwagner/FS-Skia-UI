# Close-Reason Separation (099, R4)

status=not-applicable
close-reason=not-applicable
user-close-observed=false
evidence-close-observed=false

## Why not-applicable

This feature opens no window and runs no interactive session of its own — the clock advance/sample/
retarget are pure functions of the injected delta and the live-path proofs run through the
deterministic `RetainedRender.advance`/`step` paths — so there is no window close transition to
classify. The deterministic test suites run to completion and exit; no user-close and no evidence-close
is observed. No evidence-close is reported as a user close (evidence-close-observed is never reported as
user-close-observed), and no user close is fabricated.
