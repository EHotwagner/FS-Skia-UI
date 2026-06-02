# Contract — Baseline report & parity oracle

Defines the required shape of `docs/reports/_baselines/2026-06-02-v3-before.md` and the parity-oracle
fixtures, so reproduction is mechanical (SC-001/002/003).

## Baseline report — required sections (FR-001/002/003)

Each section MUST name the exact command that reproduces its headline number.

1. **Pin** — the baseline SHA (`031e56072779c736adf6dd8b0345e17b58a62e73`, or the recorded
   branch-point SHA if the branch advanced the pin).
2. **Monolith LOC (per file)** — `src/Lib/*.fs` and `*.fsi` line counts, each with its command
   (e.g. `wc -l src/Lib/Library.fs`). Expected magnitudes from the programme plan: `Library.fs ≈2,408`,
   `KeyboardInput.fs ≈1,398`, `AgentValidation.fs ≈835`, `VulkanStartup.fs ≈119`,
   `VulkanResources.fs ≈92` (captured values are the source of truth — re-measure at the pin).
3. **Runtime dependency graph** — the package reference graph (text or Mermaid) with its command.
4. **Duplicate-type inventory** — types defined in **both** `src/Scene/Scene.fsi` and
   `src/Lib/Library.fsi` (e.g. `VertexMode`, `Vertex`, `TextRun`, `FontSpec`, `PerspectiveTransform`,
   `Colors`/`Paint`/`Path`/`Scene`), with the count and the deriving command.
5. **Leak proof** (FR-002/SC-002) — a dependency dump showing
   `FS.Skia.UI.SkiaViewer → FS.Skia.UI` **and** a generated default `app` resolving the monolith.
   Grounded in `src/SkiaViewer/SkiaViewer.fsproj` (`ProjectReference ..\Lib\Lib.fsproj` +
   `SceneConversion.fs`). Reproduction command recorded (transitive/packed-graph dump on the
   `SkiaViewer` package and on a generated `app`).
6. **Consumer inventory** (FR-003) — complete monolith-consumer work-list: runtime `src/SkiaViewer`;
   6 samples (`BasicViewer`, `EffectsGallery`, `ParityGallery`, `ScreenshotGallery`,
   `InteractiveViewer`, `DemoReel`); test projects (`Lib.Tests`, `Smoke.Tests`, `Package.Tests`,
   `Parity.Tests`, `Governance.Tests` — verified at capture); governance front-end
   `build/Governance/Front/Support.fs`. With the regenerating grep/command.

## Parity oracle — fixtures (FR-004/005)

- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/<seed>.txt` — deterministic scene-output
  golden from the **current host**. **Authoritative.** MUST re-derive **byte-identically** (SC-003).
- `tests/Parity.Tests/fixtures/v3-host-golden/screenshots/<sample>.png` — reference frames from
  `ScreenshotGallery`/`EffectsGallery`/`BasicViewer`. **Corroboration only.**
- `tests/Parity.Tests/fixtures/v3-host-golden/capture-environment.md` — OS, GPU/driver, .NET/toolchain
  versions, capture command, timestamp. So a screenshot mismatch is attributable to environment, not
  regression (headless-flake mitigation).

### Scene-output determinism rule

The scene-output encoding is **fixed and versioned with the fixture**: stable node ordering,
canonical numeric formatting, no timestamps or environment-dependent fields. Re-running the capture
on the current host at the pin yields a 0-byte diff (SC-003). The encoder reads the current host's
`Scene` values and writes text — it adds **no runtime behaviour** (FR-010/SC-007).

## ADRs (FR-009/SC-006)

`docs/adr/0007-host-ownership.md` … `0011-parity-oracle-method.md`, each with Status, Date, Decision
source, Context, Decision, Alternatives, Rationale, **Affected stages**; each linked from
`docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md`.
