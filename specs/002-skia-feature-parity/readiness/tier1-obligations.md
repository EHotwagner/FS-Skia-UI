# Tier 1 Feature Obligations

Feature: `002-skia-feature-parity`

## Public API Impact

This feature is Tier 1. Any public surface added for core scene/viewer APIs, chart/DataGrid APIs, or layout/graph APIs must be declared in a companion `.fsi` file before implementation is considered complete.

Planned package surfaces:

- `FS.Skia.UI`
- `FS.Skia.UI.Charts`
- `FS.Skia.UI.Layout`

## Visibility

- Do not place `private`, `internal`, or `public` on top-level F# bindings.
- Public availability is controlled by `.fsi` signatures.
- Surface-area baseline tests must verify exported modules and values.

## Elmish/MVU Applicability

- Stateful viewer operation, lifecycle, input, screenshots, diagnostics, render requests, and shutdown must flow through Elmish `Model` / `Msg` / `Effect` or `Cmd<Msg>`.
- Pure chart, DataGrid, layout, and graph components return scene elements and do not own application state.
- Tests for MVU-bearing tasks must assert pure `update` transitions and emitted effects, then exercise the interpreter edge with real dependencies where safe.

## Vulkan-Only Constraint

- The implementation must not add a fallback renderer.
- Upstream GL fallback behavior is an excluded/adapted baseline behavior.
- Startup diagnostics must make Vulkan capability failures explicit.

## Real vs Synthetic Evidence

- `[X]` requires real evidence for the production code path.
- `[S]` is required for mocks, fakes, canned responses, in-memory substitutes, placeholders, hardcoded production stand-ins, or tests that only exercise synthetic fixtures.
- Any `[S]` task requires a `// SYNTHETIC:` use-site disclosure, synthetic test naming, and a Synthetic-Evidence Inventory row in `tasks.md`.
- User-story tasks require a user-reachable exercise before `[X]`: packed-library/prelude transcript, smoke run, screenshot, or semantic test through the host/public package path.
