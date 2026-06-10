# Implementation Plan: Lookless Slot Composition

**Branch**: `095-lookless-slot-composition` | **Date**: 2026-06-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/095-lookless-slot-composition/spec.md`

## Summary

E5 — the roadmap's final, deliberately-narrow rung. Add a **lookless slot
mechanism** that lets a control kind declare a **closed, per-kind, typed set of
named regions** a consumer may fill with their own `Control<'msg>` sub-tree, so a
consumer can re-skin a control's *shape* (icon before a button's label, a custom
container header), not just its tokens (theme) or its visual state / style class
(E3). A slot fill is a **static `Control<'msg>` value** the consumer's own
`view : 'model -> Control<'msg>` already computed from `'model` — **not** a
data-bound `ControlTemplate`, `DataContext`, binding expression, or per-item
template instantiation. An **unfilled** slot renders the kind's current chrome, so
a slot-bearing control with no slots supplied is **byte-identical /
structurally-`Scene`-equal** to its prior (pre-slot) render (FR-003). Slotted
content is a full `Control<'msg>` in the retained tree, so it composes with E1–E4
free and keeps its E2 retained identity across a sibling-shifting re-render
(FR-005). Verification is bounded to a **representative set**: a leaf-with-regions
kind (**`Button`** — `Leading` / `Trailing` slots around its label) and a
composite-container kind (**`Panel`** — `Header` / `Footer` chrome regions); a
catalog-wide exposure of all 52 controls is explicitly out of scope (FR-007).

This feature **also** delivers the FR-010/FR-011 consumer-capability deliverable:
the package-owned + template-fragment consumer skills (`src/Controls/skill/SKILL.md`,
`template/fragments/controls/skill/SKILL.md`) are expanded to teach the full
shipped **E1–E5** surface with runnable examples. Folding that deliverable in is
the concrete trigger that **ungates** E5 implementation (the roadmap originally
demand-deferred it), so the slot mechanism is implemented here — no longer
deferred — and the skill documents E1–E5 as all-shipped capabilities (no
Principle V synthetic-evidence disclosure for an unshipped feature, FR-009/FR-010).

**Technical approach** — slots ride the **existing `Attr` mechanism under a new
`AttrCategory.Slot`**, exactly as E3 rode `AttrCategory.Style` (FR-004): a new
`AttrCategory.Slot` case and a new
`AttrValue.SlotFillsValue of (string * Control<'msg>) list` case on `Types.fsi`,
carrying an ordered name→fill association list (the codebase's single-attr
last-writer convention, mirroring `StyleClassesValue`). No new top-level `Control`
record field is added; a slotted child therefore travels through the keyed
reconciler diff (067/091/092) and keeps its E2 retained identity by construction.
**Closure is enforced at the typed `Props` front door — not by any public
slot-name surface** (the deliberate divergence from E3's free-form
`StyleClass.Custom`): the string slot name is **internal plumbing** carried behind
the existing `module internal ControlInternals` seam (no public `SlotName` type,
no public `Attr.slot name child` builder); the **only** sanctioned public
authoring path is per-kind typed optional `Props` fields. `ButtonProps` gains
`Leading` / `Trailing` and `PanelProps` gains `Header` / `Footer`, each
`Widget<'msg> option` defaulting to `None`. A consumer therefore cannot fill a
slot a kind does not declare — there is no field for it, so filling a `Header` on
a `Button` is a **compile-time error**, not a silent runtime drop (US1 scenario 3,
FR-001, SC-006). The `AttrValue.SlotFillsValue` carrier is emitted by an
`module internal ControlInternals.slotFill` helper the typed views call; lowering
an unfilled slot to its default chrome and a filled slot to the supplied sub-tree
is a **pure, total, deterministic** function in `Control.fs` (FR-006), injecting
fills into the lowered control's `Children` so they inherit E1–E4 + E2 free. `None`
for every slot ⇒ no slot `Attr` ⇒ byte-identical to today (FR-003). The typed
front door (`FS.Skia.UI.Controls.Typed`) is where consumers fill slots.

> Design note (reconciles with `research.md` Decision 2 / `data-model.md` Entity 1):
> the IR carrier keys fills by an **internal string**, not a public per-kind
> `SlotName` DU. Closure is achieved entirely by the typed `Props` record fields,
> which is lighter public surface than N per-kind slot-name unions while giving the
> same compile-time guarantee (FR-001) — chosen for idiomatic simplicity
> (Principle III). The string key is never a consumer affordance.

This is a **Tier 1** change: it moves public surface (new
`AttrCategory.Slot` / `AttrValue.SlotFillsValue` on `Types.fsi`; new typed `Props`
fields on `Widgets/Primitives.fsi` (`Button`) and `Widgets/Containers.fsi`
(`Panel`)), so controls-public-surface + per-package + cross-package surface
baselines MUST be recaptured.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: existing `FS.Skia.UI.Controls` deps only (SkiaSharp 4
preview via Scene; Layout/Yoga via `ControlInternals`). No new package dependency.
**Testing**: Expecto + FsCheck (property tests for slot-lowering purity /
determinism / totality over ≥1000 generated `(kind, slot fills)` combinations,
SC-005), deterministic structural-`Scene`-equality / lowered-IR parity tests
(SC-001, SC-002), FSI transcript exercising the typed slot-fill front door through
the packed library, FAKE targets per `Route`.
**Target Platform**: Windows and Linux (deterministic render-only evidence; no
live window required — parity is structural `Scene` / lowered-`Control<'msg>`
equality, not pixel PNGs, because `SceneEvidence` render functions are
capability-hash functions, not pixel encoders).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A to `.template.config/template.json` *content* — no
  new capability, sample, command, or package-policy surface ships into the
  `dotnet new fs-skia-ui` template, and no `template/capabilities.yml` change. The
  Controls capability fragment's **skill body** (`template/fragments/controls/skill/SKILL.md`)
  is edited to carry the E1–E5 consumer guidance (FR-011), which a generated
  project selecting the Controls capability receives; this is fragment-content,
  not a template-manifest change. The template's package **pins** are refreshed on
  merge per the standard version-bump flow (all packable libraries bumped,
  `FsSkiaUiVersion` updated), not by this plan's edits.
- **Dependency impact**: N/A — no new dependency. `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` coverage are unchanged; the slot
  mechanism uses only existing `Types`/`Control`/`Widget` types.
- **Command-surface impact**: No new gate or build target. `build.fsx`/`Routing.fs`
  are unchanged — the change routes through the **existing** controls-public-surface
  / package-surface escalation rules (public `src/Controls/*.fsi` +
  `src/Controls/Widgets/*.fsi` edits) and the skill edits route through the
  package-surface rule validated by `SkillSyncCheck` / `SkillQualityCheck` /
  `GeneratedGuidanceCheck`. FAKE-backed targets run **sequentially** in the
  deterministic escalated order — `./fake.sh build -t Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
  `EvidenceGraph` → `EvidenceAudit` — plus `ContrastCheck` / `ControlFidelity`
  (apply to slotted-content rendering as to any control). `Route` is run first and
  only the gates it prints are run. Surface baselines recaptured via
  `RefreshSurfaceBaselines` and `PerPackageSurface.captureCurrent`; the `.claude`
  skill peer is regenerated from the canonical `.agents` source, never hand-edited.
- **Generated project impact**: Additive only. A generated project that fills no
  slot renders identically (FR-003). The **selected Controls guidance** changes:
  the Controls capability fragment skill gains the E1–E5 consumer guidance
  (FR-011, US4), so a generated project selecting Controls receives the updated
  runnable examples (SC-009). No change to default/minimal generated *code*
  contents, validation logs, placeholder/excluded-history scans, or generated
  `Dev` behavior.
- **Evidence paths**: All under `specs/095-lookless-slot-composition/readiness/`:
  - `us1-slot-fill-regions.md` — a consumer fills a declared named slot and the
    lowered IR / rendered control shows the supplied sub-tree at that region; two
    slots place into two distinct regions without collision/swap (US1 / SC-001).
  - `us2-unfilled-byte-identical.md` + `parity/<kind>.<theme>.<state>.scene.txt`
    captured baselines — a slot-bearing kind with **no** slots filled is
    structurally-`Scene`-equal to a captured pre-slot baseline across the kind's
    states, and a non-slotted kind is unchanged (US2 / SC-002 / SC-007).
  - `us3-compose-e1-e4.md` — a binding inside slotted content dispatches (E1), its
    E3 style resolves, and a focusable slotted control is in the E4 tab order
    (US3 / SC-003).
  - `sc004-retained-identity.md` — a focused/with-text slotted control keeps its
    E2 retained identity across a sibling-shifting (092-case) re-render through the
    **live** retained path, not a hand-seeded `StateByIdentity` map (US3 / SC-004).
  - `sc005-lowering-property.md` — slot-lowering purity / determinism / totality
    over ≥1000 generated `(kind, slot fills)` combinations; lowering never throws
    (SC-005).
  - `sc006-typed-closed-and-nongoals.md` — an attempt to fill an undeclared slot is
    a compile-time error (a failing-to-compile fixture / typed-surface proof, not a
    runtime drop), and a structural inspection confirms **no** `DataContext` /
    binding / template-instantiation surface was introduced (SC-006 / FR-008).
  - `us4-skill-e1-e5.md` — `src/Controls/skill/SKILL.md` and
    `template/fragments/controls/skill/SKILL.md` each name + show a runnable
    consumer example for every rung E1–E5, honest (slot lowers to `Control<'msg>`,
    not a data-bound template); `SkillSyncCheck` / `SkillQualityCheck` /
    `GeneratedGuidanceCheck` green; a generated project receives the guidance
    (US4 / SC-008 / SC-009).
  - `fsi-transcript.md` — FSI exercise of the typed slot-fill front door through
    the packed library surface (Principle I).
  - `surface-baselines.md` — recaptured controls-public-surface / per-package /
    cross-package baseline diffs.
- **`.fsi` / contract impact**: **Tier 1, surface moves.** New public surface on
  `src/Controls/Types.fsi` (`AttrCategory.Slot` case; `AttrValue.SlotFillsValue of
  (string * Control<'msg>) list` case); new typed `Props` fields on
  `src/Controls/Widgets/Primitives.fsi` (`ButtonProps.Leading` / `.Trailing`) and
  `src/Controls/Widgets/Containers.fsi` (`PanelProps.Header` / `.Footer`), each
  `Widget<'msg> option`. The `slotFill` carrier builder and `slotFillsOf` extractor
  are **`module internal ControlInternals`** (not public surface), and there is
  **no** public per-kind `SlotName` type or string-keyed `Attr.slot` builder — the
  **only** sanctioned public authoring path is the typed `Props` fields, which is
  what makes the slot surface closed per kind with no free-form escape hatch
  (FR-001). controls-public-surface
  + per-package + cross-package baselines recaptured. The slot surface is documented
  honestly (a slot lowers to `Control<'msg>`; it is not a data-bound template).
  Compatibility: purely additive — the `view : 'model -> Control<'msg>` contract is
  unchanged; a consumer filling no slot (`None` defaults) sees no behavior change.
- **MVU/effect boundary**: N/A — slot lowering is a **pure, total function**
  `(kind + slot fills) → Control<'msg>`; it introduces no `Model`/`Msg`/`Effect`/`Cmd`,
  no interpreter, and no I/O (FR-006). Slotted content's events/focus/text/animation
  use the **existing** E1 (binding dispatch), E2 (retained identity), E3 (style
  resolve), and E4 (focus/key routing) mechanisms unchanged — E5 owns, mutates, or
  re-derives none of that state (FR-005). No new Elmish ceremony is warranted
  (Principle IV — pure structural function, not a stateful/I/O workflow).
- **Synthetic evidence**: None planned. All evidence is real — structural `Scene` /
  lowered-`Control<'msg>` equality from the actual lowering and an actual captured
  pre-slot baseline, FsCheck-generated real inputs (SC-005), and the SC-004 retained
  identity proven through the **live** retained render path, not a hand-seeded
  `StateByIdentity` map (the 092 gap this explicitly avoids repeating). The
  compile-time-rejection proof (SC-006) is a real does-not-compile fixture, not a
  mock. E1–E5 are all shipped by this feature, so the FR-010 skill carries **no**
  Principle V disclosure. If any `[S]` appears it triggers the full Principle V
  disclosure regime.
- **Test evidence**: Failing-first semantic tests in `Controls.Tests`: (1) a
  slot-placement test asserting the lowered IR contains the supplied sub-tree at the
  filled slot's region and two slots land in two distinct regions — fails before
  lowering places fills; (2) a parity test asserting structural-`Scene` equality vs
  a captured pre-slot baseline for each representative (kind, theme, state) with no
  slots filled — fails if exposing slots shifts the default render; (3) an FsCheck
  property asserting slot-lowering purity / determinism / totality over ≥1000
  inputs; (4) a compose test (slotted binding dispatches E1, resolves E3, is in the
  E4 tab order); (5) a retained-identity test through the live path (092 case); (6)
  an unmigrated-unchanged regression test; (7) a typed-closure proof (filling an
  undeclared slot does not compile). Governance: surface-baseline tests
  (controls-public-surface / per-package / cross-package), `SkillSyncCheck`,
  `SkillQualityCheck`, `GeneratedGuidanceCheck`, `ContrastCheck` / `ControlFidelity`.
- **Observability**: Slot lowering is pure and total over the representative kinds'
  render branches (every declared region has a default) — no partial match, no
  exception path (FR-006). An unfilled slot deterministically falls back to default
  chrome (documented, not silent); a slot fill the lowering branch does not place is
  unrepresentable in the typed surface (compile error), so there is no silent-drop
  failure mode to diagnose. No new structured-log surface; the existing
  `ControlDiagnostic` / `ControlFidelity` / `ContrastCheck` channels remain
  authoritative for slotted content as for any control.
- **Deferred scope**: Out of scope and bounded as follow-ups — a catalog-wide slot
  exposure of all 52 controls (representative `Button` + `Panel` only here, FR-007);
  a data-bound `ControlTemplate` engine, `DataContext`, binding
  expressions/observables, per-item template instantiation, dependency/attached
  properties, CSS-selector styling (permanent roadmap non-goals, FR-008); a visual
  theme/skin editor UI; a live windowed pixel-PNG capture path; a new standalone
  `.agents/skills/<id>` governance skill (the consumer guidance rides the existing
  package + template-fragment skills, FR-011). The slot mechanism itself is
  **implemented in this feature** — the roadmap's demand-gate is satisfied by
  folding in the FR-010 consumer-skill deliverable (FR-009), so implementation is no
  longer deferred.

**Initial Constitution Check: PASS** — Tier 1 with `.fsi` updates + baseline
recapture planned; no MVU boundary required (pure structural function); no
synthetic evidence (E1–E5 all shipped here, live retained path for SC-004);
idiomatic-simplicity respected (one new `AttrCategory.Slot` case + one
`AttrValue.SlotFillsValue` case carrying an internal-string-keyed fill list +
a plain fill-or-default lowering over the representative kinds' branches — no SRTP,
reflection, type providers, custom operators, or non-trivial computation
expressions).

## Project Structure

```
src/Controls/
  Types.fsi / Types.fs              # + AttrCategory.Slot, AttrValue.SlotFillsValue of
                                    #   (string * Control<'msg>) list case (Tier 1)
  Control.fsi / Control.fs          # ControlInternals: `module internal` slotFill carrier
                                    #   builder + slotFillsOf extractor + pure fill-or-default
                                    #   slot lowering for the representative kinds' branches
  Widgets/Primitives.fsi / .fs      # ButtonProps gains Leading/Trailing : Widget<'msg> option
                                    #   (default None); view lowers fills via slotFills
  Widgets/Containers.fsi / .fs      # PanelProps gains Header/Footer : Widget<'msg> option
                                    #   (default None); view lowers fills via slotFills
  skill/SKILL.md                    # fs-skia-ui-widgets — + E1–E5 consumer guidance (FR-010/011)

template/fragments/controls/skill/SKILL.md   # fs-skia-generated-controls-guidance — + E1–E5 guidance

.agents/skills/ (canonical) → .claude/skills/ (generated peer)   # SkillSyncCheck — regenerated, not hand-edited

test/Controls.Tests/                # slot-placement, parity, FsCheck property, compose,
                                    #   retained-identity (live path), regression, closure-proof

specs/095-lookless-slot-composition/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/slot-mechanism.md       # the slot declaration / fill / lowering public contract
  contracts/typed-slot-surface.md   # the typed Props slot-fill front-door contract (closure)
  contracts/consumer-capability-skill.md   # the E1–E5 skill deliverable contract
  checklists/requirements.md
  readiness/                        # evidence artifacts (paths above)
```

**Insertion points**: `AttrCategory.Slot` and `AttrValue.SlotFillsValue` land on
the already-Tier-1 `Types.fsi` (alongside the E3 `StyleClassesValue` /
`VisualStateValue` cases they mirror). The `module internal slotFill` carrier /
`slotFillsOf` extractor and the pure slot-lowering function live in `Control.fs`
(`ControlInternals`), in scope for both representative widgets' views. No `Controls.fsproj` compile-order change is
required — all touched files already exist and compile in their current order
(`Types` → `Control` → `Widgets/Primitives` → … → `Widgets/Containers`).

## Complexity Tracking

No constitution deviations requiring justification. The slot carrier is one new
`AttrCategory.Slot` case plus one `AttrValue.SlotFillsValue` case mirroring E3's
`AttrCategory.Style` / `StyleClassesValue`; lowering is a plain fill-or-default
branch over the two representative kinds. No Principle III
escape (SRTP, reflection, type providers, custom operators, non-trivial CEs,
multi-case active patterns) is used. The single deliberate design choice worth
naming — closure enforced by typed `Props` fields rather than a public free-form
builder — is the spec's FR-001 divergence from E3, justified there and carried
here, not a complexity escape.
