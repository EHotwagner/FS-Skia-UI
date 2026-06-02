# ADR 0012 — V3 monolith-retirement closeout

**Status**: Accepted
**Date**: 2026-06-02
**Feature**: `053-v3-monolith-retirement` (V3 programme Stage 5 — final stage)
**Supersedes/closes**: the V3 modular-distribution programme (ADRs 0007–0011)

## Context

The V3 modular-distribution programme split the broad `FS.Skia.UI` monolith into nine
focused packages across Stages 0–4 (features 048/050/051/052): the Vulkan/Skia host was
relocated into `FS.Skia.UI.SkiaViewer` (ADR 0007), the duplicate scene vocabulary was
collapsed to a single source in `FS.Skia.UI.Scene` (ADR 0008), the `AgentValidation`
surface moved into `FS.Skia.UI.Build` (ADR 0009), the legacy sample policy was settled
(ADR 0010), and the parity oracle method was fixed (ADR 0011). After Stage 4 the only
remaining build-consumer of `src/Lib` was `tests/Package.Tests`, and the monolith held
just the `Parity` evidence helper.

## Decision

Complete the retirement:

1. **Delete `src/Lib`.** `Library.fs(i)` + `InternalsVisibleTo.fs` + `Lib.fsproj` are
   `git rm`'d; the project is removed from `FS-Skia-UI.sln`, from `packProjects`, and from
   the packable enumerations. The `Parity` helper + its `ParityReport`/`ParityStatus`/
   `EvidenceType`/`ParityEvidenceItem` types retire with it.
2. **Stop publishing `FS.Skia.UI`.** It is dropped from `packProjects`/the pack flow and
   from `docs/reports/dependencies.md` (monolith row + the historical `SkiaViewer →
   FS.Skia.UI` leak note removed). No CPM or template pin ever named it (verify-only).
3. **Decouple the last consumer.** `tests/Package.Tests` is rewritten against the
   split-package pack shape and drops its conditional `Lib.fsproj` reference; the aggregate
   `FS.Skia.UI` surface baseline is removed.
4. **Enforce the per-package surface gate.** `PerPackageSurfaceDiff` is added to the
   `package-surface` routing rule's `RequiredGates` and to the `knownGates` allowlist, and
   rendered into `validation.contract.yml` — so a public `src/**/*.fsi` change Route-selects
   the per-package DiffPlex check and an unrecorded `.fsi` edit fails the gate (FR-007).
5. **Assert generated-app cleanliness.** `GeneratedProductCheck` gains a cleanliness gate
   asserting a generated `app`/`governed` profile carries no `samples/`, framework
   `docs/reports/` set, historical `specs/`, or framework root-README copy, and references
   the split packages rather than copying framework projects (FR-008).
6. **Publish closeout artifacts.** The V2→V3 migration guide
   (`docs/migration/v2-to-v3.md`), this ADR, and the after-measurement baseline
   (`docs/reports/_baselines/2026-06-02-v3-after.md`) mirroring the Stage-0 before-baseline.

No runtime `src/**` code moved this stage — all runtime moved and was parity-proven in
Stages 1–4. This stage is deletion + governance/enforcement only; the deterministic
scene-output oracle (ADR 0011) is preserved and authoritative.

## Consequences

- The package set is the nine split packages (`FS.Skia.UI.Scene`, `.SkiaViewer`,
  `.Elmish`, `.KeyboardInput`, `.Input`, `.Layout`, `.Controls`, `.Controls.Elmish`,
  `.Testing`) plus the `FS.Skia.UI.Build` governance engine. The broad `FS.Skia.UI`
  identity no longer exists.
- The package dependency graph is acyclic and `FS.Skia.UI.Scene` stays FSharp.Core-only.
- A repo-wide no-consumer grep over `src samples tests template build *.sln` returns zero
  hits for the monolith identity (SC-001).
- The V3 programme is closed. Remaining roadmap items (a first-class `Charts`/`DataGrid`
  package split, additional template profiles) are explicitly **future work**, not a
  further V3 stage.

## Links

- Programme ADRs: 0007 (host ownership), 0008 (scene vocabulary single source),
  0009 (AgentValidation placement), 0010 (legacy sample policy), 0011 (parity oracle method).
- Before/after baselines: `docs/reports/_baselines/2026-06-02-v3-before.md` /
  `2026-06-02-v3-after.md`.
- Migration guide: `docs/migration/v2-to-v3.md`.
