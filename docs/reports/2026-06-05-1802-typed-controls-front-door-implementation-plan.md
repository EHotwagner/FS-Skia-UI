# Typed Controls Front Door — Implementation Plan

**Date:** 2026-06-05 18:02:02 +0200
**Status:** Implementation plan. No product code changed by this document.
**Feature (proposed):** `065-typed-controls-front-door`
**Scope:** Introduce an additive, compile-time-typed authoring surface (`Widget<'msg>` + per-control immutable `Props` records, plus per-control `Model`/`Msg`/`Effect`/`update` where the control owns ephemeral UI state) that lowers to the existing `Control<'msg>` IR. Prove it on a six-control reference slice without breaking the shipped `FS.Skia.UI.Controls` API.

This plan is the merged, source-checked execution of the keystone feature identified in the two prior reports:

- `docs/reports/2026-06-05-1421-controls-suite-and-penpot-integration-analysis.md` (architecture foundation: two-axis model, FuncUI prior art, lowered-IR strategy)
- `docs/reports/2026-06-05-1429-controls-suite-penpot-speckit-plan.md` (execution scaffold: variable taxonomy, test plan, evidence artifacts, governance tables)

It deliberately produces **only the typed front door for a representative slice**. Token/Penpot work, full 47-control migration, keyed reconciliation, and catalog regeneration are explicitly **out of scope** and sequenced as later features (see §13).

---

## 1. Objective and success criteria

### 1.1 Objective

Replace the *preferred* public authoring path for controls from a weakly typed
`Attr<'msg> list` keyed by strings into per-control immutable `Props` records the
F# compiler checks, while keeping the entire downstream pipeline
(`Control<'msg>` IR → render → layout → diagnostics → event bindings → evidence)
byte-for-byte unchanged.

### 1.2 Success criteria (acceptance)

A reviewer can confirm the feature is done when **all** of these hold:

1. A new public type `Widget<'msg>` exists in `FS.Skia.UI.Controls`, declared in a curated `.fsi`.
2. Six controls expose a typed `Props` record + `defaults` + `view` (and, for stateful ones, `init`/`update`): `TextBlock`, `Button`, `CheckBox`, `TextBox`, `Stack`, `DataGrid`.
3. Every typed `view` lowers to a `Control<'msg>` that is **structurally equal** to what the equivalent legacy `Control.create`/`Attr` call produces today (proven by a parity test, see §10.3).
4. The legacy `Control.create` / `Attr` / per-control `*.create` API is unchanged and still compiles, with no behavioral diff in existing tests.
5. `./fake.sh build -t Route` over the branch diff prints the `controls-public-surface` escalation; **every printed gate passes**, including `PackageSurfaceCheck` against an intentionally-updated surface baseline.
6. The two evidence artifacts the routing rule already requires exist and are populated: `readiness/typed-controls-front-door.md` and `readiness/package-surface-expectations.md`.
7. No new dependency is added to `FS.Skia.UI.Controls` (in particular **not** `Fable.Elmish`).

### 1.3 Explicit non-goals

- No removal or deprecation-flagging of the legacy `Attr` API in this feature (deprecation is a later, separate decision — see §12 open question Q1).
- No design-token / Penpot work.
- No keyed VDOM diff/reconciliation.
- No catalog regeneration from the typed source (catalog stays hand-authored as today; typed-catalog generation is feature `066`).
- No new controls beyond the six slice members; the other 41 stay legacy-only.

---

## 2. Grounding: verified current state

All claims below were read from source on 2026-06-05, not from the prior reports.

| Fact | Evidence in repo |
| --- | --- |
| Core IR is string-keyed | `src/Controls/Types.fsi:231` — `Control<'msg> = { Kind: ControlKind (=string); Key; Attributes: Attr<'msg> list; Children; Content; Accessibility }` |
| Attributes are name-keyed, value union includes escape hatch | `Types.fsi:239` `Attr<'msg> = { Name: string; Category; Value }`; `:244` `AttrValue` includes `UntypedValue of obj` |
| A partial "typed" layer already exists but is still string-carrying | `Types.fsi:49-98` — `StandardControlKind`, `StandardEventKind`, `StandardAttributeName`, `StandardAttributeValue<'msg>` (the latter still has `StandardUntyped of obj`) |
| Legacy authoring entry points | `src/Controls/Control.fsi` — `Control.create`, `Control.standard`, per-control modules `Button`, `TextBlock`, `CheckBox`, `Stack`, … (`create: Attr<'msg> list -> Control<'msg>`) |
| The render contract is stable and IR-driven | `Control.fsi:18` `render: Theme -> Control<'msg> -> ControlRenderResult<'msg>` |
| Stateful-control MVU pattern already exists for 3 controls | `src/Controls/TextInput.fsi` — `TextInputModel`/`TextInputMsg`/`TextInputEffect`, `init`/`update`/`interpretEffect`/`diagnostics`; same shape in `DataGrid.fsi`, `Collections.fsi` |
| Elmish dependency split is already correct | `Controls.fsproj` references only `Scene`, `Layout`, `KeyboardInput` — **no `Fable.Elmish`**; `Controls.Elmish.fsproj` owns it |
| Adapter view signature is the integration seam to touch later | `ControlsElmish.fsi:33` — `View: 'model -> Control<'msg>` |
| Routing already escalates this path and already names the evidence slot | `build/Governance/Routing.fs:131` rule `controls-public-surface`, paths `["src/Controls/**"]`, tier `FocusedAuthority`, gates `ControlsCatalogCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`, `PackageSurfaceCheck`, `FsiTranscripts`, `GeneratedProductCheck`; expected artifacts already include `readiness/typed-controls-front-door.md` |
| A typed-contract test file already exists (currently asserts the `Standard*` surface) | `tests/Controls.Tests/TypedControlContractTests.fs` — reads `Types.fsi`/`Control.fsi` text and asserts presence of typed declarations |
| Test surfaces to extend | `tests/Controls.Tests/`: `PublicSurfaceTests.fs`, `RenderingTests.fs`, `InteractionTests.fs`, `AccessibilityTests.fs`, `CatalogTests.fs`, `TextInputTests.fs`, `DataGridTests.fs`, `TypedControlContractTests.fs` |
| Sample to extend for the gallery smoke | `samples/ControlsGallery/Program.fs` |
| Package is a shipped public contract | `Controls.fsproj:8` `Version 0.1.68-preview.1`, `IsPackable=true`, `PackageId=FS.Skia.UI.Controls` |

**Key consequence:** the routing rule *already* lists `readiness/typed-controls-front-door.md` as an expected artifact, so the governance system is pre-wired for this feature. The plan must produce that file (today it is the missing artifact `Route --enforce` would flag).

---

## 3. Architecture decisions

### 3.1 Two-axis control model (confirmed direction)

Every control is:

```
Control = (Props : immutable typed record)  ×  (optional MVU : Model × Msg × update)
```

- **`Props<'msg>`** — the "well-defined variable values" for the control; a closed record. This is the public authoring surface and the compile-time contract. Defaults via a `defaults` value; modification via record `with`.
- **MVU** — present **only** for controls that own ephemeral UI state. `TextBlock`, `Button`, `CheckBox`, `Stack` are pure `Props -> Widget`. `TextBox` and `DataGrid` carry `Model`/`Msg`/`update`.
- **`Widget<'msg>`** — the new public return type of every `view`. It is a thin, opaque wrapper that lowers to the existing `Control<'msg>` IR.

### 3.2 `Widget<'msg>` representation — DECISION

`Widget<'msg>` wraps the lowered IR plus the typed provenance needed for later features, but exposes neither on the `.fsi` beyond what consumers need:

```fsharp
// Types.fsi (additive)
[<Sealed>]
type Widget<'msg>

module Widget =
    val ofControl : Control<'msg> -> Widget<'msg>      // internal-leaning escape hatch / bridge
    val toControl : Widget<'msg> -> Control<'msg>      // lowering accessor used by render + adapter
    val render    : Theme -> Widget<'msg> -> ControlRenderResult<'msg>   // convenience = render (toControl w)
```

Rationale for a sealed wrapper rather than a bare alias `type Widget<'msg> = Control<'msg>`:

- Keeps the door open for keyed reconciliation metadata (feature `067`) without another public-surface break.
- Forces consumers through `Widget.toControl` so the lowering seam is explicit and greppable.
- The legacy `Control<'msg>` API is untouched; `Widget` and `Control` coexist. (This resolves prior-report open question "should `Control<'msg>` itself become the typed tree" with **no** — keep them distinct during the preview window.)

**Internal field** (`.fs` only): `Widget<'msg> = private { Lowered: Control<'msg> }`. The record stays internal so the public surface is just the sealed type + module.

### 3.3 Lowering pipeline (unchanged downstream)

```
Props<'msg>  --(module view)-->  Widget<'msg>  --(Widget.toControl)-->  Control<'msg>  --(Control.render)-->  ControlRenderResult<'msg>
```

Nothing in `Control.fs`, the renderer, layout, diagnostics, accessibility, or evidence changes. Typed `view` functions are pure constructors that emit the **same** `Attr<'msg> list` the legacy builders emit — verified by parity tests.

### 3.4 Variable taxonomy (applied per Props record)

Each `Props` record draws its fields from a fixed taxonomy so the six records are consistent and future controls follow the template:

| Class | Meaning | Example field |
| --- | --- | --- |
| Identity | stable id/key for diffing, events, focus | `Id: ControlId option` |
| Content | text, icon, children | `Text: string`, `Children: Widget<'msg> list` |
| Data | product-owned values/sources | `Rows`, `Columns`, `SelectedKey` |
| Behavior | control behavior not owned by theme | `ReadOnly`, `Enabled` |
| Variant | semantic style intent | `Intent: ButtonIntent` |
| Layout | sizing/alignment | `Width: float option`, `Orientation` |
| Theme/style | token/style references | `StyleClass: string option` |
| Accessibility | role/name/keyboard | `AccessibleName: string option` |
| Events | Elmish message callbacks | `OnClick: 'msg option`, `OnChanged: (string -> 'msg) option` |

Rule: every **required** value is a non-optional field; optional values get defaults via `defaults`. No optional string event names and no `obj` payloads in the typed surface.

### 3.5 MVU contract shape (uniform with existing TextInput)

For stateful controls, mirror the **exact** shape already shipped in `TextInput.fsi`:

```fsharp
module TextBox =
    val defaults : ControlId -> TextBoxProps<'msg>
    val init     : TextBoxProps<'msg> -> TextBoxModel * TextBoxEffect list
    val update   : TextBoxMsg -> TextBoxModel -> TextBoxModel * TextBoxEffect list
    val view     : TextBoxProps<'msg> -> TextBoxModel -> Widget<'msg>
```

`TextBox` reuses the **existing** `TextInputModel`/`TextInputMsg`/`TextInputEffect` (do not invent a parallel model) — the typed `TextBox.view` is a thin typed façade over `TextInput` + the legacy `TextBox` attrs. `DataGrid` likewise reuses the existing `DataGrid` model types. This keeps the feature additive at the model layer too.

---

## 4. The reference slice (six controls)

Chosen to exercise every distinct mechanic exactly once:

| Control | Mechanic exercised | Stateful? | Reuses existing model |
| --- | --- | --- | --- |
| `TextBlock` | content-only, pure | no | — |
| `Button` | command/event + variant (`Intent`) | no | — |
| `CheckBox` | boolean state + `(bool -> 'msg)` event | no | — |
| `TextBox` | text-input runtime, validation | **yes** | `TextInputModel/Msg/Effect` |
| `Stack` | layout composition over `Widget` children | no | — |
| `DataGrid` | data + bounded visible range runtime | **yes** | existing `DataGrid` model |

### 4.1 Illustrative typed surfaces (final API to be fixed in spec)

```fsharp
type ButtonIntent = Primary | Secondary | Danger | Ghost

type ButtonProps<'msg> =
    { Id: ControlId option
      Text: string
      Enabled: bool
      Intent: ButtonIntent
      OnClick: 'msg option }

module Button =
    val defaults : ButtonProps<'msg>
    val view     : ButtonProps<'msg> -> Widget<'msg>
```

```fsharp
type StackOrientation = Vertical | Horizontal

type StackProps<'msg> =
    { Id: ControlId option
      Orientation: StackOrientation
      Spacing: float
      Children: Widget<'msg> list }

module Stack =
    val defaults : StackProps<'msg>
    val view     : StackProps<'msg> -> Widget<'msg>
```

Authoring stays terse and compiler-checked:

```fsharp
Stack.view
  { Stack.defaults with
      Orientation = Vertical
      Children =
        [ TextBlock.view { TextBlock.defaults with Text = "Sign in" }
          Button.view    { Button.defaults with Text = "Submit"; Intent = Primary; OnClick = Some Save } ] }
```

---

## 5. Package and file layout

All new files land in `src/Controls/` (so they ship in `FS.Skia.UI.Controls`, no project moves). New compile units, inserted into `Controls.fsproj` **after** `Control.fs` (so `Widget` can depend on `Control`) and after `TextInput.fs`/`DataGrid.fs` for the stateful façades:

| New file | Contents |
| --- | --- |
| `src/Controls/Widget.fsi` / `Widget.fs` | sealed `Widget<'msg>`, `Widget.ofControl/toControl/render` |
| `src/Controls/Widgets/Primitives.fsi` / `.fs` | `TextBlockProps`, `ButtonProps`/`ButtonIntent`, `CheckBoxProps`, `StackProps`/`StackOrientation` + their `defaults`/`view` |
| `src/Controls/Widgets/TextBoxWidget.fsi` / `.fs` | `TextBoxProps`, typed `TextBox` MVU façade over `TextInput` |
| `src/Controls/Widgets/DataGridWidget.fsi` / `.fs` | `DataGridProps`, typed `DataGrid` MVU façade |

> Naming note: the legacy `module TextBox`/`module DataGrid` already exist in `Control.fsi`. To avoid collision, the typed modules live under a distinct namespace segment (e.g. `FS.Skia.UI.Controls.Typed`) **or** are named `TextBoxWidget`/`DataGridWidget`. Final choice is a spec decision (Q2, §12) — the plan assumes `FS.Skia.UI.Controls.Typed.*` so the six typed modules can keep the clean names `Button`, `TextBox`, etc. without shadowing legacy ones.

`Controls.fsproj` `<Compile>` insertions (order matters in F#):

```
... Control.fsi/fs ...
Widget.fsi / Widget.fs                 <- after Control
... Catalog, TextInput, ControlRuntime, Collections, Charts, RichText, DataGrid ...
Widgets/Primitives.fsi / .fs           <- after DataGrid (depends only on Widget + Control)
Widgets/TextBoxWidget.fsi / .fs        <- after TextInput + Widget
Widgets/DataGridWidget.fsi / .fs       <- after DataGrid + Widget
```

---

## 6. Compatibility strategy

- **Legacy API: frozen, not touched.** `Control.create`, `Attr.*`, `Control.standard`, and all 47 per-control `*.create` modules remain exactly as in `Control.fsi`/`Attributes.fsi`. No signatures change, so `PackageSurfaceCheck` sees only **additions**.
- **Bridge in both directions.** `Widget.ofControl` lets a consumer drop a legacy `Control<'msg>` into a typed `Stack.Children` list during migration; `Widget.toControl` lets the renderer and the Elmish adapter consume a `Widget` today without an adapter change (call `Control.render (Widget.toControl w) `).
- **Adapter untouched this feature.** `ControlsElmish.AdapterProgram.View: 'model -> Control<'msg>` stays. Consumers using the typed surface simply finish their `view` with `Widget.toControl`. Converging the adapter onto `Widget`/`Cmd<'msg>` is feature `068` (Q3, §12).
- **Surface baseline bump is expected and intentional.** The feature *adds* public API, so `PackageSurfaceCheck` will fail until the baseline is regenerated. Regenerating the baseline is an explicit task (T13), reviewed as part of the diff.

---

## 7. Governance and routing

### 7.1 Routing — no new rule needed

The change is confined to `src/Controls/**`, which already matches rule
`controls-public-surface` (`Routing.fs:131`). Running `./fake.sh build -t Route`
on the branch will print tier `FocusedAuthority` and this gate set:

1. `ControlsCatalogCheck`
2. `ControlsInteractionCheck`
3. `ControlsRenderingCheck`
4. `PackageSurfaceCheck`
5. `FsiTranscripts`
6. `GeneratedProductCheck`

No `Routing.fs` edit is required for the typed front door itself. (Routing edits
start at the token feature.) Because the change also touches public `.fsi` files,
package-surface gating applies regardless.

### 7.2 Required evidence artifacts

The rule's `ExpectedArtifacts` are already:

- `readiness/typed-controls-front-door.md` ← **produced by this feature** (currently missing)
- `readiness/package-surface-expectations.md` ← **produced/updated by this feature**

`./fake.sh build -t Route --enforce` will fail until both exist with content. These live under the feature's spec dir: `specs/065-typed-controls-front-door/readiness/`.

### 7.3 Constitution touch-points

- **Principle II (visibility in `.fsi`):** every new typed module needs a curated `.fsi`; the `Props` records are public, the `Widget` internal record field is not.
- **Principle IV (MVU boundary):** `TextBox`/`DataGrid` typed façades expose pure `init`/`update` and reuse the existing effect types; no I/O in updates.
- **Principle V (synthetic disclosure):** if any typed `view` ships with placeholder lowering, it must carry the `[S]` disclosure. The intent here is **real** lowering (parity-tested), so no `[S]` should be needed — call this out explicitly in the evidence file.

### 7.4 Validation order (escalated maintainer-verify path)

Because this is a consumer-contract change (public `.fsi`), run the serialized
FAKE-backed order sequentially (per AGENTS.md — never concurrently):

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

plus the `Route`-printed gates (§7.1). Run `Route` **first** and only run what it prints; the six-target order above is the escalation this change qualifies for.

---

## 8. Catalog impact

`catalog.yml` (`supportedCount: 47`) and `Catalog.fs` are **hand-authored** today.
The six typed controls correspond to existing catalog rows — no new rows. This
feature does **not** regenerate the catalog from the typed source (that is feature
`066`). `ControlsCatalogCheck` should therefore pass unchanged. The only catalog
action: confirm the six rows' `Examples`/`Tests` lists still resolve (they point at
`samples/ControlsGallery/Program.fs` and the `Controls.Tests` files we extend).

---

## 9. Implementation task breakdown (dependency-ordered)

Tasks are sized for `speckit-tasks`; IDs are illustrative. `[P]` = parallelizable
after its dependency.

| ID | Task | Depends on | Output |
| --- | --- | --- | --- |
| T1 | Failing-first contract tests: assert `Widget<'msg>` + six typed modules exist in `.fsi` (extend `TypedControlContractTests.fs`) | — | red tests |
| T2 | Author `Widget.fsi`/`Widget.fs` (sealed type, `ofControl`/`toControl`/`render`) | T1 | new compile unit |
| T3 | Wire `Widget.*` into `Controls.fsproj` after `Control.fs`; confirm `Dev` builds | T2 | green build |
| T4 | `TextBlock` typed Props/defaults/view + lowering | T3 | primitive |
| T5 | `Button` typed Props/`ButtonIntent`/defaults/view | T3 | primitive |
| T6 | `CheckBox` typed Props/defaults/view (`bool -> 'msg`) | T3 | primitive |
| T7 | `Stack` typed Props/`StackOrientation`/defaults/view over `Widget` children | T3 | primitive |
| T8 [P] | `TextBox` typed façade over existing `TextInput` (`defaults`/`init`/`update`/`view`) | T3 | stateful |
| T9 [P] | `DataGrid` typed façade over existing `DataGrid` model | T3 | stateful |
| T10 | Parity tests: each typed `view` ≡ legacy builder output (structural `Control<'msg>` equality) | T4–T9 | parity proof |
| T11 | Interaction tests: typed `OnClick`/`OnChanged`/MVU dispatch yields expected `'msg`/effects | T4–T9 | interaction proof |
| T12 | Accessibility + rendering tests for the six typed views at ≥2 viewports | T4–T9 | a11y/render proof |
| T13 | Regenerate public-surface baseline; run `PackageSurfaceCheck` | T4–T9 | surface diff |
| T14 | Extend `samples/ControlsGallery/Program.fs` with a typed-authoring panel | T4–T9 | gallery smoke |
| T15 | Write `readiness/typed-controls-front-door.md` + update `readiness/package-surface-expectations.md` | T10–T13 | evidence |
| T16 | Run `Route` + serialized six-target order; fix to green | all | gate pass |

Critical path: T1→T2→T3→(T4..T9)→T10→T15→T16. T8/T9 and T4–T7 parallelize after T3.

---

## 10. Test plan (mapped to existing files)

### 10.1 Surface / contract — `TypedControlContractTests.fs`, `PublicSurfaceTests.fs`
- Assert `Types.fsi` (or `Widget.fsi`) declares `Widget`, `Widget.toControl`, `Widget.ofControl`.
- Assert each of the six typed modules exposes `defaults` and `view` (and `init`/`update` for `TextBox`/`DataGrid`).
- Assert **no** `obj` appears in any new typed `Props` field (grep the new `.fsi`).

### 10.2 Failing-first (TDD)
- Each new test in T1/T10/T11 must be committed red first, then made green by the implementation task — matches the repo's failing-first convention.

### 10.3 Lowering parity — new `TypedLoweringTests.fs`
The keystone test. For each control, build the **same** logical control two ways and assert structural equality of the resulting `Control<'msg>`:

```fsharp
let legacy  = Button.create [ Button.text "Submit"; Button.enabled true; Button.onClick Save ]   // existing API
let typed   = Typed.Button.view { Typed.Button.defaults with Text = "Submit"; OnClick = Some Save } |> Widget.toControl
Expect.equal (normalize typed) (normalize legacy) "typed Button lowers to legacy IR"
```

`normalize` sorts attributes by name to ignore ordering. This proves the typed
surface is a faithful façade and protects every downstream test (render, a11y,
diagnostics) without duplicating them.

### 10.4 Interaction — `InteractionTests.fs`
- Typed `Button.OnClick = Some msg` produces a `ControlEventBinding` whose dispatch yields `msg`.
- Typed `CheckBox.OnChanged` maps a toggle event to `(bool -> 'msg)`.
- `TextBox.update`/`DataGrid.update` via the typed façade equal the existing `TextInput.update`/`DataGrid.update` results (delegate, don't fork).

### 10.5 Accessibility & rendering — `AccessibilityTests.fs`, `RenderingTests.fs`
- Each typed view's lowered control carries the same `AccessibilityMetadata` the legacy path does (follows automatically from parity, but assert for `Button`/`CheckBox`/`TextBox`).
- Render the six-control typed gallery panel at two viewports; node counts stable.

### 10.6 Elmish boundary — `tests/Elmish.Tests`
- A small program whose `view` ends in `Widget.toControl` runs through `AdapterProgram` unchanged (proves the bridge needs no adapter edit).
- Assert `Controls.fsproj` still has no `Fable.Elmish` reference (dependency-governance guard).

---

## 11. Evidence artifacts to produce

Under `specs/065-typed-controls-front-door/readiness/`:

- `typed-controls-front-door.md` — the `Widget`/Props design, the six-control slice, lowering-parity results, and an explicit statement that lowering is **real** (no `[S]` synthetic disclosure needed). *(Required by routing rule.)*
- `package-surface-expectations.md` — the additive surface delta and the regenerated baseline rationale. *(Required by routing rule.)*
- `controls-rendering.md` — viewport render evidence for the typed panel.
- `typed-lowering-parity.md` — the parity-test matrix (six controls × legacy≡typed).

---

## 12. Open decisions to resolve in `speckit-clarify`

| # | Question | Plan's default |
| --- | --- | --- |
| Q1 | Deprecate the legacy `Attr`/`*.create` API, or keep it permanently as a peer? | Keep as peer this feature; decide deprecation later. |
| Q2 | Typed module naming to avoid colliding with legacy `module Button`/`TextBox`: `FS.Skia.UI.Controls.Typed.*` namespace, or `*Widget` suffix, or shadow legacy? | `FS.Skia.UI.Controls.Typed.*` namespace. |
| Q3 | Should `AdapterProgram.View` gain a `Widget`-returning overload now, or wait? | Wait (feature `068`); bridge with `Widget.toControl`. |
| Q4 | Is `Widget<'msg>` a sealed wrapper or a bare alias of `Control<'msg>`? | Sealed wrapper (§3.2). |
| Q5 | Does `TextBox`/`DataGrid` reuse existing models or get fresh ones? | Reuse existing `TextInput`/`DataGrid` models. |

These five map to ≤5 `speckit-clarify` questions; the spec should bake the answers in before `speckit-plan`.

---

## 13. Where this sits in the larger program

This feature is **F-α / feature 1** of the merged roadmap. Downstream features
(each its own `specs/NNN-*`), unchanged from the prior reports' sequencing:

1. **065 — Typed controls front door** *(this plan)*
2. 066 — Typed catalog generation (regenerate `catalog.yml`/`Catalog.fs` from the typed registry)
3. 067 — Internal keyed reconciliation (VDOM diff over lowered IR; internal only)
4. 068 — `Controls.Elmish` command model (`Widget` view + `Cmd<'msg>` alignment)
5. 069 — Design tokens + Penpot tokens-first (DTCG JSON → generated F#, `DesignTokenDrift`)
6. 070 — Migrate remaining 41 controls to typed Props/MVU
7. 071+ — Catalog expansion (buttons/pickers/date-time), overlays/virtualization, motion
8. Later — Penpot MCP assist (inspect/draft/provenance), code→design catalog sync

Sequencing rationale (shared by both prior reports): type the authoring layer
first (smallest blast radius, everything else depends on it), wire tokens second,
migrate breadth last.

---

## 14. Risk register

| Risk | Impact | Mitigation |
| --- | --- | --- |
| Public-surface churn breaks consumers | `FS.Skia.UI.Controls` is shipped (v0.1.68) | Additive-only; legacy API frozen; surface baseline reviewed in diff (T13) |
| Typed `view` silently diverges from legacy IR | Downstream render/a11y tests wouldn't catch a typed-only bug | Mandatory lowering-parity test per control (§10.3) is the gate |
| Module-name collision with legacy `Button`/`TextBox` | Compile error or shadowing confusion | `FS.Skia.UI.Controls.Typed.*` namespace (Q2) |
| `Widget` wrapper leaks internals on `.fsi` | Principle II violation | Sealed type + internal record field; `.fsi` exposes only the module functions |
| Elmish dependency creeps into base Controls | Dependency-governance break | Guard test asserts `Controls.fsproj` has no `Fable.Elmish` (§10.6) |
| Scope creep into token/migration work | Feature never lands | Hard non-goals (§1.3); six-control slice only |
| Missing required evidence artifact | `Route --enforce` blocks merge | T15 produces both required `readiness/*.md` files |

---

## 15. Immediate next steps

1. Run `speckit-specify` for `065-typed-controls-front-door` using §3–§4 as the design seed.
2. Run `speckit-clarify` to resolve Q1–Q5 (§12) and bake answers into `spec.md`.
3. Run `speckit-plan` then `speckit-tasks`; expect the task graph to mirror §9.
4. Implement T1 (failing contract tests) before any production code.
5. Run `./fake.sh build -t Route` and only the gates it prints; on the escalated path, run the serialized six-target order sequentially.

---

## 16. Sources

**Repository (read 2026-06-05, authoritative grounding):**
- `src/Controls/Types.fsi` — `Control<'msg>`, `Attr<'msg>`, `AttrValue<'msg>`, `Standard*` types
- `src/Controls/Control.fsi` — legacy authoring modules + `Control.render`
- `src/Controls/Attributes.fsi` — `Attr` builders
- `src/Controls/TextInput.fsi` — reference MVU contract shape
- `src/Controls.Elmish/ControlsElmish.fsi` — `AdapterProgram.View` seam
- `src/Controls/Controls.fsproj` — compile order, package id/version, dependency set
- `src/Controls/Catalog.fs`, `src/Controls/catalog.yml` — 47-control catalog
- `build/Governance/Routing.fs:131` — `controls-public-surface` rule, gates, expected artifacts
- `tests/Controls.Tests/*` — existing test surfaces to extend
- `AGENTS.md`, `CLAUDE.md` — `Route`-first workflow and serialized six-target order

**Prior analysis (merged into this plan):**
- `docs/reports/2026-06-05-1421-controls-suite-and-penpot-integration-analysis.md`
- `docs/reports/2026-06-05-1429-controls-suite-penpot-speckit-plan.md`
