# Data Model: Persistent Launch Evidence

## PersistentLaunchRequest

Represents an explicit evidence-mode launch, separate from normal interactive
launch.

Fields:

- `mode`: must be `interactive-window` for supported-host persistent evidence.
- `command`: generated command or FAKE target used to start evidence mode.
- `viewerOptions`: title, initial size, and window behavior inputs.
- `evidencePath`: artifact path to write.
- `timeout`: bounded readiness timeout.
- `inputProbe`: optional key/input dispatch request.
- `closePolicy`: controlled evidence close policy.

Validation:

- `evidencePath` is required for readiness workflows.
- `timeout` must be finite.
- `closePolicy` must not alter default user launch behavior.

## PersistentLaunchArtifact

Machine-readable readiness record consumed by EvidenceAudit.

Required fields:

- `status`: `ok`, `failed`, or `unsupported`.
- `mode`: `interactive-window`.
- `command`: command line or target that produced the artifact.
- `window-opened`: boolean.
- `input-dispatch`: `verified`, `not-verified`, `not-required`, or `failed`.
- `exit-path`: boolean.
- `blocked-stage`: exact stage or `none`.
- `classification`: readiness classification.
- `category`: diagnostic category.
- `message`: actionable summary.

Additional fields:

- `first-frame-presented`: boolean.
- `window-visible`: observed, unsupported, or unavailable.
- `viewer-window-id`: stable viewer-native identity when available.
- `close-reason`: user, app, evidence, host, timeout, or failure.
- `diagnostic-source`: `real-launch`, `generic-host-probe`, or `synthetic-fixture`.
- `warnings`: host warning classifications.
- `missing-facts`: facts required but unavailable.

Validation:

- Passing supported-host artifacts require `status=ok`,
  `window-opened=true`, `first-frame-presented=true`, `exit-path=true`, and a
  present `input-dispatch` value.
- Observation failure with desktop prerequisites present must use an
  observation/capture blocked stage, not headless-only classification.
- Synthetic fixtures cannot produce `status=ok`.

State transitions:

1. Requested
2. DesktopPrerequisitesChecked
3. ProcessStarted
4. WindowOpened
5. FirstFramePresented
6. InputDispatchRecorded
7. EvidenceCloseRequested
8. ArtifactWritten
9. Completed, Failed, or Unsupported

## WindowObservationResult

Describes viewer-native and external observation facts for a launch attempt.

Fields:

- `viewerWindowOpened`: boolean.
- `viewerWindowVisible`: observed, unsupported, or unavailable.
- `viewerWindowIdentity`: title, handle, class, or backend identity when
  available.
- `externalObservationAttempted`: boolean.
- `externalWindowMatched`: boolean option.
- `captureAttempted`: boolean.
- `captureSucceeded`: boolean option.
- `blockedStage`: observation, capture, or none.
- `missingFacts`: unavailable facts.
- `message`: actionable diagnostic.

Validation:

- External observation failure cannot override viewer-owned window/first-frame
  success.
- Missing facts must be named explicitly.

## HostWarningClassification

Separates benign desktop warning noise from fatal readiness failures.

Fields:

- `rawMessage`: original warning or diagnostic text.
- `warningClass`: benign environment, launch failure, rendering failure, layout
  failure, package failure, or unknown.
- `fatal`: boolean.
- `evidencePath`: source artifact path.
- `supportingFacts`: launch/render/layout/package facts used for classification.
- `diagnostics`: explanatory messages.

Validation:

- Known benign markers are fatal if paired with concrete launch/render/layout or
  package failure facts.
- Unknown warnings do not become unsupported-host evidence without a blocked
  stage.

## GeneratedGuidanceCheck

Validates generated samples and docs for clear app-owned names and evidence
separation.

Fields:

- `sourcePath`: generated file or doc path.
- `usesQualifiedView`: boolean.
- `usesQualifiedHost`: boolean.
- `usesQualifiedUpdate`: boolean.
- `separatesLayoutAndLaunchEvidence`: boolean.
- `diagnostics`: missing or ambiguous references.

Validation:

- Examples that open framework capability namespaces must qualify app-owned
  `Product.Program.view`, `Product.Program.generatedHost`, and
  `Product.Program.update`.
- Layout evidence must not claim visible-window or screenshot proof.

## EvidenceAuditRequirement

Represents readiness files and artifact fields required before merge readiness.

Fields:

- `requiredReadinessFiles`: required path list.
- `requiredArtifactFields`: field names.
- `acceptedStatuses`: allowed status and field combinations.
- `blockingHits`: missing files, missing fields, synthetic evidence, or
  inconsistent classifications.

Validation:

- Final audit fails when any required readiness file is absent.
- Final audit fails when a supported-host artifact omits required fields or
  claims pass without real launch facts.
