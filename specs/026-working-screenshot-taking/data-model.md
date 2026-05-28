# Data Model: Working Screenshot Taking

## Screenshot Artifact

**Fields**:

- `path`: readiness-relative path to the PNG artifact
- `format`: expected `png`
- `width`: decoded image width, positive integer
- `height`: decoded image height, positive integer
- `contentValidation`: `non-blank`, `blank`, `unreadable`, or `zero-dimension`
- `captureSource`: `live-viewer-window` or another explicitly non-accepted source
- `createdAt`: timestamp recorded by the evidence workflow

**Validation rules**:

- Path must remain within the feature readiness artifact tree.
- File must exist, be readable, decode as an image, and have positive dimensions.
- Accepted screenshot proof must have non-blank visible pixel content.
- Static fixtures, deterministic render fallback, metadata, and manual paths do
  not satisfy this entity.

## Screenshot Evidence Record

**Fields**:

- `status`: `ok`, `unsupported`, or `failed`
- `command`: command that produced the record
- `appOrSample`: generated app, sample, or test identity
- `hostFacts`: desktop/session/runtime facts relevant to rendering and capture
- `captureMode`: selected capture mode
- `artifactPath`: screenshot PNG path when status is `ok`
- `imageDimensions`: decoded width and height when an artifact exists
- `pixelContentValidation`: validation result and message
- `blockedStage`: blocked stage or `none`
- `classification`: unsupported environment, product defect, package, lifecycle, or verification class
- `category`: diagnostic category such as startup, frame, renderer, screenshot, or artifact write
- `message`: concise human-readable outcome
- `timestamp`: evidence production time

**Validation rules**:

- `ok` requires a valid Screenshot Artifact and `captureSource=live-viewer-window`.
- `unsupported` and `failed` must not include a successful artifact claim.
- All records must include traceability fields even when capture does not
  succeed.

## Capture Mode

**Fields**:

- `name`: `viewer-render-target-png` for the planned mode
- `requiresViewer`: true
- `requiresFirstFrame`: true
- `requiresDesktopHost`: true for live viewer paths
- `fallbackKind`: deterministic scene evidence only as diagnostic fallback

**Validation rules**:

- Capture mode must exercise working viewer/product code.
- Fallback modes must be labeled as fallback and `provesScreenshot=false`.

## Pixel Content Validation

**Fields**:

- `decoded`: whether the file decodes as an image
- `width`: decoded width
- `height`: decoded height
- `nonBlank`: whether sampled pixels prove visible content
- `failureReason`: absent for accepted content

**Validation rules**:

- Fully transparent, all-zero, zero-dimension, unreadable, or missing files are
  rejected.
- Validation must run after file write and read the artifact that reviewers will
  inspect.

## Capture Failure Diagnostic

**Fields**:

- `status`: `unsupported` or `failed`
- `blockedStage`: desktop prerequisite, launch, first frame, render, capture,
  readback, pixel validation, artifact write, timeout, or unknown
- `classification`: failure classification
- `category`: diagnostic category
- `hostFacts`: runtime/session facts
- `command`: attempted command
- `message`: actionable explanation
- `missingEvidence`: fields or artifacts absent because capture stopped

**Validation rules**:

- Unsupported host diagnostics are not accepted screenshot proof.
- Failure diagnostics must name the earliest known blocked stage.

## Generated Screenshot Guidance

**Fields**:

- `command`: generated app screenshot evidence command
- `artifactDirectory`: readiness artifact directory
- `acceptanceRules`: required artifact and record fields
- `unsupportedBehavior`: how unsupported hosts are recorded
- `separationRules`: statement that launch/layout/scene evidence does not
  substitute for screenshots

**Validation rules**:

- Guidance must be generated for screenshot-ready visual profiles.
- Guidance must not imply metadata, deterministic scene output, or manual
  screenshots satisfy automated screenshot proof.
