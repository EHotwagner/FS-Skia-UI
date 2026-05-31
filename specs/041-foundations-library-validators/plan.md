# Implementation Plan: Foundations Governance Library — First Real Validators

**Branch**: `041-foundations-library-validators` | **Date**: 2026-05-31 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/041-foundations-library-validators/spec.md`

## Summary

Stage 3 of the foundations programme: move the two cheapest/highest-value validators out of the
4,839-line `build.fsx` into the compiled, unit-tested `FS.Skia.UI.Build` governance library, and
introduce a typed `Target` discriminated union so target identity, dependencies, and metadata derive
from **one** source (making the `TargetMetadataDrift` "second source of truth" structurally
impossible). The two validators are **target-metadata drift**
(`ValidateTargetMetadataDrift`/`validateTargetMetadataAgainstRepo`) and the **capability catalog**
(retiring the hand-rolled YAML parser `readCapabilityCatalog` for a typed model read via the
already-present `YamlDotNet`). The technical approach follows the 040 pattern: new
`build/Governance/*.fs` modules each with a curated `.fsi` (Principle II), `#load`'d into `build.fsx`
so the `CapabilityCheck`, `TargetMetadata`, and `TargetMetadataDrift` interpret cases call the
library **in-process**; correctness is proven by **golden-diff parity** (byte-identical reports) plus
direct Governance.Tests that assert **typed** findings. This is a Tier 2 (internal refactor)
build-tooling change: no runtime `src/**` edit, no product `.fsi` change, no shipped package.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (inherits `Directory.Build.props`: `TreatWarningsAsErrors`,
`FS0078`-as-error, Central Package Management).
**Primary Dependencies**: `YamlDotNet` (already present, build-tooling; reused behind the typed
catalog model — **no new dependency**); `Fake.Core.Target` (already present, front-end only). No new
`PackageVersion` outside `Directory.Packages.props`.
**Testing**: Expecto (`tests/Governance.Tests`, already references `FS.Skia.UI.Build.fsproj`); FAKE
targets in the repository's deterministic serialized order; golden-fixture byte-diff under the
existing `Dev`/test gate (no new FAKE target — FR-008a).
**Target Platform**: Windows and Linux (build-tooling; no runtime/visual surface touched).

**Resolved unknowns** (see [research.md](./research.md)): the three reports' Stage-0 golden fixtures
do **not** yet exist (Stage 0 captured only `EvidenceGraph`/`EvidenceAudit` fixtures); they are
captured from the pinned pre-extraction baseline as the first task (R1). The `TargetMetadata` JSON's
`generated_at_utc` field is non-deterministic and must be normalized for byte-diff (R2). Metadata is
derived from the DU via a total `spec : Target -> TargetSpec` function so totality makes drift
unrepresentable (R3).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change tier (spec):** Tier 2 (internal change — refactor/extraction with no product behavioural or
public-API change). Output parity is the explicit correctness contract. New **build-tooling** `.fsi`
companions are required by Principle II even at Tier 2; no product surface baseline moves.

**Principle compliance (re-checked post-Phase 1 — PASS):**
- **I (Spec→FSI→tests→impl):** The new public surface is drafted as `.fsi` in
  [contracts/](./contracts/) before any `.fs` body; Governance.Tests exercise those signatures
  (failing-first because the modules do not yet exist), then the `.fs` is written. ✔
- **II (Visibility in `.fsi`):** Every new module (`Findings`, `Targets`, `TargetMetadata`,
  `Capabilities`) ships a curated `.fsi`; no `private`/`internal`/`public` in `.fs` (FR-007). ✔
- **III (Idiomatic simplicity):** Plain DU + records + pure functions; the bespoke YAML state machine
  is *removed*, not replaced with cleverness. The one `mutable`-bearing parser is deleted. No SRTP,
  reflection, type providers, or non-trivial CEs introduced. ✔
- **IV (MVU boundary):** No new stateful workflow. Extracted validators are **pure** over typed
  inputs; all I/O (reading `template/capabilities.yml`, `validation.contract.yml`, docs; writing the
  three reports) stays at the `build.fsx` interpreter edge exactly as today. The build's existing
  `BuildMsg`/`BuildEffect`/`update`/`interpret` MEL boundary is preserved; engine relocation is
  Stage 5, explicitly out of scope (FR-001a). ✔
- **V (Synthetic disclosure):** No synthetic evidence anticipated — golden fixtures are *real*
  captured outputs, unit-test inputs are real crafted typed values exercising real error paths (not
  `[SEH]`). No `[S]`/`[SEH]` tasks expected. ✔
- **VI (Test evidence):** Failing-first Governance.Tests (≥6 typed-finding cases, SC-004) + golden
  byte-diff assertions (FR-008a). ✔
- **VII (Observability):** Every preserved diagnostic category/message is reproduced; a missing
  library DLL reference at extraction time is surfaced explicitly as the Stage-5 trigger (edge case
  3), never silently falls back to inline logic. ✔

### Repository Governance Decisions

- **Template ownership:** No `.template.config/template.json` change. `template/capabilities.yml` is
  *retained* as the source-of-truth data file (consumed by template generation); only the bespoke
  reader is retired. No template profile/fragment/contents change.
- **Dependency impact:** No new dependency. `YamlDotNet` is already in `Directory.Packages.props` and
  already referenced transitively in the Governance.Tests output. No `docs/reports/dependencies.md`
  row added (FR-012); `DependencyReport` coverage unchanged.
- **Command-surface impact:** **No new FAKE target.** `CapabilityCheck`, `TargetMetadata`,
  `TargetMetadataDrift` keep their names, dependencies, outputs, graph positions, and meaning within
  `Dev`/`Verify`; only their interpret-case **bodies** change to call the library, and the dispatch
  *mechanism* converts from `StartTarget of string` to a typed `Target` union for **all** arms
  (FR-001). FAKE-backed validation runs in the serialized order (`Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` →
  `EvidenceAudit`); never concurrent.
- **Generated project impact:** None. No package ships into any generated product (FR-012); generated
  contents, selected guidance, local skills, validation logs unchanged.
- **Evidence paths:** `specs/041-foundations-library-validators/readiness/` — parity reports showing
  empty golden-diff for the three reports; `build.fsx` line-count delta (before/after, SC-001); new
  Governance.Tests results; serialized FAKE gate logs; `git diff src/** = empty` proof (SC-007).
  Golden fixtures committed under `tests/Governance.Tests/fixtures/reports-golden/`.
- **`.fsi` / contract impact:** New **build-tooling** `.fsi` files only (`Findings.fsi`,
  `Targets.fsi`, `TargetMetadata.fsi`, `Capabilities.fsi`). No product `.fsi`, no surface baseline,
  no sample contract changes (FR-011). `PackageSurfaceCheck` / `FsiTranscripts` show no baseline diff.
- **MVU/effect boundary:** Preserved (see Principle IV above). No new `Model`/`Msg`/`Effect`.
- **Synthetic evidence:** None planned. Any discovered case returns to task review (no
  implementation-time relabeling, Principle V).
- **Test evidence:** Failing-first typed-finding unit tests + golden byte-diff assertions, under the
  existing `Dev`/test gate.
- **Observability:** Every existing diagnostic category and exact message string is preserved
  (the golden-diff and the typed-finding tests both enforce this). Missing-DLL-at-extraction is an
  explicit surfaced failure (Stage-5 trigger), not a silent inline fallback.
- **Deferred scope:** Stage 4 (Python evidence port), Stage 5 (MEL engine relocation, dedicated
  build front-end / `build.fsx` retirement, `Routing.fs`), Stage 1 (two-tier `Route`), Stage 2
  (single-source generation beyond the shipped sync check) — all out of scope (Framework Governance
  Prompts §Unsupported scope).

**Gate result: PASS** (no unjustified violations; no NEEDS CLARIFICATION remaining after research).

## Project Structure

```
build/Governance/
  FS.Skia.UI.Build.fsproj      # +4 module pairs added to <Compile> (after Spike/SkillSync/SkillExamples)
  Findings.fsi / Findings.fs   # NEW — uniform ValidationFinding type + finding ctor + render (FR-004)
  Targets.fsi / Targets.fs     # NEW — typed Target DU, dep graph, requiredTargets/deps derivation (FR-001)
  TargetMetadata.fsi / .fs     # NEW — TargetMetadata record + drift DU + Validate*/render (FR-002)
  Capabilities.fsi / .fs       # NEW — CapabilityRow model, YamlDotNet-backed reader, validate/render (FR-003)
  (existing Spike/SkillSync/SkillExamples unchanged)

build.fsx                      # SHRINKS ≥800 lines: removes moved validators + bespoke parser +
                               #   string registries; #loads the 4 new modules; converts ALL
                               #   StartTarget "..." arms to typed Target dispatch; the three
                               #   interpret cases call FS.Skia.UI.Build.* in-process (FR-005)

tests/Governance.Tests/
  Governance.Tests.fsproj      # already references FS.Skia.UI.Build.fsproj; +3 test files in <Compile>
  TargetMetadataTests.fs       # NEW — ≥3 typed drift-class cases (SC-004)
  CapabilityCatalogTests.fs    # NEW — ≥3 typed catalog error-class cases (SC-004)
  ReportParityTests.fs         # NEW — byte-equality of the 3 reports vs golden fixtures (FR-008a)
  fixtures/reports-golden/     # NEW — captured pre-extraction baseline reports (parity oracle, R1)
    capability-catalog.md
    target-metadata.json       # generated_at_utc normalized (R2)
    target-metadata-drift.md
```

## Phase 0 — Research

Output: [research.md](./research.md). Three unknowns resolved (golden-fixture provenance gap,
`generated_at_utc` normalization, the derive-from-DU mechanism), plus the coupling analysis of
`targetMetadata`→`focusedGateContract`.

## Phase 1 — Design & Contracts

Outputs:
- [data-model.md](./data-model.md) — entities (`Target`, `TargetSpec`, `TargetMetadata`,
  `TargetMetadataDrift`, `ValidationFinding`, `CapabilityRow`/`CapabilityCatalog`, `GoldenFixture`)
  with fields, derivation rules, and the state/validation transitions.
- [contracts/](./contracts/) — the curated `.fsi` signatures for the four new modules (the
  Principle I FSI sketch, validated before `.fs` exists).
- [quickstart.md](./quickstart.md) — the extraction + parity-capture + verification recipe.
- Agent context updated: `AGENTS.md` SPECKIT marker repointed to this plan.

**Post-design Constitution re-check: PASS** (recorded above).

## Phase 2 — (planning stops here)

Task breakdown is produced by `/speckit-tasks`; not generated by this command.
