# Research: Bomberman Demo Feedback Follow-ups

## Evidence Graph Invocation From Generated Checkouts

**Decision**: Generated evidence graph and audit workflows should invoke Spec Kit scripts through `bash script-path ...`, not by relying on executable file mode.

**Rationale**: Generated projects may be copied across archives, Windows filesystems, or source-control settings that do not preserve executable bits. Calling scripts through `bash` keeps the documented workflow portable and avoids manual `chmod` repair while preserving the authoritative script implementation.

**Alternatives considered**: Requiring `chmod +x` in quickstarts was rejected because it violates FR-001. Duplicating graph logic in generated projects was rejected because it would drift from the authoritative extension script.

## Verification Log Cleanliness

**Decision**: Verification runners should capture stdout/stderr as text, write logs with text APIs, and add a validation check that redirected `Verify` output contains no embedded NUL bytes.

**Rationale**: Readiness logs are reviewer artifacts. Embedded NUL blocks make diffs and terminal inspection unreliable and can break downstream scans. Text-only process capture keeps normal pass/fail logs reviewable.

**Alternatives considered**: Treating NUL-stripping as a viewer concern was rejected because the failure is in readiness log production and aggregation. Binary log attachment was rejected because `Verify` logs are intended to be plain text.

## Screenshot Evidence Probe Ordering

**Decision**: Screenshot evidence commands must attempt the real capture path or record why the real path cannot be attempted before reporting `unsupported`.

**Rationale**: A fallback-only unsupported report hides supported hosts and weakens reviewer confidence. Existing screenshot report fields already model viewer open status, first-frame status, capture availability, capture source, blocked stage, classification, category, message, and diagnostics; the plan extends validation around those fields instead of inventing a separate vocabulary.

**Alternatives considered**: Reporting unsupported based on environment prechecks alone was rejected because it can skip working capture implementations. Treating deterministic scene render as screenshot proof was rejected because it does not prove desktop screenshot capability.

## Generated Game Wiring

**Decision**: Provide or refine a standard generated host wiring path that accepts app-owned pure initialization, update, view, key mapping, and tick mapping, then adapts emitted app effects to viewer effects at the host boundary.

**Rationale**: Generated game apps need a repeatable pattern that launches persistently, renders frames, maps keyboard and tick input, and keeps viewer/native/file effects out of pure app transitions. Existing `GeneratedAppHost<'model,'msg>` and `Viewer.runApp`/`runAppEvidence` contracts are close to the desired boundary and should be reused or lightly extended.

**Alternatives considered**: App-specific boilerplate in every generated product was rejected because it caused the feedback item. Moving viewer effects into pure game updates was rejected by the MVU/effect boundary.

## Scene And Layout Record Authoring

**Decision**: Consumer guidance should prefer explicit constructors, helper functions, or type annotations for records with overlapping field names: coordinates, dimensions, diagnostics, state, and positions.

**Rationale**: F# record inference can choose the wrong record when nearby modules expose common fields such as `X`, `Y`, `Width`, `Height`, `Message`, `Status`, `State`, or `Position`. Local construction helpers and annotations keep samples stable and teach generated-app authors how to disambiguate without broad abstractions.

**Alternatives considered**: Renaming public records broadly was rejected as unnecessary compatibility churn. Relying on compiler errors alone was rejected because generated guidance should prevent predictable mistakes.

## Capability Skills

**Decision**: Task generation should assign the minimal matching skills per task: `fs-skia-skiaviewer`, `fs-skia-testing`, `fs-skia-elmish`, `fs-skia-scene`, `fs-skia-layout`, `fs-skia-keyboard-input` where key mapping changes, and `fs-skia-layout-evidence` for generated game evidence and guidance.

**Rationale**: The constitution requires task `skillist` metadata, and these skills map directly to the planned source ownership.

**Alternatives considered**: A single broad skill for all tasks was rejected because implementation gates require precise skill loading and ownership.
