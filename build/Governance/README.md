# FS.Skia.UI.Build

Compiled-F# governance engine (evidence graph + merge-gate audit) consumed in-process by FS.Skia.UI build tooling and generated products.

`FS.Skia.UI.Build` is one of the **FS.Skia.UI** distribution packages — an F# / Elmish UI and 2D
scene-graph framework for .NET 10 desktop, rendered through OpenGL + SkiaSharp.

## Install

```bash
dotnet add package FS.Skia.UI.Build
```

Or scaffold a full governed project that wires the FS.Skia.UI packages together:

```bash
dotnet new install FS.Skia.UI.Template
dotnet new fs-skia-ui -o MyApp
```

## Usage

`FS.Skia.UI.Build` is the compiled governance/build engine, not a library you call from
application code — you *drive* it through FAKE via the `./fake.sh` front-end shipped in a
generated project. Always run `Route` first: it reads the working-tree diff and prints the
authoritative tier plus the **minimal gate list** for that change. Then run only the gates it
names.

```bash
# 1. Ask the engine which gates this change actually needs.
./fake.sh build -t Route

# 2. Run the gate(s) it printed — a routine framework change routes to Dev only.
./fake.sh build -t Dev
```

FAKE-backed gates share repository `.fake` state and are **not** safe to run concurrently — when
more than one gate is needed, run them sequentially in deterministic order, never in parallel.

## Targets at a glance

- **Route** — reads the diff and prints the tier + minimal gate list for the current change; run it first.
- **Dev** — the light inner-loop gate (composes `Test` + `SkillSyncCheck`) for routine framework-internal changes.
- **Verify** — the escalated maintainer-verify aggregate that fans out to the full gate set (packing, template, generated-product, evidence, and metadata checks).
- **Ci** — the CI entry point: `CiPreflight` followed by `Verify`.
- **TemplateCheck** — packs, installs, instantiates, and smoke-tests the `dotnet new fs-skia-ui` template.
- **GeneratedProductCheck** — validates the generated product against the capability catalog and skill surface.
- **EvidenceGraph** / **EvidenceAudit** — validate the task DAG and run the merge-gate audit (synthetic propagation + diff scan).
- **PrePublishCheck** / **Publish** — the distribution gates: pin-parity / required-metadata consistency, then the idempotent NuGet push.

## Versioning

All `FS.Skia.UI.*` libraries share one version and move together. In a generated project a
single `<FsSkiaUiVersion>` in `Directory.Packages.props` pins every package — upgrading is one
edit; see `docs/UPGRADING.md`. Pre-release versions use a `-preview.N` suffix.

## Links

- Repository & issues: https://github.com/FS-Skia-UI/FS-Skia-UI
- License: MIT
