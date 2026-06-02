# Unit-test evidence — typed-result tests (US1/US2, T008/T013/T027, FR-013, SC-008)

All tests assert **typed** `ConstitutionCheckResult` / `ValidationFinding` /
`RuleOutcome` / changelog values — no report-text string matching.

**Authoritative command**: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -c Debug`

## Red → Green log

### RED (failing-first, skeleton bodies `failwith "…not yet implemented"`)

`ConstitutionCheckTests.fs` and `GeneratedProductContractTests.fs` were registered and
run against skeleton validators before implementation. Representative failures:

```
Failed ConstitutionCheck validator.all 11 areas filled => recognized, every area Filled, zero findings
  046 T010: classifyConstitutionCheck not yet implemented
Failed GeneratedProductContract.a violated Required rule => Fail
  046 T014: classifyViolation not yet implemented
Failed GeneratedProductContract.renderContractHeader exposes the schema_version
  046 T014: renderContractHeader not yet implemented
Failed GeneratedProductContract.a breaking changelog entry without a version bump is flagged
  046 T027: changelogConsistencyFindings not yet implemented
```

### GREEN (after T010 / T014 / T015 / T027 implementation)

```
Passed!  - Failed: 0, Passed: 355, Skipped: 0, Total: 355 - Governance.Tests.dll (net10.0)
```

The 355 total includes the 8 new `ConstitutionCheck validator` tests, the 11 new
`GeneratedProductContract` tests (T013 + T027), and **no regressions** in the pre-existing
336 governance tests.

## Coverage map

- **T008** (ConstitutionCheck): all-filled→pass; empty/boilerplate/placeholder→finding
  naming the area id + plan path; N/A-with-rationale→Filled; unrecognized-template-
  revision→distinct diagnostic; canonical 11-area set + order; this feature's own plan.md
  passes.
- **T013** (contract): Required→Fail; Deprecated window-open→Warn naming removal version;
  Deprecated window-closed→Fail; promoted Deprecated→Required→Fail; Removed/unknown→Pass;
  changelog records the transition; `renderContractHeader` exposes `schema_version`.
- **T027** (consistency): `current` contract is consistent; a breaking
  `PromotedToRequired` entry without a version bump is flagged; `current.SchemaVersion` ≥
  the maximum changelog entry version.
