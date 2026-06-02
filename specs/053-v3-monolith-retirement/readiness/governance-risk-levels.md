# Governance risk levels & validation

- **Small** (this feature's own `readiness/` notes + record/verification notes): focused
  review plus `git diff` over the edited files is the **required evidence** and is
  authoritative for the level.
- **Medium** (the rewritten `Package.Tests` packaging-contract suite, the
  `PerPackageSurfaceDiff` Route-gating + its enforcement proof, the generated-`app`
  cleanliness gate, and the after-measurement metrics): the focused
  `Dev` / `PerPackageSurfaceDiff` / `TargetMetadataDrift` / `GeneratedProductCheck`
  runs and the named repo-wide no-consumer grep are the **required evidence** and the
  authoritative signals for the level.
- **Broad** (required here — `Route` escalates this governance + public-`.fsi` + pack-flow
  change): the full serialized FAKE gate order `Route` prints —
  `Dev` -> `GeneratedGuidanceCheck` -> `TemplateCheck` -> `GeneratedProductCheck` ->
  `DependencyReport` -> `EvidenceGraph` -> `EvidenceAudit` (plus the explicit
  `PerPackageSurfaceDiff` and `TargetMetadataDrift`). **Broad validation is required**
  whenever a consumer-contract, governance, or public-`.fsi` surface changes.
  Aggregate FAKE results are recorded as **non-authoritative**; any race-like or
  environment-flaky failure is re-run in focused isolation, and that focused result is
  authoritative. The deterministic scene-output parity oracle, preserved in the
  split-package suites, is the behavioural-parity authority (nothing re-renders this stage).
