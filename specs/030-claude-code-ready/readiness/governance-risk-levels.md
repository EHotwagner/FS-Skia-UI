# Governance Risk Levels

Risk level: Broad.

Affected layers: repository agent instructions, Claude project settings, project hooks, lifecycle skills, template source output, template package contents, generated product validation, drift validation, and evidence audit patterns.

Package identity/version: no expected package identity or version change.

Public `.fsi` surface: no reusable public `src/*` module was introduced.

Risk vocabulary:

- Small: a single repository or generated artifact path changes; focused validation is enough when ownership is narrow.
- Medium: source generation, settings, hooks, or one template profile changes; run focused generated guidance, template drift, and profile validation.
- Broad: shared source model, profile coverage, package template content, or validation aggregation changes; run `TemplateCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`, and `Verify` where the environment allows it.

Required evidence:

- Repository configuration inventory.
- Generated template artifact inventory.
- Generated project validation.
- Drift validation with controlled failing diagnostics.
- Claude Code research notes.
- Evidence graph and evidence audit outputs.

Broad validation:

- `GeneratedGuidanceCheck`: PASS.
- `TemplateDrift`: PASS.
- `EvidenceGraph`: PASS.
- `TemplateCheck`: blocked by host GTK/libdecor test-host crash before template smoke.
- `EvidenceAudit`: blocked until readiness contract terms were refreshed.
