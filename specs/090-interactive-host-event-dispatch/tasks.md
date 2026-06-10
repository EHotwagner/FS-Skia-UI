# Tasks: Live Interactive Control Responsiveness — Authored Event-Binding Dispatch, Keyed-Ancestor Recovery, Text-Input Seam & a Responds-vs-Renders Proof

**Feature branch**: `090-interactive-host-event-dispatch`
**Spec**: `specs/090-interactive-host-event-dispatch/spec.md`
**Plan**: `specs/090-interactive-host-event-dispatch/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

`[S*]` is computed by the evidence audit from the DAG — never written by hand.
`[SEH]` annotates a design-approved synthetic error-handling task (none in 090 —
the plan declares **no synthetic evidence planned**; every test exercises the
real `routeInteractivePointer`/`nearestAuthored`/`TextInput.update` adapter path
and the responds-proof is a real before/after render diff).

## Change classification

**Escalated / `maintainer-verify` (Tier 1, public-contract).** New/changed public
`.fsi` in `src/Controls/**` (nearest-keyed-ancestor recovery) and
`src/Controls.Elmish/**` (binding-dispatch behavior + focus-aware text seam +
responds-proof + corrected host-contract doc) ship to consumers and emit into the
template api-surface. Run `./fake.sh build -t Route` (expected: escalate) and the
serialized six-target order. **FAKE-backed commands share `.fake` state — run them
sequentially.** Governance risk level: **broad** (public surface + evidence
obligation + skill-tree edit). `GeneratedProductCheck` env-failure is
non-authoritative (`generated-product-check-env-failure`) but still captured.

## Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- **[T1]** — Tier 1 (contracted) change (the whole feature is Tier 1)

Every task has a matching `tasks.deps.yml` entry; every line mirrors its
structured `skillist` as `[skillist: ...]` (`[skillist: []]` when empty).

## MVU/effect applicability (Principle IV)

In scope. `routeInteractivePointer` stays a pure
`state × size × model × input → state × 'msg list` step (interpreter at the
`runInteractiveApp` edge); the text seam routes through the existing pure
`TextInput.update` + `ControlRuntime.FocusedControl` state. No new effects,
subscriptions, or interpreter cases; `host.Update` folding is unchanged
(additive — no authored binding ⇒ identical behavior). Pure-transition tests +
emitted-`'msg` assertions + real-adapter evidence are required for `[X]` on the
`[US*]` tasks.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold `specs/090-interactive-host-event-dispatch/readiness/` and link spec + plan; record feature Tier 1 / escalated `maintainer-verify`
- [X] T002 [P] [skillist: []] Record Tier, affected layer (`src/Controls/**` + `src/Controls.Elmish/**` + the `.agents`/`.claude` evidence spine), public-`.fsi` impact, MVU applicability, and evidence obligations in `readiness/governance-risk-levels.md` (name small/medium/broad levels, the focused validation for **broad**, when broad validation is required, and how non-authoritative aggregate results are recorded). Note that **SC-001's "100% of catalog controls"** is a **host-mechanism guarantee** (the host dispatches any authored binding universally), proven on the representative sample per FR-005a — **not** a per-view audit of all 52 typed `Widgets/*.fs` views
- [X] T003 [P] [skillist: []] Create audit-enforced readiness placeholders discoverable before implementation — `readiness/visual-evidence-honesty.md`, `readiness/window-visibility.md` (honest "deferred — responds-proof is a headless render-target capture; no live Vulkan window required per plan"), `readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`, `readiness/generated-guidance-validation.md`, `readiness/real-image-evidence.md`, `readiness/skill-sync-check.md` — each naming the authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-ui-widgets, fs-skia-elmish] Draft the public surface `.fsi`-first (Principle I): `nearestAuthored : ControlRenderResult<'msg> -> ControlId -> ControlId option` in `src/Controls/Control.fsi` (FR-004/004a/005); the focus-aware text-routing seam field/function, the `captureRespondsProof` capture, and the binding-dispatch behavior note in `src/Controls.Elmish/ControlsElmish.fsi`; and the **corrected** host-contract doc replacing the false "`Layout.hitTestComputed` × `EventBindings`" claim (FR-002)
- [X] T005 [skillist: fs-skia-elmish] Exercise the draft `.fsi` from FSI (representative `nearestAuthored` + `routeInteractivePointer` paths the live host wires) and capture the transcript to `readiness/fsi-session.txt`
- [X] T006 [skillist: fs-skia-template-update] Record per-package surface baselines for the new/changed modules of `FS.Skia.UI.Controls` and `FS.Skia.UI.Controls.Elmish` (`PerPackageSurface.captureCurrent` — `RefreshSurfaceBaselines` alone does not regenerate `.fsi.txt`)
- [X] T007 [P] [skillist: fs-skia-evidence-mode] Record unsupported-scope handling + failure diagnostics in `readiness/runtime-limitations.md`: FR-004a `None`→`MapPointer` fallback (never invent a `Kind`/root id), FR-008a focus/tab-traversal & full editor UX deferred to E4, FR-005a scope (host **dispatch mechanism + representative sample only — no catalog-wide audit/retrofit of the 52 typed `Widgets/*.fs` views**; any per-view "exposes no binding" gap is flagged to a separate fitness pass, not fixed in 090), the non-authoritative `GeneratedProductCheck` env-failure, and render-target-only honesty for the responds-proof

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 2 — Container-keyed controls are routable (P1) [US2]

Recovery is the lower-layer (`src/Controls/**`) primitive the US1 dispatch join
consumes, so it lands first.

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US2] [skillist: fs-skia-ui-widgets, fs-skia-testing] Failing-first Expecto: a container-keyed composite hit on an inner positional node (`"0.1"`) resolves via `nearestAuthored` to the **container** id; a directly-keyed leaf resolves to itself; an unkeyed/unbound subtree returns `None` (FR-004, FR-004a, FR-005, R1/R2/R3) — compare returned `ControlId` strings (`Control<'msg>` has no equality)
- [X] T009 [P] [US2] [skillist: fs-skia-ui-widgets, fs-skia-testing] Failing-first property test: `nearestAuthored` of an already-authored id is that id (idempotent fixed point); total + deterministic over generated trees (R4)

### Implementation

- [X] T010 [US2] [skillist: fs-skia-ui-widgets] Implement `nearestAuthored` in `src/Controls/Control.fs` and export it in `Control.fsi` — re-derive the structural-path→node map (same `toLayout` `path + "." + index` scheme), ascend path-parents to the nearest node carrying a `Key` or non-empty authored `EventBindings`, return its authored `ControlId`, else `None` (FR-004, FR-004a, FR-005)
- [X] T011 [US2] [skillist: fs-skia-ui-widgets] Document the recovery's independent validation path (`contracts/recovery.md`) and confirm no layout-math change

**Checkpoint**: US2 recovery is independently testable.

---

## Phase 4: User Story 1 — Authored `onClick`/`onChanged` fire in the live window (P1) [US1]

### Tests First

- [X] T012 [P] [US1] [skillist: fs-skia-elmish, fs-skia-testing] Failing-first: a tree with one `onClick` and one `onChanged` routed through `routeInteractivePointer` (press+release at the bound control's bounds) dispatches the **bound** message and folds it, with **zero** `MapPointer` clauses authored; a competing `MapPointer` clause for the same control does **not** also fire (FR-001, FR-003, G1/G2 — no double-advance)
- [X] T013 [P] [US1] [skillist: fs-skia-elmish, fs-skia-testing] Failing-first: a control with **no** authored binding plus a `MapPointer` clause still routes via `MapPointer` exactly as today (additive / non-regressive, G3)
- [X] T014 [US1] [skillist: fs-skia-elmish] Implement the binding-dispatch join in `routeInteractivePointer`: per interaction recover the authored id (`nearestAuthored`), look up `rendered.EventBindings` by `(ControlId, eventKind)`, dispatch the bound message **xor** fall back to `MapPointer` for unconsumed interactions — preserving interaction order (FR-001, FR-003)
- [X] T015 [US1] [skillist: fs-skia-viewer-host] Correct the `ControlsElmish.fsi` host-contract doc so it accurately states whether/how authored `EventBindings` fire — no claim of dispatch the code does not perform (FR-002, G4, SC-002)
- [X] T016 [US1] [skillist: fs-skia-elmish] Document the US1 independent validation path (`contracts/host-dispatch.md`, `quickstart.md` author→host→click→verify loop)

**Checkpoint**: US1 dispatch is functional end-to-end on a leaf-keyed control.

---

## Phase 5: User Story 3 — An interactive story proves input→visible-change (P2) [US3]

### Tests First

- [X] T017 [P] [US3] [skillist: fs-skia-skiaviewer, fs-skia-testing] Failing-first: a responsive host (counter incremented by `onClick`) yields before ≠ after → `Responsive`; an inert host (binding dropped / pre-fix behavior) yields before = after → `Inert` and **fails** the proof (FR-006, P1/P3, SC-004)
- [X] T018 [P] [US3] [skillist: fs-skia-evidence-mode, fs-skia-testing] Failing-first governance test: the responds-vs-renders obligation text is present in the evidence skill tree and `.claude`↔`.agents` are byte-identical (FR-007, P4, SC-006)
- [X] T019 [US3] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Implement `captureRespondsProof`: render **before** → route + `host.Update` fold + repaint (as `SkiaViewer.fs:2469`) → render **after** → emit both frames + verdict (`Responsive`/`Inert`), reusing headless render-target capture (no live Vulkan window) (FR-006)
- [X] T020 [US3] [skillist: fs-skia-evidence-mode] Add the responds-vs-renders obligation to `.agents/skills/fs-skia-evidence-mode/SKILL.md` and regenerate the `.claude` mirror via `RefreshSurfaceBaselines` (`SkillSyncCheck` byte-identity; watch the trailing-newline drift) (FR-007)
- [X] T021 [US3] [skillist: fs-skia-skiaviewer, fs-skia-evidence-mode] Capture real responds-proof artifact pairs + verdict lines under `readiness/responds-proof/<case>/{before,after}.png` + `responds-proof.txt` on the running host for the two cases responsive by this phase — one **leaf-keyed `onClick`** (US1) and one **container-keyed composite** (US2, routed via `nearestAuthored`) — so US2 has a captured running-window proof, not only a harness test (the focused-text case is captured in Phase 6, T024); each distinct from a render-only screenshot and the offscreen route probe (`readiness/real-image-evidence.md`)

**Checkpoint**: the responds-proof distinguishes "renders" from "responds".

---

## Phase 6: User Story 4 — Text controls are typeable via a focus-aware seam (P3) [US4]

### Tests First

- [X] T022 [P] [US4] [skillist: fs-skia-keyboard-input, fs-skia-testing] Failing-first: set focus on a text control via a pointer click (focus-on-click path), deliver a keystroke through the focus-aware seam, and assert the character reaches the **focused** text control's `TextInput` model and **not** an unfocused one (FR-008, T1/T3)
- [X] T023 [US4] [skillist: fs-skia-keyboard-input, fs-skia-elmish] Implement the focus-aware text-routing seam: when `ControlRuntime.FocusedControl` names a focusable text control, deliver the keystroke/committed text to its `TextInput.update` and fold the product `'msg`, else fall through to the **unchanged** `MapKey` field (FR-008, no parallel text model)
- [X] T024 [US4] [skillist: fs-skia-viewer-host, fs-skia-skiaviewer, fs-skia-evidence-mode] Document the text seam in `ControlsElmish.fsi`/the published contract (no silent inertness; `MapKey` signature unchanged) and note the E4 scope guard (FR-008/008a, T4/T5); then capture the **focused-text running-window responds-proof** under `readiness/responds-proof/text/{before,after}.png` + `responds-proof.txt` (a keystroke to the focused control visibly changes the running host), completing the plan's leaf/container/text representative sample alongside T021

**Checkpoint**: a focused text control receives typed characters.

---

## Phase 7: Integration & Polish

- [X] T025 [skillist: fs-skia-template-update] Recapture per-package `.fsi.txt` for `FS.Skia.UI.Controls` + `FS.Skia.UI.Controls.Elmish` (`PerPackageSurface.captureCurrent`) and the published `template/base/docs/api-surface/**` via `RefreshSurfaceBaselines`; confirm currency (`TargetMetadataDrift`/`SkillSyncCheck`) (SC-006)
- [X] T026 [skillist: fsharp-build-orchestration] Run the serialized FAKE order sequentially: `Route` (expect escalate) → `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`; record the non-authoritative `GeneratedProductCheck` env-failure and capture logs (`readiness/aggregate-hang-diagnostics.md`); rerun affected commands sequentially if a failure looks race-like
- [X] T027 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and the echoed `feature-directory`/`tasks=<n>` match 090
- [X] T028 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm PASS (no `[S]`/`[S*]`, no diff-scan hits); document any `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

The plan declares **no synthetic evidence planned**; all tests use the real
adapter path and the responds-proof is a real render diff. If a genuine
error-path fixture arises it returns to design review for `[S]`/`[SEH]`
classification before use (`accepted-seh-stops-propagation`).

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
