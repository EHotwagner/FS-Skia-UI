# Data Model: Sokoban Feedback Follow-ups

## DefaultTextGlyphEvidence

- **Purpose**: Proves default text in screenshot captures renders as recognizable glyphs.
- **Fields**: `Command`, `HostPlatform`, `FontResolution`, `FallbackUsed`, `ScreenshotPath`, `Dimensions`, `GlyphCoverageMetric`, `SolidBlockMetric`, `PlaceholderMetric`, `Status`, `Diagnostics`.
- **Validation Rules**: `Status=ok` requires a decodable screenshot, visible non-background pixels in expected text bounds, glyph-shaped variation, and no solid-block-only or tofu-only result. Unsupported hosts must include `unsupported-host-reason` and must not claim pass.

## PersistentCloseEvidence

- **Purpose**: Proves generated app persistent launch can close cleanly through the app workflow without manual window closing.
- **Fields**: `Command`, `Mode`, `WindowOpened`, `FirstFramePresented`, `InputDispatch`, `CloseRequestSource`, `CloseReason`, `ExitPath`, `Elapsed`, `Status`, `FailureClassification`, `LogPath`.
- **Validation Rules**: Accepted evidence requires `mode=interactive-window` or equivalent persistent launch mode, real window/first-frame facts, an app-requested or user-confirmed close path, clean exit, and elapsed time under the configured threshold. Bounded smoke or evidence-only modes are diagnostic only.

## ConsumerApiMap

- **Purpose**: Gives generated demo authors the API shape needed before coding.
- **Fields**: `KeyboardKeys`, `HostCallbacks`, `ViewerEffects`, `AdapterCommands`, `SceneNodes`, `ExplicitFontWarning`, `SourcePath`.
- **Validation Rules**: The map must name supported keyboard key cases or normalization entry points, host responsibilities, viewer effects, adapter command categories, common Scene construction helpers, and when explicit fonts are required.

## ReadinessContract

- **Purpose**: Names required feature-scoped readiness evidence before audit execution.
- **Fields**: `FeatureReadinessDirectory`, `RequiredFiles`, `MandatoryTerms`, `AuditReadsFrom`, `RepositoryEvidenceDirectories`, `Status`.
- **Validation Rules**: The contract must distinguish `specs/032-sokoban-feedback-followups/readiness/` from repository-level output directories and list mandatory terms for governance risk levels, aggregate hang diagnostics, runtime limitations, and supported-host persistent launch evidence.

## TaskValidatorPitfallGuidance

- **Purpose**: Prevents known task graph validator failures during task generation.
- **Fields**: `TriggerPhraseExamples`, `DependencyFileShape`, `IndentationRules`, `SkillistRules`, `Examples`, `Status`.
- **Validation Rules**: Guidance must include at least two title wording or dependency formatting pitfalls, require one `tasks.deps.yml` entry per task, preserve exact task ids, and require visible `skillist` mirrors in `tasks.md`.

## FollowUpClassification

- **Purpose**: Keeps backlog items scoped to the correct owner.
- **Fields**: `Item`, `Classification`, `AffectedArtifacts`, `EvidencePath`, `DeferredReason`.
- **Validation Rules**: `Classification` is one of `framework behavior`, `generated-app guidance`, `Spec Kit guidance`, or `consumer-author mistake`. Any deferred item must name a reason and must not block the defined success criteria.
