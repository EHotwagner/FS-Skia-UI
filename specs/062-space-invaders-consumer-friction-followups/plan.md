# Implementation Plan: Space-Invaders Consumer Friction Follow-ups

**Branch**: `062-space-invaders-consumer-friction-followups` | **Date**: 2026-06-04 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/062-space-invaders-consumer-friction-followups/spec.md`

## Summary

Consolidate the SpaceInvaders2 consumer-friction feedback (SI-1…SI-10) plus the
five fourth-prompt skill-gap candidates into one feature against the current
merged state (post-061, `993d27c`; template `0.1.84`, libs `0.1.65-preview.1`).
Twelve FRs group into five workstreams:

1. **Hook execution policy (FR-001/002).** Promote the **feedback capture hook to
   `optional: false`** in its canonical template source so it auto-fires every
   phase (no manual trigger), and document a **precedence rule** for the
   *remaining* optional hooks (git commit, etc.): `auto_execute_hooks: true`
   governs whether **mandatory** hooks need a confirmation, and **never
   force-runs an optional hook** — optionals stay surfaced. Each phase skill emits
   one consolidated **effective-hooks notice** (merged across all extension files,
   deduped by `(extension, command)`, decision per entry) so the operator never
   hand-reconciles. (Closes SI-1, SI-5; folds skill-gap candidate #1.)

2. **Self-describing evidence engine (FR-004/005/007).** Make `Dev`'s **own
   output** state it writes logs/markers and does **not** compile (`Test`/`Verify`
   is authoritative); extend 061's readiness-contract schema-printing to the
   remaining format classes (`skill-loading-evidence.md`, window-visibility /
   `diagnostic-class`, SEH acceptance) so every required shape is recoverable
   **without decompiling**, backed by a generated-from-the-same-source
   `evidence-formats` reference; and have `EvidenceGraph` render the **effective
   DAG including the auto-injected Phase N+1 → Phase N edges** plus the resolved
   `skillist`-id set. (Closes SI-3, SI-7, SI-4b; folds skill-gap candidates #5, #3.)

3. **Authoring references (FR-003/006).** Ship a single discoverable
   **durable-vs-replaceable source map** (`docs/**`) naming durable vs replaceable
   `src/**/*.fs`, the `GovernanceTests`-durable / `BehaviorTests`-replaceable
   split, the must-survive source-scan strings, and a pre-design pointer to the
   `fs-skia-scene` record-label pitfall; and a repo-local **`skillist`/`owns:`
   quick reference** generated from the live `SKILL.md` registry. (Closes SI-2,
   SI-9, SI-4a; folds skill-gap candidate #3.)

4. **Mechanical symbol consistency + pitfalls (FR-008/009).** Add a **compiled,
   deterministic cross-artifact symbol set-difference** (Msg cases, union/`Screen`
   variants, entity record names, FR-/SC- IDs across `plan.md`/`data-model.md`/
   `tasks.md`), surfaced as a new analyze detection pass; and add a `Result.Ok`/
   `Result.Error` shadowing entry to `fs-skia-skiaviewer` "Common pitfalls".
   (Closes SI-6, SI-8; folds skill-gap candidate #4.)

5. **Ship the recurring arcade helpers (FR-010/011/012).** Ship the two
   thrice-re-implemented primitives — a deterministic seeded RNG
   (`seedRng`/`nextRng`/`nextBelow`, splitmix64 seed → xorshift64 stream, pure, no
   ambient `System.Random`) and `reserveHudBand` — as **real public API in
   `FS.Skia.UI.SkillSupport`** with a skill reference and a **new per-package
   surface baseline** (`FS.Skia.UI.SkillSupport.txt`), the only Tier-1 escalation
   in this feature. The remaining helper candidates and the "generated game
   simulation core" skill stay documented/deferred with recorded rationale.
   (Closes SI-10; escalates 060 FR-008 / 061 FR-011 D8; folds skill-gap
   candidate #2.)

All deferred design decisions are resolved in [research.md](./research.md)
(D1–D12); entities in [data-model.md](./data-model.md); interface contracts in
[contracts/](./contracts/); verification in [quickstart.md](./quickstart.md).

**Change classification: Tier 1**, driven solely by FR-010 (new public
`FS.Skia.UI.SkillSupport` `.fsi` surface + a new per-package surface baseline).
Every other workstream is Tier 2 content/governance (template, skill, docs,
self-describing diagnostics) and consumer-contract-bearing, so Route escalates the
gate list regardless. **Route is authoritative — re-run after each change-set;**
the FR-010 helper tasks pull in `.fsi` + surface-baseline gates that the
content-only change-sets do not.

## Technical Context

**Language/Version**: F# / .NET (`net10.0`); governance/authoring libs
`FS.Skia.UI.Build` and `FS.Skia.UI.SkillSupport`.
**Primary Dependencies**: none new. FR-010 reuses already-pinned types only
(`FS.Skia.UI.Scene.Rect` is *not* referenced — `reserveHudBand` takes/returns
plain `float`-band tuples to keep SkillSupport dependency-light; see D10).
Touches `build/Governance/**` (Evidence scans + front-end output + a symbol-diff
helper), `src/SkillSupport/**` (new `Random` + `Hud` modules), Spec Kit phase
skills (`.agents/skills/**` → generated `.claude/**`), authoring templates
(`.specify/**`, `template/**`, `template/feedback/**`), and generated docs
(`template/base/docs/**`).
**Testing**: Expecto governance/unit tests (new: RNG determinism/replay, band
clamp, symbol-diff set algebra); FAKE targets per Route; FSI exercising the new
`SkillSupport` surface; a fresh `dotnet new fs-skia-ui --feedback true` project as
the FR-001/004/005 verification harness (the feedback extension is **not**
installed in this repo, so FR-001's promotion is verified in the template / a
generated project — see Assumptions).
**Target Platform**: Windows and Linux (no runtime/rendering change; the RNG and
HUD-band helpers are pure value-type utilities).
**Routing baseline**: spec-only diff routes `tier=focused-authority`;
`gates=Dev, GeneratedGuidanceCheck, TemplateDrift` (matched `specify-catchall`).
This **escalates** as change-sets land: governance/template/skill edits pull in
`TemplateCheck, GeneratedProductCheck, SkillSyncCheck, TargetMetadataDrift,
SkillQualityCheck, EvidenceGraph, EvidenceAudit`; the FR-010 helper change-set
pulls in `PackageSurfaceCheck`/`PerPackageSurfaceDiff`. **Run `./fake.sh build -t
Route` against the actual diff for the authoritative list.** No `NEEDS
CLARIFICATION` remains.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Re-checked post-design: **PASS.**

- **Principle I (Spec → FSI → tests → impl).** Applies non-degenerately this round
  because FR-010 adds real public surface: the `SkillSupport.Random` / `.Hud`
  shapes are sketched in `.fsi` and exercised in FSI before the `.fs` bodies
  (Phase 1 contracts + quickstart). The governance/skill/diagnostics work has no
  new API and is exercised by real gate runs and a real generated project.
- **Principle II (visibility in `.fsi`).** FR-010 adds curated `.fsi` for the new
  modules and a new per-package surface baseline (`FS.Skia.UI.SkillSupport.txt`);
  no `private`/`internal`/`public` modifiers in `.fs`. FR-012 keeps baseline and
  `.fsi` updated together.
- **Principle III (idiomatic simplicity).** The RNG uses `mutable` state only
  behind a pure `seed → (value, nextState)` threading API (disclosed at the use
  site); splitmix64/xorshift64 are plain integer arithmetic, no SRTP/reflection.
  The symbol-diff helper is set algebra over parsed token lists. No clever
  abstractions requiring justification.
- **Principle IV (Elmish/MVU boundary).** N/A for the framework — no
  interpreter/effects/host change. The seeded RNG is a *pure value-type utility
  for a consumer's Elmish core* (threaded through their pure `update`), not a host
  runtime addition; it deliberately avoids ambient `System.Random` so consumer
  `update` stays pure and replayable.
- **Principle V (synthetic disclosure).** No synthetic evidence planned — all
  evidence is real (real gate runs, a real generated project, real RNG/band/
  symbol-diff unit tests). If any task cannot reach real evidence it is marked
  `[S]` with full disclosure; none is anticipated.
- **Principle VI (test evidence).** Failing-first semantic tests for the new
  helpers (determinism, replay-equality, band clamp) and the symbol-diff; real
  gate runs for the governance/diagnostics work.
- **Principle VII (observability).** Directly *improved* — FR-004 (Dev
  self-describes), FR-005 (every evidence-format failure prints its full required
  shape), FR-007 (effective-DAG render + verdict context).

### Repository Governance Decisions

- **Template ownership**: **Update required.** New generated docs
  (`template/base/docs/scaffold-map.md` for FR-003, `docs/evidence-formats.md` for
  FR-005, `docs/skillist-reference.md` for FR-006), the promoted
  `template/feedback/extensions/feedback.yml` (FR-001), and the new
  `FS.Skia.UI.SkillSupport` consumer surface (FR-010, already pinned in
  `template/base/Directory.Packages.props`) all flow through
  `.template.config/template.json`. The new docs are added to the template content
  map; the generated, single-sourced docs are `copyOnly`-style verbatim where they
  must not take `sourceName` substitution. Skill edits are made in canonical
  `.agents/skills/**` and regenerated to `.claude/**`
  (`RefreshSurfaceBaselines`).
- **Dependency impact**: **N/A — no new third-party dependency.** FR-010 reuses
  the runtime/BCL only (integer arithmetic); SkillSupport's existing
  YamlDotNet/DiffPlex/FSharp.SystemTextJson pins are untouched.
  `Directory.Packages.props` and `docs/dependencies.md` need no new entries;
  `DependencyReport` coverage is unchanged.
- **Command-surface impact**: **Update required.** `Dev` self-describes (FR-004);
  `EvidenceAudit`/`EvidenceGraph` change for broader evidence-format schema
  printing (FR-005) and effective-DAG render (FR-007); new generation/currency
  checks back the generated `evidence-formats` (FR-005) and `skillist` (FR-006)
  references and the analyze symbol-diff (FR-008); a low-cost check pins the
  feedback hook as `optional: false` (FR-001). `TemplateCheck`/
  `GeneratedProductCheck`/`TemplateDrift` change for the new docs and shipped
  helpers; `SkillSyncCheck`/`TargetMetadataDrift`/`SkillQualityCheck` stay green
  after `.agents` edits; `PackageSurfaceCheck`/`PerPackageSurfaceDiff` cover the
  new SkillSupport surface. FAKE-backed commands share `.fake` state and run
  **sequentially** in deterministic order (Dev → GeneratedGuidanceCheck →
  TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit); safe
  non-FAKE reads may parallelize. The authoritative gate list is whatever
  `./fake.sh build -t Route` prints for the actual diff.
- **Generated project impact**: **Update required.** Generated projects gain the
  three new docs pages (FR-003/005/006), the promoted always-on feedback hook and
  precedence rule (FR-001/002), the self-describing `Dev` output (FR-004), the
  new `SkillSupport` helpers + skill reference (FR-010), and the analyze symbol
  cross-check (FR-008). No default scene/behavior or selected-Controls change; no
  excluded-history or placeholder-scan change beyond the new authored docs.
- **Evidence paths**: Real evidence under
  `specs/062-space-invaders-consumer-friction-followups/readiness/`:
  `target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`,
  `aggregate-hang-diagnostics.md` (Route-required escalated-tier artifacts);
  `readiness-recoverability.md` (FR-005 proof: a freshly generated project reaches
  passing `EvidenceAudit` for every format class with no `strings -el` and no
  sibling copy); RNG/band/symbol-diff unit-test output and the updated
  `readiness/surface-baselines/FS.Skia.UI.SkillSupport.txt` (FR-010);
  `synthetic-evidence.json` only if an `--accept-synthetic` override is ever
  needed (none anticipated).
- **`.fsi` / contract impact**: **Change required (FR-010 only).** New curated
  `.fsi` for `FS.Skia.UI.SkillSupport.Random` and `.Hud`, a new per-package
  surface baseline, and a skill reference. No other framework `.fsi` signatures
  change; SI-8/SI-9 are resolved by pitfalls/map content, **not** by renaming any
  framework or consumer DU case (explicitly out of scope).
- **MVU/effect boundary**: **N/A — no stateful/IO framework work.** No `Model`/
  `Msg`/`Effect`/`init`/`update`/interpreter is added or changed in the framework.
  The seeded RNG is documented as a pure helper a consumer threads through *their*
  `update`; it owns no state and performs no I/O.
- **Synthetic evidence**: **None planned.** No mocks/fakes/placeholders/canned
  responses. All four evidence-format classes are exercised against real readiness
  files in a real generated project; the helpers have real determinism/replay
  tests. `[S]` disclosure applies only if a real path proves infeasible
  mid-implementation (not anticipated); no `[SEH]` cases foreseen.
- **Test evidence**: Failing-first Expecto tests for RNG determinism + replay
  equality + `nextBelow` bounds, `reserveHudBand` clamp/remainder math, and the
  symbol-diff set algebra; governance tests for the feedback-hook `optional:
  false` assertion, the generated-doc currency checks, and the evidence-format
  schema-print diagnostics; real gate runs (`TemplateCheck`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`) and a real generated
  project for FR-001/004/005.
- **Observability**: FR-004 adds the `Dev`-does-not-compile / use-`Test`/`Verify`
  line to `Dev`'s own emitted output; FR-005 makes each failing evidence-format
  diagnostic print its complete per-file required shape (single-sourced from the
  same constants that enforce it, so it cannot drift); FR-007 prints the effective
  DAG (explicit + injected edges, distinctly labeled) and the resolved
  `skillist`-id set; the analyze symbol-diff (FR-008) reports set-differences as
  findings for human judgment, never a hard fail on intentionally design-only
  symbols.
- **Deferred scope**: The "generated game simulation core" skill (fixed-step
  accumulator, collision/reflection, paddle rebound) is **documented-only / shipped
  partially** this round — only the seeded RNG and `reserveHudBand` are shipped;
  the rest stay documented conventions with recorded rationale (D10/D11), to ship
  on a later recurrence. No new game/demo, no new runtime capability, platform,
  release, or distribution change. The symbol cross-check, effective-hooks notice,
  and task-graph references are delivered as diagnostics/guidance, not as new hard
  merge gates (except the low-cost feedback-hook and generated-doc currency
  checks).

## Project Structure

```
specs/062-space-invaders-consumer-friction-followups/
├── spec.md                # complete + clarified
├── plan.md                # this file
├── research.md            # D1–D12 design decisions
├── data-model.md          # entities (hook-decision, evidence-format schema, symbol set, RNG state, HUD band)
├── contracts/
│   ├── hook-precedence.md      # FR-001/002 precedence rule + effective-hooks notice contract
│   ├── evidence-format.md      # FR-005 per-class schema-print + generated reference contract
│   ├── skillsupport-api.md     # FR-010 Random + Hud .fsi contract + surface baseline
│   └── symbol-crosscheck.md    # FR-008 symbol-diff input/output contract
├── quickstart.md          # end-to-end verification incl. generated-project harness
└── readiness/             # real evidence artifacts (see Evidence paths)

# Source touched (authoritative tier/gates per Route on the actual diff):
template/feedback/extensions/feedback.yml          # FR-001 optional:true → optional:false (×6)
.agents/skills/speckit-{specify,clarify,plan,tasks,analyze,checklist,implement}/SKILL.md
                                                   # FR-001 precedence rule, FR-002 effective-hooks notice,
                                                   # FR-008 analyze detection pass G
.agents/skills/fs-skia-skiaviewer/ (or src/SkiaViewer/skill/SKILL.md)
                                                   # FR-009 Common pitfalls: Result.Ok/Error shadowing
src/SkillSupport/Random.fsi|.fs                    # FR-010 seedRng/nextRng/nextBelow
src/SkillSupport/Hud.fsi|.fs                       # FR-010 reserveHudBand
src/SkillSupport/SkillSupport.fsproj               # FR-010 add new Compile entries
readiness/surface-baselines/FS.Skia.UI.SkillSupport.txt   # FR-010 NEW baseline (Tier-1)
build/Governance/Engine/Update.fs                  # FR-004 Dev self-describing output
build/Governance/Evidence/Scans.fs                 # FR-005 window-visibility schema print
build/Governance/Evidence/Audit.fs                 # FR-005 skill-loading-evidence schema print
build/Governance/Evidence/TaskParser.fs            # FR-005 SEH token schema print
build/Governance/Evidence/Render.fs                # FR-007 effective-DAG render + skillist set
build/Governance/**                                # FR-005 evidence-formats gen, FR-006 skillist-reference gen,
                                                   # FR-008 symbol-diff helper, FR-001 feedback-hook check,
                                                   # Routing/contract + currency wiring
template/base/docs/scaffold-map.md                 # FR-003 durable-vs-replaceable map (NEW)
template/base/docs/evidence-formats.md             # FR-005 generated reference (NEW, single-sourced)
template/base/docs/skillist-reference.md           # FR-006 generated skillist/owns reference (NEW)
.template.config/template.json                     # FR-003/005/006 new docs in content map
```

**Workstream → change-set / Route boundary.** Implement in dependency order so
each change-set routes cleanly: (1) hook policy (template + skills) → escalates
template/skill gates; (2) self-describing diagnostics + renders (`build/**`) →
inner-loop + Evidence gates; (3) generated docs + currency checks; (4) symbol-diff
+ pitfalls; (5) **FR-010 helpers last** — the only Tier-1 change-set, pulling in
`PackageSurfaceCheck`/`PerPackageSurfaceDiff` and the new surface baseline.
`SkillSyncCheck`/`TargetMetadataDrift`/`SkillQualityCheck` must be re-run green
after every `.agents` edit + `RefreshSurfaceBaselines`.
