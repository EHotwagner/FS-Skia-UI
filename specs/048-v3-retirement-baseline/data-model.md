# Phase 1 Data Model — V3 Stage 0

Entities are descriptive artifacts plus the small value types of the new diff capability. No runtime
state and no Elmish workflow (the capability is a pure comparison with I/O at the edge — see
`research.md` D6).

## Entity: Baseline report

- **Storage**: `docs/reports/_baselines/2026-06-02-v3-before.md`
- **Fields / required sections**:
  - `pinSha` — the baseline SHA (`031e5607…` or the recorded branch-point SHA).
  - `monolithLoc` — `src/Lib` line counts **per file** (`Library.fs`, `KeyboardInput.fs`,
    `AgentValidation.fs`, `VulkanStartup.fs`, `VulkanResources.fs`, plus `.fsi`), each with the
    reproduction command.
  - `dependencyGraph` — the runtime package dependency graph (text/Mermaid) with its command.
  - `duplicateTypeInventory` — every type defined in **both** `Scene.fsi` and `Lib/Library.fsi`, with
    the count and the command that derives it.
  - `leakProof` — dependency dump showing `FS.Skia.UI.SkiaViewer → FS.Skia.UI` and a generated default
    `app` resolving the monolith, with the reproduction command (see contract `baseline-report.md`).
  - `consumerInventory` — complete monolith-consumer list (runtime package, all sample projects at the
    pin classified monolith-consumer vs split-package-only, test projects, governance front-end), with
    the regenerating grep/command (research.md D7).
- **Rule**: **every headline metric names the command that reproduces it** (SC-001).

## Entity: Parity oracle

- **Storage**: `tests/Parity.Tests/fixtures/v3-host-golden/`
  - `scene-output/<seed>.txt` — deterministic scene-output golden (authoritative).
  - `screenshots/<sample>.png` — reference frames (corroboration only).
  - `capture-environment.md` — OS, GPU/driver, .NET/toolchain versions, capture command, timestamp.
- **Seed scenes (fixed, closed set)**: one `scene-output/<seed>.txt` per seed, derived from the
  deterministic non-interactive galleries used for screenshots — `basic-viewer`, `effects-gallery`,
  `screenshot-gallery`. The set is closed and versioned with the fixtures; adding/removing a seed is a
  reviewed fixture change, not an implementation choice. (Capture-time check: each gallery must expose a
  deterministic, non-interactive `Scene` value; any interactive-only sample is excluded.)
- **Fields (per scene-output fixture)**: seed scene id (from the set above); the fixed, versioned
  textual encoding of the `Scene` value produced by the **current host**.
- **Rules**:
  - Scene-output is the **authoritative** parity signal; screenshots corroborate (headless flake).
  - A scene-output fixture MUST **re-derive byte-identically** from the current host (SC-003, 0-byte
    diff).
  - Capture environment MUST be recorded so a screenshot mismatch is attributable to environment.

## Entity: Per-package surface baseline

- **Storage**: `readiness/per-package-surface/<PackageId>.fsi.txt` (8 split packages).
- **Fields**: `packageId`; `normalizedSurface` — the normalized, deterministically-ordered full `.fsi`
  text of the package (Controls concatenates its multiple `.fsi` files in filename order).
- **Scope rule**: exactly the 8 public split packages — `Scene`, `SkiaViewer`, `Elmish`,
  `KeyboardInput`, `Layout`, `Controls`, `Controls.Elmish`, `Testing`. **Excludes** the monolith
  `FS.Skia.UI` and the build-tooling library `FS.Skia.UI.Build`.
- **Normalization** (research.md D2): strip `//` and `(* *)` comments, trim trailing whitespace,
  collapse blank-line runs, normalize newlines to `\n`, preserve written declaration order.

## Value types: the `PerPackageSurfaceDiff` capability (curated `.fsi`)

Pure data; lives in `build/Governance/PerPackageSurface.fsi`.

- `PackageId = string` — the package's `PackageId` (e.g. `FS.Skia.UI.Scene`).
- `Surface = { PackageId: PackageId; NormalizedText: string }` — a captured or current surface.
- `SurfaceLineChange = Added of string | Removed of string` — a single normalized-line delta.
- `PackageDrift = { PackageId: PackageId; Changes: SurfaceLineChange list }` — non-empty only when a
  package drifts.
- `DiffOutcome = { Drifted: PackageDrift list; CheckedPackages: PackageId list; MissingBaselines: PackageId list }`.

### Functions (pure core + edge interpreter)

- **Pure** `normalize : string -> string` — apply the normalization rules to raw `.fsi` text.
- **Pure** `diffPackage : (* baseline *) Surface -> (* current *) Surface -> PackageDrift option` —
  `None` when identical (zero drift); `Some` with the line changes otherwise. (DiffPlex-backed.)
- **Pure** `diff : Surface list -> Surface list -> DiffOutcome` — diff each current surface against
  its baseline; a current package with no baseline lands in `MissingBaselines` (fail loud, never
  silently pass — Principle VII).
- **Edge** `captureCurrent : PackageId list -> Surface list` — read each package's `.fsi` file(s)
  from disk and normalize (the only I/O on the current side).
- **Edge** `loadBaselines : path -> Surface list` — read `readiness/per-package-surface/*.fsi.txt`.
- **Edge** `runReport : path -> DiffOutcome -> unit` — write the per-package drift report and return a
  non-zero verdict when `Drifted` or `MissingBaselines` is non-empty.

### Invariants

- At the pin, `diff (loadBaselines …) (captureCurrent eightPackages)` ⇒ `Drifted = []` and
  `MissingBaselines = []` (SC-004).
- A single mutated public signature in one package ⇒ `Drifted` contains **exactly one**
  `PackageDrift` for that package, none other (SC-005).

## Entity: ADRs 0007–0011

- **Storage**: `docs/adr/000{7..11}-<slug>.md`.
- **Fields (each)**: Status, Date, Decision source, Context, Decision, Alternatives, Rationale,
  **Affected stages**; linked from the programme plan (FR-009/SC-006).
