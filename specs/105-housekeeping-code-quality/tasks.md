# Tasks: Housekeeping Code-Quality Remediation

**Feature branch**: `105-housekeeping-code-quality`
**Spec**: `specs/105-housekeeping-code-quality/spec.md`
**Plan**: `specs/105-housekeeping-code-quality/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/evidence-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. **None is anticipated** for this
behavior-preserving refactor — the parity assertion compares real lowering
output, with no mocks, fakes, or forced error fixtures.

## Tier & scope banner

**Tier 2 (internal change).** No public `.fsi` delta under the default choices
(D3–D6): the shared `WidgetLowering` module + `onChanged*`/`tryParseFloat`
helpers are internal, the typed `AttrKey` is internal-only, and the new
`SlotName` / `EvidenceStage` / renderer-mode DUs are internal with string
boundaries. No MVU/effect boundary changes. Not a graphical-viewer feature —
no persistent-launch task. `Route` is authoritative; the
`controls-public-surface` maintainer-verify set may be selected empirically even
with zero `.fsi` delta (101/102 precedent), which is gate selection, not a
surface delta.

## Success-criterion → assertion mapping

- **SC-006** (lowered `Control<'msg>` byte-/structurally identical) → the parity
  assertion authored in **T007**, captured against the **T006** baseline (green
  by construction; red only on perturbation), exercised by US1/US3 verification
  (**T011**, **T018**).
- **SC-001/SC-002** (one body per consolidated helper; no 217-char lambda) →
  the grep transcript in **T008**/**T011**.
- **SC-003** (redundant `private` removed, comments + keep-list intact) → the
  grep transcript in **T013**.
- **SC-004** (mistyped internal id is a compile error) → exhaustive DU matches
  compile-checked in **T018**.
- **SC-005** (suites green, no parity row moves) → the routed gate set in
  **T021** + `EvidenceAudit` in **T023**.
- **SC-007** (zero public-surface delta) → the `src/**/*.fsi` diff in **T019**.
- **SC-008** (keep-as-string / deferred items untouched) → the scoped diff in
  **T019**.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope (US4 is the banner
  constraint, carried by the parity + evidence chain, not its own phase)
- Tier annotations omitted: every phase matches the spec's overall **Tier 2**.

FAKE-backed commands (`./fake.sh`, `fake.cmd`, `dotnet fake`) share repository
`.fake` state and are **not** safe to run concurrently. The FAKE-backed gate
tasks (**T021 → T022 → T023**) carry explicit graph dependencies that serialize
them in deterministic order. Non-FAKE checks (grep transcripts, `.fsi` diff) are
parallel-safe.

## Governance risk levels

- **Small**: a single widget module's helper rewire (US1 reference swap).
- **Medium**: the cross-module consolidation + the internal DU introductions
  (US1/US3) touching `FS.Skia.UI.Controls`, `FS.Skia.UI.Scene`,
  `FS.Skia.UI.SkiaViewer` `.fs` bodies — **this feature's level**. Focused
  validation = the gate set `Route` prints (predicted inner-loop `Dev`; be
  prepared for the escalated `controls-public-surface` set + `EvidenceGraph` +
  `EvidenceAudit`).
- **Broad**: required only if `Route` escalates beyond the controls set or a
  FAKE failure looks race-like; rerun the affected FAKE-backed commands
  **sequentially** before any product-regression claim. Aggregate-suite results
  obtained outside the routed focused set are recorded as **non-authoritative**.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm `specs/105-housekeeping-code-quality/` is the active feature (`.specify/feature.json`), link spec + plan, and validate the `105-housekeeping-code-quality` branch
- [X] T002 [P] [skillist: []] Re-verify the audit's file:line citations against the current working tree (the plan notes line numbers shifted: `onChanged` at `Control.fs:1606/1611/1616/1621/1628/1633/1639/1683`, `slotRegions` at `Control.fs:99`, `StandardAttributeName` at `Types.fs:80`/`Types.fsi:86`, `RetainedRender` privates at `73/87/100/113/123`) so every edit lands on the real site
- [X] T003 [P] [skillist: []] Scaffold `specs/105-housekeeping-code-quality/readiness/` with audit-enforced placeholder files discoverable before implementation: `evidence-graph.md`, `evidence-audit.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, plus the zero-`.fsi`-delta and parity-proof artifacts — each naming its authoritative command, artifact path, failure class, and next action
- [X] T004 [P] [skillist: []] Record feature Tier (2, internal), affected layer (Controls/Scene/SkiaViewer `.fs` bodies), public-API impact (none — zero public `.fsi` delta), Elmish/MVU applicability (**N/A** — no stateful/I/O behavior changes), and the evidence obligations (routed gates green, parity, suites green, zero surface delta, `EvidenceGraph` + `EvidenceAudit` verdict)

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-typed-controls] Add the new `src/Controls/Widgets/WidgetLowering.fs` as `module internal WidgetLowering` (no `.fsi`) and insert `<Compile Include="Widgets/WidgetLowering.fs" />` into `Controls.fsproj` between `CustomControl.fs` and `Widgets/Primitives.fs` so it compiles before every consuming widget module (compile-order edge case)
- [X] T006 [P] [skillist: fs-skia-typed-controls] Capture the pre-change parity baseline under `readiness/`: the `sprintf "%A"` of the lowered `Control<'msg>` for every widget whose lowering uses a consolidated helper, plus the serialized Scene-stage (`"scene"`/`"renderer"`), `RendererMode`, and slot (`leading`/`trailing`/`header`/`footer`) strings (`Control<'msg>` has no structural equality — `%A` is the established 096/097/101 pattern)
- [X] T007 [skillist: fs-skia-typed-controls] Author the parity assertion against the T006 captured baseline over the consolidated helpers and serialized boundaries (green by construction for this behavior-preserving refactor — there is no genuine red→green; it goes red only if a consolidation perturbs attribute order, event-kind strings, key application, slot lowering, or a serialized string) (P1/P2; enforces SC-006)

**Checkpoint**: shared module slot + parity guard exist — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — One source of truth for the lowering helpers (P1)

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fs-skia-typed-controls] Add the independent de-dup verification: a grep transcript asserting exactly one body for `withKeyOpt`, `onString`, `onStringList`, and that `Control.fs` keeps a single `Double.TryParse` inside `tryParseFloat` and zero inline `onChanged` copies (SC-001/SC-002), and confirm the T007 parity assertion exercises every consolidated-helper widget

### Implementation

- [X] T009 [US1] [skillist: fs-skia-typed-controls] Populate `WidgetLowering` (`withKeyOpt`, `onString`, `onStringList`, the `a11y` accessibility-metadata builder, `intentToString`) and rewire the 9 `withKeyOpt` + 4 `onString` + 1 `onStringList` copies plus the `intentStyle`→string and accessibility-metadata duplications across the 10 widget modules to reference the shared home; remove the copies (FR-001/FR-002/FR-004)
- [X] T010 [US1] [skillist: fs-skia-typed-controls] Collapse the 8 inline `onChanged` parsers in `Control.fs` into `onChangedBool` / `onChangedFloat` / `onChangedString` at module scope over a named `tryParseFloat : string -> float option`, eliminating the twice-duplicated 217-char nested-`Double.TryParse` lambda (FR-003)
- [X] T011 [US1] [skillist: fs-skia-typed-controls] Run `./fake.sh build -t Dev`, confirm the T007 parity assertion stays green and the Controls + Controls.Elmish Expecto suites pass with no test edits forced, and capture the SC-001/SC-002 grep transcript

**Checkpoint**: each lowering helper has one home; lowered output is unchanged.

---

## Phase 4: User Story 2 (US2) — The `.fsi` is the single visibility boundary (P2)

### Implementation

- [X] T012 [US2] [skillist: fs-skia-reconciliation] Drop the ~17 redundant in-source `private` keywords the audit certifies redundant (the audit's ~16 plus the 10th `LegacyControls` module): the 10 `module private *Lowering` declarations → `module`, the 3 `let private` in `Reconcile` (`attrValueEqual`/`diffAttrs`/`isKeepOp`), and the 4 `let private` in `RetainedRender` (`childPath`/`clockDuration`/`fadeAnimation`/`currentOpacity`). Retain every "hidden by `<X>.fsi`" comment and leave the keep-list untouched — `module internal SceneRenderer`, the `InternalsVisibleTo` test seams, and the `let private` helpers inside the exposed `ControlInternals` (FR-005/FR-006)

### Verification

- [X] T013 [US2] [skillist: fs-skia-reconciliation] Confirm SC-003 by grep transcript (`module private` count = 0 in `src/Controls/Widgets/`, only the uncited `let private` remain in `Reconcile`/`RetainedRender`, `module internal SceneRenderer` still present, every former site keeps its documenting comment) and re-run `./fake.sh build -t Dev` green

**Checkpoint**: visibility is expressed once in the `.fsi`; no encapsulation weakened.

---

## Phase 5: User Story 3 (US3) — Internal closed-set identifiers become DUs (P2)

### Implementation

- [X] T014 [US3] [skillist: fs-skia-typed-controls] Introduce the **internal-only** `AttrKey` DU in `Control.fs` with a `name : AttrKey -> string` projection (building on feature 101's `[<Literal>]` attr names) and a typed `tryKey` reader, and route the closed control-intrinsic attribute reads in `Control.fs` and `DataGrid.fs` through it; the public `StandardAttributeName` DU stays unchanged (D3, FR-007/FR-012); string-keyed `tryLast`/`hasAttr` remain for genuinely dynamic names
- [X] T015 [US3] [skillist: fs-skia-typed-controls] Add the **internal** `SlotName` DU (`Leading|Trailing|Header|Footer`) used by `slotRegions`/`lowerSlots`, parsing the carrier string once at the consumption edge; the public `AttrValue.SlotFillsValue : (string * Control<'msg>) list` carrier stays unchanged — no public `SlotName` surface (FR-008, preserves feature 095's omission)
- [X] T016 [P] [US3] [skillist: fs-skia-evidence-mode] Add the **internal** `EvidenceStage` DU (`Scene|Renderer`) in `Scene.fs` driving the internal comparison, with the public `BlockedStage`/`DiagnosticCategory` record fields written `string` via a single `stage -> string` projection so the evidence text stays byte-identical `"scene"`/`"renderer"` (FR-009)
- [X] T017 [P] [US3] [skillist: fs-skia-viewer-host, fs-skia-evidence-mode] Add the **internal** renderer-mode DU in `SkiaViewer.fs`, parsing `request.RendererMode` once at the dispatch edge into a closed set (`default`/`skia`/`deterministic-scene`/`unsupported-host`/`metadata-hash`/`pixel-readback`) and making the case-insensitive `match` exhaustive; every public `RendererMode` output/serialized field stays an unchanged string (FR-009, §5C)
- [X] T018 [US3] [skillist: fs-skia-evidence-mode, fs-skia-viewer-host] Confirm SC-004 (a mistyped internal identifier is a compile error — the DU matches compile only against the closed set), re-run `./fake.sh build -t Dev` with the parity assertion green (P2 serialized strings byte-identical), and confirm the keep-as-string identifiers (`ControlKind`, public diagnostic/mode fields, consumer metadata keys, `ControlEvent.Kind`) are untouched (FR-010)

**Checkpoint**: internal closed-set identifiers are type-checked; serialized strings unchanged.

---

## Phase 6: Integration & Polish (US4 banner + evidence)

- [X] T019 [P] [skillist: []] Prove **zero public-surface delta** (`git diff --stat origin/main...HEAD -- 'src/**/*.fsi'` empty — SC-007) and that the deferred / keep-as-string items are untouched in the diff (no `ControlId` wrapper, no `ControlKind` change, no public diagnostic/mode field conversion, no `AttrValue` custom-equality change, no file split — SC-008/FR-013); and confirm no retained or added comment in the diff introduces a literal evidence filename or bare gate token that a governance gate (window-visibility/diff-scan) could parse as a status/behavior token (FR-014)
- [X] T020 [P] [skillist: []] Finalize `readiness/governance-risk-levels.md`, `readiness/aggregate-hang-diagnostics.md`, and `readiness/runtime-limitations.md`: record the selected medium risk level, the focused validation for it, when broad validation is required, and how non-authoritative aggregate results are recorded
- [X] T021 [skillist: fs-skia-typed-controls] Run `./fake.sh build -t Route` then exactly the gates it prints, FAKE-backed targets **sequentially** in the documented order (`Dev` → any escalated `controls-public-surface` set → `GeneratedGuidanceCheck`/`TemplateCheck`/`GeneratedProductCheck` if printed); capture the focused-gates log and confirm the Controls + Controls.Elmish suites are green with no parity/golden row moves (SC-005)
- [X] T022 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory=specs/105-housekeeping-code-quality` and `tasks=<n>` match, no cycles, no dangling refs, no `[S*]` surprises; write `readiness/evidence-graph.md`
- [X] T023 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with **0 synthetic** tasks and no diff-scan blockers; write `readiness/evidence-audit.md` with the verdict token

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — behavior-preserving refactor; parity compares real lowering output)_ | | | | | | | | |
