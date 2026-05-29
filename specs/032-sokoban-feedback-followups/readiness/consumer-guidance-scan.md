# Consumer Guidance Scan

- Command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj`
- Files scanned: `docs/generated-apps.md`, `template/base/docs/product.md`, `docs/evidence.md`.
- Keyboard keys: found `FS.Skia.UI.KeyboardInput.ViewerKey`, `ArrowLeft`, `ArrowRight`, `Escape`, `ViewerKeyboard.normalize`, `ViewerKeyboard.normalizeEvent`, and `ViewerKeyboard.toKeyId`.
- Host callbacks: found `Viewer.GeneratedAppHost`, `Init`, `Update`, `View`, `OnTick`, `OnKey`, and `ShouldClose`.
- Viewer effects: found `OpenWindow`, `ApplyWindowOptions`, `RenderScene`, `DispatchInput`, `CloseWindow`, `EmitDiagnostic`, `CaptureScreenshot`, `CaptureImageEvidence`, and `ReadPixels`.
- Adapter commands: found `DispatchHostCommand` and `DispatchViewer`.
- Scene nodes: found `Scene.empty`, `Scene.group`, `Scene.rectangle`, `Scene.circle`, `Scene.text`, `Scene.textRun`, `Scene.line`, and `Scene.path`.
- Explicit font warning: found guidance to use explicit fonts for brand or typography guarantees beyond default readability.
- Follow-up classification: guidance distinguishes framework behavior, generated-app guidance, Spec Kit guidance, and consumer-author mistake.

