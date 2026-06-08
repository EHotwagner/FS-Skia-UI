# Tasks: Faithful Control Preview Rendering

**Feature branch**: `080-control-render-fidelity`
**Spec**: `specs/080-control-render-fidelity/spec.md`
**Plan**: `specs/080-control-render-fidelity/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/control-fidelity.md` and the propagated graph
view for the audit-computed status.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. **None is approved for this feature.**
The retained fidelity fixtures (T018) are synthetic gate *test vectors*, not
product evidence and not `[S]` tasks — they are disclosed as a fixture set
(Principle V) with a `(* SYNTHETIC FIXTURE: ... *)` banner; the catalog previews
themselves are real renders.

## Vertical-slice rule (US phases)

A `[US*]` task may be `[X]` only when the change is reachable from a user-facing
entry point and that path was actually exercised. For this feature the
user-facing surface is the rendered PNG / published catalog page / decoded gate
report — a `[US*]` task requires a real render, a decoded image, or a green gate
run captured under `readiness/`, not unit-test pass alone.

**Principle IV (MVU) is N/A** — this is a pure `Control -> Scene` rendering +
pure decode-and-assert governance change; no `Model`/`Msg`/`Effect`/interpreter
is introduced (see plan Constitution Check IV). No persistent graphical viewer
or default-executable launch surface is added or changed: the
`ControlsPreview.Harness` is a headless evidence path (helper evidence), not an
interactive viewer, so the persistent-launch task rule does not apply.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope (all P1)
- Tier omitted per-task: the whole feature is **Tier 1 (contracted, escalated
  `maintainer-verify`)**, so no phase differs from the spec's overall tier.

Every task line mirrors its structured `skillist` from `tasks.deps.yml` as
`[skillist: ...]` (`[skillist: []]` when none applies), in exact order.

## Governance risk levels

- **Small**: extraction/renderer internals (`src/Controls/Control.fs`,
  `SceneRenderer.fs`) → focused: `Dev` + harness suite.
- **Medium**: harness fidelity gate + fixtures + sample data → focused:
  `ControlFidelityCheck` (render-capable) + harness `--sequenced`.
- **Broad**: new build target + routing + `validation.contract.yml` +
  preview-asset regeneration → the escalated serialized order
  (`Route --enforce` → `Dev` → `ControlFidelityCheck` → `GeneratedGuidanceCheck`
  → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` →
  `EvidenceAudit`). Broad validation is required because this change registers a
  contract surface (FR-012). Aggregate results are recorded as
  **non-authoritative** where the local env fails (`GeneratedProductCheck` known
  local env failure).

## Validation

After writing both files, validate the DAG:

```bash
./fake.sh build -t EvidenceGraph
```

Full merge-gate (synthetic-propagation + diff-scan): `./fake.sh build -t EvidenceAudit`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm feature scaffolding and cross-links (spec.md, plan.md, research.md, data-model.md, contracts/, quickstart.md); confirm branch `080-control-render-fidelity`
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Create audit-discoverable readiness placeholders under `specs/080-control-render-fidelity/readiness/`: `control-fidelity.md`, `real-image-evidence.md`, `usage-coherence.md`, `visual-evidence-honesty.md`, `window-visibility.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md` — each naming the authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [skillist: []] Record feature Tier (Tier 1, escalated `maintainer-verify`), affected packages (`FS.Skia.UI.Controls`, `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Build`, `FS.Skia.UI.Scene`), public-API impact (no public `.fsi` delta expected), Principle IV N/A rationale, evidence obligations, and the small/medium/broad governance risk levels with focused validation per level
- [X] T004 [P] [skillist: fs-skia-skiaviewer] Capture the failing-first 079 baseline: render the current schematic previews (`-- --render`) and stage the pre-fix label-on-box PNGs from `main` (e.g. `line-chart`, `list-box`, `image`, `icon`) as `lowfi` fixture candidates (quickstart §1; SC-003 red precondition)

---

## Phase 2: Foundation (shared prerequisites)

- [X] T005 [P] [skillist: fs-skia-ui-widgets] Add failing-first extraction test asserting `chartValues` yields the structured `ChartSeries`/`ChartPoint` points (with X/Y/Label) for a typed chart control built from `sampleSeries` — today it yields `[]` (FR-002; root cause `Control.fs:159`)
- [X] T006 [skillist: fs-skia-ui-widgets] Fix `chartValues` (`src/Controls/Control.fs:159`) to read `UntypedValue(ChartSeries list)` under `"series"` (line/bar/scatter) and `UntypedValue(ChartPoint list)` under `"values"` (pie), preserving X/Y/Label, keeping the flat-list fallback; make T005 green (FR-002)
- [X] T007 [P] [skillist: fs-skia-evidence-mode] Define `PixelSignature`/`PrimitiveSignature`/`ContentSignature` and the fail-closed `FidelityDeclaration` (`Signature` | `UnsupportedNoPreview`) in the harness, and add `Fidelity` as a **required** field on `ControlSampleDefinition` so a Demonstrative sample without a signature does not compile (data-model; content-signature.contract; D5/FR-013)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 — A reader recognizes a control from its preview (US1, P1)

### Tests First (Principle I, VI)

- [X] T008 [P] [US1] [skillist: fs-skia-scene] Add renderer tests asserting per-chart-family geometry is present in `Scene.describe`: line → `PathElement`, bar → `RectangleElement` (≥ #points), pie → `ArcElement`, scatter → `PointsElement`/`CircleElement`, graph → `CircleElement` + `LineElement`, all within canvas bounds (FR-002; US1 Acceptance 1)
- [X] T009 [P] [US1] [skillist: fs-skia-scene] Add renderer tests for collections (≥3 distinct item rows), value/selection chrome+state (track+thumb, filled progress, radio circles with selection, tab strip with active tab, toggle/tick), `image` framed placeholder, and `icon` font-supported glyph — all within canvas bounds (FR-003, FR-004, FR-005, FR-011; US1 Acceptance 2–4)

### Implementation

- [X] T010 [P] [US1] [skillist: fs-skia-typed-controls] Author representative, font-safe sample data + the required `ContentSignature` for every Demonstrative control in `tests/ControlsPreview.Harness/PreviewSamples.fs`: collections ≥3 items, value/selection explicit state, `image` framed placeholder, `icon` glyph verified present in the rendering font (replace `★`); fixed literals only (FR-014; D7)
- [X] T011 [US1] [skillist: fs-skia-scene, fs-skia-layout-readability] Replace the uniform `renderNode` body (`src/Controls/Control.fs:194`) with per-`Kind` faithful geometry lowered to existing Scene primitives within canvas bounds (charts → Path/Rectangle/Arc/Points; collections → rows; value/selection → chrome+state; `image` → frame; `icon` → glyph) (FR-001, FR-003, FR-004, FR-005)
- [X] T012 [US1] [skillist: fs-skia-skiaviewer] Stop emitting the opaque `Chart` node on the preview path in `src/SkiaViewer/SceneRenderer.fs` so charts render as bounds-safe primitives and the `chartTop=180` off-canvas painter (`SceneRenderer.fs:394`) is bypassed (FR-002, FR-011; canvas-bounds edge case)
- [X] T013 [US1] [skillist: fs-skia-ui-widgets] Render a recognizable honest empty state within canvas bounds for empty/missing-data controls; fall back to `Unsupported` only where no authored data yields a recognizable depiction; `custom-control` stays `Unsupported` (FR-009, FR-011)
- [X] T014 [US1] [skillist: fs-skia-skiaviewer] Render the one-per-family sample (chart, collection, value/selection, icon, image, layout) via the harness and confirm each shows control-specific structure, not a label-on-a-box; record the family-recognition check under `readiness/real-image-evidence.md` (US1 Independent Test; SC-002)

**Checkpoint**: US1 — controls render faithful, control-specific previews.

---

## Phase 4: User Story 2 — Demonstrative is enforced, not asserted (US2, P1)

### Tests First (Principle VI — failing-first)

- [X] T015 [P] [US2] [skillist: fs-skia-evidence-mode] Add gate tests: every `lowfi` fixture **fails** its control's signature, every `faithful` fixture **passes**; and a fail-closed test where a catalog id with neither a `Signature` nor `UnsupportedNoPreview` fails with a message naming the control (SC-003; FR-013)

### Implementation

- [X] T016 [US2] [skillist: fs-skia-evidence-mode] Implement `tests/ControlsPreview.Harness/Fidelity.fs`: decode each committed PNG (`SKBitmap`), exclude the title band, compute coverage + distinct-color pixel signature, take `Scene.describe` from `Control.render Theme.light` for the primitive-kind signature, and emit a `FidelityVerdict` whose `FailureReason` names the control + missing component (FR-007; content-signature.contract; data-model)
- [X] T017 [US2] [skillist: fs-skia-skiaviewer] Add the `-- --fidelity` mode to the harness `Program.fs` that runs the gate and writes the decoded-content report `readiness/control-fidelity.md` (per-control rows + fixture matrix); classify native-Skia-absent as a **blocking host warning** (never a silent pass) per `fs-skia-evidence-mode` (FR-008; Principle VII)
- [X] T018 [P] [US2] [skillist: fs-skia-evidence-mode] Commit the retained fixture set `tests/ControlsPreview.Harness/fixtures/fidelity/lowfi/*` (from the staged pre-fix `main` renders) and `faithful/*` (regenerated counterparts) with a `(* SYNTHETIC FIXTURE: ... *)` banner; lowfi MUST fail, faithful MUST pass (D6; SC-003)
- [X] T019 [P] [US2] [skillist: fsharp-build-orchestration] Register the `ControlFidelityCheck` target: `Targets.Target` DU case + `allTargets` + `name` + `spec` (timeout=medium, cost=medium, owner=product) + `AgentValidation.knownGates` (fidelity-gate.contract §Target registration)
- [X] T020 [US2] [skillist: fsharp-build-orchestration] Add the `Engine/Update.fs` `StartTarget ControlFidelityCheck` process effect that shells out `dotnet run --project tests/ControlsPreview.Harness --no-restore -- --fidelity`, mirroring the `SkiaViewer.Tests -- --sequenced` pattern (`Update.fs:61`); keep `FS.Skia.UI.Build` SkiaSharp-free (FR-008)
- [X] T021 [US2] [skillist: fsharp-code-generation] Add `ControlFidelityCheck` to the `controls-catalog-docs` routing rule's `RequiredGates` and extend its `Paths` with `tests/ControlsPreview.Harness/**`, then regenerate `validation.contract.yml` via `./fake.sh build -t RefreshSurfaceBaselines`; `TargetMetadataDrift` enforces currency (no hand-edit) (FR-012)
- [X] T022 [US2] [skillist: fsharp-build-orchestration] Run `./fake.sh build -t ControlFidelityCheck` and demonstrate the red→green transition: the gate fails the pre-fix/lowfi previews and passes the faithful ones, with a control-naming message; capture to `readiness/control-fidelity.md` (US2 Independent Test; SC-003/SC-005)

**Checkpoint**: US2 — fidelity is build-enforced; a label-on-box cannot pass.

---

## Phase 5: User Story 3 — The catalog and evidence stop overclaiming (US3, P1)

### Implementation

- [X] T023 [P] [US3] [skillist: fs-skia-skiaviewer, fs-skia-evidence-mode] Regenerate every catalog preview from the new renderer through the real render-only evidence path (genuine decodable PNG of documented dimensions): `docs/img/controls/*.png`; no image for `Unsupported` controls (FR-006)
- [X] T024 [US3] [skillist: fsharp-code-generation] Regenerate the corrected catalog detail-page Preview prose via `CatalogDocsGen` (`docs/controls/*.md`) so each per-control claim matches the decoded image content (FR-010)
- [X] T025 [US3] [skillist: fs-skia-evidence-mode] Author `readiness/real-image-evidence.md` and `readiness/usage-coherence.md` against decoded images; present `custom-control` (and any non-depictable control) as honestly `Unsupported` (no image + explicit status); add a correction note on the 079 readiness overclaims pointing to 080 (FR-009, FR-010)

### Verification

- [X] T026 [US3] [skillist: fs-skia-evidence-mode] For every per-control evidence/catalog claim, decode the referenced image and confirm the described content is visibly present; confirm zero unverifiable per-control visual claims remain (US3 Independent Test; SC-004)

**Checkpoint**: US3 — every per-control claim is supported by visible image content.

---

## Phase 6: Integration & Polish

- [X] T027 [P] [skillist: fs-skia-scene] Recapture moved baselines: `Scene.describe` snapshots and screenshot baselines for chart/collection/value controls; per-package surface snapshots only if a `.fsi` actually changed (none expected — confirm; D8)
- [X] T028 [P] [skillist: fs-skia-skiaviewer] Run the harness suite `dotnet run --project tests/ControlsPreview.Harness -- --sequenced`; confirm totality/explicitness/idempotence retained and no product/runtime control regression (SC-006)
- [X] T029 [skillist: fsharp-build-orchestration] Run the escalated serialized FAKE order sequentially (shared `.fake` state): `Route --enforce`, `Dev`, `ControlFidelityCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck` (known non-authoritative local env failure — record as such), capturing aggregate results to `readiness/aggregate-hang-diagnostics.md`
- [X] T030 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; capture graph before/after
- [X] T031 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (fidelity fixtures disclosed as gate test vectors, not product `[S]`; no `--accept-synthetic` anticipated)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section.

No `[S]` or `[SEH]` tasks are anticipated: the catalog previews are real renders
and the gate is a real decode. The retained fidelity fixtures (T018) are
synthetic gate *test vectors* (079-style label-on-box + faithful counterparts),
disclosed as a fixture set with a `(* SYNTHETIC FIXTURE: ... *)` banner — they
are **not** product evidence and do not make any task `[S]`.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
