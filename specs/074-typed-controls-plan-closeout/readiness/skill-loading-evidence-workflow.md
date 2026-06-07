# Skill single-source contract + US1/US3 gate evidence — 074

## C1 — skill single-source contract (T004)

- `.agents/skills/<name>/SKILL.md` is the **canonical** source.
- The matching `.claude/skills/<name>/SKILL.md` is **generated** by
  `./fake.sh build -t RefreshSurfaceBaselines` and **never hand-edited**.
- New skills are discovered by `SKILL.md` frontmatter `name:` (`SkillRegistry`) — no hardcoded
  list. `fs-skia-reconciliation` (frontmatter `name: fs-skia-reconciliation`) was discovered on
  the first `RefreshSurfaceBaselines` run; its `.claude` peer and the `skillist-reference.md`
  index were produced from the canonical source.
- `SkillSyncCheck` FAILS on any drift between a `.claude` peer and a fresh render of its
  `.agents` source. Zero-drift after regeneration = pass (SC-002).

## US1 — `fsharp-code-generation` gate evidence (T008)

After editing the canonical source (C13 catalog-generation worked example + the
`[[fs-skia-typed-controls]]` Related link) and regenerating the `.claude` peer:

- `SkillSyncCheck` — ✅ PASS, **zero drift** (`.agents` ↔ `.claude` in sync, SC-002).
- `SkillQualityCheck` — ✅ PASS (all rubric sections present).
- `SkillContractPathCheck` — ✅ PASS (referenced contract paths resolve).

## US1 — independent reading test (T009, SC-001)

A maintainer reading the updated `fsharp-code-generation` skill cold can, **without reading
source**, answer:

- *What is the canonical fact table?* → `catalogFacts : TypedCatalogFact list` (51 rows).
- *What are the two generated artifacts?* → `catalog.yml` (`catalogYmlRel`) and `Catalog.fs`
  (`catalogFsRel`).
- *What regenerates them?* → `RegenerateCatalog`, run inside `RefreshSurfaceBaselines`.
- *What is the drift gate?* → `ControlsCatalogGenerationCheck` (backed by
  `currency`/`isCurrent`/`currencyDrift`).
- *What happens if I hand-edit a generated region?* → it fails the drift gate; only
  `typed-catalog/<id>` marked regions are spliced and everything outside the markers is
  untouched; the diagnostic points at `./fake.sh build -t RefreshSurfaceBaselines`.
- *What is cross-checked?* → each row's `Module`/required-attribute facts vs. the
  `FS.Skia.UI.Controls.Typed` surface.

All answerable from the skill's C13 section alone → **SC-001 satisfied**.

## US3 — `fs-skia-reconciliation` gate evidence (T015)

After creating the canonical source and regenerating the `.claude` peer + skill index:

- `SkillSyncCheck` — ✅ PASS, **zero drift** (SC-002); the new skill and its peer are in sync.
- `SkillQualityCheck` — ✅ PASS (Scope, Driven-library API, Runnable example ≥2 fences,
  ≥2 research URLs, Persistent-problem mandate, `[[...]]` Related links, Sources all present).
- `SkillContractPathCheck` — ✅ PASS.
- The skill is discovered (appears in the loaded skill list and `skillist-reference.md`).

## US3 — independent reading test (T016, SC-004)

A maintainer reading `fs-skia-reconciliation` cold can answer:

- *Is `Reconcile` dead code?* → No — a **deliberately-parked internal spike**, property-tested,
  unwired by design, not abandoned.
- *What are the diff invariants?* → key-first-then-positional child matching; `Kind`-mismatch ⇒
  whole-subtree `Replace`; the `NodePatch`/`ChildOp` (+ `UpdatePatch`/`FieldChange`/`AttrChange`)
  operation set; `KeyCollision` duplicate-key diagnostic (never throws); totality, determinism,
  identity-at-rest, round-trip.
- *What would wiring it take?* → it is a separate, out-of-scope future feature; the integration
  point is the host's full-tree render/redraw step, which would call `Reconcile.diff prev next`
  and apply the resulting `NodePatch` instead of rebuilding.

All answerable from the skill alone (no source-comment inference) → **SC-004 satisfied**.
