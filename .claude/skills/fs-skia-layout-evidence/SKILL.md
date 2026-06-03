---
name: fs-skia-layout-evidence
description: Work on generated game HUD readability, scene layout evidence, public contract guidance, and benign host warning classification.
---

# FS Skia Layout Evidence Capability

## Scope

Use this skill for tasks that change generated game HUD/status layout,
gameplay-region bounds, public scene layout evidence, generated layout
validation, public scene/host/update naming guidance, or readiness host warning
classification.

Such tasks must declare `fs-skia-layout-evidence` in `tasks.deps.yml` and mirror
it on the matching `tasks.md` line. Resolve this skill before implementation
starts and record the resolved path in the active feature's readiness evidence.

## Public Contract

Public evidence contracts must start in `.fsi` files before implementation.
Keep deterministic render metadata separate from readable layout proof:
readability evidence must report HUD region, gameplay region, text or entity
bounds, overlap status, measurement mode, unsupported reasons, and diagnostics.

Use explicit proof levels:

- `ReadableLayout` only when HUD region, gameplay region, HUD text bounds,
  gameplay entity bounds, and non-overlap diagnostics are present.
- `DeterministicRenderOnly` for hashes, scene metadata, or render readback that
  does not prove readability.
- `UnsupportedLayoutInspection` when host, font, or public API facts cannot be
  produced. Unsupported facts must be actionable and must not be converted into
  readability proof.

Public guidance must use `Product.Program.view` for the app scene,
`Product.Program.generatedHost` for the host value, and
`Product.Program.update` for reducer examples.

## Runnable example

Capture and assert scene layout bounds against the layout-evidence contract. The
driven surface is the Layout/Scene product packages plus the
`fs-skia-layout-evidence` capability; an evidence check confirms the HUD region
and gameplay entity bounds do not overlap:

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
| DeterministicRenderOnly -> failwith "render hash only; not readable-layout proof"
| _ -> failwith "unsupported layout inspection: facts not actionable"
```

## Build Commands

Prefer repository targets over ad-hoc command sequences:

- `./fake.sh build -t GeneratedGuidanceCheck`
- `./fake.sh build -t GeneratedProductCheck`
- `./fake.sh build -t TemplateCheck`
- `./fake.sh build -t PackageSurfaceCheck`
- `./fake.sh build -t EvidenceGraph`
- `./fake.sh build -t EvidenceAudit`
- `./fake.sh build -t Verify`

## Test Commands

Use the package or governance tests that match the touched surface:

- `dotnet test tests/Scene.Tests/Scene.Tests.fsproj`
- `dotnet test tests/Testing.Tests/Testing.Tests.fsproj`
- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj`
- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj`

## Generated Product

Generated game samples that claim readability must reserve a named HUD/status
region, keep active gameplay entities in the gameplay region, validate default
and constrained sizes, and fail when HUD/HUD or HUD/gameplay overlap is
detected. Unsupported host or font/layout facts must be explicit and must not be
reported as readable layout proof.

Once a HUD region is reserved, movement, wrapping, spawning, clamping,
collisions, and active entity bounds must use gameplay-region coordinates.

## Guidance

Generated and public docs must use app-owned names when showing consumer
signatures or tests:

- `Product.Program.view` for the scene-returning function.
- `Product.Program.generatedHost` for the generated host value.
- `Product.Program.update` for reducer tests or signatures.

## Validation

Prefer repository targets over ad-hoc command sequences:

- `dotnet test tests/Scene.Tests/Scene.Tests.fsproj`
- `dotnet test tests/Testing.Tests/Testing.Tests.fsproj`
- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj`
- `./fake.sh build -t GeneratedGuidanceCheck`
- `./fake.sh build -t GeneratedProductCheck`
- `./fake.sh build -t TemplateCheck`
- `./fake.sh build -t EvidenceGraph`
- `./fake.sh build -t EvidenceAudit`

## Evidence

Readiness evidence for this capability belongs under the active feature's
`readiness/` directory and must state whether it proves readable layout,
deterministic rendering only, or unsupported layout inspection.

Required readiness files for the Asteroids integration feedback feature are:

- `hud-layout-readability.md`
- `public-contract-guidance.md`
- `layout-evidence.md`
- `host-warning-classification.md`
- `generated-validation.md`
- `evidence-audit.md`

Warning classification evidence must preserve real `LaunchFailure`,
`RenderingFailure`, `LayoutFailure`, and `PackageFailure` diagnostics even when
known benign environment warnings are present.

Visual evidence honesty requires screenshot proof, rasterized scene proof,
layout readability proof, fallback classification, and unsupported proof to be
separate. Accepted visual proof names a decodable image, image dimensions,
non-trivial content, renderer mode, fallback classification, and unsupported
reason.
<!-- BEGIN GENERATED: gov/visual-proof-phrases -->
Exact visual proof rejection phrases for scans: metadata-only reports do not satisfy visual proof; 1x1 fallback images do not satisfy visual proof; layout-only bounds claims do not satisfy visual proof.
<!-- END GENERATED: gov/visual-proof-phrases -->

Asteroids feedback findings must be classified by owner (framework runtime,
generated template workflow, documentation discoverability, consumer authoring),
and host feedback must distinguish persistent-window blocking, display/session
availability, auto-close smoke needs, benign warning, blocking warning, deferred
warning, and name-collision guidance.
<!-- BEGIN GENERATED: gov/owner-phrases -->
Exact owner phrases for scans: framework runtime; generated template workflow; documentation discoverability; consumer authoring; persistent-window blocking; display/session availability; auto-close smoke; benign warning; blocking warning; deferred warning; name-collision guidance.
<!-- END GENERATED: gov/owner-phrases -->

## Package Boundary

Keep pure layout-evidence classifiers in Scene or Testing contracts. Do not move
viewer launch, filesystem, package restore, process, font host, or
window-system effects into pure validation helpers. When host warning
classification or evidence collection needs I/O, model the request and result
explicitly and keep execution at the interpreter or build-target edge.

## Related

- [[fs-skia-layout]]
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
