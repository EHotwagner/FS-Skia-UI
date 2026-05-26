# Package Resolution Evidence

selected-risk-level=broad
focused-rerun-command=./fake.sh build -t GeneratedProductCheck
focused-rerun-command=dotnet restore artifacts/generated-products/018-persistent-gui-runtime/app-source/tests/Product.Tests/Product.Tests.fsproj --no-cache --source /home/developer/.local/share/nuget-local --source https://api.nuget.org/v3/index.json
exact-match=true
failure-class=none
package-source=/home/developer/.local/share/nuget-local
package-source=https://api.nuget.org/v3/index.json
requested-version=FS.Skia.UI.Controls=0.1.17-preview.1
requested-version=FS.Skia.UI.Controls.Elmish=0.1.17-preview.1
requested-version=FS.Skia.UI.Elmish=0.1.17-preview.1
requested-version=FS.Skia.UI.KeyboardInput=0.1.17-preview.1
requested-version=FS.Skia.UI.Layout=0.1.17-preview.1
requested-version=FS.Skia.UI.Scene=0.1.17-preview.1
requested-version=FS.Skia.UI.SkiaViewer=0.1.17-preview.1
resolved-version=FS.Skia.UI.Controls=0.1.17-preview.1
resolved-version=FS.Skia.UI.Controls.Elmish=0.1.17-preview.1
resolved-version=FS.Skia.UI.Elmish=0.1.17-preview.1
resolved-version=FS.Skia.UI.KeyboardInput=0.1.17-preview.1
resolved-version=FS.Skia.UI.Layout=0.1.17-preview.1
resolved-version=FS.Skia.UI.Scene=0.1.17-preview.1
resolved-version=FS.Skia.UI.SkiaViewer=0.1.17-preview.1

Evidence logs:
- `specs/018-persistent-gui-runtime/readiness/logs/t037-generated-product-check.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t040-generated-product-check.txt`
- `specs/018-persistent-gui-runtime/readiness/generated-product-validation.md`
- `specs/018-persistent-gui-runtime/readiness/logs/t050-app-source-restore.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t050-app-source-test.txt`

Current app-profile package resolution is exact against the local package feed:
restore and tests pass for the generated app source profile using
`0.1.17-preview.1` packages. `GeneratedProductCheck` now completes with exact
package resolution and authoritative generated test execution.
