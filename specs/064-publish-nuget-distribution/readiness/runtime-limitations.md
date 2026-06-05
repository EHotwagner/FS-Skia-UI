# Runtime limitations

## Sandbox aggregate-target bootstrap

The aggregate `Verify`/`Ci` umbrella targets cannot bootstrap the `dotnet-fake` global
tool in this sandbox, so the aggregate handoff verdict reads `degraded`. Every constituent
gate that `./fake.sh build -t Route` prints is run **individually and sequentially**; the
authoritative merge gate is `EvidenceAudit`. This is a non-authoritative aggregate
limitation, not a gate failure (see [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md)).

## Distribution-feature scope

This feature is **packaging / release tooling**. No runtime model, interpreter, effect,
subscription, layout, charts, DataGrid, rendering, screenshot, Vulkan, or Skia behavior
changes. The publish machinery (`Publish` / `PrePublishCheck` targets, the `PublishPackages`
effect) lives entirely in the `FS.Skia.UI.Build` front-end and runs `dotnet pack` /
`dotnet nuget push` / an anonymous flat-container (or local-directory) read — none of which
touch the product runtime. All pre-production validation runs against a **throwaway
local-directory staging feed**, credential-free.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux, renders
through **Vulkan**, and depends on a **SkiaSharp preview** native build. Platforms remain
**unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**. This
feature changes none of that — it changes how the packages are *distributed* (a published
public feed + single-source pin + metadata), not how the runtime executes.

## Maintainer-gated production push (FR-008 / SC-008)

The irreversible first push to public nuget.org depends on the maintainer's nuget.org
credential and the permanent `FS.Skia.UI.*` package-ID-namespace claim, which cannot be
created or secured from headless automation. T041 therefore legitimately remains `[ ]`
after `EvidenceAudit verdict=PASS`; the audit gates on `[S]`/`[S*]`/diff-scan, not pending
maintainer steps.
