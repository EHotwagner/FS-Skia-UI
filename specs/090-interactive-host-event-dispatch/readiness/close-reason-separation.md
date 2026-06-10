# Close-Reason Separation (090)

close-reason=AppRequestedClose
user-close-observed=false
evidence-close-observed=false

## Observed

The live window (`live-host/`) self-closed via the host's `CloseWindow` effect
after presenting its evidence script — recorded as `AppRequestedClose`
(`logs/live-window-launch.txt`). This is an **app-requested** close, kept distinct
from a **user** close (`user-close-observed=false`) and from an evidence-mode close
(`evidence-close-observed=false`); no app-requested close is reported as a user
close.
