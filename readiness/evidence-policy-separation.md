# Evidence policy separation — generated-guidance vs product evidence

Governance evidence is kept in distinct lanes so a change to one cannot silently satisfy another's
gate:

- **Generated-guidance lane** — `.specify/templates/**`, `.specify/presets/**`, the `template/**`
  capability skills and READMEs. Validated by `GeneratedGuidanceCheck` + `TemplateDrift`. The
  canonical `.agents` skill tree is the single source; the `.claude` tree is **generated** from it
  (`RefreshSurfaceBaselines`) and currency is enforced by `SkillSyncCheck` — the two can never drift.
- **Product lane** — the shipped packages' public `.fsi` surface and generated products. Validated by
  `PackageSurfaceCheck` / `PerPackageSurfaceDiff`, `FsiTranscripts`, `TemplateCheck`, and
  `GeneratedProductCheck`. Surface baselines live under `readiness/surface-baselines/**` and
  `readiness/per-package-surface/**`.
- **Governance lane** — `specs/**` evidence (tasks, deps, readiness) audited by `EvidenceGraph` +
  `EvidenceAudit`.

Build-tooling (`build/Governance/**`, `FS.Skia.UI.Build`) is **excluded from product surface tooling**
— it is never shipped to a generated product, so no `readiness/surface-baselines/FS.Skia.UI.Build.txt`
exists. `Routing.fs` declares which lane each path escalates into; `validation.contract.yml` is
generated from it, so the lanes cannot be bypassed by hand-editing the contract.
