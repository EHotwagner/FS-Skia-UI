# Tasks: Demonstrative Control Preview Images in Published Docs

**Feature branch**: `079-doc-preview-examples`
**Spec**: `specs/079-doc-preview-examples/spec.md`
**Plan**: `specs/079-doc-preview-examples/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` (with the `synthetic-error-handling-approved` label) is a
design-time-only annotation; none is approved for this feature.

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when the change is reachable from a
user-facing entry point and that path was actually exercised. For this feature
the user-facing surface is the **published docs site / committed preview PNG a
reader sees** — so `[X]` on a US task requires real image/site evidence under
`readiness/` (a regenerated demonstrative PNG, a catalog/detail-page render, or
a strict site build), not merely a green unit test. This rule does not apply to
Setup, Foundation, or Polish tasks.

**MVU/Elmish applicability**: N/A as a runtime concern (see plan §MVU/effect
boundary). No framework `Model`/`Msg`/`Effect`/`update` changes; stateful
controls (slider, list-box selection) are initialized via their typed `init`
with **fixed** sample models at the render edge only.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- Tier annotation omitted: every phase matches the spec's overall **Tier 2
  (internal)** classification (plan §Constitution Check).

Every task has a matching entry in `tasks.deps.yml`; every task line mirrors the
structured `skillist` value as `[skillist: ...]` (use `[skillist: []]` when
empty). FAKE-backed commands share `.fake` state and MUST run **sequentially**
in the documented order; non-FAKE reads may parallelize.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold the feature workspace: confirm `spec.md`/`plan.md`/`research.md`/`data-model.md`/`contracts/` are linked, and update `AGENTS.md` SPECKIT plan reference 078 → `specs/079-doc-preview-examples/plan.md`
- [X] T002 [P] [skillist: []] Create `specs/079-doc-preview-examples/readiness/` with audit-enforced placeholder files discoverable before implementation: `controls-preview-evidence.md`, `controls-catalog-docs.md`, `docs-build.md`, `real-image-evidence.md`, `visual-evidence-honesty.md`, `window-visibility.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `gate-diagnostics.md`, `evidence-graph.md`, `evidence-audit.md` (each naming its authoritative command, artifact path, failure class, next action)
- [X] T003 [P] [skillist: []] Record feature Tier (Tier 2 internal, governance + `docs/**` consumed-contract generation surface), affected layers, **no public `.fsi`/API/behavior change**, MVU **N/A (runtime)**, and the real evidence obligations (preview evidence, catalog currency, docs build) in `readiness/runtime-limitations.md`

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-typed-controls, fs-skia-ui-widgets] Declare the single `ControlSampleDefinition` source (R1, FR-002): one entry per `CatalogGen.catalogFacts` id, in catalog order, each with `Kind = Demonstrative | Unsupported`, a typed-front-door `Build`, optional fixed per-control `Canvas` (default 320×160), and a `UsageNote` — established as the one reviewable source (content authored in US1)
- [X] T005 [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-evidence-mode] Add the committed compiled render harness (R2, FR-003/P2) referencing `FS.Skia.UI.Controls.Typed` + `Scene` + `SkiaViewer` + SkiaSharp: loop `Demonstrative` entries → `Widget.toControl` → `Control.render Theme.light` → `SceneNode.Group` → `SkiaViewer.captureScreenshotEvidence` with `CaptureMode = ViewerRenderTargetPng` → write `docs/img/controls/<id>.png`; write **no** image for `Unsupported`; document its invocation in quickstart
- [X] T006 [P] [skillist: fsharp-code-generation] Extend `build/Governance/CatalogDocsGen.fsi`/`.fs` (pure core, SkiaSharp-free) with the `PreviewContentVerdict` set and a `TrivialPreview` byte-floor finding alongside the existing `Undecodable*`/`Missing*`/`Stale*`/`Orphan*`/`DeadLink` findings (data-model entity 3) — guard logic wired in US3
- [X] T007 [P] [skillist: fs-skia-evidence-mode] Record unsupported-scope handling and failure diagnostics in `readiness/runtime-limitations.md` + `readiness/visual-evidence-honesty.md`: how the harness/gate distinguish an honest `Unsupported` declaration (no image, `preview-status: unsupported` marker) from a real `RenderingFailure`, per the evidence-mode benign/blocking rules

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — Demonstrative previews replace blank boxes (P1)

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fsharp-build-orchestration] Failing-first totality + explicitness tests (P1.1/P1.2): the set of `ControlSampleDefinition` ids set-equals `catalogFacts` ids (no gap/orphan), and no `Demonstrative` entry renders empty-content widgets
- [X] T009 [P] [US1] [skillist: fsharp-build-orchestration] Failing-first harness idempotence test (P4, FR-008, SC-004): re-running the render harness over the same sample source yields byte-identical PNGs (asserted over committed bytes / a hash manifest)

### Implementation

- [X] T010 [US1] [skillist: fs-skia-typed-controls, fs-skia-ui-widgets, fs-skia-layout-readability] Author the per-control `Demonstrative` sample content for every renderable control by family (R4): display text/glyphs/runs, labelled inputs, mid-track slider, checked/on selections, populated list/data-grid with selected row + columns/rows, sample chart series, composed layout children, single representative static frame for motion/overlay — sized/truncated to stay legible within the (fixed, documented) canvas (FR-001, SC-001)
- [X] T011 [US1] [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-evidence-mode] Run the render harness on a render-capable host to regenerate `docs/img/controls/<id>.png` through the real render-only path; commit `Demonstrative` PNGs and **no** image for `Unsupported` ids (FR-003, FR-008)
- [X] T012 [US1] [skillist: []] Pin the trivial-content floor `T` (R3): measure the smallest demonstrative PNG and the empty-canvas (~363-byte) size, set `T` to a documented round value comfortably between them with headroom, and record the procedure/result in research/readiness
- [X] T013 [US1] [skillist: fs-skia-evidence-mode] Capture US1 user-facing evidence in `readiness/real-image-evidence.md` + `readiness/visual-evidence-honesty.md`: catalog index + several detail pages show recognizable, control-specific content (not empty boxes), with 0 controls near-empty (SC-001)

**Checkpoint**: US1 — catalog previews are demonstrative and verifiable from the docs surface.

---

## Phase 4: User Story 2 (US2) — Preview reflects documented usage (P2)

- [X] T014 [US2] [skillist: fs-skia-typed-controls, fs-skia-ui-widgets] Align each `UsageNote` and sample configuration with the control's documented detail-page usage so image and prose stay coherent (FR-006): e.g. a control documented as requiring `columns`/`rows` depicts columns and rows
- [X] T015 [US2] [skillist: []] Verify coherence across a defined sample of detail pages — at minimum ≥8 pages spanning the distinct control families (display/text, labelled input, slider, checkbox/switch, list-box, data-grid, chart, composed/overlay) — and record the review (0 contradictions) in `readiness/controls-catalog-docs.md` (SC-002)

**Checkpoint**: US2 — previews are coherent with documented usage.

---

## Phase 5: User Story 3 (US3) — Previews stay honest and current (P1)

### Tests First (Principle I, Principle VI)

- [X] T016 [US3] [skillist: fsharp-build-orchestration, fsharp-io-globbing] Failing-first currency tests (P3, SC-003): `ControlsCatalogDocsCheck` FAILs with the matching finding on one negative case per class — `Trivial` (bytes < `T`), `Missing`, `Undecodable`, `Orphan`, stale/missing detail region, `DeadLink` — and PASSes on the regenerated demonstrative tree

### Implementation

- [X] T017 [US3] [skillist: fsharp-code-generation, fsharp-io-globbing] Wire the trivial-content byte-floor guard + evidence-record consistency cross-check at the `build/Governance/Engine/Update.fs` edge; the report names `TrivialPreview` with an actionable remedy and the re-render command (FR-004/FR-005, P3.3)
- [X] T018 [US3] [skillist: []] Regenerate `readiness/controls-preview-evidence.md` honesty ledger (per-control: id, name, render-only mode, decodable, dimensions, bytes, content classification) plus the reconciled `rendered = N / unsupported = M`, `N + M == |catalog|` summary (FR-010, FR-007, SC-005); confirm `ControlsCatalogDocsCheck` PASS recorded in `readiness/controls-catalog-docs.md`
- [X] T019 [US3] [skillist: fsharp-code-generation] Regenerate `build/Governance/validation.contract.yml` from `Routing.fs` IFF a routed glob changed (never hand-edited; else confirm `TargetMetadataDrift` shows no drift)

**Checkpoint**: US3 — the currency gate guards blank/trivial/missing/stale/orphan previews and the honesty ledger reconciles.

---

## Phase 6: User Story 4 (US4) — Finding controls alongside the examples (P2)

- [X] T020 [US4] [skillist: fsdocs-build] Apply the R6 `categoryindex` renumber (FR-011, N1/N2): `docs/controls/*` 2→8, `docs/roadmap.md` 7→9, `docs/development.md`/`docs/distribution.md`/`docs/migration/v2-to-v3.md` 8→10 — change **only** `categoryindex` lines; no file moves, no `index`/slug changes
- [X] T021 [US4] [skillist: fsdocs-build] Run `dotnet fsdocs build --strict --eval` (GPU-free, no render host required — FR-009) and record in `readiness/docs-build.md`: built sidebar order is Examples → **Controls** → Guides, every preview present, every image and cross-link into `docs/controls/` resolves (FR-009, N3, SC-004, SC-006)

**Checkpoint**: US4 — Controls renders immediately below Examples and above Guides with no broken links.

---

## Phase 7: Integration & Polish

- [X] T022 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Route` then the serialized FAKE order it prints (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`) sequentially; name the small/medium/broad governance risk level and focused validation in `readiness/governance-risk-levels.md`, recording non-authoritative aggregate results (incl. the known local `GeneratedProductCheck` environment-failure) in `readiness/aggregate-hang-diagnostics.md`
- [X] T023 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm no cycles, no dangling refs, no `[S*]` surprises; write `readiness/evidence-graph.md`
- [X] T024 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict PASS (no `[S]` expected at merge) or document every `--accept-synthetic` override; write `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. No `[S]` is expected
at merge (plan §Synthetic evidence): a control that cannot be honestly rendered
keeps its explicit `Unsupported` declaration (real honest evidence, not
synthetic). For `[SEH]` rows, include the approval label, design-phase source,
synthetic input class, expected error behavior, and acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
