# Surface baselines — recaptured (feature 095, T025)

This is a **Tier 1** change; the controls-public-surface / per-package / cross-package baselines were
recaptured via `./fake.sh build -t RefreshSurfaceBaselines` (per-package snapshots regenerated in the
same run). The recaptured surface adds exactly the additive E5 surface — nothing else moved.

## Recaptured baseline diffs (additive only)

- `readiness/surface-baselines/FS.Skia.UI.Controls.txt` — adds `AttrCategory.Slot`,
  `AttrValue.SlotFillsValue of (string * Control<'msg>) list`, `ButtonProps.Leading` /
  `.Trailing`, `PanelProps.Header` / `.Footer`.
- `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` — same additive deltas at the
  per-package level.
- `template/base/docs/api-surface/Controls/{Types,Control,Primitives,Containers}.fsi` — the
  generated api-surface docs regenerated to match.

The internal `ControlInternals.slotFill` / `slotFillsOf` / `slotFor` / `lowerSlots` helpers are
`module internal` and do **not** appear on the public surface (no public `Attr.slot` builder, no
public `SlotName` type) — the only public authoring path is the typed `Props` slot fields.

**Gates:** `PackageSurfaceCheck` + `PerPackageSurfaceDiff` validate the recaptured baselines; both
are part of the escalated agent-ready list run for this feature.
