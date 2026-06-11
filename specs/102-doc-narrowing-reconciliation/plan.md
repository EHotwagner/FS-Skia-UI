# Implementation Plan: Documented-Narrowing Reconciliation (R8)

**Branch**: `102-doc-narrowing-reconciliation` | **Date**: 2026-06-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/102-doc-narrowing-reconciliation/spec.md`

## Summary

R8 is a **pure honesty pass**: reconcile six places where a *description* (roadmap
prose, an in-source comment, or an advertised role classification) is slightly broader
than the *shipped truth*, so a future reader is not misled. **No observable rendering
output, parity/golden evidence, determinism property, or runtime behavior changes.** The
work is (a) prose edits to
`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md` and (b) a small
number of descriptive source comments/annotations in `FS.Skia.UI.Controls` and
`FS.Skia.UI.Layout`. Two reconciliation choices are *decided up front* and recorded here:
**annotate (not remove)** the dead `Selected`-from-`Selection` derivation (FR-002b), and
**document/annotate (not drop, not enable routing)** the `Chart`/`Graph`/`Progress`
value-role classification (FR-005, document option). Both defaults keep the change
**zero public `.fsi` delta** and **zero behavior change**.

This is a **Tier 2 (internal/documentation) change** per the constitution: no public API
surface is added, removed, or modified under the recorded default choices; `.fsi` files
and surface baselines remain untouched.

## Technical Context

**Language/Version**: F# / .NET (`net10.0`)
**Primary Dependencies**: N/A — no dependency change (no `Directory.Packages.props` edit)
**Testing**: existing Controls / Elmish / Layout suites run **unmodified** (this pass adds
no test and no property); `EvidenceGraph` + `EvidenceAudit` produce the merge-gate verdict
**Target Platform**: Windows and Linux (unchanged; nothing platform-specific)

### Verified source sites (grounding — confirmed against working tree 2026-06-11)

| Narrowing | Cited site | Verified |
|---|---|---|
| FR-001 R1 order wording | roadmap §10.3 (`…-roadmap.md:686`, `:704`) | ✅ §10.3 describes `deriveVisualState`; `.fsi` already splits |
| FR-002 dead `Selected` | `src/Controls/ControlRuntime.fs:203` (`deriveVisualState`), dead branch at `:206-207` (`model.Selection |> Option.exists`) | ✅ — see signature finding below |
| FR-003 R2 cache wording | roadmap §10.4 (`…-roadmap.md:754`, `:768` "keyed by retained identity") | ✅ overstates; shipped cache is `Bounds`/`LayoutNodeId` |
| FR-004 Yoga rationale | `src/Layout/Layout.fs:7-12` (INV-1 motive present, approval rationale absent) | ✅ |
| FR-005 value-role + FR-006 segmented | `src/Controls/Focus.fs:123-129` (`navIntentFor`, `Progress|Chart|Graph` branch); `Accessibility.defaultFor`; roadmap `:938`, `:1041` "segmented" | ✅ no `Segmented` `AccessibilityRole` exists |
| FR-007 preview-path id | `src/Controls/Control.fs:1131` (`layoutNode`: `control.Key |> Option.defaultValue control.Kind`) | ✅ legacy 080 preview/layout path, distinct from R3's `Key ?? path` |

**Signature finding (drives the FR-002 decision).** `deriveVisualState`'s dead `Selected`
branch is one `elif` on `model.Selection`; the `model` parameter is still used by the
`PressedControls`/`FocusedControl`/`HoveredControl` branches. Therefore **removing the
branch would NOT drop a parameter or change the `.fsi` signature** — the spec's "removal
changes a public signature" edge case does **not** apply here. Both annotate and remove are
zero-`.fsi`-delta. We still choose **annotate** (FR-002b) as the lowest-risk option because
the public `deriveVisualState` is exercised by tests that may seed a `Selection`; annotation
guarantees no test moves. The decision and its rationale are recorded (SC-006).

### Routing reality (read before validating)

The spec's Build-target section optimistically predicts the framework-internal
`src/**/*.fs` comment edits route **inner-loop `Dev`**. **Treat that as unconfirmed.**
Feature 101 (R7) established that **any `src/Controls/**/*.fs` edit — even a pure comment —
escalates `Route` to the `controls-public-surface` gate set**, regardless of whether a
`.fsi` changes. Because R8 touches `src/Controls/ControlRuntime.fs`, `src/Controls/Focus.fs`,
and `src/Controls/Control.fs`, **expect escalation to the full controls-public-surface set**.
`src/Layout/Layout.fs` and the roadmap doc edit route per whatever `Route` prints.

**Run `./fake.sh build -t Route` first and run exactly the gates it prints.** If it
escalates (expected), run the escalated set sequentially (FAKE-backed, never concurrent).
No public-surface baseline recapture is required under the recorded default choices because
no `.fsi` signature moves.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Result: PASS.** This is a Tier-2 documentation/internal-comment pass. No `.fsi` change,
no new dependency, no stateful/I-O workflow, no synthetic evidence. Principles I–VII are
satisfied trivially: there is no behavior to test-first (FR-008 forbids behavior change),
visibility stays entirely in the unchanged `.fsi` files (Principle II), the change is the
plainest possible (prose + comments, Principle III), no MVU boundary is touched
(Principle IV — none added or altered), no synthetic evidence is introduced
(Principle V — `EvidenceAudit` must report **0 synthetic**), no behavior-changing code means
the "fail-before/pass-after" test rule (Principle VI) is satisfied by the **existing suites
staying green and unchanged**, and no diagnostics/failure paths change (Principle VII).

### Repository Governance Decisions

- **Template ownership**: **N/A — no template change.** R8 touches a repo report and
  framework-internal source comments only; it does not alter source contracts, samples,
  Spec Kit assets, package policy, or the command surface, so
  `.template.config/template.json` is untouched. The roadmap report does not ship in the
  generated project.
- **Dependency impact**: **N/A — no dependency change.** No `Directory.Packages.props`,
  `docs/dependencies.md`, generated-template inclusion, or `DependencyReport` edit; no
  package added, removed, or repinned.
- **Command-surface impact**: **No new or changed target.** R8 adds no gate and changes no
  target. Validation is the **existing routed set** for this diff. **Run
  `./fake.sh build -t Route` first** and run only the gates it prints. FAKE-backed commands
  share `.fake` state and MUST run **sequentially** in deterministic order. Expected
  escalated order if `Route` escalates to controls-public-surface (run sequentially):
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  (plus any controls-public-surface-specific gates `Route` names; a doc-only subtree edit may
  also trip the broad doc-rules check per feature 088 — confirm via `Route`).
- **Generated project impact**: **N/A — none.** Default/minimal generated contents, selected
  Controls guidance, local skills, validation logs, and generated `Dev` behavior are all
  unchanged; the report is not part of generated output.
- **Evidence paths**: roadmap diff is its own reconciliation evidence
  (`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`); source-comment
  diffs in `src/Controls/ControlRuntime.fs`, `src/Controls/Focus.fs`,
  `src/Controls/Control.fs`, `src/Layout/Layout.fs`; gate logs and audit output under
  `specs/102-doc-narrowing-reconciliation/readiness/` (`evidence-graph.md`,
  `evidence-audit.md` with a verdict token, `generated-validation.md`); no new screenshots,
  FSI transcripts, or surface-baseline recapture (no `.fsi` move under default choices).
- **`.fsi` / contract impact**: **No `.fsi` / surface-baseline change under the recorded
  default choices (annotate, document).** `deriveVisualState`'s signature is preserved even
  if FR-002(a) removal were taken (the dead branch carries no unique parameter), so the only
  way R8 would move a baseline is a *deliberate* election of FR-002(a) removal that also
  recaptured the affected baseline — **not taken**. No sample contract, public-docs, or
  compatibility-note change.
- **MVU/effect boundary**: **N/A — no stateful or I/O work.** No `Model`/`Msg`/`Effect`/
  `init`/`update`/interpreter is added or altered. The `ControlRuntime` MVU surface is read
  for context only; its behavior is untouched.
- **Synthetic evidence**: **None.** No mocks, fakes, placeholders, canned responses, or
  in-memory substitutes are introduced. No `[S]`/`[S*]`/`[SEH]` task will exist;
  `EvidenceAudit` MUST report **0 synthetic**. R8 adds no test fixture at all.
- **Test evidence**: **No new test; existing suites run unmodified.** Because FR-008 forbids
  behavior change, the fail-before/pass-after rule (Principle VI) is met by the R1/R2/R4/R5
  property and unit suites (Controls / Elmish / Layout) staying **green and byte-identical**.
  A moved or edited test would be a red flag that a comment was parsed as a behavior token
  (FR-010) — none is expected. Governance verdict from `EvidenceGraph` + `EvidenceAudit`.
- **Observability**: **N/A — no diagnostics change.** No new log path, report field,
  missing-artifact failure class, or unsupported-environment message. Comments added are
  purely descriptive (FR-010) and MUST NOT be parseable by any gate as a status/behavior
  token (avoid bare gate-significant tokens / literal evidence filenames that would trip the
  window-visibility or diff-scan audits).
- **Deferred scope**: **R6** (behavior-changing visual-state cross-fade); **enabling**
  default navigation routing for `Chart`/`Graph`/`Progress` or adding a `Segmented`
  `AccessibilityRole` (behavior/surface change, separate feature); **landing** the R2
  intrinsic-size memo (feature 101's recorded FR-008 deferral — R8 only reconciles the
  §10.4 wording to the shipped `Bounds`/`LayoutNodeId` cache). All explicitly out of R8 scope.

## Project Structure

Documentation (this feature):

```
specs/102-doc-narrowing-reconciliation/
├── spec.md              # input (complete)
├── plan.md              # this file
├── research.md          # Phase 0 — the six reconciliations + two recorded decisions
├── data-model.md        # Phase 1 — N/A (no entities); records why
├── quickstart.md        # Phase 1 — how to verify the six reconciliations
├── contracts/
│   └── README.md        # Phase 1 — no contract change (records why)
└── readiness/           # gate logs + evidence-graph.md / evidence-audit.md (at implement)
```

Source touched (comments / annotations only — no logic, no `.fsi`):

```
docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md   # FR-001, FR-003, FR-006 (§10.3, §10.4, "segmented")
src/Controls/ControlRuntime.fs   # FR-002b — annotate dead Selected-from-Selection derivation
src/Layout/Layout.fs             # FR-004 — add maintainer blast-radius approval rationale
src/Controls/Focus.fs            # FR-005 — annotate Chart/Graph/Progress classed-but-not-routed
src/Controls/Control.fs          # FR-007 — annotate :1131 legacy 080 preview-path id
```

No `.fsi`, no baseline, no sample, no template, no `Directory.Packages.props`, no build
target changes.

## Phase 0 — Research

See [research.md](./research.md). All NEEDS CLARIFICATION resolved: there were none —
the spec pins each of the six sites and both reconciliation-choice defaults; research
confirms each site against the working tree and records the two decisions (FR-002 →
annotate; FR-005 → document).

## Phase 1 — Design & Contracts

- **data-model.md**: N/A (no entities, no state transitions) — recorded with rationale.
- **contracts/**: no external interface change — `contracts/README.md` records why and
  affirms zero `.fsi`/baseline delta under the default choices.
- **quickstart.md**: a reviewer's checklist to confirm each of the six reconciliations and
  the zero-behavior-change invariant.
- **Agent context**: `AGENTS.md` SPECKIT marker updated to point at this plan.

## Post-Design Constitution Re-Check

**PASS (unchanged).** Phase-1 artifacts introduce no entity, contract, dependency, or
synthetic evidence. The design remains a documentation/internal-comment pass with zero
`.fsi` delta and zero behavior change. Ready for `/speckit.tasks`.
