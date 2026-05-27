# Interactive Visible Window Evidence

status=ok
mode=interactive-window
window-visible=observed:true
accessible-window=true
first-frame-presented=true
self-closed-for-evidence=false
user-close-observed=true
process-running=false
process-only=false
taskbar-entry=false

Command:

- `dotnet run --project artifacts/generated-products/020-asteroids-integration-feedback/app-source/src/Product/Product.fsproj --no-restore`

Source evidence:

- `readiness/generated-consumer-validation/persistent-launch-diagnostics.log`
- `readiness/supported-host-persistent-launch.txt`

The generated application opened an interactive desktop window on the prepared
supported host, presented its first frame, and exited after an intentional user
close. This is distinct from bounded scene evidence.
