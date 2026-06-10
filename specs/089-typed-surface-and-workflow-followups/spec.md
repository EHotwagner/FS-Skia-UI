# Feature Specification: Typed Front-Door Discoverability & Spec-Kit Workflow Followups — Generated Typed-Control Surface, Verify-During-Implement Discipline, EvidenceGraph Skill-Path Echo & Clarify Source-Spec Pre-Check

**Feature Branch**: `089-typed-surface-and-workflow-followups`
**Created**: 2026-06-10
**Status**: Draft
**Input**: User description: "create specs from the not yet addressed feedback from the sibling repo controlsshowcase1"

## Context & Triage *(informative)*

The same consumer that drove feature 086 — a 52-control typed Elmish **Controls Gallery**
generated from `FS.Skia.UI` (`ControlsShowcase1`) — left per-phase Spec Kit feedback under
`ControlsShowcase1/specs/001-controls-gallery/feedback/{specify,clarify,plan,tasks,analyze,implement}-2026-06-09.md`.
Feature 086 scoped itself (per the single-feature rule) to the **interactive non-game consumer
fitness** cluster and explicitly deferred the rest. This feature picks up the **still-open
remainder**, triaged against **current framework source** so already-shipped items are *not*
re-specified.

Confirmed present and therefore **out of scope** (already shipped by 085/086/087/088, verified in
source): the Scene `Translate` (`src/Scene/Scene.fs:299`) and `SizedText` (`:302`) primitives;
per-`ControlId` `Bounds` on `ControlRenderResult` (`src/Controls/Types.fs:264`); multi-axis
layout for Dock/Grid/horizontal Stack (`src/Controls/Control.fs:1027`); the pointer-aware governed
default host for the `app` profile (`template/base/src/Product/Program.fs:152`); the
domain-neutral scaffold whose `view` calls `Control.renderTree` (`template/base/src/Product/{Model,View}.fs`);
the keyboard warm-up buffer/drain (`src/SkiaViewer/SkiaViewer.fs:1446`); the multi-file
external-tree `source-spec.md` snapshot recipe (085); and the **skillist-name registry validator**
the analyze-phase asked for — already implemented in `build/Governance/Evidence/Audit.fs:151`
(`registry.Skills.TryFind skillId` → "declared skill … is not registered"). None of those are
re-specified here.

What remains open, triaged against current source:

| # | Sev | Source phase | Finding | Current-state evidence |
|---|-----|--------------|---------|------------------------|
| TYPED-SURFACE-1 | minor (recurring) | plan + implement + specify | **The typed front door is not enumerated in any published surface, so a whole-catalog consumer must reflect `FS.Skia.UI.Controls.dll` to recover each control's `Props` field names and `view` arity.** Hit in *three* phases: plan (probed the DLL with `strings` to confirm the typed front-door modules exist), implement (reflected `GetExportedTypes()` with a throwaway `dotnet fsi` script before Pages 02–10 could compile), and noted again in specify. `catalog.yml` records `module`/`requiredAttributes`/`events` but **not** the concrete typed `Props` required-vs-optional fields or the `view` signature. | `docs/api-surface/Controls/` (emitted to `template/base/docs/api-surface/Controls/`) lists only the **legacy builder** `.fsi` (`Control.fsi`, `Attributes.fsi`, `Catalog.fsi`, `Charts.fsi`, `Collections.fsi`, `ControlRuntime.fsi`, `CustomControl.fsi`, `DataGrid.fsi`, `Diagnostics.fsi`, `RichText.fsi`, `TextInput.fsi`, `Theme.fsi`, `Types.fsi`, `Accessibility.fsi`) — no `FS.Skia.UI.Controls.Typed` surface. The typed front door lives in `src/Controls/Widgets/*.fs` (`*Props` records + `view` under `FS.Skia.UI.Controls.Typed`; **all 14** files carry a matching `.fsi`, together declaring 52 typed modules — one per catalog control) but is **not enrolled in `ApiSurfaceGen`'s capability surface** (`build/Governance/ApiSurfaceGen.fs`). The single-source `catalogFacts`/`TypedCatalogFact` (`build/Governance/CatalogGen.fsi:15`) carries `Module`/`RequiredAttributes`/`Events` but no Props-field or `view`-arity facts. |
| VERIFY-IMPL-1 | major (root cause) | implement | **`/speckit-implement` for an interactive-UI feature has no step that runs and uses the app before marking an interactive story done**, so a build that passed all 28 tests + both gates + produced 11 "real" screenshots shipped as a **non-interactive mockup** (the screenshots truthfully showed a *bespoke placeholder scene*, not the production render path). The consumer's own root-cause: "tests + gates + screenshots all passing is necessary but **not sufficient** for an interactive-UI feature; the missing discipline was *run it and use it*." 086 added the **evidence obligation** (capture must exercise `controlsExampleView` → `Control.renderTree`) as a spec-time governance prompt, but no implement-phase discipline enforces it. | `.agents/skills/speckit-implement/SKILL.md` executes tasks against the plan and updates `tasks.md`; it has **no** "for an interactive-UI story, invoke `run`/`verify` (launch + interact) and confirm the production render path was exercised before marking `[US*]` done" step. The `run` and `verify` skills exist but are never invoked from the implement phase. |
| EVGRAPH-ECHO-1 | minor | tasks | **The `EvidenceGraph` gate does not echo how each `[skillist: <id>]` token resolved**, so a name-vs-directory ambiguity (the `controlsshowcase1-widgets` token that is really the `name:` of the `fs-skia-ui-widgets/` directory) reads like a dangling ref and forces a manual `grep '^name:'` cross-check. | `build/Governance/Evidence/Graph.fs` validates/renders the DAG and computes propagation; its rendered output (`Render.fs`) emits node/edge/cycle counts but **not** a per-token `skill-id → SKILL.md path` resolution line. The resolver data exists (`SkillRegistry.fs`) but is not surfaced in the gate output. |
| CLARIFY-SOURCE-1 | minor | clarify | **`/speckit-clarify` scans only the active `spec.md` and never consults a snapshotted `source-spec.md`**, so without manual cross-checking it risks asking questions the source already answers (the consumer manually grepped `source-spec.md` first to keep its three questions on-target). | `.agents/skills/speckit-clarify/SKILL.md` forms its question set from the active spec only; there is no "if a `source-spec.md` snapshot exists in the feature directory, consult it before forming questions" step. |

**Change classification.** **Escalated / `maintainer-verify` (Tier 1).** This change touches a
**consumer-contract** surface (the published `docs/api-surface` tree, emitted into `template/**`)
by enrolling the typed front door, regenerates governance docs from a single source
(`build/Governance/**`: `ApiSurfaceGen.fs` and/or `CatalogGen`/`CatalogDocsGen`), edits the
canonical `.agents` skill tree for two Spec Kit skills (`speckit-implement`, `speckit-clarify`)
which must be regenerated into `.claude` via `RefreshSurfaceBaselines` (`SkillSyncCheck`-enforced),
and changes `EvidenceGraph` gate **output** (`build/Governance/Evidence/Graph.fs` / `Render.fs`).
No new **runtime** `src/**/*.fsi` public signature is introduced — the typed `Widgets/*.fsi`
already exist; they are merely *captured* into the published surface. `Route` is expected to
escalate this change; run the serialized six-target order (`Dev` → `GeneratedGuidanceCheck` →
`TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`), regenerate the
skill tree and surfaces with `RefreshSurfaceBaselines`, and recapture the affected api-surface
baselines.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Recover a typed control's `Props` and `view` from published docs, without reflecting the DLL (Priority: P1)

A consumer building against the typed front door (`FS.Skia.UI.Controls.Typed.*`) needs, per
control, the concrete `Props` record field names (which are required, which are optional) and the
`view` signature/arity, plus the event callbacks. They look it up in the published surface that
ships with the pinned package — and never have to decompile or reflect `FS.Skia.UI.Controls.dll`.

**Why this priority**: This is the single recurring framework/docs gap — it cost the consumer a
manual, fragile DLL probe in *three separate phases* (plan, implement, specify) and blocked
compilation of seven gallery pages until reflected. It is the standout still-open framework
deliverable and the clearest "make the typed front door a first-class, documented surface."

**Independent test**: From a clean checkout (no reflection, no decompilation), confirm the
published surface enumerates, for every supported catalog control, its typed module, its `Props`
field names with required-vs-optional disposition, its `view` signature/arity, and its event
callbacks; pick three stateful controls (e.g. a `CollectionModel`/`TextInputModel`-backed one)
and confirm a consumer can author a correct `Props` value and `view` call from the published
surface alone. Confirm the surface is **generated from a single source** and a currency check
fails if it drifts from the typed front door.

### User Story 2 - Interactive-UI work is not marked done until it has been run and used (Priority: P1)

A maintainer (or agent) implementing an interactive-UI feature is required, before marking any
interactive user story complete, to **launch the app, interact with it, and confirm the evidence
came from the production render path** — so a green-but-non-interactive mockup can no longer pass
as a finished interactive story.

**Why this priority**: This is the major, severity-flagged root cause — the discipline whose
absence let "28 tests + 2 gates + 11 screenshots, all green" ship as a non-interactive mockup.
Fixing the *workflow* (not just any one primitive) prevents the entire failure class for every
future interactive-UI feature.

**Independent test**: Inspect the `speckit-implement` skill and confirm it requires, for any
interactive-UI user story, an explicit run-and-use step (invoke `run`/`verify`: launch, interact,
observe) plus a confirmation that the captured evidence exercised the **production** render path
(`controlsExampleView` → `Control.renderTree`), not an author-built parallel scene — and that the
step must complete before the story is marked done. Confirm the guidance is visible in both the
`.agents` source and the regenerated `.claude` skill.

### User Story 3 - EvidenceGraph shows how every skillist token resolved (Priority: P2)

A maintainer running the `EvidenceGraph` gate sees, in its output, each `[skillist: <id>]` token
resolved to its concrete `SKILL.md` path (and any token that resolves to nothing flagged), so a
`name:`-vs-directory-name ambiguity is visible in the gate rather than requiring a manual
`grep '^name:'` cross-check.

**Why this priority**: Low blast radius but directly removes the tasks-phase friction the consumer
hit (the `controlsshowcase1-widgets` token that is really the `fs-skia-ui-widgets/` directory's
`name:`). It makes an already-computed resolution visible.

**Independent test**: Run `EvidenceGraph` on a feature whose `tasks.md` carries `[skillist: <id>]`
tokens and confirm the output lists, per distinct token, `id → resolved SKILL.md path`, and that a
token resolving to no installed skill is flagged distinctly in that same output.

### User Story 4 - Clarify consults the snapshotted source before asking (Priority: P3)

A maintainer running `/speckit-clarify` on a feature whose directory contains a `source-spec.md`
snapshot gets clarification questions that **skip anything the source already pins**, because the
clarify workflow consults the snapshot before forming its question set.

**Why this priority**: Narrow but real — it removes the manual "grep the source first" step the
consumer relied on to keep its questions on-target, and is a self-contained skill-doc change.

**Independent test**: Inspect the `speckit-clarify` skill and confirm it includes a step: when a
`source-spec.md` snapshot exists in the feature directory, consult it before forming questions and
do not ask what the source already resolves. Confirm the step is present in both the `.agents`
source and the regenerated `.claude` skill.

## Requirements *(mandatory)*

### Functional Requirements

**Published typed front-door surface (TYPED-SURFACE-1)**

- **FR-001**: The framework MUST publish a **typed front-door surface** that enumerates, for every
  supported catalog control, its `FS.Skia.UI.Controls.Typed` module, the control's `Props` record
  fields with each field's **required-vs-optional** disposition, the control's `view`
  signature/arity, and its event callbacks — sufficient for a consumer to author a correct typed
  `Props` value and `view` call **without reflecting or decompiling** `FS.Skia.UI.Controls.dll`.
- **FR-002**: The typed front-door surface MUST be **generated from a single source** (the existing
  `catalogFacts`/`TypedCatalogFact` single source and/or the typed `Widgets/*.fsi` already in
  `src/Controls/Widgets/`), not hand-authored, and MUST be **currency-enforced** by an existing or
  new governance check that fails when the published surface drifts from the actual typed front
  door — consistent with how `validation.contract.yml`, the catalog docs, and the api-surface are
  already kept current.
- **FR-003**: The typed front-door surface MUST be **shipped where a consumer already looks** — the
  published `docs/api-surface` tree (emitted into the generated project at
  `template/base/docs/api-surface/`) and/or the consumer-visible `catalog.yml` — so a generated
  project carries it without extra steps. The legacy builder surface MUST remain published
  (additive, not a replacement).
- **FR-004**: Coverage MUST be **whole-catalog**: every supported control (including the typed
  front-door-only controls absent from the legacy api-surface, and the stateful
  `CollectionModel`/`TextInputModel`-backed controls) MUST appear in the typed surface, so a
  whole-catalog consumer never falls back to a DLL probe for any control.

> Interacting / conflicting requirements: FR-001 (rich per-control typed facts) vs FR-002
> (single-source, no hand authoring) — resolve as: the surface is **projected from the typed front
> door itself** (the `Widgets/*.fsi` signatures and/or `catalogFacts`), so richness comes from the
> source of truth, never from hand-maintained duplicate prose that could drift.

**Verify-during-implement discipline (VERIFY-IMPL-1)**

- **FR-005**: The `speckit-implement` skill MUST require, for any **interactive-UI user story**,
  an explicit **run-and-use** step before that story is marked done — launch the app and interact
  with it (via the `run`/`verify` skill discipline), not rely on tests + gates + offscreen
  captures alone.
- **FR-006**: The same discipline MUST require confirming that the interactive **evidence exercises
  the production render path** — the real user-reachable surface the app actually drives, named by
  each feature for itself (for the controls consumer that path is `controlsExampleView` →
  `Control.renderTree`, given only as an illustrative example) — not a bespoke author-built scene
  assembled to look like it; a truthful screenshot of the *wrong* render path MUST NOT count as proof
  an interactive story is done. The durable skill guidance MUST state this generically so it binds
  every future interactive-UI feature (FR-007), never hard-coding one consumer's render-path symbol.
- **FR-007**: The discipline MUST be expressed as durable skill guidance (so it applies to every
  future interactive-UI feature, not just this one) and MUST be present in **both** the canonical
  `.agents/skills/speckit-implement` source and the regenerated `.claude` mirror.

> Interacting / conflicting requirements: FR-005/FR-006 (run-and-use gate) vs the existing
> "headless/offscreen evidence is acceptable" allowance — resolve as: offscreen capture remains
> valid evidence **only when it comes from the production render path**; for an *interactive* story
> the artifact must come from the real user-reachable surface exercised the way a user reaches it
> (launch + click/keys), per the consumer's own corrected reading of the vertical-slice rule.

**EvidenceGraph skill-path echo (EVGRAPH-ECHO-1)**

- **FR-008**: The `EvidenceGraph` gate output MUST echo, for each distinct `[skillist: <id>]` token
  in `tasks.md`/`tasks.deps.yml`, the resolved `id → SKILL.md path`, using the same resolution the
  skillist validator already performs (`SkillRegistry`), so a `name:`-vs-directory ambiguity is
  visible in the gate rather than requiring a manual cross-check.
- **FR-009**: A `[skillist: <id>]` token that resolves to **no** installed skill MUST be flagged
  distinctly in that same `EvidenceGraph` output (separate from the resolved lines), consistent
  with the existing registry validator's failure semantics.

**Clarify source-spec pre-check (CLARIFY-SOURCE-1)**

- **FR-010**: The `speckit-clarify` skill MUST include a step: when a `source-spec.md` snapshot
  exists in the feature directory, **consult it before forming questions** and do not ask
  clarifications the source already resolves.
- **FR-011**: FR-010's step MUST be present in **both** the canonical `.agents/skills/speckit-clarify`
  source and the regenerated `.claude` mirror; it MUST degrade gracefully (be a no-op) when no
  `source-spec.md` snapshot is present.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package *identity* or *version* change is required by this feature's
  intent. Package **contents** change for the controls package only insofar as the published
  typed front-door surface (FR-001/FR-003) ships alongside the existing legacy api-surface; the
  active controls package path is `src/Controls/**` (typed front door under
  `src/Controls/Widgets/*.fs(i)`). No legacy Charts package migration is involved.
- **Public contract impact**: The **published api-surface contract** changes additively — the
  typed front door is *captured into* `docs/api-surface` (emitted to `template/base/docs/api-surface/`).
  No new **runtime** `.fsi` signature is introduced; the typed `Widgets/*.fsi` already exist and
  are merely enrolled. Surface baselines for the controls package and the emitted api-surface tree
  must be recaptured.
- **State workflow impact**: None. No stateful workflow, I/O, command, effect, subscription, or
  interpreter behavior changes; the typed-surface generation is pure render/splice/currency over
  in-memory facts at the `build/Governance` interpreter edge (Principle IV), matching
  `CatalogDocsGen`/`ApiSurfaceGen`.
- **Layout/rendering impact**: None at runtime. No layout, chart, DataGrid, rendering, screenshot,
  Vulkan, Skia, or unsupported-environment behavior changes. VERIFY-IMPL-1 governs *how interactive
  evidence is produced and accepted*, not what the renderer does.
- **Evidence obligations**: Real evidence paths — the recaptured `docs/api-surface` (and
  `template/base/docs/api-surface/`) typed surface and its currency check passing; the regenerated
  `.claude/skills/{speckit-implement,speckit-clarify}` matching their `.agents` sources
  (`SkillSyncCheck`); the `EvidenceGraph` gate output showing `id → SKILL.md path` resolution; and
  the serialized six-target order passing on the change.
- **Unsupported scope**: Out of scope — any *runtime* layout/host/Scene/primitive work (all such
  086 items are already shipped, see Context & Triage), live-window persistent-launch capture,
  new package identities or versions, release/platform/distribution changes, and any IDE/agent
  tooling outside the `.agents`/`.claude` Spec Kit skill tree and `build/Governance`.
- **Build-target impact**: `GeneratedGuidanceCheck` and `TemplateCheck` (emitted api-surface +
  generated-product currency), `RefreshSurfaceBaselines` (regenerate the `.claude` skill tree and
  surface baselines), `EvidenceGraph` (new gate-output line) and `EvidenceAudit` must change/run;
  `Dev` and `GeneratedProductCheck` run as part of the serialized order. `TargetMetadataDrift` /
  `SkillSyncCheck` enforce currency of the generated artifacts.

## Success Criteria *(mandatory)*

- **SC-001**: A consumer can author a correct typed `Props` value and `view` call for **100%** of
  supported catalog controls using only the published surface shipped with the pinned package —
  **zero** controls require reflecting or decompiling `FS.Skia.UI.Controls.dll`. The sole documented
  exception is the bridge-typed `custom-control`, which by design exposes no `Props` schema and no
  `view` (it carries `TypedModule = CustomControl` and `RequiredAttributes = []`); "100%" is over the
  remaining supported controls and the `custom-control` carve-out is itself published.
- **SC-002**: For every supported control, the published typed surface states its module, its
  `Props` fields with required-vs-optional disposition, its `view` arity, and its event callbacks;
  a currency check fails if any of these drifts from the actual typed front door.
- **SC-003**: An interactive-UI user story cannot be marked done in the `speckit-implement`
  workflow without a recorded run-and-use step on the production render path; a build that renders
  a bespoke placeholder scene instead of the production path is rejected by that discipline rather
  than accepted as complete.
- **SC-004**: Running `EvidenceGraph` on a feature with `[skillist: <id>]` tokens shows each
  token's `id → SKILL.md path` resolution in the gate output, and any unresolved token is flagged
  there — no manual `grep '^name:'` cross-check is needed to confirm a token is valid.
- **SC-005**: Running `/speckit-clarify` on a feature whose directory contains a `source-spec.md`
  snapshot produces no question already answered by that snapshot.
- **SC-006**: The serialized six-target order passes on the change, the `.claude` skill mirror
  matches its `.agents` source, and the recaptured surface baselines are current.

## Assumptions

- The typed front-door modules under `src/Controls/Widgets/*.fs` are the authoritative typed
  surface; where a `.fsi` exists it pins the public shape, and where one does not the public shape
  is whatever the `.fs` exposes. The generated surface projects from these (and/or `catalogFacts`),
  so "single source" means the typed front door itself, not a new hand-authored doc.
- "Interactive-UI user story" (FR-005) is identifiable from the spec/plan (a story whose acceptance
  involves pointer/keyboard interaction with a live host); the discipline applies to those stories
  and is a no-op for non-interactive ones.
- `source-spec.md` (FR-010) is the in-repo snapshot convention established by `speckit-specify`
  (085 FR-016); the pre-check keys off that exact filename in the feature directory.
- The `EvidenceGraph` resolver already has the `SkillRegistry` data needed for FR-008/FR-009; this
  feature surfaces it in output rather than introducing new resolution logic.
- Versioning/packing follows the repo's normal merge flow (the libs, including `FS.Skia.UI.Build`,
  are bumped at merge); this spec does not pin a target version.

## Out of Scope

- All runtime primitives and host/layout work from the 086 cluster (Scene `Translate`/`SizedText`,
  per-`ControlId` bounds, multi-axis layout, pointer-aware governed host, neutral scaffold, key
  warm-up) — **already shipped**; re-verifying them is not part of this feature.
- The skillist-name **registry validator** itself (already implemented in
  `build/Governance/Evidence/Audit.fs`); EVGRAPH-ECHO-1 only adds *visibility* of the resolution,
  not new validation.
- The multi-file external-tree `source-spec.md` **snapshot routine** (already in `speckit-specify`,
  085 FR-016); CLARIFY-SOURCE-1 only *consumes* the snapshot.
- New package identities/versions, release/platform/distribution changes, live-window
  persistent-launch capture, and any tooling outside the `.agents`/`.claude` Spec Kit skill tree
  and `build/Governance`.

## Dependencies

- The single-source generation seam: `build/Governance/CatalogGen` (`TypedCatalogFact`,
  `catalogFacts`), `build/Governance/CatalogDocsGen`, and `build/Governance/ApiSurfaceGen.fs`
  (capability enrollment + emit/check of the published api-surface).
- The skill-currency spine: `RefreshSurfaceBaselines` (regenerate `.claude` from `.agents`) and
  `SkillSyncCheck` / `TargetMetadataDrift`.
- The evidence gate: `build/Governance/Evidence/{Graph,Render,SkillRegistry}.fs`.
- The `run` and `verify` skills (invoked by the implement discipline, FR-005).
- Source feedback (in-repo, sibling project):
  `ControlsShowcase1/specs/001-controls-gallery/feedback/{specify,clarify,plan,tasks,analyze,implement}-2026-06-09.md`.
