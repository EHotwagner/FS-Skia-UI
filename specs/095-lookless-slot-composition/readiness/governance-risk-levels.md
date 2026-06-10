# Governance risk levels

This is a **Tier 1** change (public surface moves): a new `AttrCategory.Slot` case +
`AttrValue.SlotFillsValue of (string * Control<'msg>) list` case on `src/Controls/Types.fsi`, new
typed `Props` slot fields on `src/Controls/Widgets/Primitives.fsi` (`ButtonProps.Leading`/`.Trailing`)
and `src/Controls/Widgets/Containers.fsi` (`PanelProps.Header`/`.Footer`), plus the two shipping
consumer skills expanded to teach E1–E5.

- **small** — the pure, total slot lowering (`ControlInternals.slotFill` / `slotFillsOf` / `slotFor`
  / `lowerSlots`) and the typed-`Props` view lowering for `Button` and `Panel`. Focused validation:
  `Dev` + the targeted `Controls.Tests` suite (`Feature095SlotCompositionTests`, including the three
  `>=1000`-input FsCheck properties). No consumer-contract surface moves here.
- **medium** — the consumer-capability skill expansion (`src/Controls/skill/SKILL.md`,
  `template/fragments/controls/skill/SKILL.md`) teaching E1–E5 with runnable examples. Focused
  validation: `SkillSyncCheck`, `SkillQualityCheck`, `GeneratedGuidanceCheck`, and a generated
  project receiving the guidance.
- **broad** — the public `Types.fsi` + `Widgets/Primitives.fsi` + `Widgets/Containers.fsi` surface
  move escalates to controls-public-surface / package-surface. **broad validation** is the
  **required evidence** before merge: the full escalated gate list `Route` prints —
  `Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck,
  ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck,
  ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, SkillSyncCheck,
  SkillQualityCheck, PhaseHookParityCheck, SkillContractPathCheck, TemplateUpdateSkillPackageCheck,
  TemplateDrift, EvidenceGraph, EvidenceAudit` — run **sequentially** (shared `.fake` state). The
  aggregate result is **non-authoritative** and recorded as such in
  [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative verdict is
  `EvidenceAudit verdict=PASS`.
