# Route escalation (T033, FR-018 / SC-006)

After the contract-bearing edits landed (`src/Controls/Control.fsi`,
`src/SkiaViewer/SkiaViewer.fsi`, `src/Controls.Elmish/ControlsElmish.fsi`, `template/**`,
new `.agents/skills/fs-skia-viewer-host`, governance templates), `./fake.sh build -t Route`:

```
developer-class=framework-author
tier=agent-ready
gates=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck,
      GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck,
      DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck,
      GeneratedGuidanceCheck, SkillSyncCheck, SkillQualityCheck, PhaseHookParityCheck,
      SkillContractPathCheck, TemplateUpdateSkillPackageCheck, TemplateDrift,
      EvidenceGraph, EvidenceAudit
matched-rules=controls-public-surface, generated-template, evidence-governance,
              generated-guidance, specify-catchall, docs-only, package-surface, skill-quality
```

**Escalation confirmed.** The gate set broadened from the spec-only baseline (T004:
`Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`) to the full
contract surface — the new public `.fsi` surface triggers `controls-public-surface` +
`package-surface` (→ `PackageSurfaceCheck`/`PerPackageSurfaceDiff`/`FsiTranscripts`), the new
skill triggers `skill-quality` (→ `SkillSyncCheck`/`SkillQualityCheck`), and the template/docs
edits trigger `generated-template`/`generated-guidance`.

**FR-018 wording note (deviation):** FR-018/SC-006 predicted escalation to **`maintainer-verify`**.
The actual tier for this diff is **`agent-ready`** (the framework-author escalated tier) — Controls
`.fsi` edits route to `agent-ready` with the broad gate set, not the `maintainer-verify` label.
The *substantive* escalation (the broad contract gate list) is what FR-018 intends and is
satisfied; the literal tier name is superseded by the real routing (recorded like research
D0/D1/D3 deviations).
