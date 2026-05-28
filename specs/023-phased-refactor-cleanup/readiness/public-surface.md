# Public Surface

status=ok

This Tier 2 cleanup preserves public package signatures and documented
contracts. No `.fsi` public surface change is planned or required for the
generated product cleanup, template source split, build governance helper
extraction, or SkiaViewer internal boundary work.

Guardrails:

- `src/SkiaViewer/SkiaViewer.fsi` remains the SkiaViewer public facade.
- Surface baselines remain unchanged for this feature.
- If implementation requires a public API or baseline change, the work stops
  and moves to a separate Tier 1 design.

Validation path:

- `PackageSurfaceCheck` remains the authoritative package-surface gate.
- Phase-specific viewer validation records `SkiaViewer.fsi` and surface
  baseline status in `viewer-internal-boundary.md`.
