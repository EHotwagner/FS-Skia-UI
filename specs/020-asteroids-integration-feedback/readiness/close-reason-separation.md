# Close Reason Separation Evidence

close-reason=user-close
user-close-observed=true
evidence-close-observed=false
self-closed-for-evidence=false
mode=interactive-window

Source evidence:

- `readiness/generated-consumer-validation/persistent-launch-diagnostics.log`
- `readiness/supported-host-persistent-launch.txt`

The supported-host launch path records a user close separately from bounded
evidence command completion. Bounded layout and image evidence do not stand in
for interactive close behavior.
