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

Run Spec Kit evidence checks through the generated FAKE targets:

1. `./fake.sh build -t EvidenceGraph`
2. `./fake.sh build -t EvidenceAudit`

If a generated FAKE-backed command fails with race-like symptoms or unknown
concurrent context, rerun the affected FAKE-backed commands sequentially before
classifying the failure as a product regression.

The generated targets delegate to the copied Spec Kit audit script through
`bash`, so the workflow does not depend on executable file mode being preserved
when the checkout is copied. Redirected `Verify` output is written as plain text
under `readiness/logs/`; pass and fail diagnostics should remain readable and
must not contain embedded NUL byte blocks.

Spec Kit is installed in this repo through `.specify/` and the project-local
`speckit-*` skills under `.agents/skills/`. Use `$speckit-specify`,
`$speckit-plan`, and `$speckit-tasks` to start governed feature work.

The product references FS.Skia.UI preview packages from the configured NuGet
sources. For local framework development, pack the source repository with
`./fake.sh build -t PackLocal` and add `~/.local/share/nuget-local` as a NuGet
source before restoring or running this generated project.

The product owns its application code, tests, documentation, readiness evidence,
and selected local skills.

For generated app profiles, `FS.Skia.UI.Controls` is the authoring path for
ordinary controls, rich text, chart controls, graph controls, and DataGrid.
When Elmish integration is selected, `FS.Skia.UI.Controls.Elmish` provides the
adapter for commands, subscriptions, and program wiring. Users moving from the
legacy Charts package should use Controls chart and DataGrid declarations
directly; there is no compatibility shim.
