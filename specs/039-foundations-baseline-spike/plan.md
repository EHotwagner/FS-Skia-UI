# Implementation Plan: Foundations Baseline & Build-Library Spike

**Branch**: `039-foundations-baseline-spike` | **Date**: 2026-05-31 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/039-foundations-baseline-spike/spec.md`

## Summary

Deliver the two prerequisites that make the foundations programme **measurable** and **safe**, plus the shaping ADRs the later stages reference — and *nothing else*:

1. A committed, git-SHA-pinned **baseline** of the "before" state (script size with orchestration-vs-validation breakdown, governance Markdown volume, F#/Bash/Python LOC mix, per-feature ceremony-time estimate) and a set of **golden evidence fixtures** (task-graph JSON + Markdown + audit count block) captured via the *existing* `EvidenceGraph`/`EvidenceAudit` path, proven byte-for-byte reproducible. These fixtures are the designated **Stage 4 parity oracle**.
2. A **de-risking spike** that stands up a compiled governance library skeleton (`FS.Skia.UI.Build`, working name per D1) and a dedicated, compiled FAKE build front-end project that references it, then drives one trivial target whose body lives in the library through the build entry point — confirming **D2** (dedicated FAKE build project) or recording a reproducible blocker that triggers the documented thin-`build.fsx` fallback.
3. Five **ADRs** recording the already-resolved shaping decisions (D1 placement/distribution, D2 build front-end form, contract-versioning policy, D4 Spec Kit fork stance, D6 configuration representation) and one written statement of the **programme meta-process** + named dogfood-feature set.

**Technical approach**: The spike's single technical unknown is whether FAKE's modular `Fake.Core.Target` API can be consumed as an ordinary NuGet *library* from a compiled `dotnet run` exe (no FSX script runner, no FSharp Compiler Services) and still register and run targets whose bodies delegate to a referenced compiled library. The plan validates exactly that on a one-target slice. Everything else is recording, archiving, and document authoring. **No runtime code, no `.fsi` surface, and no existing build target is touched.**

## Technical Context

**Language/Version**: F# / .NET `net10.0` (inherited from `Directory.Build.props`: `TreatWarningsAsErrors`, `FS0078`-as-error, Central Package Management)
**Primary Dependencies**: `Fake.Core.Target` (+ minimal `Fake.Core.*` companions) added to `Directory.Packages.props` as new central `PackageVersion` entries — consumed as a *library*, not via the FSX runner. `FSharp.Core` (already central). **No** `FSharp.Compiler.Service`. **No** new runtime package.
**Testing**: Existing `EvidenceGraph`/`EvidenceAudit` FAKE path (Bash + Python, consumed unchanged) for fixture capture; `dotnet build` under `TreatWarningsAsErrors` for the two new projects; the dedicated front-end's own run for the spike target; the existing serialized validation sequence for the no-regression guarantee. A focused `Governance.Tests` reproducibility assertion is *optional* and noted as a follow-up — the byte-for-byte fixture check is performed by re-running the evidence commands per FR-003/SC-002, not by a new test that would itself be new build-tooling surface.
**Target Platform**: Linux and Windows (repo convention). Spike validated on Linux (`net10.0`); the dedicated-project + `dotnet run` flow is platform-neutral.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification (Tier)**: **Tier 1** — the feature introduces two new projects and a new inter-project contract (the build front-end → governance-library project reference) and a new library identity that will later be packaged (D1). It therefore takes the full artifact chain *scoped to the new build-tooling projects*. Crucially, it changes **no runtime public API**, no existing `.fsi`, and no existing surface baseline: `PackageSurfaceCheck` and `FsiTranscripts` must show **no diff** (the eight runtime packages are untouched). The new governance-library skeleton carries its own curated `.fsi` for its one public module per Principle II, but it is not part of the tracked runtime surface baselines.

### Repository Governance Decisions

- **Template ownership**: **No change.** This feature touches no `template/**`, no `.template.config/template.json`, no Spec Kit assets, no command surface consumed by generated products. The governance library is *created but not yet packaged or distributed to consumers* (distribution is decided in the D1 ADR but exercised only in Stage 4/5). Deferral recorded: template consumption of `FS.Skia.UI.Build` is out of scope here.
- **Dependency impact**: **Required.** New central `PackageVersion` entries for `Fake.Core.Target` (and the minimal companion `Fake.Core.*` packages the Target API needs) are added to `Directory.Packages.props`. `docs/dependencies.md` gains the new build-tooling dependency with need/version-pinning/owner per the constitution's dependency rule. `DependencyReport` coverage: the new packages are build-tooling-only and **not** shipped in any generated product; record that scoping explicitly. **No** `FSharp.Compiler.Service` is introduced (FR-012) — verified by `grep` over the restored graph.
- **Command-surface impact**: **Additive only.** The spike adds a *new* compiled entry point (`dotnet run --project build/Build.fsproj -- <target>`) that exists alongside, and does not replace, the existing `./fake.sh`/`build.fsx` flow. **No edit** to `Dev`, `Verify`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, or `EvidenceAudit` (FR-010, FR-011). FAKE-backed validation is run sequentially in the canonical serialized order; the spike's own `dotnet run` is **not** FAKE-backed and does not touch `.fake` state, but is still run separately from any FAKE target to avoid concurrent `.fake` use.
- **Generated project impact**: **None.** No default/minimal generated contents, Controls guidance, local skills, validation logs, placeholder/excluded-history scans, or generated `Dev` behaviour change. `GeneratedProductCheck`/`GeneratedGuidanceCheck`/`TemplateCheck` remain green and unchanged.
- **Evidence paths**: (1) Baseline → `docs/reports/_baselines/2026-05-31-foundations.md` (git SHA pinned). (2) Golden fixtures → `tests/Governance.Tests/fixtures/evidence-golden/<feature>/{task-graph.json,task-graph.md,audit-counts.txt}`. (3) Spike outcome → `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md`. (4) ADRs → `docs/adr/000N-*.md`. (5) Meta-process → recorded in this plan (§Programme Meta-Process) and cross-linked from the baseline doc. (6) No-regression evidence → the standard serialized sequence run captured to `readiness/logs/`.
- **`.fsi` / contract impact**: **No runtime contract change.** The governance-library skeleton ships one public module with a curated `.fsi` (its single trivial spike function); this is a *new* build-tooling surface, not a tracked runtime baseline. Existing surface baselines and `FsiTranscripts` are untouched and must show no diff (SC-006).
- **MVU/effect boundary**: **N/A.** No stateful or I/O-bearing runtime workflow is added. The spike target is a trivial pure/console action; the MEL/interpreter extraction is explicitly Stage 5 and out of scope (FR-011).
- **Synthetic evidence**: **None expected.** Baseline counts are real `wc`/`git ls-files` measurements at a real commit; golden fixtures are real outputs of the existing evidence engine; the spike runs a real compiled target. No mocks, fakes, placeholders, or canned responses are planned. If the spike *fails* (fallback path), the recorded blocker is a real, reproducible observation — not synthetic. No `[S]`/`[SEH]` tasks anticipated; any that arise during `/tasks` must carry full Principle V disclosure.
- **Test evidence**: The fixture reproducibility check (FR-003/SC-002) is the failing-first-style evidence: capture, then re-run, then diff for byte equality. The two-project compile + spike-target run is the spike's pass/fail evidence (SC-003/SC-004). The existing serialized gate sequence is the no-regression evidence (SC-006).
- **Observability**: The spike target emits an explicit, identifiable success line proving it ran from the library (not inline). The spike-outcome doc records the exact `dotnet run` command, its output, and the restored-package graph (FCS-absence grep) so the result is reproducible. The baseline doc records the exact commands used for every count.
- **Deferred scope**: Library *population*, validator moves, Python/Bash port, MEL extraction, `build.fsx` retirement, two-tier `Route`, single-source generation, prose trimming, contract-versioning *enforcement*, and any distribution/packaging of `FS.Skia.UI.Build` are all explicitly deferred to Stages 1–7 (FR-011).

**Gate result**: **PASS.** No principle is violated. Tier 1 obligations apply only to the new build-tooling projects and are satisfied by their `.fsi` + dependency disclosure + tests; all runtime invariants are preserved by construction (the feature edits no runtime code).

## Project Structure

New artifacts introduced by this feature (additive; nothing existing is moved or deleted):

```
build/                                         # NEW top-level build-tooling root (distinct from existing scripts/build/)
├── Build.fsproj                               # NEW dedicated FAKE build front-end (Exe, dotnet run)
├── Program.fs                                  #   registers + runs targets via Fake.Core.Target; delegates body to library
└── Governance/
    ├── FS.Skia.UI.Build.fsproj                # NEW governance library skeleton (net10.0)
    ├── Spike.fsi                               #   curated public signature (Principle II) — one trivial function
    └── Spike.fs                                #   trivial target body that lives in the library

docs/
├── adr/                                        # NEW
│   ├── 0001-governance-library-placement-and-distribution.md     # D1
│   ├── 0002-build-front-end-form.md                              # D2
│   ├── 0003-generated-product-contract-versioning.md            # contract-versioning policy
│   ├── 0004-spec-kit-fork-stance.md                             # D4
│   └── 0005-configuration-representation.md                     # D6
└── reports/_baselines/                         # NEW
    ├── 2026-05-31-foundations.md               # baseline counts + golden-fixture manifest + meta-process link, SHA-pinned
    └── 2026-05-31-spike-d2-outcome.md          # "D2 confirmed" or "fallback triggered" + reproducible record

tests/Governance.Tests/fixtures/evidence-golden/   # NEW committed fixtures (parity oracle)
├── <feature-A>/{task-graph.json,task-graph.md,audit-counts.txt}
├── <feature-B>/{...}
└── <feature-C>/{...}

FS-Skia-UI.sln                                   # +2 project entries (Build, FS.Skia.UI.Build) — additive
Directory.Packages.props                         # + Fake.Core.* central PackageVersion entries
docs/dependencies.md                             # + new build-tooling dependency rows
```

**Naming note (resolved):** the dedicated build root is the **top-level `build/`** directory (matching the D2 ADR's `build/Build.fsproj` and `build/Governance/FS.Skia.UI.Build.fsproj`). This is intentionally distinct from the existing `scripts/build/*.fsx` helper directory; the two do not collide (different paths, different purpose). The library lives under `build/Governance/` rather than `src/` so it is not mistaken for a shipped runtime package and is not swept by runtime surface-baseline tooling.

## Golden-Fixture Feature Selection (resolved during planning)

Per FR-002 and the spec's assumption, three features are captured. Selection favours **frozen, already-merged** features (so the source `tasks.md`/`tasks.deps.yml` cannot drift) with **diverse evidence shapes** (so the parity oracle exercises the audit's full status vocabulary):

| Role | Feature | Why chosen |
|---|---|---|
| "current"/most-recent completed | `038-authoring-guidance-consistency` | Latest merged feature; representative of the current task/deps schema. |
| historical | `037-authoring-audit-robustness` | Exercises the audit-status-region scanner directly (audit robustness). |
| historical | `017-synthetic-error-evidence` | Exercises `[SEH]`/synthetic propagation — covers `accepted-seh-tasks` / `auto-synthetic-tasks` counts the oracle must lock. |

**Substitution rule (edge case):** if any selected feature does not produce a *stable* (reproducible) evidence output at the pinned commit, substitute another merged feature and record the substitution in the baseline doc rather than committing an unstable fixture (spec Edge Cases; FR-003).

> Note: the *in-flight* feature 039 itself has no frozen `tasks.md` at baseline-capture time (its tasks are generated after this plan). It is therefore **not** a fixture source; "current feature" is interpreted as the most-recent *completed* feature (038). This is recorded explicitly in the baseline doc to avoid a moving-tree oracle.

## Programme Meta-Process (FR-008 — recorded here, the single discoverable place)

- **Default tier for foundations features:** the **lightweight framework-author loop** — `Dev` plus a surface check only — *except* features that touch governance or consumer contracts, which **escalate** to the full serialized gate set. (The two-tier `Route` mechanism that *enforces* this is Stage 1 and is out of scope here; this is the *policy statement* the programme runs under in the interim.)
- **This feature's own tier:** it touches new build/library projects and the solution wiring, so the standard build/test/surface gates apply to those projects; it changes no governance prose or consumer contract, so it does **not** trigger the full consumer pipeline beyond the no-regression sequence.
- **Named dogfood features** (must exercise the full Spec Kit + evidence pipeline regardless of tier, keeping the harness honest): **Stage 1 (two-tier process)** and **Stage 4 (Python evidence-engine port)**.

This statement is mirrored from the implementation plan (`docs/reports/2026-05-31-1049-foundations-implementation-plan.md` §Stage 0.3 / §Decisions) and is the authoritative record for SC-007. The baseline doc links to this section.

## Phase 0 — Research

See [research.md](./research.md). All spec assumptions were pre-resolved with the maintainer (D1, D2, D4, D6); the one genuine unknown — FAKE-as-library from a compiled exe without FCS — is the spike itself and is captured as the primary research question with a documented confirm/fallback decision rule.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — the entities this feature *produces* (baseline record, golden-fixture set, ADR, spike-outcome record, meta-process record) with their fields, validation rules, and the no-regression invariant set.
- [contracts/](./contracts/) — the verifiable contracts: the spike target contract, the golden-fixture reproducibility contract, and the no-regression contract (existing targets/surface unchanged).
- [quickstart.md](./quickstart.md) — exact commands a reviewer runs to reproduce the baseline counts, regenerate-and-diff the golden fixtures, build the two new projects, run the spike target, and confirm the serialized gate sequence stays green.
- Agent context: the `AGENTS.md` SPECKIT marker is updated to point at this plan.

## Re-evaluation (post-design Constitution Check)

**PASS (unchanged).** The Phase 1 design adds only documents, fixtures, two `.fsi`-curated build-tooling projects, central dependency entries, and two solution entries. No runtime code, no existing `.fsi`, no existing target, and no surface baseline is modified. Tier 1 obligations are met for the new projects (curated `.fsi`, disclosed dependencies). The only design risk that could escalate scope — a FAKE→FCS transitive dependency — is explicitly gated by an FCS-absence check (FR-012) and, if it fails, routes to the documented fallback rather than silently adding a forbidden dependency.
