---
name: fs-skia-ui-widgets
description: Generated product guidance for FS.Skia.UI Controls widgets, chart controls, graph controls, and custom wrappers.
---

# Generated Controls Widgets

## Scope

Use this skill for generated product screens that compose controls in an
Elmish-style view function.

## Public Contract

Reference `FS.Skia.UI.Controls` and build `Control<'msg>` values with
module-per-control `create` functions and declarative attributes.

## Build Commands

Run `./fake.sh build -t Dev` and `./fake.sh build -t Verify` in the generated
product.

## Test Commands

Run `./fake.sh build -t Test` for product-owned control examples.

## Evidence

Product evidence belongs in the generated product readiness folder. Do not copy
framework readiness reports.

## Package Boundary

Controls owns widget, chart, and graph authoring. Layout remains a package
dependency but not a separate generated widget skill.

## Generated Product

Keep examples small and product-owned. Do not include `fs-skia-charts` or
generated `fs-skia-layout` widget guidance.
