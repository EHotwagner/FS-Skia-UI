---
title: Comprehensive Controls Suite + Penpot Spec-Kit Integration — Analysis & Plan
category: Design history
categoryindex: 90
---

# Comprehensive Controls Suite + Penpot Spec-Kit Integration — Analysis & Plan

**Date:** 2026-06-05 14:21 (+0200)
**Author:** Analysis prepared with Claude Code
**Status:** Draft for maintainer review (analysis + plan; no code changed)
**Scope:** Two coupled, multi-feature initiatives:
1. A comprehensive **Elmish-native, typed controls/widgets suite** that subsumes the current 47-control catalog.
2. **Penpot** (penpot.app) integration into this repo's **Spec Kit** workflow via its MCP + DTCG design tokens.

> This report is an *analysis and plan*. It deliberately produces no code or
> spec artifacts. The recommended next step (§9) is to run the repo's own
> `speckit-specify` / `speckit-plan` / `speckit-tasks` cycle on the feature
> decomposition proposed here, so the work flows through the existing
> evidence/governance gates rather than around them.

---

## 0. Executive summary

The framework already has a remarkably complete, immutable, Skia-rendered control
catalog (47 controls) and a working Elmish adapter. The single biggest
architectural lever — and exactly what your prompt points at ("elmish controls
with well-defined variable values for each control and immutable rest") — is to
**replace the stringly-typed attribute bag with typed per-control prop records
and per-control MVU models**, then regenerate the catalog/governance artifacts
from that typed source.

Three headline recommendations:

1. **Adopt a typed two-axis control model**, borrowed from Avalonia + FuncUI:
   every control is `(immutable Props record) × (optional Model/Msg/update MVU
   pair)`. *Props* are the "well-defined variable values"; the *immutable rest*
   is the view record produced from them. Keep the existing `Control<'msg>`
   render tree as the **internal lowered IR**, not the public authoring surface.
   This subsumes today's controls without throwing away the renderer, layout
   engine, or diagnostics.

2. **Promote Elmish from adapter to first-class boundary.** `Fable.Elmish 4.2.0`
   is already pinned; `Controls.Elmish` already defines an `AdapterProgram`. The
   gap is that only `TextInput`/`DataGrid`/`Collections` expose real per-control
   `Model`/`Msg`/`update`. Standardize that pattern (Constitution Principle IV
   already mandates it for stateful controls) and ship a per-control MVU
   contract.

3. **Integrate Penpot tokens-first.** The highest-leverage, lowest-risk Penpot
   integration is **DTCG design-token JSON → generated F# theme module**, drift-
   checked like any other governance artifact. MCP-driven board/component
   extraction is valuable but should be *assistive spec-drafting* (feeding
   `speckit-specify`/`speckit-clarify`), never an authoritative generator —
   consistent with this repo's "single home of all rules, generated-not-hand-
   synced" philosophy.

The work is large but decomposes cleanly into ~7 Spec Kit features that can land
incrementally without a big-bang rewrite (§8).

---

## 1. Current state assessment

### 1.1 What exists (grounded in source)

| Area | Location | State |
|---|---|---|
| Control catalog (47 controls) | `src/Controls/Catalog.fs`, `catalog.yml` | Mature; metadata-rich (roles, visual states, events, evidence refs) |
| Core control type | `src/Controls/Types.fs` | **Stringly-typed**: `Control<'msg>` = `{ Kind: string; Attributes: Attr<'msg> list; Children; Content; Accessibility }` |
| Attribute model | `src/Controls/Attributes.fs(i)` | `Attr` is `{ Name: string; Category; Value: AttrValue<'msg> }`; `AttrValue` includes `UntypedValue of obj` |
| Render pipeline | `src/Controls/Control.fs` | `render theme ctrl -> { Scene; Layout; Diagnostics; EventBindings; NodeCount }` |
| Scene graph | `src/Scene/Scene.fs(i)` | Immutable DU, ~20 node kinds; exhaustively matched in `SkiaViewer/SceneRenderer.fs` |
| Layout | `src/Layout/*` | Yoga-backed (`Yoga.Net 3.2.3`) flexbox; Measure/Arrange analog |
| Per-control MVU models | `TextInput.fs`, `DataGrid.fs`, `Collections.fs` | Real `Model`/`Msg`/`update` → `(model, effects)` |
| Elmish adapter | `src/Elmish/*`, `src/Controls.Elmish/*` | `AdapterProgram { Init; Update; View: 'model -> Control<'msg>; Subscriptions }` |
| Packaging | `Controls.fsproj` | Ships as `FS.Skia.UI.Controls` v0.1.68-preview.1 (public NuGet contract) |
| Elmish dependency | `Directory.Packages.props` | `Fable.Elmish 4.2.0` **already centrally pinned** |

### 1.2 The 47-control catalog (today's coverage)

Display: TextBlock, Label, RichText, Image, Icon, Separator, Badge.
Input: Button, IconButton, TextBox, TextArea, NumericInput, Slider.
Selection: CheckBox, Switch, RadioGroup, Tabs.
Collections: ListView, ListBox, MultiSelectList, ComboBox, TreeView, DataGrid.
Layout: Stack, Grid, Dock, Wrap, Border, Panel, ScrollViewer, SplitView.
Navigation: Menu, ContextMenu, Toolbar.
Charts: LineChart, BarChart, PieChart, ScatterPlot.
Graph: GraphView.
Overlay/feedback: Tooltip, Dialog, Toast, Overlay, ProgressBar, Spinner, ValidationMessage.
Escape hatch: CustomControl.

### 1.3 The core problem to solve

The catalog is *broad* but the authoring surface is *weakly typed*. A control is
assembled from a heterogeneous `Attr<'msg> list` keyed by strings, where invalid
attribute names, wrong value types, and missing required attributes are caught at
**runtime via diagnostics** (`Catalog.validate`) rather than at **compile time**.
`AttrValue.UntypedValue of obj` is the tell: the type system is not enforcing the
per-control contract.

Your prompt — *"well-defined variable values for each control and immutable rest"*
— is precisely the prescription for this gap: give each control a **typed Props
record** (the well-defined variables) so the F# compiler enforces the contract,
and keep everything else immutable. This is also what Avalonia's `StyledProperty`
system and FuncUI's typed attribute builders achieve in their respective worlds.

### 1.4 Coverage gaps vs a "complete" suite (from the Avalonia checklist)

Missing or thin vs Avalonia's catalog: **date/time** (Calendar, DatePicker,
TimePicker), **ColorPicker**, **AutoCompleteBox**, **MaskedTextBox**,
**NumericUpDown** (distinct from NumericInput spinner), **ToggleButton /
SplitButton / DropDownButton / RepeatButton**, **Expander**, **Carousel**,
**ItemsRepeater/virtualizing ItemsControl**, **TreeDataGrid**, **Flyout/Popup**
as first-class, **GridSplitter**, **Viewbox**, **UniformGrid**, **shapes**
(Rectangle/Ellipse/Path/Polyline as controls), **Window/modal** primitives, and
an **animation/transition** layer. The new suite should target this superset.

---

## 2. Lessons from Avalonia + Avalonia.FuncUI

Avalonia is the right reference: it renders through Skia, has a battle-tested
control architecture, and — crucially — has a mature **F#/Elmish binding
(Avalonia.FuncUI)** that already answers "what do immutable, MVU-driven controls
over a Skia renderer look like."

**Adopt:**

- **Lookless / templated separation.** Split each widget into *behavior + typed
  properties* and a *swappable visual template/theme*. This is Avalonia's single
  most reusable idea and maps to "widget logic = pure F#, appearance = data-driven
  template." Our `Theme` already exists; formalize a per-control template seam.
- **Typed property model with two tiers.** Mirror Avalonia's `StyledProperty`
  (participates in theming/animation/inheritance/precedence) vs `DirectProperty`
  (cheap, single value). Decide *per property* whether it needs the full
  precedence machinery. In F# this becomes: a `Props` record of plain values, with
  a small set of values wrapped to allow theme/inherited resolution.
- **FuncUI's "controls as immutable attribute lists + VDOM diff over a retained
  tree."** FuncUI's view DSL produces an immutable `IView` tree; `VirtualDom.Differ`
  diffs old vs new and `Patcher` mutates the real control tree in place, reusing
  nodes by **type + key**. We already have `Control.Key: ControlId option` — the
  hook for keyed reconciliation. Keep the diff/patch types **internal** (FuncUI
  keeps `IView`/`ViewDelta` internal); expose only the typed widget DSL.
- **Two-pass Measure/Arrange + batched invalidation.** Already provided by Yoga;
  keep the immutable-inputs → layout-result contract.
- **CSS-like selector styling + theme variants (light/dark) + resource lookup.**
  Token-driven theming (see Penpot §6) plugs in here naturally.
- **Elmish wiring like FuncUI:** `Program.mkSimple/mkProgram` → `withHost` →
  `runWithAvaloniaSyncDispatch`. Our `Controls.Elmish.program` is the analog; align
  its shape with real Elmish `Cmd` semantics.

**Avoid:**

- **UserControls as the reuse unit.** Avalonia explicitly scopes UserControls to
  app-specific views, not reusable widgets. Reusable widgets are templated/lookless.
- **Full styled-property precedence on *every* property** — real overhead; use the
  cheap path for hot/read-only props.
- **A XAML dialect.** We're F#-first; prefer code/data-defined templates (FuncUI
  style) over a markup parser + tooling burden.
- **Exposing VDOM internals** on the public surface (Constitution Principle II:
  visibility lives in `.fsi`).

*(Sources: Avalonia docs — Controls library, Choosing a custom control type,
Templated controls, Layout, Styling, Control Themes, Architecture; FuncUI —
GitHub repo, funcui.avaloniaui.net, `VirtualDom.Patcher.fs`. Full URLs in §11.)*

---

## 3. Target architecture — "Elmish controls with well-defined variable values"

### 3.1 The two-axis control model

Every control is the product of two things:

```
Control = (Props : immutable typed record)  ×  (optional MVU : Model × Msg × update)
```

- **Props** = the "well-defined variable values for each control." A closed,
  typed record per control. This is the public authoring surface and the compile-
  time contract. Defaults via a `Props.def` value; modification via record `with`.
- **MVU** = present only for stateful/interactive controls (Constitution
  Principle IV). Stateless controls (TextBlock, Label, Separator, Icon, Border,
  Stack, …) have **no** Model/Msg — they are pure `Props -> view`.
- **The immutable rest** = the lowered view tree (`Control<'msg>`, kept as the
  internal IR) and everything downstream (Scene, Layout) stays immutable exactly
  as today.

### 3.2 Concrete shape (illustrative sketch — not final API)

Stateless control (typed Props, pure view):

```fsharp
// Public .fsi authoring surface (the "well-defined variable values")
type ButtonProps<'msg> =
    { Text: string
      Enabled: bool
      Intent: ButtonIntent           // Primary | Secondary | Danger | ...
      OnClick: 'msg option }

module Button =
    val def : ButtonProps<'msg>                       // immutable defaults
    val view : ButtonProps<'msg> -> Widget<'msg>      // lowers to internal Control<'msg>
```

Authoring stays terse and *typed* (record update, compiler-checked):

```fsharp
Button.view { Button.def with Text = "Submit"; Intent = Primary; OnClick = Some Save }
```

Stateful control (typed Props + per-control MVU — generalizing today's TextInput):

```fsharp
type TextBoxModel = { /* committed/draft text, caret, selection, validation, focus */ }
type TextBoxMsg   = Focus | Blur | InsertText of string | MoveCaret of int | Commit | ...

module TextBox =
    val def    : TextBoxProps<'msg>
    val init   : TextBoxProps<'msg> -> TextBoxModel
    val update : TextBoxMsg -> TextBoxModel -> TextBoxModel * TextBoxEffect list   // pure
    val view   : TextBoxProps<'msg> -> TextBoxModel -> Widget<'msg>
```

This is *exactly* the pattern `TextInput.fs` and `DataGrid.fs` already implement —
the proposal is to make it the **uniform, documented contract** for every stateful
control, and to give every control a typed `Props` record even when stateless.

### 3.3 Why keep `Control<'msg>` as internal IR

`Control<'msg>` (the string-keyed tree) becomes the **lowered intermediate
representation** between typed Props and the renderer. Benefits:

- The renderer, layout, diagnostics, accessibility, and golden-image evidence
  pipelines **do not change** — they consume the same IR.
- Migration is incremental: a typed control simply *lowers to* the existing IR.
- The string keys move from the public surface (where they're unsafe) to an
  internal seam (where they're an implementation detail), satisfying Principle II.

### 3.4 Keyed reconciliation / diffing

Add an internal VDOM-style diff over the lowered IR (FuncUI's model): reuse nodes
by `Kind + Key`, patch changed props, re-bind changed event handlers, run setup
once on create. `Control.Key` already exists. This keeps Elmish re-renders cheap
and is *internal* (not on the `.fsi`).

### 3.5 Elmish integration (promote adapter → boundary)

- Keep `Controls.Elmish.AdapterProgram`, but align its `Update`/`Cmd` story with
  real `Fable.Elmish` `Cmd<'msg>` semantics so consumers get standard Elmish
  ergonomics (commands, subscriptions, effects).
- Provide **per-control message composition** helpers: a parent `update` routes a
  `TextBoxMsg` to `TextBox.update` and lifts effects, the standard "child component
  in Elmish" pattern. Document it once; every stateful control follows it.
- The composite `view : 'model -> Widget<'msg>` replaces today's
  `view : 'model -> Control<'msg>` on the public surface (Widget lowers to Control).

### 3.6 Theming / tokens seam (sets up Penpot §6)

Introduce a **design-token layer** beneath `Theme`: typed token records
(colors → `SKColor`, spacing/sizing/radius/stroke → `float32`, typography →
font records, shadow → blur record). `Theme` is *derived from* tokens. This is the
join point where Penpot's DTCG export lands as generated F#.

---

## 4. Proposed target control catalog (subsumes + extends)

The new suite is a **superset** — every current control keeps its kind string and
behavior (back-compat) but gains a typed Props record + (where stateful) the
standard MVU contract. New controls fill the Avalonia-parity gaps.

**Tier A — primitives & layout (mostly exist):** TextBlock, Label, Icon, Image,
Separator, Border, Panel, Stack, Grid, Dock, Wrap, **UniformGrid (new)**,
**Viewbox (new)**, **GridSplitter (new)**, ScrollViewer, SplitView, shapes
(Rectangle/Ellipse/Line/Path/Polyline — **promote to controls**).

**Tier B — buttons & input (extend):** Button, IconButton, **ToggleButton (new)**,
**RepeatButton (new)**, **SplitButton (new)**, **DropDownButton (new)**, CheckBox,
Switch, RadioGroup, Slider, **RangeSlider (new)**, NumericInput,
**NumericUpDown (new)**, TextBox, TextArea, **MaskedTextBox (new)**,
**AutoCompleteBox (new)**.

**Tier C — collections & data (extend):** ListView, ListBox, MultiSelectList,
ComboBox, **ItemsRepeater / virtualizing ItemsControl (new)**, TreeView,
**TreeDataGrid (new)**, DataGrid, **Carousel (new)**, Tabs, **Expander (new)**.

**Tier D — navigation & overlays (extend):** Menu, ContextMenu, Toolbar,
**Flyout/Popup (promote to first-class)**, Tooltip, Dialog/**Window-modal (new)**,
Toast, Overlay.

**Tier E — feedback & status (exist):** ProgressBar, Spinner, Badge,
ValidationMessage.

**Tier F — date/time (new cluster):** **Calendar**, **DatePicker**, **TimePicker**,
**CalendarDatePicker**.

**Tier G — pickers (new):** **ColorPicker**, (optional) **FilePicker** behind an
I/O effect.

**Tier H — visualization (exist, extend):** LineChart, BarChart, PieChart,
ScatterPlot, GraphView; (stretch) Heatmap, Treemap, Sankey.

**Tier I — motion (new infrastructure):** an **animation/transition layer**
(spring, easing, keyframes) feeding the renderer — needed for modern feel and for
faithful Penpot prototype reproduction.

**Escape hatch (exists):** CustomControl.

Sequencing note: Tiers A–E are mostly "add typed Props + MVU contract over existing
controls"; Tiers F–I are genuinely new and should be separate Spec Kit features.

---

## 5. Penpot — what's real today (and what isn't)

Distinguishing fact from aspiration matters here because the official tooling is
new (Dec 2025) and still moving.

**Solid / exists today:**

- **Penpot** itself: MPL-2.0, SVG/CSS/HTML-native, self-hostable, uses **CSS Flex
  & Grid** as its layout engine (layout intent transfers faithfully).
- **Native DTCG design tokens** (W3C Design Tokens Community Group format): 12
  token types (color, dimension, spacing, sizing, border-radius, stroke-width,
  opacity, rotation, number, typography, typography-composite, shadow), with
  `$value`/`$type`/`$description`, `{alias}` references, sets, multidimensional
  themes, and **JSON/ZIP/multifile import-export**. *This is the strongest,
  most code-ready surface.*
- **Plugin API** (`@penpot/plugin-types`): reads pages → boards → shapes, selection,
  and the **local library** (`library.local.createColor`, `createTypography`).
  Granular permissions (`content:read/write`, `library:read/write`, …).
- **Community MCP servers** with concrete named tools — notably
  **devstroop/penpot-mcp** (`penpot_tokens` exports CSS/JSON/SCSS/Tailwind;
  `penpot_components`, `penpot_exports`, `penpot_analyze`), **montevive/penpot-mcp**
  (Python; `get_file`, `get_object_tree`, `export_object`, render resources), and
  **ancrz/penpot-mcp-server** (self-hosted; `get_shape_css`, `get_design_tokens`).

**Exists but experimental:**

- **Official Penpot MCP server** (announced Penpot Fest 2025; first release
  2026-12-04; standalone repo archived 2026-02-03 and folded into `penpot/penpot`;
  docs at help.penpot.app/mcp). Distinctive architecture: it drives a **Penpot MCP
  plugin running inside the design app**, and the LLM works by **`execute_code`
  against the Plugin API** on the *currently focused page*. Tools:
  `execute_code`, `high_level_overview`, `penpot_api_info`, `export_shape`,
  `import_image`. Remote (`/mcp/stream?userToken=…`) and local (`npx @penpot/mcp`,
  `localhost:4401`) modes. **Still pre-beta.**

**Aspirational / not yet:**

- **No Plugin/MCP design-token API** for sets/themes/aliases (issue #7916, not
  implemented; apply bugs in #9162) → **use DTCG JSON export, not the token
  plugin path**.
- **Shallow component variant/state metadata** via the API (issue #7518) → MCP
  component extraction under-specifies variants; treat as draft.
- **Reliable hands-off design→code** is assistive-draft quality only.

*(Sources in §11.)*

---

## 6. Penpot → Spec Kit integration proposals

The repo's Spec Kit is **extension/hook driven** (`.specify/extensions/{git,evidence}/`,
`extensions.yml`, `before_*`/`after_*` hooks) with a **compiled-F# governance core**
(`build/Governance/**`, `Routing.fs`, generated `validation.contract.yml`) and an
**evidence/audit merge gate**. Penpot integrates as **a new extension + a generated
artifact + capability skills** — *no core skill changes required.* Three proposals,
ordered by confidence/leverage. They are additive, not exclusive.

### Proposal A — Tokens-first (recommended first; highest confidence)

**Idea:** Penpot DTCG token JSON is the source of truth for the design system;
a FAKE/F# code-gen target produces a typed F# token module; drift is governance-
checked like every other generated artifact.

**Flow:**
1. Designer defines tokens (colors/spacing/typography/…) as DTCG sets/themes in
   Penpot; exports DTCG JSON (or an MCP `penpot_tokens` export) into the repo,
   e.g. `design/tokens/*.json`.
2. New FAKE target `GenerateDesignTokens` parses the JSON (existing
   `fsharp-parsing` capability) and emits a typed F# module
   (`src/Controls/Tokens.Generated.fs`): colors → `SKColor`, dimension/spacing/
   sizing/radius/stroke → `float32`, typography-composite → font record, shadow →
   blur record; `{alias}` references resolved at generation time; math operators
   evaluated.
3. `Theme` derives from generated tokens (§3.6).
4. A `DesignTokenDrift` check (mirroring `TargetMetadataDrift`/`SkillSyncCheck`)
   fails CI if the committed generated module is stale vs the JSON — "generated,
   not hand-synced."

**Why first:** standards-based, version-controllable, free of the Plugin-API token
gap, and squarely inside existing `fsharp-parsing` + `fsharp-code-generation`
capabilities. Maps to the catalog work via the token/theme seam (§3.6).

**Routing/governance:** tokens JSON + generated module land under
`src/Controls/**` → already escalates to **focused-authority** (rule
`controls-public-surface`). Add a routing rule for `design/tokens/**`.

### Proposal B — MCP-assisted spec drafting (medium confidence; assistive only)

**Idea:** Use a Penpot MCP to read board/component inventory and **draft** the
UI sections of a `spec.md`, which a human then refines via `speckit-clarify`.

**Flow:**
1. Designer builds boards/components with disciplined, semantic naming.
2. During `speckit-specify`, an optional Penpot step calls MCP
   (`high_level_overview` / `get_object_tree` / `penpot_components`) to enumerate
   screens, components, and Flex/Grid layout intent.
3. Agent drafts spec sections — screen inventory, component list, layout intent —
   marked as **draft** with `[NEEDS CLARIFICATION]` on anything ambiguous
   (variant/state shallow per issue #7518).
4. Human runs `speckit-clarify` → `speckit-plan`.

**Guardrails:** design is **input to**, never generator of, the authoritative spec.
Output is always human-reviewed. This respects the constitution and the "single
home of all rules" principle.

### Proposal C — Design-provenance evidence (medium confidence; closes the loop)

**Idea:** Tie rendered output back to the design as an *optional* evidence
artifact at audit time.

**Flow:**
1. After implementation, an optional `after_implement`/post-audit hook exports the
   relevant Penpot board/shape (`export_shape` / `export_frame_png`) and compares
   it to the framework's deterministic render evidence (we already have golden-
   image/`fs-skia-evidence-mode` infrastructure).
2. Emits `readiness/design-provenance.md` (design-vs-rendered diff). Advisory by
   default; can be promoted to blocking per feature.

**Caveat:** pixel-perfect parity is unrealistic across a custom renderer; treat as
a *structural/coarse* check (sizes, palette, layout), not strict diff.

### Extension wiring (shared by B & C)

```
.specify/extensions/penpot/
  extension.yml          # registers commands + optional hooks
  penpot-config.yml       # MCP endpoint/mode (remote vs npx local), auth ref
  audit-patterns.yml      # optional design-related diff-scan patterns
  commands/
    speckit.penpot.tokens.md      # (Proposal A trigger / manual)
    speckit.penpot.draft-spec.md  # (Proposal B)
    speckit.penpot.provenance.md  # (Proposal C)
```

Hooks (all optional, surfaced per the existing `auto_execute_hooks` convention):
- `before_plan`: optional design-availability/token-freshness check.
- `after_implement`: optional provenance evidence (Proposal C).

New capability skill `fs-skia-penpot-design` (authored in `.agents/skills/`,
generated into `.claude/` via `RefreshSurfaceBaselines` per the SkillSyncCheck
rule) so UI tasks can carry it in `tasks.deps.yml` `skillist`.

**MCP selection:** Start with **read-only** flows. For tokens, prefer **DTCG JSON
export** over any MCP token tool (the token API gap is real). For structure
extraction, the **official server** (`execute_code` + `high_level_overview`) is the
strategic choice but pre-beta; **montevive** (read/analyze) or **devstroop**
(named tools incl. token export) are pragmatic today. Pin the choice in
`penpot-config.yml`; keep it swappable. Note: interactively-authenticated MCP
servers may be unavailable in headless/cron governance runs — keep Penpot steps
**optional** so CI never hard-depends on them.

---

## 7. Governance, routing & constitution implications

- **Principle II (visibility in `.fsi`):** every new typed control needs a curated
  `.fsi`; the typed `Props` records are the public surface, the lowered IR diff is
  internal. Surface-area baselines apply.
- **Principle IV (Elmish/MVU boundary):** stateful controls *must* expose
  `Model`/`Msg`/`Effect`/`init`/`update`(pure)/interpreter — already true for
  TextInput/DataGrid; make it uniform.
- **Principle V (synthetic disclosure):** new controls landing with placeholder
  rendering or stub interpreters must carry `[S]` + the 5-surface disclosure.
- **Routing:** `src/Controls/**` already escalates to focused-authority with
  `ControlsCatalogCheck`/`ControlsInteractionCheck`. Add rules for
  `design/tokens/**` and `.specify/extensions/penpot/**`. The **catalog is
  generated** from the typed source, so `catalog.yml` + `Catalog.fs` must be
  regenerated, not hand-edited — extend the generation story to cover typed Props.
- **Generated-not-hand-synced:** the typed-control refactor must keep
  `validation.contract.yml` (from `Routing.fs`) and the `.claude`/`.agents` skill
  trees in sync via the existing `RefreshSurfaceBaselines` / `TargetMetadataDrift`
  / `SkillSyncCheck` machinery.
- **Packaging:** `FS.Skia.UI.Controls` is a public NuGet contract (v0.1.68). The
  typed surface is **additive** if the old `Control.create`/`Attr` API is retained
  as a thin compatibility layer over the lowered IR. Plan a deprecation window
  rather than a breaking removal; bump versions via `speckit-merge`'s pack-and-bump.

---

## 8. Proposed feature decomposition (Spec Kit features)

This is a *massive* project; it must land as a sequence of independently-shippable
Spec Kit features (each its own `specs/NNN-*` with spec→plan→tasks→implement→
evidence). Proposed ordering (numbers illustrative — allocate at `speckit-specify`
time):

1. **F-α — Typed control core.** Introduce `Props`/`Widget` typed authoring layer +
   lowering to the existing `Control<'msg>` IR; keep old API as compat shim.
   Convert ~5 representative controls (Button, TextBlock, Stack, CheckBox, TextBox)
   as the reference pattern. *Foundational; everything else depends on it.*
2. **F-β — Design-token layer + Penpot Proposal A.** Token records, `GenerateDesignTokens`
   FAKE target, `DesignTokenDrift` check, `Theme`-from-tokens. Tokens-first Penpot
   integration end-to-end.
3. **F-γ — Internal VDOM diff/reconciliation.** Keyed patch over lowered IR for
   cheap Elmish re-render. Internal only.
4. **F-δ — Migrate remaining existing controls to typed Props + uniform MVU
   contract.** Subsumes the current 47. Catalog regenerated from typed source.
5. **F-ε — Catalog expansion: input & buttons + date/time + pickers** (Tiers B, F,
   G new controls).
6. **F-ζ — Catalog expansion: collections/overlays + animation layer** (Tiers C, D,
   I).
7. **F-η — Penpot Proposals B & C.** `penpot` extension, capability skill,
   draft-spec + provenance flows. *Depends on F-β tokens landing first.*

Each feature is a **dogfood/consumer-contract** change (public `.fsi`, template,
governance) → escalates to the serialized maintainer-verify path. Plan for the
full six-target order on each (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck`
→ `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`).

---

## 9. Recommended immediate next steps

1. **Maintainer decision on the core refactor** (typed Props + lowered IR). This is
   the keystone; nothing else should start until it's blessed. (See open questions.)
2. **Run `speckit-specify` for F-α** (typed control core) using §3 as the design
   seed. Let the spec→plan→tasks pipeline produce the authoritative artifacts.
3. **Spike Penpot Proposal A** (token JSON → generated F#) as a small, low-risk
   proof inside F-β's research phase — it validates the whole Penpot direction
   cheaply and delivers immediate value (token-driven theming).
4. **Author `fs-skia-penpot-design` capability skill** (draft) in `.agents/skills/`.
5. **Add routing rules** for `design/tokens/**` and `.specify/extensions/penpot/**`
   to `Routing.fs` (regenerate `validation.contract.yml`).

---

## 10. Risks & open questions

**Open questions (for the maintainer):**

- **Compat vs clean break:** keep the stringly-typed `Control.create`/`Attr` API as
  a permanent compatibility layer, or deprecate-then-remove on a version boundary?
  (Recommendation: keep as compat shim over the IR; deprecate slowly.)
- **Real Elmish `Cmd` vs the bespoke `AdapterCommand`:** converge on `Fable.Elmish`
  `Cmd<'msg>`, or keep the framework's own effect list? (Recommendation: converge,
  for consumer familiarity.)
- **Animation layer scope:** in-scope for the suite, or a separate later initiative?
- **Penpot hosting:** self-hosted Penpot (best automation surface, no Cloudflare,
  direct DB) vs cloud? Affects which MCP server is viable.
- **Official MCP maturity:** wait for the official server to reach beta, or build on
  a community server now and migrate?

**Risks:**

- **Scope.** Seven features is a quarters-long program; resist big-bang. The
  decomposition (§8) is the mitigation.
- **Public-contract churn.** `FS.Skia.UI.Controls` is shipped; typed-surface
  migration risks consumer breakage. Mitigate with the compat shim + surface
  baselines + staged version bumps.
- **Penpot tooling volatility.** Official MCP is pre-beta and just moved repos;
  the token plugin API has gaps/bugs. Mitigate by (a) tokens via DTCG JSON not the
  plugin API, (b) all Penpot steps optional, (c) pinned, swappable MCP config.
- **Generation drift.** More generated artifacts (typed catalog, tokens) = more
  drift surface; lean on existing drift checks and add new ones.

---

## 11. Sources

**Avalonia / FuncUI:**
- Avalonia Controls library — https://docs.avaloniaui.net/docs/reference/controls/
- Choosing a custom control type — https://docs.avaloniaui.net/docs/custom-controls/choosing-a-custom-control-type
- Templated controls — https://docs.avaloniaui.net/docs/custom-controls/templated-controls
- Layout — https://docs.avaloniaui.net/docs/layout/
- Styling / Control Themes — https://docs.avaloniaui.net/docs/basics/user-interface/styling/ ; …/control-themes
- Architecture — https://docs.avaloniaui.net/docs/fundamentals/architecture
- Custom rendering — https://docs.avaloniaui.net/docs/graphics-animation/custom-rendering
- Avalonia.FuncUI — https://github.com/fsprojects/Avalonia.FuncUI ; https://funcui.avaloniaui.net/
- FuncUI VDOM patcher — https://github.com/fsprojects/Avalonia.FuncUI/blob/master/src/Avalonia.FuncUI/VirtualDom/VirtualDom.Patcher.fs
- AvaloniaBook (layout/theming/rendering) — https://wieslawsoltes.github.io/AvaloniaBook/

**Penpot:**
- Penpot repo — https://github.com/penpot/penpot ; Penpot vs Figma — https://penpot.app/penpot-vs-figma
- Official MCP — https://help.penpot.app/mcp/ ; https://penpot.app/ai/mcp-server ; (archived) https://github.com/penpot/penpot-mcp
- Smashing Magazine (MCP, experimental) — https://www.smashingmagazine.com/2026/01/penpot-experimenting-mcp-servers-ai-powered-design-workflows/
- Plugin API — https://help.penpot.app/plugins/getting-started/ ; https://doc.plugins.penpot.app/
- Token/plugin gaps — issues penpot/penpot #7916, #9162, #7518
- Design tokens (DTCG) — https://help.penpot.app/user-guide/design-systems/design-tokens/ ; https://penpot.app/blog/a-practical-guide-to-the-design-tokens-json-format/ ; https://www.w3.org/community/design-tokens/
- Community MCP servers — https://github.com/devstroop/penpot-mcp ; https://github.com/montevive/penpot-mcp ; https://github.com/ancrz/penpot-mcp-server
- Figma Dev Mode MCP (reference pattern) — https://www.figma.com/blog/introducing-figma-mcp-server/
- Penpot+MCP design-to-code pipeline — https://medium.com/@akhila130104/from-pixels-to-production-building-an-ai-driven-design-to-code-pipeline-with-penpot-mcp-b9fd8be70911

**This repo (grounding):**
- `src/Controls/Types.fs`, `Attributes.fs(i)`, `Control.fs`, `Catalog.fs`, `catalog.yml`
- `src/Controls/TextInput.fs`, `DataGrid.fs`, `Collections.fs` (existing per-control MVU)
- `src/Controls.Elmish/ControlsElmish.fsi`, `src/Elmish/Elmish.fsi`
- `Directory.Packages.props` (`Fable.Elmish 4.2.0`, `Yoga.Net`, `SkiaSharp`)
- `.specify/` (extensions, templates, constitution), `build/Governance/**`, `validation.contract.yml`
```
