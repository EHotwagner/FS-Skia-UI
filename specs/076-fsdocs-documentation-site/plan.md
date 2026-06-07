# Implementation Plan: FsDocs Documentation Site on GitHub Pages

**Branch**: `076-fsdocs-documentation-site` | **Date**: 2026-06-07 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/076-fsdocs-documentation-site/spec.md`

## Summary

Stand up a single FSharp.Formatting (`fsdocs`) static site that combines (1) a
**generated API reference** for the published packages, sourced from XML doc
comments, and (2) **authored technical/architecture documentation** with one
page per major system part, each closing with an honest
strengths/weaknesses + pros/cons analysis. Two subsystems get deep, dedicated
treatment with explicit speckit-phase placement: the **governance system** and
the **newer typed control design with Penpot / design-token integration**. Key
consumer workflows are taught with **executable literate `.fsx` examples**
evaluated at build. The site publishes to **GitHub Pages via a GitHub Actions
workflow** that regenerates from source on push (no committed generated output).

Technical approach: bootstrap fsdocs with the project's `fsdocs-*` skills
(`fsdocs-setup` → `fsdocs-api-doc` → `fsdocs-technical`/`fsdocs-examples` →
`fsdocs-build`); add `///` doc comments **on the `.fsi` signature files** (F#'s
emission rule for signatured public modules — confirmed by existing `///` in
`KeyboardInput.fsi`/`Testing.fsi`); rely on the surface-baseline normalizer
(`PerPackageSurface.fs` strips `///`/`//` comments) to keep FR-004 invariant
(no contract/baseline drift); and reuse the rich existing source material under
`docs/adr/**` and `docs/reports/**` as the grounding for the authored analyses.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (existing repo target)
**Primary Dependencies**: FSharp.Formatting (`fsdocs-tool`), pinned via
`.config/dotnet-tools.json` (build-time tool, **not** a runtime/package
reference, **not** in `Directory.Packages.props`). `GenerateDocumentationFile=true`
is already set globally in `Directory.Build.props`, so XML doc emission is on.
**Testing**: `dotnet fsdocs build --strict` (treats warnings as errors — fails on
broken literate `.fsx` evaluation and, with `FsDocsWarnOnMissingDocs`, on
undocumented supported members); surface-baseline gates
(`PackageSurfaceCheck` / `PerPackageSurfaceDiff`) to prove FR-004; the
authoritative gate set comes from `./fake.sh build -t Route` for the actual diff.
**Target Platform**: docs build on Linux and Windows; published to **GitHub
Pages** (`https://ehotwagner.github.io/FS-Skia-UI/`) by a GitHub Actions workflow.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

This is a **documentation + documentation-build-config** change. It creates **no
new public runtime surface**, so Principle I's "sketch the `.fsi` in FSI first"
does not apply (there is no new contract to draft); the relevant invariant is the
inverse — the public surface MUST be unchanged (FR-004). Principle IV (MVU
boundary) does not apply (no stateful/I/O workflow). Principle V (synthetic
disclosure) is not expected to trigger: literate examples are real and
build-evaluated, and the closing analyses are authored opinion grounded in real
ADRs/reports, not synthetic fixtures.

### Repository Governance Decisions

- **Template ownership**: **N/A — no template change.** This feature documents the
  framework repository itself; it does not touch `template/**` or
  `.template.config/template.json`, and shipping fsdocs into *generated* products
  is explicitly out of scope (spec "Unsupported scope"). No template, sample,
  Spec Kit asset, package-policy, or command-surface template update is required.
- **Dependency impact**: Adds `fsdocs-tool` to `.config/dotnet-tools.json` (local
  tool manifest) as the reproducibility pin. It is a **build-time tool, not a
  product package reference**, so `Directory.Packages.props`, the generated
  template's package inclusion, and `DependencyReport` coverage (which track
  *product* package refs) are **unchanged**. `docs/dependencies.md` MAY gain a
  one-line note that fsdocs-tool is a doc-build tool; no `Directory.Packages.props`
  entry is added. **Need**: generate the API reference + render the
  authored/literate docs (FR-001/FR-002/FR-017). **Pinning strategy**: exact
  version in `.config/dotnet-tools.json`, restored via `dotnet tool restore`.
  **Maintenance owner**: the repository maintainers (docs build tooling), bumped
  alongside other build-tool pins.
- **Command-surface impact**: Adds a documentation build path (`dotnet fsdocs
  build [--strict]`) and a GitHub Actions **Pages** workflow
  (`.github/workflows/docs.yml`, new — distinct from the existing nuget
  `publish.yml`). Whether to also add a FAKE `Docs` gate / target to `build.fsx`
  so the docs build is routable is an **open decision resolved in research.md**
  (default lean: a thin `Docs` target wrapping the fsdocs CLI so `Route` can gate
  it; if added, `build.fsx`/`validation.contract.yml`/`TargetMetadata` move
  together and the change escalates to `build-target-contract`). No behavior
  change to `Dev`, `Verify`, `Ci`, `TemplateCheck`, `GeneratedGuidanceCheck`,
  `EvidenceGraph`, or `EvidenceAudit`. FAKE-backed commands remain
  sequential/non-concurrent. The authoritative tier + minimal gate list MUST be
  taken from `./fake.sh build -t Route` for the actual diff (docs-only edits route
  to the `docs-only` focused rule → `EvidenceGraph`; `src/**/*.fs` doc comments
  route to inner-loop `Dev`; `.fsi` doc comments route to `package-surface`
  → `PackageSurfaceCheck`/`FsiTranscripts`/`PerPackageSurfaceDiff`).
- **Generated project impact**: **N/A — generated products are unaffected.** No
  change to default/minimal generated contents, selected-Controls guidance, local
  skills, validation logs, placeholder/excluded-history scans, or generated `Dev`
  behavior. The feature documents the framework, it does not alter what
  `dotnet new fs-skia-ui` emits.
- **Evidence paths**: Stored under `specs/076-fsdocs-documentation-site/readiness/`:
  - `readiness/logs/fsdocs-build.txt` — local `dotnet fsdocs build --strict` log
    (complete static site, literate `.fsx` evaluated) → SC-005, SC-009.
  - `readiness/logs/route.txt` — `./fake.sh build -t Route` (and `--enforce`)
    output for the actual diff → required by every escalated rule.
  - `readiness/surface-baseline-unchanged.md` — `PackageSurfaceCheck` /
    `PerPackageSurfaceDiff` output proving baselines did not move → SC-007 / FR-004.
  - `readiness/logs/pages-deploy.txt` (or the workflow run URL) — GitHub Pages
    publish evidence → SC-005, SC-006.
  - `readiness/validation-contract.md` — required by the `docs-only` focused rule.
  - `readiness/api-coverage.md` — supported-member doc-coverage summary (zero empty
    stubs) → SC-001 / FR-003.
- **`.fsi` / contract impact**: Doc comments are added **on `.fsi` signature
  files** (required: for a module with a signature file the F# compiler emits XML
  docs from the *signature*, not the implementation). This changes `.fsi` *file
  text* but **not** any signature shape (names, arity, types) and **not** the
  surface baseline: `PerPackageSurface.fs` strips `///`/`//` comments before
  hashing, so `PackageSurfaceCheck`/`PerPackageSurfaceDiff` remain green. This is
  the FR-004 invariant and is verified, not assumed. No compatibility note or
  migration guidance is needed (no consumer-visible contract change).
- **MVU/effect boundary**: **N/A — no stateful or I/O-bearing work.** The feature
  is documentation and doc-build configuration; there is no `Model`/`Msg`/`Effect`
  to model. (The *content* explains the framework's own MVU surfaces; it does not
  introduce one.)
- **Synthetic evidence**: **None expected.** Literate `.fsx` examples are real and
  evaluated by fsdocs at build (a broken example fails the build, SC-009); closing
  analyses are authored, opinionated assessments grounded in real `docs/adr/**`
  and `docs/reports/**`; any embedded visuals follow evidence-mode rules
  (render-only, benign degradation, no fabrication — FR-015). No `[S]`/`[SEH]`
  task is anticipated; if an example must be stubbed (e.g. needs a GPU not present
  in CI), Principle V `[S]` disclosure applies and is recorded in the example and
  its task.
- **Test evidence**: The failing-first/quality signal is the **strict fsdocs
  build** — before doc comments exist, `FsDocsWarnOnMissingDocs` + `--strict`
  reports undocumented supported members (red); after, it passes (green). Literate
  examples compile/evaluate against the real packed API (SC-009). Surface-baseline
  gates confirm FR-004. Whether to add a lightweight **governance test** asserting
  every major technical doc contains a closing analysis section (SC-002 / FR-006)
  is an open decision in research.md (default lean: a small `Governance.Tests`
  check over the `docs/` Markdown set).
- **Observability**: `dotnet fsdocs build` errors/warnings are captured to
  `readiness/logs/fsdocs-build.txt`; the Pages workflow surfaces build failures in
  CI (and dumps the build log on failure, mirroring `publish.yml`). Missing-doc
  diagnostics come from `FsDocsWarnOnMissingDocs`; malformed-doc diagnostics from
  FS3390 (note: global `TreatWarningsAsErrors=true` makes malformed `///` a build
  error — a quality benefit and a watch-item, see research.md).
- **Deferred scope**: Per spec "Unsupported scope" — **out of scope this feature**:
  changing any public API signature to improve documentability (recorded as a
  follow-up, never made here); localized/translated docs; versioned/multi-release
  doc hosting; changing package versions or publishing packages; any
  runtime/product behavior change; visual redesign of controls and net-new Penpot
  tooling (the feature documents the *existing* design).

**Initial Constitution Check: PASS** (no violations; no complexity-justification
entries required — the feature uses plain Markdown/`.fsx`/`.fsi` doc comments and
the standard fsdocs toolchain).

## Project Structure

Documentation content is authored under `docs/` (consumed by fsdocs); API doc
comments are edited in-place on the published packages' `.fsi` files; the publish
path is a GitHub Actions workflow. New/changed paths for this feature:

```
.config/dotnet-tools.json            # + fsdocs-tool pin (reproducibility contract) — single fsdocs site (FR-001), buildable locally (FR-013)
Directory.Build.props                # already has GenerateDocumentationFile=true;
                                     #   add FsDocs* props (RepositoryUrl/root/theme,
                                     #   WarnOnMissingDocs) — merge, do not overwrite
.gitignore                           # + output/ .fsdocs/ tmp/  (generated, never committed)
.github/workflows/docs.yml           # NEW: build fsdocs + deploy to GitHub Pages on push (FR-012)

docs/
  index.md                           # landing page + role-based navigation (FR-016, SC-008)
  _template.html                     # only if default chrome is insufficient (likely skip)
  architecture/                      # one page per major part, each ends with analysis (FR-005/FR-006); links to API entries (FR-011)
    host-skiaviewer.md               #   rendering / host
    scene.md                         #   scene primitives + vocabulary
    layout.md                        #   Yoga-backed layout
    input.md                         #   Input + KeyboardInput
    elmish-mvu.md                    #   Elmish / MVU runtime
    controls.md                      #   Controls + Controls.Elmish suite
    testing-skillsupport.md          #   Testing + SkillSupport
    governance.md                    #   build/governance front-end (overview; deep page below)
  governance/                        # DEEP dive (US3, FR-007/FR-008, SC-003): routing/tier+gate, evidence/audit,
    index.md                         #   single-source generation, speckit-phase mapping,
    routing-and-gates.md             #   usage guidance, closing analysis
    evidence-and-audit.md
    single-source-generation.md
    speckit-placement.md             # where each governance touchpoint applies in speckit
  controls-design/                   # DEEP dive (US4, FR-009/FR-010, SC-004): typed Props/MVU front door + Penpot
    typed-front-door.md              #   design-token flow source -> typed control surface
    design-tokens-penpot.md          #   how to author; speckit phase placement; closing analysis
  speckit/                           # the speckit process + where custom FS Skia UI
    process.md                       #   components are created and consumed (FR-010)
  examples/                          # literate, build-evaluated .fsx (FR-017, SC-009)
    typed-control-mvu.fsx            #   typed control / MVU front door (required)
    design-token-flow.fsx            #   design-token flow (required)
  img/                               # any evidence-mode visuals (FR-015)

src/**/**.fsi                        # + /// XML doc comments on supported public members
                                     #   (FR-002/FR-003); internal/unsupported excluded (FR-014);
                                     #   strippable -> baselines unchanged
```

The exact per-part document list (and whether `architecture/governance.md`
collapses into the `governance/` deep section) is finalized in Phase 1
`contracts/` and may be trimmed to match the actual supported surface; the set
above satisfies "one page per major part" (FR-005 / SC-002 across host, scene,
layout, input, Elmish/MVU, controls, testing, governance).

## Phase 0 — Research

See [research.md](./research.md). Resolves: (R1) where `///` doc comments must
live given `.fsi` files and whether that perturbs the baseline; (R2) GitHub
Pages base-path / `RepositoryUrl` configuration and the `RepositoryUrl` mismatch
in `Directory.Build.props`; (R3) the GitHub Actions Pages deploy pattern
(actions/deploy-pages) and headless-graphics needs for any evaluated example that
touches rendering; (R4) whether to add a FAKE `Docs` target vs. CLI-only and
whether to add a governance check for the analysis-section requirement;
(R5) `--strict` + `TreatWarningsAsErrors` + FS3390 interaction; (R6) the major-part
document inventory mapped to the supported surface.

## Phase 1 — Design & Contracts

Outputs: [data-model.md](./data-model.md) (the documentation-site information
model — site, page sets, API reference, examples, publish path), `contracts/`
(the docs-site structure contract: required pages, the analysis-section contract,
the API-coverage contract, and the build/publish contract), and
[quickstart.md](./quickstart.md) (local build/preview + publish runbook). Agent
context (`AGENTS.md` SPECKIT marker) is updated to point at this plan.

## Phase 2 — (planning ends here)

`/speckit.tasks` will break the above into story-grouped tasks with `skillist`
metadata (the `fsdocs-*` skills per task) per the Task gate.
