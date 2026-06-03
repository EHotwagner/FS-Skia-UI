# Specification Quality Checklist: Single-Source the Duplicated Governance Corpus

**Purpose**: Validate specification completeness and quality before proceeding to planning
**Created**: 2026-06-03
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
- **Content-quality caveat (intentional):** this is a governance-tooling feature
  whose "users" are framework maintainers and the Codex/Claude agents consuming the
  corpus, so the spec names governance artifacts (`Guidance.fs`, scanners,
  `RefreshSurfaceBaselines`, currency gates) by name. These are the *subject* of
  the feature (the things being deduplicated), not leaked implementation choices —
  the spec deliberately does **not** prescribe the F# design of the solution
  (record shapes, splice mechanics, new types), leaving that to `/speckit-plan`.
- The two structural risks are explicit and testable: (1) opening a silent
  drift hole when a hand-carried copy becomes a generated copy — guarded by FR-003
  / SC-005 (every generated artifact paired with a currency gate) and FR-005 /
  SC-004 (the new generated-copy-drift red→green case); (2) accidentally
  single-sourcing content that legitimately differs per file — guarded by FR-011.
- SC-002 deliberately sets **no fixed line target** (consistent with 056's honest
  accounting); the success measure is "reduction attributable to collapsed
  duplication, not dropped rules," verified against the FR-001 catalogue.
