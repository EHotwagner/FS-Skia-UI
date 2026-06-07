# Contract: Documentation Site

The UI contract this feature exposes is a **published static site**. The
"interfaces" are the required pages, the structural guarantees a reader relies on,
and the build/publish behaviour. Each clause is checkable and maps to a
requirement + success criterion.

## C1 — Site composition (FR-001 / SC-005)

- The site MUST be produced by `dotnet fsdocs build` using the pinned
  `fsdocs-tool` from `.config/dotnet-tools.json`.
- The build output MUST be a single static site containing **both** a generated
  API reference and the authored technical content.
- The build MUST succeed with `--strict` (warnings = errors). Output goes to
  `output/` and MUST NOT be committed.

## C2 — API reference coverage (FR-002 / FR-003 / FR-014 / SC-001)

- Every **supported public member** (per the per-package surface baselines) of the
  10 published packages MUST render a non-empty `<summary>`.
- Zero supported public members render as empty stubs.
- Doc comments are authored on the member's **`.fsi`** declaration.
- Internal-only / unsupported members MUST NOT appear as supported reference.
- **Check**: `readiness/api-coverage.md` enumerates supported members and shows 0
  undocumented; `FsDocsWarnOnMissingDocs` + `--strict` fails on a missing summary.

## C3 — Contract preservation (FR-004 / SC-007)

- Adding doc comments MUST NOT change any `.fsi` signature shape or any surface
  baseline.
- **Check**: `PackageSurfaceCheck` and `PerPackageSurfaceDiff` report **no diff**
  after the doc work (`readiness/surface-baseline-unchanged.md`).

## C4 — Architecture pages + honest analysis (FR-005 / FR-006 / SC-002)

- There MUST be ≥1 architecture page per major part: host/SkiaViewer, scene,
  layout, input, Elmish/MVU, controls, testing/SkillSupport, build/governance.
- Each such page MUST end with a delineated **analysis** section that names
  **both** implementation strengths and weaknesses **and both** design pros and
  cons. A one-sided analysis fails the contract.
- **Check**: the analysis-section governance check (research.md R4) over
  `docs/architecture/**` and the deep-dive section indexes.

## C5 — Governance deep dive (FR-007 / FR-008 / SC-003)

- A dedicated governance section MUST explain routing/tier+gate selection, the
  evidence/audit model, and single-source generation.
- It MUST give **usage guidance** (how to run and respond) and MUST map each
  governance touchpoint to a **named speckit phase**.
- It MUST close with the C4 analysis.

## C6 — Typed-control / Penpot deep dive (FR-009 / FR-010 / SC-004)

- A dedicated section MUST explain the design-token flow from design source to the
  typed control surface and how to author against the typed Props/MVU front door.
- It MUST state how to use it and **where in speckit** it applies, and MUST
  explain the speckit process and the phase(s) where custom FS Skia UI components
  are **created and consumed**.
- It MUST close with the C4 analysis.

## C7 — Cross-linking (FR-011)

- An API reference entry MUST link to the architecture page for its subsystem, and
  architecture pages MUST link back to relevant API entries.
- **Check**: strict build resolves the links (no broken-link warning).

## C8 — Literate examples (FR-017 / SC-009)

- The site MUST include build-**evaluated** literate `.fsx` for the typed control /
  MVU front door and the design-token flow (minimum).
- A broken example MUST fail the build (no non-evaluated stand-in for these).

## C9 — Navigation (FR-016 / SC-008)

- The landing page MUST route consumer, contributor, and speckit-practitioner to
  their entry point in ≤ 2 steps.

## C10 — Publish path (FR-012 / FR-013 / SC-005 / SC-006)

- The site MUST build locally for preview (FR-013).
- Publishing MUST be a GitHub Actions workflow that builds fsdocs and deploys to
  GitHub Pages on push, regenerating from source; it MUST NOT serve committed
  generated output.
- Re-running the publish path after a content change MUST update the live site
  with no manual file manipulation.

## C11 — Evidence honesty (FR-015)

- Any embedded visual MUST follow evidence-mode rules: render-only, no fabricated
  visuals, benign degradation where rendering is unsupported.
