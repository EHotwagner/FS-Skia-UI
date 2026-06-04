# Phase 0 Research: Skills Quality Uplift & Per-Phase Feedback Loop

All spec `NEEDS CLARIFICATION` were resolved in the spec's 2026-06-03
clarification session and by three planning decisions (library shape, gate home,
seed scope). This file records the design resolutions those answers imply.

## D1 — Support-library shape and identity

- **Decision**: One new packable library `FS.Skia.UI.SkillSupport` with one
  module per `fsharp-*` skill family (`Graph`, `Parsing`, `Globbing`, `CodeGen`,
  `ShellProcess`; further family homes added as helpers materialize). It becomes
  the **10th** entry in `PerPackageSurface.packagesInScope`.
- **Rationale**: Matches the spec's "organized per skill family *within* the
  shipped library." One package = one pin, one surface baseline, one template
  include — the smallest governance footprint. Mirrors how `FS.Skia.UI.Build`
  already ships to the template as a non-profile, build/authoring-scoped package.
- **Alternatives**: Per-family packages (N pins/baselines/includes — rejected as
  disproportionate footprint); folding helpers into existing product packages
  (rejected — pollutes product surface with authoring helpers).

## D2 — Where the quality bar is enforced

- **Decision**: A new dedicated `SkillQualityCheck` target backed by a pure
  `build/Governance/SkillQuality.fs(/.fsi)` module that parses each in-scope
  `SKILL.md` and reports missing required sections as `Findings`. Wired through a
  new `Routing.fs` rule on `.agents/skills/**` + `template/product-skills/**`,
  added to `AgentValidation.knownGates`, and auto-serialized into
  `validation.contract.yml`.
- **Rationale**: Clear single-owner gate, independent of the currency-only
  `SkillSyncCheck`; fails loud naming skill+section (FR-003, Principle VII);
  reuses the existing Findings/Target/Routing machinery. `SkillExamplesCheck` was
  retired in feature 044, so there is no content gate to resurrect — a fresh,
  rubric-driven gate is the clean path.
- **Alternatives**: Extend `GeneratedGuidanceCheck` (rejected — mixes spec/plan
  prose obligations with skill-file structure in one gate).

## D3 — How much real helper code seeds the library ("full extraction")

- **Decision**: Lift the genuinely reusable, dependency-light helpers already
  living in `build/Governance` into `SkillSupport`, and have `FS.Skia.UI.Build`
  consume them by `ProjectReference`. The shipped helpers are then the **same
  code** governance runs (true dogfood; no copy-paste drift).
- **Extraction boundary per family** (reusable core moves; governance-specific
  policy stays as a thin consumer in `build/Governance`):
  - **Graph** ← `Evidence/Graph.fs`: generic DAG core — Kahn topo sort (with the
    documented tie-break), 3-colour DFS cycle detection. *Stays in governance*:
    synthetic-propagation status rules (`[S]`/`[S*]`/`[SEH]` semantics) consume
    the moved `Graph` primitives.
  - **Parsing** ← `Evidence/TaskParser.fs`, `DepsParser.fs`, `StatusRegion.fs`:
    generic parsing utilities — typed YAML read (YamlDotNet), JSON read
    (STJ+FSharp.SystemTextJson), a regex line-grammar helper, status-region
    scanning. *Stays in governance*: the exact `tasks.md`/`tasks.deps.yml`
    grammars (governance-specific shapes) call the moved utilities.
  - **Globbing** ← `Routing.fs` glob→regex + `SkillTreeGen`/currency: fnmatch-style
    glob→regex (`**` crosses `/`, `*`/`?` within a segment), path normalization,
    file discovery, and DiffPlex-based generation-currency diff. *Stays in
    governance*: the Route rule path tables consume the moved matcher.
  - **CodeGen** ← `Evidence/Render.fs`, `ContractView.fs`: deterministic
    Mermaid `graph TD`, Markdown tables, stable ASCII dependency-tree connectors,
    count tables (StringBuilder; no quotations). *Stays in governance*: the
    specific document layouts call the moved builders.
  - **ShellProcess** ← `Front/BuildProcess.fs`: a generic process/git runner with
    captured stdout/stderr/exit-code. *Stays in governance*: target-specific
    invocations.
- **Rationale**: The user chose full extraction. It makes the skills' "library
  API + runnable example" real (examples exercise the shipped `.fsi`), eliminates
  duplication, and means the governance build is itself the library's largest
  consumer/test.
- **Parity guard**: Bodies move behind **unchanged-shape** signatures; existing
  `build/Governance` tests are **re-pointed** at the moved modules rather than
  rewritten, so any behavioral drift surfaces as a red existing test. New
  `tests/SkillSupport.Tests` add FsCheck property coverage on the public surface.
- **Alternatives**: Minimal seed (rejected by the user — would leave examples thin);
  scaffolding-only (rejected — would weaken SC-003/SC-004).

## D4 — `fs-skia-*` skills' "driven library"

- **Decision**: The `fs-skia-*` capability and `product-skills/` skills cite the
  **existing product packages** (Scene, SkiaViewer, Elmish, KeyboardInput, Layout,
  Controls, Testing) and their already-shipped `.fsi` as their driven-library API;
  they do **not** depend on `SkillSupport`. `SkillSupport` backs only the
  `fsharp-*` authoring families.
- **Rationale**: The `fs-skia-*` skills already drive shipped product surfaces with
  governed baselines; FR-002/FR-009 are satisfied by pointing at those. This keeps
  `SkillSupport` scoped to the authoring families that today have only the
  (unshipped) `build/Governance` as backing.

## D5 — Per-phase feedback delivery mechanism

- **Decision**: Deliver via the existing Spec Kit `after_*` hook surface in the
  generated project's `.specify/extensions.yml`. A new authoring command skill
  `fs-skia-feedback-capture` is invoked by `after_specify`, `after_clarify`,
  `after_plan`, `after_tasks`, `after_analyze`, `after_implement` entries. The
  command instructs the agent to surface the four prompts (4th = skill-gaps, added by 061) (FR-013) with the phase
  name substituted, and to write one dated, phase-identified record under
  `specs/<feature>/feedback/`.
- **Gating**: The hook entries + the shipped command skill live entirely in the
  template's `feedback == true` conditional branch (template `#if`/conditional
  `sources`). With `--feedback false` no entries, no command skill, no `feedback/`
  destination are emitted → output byte-identical to today (FR-012/SC-006).
  Conditional inclusion is preferred over runtime `condition:` evaluation because
  it guarantees zero default-branch footprint and does not rely on the
  HookExecutor's condition engine.
- **Completion-only**: Records are written by the `after_*` hook, which fires only
  on phase completion; an aborted/failed phase runs no `after_*` hook and writes
  nothing (FR-016).
- **Rationale**: Reuses the exact surface the git/evidence hooks already use; no
  new bespoke runtime; no product-runtime impact.
- **Alternatives**: A new standalone runtime (rejected — needless surface);
  runtime `condition: feedback == true` on always-present entries (rejected — risks
  a non-byte-identical default branch and depends on condition evaluation).

## D6 — Persistent-problem research mandate

- **Decision**: Every in-scope `fsharp-*` (and `fs-skia-*`) skill carries a
  "Persistent problems" section stating that when a problem outlasts reasonable
  in-repo attempts, extensive external research is **mandatory** — **official
  online docs first** (F#/.NET docs; the driven library's own docs/API reference),
  then community sources (forums, Reddit, Q&A sites, issue trackers/changelogs) —
  and naming where findings + resolving links are recorded (the feature's
  `feedback/` folder and, for durable lessons, the skill's `Sources` line).
- **Offline degradation**: The mandate degrades to recording "research was blocked
  and why" rather than hard-failing a phase (FR-018).

## D7 — Surface-baseline mechanism for the new package

- **Decision**: Register `FS.Skia.UI.SkillSupport` in
  `PerPackageSurface.packagesInScope` + `packageSourceDir`, and add
  `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt`
  (governed by `PerPackageSurfaceDiff`, satisfying FR-010). If the package is also
  registered as a `capabilities.yml` catalog entry, add the aggregate
  `readiness/surface-baselines/FS.Skia.UI.SkillSupport.txt` so `PackageSurfaceCheck`
  covers it too; otherwise `PerPackageSurfaceDiff` (enumeration-based) is the
  governing check. Confirm during implementation which checks enumerate the
  package and create exactly the baselines they require (never silently skip).
- **Rationale**: `PerPackageSurfaceDiff` discovers packages by enumerating
  `packagesInScope`, so registration there guarantees coverage; the aggregate
  check is catalog-driven and only applies if the package is cataloged.

## Open items carried into Phase 1

None blocking. The aggregate-vs-per-package baseline detail (D7) and the exact set
of family-home stubs vs fully-moved bodies (D3) are settled during implementation
against the live `build/Governance` code; the contracts in Phase 1 define the
target shapes.
