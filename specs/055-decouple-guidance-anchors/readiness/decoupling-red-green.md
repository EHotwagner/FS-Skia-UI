# Decoupling Red→Green Transcript

Proof that author-guidance prose is decoupled from generation-currency anchors:
rewording that preserves a semantic obligation **passes** (US1), while
source-of-truth drift still **fails** (US2). Captured from the pure-core unit
tests in `tests/Governance.Tests/GuidanceValidatorTests.fs` and a real governed-
file tightening.

## US1 — rewording passes where the pre-055 literal table failed (SC-001)

`Guidance.evaluateGuidanceCheck` over the real `skillist-structured` obligation
(`AnyOf ["structured skillist"; "structured \`skillist\`"]`):

- **Reworded content**: `"Each task declares a structured skillist listing its
  capability skill ids in order."`
- **New evaluator**: returns `[]` (PASS) — the concept anchor `structured
  skillist` survives.
- **Pre-055 literal table**: pinned the exact phrase `structured \`skillist\``
  (backticked), which the rewording removed → the old table would emit a
  `missing` term finding.

### Live demonstration on a real governed file

`.specify/templates/tasks-template.md` (and its `fsharp-opinionated` preset twin)
line tightened:

```diff
-capability skills). Write the minimal ordered skill set to structured
+capability skills). Write the minimal ordered skills to structured
 `skillist` metadata and mirror it in `tasks.md`.
```

The pre-055 table required the literal `minimal ordered skill set`; the edit
removes the redundant `set` while preserving the `minimal ordered` obligation
anchor and the `structured \`skillist\`` token. `./fake.sh build -t
GeneratedGuidanceCheck` PASSes where the pre-055 literal table would have failed.

```
Guidance validator (relocated) – 6 passed, 0 failed
```

## US2 — source-of-truth drift still fails (SC-002, FR-003)

Concept anchor removed while the sibling `[skillist: []]` ContractToken remains:

- **Drifted content**: `"Tasks list their dependencies. Example metadata line:
  [skillist: []] for no skill."`
- **Token check**: still satisfied (token present → no token finding).
- **Obligation check**: `AnyOf` finds no concept anchor → FAIL with:

```
.specify/templates/tasks-template.md: obligation 'skillist-structured' (constitution:Local Agent Skills) not reflected [task-skillist-guidance]
```

This proves the obligation fails on **prose-concept loss**, not merely on token
loss (the anchor-disjointness rule, FR-003).

### Twin coverage (spec edge case)

With both twins in `Files`, dropping the concept in one twin only:

```
.specify/presets/fsharp-opinionated/templates/tasks-template.md: obligation 'skillist-structured' (constitution:Local Agent Skills) not reflected [task-skillist-guidance]
```

Drift in one twin is still caught; the synchronized twin passes.

## All three sites migrated (SC-003)

`task-skillist-guidance`, `controls-boundary-guidance`, and
`sequential-fake-guidance` are all converted to the
`ContractToken` + `GuidanceObligation` model. No list still freezes prose purely
as a currency proxy. See [contract-tokens.md](./contract-tokens.md) for the
enumerated machine-contract-token set (SC-004).
