# Name-Collision Migration Note (US3, FR-008)

**Breaking change (accepted per spec Clarifications): `ViewerWindowStartupState`
now requires qualified access.**

## What changed

`[<RequireQualifiedAccess>]` was added to `ViewerWindowStartupState`
(`src/SkiaViewer/SkiaViewer.fsi` + `.fs`). Its cases (`Normal`, `Maximized`,
`Minimized`, `Fullscreen`) are no longer imported unqualified by
`open FS.Skia.UI.SkiaViewer`.

## Why

A consumer who declares their own `Normal` (a very common app-state case) and
then `open`s the viewer namespace previously had the framework's bare `Normal`
shadow theirs, producing a confusing type error. See the FAIL-before transcript
in `fsi/us3-name-collision-before-FAIL.txt`.

## Before / after

```fsharp
// BEFORE — fails to compile: the framework's Normal shadows AppState.Normal
type AppState = Normal | Busy
open FS.Skia.UI.SkiaViewer
let init () : AppState = Normal          // error FS0001: expected AppState, got ViewerWindowStartupState

// AFTER — compiles: the framework's Normal requires qualification
type AppState = Normal | Busy
open FS.Skia.UI.SkiaViewer
let init () : AppState = Normal                       // resolves to AppState.Normal
let startup = ViewerWindowStartupState.Normal         // framework value, now qualified
```

## Migration for existing consumers

Any consumer that referenced `Normal`/`Maximized`/`Minimized`/`Fullscreen`
unqualified (expecting the framework's `ViewerWindowStartupState`) must now
qualify them as `ViewerWindowStartupState.Normal`, etc. All in-repo usages
(`src/SkiaViewer/SkiaViewer.fs`, `tests/SkiaViewer.Tests/Tests.fs`,
`samples/DemoReel/Program.fs`) were updated; generated samples were unaffected
(they did not reference these cases unqualified).

## Scope confirmation

Only `ViewerWindowStartupState` was hardened. The `update`/`init`-bearing
surfaces (`Viewer.*`, `ElmishAdapter.*`, `Keyboard.*`) are already
module-qualified (no `[<AutoOpen>]`) and need no change — see
`collision-name-enumeration.md`.

## Version bump

This is a Tier 1 contract change. The affected packages
(`FS.Skia.UI.SkiaViewer` and the merged `FS.Skia.UI`) receive a version bump at
merge time per the standard merge-pack flow.

## Surface baselines

`[<RequireQualifiedAccess>]` does not change an exported type name, so the
curated name-list baselines (`FS.Skia.UI.SkiaViewer.txt`, merged
`FS.Skia.UI.txt`) are unchanged; `PackageSurfaceCheck` (a presence check) stays
green. The authoritative behavioral evidence is the FAIL→PASS consumer compile
under `fsi/`.
