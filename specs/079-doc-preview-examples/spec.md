# Feature Specification: Demonstrative Control Preview Images in Published Docs

**Feature Branch**: `079-doc-preview-examples`
**Created**: 2026-06-08
**Status**: Draft
**Input**: User description: "the preview images of controls in the gh docs are mostly just white boxes, maybe with the name of the control on it. improve that to more elaborate examples that show off the characteristics/functionality of the control."

## Context & Problem

The published documentation site (feature 078) added a **Controls** section with one
detail page per supported control, each embedding a **render-only preview image** at
`docs/img/controls/<id>.png`. These previews are produced by rendering each control
through the typed front door with its **default props** (empty text, no items, no
rows, unselected state). For most controls the default state has little or no visible
content, so the committed PNGs are effectively **blank/near-blank 320×160 boxes** — a
large fraction are the minimum ~363-byte empty canvas. A reader scanning the catalog
cannot tell what a `Data Grid`, `List Box`, `Slider`, `Line Chart`, or `Switch` looks
like or does from its preview.

This feature replaces those default-state previews with **demonstrative previews**:
each control is rendered with representative sample content and state that conveys the
control's purpose and characteristics at a glance, while preserving the existing
**render-only honesty** guarantees (the image is a real raster of the real control
scene — never a fabricated, placeholder, or 1×1 image).

The feature additionally **repositions the Controls section in the published-docs
navigation** so it sits with the example/learning material rather than at the top of
the sidebar.

## Clarifications

### Session 2026-06-08

- Q: How should "put controls under examples / above guides" be realized in the docs nav? → A: Reorder the Controls category — keep it as its own top-level nav category but change its ordering so it renders immediately **below Examples** and **above Guides**; detail pages and preview assets stay in `docs/controls/` (no file restructuring, no page nesting).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Evaluating a control from its catalog preview (Priority: P1)

An app developer browsing the published Controls catalog wants to understand, at a
glance, what each control looks like and what it does before opening its API page.
Today most previews are blank boxes, so they convey nothing. After this feature, the
developer sees a populated, representative rendering for each control — a button with a
label, a list with several items and a highlighted selection, a slider positioned
mid-track, a checkbox in a checked state, a chart with a sample series — and can
quickly decide whether the control fits their need.

**Independent test**: Open the catalog index and several detail pages; confirm each
embedded preview shows recognizable, control-specific content (not an empty box), and
that the content visibly reflects the control's primary characteristics.

### User Story 2 - Preview reflects documented usage (Priority: P2)

A developer reading a control's detail page wants the preview to correspond to a
realistic use of the control, consistent with the page's described usage, so the image
is trustworthy rather than arbitrary. After this feature, each preview depicts a
representative configuration of the control that matches how the control is actually
used (the same kind of content the usage example describes), keeping image and prose
coherent.

**Independent test**: For a sample of detail pages, confirm the preview's depicted
content is consistent with the control's documented purpose and required attributes
(e.g., a control documented as requiring `columns`/`rows` shows columns and rows).

### User Story 3 - Previews stay honest and current (Priority: P1)

A maintainer regenerating docs must be able to trust that every preview is a real,
decodable render of the current control and that the currency gate still protects
against blank, missing, stale, undecodable, or orphaned previews. After this feature,
the demonstrative previews are produced through the same real render-only path and the
currency gate continues to pass/fail on the same honesty conditions, now additionally
guarding against a preview regressing to trivial/empty content.

**Independent test**: Run the controls-catalog currency gate against a tree with a
deliberately blanked or removed preview and confirm it fails; run it against the
regenerated demonstrative previews and confirm it passes.

### User Story 4 - Finding controls alongside the examples (Priority: P2)

A developer learning the library scans the docs navigation sidebar expecting reference
and example material grouped together. After this feature, the **Controls** section is
repositioned to sit directly **below the Examples section and above Guides**, so a
reader exploring runnable examples naturally arrives at the controls catalog next.

**Independent test**: Build the site and inspect the navigation sidebar; confirm the
Controls category renders immediately below Examples and above Guides, with its detail
pages and previews still served from `docs/controls/`.

### Edge Cases

- **Genuinely non-renderable controls**: A control that cannot be honestly rendered in
  the deterministic render-only environment keeps the existing **honest "unsupported"
  declaration** on its detail page and commits no image — never a fabricated or
  placeholder preview to fill the gap.
- **Controls whose sample needs interaction/animation**: Controls whose value is in
  motion or interaction (e.g., overlays, menus, animated states) are shown in a single
  representative static frame that best conveys their purpose; the limitation is stated
  rather than faked.
- **Content overflow**: Sample content that would exceed the preview canvas is sized,
  truncated, or arranged so the rendering remains representative and legible rather
  than clipped into meaninglessness.
- **Theme**: Previews continue to render against a single, consistent documented theme
  so they are visually comparable across the catalog.
- **Determinism**: Sample content must be fixed (no clocks, randomness, or
  environment-dependent data) so regenerated previews are byte-stable and the gate does
  not flap.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Each supported control's preview image MUST depict the control rendered
  with **representative sample content and/or state** that demonstrates the control's
  primary characteristics and functionality, rather than its empty default state.
- **FR-002**: The sample content for each control MUST be defined per-control from a
  single declared source so that what each preview shows is explicit, reviewable, and
  reproducible (not incidental to default props).
- **FR-003**: Demonstrative previews MUST be produced through the existing **real
  render-only evidence path** (an off-window raster of the actual control scene). No
  preview may be a fabricated, hand-drawn, placeholder, metadata-only, or 1×1 image.
- **FR-004**: Every produced preview MUST remain **decodable**, **non-1×1**, and carry
  **non-trivial visual content** (visibly more than an empty canvas), and this MUST be
  verifiable by the existing image-validation used by the currency gate.
- **FR-005**: The currency gate that guards the controls catalog MUST continue to fail
  on a missing, stale, undecodable, blank/trivial, or orphaned preview, and a
  demonstrative preview that regresses to empty/near-empty content MUST be treated as a
  failing (trivial) preview.
- **FR-006**: Where a control's detail page documents a usage example, the preview's
  depicted configuration MUST be **consistent with** that documented usage so image and
  prose stay coherent.
- **FR-007**: A control that genuinely cannot be honestly rendered MUST retain its
  explicit honest **unsupported** declaration and commit no image; the count of such
  controls MUST be visible in the preview evidence record (no silent omission).
- **FR-008**: Regenerating the previews on a render-capable host MUST be **deterministic
  and idempotent** — the same control state produces byte-identical output — so the
  committed assets and the gate do not drift between runs.
- **FR-009**: Previews MUST be committed as docs **source assets** consumed by the
  GPU-free docs build unchanged; the docs/site build MUST NOT require a render-capable
  host.
- **FR-010**: The per-control preview evidence record MUST be updated to reflect the new
  demonstrative renders (per-control: decodable, dimensions, content classification,
  renderer mode, and any unsupported declaration), replacing the prior default-state
  record.
- **FR-011**: The **Controls** section MUST be repositioned in the published-docs
  navigation so it renders **immediately below the Examples section and above Guides**.
  It MUST remain its own top-level navigation category; its detail pages and preview
  assets MUST stay under `docs/controls/` (no page nesting or file relocation), and all
  existing cross-links into the section MUST continue to resolve.

> Interacting / conflicting requirements: richer sample content (FR-001) vs. the
> fixed/legible preview canvas and determinism (Edge Cases, FR-008) — resolution:
> sample content is sized/arranged to remain representative and legible within a
> consistent, deterministic canvas; if a control's demonstration genuinely needs more
> room, the canvas size for that control may be adjusted but MUST stay fixed and
> documented for that control, never variable per run.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, contents, version, or generated-consumer
  change. This is a docs-content + governance-generation feature; it ships nothing into
  the `dotnet new fs-skia-ui` template. The active controls package path is unchanged;
  no Charts-package migration is involved.
- **Public contract impact**: No public `.fsi` signatures, documented public APIs,
  sample contracts, or public surface baselines change. The preview machinery *reads*
  the existing public control surface; it does not redefine it. (Internal governance
  build code may gain an internal per-control sample definition; that is build-tool
  internal surface, not product public contract.)
- **State workflow impact**: No runtime stateful-workflow, I/O, command, effect,
  subscription, or interpreter behavior change in the framework. Preview generation
  constructs fixed control state at the governance/render edge only.
- **Layout/rendering impact**: No change to control layout/rendering behavior, the Skia
  pipeline, or unsupported-environment diagnostics. The feature changes *what sample
  state is fed in* for previews, not how controls render; previews remain render-only
  off-window rasters and continue to distinguish honest "unsupported" from real
  rendering failure.
- **Evidence obligations**: Real evidence paths under
  `specs/079-doc-preview-examples/readiness/`: updated **per-control preview evidence**
  (decodable / dimensions / content classification / renderer mode / unsupported
  count), a **controls-catalog currency** report (gate PASS with the demonstrative
  previews; FAIL on a blanked/missing/orphan preview), and a **docs build** record
  (strict site build copies all previews with resolving image links).
- **Unsupported scope**: No new controls, no control-behavior or visual redesign, no
  docs-theme redesign, no API-reference-generator change, no live/animated/interactive
  previews, no release/distribution/platform change. Controls that cannot be honestly
  rendered remain explicitly unsupported, not faked.
- **Build-target impact**: `Dev`, `Verify`, `Ci`, `PackLocal`, `TemplateCheck`,
  `DependencyReport`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` do not change
  behavior. The existing controls-catalog currency gate and the
  `RefreshSurfaceBaselines` regeneration path may be extended to carry per-control
  sample state and the strengthened trivial-content guard; `GeneratedGuidanceCheck`
  behavior is otherwise unchanged.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of supported controls that *can* be honestly rendered have a preview
  whose visual content is demonstrably richer than an empty canvas (measured as a
  meaningful increase in rendered non-background content over the prior default-state
  preview), and 0 such controls remain near-empty.
- **SC-002**: For every control with a documented usage example, the preview's depicted
  configuration is consistent with that usage (verified by review of a defined sample
  of pages, with 0 contradictions).
- **SC-003**: The controls-catalog currency gate passes against the regenerated
  previews and fails when any preview is made blank/trivial, missing, undecodable,
  stale, or orphaned — demonstrated by at least one negative case per failure class.
- **SC-004**: Regenerating previews twice on the same render-capable host produces
  byte-identical assets (0 spurious diffs), and the GPU-free docs site build completes
  with every preview present and every image link resolving.
- **SC-005**: The number of controls rendered vs. honestly declared unsupported is
  recorded explicitly in the preview evidence, with 0 controls silently omitted.
- **SC-006**: In the built site navigation, the Controls category appears immediately
  below Examples and above Guides, with 0 broken links into the section and the detail
  pages/previews still served from `docs/controls/`.

## Key Entities

- **Control preview asset**: The committed PNG at `docs/img/controls/<id>.png` for a
  supported control — a real render-only raster, decodable, non-1×1, non-trivial.
- **Per-control sample definition**: The single declared source describing the
  representative content/state used to render a control's preview (which attributes are
  populated, with what fixed sample values, and any per-control canvas size). The
  reviewable answer to "why does this preview show what it shows."
- **Preview evidence record**: The per-control honesty ledger (decodable, dimensions,
  content classification, renderer mode, unsupported declaration) regenerated for this
  feature.
- **Controls-catalog currency gate**: The governance check that fails on missing,
  stale, undecodable, blank/trivial, or orphaned previews and broken links.

## Assumptions

- "GH docs" refers to the published fsdocs / GitHub Pages **Controls** section added in
  feature 078; the previews to improve are the `docs/img/controls/<id>.png` assets.
- The intended improvement is a single **representative populated state** per control
  (a button with a label, a populated list with a selection, a chart with a sample
  series, etc.), not a multi-state matrix or interactive/animated showcase; a control
  may be shown in a small composed arrangement where that best conveys its purpose.
- Previews continue to render against the single documented light theme used today, for
  cross-catalog visual comparability.
- The standard preview canvas remains the current size unless a specific control's
  demonstration needs more room, in which case its (still fixed, documented) size may
  differ — the spec does not mandate a single global size change.
- Sample content is fixed and free of clocks/randomness/environment data so previews
  are deterministic and byte-stable.
- The set of "supported controls" is whatever the live catalog source holds at
  generation time (~52 today); this feature does not add or remove controls.

## Out of Scope

- Adding, removing, or redesigning controls or their rendering/layout behavior.
- Redesigning the docs theme, the catalog index/detail-page structure, or the
  API-reference generator. (Repositioning the Controls section's nav *ordering* per
  FR-011 is in scope; restructuring its pages/files is not.)
- Animated, interactive, multi-frame, hover/focus-state, or dark/light side-by-side
  previews.
- Any package, version, distribution, platform, or release change.
- Faking a preview for a control that cannot be honestly rendered — such controls keep
  their explicit unsupported declaration.
