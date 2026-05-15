# Data Model: Targeted Refactor and Governance Diagnostics

## Internal Responsibility Area

- **Fields**: name, current paths, target paths or section name, owned public facade functions, owned internal helpers, surface impact, reviewer notes
- **Relationships**: Owns zero or more `StartupStage`, `NativeResourceOwnershipRule`, or governance validator responsibilities.
- **Validation Rules**: Must not require a public `Library.fsi` change. If represented by a new file, the file must have a signature strategy and surface baseline evidence.

## Internal Helper Contract

- **Fields**: file pair, visibility intent, exposed helper values, consuming implementation area, compile order, surface baseline result
- **Relationships**: Supports an `Internal Responsibility Area`.
- **Validation Rules**: New helper contracts must be declared in `.fsi` files, must not create unintended public package exports, and must not use top-level access modifiers in `.fs`.

## Native Resource Ownership Rule

- **Fields**: resource category, handle type, acquire stage, owner, transfer point, release action, release order, idempotency rule, diagnostic stage
- **Relationships**: Belongs to a `StartupStage`; produces `ResourceCleanupEvidence`.
- **Validation Rules**: Every acquired resource must have exactly one cleanup owner after acquisition and must be released exactly once on later-stage failure and shutdown.

## Startup Stage

- **Fields**: name, order, inputs, outputs, acquired resources, failure diagnostic, cleanup obligations, transfer point
- **Relationships**: Uses one or more `NativeResourceOwnershipRule` entries; participates in `StartupFailureCase`.
- **Validation Rules**: Stage names must appear in diagnostics and readiness evidence. Failure must preserve original error details.

## Startup Failure Case

- **Fields**: failed stage, resources acquired before failure, expected release sequence, observed release sequence, diagnostic severity, diagnostic stage, synthetic disclosure
- **Relationships**: Validates `StartupStage` and `NativeResourceOwnershipRule`.
- **Validation Rules**: Observed releases must match expected releases exactly once. Synthetic/instrumented acquisition must be disclosed.

## Build Organization Attempt

- **Fields**: attempted layout, files changed, target load commands, platform evidence, accepted strategy, fallback rationale
- **Relationships**: Governs `build.fsx` or split build script files.
- **Validation Rules**: Physical split is accepted only when `Dev`, `Verify`, and `Ci` load cross-platform. Otherwise the accepted strategy is named sections in one canonical `build.fsx`.

## Guidance Contract

- **Fields**: artifact type, template path, required section, required prompt, prompt scope, parity partner path
- **Relationships**: Validated by `GeneratedGuidanceCheck`.
- **Validation Rules**: Required prompts must appear under the correct section and not only in deferred scope. Active and preset templates must remain semantically equivalent for required generated artifact classes.

## Drift Path Class

- **Fields**: class name, owned path prefixes, required alignment classes, allowed deferral fields, diagnostic message
- **Relationships**: Used by `TemplateDrift` to validate changed paths.
- **Validation Rules**: A changed template-owned path must have same-diff alignment evidence and active feature spec/plan/readiness evidence naming the path or affected feature area, unless covered by an accepted deferral.

## Drift Alignment Evidence

- **Fields**: changed path, path class, alignment file, active feature evidence file, matched path or feature area phrase, deferral id
- **Relationships**: Satisfies a `Drift Path Class`.
- **Validation Rules**: Evidence must be in the same diff. Deferrals must include id, paths, rationale, owner, and target phase.

## Yoga Fallback Diagnostic Decision

- **Fields**: triggering failure, diagnostic encoding, severity, code, constraint, fallback flag, affected node/context, public surface sufficiency, follow-up id
- **Relationships**: Produces layout test evidence or `FollowUpApiProposal`.
- **Validation Rules**: If existing public fields are sufficient, the diagnostic must be observable in `LayoutResult.Diagnostics` while bounds remain safe. If insufficient, no `.fsi` change is allowed and a follow-up proposal is required.

## Public Record Invariant Decision

- **Fields**: package, record name, fields with invariants, current construction stance, recommended usage, rationale, follow-up id
- **Relationships**: Recorded in readiness inventory and validated by governance tests.
- **Validation Rules**: Every public record exported by `FS.Skia.UI`, `FS.Skia.UI.Layout`, and `FS.Skia.UI.Charts` must have an entry. Helper constructor or validation-first API recommendations require a follow-up ID.

## Verification Evidence Artifact

- **Fields**: artifact class, path, command, real or synthetic status, produced by target/test, pass/fail verdict, notes
- **Relationships**: Supports all success criteria.
- **Validation Rules**: Required evidence must not be synthetic-only unless explicitly accepted under repository policy.

## Follow-Up API Proposal

- **Fields**: id, title, blocked feature area, public surface gap, proposed next spec scope, compatibility risk, evidence link
- **Relationships**: Referenced by `Yoga Fallback Diagnostic Decision` or `Public Record Invariant Decision`.
- **Validation Rules**: Follow-up proposals must be separate from implementation and must not change current feature scope.
