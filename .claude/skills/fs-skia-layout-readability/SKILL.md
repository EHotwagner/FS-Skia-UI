---
name: fs-skia-layout-readability
description: Work on generated game HUD/status layout readability, gameplay-region bounds, and public scene/host/update naming guidance.
---

# FS Skia Layout Readability Capability

## Scope

Use this skill for tasks that change generated game HUD/status layout,
gameplay-region bounds, readable-layout proof, generated layout validation, or
public scene/host/update naming guidance. This is the **layout-design** half of
the former `fs-skia-layout-evidence` skill; the deterministic-evidence-mode and
host-warning-classification half lives in `fs-skia-evidence-mode`.

Such tasks must declare `fs-skia-layout-readability` in `tasks.deps.yml` and
mirror it on the matching `tasks.md` line. Resolve this skill before
implementation starts and record the resolved path in the active feature's
readiness evidence.

## Public Contract

Public evidence contracts must start in `.fsi` files before implementation.
Readable-layout evidence must report HUD region, gameplay region, text or entity
bounds, and overlap status.

Use the `ReadableLayout` proof level only when HUD region, gameplay region, HUD
text bounds, gameplay entity bounds, and non-overlap diagnostics are all
present. When only render hashes or scene metadata exist — not readability —
defer to `fs-skia-evidence-mode` and its `DeterministicRenderOnly` level.

Public guidance must use app-owned names when showing consumer signatures or
tests:

- `Product.Program.view` for the scene-returning function.
- `Product.Program.generatedHost` for the generated host value.
- `Product.Program.update` for reducer tests or signatures.

## Runnable example

Capture and assert scene layout bounds against the readable-layout contract. The
driven surface is the Layout/Scene product packages; an evidence check confirms
the HUD region and gameplay entity bounds do not overlap:

```fsharp
open FS.Skia.UI.Scene
open FS.Skia.UI.Testing

// Render the app scene and capture readable-layout evidence.
let scene = Product.Program.view Product.Program.initialModel
let evidence = LayoutEvidence.capture scene

// Assert reserved HUD region and gameplay entity bounds, then prove non-overlap.
let hud = evidence |> LayoutEvidence.requireRegion "hud"
let gameplay = evidence |> LayoutEvidence.requireRegion "gameplay"
match LayoutEvidence.proofLevel evidence with
| ReadableLayout when not (Bounds.intersects hud gameplay) ->
    printfn "ReadableLayout proven: HUD %A clear of gameplay %A" hud gameplay
| _ -> failwith "not readable-layout proof; classify with fs-skia-evidence-mode"
```

## Build Commands

Prefer repository targets over ad-hoc command sequences:

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t EvidenceGraph
```

## Generated Product

Generated game samples that claim readability must reserve a named HUD/status
region, keep active gameplay entities in the gameplay region, validate default
and constrained sizes, and fail when HUD/HUD or HUD/gameplay overlap is
detected. Unsupported host or font/layout facts must be explicit and must not be
reported as readable layout proof.

Once a HUD region is reserved, movement, wrapping, spawning, clamping,
collisions, and active entity bounds must use gameplay-region coordinates.

## Package Boundary

Keep pure layout classifiers in Scene or Testing contracts. Do not move viewer
launch, filesystem, package restore, process, font host, or window-system
effects into pure layout helpers. When layout evidence collection needs I/O,
model the request and result explicitly and keep execution at the interpreter or
build-target edge.

## Related

- [[fs-skia-evidence-mode]]
- [[fs-skia-scene]]

## Sources / links

- F# / .NET documentation: https://learn.microsoft.com/en-us/dotnet/fsharp/
- Yoga layout reference (flexbox layout engine): https://www.yogalayout.dev/

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the F#/.NET docs and the driven
library's own documentation/API reference), then community sources (forums, Reddit, Q&A
sites, issue trackers and changelogs). Record the findings and resolving links in the
feature's `specs/<feature>/feedback/` folder and, for durable lessons, in this skill's
**Sources** line. Offline, the mandate degrades to recording "research blocked — <why>"
rather than hard-failing the phase.
