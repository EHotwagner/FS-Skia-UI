# Feature Specification: Asteroids-Demo Consumer Friction Follow-ups & Template-Update Skill Currency

**Feature Branch**: `060-asteroids-consumer-friction-followups`
**Created**: 2026-06-03
**Status**: Draft
**Input**: User description: "@docs/reports/2026-06-03-2207-asteroids-demo-consumer-implementation-friction-analysis.md check if some points were already addressed last feature. also check if the fs-skia-ui template update skill is working as intended and current."

## Context & Triage *(informative)*

The source report (`docs/reports/2026-06-03-2207-asteroids-demo-consumer-implementation-friction-analysis.md`)
catalogues nine friction findings (F1–F9) a consumer hit while implementing the
Asteroids arcade demo on generated `FS.Skia.UI.*` `0.1.62-preview.1` packages.
Per the user's two explicit asks, each finding was triaged against feature
`059-speckit-tasks-validation-feedback` (the last feature) and the
`fs-skia-template-update` skill was audited for currency. Results:

| # | Sev | Finding | Status vs. 059 / current source |
|---|-----|---------|----------------------------------|
| F1 | P1 | Evidence audit/graph audited the wrong feature dir (`generated-evidence-workflow`, 1 task) instead of the active feature (33 tasks). | **Fixed and merged by 059 (`ce9ba61`); template now pins `0.1.63-preview.1`.** `template/base/build.fsx` now has `resolveFeatureDir` (SPECKIT_FEATURE_DIR override → `.specify/feature.json` → **loud fail, no bundled-sample fallback**) and echoes `feature-directory=`/`tasks=`. The consumer used `0.1.62`, which predates this. The remaining work is **packing + installing the already-pinned `0.1.63-preview.1` packages to the local feed** and confirming the resolver behaves end-to-end in a freshly generated project — not merging 059 or re-bumping for the resolver's sake. |
| F2 | P1 | Documented `docs/api-surface/<Pkg>/<Pkg>.fsi` contract source does not exist in generated projects; all five capability skills claim "no DLL reflection needed". | **Open.** `template/base/docs/` still emits only `effects-boundary.md` + `product.md`; no `api-surface/` tree. All five `template/product-skills/*/SKILL.md` still point at the missing path. |
| F3 | P2 | Consumer geometry records (`Vec2`) collide with framework `Point`/`Rect`; F# label resolution produces misleading cascades. | **Open.** No "common pitfalls" note in the scene skill. |
| F4 | P2 | Duplicate DU case names (`ViewerKey.Unknown` vs `ViewerRunBlockedStage.Unknown`) across co-opened modules mis-bind. | **Open.** |
| F5 | P2 | Generated `tests/Product.Tests/Tests.fs` entangles durable governance scans with replaceable scaffold-behavior tests in one compilation unit. | **Open.** Still a single `Tests.fs`. |
| F6 | P2 | Testable success criteria (e.g. first-frame ship visibility) have no enforcing assertion; a headline SC can be violated while every gate is green. | **Open.** |
| F7 | P3 | Interacting requirements (entity-bound vs. per-wave escalation) left to implementer judgment. | **Open** (spec-authoring guidance). |
| F8 | P3 | Layout-evidence governance nudged a specific HUD/gameplay geometry by trial-and-error rather than documenting the intended pattern. | **Open.** Note: 059 split `fs-skia-layout-evidence` → `fs-skia-layout-readability` + `fs-skia-evidence-mode`; the pattern doc belongs in `fs-skia-layout-readability`. |
| F9 | P2 | Capability skills are string-substituted shells; `fs-skia-keyboard-input` demonstrates a `Keyboard.init bindings` / `KeyboardEffect` reducer model the host does not use (`mapKey : ViewerKey -> bool -> Msg option`). | **Open.** The keyboard skill still shows `Keyboard.init bindings` alongside the real `mapKey` boundary, and depends on the same missing `.fsi` as F2. |

**Template-update skill audit (`fs-skia-template-update`).** The skill is
structurally sound (detect versions → bump props + `build.fsx` `#r` pin → bump
template fsproj → `TemplatePack` → install → instantiate & test `app`/`governed`),
but its **hardcoded package enumeration is stale** on two axes:

- **Phantom package:** step 5's local-feed verification loop still checks the
  bare Lib package `FS.Skia.UI.$v.nupkg` ("FS.Skia.UI (the Lib package) has no
  suffix"). Feature 053 deleted `src/Lib` and unpublished `FS.Skia.UI`; no such
  package is produced and the template props no longer pin it, so the check
  reports a permanent false `MISS (Lib)`.
- **Missing packages:** the skill enumerates "nine repo packages"
  (`Build Scene SkiaViewer Elmish KeyboardInput Layout Controls Controls.Elmish
  Testing`) but the repo now also packs `FS.Skia.UI.SkillSupport` (which the
  template props **do** pin) and `FS.Skia.UI.Input`. A maintainer following the
  loop would not verify `SkillSupport`/`Input` were packed into the local feed.

The conclusion to report to the user is: the template-update skill *works as
intended* in flow, but is **not current** — its package list must be regenerated
from the packable-project set so it cannot drift again.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Trustworthy merge gate in generated projects (Priority: P1)

A consumer implements a feature in a generated `FS.Skia.UI` project, marks tasks
`[X]`, and runs the evidence gate. They need the gate to audit **their** active
feature (resolved from `.specify/feature.json` or `SPECKIT_FEATURE_DIR`), report
the audited `feature-directory=` and `tasks=` prominently, and **fail loudly**
if the resolved feature is missing or has zero tasks — never silently pass
against a bundled placeholder.

**Independent test**: Generate a project from the shipped template, record a
feature with N>1 tasks in `.specify/feature.json`, run the evidence
graph/audit, and confirm the echoed `feature-directory`/`tasks` match the active
feature; then point it at a missing dir and confirm it fails loudly.

### User Story 2 - Authoritative API surface a consumer can actually read (Priority: P1)

A consumer needs the exact DU case arity/field order, record fields, and helper
signatures of the framework packages they consume, available **in the generated
project** as the skills promise, so API discovery does not require DLL
reflection.

**Independent test**: Generate a project, open the path each capability skill
names as the contract source, and confirm the signatures for the consumed
packages are present and accurate; confirm no skill claims a source that does
not exist.

### User Story 3 - Generated tests separate durable governance from replaceable behavior (Priority: P2)

A consumer replaces the scaffold model with their own. They need the durable
governance/source-structure scans to survive that swap, while the scaffold demo
behavior tests are clearly the replaceable part.

**Independent test**: Generate a project, replace the scaffold model, and confirm
the governance scans still compile and run while only the behavior tests need
rewriting.

### User Story 4 - Capability skills match the real host contract, with pitfalls called out (Priority: P2)

A consumer following a capability skill literally produces working, idiomatic
code: the keyboard example matches the `mapKey` boundary the host actually uses,
and each relevant skill warns about the known sharp edges (geometry-type
collisions, duplicate DU case names, the HUD/gameplay layout pattern).

**Independent test**: Follow each capability skill's example against a generated
project and confirm it compiles against the real host contract without resorting
to an unused abstraction; confirm the named pitfalls appear in the skills.

### User Story 5 - Template-update skill that cannot drift on package set (Priority: P2)

A maintainer refreshing the template after a version bump follows
`fs-skia-template-update` and its package enumeration exactly matches the current
packable-project set — no phantom Lib package, no missing `SkillSupport`/`Input`
— so the local-feed verification neither false-fails nor silently skips a
package.

**Independent test**: Run the skill's verification loop after a pack and confirm
every packable repo package is checked and present, with no entry for a
nonexistent package.

### Edge Cases

- `SPECKIT_FEATURE_DIR` set to a nonexistent path → loud failure naming the path.
- `.specify/feature.json` absent or empty `feature_directory` → loud failure
  directing the user to `/speckit.specify`.
- A new packable package is added later → the template-update skill's package
  list should be derived/checked from the packable-project set, not hand-listed,
  so it stays current.
- A capability skill's named contract path is renamed/moved → a check should fail
  if any skill points at a path the generated project does not emit.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The generated-project evidence graph/audit MUST resolve and audit
  the active feature directory (via `SPECKIT_FEATURE_DIR` override, else
  `.specify/feature.json`), MUST echo the audited `feature-directory=` and
  `tasks=`, and MUST fail loudly (never fall back to a bundled sample) when the
  feature is unresolved or has zero tasks. (Closes F1; verify 059's
  `resolveFeatureDir` end-to-end and **ship** it.)
- **FR-002**: Feature 059's template/library fixes (already merged, `ce9ba61`;
  template pinned at `0.1.63-preview.1`) MUST be **packed and installed to the
  local feed** so a freshly generated project exhibits the FR-001 behavior (the
  report's `0.1.62` predates it). A version bump is required only if the
  `0.1.63-preview.1` artifacts are not already published to the feed.
- **FR-003**: Generated projects MUST provide the authoritative API surface for
  consumed packages at the location capability skills name (emit
  `docs/api-surface/<Pkg>/<Pkg>.fsi` into generated projects, OR repoint skills
  at an in-project / packaged `.fsi`/ref source that actually exists). No skill
  may claim "no DLL reflection needed" while the named source is absent. (Closes F2.)
- **FR-004**: A check MUST fail when any capability skill references a contract
  source path that a generated project does not actually emit, so skill claims
  and generated output cannot drift apart.
- **FR-005**: Generated tests MUST separate durable model-agnostic governance
  scans from replaceable scaffold-behavior tests (e.g. `GovernanceTests.fs` +
  `BehaviorTests.fs`) so swapping the model does not break governance coverage.
  (Closes F5.)
- **FR-006**: The `fs-skia-keyboard-input` skill example MUST match the host
  contract the app profile actually uses (`mapKey : ViewerKey -> bool ->
  Msg option`) and MUST NOT present a contradicting `Keyboard.init bindings` /
  `KeyboardEffect` reducer flow as the consumer path. (Closes F9.)
- **FR-007**: The scene/keyboard capability skills MUST document the known
  collision pitfalls — consumer geometry records colliding with `Point`/`Rect`
  (with a conversion note), and duplicate DU case names across co-opened modules
  (`ViewerKey.Unknown` vs `ViewerRunBlockedStage.Unknown`, with the
  fully-qualified resolution) — as a "common pitfalls" note. (Closes F3, F4.)
- **FR-008**: The `fs-skia-layout-readability` skill MUST document the intended
  HUD/gameplay-region pattern (reserve a HUD band; confine or clamp gameplay
  bounds to the gameplay region; overdraw the HUD) so consumers reach it by
  design rather than gate-driven trial and error. (Closes F8.)
- **FR-009**: The `fs-skia-template-update` skill MUST enumerate the package set
  from the current packable-project set so it cannot drift: remove the phantom
  bare-Lib `FS.Skia.UI` package, add `FS.Skia.UI.SkillSupport` and
  `FS.Skia.UI.Input`, and correct the "nine repo packages" count and the step-5
  verification loop accordingly. (Closes the template-update currency gap.)
- **FR-010**: Where a generated-product success criterion is mechanically
  testable (first-frame content, no-overlap, determinism), the generated tasks
  template SHOULD require a corresponding assertion and the evidence audit SHOULD
  be able to map SC → test, so a headline SC cannot be silently violated while
  gates stay green. (Addresses F6.)
- **FR-011**: Spec-authoring guidance SHOULD note interacting/conflicting
  requirements explicitly (e.g. entity-count bound vs. per-wave difficulty
  escalation — "count may cap; difficulty continues via speed") so different
  implementers resolve them consistently. (Addresses F7.)
- **FR-012**: Because the `.agents` skill tree is canonical and `.claude` is
  generated, all skill edits MUST be made in `.agents/skills/**` and regenerated
  (`RefreshSurfaceBaselines`), keeping `SkillSyncCheck`/`TargetMetadataDrift`
  green.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package *identities* change. Package *contents* change
  if `docs/api-surface/*.fsi` is emitted from packaged `.fsi`/ref sources
  (FR-003). The template package version is bumped, packed, and installed
  (FR-002). Generated package **consumers** change (new generated docs, split
  test files). The `fs-skia-template-update` skill's package list is corrected to
  the real packable set including `SkillSupport`/`Input` (FR-009).
- **Public contract impact**: No framework `.fsi` *signatures* change; this
  feature *surfaces* existing signatures into generated projects (FR-003) and
  documents them. Capability-skill and template-update-skill content (consumer
  contract) changes (FR-003/006/007/008/009).
- **State workflow impact**: No interpreter/effects/command behavior change. The
  generated `build.fsx` evidence-runner *feature resolution* is verified/shipped,
  not redesigned (already implemented by 059).
- **Layout/rendering impact**: No rendering-engine change. The
  `fs-skia-layout-readability` skill documents the HUD/gameplay layout pattern
  (FR-008); no visual output of framework packages changes.
- **Evidence obligations**: Real evidence paths under
  `specs/060-asteroids-consumer-friction-followups/readiness/` — at minimum the
  Route-required artifacts for the escalated tier (target-metadata, agent-ready
  verdict, skill-loading-evidence, aggregate-hang-diagnostics), plus generated-
  project verification logs proving FR-001 (echoed `feature-directory`/`tasks`)
  and FR-003 (the api-surface path exists in a freshly generated project).
- **Unsupported scope**: No new game/demo is shipped; no new framework runtime
  capability, platform, release, or distribution change. F6/F7/F10/F11 are
  process/authoring guidance, not new executable gates beyond what FR-004/FR-010
  scope. F4's optional DU-case *rename* in framework source is out of scope
  unless trivially safe; the pitfalls note (FR-007) is the committed remedy.
- **Build-target impact**: `TemplateCheck` / `GeneratedProductCheck` /
  `TemplateDrift` likely change to cover emitted `docs/api-surface` and the split
  test files. A new check may be needed for FR-004 (skill-claimed contract path
  exists in generated output). `SkillSyncCheck` / `TargetMetadataDrift` /
  `SkillQualityCheck` must stay green after skill edits (FR-012).
  `GeneratedGuidanceCheck`, `EvidenceGraph`, `EvidenceAudit` run as usual. Final
  gate list is authoritatively determined by `./fake.sh build -t Route`.

## Success Criteria *(mandatory)*

- **SC-001**: In a project generated from the shipped template, the evidence
  graph/audit echoes a `feature-directory=` and `tasks=` matching the active
  feature (tasks > 1 when the feature has multiple tasks), and fails loudly for a
  missing/zero-task feature — demonstrated in a verification log. (F1)
- **SC-002**: Every contract-source path named by a capability skill exists in a
  freshly generated project; no skill claims "no DLL reflection needed" against a
  path that is absent. (F2/F9, FR-003/FR-004)
- **SC-003**: After replacing the scaffold model in a generated project, the
  governance test file still compiles and runs; only the behavior test file
  requires rewriting. (F5)
- **SC-004**: Following the `fs-skia-keyboard-input` skill example verbatim
  produces code that compiles against the real host contract without an unused
  reducer abstraction. (F9)
- **SC-005**: The scene/keyboard/layout-readability skills each contain the
  documented pitfalls/pattern (geometry collision + conversion, duplicate DU
  case + qualification, HUD/gameplay band). (F3/F4/F8)
- **SC-006**: The `fs-skia-template-update` skill's package enumeration exactly
  equals the current packable-project set — zero phantom packages, zero missing
  packages — verifiable by diffing the skill list against the packable `.fsproj`
  set. (Template-update currency)
- **SC-007**: All Route-printed gates for this change pass, including
  `SkillSyncCheck`/`TargetMetadataDrift` after `.agents` skill edits are
  regenerated into `.claude`, and `EvidenceAudit` returns `verdict=PASS` for
  `specs/060-asteroids-consumer-friction-followups`.

## Assumptions

- The report's `0.1.62-preview.1` observation of F1 reflects pre-059 packages;
  059's `resolveFeatureDir` is merged (`ce9ba61`) and the template already pins
  `0.1.63-preview.1`. This feature packs/installs and verifies it rather than
  re-implementing or re-merging it.
- "Authoritative API surface" (FR-003) is satisfied either by emitting
  `docs/api-surface/*.fsi` into generated projects or by repointing skills at an
  existing packaged `.fsi`/ref source; the planning phase selects the approach.
- Skill edits are made in the canonical `.agents/skills/**` tree and regenerated
  into `.claude` (053/058 precedent); this is enforced, not optional.
- F6 (SC→assertion mapping) and F7 (interacting-requirement notes) are addressed
  as authoring-template/guidance improvements, not as new hard merge gates,
  unless planning finds a low-cost executable check.
