# Seeded-violation proof — Constitution-Check completeness gate (US1, T012, SC-001)

**Gate**: `GeneratedGuidanceCheck` (`Guidance.runGeneratedGuidanceScan` →
`validateConstitutionCheck`). **No new FAKE target** (A5).
**Authoritative command**: `./fake.sh build -t GeneratedGuidanceCheck`
**Subject**: this feature's own `specs/046-foundations-rule-codification/plan.md`.

This is a real seeded failure, not a synthetic fixture: the live gate is run against
the real active plan, an area is genuinely blanked, the build genuinely fails, and the
plan is restored.

## 1. PASS — complete plan

```
$ ./fake.sh build -t GeneratedGuidanceCheck
Finished (Success) 'GeneratedGuidanceCheck'
Status:                  Ok
```

## 2. FAIL — `Deferred scope` area body blanked

Seed: replace the `- **Deferred scope**:` body with an empty body.

```
$ ./fake.sh build -t GeneratedGuidanceCheck
Generated guidance check failed:
specs/046-foundations-rule-codification/plan.md: Deferred scope is empty or absent [constitution-check:deferred-scope]
Status:                  Failure
(exit 134)
```

The finding names **the exact area** (`Deferred scope` / `deferred-scope`) **and the
file it was expected in** (`specs/046-foundations-rule-codification/plan.md`) — FR-002.

## 3. PASS — area restored

```
$ ./fake.sh build -t GeneratedGuidanceCheck
Status:                  Ok
```

`git diff --stat specs/046-foundations-rule-codification/plan.md` → empty (plan
restored byte-for-byte; no committed evidence altered).

## Unit-test corroboration (typed, no string matching)

`tests/Governance.Tests/ConstitutionCheckTests.fs` asserts the typed
`ConstitutionCheckResult` / `ValidationFinding` values for all-filled (pass), each
unfilled variant (empty / boilerplate / placeholder → finding naming the area id),
N/A-with-rationale (filled), and the unrecognized-template-revision diagnostic.
