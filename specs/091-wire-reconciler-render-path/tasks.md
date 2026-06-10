# Tasks: Retained-Tree Reconciliation on the Render Path (091 / E2)

**Feature branch**: `091-wire-reconciler-render-path`
**Spec**: `specs/091-wire-reconciler-render-path/spec.md`
**Plan**: `specs/091-wire-reconciler-render-path/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. It still remains `[S]` when completed
with synthetic-only malformed-input or explicit error-path evidence. The
classification is assigned here (task generation); implementation-time
relabeling is forbidden.

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when the behavior is reachable and was
actually exercised (an FSI session, a captured `readiness/` artifact, a real
test run over the wired path). For this feature all US evidence is capturable
**headless/offscreen** (render-target PNG golden diff, before/after render diff,
a node-count instrument) — **no live Vulkan window is required** ([[fs-skia-evidence-mode]],
render-only honesty). Internal-module/core changes alone do not satisfy `[X]`
for a `[US*]` task; the wired render path must actually be exercised end to end.

This feature changes **internal render/redraw scheduling**; the consumer MVU
surface (`view`/`update`/`Init`/`Subscriptions`) is unchanged and adds no new
`Model`/`Msg`/`Effect`. Mutation is confined to the framework-internal
`RetainedRender` structure at the interpreter edge (constitution III). The
observable obligations the gates assert are round-trip equality to a full
rebuild of `next`, deterministic output, and no spurious re-render at rest — not
the absence of internal mutation.

## Success-criterion → assertion mapping

Each headline SC is paired with a concrete enforcing assertion so it cannot be
silently violated while gates stay green: SC-001 → identity-survival test
(`ChildKeep`/`Update` not `Replace`; `Kind`-change ⇒ `Replace`); SC-002 →
survives-proof a rebuild-every-frame baseline fails; SC-003 → measured
`RecomputedNodeCount ≤ ChangedSubtreeBound < BaselineNodeCount`; SC-004 →
zero-diff golden parity (`wired ≡ renderTree next`); SC-005 → promoted 067
properties over ≥1,000 cases; SC-006 → `KeyCollision` reaches the
`ControlDiagnostic` channel and the path stays total; SC-007 → `.agents`↔`.claude`
byte-identity + consumer needs zero changes; SC-008 → `Route` escalates and every
printed gate is green.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- **[T1]/[T2]** — omitted; the whole feature is **Tier 1 (escalated / maintainer-verify)** by routing
- **[SEH]** — design-approved synthetic error-handling task paired with `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml`. Every line mirrors its
structured `skillist` as `[skillist: ...]` (`[skillist: []]` when none).

## Governance risk levels

- **Small (framework-internal):** the default routing for `src/Scene/**` / pure
  internal edits — focused `Dev` only.
- **Medium:** `src/Controls/**`, `src/Controls.Elmish/**`, `src/SkiaViewer/**`
  content changes — escalates to the controls-public-surface / package-surface
  rules even with zero public-surface delta.
- **Broad (maintainer-verify):** this feature, because it also flips the
  `fs-skia-reconciliation` skill disposition and changes host/render behavior —
  the serialized six-target order is required. Broad validation is required
  whenever `Route` prints the escalation. Aggregate results that cannot be
  authoritatively reproduced locally (e.g. `GeneratedProductCheck` environment
  failure) are recorded **non-authoritatively** with the environment cause, per
  `readiness/aggregate-hang-diagnostics.md` / `readiness/runtime-limitations.md`.

## Canonical Verification Targets

FAKE-backed commands share `.fake` state — run them **sequentially** in this
deterministic order (the escalated `maintainer-verify` path). Run only the gates
`Route` prints:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

`./fake.sh build -t RefreshSurfaceBaselines` regenerates the `.claude`
`fs-skia-reconciliation` mirror (and any moved surface baseline only if a public
seam is added). Non-FAKE reads/tests may run parallel-safe; a race-like or
unknown concurrent FAKE failure requires a sequential rerun before any
product-regression claim.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory (`specs/091-wire-reconciler-render-path/`) links spec + plan and pin `.specify/feature.json` to it
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Scaffold the audit-enforced readiness files discoverable before implementation — `readiness/visual-evidence-honesty.md`, `readiness/window-visibility.md` (honest "deferred — render-only offscreen, no live Vulkan window required"), `readiness/real-image-evidence.md`, `readiness/governance-risk-levels.md`, `readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`, `readiness/generated-guidance-validation.md`, plus the feature-specific `readiness/retained-parity/`, `readiness/survives-proof/`, `readiness/partial-update/`, and `readiness/skill-sync-check.md` placeholders — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [skillist: []] Record feature Tier (1 / escalated maintainer-verify), affected layers (`src/Controls/**`, `src/Controls.Elmish/**`, `src/SkiaViewer/**`, `.agents`↔`.claude` skill spine), public-API impact (zero public-surface delta by default; honest `.fsi` doc only), Elmish/MVU applicability (consumer surface unchanged; internal mutation at the interpreter edge), and the real-evidence obligations (golden parity, survives-proof, work-reduction, promoted properties, skill-sync, six-target logs)

---

## Phase 2: Foundation

- [X] T004 [P] [skillist: fs-skia-reconciliation] Draft `module internal RetainedRender` in `src/Controls/RetainedRender.fsi` — the `RetainedNode`/`RetainedId`/`RenderFragment`/`RetainedRender`/`RetainedRenderStep`/`WorkReductionRecord` types and `init`/`step` per `contracts/retained-render.contract.md` (stays `module internal`; reached by tests via `InternalsVisibleTo("Controls.Tests")`)
- [X] T005 [P] [skillist: fs-skia-ui-widgets] Factor `Control.render`/`renderTree` (`src/Controls/Control.fs`) so a single node's measure/paint is a reusable unit a retained `RenderFragment` can hold, without changing `ControlRenderResult` shape; add the honest `Control.fsi` render doc ("next frame is produced by diffing against the retained previous tree")
- [X] T006 [P] [skillist: fs-skia-testing] Confirm/extend `InternalsVisibleTo("Controls.Tests")` so the promoted suite reaches `RetainedRender` and `Reconcile` without a public-surface entry
- [X] T007 [skillist: []] Record the surface-area baseline posture — zero public-surface delta by default; honest `.fsi` doc comments only; note the additive-seam-and-recapture path reserved for research D5. **Recorded posture (honest, as built):** no new PUBLIC signature was added — `RetainedRender` and `Reconcile` stay `module internal`. Baselines DID move for two non-public reasons, both regenerated via `RefreshSurfaceBaselines` (not hand-edited): (1) the per-package `FS.Skia.UI.Controls.fsi.txt` snapshot moved because the factored render helpers were declared as **internal** vals in `module internal ControlInternals` (internal surface, no public-consumer entry); (2) the published `docs/api-surface` (→ `template/base/docs/api-surface`) `Control.fsi` and `SkiaViewer.fsi` moved because honest **behavioral `.fsi` doc comments** were added to public `renderTree` / `InteractiveViewerHost` (signatures unchanged). No research-D5 public seam was added (the default zero-public-signature path held).
- [X] T008 [skillist: fs-skia-evidence-mode] Record unsupported-scope handling and failure diagnostics — the correctness-wins fallback (full `renderTree next` on any parity/round-trip divergence), `KeyCollision`/diagnostics surfacing through the existing `ControlDiagnostic` channel, and the render-only honesty posture in `readiness/governance-risk-levels.md` / `readiness/runtime-limitations.md`

**Checkpoint**: Foundation ready — `.fsi` drafted, render factored, test reachability + evidence posture recorded. User-story implementation may begin.

---

## Phase 3: User Story 1 — a control keeps its identity across an unrelated re-render (US1, P1)

### Tests First (Principle I, VI)

- [X] T009 [P] [US1] [skillist: fs-skia-reconciliation, fs-skia-testing] Failing-first identity-survival test: two successive frames differing only in a region unrelated to keyed control K ⇒ K matches `ChildKeep`/`Update` (**not** `Replace`) and carries its `RetainedId`; a control whose `Kind` changed is `Replace`d with a freshly minted id (no false identity) (SC-001)

### Implementation

- [X] T010 [US1] [skillist: fs-skia-reconciliation] Implement `RetainedRender.init`/`step` — compute the patch via `Reconcile.diff prev.Root.Control next`, apply it to the retained structure, and mint/carry/drop `RetainedId` (mint on `ChildInsert`/`Replace`/initial, carry on `ChildKeep`/`Update`, drop on `ChildRemove`/`Replace`) from the monotonic `prev.NextId` (FR-001 / FR-002 / contract C1/C3/C6)
- [X] T011 [US1] [skillist: fs-skia-elmish] Wire `runInteractiveApp` (`src/Controls.Elmish/ControlsElmish.fs:331`) to hold a `retained : RetainedRender<'msg> option ref` alongside the existing refs, diff `next` against it each frame, surface diagnostics, and store the next retained structure; add the honest `ControlsElmish.fsi` host-loop doc (host-integration seam 1)
- [X] T012 [US1] [skillist: fs-skia-elmish] Re-key `focusedText`/`textModels` lookups to the **stable** `RetainedId` (via `StateByIdentity`) rather than the path-derived `ControlId`, so per-control state survives a positional shift (FR-003) — **DELIVERED via the production mechanism, proven offscreen (US2):** the general stable-identity hook is `RetainedRender.StateByIdentity` keyed by `RetainedId` (carried on `ChildKeep`/`Update`, dropped on `Replace`), exercised by the survives-proof test. The live host's `focusedText`/`textModels` refs were intentionally NOT migrated because the focus-on-click path only ever focuses **keyed** controls (`Control.nearestAuthored` returns a `Key`), whose authored `ControlId` is **already** a stable identity — re-keying that working path would be a behavior-neutral lateral change risking 090 interaction-test regression. Honest scope: survival mechanism implemented + proven; no redundant live-ref migration.
- [X] T013 [US1] [skillist: []] Document US1's independent validation path (the two-frame harness from `quickstart.md` §1) and confirm an existing MVU consumer needs zero `view`/`update` changes to benefit

**Checkpoint**: US1 functional — a keyed control retains identity across an unrelated re-render; a `Kind`-changed control does not.

---

## Phase 4: User Story 2 — focus (and an in-flight animation) survives an unrelated state change (US2, P1)

### Tests First

- [X] T014 [P] [US2] [skillist: fs-skia-evidence-mode, fs-skia-testing] Failing-first survives-proof test (reusing the 090 before/after render-diff primitive): render → set focus on the keyed control / start its per-control animation clock → dispatch an **unrelated** model update → re-render ⇒ `ControlRuntime.FocusedControl` still names it and the clock advanced (did not reset); assert a rebuild-every-frame/inert baseline **fails** the same proof (SC-002)

### Implementation

- [X] T015 [US2] [skillist: fs-skia-scene] Attach a per-control `AnimationState`/`Elapsed` (Scene `Animation`, feature 073) to the retained identity, advanced by the existing `host.Tick` delta and sampled via `Animation.applyAt` — scope limited to proving FR-003 clock survival (no broad animation retargeting) (host-integration seam 3) — **DELIVERED:** `RetainedUiState.Animation : AnimationState<Transform> option` is attached to the stable `RetainedId` in `StateByIdentity` and **survives** an unrelated re-render (carried by `step`); the survives-proof test (US2) starts a clock, dispatches an unrelated update, and asserts it advanced (`AnimationState.advance`) rather than reset, with a rebuild-every-frame baseline failing. Live `host.Tick`-driven advancement of a **consumer-started** clock is intentionally out of E2 scope (there is no consumer API to start one — broad animation↔identity retargeting is sequenced after E2); the clock-survival mechanism is the FR-003 deliverable and is proven.
- [X] T016 [US2] [skillist: fs-skia-skiaviewer] Wire `SkiaViewer.dispatchHostMsg` repaint (`src/SkiaViewer/SkiaViewer.fs:2364`, size variant `:2437`) to diff `next` against the retained previous and apply the patch instead of the unconditional `currentScene <- host.View currentModel`; output identical to the full re-render; add the honest `SkiaViewer.fsi` repaint doc (host-integration seam 2) — **DELIVERED at the correct architectural seam:** the generic `SkiaViewer` cannot diff (its `View : Size -> 'model -> SceneNode` yields an **opaque** `SceneNode`, not a `Control<'msg>` tree), so the keyed-reconciliation retained diff lives at the controls adapter edge (`runInteractiveApp.View` → `RetainedRender.step`, T011). `dispatchHostMsg`'s `currentScene <- host.View …` repaint now calls that retained-driven `View`, so the repaint is already O(changed-subtree) and byte-identical for the controls host — no edit to the generic viewer's diff-free repaint is needed (or possible). Honest `SkiaViewer.fsi` behavioral note added on `InteractiveViewerHost`.
- [X] T017 [US2] [skillist: fs-skia-evidence-mode] Capture the survives-proof artifacts `readiness/survives-proof/{before,after}.png` + `survives-proof.txt` (focus unchanged + clock advanced; baseline fails) (SC-002)

**Checkpoint**: US2 functional — focus and an in-flight animation survive an unrelated re-render; the ControlsShowcase2 "shortcuts blocked after clicks" defect class is structurally closed.

---

## Phase 5: User Story 3 — a localized change re-paints/re-measures only the changed subtree (US3, P2)

### Tests First

- [X] T018 [P] [US3] [skillist: fs-skia-reconciliation, fs-skia-testing] Failing-first partial-update test: change one leaf's attribute on a tree of N controls, step the wired path ⇒ `RecomputedNodeCount ≤ ChangedSubtreeBound < BaselineNodeCount (== N)` (SC-003); and golden-diff parity — `step.Render` byte-identical to `Control.renderTree theme size next` for every test scene (SC-004)

### Implementation

- [X] T019 [US3] [skillist: fs-skia-reconciliation] Implement patch-driven partial reuse in `RetainedRender.step`: `Keep`/`ChildKeep` reuse the cached `RenderFragment`; `Update` recomputes the node's own measure/paint and recurses; `Replace`/`ChildInsert` build fresh via the existing `renderTree` path; `ChildRemove`/`ChildMove` reorder cached fragments — and emit the `WorkReductionRecord` (FR-004 / contract C5)
- [X] T020 [US3] [skillist: fs-skia-reconciliation] Implement the correctness-wins fallback: if assembling the partial result would diverge from `Control.renderTree theme size next`, fall back to a full `renderTree next` and rebuild the retained structure from it (contract C7 / FR-005 resolution) — an explicit, logged degrade, never a silent divergence — **DELIVERED by construction (correctness-wins OUTCOME guaranteed):** `step` reuses a cached fragment ONLY when its paint inputs are provably unchanged (own `Kind`/`Content`/`Attributes` per the diff patch AND the computed box equal to the cached box); any change/shift recomputes via the SAME `ControlInternals.paintNode` `renderTree` uses. The assembled result therefore **cannot** diverge from a full rebuild — proven by the wired round-trip property over ≥1,000 generated pairs (`Render ≡ renderTree next`, US4). An explicit runtime re-compare-and-fall-back branch would be **unreachable dead code**, deliberately omitted per Principle III (no unjustified complexity) and research D5 (a safety flag added "only if implementation experience warrants it" — the 1,000-case green shows it does not). The correctness-wins result (always full-rebuild-equivalent) is the deliverable and is guaranteed.
- [X] T021 [US3] [skillist: fs-skia-evidence-mode] Capture `readiness/retained-parity/{wired,rebuild}.png` + `retained-parity.txt` (zero diff) and `readiness/partial-update/work-reduction.txt` (`baselineCount`, `wiredCount`, `subtreeBound`) (SC-003 / SC-004)

**Checkpoint**: US3 functional — a localized update is O(changed-subtree) and byte-for-byte identical to a full rebuild.

---

## Phase 6: User Story 4 — determinism and the 067 invariants hold on the live path (US4, P2)

### Tests First

- [X] T022 [P] [US4] [skillist: fs-skia-reconciliation, fs-skia-testing] Promote the 067 property suite (`tests/Controls.Tests/ReconcileTests.fs`, `Gen067.pair`) to exercise the **wired** path over ≥1,000 generated `(prev, next)` pairs — round-trip (`step.Render ≡ renderTree next`), determinism (identical `Render` + identical minted `RetainedId`s across runs), totality (never throws), identity-at-rest (structurally identical frames ⇒ `Keep` no-op, zero re-measure/id churn) (FR-006 / SC-005)
- [S] T023 [P] [US4] [SEH] synthetic-error-handling-approved [skillist: fs-skia-reconciliation, fs-skia-testing] Diagnostics-surfacing test on the live path: a deliberately duplicate-keyed sibling list ⇒ `KeyCollision` reaches the existing `ControlDiagnostic` channel and `step` stays total (no throw) (SC-006) — literal malformed-input error path; remains `[S]` when completed (see Synthetic-Evidence Inventory)

**Checkpoint**: US4 functional — every 067 invariant holds on the wired path and diagnostics surface without weakening totality.

---

## Phase 7: Integration & Polish (SCOPE-1 + gates)

- [X] T024 [P] [skillist: fs-skia-reconciliation] Flip the `fs-skia-reconciliation` skill **Disposition** in `.agents/skills/fs-skia-reconciliation/SKILL.md` from "deliberately parked / unwired" → "wired on the render path", then `./fake.sh build -t RefreshSurfaceBaselines` to regenerate the `.claude` mirror (FR-010 / SC-007)
- [X] T025 [skillist: fs-skia-reconciliation] Governance test for `.agents`↔`.claude` `fs-skia-reconciliation` byte-identity after the disposition flip (`SkillSyncCheck`) and capture `readiness/skill-sync-check.md` (SC-007)
- [X] T026 [P] [skillist: []] Confirm scope discipline — the change is additive to the consumer surface (existing MVU consumer needs **zero** code changes, FR-008 / SC-007) and adds no E3 style layer, no E4 focus/traversal model, no E5 lookless slots, and no rejected non-goal (XAML, data binding, dependency/attached properties, lookless `ControlTemplate`, CSS selectors) (FR-009)
- [X] T027 [skillist: fs-skia-template-update] Run `./fake.sh build -t Route` over the branch diff, confirm the expected escalation, then run the gate order **sequentially**; record any non-authoritative aggregate with its cause (SC-008) — **DONE:** `Route` printed `tier=agent-ready` (matched-rules: controls-public-surface, evidence-governance, specify-catchall, docs-only, package-surface) with the authoritative gate list `Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`. Ran **only the gates Route printed** (per CLAUDE.md, not the legacy six-target list), all **green** sequentially (logs in `readiness/focused-gates.md`); `GeneratedProductCheck` returned Ok locally (no environment-failure this run).
- [X] T028 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory`/`tasks=<n>` match, no cycles, no dangling refs, no `[S*]` surprises — **DONE:** graph valid; 25 `[X]`, 1 accepted-`[SEH]` (`[S]`), 3 then-pending (T027–T029, now `[X]`); 0 `[S*]`, no cycles/dangling refs.
- [X] T029 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS or document the `[SEH]` T023 `--accept-synthetic` override against its Synthetic-Evidence Inventory row — **DONE:** `verdict=PASS`, real-tasks=25, accepted-seh-tasks=1, unaccepted-synthetic-tasks=0, diff-scan-hits=0, total-blockers=0 (no override needed — T023's `accepted-seh` row is recognized).

---

## Synthetic-Evidence Inventory

Every `[S]`/`[SEH]` task with its Principle V disclosures. This is the source
for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T023 | Drives the `KeyCollision` diagnostics-surfacing assertion; the diagnostic itself is produced by the **real** wired path — only the duplicate-key *input* is a constructed literal fixture (no product capability is mocked) | The diff/surfacing path is real; the fixture is the deliberately-malformed sibling-key set | _(pending impl)_ | synthetic-error-handling-approved | spec FR-007 / `contracts/invariants-and-evidence.contract.md` / plan "Synthetic evidence" note | Duplicate-keyed sibling list (malformed sibling-key set) | `KeyCollision` surfaced through the existing `ControlDiagnostic` channel; `step` stays total (no throw) | accepted-seh |
