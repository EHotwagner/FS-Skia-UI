# Feature Specification: Refresh live-path skill currency

**Feature Branch**: `104-refresh-live-path-skills`
**Created**: 2026-06-11
**Status**: Draft
**Input**: User description: "create specs to fix those problems, incorporate the improvements."

> **Origin.** This feature was opened from a skill-situation analysis that found the skill
> corpus structurally healthy (36 canonical `.agents/skills/**` + 7 `src/**/skill/` package
> skills; `.claude/skills/**` a byte-identical generated mirror; `SkillSyncCheck` and
> `SkillQualityCheck` both green) but **stale and partly inaccurate** relative to the
> just-merged live-path roadmap. The R1–R6 remediations — features **096** (runtime
> visual-state bridge), **097** (incremental partial re-layout), **098** (binding-aware
> recovery), **099** (live animation clock), **100** (general navigation keys), **101**
> (layout dirty-set guard), **102** (doc-narrowing reconciliation), and **103** (visual-state
> cross-fade) — all landed on `main`, but the skills that should teach that path were last
> authored before them.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Retained-render skill is current through feature 103 (Priority: P1)

An agent (or maintainer) opens the skill that documents the live retained render path to
understand its disposition before editing `RetainedRender`/`Reconcile`. Today
`fs-skia-reconciliation` freezes that disposition at **feature 091** ("the host loops no
longer rebuild the whole tree every frame") and tells the reader that "further work (E3 style,
E4 focus, virtualization) *builds atop* the wired path" — as if it were still future. In
reality `RetainedRender.step` has since accreted a **`LayoutResult` bounds cache +
`RemeasuredNodeCount`** (097/101), an **`AnimationClock` advanced from an injected host delta
with sample-on-paint compositing** (099), **prior-under-next cross-fade compositing** (103),
and a **runtime visual-state bridge** that stamps `applyRuntimeVisualState` pre-reconcile from
pointer/focus state (096). The reader must come away knowing the path's *current* shape, not
its state eight features ago.

**Independent test**: Read the skill cold and check every claim about the live render path
against `src/Controls/RetainedRender.fsi`, `Reconcile.fsi`, and the live host in
`src/Controls.Elmish/ControlsElmish.fs`. Every disposition statement resolves to a fact that
is true on `main` today; no statement implies 096–103 are still future work.

### User Story 2 - Controls package skill stops teaching superseded APIs (Priority: P2)

A consumer or agent reads `src/Controls/skill/SKILL.md` to learn focus traversal and visual
state. The **E4** section describes `Focus.route` as merely "classifies a delivered key
against the focused control" — the pre-100 shape — but feature 100 generalized routing into a
closed `NavIntent` (`ValueStep`/`SelectionMove`/`GridMove`) produced from a widened
`Focus.route` that takes a role and a `NavRange`. The **E3** section teaches the style-class
resolver but never names the public `deriveVisualState` / internal `applyRuntimeVisualState`
surface that feature 096 added as the documented entry point for runtime visual state. The
reader must learn the shipped surface, not a superseded one.

**Independent test**: Read E3 and E4 and check them against `src/Controls/Focus.fsi` and the
visual-state surface on `main`. `Focus.route`'s description matches its current signature and
the `NavIntent` model; the `deriveVisualState` entry point is named where a reader would look
for runtime visual state. No example references a signature that no longer exists.

### User Story 3 - The interactive host has discoverable skill coverage (Priority: P3)

The interactive controls host — `Controls.Elmish.runInteractiveApp` and the retained-identity
wiring at the interpreter edge — is where features 092–103 did the bulk of their work, yet the
`Controls.Elmish` package has **no `skill/` directory**. A reader looking for "how the live
host holds retained state, advances clocks, and stamps visual state per frame" finds host
mechanics scattered across the reconciliation skill, the Controls skill, and the
consumer-facing `fs-skia-viewer-host` skill, with no single home. The maintainer-facing host
seam must be discoverable as its own skill.

**Independent test**: A reader searching the skill corpus for the interactive host finds one
skill whose scope is the `Controls.Elmish` host seam, passing the `SkillQualityCheck` rubric,
generated into `.claude/skills/**` and listed in `skillist-reference.md`. It cross-links the
reconciliation and viewer-host skills rather than duplicating them.

### Edge cases

- **Pure-honesty discipline (precedent: feature 102).** This is a documentation-currency pass.
  It MUST NOT change any `.fsi` signature, runtime behavior, or test outcome; the only product
  changes are skill text and (US3) one new skill plus its generated mirror.
- **Generated mirror must not drift.** `.claude/skills/**` is generated from `.agents/skills/**`;
  every edited or added canonical skill must be regenerated so `SkillSyncCheck` stays green and
  the `.claude` copy is byte-identical.
- **New-skill naming collision.** A new `Controls.Elmish` skill id must not collide with an
  existing package or `.agents` skill id (the `fs-skia-viewer-host` precedent renamed to dodge a
  collision). The skillist registry must list the new id.
- **Rubric completeness.** Every edited/added skill must still satisfy all seven required
  `SkillQualityCheck` sections (Scope, Driven-library API, Runnable example, ≥2 research URLs,
  persistent-problem mandate, `[[related]]` links, Sources).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The skill that documents the live retained render path MUST present a disposition
  that is current as of feature 103 — explicitly covering the `LayoutResult` bounds cache and
  `RemeasuredNodeCount` (097/101), the `AnimationClock` with injected-delta advance and
  sample-on-paint compositing (099), prior-under-next cross-fade compositing (103), and the
  runtime visual-state bridge stamped pre-reconcile (096).
- **FR-002**: That skill MUST NOT contain any statement that frames features 096–103 as future
  or not-yet-shipped work (e.g. "further work builds atop the wired path"); superseded
  forward-looking language MUST be replaced with shipped-truth.
- **FR-003**: The Controls package skill's focus/traversal guidance MUST describe `Focus.route`
  as it ships after feature 100 — the role + `NavRange` inputs and the closed `NavIntent`
  (`ValueStep`/`SelectionMove`/`GridMove`) output — and MUST NOT describe only the pre-100
  "classifies a delivered key" behavior.
- **FR-004**: The Controls package skill's visual-state guidance MUST name the runtime
  visual-state entry point added in feature 096 — public `deriveVisualState` and the internal
  `applyRuntimeVisualState` arbitration — where a reader looks for runtime visual state.
- **FR-005**: A skill dedicated to the `Controls.Elmish` interactive host seam
  (`runInteractiveApp` and the retained-identity / clock / visual-state per-frame wiring) MUST
  exist in the canonical `.agents/skills/**` tree with a non-colliding id, and MUST be
  cross-linked from (and link back to) the reconciliation and viewer-host skills.
- **FR-006**: Every skill created or edited by this feature MUST satisfy all seven
  `SkillQualityCheck` rubric sections and MUST pass `SkillQualityCheck`.
- **FR-007**: The generated `.claude/skills/**` mirror MUST be regenerated so it is
  byte-identical to the canonical `.agents/skills/**` tree and `SkillSyncCheck` passes; the new
  US3 skill MUST appear in `skillist-reference.md`.
- **FR-008**: This feature MUST introduce zero change to any `.fsi` public/internal signature,
  zero runtime behavior change, and zero change to existing test outcomes; its product surface
  is skill documentation and governance-generated artifacts only.
- **FR-009**: Claims added to skills MUST be verifiable against the source on `main` at
  authoring time (signatures, module dispositions, feature numbers); a claim that cannot be
  traced to current source MUST NOT be written.

> Interacting / conflicting requirements: FR-001/FR-005 add substantial new content while
> FR-008 forbids behavior change — resolve by treating every addition as *describing existing
> shipped code*, never as motivating a code change. If accurately documenting the path would
> require a source edit, the source is out of scope for this feature and the skill states the
> current truth as-is.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This section is
> expected to name concrete packages, paths, build targets, and evidence artifacts.

- **Package impact**: None. US3's host skill is a **repo-local `.agents/skills/<id>` domain
  skill** (the `fs-skia-reconciliation` / `fs-skia-viewer-host` precedent), NOT a package
  `src/Controls.Elmish/skill/SKILL.md` capability skill — package skills are not mirrored into
  `.claude/skills/**`, so a package skill could not satisfy FR-007's `.claude`/`skillist`
  discoverability. No `src/**` library or package directory is touched; no package identity,
  contents, or version change. (Routine merge-time version bumps remain governed by
  `speckit-merge`, out of this feature's scope.)
- **Public contract impact**: None. No `.fsi` signatures, documented public APIs, sample
  contracts, or surface baselines change. Skill edits that touch `src/Controls/**` markdown may
  still route through the controls-public-surface rule (precedent: feature 102's comment-only
  edits escalated despite zero `.fsi` delta) — that is a routing fact, not a contract change.
- **State workflow impact**: None. No stateful workflow, I/O, command, effect, subscription, or
  interpreter behavior changes; the host wiring is *described*, not modified.
- **Layout/rendering impact**: None. No layout, chart, DataGrid, rendering, screenshot, Vulkan,
  Skia, or visual output changes; the animation clock / cross-fade / layout cache behavior is
  *described*, not modified.
- **Evidence obligations**: Spec Kit readiness evidence under
  `specs/104-refresh-live-path-skills/` (tasks, evidence graph, evidence audit) plus the
  governance gate outputs that prove currency: `SkillQualityCheck`, `SkillSyncCheck`, and the
  `RefreshSurfaceBaselines` regeneration that updates `.claude/skills/**` and
  `skillist-reference.md`.
- **Unsupported scope**: No source/behavior changes; no `.fsi` edits; no new framework
  capability; no template changes; no migration of the full 36-skill corpus (only the
  live-path-relevant skills named here); no consumer-facing redesign of `fs-skia-viewer-host`
  beyond a cross-link.
- **Build-target impact**: `SkillQualityCheck` and `SkillSyncCheck` must pass; `RefreshSurfaceBaselines`
  must be run to regenerate the `.claude` mirror and skillist reference; `Route` determines the
  authoritative gate list for the change (expected to include the skill governance gates, and —
  per the 102 precedent — possibly the controls-public-surface set if `src/Controls/**` is
  touched). `EvidenceGraph`/`EvidenceAudit` apply per the standard readiness path.

## Success Criteria *(mandatory)*

- **SC-001**: A reader of the retained-render skill can correctly state the live path's current
  shape — layout bounds cache, `RemeasuredNodeCount`, animation clock, cross-fade compositing,
  and the runtime visual-state bridge — without consulting any source file, and finds no claim
  that 096–103 are unshipped.
- **SC-002**: The Controls skill's E3/E4 guidance matches the shipped `Focus.route`/`NavIntent`
  and `deriveVisualState` surface; zero examples reference a superseded signature.
- **SC-003**: A reader searching the corpus for the interactive host finds exactly one
  dedicated `Controls.Elmish` host skill; it passes the rubric and appears in
  `skillist-reference.md`.
- **SC-004**: `SkillQualityCheck` and `SkillSyncCheck` pass; `.claude/skills/**` is
  byte-identical to `.agents/skills/**`.
- **SC-005**: The feature introduces zero `.fsi` delta and zero test-outcome change (verifiable
  by diff and by an unchanged test run), matching the feature-102 pure-honesty precedent.

## Assumptions

- **A1**: US1 is satisfied by **refreshing the existing `fs-skia-reconciliation` skill** to
  current disposition (and may, at the plan's discretion, factor the retained-render specifics
  into a clearly cross-linked section or sibling), rather than by deleting it. The spec
  constrains *currency and accuracy*, not file layout.
- **A2**: The US3 host skill is **maintainer-facing** (host wiring, per-frame state) and
  distinct from the **consumer-facing** `fs-skia-viewer-host`; the two cross-link rather than
  merge.
- **A3**: Lower-value packages without skills (`Color`, `Input`, `SkillSupport`) are
  intentionally **out of scope** — they are leaf/internal and were not implicated by the
  live-path roadmap.
- **A4**: "Current as of feature 103" means the disposition on `main` at authoring time;
  later features may supersede it and are not this feature's concern.
