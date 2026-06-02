# Closeout docs (FR-009/010/011/012)

- **V2→V3 migration guide**: `docs/migration/v2-to-v3.md` — surface map (old `FS.Skia.UI` →
  the split packages), package-reference move steps, removed-`SceneConversion` note, rich
  keyboard-input → `FS.Skia.UI.Input`. `.Controls.Elmish` / `.Testing` noted as having no
  monolith predecessor.
- **ADR 0012**: `docs/adr/0012-monolith-retirement-closeout.md` — status Accepted; records the
  retirement (delete `src/Lib`, unpublish `FS.Skia.UI`, enforce the per-package gate, add the
  cleanliness gate) and links programme ADRs 0007–0011.
- **After-baseline**: `docs/reports/_baselines/2026-06-02-v3-after.md` — mirrors the Stage-0
  before-baseline; `src/Lib` LOC → 0, monolith transitive-pull → none, duplicate-type count
  → 0, package count = 9 split + build engine, per-package baselines present (9), generated-
  `app` cleanliness asserted — each metric with its reproduction command.
- **ParityGallery / oracle policy**: `readiness/paritygallery-policy.md` — oracle preserved,
  `samples/ParityGallery` kept (split-package-only).
- Migration guide + ADR linked from the implementation plan (`plan.md` § Closeout artifacts).

failure class: ClosseoutDocsMissing. next action: none — all four published.
