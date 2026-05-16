# Contract: Generated Product Content

## Purpose

Generated products are framework consumers. They must contain product-owned
source, product tests, full product governance, selected skills, and selected
package references without copying framework repository internals.

## Default App Must Include

- one product application project
- one product test project
- product README
- minimal product docs
- command wrappers
- full product Spec Kit governance assets
- local project skill
- selected capability skills for Scene, SkiaViewer, Elmish, KeyboardInput,
  Layout, and Charts
- package references or equivalent generated consumer references for Scene,
  SkiaViewer, Elmish, KeyboardInput, Layout, and Charts

## Default App Must Exclude

- framework sample directories
- framework gallery applications
- framework parity suite
- historical feature specs
- framework readiness evidence
- framework documentation set
- framework README content
- framework implementation projects
- framework template package project
- framework generated validation roots

## Validation Contract

`GeneratedProductCheck` must produce a file-list report for every generated
validation row and fail on:

- missing required product files
- more than one product app or test project in the default app profile
- copied framework implementation projects in consumer mode
- copied framework docs or README sections
- copied samples when sample profile is not selected
- copied historical specs or readiness evidence
- missing full product governance assets
- missing selected capability skills
- unrelated capability skills

Failures must include generated root, profile, capability selection, offending
path, and expected rule.
