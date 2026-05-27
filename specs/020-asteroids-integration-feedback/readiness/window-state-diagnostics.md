# Window State Diagnostics

status=ok
mode=interactive-window
native-handle=observed:true
visible=observed:true
focusable=observed:true
renderable-surface=observed:true
input-devices=observed:true
taskbar-entry=false
unsupported-host-only=false

diagnostic-class=environment-session
diagnostic-class=window-visibility
diagnostic-class=app-lifecycle
diagnostic-class=product-defect
failure-class=none

Source evidence:

- `readiness/generated-consumer-validation/persistent-launch-diagnostics.log`
- `readiness/host-warning-classification.md`

Environment-session diagnostics were successful on the prepared supported host:
runtime directory, display socket, and session bus were present. Any future
non-visible launch must be classified before app-lifecycle debugging.
