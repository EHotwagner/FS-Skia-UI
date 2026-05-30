# Implementation Plan: Package API Discovery And Name Safety

**Branch**: `035-api-discovery-names` | **Date**: 2026-05-30 | **Spec**: `specs/035-api-discovery-names/spec.md`
**Input**: Feature specification from `/specs/035-api-discovery-names/spec.md`

## Summary

Make packaged FS.Skia.UI consumption discoverable without repository source
inspection or assembly reflection, and make mixed `FS.Skia.UI.Scene` plus
`FS.Skia.UI.Controls` authoring stable when public names overlap. The
implementation will add source-shaped package API reference material generated
from curated `.fsi` signatures, generated-product guidance that points agents
to that reference before coding, a collision inventory with either
contract-level qualification decisions or explicit consumer guidance, and clean
package-consumer validation that exercises Scene primitives, `Paint` helpers,
viewer/geometry records, and Controls-adjacent declarations.

This is a Tier 1 contracted governance/package change. It affects packaged
artifacts or package-adjacent reference files, public `.fsi` signatures only
where collision safety requires a contract change, generated template guidance,
package surface baselines, generated consumer validation, and readiness
evidence. Runtime rendering, screenshots, Vulkan, Skia behavior, state
workflow semantics, external documentation hosting, and package publishing are
out of scope.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for framework packages, generated
templates, API-reference generation, semantic tests, FSI transcripts, and FAKE
targets.

**Primary Dependencies**: Existing FS.Skia.UI packages (`Scene`, `SkiaViewer`,
`Elmish`, `KeyboardInput`, `Layout`, `Controls`, `Controls.Elmish`, `Testing`),
Expecto, FAKE, local NuGet packaging under `~/.local/share/nuget-local/`, and
Spec Kit evidence scripts. No new runtime dependency is planned. If a
documentation generator or parser dependency is added, it must be pinned in
`Directory.Packages.props`, documented in `docs/dependencies.md`, included in
template package governance where relevant, and covered by `DependencyReport`.

**Testing**: Expecto governance/semantic tests through `.fsi`, FSI transcripts
for public authoring examples, package surface tests, generated product
validation, clean local package-consumer restore/build tests, generated
guidance scans, and FAKE targets (`Dev`, `PackLocal`, `PackageSurfaceCheck`,
`FsiTranscripts`, `GeneratedGuidanceCheck`, `TemplateCheck`,
`GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`). Multiple
FAKE-backed commands must run sequentially because `.fake` state is shared.

**Target Platform**: Windows and Linux package consumers. Validation is
non-visual and compile/guidance focused; desktop host, Vulkan, Skia raster, and
screenshot availability do not gate this feature.

**Public Surface**: Start from all public `.fsi` signatures under `src/*`.
Additive contract changes are allowed only when the collision inventory proves
that qualification attributes, safer names, or explicit front doors are needed
for stable mixed Scene/Controls authoring. Candidate review points include
`src/Scene/Scene.fsi`, `src/Controls/*.fsi`, `src/SkiaViewer/SkiaViewer.fsi`,
`src/Testing/Testing.fsi`, `readiness/surface-baselines/*.txt`, and
`template/capabilities.yml`.

**Evidence Requirement**: Required readiness paths are:

- `specs/035-api-discovery-names/readiness/api-discovery.md`
- `specs/035-api-discovery-names/readiness/name-collision-safety.md`
- `specs/035-api-discovery-names/readiness/generated-consumer-validation.md`
- `specs/035-api-discovery-names/readiness/feedback-classification.md`
- `specs/035-api-discovery-names/readiness/package-reference-material.md`
- `specs/035-api-discovery-names/readiness/package-surface-baseline.md`
- `specs/035-api-discovery-names/readiness/evidence-graph.md`
- `specs/035-api-discovery-names/readiness/evidence-audit.md`

**Synthetic Evidence**: Synthetic positive package-consumer discovery is not
acceptable. Success requires generated reference material from real `.fsi`
signatures and a clean consumer project that restores packages and compiles
without reflection or repository source reads. Synthetic malformed guidance or
collision fixtures are allowed only for scanner/error-path tests with
Principle V disclosure and `[SEH]` task labeling.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Generated product docs and template guidance must
  point consumers to package API reference material and must include a stable
  Scene/Controls qualification rule. Update `.template.config/template.json`
  only if new files are included in generated projects or template package
  contents; otherwise update existing docs/fragments/scanners.
- **Dependency impact**: PASS with no planned new runtime dependency. Prefer a
  repository-owned `.fsi` source-shaped reference generator using existing F#
  tooling and plain text/Markdown output. Any new parser/docs dependency
  triggers package governance and `DependencyReport`.
- **Command-surface impact**: `PackLocal`, `PackageSurfaceCheck`,
  `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, and `EvidenceAudit` are in scope. `Dev` remains the default
  build/test target. `Verify`, `Ci`, `DependencyReport`, `TemplateDrift`, and
  target metadata change only if new dependency, target, or documented command
  surface is introduced.
- **Generated project impact**: Generated consumers must restore local NuGet
  packages, read packaged/package-adjacent API reference material, compile
  Scene plus Controls examples with explicit qualification where needed, and
  avoid copying `src/` implementation files or using reflection as an authoring
  substitute.
- **Evidence paths**: Required readiness files are listed in Technical Context.
  Package-reference evidence must cite the generated reference files or package
  entries, sampled symbol counts, clean consumer project path/logs, and absence
  of reflection/source inspection.
- **`.fsi` / contract impact**: Any public contract change starts in `.fsi`,
  then failing-first semantic/Fsi tests, then implementation, then surface
  baseline refresh. If `RequireQualifiedAccess` or safer front-door functions
  are selected for collision-prone names, compatibility notes and migration
  guidance are required.
- **MVU/effect boundary**: PASS. This feature is not changing stateful runtime
  workflows. Any I/O for reference generation, package inspection, or consumer
  validation belongs in FAKE scripts/tests at the edge, with structured reports
  written to readiness paths.
- **Synthetic evidence**: PASS with restrictions. Positive discovery and name
  safety evidence must be real package-consumer compile/guidance evidence.
  Synthetic scanner fixtures require `[SEH]` disclosure.
- **Test evidence**: Add failing-first tests for reference generation coverage,
  F# source-shaped names, packed reference inclusion or discoverability,
  generated guidance terms, mixed Scene/Controls compilation, no-reflection/no
  source-inspection validation, and feedback classification categories.
- **Observability**: Reports must include package id, package version, source
  `.fsi` path, reference output path or package entry, sampled symbol counts,
  skipped/unsupported symbol reasons, collision name, owner namespaces,
  decision, guidance text, consumer project/log path, and next action.
- **Deferred scope**: No external documentation hosting, NuGet publishing,
  broad API redesign, runtime behavior fixes, visual validation, or generated
  game demo work.

**Pre-design gate result**: PASS. The plan preserves `.fsi`-first contracts,
  avoids runtime workflow changes, requires real package-consumer evidence, and
  treats synthetic fixtures as scanner/error-path evidence only.

## Project Structure

```text
src/
  Scene/Scene.fsi                    # Source-shaped Scene/paint/geometry contracts
  Controls/*.fsi                     # Controls front doors and collision-prone names
  SkiaViewer/SkiaViewer.fsi          # Viewer authoring records/unions
  Testing/Testing.fsi                # Validators if exposed publicly
  */skill/SKILL.md                   # Capability-owned implementation guidance

scripts/
  refresh-surface-baselines.fsx      # Existing public surface baseline path
  build/*.fsx                        # Report and scanning helpers if extended

template/
  base/docs/product.md               # Generated consumer API map and qualification rule
  fragments/*/README.md              # Capability-specific guidance if needed
  capabilities.yml                   # Package/docs/surface metadata review

docs/
  generated-apps.md                  # Consumer authoring guidance
  controls.md                        # Controls authoring and mixed Scene guidance
  testing.md                         # Validation workflow guidance if updated

tests/
  Package.Tests/                     # Package/reference/surface checks
  Governance.Tests/                  # Guidance, collision, scanner tests
  Scene.Tests/                       # Source-shaped Scene authoring samples
  Controls.Tests/                    # Mixed Controls authoring samples
  Testing.Tests/                     # Public validator semantics if added

specs/035-api-discovery-names/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    source-shaped-api-reference-contract.md
    name-collision-safety-contract.md
    generated-guidance-contract.md
    feedback-classification-contract.md
    generated-consumer-validation-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/035-api-discovery-names/research.md`.
Key decisions:

- Generate package consumer reference from curated `.fsi` signatures rather
  than compiled reflection metadata so F# authoring names, union-case labels,
  record fields, module functions, parameter names, and XML documentation stay
  source-shaped.
- Make the reference discoverable from package artifacts or package-adjacent
  generated files produced by the packaging workflow, with one index per
  package and stable paths reported under readiness evidence.
- Keep collision remediation decision-based: prefer existing or additive
  contract-level qualification for discriminated unions or broad categories
  when compatible; otherwise require explicit namespace/module qualification in
  generated guidance and samples.
- Validate with a clean package consumer that opens Scene and Controls in more
  than one order or avoids order dependence through explicit qualification.
- Classify feedback into package documentation discoverability, public contract
  ergonomics, generated template workflow, or consumer authoring guidance, with
  a required evidence path and next action.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/035-api-discovery-names/research.md`
- `specs/035-api-discovery-names/data-model.md`
- `specs/035-api-discovery-names/contracts/source-shaped-api-reference-contract.md`
- `specs/035-api-discovery-names/contracts/name-collision-safety-contract.md`
- `specs/035-api-discovery-names/contracts/generated-guidance-contract.md`
- `specs/035-api-discovery-names/contracts/feedback-classification-contract.md`
- `specs/035-api-discovery-names/contracts/generated-consumer-validation-contract.md`
- `specs/035-api-discovery-names/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. Public contract changes
  are optional and must begin in `.fsi`; package-reference and guidance changes
  require failing-first scanner/consumer tests before implementation.
- **Visibility in `.fsi`**: PASS. Public exposure remains controlled by
  signatures. No top-level visibility modifiers are planned in `.fs`.
- **Idiomatic simplicity**: PASS. The design uses Markdown/reference records,
  plain parsers over curated signatures where possible, and deterministic
  reports. Reflection is explicitly not an authoring-discovery strategy.
- **MVU/effect boundary**: PASS. No runtime state workflow changes are planned.
  File/package I/O is limited to scripts/tests/FAKE edge commands.
- **Synthetic disclosure**: PASS with restrictions. Positive evidence is real
  package-consumer evidence; synthetic malformed fixtures remain `[SEH]` only.
- **Test evidence**: PASS. The quickstart names package, generated guidance,
  mixed namespace, and audit checks in deterministic sequential FAKE order.
- **Observability and safe failure**: PASS. Contracts require actionable
  reports with package id, symbol coverage, collisions, qualification decisions,
  consumer log paths, classification, and next action.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks with
`skillist` metadata, `.fsi`-first work for any public API changes, failing-first
tests, package-reference generation, guidance updates, generated consumer
validation, readiness evidence, and sequential FAKE-backed verification.
