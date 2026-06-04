# Evidence-Format Recoverability (FR-005 / SC-002)

**Claim.** A consumer hitting an `EvidenceAudit` failure for **any** evidence-format class
can learn the **full required shape** of each failing file — its name, the complete
required-token/column list, ordering rules, and resolved-path pattern — from the audit/graph's
**own output** and/or the generated `docs/evidence-formats.md` reference, **without**
running `strings -el FS.Skia.UI.Build.dll` and **without** copying a passing sibling project.

This file is the FR-005 proof. It is filled by T018 against a freshly generated project.

## Mechanism (D5: single-sourced from the enforcing constants)

Two complementary mechanisms, both single-sourced from the constants that enforce each rule
(so neither can drift from the validator):

1. **Diagnostics print the full per-file schema** for every failing format class, extending
   the proven 061 `Required`-token pattern on the readiness-contract scan:
   - **readiness-contract** (`Scans.fs` `readinessContract`) — full `Required` token list per
     file (shipped in 061).
   - **skill-loading-evidence** (`Audit.fs` `validateSkillLoadingEvidence`) — the 8-column
     row schema, the `loaded_at < work_started_at` ordering rule, and the resolved
     `.agents/skills/<id>/SKILL.md` path pattern.
   - **window-visibility / `diagnostic-class`** (`Scans.fs` `windowVisibility`) — the
     required `key=value` keys per file and the `diagnostic-class` ∈
     `{environment-session, window-visibility, app-lifecycle, product-defect}` value set.
   - **seh-acceptance** (`TaskParser.fs`) — the tokens `accepted-seh` and
     `synthetic-error-handling-approved` (no backticks).
2. **A generated reference page** `template/base/docs/evidence-formats.md`, emitted from the
   **same `EvidenceFormatSchema` constants** (ApiSurfaceGen-style generation + currency
   check), so an author can recover every file's required shape **before** triggering a
   failure.

## Proof log (T018) — against a real generated project

Generated project: `dotnet new fs-skia-ui --name SI062Probe --feedback true` (template
packed from current source: `FS.Skia.UI.Template.0.1.84-preview.1`), plus the
`TemplateCheck`-instantiated projects under
`artifacts/template-check/062-space-invaders-consumer-friction-followups/`.

**Up-front recoverability (no failure needed).** The generated project ships
`docs/evidence-formats.md` listing every format class's complete required shape:

```
## readiness-contract
## skill-loading-evidence
## window-visibility
## seh-acceptance
```

Each section names the required tokens, columns, ordering rules, and resolved-path
pattern — so an author recovers the contract **before** triggering a failure, with **no
`strings -el FS.Skia.UI.Build.dll`** and **no sibling copy**.

**On-failure recoverability (diagnostics print the schema).** For each failing class the
audit/graph prints the complete required shape, single-sourced from `EvidenceFormatSchema`
(unit-proven in `tests/Governance.Tests/Feature062GovernanceTests.fs`):

| class | where the full shape prints | recovered without decompiling |
|---|---|---|
| readiness-contract | `EvidenceAudit` log "readiness-contract required shapes (FR-004)" | yes (token list per file) |
| skill-loading-evidence | `task-graph.md` "skill-loading-evidence required shape (FR-005)" on a skill-loading error | yes (8-column row + ordering + path) |
| window-visibility | `EvidenceAudit` log "window-visibility required shapes (FR-005)" when it blocks | yes (keys + `diagnostic-class` set) |
| seh-acceptance | `EvidenceAudit` log "seh-acceptance required shapes (FR-005)" on an unaccepted/invalid SEH | yes (`accepted-seh` / `synthetic-error-handling-approved`, no backticks) |

The diagnostics and the generated reference both derive from the single
`EvidenceFormatSchema` source, so neither can drift from the validator (D5). SC-002 met.
