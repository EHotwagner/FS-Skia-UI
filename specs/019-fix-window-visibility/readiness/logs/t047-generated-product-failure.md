# T047 Generated Product Failure

status=failed
task=T047
command=./fake.sh build -t GeneratedProductCheck
blocked-stage=generated-consumer-persistent-launch
failure-class=window-visibility
generated-app=artifacts/generated-products/019-fix-window-visibility/app-source/src/Product/bin/Debug/net10.0/Product

`PackLocal` completed and `TemplateCheck` passed after excluding generated readiness logs from the template identity-token scan. `GeneratedProductCheck` then blocked while running the generated app default interactive path.

Observed supported-host behavior from the user during the run: the app appeared only as a taskbar symbol and was not maximizable or interactable. This is not acceptable visible-window evidence for T047 and must remain a failure until the native/interpreter path reports this as inaccessible or produces a usable visible window.

The blocked process was terminated manually:

- `dotnet run --project src/Product/Product.fsproj --no-restore`
- `artifacts/generated-products/019-fix-window-visibility/app-source/src/Product/bin/Debug/net10.0/Product`

