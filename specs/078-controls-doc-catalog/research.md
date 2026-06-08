# Research: Authoritative Controls Catalog in Published Docs

Resolves the open unknowns for feature 078 before design. Each item: Decision /
Rationale / Alternatives considered.

## R1 — Per-control preview generation under a GPU-free docs CI

**Decision**: Pre-render each control's preview to a committed PNG **source** asset
under `docs/img/controls/<id>.png`, produced through the existing deterministic
render-only evidence path on a render-capable host, and consumed (copied) by the
GPU-free docs CI job. Where a control cannot be honestly rendered, emit **no** PNG
and have the detail page state the honest unsupported reason. Validate each produced
PNG with the existing `FS.Skia.UI.Testing` PNG check (`readPngArtifact`: decodable,
real dimensions, non-trivial content).

**Rationale**: `.github/workflows/docs.yml` is deliberately GPU-free ("needs NO
Xvfb / Vulkan stack … If a future example renders, add the headless graphics steps …
or mark that example `[S]`"). Treating previews as *committed source assets* (like
the already-committed generated `catalog.yml`/`Catalog.fs` and `docs/img/`) keeps the
docs CI unchanged and deterministic, while the currency check guarantees the assets
stay in lockstep with the catalog. The framework already owns the honest visual-proof
machinery: `SkiaViewer` (`RenderScene`, `CaptureImageEvidence`, `WriteVisualEvidence`)
and `Testing` (`VisualEvidence*`, `readPngArtifact`, the
"metadata-only / 1×1 fallback / layout-only" rejection phrases). FR-003a and the
`fs-skia-evidence-mode` skill require exactly this honesty contract.

**Alternatives considered**:
- *Render during docs CI with added Xvfb/Vulkan*: rejected — contradicts the
  intentionally GPU-free docs job, adds fragility/non-determinism, and the deploy is
  serial/uncancellable.
- *CPU off-screen raster render at docs-build time*: attractive (deterministic,
  GPU-free) **iff** an off-screen raster render path exists that takes an arbitrary
  `SKCanvas`. The current capture path is wired to the viewer host. If a small
  off-screen raster entry can be added cheaply within the render-only contract it is
  preferable to committing assets; this is a Phase-1/implementation spike. Default
  remains committed assets to de-risk; the currency model is identical either way.
- *Fabricated/placeholder images*: rejected outright — 1×1 and metadata-only
  "previews" are explicitly disallowed by the visual-proof rules.

## R2 — API-reference page slug per control

**Decision**: Derive each detail page's API link from the control's `Module` field in
`catalogFacts`, targeting the fsdocs per-type page under `reference/`. The exact slug
form (`reference/fs-skia-ui-controls-<module>.html` vs. the
`…-controls-typed-<module>.html` typed-namespace page) is **verified against a real
`dotnet fsdocs build` output** during Phase 1 and encoded once in the generator; the
currency check then enforces that every generated API link **resolves** in the built
site (no dead links), so a wrong slug fails the gate rather than shipping.

**Rationale**: fsdocs derives reference slugs from fully-qualified type names lowered
to kebab-case (observed pattern: `reference/fs-skia-ui-controls-button.html`). The
catalog already records the canonical `Module` per control, making the link a pure
projection. Verifying against built output (not guessing) is the project's standard
for generated-link correctness, and link-resolution is itself an FR-009/SC-003
requirement.

**Alternatives considered**: Linking only to the reference *index* — rejected;
FR-002/SC-003 require a resolving link to the control's *own* API page.

## R3 — Section placement and ordering in the fsdocs nav

**Decision**: A new top-level category **"Controls"** owning, in `index:` order:
`spec-kit-workflow.md` (narrative + Penpot subsection) → `catalog.md` (generated
index) → the 52 `<id>.md` detail pages. fsdocs orders pages by frontmatter
`category` + `categoryindex` (group) and `index` (within group); the new category
takes a `categoryindex` ahead of the existing "Controls & design tokens" group, and
detail pages carry ascending `index` values grouped by catalog category.

**Rationale**: Matches FR-006's required order exactly and uses the site's existing
frontmatter-driven nav (seen in `controls-design/*.md`) with no theme/template
change. Keeping the Penpot material as a `##` subsection of the narrative satisfies
FR-007 ("the usage narrative MUST include a Penpot/design-tokens subsection") while
cross-linking to the deeper existing `controls-design/design-tokens-penpot.md`.

**Alternatives considered**: Relocating the existing `controls-design`/architecture
pages into the new section — rejected; the spec says existing pages **cross-link
into** the new section rather than move or duplicate (FR-006).

## R4 — Reuse vs. new generator/currency module

**Decision**: Add a new `build/Governance/CatalogDocsGen.fs(/.fsi)` that **reuses**
the `CatalogGen` single source (`catalogFacts`) and its splice-marker helpers
(`spliceWith`/currency-per-file). The catalog index and each detail page's canonical
header are emitted into `BEGIN/END GENERATED` regions; `RefreshSurfaceBaselines`
regenerates them; the new `ControlsCatalogDocsCheck` computes currency + completeness
+ link/preview validation.

**Rationale**: `CatalogGen` already proves the single-source, splice-marker, and
per-file currency pattern over `catalog.yml`/`Catalog.fs`; the docs index/header is
the same projection to new targets. A sibling module keeps docs-generation concerns
separate from the source-catalog generation while sharing the marker/currency
primitives. Memory `catalog-splice-marker-insertion` applies: marker pairs must be
placed in each file first, then `RefreshSurfaceBaselines` fills them; growing the set
needs the markers pre-placed.

**Alternatives considered**: Hand-maintaining the index — rejected by FR-004/SC-004.
Cramming docs generation into `CatalogGen.fs` — rejected for separation of concerns
and file size.

## R5 — Routing the new gate

**Decision**: Wire `ControlsCatalogDocsCheck` so it is selected when the diff touches
the catalog single source (`build/Governance/CatalogGen.fs` / `src/Controls/catalog.yml`),
the docs section (`docs/controls/**`, `docs/img/controls/**`), or the new generator.
Add the gate to `AgentValidation.knownGates`; regenerate `validation.contract.yml`
from `Routing.fs` via `RefreshSurfaceBaselines` (`TargetMetadataDrift`-enforced).

**Rationale**: Mirrors how `ControlsCatalogGenerationCheck` is routed with the
controls surface (Routing.fs). Memory `accepted-seh-stops-propagation` notes a new
gate must also be added to `AgentValidation.knownGates` or contract validation fails;
memory `per-package-baseline-not-in-refresh-target`/`refresh-surface-baselines-…`
confirm the regeneration path. `Route` then prints the authoritative gate set for the
actual diff.

**Alternatives considered**: Folding into the existing `docs-only` rule (currently →
`EvidenceGraph` only) without a dedicated gate — rejected; docs-only can't enforce
catalog↔index↔detail↔preview currency.
