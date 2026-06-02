# Implementation Plan: V3 Stage 5 Closeout — Delete `src/Lib`, Decommission `FS.Skia.UI`, Enforce & Measure

**Branch**: `053-v3-monolith-retirement` | **Date**: 2026-06-02 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/053-v3-monolith-retirement/spec.md`

## Summary

The final stage of the V3 modular-distribution programme. Stages 0–4 (features
048/050/051/052) extracted the host, closed the modularity leak, relocated
`AgentValidation`, and rehomed the rich keyboard runtime into `FS.Skia.UI.Input`.
After Stage 4 the **only `ProjectReference` consumer** of `src/Lib` is
`tests/Package.Tests`, and `src/Lib` is otherwise a reference-free husk holding
just the `Parity` evidence helper (`Library.fs(i)`, 142+61 LOC) and
`InternalsVisibleTo.fs` (no `VulkanStartup`/`VulkanResources` residue survives —
those moved in Stage 1).

This feature completes the retirement:

1. **Decouple the last consumer** — rewrite `tests/Package.Tests`'s monolith
   *packaging-contract* assertions against the split packages (or retire with
   justification) and drop its conditional `Lib.fsproj` reference; remove the
   `Parity` helper from `src/Lib/Library.fs(i)`.
2. **Delete `src/Lib`** — `git rm` the project, remove it from the solution,
   `packProjects` (Helpers.fs), the `AsteroidsFeedbackSkillGuidanceTests`
   packable enumeration, and **every path-string reference** repo-wide (the
   Stage-2 lesson: a deleted file is referenced by *path*, not just symbol —
   ~14 such call sites exist across `tests/Governance.Tests`, `tests/Controls.Tests`,
   and `build/Governance`).
3. **Stop publishing `FS.Skia.UI`** — drop from `packProjects` / the pack-version
   flow / `docs/reports/dependencies.md`; confirm no CPM or template pin names it.
4. **Route-gate the per-package surface baselines** — add `PerPackageSurfaceDiff`
   to the existing `package-surface` `Routing.fs` rule's `RequiredGates` and to the
   `knownGates` allowlist (now governance config in `FS.Skia.UI.Build` since Stage 2),
   so an unrecorded per-package `.fsi` change fails the gate.
5. **Add a generated-project cleanliness gate** asserting a generated default `app`
   contains no `samples/`, framework docs set, historical `specs/`, or framework
   README copy.
6. **Publish closeout docs + measurement** — the V2→V3 migration guide, ADR 0012
   (programme closeout), and the after-baseline `2026-06-02-v3-after.md` mirroring
   the Stage-0 before-baseline.

**Approach:** deletion + governance/enforcement only — **no runtime `src/**` code
moves this stage** (all runtime moved in Stages 1–4). The change escalates via
`Route` (it touches `template/**`-adjacent governance, public-`.fsi` routing, and
the pack flow) and is dogfood. Output parity is already proven and preserved in
the split-package suites; nothing here re-renders.

## Technical Context

**Language/Version**: F# / .NET 10 (`net10.0`, `TreatWarningsAsErrors`,
`FS0078`-as-error, Central Package Management)
**Primary Dependencies**: none added. The nine in-scope split packages
(`FS.Skia.UI.Scene` … `.Testing`) keep their identities; the `FS.Skia.UI`
monolith identity is **removed**. `FS.Skia.UI.Scene` stays FSharp.Core-only.
**Testing**: Expecto + FsCheck; FAKE targets (`Dev`, `PerPackageSurfaceDiff`,
`GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`,
`EvidenceAudit`, `TargetMetadataDrift`, `SkillSyncCheck`); deterministic
scene-output parity oracle (headless, preserved from Stage 4).
**Target Platform**: Windows and Linux.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

This is a **Tier 1 (contracted change)**: it removes a public package identity
(`FS.Skia.UI`), changes `validation.contract.yml` (a contract artifact), and
edits public-`.fsi` routing. The full artifact chain (spec, plan, surface-baseline
posture, test evidence, docs) applies.

### Repository Governance Decisions

- **Template ownership**: The `dotnet new fs-skia-ui` template (`app`/`sample-pack`/
  `governed` profiles) already references **only** split packages — its
  `template/base/Directory.Packages.props` names no `FS.Skia.UI` monolith pin
  (confirmed). So **no `.template.config/template.json` or template-content change is
  required** for the retirement itself. The new generated-project **cleanliness gate**
  (FR-008) asserts the template's already-clean shape; if it is implemented as a
  `GeneratedProductCheck` extension it changes `build/Governance/GeneratedProduct.fs`,
  not template content. **Decision: no template content change; template currency
  (`TemplateCheck`/`TemplateDrift`) re-verified as a gate, and the cleanliness gate
  asserts the existing clean shape.**
- **Dependency impact**: No external `PackageVersion` added or removed —
  `Directory.Packages.props` (root) and `template/base/Directory.Packages.props`
  name no monolith pin, so both are **unchanged** by the unpublish (confirmed; FR-005
  is a verify-only obligation). `docs/reports/dependencies.md` **changes**: the
  monolith row + the historical `SkiaViewer → FS.Skia.UI` leak note are removed and
  the preferred-package list affirmed (FR-004). `DependencyReport` re-runs to confirm
  the package graph stays acyclic and `FS.Skia.UI.Scene` stays FSharp.Core-only
  (SC-008). **Decision: drop the monolith from dependency docs; no CPM/external pin
  change; re-verify graph acyclicity.**
- **Command-surface impact**: `build.fsx`/`Dev`/`Verify`/`Ci` are unchanged.
  **`Routing.fs` changes** — `Targets.PerPackageSurfaceDiff` (already a `Targets` DU
  case) is added to the existing `package-surface` rule's `RequiredGates`
  (`build/Governance/Routing.fs:201`), so a public-`.fsi` change Route-selects it;
  the stale `knownGates` comment at `Routing.fs:214` is corrected (FR-013).
  **`knownGates` changes** — `"PerPackageSurfaceDiff"` is added to the allowlist in
  `build/Governance/AgentValidation.fs` so the contract validator recognises the new
  required gate. `validation.contract.yml` is **regenerated** from `Routing.fs`; a
  new/extended generated-project **cleanliness gate** is added. `TargetMetadataDrift`
  (contract currency vs `Routing.fs`) and `SkillSyncCheck` MUST stay green.
  FAKE-backed targets run in the deterministic serialized order, never concurrently:
  1. `./fake.sh build -t Route`           *(confirms escalation + required artifacts)*
  2. `./fake.sh build -t Dev`
  3. `./fake.sh build -t GeneratedGuidanceCheck`
  4. `./fake.sh build -t TemplateCheck`
  5. `./fake.sh build -t GeneratedProductCheck`
  6. `./fake.sh build -t EvidenceGraph`
  7. `./fake.sh build -t EvidenceAudit`

  plus `./fake.sh build -t PerPackageSurfaceDiff` for the per-package baselines and
  `./fake.sh build -t TargetMetadataDrift` for contract currency. **Decision: add the
  `PerPackageSurfaceDiff` routing rule + `knownGates` entry + cleanliness gate; run
  the escalated serialized order whichever gates `Route` prints (authoritative).**
- **Generated project impact**: Behaviourally unchanged — a generated default `app`
  already references split packages only and does not pull the monolith transitively
  (true since Stage 1). What changes is **enforcement**: the cleanliness gate (FR-008)
  newly *asserts* the generated `app` has no `samples/`, no framework docs set, no
  historical `specs/`, no framework README copy, and references packages rather than
  copying framework projects — and rejects a planted instance of each. `GeneratedProductCheck`
  must stay green. **Decision: no change to generated contents; add a cleanliness gate
  that asserts and defends the already-clean shape.**
- **Evidence paths**: Per-feature readiness notes under
  `specs/053-v3-monolith-retirement/readiness/`; the standard repo-root governance
  policy docs already exist and are re-verified for `Route --enforce`
  (`readiness/{validation-contract,evidence-graph,evidence-audit,evidence-policy-separation,package-surface-expectations}.md`).
  New/changed artifacts: the **no-consumer grep proof** (zero `Lib`/`FS.Skia.UI`
  monolith references across `src/** samples/** tests/** template/** build/**`,
  recorded in the feature readiness notes); the **after-baseline**
  `docs/reports/_baselines/2026-06-02-v3-after.md` (each metric with its reproduction
  command); the **`PerPackageSurfaceDiff` enforcement evidence** (an unrecorded
  one-package `.fsi` edit fails the gate; recording the baseline clears it); the
  **cleanliness-gate** green run on a generated `app`; the **V2→V3 migration doc**
  under `docs/`; **ADR 0012**. The nine per-package baselines under
  `readiness/per-package-surface/` are unchanged by this stage (the monolith never had
  one). FAKE logs land in `logs/` (gitignored).
- **`.fsi` / contract impact**: **Yes — two distinct contracts change.** (1)
  `src/Lib/Library.fsi` is **deleted** (the `Parity` helper + `ParityReport`/
  `ParityStatus`/`EvidenceType`/`ParityEvidenceItem` types retire with the monolith).
  (2) `validation.contract.yml` **changes** — the `PerPackageSurfaceDiff` gate is
  rendered into the `package-surface` rule's `required_gates`; currency vs `Routing.fs`
  (`TargetMetadataDrift`) MUST hold. No *split-package* `.fsi` changes — no runtime
  surface moves this stage, so the nine per-package surface baselines stay at zero
  drift. The aggregate `PackageSurfaceCheck` baseline sheds the monolith's
  `FS.Skia.UI.*` `ParityReport`/`Parity` types (recorded). `Package.Tests`'
  packaging-contract assertions are rewritten against the split packages.
- **MVU/effect boundary**: N/A for new behaviour — **no stateful/I/O workflow,
  command, effect, subscription, or interpreter behaviour changes this stage**. All
  runtime (including the Elmish-style rich input model) moved and was parity-proven in
  Stages 1–4. This stage deletes dead code and adds governance/enforcement; the
  generated `app` runtime is unchanged. The governance gates themselves
  (`PerPackageSurfaceDiff`, cleanliness) are pure file-diff validators, not MVU
  workflows.
- **Synthetic evidence**: **None planned.** The retirement uses real deletions, real
  package references, real gate runs, and the real deterministic scene-output oracle.
  The `PerPackageSurfaceDiff` enforcement proof is a **real reverted `.fsi` edit** that
  fails the gate (not a mock). No `[S]`/`[S*]`/`[SEH]` disclosure expected.
  Reference-screenshot re-capture remains **headless-GPU-infeasible** and is disclosed
  as such (Principle V infeasibility disclosure, corroboration-only — the scene-output
  oracle is authoritative); it is not synthetic evidence.
- **Test evidence**: Failing-first where it applies — the rewritten `Package.Tests`
  packaging-contract assertions are authored against the split-package shape and must
  go red on the old monolith expectation before the rewrite, then green. The
  `PerPackageSurfaceDiff` Route-selection gains an Expecto assertion in
  `tests/Governance.Tests/RoutingTests.fs` (the `src/**/*.fsi` rule now also requires
  `PerPackageSurfaceDiff`); its `src/Lib/Foo.fsi` test inputs are repointed to a live
  package path (e.g. `src/Scene/Foo.fsi`) since `src/Lib` is gone. Governance tests
  that name `src/Lib` paths (`AsteroidsFeedbackSkillGuidanceTests`,
  `DependencyGovernanceTests`, `RuntimeOrganizationTests`, `PublicRecordInvariantTests`,
  `ControlsBoundaryCompositionTests`, `AgentValidationFrameworkTests`,
  `Controls.Tests/DiagnosticsTests`) are each triaged: drop the monolith from
  enumerations, repoint generic-`.fsi`-example inputs, and keep negative/illustrative
  diagnostic-string cases that survive deletion. The full `Dev` suite is green with
  zero `Lib` references.
- **Observability**: `PerPackageSurfaceDiff` emits an actionable diff naming the
  package whose `.fsi` drifted and the missing baseline path; the cleanliness gate
  fails with the specific forbidden artifact it found (named `samples/` / docs /
  `specs/` / README path) rather than a generic failure. `Route --enforce` names any
  missing escalated evidence artifact and the requiring tier. Missing-baseline /
  missing-after-report conditions fail their gates with a named artifact path
  (Principle VII).
- **Deferred scope**: The `Charts`/`DataGrid` package split, new first-class template
  profiles (`headless-scene`, `full-governed`, `sample-pack` switches beyond today's),
  any dynamic/plugin loader (no FCS, no runtime script loading), and
  reference-screenshot re-capture (headless-GPU-infeasible) remain **out of scope**
  (Non-Goals, carried from `v3Design.md`). This is the programme's final stage — no
  further V3 stage follows; remaining roadmap items (the separate `Charts` package) are
  explicitly future work.

**Post-design re-check:** the design (Phase 1 below) introduces no new public surface
and no new dependency; it removes one package identity and adds two governance gates.
The Constitution Check still holds — Tier 1 obligations are met by the `.fsi` deletion
record, the regenerated `validation.contract.yml` + currency check, the rewritten
packaging-contract tests, and the migration/ADR/after-baseline docs.

## Project Structure

```
src/
  Lib/                                 # DELETED this feature
    Library.fs(i)                      #   Parity helper + ParityReport types — git rm
    InternalsVisibleTo.fs              #   git rm
    Lib.fsproj                         #   git rm; removed from .sln + packProjects
FS-Skia-UI.sln                         # Lib project entry removed

tests/
  Package.Tests/
    Package.Tests.fsproj               # drop conditional ..\..\src\Lib\Lib.fsproj ref
    Tests.fs                           # rewrite packProjects/PackLocal monolith asserts
  Governance.Tests/                    # path-string sweep — drop/repoint src/Lib paths
    AsteroidsFeedbackSkillGuidanceTests.fs   #   drop the FS.Skia.UI packable enumeration row
    DependencyGovernanceTests.fs             #   drop src/Lib/Lib.fsproj entries
    RuntimeOrganizationTests.fs              #   drop src/Lib/Library.fs entry
    PublicRecordInvariantTests.fs            #   drop src/Lib/Library.fsi entry
    ControlsBoundaryCompositionTests.fs      #   drop "src/Lib" entry
    AgentValidationFrameworkTests.fs         #   repoint stale src/Lib/AgentValidation.fsi rule input
    RoutingTests.fs                          #   repoint src/Lib/Foo.fsi inputs; assert PerPackageSurfaceDiff
  Controls.Tests/
    DiagnosticsTests.fs                #   triage src/Lib diagnostic-string examples (keep/repoint)

build/Governance/
  Routing.fs                           # add PerPackageSurfaceDiff to package-surface rule; fix :214 comment
  AgentValidation.fs                   # add "PerPackageSurfaceDiff" to knownGates allowlist
  Front/Helpers.fs                     # remove "src/Lib/Lib.fsproj","FS.Skia.UI" from packProjects
  PerPackageSurface.fs                 # update stale monolith-exclusion comment (:29)
  GeneratedProduct.fs                  # add/extend generated-project cleanliness gate (FR-008)
validation.contract.yml               # REGENERATED from Routing.fs (PerPackageSurfaceDiff in required_gates)

docs/
  reports/dependencies.md              # drop monolith row + leak note (FR-004)
  reports/_baselines/2026-06-02-v3-after.md   # NEW — after-measurement (FR-010)
  migration/v2-to-v3.md                # NEW — V2→V3 surface map + ref-move steps (FR-009)
  adr/0012-monolith-retirement-closeout.md    # NEW — programme closeout (FR-011)

specs/053-v3-monolith-retirement/readiness/   # per-feature readiness notes + no-consumer grep proof
```

## Phasing within this feature

1. **Decouple the last consumer.** Rewrite `Package.Tests` packaging-contract
   assertions against the split packages (FR-001); remove the `Parity` helper from
   `Library.fs(i)` (FR-002); drop the `Lib.fsproj` `ProjectReference`. Suite green.
2. **Path-string sweep + delete.** Remove every `src/Lib`/`Lib.fsproj`/`FS.Skia.UI`
   monolith reference across `tests/**` and `build/**` (the ~14 call sites), then
   `git rm src/Lib`, remove from `.sln` and `packProjects` (FR-003/004/006). Grep proof.
3. **Unpublish + dependency docs.** Drop the monolith from `packProjects`/pack-flow and
   `docs/reports/dependencies.md`; verify no CPM/template pin names it (FR-004/005).
4. **Route-gate the per-package baselines.** Add `PerPackageSurfaceDiff` to the
   `package-surface` rule + `knownGates`; regenerate `validation.contract.yml`; fix the
   stale `Routing.fs:214` comment; prove an unrecorded `.fsi` edit fails the gate
   (FR-007/013, SC-004). `TargetMetadataDrift` green.
5. **Cleanliness gate.** Add/extend the generated-project cleanliness gate; prove it
   green on a generated `app` and red on planted `samples/`/docs/`specs/`/README (FR-008).
6. **Closeout docs + measurement.** V2→V3 migration doc (FR-009), ADR 0012 (FR-011),
   after-baseline (FR-010); settle `ParityGallery`/Parity-oracle residue per ADR 0010
   and clean governance lists naming `tests/Parity.Tests` (FR-012).
7. **Verify.** `src/Lib` gone + reference-free; escalated gate set + `PerPackageSurfaceDiff`
   + `TargetMetadataDrift` green; `EvidenceAudit` PASS with zero synthetic (SC-009).
```

## Closeout artifacts (FR-009/010/011)

- V2→V3 migration guide: [`docs/migration/v2-to-v3.md`](../../docs/migration/v2-to-v3.md)
- Programme closeout: [`docs/adr/0012-monolith-retirement-closeout.md`](../../docs/adr/0012-monolith-retirement-closeout.md)
- After-measurement baseline: [`docs/reports/_baselines/2026-06-02-v3-after.md`](../../docs/reports/_baselines/2026-06-02-v3-after.md)
