# Research: V3 Stage 3–4 Residual — Decouple Remaining Consumers from `src/Lib`

All NEEDS CLARIFICATION resolved. Findings are grounded in the on-disk state of the
repository at branch `052-v3-lib-decoupling` (pin after `388737e` / `62f3505`).

## R1 — Home for the rich keyboard-input runtime

**Decision:** A **new dedicated packable project `FS.Skia.UI.Input`** (`src/Input`)
referencing `FS.Skia.UI.Scene` + `FS.Skia.UI.SkiaViewer`. The module moves with only a
namespace rename (`FS.Skia.UI` → `FS.Skia.UI.Input`); the body is unchanged.

**Rationale:**
- The rich runtime's `.fsi` opens `FS.Skia.UI.Scene` and `FS.Skia.UI.SkiaViewer.Host`, so
  it depends on `SkiaViewer`. Observed reference edges: `SkiaViewer → KeyboardInput(lean) + Scene`;
  `Elmish → SkiaViewer + Scene`; `Scene → FSharp.Core only`.
- Therefore the rich runtime **cannot** live in the lean `FS.Skia.UI.KeyboardInput` package —
  that package is *upstream* of `SkiaViewer`, so adding a `SkiaViewer` dependency to it forms
  the cycle `KeyboardInput → SkiaViewer → KeyboardInput`.
- A dedicated package downstream of `SkiaViewer` keeps `SkiaViewer`'s public surface lean and
  makes the ~1,852-LOC runtime an **opt-in** capability — the V3 modularity ethos. (Maintainer
  decision, 2026-06-02.)

**Alternatives considered:**
- *Fold into `SkiaViewer`* — simplest (no new identity, no version/pin churn) but bloats
  `SkiaViewer`'s contents ~1,400 LOC and forces the rich input on every viewer consumer.
  Rejected for modularity.
- *Fold into `Elmish`* — acyclic (`Elmish → SkiaViewer + Scene`) but mixes a host-coupled input
  runtime into the MVU-glue package, muddying its purpose. Rejected.

**Acyclic proof of the chosen edge:** `Input → SkiaViewer → {Scene, KeyboardInput}`,
`Input → Scene`. No package depends on `Input` except samples/tests, so no back-edge. `Scene`
stays FSharp.Core-only. Invariant 4 preserved.

## R2 — Package naming & namespace collision

**Decision:** Package id and namespace `FS.Skia.UI.Input`; the public module stays
`KeyboardInput` (so consumers `open FS.Skia.UI.Input`). The lean package keeps
`FS.Skia.UI.KeyboardInput` (module `Keyboard`/`ViewerKeyboard`). The two are distinct package
ids and distinct namespaces, so a consumer can name exactly the one it depends on; no symbol
collision arises (the rich runtime references host/scene types by their `Scene`/`SkiaViewer.Host`
namespaces, not the lean package's).

**Rationale:** Mirrors the Stage-2 discipline (relocation = namespace rename only; zero surface
delta). "Input" vs "KeyboardInput" reflects scope: the lean package is an Elmish keyboard *model*;
the rich package is the full interactive *input runtime* (YAML bindings, modes, sequences,
command intents, diagnostics, bigram analysis, state-display projection).

**Alternative considered:** `FS.Skia.UI.SkiaViewer.Input` — implies it is part of the SkiaViewer
package rather than a peer downstream of it. Rejected as misleading.

## R3 — Parity sign-off mechanism (precondition for retiring `Parity.Tests`)

**Decision:** The deterministic, headless scene-output oracle established in Stage 0 and retained
in Stage 1 (`tests/Parity.Tests/fixtures/v3-host-golden/scene-output/<seed>.txt`, format
`scene-output/v1`) is the authoritative sign-off. Before removing `Parity.Tests`, confirm the
scene-output check is byte-identical to the Stage-0 golden, then migrate any still-valuable
assertions into `SkiaViewer.Tests`/`Scene.Tests`.

**Rationale:** Stage 1 established that the parity harness never used the Vulkan host — it
serializes deterministic `Scene` values — so parity is fully headless-verifiable and the
"old-vs-new" comparison `Parity.Tests` existed for is moot once `Lib`'s host is gone (it was gone
in Stage 1). Reference-screenshot re-capture stays headless-GPU-infeasible (disclosed
corroboration-only).

**Alternative considered:** Keep `Parity.Tests` indefinitely. Rejected — it is old-vs-new
scaffolding with no "old" left to compare; ADR 0011 scopes it to retire after sign-off.

## R4 — `Lib.Tests` / `Package.Tests` disposition

**Findings:** `tests/Lib.Tests` compiles `KeyboardInputTests.fs` + `Tests.fs` and references
`Scene` + `SkiaViewer` + `Lib`. `tests/Package.Tests` carries a *conditional* `Lib.fsproj`
reference.

**Decision:**
- `KeyboardInputTests.fs` → new `tests/Input.Tests` referencing `FS.Skia.UI.Input` (assertions
  preserved; exercise the API through the new package surface per Principle I).
- Triage `Tests.fs`: rich-runtime assertions travel to `Input.Tests`; any assertion against the
  dead `Parity` helper retires with it. If `Lib.Tests` is emptied, retire the project; otherwise
  repoint it off `Lib`.
- `Package.Tests`: drop the conditional `Lib.fsproj` reference.

**Rationale:** Assertions of value travel with their subject (no silent loss — spec edge case);
the rename keeps the test names/behaviour intact.

## R5 — `ParityGallery` policy (ADR 0010)

**Finding:** `samples/ParityGallery/ParityGallery.fsproj` **already** references `Scene` +
`SkiaViewer` only (repointed in Stage 1) — it does not reference the monolith.

**Decision:** Record the ADR-0010 keep-vs-retire decision explicitly. Recommended: **retire**
`ParityGallery` together with the `Parity.Tests` bridge — its purpose was to visualize the
old-vs-new parity report, which is moot once the bridge is gone. If it still demonstrates a
supported capability worth keeping, keep it on `Scene`+`SkiaViewer` and note why. Either way it
does not block `src/Lib` decoupling (it is already monolith-free).

## R6 — Packaging / version flow for the new package

**Decision:** Add `FS.Skia.UI.Input` to the solution and to `PackLocal`; on merge it follows the
standard two-commit version-bump + template-pin flow (per Stage 2's outcome). It introduces no
new external `PackageVersion`, so `Directory.Packages.props` is unchanged. The template does not
reference it, so no template-content change — only `dependencies.md`/`DependencyReport` gain the
new internal package row. `InteractiveViewer`'s `UsePackedPackage` path consumes it from the
local feed.

**Rationale:** Mirrors how the other split packages are packed/pinned; keeps CPM untouched.
