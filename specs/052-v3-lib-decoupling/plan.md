# Implementation Plan: V3 Stage 3–4 Residual — Decouple Remaining Consumers from `src/Lib`

**Branch**: `052-v3-lib-decoupling` | **Date**: 2026-06-02 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/052-v3-lib-decoupling/spec.md`

## Summary

Remove every remaining consumer reference to the legacy monolith `src/Lib`, so the
Stage 5 deletion of `src/Lib` / unpublishing of `FS.Skia.UI` is a no-consumer
operation. Concretely:

1. **Rehome the rich keyboard-input runtime** (`src/Lib/KeyboardInput.fs(i)`,
   ~1,852 LOC, 38 public types + the `KeyboardInput` module) into a **new dedicated
   split package `FS.Skia.UI.Input`** (project `src/Input`) that references
   `FS.Skia.UI.Scene` + `FS.Skia.UI.SkiaViewer`. This was a maintainer decision
   (2026-06-02): the rich runtime depends on `Scene` + `SkiaViewer.Host`, so it must
   sit **downstream of `SkiaViewer`** — it cannot live in the lean
   `FS.Skia.UI.KeyboardInput` package (that would form
   `KeyboardInput → SkiaViewer → KeyboardInput`). A dedicated package keeps
   `SkiaViewer` lean and makes the rich runtime an **opt-in** capability, matching the
   V3 modularity ethos.
2. **Retire the `Parity.Tests` bridge + the dead `Parity` helper** (`src/Lib/Library.fs(i)`)
   once Stage-1 parity is signed off, folding any still-valuable assertions into the
   split-package suites first.
3. **Settle the `ParityGallery` keep/retire policy** (ADR 0010) and confirm no sample
   references the monolith.

End state: `src/Lib` has **zero** consumers repo-wide (an unreferenced husk), but is
still present and `FS.Skia.UI` still packable — deletion/unpublish is Stage 5.

**Approach:** behaviour-preserving relocation (no semantic change), gated by the
deterministic scene-output parity oracle and the migrated test suites. Follows the
Constitution's Spec → FSI → semantic-tests → implement order; the new package's `.fsi`
is the sole surface declaration (Principle II) and gets its own per-package surface
baseline.

## Technical Context

**Language/Version**: F# / .NET 10 (`net10.0`, `TreatWarningsAsErrors`, `FS0078`-as-error, Central Package Management)  
**Primary Dependencies**: `FS.Skia.UI.Scene` (FSharp.Core-only) and `FS.Skia.UI.SkiaViewer` (Silk.NET/SkiaSharp host); the rich runtime's only external dependency beyond those is whatever it already used in `src/Lib` (no new heavy dependency introduced).  
**Testing**: Expecto + FsCheck unit/property tests; FAKE targets (`Dev`, `PerPackageSurfaceDiff`, `TemplateCheck`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`); deterministic scene-output parity oracle (headless).  
**Target Platform**: Windows and Linux.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: The `dotnet new fs-skia-ui` template (`app`/`governed`
  profiles) references `Scene`/`SkiaViewer`/`Elmish`/`KeyboardInput`/`Layout`/`Controls`
  and does **not** consume the rich input runtime, so `.template.config/template.json`
  needs **no** change for the rehome — the new `FS.Skia.UI.Input` package is sample/opt-in
  only. `TemplateCheck`/`TemplateDrift` must stay green to prove the generated `app` is
  unaffected. **Decision: no template content change; template currency re-verified as a gate.**
- **Dependency impact**: A new packable project `FS.Skia.UI.Input` is added. It pulls
  no new external `PackageVersion`, so `Directory.Packages.props` is **unchanged**.
  `docs/reports/dependencies.md` / `DependencyReport` gain one internal package row (the
  new package + its `Scene`/`SkiaViewer` project references). The sample `InteractiveViewer`'s
  `UsePackedPackage` path references `FS.Skia.UI.Input` from the local feed, so the package
  participates in `PackLocal` and the version-bump flow on merge. **Decision: add the package
  to the pack/version flow; no CPM/external-dependency change.**
- **Command-surface impact**: No new build targets and no `Routing.fs` change (the
  per-package Route-gating rule is Stage 5). `Route` selects the escalated gate set because
  the change touches `src/**/*.fsi` and adds a packable project. FAKE-backed targets run in
  the deterministic serialized order, never concurrently:
  1. `./fake.sh build -t Route`
  2. `./fake.sh build -t Dev`
  3. `./fake.sh build -t GeneratedGuidanceCheck`
  4. `./fake.sh build -t TemplateCheck`
  5. `./fake.sh build -t GeneratedProductCheck`
  6. `./fake.sh build -t EvidenceGraph`
  7. `./fake.sh build -t EvidenceAudit`
  plus `./fake.sh build -t PerPackageSurfaceDiff` for the new/changed baselines.
  **Decision: no command surface change; run the escalated serialized order + PerPackageSurfaceDiff.**
- **Generated project impact**: None. A generated default `app` does not include the rich
  input runtime, must continue to restore/build/run, and must not pull the monolith
  transitively (already true after Stage 1). The rich runtime is sample/opt-in. **Decision:
  generated-product behaviour unchanged; re-verified by GeneratedProductCheck.**
- **Evidence paths**: Readiness notes under `specs/052-v3-lib-decoupling/` and the standard
  repo-root governance docs
  (`readiness/{validation-contract,evidence-graph,evidence-audit,evidence-policy-separation,package-surface-expectations}.md`,
  carried from Stage 2). New/updated artifacts: `readiness/per-package-surface/FS.Skia.UI.Input.fsi.txt`
  (new baseline), updated `readiness/per-package-surface/FS.Skia.UI.fsi.txt` (monolith shrinks),
  the aggregate `PackageSurfaceCheck` baseline (gains `FS.Skia.UI.Input.*`, loses the monolith's
  KeyboardInput/Parity types), and the scene-output parity golden under
  `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/` (the parity sign-off evidence consumed
  before `Parity.Tests` retires). FAKE logs land in `logs/` (gitignored).
- **`.fsi` / contract impact**: **Yes.** The new package `FS.Skia.UI.Input/KeyboardInput.fsi`
  declares the rich runtime's surface — a **namespace rename** of the existing
  `src/Lib/KeyboardInput.fsi` (`namespace FS.Skia.UI` → `namespace FS.Skia.UI.Input`); zero
  `val`/`type`/field/case added, removed, or retyped (mirrors the Stage-2 rename discipline).
  The monolith `FS.Skia.UI` surface **shrinks** (sheds the rich KeyboardInput types + the
  `Parity` helper once retired). New package gets a per-package surface baseline; both baselines
  recorded. `validation.contract.yml` is **unchanged**.
- **MVU/effect boundary**: The rich runtime is an Elmish-style stateful input model:
  `InputRuntime`/`CanonicalInputModel` (Model), `InputMsg` (Msg), `InputEffect` (Effect),
  with `KeyboardInput.*` init/update functions and `ResolvedCommand`/`CommandPlan` outputs.
  This boundary is **preserved exactly** — the relocation is behaviour-neutral. Real
  interpreter evidence = the migrated `KeyboardInputTests.fs` (now exercising the API through
  the new package surface, per Principle I/III).
- **Synthetic evidence**: None planned. The relocation uses real package references and real
  tests; the parity oracle is deterministic scene-output (not a mock). No `[S]` disclosure
  expected. Screenshot re-capture remains headless-GPU-infeasible and is disclosed as such
  (corroboration-only; scene-output is authoritative) — an infeasibility disclosure, not synthetic.
- **Test evidence**: `KeyboardInputTests.fs` migrates from `tests/Lib.Tests` to a new
  `tests/Input.Tests` (referencing `FS.Skia.UI.Input`), preserving assertions (FSI-surface
  exercise per Principle I). `Lib.Tests`'s residual `Tests.fs` content is triaged: anything
  testing the rich runtime travels; anything testing the dead `Parity` helper retires with it.
  `Parity.Tests` assertions of lasting value fold into `SkiaViewer.Tests`/`Scene.Tests` before
  `Parity.Tests` is removed. Failing-first: the new package's tests are written/red before the
  module compiles in its new home.
- **Observability**: The rich runtime's `InputDiagnostic`/`InputDiagnosticCode` diagnostics are
  preserved verbatim. `PerPackageSurfaceDiff` emits an actionable diff naming the package whose
  `.fsi` drifted; the parity oracle emits a byte-level diff vs the Stage-0 golden on mismatch.
  Missing-baseline / missing-golden conditions fail their gates with a named artifact path.
- **Deferred scope**: Deleting `src/Lib`, unpublishing `FS.Skia.UI`, adding the
  `PerPackageSurfaceDiff` `Routing.fs` rule + hard-gate enforcement, the generated-project
  cleanliness gate, V2→V3 migration docs, and the after-measurement are **Stage 5**. The
  `Charts`/`DataGrid` package split, new template profiles, and any dynamic/plugin loader remain
  out of scope (Non-Goals). Reference-screenshot re-capture stays deferred (headless-infeasible).

## Project Structure

```
src/
  Input/                              # NEW packable project — FS.Skia.UI.Input
    Input.fsproj                      #   PackageId FS.Skia.UI.Input; refs Scene + SkiaViewer
    KeyboardInput.fsi                 #   moved from src/Lib; namespace FS.Skia.UI → FS.Skia.UI.Input
    KeyboardInput.fs                  #   moved from src/Lib; body unchanged
  Lib/                                # husk after this feature (deleted in Stage 5)
    KeyboardInput.fs(i)               #   REMOVED (moved to src/Input)
    Library.fs(i)                     #   Parity helper REMOVED once Parity.Tests retires
    InternalsVisibleTo.fs             #   remains until Stage 5
    Lib.fsproj                        #   remains, now reference-free, until Stage 5

tests/
  Input.Tests/                        # NEW — migrated from Lib.Tests/KeyboardInputTests.fs
    Input.Tests.fsproj                #   refs FS.Skia.UI.Input (+ Scene/SkiaViewer as needed)
    KeyboardInputTests.fs
  Lib.Tests/                          # repointed off Lib (or retired if emptied)
  Parity.Tests/                       # RETIRED after parity sign-off (assertions migrated)
  Package.Tests/                      # drop the conditional Lib.fsproj reference
  SkiaViewer.Tests/ , Scene.Tests/    # receive any migrated parity assertions

samples/
  InteractiveViewer/                  # repointed: drop Lib; add FS.Skia.UI.Input
  ParityGallery/                      # keep-vs-retire decision recorded (ADR 0010)

readiness/per-package-surface/
  FS.Skia.UI.Input.fsi.txt            # NEW baseline
  FS.Skia.UI.fsi.txt                  # UPDATED (monolith shrinks)
```

## Phasing within this feature

1. **Create `FS.Skia.UI.Input`** and move `KeyboardInput.fs(i)` into it (namespace rename
   only); add to solution + `PackLocal`/version flow; capture its per-package surface baseline.
2. **Repoint consumers**: `InteractiveViewer`, `Lib.Tests` (→ new `Input.Tests`), `Package.Tests`.
3. **Parity sign-off + retire** `Parity.Tests` and the `Parity` helper; migrate valuable assertions.
4. **Settle `ParityGallery` policy** (ADR 0010) and prove no sample references the monolith.
5. **Verify**: `src/Lib` reference-free; escalated gate set green; `EvidenceAudit` PASS.
