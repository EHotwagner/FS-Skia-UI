# Implementation Plan: General Navigation-Key Delivery (R5 / feature 100)

**Branch**: `100-general-navigation-keys` | **Date**: 2026-06-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/100-general-navigation-keys/spec.md`

## Summary

R5 generalizes the focused-control navigation path from **slider-only** to **all
interactive roles**. The host's `routeFocusedKey` `Navigate` arm
(`src/Controls.Elmish/ControlsElmish.fs:455-478`) today filters bindings by
`EventKind = "changed"` and emits a hardcoded `0.1` slider float
(`steppedValue`, `:366-381`), so a focused radio-group/tab/menu/list/grid does nothing on
arrow keys. The plan introduces a **closed, role-derived navigation-intent model**:
`Focus.route` is widened so its `Navigate` case carries a closed
`NavIntent = ValueStep of float | SelectionMove of Direction | GridMove of int * int`
(the single role-specific branch), and the host arm becomes a **uniform per-intent
resolver** that reads the live value/selection/grid model and dispatches the role's binding
with a closed `NavPayload` on `ControlEvent`. Range roles step by their **declared**
`NavRange` (step/min/max in `AccessibilityMetadata`), not a hardcoded constant; a
default-step slider stays byte-identical. The design is architecture-preserving: no new
open key-handler surface, no data binding, no CSS, no template engine.

This is a **Tier 1 (contracted)** change — public `.fsi` surface moves in `Focus`, `Types`,
and `Accessibility` — and it is stateful host routing, but the MVU boundary already exists
from E4 (feature 094); R5 extends the `Navigate` arm only.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: `FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish` (no new
package dependency); SkiaSharp/Vulkan only transitively via the live evidence host.
**Testing**: Expecto + FsCheck (`Check.One`, no `testProperty` in this repo), FAKE targets,
compiled self-closing live host for the responds-vs-renders artifact.
**Target Platform**: Windows and Linux. Live window evidence via the X11 path
(`live-vulkan-window-x11-path`).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Pre-Phase-0 evaluation**: PASS. Tier 1 contracted change with the full artifact chain
planned (spec ✓, plan, `.fsi` updates, surface baselines, tests, evidence). The
stateful/host-routing work reuses the established E4 MVU seam; `update` stays pure and the
key event is interpreted only at the host edge (Principle IV). No new dependencies
(Principle III/Engineering Constraints). Closed intent/payload model avoids the rejected
open-handler design.

**Post-Phase-1 re-evaluation**: PASS. The design (research.md, data-model.md, contracts/)
keeps visibility in `.fsi` (Principle II), uses plain DUs/records over cleverness
(Principle III — no SRTP/reflection/custom operators), exercises the public surface through
FSI-shaped tests (Principle I), and routes all I/O-adjacent behavior through the pure
router + host-edge resolver (Principle IV). No synthetic evidence is planned (Principle V);
all four evidence obligations have a real path.

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json` change. R5 touches
  framework library source (`src/Controls/**`, `src/Controls.Elmish/**`) and tests only;
  generated projects consume general navigation transparently with no scaffold change and
  no new selectable capability. Template package **pins** are bumped only at squash-merge by
  `speckit-merge` (the separate template version track), not by this feature's diff.
- **Dependency impact**: N/A — no new package. `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` coverage are unchanged; R5 adds only F#
  types and behavior within existing packages.
- **Command-surface impact**: No new FAKE target. `Dev` runs the new
  unit/property/integration tests. Because the diff edits public `.fsi` in
  `Focus`/`Types`/`Accessibility`, `Route` **escalates** to the controls-public-surface
  (agent-ready / maintainer-verify) route, so the serialized order applies:
  `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`,
  `EvidenceAudit`. `Route` is authoritative — run `./fake.sh build -t Route` against the
  real diff and run only the gates it prints (`--enforce` for missing evidence). FAKE-backed
  targets run **sequentially** in the deterministic order (shared `.fake` state):
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A for default/minimal contents and the scaffold — no
  generated-content change. Generated apps gain general navigation behavior for free through
  the updated `FS.Skia.UI.Controls.Elmish` host once packages are bumped at merge; no
  placeholder/excluded-history/`Dev`-behavior change in the generated project.
- **Evidence paths**: under `specs/100-general-navigation-keys/`:
  - `evidence/responds-vs-renders.*` — arrow → selection-move on a focused radio-group/tab
    on the live host (pre-R5 dispatches nothing).
  - `evidence/declared-step.*` — non-default-step slider steps by declared step; default-step
    slider byte-identical (non-regressive numeric golden output).
  - `evidence/role-coverage.*` — value + linear-selection + grid roles, each with
    `Accessibility.validate` output.
  - `evidence/closed-model.*` — `NavIntent`/`NavPayload` exhaustiveness/property proof.
  - Standard `readiness/` audit artifacts, plus `EvidenceGraph`/`EvidenceAudit` output and
    recaptured published api-surface + per-package `.fsi.txt` baselines for the edited
    Controls modules.
- **`.fsi` / contract impact**: **Tier 1.** Signatures change in `src/Controls/Focus.fsi`
  (`Direction`, `NavIntent`, `Navigate of NavIntent`, widened `route`),
  `src/Controls/Types.fsi` (`NavRange`, `NavPayload`, `Navigation` field on
  `AccessibilityMetadata`, `Nav` field on `ControlEvent`), and
  `src/Controls/Accessibility.fsi` (`metadata` accepting `NavRange option`). Possibly
  `src/Controls.Elmish/ControlsElmish.fsi` **only if** the per-intent resolver is promoted
  beyond module-internal (default: keep it module-internal, no Elmish `.fsi` change). Every
  `.fsi` edit requires recaptured published api-surface + per-package baselines
  (`PerPackageSurface.captureCurrent`; `RefreshSurfaceBaselines` does **not** cover the
  per-package `.fsi.txt` snapshots — regenerate those explicitly). Compatibility: existing
  `ControlEvent`/`AccessibilityMetadata` consumers must add the new field at construction;
  all framework-internal construction sites are updated in the same change. No consumer API
  rename.
- **MVU/effect boundary**: This is stateful host input routing reusing the **landed E4
  seam**. `Model` — the consumer's `'model` (unchanged) plus the host's `RetainedRender`
  retained state (focus identity, value/selection model already tracked). `Msg` — the
  consumer's `'msg` (now a selection move carries a closed `NavPayload`/moved item) and the
  internal `ControlRuntimeMsg` (`FocusControl`). `Effect`/`Cmd` — none new; navigation
  produces `'msg list`, it does not perform I/O. `init` — unchanged. `update` —
  `routeFocusedKey`/`Focus.route` are **pure** (given role + metadata + key → intent →
  messages); the key event is interpreted at the host edge (`runInteractiveApp`). Evidence:
  pure transition tests on `Focus.route` (role+key → `NavIntent`) and on the resolver
  (intent + live model → dispatched messages), plus the live responds-vs-renders artifact
  through the real retained seam — no hand-seeded identity map (mirrors `Feature094*`).
- **Synthetic evidence**: **None planned.** All four evidence obligations have a real path:
  pure router/resolver tests over the public surface, a real non-regressive numeric golden,
  `Accessibility.validate` over real controls, and a live-window responds-vs-renders capture
  via the compiled self-closing host. No `[S]`/`[SEH]` task is anticipated; if a live-window
  capture proves infeasible in the run environment, the render-only deterministic host path
  is the documented fallback and any residual gap is disclosed per Principle V, not silently
  greened.
- **Test evidence**: Failing-first semantic tests — `tests/Controls.Tests/Feature100*` (pure
  `Focus.route` → `NavIntent` per role; `NavIntent`/`NavPayload` closed-set exhaustiveness
  via `Check.One`) and `tests/Elmish.Tests/Feature100*` (host resolver through the real
  `RetainedRender` seam: radio-group selection move + dispatched binding; non-default-step
  slider declared-step; grid 2-D move; boundary clamp no-op for each; non-navigable button
  no-op). Governance tests: recaptured surface baselines validated by the per-module
  surface-area test. Each test fails on a pre-R5 build (radio-group dispatches nothing;
  slider steps by 0.1 regardless of declared step).
- **Observability**: Navigation routing is pure data, not an I/O critical path, so no new
  structured-log sink is required. The honest failure modes are surfaced as **no-ops with no
  spurious dispatch** (empty group, unset index, boundary clamp) — asserted explicitly in
  tests so "nothing happened" is a verified outcome, not a swallowed error.
  `Accessibility.validate` continues to flag a focusable control with no operable key set.
  The evidence artifacts under `evidence/` are the actionable diagnostics for the live path;
  a missing required artifact fails `Route --enforce` (named artifact + requiring tier).
- **Deferred scope**: Current obligations = the three representative roles
  (value/selection/grid) with clamp boundary and the closed model. Explicitly deferred
  (bounded follow-ups, per spec Out of Scope): full-52-control navigation coverage; wrap
  boundary policy beyond opt-in; multi-select range extension (Shift-arrow); type-ahead /
  incremental search; drag-reorder; any consumer custom key-binding/remapping API. No
  focus-traversal (Tab/Shift-Tab) or activation (Space/Enter) change — those are E4.
  **This is the final roadmap remediation (R1–R5); no successor.**

## Project Structure

```
specs/100-general-navigation-keys/
├── spec.md                       # feature specification (input)
├── plan.md                       # this file
├── research.md                   # Phase 0 — 8 resolved decisions
├── data-model.md                 # Phase 1 — closed type surface
├── contracts/
│   ├── Focus.nav.fsi             # Direction / NavIntent / widened route sketch
│   ├── Types.nav.fsi             # NavRange / NavPayload / ControlEvent / metadata sketch
│   └── resolver.behavior.md      # host per-intent resolver behavior contract
├── quickstart.md                 # Phase 1 — consumer + maintainer walkthrough
├── checklists/
│   └── requirements.md           # spec quality checklist (16/16)
└── evidence/                     # (created at implementation) the 4 evidence artifacts

src/Controls/
├── Focus.fsi / Focus.fs          # Direction, NavIntent, Navigate of NavIntent, route widening
├── Types.fsi / Types.fs          # NavRange, NavPayload, ControlEvent.Nav, AccessibilityMetadata.Navigation
└── Accessibility.fsi / Accessibility.fs   # metadata accepts NavRange option; keyboardFor unchanged keys

src/Controls.Elmish/
└── ControlsElmish.fs (.fsi only if resolver promoted)   # Navigate arm → per-intent resolver

tests/Controls.Tests/Feature100*.fs          # pure route + closed-set proofs
tests/Elmish.Tests/Feature100*.fs            # host resolver routing proofs
```

## Phase 0 — Outline & Research

Complete. See [research.md](./research.md): 8 decisions (intent produced in `Focus.route`;
selection binding selected-then-changed fallback; new closed `Nav` field on `ControlEvent`;
`NavRange` in `AccessibilityMetadata`; role-oriented direction mapping; clamp default;
empty/unset no-op; three-tier test + live-artifact strategy). No `NEEDS CLARIFICATION`
remains.

## Phase 1 — Design & Contracts

Complete. [data-model.md](./data-model.md) (closed type surface + placement),
[contracts/](./contracts/) (`.fsi` sketches + resolver behavior), [quickstart.md](./quickstart.md).
`AGENTS.md` SPECKIT plan reference updated to this plan.

## Phase 2 — Next command

`/speckit-tasks` will break this into story-grouped tasks (US1 radio-group selection move
P1, US2 declared-step slider P1, US3 grid move P2, US4 closed-model invariant P1) with
`tasks.deps.yml` + `skillist` metadata. The applicable capability skill is
`fs-skia-ui-widgets` (Controls) plus `fs-skia-elmish` (host routing) and
`fs-skia-keyboard-input` (key delivery); `fs-skia-evidence-mode` for the live artifact.
