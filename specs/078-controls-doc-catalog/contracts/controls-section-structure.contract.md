# Contract: Controls Section Structure & Navigation

Defines the published "Controls" section layout the docs MUST present (FR-006, FR-008).

## S1 — Section ordering

A new top-level **"Controls"** category, ordered by fsdocs frontmatter:

1. `docs/controls/spec-kit-workflow.md` — usage narrative (US2) **with a Penpot/
   design-tokens `##` subsection** (US3, FR-007). Lowest `index:`.
2. `docs/controls/catalog.md` — generated catalog index (US1).
3. `docs/controls/<id>.md` × 52 — per-control detail pages, ascending `index:`,
   grouped by catalog category.

Frontmatter on every page: `title`, `category: Controls`, a shared `categoryindex`
ahead of "Controls & design tokens", and an `index` giving the order above.

## S2 — Reachability

- The Controls section MUST be reachable from `docs/index.md` entry-point nav
  (consumer path) — add a link (FR-008, SC-001).
- The catalog index MUST link to every detail page; each detail page MUST link to its
  API reference and back to the index (FR-008).

## S3 — Cross-linking (no relocation/duplication)

Existing pages **cross-link into** the new section, not move or duplicate it:
- `docs/architecture/controls.md` → link to the catalog index.
- `docs/controls-design/typed-front-door.md` → already explains typed authoring; the
  narrative links to it for the "author during implement" step.
- `docs/controls-design/design-tokens-penpot.md` → the Penpot subsection links here as
  the deep dive.

## S4 — Narrative content obligations

The narrative MUST let a reader (SC-005/SC-006) state:
- at which Spec Kit phase(s) controls are **chosen** (specify/plan), **authored**
  (implement, via the typed front door) and **validated** (the evidence/gates the
  workflow expects), with links to the typed-controls + evidence guidance/skills;
- the path from a Penpot/design-token change to control theming, and where the design-
  token single source lives (link to `design-tokens-penpot`).

## S5 — No dead links

All narrative/index/detail cross-links resolve in the built site (FR-009) — enforced
by the currency check's link-resolution clause (C5).
