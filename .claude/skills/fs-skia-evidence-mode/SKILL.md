---
name: fs-skia-evidence-mode
description: Deterministic render-only evidence mode, visual-proof honesty, and benign/blocking host-warning classification.
---

# FS Skia Evidence Mode Capability

## Scope

Use this skill for tasks that change deterministic render-only evidence, visual
evidence honesty, fallback classification, or readiness host-warning
classification. This is the **evidence-discipline** half of the former
`fs-skia-layout-evidence` skill; the generated-HUD/layout-readability half lives
in `fs-skia-layout-readability`.

Such tasks must declare `fs-skia-evidence-mode` in `tasks.deps.yml` and mirror it
on the matching `tasks.md` line. Resolve this skill before implementation starts
and record the resolved path in the active feature's readiness evidence.

## Public Contract

Public evidence contracts must start in `.fsi` files before implementation. Keep
deterministic render metadata separate from readable layout proof. Use explicit
proof levels:

- `DeterministicRenderOnly` for hashes, scene metadata, or render readback that
  does not prove readability.
- `UnsupportedLayoutInspection` when host, font, or public API facts cannot be
  produced. Unsupported facts must be actionable and must not be converted into
  readability proof.

Readable-layout proof itself (the `ReadableLayout` level) belongs to
`fs-skia-layout-readability`; this skill governs what is **not** readable proof.

## Runnable example

Classify render evidence honestly: a render hash is deterministic-render-only and
must never be reported as readable layout.

```fsharp
open FS.Skia.UI.Scene
open FS.Skia.UI.Testing

let scene = Product.Program.view Product.Program.initialModel
let evidence = LayoutEvidence.capture scene

match LayoutEvidence.proofLevel evidence with
| DeterministicRenderOnly -> printfn "render hash only; not readable-layout proof"
| UnsupportedLayoutInspection -> failwith "unsupported layout inspection: facts not actionable"
| _ -> () // readable layout is proven and classified by fs-skia-layout-readability
```

## Build Commands

Prefer repository targets over ad-hoc command sequences:

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Evidence

Readiness evidence for this capability belongs under the active feature's
`readiness/` directory and must state whether it proves deterministic rendering
only or unsupported layout inspection.

Warning classification evidence must preserve real `LaunchFailure`,
`RenderingFailure`, `LayoutFailure`, and `PackageFailure` diagnostics even when
known benign environment warnings are present.

Visual evidence honesty requires screenshot proof, rasterized scene proof,
layout readability proof, fallback classification, and unsupported proof to be
separate. Accepted visual proof names a decodable image, image dimensions,
non-trivial content, renderer mode, fallback classification, and
unsupported reason.
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

Keep pure evidence classifiers in Scene or Testing contracts. Do not move viewer
launch, filesystem, package restore, process, font host, or window-system
effects into pure validation helpers. When host warning classification or
evidence collection needs I/O, model the request and result explicitly and keep
execution at the interpreter or build-target edge.

## Related

- [[fs-skia-layout-readability]]
- [[fs-skia-scene]]

## Sources / links

- F# / .NET documentation: https://learn.microsoft.com/en-us/dotnet/fsharp/
- SkiaSharp API reference: https://learn.microsoft.com/en-us/dotnet/api/skiasharp

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the F#/.NET docs and the driven
library's own documentation/API reference), then community sources (forums, Reddit, Q&A
sites, issue trackers and changelogs). Record the findings and resolving links in the
feature's `specs/<feature>/feedback/` folder and, for durable lessons, in this skill's
**Sources** line. Offline, the mandate degrades to recording "research blocked — <why>"
rather than hard-failing the phase.
