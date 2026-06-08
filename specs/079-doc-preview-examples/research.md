# Phase 0 Research: Demonstrative Control Preview Images

All NEEDS CLARIFICATION from Technical Context are resolved below.

## R1 — Single declared per-control sample source (FR-002)

**Decision**: Introduce one **declared list of per-control sample definitions** keyed by
control `Id`, in `CatalogGen.catalogFacts` order, as the single reviewable source of
"what each preview shows." Each entry pairs a control id with an explicit, fixed sample
construction (which typed-front-door props/state are populated, with what literal sample
values) and an optional per-control canvas size. It carries an explicit **kind**:
`Demonstrative` (renders sample content) or `Unsupported` (honest no-image declaration,
FR-007). The set is **total** over the supported catalog: every supported control id has
exactly one entry; a missing entry is a build/test failure, not a default fallback.

**Rationale**: Feature 078 rendered with bare `.defaults`, so what each preview showed was
*incidental* to default props — not declared, not reviewable, and mostly blank. FR-002
requires the depicted content be explicit and reproducible. A single keyed list mirrors the
existing `catalogFacts` single-source pattern (CatalogGen.fs), so reviewers diff one place.

**Alternatives considered**: (a) Per-control props scattered across detail-page frontmatter
— rejected: not a single source, hard to keep coherent with prose. (b) Auto-deriving sample
content from `RequiredAttributes` — rejected: required-attribute *names* don't carry
representative *values*; FR-001/FR-006 want curated, usage-coherent content, not synthesized
filler.

## R2 — Committed, deterministic render harness (FR-008)

**Decision**: Add a **committed, compiled render harness** that 078 never had: a small
generator (a console entry or a render test in a project that already references the typed
`FS.Skia.UI.Controls.Typed` front door, `Scene` (`Control.render`/`Theme.light`),
`SkiaViewer`, `Testing`, and SkiaSharp — the `tests/SkiaViewer.Tests` precedent
`Feature063RendererTests.renderToPng` is the template). It loops over the R1 sample source
and, for each `Demonstrative` entry, builds the typed widget → `Widget.toControl` →
`Control.render Theme.light` → `SceneNode.Group [ result.Scene ]` →
`SkiaViewer.captureScreenshotEvidence` with `CaptureMode = ViewerRenderTargetPng` → writes
`docs/img/controls/<id>.png`. `Unsupported` entries write **no** image. Its invocation
command is documented in quickstart.md and runs only on a render-capable host.

**Rationale**: FR-008 demands deterministic, idempotent regeneration; a committed compiled
harness driven by fixed literals makes "the same control state produces byte-identical
output" testable and reviewable, and removes 078's ad-hoc manual rendering. Keeping it in a
Controls/SkiaViewer-referencing test/generator project (not in dependency-light
`build/Governance`) respects the build front's SkiaSharp-free constraint.

**Determinism controls** (all fixed, no clocks/random/env): `Theme.light`; fixed canvas
size (default 320×160, per-control override allowed per R4); `ViewerRenderTargetPng`
off-window raster; stateful controls initialized via their typed `init` with fixed sample
models. Idempotence is asserted by a governance test over committed bytes / a hash manifest.

**Alternatives considered**: A `.fsx` script under `scripts/` — workable and lighter, but a
compiled harness gives type-checked sample literals and a natural home for the
idempotence/totality tests; the script form is the documented fallback in quickstart.

## R3 — Trivial-content guard in a SkiaSharp-free build (FR-004/FR-005, SC-003)

**Decision**: Strengthen `ControlsCatalogDocsCheck` with a **byte-floor trivial-content
guard**: a committed preview must exceed a pinned byte threshold `T` (a real structural
property readable without decoding). Add a `TrivialPreview` finding class alongside the
existing `UndecodablePreview`. `T` is **pinned in this research** by measuring the
regenerated demonstrative assets: choose `T` strictly between the largest near-empty canvas
(the spec's ~363-byte empty box) and the smallest *demonstrative* PNG, with margin. The gate
additionally cross-checks the committed `controls-preview-evidence.md` record for
consistency (every present preview classified `demonstrative`, none `trivial`).

**Rationale**: The governance build is intentionally dependency-light (no SkiaSharp), so it
cannot decode pixels. PNG compression makes byte size a **faithful** proxy here: a solid-
background near-empty 320×160 canvas compresses to ~363 bytes, while a populated render is
materially larger. This is a real property of the committed bytes — not synthetic — and the
SC-003 negative case (blank a preview → it drops under `T` → FAIL) demonstrates it. Pixel-
level "richer than empty" is honestly verified at **render time** on the render-capable host
(where SkiaSharp exists) and recorded in the evidence ledger (R5).

**Threshold pinning procedure** (executed during implementation, recorded in research/
readiness): after regenerating, record min demonstrative byte size and the empty-canvas byte
size; set `T` to a round value comfortably between them (documented), and add a test asserting
every committed demonstrative preview ≥ `T` with headroom and an empty canvas < `T`.

**Alternatives considered**: (a) Add SkiaSharp to the build to decode + count non-background
pixels — rejected: violates the dependency-light build constraint and `DependencyReport`
posture for a check that runs in GPU-free CI. (b) Compare against the prior committed PNG's
size — rejected: brittle and stateful. (c) Embed a content metric in PNG metadata the gate
parses — rejected: more moving parts than a byte floor for equal honesty.

## R4 — Per-control demonstrative content + overflow/canvas policy (FR-001/FR-006)

**Decision**: Each control is shown in a **single representative populated state** coherent
with its documented usage (FR-006), sized to remain legible within the canvas. Guiding
choices by control family (full per-control table authored in the harness sample source):

- **Display** (text-block, label, badge, icon, separator, image, rich-text): show real
  sample text / a glyph / sample runs, not empty strings.
- **Input** (button, text-box, text-area, numeric-input, slider): a labelled button
  ("Save"), populated text value, a slider positioned mid-track with a fixed value.
- **Selection** (check-box checked, switch on, radio-group with a chosen option, list-box /
  multi-select-list with several items and a highlighted selection, combo-box, color-picker
  swatches).
- **Data** (list-view, tree-view, data-grid): several columns/rows of fixed sample data with
  a selected row; data-grid shows `columns`+`rows` per FR-006.
- **Chart** (line/bar/pie/scatter, graph-view): a small fixed sample series / node-edge set.
- **Layout** (stack, grid, dock, wrap, border, panel, scroll-viewer, split-view): a small
  composed arrangement of a few child placeholders that conveys the layout's behavior.
- **Navigation/overlay/feedback** (tabs, menu, context-menu, toolbar, tooltip, dialog,
  toast, overlay, progress-bar, spinner, validation-message): a single representative static
  frame (FR Edge Cases: motion/interaction shown as one frame; limitation stated, not faked).

**Canvas/overflow**: default 320×160 preserved for cross-catalog comparability; a control
whose demonstration genuinely needs more room MAY declare a **fixed, documented** per-control
size in its sample entry (still byte-stable per run — never variable). Content that would
overflow is sized/truncated/arranged to stay representative and legible rather than clipped
into meaninglessness.

**Rationale**: SC-001/SC-002 require content demonstrably richer than empty *and* coherent
with documented usage. Family-level guidance keeps the curation tractable while the per-
control literals stay explicit in the single source.

**Alternatives considered**: A multi-state matrix or animated showcase — rejected by spec
Assumptions/Out-of-Scope (single representative state, no animation/interaction/multi-frame).

## R5 — Preview evidence record gains content classification (FR-010, SC-005)

**Decision**: Regenerate `controls-preview-evidence.md` as the per-control honesty ledger
with columns: control id, display name, renderer mode (`render-only / ViewerRenderTargetPng`),
decodable (yes/no), dimensions, **bytes**, and **content classification**
(`demonstrative` | `unsupported`). Add an explicit **summary line**: count rendered vs. count
honestly declared **unsupported**, with the total reconciled against the supported catalog
size (no silent omission). The classification is computed at render time on the render-capable
host (where pixels are decodable); the committed record is what the gate cross-checks (R3).

**Rationale**: FR-010 requires the record reflect the new demonstrative renders; SC-005
requires rendered-vs-unsupported counts be explicit. A classification column plus a reconciled
summary makes the honesty ledger machine-checkable against `catalogFacts`.

## R6 — Nav repositioning: Examples → Controls → Guides (FR-011)

**Decision**: Reposition by **renumbering fsdocs `categoryindex`** only (no page nesting, no
file moves, no URL/slug change). Current state: Controls=2 (top), Examples=7 (with Roadmap
also at 7), Guides=8, Design history=90. Resolve the Examples/Roadmap collision and place
Controls immediately after Examples and above Guides with this concrete table:

| Category                  | categoryindex (was → now) | Files affected                                   |
| ------------------------- | ------------------------- | ------------------------------------------------ |
| Examples                  | 7 → 7 (unchanged)         | `docs/examples/*.fsx`                             |
| **Controls**              | **2 → 8**                 | `docs/controls/*.md` (52 detail + catalog + spec-kit-workflow) |
| Roadmap                   | 7 → 9                     | `docs/roadmap.md`                                |
| Guides                    | 8 → 10                    | `docs/development.md`, `docs/distribution.md`, `docs/migration/v2-to-v3.md` |
| Architecture / C&dt / Governance / Spec Kit | unchanged (3/4/5/6) | (no change)                          |
| Design history            | 90 (unchanged)            | `docs/design-history.md`                         |

Resulting sidebar order: Architecture(3), Controls & design tokens(4), Governance(5),
Spec Kit(6), Examples(7), **Controls(8)**, Roadmap(9), Guides(10), Design history(90) — so
Controls renders **immediately below Examples** and **above Guides**. Within-category `index:`
values (narrative=1, catalog=2, detail pages=3..54) are **unchanged**. Because `categoryindex`
affects only sidebar grouping order — not URLs or slugs — all existing cross-links into
`docs/controls/` continue to resolve (enforced by the gate's `DeadLink` check).

**Rationale**: FR-011 mandates Controls stay its own top-level category with pages/assets in
`docs/controls/` and links resolving; renumbering `categoryindex` is the minimal, deterministic
mechanism fsdocs provides. Moving Roadmap below Controls makes "immediately below Examples"
literal (nothing between Examples and Controls).

**Alternatives considered**: (a) Setting Controls to a non-integer index between 7 and 8 —
rejected: fsdocs `categoryindex` is integer-valued. (b) Leaving Roadmap at 7 and only bumping
Guides — rejected: Roadmap (alphabetically after Examples at the same index) would render
between Examples and Controls, breaking "immediately below."

## Consolidated outcome

All Technical Context unknowns resolved; no NEEDS CLARIFICATION remains. The design renders
through the **real** evidence path, keeps the build SkiaSharp-free with an honest byte-floor
trivial guard, declares sample content from a single reviewable source, regenerates
deterministically, and repositions nav by integer `categoryindex` renumbering with no file
relocation.
