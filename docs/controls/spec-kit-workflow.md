---
title: Controls in the Spec Kit workflow
category: Controls
categoryindex: 8
index: 1
description: Where controls are chosen, authored, and validated across the Spec Kit phases (specify → plan → tasks → implement), and how Penpot/design-token changes flow into control theming.
---

# Controls in the Spec Kit workflow

This page explains **where controls live in the development process** — which Spec
Kit phase chooses them, which authors them, and which validates them — so you can
go from "I need a control" to "the control ships, proven" without guessing. For
the exhaustive list of what exists, see the [Controls Catalog](catalog.html).

FS.Skia.UI is built with **Spec Kit**: each feature flows through
`specify → plan → tasks → implement`, and controls enter the picture at distinct
phases.

## Where controls are **chosen** — specify & plan

- **`specify`** captures the user-facing need ("the user must pick a date", "show a
  progress indicator"). At this phase you choose controls by their **purpose**, not
  their API — the [Controls Catalog](catalog.html) is the menu: every supported
  control with a one-line purpose, grouped by category (display, input, selection,
  navigation, layout, feedback, data, chart, graph, overlay, custom).
- **`plan`** commits to the concrete controls and their composition. Because the
  catalog is generated from the single-source control registry, the controls named
  in a plan are guaranteed to exist and to match the shipped API — a plan can link
  straight to a control's detail page and its API reference.

## Where controls are **authored** — implement (the typed front door)

During **`implement`**, author UI through the **typed Props/MVU front door** under
`FS.Skia.UI.Controls.Typed`. Each control is an immutable `Props` record plus
`defaults` and a `view` returning a `Widget`, so the compiler checks every required
attribute and the control composes in an Elmish/MVU shape. The typed surface is the
preferred authoring path; it lowers structurally to the legacy builder, so there is
one catalog and one runtime behavior behind two authoring styles.

See **[Typed control front door](../controls-design/typed-front-door.html)** for the
authoring recipe, the immutable-Props pattern, and the per-control parity guarantee.
Each control's [detail page](catalog.html) names the exact module
(`FS.Skia.UI.Controls.Typed.<Control>` or its legacy peer) and links its generated
API reference.

## Where controls are **validated** — the gates

Controls are validated by the gates the change routes to (run
`./fake.sh build -t Route` to see the authoritative list for your diff):

- **`ControlsCatalogCheck`** — the catalog's row count, metadata, examples, tests,
  evidence, and accessibility facts.
- **`ControlsCatalogGenerationCheck`** — the generated `catalog.yml` / `Catalog.fs`
  rows are a current projection of the single source.
- **`ControlsInteractionCheck` / `ControlsRenderingCheck`** — pointer/keyboard
  dispatch, disabled/read-only suppression, and render evidence across viewports.
- **`ControlsCatalogDocsCheck`** — *this* documentation section stays a current,
  complete, honest projection of the catalog (every control has a detail page, a
  resolving API link, and a preview or an honest note).

A control is "done" when the user-facing surface is reachable and the routed gates
pass — not merely when the model compiles.

## Penpot & design tokens

Control **theming is not hand-coded per control** — it derives from **design
tokens**. The 10 `Theme` primitives in `FS.Skia.UI.Controls` (foreground,
background, accent, danger, muted, font family, font size, density, corner radius,
and the contrast-required ratio, each for `light` and `dark`) are **single-sourced**
from a DTCG (Design Tokens Community Group) JSON document and generated into the
typed `DesignTokens.Light` / `DesignTokens.Dark` surface.

The path from a design change to control appearance:

1. **The single source** is `src/Controls/design-tokens.tokens.json` — the one edit
   point. A token can be a literal value or a DTCG **alias** of another token
   (`"{light.danger}"`).
2. **Regenerate** with `./fake.sh build -t RefreshSurfaceBaselines`, which renders
   the DTCG source into the generated `src/Controls/DesignTokens.fs` module.
3. **The `DesignTokenDrift` gate** fails if the generated module is not a
   byte-identical regeneration of the source, so the tokens a control renders with
   can never drift from the design source of truth.
4. **Controls render against the active `Theme`**, so changing a token value
   re-themes every control that uses it — no per-control edit.

A **Penpot** design workflow drives this by exporting design decisions as DTCG
tokens into that single source (live Penpot/MCP sync is documented as future work,
not yet wired). For the full token edit flow, the alias semantics, the drift gate,
and authoring against the typed token surface, see
**[Design tokens & Penpot](../controls-design/design-tokens-penpot.html)**.
