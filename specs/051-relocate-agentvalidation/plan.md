# Implementation Plan: V3 Stage 2 — Relocate `AgentValidation` out of the Runtime Monolith

**Branch**: `051-relocate-agentvalidation` | **Date**: 2026-06-02 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/051-relocate-agentvalidation/spec.md`

## Summary

Relocate the `AgentValidation` governance contract parser (`AgentValidation.fsi` 261 LOC +
`AgentValidation.fs` 835 LOC) from the runtime monolith `src/Lib` into the
`FS.Skia.UI.Build` governance library, repoint its sole consumer
(`tests/Governance.Tests/AgentValidationFrameworkTests.fs`), drop the now-unused
`Governance.Tests → src/Lib/Lib.fsproj` reference, and shrink the monolith's surface
baseline accordingly. Behaviour is preserved byte-for-byte (only the `namespace` line and a
doc-comment phrase change); the relocation turns the `knownGates` allowlist into governance
config that Stage 5 can extend without touching runtime, satisfying the precondition the
Stage-0 per-package Route rule deferred. **Tier 1** (removes public API surface from the
`FS.Skia.UI` package), realized as a near-100% file rename. No host/scene/layout/rendering
change; generated products unaffected.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: BCL only for the moved module (`System.Diagnostics`/`IO`/`Text.Json`);
the destination library already references `YamlDotNet` + `DiffPlex`. No new dependency.
**Testing**: Expecto (`tests/Governance.Tests/AgentValidationFrameworkTests.fs` — same
fixtures, same assertion count, repointed), FAKE targets per `Route`.
**Target Platform**: Windows and Linux (governance tooling; build host).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Tier**: Tier 1 (contracted change) — removes the `AgentValidation` surface from the public
`FS.Skia.UI` package and updates its surface-area baseline. Realized as a relocation with
behaviour preserved (Principle V: no synthetic evidence — the repointed real test suite + the
structural rename diff are the oracle).

- **I. Spec → FSI → Semantic Tests → Implementation**: the `.fsi` already exists and is
  curated; the move preserves it (D3). The semantic test (`AgentValidationFrameworkTests`)
  exercises the public FSI surface and is repointed first, then the `.fs` body moves to
  satisfy it. ✔
- **II. Visibility in `.fsi`**: the relocated module keeps its explicit `.fsi`; no
  `private`/`internal`/`public` modifiers introduced; the monolith surface baseline is
  updated (D4). ✔
- **III. Idiomatic simplicity**: pure relocation; no new abstractions, operators, SRTP,
  reflection, or computation expressions. ✔
- **IV. Elmish/MVU boundary**: the existing `ValidationSelection` MVU + interpreter moves
  intact — `update` stays pure, I/O (file reads + `git`) stays at the interpreter edge
  (data-model.md). No new effect/command/subscription. ✔
- **V. Synthetic disclosure**: **none** — all evidence is real (real contract-shaped
  fixtures, real interpreter edges, real surface diff). No `[S]`/`[SEH]` tasks anticipated. ✔
- **VI. Test evidence**: the repointed suite must compile against the new home and pass;
  failing-first is the compile break from the namespace change. ✔
- **VII. Observability**: no diagnostics path changes (the interpreter's existing diagnostics
  move unchanged). ✔

### Repository Governance Decisions

- **Template ownership**: **N/A** — no source/docs/samples/tests/Spec-Kit-asset/package-policy
  or command-surface change reaches the template. `AgentValidation` was never templated;
  `.template.config/template.json` is untouched and no deferral is needed.
- **Dependency impact**: **N/A** — no new dependency. The moved module is BCL-only; the
  destination `FS.Skia.UI.Build` already pins `YamlDotNet`/`DiffPlex` centrally.
  `Directory.Packages.props`, `docs/dependencies.md`, and `DependencyReport` coverage are
  unchanged.
- **Command-surface impact**: No `build.fsx`/wrapper/`Dev`/`Verify`/`Ci`/`TemplateCheck`/
  `DependencyReport`/`GeneratedGuidanceCheck`/`TemplateDrift`/`EvidenceGraph`/`EvidenceAudit`
  **behaviour** changes. `validation.contract.yml` is **not** edited (currency vs `Routing.fs`
  preserved — SC-007). FAKE-backed targets share `.fake` state: run sequentially in the
  deterministic order (Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck →
  EvidenceGraph → EvidenceAudit); preserve non-FAKE parallel reads only.
- **Generated project impact**: **N/A** — `AgentValidation` is not shipped to products.
  Default/minimal generated contents, Controls guidance, local skills, validation logs,
  placeholder/excluded-history scans, and generated `Dev` behaviour are all unchanged; the
  default `app` is byte-unchanged (SC-006).
- **Evidence paths**: governance-suite result via `./fake.sh build -t Dev`; structural-parity
  via `git diff -M` (rename similarity); surface delta in
  `readiness/surface-baselines/FS.Skia.UI.txt`; consumer gates via `GeneratedGuidanceCheck`/
  `TemplateCheck`/`GeneratedProductCheck`; graph/audit via `EvidenceGraph`/`EvidenceAudit`.
  Per-feature readiness notes under `specs/051-relocate-agentvalidation/readiness/`.
- **`.fsi` / contract impact**: the monolith's public `.fsi` **shrinks** (the
  `AgentValidation` module leaves `src/Lib`); the governance library gains the curated `.fsi`
  (namespace + doc-comment phrase adapted only — D3). The monolith surface baseline
  `readiness/surface-baselines/FS.Skia.UI.txt` loses its 48 `FS.Skia.UI.AgentValidation.*`
  lines. **No runtime split package `.fsi` or per-package baseline changes** (SC-006). No
  product API/sample contract changes.
- **MVU/effect boundary**: the relocated `ValidationSelection` carries `Model`
  (`ValidationSelectionModel`), `Msg` (`ValidationSelectionMsg`), `Effect`
  (`ValidationSelectionEffect`), `init`, pure `update`, and the edge
  `ValidationSelectionInterpreter` (file/`git` I/O). Behaviour preserved (FR-004); pure-update
  and interpreter tests already exist in the repointed suite (Principle IV evidence is real).
- **Synthetic evidence**: **none.** No mocks/fakes/placeholders/canned responses introduced;
  the parity oracle is the real repointed test suite + the real surface diff. No `[S]`
  disclosure required.
- **Test evidence**: failing-first is the deliberate namespace-driven compile break in
  `AgentValidationFrameworkTests.fs`; after repointing + the move, the suite passes with the
  **same assertion count** (SC-002). No assertion weakened; no test skipped.
- **Observability**: no new diagnostics; the interpreter's existing actionable diagnostics
  (contract parse errors, git/metadata-unavailable degradation) move unchanged. No missing
  artifact-class behaviour added.
- **Deferred scope**: Stage 5 — adding `PerPackageSurfaceDiff` to `validation.contract.yml`,
  wiring its `Routing.fs` rule, the hard-gate enforcement, and deleting `src/Lib`. Stages 3–4
  — sample-pack/`ParityGallery`/`Parity.Tests` retirement and the residual `KeyboardInput`
  home. This feature only makes the `knownGates` precondition possible (SC-005); it wires
  nothing.

**Post-Phase-1 re-check**: design confirms zero new types/signatures, namespace chosen to
avoid the `Front/Support.fs` collision (D1/FR-011), compile slot is BCL-only and forward-
compatible with the Stage-5 `Routing → knownGates` consumption (D2). Constitution Check still
PASS; no new violations.

## Project Structure

```
build/Governance/
  FS.Skia.UI.Build.fsproj        # gains: <Compile Include="AgentValidation.fsi/.fs"/> after the Spike pair
  AgentValidation.fsi            # MOVED from src/Lib; namespace -> FS.Skia.UI.Build.AgentValidation
  AgentValidation.fs             # MOVED from src/Lib; namespace -> FS.Skia.UI.Build.AgentValidation
  Front/Support.fs               # UNCHANGED (keeps its own shadow types; distinct namespace)
  Routing.fs                     # UNCHANGED (Stage 5 will consume knownGates; compile order already ahead)

src/Lib/
  Lib.fsproj                     # drops the two AgentValidation <Compile Include> lines
  AgentValidation.fsi            # DELETED (moved)
  AgentValidation.fs             # DELETED (moved)
  KeyboardInput.*, Library.*     # UNCHANGED (residual; Stages 3–5)

tests/Governance.Tests/
  Governance.Tests.fsproj        # drops ProjectReference to ..\..\src\Lib\Lib.fsproj
  AgentValidationFrameworkTests.fs  # open FS.Skia.UI.AgentValidation -> FS.Skia.UI.Build.AgentValidation

readiness/surface-baselines/
  FS.Skia.UI.txt                 # drops the 48 FS.Skia.UI.AgentValidation.* lines

specs/051-relocate-agentvalidation/
  spec.md, plan.md, research.md, data-model.md, quickstart.md
  contracts/agentvalidation-surface.md
```

## Phase 2 outlook (for `/speckit-tasks`)

Story-grouped, dependency-ordered. Indicative shape (tasks command owns the final list +
`skillist` metadata; the `fsharp-build-orchestration` / `fs-skia-layout-evidence` skills are
the likely matches for the move + governance-evidence work):

1. **US1 (P1) — module lives in the governance library**: add the two `<Compile Include>`
   lines after `Spike` in `FS.Skia.UI.Build.fsproj`; `git mv` the `.fsi`/`.fs` into
   `build/Governance/`; rewrite the `namespace` line + doc-comment phrase; build the library.
2. **US1 — drop from the monolith**: remove the two `<Compile Include>` lines from
   `Lib.fsproj`; update `readiness/surface-baselines/FS.Skia.UI.txt` (drop the 48 lines);
   build the monolith.
3. **US2 (P2) — repoint the test + cut the coupling**: change the `open` in
   `AgentValidationFrameworkTests.fs`; remove the `Lib.fsproj` `ProjectReference` from
   `Governance.Tests.fsproj`; run the suite (`Dev`).
4. **US3 (P3) — precondition proof**: confirm `knownGates` is in `FS.Skia.UI.Build` and that
   extending it touches no `src/**` (review note; no contract/Routing edit).
5. **Evidence/parity + gates**: grep proof of no `FS.Skia.UI.AgentValidation` consumers;
   structural rename diff; run the escalated gate set `Route` prints; `EvidenceGraph` +
   `EvidenceAudit` PASS on real evidence; write readiness notes.
