---
title: Comprehensive Controls Suite and Penpot Spec Kit Integration Plan
---

# Comprehensive Controls Suite and Penpot Spec Kit Integration Plan

**Date:** 2026-06-05 14:29:15 +0200  
**Status:** Analysis and implementation plan. No product code changed.  
**Scope:** A multi-feature program to replace the current weakly typed Controls authoring surface with a comprehensive Elmish-oriented widget suite, and to integrate Penpot into the Spec Kit workflow through tokens, MCP-assisted design inventory, and optional design-provenance evidence.

This report was prepared after reading the active repository plan at `specs/064-publish-nuget-distribution/plan.md`, the current Controls and Elmish contracts under `src/Controls/**`, `src/Controls.Elmish/**`, `src/Elmish/**`, the generated-product guidance, and current primary online sources for Penpot MCP and Avalonia UI.

There is an existing untracked report at `docs/reports/2026-06-05-1421-controls-suite-and-penpot-integration-analysis.md`. This report leaves that file untouched and provides a newer, source-checked companion.

## Executive Summary

The repo already has the right strategic pieces: immutable scene/layout data, a 47-row Controls catalog, per-control MVU examples in `TextInput`, `Collections`, and `DataGrid`, a `Controls.Elmish` adapter, and a centrally pinned `Fable.Elmish 4.2.0` dependency. The gap is not breadth alone. The main gap is that the public Controls authoring surface still depends on string names and heterogeneous `Attr<'msg>` values, so many contract errors are runtime diagnostics instead of compiler errors.

Recommended direction:

- Keep `FS.Skia.UI.Controls` as the pure, dependency-light control contract package. Do not move the Elmish package dependency into base Controls. `Fable.Elmish` is already pinned and is already owned by `FS.Skia.UI.Elmish`, `FS.Skia.UI.Controls.Elmish`, and `SkiaViewer`.
- Add an additive typed widget front door: per-control immutable `Props` records, per-control typed value unions, and optional per-control `Model`/`Msg`/`Effect`/`update` where the control owns ephemeral UI state.
- Keep the existing `Control<'msg>` tree as a lowered internal/intermediate representation during migration. This protects the current renderer, layout, diagnostics, catalog, and event-binding machinery while moving unsafe stringly APIs out of the preferred public path.
- Use Avalonia UI as a reference for concepts, not as a dependency: property types, routed events, pseudo-class state, templated/lookless controls, control themes, and collection virtualization.
- Integrate Penpot tokens first. Penpot's native design tokens use the W3C DTCG format, which is the most stable design-to-code surface. Generate typed F# token/theme modules from committed token JSON and drift-check them like other generated artifacts.
- Treat Penpot MCP as assistive workflow input, not a source of truth. MCP can inspect the active design file and draft spec sections or export provenance images, but authoritative specs, plans, tasks, routing rules, and evidence remain in this repo's Spec Kit/governance flow.

This should be implemented as a sequence of Spec Kit features, not one feature. The first feature should prove the typed front door with a representative slice before expanding the suite.

## Current Repository State

### Controls Surface

`src/Controls/Catalog.fs` currently defines **47 supported controls**. The rows include display primitives, input controls, selection controls, collection/data controls, layout controls, navigation, overlays, feedback, charts, graph view, and custom control support.

Key current files:

| Area | Local source | Current state |
| --- | --- | --- |
| Core type model | `src/Controls/Types.fsi` | `Control<'msg>` stores `Kind: string`, `Attributes: Attr<'msg> list`, `Children`, `Content`, and accessibility metadata. |
| Attribute model | `src/Controls/Attributes.fsi` | `Attr` is keyed by `Name: string`; values include typed cases plus `UntypedValue of obj`. |
| Convenience modules | `src/Controls/Control.fsi` | Module-per-control functions such as `Button.create`, `TextBox.value`, `Tabs.onChanged`. |
| Runtime interaction | `src/Controls/ControlRuntime.fsi` | Focus, hover, pressed, caret, selection, composition, drag, diagnostics. |
| Stateful controls | `TextInput.fsi`, `Collections.fsi`, `DataGrid.fsi` | Already expose pure `init` and `update` functions with `Model`/`Msg`/`Effect`. |
| Elmish adapter | `src/Controls.Elmish/ControlsElmish.fsi` | Adapter program, commands, subscriptions, keyboard/control runtime effect interpretation. |
| General Elmish adapter | `src/Elmish/Elmish.fsi` | Viewer adapter model/message/effects. |
| Catalog evidence | `tests/Controls.Tests/**`, `samples/ControlsGallery/Program.fs` | Contract tests, interaction/rendering tests, gallery smoke. |

The current API is broad and usable, but it is not yet a fully typed widget/control system. Examples:

- `ControlKind`, attribute names, event kinds, and several payloads are strings.
- Invalid attribute/control combinations are mostly reported by diagnostics after construction.
- Several catalog rows exist before a dedicated, strongly typed module exists for each row.
- `AttrValue.UntypedValue of obj` is necessary today for chart/data payloads but is the wrong default for the future public surface.

### Elmish Dependency Status

The prompt suggests adding Elmish as a dependency. The repo already has it:

- `Directory.Packages.props` pins `Fable.Elmish` version `4.2.0`.
- `src/Elmish/Elmish.fsproj` references `Fable.Elmish`.
- `src/Controls.Elmish/Controls.Elmish.fsproj` references `Fable.Elmish`.
- The base `src/Controls/Controls.fsproj` intentionally does **not** reference `Fable.Elmish`.

That split is worth keeping. The base Controls package can expose pure model/update contracts without referencing Elmish. `Controls.Elmish` should own concrete `Cmd<'msg>` and program/subscription ergonomics.

### Routing/Governance Impact

Current routing already escalates `src/Controls/**` under the `controls-public-surface` rule to `focused-authority` with:

- `ControlsCatalogCheck`
- `ControlsInteractionCheck`
- `ControlsRenderingCheck`
- `PackageSurfaceCheck`
- `FsiTranscripts`
- `GeneratedProductCheck`

Public `.fsi` changes route separately through package-surface gates. `.specify/**`, `.agents/skills/**`, and template/guidance paths also have dedicated rules. Any Penpot extension or token-source path needs explicit routing instead of depending on fallback behavior.

## What Avalonia UI Does That Matters Here

Avalonia UI is relevant because it is also a cross-platform, Skia-backed UI framework with a mature control model. The goal is to borrow concepts, not introduce Avalonia as a dependency.

### Property System

Avalonia distinguishes styled, direct, and attached properties. Its docs describe styled properties as values that participate in styling, animations, and value precedence, while direct properties are cheaper and backed by normal fields. This maps cleanly to FS.Skia.UI:

- Use ordinary immutable F# record fields for most control props.
- Introduce a small token/style reference wrapper only where the value participates in theme inheritance, variants, animation, or design-token resolution.
- Do not build a full dependency-property clone for every value.

Avalonia source confirms the split in real controls. `Button` has styled properties for command-like/configuration values, a routed `ClickEvent`, a direct `IsPressedProperty`, and pseudo-classes for visual state. `ToggleButton` adds an `IsChecked` styled property with two-way binding defaults and checked/unchecked/indeterminate pseudo-classes.

### Routed Events and Elmish Messages

Avalonia routed events travel through the element tree and allow composed controls to handle input at a common parent. For FS.Skia.UI, the equivalent should not be CLR events. It should be a typed event envelope that maps to Elmish messages:

- Input event source: pointer, keyboard, text, focus, selection, clipboard, timer.
- Control event identity: control id plus event kind.
- Routed path: control ancestry produced during layout/lowering.
- Handled/continue decision: pure event routing function returns messages plus runtime effects.

The current `Control.dispatch` and `ControlEventBinding<'msg>` are a good seed. The typed suite should make routed event information explicit and typed while still lowering to the existing event-binding path during migration.

### Visual State, Pseudo-Classes, and Styling

Avalonia uses pseudo-classes such as `:pressed`, `:disabled`, `:focus-visible`, `:checked`, and `:error` for conditional styling. FS.Skia.UI already has `VisualState` and runtime focus/hover/pressed model fields. The missing piece is a selector/style layer that can target:

- control kind
- stable key
- style class
- visual state
- validation state
- accessibility state
- variant/intent

Recommendation: define a small typed selector model and avoid a CSS parser initially. Penpot token integration should feed the resolved style values.

### Lookless Controls and Control Themes

Avalonia separates behavior from appearance through templated/lookless controls and control themes. That idea fits this repo well:

- Control logic and typed props live in F# modules.
- Appearance is resolved from a theme/template data model.
- Templates render into the existing immutable `SceneNode`/layout content.
- Product code can override theme values or provide custom templates without rewriting control behavior.

Do not add XAML. A code/data-defined template layer is enough and aligns with the repo's F# governance.

### Collections and Virtualization

Avalonia's docs distinguish `ItemsControl`, `ListBox`, and `ItemsRepeater`: simple repeating data, selectable lists, and virtualized large collections. FS.Skia.UI already has `Collections.visibleRange` and `DataGrid.VisibleRange`. The comprehensive suite should formalize this into:

- `ItemsControl` for small collections and custom layout.
- `ItemsRepeater` for virtualized repeated UI.
- `ListBox`/`MultiSelectList`/`ComboBox` as selection behaviors over collection primitives.
- `DataGrid` and `TreeDataGrid` as data controls with bounded visible ranges.

## Target Architecture

### Core Shape

Every control should have two axes:

```fsharp
Control = typed immutable Props * optional pure MVU runtime
```

Where:

- `Props` are the well-defined variable values for each control.
- `Model`/`Msg`/`Effect` exist only for controls that own ephemeral UI state.
- Product/business state stays in the product's Elmish model.
- The lowered view tree stays immutable.
- I/O stays at the host/interpreter edge.

### Recommended Package Boundaries

| Package | Responsibility |
| --- | --- |
| `FS.Skia.UI.Controls` | Typed props, typed value unions, pure control runtime models, lowering to existing `Control<'msg>` IR, render/layout/diagnostics contract. No direct `Fable.Elmish` dependency. |
| `FS.Skia.UI.Controls.Elmish` | Real Elmish ergonomics: `Cmd<'msg>` conversion, child-message lifting helpers, subscriptions, standard control runtime wiring, adapter programs. Owns `Fable.Elmish`. |
| `FS.Skia.UI.Elmish` | Viewer/app adapter boundary. May stay separate from Controls-specific child control wiring. |
| `FS.Skia.UI.Layout` | Layout engine and measured/arranged bounds. Controls consume it but do not own layout algorithms. |
| `FS.Skia.UI.Scene` | Immutable render vocabulary. Controls lower into it. |

### Public Authoring Model

Add a `Widget<'msg>` or similarly named typed authoring layer, then lower to `Control<'msg>` internally:

```fsharp
type ButtonIntent =
    | Primary
    | Secondary
    | Danger
    | Ghost

type ButtonProps<'msg> =
    { Id: ControlId option
      Text: string
      Enabled: bool
      Intent: ButtonIntent
      Icon: IconName option
      OnClick: 'msg option }

module Button =
    val defaults<'msg> : ButtonProps<'msg>
    val view : ButtonProps<'msg> -> Widget<'msg>
```

For a stateful control:

```fsharp
type TextBoxProps<'msg> =
    { Id: ControlId
      Value: string
      Placeholder: string option
      ReadOnly: bool
      Validation: ValidationState
      OnChanged: (string -> 'msg) option
      OnCommitted: (string -> 'msg) option }

type TextBoxModel =
    { DraftText: string
      CaretIndex: int
      Selection: TextSelection option
      Composition: string option
      Focused: bool }

type TextBoxMsg =
    | Focus
    | Blur
    | InsertText of string
    | MoveCaret of int
    | SelectRange of int * int
    | StartComposition of string
    | CommitComposition of string
    | Commit
    | Cancel

module TextBox =
    val defaults<'msg> : TextBoxProps<'msg>
    val init : TextBoxProps<'msg> -> TextBoxModel
    val update : TextBoxMsg -> TextBoxModel -> TextBoxModel * TextBoxEffect list
    val view : TextBoxProps<'msg> -> TextBoxModel -> Widget<'msg>
```

This generalizes the existing `TextInput`, `DataGrid`, and `Collections` pattern.

### Variable Taxonomy

Each control should declare its values in a consistent taxonomy:

| Value class | Meaning | Example |
| --- | --- | --- |
| Identity | Stable id/key for diffing, event routing, focus, evidence | `Id: ControlId option` |
| Content | Text, icon, child widgets, item template | `Text`, `Icon`, `Child`, `Children` |
| Data | Product-owned values and item sources | `Items`, `Rows`, `Columns`, `SelectedKey` |
| Behavior | Control behavior not owned by theme | `ReadOnly`, `IsThreeState`, `SelectionMode` |
| Variant | Semantic style intent | `Primary`, `Danger`, `Compact`, `Outline` |
| Layout | Sizing/alignment constraints | `Width`, `Height`, `MinWidth`, `Padding` |
| Theme/style | Token references and optional local overrides | `StyleClass`, `ThemeKey`, `ForegroundToken` |
| Accessibility | Role, name, help text, keyboard behavior | `AccessibleName`, `Role` |
| Events | Elmish message callbacks | `OnClick`, `OnChanged`, `OnSelected` |

Every required value belongs in the record type. Optional values get defaults. Avoid optional string event names and object payloads in the preferred API.

### Internal Lowering

Keep the existing `Control<'msg>` as the migration IR:

```fsharp
Widget<'msg> -> Control<'msg> -> ControlRenderResult<'msg> -> Scene/Layout
```

Benefits:

- Current rendering tests and deterministic evidence continue to apply.
- Existing galleries can be migrated one control at a time.
- Compatibility shims can call typed builders internally.
- Public `.fsi` growth is additive at first.

Long term, `Control<'msg>` can remain as an extension API or become internal-only after a deprecation window.

### Diff/Reconciliation

Add an internal keyed diff in a later feature:

- Reuse nodes by `Kind + Id`.
- Re-bind event callbacks when product model changes.
- Patch only changed props/templates.
- Preserve control runtime state where key and control type match.
- Drop state when key disappears or type changes.

This is especially important once views become deeply Elmish-driven and re-render frequently.

## Comprehensive Suite Scope

The new suite should subsume all 47 current catalog rows and expand toward Avalonia-parity where it makes sense for a Skia-rendered F# toolkit.

### Phase 1: Type Existing Foundation

Convert these first as the reference slice:

- `TextBlock`
- `Button`
- `CheckBox`
- `TextBox`
- `Stack`
- `DataGrid`

This slice exercises content, command, boolean state, text input runtime, layout composition, data/control runtime, and catalog generation.

### Phase 2: Migrate Current Catalog

Typed props and, where applicable, MVU contracts for:

- Display: `TextBlock`, `RichText`, `Label`, `Image`, `Icon`, `Separator`, `Badge`.
- Inputs: `Button`, `IconButton`, `TextBox`, `TextArea`, `NumericInput`, `Slider`.
- Selection: `CheckBox`, `Switch`, `RadioGroup`.
- Collections/data: `ListView`, `ListBox`, `MultiSelectList`, `ComboBox`, `TreeView`, `DataGrid`.
- Layout: `Stack`, `Grid`, `Dock`, `Wrap`, `Border`, `Panel`, `ScrollViewer`, `SplitView`.
- Navigation: `Tabs`, `Menu`, `ContextMenu`, `Toolbar`.
- Overlays/feedback: `Tooltip`, `Dialog`, `Toast`, `Overlay`, `ProgressBar`, `Spinner`, `ValidationMessage`.
- Visualization: `LineChart`, `BarChart`, `PieChart`, `ScatterPlot`, `GraphView`.
- Escape hatch: `CustomControl`.

### Phase 3: Add High-Value Missing Controls

Priority additions:

| Cluster | Controls | Reason |
| --- | --- | --- |
| Button variants | `ToggleButton`, `RepeatButton`, `DropDownButton`, `SplitButton` | Common app/tool surfaces. Mirrors Avalonia button hierarchy concepts. |
| Pickers | `Calendar`, `DatePicker`, `TimePicker`, `ColorPicker` | Major gap for real tools and settings UI. |
| Text/value input | `MaskedTextBox`, `AutoCompleteBox`, `NumericUpDown`, `RangeSlider` | Common form completeness gap. |
| Collections | `ItemsControl`, `ItemsRepeater`, `TreeDataGrid`, `Carousel` | Needed for scalable product UIs and virtualization. |
| Layout | `UniformGrid`, `Viewbox`, `GridSplitter` | Common shell/dashboard/layout needs. |
| Disclosure/overlay | `Expander`, `Flyout`, `Popup` | Needed for menus, inspectors, and dense tools. |
| Shapes as controls | `Rectangle`, `Ellipse`, `Line`, `Path`, `Polyline` | Useful design-system primitives over the scene vocabulary. |
| Motion | `Transition`, `Animation`, `Easing`, `Spring` | Optional but important for modern controls and Penpot prototype parity. |

## Penpot Integration

### What Is Stable Enough To Use

Primary source findings:

- Penpot MCP connects an MCP-aware agent to the currently focused Penpot file/page. The official docs emphasize that agents can read and write file structure, components, styles, tokens, pages, and layers.
- The official MCP flow has remote and local modes. Remote uses a Penpot-provided URL with a `userToken`; local uses `npx @penpot/mcp@stable` and the local server/plugin flow.
- Penpot MCP tools listed in current help docs include `execute_code`, `high_level_overview`, `penpot_api_info`, `export_shape`, and `import_image`. Remote mode limits local filesystem access.
- Penpot warns that connected agents can change the current design file and recommends starting with read-only actions, describing intended writes, and making small reversible changes.
- Penpot design tokens use the W3C DTCG format and are intended as a single source of truth across tools. The docs also state plugin access to the tokens API is coming, which means token JSON export is safer than depending on a live plugin token API today.
- The Penpot plugin API can inspect current file/page/selection, generate markup/style, create shapes/boards/text, and work with libraries.

### Proposal A: Tokens-First Integration

This is the recommended first Penpot feature.

Add committed design-token inputs:

```text
design/
  penpot/
    tokens/
      fs-skia-light.tokens.json
      fs-skia-dark.tokens.json
      controls.tokens.json
```

Add generated outputs:

```text
src/Controls/Tokens.Generated.fs
src/Controls/Tokens.Generated.fsi
src/Controls/Theme.Generated.fs
readiness/design-token-drift.md
```

Flow:

1. Export Penpot DTCG token JSON into `design/penpot/tokens/**`.
2. Parse tokens in compiled F# using the repo's existing parsing/code-generation patterns.
3. Resolve aliases and theme sets at generation time.
4. Generate typed token records and theme constructors.
5. Add a `DesignTokenDrift` target that fails if generated token modules are stale.
6. Make Controls theme values derive from generated tokens.

This gives immediate value and does not require live Penpot access during CI.

### Proposal B: MCP-Assisted Spec Drafting

Use Penpot MCP as an optional input to `speckit-specify` or `speckit-clarify`.

Flow:

1. Designer opens the relevant Penpot file and connects MCP.
2. Agent performs read-only inspection of the active page: pages, boards, components, naming, layout structure, style/token usage, exported shape thumbnails if needed.
3. Agent writes a draft `spec.md` screen/component inventory with explicit `[NEEDS CLARIFICATION]` markers for ambiguous variants, states, interactions, or data ownership.
4. Maintainer runs normal `speckit-clarify`, `speckit-plan`, and `speckit-tasks`.

Guardrail: Penpot output is not authoritative. It informs the spec, then the repo's spec/plan/tasks/governance artifacts take over.

### Proposal C: Design Provenance Evidence

Use Penpot MCP after implementation to produce optional design-to-render evidence.

Flow:

1. Export a Penpot board/shape image or structural summary.
2. Capture FS.Skia.UI deterministic render evidence for the implemented view.
3. Produce `specs/<feature>/readiness/design-provenance.md`.
4. Compare structure, palette/token usage, and approximate layout. Avoid strict pixel-perfect diff by default.

This evidence should be advisory at first and promoted to blocking only for features that explicitly commit to design conformance.

### Proposal D: Code-to-Design Catalog Sync

Once typed props and token generation exist, add a reverse flow:

1. Generate a Penpot "FS.Skia.UI Controls Catalog" page from the typed control registry.
2. Include component boards for controls, variants, states, and token names.
3. Use MCP write operations only on a dedicated catalog/design-system file, never on arbitrary product pages.

This closes the loop between code catalog and design system. It should land after Proposal A and the typed-control registry exist.

### Proposed Spec Kit Extension Shape

```text
.specify/extensions/penpot/
  extension.yml
  penpot-config.example.yml
  commands/
    speckit.penpot.tokens.md
    speckit.penpot.inspect.md
    speckit.penpot.draft-spec.md
    speckit.penpot.provenance.md
```

`extension.yml` should expose optional commands. Hooks should be opt-in:

- `before_plan`: optional token freshness or design inventory check.
- `after_implement`: optional design-provenance evidence.

Do not make live MCP access mandatory for headless CI. Token drift can be mandatory because it only needs committed JSON.

### Penpot Security and Reliability Rules

- Never commit MCP keys or `userToken` URLs.
- Do not log full remote MCP URLs.
- Prefer local/token-file workflows in governance.
- Start MCP actions read-only.
- Require a written change summary before any MCP write operation.
- Scope write operations to a dedicated design-system file unless a feature explicitly authorizes product-design edits.
- Remember that Penpot MCP acts on the active focused page/tab. A workflow must record file/page identifiers in readiness evidence.

## Governance Plan

### New Routing Rules

Add routing rules for:

| Path | Suggested tier | Gates |
| --- | --- | --- |
| `design/penpot/tokens/**` | `FocusedAuthority` | `DesignTokenDrift`, `ControlsRenderingCheck`, `GeneratedProductCheck`, `PackageSurfaceCheck` when generated `.fsi` changes. |
| `.specify/extensions/penpot/**` | `FocusedAuthority` | `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph` if hooks/tasks are affected. |
| `.agents/skills/fs-skia-penpot-design/**` | `FocusedAuthority` | existing `SkillQualityCheck`, `SkillSyncCheck`, `SkillContractPathCheck`. |
| `src/Controls/Tokens.Generated.*` | `FocusedAuthority` | existing `controls-public-surface` plus token drift. |

If this work touches `build/Governance/**` or new target metadata, Route will likely escalate to maintainer verification.

### New Targets

Suggested targets:

- `GenerateDesignTokens`: generate F# token/theme modules from committed Penpot DTCG JSON.
- `DesignTokenDrift`: compare generated outputs against token inputs.
- `TypedControlsCatalogCheck`: verify the typed registry, generated catalog, `.fsi` surface, and catalog YAML are in sync.
- `ControlsDiffCheck`: later, for internal reconciliation invariants.

Target identity should live in `build/Governance/Targets.fs`, with rules in `Routing.fs`, and generated contract currency through existing target metadata drift checks.

### Evidence Artifacts

For the first typed-control feature:

- `readiness/typed-controls-front-door.md`
- `readiness/controls-compat-shim.md`
- `readiness/package-surface-expectations.md`
- `readiness/controls-rendering.md`
- `readiness/generated-product-controls.md`

For the Penpot token feature:

- `readiness/design-token-source.md`
- `readiness/design-token-drift.md`
- `readiness/theme-token-rendering.md`
- `readiness/generated-token-api.md`

For MCP-assisted flows:

- `readiness/penpot-inspection.md`
- `readiness/penpot-draft-spec-input.md`
- `readiness/design-provenance.md`

## Test Plan

### Typed Controls

- Failing-first tests proving invalid props are impossible or rejected at construction.
- Surface tests for every new `.fsi`.
- Catalog generation tests: typed registry -> `Catalog.fs`/`catalog.yml` rows.
- Compatibility tests: old `Button.create [ Button.text ... ]` and new `Button.view { ... }` lower to equivalent control IR.
- Interaction tests: typed events dispatch current model values after rerender.
- Accessibility tests: each control has role, name source, keyboard behavior, contrast evidence.
- Rendering tests at multiple viewports and densities.
- Property tests for normalization and value constraints where applicable.

### Elmish Integration

- Child control update helpers map child messages and effects into parent messages.
- `Controls.Elmish` can convert control effects into `Cmd<'msg>` without leaking I/O into pure updates.
- Subscription disposal/cancellation is deterministic.
- No direct `Fable.Elmish` dependency is introduced into base Controls.

### Penpot Tokens

- DTCG parser handles aliases, grouped token names, type-specific validation, and theme sets.
- Generated F# is byte-stable.
- Drift target fails on stale generated modules.
- Token-derived themes render representative controls without contrast/accessibility regressions.

### Penpot MCP

- Read-only inspection command records file/page identity and active MCP mode.
- Draft-spec command marks ambiguous design-derived requirements with clarification markers.
- Provenance evidence can run in "unavailable" mode without failing unrelated headless CI.
- Write-capable commands require an explicit operator confirmation path in docs and are not auto-hooks.

## Feature Decomposition

Use separate Spec Kit features. Suggested sequence:

1. **Typed controls front door**: Add `Widget<'msg>`, typed props records, lowering, and representative controls (`TextBlock`, `Button`, `CheckBox`, `TextBox`, `Stack`, `DataGrid`). Keep compatibility shims.
2. **Typed catalog generation**: Move control catalog data to a typed source and generate current catalog artifacts.
3. **Controls.Elmish command model**: Align adapter commands with `Fable.Elmish` `Cmd<'msg>` while preserving current adapter API during migration.
4. **Penpot tokens and theme generation**: Add token JSON path, generator, drift target, and token-derived themes.
5. **Existing catalog migration**: Convert all 47 current controls to typed props/MVU contracts.
6. **Internal reconciliation**: Add keyed diff/patch over lowered controls.
7. **Catalog expansion 1**: Button variants, text/value inputs, pickers, date/time.
8. **Catalog expansion 2**: ItemsRepeater, TreeDataGrid, virtualization, overlays, layout additions.
9. **Motion/animation layer**: transitions and easing, with evidence gates.
10. **Penpot MCP extension**: Optional inspect/draft/provenance commands and `fs-skia-penpot-design` skill.
11. **Code-to-design catalog sync**: Generate/update a Penpot controls catalog from the typed registry.

This order minimizes blast radius: type the authoring layer first, wire tokens second, migrate breadth later.

## Immediate Next Steps

1. Create a Spec Kit feature for **Typed controls front door**.
2. In its plan, explicitly choose compatibility behavior for old `Control.create`/`Attr` APIs.
3. Add a tiny typed registry for the first six controls and prove lowering equivalence.
4. Keep `Fable.Elmish` in adapter packages. Do not add it to base Controls unless a future plan intentionally changes dependency ownership.
5. Create a small sample Penpot DTCG token JSON fixture for the tokens feature research phase.
6. Draft `fs-skia-penpot-design` as a capability skill only after the extension shape is accepted.
7. Run `Route` for each feature and only run the printed gates.

## Open Questions

- Should the new preferred public type be called `Widget<'msg>`, `ControlView<'msg>`, or should `Control<'msg>` itself become the typed tree?
- Should old stringly `Attr` APIs be permanent extension APIs or deprecated over a preview-cycle window?
- Should `Controls.Elmish` converge fully on `Cmd<'msg>` or keep `AdapterCommand<'msg>` as a stable compatibility abstraction with conversions?
- Should design-token generation support only committed JSON first, or also invoke Penpot MCP token export manually?
- Should Penpot provenance ever become blocking, or remain advisory unless a feature opts in?
- How much animation is part of "comprehensive controls" versus a separate renderer feature?

## Risks

| Risk | Impact | Mitigation |
| --- | --- | --- |
| Scope explosion | The suite becomes too large to finish. | Land as independent Spec Kit features with representative slices. |
| Public API churn | Consumers of `FS.Skia.UI.Controls` break. | Add typed front door first, keep old API shims, deprecate slowly. |
| Dependency drift | Elmish dependency leaks into base Controls. | Keep `Fable.Elmish` in `Controls.Elmish`; assert with dependency governance tests. |
| Generated artifact drift | Token/catalog/generated docs get hand-edited. | Add generation and drift targets, route token paths explicitly. |
| Penpot MCP volatility | Live design integration changes or is unavailable. | Tokens via committed DTCG JSON first; MCP commands optional and read-only by default. |
| Pixel parity expectations | Design provenance becomes false-negative heavy. | Start with structural/token/layout evidence, not strict pixel diffs. |

## Sources Checked

Avalonia UI:

- Defining properties: https://docs.avaloniaui.net/docs/custom-controls/defining-properties
- Defining events: https://docs.avaloniaui.net/docs/custom-controls/defining-events
- Routed events: https://docs.avaloniaui.net/docs/input-interaction/routed-events
- Styling controls and pseudo-classes: https://docs.avaloniaui.net/docs/how-to/styling-controls-how-to
- Control themes: https://docs.avaloniaui.net/docs/styling/control-themes
- ItemsControl reference: https://docs.avaloniaui.net/controls/data-display/collections/itemscontrol
- ItemsControl and ItemsRepeater guide: https://docs.avaloniaui.net/docs/how-to/itemscontrol-how-to
- Button source: https://raw.githubusercontent.com/AvaloniaUI/Avalonia/master/src/Avalonia.Controls/Button.cs
- ToggleButton source: https://raw.githubusercontent.com/AvaloniaUI/Avalonia/master/src/Avalonia.Controls/Primitives/ToggleButton.cs
- ItemsControl source: https://raw.githubusercontent.com/AvaloniaUI/Avalonia/master/src/Avalonia.Controls/ItemsControl.cs

Penpot:

- Penpot MCP help: https://help.penpot.app/mcp/
- Penpot MCP product page: https://penpot.app/ai/mcp-server
- Penpot MCP in the main repo: https://github.com/penpot/penpot/tree/develop/mcp
- Penpot design tokens: https://help.penpot.app/user-guide/design-systems/design-tokens/
- Penpot Plugin API: https://doc.plugins.penpot.app/interfaces/Penpot
- Penpot developer UI guide and token levels: https://help.penpot.app/technical-guide/developer/ui/

Local repository:

- `specs/064-publish-nuget-distribution/plan.md`
- `src/Controls/Types.fsi`
- `src/Controls/Control.fsi`
- `src/Controls/Attributes.fsi`
- `src/Controls/Catalog.fs`
- `src/Controls/ControlRuntime.fsi`
- `src/Controls/TextInput.fsi`
- `src/Controls/Collections.fsi`
- `src/Controls/DataGrid.fsi`
- `src/Controls.Elmish/ControlsElmish.fsi`
- `src/Elmish/Elmish.fsi`
- `Directory.Packages.props`
- `build/Governance/Routing.fs`
- `template/product-skills/fs-skia-ui-widgets/SKILL.md`
