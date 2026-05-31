# Contract: Name-Collision Hardening (US3)

**Satisfies:** FR-008 · SC-003 · Tier 1 contract change

## Surface delta

- `src/SkiaViewer/SkiaViewer.fsi` (+ `.fs`): `[<RequireQualifiedAccess>]` on
  `ViewerWindowStartupState` (bare `Normal` at `SkiaViewer.fsi:45` collides with a
  consumer's `Normal`).
- Viewer/input `update`/`init`-bearing surfaces (`Viewer.*` in `SkiaViewer.fsi`,
  `ElmishAdapter.*` in `Elmish.fsi`, input bindings in `KeyboardInput.fsi`): apply
  RQA where a real shadow exists, or confirm existing module-qualification.
  Applied **consistently** across the public surface, not case by case.

## Rules

1. After hardening, a consumer who `open`s the framework namespace and defines
   their own `Normal`, `update`, `init` has those resolve to the **consumer's**
   definitions; the framework's require qualification.
2. Existing positional/other surface is otherwise unchanged.

## Process (breaking change accepted)

- Update `.fs` + `.fsi` together (visibility lives in `.fsi`).
- Refresh `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`, merged
  `FS.Skia.UI.txt`, and any affected input baseline via
  `scripts/refresh-surface-baselines.fsx`.
- Update **all generated samples** so a freshly generated project compiles with
  the clean surface.
- Record migration note (`readiness/name-collision-migration.md`) and bump
  package versions on merge.

## Failing-first fixture

`readiness/fsi/` — a consumer fixture defining its own `Normal`/`update`/`init`
after `open`: FAILS to compile before the hardening, compiles after.
