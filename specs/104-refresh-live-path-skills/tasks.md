# Tasks: Refresh live-path skill currency

**Feature branch**: `104-refresh-live-path-skills`
**Spec**: `specs/104-refresh-live-path-skills/spec.md`
**Plan**: `specs/104-refresh-live-path-skills/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. **This feature expects zero `[S]`/`[S*]`/`[SEH]` rows** — it is a
pure documentation-currency (skill-honesty) pass that introduces no synthetic
evidence; `EvidenceAudit` MUST report **0 synthetic** (feature-102 precedent).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase). FAKE-backed gate
  tasks are never `[P]`: `./fake.sh` shares `.fake` state and MUST run
  sequentially in the deterministic order printed below.
- **[US1]**, **[US2]**, **[US3]** — user-story scope.
- **[T2]** — Tier 2 (internal/documentation) change. Per the plan's Constitution
  Check, feature 104 is wholly Tier 2: no public/internal `.fsi` surface is
  added, removed, or modified; the product surface is skill documentation +
  governance-generated artifacts only (FR-008).

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors its
structured `skillist` via `[skillist: ...]` (`[skillist: []]` when empty).

## Pure-honesty discipline (load-bearing — FR-008 / SC-005)

This is a documentation-currency pass: it MUST NOT change any `.fsi` signature,
runtime behavior, or test outcome. Every skill addition *describes existing
shipped code* (features 096–103 on `main`); none motivates a source edit. Every
claim is anchored to a verified source signature in
`contracts/currency-claims.md` (C1/C2/C3) — a claim that cannot be traced to
current source MUST NOT be written (FR-009). The only `src/**` file touched is
`src/Controls/skill/SKILL.md` (Markdown); the only canonical hand-authored
inputs are the three skill files. `.claude/skills/**` and `skillist-reference.md`
are **generated, never hand-edited** (D4) — regenerate via
`RefreshSurfaceBaselines`.

## Routing reality (read before validating)

Run `./fake.sh build -t Route` first and run **only** the gates it prints.
Feature 101/102 established that **any edit under `src/Controls/**` — even a
pure-Markdown skill edit — can escalate `Route` to the `controls-public-surface`
gate set**, regardless of whether a `.fsi` changes. US2 edits
`src/Controls/skill/SKILL.md`, so **expect escalation**. No public-surface
baseline recapture is required (zero `.fsi` signature moves). FAKE-backed
commands run **sequentially** in this order when escalated:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

The skill-currency gates `SkillQualityCheck` (7-section rubric) and
`SkillSyncCheck` (`.agents`↔`.claude` byte-identity) are the dedicated proof of
this change; run them per the printed `Route` list.

## Governance risk levels

- **Small**: the `.agents/skills/fs-skia-reconciliation` refresh (US1) and the
  NEW `.agents/skills/fs-skia-controls-host` skill (US3) — `.agents`-domain
  Markdown, routed per `Route` to the skill governance gates
  (`SkillQualityCheck`, `SkillSyncCheck`).
- **Medium / broad**: the `src/Controls/skill/SKILL.md` edit (US2), which may
  escalate to the controls-public-surface set (feature 101/102 rule). Focused
  validation is the escalated set above. Broad validation is required only if
  `Route --enforce` names a missing evidence artifact. Non-authoritative
  aggregate results (e.g. a `GeneratedProductCheck` environment-class failure)
  are recorded as environment-class, not product defects, in `readiness/`.

---

## Phase 1: Setup

- [X] T001 [T2] [skillist: []] Confirm the feature directory links spec + plan and that the three skill targets exist where the plan's "Artifacts touched" table names them (`.agents/skills/fs-skia-reconciliation/SKILL.md` present, `src/Controls/skill/SKILL.md` present, `.agents/skills/fs-skia-controls-host/` absent = to-create) — verified: reconciliation + Controls skills present, `fs-skia-controls-host` absent (to-create)
- [X] T002 [P] [T2] [skillist: []] Scaffold `specs/104-refresh-live-path-skills/readiness/` audit-enforced placeholders discoverable before implementation: `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, `window-visibility.md` (not-applicable — non-visual skill docs, no screenshots/window launch) + the full window-visibility satellite set (`interactive-visible-window.md`, `close-reason-separation.md`, `window-state-diagnostics.md`, `window-options.md`, `real-image-evidence.md`), `skill-loading-evidence.md`, `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [T2] [skillist: []] Record feature classification: Tier 2 (internal/documentation), affected artifacts = `.agents/skills/fs-skia-reconciliation` + `src/Controls/skill` + NEW `.agents/skills/fs-skia-controls-host` + generated `.claude/skills/**` & `skillist-reference.md`, public-API impact = none (zero `.fsi` delta, FR-008), **Principle IV (MVU/effect) is not applicable** (no `Model`/`Msg`/`Effect`/`update` added or altered — the host's MVU boundary is *documented*, not modified), and evidence obligations = `SkillQualityCheck`/`SkillSyncCheck` green + `RefreshSurfaceBaselines` regeneration + routed gate set green + `EvidenceGraph`/`EvidenceAudit` with 0 synthetic — recorded in `readiness/governance-risk-levels.md` + `readiness/runtime-limitations.md`

---

## Phase 2: Foundation — verify anchors and decisions before authoring

- [X] T004 [P] [T2] [skillist: []] FR-009: verify every C1/C2/C3 source anchor in `contracts/currency-claims.md` is present on `main` at the cited lines — `src/Controls/RetainedRender.fsi` (`AnimationClock`, `LayoutResult` bounds cache, `RemeasuredNodeCount`, `sampleOnPaint`), `src/Controls/Focus.fsi` (`route(role, keyboard, navRange, key, isTab, shift)`, closed `NavIntent`), `src/Controls/ControlRuntime.fsi` (public `deriveVisualState`, internal `applyRuntimeVisualState`), `src/Controls.Elmish/ControlsElmish.fsi` (`runInteractiveApp`, `routeFocusedKey`, `retainedHitTest`) — record the verification in `research.md` (R7: all anchors present/current on `main`)
- [X] T005 [P] [T2] [skillist: []] Confirm the new id `fs-skia-controls-host` is free in BOTH the `.agents/skills/<id>` and the `src/*/skill` package-skill namespaces (single `SkillSyncCheck` namespace — the `fs-skia-viewer-host` rename precedent), so US3 introduces no collision; record the check in `research.md` (R8: free in both namespaces)
- [X] T006 [T2] [skillist: []] Confirm the four plan decisions hold against the working tree: D1 (refresh `fs-skia-reconciliation` in place, no new sibling), D2 (US3 is an `.agents` domain skill `fs-skia-controls-host`, not a `src/Controls.Elmish/skill` package skill — package skills are not mirrored into `.claude/**`), D3 (no constitution edit — its registry omits `fs-skia-reconciliation`/`fs-skia-viewer-host`), D4 (regenerate, never hand-edit, `.claude/**` + `skillist-reference.md`) — recorded in `research.md` R9

**Checkpoint**: Anchors verified, id free, decisions confirmed — skill authoring may begin.

---

## Phase 3: User Story 1 (US1) — reconciliation skill is current through feature 103

- [X] T007 [P] [US1] [T2] [skillist: fs-skia-reconciliation] FR-001: refresh `.agents/skills/fs-skia-reconciliation/SKILL.md` disposition to current-through-103, adding a "Live retained render path (096–103)" account covering C1 claims — status current through 103 not frozen at 091 (C1#1); `RetainedRender.step` threads a previous-frame `LayoutResult` bounds cache, unchanged subtrees reuse bounds (C1#2); `WorkReductionRecord.RemeasuredNodeCount` reports the post-propagation re-measure set, 097/101 (C1#3); per-identity `AnimationClock {Anim;Elapsed;Target;From}` advanced by an injected host delta (no wall-clock) and sampled on paint, settled/absent ⇒ byte-identical at rest, 099 (C1#4); the paint cross-fade is a two-snapshot composite — prior `From` fading out under next own-scene fading in via `sampleOnPaint`, not a `Color` tween, 103 (C1#5); runtime visual state is stamped pre-reconcile by `applyRuntimeVisualState`, `updateClockForState` decides start/retarget/advance/drop, 096 (C1#6)
- [X] T008 [US1] [T2] [skillist: fs-skia-reconciliation] FR-002: remove the stale forward-looking framing at `fs-skia-reconciliation/SKILL.md:33-35` ("further work (E3 style, E4 focus, virtualization) *builds atop* the wired path") and replace it with shipped-truth — those landed as 093/094 and 096–103; no statement may imply 096–103 are future/not-yet-shipped work
- [X] T009 [US1] [T2] [skillist: fs-skia-reconciliation] Preserve the 067 diff contract / operation-set / totality-determinism-identity-at-rest-round-trip invariants and the `module internal` disposition (zero public-surface delta); confirm all 7 `SkillQualityCheck` rubric sections survive; add the `[[fs-skia-controls-host]]` back-link to the Related section (FR-005 cross-link — reconciliation side)
- [X] T010 [US1] [T2] [skillist: fs-skia-reconciliation] US1 independent test (SC-001): read the refreshed skill cold and check every live-path disposition claim against `src/Controls/RetainedRender.fsi`, `Reconcile.fsi`, and the live host in `src/Controls.Elmish/ControlsElmish.fs`; confirm each statement is true on `main` today and that no claim frames 096–103 as unshipped; record the diff as the currency evidence

**Checkpoint**: The reconciliation skill maps the live path's current (103) shape.

---

## Phase 4: User Story 2 (US2) — Controls skill stops teaching superseded APIs

- [X] T011 [P] [US2] [T2] [skillist: fs-skia-ui-widgets] FR-004: in `src/Controls/skill/SKILL.md` E3, name the runtime visual-state entry point added in feature 096 — public `deriveVisualState model controlId : VisualState` (the closed precedence tail the resolver consumes, C2#1) and internal `applyRuntimeVisualState` which stamps the derived state pre-reconcile while consumers read state via `deriveVisualState` (C2#2) — placed where a reader looks for runtime visual state
- [X] T012 [P] [US2] [T2] [skillist: fs-skia-ui-widgets] FR-003: in `src/Controls/skill/SKILL.md` E4, describe `Focus.route` as it ships after feature 100 — inputs `role`, `keyboard`, `navRange`, `key`, `isTab`, `shift` returning `KeyRouting` (C2#3) and the closed `NavIntent` = `ValueStep of delta` | `SelectionMove of Direction` | `GridMove of rowDelta*colDelta` carried by `KeyRouting.Navigate` (C2#4); remove the pre-100 "classifies a delivered key against the focused control" prose at `:124-127` and its two-line example at `:129-132` (the `### E4` heading at `:122` stays)
- [X] T013 [US2] [T2] [skillist: fs-skia-ui-widgets] Confirm the E3/E4 edits stay within the existing E3/E4 headings, that no code example references a signature that no longer exists, and that the skill still passes all 7 `SkillQualityCheck` rubric sections (Sources/Related/mandate/examples retained)
- [X] T014 [US2] [T2] [skillist: fs-skia-ui-widgets] US2 independent test (SC-002): read E3 and E4 and check them against `src/Controls/Focus.fsi` and the visual-state surface (`ControlRuntime.fsi`) on `main` — `Focus.route`'s description matches its current signature and the `NavIntent` model, `deriveVisualState` is named where runtime visual state is taught, and zero examples reference a superseded signature

**Checkpoint**: The Controls skill's E3/E4 guidance matches the shipped surface.

---

## Phase 5: User Story 3 (US3) — the interactive host has its own skill

- [X] T015 [US3] [T2] [skillist: fs-skia-reconciliation, fs-skia-viewer-host] FR-005: create `.agents/skills/fs-skia-controls-host/SKILL.md` (id `fs-skia-controls-host`) covering the maintainer-facing `Controls.Elmish` interactive-host seam — C3 claims: `runInteractiveApp` live entry + host record `Init/Update/View/MapKey/MapPointer/Tick/Theme` (C3#1); host holds the `RetainedRender` structure in interpreter-edge ref state and produces each frame via `RetainedRender.step` carrying `StateByIdentity`/`Layout`/`Theme` (C3#2); `host.Tick` advances each identity's `AnimationClock` by the injected delta before render, sample-on-paint composites the cross-fade (C3#3); visual state assembled from pointer/focus and stamped via `applyRuntimeVisualState` pre-reconcile each frame (C3#4); key delivery via internal `routeFocusedKey` (E1 text seam → `Focus.route` activation/navigation/Tab → fallthrough to `host.MapKey`) (C3#5); pointer hit-testing via `retainedHitTest` resolving to a stable identity (C3#6)
- [X] T016 [US3] [T2] [skillist: fs-skia-viewer-host] FR-006: author all 7 `SkillQualityCheck` rubric sections in the new skill — Scope, Driven-library API, a runnable example, ≥2 research URLs, the persistent-problem mandate phrase "official online docs first" (one line), `[[related]]` links, and Sources — with cross-links `[[fs-skia-reconciliation]]` (the retained structure it drives), `[[fs-skia-viewer-host]]` (the consumer-facing counterpart), and `[[fs-skia-ui-widgets]]` (the controls it hosts)
- [X] T017 [US3] [T2] [skillist: fs-skia-viewer-host] FR-005 (viewer-host side): add the `[[fs-skia-controls-host]]` back-link to `.agents/skills/fs-skia-viewer-host/SKILL.md` Related section (the reconciliation back-link landed in T009), keeping the consumer-facing scope unchanged — a cross-link, not a redesign (spec A2)
- [X] T018 [US3] [T2] [skillist: fs-skia-viewer-host] US3 independent test (SC-003): confirm a reader searching the corpus for the interactive host finds exactly one dedicated `Controls.Elmish` host skill, distinct from the consumer-facing `fs-skia-viewer-host`, that passes the rubric and cross-links (rather than duplicates) the reconciliation and viewer-host skills

**Checkpoint**: The interactive host is discoverable as its own maintainer-facing skill.

---

## Phase 6: Integration, regeneration & evidence

- [X] T019 [T2] [skillist: []] D4 / FR-007: run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate `.claude/skills/**` byte-identical to `.agents/skills/**` (including the NEW `.claude/skills/fs-skia-controls-host/SKILL.md`) and `template/base/docs/skillist-reference.md` (registering the new `fs-skia-controls-host` id); never hand-edit `.claude/**`
- [X] T020 [T2] [skillist: []] Run `./fake.sh build -t Route` and record the printed tier + minimal gate list in `readiness/generated-validation.md` (expect the skill gates `SkillQualityCheck`/`SkillSyncCheck` and — because US2 edits `src/Controls/**` — possibly the escalated controls-public-surface set + `EvidenceGraph` + `EvidenceAudit`, per the 102 precedent); run only the gates it prints
- [X] T021 [T2] [skillist: []] Run the routed gate set **sequentially** (deterministic order, no concurrent FAKE); confirm `SkillQualityCheck` PASS for every in-scope skill (7 sections each), `SkillSyncCheck` reports no `.agents`↔`.claude` drift (mirror byte-identical), `Dev` is green, and the FR-008/SC-005 proof holds — `git diff --stat` shows zero `src/**/*.fsi` lines and the only `src/**` file touched is `src/Controls/skill/SKILL.md` (Markdown), with no product test file changed (SC-004, SC-005). A moved or edited test is a red flag that a skill token was parsed as a behavior change — investigate, do not accept
- [X] T022 [T2] [skillist: []] Record the governance risk level, the focused validation run for it, whether broad validation was required (`Route --enforce` named no missing artifact), and any non-authoritative aggregate result (e.g. a `GeneratedProductCheck` environment-class failure) in `readiness/governance-risk-levels.md` + `readiness/aggregate-hang-diagnostics.md`; confirm `readiness/window-visibility.md` records the non-applicable verdict (no window launch / no screenshots in a docs-only change)
- [X] T023 [T2] [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory`/`tasks=<n>` match this feature, no cycles, no dangling refs, no `[S*]` surprises; write `readiness/evidence-graph.md`
- [X] T024 [T2] [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with **0 synthetic**; write `readiness/evidence-audit.md` with a verdict token and ensure `readiness/generated-validation.md` records package-resolution=resolved / package-mismatch=false

---

## Synthetic-Evidence Inventory

This feature introduces **no** synthetic evidence. No `[S]`/`[S*]`/`[SEH]` task
exists; every skill claim is anchored to a real, verified source signature on
`main` (`contracts/currency-claims.md`). `EvidenceAudit` MUST report 0 synthetic.
This table is intentionally empty.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
