# Phase 1 Data Model: Demonstrative Control Preview Images

Authoritative source remains `FS.Skia.UI.Build.CatalogGen.catalogFacts` (the 52 supported
controls). This feature adds a **per-control sample definition** layer that drives rendering,
and strengthens the preview **evidence/verdict** layer. Nothing here changes the public
product surface.

## Entities

### 1. ControlSampleDefinition (NEW — single declared source, FR-002)

The reviewable answer to "why does this preview show what it shows." One entry per supported
control, in `catalogFacts` order.

| Field          | Type                         | Notes                                                        |
| -------------- | ---------------------------- | ----------------------------------------------------------- |
| `Id`           | string                       | Control id; MUST match a `catalogFacts` id.                 |
| `Kind`         | `Demonstrative \| Unsupported` | Unsupported ⇒ no image committed (FR-007).                 |
| `Build`        | (host-side) typed-widget construction | Fixed sample props/state through the typed front door; only meaningful for `Demonstrative`. |
| `Canvas`       | `{ Width; Height }` (default 320×160) | Per-control override allowed (R4); fixed/documented, never per-run variable (FR-008). |
| `UsageNote`    | string                       | One line tying the sample to documented usage (FR-006 coherence). |

**Invariants**
- **Totality**: `{ definition ids } == { catalogFacts ids }` exactly (no gap, no orphan).
- **Determinism**: `Build` uses only fixed literals — no clock, randomness, or environment
  data (FR-008).
- **Honesty**: `Unsupported` entries produce no PNG and a `preview-status: unsupported`
  marker on the detail page; never a fabricated/1×1/placeholder image (FR-003).

### 2. ControlPreviewAsset (committed PNG — refined from 078)

`docs/img/controls/<id>.png` for each `Demonstrative` control.

| Field        | Type    | Constraint                                                       |
| ------------ | ------- | --------------------------------------------------------------- |
| Path         | string  | `docs/img/controls/<id>.png`.                                   |
| Decodable    | bool    | MUST be true (valid PNG signature + IHDR).                      |
| Dimensions   | W×H     | Non-1×1; equals the definition's `Canvas`.                      |
| Bytes        | int     | MUST exceed the pinned trivial-content floor `T` (R3, FR-004).  |
| RendererMode | enum    | `render-only / ViewerRenderTargetPng` only.                     |

### 3. PreviewContentVerdict (NEW — trivial-content guard, FR-004/FR-005)

Computed by the strengthened currency gate over committed bytes (SkiaSharp-free).

```
Verdict =
  | Demonstrative      // decodable, non-1×1, bytes ≥ T
  | Trivial            // decodable but bytes < T   → FAIL (TrivialPreview)
  | Undecodable        // fails PNG structural validation → FAIL
  | Missing            // no file and no unsupported marker → FAIL
  | Orphan             // file for an id not in catalogFacts → FAIL
  | UnsupportedDeclared // no file WITH unsupported marker → PASS (honest)
```

`Trivial` joins the existing finding set (`IndexStale`, `Missing*`, `Stale*`, `Orphan*`,
`Undecodable*`, `DeadLink`) in `CatalogDocsGen.fs`.

### 4. PreviewEvidenceRecord (regenerated — FR-010, SC-005)

`specs/079-doc-preview-examples/readiness/controls-preview-evidence.md`. Per-control row:
id, display name, renderer mode, decodable, dimensions, bytes, **content classification**
(`demonstrative | unsupported`). Plus a **reconciled summary**: `rendered = N`,
`unsupported = M`, `N + M == |supported catalog|` (no silent omission).

### 5. NavCategoryPlacement (FR-011)

fsdocs frontmatter on `docs/controls/*.md` and the renumbered peers. Only `categoryindex`
changes (R6 table); `category` stays `Controls`, within-category `index` unchanged, file
paths/slugs unchanged.

| Property        | Before | After | Effect                                  |
| --------------- | ------ | ----- | --------------------------------------- |
| Controls index  | 2      | 8     | renders below Examples(7), above Guides |
| Roadmap index   | 7      | 9     | no longer between Examples and Controls |
| Guides index    | 8      | 10    | stays below Controls                    |

## State / generation flow

```
catalogFacts (source)
   └─► ControlSampleDefinition[] (R1, single source, total over ids)
          ├─ Demonstrative ─► render harness (R2, render-capable host, deterministic)
          │                      └─► docs/img/controls/<id>.png  +  evidence row (demonstrative)
          └─ Unsupported  ─► no PNG  +  detail-page unsupported marker  +  evidence row (unsupported)

committed tree ─► ControlsCatalogDocsCheck (SkiaSharp-free, GPU-free CI)
                    └─► PreviewContentVerdict per id  ─► PASS/FAIL (incl. TrivialPreview)
                    └─► cross-check controls-preview-evidence.md consistency & counts
```

## Validation rules (traceability)

- FR-001/FR-006 → every `Demonstrative` definition populates representative, usage-coherent
  content (R4).
- FR-002 → totality invariant on `ControlSampleDefinition`.
- FR-003/FR-004 → `ControlPreviewAsset` constraints + `RendererMode` restriction.
- FR-005 → `PreviewContentVerdict.Trivial` ⇒ FAIL.
- FR-007/SC-005 → `Unsupported` honesty + reconciled summary counts.
- FR-008 → determinism invariant + harness idempotence test.
- FR-009 → assets are committed source; gate/site build need no render host.
- FR-010 → `PreviewEvidenceRecord` schema.
- FR-011 → `NavCategoryPlacement` (R6).
