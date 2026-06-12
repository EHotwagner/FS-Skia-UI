# Implementation Plan: View Memoization and Stable Dependency Contracts

**Branch**: `113-view-memoization` | **Date**: 2026-06-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/113-view-memoization/spec.md`

## Summary

Features 109–112 hardened the retained hot path (honest metrics + corpus, retained
pointer routing, view-skip scheduler, targeted runtime visual-state stamp). The
report's **Phase 5** is the remaining half of cross-framework practice: **stop
recomputing expensive pure control subtrees when their declared inputs are unchanged**
(React `memo`/`useMemo`, Compose `remember`/skipping, SwiftUI dependency-local bodies)
and **make the unstable inputs that defeat reuse visible**.

**Technical approach (Phase 5 of the performance report, "Do next" #2):**

1. **Control-internal memoization seam** (`FS.Skia.UI.Controls`, **internal**). A pure
   seam keyed by a control's stable `ControlId` plus a **caller-supplied deterministic
   dependency value**: when the dependency compares **equal** to the prior frame's, the
   previously-lowered subtree for that identity is reused (a **hit**, no recompute);
   otherwise the transform recomputes and the result is stored keyed by identity +
   dependency (a **miss**). The cache is carried frame-to-frame in the retained
   structure's per-identity state (`RetainedRender`). **No** public `Control.memo` /
   `Widget.memo` primitive this rung (deferred, clarified 2026-06-12).

2. **Apply it to a representative expensive transform** (FR-003): the **DataGrid
   row/column projection** (the report's named #1 — `Control.fs` `gridGeom`, the
   `cells → Scene` tabular projection), proven **byte-identical** to the non-memoized
   build. This is the **sole** memoized site this rung. `Style.resolve` is **explicitly
   deferred** — the seam is kept general enough to wrap it later, but `Style.resolve`
   lowers to a `ResolvedStyle` rather than the `Scene list` the memo entry stores, so
   wiring it requires widening the stored subtree type; that widening + the second site are
   a later rung. The stored `MemoEntry.Subtree` is therefore **specialized to `Scene list`**
   this rung and the boxed dependency value (`obj`, compared by F# structural `=`, never
   object identity) keeps `MemoCache` a single uniform map — see
   [data-model.md](./data-model.md) `MemoEntry`.

3. **Public `MemoHitCount` / `MemoMissCount` `FrameMetrics` fields** (breaking
   `ControlsElmish.fsi` change, two new fields — precedent 109/110/111). Threaded from
   the retained step / lowering path, surfaced on the deterministic `Perf.runScript`
   path (golden-asserted) and through the live `OnFrameMetrics` sink. Both `0` on a
   frame that evaluates no memoizable control.

4. **Memo-off parity oracle** (FR-008): an internal always-miss switch so the seam can
   be disabled with **zero** change to rendered output, the authority that a dependency
   value is not too coarse (FR-006/FR-007).

5. **Stability-diagnostic report** (`FS.Skia.UI.Controls` `Diagnostics`, **public**,
   in the spirit of 101's `layoutDriftReport`): given a control (sub)tree built across
   two frames, flag the attributes/events that compared **unequal** despite no semantic
   change — the always-new inputs (rebuilt `UntypedValue`, per-frame closures, rebuilt
   lists, unstable keys) — naming the control + input. A **report tool asserted in
   tests, NOT an enforced gate** (clarified). Plus an author-facing **stable-props
   guidance page** under `docs/controls/`.

This is **additive performance + diagnostics only** (FR-014): at-rest rendered output,
geometry, focus/keyboard routing, and every dispatch outcome stay **byte-identical**.
The only intended observable deltas are (a) reused subtrees on memo hits, (b) the two
new `FrameMetrics` fields, and (c) the new diagnostic report + guidance doc.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: No new dependencies. Edits `FS.Skia.UI.Controls`
(`RetainedRender` per-identity memo store; the representative `Control.fs` DataGrid
projection seam; a new `Diagnostics` stability-report `val`) and
`FS.Skia.UI.Controls.Elmish` (the `FrameMetrics` record + threading through the retained
step / `Perf.runScript` / `OnFrameMetrics`). Consumes existing `ControlId`,
`RetainedRender.step`, `WorkReductionRecord`, `Style.resolve`, `gridGeom`.
**Testing**: Expecto + FsCheck. Memo seam + memo-on/memo-off scene parity +
stability-diagnostic tests in `tests/Controls.Tests` (reaching the internal seam via
`InternalsVisibleTo "Controls.Tests"`); `MemoHitCount`/`MemoMissCount` corpus goldens in
`tests/Elmish.Tests` over `ControlsElmish.Perf.runScript`; the standing Scene-parity
golden suite under `Dev` for at-rest byte-identity; FAKE targets.
**Target Platform**: Windows and Linux (no platform-specific code; no
Vulkan/Skia/visual-output change).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification — Tier 1 (contracted change).** Two new **public**
`FrameMetrics` fields (`ControlsElmish.fsi`), a new **public** stability-diagnostic
`val` (`Controls` `Diagnostics.fsi`), and a new **internal** memoization seam
(`Controls` `.fsi`, reached via `InternalsVisibleTo`). The top-level surface baseline
and per-package baselines move; the full artifact chain applies (`.fsi` updates,
baseline regeneration, test evidence, XML-doc). `Route` escalates to the
**controls-public-surface** tier.

**Principle compliance.**
- *I (Spec→FSI→Tests→Impl)*: the memo seam, the `FrameMetrics` fields, and the
  diagnostic `val` are drafted in `.fsi` signature form first and exercised from
  FSI/tests; the memo-on/memo-off parity test and the hit/miss count test are the
  failing-first proofs.
- *II (Visibility in `.fsi`)*: the memo seam + cache types are `internal` (declared in
  the owning `Controls` `.fsi`, hidden from consumers); the metric fields and the
  diagnostic `val` are public, declared in their `.fsi`. No access modifiers in `.fs`.
- *III (Idiomatic simplicity)*: a plain `Map<ControlId, MemoEntry>` carried in the
  retained state and an equality check on the dependency value; a recursive two-build
  attribute/event comparison for the diagnostic. No SRTP/reflection/type-providers. The
  dependency value is a plain structural value (no object-identity tricks); any
  `mutable` count accumulator is disclosed at the use site.
- *IV (Elmish/MVU boundary)*: unchanged — `Update`, effects, subscriptions, commands,
  interpreter are untouched; only *whether a pure subtree is recomputed or reused*
  changes. Dispatch outcomes byte-identical (FR-014). The memo cache lives in the
  existing retained interpreter-edge state, not in `update`.
- *V (Synthetic disclosure)*: none expected — parity uses the real always-miss oracle
  over real control trees; the counts are the real retained-step results; the
  diagnostic runs over real fixture trees with a real injected always-new input. Any
  unavoidable stub returns to task review for `[S]` disclosure.
- *VI (Test evidence)*: memo-on/memo-off scene parity, hit/miss counts, no-staleness on
  a real-input change, idle = 0/0, and stability-diagnostic flag/no-flag all fail before
  / pass after; no assertion weakening.
- *VII (Observability)*: `MemoHitCount`/`MemoMissCount` make a regression that defeats
  reuse (e.g. an always-new dependency) visible as misses in the goldens instead of
  silent CPU; the stability-diagnostic report names the offending control + input.

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**`, sample, or command-surface change; the
  memoization seam, the metric fields, and the diagnostic do not alter
  `.template.config/template.json`. (The merge-time template package-pin bump is the
  standard post-merge step, not a content change in this feature.)
- **Dependency impact**: N/A — no new package; `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` are unchanged.
- **Command-surface impact**: No new gate (the stability diagnostic is report-only this
  rung, clarified). Escalated **controls-public-surface** set because the Controls and
  `Controls.Elmish` `.fsi` surfaces change; run `Route` first and obey its printed
  minimal list. `RefreshSurfaceBaselines` regenerates the top-level + per-package
  baselines after the `FrameMetrics`/`Diagnostics`/seam additions; the `Perf.runScript`
  corpus goldens are regenerated (`PERF_CORPUS_REGEN=1`) to carry the two new metric
  fields. FAKE-backed commands run sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A to behaviour — generated default/minimal contents
  and generated `Dev` behaviour are unchanged. Generated projects gain the two new
  public `FrameMetrics` fields transitively (additive; `OnFrameMetrics` default stays
  `ignore`, byte-identical at rest). The internal memo seam is not surfaced into
  generated projects.
- **Evidence paths**: memo seam + memo-on/memo-off scene-parity (incl. real-input-change
  no-staleness) + idle-0/0 tests under `tests/Controls.Tests/Feature113*.fs`; the
  `MemoHitCount`/`MemoMissCount` corpus goldens under
  `specs/109-perf-metrics-baseline/readiness/perf-corpus/*.golden.txt` (regenerated) +
  asserted in `tests/Elmish.Tests/Feature113*.fs`; stability-diagnostic tests under
  `tests/Controls.Tests/Feature113*.fs`; the stable-props guidance page at
  `docs/controls/stable-props.md`; at-rest byte-identity via the standing Scene-parity
  suite under `Dev`; skill-loading evidence in `readiness/skill-loading-evidence.md`;
  the window-visibility not-applicable set; `readiness/evidence-audit.md` (verdict
  token); generated-validation package-resolution tokens; surface/per-package baselines
  under `readiness/surface-baselines/` + `readiness/per-package-surface/`.
- **`.fsi` / contract impact**: **Breaking** `ControlsElmish.fsi` `FrameMetrics` change —
  two new public fields `MemoHitCount` / `MemoMissCount` (with XML-doc; doc-preservation
  gate). A new **public** stability-diagnostic `val` in `Controls` `Diagnostics.fsi`
  (returning `ControlDiagnostic list`). A new **internal** memo seam + cache/entry types
  `val internal` / `type internal` in the owning `Controls` `.fsi` (consumed by control
  internals, reached via `InternalsVisibleTo`). No public consumer
  `Control.memo`/`Widget.memo` primitive (deferred). The top-level surface baseline
  changes (the `FrameMetrics` fields); per-package Controls + Controls.Elmish baselines
  regenerate.
- **MVU/effect boundary**: Unchanged (preserved, not modified). `Model`/`Msg`/`Effect`/
  `init`/`update`/interpreter are untouched; the memo cache lives in the retained
  interpreter-edge state. Only *whether a pure subtree is recomputed or reused* changes;
  dispatch outcomes are byte-identical.
- **Synthetic evidence**: None planned. Parity oracle = the real always-miss mode; the
  counts = the real retained-step results; the diagnostic = a real two-build comparison
  with a real injected always-new input. Any unavoidable stub returns to task review for
  `[S]` disclosure.
- **Test evidence**: failing-first — memo hit on a stable-dependency steady-state frame;
  memo miss on a changed dependency / cold frame; memo-on/memo-off scene byte-identity
  over the corpus including a real-input-change no-staleness case; `MemoHitCount`/
  `MemoMissCount` goldens (steady-state hits vs perturbed/cold misses; idle 0/0);
  stability-diagnostic flags an injected always-new input and reports nothing for a
  stable tree; at-rest byte-identity via the Scene-parity suite.
- **Observability**: `MemoHitCount`/`MemoMissCount` (public, deterministic, golden-
  asserted via `Perf.runScript`, plus live `OnFrameMetrics`) + the stability-diagnostic
  report (public, `Controls.Tests`-asserted). No unsupported-environment message change.
- **Deferred scope**: Phase 5 only. OUT: a **public consumer `Control.memo`/
  `Widget.memo`** primitive (deferred); viewport **virtualization** (Phase 6); damage
  rects / Skia picture / paint caches (Phase 7); text / layout-boundary caches (Phase 8);
  `SkiaViewer` backend / render-thread review (Phase 9); any **enforced stability gate**
  (the diagnostic is report-only this rung). Full 52-control migration to memoized
  transforms is OUT — only a representative site (DataGrid projection) is memoized. No
  renderer rewrite, no Avalonia/WPF redesign, no platform/release/distribution scope.
  Features 110/111/112 are unchanged.

**Gate result: PASS.** No unjustified violations. Tier 1 obligations (`.fsi`, baselines,
tests, docs) are enumerated above and carried into Phase 1.

## Project Structure

Edited / added paths for this feature:

```
src/Controls/
  RetainedRender.fsi          # internal memo cache/entry types + a memo slot on RetainedUiState (or a
                              #   sibling memo map on RetainedRender); val internal memoize seam (+ XML-doc)
  RetainedRender.fs           # thread the memo cache through `step`; populate MemoHit/Miss counts on the
                              #   step result; always-miss switch (FR-008)
  Control.fs                  # wrap the DataGrid row/column projection (`gridGeom`/cells projection) in the
                              #   memo seam keyed by ControlId + a deterministic dependency value
  Diagnostics.fsi             # new public val: stability-diagnostic report (+ XML-doc)
  Diagnostics.fs              # the two-build attribute/event-equality comparison flagging always-new inputs

src/Controls.Elmish/
  ControlsElmish.fsi          # FrameMetrics gains public MemoHitCount / MemoMissCount (+ XML-doc)
  ControlsElmish.fs           # thread the counts from the retained step into FrameMetrics (zero record +
                              #   every per-frame construction site); Perf.runScript + OnFrameMetrics surface

docs/controls/
  stable-props.md             # author-facing stable-props guidance page (reuse-breaking patterns + fixes)

readiness/surface-baselines/  +  readiness/per-package-surface/
  FS.Skia.UI.Controls*.txt    # regenerated (RefreshSurfaceBaselines): top-level (FrameMetrics fields) +
                              #   per-package (Diagnostics val, internal memo seam)

specs/109-perf-metrics-baseline/readiness/perf-corpus/
  *.golden.txt                # regenerated (PERF_CORPUS_REGEN=1) to carry MemoHitCount / MemoMissCount

tests/Controls.Tests/
  Feature113MemoSeamTests.fs        # FR-001/004/005 hit/miss/cold; reference-reuse on a hit
  Feature113MemoParityTests.fs      # FR-006/007 memo-on vs memo-off scene byte-identity + no-staleness
  Feature113StabilityDiagTests.fs   # FR-011/012 stable→no findings; injected always-new→flagged

tests/Elmish.Tests/
  Feature113MemoMetricsTests.fs     # FR-009/010 MemoHit/Miss goldens (steady-state hits, perturbed/cold
                                     #   misses, idle 0/0) over Perf.runScript

specs/113-view-memoization/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/memoization-seam.md  contracts/stability-diagnostic.md
  readiness/   # evidence-audit.md, skill-loading-evidence.md, byte-identity authority, window-visibility set
```

**Key seams (file:line anchors):**
- Representative memoized site: DataGrid projection `gridGeom` `Control.fs:550` (the **sole**
  memoized site this rung); the per-kind `Style.resolve` calls
  `Control.fs:594/631/667/704/...` are the **deferred** future site (not wired this rung).
- Retained per-identity state to extend with the memo cache: `RetainedUiState`
  `RetainedRender.fsi:67`; `RetainedRender` carrier `:76`; `step` `:179`.
- Work-record threading precedent (how `RemeasuredNodeCount` reaches `FrameMetrics`):
  `WorkReductionRecord` `RetainedRender.fsi:97`; `lastWorkReduction`
  `ControlsElmish.fs:856`/`:993`.
- `FrameMetrics` type + every construction site: `ControlsElmish.fsi:68`; `.fs` `zero`
  record `:1320`; per-frame records `:1357`, `:1404`, plus the model/key/idle branches.
- Deterministic corpus driver: `ControlsElmish.Perf.runScript` `ControlsElmish.fs:1235`;
  corpus goldens + test `tests/Elmish.Tests/Feature109CorpusTests.fs`.
- Diagnostic precedent (a pure report `val`, report-only, asserted in tests):
  101's `layoutDriftReport` (`tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs`).

## Phase 0: Research

See [research.md](./research.md). Resolves: (a) the memo cache key — **`ControlId`**
(the report/clarification key) vs the retained `RetainedId`, and why a `Map<ControlId,
MemoEntry>` carried in the retained state is correct (and why a too-coarse/unstable key
is caught by the memo-on/memo-off parity test, not shipped as staleness); (b) the
**dependency value** shape — a deterministic structural value capturing every input that
can change the memoized subtree (for the DataGrid projection: the cells/columns/theme
inputs), equality as the sole reuse condition; (c) why the **DataGrid row/column
projection** is the load-bearing representative site (report #1, genuinely expensive,
produces a subtree) and `Style.resolve` is a candidate secondary; (d) the always-miss
**parity oracle** switch (FR-008) and why it is the authority for "dependency not too
coarse"; (e) how `MemoHitCount`/`MemoMissCount` thread from the retained step into
`FrameMetrics` on the deterministic path (mirroring `RemeasuredNodeCount`); (f) the
**stability-diagnostic** algorithm — a two-build attribute/event-equality comparison
that distinguishes a real semantic change from an always-new-but-equivalent input, and
why it is report-only (event closures are legitimate) this rung.

## Phase 1: Design & Contracts

- [data-model.md](./data-model.md): the internal `MemoEntry` (dependency value + cached
  lowered subtree) and `MemoCache` (`Map<ControlId, MemoEntry>`), the memo store's
  placement in `RetainedRender`/`RetainedUiState`, the `MemoOutcome` (Hit/Miss) and how
  it aggregates into `MemoHitCount`/`MemoMissCount`, the always-miss switch, the
  `FrameMetrics` field additions, and the stability-diagnostic finding shape.
- [contracts/memoization-seam.md](./contracts/memoization-seam.md): the internal memo
  seam contract — hit reuses the prior subtree (reference-equal where guaranteed) without
  recomputing; miss recomputes + stores; never reuses across an unequal/unknown
  dependency; memo-on ≡ memo-off rendered scene (FR-006); no staleness on a real input
  change (FR-007); the count semantics (idle = 0/0).
- [contracts/stability-diagnostic.md](./contracts/stability-diagnostic.md): the public
  stability-diagnostic contract — given a (sub)tree built twice, flag attributes/events
  that compared unequal despite no semantic change, naming control + input; no findings
  for a stable tree; report-only (not a gate).
- [quickstart.md](./quickstart.md): how to run the memo seam / parity / metrics /
  stability-diagnostic tests, regenerate the corpus goldens (`PERF_CORPUS_REGEN=1`) and
  surface baselines (`RefreshSurfaceBaselines`), read the stable-props guidance page, and
  run the escalated gate set.
- Agent context update: `AGENTS.md` SPECKIT marker repointed to this plan.

## Phase 2: Planning complete

Stop after design. `tasks.md` is produced by `/speckit.tasks`.
