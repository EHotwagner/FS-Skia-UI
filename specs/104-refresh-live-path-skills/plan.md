# Implementation Plan: Refresh live-path skill currency

**Branch**: `104-refresh-live-path-skills` | **Date**: 2026-06-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/104-refresh-live-path-skills/spec.md`

## Summary

Three skill-documentation defects, all consequences of the R1–R6 live-path roadmap (features
096–103) landing after the relevant skills were authored. The fix is a **pure documentation
honesty pass** (feature-102 precedent): no `.fsi`, no behavior, no test-outcome change.

1. **US1 (P1)** — refresh `.agents/skills/fs-skia-reconciliation/SKILL.md` so its live-path
   disposition is current through feature 103 (layout bounds cache + `RemeasuredNodeCount`,
   `AnimationClock`/`sampleOnPaint` cross-fade, runtime visual-state bridge), replacing the
   superseded "further work *builds atop* the wired path" forward-looking language.
2. **US2 (P2)** — correct `src/Controls/skill/SKILL.md` E3 (name the public `deriveVisualState`
   / internal `applyRuntimeVisualState` surface, feature 096) and E4 (`Focus.route`'s post-100
   role + `navRange` → `NavIntent` shape).
3. **US3 (P3)** — add a maintainer-facing `.agents/skills/fs-skia-controls-host/SKILL.md`
   covering the `Controls.Elmish` interactive-host seam (`runInteractiveApp` + per-frame
   retained-state / clock / visual-state wiring), cross-linked with the reconciliation and
   viewer-host skills.

Every claim is anchored to a verified source signature (see `contracts/currency-claims.md`).
The `.claude/skills/**` mirror is regenerated from the canonical `.agents` tree, and the new
US3 id is registered into `skillist-reference.md`, via `RefreshSurfaceBaselines`.

**Tier: 2 (internal change).** No public API surface changes; no `.fsi`/baseline delta. This is
documentation + governance-generated artifacts only.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (the governance/skill artifacts are Markdown; the
gates that police them are compiled F# in `FS.Skia.UI.Build`).
**Primary Dependencies**: None new. Touches Markdown skill files and the `SkillTreeGen` /
`SkillistReference` / `SkillQuality` / `SkillSync` governance in `build/Governance/**`.
**Testing**: `SkillQualityCheck` + `SkillSyncCheck` gates; `Governance.Tests`; the standard
readiness chain (`EvidenceGraph`/`EvidenceAudit`). No new product tests (FR-008 forbids
behavior change, so there is nothing new to assert in the product test suites).
**Target Platform**: Windows and Linux (unchanged).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A — no source/docs/samples/tests/Spec-Kit-asset/package-policy or
  command-surface change reaches `.template.config/template.json`. The edited skills are
  repo-local (`.agents/skills/**`) and the Controls package skill body; neither is a template
  asset. No template update or deferral required.
- **Dependency impact**: N/A — no dependency change; `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` are untouched.
- **Command-surface impact**: No new targets. Existing gates do the work: `Route` selects the
  gate list; `SkillQualityCheck` + `SkillSyncCheck` validate currency/sync;
  `RefreshSurfaceBaselines` regenerates `.claude/skills/**` + `skillist-reference.md`. FAKE-backed
  targets run sequentially in the documented order. Example order:
  1. `./fake.sh build -t Route` (authoritative gate list for this diff)
  2. `./fake.sh build -t RefreshSurfaceBaselines` (regenerate `.claude` mirror + skillist)
  3. `./fake.sh build -t Dev`
  4. the gates `Route` prints (expected: `SkillQualityCheck`, `SkillSyncCheck`,
     `TargetMetadataDrift`/`SkillSyncCheck` currency, and — per the 102 precedent, because US2
     edits `src/Controls/**` — possibly the controls-public-surface set + `EvidenceGraph` +
     `EvidenceAudit`).
- **Generated project impact**: N/A — no default/minimal generated content, selected-Controls
  guidance, local skills, validation logs, placeholder/excluded-history scans, or generated
  `Dev` behavior changes. The edited skills are not template-shipped product skills.
- **Evidence paths**: `specs/104-refresh-live-path-skills/readiness/**` (focused-gates, evidence
  graph, evidence audit, skill-quality-check output); the regenerated
  `.claude/skills/**` tree and `template/base/docs/skillist-reference.md` (currency proof); a
  `git diff --stat` showing zero `src/**/*.fsi` lines (FR-008/SC-005 proof).
- **`.fsi` / contract impact**: **None.** No signature, public doc, surface baseline, sample
  contract, or compatibility note changes. This is the load-bearing constraint (FR-008): the
  skills *describe* the existing `RetainedRender.fsi` / `Focus.fsi` / `ControlRuntime.fsi`
  surface; they do not alter it. Tier 2.
- **MVU/effect boundary**: N/A — no stateful or I/O-bearing work. The interactive host's
  Model/Msg/Effect/interpreter boundary is *documented* by the US3 skill, not modified.
- **Synthetic evidence**: None. No mocks/fakes/placeholders; every skill claim is anchored to a
  real, verified source signature on `main` (`contracts/currency-claims.md`). No `[S]` tasks
  anticipated.
- **Test evidence**: Failing-first is expressed at the *currency* level: before the edits the
  skills assert stale facts (e.g. "further work builds atop the wired path"); after, those
  assertions are gone and `SkillQualityCheck`/`SkillSyncCheck` pass against the regenerated
  tree. `Governance.Tests` covers the skillist-reference regeneration for the new id.
- **Observability**: The gate outputs are the diagnostics — `SkillQualityCheck` names any
  missing rubric section per skill; `SkillSyncCheck` names any `.agents`↔`.claude` drift;
  `Route --enforce` names any missing evidence artifact.
- **Deferred scope**: The remaining skill-less packages (`Color`, `Input`, `SkillSupport`) and
  any future supersession past feature 103 are explicitly deferred (spec A3/A4). No full 36-skill
  corpus migration; no `fs-skia-viewer-host` consumer-redesign beyond a cross-link.

**Gate result: PASS.** Tier 2, no `.fsi`/baseline obligation; all governance areas filled.

## Project Structure

### Artifacts touched (no `src/**` library code)

```
.agents/skills/fs-skia-reconciliation/SKILL.md      # US1 — refresh disposition to 103
.agents/skills/fs-skia-controls-host/SKILL.md        # US3 — NEW maintainer host skill
src/Controls/skill/SKILL.md                          # US2 — E3 + E4 currency edits
.claude/skills/fs-skia-reconciliation/SKILL.md       # generated mirror (RefreshSurfaceBaselines)
.claude/skills/fs-skia-controls-host/SKILL.md         # generated mirror (NEW)
template/base/docs/skillist-reference.md             # generated — registers new US3 id
```

### Source read for grounding (NOT edited)

```
src/Controls/RetainedRender.fsi      # AnimationClock, LayoutResult cache, RemeasuredNodeCount, sampleOnPaint
src/Controls/Focus.fsi               # NavIntent, route(role, navRange, ...)
src/Controls/ControlRuntime.fsi      # deriveVisualState (public), applyRuntimeVisualState (internal)
src/Controls.Elmish/ControlsElmish.fsi  # runInteractiveApp, routeFocusedKey host seam
```

### Governance that enforces the change (NOT edited)

```
build/Governance/SkillQuality.fs     # 7-section rubric gate
build/Governance/SkillSync.fs        # .agents -> .claude byte-identity
build/Governance/SkillTreeGen.fs     # .claude mirror generator
build/Governance/SkillistReference.fs / SkillistView.fs  # skillist-reference.md generator
```

## Design decisions (resolved in Phase 0)

- **D1 — US1 in place, not a new sibling.** Refresh `fs-skia-reconciliation` rather than spawn a
  new `fs-skia-retained-render`. The diff and the wired path are one story the skill already
  tells; a second skill would fragment it and inflate corpus churn against FR-008's minimalism.
  The refresh updates the **Disposition** section and adds a **"Live retained render path
  (096–103)"** subsection.
- **D2 — US3 is an `.agents` domain skill, id `fs-skia-controls-host`.** Not a
  `src/Controls.Elmish/skill/` package skill: package skills are not mirrored into
  `.claude/skills/**` and would not satisfy FR-007's `.claude`/`skillist` discoverability, and
  the host story is cross-cutting (Controls.Elmish host + Controls `RetainedRender`), which fits
  a repo-local domain skill better than a single-package capability skill. Id verified free in
  both `.agents` and package-skill namespaces; distinct from the consumer-facing
  `fs-skia-viewer-host` (spec A2).
- **D3 — No constitution edit.** The constitution's capability-skill registry (lines 261–286) is
  selective and does not list every `.agents` skill (`fs-skia-reconciliation`,
  `fs-skia-viewer-host` are absent), so the new `fs-skia-controls-host` requires no constitution
  amendment. Keeping governance docs untouched preserves the Tier-2 / zero-churn posture.
- **D4 — Regenerate, never hand-edit, the mirror.** `.claude/skills/**` and
  `skillist-reference.md` are produced by `RefreshSurfaceBaselines`; editing them by hand would
  trip `SkillSyncCheck` / `TargetMetadataDrift`. The canonical `.agents` files are the only
  hand-authored inputs.

## Phase 0 — Research

See [research.md](./research.md). All unknowns resolved (no `NEEDS CLARIFICATION` remained from
the spec). Source-signature grounding captured in [contracts/currency-claims.md](./contracts/currency-claims.md).

## Phase 1 — Design & Contracts

- **Entities** — [data-model.md](./data-model.md): the skill artifacts, their required rubric
  sections, and the claim→source verification mapping.
- **Contracts** — [contracts/currency-claims.md](./contracts/currency-claims.md): the
  authoritative per-skill list of claims each refreshed/new skill MUST make, each bound to a
  verified `.fsi` anchor; and [contracts/rubric.md](./contracts/rubric.md): the 7-section
  `SkillQualityCheck` contract every edited/added skill must satisfy.
- **Quickstart** — [quickstart.md](./quickstart.md): how to validate the change (Route, the
  gates, the zero-`.fsi`-delta proof).
- **Agent context** — `AGENTS.md` SPECKIT marker updated to point at this plan.

## Re-evaluation (post-design)

No new violations introduced by the design. Still Tier 2, still zero `.fsi`/baseline delta, all
governance areas filled, no synthetic evidence. **Constitution Check: PASS.**
