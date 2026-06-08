# ControlsCatalogDocsCheck — diagnostics & unsupported-scope handling (078) — T008

The gate computes `CatalogDocsGen.catalogDocsCurrency` (pure) over the observed docs tree and
`currencyDrift` turns each finding into one actionable line. Every finding names a remedy; the
regenerate command `./fake.sh build -t RefreshSurfaceBaselines` is named on every
generated-region finding. The structured report is always written to
`readiness/controls-catalog-docs.md` and asserted present via `RequireFiles` (loud on a missing
report); any drift additionally `FailWith`s the full diagnostic list.

| Finding | Trigger | Remedy named in the report |
|---------|---------|----------------------------|
| `IndexStale` | `catalog-docs/index` region ≠ render of `catalogFacts` | `RefreshSurfaceBaselines` |
| `MissingDetailPage` | a supported control has no `docs/controls/<id>.md` | author the page (commit stub + marker pair, then `RefreshSurfaceBaselines`) |
| `StaleDetailHeader` | a `catalog-docs/<id>` header region ≠ render | `RefreshSurfaceBaselines` |
| `OrphanDetailPage` | `<id>.md` for an id not in `catalogFacts` | remove the page |
| `MissingPreview` | required preview absent **and** no honest unsupported note | render via the render-only path, or add the `preview-status: unsupported` note (never a 1×1/placeholder) |
| `UndecodablePreview` | preview present but fails PNG validation (undecodable / 1×1 / trivial) | re-render through the deterministic render-only path |
| `OrphanPreview` | `<id>.png` for an id not in `catalogFacts` | remove the asset |
| `DeadLink` | a generated detail→API link slug does not resolve | fix the slug (research R2) or the target page |

## Unsupported-scope handling

- **Missing artifacts fail loudly, not silently.** A missing detail page is `MissingDetailPage`
  (a hard `FailWith`), not a silent pass; a missing/absent index region is `IndexStale`.
- **Preview honesty.** A control with no honest render must carry the
  `preview-status: unsupported` note; the gate distinguishes that honest omission from an
  accidental one (`MissingPreview`) and rejects fabricated/1×1 images (`UndecodablePreview`).
- **API-link resolution.** When a built site is present (`output/reference/`), every generated
  API slug is resolved against it (`DeadLink` on a miss). When absent, resolution is deferred to
  the authoritative `dotnet fsdocs build --strict` step — never silently skipped without saying so
  (the report states `api-links: resolution deferred …`).
