# Governance Risk Levels — Feature 092

Governance risk level for this change is **broad** (consumer-contract public `.fsi` moves in two
packages — `SkiaViewer.InteractiveViewerHost.MapKey : 'msg list` and the re-keyed
`Controls.Elmish` focus seam — plus the internal `RetainedRender.fsi` deltas and a runtime evidence
obligation), so the focused validation is the escalated serialized six-target order plus the
recaptured surface baselines. `Route` is expected to escalate to the consumer-contract tier.

- **Small** (framework-internal byte-identical): `Dev` + the failing-first Feature 092 tests.
  Not this change.
- **Medium** (single governance seam): adds `GeneratedGuidanceCheck` / surface currency over the
  regenerated api-surface + per-package snapshots. Not sufficient alone here.
- **Broad** (consumer-contract `.fsi` + evidence obligation): the escalated order, recorded
  **non-authoritatively** in `logs/` with per-target verdicts. `GeneratedProductCheck` may fail
  locally for environment reasons (see `runtime-limitations.md`).

Authoritative gates for this change: `Dev` (build + full unit/governance suites, incl. the new
`Feature092RetainedRenderTests` + `Feature092LiveSurvivalTests` and the migrated 085/090/091
suites), `PerPackageSurfaceDiff` / `PackageSurfaceCheck` over the recaptured `FS.Skia.UI.SkiaViewer`,
`FS.Skia.UI.Controls.Elmish` (public deltas) and `FS.Skia.UI.Controls` (internal `.fsi`) baselines,
`EvidenceGraph`, and `EvidenceAudit` — all PASS.

## Required evidence per risk level

- **Small** — **required evidence**: `Dev` + the Feature 092 tests.
- **Medium** — **required evidence**: the above plus the currency gates
  (`GeneratedGuidanceCheck`, `PackageSurfaceCheck`) over the regenerated api-surface, the per-package
  `.fsi.txt` snapshots, and the `MapKey` widening migration note.
- **Broad validation** — **required evidence**: the escalated six-target order run sequentially,
  recorded non-authoritatively in `logs/`. **Broad validation** is required here because the change
  moves public `.fsi` signatures in `src/SkiaViewer/**` and `src/Controls.Elmish/**`, the internal
  `src/Controls/RetainedRender.fsi`, and the emitted `docs/api-surface` tree. The host change is
  **shape-compatible** (`Some m` → `[ m ]`, `None` → `[]`), so effective gate coverage is preserved.

## MapKey widening scope note (resolved at contract time)

The only sibling `ViewerKey -> bool -> 'msg option` field is `GeneratedAppHost.MapKey` (the
non-interactive `Viewer.runApp` host). It is **deliberately left at `'msg option`**: it backs the
generated-project / samples path where multi-message keys are not needed, and widening it would
churn the template and generated host for no behavioral gain. Only the interactive
`InteractiveViewerHost.MapKey` (the path the Controls adapter drives) widens to `'msg list`.
