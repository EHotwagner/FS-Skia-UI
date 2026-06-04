# Feature Specification: Lunar-Lander Consumer Friction Follow-ups

**Feature Branch**: `063-lunar-lander-consumer-friction-followups`
**Created**: 2026-06-04
**Status**: Ready
**Input**: User description: "create specs from the feedback from the feedback hook in the sibling repo lunarlander1 in specs/001.../feedback..."

## Context & Triage *(informative)*

A consumer implemented the **Lunar Lander** physics game in a generated
`FS.Skia.UI` project (`LunarLander1`) on the **062-merged** packages (libs
`0.1.66-preview.1`, template `0.1.85`; verified from
`LunarLander1/Directory.Packages.props`). The per-phase feedback hook captured
**four** records under `LunarLander1/specs/001-lunar-lander/feedback/`
(`specify-`, `plan-`, `analyze-`, `implement-2026-06-04.md`; no `tasks`/`clarify`
record this run). Because the project was generated **post-062**, every finding
below is against the **current merged state**, and 062's own deliverables are
confirmed present and exercised in the consumer: the scaffold model-swap map
(`docs/scaffold-map.md`) and the evidence-format reference (`docs/evidence-formats.md`)
both shipped into the generated project, the shipped `SkillSupport.Random`/`.Hud`
helpers and the analyze symbol pass were available, and the feedback hook
auto-fired every phase (the FR-001 promotion landed). These are the *next* layer
of friction.

This round differs from 060/061/062 in one important way: it surfaces the
**first genuine framework rendering defect** of the consumer-friction series
(LL-1 below), not only governance/docs/discoverability friction. Each finding is
triaged against `060-asteroids-`, `061-breakout-`, and
`062-space-invaders-consumer-friction-followups` (the three most recently merged
features) and against current source, since several findings are the **residual**
of a 060/061/062 fix rather than wholly new.

| # | Sev | Finding | Status vs. 060 / 061 / 062 / current source |
|---|-----|---------|----------------------------------------------|
| LL-1 | **high** (framework defect) | The **image-evidence / screenshot renderer draws only a subset of `SceneNode` primitives.** `drawScreenshotScene` (`src/SkiaViewer/SkiaViewer.fs:1771-1808`, the `--image-evidence` / `writeSceneImageEvidence` / `runAppEvidence` path) handles only `Rectangle` / `PaintedRectangle` / `Circle` / `FilledEllipse` / container nodes, draws `Text` as a **placeholder rectangle** (not glyphs), and routes **everything else — `Line`, `Path`, `Ellipse`, `Points`, `Vertices`, `Arc`, `TextRun`, `Image`, `RegionNode`, `Chart` — to a wildcard that draws a single 40×40 teal placeholder rect.** The consumer's terrain (73 `Line` nodes) and filled ground (`Path`) rendered as **nothing**; only the `filledRectangle` pad showed. Yet they appear in `Scene.describe` and pass node-count assertions, so the scene read as "visible." The interactive Vulkan renderer (`src/SkiaViewer/Host/Vulkan.fs:1005-1160`, `drawScene`) handles **all** cases — the screenshot path is a **stunted parallel renderer** that silently diverged. | **Open & NEW — confirmed in current source.** No prior feature touched the image-evidence renderer's primitive coverage. This is the first hard framework defect in the 060/061/062 series: a real loss of visual fidelity *and* a false-confidence trap (node-count tests pass on invisible scenes). Verified: `drawScreenshotScene` wildcard at `SkiaViewer.fs:1804-1806` vs full coverage at `Vulkan.fs:1040-1048` (Line/Path). |
| LL-2 | minor | **`SymbolCrossCheck` has no invocation path.** 062 FR-008 added the compiled `SymbolCrossCheck` (`build/Governance/SymbolCrossCheck.fs`) and the analyze "pass G" instruction says to run it and "do not eyeball it" — but it is a **library type with no thin CLI/FAKE entry point**, so a read-only analyze pass cannot invoke it without authoring a throwaway harness. The consumer fell back to a manual set-difference and disclosed it. Wants `./fake.sh build -t SymbolCrossCheck <files...>` (or a `--symbol-crosscheck` evidence command) that prints the documented `## Symbol consistency (analyze pass G)` markdown. | **Residual of 062 FR-008 — confirmed.** `SymbolCrossCheck` is **not** among the ~45 declared FAKE targets in `build/Governance/Targets.fs`, nor referenced by `Routing.fs`. 062 shipped the analyzer but not its deterministic command surface; the gap is a missing *target*, not a missing skill. |
| LL-3 | minor | **Evidence-path token drift in generated artifacts.** At analyze time the spec referenced `specs/001-lunar-lander/evidence/` while plan/tasks/contracts standardized on `readiness/`; the drift was caught only by cross-reading (consumer finding F1). The consumer reconciled it (the merged spec uses `readiness/`), but a **single canonical evidence-path token** referenced by all generated artifacts would remove the whole class. | **Open & NEW — consumer-self-resolved.** Likely a consumer-authoring slip rather than a seeded template token; planning must confirm whether any generated spec/plan/tasks template or readiness-placeholder guidance seeds a divergent `evidence/` vs `readiness/` token, and if so unify it. Lowest-confidence finding. |
| LL-4 | medium | **Readiness/evidence-format discoverability + missing-vs-required diagnostic clarity.** 062 FR-005 shipped `docs/evidence-formats.md` (it *did* ship into LunarLander), but the consumer authored readiness files first and hit **three audit round-trips** before finding it — it is not surfaced *before* writing. Separately, the per-file diagnostic prints the **full required token set** (the `Required` field) which "initially read as 'all missing' when only one token was absent," even though the serialized `missing` array is only the absent subset. | **Residual of 062 FR-005 — discoverability + UX.** The reference exists and is correct; the gaps are (a) pointing implement at it *before* authoring, and (b) the printed output not visually distinguishing the **absent subset** from the **full required set**. Confirmed: `missing` = absent subset (`Scans.fs:104-106`), `Required = Some terms` = full set (`Scans.fs:101`). |
| LL-5 | medium | **`skill-loading-evidence.md` location + late surfacing.** The engine reads it from the **feature** readiness dir (`specs/<feature>/readiness/`, not repo-root `readiness/`) and requires one row per (task, declared-skill) with `.agents/skills/<id>/SKILL.md` paths and `loaded_at < work_started_at`; the graph only enforces this **once tasks flip to `[X]`**, so it surfaces late in implementation. Worth calling out in the `speckit-implement` skill body. | **Open & NEW.** Confirmed: read at `Governance.fs:584` from `featureReadiness` (`Model.fs:82-88`); enforced only for `Done`/`Synthetic` tasks (`Audit.fs:307-315`). 062 touched evidence-format *schemas* but not the implement-skill guidance on *when/where* this file is read. |
| LL-6 | minor | **External source-spec provenance.** The Lunar Lander source spec was supplied as an external GitHub URL (`docs/testSpecs/lunar-lander.md`) and fetched via web before authoring — works, but adds a network dependency, is not reproducible offline, and leaves the source of truth outside the repo. Wants a local snapshot under the feature directory (or a documented fetch step / skill) when the `/speckit-specify` input is a URL. | **Open & NEW.** A generalizable specify-phase process improvement; no prior feature addressed URL-sourced specs. |
| LL-7 | minor | **`scaffold-map.md` discoverability + `.fsi`-authoritative pointer.** 062 FR-003's `docs/scaffold-map.md` shipped into LunarLander and *does* cover durable-vs-replaceable files, the `GovernanceTests`-durable / `BehaviorTests`-replaceable split, must-survive scan strings, and the evidence CLI vocabulary — but the consumer **reconstructed the map by hand** without finding it, reading all six `src/*.fs`, both test files, `build.fsx`, and `EvidenceCommands.fs`. Separately, the Explore-agent framework-API summary mixed confirmed APIs with **inferred** type shapes, so the shipped `.fsi` / `docs/api-surface` had to serve as ground truth. | **Residual of 062 FR-003 — discoverability.** The content exists and is sufficient; the gap is that nothing in the plan-phase flow *points* the author at `scaffold-map.md` first, and the map lacks an explicit "shipped `.fsi`/`docs/api-surface` is authoritative; Explore summaries are not" pointer. |
| LL-8 | n/a (generalizable) | **Deterministic `--evidence-run` summary pattern recurs (4th demo).** The pure-model + per-frame held-input script + `InvariantCulture`/`F3` fixed-precision summary pattern was re-derived again. 062 FR-011 **deferred** the standalone "generated game simulation core" skill with rationale "ship on recurrence; not yet at the 3-demo bar for the loop primitives." Lunar Lander is the next recurrence. | **Escalates 062-deferred D11.** The seeded RNG and `reserveHudBand` already shipped (062 FR-010); the *loop/summary* primitives stayed documented-only. This round is the recurrence trigger 062 named — disposition required (ship a helper/skill or re-defer with a fresh rationale). |
| LL-9 | n/a (generalizable) | **New helper candidates: `wrapDeltaX` + camera-centered projection.** Shortest wrap-aware delta on a toroidal axis, and a camera-centered world→screen projection, were identified as generic for any scrolling/toroidal game. | **Open & NEW candidates.** Per-helper triage (ship / document / defer); toroidal-wrap recurs across Asteroids/SpaceInvaders too, so it may meet the recurrence bar, while the camera projection is first-seen. |

**Feedback specifics this round.** The `tasks` and `clarify` records are absent
(the consumer did not capture them this run), so there is no fourth-prompt
skill-gap *harvest* table as in 062; the skill-gap signal instead arrives inline
in the `plan` and `analyze` records (the "deterministic fixed-step simulation +
evidence summary" and "scaffold model-swap contract" candidates — both already
addressed by 062's deferred D11 and shipped FR-003 respectively, leaving only the
LL-8 recurrence and the LL-7 discoverability residual).

**Scope note.** Per the house pattern (one consolidated "consumer friction
follow-ups" feature per demo — 060/061/062/034/022 — and the single-feature rule),
this is **one** feature consolidating all LunarLander1 feedback, not one spec per
friction item. No new USER deliverable was requested this round.

**Change classification.** **Tier 1 (consumer-contract / framework-behavior
change)** is expected this round because **LL-1 changes real rendering behavior**
in `src/SkiaViewer/**` (image-evidence visual output) and LL-2 adds a new FAKE
target (governance/Route surface). Documentation/skill/template content
(LL-4/5/6/7) is Tier-2 by itself, but the renderer fix and new target escalate the
overall change. If LL-8/LL-9 ship helpers, the new `FS.Skia.UI.SkillSupport`
`.fsi` surface and its per-package baseline are pulled in as well. The
authoritative tier and gate list is whatever `./fake.sh build -t Route` prints for
the actual diff.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Image evidence faithfully renders every scene primitive (Priority: P1)

A consumer capturing image/screenshot evidence of a scene built from `Line`,
`Path`, `Arc`, real `Text`, and the other `SceneNode` primitives sees those
primitives **drawn to actual pixels** in the evidence image — the same primitives
the interactive renderer draws — instead of a single placeholder rectangle. When a
primitive is genuinely not render-backed in evidence mode, that fact is **made
explicit** (the renderer does not silently swallow it to a placeholder) and
node-count / `Scene.describe` assertions can no longer give false "the scene is
visible" confidence. The set of render-backed primitives is documented in the
`fs-skia-scene` skill.

**Independent test**: Build a scene containing `Line` and `Path` nodes (e.g. a
polyline terrain and a filled ground), run the image-evidence path, and confirm
the resulting image contains the corresponding pixels (not a 40×40 placeholder).
Separately, confirm a scene whose only content is an unrendered-in-evidence
primitive does **not** pass a "scene is visible" assertion by node count alone.

### User Story 2 - Analyze can invoke the symbol cross-check deterministically (Priority: P1)

An author (or a read-only `/speckit-analyze` pass) can run the compiled
`SymbolCrossCheck` through a **deterministic command surface** — `./fake.sh build
-t SymbolCrossCheck <files...>` (or a `--symbol-crosscheck` evidence command) —
that prints the documented `## Symbol consistency (analyze pass G)` markdown, so
pass G consumes real compiled output instead of a hand-derived set-difference and a
disclosed fallback.

**Independent test**: From a read-only checkout, invoke the new target/command over
`plan.md`/`data-model.md`/`tasks.md` and confirm it prints the proper-subset
findings in the documented format without requiring a throwaway harness.

### User Story 3 - Evidence formats are discoverable before authoring, and diagnostics read clearly (Priority: P2)

A consumer about to write readiness/evidence files is pointed at the in-repo
evidence-format reference (`docs/evidence-formats.md`) **before** authoring — from
the `speckit-implement` skill body — so the shapes are learned up front instead of
through audit round-trips. When a file is partially correct, the failing
diagnostic distinguishes the **absent token(s)** from the **full required set**, so
one missing token no longer reads as "all missing." The same skill body notes that
`skill-loading-evidence.md` is read from `specs/<feature>/readiness/` (not repo
root) and is enforced only once tasks flip to `[X]`.

**Independent test**: Trigger a readiness-contract failure with exactly one absent
token and confirm the diagnostic visibly separates the absent token from the full
required set; confirm the `speckit-implement` skill body names the
`docs/evidence-formats.md` reference and the `skill-loading-evidence.md`
location/timing.

### User Story 4 - Scaffold map and source spec are reproducible and discoverable (Priority: P2)

A consumer planning a scaffold-model swap is pointed at `docs/scaffold-map.md`
(which already carries the durable-vs-replaceable map) from the plan-phase flow
before reverse-engineering it, and the map states that the shipped `.fsi` /
`docs/api-surface` is the authoritative API reference (agent-generated API
summaries are not). Separately, when `/speckit-specify` is given an **external URL**
as the source spec, the source is snapshotted into the feature directory (or a
documented fetch step is recorded) so the specify phase is reproducible offline and
provenance lives in-repo.

**Independent test**: Confirm the plan flow references `docs/scaffold-map.md` and
that the map carries the `.fsi`-is-authoritative pointer; confirm that specifying
from a URL produces an in-repo snapshot (or a documented, reproducible fetch step)
under the feature directory.

### User Story 5 - Recurring game helpers are dispositioned, not silently re-deferred (Priority: P3)

A consumer building any physics/scrolling game can reach the recurring helpers
they keep re-deriving — the deterministic `--evidence-run` summary pattern and
`wrapDeltaX` / camera-centered projection — either as shipped
`FS.Skia.UI.SkillSupport` API (with skill references) or as an explicitly recorded
ship-vs-document-vs-defer decision per helper, so nothing is silently dropped or
re-deferred without rationale.

**Independent test**: Confirm each of the LL-8 summary-pattern and LL-9
`wrapDeltaX`/camera candidates has a recorded disposition (shipped-with-reference,
documented-as-convention, or deferred-with-rationale), and that anything shipped
has updated surface baselines and a skill reference.

### Edge Cases

- A scene mixes render-backed and not-yet-render-backed primitives in evidence
  mode → the image must draw the render-backed ones faithfully **and** signal the
  unrendered ones rather than substituting a single placeholder that hides both
  the content and the gap.
- `Text` in evidence mode: the current renderer draws a placeholder box, not
  glyphs → the success criterion must state whether evidence-mode `Text` becomes
  real glyphs or is explicitly classified as a known non-glyph placeholder (so
  visual-proof honesty vocabulary still applies).
- The `SymbolCrossCheck` target is invoked with files that have **no** symbol drift
  → it must print a well-formed empty `## Symbol consistency` section (parity with
  the "no findings" path), not nothing.
- A readiness file is partially correct (right name, one missing token) → the
  diagnostic must make the single absent token recoverable without reading the full
  required set as "all missing," for every format class.
- `/speckit-specify` input is a **local** file or inline text (not a URL) → the
  snapshot step is a no-op and must not fabricate a redundant copy.
- A generalizable helper candidate (LL-8/LL-9) is deferred → the deferral records a
  family/topic so the candidate is findable on the next recurrence, not lost.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The **image-evidence / screenshot renderer MUST render the
  `SceneNode` primitives that the interactive renderer draws** — at minimum `Line`
  and `Path` (the consumer's terrain and ground), and extending toward parity with
  `src/SkiaViewer/Host/Vulkan.fs`'s `drawScene` for the documented render-backed
  primitives (`Ellipse`, `Arc`, `Points`, `Vertices`, `Image`, `RegionNode`, and
  real `Text` glyphs where feasible). The silent catch-all wildcard in
  `drawScreenshotScene` (`src/SkiaViewer/SkiaViewer.fs:1804-1806`) that substitutes
  a single 40×40 placeholder rectangle MUST be eliminated for the primitives the
  framework claims are visible. (Closes LL-1; primary framework fix.)
- **FR-002**: The framework MUST ensure **node-count / `Scene.describe` assertions
  no longer give false "scene is visible" confidence** for primitives that are not
  render-backed in evidence mode: any primitive the evidence renderer does **not**
  draw to pixels MUST be made explicit (classified/flagged, e.g. via the existing
  visual-evidence honesty vocabulary or a diagnostic), not silently mapped to a
  placeholder; and the set of render-backed primitives MUST be **documented in the
  `fs-skia-scene` skill** so node-count tests are understood as structural, not
  visual, proof. Per the plan's exhaustive shared painter (D1/D3), the residual
  unrendered set is **empty for every modeled `SceneNode` primitive**, so this
  honesty guarantee is delivered **structurally** (the placeholder is deleted, the
  match is exhaustive) rather than as a runtime "unrendered" flag; the
  flag-don't-hide rule governs only a hypothetical future primitive the framework
  models but cannot yet paint. (Closes LL-1's false-confidence half; pairs with FR-001.)
- **FR-003**: `SymbolCrossCheck` MUST gain a **deterministic command surface** — a
  `./fake.sh build -t SymbolCrossCheck <files...>` FAKE target and/or a
  `--symbol-crosscheck` evidence command — that runs the existing compiled
  `build/Governance/SymbolCrossCheck.fs` analyzer and prints the documented
  `## Symbol consistency (analyze pass G)` markdown, so a read-only `/speckit-analyze`
  pass can invoke it without a throwaway harness. The new target MUST be registered
  in `build/Governance/Targets.fs` (and `Routing.fs` / `validation.contract.yml` as
  required) so it is a real, discoverable gate/command. (Closes LL-2; completes 062
  FR-008.)
- **FR-004**: The `speckit-implement` skill body MUST **point authors at
  `docs/evidence-formats.md` before writing readiness/evidence files**, and the
  per-file evidence diagnostic MUST **visually distinguish the absent token(s) from
  the full required set** so one missing token no longer reads as "all missing"
  (the `missing` subset and the `Required` full set must be separately labelled in
  the printed output). (Closes LL-4; extends 062 FR-005.)
- **FR-005**: The `speckit-implement` skill body MUST document that
  `skill-loading-evidence.md` is read from the **feature** readiness dir
  (`specs/<feature>/readiness/`, not repo-root `readiness/`), requires one row per
  (task, declared-skill) with `.agents/skills/<id>/SKILL.md` paths and
  `loaded_at < work_started_at`, and is **enforced only once tasks flip to `[X]`**
  (so it surfaces late) — so consumers prepare it up front. (Closes LL-5.)
- **FR-006**: The plan-phase flow MUST **point authors at `docs/scaffold-map.md`**
  (the durable-vs-replaceable map that already ships) before they reconstruct it by
  hand, and `scaffold-map.md` MUST carry an explicit pointer that the shipped
  `.fsi` surfaces / `docs/api-surface` are the **authoritative** API reference and
  agent-generated API summaries are not ground truth. (Closes LL-7; folds the
  `.fsi`-authoritative half of LL-7.)
- **FR-007**: When `/speckit-specify` is given an **external URL** as the source
  spec, the specify flow MUST **snapshot the fetched source into the feature
  directory** (or record a documented, reproducible fetch step) so provenance lives
  in-repo and the phase is reproducible offline; for local-file or inline input the
  step is a no-op. (Closes LL-6.)
- **FR-008**: Planning MUST **confirm whether any generated artifact template**
  (spec/plan/tasks template or the readiness-placeholder guidance) seeds a divergent
  `evidence/` vs `readiness/` evidence-path token, and if so **unify on a single
  canonical token** referenced by all generated artifacts; if the drift is purely
  consumer-authoring (no template seeds it), record that finding and close without a
  code change. (Closes LL-3.)
- **FR-009**: The recurring **deterministic `--evidence-run` summary pattern**
  (LL-8) MUST be dispositioned now that it has recurred on a fourth demo —
  **shipped** as a `FS.Skia.UI.SkillSupport` helper and/or a created skill, folded
  into an existing skill (`fs-skia-elmish` / `fs-skia-layout-readability`), or
  **re-deferred with a fresh, recorded rationale** that names the next recurrence
  bar. (Closes LL-8; escalates 062-deferred D11.)
- **FR-010**: The new helper candidates **`wrapDeltaX`** (shortest wrap-aware delta
  on a toroidal axis) and a **camera-centered projection** (LL-9) MUST each be
  triaged per-helper with the ship-vs-document-vs-defer decision recorded; anything
  shipped lands in `FS.Skia.UI.SkillSupport` with a skill reference and updated
  surface baseline. (Closes LL-9.)
- **FR-011**: Because the `.agents` skill tree is canonical and `.claude` is
  generated, all skill edits (FR-002/004/005/006) MUST be made in
  `.agents/skills/**` and regenerated (`RefreshSurfaceBaselines`), keeping
  `SkillSyncCheck` / `TargetMetadataDrift` / `SkillQualityCheck` green; the new
  `SymbolCrossCheck` target (FR-003) MUST be added to the `knownGates` allowlist and
  `validation.contract.yml` regenerated so `TargetMetadataDrift` stays green; and if
  FR-009/FR-010 ship helpers, the new `FS.Skia.UI.SkillSupport` `.fsi` surface and
  its per-package surface baseline MUST be updated together (Tier-1 escalation for
  those helpers).

> Interacting / conflicting requirements: FR-001 (draw the primitives) and FR-002
> (honestly flag any primitive still not drawn) pull together, not apart —
> resolution: FR-001 maximizes faithful pixels; FR-002 governs the *residual* set
> the evidence renderer still cannot draw, which must be flagged, never
> placeholder-substituted. For evidence-mode `Text`, FR-001 *prefers* real glyphs
> but FR-002 *permits* an explicitly-classified non-glyph placeholder if real text
> rasterization is out of scope — planning picks one and SC-001/SC-002 check the
> outcome (faithful-or-flagged, never silently-hidden). FR-009/FR-010 lean toward
> shipping on recurrence but planning may keep a helper documented/deferred with a
> recorded rationale — SC-006 checks the per-helper decision is recorded and the
> outcome holds.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package *identities* change. Package *contents* change:
  `FS.Skia.UI.SkiaViewer` rendering behavior changes (FR-001/002), `FS.Skia.UI.Build`
  gains the `SymbolCrossCheck` target/command (FR-003), and `FS.Skia.UI.SkillSupport`
  contents change *if* FR-009/FR-010 ship helpers. Generated-project docs/skills
  change (FR-002/004/005/006). The template package version is bumped/packed/installed
  so generated projects inherit the skill-body pointers, the `scaffold-map.md`
  pointer, and any canonical evidence-path token. Generated package **consumers**
  change accordingly.
- **Public contract impact**: `FS.Skia.UI.SkiaViewer` rendering output changes but
  its `.fsi` *signatures* should not (FR-001/002 are internal renderer behavior).
  New public `.fsi` surface is added **only if** FR-003 exposes a public command
  API and/or FR-009/FR-010 ship `SkillSupport` helpers (per-package surface-baseline
  update). Consumer-facing skill/template/docs content changes
  (FR-002/004/005/006).
- **State workflow impact**: No interpreter/effects/command behavior change. The
  `--evidence-run` summary helper (FR-009) and `wrapDeltaX`/camera projection
  (FR-010), if shipped, are pure value-type utilities for the Elmish core, not host
  runtime changes.
- **Layout/rendering impact**: **Yes — this is the primary change.** The
  image-evidence renderer gains primitive coverage (FR-001) and honest classification
  of any residual unrendered primitives (FR-002); this is real visual-output change
  in `src/SkiaViewer/**` and must be proven with regenerated image/screenshot
  evidence. No interactive-renderer (`Vulkan.fs`) change is required — it already has
  full coverage.
- **Evidence obligations**: Real evidence under
  `specs/063-lunar-lander-consumer-friction-followups/readiness/` — at minimum the
  Route-required escalated-tier artifacts (target-metadata, agent-ready verdict,
  skill-loading-evidence, aggregate-hang-diagnostics), plus, for FR-001/002, a
  before/after **image-evidence proof** that `Line`/`Path` (and the targeted
  primitives) now render to pixels and that node-count assertions no longer pass on
  an invisible scene, and, if FR-009/FR-010 ship helpers, their unit tests and
  surface baselines.
- **Unsupported scope**: No new game/demo is shipped; no new framework runtime
  capability, platform, release, or distribution change. Renaming framework or
  consumer DU cases is out of scope. The evidence-format diagnostic clarity (FR-004),
  skill-body docs (FR-005), scaffold-map pointer (FR-006), and source-spec snapshot
  (FR-007) are delivered as diagnostics/guidance/process, not as new hard merge
  gates, unless planning finds a low-cost executable check. Full interactive-renderer
  parity beyond the documented render-backed primitives is out of scope for the
  evidence path if planning classifies a primitive as evidence-unsupported (FR-002
  covers honest flagging instead).
- **Build-target impact**: A new `SymbolCrossCheck` target is added to
  `build/Governance/Targets.fs` (FR-003), with `knownGates` / `Routing.fs` /
  `validation.contract.yml` updated (FR-011). `EvidenceAudit` / image-evidence
  capture changes for the renderer fix (FR-001/002). `TemplateCheck` /
  `GeneratedProductCheck` / `TemplateDrift` likely change for the skill/docs updates
  and any canonical evidence-path token. `SkillSyncCheck` / `TargetMetadataDrift` /
  `SkillQualityCheck` must stay green after `.agents` edits (FR-011). The
  authoritative gate list is whatever `./fake.sh build -t Route` prints.

## Success Criteria *(mandatory)*

- **SC-001**: An image-evidence capture of a scene containing `Line` and `Path`
  nodes (and the other targeted primitives) shows those primitives **as actual
  pixels** in the output image — verified by a before/after capture where the
  before image shows only the placeholder/pad and the after image shows the
  terrain/ground. (LL-1)
- **SC-002**: No scene whose visible content is a primitive the evidence renderer
  does **not** draw can pass a "scene is visible" check by node count alone; any
  such primitive is explicitly classified/flagged (honesty vocabulary or
  diagnostic), and the `fs-skia-scene` skill documents the render-backed primitive
  set. (LL-1 false-confidence half)
- **SC-003**: `./fake.sh build -t SymbolCrossCheck <files...>` (and/or the
  `--symbol-crosscheck` command) runs the compiled analyzer from a read-only
  checkout and prints the documented `## Symbol consistency (analyze pass G)`
  markdown — verified by seeding a symbol drift and confirming the proper-subset
  finding is reported without a throwaway harness. (LL-2)
- **SC-004**: A readiness-contract failure with exactly one absent token produces a
  diagnostic that visibly separates the absent token from the full required set
  (not "all missing"); the `speckit-implement` skill body names
  `docs/evidence-formats.md` as a before-authoring reference and documents the
  `skill-loading-evidence.md` feature-dir location and `[X]`-gated enforcement
  timing. (LL-4, LL-5)
- **SC-005**: The plan flow references `docs/scaffold-map.md` and the map carries
  the `.fsi`/`docs/api-surface`-is-authoritative pointer; specifying from an
  external URL yields an in-repo source-spec snapshot (or documented reproducible
  fetch step) under the feature directory; and the evidence-path token question
  (LL-3) is resolved (unified token *or* recorded as consumer-authoring-only).
  (LL-7, LL-6, LL-3)
- **SC-006**: The `--evidence-run` summary pattern (LL-8) and the
  `wrapDeltaX`/camera-projection candidates (LL-9) are each shipped in
  `FS.Skia.UI.SkillSupport` with a skill reference and surface baseline, or recorded
  as documented-as-convention / deferred-with-rationale (naming the next recurrence
  bar) — no candidate silently dropped. (LL-8, LL-9)
- **SC-007**: All Route-printed gates for this change pass — including the new
  `SymbolCrossCheck` target wired into `knownGates` / `validation.contract.yml`,
  `SkillSyncCheck` / `TargetMetadataDrift` / `SkillQualityCheck` after `.agents`
  edits are regenerated, the per-package surface baseline if FR-009/FR-010 ship
  helpers, and the regenerated image-evidence proofs — and `EvidenceAudit` returns
  `verdict=PASS` for `specs/063-lunar-lander-consumer-friction-followups`.

## Assumptions

- LunarLander1 was generated from the **062-merged** packages (template `0.1.85`,
  libs `0.1.66-preview.1`; confirmed from its `Directory.Packages.props`), so all
  findings are against the current merged state; this feature does not re-merge or
  re-verify 060/061/062 deliverables. The consumer exercised 062's shipped
  `scaffold-map.md`, `evidence-formats.md`, `SkillSupport.Random`/`.Hud`, and the
  auto-firing feedback hook.
- The LL-1 renderer gap is a genuine **framework defect**, not a consumer error:
  the image-evidence path (`drawScreenshotScene`, `SkiaViewer.fs:1771-1808`) and the
  interactive path (`drawScene`, `Vulkan.fs:1005-1160`) are two separate renderers
  and the screenshot one omits `Line`/`Path`/etc.; the fix targets the screenshot
  renderer only (the interactive renderer already has full coverage).
- "Faithful or flagged, never silently hidden" (FR-001/002) is the resolution for
  the renderer: planning decides which residual primitives (notably evidence-mode
  `Text` glyphs) are brought to pixels vs explicitly classified as
  evidence-unsupported; SC-001/SC-002 check the outcome, not the mechanism.
- FR-003 is a **completion of 062 FR-008**, not a new analyzer: the
  `SymbolCrossCheck` logic already exists in `build/Governance/SymbolCrossCheck.fs`;
  only the FAKE target / command surface and its governance wiring are added.
- FR-004/005/006/007 are guidance/diagnostics/process changes, not new hard merge
  gates, unless planning finds a low-cost executable check. The evidence-format
  reference and scaffold map already exist and ship; the residual is
  discoverability + diagnostic clarity, not new content.
- LL-3 (evidence-path token) is the lowest-confidence finding (consumer
  self-reconciled to `readiness/`); FR-008 may close it as "consumer-authoring-only,
  no template seeds the divergent token" if planning confirms no generated template
  is at fault.
- The seeded RNG and `reserveHudBand` already shipped in 062 FR-010, so this round's
  generalizable work (FR-009/FR-010) is the *remaining* helpers (summary pattern,
  `wrapDeltaX`, camera projection), not a re-ship of what 062 delivered.
- "One feature, not one-per-item" is the correct reading of "create specs" given the
  consolidated consumer-friction-followups house pattern (060/061/062/034/022) and
  the one-feature-per-`/speckit-specify` rule.
- No new USER deliverable was requested this round; the LunarLander1 records were
  produced by the per-phase feedback hook that 062 already promoted to mandatory.
