# Governance risk levels & validation

- **Small** (this feature's own `readiness/` notes + record/verification): focused review plus
  `git diff` over the edited files is the **required evidence** and is authoritative for the level.
- **Medium** (the `FS.Skia.UI` monolith surface-baseline edit + the structural-rename parity): the
  focused `PackageSurfaceCheck` run (clean) and the `git diff -M` rename similarity are the
  **required evidence** — the clean baseline diff (SC-006) and the high-similarity rename (only the
  `namespace` line + the doc-comment phrase differ — SC-003) are the authoritative signals.
- **Broad** (required here — `Route` escalates this governance-path + monolith-`.fsi`-shrinking
  change to the `agent-ready` gate set): the full gate order `Route` prints —
  `Dev` -> `PackageSurfaceCheck` -> `FsiTranscripts` -> `GeneratedGuidanceCheck` -> `TemplateDrift`
  -> `EvidenceGraph` -> `EvidenceAudit`. **Broad validation is required** whenever a public `.fsi`
  surface or a consumer contract changes. Aggregate FAKE results are recorded as
  **non-authoritative**; any race-like or environment-flaky failure is re-run in focused isolation,
  and that focused result is authoritative. The repointed `AgentValidationFrameworkTests` suite
  (same fixtures, same assertion count) is the behavioural-parity oracle.
