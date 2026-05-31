# Feature Specification: Foundations F# Capability Skills

**Feature Branch**: `040-foundations-capability-skills`  
**Created**: 2026-05-31  
**Status**: Draft  
**Input**: User description: "@docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md create the necessary agent skills."

## Overview

The Bash/Python → F# foundations port (companion analysis and implementation-plan reports,
consumed by plan Stages 2–6) requires a fixed set of F# *capabilities*: parsing, graph algorithms,
artifact and source generation, file discovery/globbing, shell/process/git wrapping, and build
orchestration/testing. The capability report
(`docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md`) enumerated those
capabilities against the actual scripts and chose, per capability, the right library (or
"hand-roll") with an adopt/consider/reject verdict.

This feature captures that knowledge as **reusable agent skills** so that every agent executing a
foundations-port stage invokes grounded, durable guidance instead of re-deriving the library
landscape and parity cautions each time. The skills are **capability/reference** guidance, not Spec
Kit command skills, and are not referenced by any task `skillist` — they do not alter the evidence
graph.

## Clarifications

### Session 2026-05-31

- Q: The six skills already exist (byte-identical, covering C1–C21) — what work should this feature perform on them? → A: Refine all six — revise every skill's content for completeness/consistency regardless of current state, then re-verify byte-identity across both trees.
- Q: Should byte-identity (FR-002) be mechanized, or deferred? → A: Add a `SkillSyncCheck` FAKE target now that fails on any `.claude`/`.agents` drift across the six skills.
- Q: How should "comprehensive" be defined for each skill? → A: Code-heavy cookbook — multiple runnable F# examples and a full API walkthrough for *each capability* the skill owns.
- Q: What correctness bar applies to the code examples? → A: Compile-verified — every example must provably build (not merely illustrative).
- Q: How are the compile-verified examples built and enforced? → A: Tangle-and-compile — a build step extracts the ` ```fsharp ` blocks from the six skills into a generated examples project that references the adopted packages, compiles it, and fails on error; `SKILL.md` stays the single source; wired into `Dev`/`Verify`.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Stage agent invokes capability guidance instead of re-deriving it (Priority: P1)

An agent assigned a foundations-port stage (e.g. the Stage 4 Python port, or the Stage 2
single-source generation) needs to know which F# library to use for a capability, with which parity
cautions, and what the verdict's rationale is. The agent discovers and invokes the matching
capability skill and receives the verdict, the concrete F# approach, the exact input grammar to
reproduce, and the determinism/parity cautions — without reading the full report or re-surveying the
F# ecosystem.

**Independent test**: From a clean agent context, ask "which library parses `tasks.deps.yml` in the
port, and what parity risk must I guard?" The `fsharp-parsing` skill alone answers (YamlDotNet,
typed; the two-shapes parity caution) without consulting the source report.

### User Story 2 - Maintainer keeps the two skill trees synchronized (Priority: P1)

The repository requires that `.claude/skills/**` and `.agents/skills/**` are synchronized peers
(byte-identical for shared skills). The maintainer runs the `SkillSyncCheck` target, which verifies
that each capability skill exists identically in both trees and fails (naming the offender) on any
drift, so Claude Code and Codex agents always receive the same guidance.

**Independent test**: Run `SkillSyncCheck` with the trees in sync — it passes. Desynchronize one
pair (change one byte) — it fails and names the drifted skill. Restore — it passes again.

### User Story 4 - Agent copies a worked example that actually compiles (Priority: P1)

A stage agent implementing a capability copies a runnable F# example straight from the skill (e.g. a
YamlDotNet two-shape deserialize, a Kahn topo sort, a `Utf8JsonWriter` schema-1.0 emit) and adapts
it, trusting it is API-correct because the build compiles every example. The example is not an
approximate sketch — it builds against the named package versions.

**Independent test**: Run the examples-compile gate — every ` ```fsharp ` block extracted from the
six skills compiles. Break one example (wrong API call) — the gate fails and points at the offending
skill/block. Fix it — the gate passes.

### User Story 3 - Coverage of every capability the port needs (Priority: P2)

A maintainer auditing readiness for the port confirms that every capability family the report
identified (the eight families across C1–C21) maps onto exactly one capability skill, with no
capability left uncovered and no skill inventing scope beyond the report.

**Independent test**: For each capability family in the report's inventory, locate the owning skill;
confirm all six skills together cover C1–C21 and that each skill cites the report as its source.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The feature MUST provide six capability skills covering the report's eight capability
  families: `fsharp-parsing` (C1–C5, C16, C21; C5 JSON-write folded with C4 per report §3.4),
  `fsharp-graph-algorithms` (C6–C9),
  `fsharp-code-generation` (C10–C12), `fsharp-io-globbing` (C13–C14), `fsharp-shell-process`
  (C15, C17), and `fsharp-build-orchestration` (C18–C20).
- **FR-002**: Each skill MUST exist byte-identically in both `.claude/skills/<name>/SKILL.md` and
  `.agents/skills/<name>/SKILL.md` (synchronized-peer requirement), enforced mechanically by the
  `SkillSyncCheck` target (FR-011).
- **FR-003**: Each skill MUST carry frontmatter with `name`, a one-line `description`, a
  `compatibility` line scoping it to the F# governance library (`build/Governance`) under net10.0 as
  build-tooling-only, and `metadata` citing the capability report as `source`.
- **FR-004**: Each skill MUST state the report's library verdicts for its capabilities — the adopted
  package(s), and the explicitly rejected/deferred alternatives with the one-line reason — so an
  agent need not re-survey the ecosystem.
- **FR-005**: Skills covering parity-critical ports (`fsharp-parsing`, `fsharp-graph-algorithms`)
  MUST reproduce the exact input grammars / algorithm rules to be matched (e.g. the `tasks.md` task
  line regex, box/annotation tokens, the synthetic-propagation rule) and MUST state the byte-parity
  obligation against the Stage-0 golden fixtures before the Bash/Python is deleted.
- **FR-006**: Skills MUST record the report's named cautions where they apply — notably the two
  `tasks.deps.yml` shapes (object vs legacy bare-list) and .NET-glob vs Python-`fnmatch` semantic
  drift — each with the "golden-test before cutover" mitigation.
- **FR-007**: Skills MUST be capability/reference skills only: they MUST NOT be referenced by any
  task `skillist`, MUST NOT introduce Spec Kit command behavior, and therefore MUST NOT alter the
  evidence graph.
- **FR-008**: Skill guidance MUST stay within the report's adopt set and scope — it MUST NOT
  recommend re-introducing `FSharp.Compiler.*`/FCS or runtime-script compilation, and MUST confine
  all package additions to build-tooling scope (`build/Governance/**` and/or
  `tests/Governance.Tests`).
- **FR-009**: Each skill MUST name the plan stages that consume it (per the report's §10/§11
  mapping) so an agent can find the right skill from the stage it is executing.
- **FR-010**: All six skills MUST be refined in this feature to meet the cookbook bar (FR-012) — the
  content of every skill is revised (not only the skills with a detected gap) — and byte-identity
  across both trees MUST be re-verified after refinement.
- **FR-011**: The feature MUST add a `SkillSyncCheck` build target that fails when any of the six
  capability skills' `SKILL.md` differs between `.claude/skills/<name>/` and `.agents/skills/<name>/`,
  so FR-002 is enforced mechanically rather than by manual inspection. The check MUST report which
  skill(s) drifted. (Repository note: this environment has no `diff`/`cmp`; the check relies on a
  content hash such as `sha256sum`, not those binaries.)
- **FR-012**: Each skill MUST be a **code-heavy cookbook**: for *every capability it owns*, the skill
  MUST provide multiple runnable F# code examples and a walkthrough of the relevant API surface of the
  adopted library, in addition to the verdict (FR-004), parity grammar/rules where applicable
  (FR-005), cautions (FR-006), and a Sources/links list (FR-013). Barebones prose-only guidance does
  not satisfy this requirement.
- **FR-013**: Each skill MUST include a Sources/links section with working links to the adopted
  library's documentation/API reference (and the capability report) for the capabilities it covers,
  so an agent can follow up without re-searching.
- **FR-014**: Every F# code example in the six skills MUST be **compile-verified** via a
  tangle-and-compile build target: the build extracts the ` ```fsharp ` blocks from each `SKILL.md`
  into a generated examples project that references the adopted packages (build-tooling scope only),
  compiles it, and fails on any compile error, naming the offending skill/block. `SKILL.md` remains
  the single source of the example text (examples are not hand-duplicated elsewhere). The target MUST
  be wired into `Dev`/`Verify` and obey the repository's serialized FAKE-run order.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No *product* or generated-package-consumer change. To compile-verify the
  examples (FR-014), the adopt-set build-tooling packages (e.g. FSharp.SystemTextJson, XParsec,
  Microsoft.Extensions.FileSystemGlobbing, Fake.IO.*, Fake.Tools.Git, DiffPlex, FsCheck; YamlDotNet
  is present) ARE referenced now by the generated examples project, with versions pinned in
  `Directory.Packages.props` per Central Package Management. These are build-tooling scope only,
  never shipped in a generated product, and must not introduce `FSharp.Compiler.*`/FCS.
- **Public contract impact**: No `.fsi` signatures, documented public APIs, sample contracts, or
  surface baselines change. Skills are author/agent reference material only.
- **State workflow impact**: None. No stateful workflow, I/O, commands, effects, subscriptions, or
  interpreter behavior changes.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshots, Vulkan,
  Skia, visual output, or unsupported-environment diagnostics change.
- **Evidence obligations**: Real evidence is the rendered (refined) skill files under
  `.claude/skills/<name>/SKILL.md` and `.agents/skills/<name>/SKILL.md`; a passing `SkillSyncCheck`
  run proving byte-identity across both trees; and a passing examples-compile (tangle-and-compile)
  run proving every ` ```fsharp ` example builds. These are author-time governance artifacts, not
  runtime/visual evidence.
- **Unsupported scope**: This feature delivers the six skills, their sync gate, and their
  example-compile gate. It does NOT port any script, write any port-stage governance-library F#
  (parsers, graph algorithms, generators) beyond the small example snippets, ship any package in a
  generated product, or author the Stage-2…6 code — those remain later features. No visual, release,
  platform, or distribution boundary is touched.
- **Build-target impact**: Two build targets are added/affected: `SkillSyncCheck` (FR-011, byte-identity)
  and the tangle-and-compile examples gate (FR-014, e.g. `SkillExamplesCheck`). Both MUST be wired
  into the appropriate aggregate validation (e.g. `Dev`/`Verify`) so drift and broken examples are
  caught in normal runs. No other target (`Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`) changes its meaning.
  Both new targets are FAKE-backed and MUST obey the repository's serialized FAKE-run order.

## Success Criteria *(mandatory)*

- **SC-001**: All six capability skills exist; together they cover 100% of the report's capability
  inventory (C1–C21 / eight families) with each capability owned by exactly one skill.
- **SC-002**: For each of the six skills, `.claude/skills/<name>/SKILL.md` and
  `.agents/skills/<name>/SKILL.md` are byte-identical, and the `SkillSyncCheck` target passes; the
  same target fails (naming the drifted skill) when any one pair is deliberately desynchronized.
- **SC-003**: Every skill cites the capability report as its `source`, states the adopted library and
  the rejected/deferred alternatives for each capability it covers, and — per the cookbook bar — for
  every capability it owns provides multiple runnable F# examples, an API walkthrough, and a
  Sources/links section with working documentation links.
- **SC-004**: An agent given a stage task can, using only the matching skill, name the correct
  library and the controlling parity caution for any capability in that stage — verified by spot
  checks for parsing (YAML two-shapes), globbing (fnmatch drift), and graph (synthetic propagation).
- **SC-005**: None of the six skills appears in any task `skillist` and none changes the evidence
  graph (the existing `EvidenceGraph`/`EvidenceAudit` outputs are unaffected).
- **SC-006**: All six skills are refined in this feature (every skill's content revised to meet the
  cookbook bar of FR-003…FR-013), not only those with a pre-existing gap, and the refined set still
  passes `SkillSyncCheck`.
- **SC-007**: Every ` ```fsharp ` example across the six skills compiles via the tangle-and-compile
  target against the pinned adopted-package versions; introducing a deliberately broken example
  fails the gate and names the offending skill/block, and removing the break restores a pass.

## Assumptions

- The six capability skills were already drafted and committed alongside the report (commit
  "Add F# capabilities/library analysis + capability skills for foundations port") but are currently
  barebones (~50 lines each, no code blocks, no links). This feature **refines all six into
  code-heavy cookbooks** (FR-012) — not authoring from nothing, but a substantial content uplift —
  and re-verifies their cross-tree byte-identity.
- The adopted libraries are all available on NuGet today (XParsec v1.0.0, FSharp.SystemTextJson,
  Microsoft.Extensions.FileSystemGlobbing, Fake.*, DiffPlex, FsCheck; YamlDotNet present), so a
  build-tooling examples project can reference and compile them now even though the port-stage
  governance code does not yet exist. Compile-verification (FR-014) checks the example snippets only,
  not any ported algorithm.
- "Synchronized peers" means byte-identical `SKILL.md` content for these shared capability skills;
  the `SkillSyncCheck` target built in this feature (FR-011) mechanizes that check.
- The library verdicts are authoritative as written in the report; refinement improves
  completeness/consistency of the guidance but does NOT re-open the adopt/consider/reject decisions.
