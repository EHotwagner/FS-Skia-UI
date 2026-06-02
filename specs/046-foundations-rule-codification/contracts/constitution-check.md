# Contract: Constitution-Check Completeness Gate

**Feature**: 046 | **FRs**: FR-001, FR-002, FR-003 | **Host gate**: `GeneratedGuidanceCheck`
(`Guidance.runGeneratedGuidanceScan`) — no new FAKE target.

## Input

The **active feature's** `plan.md` (the feature being validated), parsed via the existing
`Guidance.markdownSections` parser. The `.specify/templates/plan-template.md` structure is
read **only** to detect an unrecognized template revision — never to derive the required
set.

## Required decision areas (canonical, hard-coded — FR-001)

Owned by the validator as a typed list of 11 **stable identifiers** (not headings).
Adding/removing an area is a code change + test.

| Id | Display name |
|----|--------------|
| `template-ownership` | Template ownership |
| `dependency-impact` | Dependency impact |
| `command-surface` | Command-surface impact |
| `generated-project` | Generated project impact |
| `evidence-paths` | Evidence paths |
| `fsi-contract` | `.fsi` / contract impact |
| `mvu-boundary` | MVU/effect boundary |
| `synthetic-evidence` | Synthetic evidence |
| `test-evidence` | Test evidence |
| `observability` | Observability |
| `deferred-scope` | Deferred scope |

## Behaviour

| Condition | Outcome |
|-----------|---------|
| All 11 areas present and filled | **PASS** |
| An area body is empty / heading-only | **FAIL**, finding names the area + the `plan.md` path |
| An area body still contains template boilerplate prompt text (`Guidance.planGuidancePrompts`) | **FAIL**, naming the area |
| An area body contains `NEEDS CLARIFICATION` / `TODO` | **FAIL**, naming the area |
| An area explicitly marked **N/A with rationale** | **FILLED → PASS** (completeness-of-decision, FR-003) |
| Live `plan-template.md` no longer maps to the typed identifiers | **FAIL** with distinct `unrecognized template revision` diagnostic (FR-003) — never a false pass |

A failure surfaces through `GeneratedGuidanceCheck` (build-failing, escalated set). Each
finding is a `Findings.ValidationFinding { ArtifactClass="constitution-check";
Path=<planPath>; Rule=<areaId>; Message=... }`.

## Acceptance (SC-001)

Live `fail → fix → pass` on this feature's own `plan.md`, plus typed unit tests in
`tests/Governance.Tests/ConstitutionCheckTests.fs` covering: all-filled pass; each
unfilled variant (empty / boilerplate / placeholder) fail naming the exact area; N/A-with-
rationale pass; unrecognized-template-revision diagnostic. Tests assert typed
`ConstitutionCheckResult` / `ValidationFinding` values — no string matching (FR-013).
