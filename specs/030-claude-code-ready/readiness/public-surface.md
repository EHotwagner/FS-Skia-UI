# Public Surface Evidence

- Status: no public framework API added.
- Scope reviewed: repository instructions, Claude Code project artifacts, template files, build validation, drift validation, and readiness reports.
- Public `.fsi` impact: none. No reusable `src/*` F# module was introduced for this feature.
- Contract impact: agent workflow contracts and generated artifact contracts changed through `specs/030-claude-code-ready/contracts/`, not through runtime package APIs.
- Validation path: `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`, `TemplateCheck`, and `Verify` exercise the repository and template-facing changes.

