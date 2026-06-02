# Versioned generated-product contract — live evidence (US2, T016, SC-002/SC-003)

**Authoritative command**: `./fake.sh build -t GeneratedProductCheck`
**Raw log**: `readiness/logs/generated-product-check.log` (regenerable; gitignored)
**Discoverable header**: `readiness/generated-file-lists/summary.md`

## SC-002 / SC-003 — current generated project stays green, schema_version discoverable

`GeneratedProductCheck` → `Status: Ok`. The summary header renders the explicit contract
version and rule lifecycle:

```
# Generated Product Check

schema_version: 1.0

Generated-product structural rule lifecycle:
- `product-app-present`: required — exactly one product app project
- ... (14 rules, all `required` at schema_version 1.0) ...

Contract changelog:
- 1.0 `(baseline)` added: Structural contract codified with an explicit schema version and deprecation window (feature 046).
```

Every existing structural check is wrapped as a `Required` rule, so the gate stays
behaviour-identical (no product regression) — the current generated project validates green.

## warn → promote → fail (FR-005, deprecation window)

There are currently zero `Deprecated` rules, so the warn→promote→fail transition is proven
by the typed unit tests over the real contract model (`GeneratedProductContractTests.fs`,
asserting typed `RuleOutcome` — no string matching):

- a rule marked `Deprecated (removalVersion = 2.0)` violated while `schema_version < 2.0`
  → `Warn` **naming the removal version** (passes, not a failure);
- after the `schema_version` reaches `2.0` and the rule is promoted to `Required`
  → the same product **fails** (`Fail`);
- the typed `Changelog` records each transition, and `changelogConsistencyFindings`
  (enforced at gate time in `runScanV3GeneratedProducts`) fails the build if a breaking
  rule change forgets the version bump (FR-006, SC-011).

The live gate also routes the `required-files-present` structural violation through
`GeneratedProductContract.classifyViolation`, so the deprecation machinery is exercised on
the real product-scan path (Required → hard-fail today; a future Deprecated rule warns).
