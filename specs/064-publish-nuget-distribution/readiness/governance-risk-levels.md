# Governance risk levels

- **small** — a single governance source edit (e.g. one routing rule, one `knownGates`
  entry, the `Targets.fs` DU additions). Focused validation: `Dev` + the directly-affected
  `Governance.Tests`. Covers T006/T007 in isolation.
- **medium** — target registry + contract regeneration: the `Publish`/`PrePublishCheck`
  registry rows and `validation.contract.yml` regeneration (T019/T036). Focused validation:
  `Dev`, `GeneratedGuidanceCheck`, and the regenerated `validation.contract.yml` currency
  checks (`TargetMetadataDrift`, `SkillSyncCheck`).
- **broad** — the template / `GeneratedProduct.fs` consumer-contract change (public-feed
  `NuGet.config`, single-source `<FsSkiaUiVersion>`, `build.fsx` runtime read, per-package
  metadata). **broad validation** (`TemplateCheck` / `GeneratedProductCheck`) is the
  **required evidence** before merge; FAKE-backed gates run **sequentially**. The aggregate
  result is **non-authoritative** and recorded as such in
  [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative
  verdict is `EvidenceAudit verdict=PASS`.
