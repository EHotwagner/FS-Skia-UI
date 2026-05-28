# Data Model: Racer Feedback Follow-Ups

## Feedback Item

Represents one accepted consumer-observed follow-up.

- **Fields**: `id`, `summary`, `sourceEvidencePath`, `priority`,
  `affectedArea`, `expectedChange`, `acceptanceEvidencePath`
- **Relationships**: Drives one or more Generated Guidance, Screenshot
  Evidence Result, Host Warning Classification, or Detached Launch Guidance
  records.
- **Validation**: Must link back to
  `Mailbox/2026-05-28T07-40-55+0200-top-down-racer-fs-skia-ui-feedback.md`
  and to at least one readiness artifact.

## Generated Guidance

User-facing generated sample, docs, or validation text.

- **Fields**: `path`, `topic`, `recommendedExamples`,
  `forbiddenRecommendations`, `validationCommand`, `evidencePath`
- **Validation**: Geometry guidance must include at least three domain-specific
  examples and must not recommend app-domain examples named only `Rect`,
  `Point`, or `Size` when scene/layout primitives are in scope.
- **Relationships**: Validated by guidance tests and readiness file
  `generated-guidance-validation.md`.

## Screenshot Evidence Result

Machine-readable evidence for live screenshot capture, unsupported capture, or
failed capture.

- **Fields**: `status`, `evidenceKind`, `artifactPath`, `width`, `height`,
  `captureSource`, `firstFramePresented`, `viewerOpenStatus`,
  `captureAvailability`, `unsupportedReason`, `fallbackKind`, `diagnostics`
- **Validation**: Successful screenshot proof requires `status=ok`,
  `evidenceKind=screenshot`, PNG artifact path, positive dimensions,
  `firstFramePresented=true`, and a live-window capture source. Unsupported
  results must not contain successful PNG proof fields and must preserve
  capability detail where available.
- **State transitions**:
  - `not-started -> viewer-opened -> first-frame-presented -> captured`
  - `not-started -> viewer-open-failed -> unsupported-or-failed`
  - `viewer-opened -> capture-unavailable -> unsupported`
  - `viewer-opened -> capture-failed -> failed`

## Host Warning Classification

Evidence classification for host warnings observed during launch.

- **Fields**: `rawWarningText`, `normalizedWarningKey`, `classification`,
  `requiresFirstFrameSuccess`, `relatedFailure`, `evidencePath`
- **Validation**: `colorreload-gtk-module` and
  `window-decorations-gtk-module` warnings may be `benign-host-warning` only
  when first-frame evidence succeeds and no unrelated failure is present. Raw
  warning text must be preserved.
- **Relationships**: Appears in launch/readiness evidence and is validated by
  host-warning classifier tests.

## Detached Launch Guidance

Generated guidance for running GUI apps in the background.

- **Fields**: `platform`, `commandPattern`, `logPath`, `stdinHandling`,
  `stdoutHandling`, `stderrHandling`, `unsupportedPatterns`, `evidencePath`
- **Validation**: Linux guidance must include detached session handling, log
  capture, and standard input redirected away from the terminal. Simple
  terminal detachment must not be presented as the reliable default.

## Readiness Evidence Artifact

One required feature-level acceptance artifact.

- **Fields**: `path`, `purpose`, `producerCommand`, `requiredFacts`,
  `hostScope`, `reviewStatus`
- **Validation**: All required readiness paths from the spec must exist before
  acceptance. Screenshot acceptance requires real success on at least one
  supported Windows or Linux desktop host plus explicit capability or deferral
  evidence for the other supported OS if unavailable.
