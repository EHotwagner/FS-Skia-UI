# Phase 0 Research: Typed-Controls Plan Closeout

All open questions were resolved in the spec's **Clarifications / Session 2026-06-06**, so
there are no `NEEDS CLARIFICATION` markers to discharge. The decisions below restate those
resolutions and record the load-bearing facts the skill authoring depends on, gathered by
reading the shipped source on `main`.

## Decision 1 — Where to document the catalog-generation pattern (US1)

- **Decision**: Fold the feature-066 worked example into the existing
  `fsharp-code-generation` skill. Do **not** create a standalone `fs-skia-catalog-generation`
  skill.
- **Rationale**: `CatalogGen` lives in `build/Governance`, which is exactly the
  `fsharp-code-generation` skill's declared scope ("F# governance library (build/Governance)
  under net10.0; build-tooling scope only"). The plan's own §16.3 allowed folding it in. A
  separate skill would be corpus sprawl for a single worked example.
- **Alternatives considered**: A new `fs-skia-catalog-generation` skill (rejected — sprawl,
  duplicates the build-tooling scope); a contributor note in the plan doc only (rejected —
  not discoverable from the skill corpus a maintainer searches).
- **Source facts** (`build/Governance/CatalogGen.fsi`, read-only): the single source is
  `val catalogFacts: TypedCatalogFact list` (the six 065-typed controls: text-block, button,
  check-box, stack, text-box, data-grid); renderers `renderFSharpRow`/`renderYamlRow` and
  splicers `spliceFSharp`/`spliceYaml` write into `typed-catalog/<id>` marked regions only,
  leaving the 41 hand-authored rows (outside markers) untouched; `currency`/`isCurrent`/
  `currencyDrift` back the drift gate, whose diagnostic points at
  `./fake.sh build -t RefreshSurfaceBaselines`. Generated outputs are `catalog.yml` +
  `Catalog.fs` (`catalogYmlRel`/`catalogFsRel`). The `Module`/required-attribute facts are
  cross-checked against the `FS.Skia.UI.Controls.Typed` surface.

## Decision 2 — How to capture the keyed-reconciliation knowledge (US3)

- **Decision**: Create a **new `fs-skia-reconciliation` capability skill** (reverses the
  spec's original Assumption A5). It teaches the diff invariants **and** records the module's
  disposition.
- **Rationale**: A dedicated skill makes the parked spike's invariants teachable and gives an
  eventual wiring feature a documented foundation rather than source-comment archaeology. The
  registry discovers skills dynamically from `SKILL.md` frontmatter `name:`, so adding one is
  a pure-authoring act with no hardcoded list to edit.
- **Alternatives considered**: A contributor note only (rejected by the clarification —
  insufficiently durable/discoverable); promoting `Reconcile` to public to document it via
  the surface baseline (rejected — violates the no-public-surface constraint, SC-005, and the
  deliberate parked-internal decision from 067).
- **Source facts** (`src/Controls/Reconcile.fsi`, read-only): `module internal Reconcile`;
  `diff: prev -> next -> ReconcileResult` is pure/total/deterministic; children match by
  `Key` first, then unkeyed residuals positionally; a `Kind` mismatch yields a whole-subtree
  `Replace`; the operation set is `NodePatch` (`Keep`/`Replace`/`Update`) + `ChildOp`
  (`ChildKeep`/`ChildMove`/`ChildInsert`/`ChildRemove`) with `UpdatePatch`/`FieldChange`/
  `AttrChange`; `ReconcileResult` carries `Diagnostics` (e.g. duplicate-key `KeyCollision`);
  `apply` proves the round-trip (reconstructs a tree structurally equal to `next`). Tests
  reach internals via `[<assembly: InternalsVisibleTo("Controls.Tests")>]`. The module is
  deliberately **not** in the Controls capability `contracts:` list (zero public-surface
  entry). The deferred integration point named in the skill is the render/diff path that a
  future wiring feature would touch.

## Decision 3 — Plan-document refresh scope (US2)

- **Decision**: Refresh **only** the forward-looking/status sections (status-by-feature table,
  §13 roadmap, §16 skills backlog). Leave the preserved provenance body (§1 onward) unedited.
- **Rationale**: The document explicitly retains its original plan text from §1 onward for
  provenance (stated at the top of the report). Refreshing only the status/forward sections
  resolves the FR-004/FR-006-vs-provenance tension noted in the spec.
- **Stale items to correct** (verified against the doc + `git log` on `main`): the status row
  shows 069 as "awaiting clarify/plan" and 070/071+ as "Planned" though all of 065–073
  merged; §16.2 instructs updating a non-existent `fs-skia-project` skill (re-attribute the
  "typed-is-preferred" guidance to `fs-skia-typed-controls`, per Assumption A3); §16.3 lists
  proposed skills without recording which shipped (`fs-skia-typed-controls`,
  `fs-skia-design-tokens`, now `fs-skia-reconciliation`) vs. which were folded
  (`fs-skia-catalog-generation` → `fsharp-code-generation`); §13 does not record 073
  (animations) as the delivered "motion" item.
- **Alternatives considered**: Rewriting the whole document (rejected — destroys provenance);
  leaving it stale with an erratum note (rejected — FR-004 requires the table itself match
  reality).

## Mechanics fact (applies to US1 + US3)

Skills are generated from a single source: edit the canonical
`.agents/skills/<name>/SKILL.md`, then regenerate the `.claude` peer with
`./fake.sh build -t RefreshSurfaceBaselines`. `SkillSyncCheck` fails on any drift, so the
`.claude` copy must never be hand-edited. New skills are discovered from frontmatter `name:`
(`build/Governance/Evidence/SkillRegistry.fs`), so adding `fs-skia-reconciliation` needs no
hardcoded-list edit — but its `.claude` peer and the regenerated skill index
(`GENERATED.md` / skillist-reference) only appear after a `RefreshSurfaceBaselines` run.
