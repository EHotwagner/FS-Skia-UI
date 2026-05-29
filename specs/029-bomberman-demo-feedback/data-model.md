# Data Model: Bomberman Demo Feedback Follow-ups

## Generated App

Represents a project created from the FS.Skia.UI template.

**Fields**

- `profile`: Template profile such as `app`, `governed`, `headless-scene`, or `sample-pack`.
- `sourceRoot`: Generated project root.
- `commands`: Supported generated commands: `Dev`, `Test`, `Verify`, `EvidenceGraph`, `EvidenceAudit`, and explicit evidence flags.
- `capabilities`: Selected FS.Skia.UI capabilities and local skills.

**Validation rules**

- Evidence graph/audit commands must run without executable-mode repair.
- Default graphical launch remains persistent and evidence-free.
- Explicit evidence commands remain separate from normal launch.

## Evidence Workflow

Represents a documented command path that creates readiness evidence.

**Fields**

- `command`: Exact command line.
- `target`: FAKE target or explicit generated command.
- `authority`: `framework`, `generated-command`, or `delegated-authoritative`.
- `outputPath`: Readiness artifact path.
- `status`: `ok`, `unsupported`, or `failed`.
- `diagnostics`: Actionable text lines.

**Validation rules**

- Logs are valid text and contain no embedded NUL bytes.
- Unsupported status must not hide skipped real capability paths.
- Failed implementation errors exit non-zero and preserve blocked stage/classification where available.

## Screenshot Evidence Report

Represents screenshot capture proof from generated commands.

**Fields**

- `status`: `ok`, `unsupported`, or `failed`.
- `command`: Usually `--screenshot-evidence`.
- `appOrSample`: App/sample identity.
- `hostFacts`: Host facts relevant to capture support.
- `captureMode`: Capture mode such as `ViewerRenderTargetPng`.
- `captureAvailability`: `CaptureAvailable`, `CaptureUnavailable`, or `CaptureAvailabilityUnknown`.
- `captureSource`: `LiveViewerWindow`, `DeterministicSceneRender`, `PixelReadbackSource`, or `NoCaptureSource`.
- `viewerOpenStatus`: Viewer open result.
- `firstFrameStatus`: First-frame result.
- `screenshotPath`: Captured artifact path when present.
- `pixelContentValidation`: Nonblank/blank/unreadable/not-validated result.
- `blockedStage`, `classification`, `category`, `message`: Failure classification fields.
- `fallback`: Fallback kind when unsupported.

**Validation rules**

- `ok` requires a screenshot artifact under readiness, decodable image dimensions, nonblank content, and `proves-screenshot=true`.
- `unsupported` requires proof that real capture availability was checked or a concrete reason it could not be checked.
- App-command implementation errors are `failed`, not `unsupported`.

## Viewer Host Wiring

Represents the standard generated game boundary between pure app state and viewer effects.

**Fields**

- `init`: Pure app initial model and app effects.
- `update`: Pure app message transition returning next model and app effects.
- `view`: Pure model-to-scene function.
- `mapKey`: Viewer key and key state to optional app message.
- `tick`: elapsed time to optional app message.
- `adaptEffects`: Host-only adaptation from app effects to viewer/file/native effects.
- `run`: Persistent launch through viewer host.

**Validation rules**

- Pure update must not perform viewer, filesystem, process, screenshot, or native work.
- Host adaptation must preserve app effects separately enough for tests and diagnostics.
- Persistent launch, key input, tick input, rendering, and evidence launch must all be testable.

## Scene/Layout Authoring Guidance

Represents public examples and helper patterns for record-heavy construction.

**Fields**

- `category`: `coordinates`, `dimensions`, `diagnostics`, `state`, or `positions`.
- `ambiguousFields`: Field names likely to overlap.
- `recommendedPattern`: Type annotation, module-qualified helper, or constructor helper.
- `examplePath`: Documentation, template, or test path containing the example.

**Validation rules**

- All five categories from the feedback must be covered.
- Examples must compile or be validated by generated guidance tests.
- Helpers must not introduce viewer or host dependencies into Scene/Layout pure packages.
