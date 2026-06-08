# Docs Site Build (078) — T018 / T027

- status: PASS
- authoritative-command: `dotnet tool restore` → build the 10 documented libraries →
  `dotnet fsdocs build --strict --eval --ignoreuncategorized --projects <10 libs>`
- exit-code: 0 (no broken-link / project-crack / eval failures under `--strict`)
- failure-class: none

## Verified output presence

- `output/controls/catalog.html` — generated catalog index (present)
- `output/controls/spec-kit-workflow.html` — narrative + Penpot subsection (present)
- `output/controls/<id>.html` — 52 per-control detail pages (present; 54 total pages
  under `output/controls/` = 52 detail + catalog + spec-kit-workflow)
- API-reference targets resolve, e.g. `output/reference/fs-skia-ui-controls-button.html`,
  `output/reference/fs-skia-ui-controls-typed-datepicker.html` (072 typed page),
  `output/reference/fs-skia-ui-controls-collections.html` — confirming the generated
  detail→API links resolve in the built site.

## Notes

- `--strict` did **not** fail: the only diagnostics were pre-existing `FD0001`
  "no documentation for …" coverage **warnings** on `Scene` / `SkiaViewer` `.fsi`
  union-case properties, unrelated to this feature (`--strict` warns but does not fail
  on those, per the docs CI design).
- All **52 per-control preview images are present** (see
  `controls-preview-evidence.md`): produced through the render-only path and committed
  under `docs/img/controls/`. The strict build copies all 52 into
  `output/img/controls/` and every detail-page `<img>` link resolves — no broken links.
