# Implementation Plan: Foundations F# Capability Skills

**Branch**: `040-foundations-capability-skills` | **Date**: 2026-05-31 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/040-foundations-capability-skills/spec.md`

## Summary

Refine the six already-committed-but-barebones F# capability skills (`fsharp-parsing`,
`fsharp-graph-algorithms`, `fsharp-code-generation`, `fsharp-io-globbing`, `fsharp-shell-process`,
`fsharp-build-orchestration`) into **code-heavy cookbooks** — each owning a set of report
capabilities (C1–C21), each carrying compile-verified runnable F# examples, an API walkthrough of
its adopted library, the report's verdicts, the parity grammars/cautions, and a Sources/links
section — written byte-identically into both `.claude/skills/<name>/SKILL.md` and
`.agents/skills/<name>/SKILL.md`. Two FAKE-backed governance gates make the obligations mechanical:
**`SkillSyncCheck`** (SHA-256 byte-identity across the two trees, naming any drifted skill) and a
**tangle-and-compile examples gate** (`SkillExamplesCheck`) that extracts every ` ```fsharp ` block
into a generated build-tooling examples project, references the adopt-set packages, compiles it, and
fails on any error naming the offending skill/block. Both gates wire into `Dev`/`Verify` and obey
the repository's serialized FAKE-run order.

These are **capability/reference** skills only: no task `skillist` references them, no Spec Kit
command behavior is added, and the evidence graph is unchanged.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (build-tooling scope: `build/Governance`, `build/`, and a
new generated examples project).
**Primary Dependencies (build-tooling only, never shipped in a generated product, FCS-free)**:
- Present: `YamlDotNet 17.1.0`, `Fake.Core.Target 6.1.4`, `Expecto 10.2.2`, `FSharp.Core 10.1.300`.
- Added to `Directory.Packages.props` for the examples project: `FSharp.SystemTextJson`, `XParsec`
  (v1.0.0), `Microsoft.Extensions.FileSystemGlobbing`, `Fake.IO.FileSystem`, `Fake.Tools.Git`,
  `DiffPlex`, `FsCheck` (v3, via `Expecto.FsCheck`). `System.Text.Json` and `System.Security.Cryptography`
  are in the BCL. **No `FSharp.Compiler.*`/FCS.**
**Testing**: Expecto governance tests under `tests/Governance.Tests`; FAKE gates (`SkillSyncCheck`,
`SkillExamplesCheck`) wired into `Dev`/`Verify`; the examples project's own compile IS the evidence
for FR-014.
**Target Platform**: Windows and Linux (build-tooling; runs anywhere the SDK runs). The environment
has no `diff`/`cmp`; byte-identity uses in-process SHA-256 (`System.Security.Cryptography.SHA256`),
not external binaries.
**Change Tier**: **Tier 1 (contracted change)** — introduces new build-tooling dependencies and two
new build targets. No *product* public API, `.fsi` product surface, sample contract, or surface
baseline changes; the new build/Governance modules still require curated `.fsi` (Principle II).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: No `.template.config/template.json` change. The capability skills live in
  `.claude/skills/**` / `.agents/skills/**` (repo-local governance skills), not in any package
  `src/*/skill/` or template fragment, so generated products are untouched. No command-surface or
  Spec Kit asset change beyond the two new FAKE targets (below). **Deferral: none required.**
- **Dependency impact**: `Directory.Packages.props` GAINS the seven adopt-set package versions in a
  build-tooling `ItemGroup` (alongside the existing `Fake.Core.Target` build-tooling group), each
  pinned per Central Package Management. `docs/reports/dependencies.md` MUST gain rows for the new
  build-tooling packages (need, version, maintenance owner = build-tooling/governance). The new
  examples project and `tests/Governance.Tests` are the only consumers; `DependencyReport` coverage
  MUST recognize the build-tooling scope so the additions do not read as product/runtime
  dependencies. **No package ships in any generated product.**
- **Command-surface impact**: `build.fsx` GAINS two targets: `SkillSyncCheck` and
  `SkillExamplesCheck`. Both added to `requiredTargets`, `targetDependencyRows`, and the `Dev`
  (and therefore transitively `Verify`) dependency list. No existing target
  (`Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
  `EvidenceGraph`, `EvidenceAudit`) changes meaning. Both new targets are FAKE-backed and MUST obey
  the serialized FAKE-run order in `CLAUDE.md`/`AGENTS.md`. Serialized validation order for this
  feature:
  1. `./fake.sh build -t Dev` (now includes `SkillSyncCheck` + `SkillExamplesCheck`)
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: None. No default/minimal generated contents, selected Controls
  guidance, generated local skills, validation logs, placeholder/excluded-history scans, or generated
  `Dev` behavior change. The six skills are author/agent reference material, never generated into a
  product.
- **Evidence paths**:
  - Refined skills: `.claude/skills/<name>/SKILL.md` and `.agents/skills/<name>/SKILL.md` (×6).
  - Sync gate log: `readiness/logs/skill-sync-check.txt`; readiness report
    `readiness/skill-sync-check.md` (PASS lists the six pairs + their matching hashes; FAIL names
    drifted skill(s)).
  - Examples gate: generated project under `build/SkillExamples/` (sources tangled from the skills);
    compile log `readiness/logs/skill-examples-check.txt`; readiness report
    `readiness/skill-examples-check.md` (PASS lists block count per skill; FAIL names skill/block).
  - Governance unit/property tests: `tests/Governance.Tests` (extractor + hasher behavior).
- **`.fsi` / contract impact**: No *product* `.fsi`, public docs, surface baseline, sample contract,
  or compatibility note changes. New build/Governance modules (skill discovery, ` ```fsharp `
  extractor/tangler, SHA-256 byte-identity comparator) are public F# modules and therefore REQUIRE
  curated `.fsi` companions (Principle II) even though they are build-tooling. No access modifiers in
  `.fs`. The "contract" surfaces for this feature are documented under `contracts/`: the SKILL.md
  frontmatter schema and the two build-target CLI contracts.
- **MVU/effect boundary**: The build engine in `build.fsx` is already an Elmish/MVU `update` +
  effect-interpreter. The two new targets are added as `StartTarget` dispatch arms returning new
  effects (`SkillSyncCheck`, `SkillExamples`/compile), interpreted at the edge. The tangle/hash logic
  itself is **pure** over its file inputs (read text → extract blocks → hash/compare) with I/O
  performed only by the interpreter (read skill files, write examples project, run `dotnet build`).
  No new long-lived stateful workflow is introduced, so a full new `Model`/`Msg` algebra is not
  required — the work plugs into the existing boundary. In-process-first per `fsharp-shell-process`:
  hashing and extraction are in-process F#; only `dotnet build` of the examples project shells out.
- **Synthetic evidence**: **None.** Real evidence throughout — the rendered skill files, a real
  SHA-256 comparison over the real two trees, and a real `dotnet build` of the real tangled examples
  against the real pinned packages. No mocks, fakes, placeholders, or canned responses. No `[S]`/
  `[SEH]` tasks anticipated. The deliberate-break checks in SC-002/SC-007 are gate self-tests, not
  shipped synthetic fixtures.
- **Test evidence**: Failing-first semantic tests for the governance helpers in
  `tests/Governance.Tests`: (a) extractor returns the ` ```fsharp ` blocks with stable
  skill/block identity (fails before the extractor exists); (b) SHA-256 comparator reports drift
  naming the offender (fails before the comparator exists). Gate-level evidence: `SkillSyncCheck`
  passes in-sync / fails-and-names on a one-byte desync / passes on restore; `SkillExamplesCheck`
  passes on the refined skills / fails-and-names on a deliberately broken block / passes on fix.
- **Observability**: Both gates emit structured diagnostics. `SkillSyncCheck` FAIL message names each
  drifted skill and both hashes; PASS report lists the six pairs and the shared hash. `SkillExamplesCheck`
  FAIL surfaces the F# compiler diagnostic mapped back to the owning skill file + block index; missing
  examples-project artifact fails the gate explicitly (no silent skip). Fail-fast, no swallowed
  errors (Principle VII).
- **Deferred scope**: This feature delivers ONLY the six refined skills, the sync gate, and the
  examples-compile gate. It does NOT port any Bash/Python script, write any Stage-2…6 governance
  algorithm (parsers, graph, generators) beyond the in-skill example snippets, ship any package in a
  generated product, or change visual/release/platform/distribution boundaries. The actual port
  stages remain later features (report §13).

**Gate result**: PASS. No unjustified violations. Tier 1 obligations that apply (new dependencies,
new build-tooling `.fsi`, dependency-doc updates) are scheduled; product-surface obligations
(`.fsi` baselines, sample contracts) are correctly N/A because no product surface changes.

## Project Structure

```
specs/040-foundations-capability-skills/
  spec.md                      # input (this feature)
  plan.md                      # this file
  research.md                  # Phase 0 — decisions (tangle strategy, warning policy, sync mechanism)
  data-model.md                # Phase 1 — Skill, CodeBlock, SyncPair, capability→skill ownership entities
  quickstart.md                # Phase 1 — how to run/extend the two gates and refine a skill
  contracts/
    skill-frontmatter.contract.md   # SKILL.md frontmatter + section schema (FR-003…FR-013)
    build-targets.contract.md       # SkillSyncCheck / SkillExamplesCheck CLI + exit/report contract

.claude/skills/<name>/SKILL.md       # ×6 refined cookbooks (Claude tree)
.agents/skills/<name>/SKILL.md       # ×6 byte-identical peers (Codex tree)

build/
  Governance/
    FS.Skia.UI.Build.fsproj          # gains: SkillSync.fs(i), SkillExamples.fs(i) (extractor/tangler/hasher)
    SkillSync.fsi / SkillSync.fs     # discover six skill pairs, SHA-256 compare, name drift (pure + IO edge)
    SkillExamples.fsi / SkillExamples.fs  # extract ```fsharp blocks, render examples project sources
  SkillExamples/                     # NEW generated build-tooling project (tangle target)
    SkillExamples.fsproj             # references adopt-set packages; IsPackable=false; build-tooling only
    Generated/*.fs                   # tangled blocks (one module per skill/block) — regenerated, not hand-edited
build.fsx                            # gains SkillSyncCheck + SkillExamplesCheck targets, wired into Dev

Directory.Packages.props             # gains 7 build-tooling PackageVersion entries
docs/reports/dependencies.md         # gains rows for the new build-tooling packages
tests/Governance.Tests/              # gains extractor + hasher semantic/property tests
```

## Phase 0 — Outline & Research

See [research.md](./research.md). Resolves: (1) how each ` ```fsharp ` block is made
compilable in isolation (module-wrapping + authoring convention); (2) warning policy for the
examples project given repo-wide `TreatWarningsAsErrors`; (3) in-process SHA-256 vs `sha256sum`
for the sync gate (no `diff`/`cmp` in env); (4) how blocks map back to skill/block identity for
diagnostics; (5) examples-project package set and how to keep it FCS-free; (6) whether to reuse
existing effects or add new effect DU cases in `build.fsx`.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md): entities — `CapabilitySkill`, `CodeBlock`, `SkillPair`,
  `CapabilityOwnership` (C1–C21 → exactly one skill), `AdoptVerdict`, and the validation rules from
  FR-001…FR-014 / SC-001…SC-007.
- [contracts/skill-frontmatter.contract.md](./contracts/skill-frontmatter.contract.md): required
  frontmatter (`name`, `description`, `compatibility`, `metadata.source`) and required body sections
  (When to use, Library verdicts, Exact grammars where parity-critical, Cautions, API walkthrough +
  runnable examples per owned capability, Consuming stages, Sources/links).
- [contracts/build-targets.contract.md](./contracts/build-targets.contract.md): the `SkillSyncCheck`
  and `SkillExamplesCheck` invocation, exit-code, log/report-path, and failure-diagnostic contract.
- Agent context update: `AGENTS.md` plan reference (between `<!-- SPECKIT START/END -->`) repointed
  to this plan.

## Phase 2 — Stop & Report

Planning ends here (per the speckit-plan workflow). `/speckit-tasks` is the next step.
