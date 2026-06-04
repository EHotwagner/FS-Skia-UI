# Implementation Plan: Skills Quality Uplift & Per-Phase Feedback Loop

**Branch**: `058-skills-quality-feedback` | **Date**: 2026-06-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/058-skills-quality-feedback/spec.md`

## Summary

Raise every FS-Skia-UI-authored skill (`fsharp-*`, `fs-skia-*`, template
`product-skills/`) to one section-rubric quality bar enforced by a new
`SkillQualityCheck` gate; give the `fsharp-*` skills a real, shipped backing
library by **extracting the reusable governance helpers from `build/Governance`
into a new packable `FS.Skia.UI.SkillSupport` library** (per-family modules,
`.fsi`-governed, consumed by `FS.Skia.UI.Build` itself so the shipped helpers are
the *same code* governance runs); and add an opt-in `dotnet new fs-skia-ui
--feedback` parameter that, when `true`, wires per-phase `after_*` feedback hooks
into the generated project so an agent records process friction, generalizable-code
candidates, and a severity signal into `specs/<feature>/feedback/` after each Spec
Kit phase. The `fs-skia-*` skills point at the existing product packages (Scene,
Layout, Controls, …) as their driven library; the five library-backed `fsharp-*`
families gain the new `SkillSupport` surface, while `fsharp-build-orchestration`
keeps `FS.Skia.UI.Build` (the build front-end) as its driven surface.

**Tier**: Tier 1 (contracted change) — new public `.fsi` surface, new package
identity, new template parameter, new gate. Route escalates to the
**maintainer-verify / full-pipeline** path (consumer-contract: `template/**`, new
`.fsi`, governance, `.agents/skills/**`, `.specify/**`).

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: existing — DiffPlex (currency/surface diff), YamlDotNet,
System.Text.Json (+ FSharp.SystemTextJson), Expecto + FsCheck (tests). No new
third-party dependency; `FS.Skia.UI.SkillSupport` is built from code already in
`build/Governance`.
**Testing**: Expecto + FsCheck unit/property tests against the packed/`.fsi`
surface (constitution Principle I — FSI-first), FAKE governance gates, template
generation evidence for both `--feedback` values.
**Target Platform**: Windows and Linux (governance/authoring scope; no runtime,
layout, or rendering change).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification**: Tier 1 (contracted). Adds public `.fsi` surface
(`FS.Skia.UI.SkillSupport`), new package identity + template pin, new template
parameter, new gate target. Full artifact chain required: spec, plan, `.fsi`
updates, surface-area baselines, test evidence, doc/skill updates.

**Principle gate review**:

- **I. Spec → FSI → Semantic Tests → Implementation**: The `SkillSupport`
  per-family modules are designed `.fsi`-first (Phase 1 `contracts/`), exercised
  in FSI/packed form by semantic tests, then implemented by *moving* the proven
  `build/Governance` bodies behind the new signatures. PASS-by-design.
- **II. Visibility lives in `.fsi`**: Every `SkillSupport` module ships a curated
  `.fsi`; no `private`/`internal`/`public` modifiers in `.fs`; per-package
  surface baseline added (FR-010). PASS-by-design.
- **III. Idiomatic simplicity**: Helpers are plain functions/records; the
  extraction introduces no SRTP, reflection, type providers, or custom operators.
  No justification block required.
- **IV. Elmish/MVU boundary**: **N/A to product runtime** — this feature adds no
  stateful/I-O product workflow. The only new "flow" is the authoring-time
  per-phase feedback prompt, delivered through the existing Spec Kit
  extensions/hooks surface (same mechanism the git/evidence hooks use), which is
  not product runtime and owns no `Model`/`Msg`/`Cmd`.
- **V. Synthetic evidence**: None planned. The skill-quality check runs against
  the *real* skill corpus; `SkillSupport` semantic tests run against the real
  packed/`.fsi` surface; template generation evidence is produced by real
  `dotnet new` runs for both `--feedback` values. No `[S]` tasks anticipated; if
  any arise they take the full Principle V disclosure.
- **VI. Test evidence mandatory**: New gate, new library surface, and template
  behavior each get failing-first semantic tests (skill corpus regression test,
  `SkillSupport` per-family tests, template-generation byte-identity + feedback
  presence tests).
- **VII. Observability / safe failure**: `SkillQualityCheck` fails loud, naming
  the offending skill and the missing section (FR-003); `PerPackageSurfaceDiff`
  never silently passes a missing baseline; the offline research mandate degrades
  to a recorded "research blocked + why" note rather than hard-failing a phase
  (FR-018).

### Repository Governance Decisions

- **Template ownership**: **Yes — `.template.config/template.json` changes.** A
  new boolean `feedback` parameter (`datatype: bool`, `defaultValue: false`) is
  added to `symbols`; conditional `sources`/content gate the per-phase feedback
  hooks on `feedback == true`. The new `FS.Skia.UI.SkillSupport` package ships
  unconditionally in `template/base` (same shipping pattern as
  `FS.Skia.UI.Build`), so `template/base/Directory.Packages.props` gains its pin
  and `template/capabilities.yml` is updated if the package is registered as a
  catalog entry. `fs-skia-*` `product-skills/` are rewritten to the quality bar.
- **Dependency impact**: **No new third-party dependency.**
  `Directory.Packages.props` (repo) gains the `FS.Skia.UI.SkillSupport`
  project/package wiring; `template/base/Directory.Packages.props` gains its pin.
  `FS.Skia.UI.Build` adds a `ProjectReference` to `src/SkillSupport` (it consumes
  the extracted helpers — no version circularity because it references the
  project, not the packed nupkg). `docs/dependencies.md` /`DependencyReport`
  updated to list the new first-party package. Extraction reuses already-pinned
  DiffPlex/YamlDotNet/STJ.
- **Command-surface impact**: **Yes.** New `SkillQualityCheck` target added to
  the `Targets.Target` union + metadata + a `Routing.fs` rule matching every
  in-scope skill home — `.agents/skills/**`, `src/**/skill/SKILL.md`,
  `template/product-skills/**`, `template/fragments/**/skill/SKILL.md`, and
  `template/base/.agents/skills/**` (the vendored `speckit-*` tree excluded);
  added to `AgentValidation.knownGates`;
  auto-serialized into `validation.contract.yml` via `RefreshSurfaceBaselines` →
  `ContractView.render` (currency enforced by `TargetMetadataDrift`).
  `PerPackageSurfaceDiff` gains the new package via `packagesInScope`. FAKE-backed
  commands share `.fake` state — run sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  (plus `Route`, `RefreshSurfaceBaselines`, `PerPackageSurfaceDiff`, `PackLocal`).
- **Generated project impact**: **Yes.** Generated projects gain the shipped
  `FS.Skia.UI.SkillSupport` package + its `.fsi`, the rewritten product skills,
  **and the five library-backed `fsharp-*` skills (`fsharp-graph-algorithms`,
  `fsharp-parsing`, `fsharp-io-globbing`, `fsharp-code-generation`,
  `fsharp-shell-process`) shipped unconditionally into
  `template/base/.agents/skills/**` (mirrored to `.claude` via generation) so
  their `SkillSupport` `.fsi` references resolve in-project (SC-004/US2-AC2)**;
  `fsharp-build-orchestration` stays repo-only (it drives the repo build
  front-end). Only under `--feedback true` do `after_*` feedback hooks, the
  `fs-skia-feedback-capture` command skill, and a `specs/<feature>/feedback/`
  destination appear; with `--feedback false` the `feedback` flag induces no
  diff (FR-012/SC-006): the feedback hooks/command/skill live entirely in the
  `feedback == true` conditional branch and the false branch emits no
  markers/whitespace.
- **Evidence paths** (under `specs/058-skills-quality-feedback/readiness/` unless
  noted):
  - `skill-quality-check.md` — `SkillQualityCheck` PASS over the full in-scope set
    + a demonstrated FAIL naming skill+section.
  - `skill-loading-evidence.md` — pre-task skill-loading record (ISO-8601 stamps).
  - `skill-sync.md` — `.agents` → `.claude` currency (no `SkillSyncCheck` drift).
  - `surface-baseline.md` + `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt`
    (and `readiness/surface-baselines/FS.Skia.UI.SkillSupport.txt` if catalog-registered).
  - `template-feedback-false.md` / `template-feedback-true.md` — generation
    evidence for both parameter values (default byte-identity; `true` produces the
    four prompts (4th = skill-gaps, added by 061) wiring + `feedback/` folder).
  - `support-library-tests.md` — `SkillSupport` semantic/property test transcript.
  - `feedback-record-example.md` — one worked feedback entry routing a
    generalizable-code candidate toward `SkillSupport` (SC-007).
  - `target-metadata.md`, `agent-ready-verdict.md` — required by the
    maintainer-verify Route path.
  - `aggregate-hang-diagnostics.md` — required by `EvidenceAudit` readiness.
- **`.fsi` / contract impact**: **Yes.** New public `.fsi` for each
  `SkillSupport` family module (`Graph`, `Parsing`, `Globbing`, `CodeGen`,
  `ShellProcess`, plus any further family homes). Per-package surface baseline
  added and governed by `PerPackageSurfaceDiff` (FR-010). **No existing public
  `.fsi` signature is altered**; `build/Governance` modules that move their bodies
  keep their own `.fsi` and delegate to `SkillSupport`. Compatibility note:
  additive only.
- **MVU/effect boundary**: **N/A.** No stateful/I-O product workflow. The feedback
  prompt is authoring-time agent guidance via Spec Kit hooks, with no `Model`,
  `Msg`, `Effect`/`Cmd`, `init`, or interpreter. Recorded here as N/A with
  rationale.
- **Synthetic evidence**: **None planned.** All evidence is real (real skill
  corpus, real packed surface, real `dotnet new` runs). If a genuinely infeasible
  real path appears, it takes `[S]`/`[SEH]` with full Principle V disclosure;
  none is anticipated.
- **Test evidence**: Failing-first semantic tests — (a) `SkillQualityCheck`
  corpus test: red on a deliberately-thinned skill, green when all required
  sections present, fails naming skill+section; (b) `SkillSupport` per-family
  unit/property tests against the `.fsi`/packed surface (parity-preserving since
  bodies are moved, not rewritten — existing `build/Governance` tests are
  re-pointed at the new modules); (c) template-generation tests: `--feedback
  false` byte-identity vs current output, `--feedback true` presence of the three
  prompts + `feedback/` wiring + aborted-phase writes nothing.
- **Observability**: `SkillQualityCheck` emits a Markdown report naming each
  checked skill and, on failure, the exact missing section per skill;
  `PerPackageSurfaceDiff` reports added/removed `.fsi` lines and never silently
  passes a missing baseline; the offline research mandate records "research
  blocked + why" rather than failing (FR-018). Report paths under `readiness/`.
- **Deferred scope**: Out of scope / deferred — rewriting the vendored `speckit-*`
  command skills (FR-004); any visual/screenshot/rendering/Vulkan/Skia change; any
  release/distribution/platform/roadmap change; any product runtime behavior
  change; building an automated online-research scraper/CI (only the documented
  *mandate* + manual link capture, FR-017). Dogfooding `--feedback` inside *this*
  repo's own `.specify/extensions.yml` is an optional bounded follow-up, not a
  blocker for the deliverable.

## Project Structure

```
src/
  SkillSupport/                         # NEW packable library — FS.Skia.UI.SkillSupport
    SkillSupport.fsproj
    Graph.fsi / Graph.fs                # fsharp-graph-algorithms family (from Evidence/Graph)
    Parsing.fsi / Parsing.fs            # fsharp-parsing family (from Evidence/TaskParser, DepsParser, StatusRegion helpers)
    Globbing.fsi / Globbing.fs          # fsharp-io-globbing family (glob→regex from Routing; currency diff)
    CodeGen.fsi / CodeGen.fs            # fsharp-code-generation family (Mermaid/Markdown/ASCII/count-table builders)
    ShellProcess.fsi / ShellProcess.fs  # fsharp-shell-process family (process/git wrappers from Front/BuildProcess)
    skill/SKILL.md                      # (optional) package-owned skill, if registered as a capability

build/Governance/                       # consumes FS.Skia.UI.SkillSupport (ProjectReference)
  SkillQuality.fsi / SkillQuality.fs    # NEW — parse SKILL.md, check required sections → Findings
  Targets.fs / Targets.fsi              # + SkillQualityCheck variant, deps, metadata
  Routing.fs                            # + rule: .agents/skills/** + template/product-skills/** → SkillQualityCheck
  AgentValidation.fs                    # + "SkillQualityCheck" in knownGates
  PerPackageSurface.fs                  # + "FS.Skia.UI.SkillSupport" in packagesInScope + packageSourceDir
  Evidence/Graph.fs, TaskParser.fs, ...# bodies moved to SkillSupport; thin governance-specific consumers remain
  Engine/Update.fs, Interpret.fs       # + SkillQualityCheck dispatch + scan effect

tests/
  SkillSupport.Tests/                   # NEW — per-family semantic/property tests against the .fsi surface
  (existing build/Governance tests re-pointed at SkillSupport where bodies moved)

.agents/skills/                         # CANONICAL skill tree (edits here; .claude generated)
  fsharp-*/SKILL.md                     # already rich — confirm/normalize to the rubric, point at SkillSupport .fsi
  fs-skia-layout-evidence/SKILL.md      # raised to bar (API = product package surface, ≥1 example, ≥2 links, sources)
  fs-skia-template-update/SKILL.md      # raised to bar
  fs-skia-feedback-capture/SKILL.md     # NEW authoring command skill invoked by after_* feedback hooks
.claude/skills/                         # GENERATED via RefreshSurfaceBaselines (no hand-sync)

src/*/skill/SKILL.md                    # 7 package-owned product capability skills raised to the bar (in scope)

template/
  product-skills/fs-skia-*/SKILL.md     # 6 product skills raised to the bar
  fragments/samples/skill/SKILL.md      # fs-skia-samples raised to the bar (in scope)
  base/Directory.Packages.props         # + FS.Skia.UI.SkillSupport pin
  base/.specify/extensions.yml          # + #if(feedback) after_* feedback hook entries (false branch byte-identical)
  base/.agents/skills/fsharp-*/         # 5 library-backed fsharp-* skills shipped unconditionally (C1; .claude mirrored)
  base/.agents/skills/fs-skia-project/  # repo-authored shipped skill raised to the bar (in scope)
  base/.agents/skills/fs-skia-feedback-capture/  # shipped command skill (feedback branch)
  capabilities.yml                      # + SkillSupport entry if catalog-registered
.template.config/template.json          # + feedback bool symbol + conditional sources

.specify/templates/, .specify/presets/  # plan/spec guidance unchanged unless a new obligation is added
```

## Phase Plan (high level)

- **Phase 0 — research.md**: resolve the extraction boundary (what is reusable vs
  governance-specific per family), the feedback hook delivery mechanism, and the
  byte-identity strategy for `--feedback false`.
- **Phase 1 — contracts/, data-model.md, quickstart.md**: the skill-quality
  rubric contract, the `SkillSupport` per-family `.fsi` surface contract, the
  feedback record schema + prompt contract, and the template parameter contract.
  Update the `AGENTS.md` SPECKIT plan reference.
- **Phase 2 (handled by `/speckit.tasks`)**: story-grouped tasks with `skillist`
  metadata. Not produced by this command.

## Notes / Risks

- **Extraction blast radius** (chosen "full extraction"): moving bodies out of
  `build/Governance` risks compile ripple in the build front-end and re-pointed
  tests. Mitigation: move bodies behind unchanged-shape signatures, keep
  governance-specific grammars/propagation as thin consumers in `build/Governance`,
  and re-point existing tests rather than rewriting them — preserving parity.
- **Bootstrapping**: `FS.Skia.UI.Build` references `src/SkillSupport` by
  `ProjectReference` (source), not the packed nupkg, avoiding build-of-the-builder
  version circularity; the packed nupkg is produced only for the template.
- **Byte-identity**: `--feedback false` must equal today's output exactly
  (SC-006). All feedback content sits in the `feedback == true` branch; verify the
  false branch emits no stray markers/whitespace via a generation diff test.
- **Two baseline mechanisms**: confirm whether the new package needs only the
  `readiness/per-package-surface/*.fsi.txt` baseline (PerPackageSurfaceDiff) or
  also a `readiness/surface-baselines/*.txt` (aggregate PackageSurfaceCheck) entry
  — resolved in research.md.
```
