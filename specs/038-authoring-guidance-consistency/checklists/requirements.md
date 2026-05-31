# Specification Quality Checklist: Authoring Guidance Consistency

**Purpose**: Validate specification completeness and quality before proceeding to planning
**Created**: 2026-05-30
**Feature**: [spec.md](../spec.md)

## Content Quality

- [x] No implementation details (languages, frameworks, APIs)
- [x] Focused on user value and business needs
- [x] Written for non-technical stakeholders
- [x] All mandatory sections completed

## Requirement Completeness

- [x] No [NEEDS CLARIFICATION] markers remain
- [x] Requirements are testable and unambiguous
- [x] Success criteria are measurable
- [x] Success criteria are technology-agnostic (no implementation details)
- [x] All acceptance scenarios are defined
- [x] Edge cases are identified
- [x] Scope is clearly bounded
- [x] Dependencies and assumptions identified

## Feature Readiness

- [x] All functional requirements have clear acceptance criteria
- [x] User scenarios cover primary flows
- [x] Feature meets measurable outcomes defined in Success Criteria
- [x] No implementation details leak into specification

## Notes

- Items marked incomplete require spec updates before `/speckit-clarify` or `/speckit-plan`.
- Grounding was verified against the repo: the framework source IS present
  (`src/*.fsi`) and the template lives at `template/` (singular), so framework
  public-contract changes (FR-008, FR-010) and template changes (FR-004, FR-007)
  are all actionable here. An earlier draft wrongly treated framework source as
  upstream/out-of-scope; that was corrected.
- Verified live issues: `speckit-debug-loop` dangling
  (`speckit-tasks/SKILL.md:145,149`); tetris in starter tests
  (`template/base/tests/Product.Tests/Tests.fs:430-431`); no
  `[<RequireQualifiedAccess>]` on `ViewerWindowStartupState`
  (`src/SkiaViewer/SkiaViewer.fsi:44-48`); tuple-heavy scene constructors
  (`src/Scene/Scene.fsi:322-332,410`); no canonical effects-boundary doc; no
  local API reference bundled into generated projects.
- Already resolved by feature 037 (now regression-guards only, not new work):
  evidence-gate feature.json targeting and non-incidental triggering
  (`build.fsx:290-338`) → FR-011. The author-reported `tddemo1-widgets`/
  `fs-skia-layout` traps were generated-project artifacts, not reproducible in
  current source → addressed as the FR-001/FR-002 resolution guard rather than
  one-off renames.
- FR-004 ("local API reference") and FR-008/FR-010 mechanisms intentionally state
  the *outcome* and defer the *form* to `/speckit-plan`; documented in the
  Assumptions section rather than left as clarification markers.
- Explicitly out of scope per the user: mouse/pointer input, headless raster
  backend, dotnet fsi window/font usability.
- Clarified 2026-05-30 (see spec `## Clarifications`): consumer project has
  absolute priority (new governing SC-001); FR-004 = bundle real `.fsi`
  signatures; FR-008 may be breaking with a migration note + version bump +
  updated generated samples; FR-001/FR-011 deprioritized to P3, never blocking
  consumer work.
