# Contract: Self-describing readiness-contract diagnostics

**Feature**: `061-breakout-consumer-friction-followups` · **FR-004, FR-005,
FR-007**
**Surface**: `EvidenceAudit` / `EvidenceGraph` in-process output
(`build/Governance/Front/Governance.fs`) sourced from the readiness grammar in
`build/Governance/Evidence/Scans.fs`. Single source of truth = the `requiredTokens`
list already enforced; this contract requires that same data be **printed** on
failure.

## FR-004 — readiness-contract failure diagnostic

When a readiness file is missing or partial, the audit MUST emit, per failing
file, the complete expected shape:

```
readiness-contract: <fileName>
  status: missing | partial
  required-tokens: <token>, <token>, …          # the full enforced list
  required-fields: <field>, …                    # when a field-list file
  required-table-header: | <col> | <col> | …     # when a table file
  missing: <subset actually absent>
```

| ID | Assertion |
|----|-----------|
| RC-1 | Each failing readiness file prints its full `required-tokens` (not just the first missing one, not a bare count). *(SC-003)* |
| RC-2 | The printed list is derived from the **same** `requiredTokens` data that enforces the rule (cannot drift). |
| RC-3 | A consumer reaches a passing `EvidenceAudit` from a fresh project using only this output (and/or a shipped template), without decompiling `FS.Skia.UI.Build.dll` or copying a sibling. *(SC-003)* |

## FR-005 — single defect-class spelling

| ID | Assertion |
|----|-----------|
| DC-1 | The defect-class concept is required under one spelling — `product-defect` — across the readiness audit (`window-state-diagnostics.md`: `diagnostic-class=product-defect` / `failure-class=product-defect`) and any source governance scan. *(SC-004)* |
| DC-2 | No governance rule, template, doc, or test still requires the project-prefixed `<project>-defect` spelling for the same concept; if any genuinely-distinct use exists it is documented as distinct at both sites. |

## FR-007 — explicit graph verdict line

| ID | Assertion |
|----|-----------|
| GV-1 | A clean `EvidenceGraph` prints a single greppable terminal token `verdict=ok (no cycles, no dangling refs, no [S*])`. *(SC-005)* |
| GV-2 | A failing graph prints `verdict=error (<reason>)`. |
| GV-3 | The token style is consistent with `EvidenceAudit`'s existing `verdict=PASS|FAIL`. |

## Out of scope

- Changing which files are required or which tokens enforce them (the grammar in
  `Scans.fs` is authoritative and unchanged except for the FR-005 spelling
  resolution).
- Replacing exit-code semantics — the verdict line is additive.
