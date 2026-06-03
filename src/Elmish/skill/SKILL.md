---
name: fs-skia-elmish
description: Work on Elmish adapter contracts and generated product Elmish wiring.
---

# Elmish Capability

## Scope

Owns `src/Elmish/`, Elmish adapter tests, `template/fragments/elmish/`, and generated product Elmish entry points.

## Public Contract

The supported API lives in `src/Elmish/Elmish.fsi`. Surface changes require `readiness/surface-baselines/FS.Skia.UI.Elmish.txt`.

## Build Commands

Run `./fake.sh build -t CapabilityCheck`, `./fake.sh build -t DependencyReport`, and `./fake.sh build -t PackLocal`.

## Test Commands

Run `dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj` and `./fake.sh build -t GeneratedProductCheck`.

## Evidence

Record transition and effect evidence under the active feature readiness
package-surface reports when adapter behavior changes. Stable public surface
baselines live under `readiness/surface-baselines/`.

## Package Boundary

Keep `Model`, `Msg`, `Effect`, `init`, and `update` pure. Native viewer I/O belongs to SkiaViewer interpreter code.

## Generated Product

Products that select Elmish receive Scene and SkiaViewer prerequisites plus this skill.

## Runnable example

Open the package namespace and initialize the adapter over a pure user model:

```fsharp
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.Elmish

let options = { Title = "elmish"; InitialSize = { Width = 320; Height = 240 } }
let render (count: int) = Text((10.0, 20.0), sprintf "count=%d" count, Colors.white)

let model, _effects = ElmishAdapter.init options 0 (render 0)
let next, _ = ElmishAdapter.update render (UserMsg 1) model
printfn "user model = %d" next.UserModel
```

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the F#/.NET docs and the driven
library's own documentation/API reference), then community sources (forums, Reddit, Q&A
sites, issue trackers and changelogs). Record the findings and resolving links in the
feature's `specs/<feature>/feedback/` folder and, for durable lessons, in this skill's
**Sources** line. Offline, the mandate degrades to recording "research blocked — <why>"
rather than hard-failing the phase.

## Related

- [[fs-skia-skiaviewer]] provides the `ViewerModel`/`ViewerMsg` this adapter wraps.
- [[fs-skia-scene]] supplies the `SceneNode` the render function produces.

## Sources / links

- F#/.NET docs: https://learn.microsoft.com/en-us/dotnet/fsharp/
- Fable.Elmish (the Elmish architecture this adapter follows): https://elmish.github.io/elmish/
