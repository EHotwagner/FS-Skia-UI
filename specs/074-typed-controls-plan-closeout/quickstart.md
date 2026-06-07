# Quickstart: Typed-Controls Plan Closeout

A maintainer's how-to for the three close-out items. Each is independent; recommended order is
US1 → US2 → US3. **Never hand-edit `.claude/skills/**`** — edit `.agents` and regenerate.

## Prerequisites

- On branch `074-typed-controls-plan-closeout`.
- `./fake.sh` works (FAKE-backed; run FAKE targets **sequentially**, never concurrently —
  `.fake` state is shared).

## US1 — Document the catalog-generation pattern

1. Read the read-only reference: `build/Governance/CatalogGen.fsi`.
2. Edit `.agents/skills/fsharp-code-generation/SKILL.md`: add a "product-surface generator"
   worked-example section that names `catalogFacts`, `catalog.yml` + `Catalog.fs`,
   `RegenerateCatalog` (within `RefreshSurfaceBaselines`), `ControlsCatalogGenerationCheck`,
   the `Typed`-surface cross-check, and the "hand-editing a generated region fails the gate"
   rule. Add a `[[fs-skia-typed-controls]]` link under `Related`.
3. Regenerate the peer: `./fake.sh build -t RefreshSurfaceBaselines`.
4. **Acceptance**: a reader can name the fact table, both artifacts, the regen target, and the
   drift gate without reading source (SC-001).

## US2 — Refresh the implementation-plan report

1. Open `docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md`.
2. Edit **only** the status header + status-by-feature table, §13 roadmap, and §16 skills
   backlog:
   - Mark 065–073 merged with squash commits; remove "awaiting"/"Planned" for merged work.
   - §13: record 073 (animations) as the delivered "motion" item.
   - §16.2: delete the `fs-skia-project` row; re-attribute "typed is preferred" to
     `fs-skia-typed-controls`; mark the catalog-generation item **done** (→ US1).
   - §16.3: record shipped (`fs-skia-typed-controls`, `fs-skia-design-tokens`,
     `fs-skia-reconciliation`) vs. folded (`fs-skia-catalog-generation` →
     `fsharp-code-generation`).
3. Leave §1 onward (provenance body) untouched.
4. **Acceptance**: cross-check every status claim against `git log` on `main` — zero
   contradictions, zero `fs-skia-project` references (SC-003).

## US3 — Add the `fs-skia-reconciliation` skill

1. Read the read-only reference: `src/Controls/Reconcile.fsi`.
2. Create `.agents/skills/fs-skia-reconciliation/SKILL.md` with frontmatter `name:
   fs-skia-reconciliation`, a one-line `description`, `compatibility`, and
   `metadata.{author,source}`. Body teaches the invariants, key handling, operation set,
   property-test approach, and the module **disposition** (internal, tested, deliberately
   unwired, parked; integration deferred — name the integration point).
3. Regenerate + discover: `./fake.sh build -t RefreshSurfaceBaselines` (generates the
   `.claude` peer and refreshes the skill index).
4. **Acceptance**: a reader can answer "is it dead code?", "what are the diff invariants?",
   and "what would wiring it take?" from the skill alone (SC-004).

## Validate (after the edits land)

Run the router first, then run **only** the gates it prints, sequentially:

```
./fake.sh build -t Route          # authoritative tier + minimal gate list for the diff
# then, e.g. (whatever Route prints):
./fake.sh build -t Dev
./fake.sh build -t SkillSyncCheck
./fake.sh build -t SkillQualityCheck
./fake.sh build -t SkillContractPathCheck
./fake.sh build -t TemplateUpdateSkillPackageCheck
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateDrift
```

**Done when**: every gate `Route` prints passes (SC-006); `SkillSyncCheck` reports zero drift
for both skills (SC-002); the public surface baseline shows zero delta (SC-005).
