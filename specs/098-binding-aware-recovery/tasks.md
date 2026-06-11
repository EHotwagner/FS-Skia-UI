# Tasks: Binding-Aware Ancestor Recovery (R3)

**Feature branch**: `098-binding-aware-recovery`
**Spec**: `specs/098-binding-aware-recovery/spec.md`
**Plan**: `specs/098-binding-aware-recovery/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` is an annotation for design-approved synthetic error-handling work; it
remains `[S]` when completed. **None planned for this feature.** R3 is a pure,
total, deterministic data correction over an already-computed
`ControlRenderResult`: `boundIdsOf`/`eventBindings`/`collectBoundsWith` are total
walks (an unbound/unkeyed node simply contributes no id, never throws), and
`nearestAuthored` returns `None` (host → `MapPointer`) when nothing on the path
is keyed or bound — there is no runtime error path to fixture. US1's headline
proof is a **real** dispatch through the live-adapter routing seam
(`routeInteractivePointer`); US3 uses FsCheck-**generated** trees. Any `[S]` that
appears triggers the full Principle V disclosure regime.

## Tier & MVU posture

This is a **Tier 1 (contracted) change** — uniform across all tasks, so per-task
`[T1]` marks are omitted. The public surface moves in two ways: (1)
`ControlRenderResult<'msg>` gains `BoundIds : Set<ControlId>` (`src/Controls/Types.fsi`,
mirrored in `Types.fs`), and `ControlInternals` gains `val boundIdsOf`
(`src/Controls/Control.fsi`); (2) the canonical `ControlId` **value** for **unkeyed**
controls changes from `Kind` to the structural path in the public `Bounds` list and
in the `ControlEvent.ControlId` payload (FR-007, an accepted/documented
canonicalization). `nearestAuthored`'s **signature is unchanged**
(`result -> hit -> ControlId option`); only its behavior widens. Surface-area
baselines (api-surface + per-package `.fsi.txt`) for `FS.Skia.UI.Controls` are
**recaptured** (SC-006). Because the public `src/Controls/**/*.fsi` changes, `Route`
**escalates** this change to the serialized six-target maintainer-verify path.

**MVU/Elmish is untouched.** R3 adds **no** `Model`/`Msg`/`Effect`/`Cmd`/`init`/`update`.
`nearestAuthored`, `boundIdsOf`, the binding collectors, and the unified id
derivation are **pure** functions of the already-computed render result /
`Control<'msg>` tree (no clock, no randomness, resume-safe). `bindingMessagesFor`
(`ControlsElmish.fs:155`) is unchanged except that the recovered id now matches its
`EventBindings` key under the unified scheme. `ControlRuntime`, `Pointer`, and the
focus seam are untouched. Principle IV is therefore **not applicable as a new
surface**; the dispatch evidence is the existing `routeInteractivePointer` routing
seam (a real pointer interaction resolving an authored `EventBindings` entry), not a
new transition to assert.

This is **not** a persistent graphical viewer feature. R3 changes id derivation and
recovery only; the `Scene`, `Layout`, and computed `Bounds` **rectangles** are
byte-identical — only the `ControlId` **labels** on unkeyed bounds change (FR-007).
Recorded as a visible decision in T003: the viewer-launch task-generation rule does
**not** apply (no persistent-launch / screenshot / `real-image` obligation). Proof is
the live-adapter routing seam plus structural/property tests, and the existing
responds-vs-renders proof primitive (E1) — an inert/un-fixed build cannot produce the
`us1-unkeyed-dispatch` artifact.

## Vertical-slice rule (US phases)

A `[US*]` task is `[X]` only when the user-reachable surface — an authored
`view` rendered through `renderTree` and routed through the **live-adapter**
`routeInteractivePointer` dispatch seam (the same seam `runInteractiveApp` wires)
— was actually exercised and dispatched. Passing unit tests on the pure helpers
alone do **not** satisfy `[X]`. Because the runtime model is untouched, the MVU
evidence for these stories is the read of the already-computed `EventBindings` +
`BoundIds` driving a real recovered dispatch on the live seam; no new transition is
introduced to assert.

## Success-criterion → assertion mapping

- **SC-001** (unkeyed authored `Button.onClick`, and a nested unkeyed bound control,
  dispatch in the live-adapter routing seam — an artifact an un-fixed build cannot
  produce) → T009 failing-first routing-seam dispatch test + T010 `nearestAuthored`
  widening (`us1-unkeyed-dispatch.md`).
- **SC-002** (keyed-leaf dispatch + container-keyed recovery byte-identical to 090;
  `MapPointer`-only consumers bit-for-bit unchanged) → T012 routing-seam non-regression
  + T013 `Control.dispatch` keyed regression + T006 unification keyed-branch identity
  (`us2-keyed-nonregression.md`).
- **SC-003** (a single canonical `ControlId` scheme `Key ?? path` spans `Bounds`,
  `EventBindings`, and recovery — all three agree for a node) → T016 single-scheme
  agreement test + T006/T007 unification (`surface-baseline.md` / `us3-...`).
- **SC-004** (two unkeyed same-kind bound siblings mint distinct ids and route only to
  their own bindings — property-tested for determinism + distinctness ≥1000 cases) →
  T015 FsCheck property suite (`us3-sibling-disambiguation.md`).
- **SC-005** (an unkeyed control with no bound/keyed ancestor recovers `None` →
  `MapPointer`, no spurious dispatch; a keyed leaf is a recovery fixed point) → T009
  AS3 + T010 (`fallback-and-mappointer.md`).
- **SC-006** (`ControlRenderResult` exposes `BoundIds : Set<ControlId>`; recovery reads
  it; `FS.Skia.UI.Controls` surface baseline recaptured for `BoundIds` + the
  canonical-id change) → T005 field + T020 recapture (`surface-baseline.md`).
- **SC-007** (the 092 retained focus path `resolveFocus`/`retainedHitTest`/`RetainedId`
  is not regressed) → T018 focus-nonregression (`focus-nonregression.md`).
- **SC-008** (the escalated six-target order is green with `EvidenceAudit` reporting no
  synthetic/stub work) → T021 first-four sequential + T022 graph + T023 audit
  (`validation-log.md` / `evidence-graph.md` / `evidence-audit.md`).

## Non-SC requirement traceability

- **FR-001** (one canonical `ControlId` per node, single scheme `Key ?? structural-path`;
  `Key ?? Kind` replaced in `eventBindings` `:194` and `collectBoundsWith` `:1332`) → T006.
- **FR-002** (`renderTree` **and** `render` emit `BoundIds : Set<ControlId>` in the same
  path scheme as `EventBindings`; `render.BoundIds` populated while `render.Bounds`
  stays `[]`) → T005 + T007.
- **FR-003** (`nearestAuthored` binding-aware: authored = keyed OR canonical id ∈
  `BoundIds`; returns nearest such ancestor; `None` only when none qualifies; keyed leaf
  fixed point) → T010.
- **FR-004** (`bindingMessagesFor` looks up the recovered id under the unified scheme;
  binding wins; `MapPointer` only on `None`/no-match; no double-dispatch) → T011.
- **FR-005** (all E1/090 dispatch non-regressive; `MapPointer`-only consumers
  bit-for-bit unchanged) → T012 + T013 + T014.
- **FR-006** (unified scheme disambiguates unkeyed same-kind siblings; property-tested
  for determinism + distinctness) → T015.
- **FR-007** (`ControlEvent.ControlId` / `Bounds` id for unkeyed controls may change
  `Kind → path`, documented; keyed unchanged; single consistent scheme) → T016 + T020.
- **FR-008** (scoped to the dispatch path; 092 retained focus path not altered/regressed)
  → T018.
- **FR-009** (additive, non-goal-preserving; no routed/bubbling/command/new-event-type;
  flat per-`ControlId` bindings; permanent non-goals) → T003 + T007 + T008.

## Governance risk levels

- **Small** — the pure `nearestAuthored` widening (one-predicate change) and the
  `boundIdsOf` derivation: focused validation is `Dev` + the targeted `Controls.Tests`
  recovery / single-scheme / FsCheck-distinctness suites.
- **Medium** — the id-scheme unification (`eventBindings`/`collectBoundsWith`/
  `Control.dispatch` path-threading), the `RetainedRender` `BoundIds` population, and
  the live-adapter dispatch seam: `Dev` + the `Controls.Tests` keyed-regression and the
  `Controls.Elmish.Tests` routing-seam dispatch / fallback suites.
- **Broad** — escalation **applies**: the public `src/Controls/**/*.fsi` change (the
  `BoundIds` field + `val boundIdsOf` + the canonical-id behavior) forces the serialized
  `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph
  → EvidenceAudit` maintainer-verify path. **`Route` is authoritative** — run
  `./fake.sh build -t Route` first and run exactly the gates it prints. FAKE-backed
  targets run **sequentially** (shared `.fake` state); aggregate results are recorded as
  **non-authoritative** unless re-confirmed sequentially.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- Every task has a matching `tasks.deps.yml` entry; every line mirrors the
  structured `skillist` via `[skillist: ...]`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory artifacts are present and linked (spec, plan, research, data-model, quickstart, `contracts/control-render-result.md`, `contracts/nearest-authored.md`, `checklists/requirements.md`) and that `.specify/feature.json` resolves `specs/098-binding-aware-recovery`
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Scaffold audit-discoverable readiness placeholders under `readiness/`: `us1-unkeyed-dispatch.md`, `us2-keyed-nonregression.md`, `us3-sibling-disambiguation.md`, `fallback-and-mappointer.md`, `focus-nonregression.md`, `surface-baseline.md`, `validation-log.md`, `fsi-transcript.md`, plus `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `real-image-evidence.md`, `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action (use `key=value` lines, not bare image-filename claims; `real-image-evidence.md` records **not-applicable**: framework-internal id/recovery change, no rendered-output/geometry change)
- [X] T003 [P] [skillist: []] Record feature Tier 1 (contracted: `ControlRenderResult.BoundIds` field + `val boundIdsOf` + the unkeyed canonical-id change in `Bounds`/`ControlEvent.ControlId`), affected layers (`FS.Skia.UI.Controls` — `Types.fsi`/`Types.fs` field, `Control.fs`/`Control.fsi` id-unification + `boundIdsOf` + `nearestAuthored` + `Control.dispatch`, `RetainedRender.fs` `BoundIds` population; `FS.Skia.UI.Controls.Elmish` — `bindingMessagesFor` verify-only), public-API impact (`BoundIds` field added, unkeyed canonical-id changed, `nearestAuthored` signature unchanged), MVU applicability (untouched — pure recovery; no new `Msg`/`Effect`/`update`), and the evidence obligations from the plan; record as a **visible decision** that this is **not** a persistent graphical viewer feature (framework-internal dispatch/id correction; `Scene`/`Layout`/`Bounds` rectangles byte-identical; proof is the `routeInteractivePointer` seam + property tests; no persistent-launch / screenshot / real-image obligation)
- [X] T004 [skillist: []] Run `./fake.sh build -t Route`; confirm the public `src/Controls/**/*.fsi` change **escalates** to the serialized six-target maintainer-verify path (`Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit`) and record the authoritative gate list plus the small/medium/broad governance risk levels into `readiness/governance-risk-levels.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-ui-widgets] Add the public `BoundIds : Set<ControlId>` field to `ControlRenderResult<'msg>` in `src/Controls/Types.fsi` (≈ line 345) and mirror it in `src/Controls/Types.fs` (≈ line 287); record the current `FS.Skia.UI.Controls` api-surface + per-package `.fsi.txt` baselines as the **pre-change reference** for the Phase 6 recapture (the canonical-id change and the new field move this baseline; SC-006/FR-002)
- [X] T006 [skillist: fs-skia-ui-widgets] Unify the canonical id scheme to `Key ?? structural-path` in one atomic, compiling change (FR-001, D1/D2/D5): make the internal `eventBindings` (`Control.fs:194`) path-aware — thread the `parent + "." + index` path `collectBoundsWith` already mints and derive `id = Key ?? path` (replacing `Key ?? Kind`); change `collectBoundsWith`'s emitted `controlId` (`:1332`) from `Key ?? Kind` to the `layoutId = Key ?? path` it already computes (`:1331`); thread the path into `Control.dispatch` (`:1480`) so its `event.ControlId = Some binding.ControlId` matching uses the unified scheme — eliminating the last residual `Key ?? Kind` derivation. The **keyed branch is byte-identical** (`Key` remains the id), so `InteractionTests.fs` keyed cases and the `event.ControlId = None` wildcard are unaffected; only the **unkeyed** fallback shifts `Kind → path`. `nearestAuthored` is **not** yet widened (still key-only) so the US1 failing-first test goes **RED**
- [X] T007 [skillist: fs-skia-ui-widgets, fs-skia-reconciliation] Add `ControlInternals.boundIdsOf : Control<'msg> -> Set<ControlId>` (a `go "0"` walk collecting `Key ?? path` for every node whose path-aware `eventBindings` is non-empty) and its `val boundIdsOf` in the `Control.fsi` `ControlInternals` block; populate `BoundIds` from this single source at all four `ControlRenderResult` construction sites — `Control.render` (`:1385`), `Control.renderTree` (`:1409`), and both `RetainedRender.fs` frames (`:118` first frame, `:374` subsequent) — so the retained path is byte-identical to the full rebuild by construction; `render.BoundIds` is **populated** (mirrors its populated `EventBindings`) while `render.Bounds` stays `[]` (FR-002, D3/D6)
- [X] T008 [P] [skillist: fs-skia-evidence-mode] Record unsupported-scope, permanent non-goals, and failure diagnostics into `readiness/runtime-limitations.md` (FR-008/FR-009): no routed/bubbling/tunneling event system, no command system, no new public event type, no framework-level focus-traversal change, no catalog-wide retrofit of all 52 typed views' bindings (separate fitness pass); the 092 retained focus path (`resolveFocus`/`retainedHitTest`/`RetainedId`) is **out of scope and must not regress** (FR-008); recovery is **total** (`None` → `MapPointer` when nothing on the path is keyed or bound — never a throw, never an invented id); the `disabledOrReadOnly` guard is preserved (a disabled bound node does not dispatch); the click-equivalent kinds stay the existing closed set (`click`/`changed`/`selected`); no data-binding/observable/dependency-property/selector/lookless-template surface (permanent non-goals)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — an unkeyed authored button responds in the live host

### Tests First (Principle I, Principle VI)

- [X] T009 [P] [US1] [skillist: fs-skia-elmish, fs-skia-testing] Add the failing-first routing-seam dispatch suite (`tests/Controls.Elmish.Tests`, fails against the un-widened `nearestAuthored`; SC-001/SC-005): (AS1) a view with a single **unkeyed** `Button.onClick` over its bounds — a press+release Click dispatches the authored message and `MapPointer` is **not** consulted; (AS2) a nested unkeyed bound control inside an **unbound, unkeyed** container — a Click on the inner control recovers the inner bound node and its binding dispatches; (AS3) an unkeyed **unbound** leaf with no bound/keyed ancestor — a Click recovers `None` and falls back to `MapPointer` exactly as 090 (no spurious dispatch)

### Implementation

- [X] T010 [US1] [skillist: fs-skia-ui-widgets] Widen `nearestAuthored` (`Control.fs:1459`) to be **binding-aware** (FR-003, D4): at each node on the hit path treat it as *authored* when `node.Id <> path` (keyed) **OR** `Set.contains node.Id result.BoundIds` (bound), and return the nearest such ancestor (including self); return `None` only when nothing on the path qualifies. `node.Id` is already `Key ?? path`, so it **is** the canonical id — a directly-keyed leaf stays a fixed point, and an unkeyed-bound node now returns `Some node.Id` (its path) where it returned `None` before; this is a one-predicate widening with no control-flow restructure
- [X] T011 [US1] [skillist: fs-skia-elmish, fs-skia-evidence-mode] Verify `bindingMessagesFor` (`ControlsElmish.fs:155`) resolves the unkeyed-bound case **for free** — the recovered id and the `EventBindings` keys now share the unified scheme, so the lookup matches; confirm the precedence is preserved (an authored binding **wins**; `MapPointer` is consulted **only** when recovery is `None` or no click-equivalent binding matches — never both, no double-dispatch; FR-004); capture US1 to `readiness/us1-unkeyed-dispatch.md` (real dispatch through the live-adapter `routeInteractivePointer` seam — an artifact an un-fixed build cannot produce) and the `None`-fallback half to `readiness/fallback-and-mappointer.md` (SC-001/SC-005)

**Checkpoint**: User Story 1 is functional and testable independently.

---

## Phase 4: User Story 2 (US2) — keyed and container-keyed dispatch remain non-regressive

### Tests First (Principle I)

- [X] T012 [P] [US2] [skillist: fs-skia-elmish, fs-skia-testing] Re-run the 090 representative dispatch cases through the R3 routing seam and assert **identical** dispatched messages and **identical** recovered ids (FR-005, SC-002, AS1–3): (AS1) a directly-keyed leaf with a binding — recovery resolves to its `Key` (a fixed point) and its binding dispatches, unchanged from 090; (AS2) a container-keyed composite — a Click on an inner **unkeyed, unbound** positional node climbs to the keyed container and dispatches the container's binding, unchanged from 090; (AS3) a control with both a `Key` **and** a binding — the binding is found by the unified id (the `Key`), with no double-dispatch; plus a `MapPointer`-only consumer (no authored bindings) is **bit-for-bit unchanged**
- [X] T013 [P] [US2] [skillist: fs-skia-ui-widgets, fs-skia-testing] Confirm the `Control.dispatch` keyed regression suite (`InteractionTests.fs` — the 8 keyed `"save-button"` cases + typed parity) stays **green unchanged** under the path-threaded `dispatch` (D5): the keyed branch is byte-identical and the `event.ControlId = None` wildcard path is unchanged; no test passes an unkeyed `Kind` id to `dispatch` today, so no payload regression for keyed `dispatch` consumers

### Implementation / Evidence

- [X] T014 [US2] [skillist: fs-skia-evidence-mode] Capture the non-regression evidence: `readiness/us2-keyed-nonregression.md` (keyed-leaf fixed point + container-keyed recovery byte-identical to 090, same dispatched messages + recovered ids, no double-dispatch for key+binding) and the `MapPointer`-only invariance half of `readiness/fallback-and-mappointer.md` (consumers with no authored bindings are bit-for-bit unchanged; binding-wins precedence preserved) (FR-005, SC-002)

**Checkpoint**: User Story 2 is functional and testable independently.

---

## Phase 5: User Story 3 (US3) — same-kind unkeyed siblings disambiguate by path

### Tests First (Principle I)

- [X] T015 [P] [US3] [skillist: fs-skia-ui-widgets, fs-skia-testing] Add the FsCheck property suite (`tests/Controls.Tests`, **≥1000** generated cases; FR-006, SC-004): **determinism** — `boundIdsOf`/`collectBoundsWith`/`eventBindingsOf` over the same tree produce identical results across runs; **same-kind-sibling distinctness** — any two distinct unkeyed same-kind nodes have distinct canonical ids (their structural paths `"0.0"`/`"0.1"`, never a single shared `Kind` id); plus a concrete two-unkeyed-bound-sibling routing case — a Click on the second dispatches the second's message and **not** the first's (no cross-routing)
- [X] T016 [P] [US3] [skillist: fs-skia-ui-widgets, fs-skia-testing] Add the single-canonical-scheme agreement test (SC-003, FR-007): for a laid-out node, the id in `Bounds`, the id in `EventBindings` (when bound), the `BoundIds` membership key, and the id `nearestAuthored` returns are **the same value** (no node reports `Kind` from one surface and `path` from another); assert `render.BoundIds` is **populated** from its bound nodes while `render.Bounds` stays `[]`

### Implementation / Evidence

- [X] T017 [US3] [skillist: fs-skia-evidence-mode] Write `readiness/us3-sibling-disambiguation.md`: two unkeyed same-kind bound siblings mint **distinct** structural ids and route only to their own bindings (no collision, no cross-routing), property-tested for determinism + same-kind-sibling distinctness across ≥1000 generated cases, and the single canonical scheme spans `Bounds`/`EventBindings`/`BoundIds`/recovery — read from the real suites, not assumed (SC-003/SC-004)

**Checkpoint**: User Story 3 is functional and testable independently.

---

## Phase 6: Integration & Polish

- [X] T018 [P] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Write `readiness/focus-nonregression.md` (FR-008/SC-007): the 092 retained focus path (`resolveFocus`/`RetainedRender.retainedHitTest`, returning a `RetainedId`) is **untouched and not regressed** — focus resolution behavior is identical (the `RetainedId` domain is separate from the `Layout.evaluate` + `nearestAuthored` + `EventBindings` dispatch seam R3 corrects); demonstrate via the existing 092 focus suite staying green
- [X] T019 [P] [skillist: fs-skia-ui-widgets] Exercise the unified scheme from FSI against the packed library per `quickstart.md` — author an unkeyed `Button.onClick`, confirm `renderTree` emits a populated `BoundIds` whose ids match its `EventBindings` keys in the `Key ?? path` scheme, and that `render.BoundIds` is populated while `render.Bounds` stays `[]` — and capture the session transcript to `readiness/fsi-transcript.md`
- [X] T020 [P] [skillist: fs-skia-ui-widgets] Recapture the `FS.Skia.UI.Controls` api-surface + per-package `.fsi.txt` baselines vs the T005 reference and confirm the diff shows exactly the `BoundIds` field, the `val boundIdsOf`, and the documented unkeyed canonical-id change (no other surface drift); record to `readiness/surface-baseline.md` (SC-006, FR-007)
- [X] T021 [skillist: fs-skia-testing] Run exactly the gates `Route` printed (T004) — the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck` prefix **sequentially** (shared `.fake` state, never concurrently) — and record the aggregate results as **non-authoritative** into `readiness/generated-guidance-validation.md` and the run transcript into `readiness/validation-log.md`; rerun any race-like FAKE failure sequentially before any product-regression claim (SC-008)
- [X] T022 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory` + `tasks=<n>` match this feature, no cycles, no dangling refs, no `[S*]` surprises; record to `readiness/evidence-graph.md`
- [X] T023 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (synthetic-propagation + diff-scan; no synthetic/stub work) or document every `--accept-synthetic` override; record to `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. **None planned**
— see the Status Legend rationale (a pure, total, deterministic id/recovery
correction with no runtime error path; `None` → `MapPointer` is a designed
fallback, not an error; real routing-seam dispatch, keyed-regression, FsCheck
distinctness, single-scheme agreement, and surface-baseline evidence). For any
`[SEH]` rows, include the approval label, design-phase source, synthetic input
class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
