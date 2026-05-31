# Phase 0 Research: Foundations Baseline & Build-Library Spike

## Status of unknowns

The spec carries no open `NEEDS CLARIFICATION` markers: the shaping decisions (D1, D2, D4, D6) are already resolved with the maintainer and this feature *records* them. The only genuine technical unknown is the spike's subject. Each item below is either a resolved decision being recorded or the spike's confirm/fallback rule.

---

## R1 — Can FAKE's Target API be driven from a compiled `dotnet run` exe without FSharp Compiler Services? (the spike's core unknown — D5)

- **Decision**: Build the dedicated front-end as a normal compiled F# `Exe` that takes a `PackageReference` on `Fake.Core.Target` (and the minimal companion `Fake.Core.*` packages its API requires), defines targets in `Program.fs` via the modular `Target.create` / `Target.runOrDefault` API, and invokes them with `dotnet run --project build/Build.fsproj -- <target>`. The target *body* calls `FS.Skia.UI.Build.Spike.<fn>` in the referenced library — no logic is inlined in the front-end.
- **Rationale**: FAKE 5/6 is explicitly modular — the `Fake.Core.Target` package is a plain library whose `Target.create`/`runOrDefault` functions work in any compiled assembly. The FSX *script runner* (which pulls FSharp Compiler Services to compile `build.fsx` at invocation) is a *separate* concern; consuming the Target API as a library does not require it. This is precisely the property D2 bets on (compiled, IDE-grade, `dotnet run` builds the whole project graph with no DLL bootstrap-order wrinkle), and the spike's job is to prove it on a one-target slice before Stage 5 commits to it.
- **Alternatives considered**:
  - *Thin `build.fsx` `#r` shim (the documented fallback)*: keep `dotnet fake` but `#r` the compiled DLL from a <200-line script. Rejected as the *primary* path because it retains the per-invocation FSX compile tax this programme exists to remove; retained **only** as the fallback Stage 5 takes **if** this spike surfaces a concrete blocker.
  - *Custom MSBuild targets / Nuke / Cake*: rejected — changes the orchestration model wholesale and is out of scope; D2 is already decided as FAKE-as-library.
- **Confirm/fallback decision rule (FR-007, SC-004)**:
  - **"D2 confirmed"** ⟺ both new projects compile clean under `net10.0`/`TreatWarningsAsErrors`, the front-end runs the one trivial target through `dotnet run` to reported success, the target body demonstrably executed from the library (not inlined), **and** no `FSharp.Compiler.Service` package appears in the restored graph (FR-012).
  - **"fallback triggered"** ⟺ any of the above cannot be achieved; the exact blocker (error text, package, command) is recorded reproducibly and the thin-`build.fsx` shim is documented as the Stage 5 path.
  - **Failure (neither)** ⟺ an ambiguous result with no recorded confirm and no reproducible blocker. The spike is not allowed to end here (spec Edge Cases).

## R2 — FCS-absence verification method (FR-012)

- **Decision**: After `dotnet restore`/build of `build/Build.fsproj`, assert no `FSharp.Compiler.Service` (or `FSharp.Compiler.*`) package is present, via `dotnet list build/Build.fsproj package --include-transitive` filtered for the package, recorded in the spike-outcome doc.
- **Rationale**: The whole point of compiled-config (D6) and dedicated-project (D2) is to remove the runtime-compile tax; a transitive FCS pull would defeat it and violate FR-012. Making the check explicit turns a silent regression into a fallback trigger.
- **Alternatives considered**: trusting the package author's dependency graph (rejected — must be proven, not assumed).

## R3 — Which FAKE packages are the *minimal* set?

- **Decision**: Start with `Fake.Core.Target` only; add `Fake.Core.Process`/`Fake.IO.FileSystem` *only if* the trivial target genuinely needs them (the spike's trivial target should need neither). Each added package gets a central `PackageVersion` in `Directory.Packages.props` and a `docs/dependencies.md` row.
- **Rationale**: Constitution's dependency-minimisation rule; the spike must demonstrate the *thinnest* viable dependency surface so Stage 5 inherits a clean baseline. Fewer packages also shrinks the FCS-transitive-pull risk surface.
- **Alternatives considered**: pulling the FAKE meta-package (rejected — drags in the script runner and FCS, directly contradicting FR-012).

## R4 — Governance-library placement (recording D1)

- **Decision**: Library project at `build/Governance/FS.Skia.UI.Build.fsproj`; the repo build front-end project-references it in-solution. (Distribution to generated consumers via a *published package* is the D1 end-state but is exercised only in Stage 4/5; this feature creates the project, it does not pack or publish it.)
- **Rationale**: Placing it under `build/` (not `src/`) keeps it out of the runtime package set and the runtime surface-baseline tooling, while co-locating it with the front-end that drives it. Matches the implementation plan's `build/Governance/` path.
- **Alternatives considered**: `src/Build/` (rejected — risks being swept by runtime surface checks and read as a shipped runtime package); a separate repo (rejected — D1 chose in-solution project reference + later published package).

## R5 — Golden-fixture determinism & feature selection (FR-002, FR-003)

- **Decision**: Capture three frozen, already-merged features (`038-authoring-guidance-consistency`, `037-authoring-audit-robustness`, `017-synthetic-error-evidence`) via the *existing* `EvidenceGraph`/`EvidenceAudit` path, archiving `task-graph.json`, `task-graph.md`, and the audit count block. Re-run and byte-diff to prove reproducibility before committing. Pin the capture commit SHA in the baseline.
- **Rationale**: Frozen features cannot drift; the three chosen cover the audit's status vocabulary (including `[SEH]`/synthetic propagation via 017). The existing engine is consumed unchanged (FR-011) so the fixtures are an honest "before" oracle for the Stage 4 port.
- **Determinism risk**: if any re-run does not reproduce byte-for-byte, the non-determinism is identified (e.g. timestamp/ordering in the output) and the fixture is re-captured deterministically, or the feature is substituted and the substitution recorded (spec Edge Cases). An unstable fixture is never committed.
- **Alternatives considered**: using the in-flight feature 039 as a fixture source (rejected — its `tasks.md` is not frozen at capture time, producing a moving-tree oracle).

## R6 — Baseline counts: exact measurement commands (FR-001)

- **Decision**: Record each metric with the literal command used, so a reviewer reproduces it:
  - `build.fsx` size: `wc -l build.fsx` (4,688 at authoring) + a function-level orchestration-vs-validation breakdown derived from the `interpret`/`StartTarget` cases vs the `Validate*` functions.
  - Governance Markdown: `wc -l` over `.claude/skills/**`, `.agents/skills/**` (byte-identical mirror), `.specify/memory/constitution.md`, templates, and `specs/**`.
  - Language LOC mix: `git ls-files | grep -E '\.(fs|fsx|fsi)$|\.sh$|\.py$'` piped to `wc -l`, split F# / Bash / Python.
  - Ceremony-time estimate: record the current ~12–14h/feature figure from the implementation plan, labelled an estimate.
- **Rationale**: Every later-stage reduction claim is checked against these exact commands at the pinned SHA; ambiguity here makes the whole programme unfalsifiable.
- **Alternatives considered**: prose-only descriptions (rejected — not reproducible).

## R7 — No-regression guarantee for existing targets (FR-009, FR-010, SC-006)

- **Decision**: Treat all five spec invariants as standing acceptance gates: no runtime-source edits; no `.fsi`/surface-baseline diff; existing targets' behaviour/output unchanged; new projects inherit `Directory.Build.props`; FAKE-backed validation runs in the canonical serialized order. Adding the two projects to `FS-Skia-UI.sln` is additive — they compile under `Dev` but change no existing target's *output*.
- **Rationale**: The feature's entire value is being safe; a regression here would defeat its purpose.
- **Verification**: run the canonical serialized sequence (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`) and `PackageSurfaceCheck`/`FsiTranscripts`; all green, no baseline diff.

---

## Consolidated decisions

| # | Decision | Rationale (short) | Recorded in |
|---|---|---|---|
| R1 | FAKE `Target` API as a library from a compiled `dotnet run` exe; delegate body to library | Proves D2 cheaply; modular FAKE supports it without FSX runner | spike-outcome doc + ADR 0002 |
| R2 | Explicit FCS-absence check after restore | FR-012; turns silent regression into fallback trigger | spike-outcome doc |
| R3 | Minimal FAKE package set (`Fake.Core.Target` first) | Dependency minimisation; shrinks FCS-pull risk | `Directory.Packages.props`, `docs/dependencies.md` |
| R4 | Library at `build/Governance/FS.Skia.UI.Build.fsproj`, in-solution reference | Out of runtime set; matches D1 | ADR 0001 |
| R5 | Three frozen features for golden fixtures (038, 037, 017) | Frozen + diverse evidence shapes | baseline doc + fixtures |
| R6 | Literal measurement commands per metric | Reproducibility; falsifiable claims | baseline doc |
| R7 | Five invariants as standing gates; sln additions additive | Feature's core promise is safety | quickstart + no-regression contract |
