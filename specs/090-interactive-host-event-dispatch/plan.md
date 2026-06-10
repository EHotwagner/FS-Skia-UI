# Implementation Plan: Live Interactive Control Responsiveness — Authored Event-Binding Dispatch, Keyed-Ancestor Recovery, Text-Input Seam & a Responds-vs-Renders Proof

**Branch**: `090-interactive-host-event-dispatch` | **Date**: 2026-06-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/090-interactive-host-event-dispatch/spec.md`

## Summary

`ControlsShowcase2`'s **implement** feedback (severity **major**) is the headline: a controls
gallery shipped with build + 39/39 tests + both merge gates green, yet **the live window did not
respond to input**. Triaged against current source, the root cause is a single framework defect with
three contributing/adjacent gaps, all in the interactive-host spine — this is **step E1** of the
maintainer-confirmed declarative-retained trajectory ([[architecture-evolution-no-redesign]]), **not**
an architecture redesign.

1. **LIVE-DISPATCH-1 (P1, root cause).** `routeInteractivePointer` computes `rendered =
   Control.renderTree …` but uses **only** `rendered.Layout`; `rendered.EventBindings` is never
   referenced. Routing is purely `interpretPointerOutcome host.MapPointer interactions`
   (`ControlsElmish.fs:182-184`). So a consumer's authored `Button.onClick`/`CheckBox.onChanged`
   bindings are **dead** in the live window, and the published `.fsi` doc
   (`ControlsElmish.fsi:135`) *falsely* claims the host hit-tests "`Layout.hitTestComputed` ×
   `EventBindings` by `ControlId`." **Approach:** in `routeInteractivePointer`, after `Pointer.update`
   emits interactions, resolve each interaction's hit `ControlId` to the **authored** control id
   (FR-004 recovery), join with `rendered.EventBindings` by `(ControlId, eventKind)`, and dispatch the
   bound message. **Authored binding wins; `MapPointer` is the fallback** consulted only for
   interactions no binding consumed (FR-003, clarified — no double-dispatch). Correct the `.fsi` doc to
   match (FR-002).

2. **KEYED-ANCESTOR-1 (P1, contributing).** Pointer interactions carry the **structural layout id**
   (`c.Key |> Option.defaultValue path`, `Control.fs:1052`/`1129`), so a hit inside a **container-keyed**
   composite returns the deepest *inner positional* id (`"0.1"`), which matches no binding (bindings are
   keyed `Key |> defaultValue Kind`). **Approach:** add a **public, option-returning nearest-keyed-ancestor
   recovery** in `src/Controls/**` that walks the control tree from the deepest hit path to the nearest
   ancestor carrying a `withKey`/authored binding, returning that authored `ControlId` (FR-004); `None`
   when no keyed/bound ancestor exists (FR-004a → host falls back to `MapPointer` raw). A directly-keyed
   leaf resolves to itself (FR-005, non-regressive).

3. **RESPONDS-EVIDENCE-1 (P1/US3).** Add a **framework-side, capturable responds-proof**: capture the
   rendered output **before and after** the same dispatched live interaction and assert they differ, as a
   decodable artifact distinct from (a) a render-only screenshot and (b) the offscreen
   `runInteractivePointerOnce` route probe. Encode the **obligation** durably in the `.agents` evidence
   skill tree (regenerated into `.claude` via `RefreshSurfaceBaselines`, `SkillSyncCheck`-enforced) so it
   binds future interactive-UI stories; an inert app cannot produce it (FR-006/FR-007).

4. **TEXT-INPUT-1 (P3).** Add a **focus-aware text-routing seam** so `TextBox`/`TextArea`/`NumericInput`
   are typeable in `runInteractiveApp`. The host delivers a keystroke (and committed/composed text) to the
   **currently focused** text control via the existing `ControlRuntime.FocusedControl` + `TextInput`
   pipeline; pointer-click sets focus; the seam is documented (FR-008). Scope is the seam only — full
   editor UX and general focus/tab-traversal are trajectory item **E4** (FR-008a).

090 delivers the **host dispatch mechanism + representative end-to-end verification** (one leaf-keyed,
one container-keyed, one text control); it does **not** audit/retrofit all 52 typed `Widgets/*.fs` views
(FR-005a).

## Technical Context

**Language/Version**: F# / .NET (`net10.0`). Runtime packages `FS.Skia.UI.Controls`,
`FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.SkiaViewer`; governance assembly `FS.Skia.UI.Build`
(`build/Governance/**`).
**Primary Dependencies**: None new. Existing seams only — `Control.renderTree`/`EventBindings`/`hitTest`,
`FS.Skia.UI.Layout.Layout.hitTestComputed`, `Pointer.update`/`PointerInteraction`,
`ControlRuntime.FocusedControl`, `TextInput.update`, `SkiaViewer` repaint loop, the `.agents`→`.claude`
skill generator.
**Testing**: Expecto semantic/unit tests (Controls + Controls.Elmish test projects), headless
`routeInteractivePointer`/`runInteractivePointerOnce` adapter-path tests (research D6 pattern), a captured
responds-proof artifact, governance tests for the evidence obligation, the serialized six-target order.
**Target Platform**: Windows and Linux. The responds-proof is capturable headless/offscreen (render-target
PNG diff) — no live Vulkan window required for evidence (`[[fs-skia-evidence-mode]]`, render-only honesty).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: **Update required (regenerated artifacts only).** New/changed public `.fsi`
  in `src/Controls/**` (recovery helper) and `src/Controls.Elmish/**` (binding-dispatch behavior +
  text-routing seam + corrected host-contract doc) re-emit into the published api-surface tree
  (`docs/api-surface/**` → `template/base/docs/api-surface/**`). These are regenerated via
  `RefreshSurfaceBaselines`, not hand-edited. No `.template.config/template.json` manifest change (no new
  top-level file class — the api-surface dir already ships). No template source/sample change.
- **Dependency impact**: **N/A — no dependency change.** No new package, no `Directory.Packages.props`
  edit, no `docs/dependencies.md` / `DependencyReport` change. All work reuses already-referenced seams.
- **Command-surface impact**: **Update required (output/behavior of existing targets; no new wrapper, no
  new gate).** `RefreshSurfaceBaselines` regenerates the api-surface tree, the per-package surface
  snapshots, and any touched `.claude` skill mirror. The serialized six-target order changes output and
  must be re-run. No new FAKE target; no `validation.contract.yml` routing-row change (no gate added).
  FAKE-backed commands share `.fake` state — run sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: **Update required (additive).** Generated projects inherit the enriched
  `docs/api-surface/**` (recovery helper + host seam signatures) and, where RESPONDS-EVIDENCE-1 touches
  the skill tree, the regenerated evidence-mode guidance. No generated `Dev` behavior, placeholder-scan,
  or excluded-history-scan change. The host change is **additive**: a control with no authored binding
  behaves exactly as before (FR-003), so existing `MapPointer`-only generated products are unbroken.
- **Evidence paths**: Real evidence — (a) the captured **input→visible-change responds-proof**
  artifact pair + diff verdict on the running host (FR-006), e.g.
  `readiness/responds-proof/{before,after}.png` + `responds-proof.txt`, distinct from the existing
  render-only screenshots and the offscreen route probe; (b) recaptured per-package surface
  `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` and
  `…FS.Skia.UI.Controls.Elmish.fsi.txt`; (c) recaptured `template/base/docs/api-surface/**`;
  (d) the corrected `ControlsElmish.fsi` host-contract doc; (e) `readiness/skill-sync-check.md` byte
  identity for any edited `.agents`↔`.claude` evidence skill; (f) the serialized six-target logs;
  (g) Expecto test logs for dispatch/recovery/text-seam.
- **`.fsi` / contract impact**: **Public `.fsi` signatures change (Tier 1).** New public surface:
  the nearest-keyed-ancestor recovery in `src/Controls/**` (FR-004); the focus-aware text-routing seam in
  `src/Controls.Elmish/**` (FR-008); a **behavioral change** to `routeInteractivePointer`/`runInteractiveApp`
  dispatch (FR-001, additive) with a **corrected** host-contract doc (FR-002 — today's claim is false).
  Per-package and published api-surface baselines recaptured. Compatibility note: additive — no signature
  is removed; `MapPointer` remains (FR-003). The text seam is designed to avoid a breaking change to the
  existing `MapKey: ViewerKey -> bool -> 'msg option` field (see research D4).
- **MVU/effect boundary**: **In scope — interactive host dispatch.** Model = consumer `'model`;
  `Msg` = consumer `'msg` (now also produced by authored bindings, not only `MapPointer`); the host folds
  via `host.Update` (unchanged). Pointer routing stays pure: `routeInteractivePointer` is a pure
  `state × size × model × input → state × 'msg list` step (interpreter at the `runInteractiveApp` edge,
  exactly as today). The text seam routes through the existing `TextInput.update` (pure) and
  `ControlRuntime.FocusedControl` state — no new parallel text model (FR-008). No new effects,
  subscriptions, or interpreter behavior introduced; `host.Update` folding is otherwise unchanged.
- **Synthetic evidence**: **None planned.** Dispatch/recovery/text-seam tests exercise the **real**
  adapter path (`routeInteractivePointer`/`runInteractivePointerOnce`) with real control trees and real
  `Pointer.update` hit-testing; the responds-proof is a **real** before/after render diff. No mocks,
  fakes, or placeholders anticipated. If a genuine error-path fixture arises it gets full Principle-V
  `[S]`/`[SEH]` disclosure ([[accepted-seh-stops-propagation]]).
- **Test evidence**: Failing-first semantic tests — (a) an `onClick`/`onChanged` control routed via
  `routeInteractivePointer` dispatches the **bound** message with zero `MapPointer` clauses, and does
  **not** also fire `MapPointer` (FR-001/FR-003 precedence); (b) a container-keyed composite resolves to
  its authored container id via the recovery and routes its binding; a directly-keyed leaf resolves to
  itself; an unkeyed/unbound hit returns `None` and falls back to `MapPointer` (FR-004/FR-004a/FR-005);
  (c) a keystroke delivered through the text seam reaches the **focused** text control's `TextInput`
  model and not an unfocused one (FR-008); (d) the responds-proof differs for a responsive app and
  **fails to differ** for an inert one (FR-006); (e) governance test that the durable obligation +
  `.claude`/`.agents` byte-identity hold (FR-007). Each fails before the change.
- **Observability**: The corrected host-contract doc (FR-002) is itself an observability fix — the
  published contract stops lying about dispatch. The responds-proof emits an actionable verdict
  (responsive vs inert, with the diff artifact path). The recovery returns an explicit `None` rather
  than inventing a `Kind`/root id (FR-004a), so an unroutable hit is visible, not silently mis-routed.
  No silent-failure path introduced; no swallowed exceptions in the routing step.
- **Deferred scope**: Out of scope / deferred per spec — the OUT-of-scope triage items (already shipped:
  run-and-use discipline (089), WCAG contrast helper, skillist validator/echo, repaint-on-update, key
  warm-up, keyed-*leaf* hit-test, snapshot helper); the deferred governance/docs items (catalog
  demonstrable-count, readiness value-grammar docs, must-survive token manifest, durable symbol manifest,
  Spec-Kit niceties); a catalog-wide audit/retrofit of the 52 typed views (FR-005a); a complete
  text-editing UX and general focus/tab-traversal (trajectory item **E4**, FR-008a); E2–E5 trajectory
  work. Versioning/packing follows the normal merge flow (libs incl. `FS.Skia.UI.Build` bumped at merge).

**Change classification**: **Escalated / `maintainer-verify` (Tier 1, public-contract).** New/changed
public `.fsi` in `src/Controls/**` and `src/Controls.Elmish/**` ship to consumers and emit into the
template's api-surface; the responds-proof adds a runtime evidence obligation that may touch the `.agents`
skill tree. `Route` is expected to escalate — run the serialized six-target order, recapture per-package
and api-surface baselines, and regenerate any touched `.claude` skill via `RefreshSurfaceBaselines`.

## Project Structure

Files this feature touches (all under existing seams — no new project, no new runtime package):

```
# LIVE-DISPATCH-1 + KEYED-ANCESTOR-1 (host dispatch + recovery)
src/Controls/Control.fs / .fsi              # +public nearest-keyed-ancestor recovery (FR-004/004a/005)
src/Controls.Elmish/ControlsElmish.fs       # routeInteractivePointer: join EventBindings via recovery,
                                            #   authored-binding-wins precedence over MapPointer (FR-001/003)
src/Controls.Elmish/ControlsElmish.fsi      # corrected host-contract doc (FR-002); new text-seam signature

# TEXT-INPUT-1 (focus-aware text routing seam)
src/Controls.Elmish/ControlsElmish.fs(i)    # focus-aware keystroke delivery to focused TextInput (FR-008)
                                            #   (reuses ControlRuntime.FocusedControl + TextInput.update)

# RESPONDS-EVIDENCE-1 (capturable runtime proof + durable obligation)
src/SkiaViewer/SkiaViewer.fs(i) OR src/Controls.Elmish/**  # capture before/after render diff around a
                                            #   dispatched live interaction (FR-006) — research D5 decides home
.agents/skills/fs-skia-evidence-mode/SKILL.md   # +responds-vs-renders obligation (FR-007)
.claude/skills/fs-skia-evidence-mode/SKILL.md   # REGENERATED mirror (RefreshSurfaceBaselines / SkillSyncCheck)

# Tests
src/Controls/**Tests** , src/Controls.Elmish/**Tests**   # failing-first dispatch/recovery/text-seam/proof
build/Governance/**Tests**                  # responds-obligation durability + skill byte-identity

# Regenerated currency artifacts
readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt
readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt
template/base/docs/api-surface/**           # recaptured api-surface for the two packages
readiness/responds-proof/**                 # the new input→visible-change artifact
```

See [research.md](./research.md) for the seam-by-seam findings and the four open design decisions
(D1 recovery shape, D2 precedence join, D3 responds-proof mechanism, D4 text-seam signature),
[data-model.md](./data-model.md) for the recovery/binding-join/responds-proof shapes,
[contracts/](./contracts/) for the host-dispatch, recovery, text-seam, and responds-proof contracts, and
[quickstart.md](./quickstart.md) for the author→host→click→verify loop.
</content>
