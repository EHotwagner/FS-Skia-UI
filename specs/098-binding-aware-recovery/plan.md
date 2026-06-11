# Implementation Plan: Binding-Aware Ancestor Recovery (R3)

**Branch**: `098-binding-aware-recovery` | **Date**: 2026-06-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/098-binding-aware-recovery/spec.md`

## Summary

E1 (feature 090) wired authored `EventBindings` into the interactive host, but the
§10.5 audit found dispatch works **only for keyed controls**: an unkeyed
`Button.onClick` — the documented, obvious authoring — renders but does nothing.
Two id schemes diverge underneath: bindings/bounds key by `Key ?? Kind`
(`eventBindings` `Control.fs:194`, `collectBoundsWith` `controlId` `:1332`) while
recovery/hit lives in the layout-path domain `Key ?? path`
(`nearestAuthored` `:1459`, the Click id from `Layout.evaluate`). They agree
**only when a `Key` is present**, and the `Kind` half collides for same-kind siblings.

R3 closes this with two architecture-preserving moves:

1. **Unify the per-node `ControlId` to one scheme — `Key ?? structural-path`** —
   across the public `Bounds` list, `EventBindings`, and `nearestAuthored` recovery,
   replacing the `Key ?? Kind` fallback. Keyed nodes are unchanged; only the unkeyed
   fallback shifts from `Kind` → the positional path (`"0.1"`).
2. **Make `nearestAuthored` binding-aware** — a new `BoundIds : Set<ControlId>` field
   on `ControlRenderResult<'msg>` (emitted by `renderTree`/`render` and the retained
   path) lets recovery treat an ancestor as authored when it is **keyed OR in
   `BoundIds`**, so an unkeyed bound node recovers its own path id and dispatches.

No routed/bubbling events, no command system, no new public event type. The fix
corrects an id-derivation divergence and widens an existing recovery predicate so
data already produced by `renderTree` routes correctly.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: existing only — `FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.Layout`. No new package, dependency, or DTCG token.
**Testing**: Expecto + FsCheck (Controls.Tests, Controls.Elmish.Tests); FAKE escalated six-target order; live-adapter routing seam (`routeInteractivePointer`) as real dispatch evidence.
**Target Platform**: Windows and Linux (framework-internal; no Skia/Vulkan surface change).
**Change Tier**: **Tier 1 (contracted change)** — adds a public field (`ControlRenderResult.BoundIds`) and changes the canonical `ControlId` value for unkeyed controls in `Bounds`/`ControlEvent.ControlId`. Escalates to the controls-public-surface route.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json` or command-surface
  change. R3 is a framework-internal id/recovery correction in two existing `src/**`
  packages. Template pins are refreshed only on merge by the standard version-bump flow
  (all packable libs bumped together), not by this plan.
- **Dependency impact**: N/A — no dependency change. No edit to
  `Directory.Packages.props`, `docs/dependencies.md`, generated template inclusion, or
  `DependencyReport` coverage; the change uses only already-referenced packages.
- **Command-surface impact**: No new gate and no `build.fsx` change. `Route` escalates
  this change (public `src/Controls/**/*.fsi`) to the serialized maintainer-verify path;
  run only the gates `Route` prints, in deterministic order, never concurrently (shared
  `.fake` state): 1) `./fake.sh build -t Dev` 2) `GeneratedGuidanceCheck`
  3) `TemplateCheck` 4) `GeneratedProductCheck` 5) `EvidenceGraph` 6) `EvidenceAudit`.
  Surface baselines (api-surface + per-package `.fsi.txt`) for `FS.Skia.UI.Controls` are
  recaptured.
- **Generated project impact**: N/A — no change to default/minimal generated contents,
  selected Controls guidance text, local skills, validation logs, placeholder/excluded-history
  scans, or generated `Dev` behavior. The capability is consumed by hand-authored consumer
  views, not by scaffold defaults.
- **Evidence paths**: All readiness artifacts under
  `specs/098-binding-aware-recovery/readiness/`:
  - `us1-unkeyed-dispatch.md` — unkeyed `Button.onClick` dispatches through
    `routeInteractivePointer`; nested unkeyed bound control dispatches via recovery
    (artifact an un-fixed build cannot produce).
  - `us2-keyed-nonregression.md` — keyed-leaf + container-keyed recovery identical to 090.
  - `us3-sibling-disambiguation.md` — FsCheck determinism + same-kind-sibling distinctness
    (≥1000 cases) over the unified id scheme.
  - `fallback-and-mappointer.md` — recovery-`None` → `MapPointer`; `MapPointer`-only
    consumers bit-for-bit unchanged.
  - `focus-nonregression.md` — 092 `resolveFocus`/`retainedHitTest`/`RetainedId` unchanged.
  - `surface-baseline.md` — recaptured api-surface + per-package `.fsi.txt` diffs showing
    `BoundIds` and the canonical-id change.
  - `validation-log.md` — the six-target run transcript + `EvidenceAudit` verdict.
- **`.fsi` / contract impact**: **Yes — Tier 1.** (1) `ControlRenderResult<'msg>` gains
  `BoundIds : Set<ControlId>` in `src/Controls/Types.fsi` (and the mirror in
  `src/Controls/Types.fs`). (2) `nearestAuthored`'s signature is **unchanged**
  (`result -> hit -> ControlId option`); only its behavior widens. (3) The canonical
  `ControlId` value for unkeyed controls changes (`Kind → path`) in the public `Bounds`
  list and `ControlEvent.ControlId` payload — documented compatibility note: keyed
  authoring is unchanged; the old `Kind` fallback was collision-prone, so this is a net
  correctness gain that only *adds* dispatch for the previously-dead unkeyed case.
- **MVU/effect boundary**: No new MVU surface. `nearestAuthored` and the binding
  collectors stay **pure/total/deterministic** over the already-computed render result
  (no clock, no randomness, resume-safe). `bindingMessagesFor` is unchanged except that
  the recovered id now matches under the unified scheme. No new `Model`/`Msg`/`Effect`/
  `Cmd`/subscription/interpreter; `ControlRuntime`, `Pointer`, and the focus seam are
  untouched. The binding-wins-over-`MapPointer` precedence is preserved (no double-dispatch).
- **Synthetic evidence**: None planned. US1's headline proof is a **real** dispatch
  through the live-adapter routing seam (`routeInteractivePointer`) — the same seam
  `runInteractiveApp` wires — not a hand-seeded binding. US3 uses FsCheck-**generated**
  trees (generated, not canned). No `[S]`/`[SEH]` task is anticipated; if any arises it
  will carry full Principle V disclosure at task/code/test/spec/PR surfaces.
- **Test evidence**: Failing-first semantic tests added to existing projects:
  - `Controls.Elmish.Tests` — unkeyed `Button.onClick` dispatches via
    `routeInteractivePointer`/`bindingMessagesFor` (RED on today's `nearestAuthored`),
    nested-unkeyed recovery, recovery-`None` fallback, keyed/container-keyed non-regression
    (re-running the 090 cases), `MapPointer`-only invariance.
  - `Controls.Tests` — single-canonical-scheme agreement across `Bounds`/`EventBindings`/
    `BoundIds`; FsCheck determinism + same-kind-sibling distinctness (≥1000 cases);
    `Control.dispatch` keyed regression suite (`InteractionTests.fs`) stays green;
    `render.BoundIds` populated while `render.Bounds` stays `[]`.
  - Surface-baseline test recaptured for `BoundIds` + canonical-id change.
- **Observability**: No new diagnostics needed — the change is a pure data correction.
  The existing responds-vs-renders proof primitive (E1) is the actionable signal: an
  inert/un-fixed build fails the `us1-unkeyed-dispatch` artifact. No new log path, report
  field, missing-artifact-class failure, or unsupported-environment message is introduced.
- **Deferred scope**: Out of scope and explicitly deferred — R4 (animation clock /
  transitions), R5 (general navigation-key delivery), any routed/bubbling/tunneling event
  system, command system, new public event type, the 092 retained focus path (FR-008), and
  any catalog-wide retrofit of all 52 typed views' binding surfaces (a separate fitness
  pass). CSS selectors, attached/dependency properties, lookless templates, and data
  binding remain permanent roadmap non-goals.

**Initial Constitution Check: PASS** — Tier 1 declared with `.fsi` updates and baseline
recapture planned (Principle I/II); pure/total/deterministic recovery keeps the change
idiomatic (III); no new stateful/I/O boundary (IV); no synthetic evidence anticipated (V);
failing-first real-dispatch + property tests defined (VI); responds-vs-renders proof is the
safe-failure signal (VII).

## Project Structure

```
specs/098-binding-aware-recovery/
├── spec.md
├── plan.md                # this file
├── research.md            # Phase 0 — design decisions (id unification, BoundIds, dispatch consistency)
├── data-model.md          # Phase 1 — entities: canonical ControlId, BoundIds, binding-aware recovery
├── contracts/
│   ├── control-render-result.md   # ControlRenderResult.BoundIds + canonical-id contract
│   └── nearest-authored.md         # widened recovery predicate + dispatch lookup contract
├── quickstart.md          # Phase 1 — author an unkeyed Button.onClick and see it dispatch
└── readiness/             # populated during /speckit-implement (evidence paths above)
```

### Source files touched (existing only — no new files)

- `src/Controls/Types.fsi` — add `BoundIds : Set<ControlId>` to `ControlRenderResult<'msg>` (≈ line 345).
- `src/Controls/Types.fs` — mirror the `BoundIds` field (≈ line 287).
- `src/Controls/Control.fs` —
  - make binding collection **path-aware** (`eventBindings`/`eventBindingsOf` thread the
    same `parent + "." + index` path `collectBoundsWith` already mints), deriving
    `id = Key ?? path` (replaces `Key ?? Kind`, `:194`);
  - change `collectBoundsWith`'s emitted `controlId` from `Key ?? Kind` (`:1332`) to
    `Key ?? path` (the `layoutId` it already computes);
  - add `boundIdsOf : Control<'msg> -> Set<ControlId>` to `ControlInternals`;
  - populate `BoundIds` in `Control.render` (`:1385`) and `Control.renderTree` (`:1409`);
  - widen `nearestAuthored` (`:1459`) to read `result.BoundIds` (authored = keyed OR
    canonical path id ∈ `BoundIds`);
  - thread the path in `Control.dispatch` (`:1480`) so its by-`event.ControlId` matching
    uses the same unified scheme (keyed cases unchanged → `InteractionTests.fs` stays green).
- `src/Controls/Control.fsi` — add `val boundIdsOf` to the `ControlInternals` signature;
  `nearestAuthored`/`render`/`renderTree`/`collectBoundsWith`/`eventBindingsOf` signatures
  are unchanged.
- `src/Controls/RetainedRender.fs` — populate `BoundIds` at both `ControlRenderResult`
  construction sites (`:118` first frame, `:374` subsequent frame) via `boundIdsOf`.
- `src/Controls.Elmish/ControlsElmish.fs` — `bindingMessagesFor` (`:155`) logic is
  unchanged; it now resolves the unkeyed-bound case for free because the recovered id and
  `EventBindings` keys share the unified scheme. (Verify only.)

### Tests touched (existing projects)

- `tests/Controls.Elmish.Tests/**` — routing-seam dispatch + non-regression + fallback.
- `tests/Controls.Tests/**` — single-scheme agreement, FsCheck distinctness/determinism,
  `Control.dispatch` keyed regression, `render.BoundIds`, surface baseline.

## Phase 0 — Research

See [research.md](./research.md). All NEEDS CLARIFICATION are resolved (the spec's
Clarifications session settled the four open questions: canonical scheme adoption,
`BoundIds` as a field, focus path out of scope, and `render` participation). Phase 0
additionally settles the **internal mechanism** decisions the spec leaves to the plan:
how to thread the path into binding collection, where `boundIdsOf` lives, and the
`Control.dispatch` consistency decision.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — the canonical `ControlId`, the `BoundIds` set, and the
  binding-aware recovery predicate as data, plus the determinism/distinctness invariants.
- [contracts/control-render-result.md](./contracts/control-render-result.md) — the
  `BoundIds` field contract and the canonical-id guarantee across `Bounds`/`EventBindings`.
- [contracts/nearest-authored.md](./contracts/nearest-authored.md) — the widened recovery
  predicate, the dispatch lookup, and the non-regression fixed points.
- [quickstart.md](./quickstart.md) — author an unkeyed `Button.onClick` and observe dispatch.
- Agent context: `AGENTS.md` SPECKIT marker repointed to this plan.

## Phase 2 — (planning ends here)

`/speckit-tasks` will break this into story-grouped tasks with `skillist` metadata
(`fs-skia-ui-widgets` for the Controls id/recovery work, `fs-skia-elmish` for the dispatch
seam, `fs-skia-reconciliation` for the retained-path `BoundIds` emission, `fs-skia-testing`
for the failing-first / property suites, `fs-skia-evidence-mode` for the responds-vs-renders
artifacts) and emit `tasks.deps.yml`.
