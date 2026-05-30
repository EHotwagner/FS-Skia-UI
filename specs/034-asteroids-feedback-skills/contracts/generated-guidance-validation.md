# Contract: Generated Guidance Validation

## Scope

Repository validation proves the improved guidance is visible before `/speckit-implement`.

## Required Behavior

- Validation scans real repository guidance and generated-product guidance paths, not only synthetic fixtures.
- Validation checks for skill assignment guidance, multiple-skill examples, readiness scaffold coverage, visual evidence honesty wording, and feedback owner classification.
- Validation checks public `.fsi` XML documentation coverage and packed XML documentation inclusion when documentation surfaces are changed.
- Validation records command path, scanned files, observed terms, missing terms, advisory-only status, failure classification, and next action.
- Existing correctly authored task lists continue to pass graph validation after advisory skill suggestions are added.

## Acceptance Cues

- `GeneratedGuidanceCheck` or targeted governance tests fail before the guidance exists and pass after it is added.
- `TemplateCheck` and `GeneratedProductCheck` are used when template or generated-product outputs change.
- Package or documentation validation fails when generated XML documentation is missing, empty, undocumented for public `.fsi` members, or absent from packed NuGet artifacts.
- `EvidenceGraph` output remains graph-only and `EvidenceAudit` remains the merge-gate audit.
