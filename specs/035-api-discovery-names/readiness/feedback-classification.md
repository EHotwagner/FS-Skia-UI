# Feedback Classification Evidence

Status: pass.

Classification record schema:

- category:
- owner:
- contract-change:
- generated-guidance:
- runtime-scope:
- evidence-path:
- next-action:

| Feedback | Category | Owner | contract-change | generated-guidance | runtime-scope | evidence-path | next-action |
|----------|----------|-------|-----------------|--------------------|---------------|---------------|-------------|
| Agent had to use reflection to discover `Paint` and `TextRun` authoring shapes. | `PackageDocumentationDiscoverability` | package documentation / package reference material | false | true | false | `specs/035-api-discovery-names/readiness/package-reference-material.md` | Keep generated source-shaped API references current from curated `.fsi` files and point generated docs to them before coding. |
| Mixed `open FS.Skia.UI.Scene` and `open FS.Skia.UI.Controls` made `Changed` and text-related names unclear. | `PublicContractErgonomics` | public `.fsi` contract review | true | true | false | `specs/035-api-discovery-names/readiness/name-collision-safety.md` | Preserve existing `[<RequireQualifiedAccess>]` controls event/attribute contracts and use explicit qualification for remaining overlaps. |
| Generated product docs did not tell agents where the package reference material lives. | `GeneratedTemplateWorkflow` | template and generated product guidance | false | true | false | `template/base/docs/product.md` | Generated docs must name the source-shaped package API reference and forbid reflection/source inspection as authoring substitutes. |
| Local sample used unqualified `create`, `children`, and scene record names. | `ConsumerAuthoringGuidance` | consumer examples and product code | false | true | false | `specs/035-api-discovery-names/readiness/fsi/mixed-scene-controls-open-scene-first.fsx` | Qualify `FS.Skia.UI.Scene.*` records and `FS.Skia.UI.Controls.*` builder helpers in examples. |

Timed checklist transcript:

- started: 2026-05-30T11:24:00Z
- finished: 2026-05-30T11:28:12Z
- elapsed: 00:04:12
- representative items classified: 4
- result: pass

Checklist:

1. Identify whether the symptom is missing package reference material,
   collision-prone public naming, generated template workflow, or local
   consumer authoring.
2. Set `contract-change` only when a public `.fsi` decision is required.
3. Set `generated-guidance` when generated docs, examples, or template
   fragments need a wording or sample change.
4. Set `runtime-scope` only for rendering, viewer, input, process, or host
   behavior. All representative findings here are package/discovery guidance,
   so `runtime-scope` is false.
5. Link the concrete readiness or guidance path and record the next action.
