# Docs Site Build (US4) — 079-doc-preview-examples (T021)

- **status: PASS**
- **authoritative-command:** `dotnet tool restore` → Release-build the 10 documented
  libraries → `dotnet fsdocs build --clean --strict --eval --ignoreuncategorized
  --projects <10 libs>` (GPU-free; no render host required — FR-009).
- **exit-code: 0** (no broken-link / project-crack / eval failures under `--strict`).
- **failure-class: none**

The 10 documented libraries: `src/Scene`, `src/Layout`, `src/KeyboardInput`, `src/Input`,
`src/SkiaViewer`, `src/Elmish`, `src/Controls`, `src/Controls.Elmish`, `src/Testing`,
`src/SkillSupport`.

## Built-nav order (FR-011, N1, SC-006)

Category group order in the generated sidebar (`output/controls/button.html`,
`aside#fsdocs-main-menu`, `<li class="nav-header">` in document order):

```
Links · Architecture · Controls & design tokens · Governance · Spec Kit ·
Examples · Controls · Roadmap · Game specs · Guides · Productivity specs ·
Design history · API Reference
```

**Examples → Controls** are adjacent (nothing renders between them) and **Controls is above
Guides** — Controls renders immediately below Examples and above Guides as required (N1.1
Controls stays its own top-level `category: Controls`; N1.2 nothing between Examples and
Controls). The `categoryindex` renumber: controls 2→8, roadmap 7→9, guides 8→10; only
`categoryindex` frontmatter lines changed (N2 — no file moves, no `index`/slug changes).

## Previews present, links resolve (N3, SC-004, SC-006)

- `output/img/controls/*.png` — **51** preview images copied (custom-control absent —
  honestly unsupported, no broken `<img>`).
- `output/controls/*.html` — **54** pages (52 detail + catalog + spec-kit-workflow).
- Every demonstrative detail page's `<img src="../img/controls/<id>.png">` resolves to a
  present file; `output/controls/custom-control.html` carries **0** references to the removed
  image (its Preview section is the honest unsupported note).
- API-reference targets resolve (e.g. `output/reference/fs-skia-ui-controls-button.html`,
  `output/reference/fs-skia-ui-controls-typed-datepicker.html`) — generated detail→API links
  resolve in the built site; no `DeadLink`.

## Notes

- `--strict` did **not** fail. The only diagnostics were pre-existing coverage warnings:
  `FD0001`/"no documentation for …" and "entity `System.String`/`System.Guid` was not
  registered before" on `string`/`Guid` type-alias members (e.g. `ControlId`, `KeyId`) in
  `Scene`/`Layout`/`KeyboardInput`/`Input`/`Controls` `.fsi` — unrelated to this feature and
  non-failing under `--strict` (same class as the 078 build).
- `output/` is a build artifact and is not committed.
