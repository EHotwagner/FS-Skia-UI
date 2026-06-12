# Implementation Plan: Narrow Runtime Visual-State Updates (Targeted Hover/Focus/Press Stamping)

**Branch**: `112-narrow-visual-state-stamping` | **Date**: 2026-06-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/112-narrow-visual-state-stamping/spec.md`

## Summary

The live host stamps runtime visual state across the **whole** control tree every
frame it paints: `renderRetained` calls `ControlRuntime.applyRuntimeVisualState`
(`ControlRuntime.fs:233`) at `ControlsElmish.fs:907` (first frame) and `:914` (each
step), and that function recursively reconstructs **every** node to stamp its derived
`VisualState` — even when only the control that gained or lost hover/focus/press
changed (source report Phase 4 gap).

**Technical approach (Phase 4 of the performance report, "Do next" #2):** add an
internal **targeted** stamp in `ControlRuntime` and wire it into the live
model-unchanged hot path, re-stamping only the controls whose **final** visual state
changed between the previous and current runtime model.

1. Add `ControlRuntime.applyRuntimeVisualStateTargeted` (internal) — a **parallel
   walk** of the previous frame's already-stamped tree (`retained.Root.Control`) and
   the current un-stamped view tree (feature 111's `viewFor` output, same structure
   because the model is unchanged), zipped node-for-node. For each node it computes
   the **final** state under each model — `finalState M = if consumer-set ≠ Normal
   then consumer-set else deriveVisualState M id` — and:
   - if `finalState cur = finalState prev` **and** no descendant changed → **reuse the
     previous-frame node instance untouched** (`0` nodes touched in that subtree);
   - else → rebuild this node from the **fresh** view node with `finalState cur`
     stamped (so the stamp is clean), counting `+1` touched.
   It returns the stamped tree + `RuntimeStateTouchedNodeCount` (the rebuilt count).
2. Keep the existing whole-tree `applyRuntimeVisualState` as the **parity oracle /
   fallback** (FR-005): the live host uses the targeted stamp only on the
   model-unchanged path (a host-owned hover/focus/press repaint); a model-changing or
   first frame re-views and uses the full stamp (the whole tree is re-built anyway),
   and any structural misalignment falls back to the full stamp (FR-006).
3. Surface `RuntimeStateTouchedNodeCount` as an **internal** count (clarified): the
   targeted result carries it (asserted in `Controls.Tests`), and the live host
   surfaces it best-effort — **no** public `FrameMetrics` field, no corpus-golden
   churn.

This is a **hot-path stamp-mechanism change only** (FR-008): the targeted stamp
produces the **byte-identical** stamped tree the full walk would (a reused node from
last frame already carries `finalState prev = finalState cur`; a rebuilt node is the
fresh node with `finalState cur` — exactly the oracle's output), so at-rest rendered
output, geometry, focus/keyboard semantics, and every dispatch outcome stay
byte-identical. The only intended observable deltas are fewer nodes rebuilt to stamp
and the new internal `RuntimeStateTouchedNodeCount`.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: No new dependencies. Edits `FS.Skia.UI.Controls`
(`ControlRuntime`) and `FS.Skia.UI.Controls.Elmish` (the live `renderRetained`
stamping seam). Consumes existing `ControlRuntimeModel` / `deriveVisualState` /
`setVisualState` / `ControlInternals.visualStateOf`.
**Testing**: Expecto + FsCheck (targeted-vs-oracle scene parity, touched-node count,
precedence, no-change=0) in `Controls.Tests`, reaching the internal seam via
`InternalsVisibleTo "Controls.Tests"`; the standing Scene-parity golden suite under
`Dev` for at-rest byte-identity; FAKE targets.
**Target Platform**: Windows and Linux (no platform-specific code; no
Vulkan/Skia/visual-output change).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification — Tier 1 (contracted change).** A new `val internal` (+ an
internal result type) is added to `ControlRuntime.fsi`, so the Controls package
per-package surface baseline moves; the full artifact chain applies (`.fsi` update,
surface + per-package baseline regeneration, test evidence, XML-doc). `Route`
escalates to the **controls-public-surface** tier.

**Principle compliance.**
- *I (Spec→FSI→Tests→Impl)*: the new internal `.fsi` seam is drafted in signature
  form first and exercised from FSI/`Controls.Tests`; the parity test (targeted vs
  full oracle) is the failing-first proof.
- *II (Visibility in `.fsi`)*: the targeted stamp + its result type are `internal`
  (declared in `ControlRuntime.fsi`, hidden from consumers, reached via
  `InternalsVisibleTo`); no access modifiers in `.fs`. No public signature changes.
- *III (Idiomatic simplicity)*: a plain recursive parallel walk over the two trees
  with a `mutable`/accumulated touched count; the `setVisualState`/`visualStateOf`
  primitives are reused, not re-implemented. No SRTP/reflection/type-providers.
- *IV (Elmish/MVU boundary)*: unchanged — `Update`, effects, subscriptions, commands,
  interpreter are untouched; only the per-frame visual-state *stamp mechanism*
  changes. Dispatch outcomes byte-identical (FR-008).
- *V (Synthetic disclosure)*: none expected — parity uses the real preserved full-tree
  oracle, the count is the real targeted result, and precedence uses a real
  consumer-set control. If any task needs a stub it is marked `[S]` with disclosure.
- *VI (Test evidence)*: targeted-vs-oracle parity + touched-count + precedence +
  no-change=0 fail before / pass after; no assertion weakening.
- *VII (Observability)*: `RuntimeStateTouchedNodeCount` makes a regression to
  whole-tree stamping observable (the count jumps to the node count) rather than
  silent; an unexpected misalignment degrades to the correct full-stamp oracle.

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**`, sample, or command-surface change;
  the framework-internal stamp mechanism does not alter
  `.template.config/template.json`. (The merge-time template package-pin bump is the
  standard post-merge step, not a content change in this feature.)
- **Dependency impact**: N/A — no new package; `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` are unchanged.
- **Command-surface impact**: No new gate. Escalated controls-public-surface set
  because of the `ControlRuntime.fsi` change; run `Route` first and obey its printed
  list. `RefreshSurfaceBaselines` regenerates the surface + per-package baselines
  after the internal-seam addition. FAKE-backed commands run sequentially in the
  deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A — generated default/minimal contents, selected
  Controls guidance, and generated `Dev` behaviour are unchanged; the internal
  `ControlRuntime` stamp is not surfaced into generated projects (no public API
  delta).
- **Evidence paths**: targeted-vs-oracle parity + touched-count + precedence tests
  under `tests/Controls.Tests/Feature112*.fs`; at-rest byte-identity via the standing
  Scene-parity suite under `Dev`; before/after touched-node delta recorded in
  `specs/112-narrow-visual-state-stamping/readiness/`; skill-loading evidence in
  `readiness/skill-loading-evidence.md`; `readiness/evidence-audit.md` (verdict
  token); generated-validation package-resolution tokens; surface/per-package
  baselines under `readiness/surface-baselines/` + `readiness/per-package-surface/`.
- **`.fsi` / contract impact**: `ControlRuntime.fsi` gains an internal result type
  (the stamped tree + `RuntimeStateTouchedNodeCount`) and `val internal
  applyRuntimeVisualStateTargeted` with XML-doc (doc-preservation gate). The existing
  `applyRuntimeVisualState` (full oracle) is **retained unchanged**. No public
  signature changes; the public Controls surface baseline (type-level) does not
  change, but the per-package Controls surface gains the internal seam (regenerated).
- **MVU/effect boundary**: Unchanged (preserved, not modified). `Model`/`Msg`/
  `Effect`/`init`/`update`/interpreter are untouched; this feature changes only how
  the host stamps per-frame runtime visual state, not the transition algebra.
- **Synthetic evidence**: None planned. Parity oracle = the real preserved full-tree
  stamp; the count = the real targeted result; precedence = a real consumer-set
  control. Any unavoidable stub returns to task review for `[S]` disclosure.
- **Test evidence**: failing-first targeted-vs-oracle scene-parity test across
  hover-move / focus-move / press transitions over representative trees; a
  touched-node-count test (localized change « N, no-change = 0); a precedence test
  (consumer-set `Disabled`/`Selected` wins under targeting); an at-rest byte-identity
  confirmation via the Scene-parity suite.
- **Observability**: `RuntimeStateTouchedNodeCount` (internal, deterministic,
  `Controls.Tests`-asserted) + best-effort live surfacing. No public `FrameMetrics`
  field (clarified). No unsupported-environment message change.
- **Deferred scope**: Phase 5+ is OUT — view/control memoization + stable-dependency
  diagnostics (Phase 5), viewport virtualization (Phase 6), damage rects / picture /
  paint caches (Phase 7), text / layout-boundary caches (Phase 8), `SkiaViewer`
  backend review (Phase 9). The full-tree stamp is **not** removed (preserved as
  oracle/fallback). Narrowing the reconciler **diff** (vs the stamp) is out of scope —
  this feature narrows only the stamp. No renderer rewrite, no Avalonia/WPF redesign,
  no platform/release/distribution scope. Features 110/111 are unchanged.

**Gate result: PASS.** No unjustified violations. Tier 1 obligations (`.fsi`,
baselines, tests, docs) are enumerated above and carried into Phase 1.

## Project Structure

Edited / added paths for this feature:

```
src/Controls/
  ControlRuntime.fsi          # internal result type + val internal applyRuntimeVisualStateTargeted (+ XML-doc)
  ControlRuntime.fs           # the targeted parallel-walk stamp; reuse unchanged subtrees, rebuild changed paths, count touched

src/Controls.Elmish/
  ControlsElmish.fs           # renderRetained: targeted stamp on the model-unchanged path; full stamp (oracle) on
                              #   model-change/first frame; store lastRuntimeModel; surface RuntimeStateTouchedNodeCount best-effort

readiness/surface-baselines/  +  readiness/per-package-surface/
  FS.Skia.UI.Controls.*.txt   # regenerated (RefreshSurfaceBaselines) — per-package Controls surface gains the internal seam

tests/Controls.Tests/
  Feature112TargetedStampParityTests.fs   # FR-005 targeted vs full-tree oracle scene parity (hover/focus/press)
  Feature112TouchedCountTests.fs          # FR-001/FR-004/FR-007 touched-node count (« N; no-change = 0)
  Feature112PrecedenceTests.fs            # FR-003 consumer-set/Disabled precedence preserved

specs/112-narrow-visual-state-stamping/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/targeted-stamp.md
  readiness/   # evidence-audit.md, skill-loading-evidence.md, touched-node delta, byte-identity authority
```

**Key seams (file:line anchors):**
- Full-tree stamp to companion: `ControlRuntime.applyRuntimeVisualState`
  `ControlRuntime.fs:233`; `deriveVisualState` `:203`; `setVisualState` `:222`.
- Live stamping seam to narrow: `renderRetained` `ControlsElmish.fs:903-920`
  (the `None` first-frame branch `:905-911` and the `Some prev` step branch `:912-920`).
- Runtime model assembly: `assembleRuntimeModel` `ControlsElmish.fs:866`.
- View cache (feature 111, provides the fresh un-stamped tree + the model-unchanged
  signal): `viewFor` / `lastView` `ControlsElmish.fs` (the `obj.ReferenceEquals(model,
  cachedModel) && size = cachedSize` predicate).

## Phase 0: Research

See [research.md](./research.md). Resolves: (a) the exact changed-identity set and why
the **final-state** parallel walk (not a `{prev∪cur}` id set) is the correct,
parity-preserving formulation; (b) why the targeted stamp operates on the previous
**stamped** tree + the current **fresh** tree (and why the model-unchanged gate makes
them structurally align); (c) the consumer-vs-derived precedence handling (read from
the fresh node, unambiguous); (d) the fallback boundary (model change / first frame /
misalignment → full stamp); (e) why the count is internal and deterministically
testable without a live window.

## Phase 1: Design & Contracts

- [data-model.md](./data-model.md): the internal `RuntimeStampResult` (stamped tree +
  `RuntimeStateTouchedNodeCount`), the targeted-stamp read set (prev model, cur model,
  prev-stamped tree, fresh tree), the preserved full-tree oracle, and the
  classification of which frames take the targeted vs full path.
- [contracts/targeted-stamp.md](./contracts/targeted-stamp.md): the internal
  targeted-stamp contract, its scene-parity obligation vs. the oracle, the precedence
  rule, the no-change=0 rule, and the fallback rule.
- [quickstart.md](./quickstart.md): how to run the parity / count / precedence tests,
  observe the touched-node delta, and run the escalated gate set.
- Agent context update: `AGENTS.md` SPECKIT marker repointed to this plan.

## Phase 2: Planning complete

Stop after design. `tasks.md` is produced by `/speckit.tasks`.
