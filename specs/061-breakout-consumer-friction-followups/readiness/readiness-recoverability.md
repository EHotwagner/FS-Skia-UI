# Readiness-Contract Recoverability (FR-004 / BD-2 / SC-003)

**Claim.** A consumer hitting an `EvidenceAudit` readiness-contract failure can learn the
**full required shape** of each failing readiness file — its name, status, the complete
required-token list, and the missing subset — from the audit's **own output**, without
decompiling `FS.Skia.UI.Build.dll` or copying a passing sibling project.

## Mechanism (D2: audit-prints-schema)

`build/Governance/Evidence/Scans.fs` (`readinessContract`) already holds the enforced token
list per file. Two changes surface it instead of hiding it:

1. Each readiness-contract `ScanHit` now carries `Required = Some terms` — the **full**
   enforced token list — alongside the existing `MissingTerms` (the absent subset). Single
   source = the same `terms` that enforce the rule, so the printed schema **cannot drift**
   (RC-2). The field is not serialized into `readiness-contract-hits.json`, so existing
   goldens are unchanged.
2. `Render.readinessContractDiagnostics` formats, per failing file:
   ```
   readiness-contract: <fileName>
     status: missing | partial
     required-tokens: <full enforced list>
     missing: <subset actually absent>
   ```
   The audit front-end (`Front/Governance.fs` `runEvidenceAuditCheck`) appends this block to
   `readiness/logs/evidence-audit.txt` under `--- readiness-contract required shapes (FR-004) ---`
   whenever the readiness-contract scan blocks.

## Proof without decompiling (SC-003)

The behaviour is pinned by `tests/Governance.Tests/Feature061GovernanceTests.fs`:

- "FR-004 a missing readiness file prints its full required shape" — feeds an empty
  readiness set to `Scans.readinessContract`, asserts the diagnostic names
  `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`,
  prints `status: missing`, the `required-tokens:` line, and every enforced token
  (`small`, `medium`, `broad`, `required evidence`, `broad validation`). **RC-1.**
- "FR-004 RC-2 a partial file still prints the FULL enforced token list" — a file missing
  only `broad validation` still prints the whole list plus `missing: broad validation`.
  **RC-2.**

A consumer reads the required shape directly from the audit log and authors each readiness
file to satisfy it — no `.dll` decompilation, no sibling copy. This very feature's own
readiness set (`governance-risk-levels.md`, `aggregate-hang-diagnostics.md`,
`runtime-limitations.md`) was authored to the enforced token lists and reaches
`EvidenceAudit verdict=PASS`. **RC-3.**

## FR-005 — single defect-class spelling (DC-1/DC-2)

The readiness audit requires the defect class under **one** spelling — `product-defect`
(`Scans.fs` `requiredClasses = [ … "product-defect" ]`, matched as
`diagnostic-class=product-defect` / `failure-class=product-defect`). A repo-wide scan
(`grep -rn 'defect' build/Governance .specify/extensions`) confirms **no** governance rule,
template, or test requires the project-prefixed `<project>-defect` (e.g.
`breakoutdemo2-defect`) form — that spelling existed only in the consumer's own project, not
in any shipped contract. `product-defect` is project-agnostic (correct for a generated
template) and is the single authoritative spelling everywhere a consumer must type it. **DC-1/DC-2.**
