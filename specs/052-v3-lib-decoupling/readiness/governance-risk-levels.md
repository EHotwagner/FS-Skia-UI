# Governance risk levels & validation

- **Small** (this feature's own `readiness/` notes + record/verification): focused review plus
  `git diff` over the edited files is the **required evidence** and is authoritative for the level.
- **Medium** (the new `FS.Skia.UI.Input` per-package + aggregate baselines, the monolith surface
  shrink, the structural-rename parity, and the deterministic parity sign-off): the focused
  `PerPackageSurfaceDiff` (zero drift across nine packages) and `PackageSurfaceCheck` runs, the
  `git diff -M` rename similarity (only the `namespace` line differs), and the scene-output
  byte-identity vs the Stage-0 golden are the **required evidence** and the authoritative signals.
- **Broad** (required here — `Route` escalates this `src/**/*.fsi`-changing change to the
  `agent-ready` gate set): the full gate order `Route` prints —
  `Dev` -> `PackageSurfaceCheck` -> `FsiTranscripts` -> `GeneratedGuidanceCheck` -> `TemplateDrift`
  -> `EvidenceGraph` -> `EvidenceAudit` (plus the explicit `PerPackageSurfaceDiff`).
  **Broad validation is required** whenever a public `.fsi` surface or a consumer contract changes.
  Aggregate FAKE results are recorded as **non-authoritative**; any race-like or environment-flaky
  failure is re-run in focused isolation, and that focused result is authoritative. The migrated
  `Input.Tests` suite (same fixtures, same assertion count) is the behavioural-parity oracle.
