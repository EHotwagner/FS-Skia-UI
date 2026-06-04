# Product

This generated product references selected FS.Skia.UI capabilities instead of
copying the framework repository.

The selected capabilities are controlled by `--profile`:

- `app`: Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Controls, Controls.Elmish adapter
- `headless-scene`: Scene
- `governed`: Scene, Testing
- `sample-pack`: Scene, SkiaViewer, Elmish, Samples

## Quickstart

Run the generated product governance checks:

Generated FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`)
share `.fake` state and are not safe to run concurrently. Run multiple
FAKE-backed validation commands sequentially, and record that order in
readiness evidence. Non-FAKE checks may still run in parallel when they do not
invoke FAKE or depend on `.fake`.

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t Test`
3. `./fake.sh build -t Verify`

> **`Dev` is a completion-marker / log-writer target, not a compiler.** It only
> records progress to `readiness/logs/Dev.txt` — it does **not** give you real
> compiler or test feedback. The authoritative compile/test path is
> `./fake.sh build -t Test` / `./fake.sh build -t Verify` (which run
> `dotnet test`); use those (or `dotnet build` / `dotnet test` directly) when you
> need actual compiler errors and test results. Do not infer "it compiles" from a
> green `Dev`.

Run Spec Kit evidence checks through the generated FAKE targets:

1. `./fake.sh build -t EvidenceGraph`
2. `./fake.sh build -t EvidenceAudit`

If a generated FAKE-backed command fails with race-like symptoms or unknown
concurrent context, rerun the affected FAKE-backed commands sequentially before
classifying the failure as a product regression.

The generated targets run the evidence graph and merge-gate audit in-process
through the packaged `FS.Skia.UI.Build` engine (no Python or shell audit script
is copied into or executed by the generated project).
The workflow does not depend on executable file mode being preserved when the
checkout is copied. Redirected `Verify` output is written as plain text under
`readiness/logs/`; pass and fail diagnostics should remain readable and must not
contain embedded NUL byte blocks.

## Explore the app in FSI

To load the built app and all its transitive `FS.Skia.UI.*` references into FSI
in a single step — with **zero manual reference edits** — build once, then run
the generated load script:

1. `./fake.sh build -t Dev`
2. `dotnet fsi load-product.fsx`

`load-product.fsx` is **generated** and stays in sync with the product's
assembly set: it is derived from `Directory.Packages.props` and the built
`Product` output, not a hand-maintained reference list, so do not edit it. It
only `#r`s the assemblies and `open`s `Product` — it launches nothing, so it
neither emits nor suppresses host warnings, and a missing assembly surfaces as a
normal load failure.

Spec Kit is installed in this repo through `.specify/` and the project-local
`speckit-*` skills under `.agents/skills/`. Use `$speckit-specify`,
`$speckit-plan`, and `$speckit-tasks` to start governed feature work.

The product references FS.Skia.UI preview packages from the configured NuGet
sources. For local framework development, pack the source repository with
`./fake.sh build -t PackLocal` and add `~/.local/share/nuget-local` as a NuGet
source before restoring or running this generated project.
Use the generated source-shaped package API reference. Do not use assembly reflection or
repository source inspection as an authoring substitute. When Scene and Controls are used in
the same file, qualify collision-prone names such as
`FS.Skia.UI.Scene.Rect`, `FS.Skia.UI.Scene.Paint`,
`FS.Skia.UI.Scene.TextRun`, `FS.Skia.UI.Controls.TextBlock.create`,
`FS.Skia.UI.Controls.TextBox.onChanged`, and
`FS.Skia.UI.Controls.Stack.children`. Do not rely on namespace open order.

The product owns its application code, tests, documentation, readiness evidence,
and selected local skills.

Visual demo task lists assign scene rendering -> fs-skia-scene, screenshot
capture -> fs-skia-skiaviewer, layout readability -> fs-skia-layout-readability,
persistent viewer launch -> fs-skia-skiaviewer, deterministic evidence mode ->
fs-skia-evidence-mode, generated-package validation ->
fs-skia-template-update, graph validation -> speckit-evidence-graph, and audit
validation -> speckit-evidence-audit. Ordered multi-skill examples preserve
implementation-before-evidence, graph-before-audit, debug-before-broad-rerun,
and visible mirrors such as `[skillist: speckit-tasks, fs-skia-layout-readability]`.

Generated readiness scaffolds include `readiness/visual-evidence-honesty.md`,
`readiness/window-visibility.md`, `readiness/governance-risk-levels.md`,
`readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`,
`readiness/generated-guidance-validation.md`, and
`readiness/real-image-evidence.md`. Each scaffold records the authoritative
command, artifact path, failure class, and next action.

For generated app profiles, `FS.Skia.UI.Controls` is the authoring path for
ordinary controls, rich text, chart controls, graph controls, and DataGrid.
When Elmish integration is selected, `FS.Skia.UI.Controls.Elmish` provides the
adapter for commands, subscriptions, and program wiring. Users moving from the
legacy Charts package should use Controls chart and DataGrid declarations
directly; there is no compatibility shim.

## Archive And API Reference Guidance

For generated product governance, current feature readiness paths are authoritative for current gates. historical feature readiness is audit context only unless a current evidence map explicitly marks it as supporting evidence.
Archived material must not be cited as current package, template, generated-product, or audit pass/fail evidence.

The source-shaped `.fsi` package API reference remains authoritative for agent
authoring. FSharp.Formatting/fsdocs output is secondary or hybrid unless the
active generator decision record marks it authoritative. Package consumers must not use assembly reflection or repository source inspection as an authoring substitute.
