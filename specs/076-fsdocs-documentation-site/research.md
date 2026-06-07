# Phase 0 Research: FsDocs Documentation Site

All Technical-Context unknowns resolved below. Format per decision/rationale/alternatives.

## R1 — Where must `///` XML doc comments live (given `.fsi` files), and does that change the baseline?

**Decision**: Add `///` doc comments **on the `.fsi` signature files**, not the
`.fs` implementations.

**Rationale**: This repo mandates a curated `.fsi` for every public module
(Constitution II). For a module that has a signature file, the F# compiler emits
the public symbol's XML documentation from the **signature**, not the
implementation — `///` comments on `.fs` members that are also declared in the
`.fsi` are not emitted. Empirically, the repo already documents public surface in
`.fsi` (`src/Input/KeyboardInput.fsi`, `src/KeyboardInput/KeyboardInput.fsi`,
`src/Testing/Testing.fsi` already carry `///`). Critically, this does **not**
break FR-004: the surface-baseline normalizer `build/Governance/PerPackageSurface.fs`
(lines ~93–105) strips `///`/`//` line comments and `(* *)` blocks before hashing
the surface, so adding doc comments leaves `PackageSurfaceCheck` /
`PerPackageSurfaceDiff` green. Signature *shape* (names, arity, types) is
untouched, so there is no contract change — only doc text.

**Consequence for Route**: editing `.fsi` files routes to the `package-surface`
rule (`PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff`). The
plan's FR-004 evidence (`readiness/surface-baseline-unchanged.md`) is produced by
exactly these gates. The generic `fsdocs-api-doc` skill says "edits `.fs` source
files"; for **this** repo that guidance is overridden by the signature-file rule —
record this deviation when invoking the skill.

**Alternatives considered**: (a) comments on `.fs` — rejected: not emitted for
signatured public members, so the generated reference would be empty stubs
(fails SC-001). (b) Drop `.fsi` for documented modules — rejected: violates
Constitution II and changes the contract.

## R2 — GitHub Pages base path and the `RepositoryUrl` mismatch

**Decision**: Target Pages at `https://ehotwagner.github.io/FS-Skia-UI/` and pass
the project-pages **subpath** as the fsdocs site root so all links resolve under
`/FS-Skia-UI/`. Set `RepositoryUrl`/`PackageProjectUrl` used for fsdocs source
links to the **actual** remote (`https://github.com/EHotwagner/FS-Skia-UI`).

**Rationale**: The git remote is `github.com/EHotwagner/FS-Skia-UI`, but
`Directory.Build.props` currently declares `github.com/FS-Skia-UI/FS-Skia-UI`
(an aspirational org). A project Pages site is served from a subpath
(`/FS-Skia-UI/`), so the fsdocs build must be told its root (CLI:
`fsdocs build --strict --eval --properties Configuration=Release` plus the
`--parameters root <url>` / `<FsDocsRoot>` property, version-dependent — confirm
against the pinned tool's docs) or every absolute link 404s. The source-link
`RepositoryUrl` must match the host that actually serves the source, else
"edit/source" links break.

**Open sub-decision (flag to maintainer)**: whether to (i) change the
`Directory.Build.props` `RepositoryUrl`/`PackageProjectUrl` to the `EHotwagner`
remote, or (ii) keep the published-package URLs aspirational and override **only**
the fsdocs source-link/root via fsdocs-specific properties. (ii) is safer because
changing `PackageProjectUrl` touches packable metadata (pre-publish surface);
(i) is simpler but widens the diff. **Lean: (ii)** — set `FsDocs*`/root for the
docs build without disturbing package metadata. Confirm during setup.

**Alternatives considered**: user/organization Pages at the domain root
(`ehotwagner.github.io`) — rejected: that root is reserved for a single
`<user>.github.io` repo, not this project repo.

## R3 — GitHub Actions Pages deploy pattern; headless-graphics needs

**Decision**: New `.github/workflows/docs.yml` using the official Pages pipeline:
`actions/configure-pages` → build (`dotnet tool restore` → `dotnet fsdocs build
--strict`) → `actions/upload-pages-artifact` (the `output/` dir) →
`actions/deploy-pages`, with `permissions: { pages: write, id-token: write }` and
a `github-pages` environment. Trigger on push to `main` (+ `workflow_dispatch`).
Regenerates from source; commits no generated output (FR-012).

**Rationale**: This is the current, secret-less GitHub-recommended Pages flow and
mirrors the repo's existing OIDC-style `publish.yml` conventions
(`concurrency` group, environment gate). **Headless graphics**: only required if a
literate `.fsx` evaluated at build actually renders via Skia/Vulkan. The two
required examples (typed-control/MVU front door, design-token flow) are chosen to
exercise **pure model/props/lowering** code paths that do **not** require a GPU,
so the docs job needs no Xvfb/Vulkan stack. If a future example must render, add
the Xvfb + Mesa lavapipe steps from `publish.yml`, or mark that example `[S]`
(non-evaluated) under Principle V — but the two FR-017 minimums stay GPU-free.

**Alternatives considered**: push generated HTML to a `gh-pages` branch — rejected:
commits generated output (violates FR-012) and needs a long-lived token.

## R4 — FAKE `Docs` target vs. CLI-only; governance check for the analysis section

**Decision (docs build)**: Keep the docs build as the **fsdocs CLI** invoked by
the workflow and the quickstart; add a **thin FAKE `Docs` target** that shells the
same `dotnet fsdocs build --strict` so the build is *routable* and locally
uniform. Treat adding it as optional-but-preferred; if added it escalates to the
`build-target-contract` rule, so `build.fsx` + `validation.contract.yml` +
`TargetMetadata` move together (regenerate `validation.contract.yml` from
`Routing.fs`; `TargetMetadataDrift` enforces currency).

**Decision (analysis-section gate)**: Add a small **`Governance.Tests`** check
asserting every page under `docs/architecture/**` (and the two deep-dive section
indexes) contains a delineated closing analysis with both
strengths **and** weaknesses and design pros **and** cons (SC-002 / FR-006). This
turns the spec's "honest analysis" deliverable into an enforced, machine-checkable
invariant rather than reviewer vigilance.

**Rationale**: The project's whole governance philosophy is "rules live in
compiled F#, enforced by gates." A docs deliverable whose distinguishing value is
the analysis section should be gated the same way. A literal heading/keyword
detector (mirroring the existing `SkillQuality` rubric style) is sufficient and
cheap.

**Alternatives considered**: rely on review only — rejected: the spec elevates the
analysis to *the* core deliverable (US2/SC-002); ungated, it silently regresses.

## R5 — `--strict` + `TreatWarningsAsErrors` + FS3390 interaction

**Decision**: Build docs with `fsdocs build --strict` and enable
`FsDocsWarnOnMissingDocs`. Be aware that the repo's global
`TreatWarningsAsErrors=true` (`Directory.Build.props`) already promotes the
malformed-XML-doc warning **FS3390** to a hard **compile error** during the
`dotnet build` that precedes fsdocs.

**Rationale**: This is a quality benefit — a malformed `///` (bad tag, unmatched
element, dangling `cref`) fails the build, so the API reference can't ship broken
doc XML. The watch-item: as coverage is added across all packages, malformed
comments fail `dotnet build` *before* fsdocs runs, so authors must write
well-formed XML. `--strict` additionally fails on fsdocs-level warnings (broken
links, unevaluated examples), giving SC-009 its teeth. Capture both phases to
`readiness/logs/fsdocs-build.txt`.

**Alternatives considered**: relax `TreatWarningsAsErrors` for doc warnings —
rejected: weakens an existing repo-wide invariant for no benefit; well-formed XML
is the desired bar.

## R6 — Major-part document inventory mapped to the supported surface

**Decision**: One architecture page per published part, derived from `src/` and
the existing `docs/reports/**`:

| Part | Package(s) | Authored page | Primary source material |
|---|---|---|---|
| Rendering / Host | `SkiaViewer` | `architecture/host-skiaviewer.md` | ADR 0007 host-ownership; `reports/runtime-design.md`, `reports/architecture.md` |
| Scene | `Scene` | `architecture/scene.md` | ADR 0008 scene-vocabulary; `reports/architecture.md` |
| Layout | `Layout` | `architecture/layout.md` | `reports/architecture.md`; Yoga layout notes |
| Input | `Input`, `KeyboardInput` | `architecture/input.md` | spec 075 plan; input skills |
| Elmish / MVU | `Elmish` | `architecture/elmish-mvu.md` | `reports/runtime-design.md`; ADR 0005 |
| Controls suite | `Controls`, `Controls.Elmish` | `architecture/controls.md` | `reports/controls.md`, controls-boundary report |
| Testing / SkillSupport | `Testing`, `SkillSupport` | `architecture/testing-skillsupport.md` | `reports/evidence.md` |
| Build / Governance | `build/Governance/**` | `architecture/governance.md` + `governance/**` deep dive | `reports/2026-06-05-2237-governance-system-comprehensive-analysis.md`, `2026-06-03-2018-governance-system-agent-analysis.md`, ADR 0001/0002/0009 |
| Typed controls + Penpot | `Controls.Typed` (in `Controls`) | `controls-design/**` deep dive | `reports/2026-06-05-1421-controls-suite-and-penpot-integration-analysis.md`, `2026-06-05-1802-typed-controls-front-door-implementation-plan.md` |
| Speckit placement | (process) | `speckit/process.md` | `reports/speckit.md`, `reports/2026-06-03-2128-speckit-tasks-governance-process-analysis.md` |

**Rationale**: Mirrors the spec's "each part" assumption (host, scene, layout,
input, Elmish/MVU, controls, testing/SkillSupport, build/governance) and reuses
the substantial existing analysis reports as grounding for the closing analyses
(keeps them honest and non-fabricated). `Input` and `KeyboardInput` share one
page (both are input); `Controls` + `Controls.Elmish` share one (the suite). The
governance and typed-controls/Penpot parts get an overview architecture page
**plus** the dedicated deep-dive section the spec requires (US3/US4).

**Alternatives considered**: one page per package (10 pages) — rejected: splits
the input pair and the controls pair across pages that would duplicate context;
the part-based grouping reads better and still leaves no major part undocumented
(SC-002).
