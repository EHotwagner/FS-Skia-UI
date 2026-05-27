# Evidence Report Conventions

## Status

status=ok
story=US5
generated-app=/tmp/fs-skia-ui-report-check
verification-command=dotnet new fs-skia-ui plus generated `dotnet run` evidence commands

## Shared Convention

All generated evidence reports below start with:

- `status`
- `command`
- `output`

Unsupported reports include `unsupported-host-reason` and
`fallback=deterministic-scene-evidence`. Generated reports use stable key-value
ordering and write the same lines to stdout and the report file.

## Layout Evidence

```text
status=ok
command=--layout-evidence
output=/tmp/fs-skia-ui-report-check/readiness/layout-evidence.txt
scene=ReportConventionCheck.Program.view
output-size=640x480
proof-level=ReadableLayout
hud-region=hud:0,0,640,96
gameplay-region=gameplay:0,96,640,384
text-bounds=4
gameplay-bounds=1
overlap-status=NoLayoutOverlap
measurement-mode=ApproximateTextBounds
accepted=True
diagnostics=hud-region=present|gameplay-region=present|measurement-mode=approximate
```

## Screenshot Evidence

```text
status=unsupported
command=--screenshot-evidence
output=/tmp/fs-skia-ui-report-check/readiness/screenshot-evidence.txt
mode=persistent-evidence
evidence-kind=screenshot
renderer-mode=skia
unsupported-host-reason=screenshot capture is unavailable for this viewer host
fallback=deterministic-scene-evidence
screenshot-path=none
width=none
height=none
frames-rendered=none
diagnostics=status=unsupported|evidence-kind=screenshot|fallback=deterministic-scene-evidence|scene-capabilities=66
```

## Pixel Readback Evidence

```text
status=ok
command=--pixel-readback-evidence
output=/tmp/fs-skia-ui-report-check/readiness/pixel-readback-evidence.txt
mode=persistent-evidence
evidence-kind=pixel-readback
supported-host=true
fallback-reason=screenshot-unavailable
board-readable=true
input-or-progress-observed=true
self-closed-for-evidence=true
input-dispatch=not-required
first-frame-presented=true
renderer-mode=deterministic-scene
scene-evidence-format=Hash
value=93d91fb31d4eca007e60e120c21ad7c3085c54c7434624bfc801d4e0bdb43e48
```

## Verification

- `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --no-restore --logger "console;verbosity=minimal"`:
  28 passed.
- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-restore --filter "Generated guidance hardening"`:
  16 passed.
- `./fake.sh build -t TemplateCheck`:
  passed; latest output saved in `readiness/template-check-us5d.log`.
