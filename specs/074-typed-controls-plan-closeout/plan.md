# Implementation Plan: Close Out the Typed-Controls Front-Door Plan Loose Ends

**Branch**: `074-typed-controls-plan-closeout` | **Date**: 2026-06-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/074-typed-controls-plan-closeout/spec.md`

## Summary

A **documentation / governance-only** close-out that pays down three debts left by the
typed-controls front-door programme (065–073). No runtime behavior, no public package
surface, no `.fsi` change. Three independent user stories, landable in any order:

1. **US1 (P1)** — Document the feature-066 single-source catalog-generation pattern in the
   existing `fsharp-code-generation` skill (canonical `.agents` source, regenerated `.claude`
   peer). The worked example names `catalogFacts`, the two generated artifacts
   (`catalog.yml` + `Catalog.fs`), `RegenerateCatalog` within `RefreshSurfaceBaselines`, and
   the `ControlsCatalogGenerationCheck` drift gate, and states that hand-editing a generated
   artifact fails the gate.
2. **US2 (P2)** — Refresh the stale forward-looking sections of
   `docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md` (status
   table, §13 roadmap, §16 skills backlog) so they match `main`'s git history, remove the
   reference to a non-existent `fs-skia-project` skill, and re-attribute the
   "typed-is-preferred" guidance to `fs-skia-typed-controls`. The preserved provenance body
   (§1 onward) is left unedited.
3. **US3 (P3)** — Add a new `fs-skia-reconciliation` capability skill (canonical `.agents`
   source, regenerated `.claude` peer) teaching the keyed-VDOM-diff invariants and recording
   the module's disposition: `module internal`, property-tested, deliberately unwired,
   parked — with live-render-path integration named as deferred future work. The `Reconcile`
   module itself is **not** touched.

**Technical approach**: pure Markdown/skill authoring plus one regeneration target. The
single mechanical contract is the skill single-source pipeline — edit `.agents`, run
`./fake.sh build -t RefreshSurfaceBaselines`, never hand-edit `.claude`. No F#, no
`Routing.fs` rule, no baseline content change.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (no compiled code changes this feature; the only
F# touched is read-only reference material for the skill prose: `build/Governance/CatalogGen.fsi`
and `src/Controls/Reconcile.fsi`).
**Primary Dependencies**: None added. Relies on the existing skill single-source pipeline
(`SkillRegistry`/`SkillTreeGen` → `RefreshSurfaceBaselines`, enforced by `SkillSyncCheck`),
the feature-066 catalog generator (`CatalogGen`, `ControlsCatalogGenerationCheck`,
`RegenerateCatalog`), and the feature-067 reconciliation module (`src/Controls/Reconcile.*`).
**Testing**: Governance gates only — `SkillSyncCheck`, `SkillQualityCheck`,
`SkillContractPathCheck`, `TemplateUpdateSkillPackageCheck` (and the `.specify`-driven
`GeneratedGuidanceCheck` / `TemplateDrift`). No new product or unit tests; no failing-first
semantic test, because there is no behavior change (Tier 2). Authoritative gate list comes
from `./fake.sh build -t Route` **after** the skill edits land.
**Target Platform**: Windows and Linux (governance build; platform-agnostic Markdown).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

This is a **Tier 2 (internal change)** per the Change Classification: no public API surface,
no new dependency, no inter-project contract change, no observable behavior change. The
constitution's Tier 1 obligations (`.fsi` updates, surface-area baseline updates) do **not**
apply and MUST remain untouched (SC-005). Principle I (Spec→FSI→Tests→Impl) and Principle VI
(failing-first behavior tests) are satisfied vacuously — there is no behavior to sketch in
FSI or test; the "tests" for documentation are the skill-currency/quality governance gates.
Principle II (visibility in `.fsi`) is preserved: `Reconcile` stays `module internal`, never
promoted. Principle V (synthetic disclosure) is not engaged — no mocks/fakes/placeholders are
introduced. No gate violations; no justification entries required.

### Repository Governance Decisions

- **Template ownership**: N/A — no source, sample, test, package-policy, or command-surface
  change ships into a generated project. The only template-adjacent gate is
  `TemplateUpdateSkillPackageCheck`, which the skill-source routing pulls in; it is satisfied
  because no package set or template pin changes. `.template.config/template.json` is not
  touched.
- **Dependency impact**: N/A — no `Directory.Packages.props`, `docs/dependencies.md`,
  generated-template inclusion, or `DependencyReport` change; no dependency added or removed.
- **Command-surface impact**: No new or changed FAKE target. `RefreshSurfaceBaselines` is
  **run** (to regenerate the `.claude` peers) but its definition is unchanged. Validation
  uses only the gates `Route` prints; FAKE-backed commands are run sequentially in
  deterministic order (`.fake` state is shared and not concurrency-safe). Expected order
  after the edits: `1. ./fake.sh build -t Route` → run exactly the printed gates, e.g.
  `2. ./fake.sh build -t Dev`, then `SkillSyncCheck`, `SkillQualityCheck`,
  `SkillContractPathCheck`, `TemplateUpdateSkillPackageCheck`, `GeneratedGuidanceCheck`,
  `TemplateDrift`.
- **Generated project impact**: N/A — default/minimal generated contents, selected-Controls
  guidance, generated-project local skills, validation logs, and placeholder/excluded-history
  scans are unaffected. The two skills edited/added are repo-local governance skills under
  `.agents/skills/**`, not generated-project capability skills.
- **Evidence paths**: Readiness artifacts under
  `specs/074-typed-controls-plan-closeout/` — this `plan.md`, `research.md`, `data-model.md`,
  `contracts/`, `quickstart.md`, the forthcoming `tasks.md`/`tasks.deps.yml`, and the routed
  tier's `readiness/` logs. Skill-currency evidence is the in-sync canonical↔generated pair
  for `.agents/skills/fsharp-code-generation/SKILL.md` ↔
  `.claude/skills/fsharp-code-generation/SKILL.md` and
  `.agents/skills/fs-skia-reconciliation/SKILL.md` ↔
  `.claude/skills/fs-skia-reconciliation/SKILL.md`, proven by `SkillSyncCheck`. The refreshed
  plan document is `docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md`.
- **`.fsi` / contract impact**: N/A — no `.fsi` signature, public doc, surface baseline, or
  sample contract changes (SC-005). `Reconcile.fsi` and `CatalogGen.fsi` are **read** as
  reference material for the skill prose; neither is edited. The governed "contract" this
  feature must honor is the skill single-source invariant (`.agents` is canonical, `.claude`
  is generated) — see `contracts/`.
- **MVU/effect boundary**: N/A — no stateful or I/O-bearing work. No `Model`/`Msg`/`Effect`/
  `init`/`update`/interpreter is introduced or changed. (The `Reconcile` module being
  documented is pure and remains unwired.)
- **Synthetic evidence**: None. No mocks, fakes, placeholders, canned responses, or
  in-memory substitutes are introduced, so no `[S]`/`[SEH]` disclosure is required. Every
  task is expected to land as `[X]` against real governance-gate evidence.
- **Test evidence**: Governance gates serve as the test evidence — `SkillSyncCheck`
  (currency), `SkillQualityCheck` (skill quality), `SkillContractPathCheck` (contract-path),
  `GeneratedGuidanceCheck` (plan completeness). No behavior-changing code, so no
  failing-first unit/property test is authored (Principle VI applies to behavior-changing
  code; this feature changes none).
- **Observability**: N/A for runtime diagnostics (no runtime code). The relevant
  observability is gate diagnostics: `SkillSyncCheck` names the drifted skill on staleness,
  `currencyDrift` names the divergent control/file for the catalog gate, and
  `GeneratedGuidanceCheck` names any empty governance area — all already actionable.
- **Deferred scope**: Explicitly deferred to separate future features — (a) wiring keyed
  reconciliation into the live render/diff path (the skill documents the parked module, it
  does not wire it); (b) creating a standalone `fs-skia-catalog-generation` skill (folded
  into `fsharp-code-generation` instead). No visual/screenshot, release-validation, or
  distribution work is in scope.

## Project Structure

Documentation/governance paths only — no `src/**` or `build/**` files are modified.

```
.agents/skills/
  fsharp-code-generation/SKILL.md        # US1: edit — add the catalog-generation worked example
  fs-skia-reconciliation/SKILL.md        # US3: NEW canonical skill source
.claude/skills/
  fsharp-code-generation/SKILL.md        # US1: REGENERATED from .agents (never hand-edited)
  fs-skia-reconciliation/SKILL.md        # US3: REGENERATED from .agents (never hand-edited)
  GENERATED.md                           # regenerated skill index (RefreshSurfaceBaselines)
docs/reports/
  2026-06-05-1802-typed-controls-front-door-implementation-plan.md   # US2: refresh status/§13/§16 only

specs/074-typed-controls-plan-closeout/
  spec.md                                # done
  plan.md                                # this file
  research.md                            # Phase 0 output
  data-model.md                          # Phase 1 output (documentation artifacts as entities)
  contracts/skill-governance-contracts.md# Phase 1 output (governed-artifact contracts touched)
  quickstart.md                          # Phase 1 output (maintainer how-to)
  checklists/                            # existing quality checklist(s)

# READ-ONLY reference material (cited by the skills, never edited):
build/Governance/CatalogGen.fsi          # US1 fact-table / drift-gate reference
src/Controls/Reconcile.fsi               # US3 invariants / disposition reference
```

### Per-story file map

- **US1** → edit `.agents/skills/fsharp-code-generation/SKILL.md` (add a "C13 / product-surface
  generator" worked example section + a `Related` link to `fs-skia-typed-controls`), then
  regenerate its `.claude` peer.
- **US2** → edit only the status table (lines ~17–32), §13 roadmap (line ~437+), and §16
  skills backlog (lines ~481–533) of the plan report; leave §1 onward provenance text intact.
- **US3** → create `.agents/skills/fs-skia-reconciliation/SKILL.md` with frontmatter `name:
  fs-skia-reconciliation`, then run `RefreshSurfaceBaselines` so `SkillRegistry` discovers it,
  generates the `.claude` peer, and refreshes `GENERATED.md` / the skill index.

## Phasing & sequencing notes

The three user stories are independent and may land in any order. Recommended order matches
priority: US1 (substantive backlog) → US2 (record correction, references US1's outcome in
§16) → US3 (new skill). Each skill-touching story ends with one `RefreshSurfaceBaselines` run
and `SkillSyncCheck`; batching both skill edits before a single regeneration is acceptable and
cheaper. Run `Route` after the edits and run only the gates it prints, sequentially.
