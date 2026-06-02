# Governance risk levels & validation

- **Small** (this feature's own `readiness/` notes + record/verification): focused review plus
  `git diff` over the edited files is the **required evidence** and is authoritative for the level.
- **Medium** (the `FS.Skia.UI.Scene` per-package surface update + the leak proof): the focused
  `PerPackageSurfaceDiff` run (clean) and the monolith-absent leak-proof dump are the **required
  evidence** and the authoritative signals for the level.
- **Broad** (required here — `Route` escalates this consumer-contract + `src/**/*.fsi` change to the
  agent-ready gate set): the full gate order — `Dev` -> `PackageSurfaceCheck` ->
  `GeneratedGuidanceCheck` -> `TemplateDrift` -> `FsiTranscripts` -> `EvidenceGraph` ->
  `EvidenceAudit` (plus the explicit `PerPackageSurfaceDiff`). Broad validation is required whenever a
  public runtime `.fsi` or a consumer contract changes. Aggregate FAKE results are recorded as
  non-authoritative; any race-like/environment-flaky failure (the known `SkiaViewer.Tests` headless
  libdecor-gtk crash) is re-run in focused isolation, which is authoritative, with deterministic
  scene-output as the primary parity oracle.
