# Quickstart: Window Startup Options & Consumer Follow-ups

FSI-first exercise of the changed surface, then the escalated validation order.

## 1. Exercise the window surface in FSI (Principle I)

Load the packed `FS.Skia.UI.SkiaViewer` and confirm the new state and
reclassification through the same surface a consumer uses:

```fsharp
#r "nuget: FS.Skia.UI.SkiaViewer, <preview>"
open FS.Skia.UI

// New default is windowed fullscreen.
Viewer.defaultWindowBehavior.StartupState
// val it : ViewerWindowStartupState = WindowedFullscreen

// Fullscreen and windowed fullscreen both validate as honored (none Unsupported).
let req s = { Viewer.defaultWindowBehavior with StartupState = s }
Viewer.validateWindowBehavior (req ViewerWindowStartupState.Fullscreen)
Viewer.validateWindowBehavior (req ViewerWindowStartupState.WindowedFullscreen)
// every startup-state result Status = Honored
```

## 2. Confirm the live window honors the flag (real evidence, SC-001/SC-002)

From a freshly generated project on a display-capable host:

```bash
# No flag → windowed fullscreen (borderless, work-area), captured as real evidence.
dotnet run --project src/<ProjectName> -- --scene-evidence ...
# Then once per supported state:
dotnet run --project src/<ProjectName> -- --window-startup normal              ...
dotnet run --project src/<ProjectName> -- --window-startup maximized           ...
dotnet run --project src/<ProjectName> -- --window-startup fullscreen          ...
dotnet run --project src/<ProjectName> -- --window-startup windowed-fullscreen ...
```

Confirm each produces the matching real window state and none is reported
"unsupported". In a headless environment the render-only evidence path reports the
environment honestly (no false visible-window claim).

## 3. Read the complete readiness contract from shipped docs (SC-003)

```bash
sed -n '/## window-visibility/,/^## /p' docs/evidence-formats.md
```

Confirm all **seven** files appear with their tokens:
`interactive-visible-window.md`, `window-state-diagnostics.md`,
`window-options.md`, `close-reason-separation.md`, `real-image-evidence.md`,
`generated-validation.md`, `evidence-audit.md` — no decompilation, no sibling copy.

## 4. Confirm audit stdout legibility (SC-004)

Introduce a deliberate readiness gap, then:

```bash
./fake.sh build -t EvidenceAudit
```

Confirm stdout names each blocker (area, file, one-line reason, hit-file path) and
a non-misleading `diff-scan base_ref:` line — without opening any `*-hits.json`.

## 5. Honest build signals & graceful analyze (SC-006/SC-007)

```bash
./fake.sh build -t Test     # first real compile / mid-implementation green-test path
./fake.sh build -t Verify   # embeds EvidenceGraph + EvidenceAudit, then tests; hard-blocks until complete
```

Confirm the guidance states `Dev` is log-only and names `-t Test`; run
`/speckit-analyze` in a project lacking `SymbolCrossCheck` and confirm it skips-
with-notice rather than failing.

## 6. Escalated validation order (this feature, framework repo)

```bash
./fake.sh build -t Route                  # confirm escalation + minimal gate list
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Recapture surface baselines + `.claude` skill mirror + `validation.contract.yml`
via `./fake.sh build -t RefreshSurfaceBaselines` after the `.fsi` / governance /
skill edits. Run FAKE-backed targets sequentially (shared `.fake` state).
