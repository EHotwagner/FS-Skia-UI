---
title: Ant Design UI Story Adoption Analysis
category: Reports
categoryindex: 8
---

# Ant Design UI Story Adoption Analysis

- Date: 2026-06-09
- Status: Research report. No product code changed.
- Scope: How FS.Skia.UI can make heavy use of Ant Design's design principles,
  visual choices, token model, enterprise patterns, and agent-facing guidance
  without taking a React or DOM dependency. Updated decision: Ant Design's
  color/contrast choices should be selectable as a design-system policy that can
  replace the current WCAG-only gate for generated-template validation.
- Primary external source: <https://ant.design/docs/spec/introduce/?theme=light>
- Ant Design docs version observed during research: 6.4.3.

## Executive Summary

Ant Design should be adopted here as a design language, token taxonomy, pattern
library, and quality bar. It should not be adopted as a direct React component
dependency. FS.Skia.UI renders through Skia and F# control IR, so the practical
path is to translate Ant's stable ideas into local primitives:

- A richer semantic token layer generated from the existing DTCG source, with
  Ant-style seed, map, alias, and component-token groups.
- A design-system color/contrast policy selector. The current WCAG gate becomes
  the `wcag` policy; Ant Design becomes the first alternative policy; later
  policies can add Material, Fluent/Fluid, or other systems without changing the
  template UX.
- A central visual-state style resolver that maps `ControlKind`, `VisualState`,
  intent, validation state, and theme into concrete draw styles.
- A controls story that emphasizes enterprise workflows: list pages, dense
  data grids, forms, validation, inline editing, feedback, overlays, navigation,
  dashboards, empty states, and evidence captures.
- Local agent guidance under `.agents/skills/fs-skia-ant-design/SKILL.md`,
  mirrored to `.claude` by the existing generated-skill machinery, so Codex and
  Claude follow the same Ant-inspired rules.

The revised design decision is that the repo should no longer hard-code one
universal color/contrast authority. Instead, validation should be policy-driven:
`wcag` preserves today's explicit ratio gate, `ant` validates Ant Design's own
semantic color and contrast choices, and future policies such as `material` or
`fluent` can plug into the same mechanism. Ant's default brand blue is
`#1677ff`, but the point is broader than one color: selecting the Ant policy
should select the Ant token palette, neutral transparency model, functional
colors, text/background contrast expectations, and restrained enterprise color
usage.

## Design Decision: Policy-Selected Color And Contrast

The design-system policy should become a template parameter and a governance
input, not a fixed assumption baked into `ContrastCheck`.

Proposed template surface:

```text
dotnet new fs-skia-ui --design-system wcag
dotnet new fs-skia-ui --design-system ant
```

`wcag` should be the compatibility default at first because it preserves current
generated-product behavior. `ant` should be the first richer design-system
alternative. Future values can include `material`, `fluent`, or `fluid` depending
on the naming decision for the next design language.

The conceptual gate should become `DesignSystemColorCheck` or
`ColorPolicyCheck`. It may keep the existing `ContrastCheck` target name for
backward compatibility, but internally it should delegate to a selected policy:

```fsharp
type DesignSystemPolicy =
    | Wcag
    | Ant
    | Material
    | Fluent
    | Custom of string

type ColorPolicy =
    { Id: DesignSystemPolicy
      TokenSeed: string
      Pairings: ValidatedPairing list
      ThresholdFor: Role -> float
      ReportLabel: string }
```

The policy controls both token selection and validation semantics. The template
parameter should not merely switch colors while leaving the WCAG-only gate in
place; that would make Ant a paint preset rather than a design-system choice.
For Ant, the policy should encode Ant Design's choices: `colorPrimary #1677ff`,
functional color families such as success, warning, error, and info, neutral text
and surface roles, default typography of 14/22, body/title contrast expectations,
and semantic pairings for text, controls, feedback, selection, and navigation.

This gives follow-up work a clean extension seam:

| Template value | Policy source | Gate behavior |
|----------------|---------------|---------------|
| `wcag` | Current local WCAG pairings and thresholds. | Preserves today's `ContrastCheck` semantics. |
| `ant` | Ant Design visual/token/pattern docs. | Replaces WCAG-only validation with Ant semantic color/contrast validation. |
| `material` | Future Material policy. | Adds Material tokens, pairings, and contrast rules through the same interface. |
| `fluent` / `fluid` | Future Fluent/Fluid policy. | Adds that design language without changing templates or generated-product contracts. |

This also changes the report's earlier recommendation: Ant colors should not just
be "certified by the local WCAG gate." For the Ant option, Ant's policy is the
selected authority. A WCAG-derived measurement can remain useful implementation
machinery, but the user-facing contract is "this generated product follows the
Ant Design policy."

## Research Basis

The relevant Ant Design source material breaks into five groups.

| Area | Official source | Why it matters here |
|------|-----------------|---------------------|
| Design purpose | <https://ant.design/docs/spec/introduce/?theme=light> | Ant frames itself as a system for enterprise products, reusable components, reusable pages, and faster design/development collaboration. |
| Design values | <https://ant.design/docs/spec/values/> | The four values are Natural, Certain, Meaningful, and Growing. These are useful review criteria for generated apps and control docs. |
| Visual system | <https://ant.design/docs/spec/colors/>, <https://ant.design/docs/spec/font/>, <https://ant.design/docs/spec/layout/>, <https://ant.design/docs/spec/dark/>, <https://ant.design/docs/spec/shadow/>, <https://ant.design/docs/spec/icon/>, <https://ant.design/docs/spec/motion/> | These define the color model, typography, layout grid, dark mode behavior, elevation, icon consistency, and restrained motion principles. |
| Interaction principles | <https://ant.design/docs/spec/direct/>, <https://ant.design/docs/spec/stay/>, <https://ant.design/docs/spec/lightweight/>, <https://ant.design/docs/spec/invitation/>, <https://ant.design/docs/spec/transition/>, <https://ant.design/docs/spec/reaction/> | These map directly to pointer, keyboard, hover, focus, inline edit, overlay, loading, and validation behavior in FS.Skia.UI.Controls. |
| Enterprise patterns | <https://ant.design/docs/spec/overview/>, <https://ant.design/docs/spec/data-entry/>, <https://ant.design/docs/spec/data-display/>, <https://ant.design/docs/spec/feedback/>, <https://ant.design/docs/spec/navigation/>, <https://ant.design/docs/spec/buttons/>, <https://ant.design/docs/spec/research-form/>, <https://ant.design/docs/spec/research-list/> | These are the best raw material for the FS.Skia.UI showcase, templates, and generated-product story. |

Development-side sources are also useful, even though this project is not a
React renderer:

| Area | Official source | Useful takeaway |
|------|-----------------|-----------------|
| Theme customization | <https://ant.design/docs/react/customize-theme/> | Ant v5/v6 structures tokens as Seed Token -> Map Token -> Alias Token and supports component tokens. This is the strongest model for local DTCG expansion. |
| CSS variables | <https://ant.design/docs/react/css-variables/> | Ant treats token values as late-bound variables to reduce theme-switching cost. Locally, the equivalent is generated typed tokens plus a runtime `Theme`/style resolver. |
| CLI | <https://ant.design/docs/react/cli/> | `@ant-design/cli` exposes offline component, token, demo, semantic, and changelog metadata. Useful for follow-up research and agent workflows. |
| MCP | <https://ant.design/docs/react/mcp/> | The official `antd mcp` server can expose component docs, demos, tokens, semantic structure, and changelog data to agents. |
| LLM files | <https://ant.design/docs/react/llms/>, <https://ant.design/llms.txt>, <https://ant.design/llms-full.txt>, <https://ant.design/llms-semantic.md> | Useful for offline or semi-offline agent research, especially when drafting a local skill. |

## What Ant Design Is Really Offering

Ant's value is not just a palette or a catalog of widgets. The durable ideas are:

| Ant idea | Interpretation for FS.Skia.UI |
|----------|-------------------------------|
| Enterprise product focus | Prefer dense, clear, task-oriented app surfaces over marketing-style pages. The showcase should feel like a real operational tool. |
| Natural | Users should see cause and effect: hover states, pressed states, inline edit affordances, immediate validation, contextual controls. |
| Certain | Components, tokens, layout, interaction, docs, and generated examples should be predictable and reusable. Local generated-token and generated-skill machinery already supports this. |
| Meaningful | UI elements should exist because they help the user complete a work mission. This argues against decorative chrome and random color variety. |
| Growing | The system should help users discover capability over time through invitations, progressive disclosure, guided empty states, and consistent patterns. |
| Restraint | Use color for information delivery, operational guidance, and feedback. Avoid palettes that dominate the whole interface with one hue. |
| Pattern reuse | Page-level recipes matter as much as individual controls: form page, list page, detail page, workbench, result page, exception page. |

The strongest match for this repository is an "Ant-inspired Skia enterprise UI
kit": locally rendered, locally tokenized, and locally governed, but borrowing
Ant's vocabulary and decision rules.

## Local Machinery Today

The repository already has several pieces that make this feasible.

| Local surface | Current role | Ant Design adoption implication |
|---------------|--------------|---------------------------------|
| `src/Controls/design-tokens.tokens.json` | DTCG single source for the shipped light/dark theme primitives. | This is the right root for Ant-inspired token expansion. |
| `src/Controls/DesignTokens.fs` | Generated typed F# token module. | New semantic tokens should be generated, not hand-coded. |
| `src/Controls/Theme.fs` | Builds `Theme.light` and `Theme.dark` from generated tokens. | Keep the existing public `Theme` stable while adding richer internal/adaptive style resolution. |
| `src/Controls/Types.fsi` | Public `Theme`, `VisualState`, diagnostics, accessibility, and control contracts. | Adding fields to public records is a contract change; avoid doing that until a feature explicitly routes and validates it. |
| `src/Color/Contrast.fsi` | WCAG contrast arithmetic and verdicts. | Keep this as the `wcag` policy engine and optional ratio diagnostics for other policies, not the only possible authority. |
| `src/Color/Palettes.fsi` | Radix-derived, role-labelled light/dark ramps. | Use these as accessible raw material for `wcag`; Ant should prefer Ant seed, functional, and neutral tokens. |
| `build/Governance/ContrastGate.fs` | Explicit validated pairings for foreground/background roles. | Refactor this conceptually into policy-backed color validation: `wcag` uses current pairings, `ant` uses Ant pairings, later policies add Material/Fluent equivalents. |
| `docs/testSpecs/Showcase/*.md` | Controls-gallery user story, palette, state mapping, and evidence requirements. | This is the best near-term home for Ant-inspired UI-story changes. |
| `.agents/skills/**` and `.claude/skills/**` | Canonical skills plus generated mirror. | A new Ant skill should be authored in `.agents`, then synced/generated into `.claude`; do not hand-maintain both. |
| `template/**` | `dotnet new fs-skia-ui` consumer defaults. | Add a design-system parameter such as `--design-system wcag|ant`; default to `wcag` for compatibility and route template changes through the escalated gates. |

The current design-token set is intentionally small: foreground, background,
accent, danger, muted, font family, font size, density, corner radius, and
contrast ratio for light and dark. Ant's theme model is much broader. The
adoption path should therefore start with an additive semantic layer rather than
immediately changing the public `Theme` record.

## Target Design Story

The UI story should become:

> FS.Skia.UI.Controls provides a deterministic, Skia-rendered, F#-native
> enterprise interface system. It uses typed controls, generated DTCG design
> tokens, policy-selected color/contrast validation, and Ant-inspired interaction patterns to produce
> clear, dense, consistent operational UIs.

That story can be expressed in four user-visible ways:

| Story layer | Concrete expression |
|-------------|---------------------|
| Showcase | A persistent shell, side navigation, top toolbar, content pages, feedback strip, theme/density controls, and Ant-style list/form/table/feedback examples. |
| Docs | Short design rules for each control family: when to use it, how states render, contrast obligations, layout density, and keyboard/pointer expectations. |
| Templates | Generated apps that look like enterprise tools from first launch: workbench, CRUD list, detail page, form wizard, dashboard, result/exception states. |
| Agent skills | Local guidance that tells agents how to translate Ant docs into this repository's token, control, renderer, and gate machinery. |

## Visual System Adoption

### 1. Token Taxonomy

Ant's `customize-theme` docs describe a three-layer token lifecycle:

- Seed tokens: design intent roots, such as primary color, base background,
  base text, font size, border radius, and control height.
- Map tokens: derived gradients and state-specific values, such as primary
  hover, primary active, error background, container background, border, fill,
  and text variants.
- Alias tokens: common semantic aliases used across components, such as link
  color, elevated background, split border, selected item background, and text
  secondary.

FS.Skia.UI should mirror that structure in DTCG, but adapt the names to F# and
Skia rendering. A plausible local taxonomy:

| Token group | Examples | Notes |
|-------------|----------|-------|
| `seed` | `colorPrimary`, `colorSuccess`, `colorWarning`, `colorError`, `colorInfo`, `colorTextBase`, `colorBgBase`, `fontSize`, `lineHeight`, `borderRadius`, `controlHeight`, `sizeUnit`, `sizeStep`, `motionUnit` | Stable inputs. Ant default values are useful references, not automatic choices. |
| `map.light` / `map.dark` | `colorPrimaryBg`, `colorPrimaryHover`, `colorPrimaryActive`, `colorErrorBg`, `colorWarningBg`, `colorSuccessText`, `colorBorder`, `colorFillSecondary`, `colorBgContainer`, `colorBgElevated`, `colorBgLayout`, `colorText`, `colorTextSecondary`, `colorTextDisabled` | Derived values. Local generation can initially be explicit DTCG aliases/values before adding algorithms. |
| `alias.light` / `alias.dark` | `text.default`, `text.secondary`, `surface.canvas`, `surface.container`, `surface.elevated`, `border.default`, `item.hoverBg`, `item.selectedBg`, `focus.ring`, `feedback.errorText`, `feedback.warningText` | Friendly local names for renderer code. |
| `component.<control>` | `button.primaryBg`, `button.defaultBorder`, `input.activeBorder`, `table.headerBg`, `table.rowHoverBg`, `tabs.itemSelectedColor`, `menu.itemSelectedBg`, `alert.infoBg` | Needed once individual controls stop hardcoding generic theme roles. |

Important migration rule: adding public token names under `DesignTokens.fsi` is a
contract change and must route through the repo's public-surface gates. Until
then, an internal generated module or an additive namespace can absorb the
experiment.

### 2. Color Choices

Ant's color docs define a system-level palette and product-level semantics:
brand color, functional colors, neutral text/background/border/separator roles,
and restrained enterprise usage. Ant's default brand color is `#1677ff`.
Functional defaults include success green, warning gold, and error red families.

Policy-specific recommendation:

| Role | Ant-inspired intent | Local implementation rule |
|------|---------------------|---------------------------|
| Primary/accent | Key action point, operation state, important highlight, graphic emphasis. | For `wcag`, choose a local step that passes the current pairings. For `ant`, prefer Ant blue semantics and validate through Ant policy pairings. |
| Success | Completion, valid state, positive result. | Add success text/bg/border pairings to both policies before rendering success UI broadly. |
| Warning | Caution, pending risk, recoverable issue. | Warning on light backgrounds often needs darker text than the visible swatch. Make that a policy-specific pairing, not a visual guess. |
| Error/danger | Failure, destructive action, invalid input. | Map existing `Danger` to a richer `error` family; distinguish danger button, validation, alert, and status text. |
| Info | Neutral helpful feedback and links. | Keep separate from primary where possible so every blue mark is not a primary action. |
| Neutral | Text, secondary text, disabled text, border, separator, layout, container, elevated surface. | Expand the current foreground/background/muted model into transparent or explicit neutral roles. |

Ant's font docs target high contrast for body/title text. This repo currently
uses `contrastRequiredRatio = 4.5` in shipped tokens. That remains the `wcag`
policy's compatibility contract. For `ant`, the right move is to encode Ant's
body/title, neutral, functional, and primary pairings as the selected policy.
The checker can still report ratios, but it should not reject Ant solely because
Ant's chosen policy differs from the existing WCAG-only gate.

### 3. Typography

Ant's typography story is practical and compatible with this repo:

| Ant choice | Local adaptation |
|------------|------------------|
| System UI font stack | Keep `FontFamily = None` as the default platform/system choice unless a host needs a concrete Skia typeface. |
| Base font size 14, line height 22 | Add `lineHeight` or `lineHeightRatio` tokens. Current `Theme.FontSize = 14.0` already aligns. |
| 3-5 font scales in product systems | Define a small scale: body, small, title, section, display. Avoid arbitrary per-control font sizes. |
| Regular/medium/semibold only | Add weight aliases only when the text renderer and docs can honor them consistently. |
| Tabular numbers | Useful for data grids, statistics, dates, numeric inputs, charts, and status strips. Add a `numericVariant` story when text shaping supports it. |

### 4. Layout And Density

Ant uses an 8-unit layout grid and a 24-column raster for page layout. Ant's
React token model also exposes `controlHeight = 32`, `sizeUnit = 4`, and
`sizeStep = 4`.

Local adaptation:

| Concept | Local rule |
|---------|------------|
| Page shell | Keep the showcase's persistent top bar, left navigation rail, content fill, and bottom status strip. This already matches enterprise app expectations. |
| Spacing | Move from ad hoc sizes to semantic spacing tokens: `space.xs = 4`, `space.sm = 8`, `space.md = 16`, `space.lg = 24`, `space.xl = 32`. |
| Density | Replace a raw scalar-only story with named density modes: `Comfortable`, `Middle`, `Compact`, while preserving `Theme.withDensity` for compatibility. |
| Control height | Use 32 as the default middle control height. Define small/large variants only if the typed control surface can expose them coherently. |
| Tables | Use dense but readable row heights and stable columns. Ant's table guidance is strongly aligned with the local data-grid story. |
| Cards | Use cards only for grouped content, not as default page-section decoration. This matches both Ant restraint and the repo's frontend guidance. |

### 5. Elevation, Overlays, And Shadow

Ant's shadow docs define height levels for grounded controls, hover/floating
states, dropdowns, and dialogs. FS.Skia.UI can translate this into local
elevation tokens:

| Elevation role | Ant analogue | Local uses |
|----------------|--------------|------------|
| `elevation.none` | Ground level | Inputs, default panels, table rows. |
| `elevation.low` | Hover/floating card | Hover reveal, raised command surfaces, lightweight cards. |
| `elevation.medium` | Dropdown panel | Menus, combo popups, date/time pickers, tooltip/popover surfaces. |
| `elevation.high` | Dialog | Modal dialogs, drawers, blocking feedback. |

Do not implement shadow as decoration first. Implement it where it communicates
layering: overlay, menu, dialog, popover, drawer, fixed headers.

### 6. Icons

Ant's icon docs emphasize clear meaning, consistent style, flat perspective,
consistent stroke/rounding, and color matching surrounding text except for state
icons.

Local adaptation:

- The `icon` and `icon-button` controls should distinguish system icons from
  business/illustrative icons.
- Icon-only buttons need accessible names and tooltip/invitation behavior.
- State icons should use semantic status tokens, not arbitrary accent colors.
- A future icon import pipeline could consume Ant Design Icons metadata or SVG
  paths, but it should be converted into local path primitives rather than
  shipping a web icon dependency.

### 7. Motion

Ant's motion docs emphasize Natural, Performant, and Concise. That aligns with
this repo's deterministic evidence mode and control-preview obligations.

Local rule:

- Animate only state transitions that explain cause and effect: hover, press,
  selection, loading, insertion/removal, validation, overlay enter/exit.
- Keep transitions short. Generated evidence must be able to capture stable
  states deterministically.
- Add motion tokens only after render timing and evidence captures can verify
  them without flake.

## Interaction Pattern Adoption

Ant's interaction principles map cleanly to the existing `VisualState` and
pointer/keyboard model.

| Ant principle | Local implementation |
|---------------|----------------------|
| Make it Direct | Support inline edit, editable table cells, drag handles, row actions, and click-to-edit patterns. Preserve layout stability when controls switch from read to edit mode. |
| Stay on the Page | Prefer popconfirm, undo toast, drawer, overlay, inlay, tabs, and process flow panels before navigating away. Avoid modal overuse. |
| Keep it Lightweight | Keep critical actions visible, reveal contextual tools on hover, expand hit targets, and avoid forcing every action into a toolbar. |
| Provide an Invitation | Use hover affordances, empty-state calls to action, help icons, placeholders, tooltips, and visible focus cues to teach available interactions. |
| Use Transition | Use changes in fill, outline, position, or opacity to explain what just happened, but keep motion restrained and evidence-friendly. |
| React Immediately | Every click, keypress, input, selection, validation, loading state, and failure should have immediate visible feedback. |

The local `VisualState` union already names `Normal`, `Disabled`, `Hover`,
`Pressed`, `Focused`, `Selected`, `Loading`, and `Validation`. The missing piece
is a central resolver that turns these states into consistent draw styles across
all controls.

Proposed resolver shape:

```fsharp
type ControlIntent =
    | Default
    | Primary
    | Success
    | Warning
    | Danger
    | Link
    | Text

type ControlStyle =
    { Text: Color
      Background: Color
      Border: Color
      FocusRing: Color option
      Shadow: string option
      FontSize: float
      Radius: float }

val resolve:
    theme: Theme ->
    kind: ControlKind ->
    intent: ControlIntent ->
    states: VisualState list ->
    ControlStyle
```

The exact types should follow existing code style, but the key idea is stable:
control rendering should consume semantic roles and states, not sprinkle
`theme.Accent`, `theme.Muted`, and `theme.Danger` directly across render code.

## Component And Page Pattern Adoption

### Buttons And Commands

Ant button guidance gives specific rules that are worth importing:

- Default buttons are safe non-primary actions.
- Primary buttons are for completion or recommended actions, with at most one
  primary button per group.
- Text buttons are for low-emphasis actions, especially in tables.
- Icon-only buttons need tooltips.
- Dashed buttons guide "add content" actions.
- Danger buttons mark destructive operations.
- Labels should be concise verbs and describe the action result.

Local implications:

- `ButtonIntent.Primary`, `ButtonIntent.Secondary`, and `ButtonIntent.Danger`
  should render through the style resolver. If an intent is lowered into attrs
  but not consumed by the renderer, that is a fidelity gap.
- Add or document `Default`, `Text`, `Link`, and `Additive/Dashed` button modes
  only when the control API can express them consistently.
- Table row actions should default to low-emphasis text/icon buttons, with
  destructive actions gated by popconfirm/undo patterns.

### Forms And Data Entry

Ant data-entry and form-page docs emphasize clear labels, contextual help,
structured formats, good defaults, immediate feedback, task decomposition, and
security/recovery actions.

Local priorities:

| Feature | Ant-inspired behavior |
|---------|-----------------------|
| Labels | Labels can be novice-friendly text or domain terminology, but must be consistent in a form. |
| Help text | Use short help below fields; use info icon/tooltip for longer explanations. |
| Validation | Show validation beside the field and keep it until corrected. Avoid disappearing important errors. |
| Step forms | Use `Steps` style navigation for linear multi-part tasks; add progress and final confirmation. |
| Settings | Distinguish instant-effect settings from submit-effect settings. |
| Undo/reset | Provide reset, cancel, undo, or quick fix paths for high-consequence changes. |

This points to a future `Form`, `FormItem`, `FieldHelp`, and `ValidationSummary`
story, even if the initial implementation is docs/templates rather than new
controls.

### Data Display

Ant data-display guidance is especially relevant to `DataGrid`, `ListView`, and
charts:

- Organize by information importance, operation frequency, and association.
- Design extreme states: long text, empty content, loading content.
- Tables are efficient for structured comparison and often pair with sort,
  search, filter, and paging.
- Empty table cells should use a clear placeholder, not appear broken.
- Cards are useful for lighter, personalized information blocks, but should be
  limited by grid and spacing discipline.

Local priorities:

| Surface | Adoption path |
|---------|---------------|
| DataGrid | Add Ant-style row hover, selected row, focused cell, column sort, filter, pagination, empty cell placeholder, and toolbar story. |
| ListView | Add scannable item layouts, row actions, status badges, and hover-reveal controls. |
| Charts | Use AntV/Ant Design Charts as conceptual references for dashboard layout and categorical palette, but keep chart rendering local. |
| Empty states | Add explicit empty/loading/error states to data controls and showcase pages. |

### Navigation

Ant navigation guidance maps to the showcase shell:

- Side navigation is suited to multi-level, operation-intensive dashboard apps.
- Breadcrumbs should be used sparingly and kept shallow.
- Tabs switch large information areas without page transitions.
- Steps guide predefined workflows.
- Pagination helps users understand large content sets and remaining work.

Local priorities:

- Keep the current left navigation rail as the primary showcase/navigation
  pattern.
- Add tabs for local subviews, not whole-app navigation unless the page scope is
  limited.
- Add steps for form wizard examples.
- Add pagination for data-grid/list examples instead of relying only on scroll.

### Feedback

Ant feedback guidance is a direct checklist for this repo:

| Feedback type | Local role |
|---------------|------------|
| Alert | Non-blocking page-level information that remains until dismissed. |
| Notification | Complex global notification in a top/right overlay region. |
| Badge | Aggregated count or dot on menu/avatar/icon/title. |
| Tooltip | Short description for icons, graphics, links, or unfamiliar affordances. |
| Popover | Richer contextual card with text/actions. |
| Loading/progress | Immediate feedback for operations over roughly two seconds; long work should show progress and allow cancel where possible. |
| Validation message | Inline, persistent until corrected. |
| Popconfirm | Lightweight confirmation near the target element when undo is not valid. |
| Message/toast | Lightweight, auto-dismiss result feedback for non-critical outcomes. |
| Dialog | Strong blocking feedback only for important actionable information. |

The existing showcase page `08-overlays-and-feedback.md` is the natural place to
make this concrete.

### Page Templates For Generated Apps

Ant's research template docs are useful raw material for generated-product
followups:

| Template | Local generated-app expression |
|----------|--------------------------------|
| Workbench | Operational home with tasks, recent activity, stats, charts, and shortcuts. |
| List page | Filter/search form, action toolbar, table/list, pagination, row actions, empty state. |
| Detail page | Header summary, descriptions, tabs, timeline/activity, related data. |
| Form page | Basic form, grouped form, step form, editable list/table, settings page. |
| Result page | Success/failure/pending outcome with next actions. |
| Exception page | Recoverable 403/404/500-like states in a desktop/app context. |

This gives the UI story a stronger product feel than a pure controls gallery.
The gallery can still be the evidence vehicle; generated apps can be the
consumer story.

## Governance And Gate Impact

Adopting Ant Design deeply will touch governed surfaces. Route decisions should
be expected, not worked around.

| Change type | Likely impact | Gate attention |
|-------------|---------------|----------------|
| Report-only docs under `docs/reports` | Low risk. | Run `Route`; run only printed gates. |
| Showcase spec docs | Docs/test-spec impact. | `Route` decides. Likely docs/guidance gates. |
| DTCG value-only token changes | Token public-surface area and selected-policy color behavior. | `RefreshSurfaceBaselines`, `DesignTokenDrift`, policy-backed color gate, plus printed gates. |
| New token names exposed in `.fsi` | Public API contract. | Per-package surface checks and generated surface baselines. |
| `--design-system` template parameter | Consumer template contract. | `TemplateCheck`, generated product validation, routed maintainer evidence. |
| New design-system policy | Governance/build surface. | Policy unit tests, generated policy report, routed governance evidence. |
| `Theme` record shape changes | Breaking consumer contract. | Avoid until explicitly planned; likely high/escalated route. |
| New style resolver internal to controls | Framework/internal behavior. | Dev plus focused rendering/interaction checks if routed. |
| New controls or typed props | Public control surface. | Typed parity tests, public-surface checks, rendering/interaction checks. |
| New local skill | Agent guidance generated surface. | `GeneratedGuidanceCheck` / `SkillSyncCheck` if route selects them. |

The existing `.agents` to `.claude` skill synchronization is important: the
canonical Ant skill should be `.agents/skills/fs-skia-ant-design/SKILL.md`.
The `.claude` tree should be regenerated by the established skill-tree generator
and verified by the skill sync/guidance gates. Do not hand-sync those trees.

## Recommended Roadmap

### Phase 0 - Research And Decision Record

Done by this report:

- Capture official sources.
- Identify local machinery.
- Define the adoption direction and risks.
- Draft a local skill.

### Phase 1 - UI Story Refresh

Low implementation risk, high clarity:

- Update showcase specs to explicitly say "Ant-inspired enterprise controls
  story".
- Document the design-system selector as the governing color/contrast decision:
  `wcag` remains the default, `ant` becomes the first alternative, and future
  values can add Material or Fluent/Fluid policies.
- Replace palette-only language with semantic roles: primary, success, warning,
  error, info, text, secondary text, disabled text, border, separator, container,
  elevated, selected, hover, focus.
- Add page-pattern examples: list page, form page, feedback page, workbench.
- Add a source index in the docs so future agents do not rediscover the same
  Ant pages.

### Phase 2 - Token Model Design

Design before code:

- Write a token taxonomy doc mapping Ant Seed/Map/Alias/Component tokens to
  DTCG groups and generated F# names.
- Decide whether the first implementation is explicit DTCG values/aliases or a
  generator-side derivation algorithm.
- Introduce `DesignSystemPolicy`/`ColorPolicy` plumbing:
  `wcag` delegates to today's pairings, `ant` owns Ant semantic pairings, and
  later providers add Material/Fluent equivalents.
- Define policy-specific pairings before shipping status/selection/focus text
  roles.
- Decide which tokens are public and which remain internal.

### Phase 3 - Style Resolver

Implementation focus:

- Add a central resolver for kind + intent + visual state + theme.
- Migrate buttons, text inputs, list rows, tabs, menus, and data-grid states
  first. These make the largest visual difference.
- Add tests proving selected, hover, pressed, focused, disabled, loading, and
  validation states resolve to intended roles.
- Keep old theme fields working.

### Phase 4 - Component Pattern Parity

Broaden components where Ant adds real product value:

- Buttons: default, primary, text/link, danger, add/dashed, icon-only tooltip.
- Forms: form item, help text, validation summary, grouped/step form patterns.
- Data: table toolbar, sort/filter/pagination/empty states.
- Feedback: alert, notification, popconfirm, message/toast, dialog severity.
- Navigation: side menu, tabs, steps, pagination.

### Phase 5 - Generated Product Story

Use Ant page templates as the consumer-facing demonstration:

- `dotnet new fs-skia-ui --profile workbench`
- `--profile list-page`
- `--profile form-page`
- `--profile dashboard`
- `--profile operations`

Even if these profiles are not implemented immediately, designing the showcase
around those profiles will make the project feel like a coherent UI system.

## Key Risks And Decisions

| Risk | Why it matters | Recommendation |
|------|----------------|----------------|
| Token sprawl | Ant exposes many tokens; copying them all would create maintenance burden. | Start with roles the renderer actually consumes. Add component tokens only when a component needs them. |
| Contrast mismatch | Ant defaults are designed for their own component contexts, not this Skia theme's current WCAG-only gate. | Do not force Ant through the WCAG policy. Make the selected policy explicit, report the active verdict, and optionally show WCAG ratios as diagnostics. |
| Policy ambiguity in templates | Consumers may not know whether generated apps use WCAG or Ant color rules. | Expose a visible template parameter, keep `wcag` as the default, and write the selected policy into generated app metadata/docs. |
| Public API breakage | Adding fields to public F# records is costly for consumers. | Prefer additive modules and internal resolver state first. |
| Renderer drift | Docs may promise Ant-like states that renderer does not draw. | Tie every promise to fidelity tests/evidence captures. |
| Agent inconsistency | Codex and Claude can diverge if local guidance is hand-copied. | Author only in `.agents`; let generated sync own `.claude`. |
| Over-modalization | Ant explicitly warns against modal overuse in stay-on-page flows. | Prefer undo, popconfirm, drawer, inlay, and inline edit before modal dialogs. |
| One-note palette | Ant's default blue can dominate if every selected/hover/info/link/action role uses it. | Separate primary, info, selection, link, and focus roles where useful. |

## Draft Local Skill

This is a proposed skill, not an installed file. If adopted, place it at
`.agents/skills/fs-skia-ant-design/SKILL.md`, regenerate the `.claude` mirror
through the existing surface-baseline workflow, and let `Route` select the gates.

```markdown
---
name: fs-skia-ant-design
description: Use when designing, documenting, or implementing FS.Skia.UI controls, showcase pages, generated app templates, design tokens, visual states, or local agent guidance that should follow Ant Design enterprise UI principles without introducing a React/DOM dependency.
compatibility: FS.Skia.UI.Controls + DTCG token pipeline + Skia renderer + generated skill sync; source references are Ant Design 6.x docs.
metadata:
  author: fs-skia-ui
  sources:
    - https://ant.design/docs/spec/introduce/
    - https://ant.design/docs/spec/values/
    - https://ant.design/docs/react/customize-theme/
---

# fs-skia-ant-design

## Scope

Use this skill for Ant-inspired UI story, control behavior, design-token,
theme, showcase, generated-template, color/contrast policy, and docs work in
FS.Skia.UI.

Do not use this skill to import Ant Design React components, CSS classes,
Less variables, DOM structure, or runtime dependencies into the Skia/F# product.
Do not treat the current WCAG gate as the only acceptable color/contrast
authority. Generated templates should select a design-system policy such as
`wcag` or `ant`.

## Source Priority

1. Official Ant Design spec docs:
   - Introduction: https://ant.design/docs/spec/introduce/
   - Values: https://ant.design/docs/spec/values/
   - Colors: https://ant.design/docs/spec/colors/
   - Font: https://ant.design/docs/spec/font/
   - Layout: https://ant.design/docs/spec/layout/
   - Dark mode: https://ant.design/docs/spec/dark/
   - Shadow: https://ant.design/docs/spec/shadow/
   - Icons: https://ant.design/docs/spec/icon/
   - Motion: https://ant.design/docs/spec/motion/
2. Official Ant pattern docs:
   - Design pattern overview: https://ant.design/docs/spec/overview/
   - Data entry: https://ant.design/docs/spec/data-entry/
   - Data display: https://ant.design/docs/spec/data-display/
   - Feedback: https://ant.design/docs/spec/feedback/
   - Navigation: https://ant.design/docs/spec/navigation/
   - Button: https://ant.design/docs/spec/buttons/
   - Direct manipulation: https://ant.design/docs/spec/direct/
   - Stay on page: https://ant.design/docs/spec/stay/
   - Lightweight interaction: https://ant.design/docs/spec/lightweight/
   - Invitations: https://ant.design/docs/spec/invitation/
   - Transitions: https://ant.design/docs/spec/transition/
   - Immediate reaction: https://ant.design/docs/spec/reaction/
3. Official development docs when token/component detail is needed:
   - Theme tokens: https://ant.design/docs/react/customize-theme/
   - CSS variables: https://ant.design/docs/react/css-variables/
   - CLI: https://ant.design/docs/react/cli/
   - MCP: https://ant.design/docs/react/mcp/
   - LLM files: https://ant.design/docs/react/llms/

## Workflow

1. Classify the task: token, style resolver, control, page pattern, showcase,
   generated template, docs, or skill guidance.
2. Read the smallest relevant Ant source page before making design claims.
3. Translate Ant concepts into local FS.Skia.UI primitives:
   - Ant tokens -> DTCG source and generated F# token modules.
   - Ant component states -> `VisualState` and a local style resolver.
   - Ant components -> local typed controls and Skia draw styles.
   - Ant page templates -> showcase specs and generated app profiles.
   - Ant feedback patterns -> local overlay/toast/dialog/validation controls.
   - Ant color and contrast choices -> selected `ant` policy pairings, not
     WCAG-only certification.
4. Preserve local governance:
   - Edit `src/Controls/design-tokens.tokens.json` for token values.
   - Regenerate generated surfaces instead of hand-editing them.
   - Run `./fake.sh build -t Route` before validation.
   - Run only the gates that `Route` prints.
5. For colors, identify the selected design-system policy:
   - `wcag`: use today's contrast pairings and thresholds.
   - `ant`: use Ant seed, functional, neutral, body/title, and semantic pairing
     expectations as the authority; ratios can be diagnostic.
   - future policies: add Material, Fluent/Fluid, or project-specific providers
     through the same policy interface.

## Mapping Rules

- Natural: every interaction has visible cause/effect.
- Certain: use common components, stable layout, generated tokens, and reusable
  patterns rather than per-page visual invention.
- Meaningful: avoid decorative UI; each control should support a task.
- Growing: use empty states, invitations, progressive disclosure, and helpful
  defaults to reveal capability over time.
- Keep color restrained: primary actions, information hierarchy, operation
  status, and feedback can use color; neutral surfaces carry most of the UI.
- Prefer stay-on-page patterns: inline edit, undo toast, popconfirm, drawer,
  inlay, tabs, and step flows before full navigation or modal interruption.

## Checks

- Docs-only: run `Route`; obey printed gates.
- Token value/name change: expect `RefreshSurfaceBaselines`,
  `DesignTokenDrift`, policy-backed color checks, and public-surface gates if
  routed.
- Template policy change: expect `TemplateCheck` and generated-product checks if
  routed.
- Renderer/style change: add focused rendering and interaction tests for visual
  states touched.
- Skill change: regenerate/check `.claude` mirrors through existing generated
  guidance gates.

## Useful Local Anchors

- `src/Controls/design-tokens.tokens.json`
- `src/Controls/DesignTokens.fs`
- `src/Controls/Theme.fs`
- `src/Controls/Types.fsi`
- `src/Color/Contrast.fsi`
- `src/Color/Palettes.fsi`
- `build/Governance/ContrastGate.fs`
- `docs/testSpecs/Showcase/00-controls-gallery-overview.md`
```

## Useful Resources For Later

### Ant Design Core

- Introduction: <https://ant.design/docs/spec/introduce/>
- Design Values: <https://ant.design/docs/spec/values/>
- Cases: <https://ant.design/docs/spec/cases/>
- Component overview: <https://ant.design/components/overview/>
- GitHub repository: <https://github.com/ant-design/ant-design>

### Visual System

- Colors: <https://ant.design/docs/spec/colors/>
- Dark Mode: <https://ant.design/docs/spec/dark/>
- Font: <https://ant.design/docs/spec/font/>
- Layout: <https://ant.design/docs/spec/layout/>
- Shadow: <https://ant.design/docs/spec/shadow/>
- Icons: <https://ant.design/docs/spec/icon/>
- Motion: <https://ant.design/docs/spec/motion/>
- Ant Motion: <https://motion.ant.design/>
- Theme Editor: <https://ant.design/theme-editor>

### Pattern Pages

- Design Patterns overview: <https://ant.design/docs/spec/overview/>
- Data Entry: <https://ant.design/docs/spec/data-entry/>
- Data Display: <https://ant.design/docs/spec/data-display/>
- Feedback: <https://ant.design/docs/spec/feedback/>
- Navigation: <https://ant.design/docs/spec/navigation/>
- Button: <https://ant.design/docs/spec/buttons/>
- Form Page: <https://ant.design/docs/spec/research-form/>
- List Page: <https://ant.design/docs/spec/research-list/>
- Workbench: <https://ant.design/docs/spec/research-workbench/>
- Result Page: <https://ant.design/docs/spec/research-result/>
- Exception Page: <https://ant.design/docs/spec/research-exception/>

### Interaction Principles

- Make it Direct: <https://ant.design/docs/spec/direct/>
- Stay on the Page: <https://ant.design/docs/spec/stay/>
- Keep it Lightweight: <https://ant.design/docs/spec/lightweight/>
- Provide an Invitation: <https://ant.design/docs/spec/invitation/>
- Use Transition: <https://ant.design/docs/spec/transition/>
- React Immediately: <https://ant.design/docs/spec/reaction/>
- Proximity: <https://ant.design/docs/spec/proximity/>
- Alignment: <https://ant.design/docs/spec/alignment/>
- Contrast: <https://ant.design/docs/spec/contrast/>
- Repetition: <https://ant.design/docs/spec/repetition/>

### Development And Agent Resources

- Customize Theme: <https://ant.design/docs/react/customize-theme/>
- CSS Variables: <https://ant.design/docs/react/css-variables/>
- Ant Design CLI: <https://ant.design/docs/react/cli/>
- Ant Design MCP Server: <https://ant.design/docs/react/mcp/>
- LLMs.txt guide: <https://ant.design/docs/react/llms/>
- LLM navigation file: <https://ant.design/llms.txt>
- Full LLM docs: <https://ant.design/llms-full.txt>
- Semantic LLM docs: <https://ant.design/llms-semantic.md>

### Ecosystem References

- Ant Design X: <https://x.ant.design/>
- Ant Design Charts: <https://charts.ant.design/>
- AntV: <https://antv.antgroup.com/>
- Ant Design Pro: <https://pro.ant.design/>
- Pro Components: <https://procomponents.ant.design/>
- Ant Design Mobile: <https://mobile.ant.design/>
- Ant Design Mini: <https://mini.ant.design/>
- Ant Design Web3: <https://web3.ant.design/>
- Ant Design Landing: <https://landing.ant.design/>
- Scaffolds: <https://scaffold.ant.design/>
- Umi: <https://umijs.org/>
- dumi: <https://d.umijs.org/>
- qiankun: <https://qiankun.umijs.org/>
- Kitchen: <https://kitchen.alipay.com/>
- Ant Design Blazor: <https://antblazor.com/>

## Follow-Up Questions Worth Deciding

1. Should the project explicitly market the controls story as "Ant-inspired", or
   should Ant remain internal inspiration and research lineage?
2. What should the final template values be named: `wcag`/`ant`, or more
   explicit names such as `wcag-aa`/`ant-design`?
3. Should Ant semantic tokens be public API, or should they remain an internal
   style system behind the existing `Theme` record?
4. Should token derivation be explicit DTCG values first, or should the generator
   learn Ant-like color/radius/spacing algorithms?
5. Which first vertical slice proves the adoption best: button states, form
   validation, data-grid/table, or the generated workbench/list-page template?

## Recommended First Slice

The highest-signal first implementation slice is:

1. Add the design-system selector contract to the template/UI story:
   `--design-system wcag|ant`, with `wcag` as the compatibility default.
2. Add an Ant-inspired semantic-token and color-policy design doc.
3. Add internal semantic roles for primary, success, warning, error, info,
   surface, border, selected, hover, focus, and secondary text.
4. Implement the policy-backed color checker with `wcag` and `ant` providers.
5. Implement a style resolver for buttons, text inputs, list rows, tabs, and
   data-grid rows/cells.
6. Update the showcase specs so the Controls Gallery presents an Ant-inspired
   enterprise shell and pattern pages.
7. Add the local `fs-skia-ant-design` skill and sync it to `.claude`.

This slice is narrow enough to govern and test, but broad enough to make the UI
story visibly different.
