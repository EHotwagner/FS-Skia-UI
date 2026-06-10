# Implementation Plan: Retained-Tree Reconciliation on the Render Path — Wiring the Parked Keyed Reconciler for Cross-Frame Control Identity & Partial Updates (Roadmap E2)

**Branch**: `091-wire-reconciler-render-path` | **Date**: 2026-06-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/091-wire-reconciler-render-path/spec.md`

## Summary

Feature **067** already shipped the keyed VDOM diff — `module internal Reconcile`
(`src/Controls/Reconcile.fsi`): `diff`/`apply`, the `NodePatch`/`UpdatePatch`/`ChildOp`/
`ReconcileResult` operation set, pure/total/deterministic, round-trip property-tested over ≥1,000
generated cases, and **deliberately parked / unwired**. This feature (**E2**, the roadmap's "linchpin")
is the **wiring + invariant-preservation** step that promotes that asset onto the live render path. It is
**not a new algorithm**.

Today the live loop rebuilds the whole tree every frame: `runInteractiveApp` calls
`Control.renderTree host.Theme size (host.View size model)` on every interaction
(`ControlsElmish.fs:349/376/391`) and `SkiaViewer`'s `dispatchHostMsg` recomputes `currentScene <-
host.View currentModel` after every dispatched message (`SkiaViewer.fs:2364`, also `:2320`/`:2437`).
Per-control identity is derived structurally — `let id = c.Key |> Option.defaultValue path`
(`Control.fs:1052`) — so an unrelated change that **shifts a control's position** changes its
path-derived `ControlId`, which is exactly why the host's per-control state keyed by that id
(`focusedText : ControlId option`, `textModels : Map<ControlId, TextInputModel>`,
`ControlRuntime.FocusedControl`) is **lost across an unrelated re-render** (the ControlsShowcase2
"shortcuts blocked after clicks" symptom). The reconciler's key-first-then-positional matching is the
fix: it tells the render path which next-frame control **is the same** as a prev-frame control even when
its position moved, giving identity-bearing state a stable hook.

Approach, four FR clusters:

1. **RETAINED-PATH-1 (FR-001/002/003).** Introduce a framework-internal **retained render structure** that
   persists between frames in the host loop's existing mutable-ref state. Each frame: hold the previous
   lowered `Control<'msg>` tree, compute `Reconcile.diff prev next`, surface `Diagnostics`, and drive the
   next render from the patch — `Keep` subtrees reuse the retained render fragment; `Update`/`Replace`/
   `ChildInsert` recompute. The retained structure carries each control's **stable identity** (the diff's
   match, not the raw path id), and per-control UI state (focus, a per-control animation clock) is re-keyed
   to that stable identity so it survives an unrelated re-render. Mutation is confined to this internal
   structure at the interpreter edge; the consumer `view : 'model -> Control<'msg>` contract is untouched.

2. **PARTIAL-UPDATE-1 (FR-004/005).** The patch drives partial re-measure/re-paint: only `Update`/`Replace`/
   inserted subtrees are re-evaluated through `renderTree`'s layout/paint; `Keep`/`ChildKeep` subtrees reuse
   their cached `LayoutNode`/Scene fragment. Output MUST be **byte-for-byte identical to a full rebuild of
   `next`** (golden parity) — partial update is an internal efficiency, never a visible difference. The
   conflict-resolution rule (spec): **correctness wins** — on any mismatch the path falls back to the
   full-rebuild-equivalent result.

3. **INVARIANTS-LIVE-1 (FR-006/007).** Promote the 067 property suite (`tests/Controls.Tests/
   ReconcileTests.fs`) to exercise the **wired** path: round-trip (wired output ≡ full rebuild of `next`),
   determinism, totality (incl. duplicate-key/empty-tree → `KeyCollision` diagnostic, never a throw),
   identity-at-rest (no spurious patch/re-render on structurally identical frames). `ReconcileResult.
   Diagnostics` surface through the existing `ControlDiagnostic` channel at the wiring boundary.

4. **SCOPE-1 (FR-008/009/010).** Additive to the consumer surface — an existing MVU consumer's
   `view`/`update`/`Init`/`Subscriptions` needs **zero** changes to benefit. No E3 style layer, no E4
   focus/traversal model, no E5 lookless slots, and no rejected non-goal (XAML, data binding,
   dependency/attached properties, lookless `ControlTemplate`, CSS selectors). Update the
   `fs-skia-reconciliation` skill **Disposition** from "parked, not wired" → "wired on the render path"
   and regenerate the `.claude` mirror (`SkillSyncCheck`).

`Reconcile` stays `module internal` (no promotion to public just to wire it — consistent with 067 SC-005
and the `module internal SceneRenderer` precedent). The default behavior is the wired retained path (not a
per-call opt-in flag); a safety flag is introduced additively only if implementation experience warrants
it (research D5).

## Technical Context

**Language/Version**: F# / .NET (`net10.0`). Runtime packages `FS.Skia.UI.Controls`,
`FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.SkiaViewer`; governance assembly `FS.Skia.UI.Build`
(`build/Governance/**`).
**Primary Dependencies**: None new. Existing seams only — `Reconcile.diff`/`apply` +
`NodePatch`/`ChildOp` (feature 067, `src/Controls/Reconcile.fsi`); `Control.renderTree`/`render` +
structural id derivation + `ControlRenderResult` (`src/Controls/Control.fs:996/1014`,
`Types.fsi:285`); `ControlRuntimeModel.FocusedControl` (`src/Controls/ControlRuntime.fsi:42`);
`runInteractiveApp` host loop + its mutable-ref retained state (`src/Controls.Elmish/ControlsElmish.fs:331`);
the `SkiaViewer` `dispatchHostMsg` repaint (`src/SkiaViewer/SkiaViewer.fs:2364`); Scene-level
`Animation`/`Tween`/`AnimationState`/`applyAt` for the per-control clock (feature 073,
`src/Scene/Animation.fsi`); the 090 responds-proof primitive reused as the survives-proof.
**Testing**: Expecto + FsCheck (promote `tests/Controls.Tests/ReconcileTests.fs` to the wired path;
new wired-path round-trip/determinism/totality/identity-at-rest + golden-diff parity + measured
work-reduction + focus/animation survives-proof), FAKE targets, FSI render-target captures, generated
product evidence as needed.
**Target Platform**: Windows and Linux. All correctness/perf evidence is capturable headless/offscreen
(render-target PNG golden diff + a node-count instrument) — no live Vulkan window required for evidence
([[fs-skia-evidence-mode]], render-only honesty). The focus/animation survives-proof reuses the 090
before/after render-diff mechanism, which is offscreen-capturable.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: **Update required only if a public seam is added (regenerated artifacts, no
  manifest change).** The default plan adds **no** public `.fsi` (the retained path is internal wiring;
  `Reconcile` stays `module internal`), so no api-surface delta. If research D5 lands a small additive
  public seam (a `ViewerDiagnosticsOptions` work-metric field or a safety flag), it re-emits into the
  published api-surface tree (`docs/api-surface/**` → `template/base/docs/api-surface/**`) via
  `RefreshSurfaceBaselines`, **not** hand-edited. No `.template.config/template.json` manifest change
  either way (no new top-level file class; the api-surface dir already ships). No template source/sample
  change.
- **Dependency impact**: **N/A — no dependency change.** No new package, no `Directory.Packages.props`
  edit, no `docs/dependencies.md` / `DependencyReport` change. All work reuses already-referenced seams
  (`Reconcile`, `Control.renderTree`, `ControlRuntime`, Scene `Animation`).
- **Command-surface impact**: **Update required (output/behavior of existing targets; no new wrapper, no
  new gate).** `RefreshSurfaceBaselines` regenerates the `.claude` `fs-skia-reconciliation` skill mirror
  (and any moved surface baseline if a seam is added). The serialized six-target order changes output and
  must be re-run. No new FAKE target; no `validation.contract.yml` routing-row change (no gate added).
  FAKE-backed commands share `.fake` state — run sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: **Update required (additive, behavior-only by default).** Generated
  products inherit the internally-wired retained render path automatically — the live loop becomes
  O(changed-subtree) and focus/animation survive unrelated re-renders, with **no** generated source,
  `Dev` behavior, placeholder-scan, or excluded-history-scan change and **no** consumer rewrite (FR-008).
  Where the disposition update touches the `.agents` reconciliation skill, generated projects inherit the
  regenerated guidance. If research D5 adds a public seam, generated `docs/api-surface/**` is recaptured.
- **Evidence paths**: Real evidence — (a) **golden-diff parity** PNGs proving wired output ≡ full rebuild
  of `next` for every test scene (zero diff), e.g. `readiness/retained-parity/{wired,rebuild}.png` +
  `retained-parity.txt`; (b) a **measured per-frame work-reduction** record for a localized single-control
  change vs the redraw-the-world baseline (re-measured/re-painted node counts bounded by the changed
  subtree, not N), e.g. `readiness/partial-update/work-reduction.txt`; (c) a **focus/animation-survives**
  proof pair reusing the 090 before/after render-diff primitive (`readiness/survives-proof/{before,after}.png`
  + `survives-proof.txt`) that a rebuild-every-frame/inert baseline fails; (d) Expecto logs for the wired
  round-trip/determinism/totality/identity-at-rest properties (≥1,000 cases) + the `KeyCollision`
  diagnostics-surfacing test; (e) `readiness/skill-sync-check.md` byte identity for the edited
  `.agents`↔`.claude` `fs-skia-reconciliation` skill; (f) recaptured per-package + `docs/api-surface`
  baselines **only if** a seam is added; (g) the serialized six-target logs.
- **`.fsi` / contract impact**: **Primarily internal wiring; default = zero public-surface delta.**
  `Reconcile` stays `module internal`. The behavior of the existing host/render path changes internally
  even where its signatures do not — documented honestly in the affected `.fsi` doc comments
  (`ControlsElmish.fsi` host-loop doc, `Control.fsi` render doc: "next frame is produced by diffing
  against the retained previous tree"). The new retained-render structure is `module internal`, reached
  by tests via `[<assembly: InternalsVisibleTo("Controls.Tests")>]` — no baseline entry. If research D5
  introduces an additive public seam, it is the **only** baseline movement and per-package + api-surface
  baselines are recaptured then. No signature removed; consumer surface unchanged (Tier 1 by routing,
  but minimal/zero public delta).
- **MVU/effect boundary**: **In scope — internal render/redraw path, consumer MVU surface unchanged.**
  Model = consumer `'model`; `Msg` = consumer `'msg`; `host.Update` folding is **unchanged** (no new
  effects/subscriptions/interpreter behavior). The retained tree + the diff-and-apply step are
  framework-internal **mutable state at the interpreter edge** (the `runInteractiveApp` / `dispatchHostMsg`
  closure), exactly where the existing `pointerState`/`textModels`/`currentScene` refs already live —
  `update`/`view` stay pure (constitution III: mutation at the edge, disclosed at the use site). Observable
  behavior asserted by the gates is round-trip equality to a full rebuild, deterministic output, and no
  spurious re-render at rest — not the absence of internal mutation (spec FR-002↔FR-006 resolution).
- **Synthetic evidence**: **None planned.** The wired-path tests run the **real** retained render path over
  **real** generated `(prev, next)` control trees (the 067 FsCheck generators) and **real** `renderTree`
  layout/paint; golden parity is a real PNG diff; the work-reduction metric is a real node count; the
  survives-proof is a real before/after render diff on the real host loop. No mocks/fakes/placeholders
  anticipated. A genuine error-path fixture (e.g. a deliberately duplicate-keyed sibling list to exercise
  `KeyCollision` surfacing) gets full Principle-V `[S]`/`[SEH]` disclosure if it exercises only literal
  malformed input ([[accepted-seh-stops-propagation]]).
- **Test evidence**: Failing-first semantic tests — (a) two successive renders differing only in a region
  unrelated to keyed control K: the wired path matches K (`ChildKeep`/`Update`, **not** `Replace`) and K's
  identity-bearing state survives; a control whose `Kind` changed is `Replace`d (no false identity)
  (FR-003/SC-001); (b) focus (`ControlRuntime.FocusedControl`) and an in-flight per-control animation clock
  survive an unrelated model update — and a rebuild-every-frame/inert baseline **fails** the same proof
  (FR-003/SC-002); (c) a localized single-leaf change re-measures/re-paints only the changed subtree —
  measured node count bounded by the subtree, not N (FR-004/SC-003); (d) golden-diff parity: wired output ≡
  full rebuild of `next`, zero diff, every test scene (FR-005/SC-004); (e) the promoted 067 properties hold
  on the wired path over ≥1,000 cases — round-trip/determinism/totality/identity-at-rest (FR-006/SC-005);
  (f) duplicate-key `KeyCollision` (and other diagnostics) surface through the existing channel and the
  path stays total (FR-007/SC-006); (g) governance test that the `.agents`↔`.claude`
  `fs-skia-reconciliation` byte-identity holds after the disposition flip (FR-010/SC-007). Each fails
  before the change.
- **Observability**: `ReconcileResult.Diagnostics` (e.g. `KeyCollision` from duplicate sibling keys) are
  surfaced through the existing `ControlDiagnostic` channel at the wiring boundary — **never silently
  dropped** (FR-007), and the path stays total in their presence (constitution VII: no swallowed
  exceptions, explicit degrade). The work-reduction metric emits an actionable record (re-measured/
  re-painted node counts) rather than a silent optimization. The correctness-wins fallback (FR-005) is an
  explicit, logged degrade to the full-rebuild-equivalent result, not a silent divergence.
- **Deferred scope**: Out of scope / deferred per spec — **E3** visual-state/style layer, **E4**
  focus/keyboard-traversal/input-routing model, **E5** lookless slot composition; **collection
  virtualization** (a later layer atop identity + partial updates); **broad per-control animation
  retargeting** beyond proving FR-003's clock survival (full animation↔identity coupling is sequenced
  after E2, roadmap §8); every **rejected non-goal** (XAML, data binding, dependency/attached properties,
  lookless `ControlTemplate` engine, CSS-selector styling); new package identity/version; release/platform/
  distribution changes. Versioning/packing follows the normal merge flow (libs incl. `FS.Skia.UI.Build`
  bumped at merge).

**Change classification**: **Escalated / `maintainer-verify` (Tier 1).** The change touches
`src/Controls/**` (promoting `Reconcile` onto the render path + the new internal retained-render structure +
honest `.fsi` doc), `src/Controls.Elmish/**` (host-loop wiring), and `src/SkiaViewer/**` (the
`dispatchHostMsg` repaint seam) — the controls-public-surface / package-surface routing rules apply, so
`Route` is expected to escalate and the serialized six-target order is expected to run, **even though the
public-surface delta is zero by default** (the disposition skill flip + behavioral `.fsi` doc still
escalate). The `.agents/skills/fs-skia-reconciliation` disposition update regenerates into `.claude` via
`RefreshSurfaceBaselines` (`SkillSyncCheck`-enforced). Any additive public seam from research D5 recaptures
per-package + api-surface baselines. **Run only the gates `Route` prints.**

## Project Structure

Files this feature touches (all under existing seams — no new project, no new runtime package):

```
# RETAINED-PATH-1 + PARTIAL-UPDATE-1 (wire Reconcile onto the render path)
src/Controls/Reconcile.fsi                  # +honest doc: now consumed by the render path (stays internal)
src/Controls/RetainedRender.fs / .fsi       # NEW module internal: retained structure pairing each
                                            #   Control<'msg> node with its cached LayoutNode/Scene fragment
                                            #   + stable identity; diff-driven reuse (Keep) vs recompute
                                            #   (Update/Replace/Insert); total in presence of Diagnostics.
                                            #   Reached by tests via InternalsVisibleTo("Controls.Tests").
src/Controls/Control.fs / .fsi              # render/renderTree: factor the per-node measure/paint so a
                                            #   retained fragment can be reused; honest .fsi render doc.
src/Controls.Elmish/ControlsElmish.fs       # runInteractiveApp: hold prev tree + retained render in the
                                            #   existing ref state; diff next vs prev, apply patch, re-key
                                            #   focusedText/textModels to the stable identity (FR-003).
src/Controls.Elmish/ControlsElmish.fsi      # honest host-loop doc (retained/partial render; behavior note)
src/SkiaViewer/SkiaViewer.fs                # dispatchHostMsg repaint (:2364/:2320/:2437): retained diff
                                            #   instead of unconditional currentScene <- host.View model
src/SkiaViewer/SkiaViewer.fsi               # honest repaint doc (behavioral note; signatures unchanged)

# Per-control animation clock (only as far as FR-003 survival needs)
src/Scene/Animation.fsi                      # reused as-is (Tween/AnimationState/applyAt); clock attaches
                                            #   to the retained identity in the host loop — no new public API

# FR-010 disposition flip (single source of truth)
.agents/skills/fs-skia-reconciliation/SKILL.md   # Disposition: parked/unwired -> wired on the render path
.claude/skills/fs-skia-reconciliation/SKILL.md   # REGENERATED mirror (RefreshSurfaceBaselines / SkillSyncCheck)

# Tests
tests/Controls.Tests/ReconcileTests.fs       # PROMOTE the 067 round-trip/determinism/totality/identity-
                                            #   at-rest properties to exercise the WIRED path (>=1000 cases)
tests/Controls.Tests/**                      # +identity-survival, golden parity, work-reduction,
                                            #   KeyCollision-surfacing, survives-proof tests (failing-first)
build/Governance/**Tests**                   # .agents<->.claude byte-identity for the disposition flip

# Regenerated currency / evidence artifacts
readiness/retained-parity/**                 # golden-diff parity (wired vs full rebuild)
readiness/partial-update/work-reduction.txt  # measured re-measure/re-paint node-count reduction
readiness/survives-proof/**                  # focus/animation survives an unrelated update (090 primitive)
# (recaptured per-package + template/base/docs/api-surface/** ONLY if research D5 adds a public seam)
```

See [research.md](./research.md) for the seam-by-seam findings and the five open design decisions
(D1 retained-structure shape & home, D2 identity re-keying of focus/text state, D3 partial-render reuse
mechanism & golden-parity guarantee, D4 the per-control animation clock for the survives-proof, D5
public-seam-or-internal & default-vs-flag), [data-model.md](./data-model.md) for the retained-structure /
patch-consumption / identity-bearing-state shapes, [contracts/](./contracts/) for the retained-render,
host-integration, invariant, and diagnostics contracts, and [quickstart.md](./quickstart.md) for the
two-frame identity → survives-proof → golden-parity verify loop.
