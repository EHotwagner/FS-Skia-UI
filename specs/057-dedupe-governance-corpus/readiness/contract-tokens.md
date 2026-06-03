# Contract-Token & Obligation Survival (US2, FR-004/FR-005, SC-003/SC-006)

100% of governed tokens and obligations are preserved after the single-sourcing
migration. `build/Governance/Guidance.fs` remains the single home of the rule set;
its `ContractToken` / `GuidanceObligation` / `Forbidden` inventory is byte-unchanged
(only the *carriage* of the echoes changed).

## Authoritative confirmation

`./fake.sh build -t GeneratedGuidanceCheck` → `Status: Ok` over the regenerated
corpus. The gate's `evaluateGuidanceCheck` asserts, per home file:

- every `ContractToken` is present (`[SEH]`, `synthetic-error-handling-approved`,
  the controls package/type tokens, the skillist tokens, the implement fields, …);
- every `GuidanceObligation` resolves under its `AnyOf`/`AllOf` mode (skillist
  structure/ordering, confidence fields, skill breadth, persistent-launch,
  seh-discipline, skill gates, constitution skill gates, …);
- no `Forbidden` term re-entered the combined controls guidance.

A green run means all of the above hold simultaneously over the post-migration
corpus, so no token or obligation was lost (SC-003/SC-006).

## Echo deletion did not drop a governed token

The deleted `Exact skill phrases for scans:` / `Exact readiness phrases for scans:`
echoes were **not** `Guidance.fs` contract tokens — no compiled scanner read them
(verified by `grep` over `build/Governance/**`). Their only consumer was the
`AsteroidsFeedbackSkillGuidanceTests` literal-substring scanner, now reading the
canonical natural prose with incidental line-wrapping normalized
(`expectFileContainsNormalized`, FR-006). The 20 governance tests covering these
files pass:

```
Passed!  - Failed: 0, Passed: 20  (Asteroids + GovernedBlocks + ConstitutionFragments)
```

## Preserved negatives still bite

The three 056 mutations still fail `GeneratedGuidanceCheck` with a file+rule
diagnostic (the `Guidance.fs` rule set is unchanged); see `dedupe-red-green.md`
and the preserved negatives in
`tests/Governance.Tests/GuidanceValidatorTests.fs`.
