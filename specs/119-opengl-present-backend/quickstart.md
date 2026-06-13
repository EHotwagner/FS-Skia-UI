# Quickstart: OpenGL Present Backend

How to build, exercise, and validate the GL host backend. Run `./fake.sh build -t Route` first —
it prints the authoritative escalated gate list for this change; run only what it prints.
FAKE-backed targets share `.fake` state — run them **sequentially**.

## 1. Dependency swap

`Directory.Packages.props`:
- remove `Silk.NET.Vulkan` and `Silk.NET.Vulkan.Extensions.KHR`
- add `Silk.NET.OpenGL` version `2.23.0`

`src/SkiaViewer/SkiaViewer.fsproj`: replace the two `Silk.NET.Vulkan*` `PackageReference`s with
`Silk.NET.OpenGL` (keep `Silk.NET.Windowing*` / `Silk.NET.Input` / `SkiaSharp*`).

## 2. FSI-first (Principle I) — before the `.fs` body

Draft `Host/OpenGl.fsi` per `contracts/gl-host-surface.md`, then exercise the surface through a
prelude transcript against the packed library:

```fsharp
// readiness/fsi/gl-host-prelude.fsx
#r "nuget: FS.Skia.UI.SkiaViewer, <bumped-version>"
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.SkiaViewer.Host
GlResources.empty |> GlResources.acquire "ctx" GlResources.GlContext "startup" "host" "destroy"
GlStartup.stages |> List.map (fun s -> s.Name, s.Order)
{ /* ViewerOptions */ PresentMode = ViewerPresentMode.DirectToSwapchain }   // default on GL
```

## 3. Build + unit/property tests

```
./fake.sh build -t Dev
```
Includes the new `Feature119` tests (present-mode mapping, diagnostic classification, GL
resource/startup-ledger property tests) and the updated governance tests.

## 4. Real live-host evidence (GPU-passthrough machine)

```
# persistent interactive window — confirm scene renders + input works
<launch the interactive host>      → readiness/supported-host-persistent-launch.txt

# zero-readback proof: instrument the direct path; assert per-frame readback count == 0
<run direct-present mode>          → readiness/smoke/zero-readback-present.md

# sample-smoke captures match baselines (controls/charts/datagrid)
<run sample-smoke>                 → readiness/sample-smoke/*

# unsupported-GL: run with GL unavailable (headless / no passthrough)
<run without GL>                   → readiness/smoke/unsupported-gl-diagnostic.md
```
Timing is a diagnostic signal only — **no timing-based pass/fail gate** (118 §6). The
zero-readback claim is asserted by **counts/booleans**, not milliseconds.

## 5. Surface + dependency + governance gates

```
./fake.sh build -t DependencyReport          # Silk.NET.Vulkan* gone, Silk.NET.OpenGL present
./fake.sh build -t GeneratedGuidanceCheck     # evidence-formats.md regenerated (Vulkan→OpenGL token)
./fake.sh build -t TemplateCheck              # expected pin-lag failure pre-merge (template follow-up)
./fake.sh build -t GeneratedProductCheck      # generated runtime-limitations.md seed = OpenGL
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit              # verdict=PASS, 0 synthetic
# plus PackageSurfaceCheck / PerPackageSurfaceDiff after RefreshSurfaceBaselines
./fake.sh build -t RefreshSurfaceBaselines
```

## 6. Governance token + constitution (FR-010 / FR-011)

- `build/Governance/Evidence/EvidenceFormatSchema.fs` — `readinessContractChecks`
  `runtime-limitations.md` row: `"Vulkan"` → `"OpenGL"` (single source; regenerates the
  reference).
- `build/Governance/GeneratedProduct.fs:970` — generated seed "Vulkan backend required" →
  "OpenGL backend required".
- `build/Governance/GovernedBlocks.fs:267` + `build/Governance/README.md` — constraint/prose.
- `/speckit-constitution` — amend the `Project-specific constraints` and "Vulkan smoke" clause to
  OpenGL; the `GovernedBlocks.fs` fragment must match the amended constitution.
- Write `readiness/runtime-limitations.md` with the GL token set (`.NET 10 desktop`, `OpenGL`,
  `SkiaSharp preview`, `unsupported macOS/mobile/browser`, `no software-renderer fallback`).

## 7. Migration note (FR-009)

`readiness/migration.md` lists every removed/renamed public member with its GL replacement (see
`contracts/gl-host-surface.md` §4). Confirm `runInteractiveApp` / `runInteractiveViewer` /
`ViewerOptions` compile unchanged.

## Done-when

- SC-001 zero per-frame readback in direct mode (counts) ✓
- SC-002 sample/evidence pixels match baseline ✓
- SC-003 pointer/keyboard/focus/animation identical ✓
- SC-004 GL-unavailable classified 100%, no false success ✓
- SC-005 high-level entry points compile unchanged; migration names every member ✓
- SC-006 no governance gate asserts Vulkan; full routed set + EvidenceAudit PASS, 0 synthetic ✓
