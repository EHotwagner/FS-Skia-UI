# Phase 1 Data Model — V3 Stage 1 Host Extraction

This stage moves runtime code rather than introducing a new domain type, so the "entities" are the
**artifacts and contracts** the move produces and is verified against.

## Entity: Host modules (the thing being moved)

The Vulkan/Skia host, currently in `src/Lib/Library.fs` (namespace `FS.Skia.UI`) plus the separate
`VulkanStartup.fs(i)`/`VulkanResources.fs(i)`:

- **`Viewer`** (public; `Library.fs` 2364–2409) — `create`, `run`, `withEventMapping`,
  `withEffectMapping`, `withSubscription`, `defaultConfiguration`. The Elmish runtime edge.
- **`VulkanHost`** (internal; `Library.fs` 838–2363) — the host body.
- **`VulkanStartup`** / **`VulkanResources`** (internal; separate files) — native startup + resource
  lifetime/cleanup.
- **`Diagnostics`** / `RenderDiagnostic` (`Library.fs` 364–400) — structured host diagnostics.

**Destination:** `src/SkiaViewer/Host/*.fs(i)` (package `FS.Skia.UI.SkiaViewer`, namespace
`FS.Skia.UI.SkiaViewer`). **Invariant:** public function shapes preserved (FR-001); behaviour
preserved (parity proves it).

## Entity: Retype substitution map

The host's internal scene-type uses are rewritten from `Lib`'s `FS.Skia.UI` copy onto the
`FS.Skia.UI.Scene` package (see research D2): `Vertex`, `VertexMode`, `TextRun`, `FontSpec`,
`PerspectiveTransform`, and the `Scene`/`Paint`/`Path`/`Colors` modules. **Invariant:** no `Scene`
public API changes; `Scene` stays FSharp.Core-only; no `Scene → SkiaViewer` back-edge (FR-010/SC-006).
**Consequence:** `src/SkiaViewer/SceneConversion.fs` is deleted (FR-003) — identity conversion is dead.

## Entity: Parity oracle (the merge gate)

Committed Stage-0 deterministic golden, re-derived by the moved host:

- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/basic-viewer.txt` (authoritative)
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/effects-gallery.txt` (authoritative)
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/screenshot-gallery.txt` (authoritative)
- `tests/Parity.Tests/fixtures/v3-host-golden/screenshots/basic-viewer.png` (corroboration)
- `tests/Parity.Tests/fixtures/v3-host-golden/capture-environment.md` (recorded environment)

**Invariant (SC-002 / FR-008):** the moved host re-derives every scene-output golden **byte-identically
(0-byte diff)**; the legacy `Lib` host source is deleted **only after** the diff is clean.
**Harness:** `Parity.Tests` is **repointed onto the moved host and retained** (FR-007) — not retired
(retirement is Stage 4).

## Entity: Repoint matrix

The before/after `ProjectReference` set for every affected consumer (research D5). Two classes:
- **Repointed off the deleted modules** → `Scene` + `SkiaViewer` (+`Elmish` where used): samples
  `BasicViewer`/`EffectsGallery`/`ScreenshotGallery`/`InteractiveViewer`/`DemoReel`; tests
  `Lib.Tests`/`Smoke.Tests`/`Package.Tests`/`Parity.Tests`.
- **Reduced reference retained** (consume only `Lib` residue): `Governance.Tests` → `AgentValidation`
  (Stage 2); `ParityGallery` → `Parity` helper (Stage 4).

**Invariant (SC-005):** no project references the deleted `Lib` modules; all build green.

## Entity: `SkiaViewer` per-package surface baseline + delta

`readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt`, updated to the post-move `.fsi`.

**Invariant (FR-011/SC-007):** net public-surface delta is **empty or explicitly justified**, and
`PerPackageSurfaceDiff` is **clean** against the updated baseline. The aggregate `PackageSurfaceCheck`
stays green and unweakened.

## Entity: Leak proof

The Stage-0 reproduction command's output for two targets:
- packed `FS.Skia.UI.SkiaViewer` → **no** `FS.Skia.UI` package dependency (SC-001).
- generated default `app` → resolves **without** `FS.Skia.UI` in its transitive graph (SC-003).

**Invariant:** both show the monolith absent; recorded at `readiness/leak-proof.md`.

## State / behaviour invariants (carried)

- The Elmish boundary (Principle IV) moves with **identical function shapes**; `update` purity and
  effect-at-the-edge are unchanged. Parity + native startup/cleanup tests are the real-interpreter
  evidence.
- Native startup/cleanup lifetime/behaviour MUST NOT change (FR-012); the startup-cleanup tests travel
  with the host into `SkiaViewer` and run in the gate.
- No FCS / dynamic compilation / runtime script-loading introduced (FR-013, invariant 7).
- Package graph remains acyclic; `Scene` FSharp.Core-only (FR-010/SC-006).
