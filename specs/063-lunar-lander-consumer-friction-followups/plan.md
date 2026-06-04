# Implementation Plan: Lunar-Lander Consumer Friction Follow-ups

**Branch**: `063-lunar-lander-consumer-friction-followups` | **Date**: 2026-06-04 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/063-lunar-lander-consumer-friction-followups/spec.md`

## Summary

Consolidate the `LunarLander1` consumer-friction feedback (LL-1…LL-9) into one
feature against the current merged state (post-062, `c7d4124`; template `0.1.85`,
libs `0.1.66-preview.1` — the packages LunarLander1 was generated from). Eleven FRs
group into four workstreams:

1. **Renderer parity — the headline framework fix (FR-001/002).** The image-evidence
   renderer `drawScreenshotScene` drew only a few primitives and routed
   `Line`/`Path`/`Arc`/… to a placeholder-rect wildcard, while the interactive
   `Vulkan.drawScene` handled all cases — two divergent renderers. Fix at the root:
   extract **one shared `SceneRenderer.paintNode`** (a non-public module) that both
   paths delegate to, with an **exhaustive match (no wildcard)** so the compiler
   guarantees they can never diverge again; render `Text` as **real glyphs**. The
   placeholder substitution that produced false "scene is visible" confidence is
   deleted, so node-count assertions can no longer pass on an invisible scene; the
   unified renderer is documented in `fs-skia-scene`. (Closes LL-1.)

2. **Complete the symbol cross-check (FR-003).** 062 shipped the `SymbolCrossCheck`
   analyzer but no invocation path. Add a **`SymbolCrossCheck` FAKE target** that
   derives `plan.md`/`data-model.md`/`tasks.md` from the feature dir (the
   `DependencyReport` pattern), runs the existing `diff`+`render` (which already emit
   the documented `## Symbol consistency (analyze pass G)` markdown), prints and
   writes `readiness/symbol-cross-check.md`, and is wired into
   `knownGates`/`validation.contract.yml`. Delivered as a command, not a hard gate.
   (Closes LL-2.)

3. **Authoring discoverability + diagnostics (FR-004/005/006/007/008).** Point
   `speckit-implement` at `docs/evidence-formats.md` before authoring and document the
   `skill-loading-evidence.md` feature-dir location + `[X]`-gated timing; relabel the
   readiness diagnostic so the full required set and the absent subset read
   distinctly; point `speckit-plan` at `docs/scaffold-map.md` and add an
   `.fsi`-is-authoritative note to the map; snapshot external-URL source specs into
   the feature dir in `speckit-specify`; and **close the evidence-path-token finding
   with no code change** (confirmed no template seeds a divergent `evidence/` token).
   (Closes LL-3/4/5/6/7.)

4. **Disposition the recurring helpers (FR-009/010/011).** **Ship `wrapDeltaX`** into
   `FS.Skia.UI.SkillSupport` as a new `Wrap` module (pure, float-only, past the
   3-demo recurrence bar) — the lone Tier-1 escalation (new `.fsi` + surface
   baseline). **Document** the camera-centered projection (game-specific) and the
   `--evidence-run` summary discipline (field shapes vary per game), each with a
   recorded deferral rationale and next-recurrence bar, so nothing is silently
   dropped. (Closes LL-8/9.)

All deferred design decisions are resolved in [research.md](./research.md) (D1–D11);
entities in [data-model.md](./data-model.md); interface contracts in
[contracts/](./contracts/); verification in [quickstart.md](./quickstart.md).

**Change classification: Tier 1.** Two triggers: FR-010 adds public
`FS.Skia.UI.SkillSupport.Wrap` `.fsi` surface + a per-package baseline line, and
FR-003 adds a new governance target/`knownGates` entry. The renderer fix
(FR-001/002) is internal behavior (the shared painter is **non-public** — no
SkiaViewer surface change) but is consumer-observable evidence output, so it ships
with regenerated image evidence. **Route is authoritative — re-run after each
change-set;** the FR-010 change-set pulls in `PackageSurfaceCheck`/
`PerPackageSurfaceDiff`, the FR-003 change-set pulls in `TargetMetadataDrift`.

## Technical Context

**Language/Version**: F# / .NET (`net10.0`). Touches `src/SkiaViewer/**` (new shared
renderer + delegation), `src/SkillSupport/**` (new `Wrap` module),
`build/Governance/**` (new `SymbolCrossCheck` target + readiness-diagnostic
relabel), Spec Kit phase skills (`.agents/skills/**` → generated `.claude/**`), and
generated docs (`template/base/docs/scaffold-map.md`).
**Primary Dependencies**: none new. The shared painter reuses SkiaSharp APIs already
used by both renderers (raster `SKCanvas` supports `DrawLine`/`DrawPath`/`DrawArc`/
`DrawText`); `Wrap` is float-only (no `Scene`/`Layout` dependency, keeping
SkillSupport dependency-light); `SymbolCrossCheck` already exists.
**Testing**: Expecto (renderer golden/pixel assertions, `Wrap` determinism/range,
symbol-diff target output); FAKE targets per Route; FSI exercising the packed `Wrap`
surface; a before/after image capture for FR-001. The feedback extension is **not**
installed in this repo, so the LunarLander1 records were produced in the consumer;
this feature verifies the framework fixes locally + in a generated project.
**Target Platform**: Windows and Linux. The renderer fix draws onto a raster
`SKBitmap` canvas (no GPU/window-system dependency for evidence); `Wrap` is a pure
value-type utility.
**Routing baseline**: spec/plan-only diff routes light; this **escalates** as
change-sets land — `src/SkiaViewer/**` (renderer) pulls inner-loop + evidence gates,
`build/Governance/**` (new target) pulls `TargetMetadataDrift`/`Test`/`Evidence*`,
`.agents/**` edits pull `SkillSyncCheck`/`SkillQualityCheck`/`TemplateCheck`/
`GeneratedProductCheck`, and the FR-010 helper pulls
`PackageSurfaceCheck`/`PerPackageSurfaceDiff`. **Run `./fake.sh build -t Route`
against the actual diff for the authoritative list.** No `NEEDS CLARIFICATION`
remains.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Re-checked post-design: **PASS.**

- **Principle I (Spec → FSI → tests → impl).** Applies non-degenerately via FR-010:
  `SkillSupport.Wrap.wrapDeltaX` is sketched in `.fsi` and exercised in FSI before
  the `.fs` body (Phase 1 contract + quickstart). The renderer fix is behavior with
  failing-first golden/pixel tests; the governance target and skill/doc edits are
  exercised by real gate runs.
- **Principle II (visibility in `.fsi`).** FR-010 adds a curated `Wrap.fsi` and a new
  per-package surface-baseline line, updated together (FR-011). The shared
  `SceneRenderer` module is **non-public** (no `.fsi` export) so SkiaViewer's surface
  baseline is unchanged. No `private`/`internal`/`public` modifiers on `.fs`
  top-level bindings.
- **Principle III (idiomatic simplicity).** The shared painter is one plain
  `match`; `wrapDeltaX` is 4 lines of scalar arithmetic; the diagnostic change is a
  label swap. No SRTP/reflection/clever abstractions. The renderer reuses existing
  helpers (`configurePaint`/`toSkPath`/`drawTextWithFallback`) rather than
  re-deriving them.
- **Principle IV (Elmish/MVU boundary).** N/A for the framework — no
  interpreter/effects/host-runtime change. `wrapDeltaX` is a pure helper a consumer
  threads through *their* `update`; it owns no state and performs no I/O.
- **Principle V (synthetic disclosure).** No synthetic evidence planned — real golden
  image capture, real gate runs, real `Wrap`/symbol-diff tests. Any task that cannot
  reach real evidence is marked `[S]` with full disclosure; none anticipated; no
  `[SEH]` foreseen.
- **Principle VI (test evidence).** Failing-first: renderer golden/pixel assertions
  (Line/Path/Text render), `Wrap` determinism/range/shortest-path, and the
  `SymbolCrossCheck` target output on a seeded drift; real gate runs for the
  governance/skill/doc work.
- **Principle VII (observability).** Directly improved — FR-002 removes a silent
  false-positive (placeholder masquerading as a rendered scene), FR-004 makes the
  readiness diagnostic legible (absent vs full required set), FR-003 gives the
  symbol cross-check a real invocation surface.

### Repository Governance Decisions

- **Template ownership**: **Update required.** The `.fsi`-authoritative note in
  `template/base/docs/scaffold-map.md` (FR-006) flows through the existing template
  content map; no new template file is added (the doc already ships). The phase-skill
  edits (`speckit-implement`/`-plan`/`-specify`/`-analyze`) are made in canonical
  `.agents/skills/**` and regenerated to `.claude/**` (`RefreshSurfaceBaselines`);
  `.template.config/template.json` needs no new entries. The bumped/packed template
  carries the regenerated skills + the scaffold-map note + the `wrapDeltaX` consumer
  surface (already pinned in `template/base/Directory.Packages.props`).
- **Dependency impact**: **N/A — no new third-party dependency.** The renderer reuses
  SkiaSharp APIs already referenced; `Wrap` uses the BCL only;
  `SymbolCrossCheck` is existing `FS.Skia.UI.Build` code. `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` coverage are unchanged.
- **Command-surface impact**: **Update required.** A new `SymbolCrossCheck` FAKE
  target is added (`Targets.fs` DU/`allTargets`/`name`/`directPrerequisites`,
  `AgentValidation.knownGates`, `Engine/Model.fs` effect, `Engine/Update.fs`
  dispatch, `Engine/Interpret.fs` interpreter, `Front/Helpers.fs` contract) and
  `validation.contract.yml` is regenerated via `RefreshSurfaceBaselines` so
  `TargetMetadataDrift` stays green (FR-003). `EvidenceAudit`/image-evidence capture
  reflects the renderer fix; `Render.fs` relabels the readiness diagnostic (FR-004).
  `SkillSyncCheck`/`SkillQualityCheck`/`TemplateCheck`/`GeneratedProductCheck` change
  for the skill/doc edits; `PackageSurfaceCheck`/`PerPackageSurfaceDiff` cover the
  new SkillSupport surface (FR-010). FAKE-backed commands share `.fake` state and run
  **sequentially** in deterministic order (Dev → GeneratedGuidanceCheck →
  TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit); safe
  non-FAKE reads may parallelize. The authoritative gate list is whatever
  `./fake.sh build -t Route` prints for the actual diff.
- **Generated project impact**: **Update required.** Generated projects gain a
  faithful image-evidence renderer (Line/Path/Arc/real-glyph Text now render —
  FR-001/002), the regenerated phase skills (evidence-formats pointer + skill-loading
  note + scaffold-map pointer + URL snapshot + analyze pass-G target — FR-004/005/
  006/007), the scaffold-map `.fsi`-authoritative note (FR-006), and the `wrapDeltaX`
  helper + skill reference (FR-010). No default scene/behavior change; no
  excluded-history or placeholder-scan change.
- **Evidence paths**: Real evidence under
  `specs/063-lunar-lander-consumer-friction-followups/readiness/`:
  `target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`,
  `aggregate-hang-diagnostics.md` (Route-required escalated-tier); the **before/after
  image capture** proving Line/Path/Text render to pixels (FR-001/002);
  `symbol-cross-check.md` (FR-003); the `Wrap` unit-test output and the updated
  `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` (FR-010); the
  template-scan record for the FR-008 disposition; `synthetic-evidence.json` only if
  an `--accept-synthetic` override is ever needed (none anticipated).
- **`.fsi` / contract impact**: **Change required (FR-010 only).** New curated
  `Wrap.fsi` + the per-package surface baseline. The shared `SceneRenderer` is
  non-public, so **no SkiaViewer `.fsi`/surface change**. No framework or consumer DU
  case is renamed (out of scope).
- **MVU/effect boundary**: **N/A — no stateful/IO framework work.** No
  `Model`/`Msg`/`Effect`/`init`/`update`/interpreter is added or changed. The
  renderer is a pure draw walk; `wrapDeltaX` is a pure helper for a consumer's
  `update`.
- **Synthetic evidence**: **None planned.** Real golden image capture, real gate
  runs, real `Wrap`/symbol-diff tests. `[S]` disclosure applies only if a real path
  proves infeasible mid-implementation (not anticipated); no `[SEH]` cases foreseen.
- **Test evidence**: Failing-first Expecto tests — renderer golden/pixel assertions
  (Line/Path/Text visible; exhaustive match = compile guard), `Wrap`
  determinism/range/shortest-path/identity, and the `SymbolCrossCheck` target output
  on a seeded drift; governance tests for the new target (knownGates/contract
  currency); real gate runs (`TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit`).
- **Observability**: FR-002 removes the silent placeholder false-positive and makes
  the evidence/interactive renderers one shared painter; FR-004 prints
  `full-required-set:` vs `absent-from-file:` distinctly; FR-003 gives pass G a real
  command (`./fake.sh build -t SymbolCrossCheck`) writing
  `readiness/symbol-cross-check.md`.
- **Deferred scope**: The camera-centered projection and the `--evidence-run`
  deterministic-summary helper are **documented, not shipped** (D9/D10) — field
  shapes vary per game / soft `Scene` dependency; each is recorded with a rationale
  and next-recurrence bar (SC-006). No new game/demo, no new runtime capability,
  platform, release, or distribution change. The `SymbolCrossCheck` target and the
  effective diagnostics are delivered as commands/diagnostics, not new hard merge
  gates. FR-008 is closed with no code change (no template seeds the divergent
  `evidence/` token).

## Project Structure

```
specs/063-lunar-lander-consumer-friction-followups/
├── spec.md                 # complete (no clarifications needed)
├── plan.md                 # this file
├── research.md             # D1–D11 design decisions
├── data-model.md           # entities (shared painter, symbol-target wiring, readiness labels, Wrap)
├── contracts/
│   ├── renderer-parity.md            # FR-001/002 shared SceneRenderer contract
│   ├── symbol-crosscheck-target.md   # FR-003 FAKE target contract + wiring
│   ├── skillsupport-wrap-api.md      # FR-010 Wrap.wrapDeltaX .fsi + baseline
│   └── authoring-and-skill-edits.md  # FR-004/005/006/007/008 skill + diagnostic edits
├── quickstart.md           # end-to-end verification (Route-first, serialized gates)
└── readiness/              # real evidence artifacts (see Evidence paths)

# Source touched (authoritative tier/gates per Route on the actual diff):
src/SkiaViewer/SceneRenderer.fs                    # FR-001/002 NEW non-public shared painter (paintNode + moved helpers)
src/SkiaViewer/Host/Vulkan.fs                      # FR-001/002 drawScene delegates; helpers moved out
src/SkiaViewer/SkiaViewer.fs                       # FR-001/002 drawScreenshotScene delegates; wildcard deleted
src/SkiaViewer/SkiaViewer.fsproj                   # FR-001 add SceneRenderer.fs compile entry (before consumers)
src/SkillSupport/Wrap.fsi|.fs                      # FR-010 wrapDeltaX (NEW module)
src/SkillSupport/SkillSupport.fsproj               # FR-010 add Wrap compile entries (after Hud)
readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt   # FR-010 baseline += Wrap (Tier-1)
build/Governance/Targets.fs                        # FR-003 SymbolCrossCheck in DU/allTargets/name/prereqs
build/Governance/AgentValidation.fs                # FR-003 "SymbolCrossCheck" in knownGates
build/Governance/Engine/Model.fs                   # FR-003 SymbolCrossCheckAnalyze BuildEffect
build/Governance/Engine/Update.fs                  # FR-003 StartTarget dispatch + RequireFiles
build/Governance/Engine/Interpret.fs               # FR-003 interpret: read feature artifacts, render, write
build/Governance/Front/Helpers.fs                  # FR-003 focusedGateContract case
build/Governance/Evidence/Render.fs                # FR-004 readiness diagnostic relabel (full-required vs absent)
validation.contract.yml                            # FR-003 regenerated via RefreshSurfaceBaselines
.agents/skills/speckit-implement/SKILL.md          # FR-004 / FR-005 evidence-formats pointer + skill-loading note
.agents/skills/speckit-plan/SKILL.md               # FR-006 scaffold-map pointer
.agents/skills/speckit-specify/SKILL.md            # FR-007 URL source snapshot step
.agents/skills/speckit-analyze/SKILL.md            # FR-003 pass G runs ./fake.sh build -t SymbolCrossCheck
.agents/skills/fs-skia-scene/ (src/Scene/skill/SKILL.md)   # FR-002 document the unified evidence/interactive renderer
.agents/skills/fs-skia-layout-readability/SKILL.md # FR-010 wrapDeltaX reference + deferred camera/summary docs
template/base/docs/scaffold-map.md                 # FR-006 ".fsi is authoritative" note
```

**Workstream → change-set / Route boundary.** Implement in dependency order so each
change-set routes cleanly: (1) **renderer parity** (`src/SkiaViewer/**`) → inner-loop
+ evidence/image gates, SkiaViewer surface **unchanged**; (2) **`SymbolCrossCheck`
target** (`build/Governance/**` + `validation.contract.yml`) → `TargetMetadataDrift`
+ `Test`; (3) **authoring/diagnostic content** (`.agents/**`, `Render.fs`,
`scaffold-map.md`) → `SkillSyncCheck`/`SkillQualityCheck`/`TemplateCheck`/
`GeneratedProductCheck`; (4) **FR-010 `Wrap` helper last** — the only Tier-1
change-set, pulling in `PackageSurfaceCheck`/`PerPackageSurfaceDiff` + the new
baseline line. Re-run `SkillSyncCheck`/`TargetMetadataDrift`/`SkillQualityCheck`
green after every `.agents` edit + `RefreshSurfaceBaselines`.
