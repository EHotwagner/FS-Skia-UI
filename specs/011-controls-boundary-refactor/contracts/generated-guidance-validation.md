# Contract: Generated Guidance Validation

## Purpose

Define generated product guidance and template validation obligations for the
new Controls boundary.

## Required Generated Content

Generated products that select Controls must include:

- `FS.Skia.UI.Controls` package reference
- concise Controls guidance for forms, rich text, charts, graph views, and
  DataGrid
- representative product-owned controls usage
- at least one ordinary form control example
- at least one chart, graph, or DataGrid example
- generic message-based Controls flow
- Elmish adapter flow when Elmish program integration is selected

## Forbidden Generated Content

Generated products must not include:

- `FS.Skia.UI.Charts` package reference
- active `charts` capability selection
- chart-only generated skill or guidance for new control work
- renderer-neutral controls promises
- framework sample/gallery source copied as product implementation
- historical specs, readiness evidence, framework docs, framework README copy,
  or framework implementation projects

## Guidance Scan Rules

Generated guidance checks fail when they find:

- stale chart-only active capability references
- DataGrid described only as a chart
- Controls described as renderer-neutral
- direct host-loop ownership required for ordinary Controls declarations
- missing migration guidance for removed Charts package users
- missing distinction between generic message-based Controls and Elmish
  adapter integration

## Validation Commands

- `TemplateCheck`
- `GeneratedProductCheck`
- `GeneratedGuidanceCheck`
- `TemplateDrift`
- generated product `Dev`, `Test`, and `Verify` commands where applicable

Failures must identify the generated profile, file path, package reference,
capability id, skill path, or stale text pattern.
