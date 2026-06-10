# Implementation Plan: Typed Front-Door Discoverability & Spec-Kit Workflow Followups

**Branch**: `089-typed-surface-and-workflow-followups` | **Date**: 2026-06-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/089-typed-surface-and-workflow-followups/spec.md`

## Summary

Four still-open `ControlsShowcase1` feedback items, triaged against current source and scoped to
**governance / docs / Spec-Kit skill tree** (no runtime behavior change):

1. **TYPED-SURFACE-1 (P1).** Publish the typed front door (`FS.Skia.UI.Controls.Typed`) in the
   surface a consumer already reads, so a whole-catalog consumer never has to reflect
   `FS.Skia.UI.Controls.dll`. **Approach:** enroll the 14 existing `src/Controls/Widgets/*.fsi`
   files — which together declare 52 `FS.Skia.UI.Controls.Typed` modules, one per catalog control —
   into `ApiSurfaceGen`'s capability surface (via `template/capabilities.yml` `contracts:`) so they
   emit byte-identically into `docs/api-surface/Controls/` (and `template/base/docs/api-surface/Controls/`),
   giving every control's `Props` fields (optional = `option`-typed), `view` arity, and event
   callbacks straight from the source of truth; **plus** a thin, single-source **id → typed-module**
   index (a new `TypedModule` field on `TypedCatalogFact`, rendered into `catalog.yml`) so the
   per-control linkage is navigable for the ~7 controls whose legacy `Module` token differs from
   their typed module (e.g. `list-view` → legacy `Collections`, typed `ListView`). Currency is
   enforced by the **existing** `ApiSurfaceGen.currency` / `TargetMetadataDrift` and the catalog
   currency gate — no hand-authored Props prose, so nothing can drift.

2. **VERIFY-IMPL-1 (P1).** Add durable run-and-use discipline to `speckit-implement`: for any
   interactive-UI user story, launch + interact via the `run`/`verify` skills and confirm the
   evidence exercised the **production** render path (`controlsExampleView` → `Control.renderTree`)
   before marking `[US*]` done.

3. **EVGRAPH-ECHO-1 (P2).** Make `EvidenceGraph` echo each distinct `[skillist: <id>]` token's
   `id → SKILL.md path` resolution (reusing `SkillRegistry`), flagging unresolved/alias/ambiguous
   tokens distinctly — removing the manual `grep '^name:'` cross-check.

4. **CLARIFY-SOURCE-1 (P3).** Add a `speckit-clarify` step: when a `source-spec.md` snapshot exists
   in the feature directory, consult it before forming questions (no-op when absent).

Items 2 and 4 are `.agents` skill-source edits regenerated into `.claude` via
`RefreshSurfaceBaselines` (`SkillSyncCheck`-enforced). Item 3 is a `build/Governance/Evidence`
render change. Item 1 touches `template/capabilities.yml`, `build/Governance/CatalogGen`, and the
emitted `docs/api-surface` tree.

## Technical Context

**Language/Version**: F# / .NET (`net10.0`); `FS.Skia.UI.Build` governance assembly under `build/Governance/**`
**Primary Dependencies**: None new. Existing seams only — `ApiSurfaceGen`, `CatalogGen`/`CatalogDocsGen`, `Evidence/{Graph,Render,SkillRegistry}`, the `.agents`→`.claude` skill-tree generator.
**Testing**: Expecto governance tests (`build/Governance/**` test project), FAKE targets, the serialized six-target order; markdown/byte currency assertions (no FSI/product runtime change)
**Target Platform**: Windows and Linux (governance/docs only; no Skia/Vulkan/runtime surface touched)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: **Update required.** `template/capabilities.yml` (Controls `contracts:`)
  gains the 14 `src/Controls/Widgets/*.fsi` entries; consequently the emitted
  `template/base/docs/api-surface/Controls/` tree gains 14 typed `.fsi` files, and
  `template/base/...catalog.yml` gains the new `TypedModule` token per control. These are
  regenerated artifacts (`RefreshSurfaceBaselines`), not hand-edited. No `.template.config/template.json`
  manifest change (no new top-level file class — the api-surface dir and catalog.yml already ship).
- **Dependency impact**: **N/A — no dependency change.** No new package, no `Directory.Packages.props`
  edit, no `docs/dependencies.md` / `DependencyReport` change.
- **Command-surface impact**: **Update required (output/behavior of existing targets, no new wrapper).**
  `RefreshSurfaceBaselines` regenerates the api-surface tree, catalog.yml/Catalog.fs (new field),
  the `.claude` skill mirror, and `validation.contract.yml` if routing rows change (they do not —
  no new gate is added). `TargetMetadataDrift`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `SkillSyncCheck`, `EvidenceGraph` change output and must be re-run. **No new FAKE target.**
  FAKE-backed commands share `.fake` state — run sequentially in the deterministic six-target order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: **Update required (additive).** Generated projects inherit the
  enriched `docs/api-surface/Controls/` (typed `.fsi` now present) and the `TypedModule` token in
  `catalog.yml`. No generated `Dev` behavior, placeholder-scan, or excluded-history-scan change;
  the legacy builder surface remains published (additive, not a replacement — FR-003).
- **Evidence paths**: `readiness/` and emitted-tree artifacts —
  `template/base/docs/api-surface/Controls/*.fsi` (14 new typed files, byte-identical to source);
  `src/Controls/catalog.yml` + `template/base/...catalog.yml` (new `TypedModule` token);
  `readiness/task-graph.md` + `logs/evidence-graph.txt` (new `id → SKILL.md path` resolution
  section); `readiness/skill-sync-check.md` (`.claude` ↔ `.agents` byte-identity for the two edited
  skills); `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` (recaptured — unchanged
  expected, the Widgets `.fsi` were already in-package); the serialized six-target logs.
- **`.fsi` / contract impact**: **No new runtime `.fsi` signature.** The typed `Widgets/*.fsi`
  already exist and are merely *captured* into the published api-surface (Tier-1 *published-contract*
  change, additive). The one source-of-truth schema change is the `TypedModule` field added to the
  governance type `CatalogGen.TypedCatalogFact` (a `build/Governance` type, not a shipped runtime
  surface) — its `.fsi` (`CatalogGen.fsi`) is updated. Per-package and api-surface baselines recaptured.
- **MVU/effect boundary**: **N/A — no stateful or I/O-bearing runtime work.** All generation is pure
  render/splice/currency over in-memory facts at the `build/Governance` interpreter edge (Principle
  IV applies to the framework runtime, not to this docs/governance change), matching the existing
  `CatalogDocsGen` / `ApiSurfaceGen` posture. VERIFY-IMPL-1 governs *how interactive evidence is
  accepted in the workflow*, introducing no runtime `Model`/`Msg`/`Effect`.
- **Synthetic evidence**: **None planned.** Every artifact is real — real emitted `.fsi` bytes copied
  from source, real catalog regeneration, real gate output, real `.claude`/`.agents` byte-identity.
  No mocks, fakes, placeholders, or `[S]` tasks anticipated; if any arises it gets full Principle-V
  disclosure.
- **Test evidence**: Failing-first governance tests in the `build/Governance` test project —
  (a) `ApiSurfaceGen`/capabilities currency includes the 14 typed `.fsi`; (b) `CatalogGen` renders
  the `TypedModule` token and currency fails on drift; (c) `Evidence/Render` emits the
  `id → SKILL.md path` resolution + unresolved flag for resolved/alias/ambiguous/unknown ids;
  (d) `SkillSyncCheck` / skill-quality-exclusion holds for the two edited speckit skills. No host
  smoke test (no runtime change).
- **Observability**: The new `EvidenceGraph` resolution section is the observability win (makes an
  already-computed resolution visible, flags unresolved tokens distinctly — Principle VII). Currency
  diagnostics already name the drifted file + the `RefreshSurfaceBaselines` remedy; the new field and
  emitted files inherit that. No silent failure path is introduced.
- **Deferred scope**: All 086-cluster runtime primitives (Scene `Translate`/`SizedText`,
  per-`ControlId` bounds, multi-axis layout, pointer-aware host, neutral scaffold, key warm-up) are
  **already shipped** and out of scope. Live-window persistent-launch capture, new package
  identities/versions, and any tooling outside `.agents`/`.claude`/`build/Governance` are deferred /
  out of scope (see spec "Out of Scope"). Versioning/packing follows the normal merge flow.

**Change classification**: **Escalated / `maintainer-verify` (Tier 1, published-contract).** Touches
consumer-contract surfaces (`template/**`, emitted `docs/api-surface`), the `.agents` skill tree, and
governance code — `Route` is expected to escalate; run the serialized six-target order.

## Project Structure

Files this feature touches (all under existing seams — no new project, no new runtime module):

```
# TYPED-SURFACE-1
template/capabilities.yml                         # +14 Widgets/*.fsi in Controls contracts:
build/Governance/CatalogGen.fs / .fsi             # +TypedModule field on TypedCatalogFact; render into YAML row
build/Governance/CatalogDocsGen.* (if id→module index is surfaced in docs/controls)  # optional, see research
template/base/docs/api-surface/Controls/*.fsi     # REGENERATED: +14 typed .fsi (RefreshSurfaceBaselines)
src/Controls/catalog.yml                          # REGENERATED: +TypedModule token per control
src/Controls/Catalog.fs                           # REGENERATED if F# row carries the field
template/base/.../catalog.yml                      # REGENERATED mirror

# VERIFY-IMPL-1
.agents/skills/speckit-implement/SKILL.md         # +interactive run-and-use step (~after Workflow step 6)
.claude/skills/speckit-implement/SKILL.md         # REGENERATED mirror (RefreshSurfaceBaselines)

# EVGRAPH-ECHO-1
build/Governance/Evidence/Render.fs / .fsi        # +skillist id→SKILL.md path resolution section
build/Governance/Evidence/Engine.fs               # thread SkillRegistry into taskGraphMd (already in EvidenceInputs)

# CLARIFY-SOURCE-1
.agents/skills/speckit-clarify/SKILL.md           # +source-spec.md pre-check step (~after step 1)
.claude/skills/speckit-clarify/SKILL.md           # REGENERATED mirror

# Tests
build/Governance/**/*Tests*.fs (governance test project)  # failing-first coverage for each item above

# Regenerated currency artifacts
readiness/task-graph.md, logs/evidence-graph.txt, readiness/skill-sync-check.md,
readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt
```

See [research.md](./research.md) for the seam-by-seam findings, [data-model.md](./data-model.md) for
the `TypedModule` fact and resolution-line shapes, [contracts/](./contracts/) for the published-surface /
gate-output / skill-guidance contracts, and [quickstart.md](./quickstart.md) for the regenerate-and-verify loop.
</content>
