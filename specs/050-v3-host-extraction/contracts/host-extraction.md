# Contract — Host Extraction & Retype

The behavioural contract the moved host must satisfy. Verified by `Parity.Tests`, the native
startup/cleanup tests, `PerPackageSurfaceDiff`, and the leak proof.

## Moved surface (FR-001)

`FS.Skia.UI.SkiaViewer` (under `Host/`) MUST expose the host with **identical function shapes**:

| Symbol | Shape preserved | Home before → after |
|---|---|---|
| `Viewer.create` | yes | `Lib` `FS.Skia.UI.Viewer` → `SkiaViewer` `Host/Viewer` |
| `Viewer.run` | yes | same |
| `Viewer.withEventMapping` | yes | same |
| `Viewer.withEffectMapping` | yes | same |
| `Viewer.withSubscription` | yes | same |
| `Viewer.defaultConfiguration` | yes | same |
| `Diagnostics`/`RenderDiagnostic` | yes | `Lib` → `SkiaViewer` `Host/Diagnostics` (or canonical home per research D1) |
| `VulkanStartup` / `VulkanResources` (internal) | yes | `Lib` files → `SkiaViewer` `Host/Vulkan` |

The `SkiaViewer` wrapper already re-exposed this API, so its **outward** `.fsi` is expected stable
(FR-011). Every moved public module carries a curated `.fsi` (Principle II).

## Retype substitution (FR-002 / FR-003)

Every host-internal use of `Lib`'s `FS.Skia.UI` scene types is replaced by the `FS.Skia.UI.Scene`
equivalent (`Vertex`/`VertexMode`/`TextRun`/`FontSpec`/`PerspectiveTransform`/`Scene`/`Paint`/`Path`/
`Colors`). `SceneConversion.fs` is **deleted** — no conversion remains.

## Dependency contract (FR-004 / FR-010)

- `SkiaViewer.fsproj` has **zero** `ProjectReference` to `Lib`.
- packed `FS.Skia.UI.SkiaViewer` has **zero** package dependency on `FS.Skia.UI`.
- `SkiaViewer → {Scene, KeyboardInput}` + native packages only.
- **No** `Scene → SkiaViewer` back-edge; `Scene` stays FSharp.Core-only; graph acyclic.

## Deletion precondition (FR-005 / FR-008 — the merge gate)

`Lib`'s `Colors`/`Paint`/`Path`/`Scene`/`Diagnostics`/`Viewer`/`VulkanHost` (+ the moved
`VulkanStartup`/`VulkanResources`) MUST NOT be deleted until the `Parity.Tests` scene-output diff is
**0 bytes** for all three seeds. After deletion, `Lib` retains only `AgentValidation`, the duplicate
`KeyboardInput`, and the `Parity` helper (SC-004).

## Surface contract (FR-011 / SC-007)

`readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt` is updated to the post-move `.fsi`; net
delta is empty or explicitly justified; `PerPackageSurfaceDiff` is clean; aggregate
`PackageSurfaceCheck` stays green.

## Observability contract (FR-013 / Principle VII)

Structured host diagnostics (`RenderDiagnostic`, the `Diagnostics` module) travel unchanged — startup,
subsystem init, and asset/IO failure still emit actionable context and fail fast. No FCS / dynamic
compilation / runtime script-loading is introduced.
