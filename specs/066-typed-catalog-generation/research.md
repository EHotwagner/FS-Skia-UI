# Phase 0 Research: Typed Catalog Generation

All Technical Context unknowns resolved. The spec is detailed and already
clarified; the open questions here are design choices among existing repository
patterns, resolved with rationale below. No external research was required —
grounding is from source read on 2026-06-05.

## R1 — Where the single-source catalog-fact table lives

**Decision**: A typed F# value `catalogFacts : TypedCatalogFact list` (the six
controls) declared inside the new `build/Governance/CatalogGen.fs`. This is the
single source for the six rows.

**Rationale**: FR-009 forbids adding a runtime dependency to
`FS.Skia.UI.Controls` and requires generation logic in `FS.Skia.UI.Build`. The
existing precedent is `ContractView` (single-sources `validation.contract.yml`
from `Routing.fs`, both in the build front) — the canonical value and the
generator live together in `build/Governance`. The 065 typed `Props` records
carry typed *shape* (`ButtonProps.Text: string`) but **not** catalog strings
(category, displayName, purpose, accessibility role, event names), so those
facts genuinely have no prior home and cannot be reflected out of the Props.
Declaring them once in `CatalogGen` satisfies "each fact declared in exactly one
place" (FR-001) without a runtime dep.

**Alternatives considered**: (a) a shipping fact module in `src/Controls/` read
by the build — rejected: risks a runtime-surface/baseline change and inverts the
build↔package dependency. (b) reflecting facts out of the compiled `Typed`
assembly — rejected: the Props don't encode the catalog strings, so reflection
is insufficient and adds a compiled-assembly load to a currency gate.

## R2 — Splice mechanism: per-control inline marked regions

**Decision**: Each of the six rows is wrapped in its own
`BEGIN/END GENERATED: typed-catalog/<control-id>` marker pair in both files
(`#` comment markers in `catalog.yml`, `//` comment markers in `Catalog.fs`),
mirroring `GovernedBlocks` (`<!-- BEGIN GENERATED: gov/<id> -->`). The generator
splices each control's rendered row into its own region; currency checks each
region independently.

**Rationale**: The six typed controls are **non-contiguous** in both files
(`Catalog.fs` order: text-block #1, button #8, text-box #10, check-box #13,
data-grid #22, stack #24). A single contiguous generated block is therefore
impossible without reordering, and reordering would change
`Catalog.supportedControls`/`markdownSummary` observable order — a behavioral
change the spec forbids (FR-003, SC-005). Per-control regions preserve order,
leave the 41 hand-authored rows entirely outside any marker (untouched), and let
the drift diagnostic **name the divergent control** via the region id
(FR-005). `GovernedBlocks` already implements keyed multi-region splice +
currency, so the primitives are reused, not reinvented.

**Alternatives considered**: (a) one contiguous block (requires reordering —
rejected, observable churn). (b) full-file regeneration owning all 47 rows —
rejected: moves the 41 hand-authored rows into the build, out of scope.

## R3 — A new named gate vs. folding currency into `TargetMetadataDrift`

**Decision**: Add a **new named gate** `ControlsCatalogGenerationCheck` and list
it in the `controls-public-surface` routing rule.

**Rationale**: The spec is explicit and repeated (FR-006, US3, SC-004,
Build-target impact): the currency gate MUST be *listed by* `./fake.sh build -t
Route` for `src/Controls/**` and `Route --enforce` MUST block on a stale catalog.
The recent repo precedent (042/044/057/060/062) folds new single-source currency
into the existing `TargetMetadataDrift` gate, which is **not** path-routed into
`controls-public-surface` and so would not appear in the Route gate list for a
controls change. To meet the spec literally, the catalog gate follows the
**`SkillSyncCheck` precedent** instead — a standalone named gate with its own
arm. The generator's currency logic stays pure and is the single implementation;
the gate arm and (optionally, as a backstop) any future fold call the same
`CatalogGen.currency`. To avoid double-maintenance, currency is **not** also
duplicated into `TargetMetadataDrift`; the standalone gate is authoritative.
(`validation.contract.yml` naturally still currency-checks in
`TargetMetadataDrift`, which is how the new gate's presence in the contract stays
honest.)

**Alternatives considered**: fold-only into `TargetMetadataDrift` — rejected:
would not surface in the Route gate list for `src/Controls/**`, failing
FR-006/US3/SC-004.

## R4 — Regeneration target

**Decision**: A new `RegenerateCatalog` effect, interpreted at the
`Engine/Interpret.fs` edge, folded into the existing `RefreshSurfaceBaselines`
target alongside `RegenerateGovernedBlocks`, `RegenerateApiSurface`, etc. It
rewrites the six regions in both files in one invocation.

**Rationale**: `RefreshSurfaceBaselines` is already the single home for "make
every generated artifact current so the currency gates cannot trip"
(`Update.fs:97-126`). Adding `RegenerateCatalog` there keeps one regeneration
entry point and satisfies FR-002 (both files updated from one source in one
operation) and the partial-regeneration edge case (the two outputs cannot
diverge because one effect writes both).

**Alternatives considered**: a dedicated `RefreshCatalog` target — rejected:
fragments the regeneration surface; the spec's "analogous to
RefreshSurfaceBaselines" is satisfied by folding into it.

## R5 — Typed-registry correspondence: test, not build reflection

**Decision**: Assert the fact table corresponds to the six real
`FS.Skia.UI.Controls.Typed` modules via a **semantic test** in
`tests/Controls.Tests/CatalogTests.fs` (covers exactly the six ids/modules; each
fact's `requiredAttributes` agree with the typed `Props` required fields), not via
reflection inside the currency gate.

**Rationale**: Keeps the gate pure and light (`directPrerequisites = []`, no
compiled-assembly load on a focused gate) while still proving the "single source
is associated with the typed registry" claim (FR-001, Key Entities). The test is
the honest audience for the association; the gate's job is byte-currency only.

## R6 — Gate metadata (name, prerequisites, ownership)

**Decision**: `ControlsCatalogGenerationCheck`; `directPrerequisites = []`;
`timeoutClass = "focused"`; `cost = "low"`; `failureOwner = "product"` — matching
the sibling `ControlsCatalogCheck` row exactly.

**Rationale**: Groups with the existing `Controls*Check` family for
discoverability; the check is pure text comparison over committed files (no
build prerequisite), so it is cheap and focused. `validation.contract.yml` is
regenerated so the new gate appears under `controls-public-surface`, and
`TargetMetadataDrift` enforces that contract's currency.

## Parity & FR-008 (pre-existing mismatch) handling

The migration's success bar is **byte-identical** generated rows (FR-004). The
implementer captures the six pre-migration rows, points the renderer's fact table
at values that reproduce them, and asserts equality. If any typed-registry-derived
value is found to disagree with today's hand-authored row, the **registry is
authoritative** (edge case 1 / FR-008): the row is corrected to the registry value,
the correction is disclosed in `readiness/typed-catalog-parity.md`, and parity is
asserted against the corrected row. The expectation from source inspection
(`Catalog.fs:72-124`, `catalog.yml:31-375`) is that the six rows are already
internally consistent, so no correction is anticipated; the path exists for honesty.
