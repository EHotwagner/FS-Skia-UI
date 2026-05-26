# Evidence Launch Mode Evidence

Status: US2 recorded with real bounded evidence.

## Commands

- Core focused tests: `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj -m:1 --filter "runAppEvidence|evidence launch|bounded run timeout|bounded run frame-count"`
- Generated command test: `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj -m:1 --filter "generated evidence commands"`
- Generated launch evidence command: `dotnet run --project artifacts/generated-products/018-persistent-gui-runtime/app-source/src/Product/Product.fsproj --no-restore -- --launch-evidence specs/018-persistent-gui-runtime/readiness/logs/t028-generated-app-real-launch-evidence-output-after-cache-clear.txt`
- Generated product check: `./fake.sh build -t GeneratedProductCheck`

## Evidence Logs

- `readiness/logs/t024-fsi-packed-evidence-launch.txt`
- `readiness/logs/t025-generated-evidence-command-red.txt`
- `readiness/logs/t026-skiaviewer-evidence-tests.txt`
- `readiness/logs/t027-generated-evidence-command-green.txt`
- `readiness/logs/t027-generated-launch-evidence-command.txt`
- `readiness/logs/t027-generated-launch-evidence-output.txt`
- `readiness/logs/t024-real-bounded-evidence-tests.txt`
- `readiness/logs/t028-generated-app-real-launch-evidence-command-after-cache-clear.txt`
- `readiness/logs/t028-generated-app-real-launch-evidence-output-after-cache-clear.txt`
- `readiness/logs/t028-generated-product-check-real-bounded-after-cache-fix.txt`
- `readiness/generated-consumer-validation/bounded-smoke.txt`
- `readiness/generated-product-validation.md`

## Outcome Fields

The generated launch evidence output recorded:

```text
status=ok
mode=persistent-evidence
command=--launch-evidence
self-closed-for-evidence=true
first-frame-presented=True
input-dispatch=not-required
window-opened=true
renderer-mode=skia
user-close-observed=false
exit-path=true
```

`mode=persistent-evidence` is the bounded evidence path. It is not normal interactive play and must not be used as proof that the generated executable remains open for a user session. `self-closed-for-evidence=true` means the evidence command intentionally closes after the first-frame target. `input-dispatch=not-required` means the command did not attempt to prove keyboard gameplay.

## Real Evidence Disclosure

No `FS_SKIA_ENABLE_BOUNDED_VIEWER_SIMULATION` override was used for the current
accepted evidence. `Viewer.runBounded` now opens a real bounded Silk.NET window
on this host, renders until the explicit evidence target is reached, writes
evidence, and self-closes as an evidence command rather than as interactive
play.

The generated consumer bounded smoke record reports:

```text
status=ok
smoke=bounded-viewer
frames-rendered=1
renderer-mode=vulkan
```

The generated `--launch-evidence` command reports:

```text
status=ok
mode=persistent-evidence
command=--launch-evidence
self-closed-for-evidence=true
first-frame-presented=True
input-dispatch=not-required
window-opened=true
renderer-mode=skia
user-close-observed=false
exit-path=true
```

The supported-host acceptance path is:

```text
dotnet run --project template/base/src/Product/Product.fsproj -- --launch-evidence readiness/evidence-launch-mode.txt
```

Reviewers should reject any bounded evidence output that claims
`mode=interactive-window`, `user-close-observed=true`, or interactive input
dispatch unless a separate interactive lifecycle command proves those fields.
