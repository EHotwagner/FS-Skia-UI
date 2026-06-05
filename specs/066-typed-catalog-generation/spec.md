# Feature Specification: Typed Catalog Generation

**Feature Branch**: `066-typed-catalog-generation`  
**Created**: 2026-06-05  
**Status**: Draft  
**Input**: User description: "implement the next part of docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md" — §13 roadmap item 2: *Typed catalog generation (regenerate `catalog.yml`/`Catalog.fs` from the typed registry).*

## Context

Feature `065` shipped a compile-time-typed authoring front door — a sealed
`Widget<'msg>` plus six typed `Props`/`view` modules under the
`FS.Skia.UI.Controls.Typed` namespace (`TextBlock`, `Button`, `CheckBox`,
`Stack`, `TextBox`, `DataGrid`) — that lowers to the existing `Control<'msg>` IR.

The control **catalog** is the framework's contract describing which controls
exist and their authoring facts (id, display name, category, owning module,
required attributes, events, accessibility role). It lives in two places today,
each **hand-authored and independently maintained**:

- `src/Controls/catalog.yml` — the structured YAML source (`supportedCount: 47`).
- `src/Controls/Catalog.fs` — the public F# `Catalog.supportedControls` list.

Because the two are hand-synced, and neither is derived from the typed registry
introduced in `065`, a fact about a typed control (e.g. that `Button`'s events
are `[onClick]`, or that `DataGrid`'s category is `data`) can be stated in
**three** places — the typed `Props` module, `catalog.yml`, and `Catalog.fs` —
and drift apart silently. This violates the repository's established principle
that governance artifacts are *generated from a single source, not hand-synced*
(the same principle that makes `validation.contract.yml` generated from
`Routing.fs` and the `.claude` skill tree generated from `.agents`).

This feature introduces a single canonical fact table for the six typed-covered
controls — a build-front `catalogFacts` declaration associated with the `065`
typed modules (its `Module` and required-attribute facts are mechanically
cross-checked against the `FS.Skia.UI.Controls.Typed` surface) — generates both
catalog rows deterministically from it, and adds a currency (drift) gate so
hand-edits that diverge from regeneration fail the build — exactly as
`TargetMetadataDrift` and `SkillSyncCheck` do for their artifacts. The fact table,
not `catalog.yml` or `Catalog.fs`, becomes the one place each catalog fact is
declared.

### Scope boundary (what this feature is *not*)

- It does **not** type or migrate the other 41 controls; those rows stay
  hand-authored. Only the six `065` controls become generated.
- It does **not** change any control's catalog facts. The generated rows are
  **byte-identical** to today's hand-authored rows for those six controls
  (parity, proven by test). The catalog's observable content is unchanged.
- It does **not** add or remove controls, change `supportedCount: 47`, change
  categories, or alter the public `ControlDefinition` shape consumed by
  `Catalog.supportedControls`.
- It does **not** touch the typed authoring surface (`Widget`/Props/`view`)
  shipped by `065`, the legacy `Attr`/`*.create` API, or the lowering pipeline.
- No design-token / Penpot work; that is feature `069`.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Catalog facts for typed controls have a single source (Priority: P1)

A framework maintainer changes an authoring fact for one of the six typed
controls — for example, renames `Button`'s display name or adds an event. Today
they must remember to edit the typed module, `catalog.yml`, **and** `Catalog.fs`
and keep all three consistent by hand. After this feature, they edit the fact in
**one** canonical place (the typed registry), regenerate, and both catalog
artifacts update together; the drift gate fails the build if they forget to
regenerate.

**Independent test**: Edit a catalog-relevant fact for a typed control in the
registry source, run the regeneration target, and confirm both `catalog.yml` and
`Catalog.fs` reflect the change identically with no manual edits to either file.
Separately, edit one of the generated files by hand without regenerating and
confirm the drift gate fails and names the divergent control.

### User Story 2 - Generated catalog rows are provably identical to today's (Priority: P1)

A reviewer must be able to confirm the migration is non-behavioral: the six
generated rows match the previously hand-authored rows exactly, so no downstream
consumer of the catalog (gallery, tests, generated guidance) observes any change.

**Independent test**: A parity check asserts each of the six generated catalog
rows (in both `catalog.yml` and `Catalog.fs`) is structurally equal to the
hand-authored row captured before the migration. `ControlsCatalogCheck` and the
existing `CatalogTests` pass unchanged.

### User Story 3 - The drift gate is wired into routing and runs on catalog changes (Priority: P2)

A maintainer changing `src/Controls/**` (which already escalates via the
`controls-public-surface` rule) sees the new currency gate listed by
`./fake.sh build -t Route` and run as part of validation, so catalog/registry
drift cannot merge.

**Independent test**: Run `./fake.sh build -t Route` over a branch diff that
touches the typed registry; confirm the printed gate set includes the catalog
generation-currency check, and that `Route --enforce` flags a stale generated
catalog as a missing/failed obligation.

### Edge Cases

- **A typed control's fact differs from its current catalog row.** If, at
  migration time, the typed registry would generate a row that differs from the
  hand-authored row, that is a real pre-existing inconsistency. Resolution: the
  registry value is authoritative; the discrepancy MUST be surfaced in evidence
  and the hand-authored row corrected to match, never silently overwritten
  without disclosure.
- **A control is typed but not yet cataloged (or vice versa).** The generator
  covers exactly the six `065` controls; it MUST NOT invent rows for untyped
  controls nor drop hand-authored rows for the 41 untyped controls.
- **Ordering / formatting churn.** Regeneration MUST be deterministic and
  produce stable ordering and formatting so the drift gate compares clean diffs
  and does not flap on incidental whitespace.
- **Partial regeneration.** Regenerating must update *both* `catalog.yml` and
  `Catalog.fs` from the same source in one operation; the two generated outputs
  cannot diverge from each other.

> Interacting / conflicting requirements: "generated rows must be byte-identical
> to today's hand-authored rows" (FR-004) vs. "the registry value is
> authoritative on mismatch" (edge case 1). Resolution: parity is the success
> bar for the migration commit; where today's hand-authored row is found to
> already disagree with the typed registry, the row is corrected to the registry
> value and the correction is disclosed in evidence — parity is then asserted
> against the corrected row, not the stale one.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The six typed controls' catalog-relevant facts MUST have a single
  canonical source — a `catalogFacts` table associated with the `065`
  `FS.Skia.UI.Controls.Typed` modules — such that each fact is declared in exactly
  one place and flows to both catalog artifacts by generation. The association to
  the typed modules MUST be enforced for the facts that exist as typed data
  (`Module` name and required attributes, cross-checked by test); the remaining
  descriptive facts (display name, category, purpose, events, accessibility role)
  are declared once in `catalogFacts` and exist nowhere else.
- **FR-002**: A deterministic regeneration operation MUST emit the catalog rows
  for those six controls into both `src/Controls/catalog.yml` and
  `src/Controls/Catalog.fs` from that single source, in one invocation, with
  stable ordering and formatting.
- **FR-003**: Regeneration MUST leave the rows for the other 41 hand-authored
  controls, `supportedCount: 47`, the category list, summary metadata, and the
  public `ControlDefinition` shape unchanged.
- **FR-004**: For the migration commit, every generated row MUST be byte-identical
  to the row it replaces (parity) — same text, whitespace, and field ordering, so
  the migration diff for those rows is empty — verified by an automated golden-diff
  check. (Byte-identity implies structural identity, so no catalog consumer observes
  a behavioral change.)
- **FR-005**: A generation-currency (drift) gate MUST fail when either generated
  catalog artifact diverges from what regeneration would produce from the
  registry, and MUST name the divergent control(s). The gate MUST follow the
  same pattern as the existing `TargetMetadataDrift` / `SkillSyncCheck`
  currency gates.
- **FR-006**: The drift gate MUST be discoverable through `./fake.sh build -t
  Route` for changes under `src/Controls/**`, and `Route --enforce` MUST treat a
  stale generated catalog as a blocking obligation.
- **FR-007**: Existing catalog validation (`ControlsCatalogCheck`, the
  `CatalogTests` contract, generated-guidance consumers of the catalog) MUST
  continue to pass without modification to their assertions, except where a test
  is intentionally extended to assert the new generated-vs-source parity.
- **FR-008**: Where a typed control's registry fact is found to disagree with its
  current hand-authored catalog row, the hand-authored row MUST be corrected to
  the registry value and the correction disclosed in evidence; the registry is
  authoritative.
- **FR-009**: The feature MUST NOT add a runtime dependency to
  `FS.Skia.UI.Controls`; the generation logic lives in the build/governance
  front (`FS.Skia.UI.Build`), consistent with how other artifacts are generated.
- **FR-010**: Required evidence artifacts MUST be produced: a readiness record
  describing the single-source generation and the drift gate, and a
  generated-vs-source parity matrix for the six controls.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, version, or new package. Package
  **contents** of `FS.Skia.UI.Controls` change only in that `catalog.yml` and
  `Catalog.fs` become generated outputs rather than hand-authored; the
  observable catalog content for the six rows is byte-identical. Active package
  path: `FS.Skia.UI.Controls`. No Charts package migration involved.
- **Public contract impact**: The `Catalog.supportedControls` / `ControlDefinition`
  public surface is **unchanged** in shape and values; `catalog.yml` (a consumer
  contract) content is unchanged for the six rows. No `.fsi` signature changes
  are expected. Surface baselines should not move; if they do, that is a
  regression to investigate, not an expected delta.
- **State workflow impact**: None. No stateful workflow, I/O, command, effect,
  subscription, or interpreter behavior changes. Generation is a pure,
  deterministic build-time transform.
- **Layout/rendering impact**: None. No layout, chart, DataGrid, rendering,
  screenshot, Vulkan, Skia, or unsupported-environment diagnostic behavior
  changes — the catalog describes controls but does not render them.
- **Evidence obligations**: Real evidence paths under
  `specs/066-typed-catalog-generation/readiness/`:
  `typed-catalog-generation.md` (single-source design + drift gate) and
  `typed-catalog-parity.md` (six-row generated-vs-source parity matrix). The
  `controls-public-surface` routing rule's existing expected artifacts continue
  to apply.
- **Unsupported scope**: No new controls, no migration of the 41 untyped
  controls, no token/Penpot work, no catalog schema/`supportedCount` change, no
  keyed reconciliation, no adapter changes. These are later roadmap features
  (067–070).
- **Build-target impact**: A new generation-currency gate (drift check) and a
  regeneration target (analogous to `RefreshSurfaceBaselines`) are added to
  `FS.Skia.UI.Build`; `GeneratedProductCheck` / `GeneratedGuidanceCheck` and the
  routing metadata (`validation.contract.yml`, currency-checked by
  `TargetMetadataDrift`) must reflect the new gate. `Dev`, `EvidenceGraph`, and
  `EvidenceAudit` participate via the escalated path but do not change behavior.

## Success Criteria *(mandatory)*

- **SC-001**: A maintainer can change any catalog-relevant fact for one of the
  six typed controls in exactly one source location and, after one regeneration
  command, see both `catalog.yml` and `Catalog.fs` updated consistently — zero
  manual edits to either generated file.
- **SC-002**: 100% of the six typed controls' catalog rows are generated; the
  generated rows are provably identical (parity check passes) to the
  pre-migration hand-authored rows, so no downstream catalog consumer changes
  behavior.
- **SC-003**: Hand-editing either generated catalog file out of sync with the
  registry causes the drift gate to fail and name the divergent control(s); a
  clean (regenerated) tree passes the gate.
- **SC-004**: `./fake.sh build -t Route` over a registry-touching diff lists the
  new currency gate, and the full escalated validation set the route prints
  passes, including unchanged `ControlsCatalogCheck` and `PackageSurfaceCheck`.
- **SC-005**: The catalog still reports `supportedCount: 47` with all ten
  categories present and all 47 rows valid; the 41 untyped rows are byte-for-byte
  unchanged.
- **SC-006**: Both required evidence artifacts exist, are populated, and
  disclose that generation is real (no synthetic `[S]` placeholder) and list any
  hand-authored row corrected to match the registry.

## Key Entities

- **Typed control registry**: the canonical association between the six
  `FS.Skia.UI.Controls.Typed` modules and their catalog-relevant facts (id,
  display name, category, module, purpose, required attributes, events,
  accessibility role) — the single source for generation.
- **Catalog row (`ControlDefinition`)**: one entry describing a control;
  generated for the six typed controls, hand-authored for the other 41.
- **`catalog.yml`**: the structured YAML catalog source (consumer contract);
  its six typed rows become generated outputs.
- **`Catalog.fs` / `Catalog.supportedControls`**: the public F# catalog list;
  its six typed rows become generated outputs.
- **Generation-currency gate**: a drift check (peer of `TargetMetadataDrift` /
  `SkillSyncCheck`) that fails when a generated catalog artifact is stale
  relative to the registry.

## Assumptions

- The six controls covered by generation are exactly the `065` slice:
  `TextBlock`, `Button`, `CheckBox`, `Stack`, `TextBox`, `DataGrid`.
- The catalog facts for these six controls are currently consistent between
  `catalog.yml` and `Catalog.fs`; any discovered inconsistency is treated as a
  pre-existing bug corrected under FR-008 with disclosure.
- The generation logic belongs in `FS.Skia.UI.Build` (the governance front),
  consistent with `validation.contract.yml` and skill-tree generation, so no
  runtime dependency is added to the shipped Controls package.
- `supportedCount` stays 47 and the catalog schema version is unchanged; this
  feature changes *how* six rows are authored, not *what* the catalog contains.
- Routing already escalates `src/Controls/**` via `controls-public-surface`, so
  no new routing **rule** is required — only registering the new currency gate in
  the existing rule's gate set / metadata.
