# Feature Specification: Authoritative Controls Catalog in Published Docs

**Feature Branch**: `078-controls-doc-catalog`
**Created**: 2026-06-08
**Status**: Draft
**Tier**: Tier 2 (internal change) — governance/generated-guidance escalation; no public API/`.fsi`, package identity, or inter-project contract change (see Framework Governance Prompts).
**Input**: User description: "create an authoritative list of all controls on the gh docs using fsdoc skills, similar to what avalonia has. also add a section before explaining how these controls are to be used within the spec-kit workflow and how penpot can be used."

## Overview

The published documentation site (GitHub Pages) explains the FS.Skia.UI control
system in architecture and design-pattern prose, and the auto-generated API
reference produces one page per control type. But there is **no single
authoritative page that lists every supported control and explains it**. App
developers cannot answer "what controls exist and what does each one do?" from
one place — they must cross-reference source (`catalog.yml`), scattered prose,
and per-type API pages.

This feature adds an **authoritative Controls Catalog** to the docs site —
comparable to Avalonia's "Controls" reference — generated from the single source
of truth so it can never drift from the shipped control set. It is preceded by a
**usage narrative** that explains how a consumer authors with these controls
inside the Spec Kit workflow and how Penpot/design tokens feed the control theme.

## Clarifications

### Session 2026-06-08

- Q: Is the catalog a single index that links out, or does each control get its own authored detail page? → A: Catalog index **plus one hand-authored detail page per control** (full Avalonia parity) — each control page carries prose, usage, and an API link; the index is still generated from the authoritative catalog source.
- Q: Should control detail pages include rendered visual previews of the control? → A: Yes — a **rendered preview per control**, produced via the render-only evidence/screenshot pipeline, embedded on each detail page and currency-checked like other evidence.
- Q: Where should the new Controls section live in the docs site structure? → A: A **new top-level "Controls" section** that owns, in order, the Spec Kit usage narrative → Penpot subsection → catalog index → per-control detail pages; existing architecture/`controls-design` pages cross-link into it.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Discover and understand every available control (Priority: P1)

An app developer evaluating or building with FS.Skia.UI opens the docs site and
finds a Controls Catalog index page that lists **all** supported controls,
grouped by category, each with a short explanation of its purpose and a link to
the control's **own detail page** (carrying prose, usage, and an API reference
link). They can answer "does this framework have a date picker / data grid /
tree view?" and "what is each control for?" without reading source, and drill
into any control for full detail.

**Independent test**: Open the published Controls Catalog index; confirm every
control in the authoritative catalog source appears, grouped, each linking to a
control detail page that exists; confirm no control is missing and no listed
control is absent from the shipped framework.

**Acceptance scenarios**:

1. **Given** the framework ships N supported controls in the authoritative
   catalog, **When** a reader opens the Controls Catalog index, **Then** all N
   controls are listed, each with a name, category, one-line purpose, and a link
   to its own detail page.
2. **Given** a control's detail page, **When** the reader opens it, **Then** it
   shows the control's purpose/explanation, a rendered visual preview of the
   control, a usage example where one exists in the catalog source, and a link to
   the control's generated API reference page.
3. **Given** a new control is added to the authoritative catalog source, **When**
   the docs are regenerated, **Then** the catalog index includes the new control
   with no hand-editing of the index body, and a currency check fails if the
   index — or a required control detail page — is missing or stale relative to
   the source.

### User Story 2 - Learn how controls fit the Spec Kit workflow (Priority: P2)

A developer adopting the project's Spec Kit workflow reads a section, placed
**before** the catalog, that explains how controls are meant to be used across
the specify → plan → tasks → implement flow: where control choices belong in a
spec, how the typed control front door is authored during implementation, and
which evidence the workflow expects. They finish knowing how to go from "I need
a form" to authored, validated controls within the workflow.

**Independent test**: A reader unfamiliar with the workflow can, from the section
alone, describe at which workflow phase controls are selected, authored, and
validated, and point to the relevant skills.

**Acceptance scenarios**:

1. **Given** the usage section, **When** a reader follows it, **Then** they can
   identify the workflow phase(s) where controls are chosen and authored and the
   evidence/validation expected for control work.
2. **Given** the typed control authoring path, **When** a reader follows the
   section's links, **Then** they reach the relevant authoring guidance (typed
   front-door / typed-controls skill material) without dead links.

### User Story 3 - Understand how Penpot drives control theming (Priority: P3)

A designer or developer reads a subsection explaining how Penpot and the design
token pipeline relate to controls: that control appearance derives from design
tokens, how token values originate/round-trip with Penpot, and how a token
change flows to the typed token surface the controls consume.

**Independent test**: A reader can describe, from the subsection, the path from a
Penpot/design-token change to the control theming consumed by the framework, and
locate the design-token authoring guidance.

**Acceptance scenarios**:

1. **Given** the Penpot subsection, **When** a reader follows it, **Then** they
   can describe how a design-token change reaches control theming and where the
   token single source lives.

### Edge cases

- A control present in source but absent from the authoritative catalog source
  (or vice-versa): the catalog page reflects the authoritative source, and the
  drift is surfaced by a currency/consistency check rather than silently hidden.
- A control with no authored usage example: its entry still appears with name,
  category, purpose, and API link; the missing example is not fabricated.
- Category with a single control, or a control belonging to "Extended"/"Custom":
  still grouped and shown.
- Renamed or removed control: regenerating the page removes/renames the entry;
  the published page never lists a control that no longer ships.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The published docs site MUST include an authoritative Controls
  Catalog **index page** that lists every supported control, grouped by the
  catalog's categories, with each control linking to its own detail page.
- **FR-002**: Each control MUST have its **own detail page** that shows the
  control's name, category, a purpose/explanation, and a link to its generated
  API reference page; the catalog index entry MUST link to that detail page.
- **FR-003**: Each control detail page MUST surface the control's authored usage
  example when one exists in the authoritative catalog source, or link to a
  runnable example where available; controls without an authored example MUST
  still have a detail page with name/category/purpose/API link.
- **FR-003a**: Each control detail page MUST embed a **rendered visual preview**
  of the control, produced through the render-only evidence/screenshot pipeline
  (honest, deterministic; benign vs. blocking host-warning rules apply). The
  preview asset MUST be currency-checked so a missing, stale, or orphaned preview
  is caught the same way other generated evidence is. Where a control cannot be
  honestly rendered in the docs environment, the page MUST state that explicitly
  rather than show a fabricated or placeholder image.
- **FR-004**: The catalog **index** content MUST be **generated from the single
  authoritative catalog source** (not hand-maintained), so adding/renaming/
  removing a control in the source is reflected on regeneration without editing
  the index body. Per-control detail pages carry hand-authored prose, but their
  **required existence and linkage** are driven by the catalog source (see
  FR-005), not invented independently.
- **FR-005**: A currency/consistency check MUST fail when (a) the generated
  catalog index is stale relative to the authoritative catalog source (count or
  entries drift), or (b) a supported control is missing its required detail page
  (or a detail page exists for a control no longer in the source), consistent
  with how other generated governance artifacts are kept current.
- **FR-006**: The docs MUST present the controls material as a **new top-level
  "Controls" section** ordered: Spec Kit usage narrative → Penpot subsection →
  catalog index → per-control detail pages. The usage narrative (placed **before**
  the catalog) MUST explain how controls are selected, authored, and validated
  across the Spec Kit workflow phases (specify → plan → tasks → implement), with
  links to the relevant authoring guidance/skills. Existing architecture and
  `controls-design` pages MUST cross-link into this section rather than duplicate
  it.
- **FR-007**: The usage narrative MUST include a Penpot/design-tokens subsection
  explaining how control theming derives from design tokens and how Penpot/token
  changes reach the typed token surface the controls consume, linking to the
  design-token authoring guidance.
- **FR-008**: The catalog index page MUST be reachable from the docs site
  navigation/index so a reader can find it without knowing its URL, and each
  control detail page MUST be reachable from the index.
- **FR-009**: All cross-links from the usage narrative, the catalog index, and the
  control detail pages (to API reference, examples, and skills/guidance) MUST
  resolve within the published site (no dead links).
- **FR-010**: The displayed total control count MUST equal the authoritative
  catalog's supported count, and MUST stay correct as that count changes.

> Interacting / conflicting requirements: generation-from-source (FR-004) vs.
> per-control authored detail pages (FR-002, full Avalonia parity). Resolution:
> the **catalog index** is fully generated from the authoritative source and is
> never hand-edited; the **per-control detail pages** carry hand-authored prose
> and usage, but their required *existence and linkage* are governed by the source
> and enforced by the currency check (FR-005), so a control can never ship without
> a detail page and a detail page can never outlive its control. Authored prose
> may enrich beyond the terse catalog `purpose`, but the canonical name, category,
> and API link on each detail page derive from the source.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identities, contents, or versions change. This is
  documentation-site content plus a docs-generation step; no controls/chart/
  graph/DataGrid authoring package changes. No legacy Charts package migration is
  involved.
- **Public contract impact**: No `.fsi` signatures or surface baselines change.
  The feature consumes the existing public control surface and the authoritative
  catalog metadata; it does not alter documented public APIs. (If generation reads
  catalog/source, it reads existing data — it does not redefine it.)
- **State workflow impact**: None. No stateful workflow, I/O, commands, effects,
  subscriptions, or interpreter behavior changes.
- **Layout/rendering impact**: No change to framework layout/rendering *behavior*,
  but the feature **exercises** the existing render-only evidence/screenshot path
  to produce per-control preview images for the docs. Previews must follow
  visual-proof honesty rules (deterministic render-only mode; benign vs. blocking
  host-warning classification); no new rendering capability or visual redesign is
  introduced. The rendered HTML documentation site is the other visual output
  affected.
- **Evidence obligations**: A rendered/generated Controls Catalog index, the
  per-control detail pages, and a **per-control rendered preview image** in the
  docs output; plus the currency-check result demonstrating the index matches the
  authoritative catalog source (count + entries), that every supported control has
  its required detail page, and that every required preview asset is present and
  current (no missing/stale/orphaned previews). Previews are produced via the
  render-only evidence mode under its honesty rules. The docs build succeeds and
  the catalog index, detail pages, and previews are present in the published
  output.
- **Unsupported scope**: No new controls, no control behavior changes, no visual
  redesign of the docs theme, no release/distribution/platform changes. Not a
  redesign of the API reference generator; the catalog augments it. Penpot
  tooling itself is not built or hosted here — only documented.
- **Build-target impact**: Expected to touch the documentation generation/build
  path and a generated-currency check (in the family of `GeneratedGuidanceCheck`/
  generated-artifact currency). `Dev`, `Verify`, `Ci`, `PackLocal`,
  `TemplateCheck`, `DependencyReport`, `TemplateDrift`, `EvidenceGraph`, and
  `EvidenceAudit` are not expected to change behavior; `Route` will determine the
  authoritative gate list for the actual diff.

## Success Criteria *(mandatory)*

- **SC-001**: A reader can find, from the docs site index/navigation, a single
  page listing 100% of supported controls in under 30 seconds (no source reading
  required).
- **SC-002**: The number of controls on the catalog page equals the authoritative
  catalog's supported count exactly — zero missing, zero extra.
- **SC-003**: Every supported control has its own detail page reachable from the
  index, and every detail page has a name, category, purpose, a rendered preview
  image (or an explicit honest note where it cannot be rendered), and a resolving
  link to its API reference; 100% of index→detail and detail→API links resolve
  within the published site, and 100% of required preview assets are present and
  current.
- **SC-004**: Adding, renaming, or removing one control in the authoritative
  catalog source, then regenerating, updates the catalog index correctly with zero
  hand-edits to the index body; a stale index, or a supported control missing its
  required detail page, is caught by the currency check.
- **SC-005**: A reader unfamiliar with the workflow can, after reading the usage
  narrative, correctly state at which workflow phase controls are chosen and
  authored and where the relevant authoring guidance lives.
- **SC-006**: A reader can, after reading the Penpot subsection, correctly
  describe the path from a design-token/Penpot change to control theming and
  locate the design-token single source.

## Key Entities

- **Control catalog entry**: One supported control as defined in the authoritative
  catalog source — name, category, purpose, attributes/events (as available),
  example reference, and links to its API reference.
- **Authoritative catalog source**: The single source of truth enumerating the
  supported controls and their metadata (the catalog the framework already treats
  as authoritative), from which the docs page is generated.
- **Controls Catalog index**: The generated docs-site page presenting all entries,
  grouped by category, with the displayed total count, each linking to a control
  detail page.
- **Control detail page**: A per-control docs page (one per supported control,
  Avalonia-style) carrying the control's purpose/explanation, a rendered preview
  image, usage example where available, and a link to its generated API reference.
- **Control preview asset**: The render-only screenshot image for a control,
  produced under evidence-mode honesty rules and currency-checked against the
  catalog source (present, current, no orphans).
- **Usage narrative**: The hand-authored docs section(s) preceding the catalog
  covering the Spec Kit workflow usage and the Penpot/design-token subsection.

## Assumptions

- "gh docs" means the project's published GitHub Pages documentation site (built
  with the existing fsdocs pipeline), not GitHub issue/wiki content.
- "using fsdoc skills" means the work is carried out via the repository's fsdocs
  authoring skills (setup/build/examples/technical/api-doc) and fits the existing
  docs pipeline, rather than introducing a new docs toolchain.
- "similar to what Avalonia has" means a categorized controls reference with a
  single entry-point **index** plus a **detail page per control** (Avalonia
  parity), each linking to detail/API and examples — not a pixel-for-pixel copy of
  Avalonia's site.
- The authoritative catalog already enumerates the supported controls with
  per-control metadata (purpose, examples, category); the catalog **index** is
  generated from it rather than hand-maintained, matching how this repo keeps
  generated governance artifacts current, while the per-control detail pages carry
  authored prose whose existence/linkage the source governs.
- The per-control API reference pages already produced by the docs pipeline remain
  the deep API target that each control detail page links to; this feature adds the
  authored detail pages and index on top of them rather than replacing them.
- The "before the catalog" ordering means the usage + Penpot narrative renders
  ahead of the enumerated catalog within a **new top-level "Controls" section**
  that owns the narrative, the Penpot subsection, the catalog index, and the
  per-control detail pages; existing architecture/`controls-design` pages
  cross-link into it rather than being relocated or duplicated.
