# Close-Reason Separation (086)

status=ok
close-reason=AppRequestedClose
user-close-observed=false
evidence-close-observed=false

## Classification

The controls-family `runInteractiveApp` launch self-closed on the host's own
`Tick -> CloseWindow` request, which the runner classified as `close-reason=AppRequestedClose`
(`app-close-observed=true`). No user close occurred (`user-close-observed=false`) and the
close was **not** fabricated as a user close — the evidence-driven app request and a user
close stay separated. `evidence-close-observed=false` because the framework maps this
host-requested close to the `AppRequestedClose` reason (the app asked to close after its
first frames presented), distinct from both a user close and an `EvidenceRequestedClose`.
Source: `readiness/logs/interactive-launch.txt`.
