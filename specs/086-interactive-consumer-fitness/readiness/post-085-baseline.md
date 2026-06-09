# Post-085 baseline (T002)

This feature builds on the post-085 package baseline.

- Package versions in flight: `FS.Skia.UI.* 0.1.91-preview.1` (all packable libs + Build).
- Version-bump-on-merge obligation: `speckit-merge` bumps the **11 packable libs**
  (including `FS.Skia.UI.Build` under `build/Governance/**`, which a src-only bump would
  miss). The **template** is a **separate version track / flow** — its single
  `FsSkiaUiVersion` property drives all generated pins and is updated on its own template
  flow, not by the lib bump. See memory: merge-bump-scope-libs-not-template,
  template-update-064-single-version, build-package-version-drift-gotcha.
- No new package identities are introduced by 086 — all work is within existing
  `FS.Skia.UI.*` packages (Scene, Controls, Controls.Elmish, SkiaViewer, Layout) plus the
  template.
