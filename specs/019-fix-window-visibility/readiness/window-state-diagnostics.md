# Window State Diagnostics Evidence

status=diagnostic-recorded
native-handle=observed:true
visible=observed:true
focusable=unsupported
renderable-surface=observed:true
input-devices=observed:true
diagnostic-class=environment-session
diagnostic-class=window-visibility
diagnostic-class=app-lifecycle
diagnostic-class=product-defect

Status: focused US2 implementation evidence captured.

## Commands

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj`
  - Evidence: `readiness/logs/t027-t028-skiaviewer-diagnostics-implementation-tests.txt`
  - Result: 38 passed, 0 failed.
- `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj`
  - Evidence: `readiness/logs/t029-generated-window-diagnostics-tests.txt`
  - Result: 16 passed, 0 failed.
- Focused audit fixture runs:
  - Evidence: `readiness/logs/t030-focused-summary.txt`
  - Result: 3 focused audit cases passed.

## Invalid State Matrix

| Case | status | diagnostic-class | visible | focusable | minimized | client-size | renderable-surface | result |
|------|--------|------------------|---------|-----------|-----------|-------------|--------------------|--------|
| taskbar-only | failed | window-visibility | observed:false | observed:false | observed:false | 640x480 | observed:true | inaccessible-window |
| hidden | failed | window-visibility | observed:false | unsupported | observed:false | 640x480 | observed:true | inaccessible-window |
| minimized-only | failed | window-visibility | observed:true | observed:true | observed:true | 640x480 | observed:true | inaccessible-window |
| off-screen | failed | window-visibility | observed:true | observed:false | observed:false | 640x480 | observed:true | inaccessible-window |
| unmapped | failed | window-visibility | observed:false | observed:true | observed:false | 640x480 | observed:true | inaccessible-window |
| zero-sized | failed | product-defect | observed:true | observed:true | observed:false | 0x0 | observed:true | inaccessible-window |
| surface-less | failed | product-defect | observed:true | observed:true | observed:false | 640x480 | observed:false | inaccessible-window |
| unsupported session | unsupported | environment-session | unsupported | unsupported | unsupported | unavailable | unsupported | unsupported |

## Native Facts

The public diagnostic record now carries:

- `native-handle=observed:true|observed:false|unsupported|unavailable`
- `visible=observed:true|observed:false|unsupported|unavailable`
- `focusable=observed:true|observed:false|unsupported|unavailable`
- `focused=observed:true|observed:false|unsupported|unavailable`
- `minimized=observed:true|observed:false|unsupported|unavailable`
- `maximized=observed:true|observed:false|unsupported|unavailable`
- `client-size=<width>x<height>|unavailable`
- `renderable-surface=observed:true|observed:false|unsupported|unavailable`
- `input-devices=observed:true|observed:false|unsupported|unavailable`
- `backend=<backend>|none`

Silk.NET interpreter evidence currently observes initialization, native handle availability, client size, renderable-surface availability, backend label, and input-device availability. Focus/focusable/minimized/maximized remain `unsupported` where the host API does not provide a reliable cross-platform fact.

## Failure Classes

- `environment-session`: missing or unsupported desktop/session facts before app lifecycle debugging.
- `window-visibility`: taskbar-only, hidden, minimized-only, off-screen, unmapped, or otherwise inaccessible window state.
- `app-lifecycle`: visible window diagnostics passed, but app lifecycle failed after that point.
- `product-defect`: generated product requested an invalid or unusable state such as zero-sized or surface-less output.

## Supported-Host Notes

Supported-host desktop evidence still belongs in `interactive-visible-window.md`. This file records diagnostic semantics and focused tests; it does not substitute unsupported-host-only records for visible-window readiness.

Aggregate note: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj` was non-authoritative for T030 because unrelated controls-boundary checks and FAKE cache/package setup failed. Focused audit cases were run separately and passed.
