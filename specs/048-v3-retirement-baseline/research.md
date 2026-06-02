# Phase 0 Research — V3 Stage 0 Baseline, Surface Baselines & Parity Oracle

All NEEDS CLARIFICATION resolved. Each decision records what was chosen, why, and the alternatives
weighed. Findings are grounded in the current tree at the pin SHA.

## D1 — New capability name & relationship to the existing `PackageSurfaceCheck`

- **Decision**: Add a **new, distinctly-named target `PerPackageSurfaceDiff`** over a new artifact
  tree `readiness/per-package-surface/`. Leave the existing `PackageSurfaceCheck` target, its
  `readiness/surface-baselines/*.txt` artifacts, the `package-surface` Routing rule, and
  `tests/Package.Tests/SurfaceAreaTests.fs` **untouched and green**.
- **Rationale**: The programme plan (§0.3) loosely says "add a `PackageSurfaceCheck` extension," but a
  target of that exact name **already exists** (`build/Governance/Targets.fs:11,107`; Routing rule
  `package-surface` at `Routing.fs` ~line 202). Spec **FR-011** forbids weakening or replacing the
  existing aggregate check in this feature. Reusing the name would force a behavioural change to a
  live gate. A separate name + separate artifact tree keeps the new capability strictly **additive**
  (FR-011, SC-008) and avoids colliding with the `package-surface` rule's artifact contract.
- **Alternatives considered**:
  - *Extend `PackageSurfaceCheck` in place* — rejected: violates FR-011 (changes a live gate's
    behaviour) and entangles Stage-0 with the existing monolith-inclusive baselines.
  - *Implement as a test only (no target)* — rejected: the spec's "Build-target impact" and §0.3 call
    for a build-target **capability** consuming the new baselines; a target is the contract surface
    Stage 5 later promotes to a hard gate.

## D2 — Surface representation: full normalized `.fsi` text, not type-name sets

- **Decision**: Each per-package baseline captures the **normalized full public-surface text of the
  package's `.fsi` file(s)** — comment/whitespace-normalized, deterministically ordered — diffed with
  the repository's existing **DiffPlex** facility. The `Controls` package concatenates its multiple
  `.fsi` files (`Accessibility`, `Attributes`, `Catalog`, `Charts`, `Collections`, `Control`,
  `ControlRuntime`, `CustomControl`, `DataGrid`, `Diagnostics`, `RichText`, `TextInput`, `Theme`,
  `Types`) in filename order.
- **Rationale**: **SC-005** requires detecting a single changed **signature** in exactly one package.
  The existing check stores **exported type-NAME sets** (reflection), which cannot see a changed
  function/member signature inside an existing type — it would miss the SC-005 case. The `.fsi` is
  already the sole authoritative surface declaration (Constitution Principle II), so normalized `.fsi`
  text is the faithful per-package contract and is signature-sensitive.
- **Alternatives considered**:
  - *Reflection over compiled assemblies* (as the existing check) — rejected: type-name granularity
    misses signature changes; also pulls the monolith in via assembly identity.
  - *XML-doc / metadata extraction* — rejected: heavier, and `.fsi` text is already canonical and
    diff-friendly with the existing DiffPlex golden tooling.
- **Normalization rules** (recorded so re-derivation is deterministic): strip `//` line comments and
  `(* *)` blocks, trim trailing whitespace, collapse blank-line runs, normalize newlines to `\n`,
  preserve declaration order as written in the `.fsi`. These rules live with the capability and are
  unit-tested.

## D3 — Baseline artifact location

- **Decision**: `readiness/per-package-surface/<PackageId>.fsi.txt` at the **repo root** (e.g.
  `readiness/per-package-surface/FS.Skia.UI.Scene.fsi.txt`).
- **Rationale**: These are **durable governance artifacts** consumed by a permanent target and later
  promoted to a hard gate (programme Stage 5) — they must persist at a stable path independent of the
  feature spec dir, mirroring the existing `readiness/surface-baselines/` convention. A distinct
  subdirectory keeps them from colliding with the aggregate check's files.
- **Alternatives considered**: *under `specs/048-.../`* — rejected: baselines outlive the feature and
  must be reachable by the standing target. *Reuse `readiness/surface-baselines/`* — rejected: that
  directory is owned by the aggregate check and is watched by the `package-surface` Routing rule
  (D1). *Note* the repo-root `readiness/` is tracked; gate runs may churn unrelated fixtures — scratch
  work stays in `/tmp` (carried hazard from prior features).

## D4 — Parity oracle method (scene-output authoritative, screenshots corroborate)

- **Decision**: Capture **deterministic scene-output golden fixtures** from the **current monolith
  host** for a **fixed, closed set of seed scenes** — `basic-viewer`, `effects-gallery`,
  `screenshot-gallery` (the deterministic non-interactive galleries also used for the screenshots) —
  committed as text under `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/<seed>.txt`.
  The set is versioned with the fixtures (data-model.md "Parity oracle"); adding/removing a seed is a
  reviewed fixture change, not an implementation choice. A test re-derives them and asserts
  **byte-identical** output (SC-003). Reference **screenshots** for the visual galleries
  (`ScreenshotGallery`, `EffectsGallery`, `BasicViewer`) are committed under
  `.../screenshots/` as **corroboration only**, with `capture-environment.md` recording OS, GPU/driver,
  toolchain, and command. Scene-output is the **authoritative** signal.
- **Rationale**: **FR-004/005** and the spec edge case: headless rendering is flaky (the known
  `SkiaViewer.Tests` libdecor-gtk crash). Deterministic scene-output is reproducible and
  environment-independent; screenshots are not. Recording the environment makes a screenshot mismatch
  attributable to environment rather than regression. These golden fixtures become the **Stage-1
  parity gate**.
- **Important finding**: the **existing `tests/Parity.Tests` is not** a scene-output golden harness —
  it validates a `parity-evidence.json` report against an upstream Skia commit SHA
  (`7aac43dd…`, the historical `002-skia-feature-parity` work) and references the monolith's
  `FS.Skia.UI.Parity` module. It is **left untouched** (it retires in programme Stage 4). The new
  golden fixtures are captured by a **new** deterministic serializer of `Scene` values produced by the
  current host, committed alongside, not by re-purposing the old report tests.
- **Scene-output serialization**: a deterministic, stable textual encoding of a `Scene` value (node
  order, primitives, paints, transforms, text runs) emitted for each seed scene. The encoding is
  fixed and versioned with the fixture so re-derivation is byte-stable. Exact encoder shape is a
  Phase-1 design detail (see `data-model.md`); it adds no runtime behaviour — it reads the current
  host's scene values and writes text.
- **Alternatives considered**: *screenshots as primary* — rejected (headless flake). *Re-use old
  `Parity.Tests`* — rejected (different purpose; it compares to upstream Skia, not host-vs-host).

## D5 — Leak proof reproduction command

- **Decision**: The baseline report's leak proof is a recorded, re-runnable dependency dump showing
  `FS.Skia.UI.SkiaViewer → FS.Skia.UI`. The concrete leak source is
  `src/SkiaViewer/SkiaViewer.fsproj` (`<ProjectReference Include="..\Lib\Lib.fsproj" />` plus
  `SceneConversion.fs`). Reproduction: a packed-graph / `dotnet list package --include-transitive`
  dump on the `SkiaViewer` package and on a generated default `app`, each command named beside its
  output in the report (SC-002).
- **Rationale**: FR-002/SC-002 require the leak be **provable** via a recorded command, not asserted.
  Grounding it in the actual `.fsproj` reference makes the proof concrete and stable at the pin.
- **Alternatives considered**: *prose assertion* — rejected (not reproducible). *Only the `.fsproj`
  grep* — kept as corroboration but the transitive/packed dump is the headline proof (it shows the
  default `app` profile pulling the monolith, which a project grep alone does not).

## D6 — Capability shape: pure diff + edge interpreter (no MVU)

- **Decision**: Model the capability as a **pure function** `diff : Baseline -> CurrentSurface ->
  PackageDrift list` plus a thin **edge interpreter** that reads `.fsi`/baseline files and writes the
  report. No Elmish `Model`/`Msg`/`Cmd`/subscription.
- **Rationale**: Constitution Principle IV requires MVU only for stateful/I/O **workflows**; this is a
  single pure comparison with file reads at the edge (spec State-workflow impact = None). Adding MVU
  ceremony would violate Principle III (idiomatic simplicity). Purity is unit-tested; the edge is
  exercised by a real-filesystem interpreter test (Principle IV's both-sides rule still honoured:
  pure-transition tests + real interpreter test).
- **Alternatives considered**: *local MVU algebra* — rejected as unjustified ceremony for a pure
  comparison.

## D7 — Consumer inventory (work-list for later stages)

- **Decision**: The baseline report enumerates every monolith consumer as the retirement work-list
  (FR-003). Grounded inventory at the pin:
  - **Runtime package**: `src/SkiaViewer` (`ProjectReference` to `Lib` + `SceneConversion.fs`).
  - **Samples**: the report classifies **all** sample projects present at the pin (12 at capture:
    `BasicViewer`, `ChartsGallery`, `ControlsGallery`, `DataGridGallery`, `DemoReel`, `EffectsGallery`,
    `InteractiveViewer`, `KeyboardInput`, `KeyboardInputGallery`, `LayoutGraphGallery`, `ParityGallery`,
    `ScreenshotGallery`) as monolith-consumer vs split-package-only, via the reproduction command — not
    a pre-narrowed subset. Known monolith consumers include `BasicViewer`, `EffectsGallery`,
    `ParityGallery`, `ScreenshotGallery`, `InteractiveViewer` (`PackageReference`/`ProjectReference
    FS.Skia.UI`), and `DemoReel` (references both); the remainder are confirmed split-package-only at
    capture time rather than assumed.
  - **Test projects**: `Lib.Tests`, `Smoke.Tests`, `Package.Tests`, `Parity.Tests`, `Governance.Tests`
    (the exact set is enumerated by the report's reproduction grep; `Controls.Tests`/`Elmish.Tests`
    hits are verified at capture time and classified runtime-vs-incidental).
  - **Governance build front-end**: `build/Governance/Front/Support.fs` (consumes
    `FS.Skia.UI.AgentValidation` from the monolith — relocates in Stage 2).
- **Rationale**: FR-003 requires a complete, reproducible inventory; the report names the grep/command
  that regenerates it so it stays honest at the pin.

## D8 — ADR scope (0007–0011)

- **Decision**: Five ADRs continuing the `000N` series (foundations ended at 0006), each with
  Status/Date/Decision-source, Context, Decision, Alternatives, Rationale, and **Affected stages**,
  and each **linked from the programme implementation plan**:
  - 0007 Host ownership → `SkiaViewer` owns the Vulkan/Skia host (shapes Stage 1).
  - 0008 Scene-vocabulary single source → `FS.Skia.UI.Scene` canonical; `Lib` duplicates deleted, no
    permanent `SceneConversion` shim (Stage 1).
  - 0009 `AgentValidation` placement → moves to `FS.Skia.UI.Build` governance library (Stage 2).
  - 0010 Legacy-sample policy → repoint to split packages or opt-in sample-pack; `ParityGallery`
    retires with the bridge (Stage 3).
  - 0011 Parity-oracle method → byte-identical scene-output (primary) + visual screenshots
    (corroboration) as the host-move merge gate (Stages 1, 4).
- **Rationale**: FR-009/SC-006 enumerate exactly these decisions; the existing ADR format
  (`docs/adr/0006-*`) is the template.

## D9 — Routing & target wiring

- **Decision**: Add `Target.PerPackageSurfaceDiff` to `Targets.fs(i)` (`allTargets`, `name`,
  `directPrerequisites = [ Build ]`, metadata), a `BuildEffect`/`StartTarget` arm in
  `Engine/Model.fs` + `Engine/Update.fs`, and a **new Routing rule** (e.g. `per-package-surface`) over
  `readiness/per-package-surface/**` + the new module path, tier `FocusedAuthority`, gates
  `[ PerPackageSurfaceDiff ]`, expected artifact `readiness/per-package-surface-expectations.md`.
  Regenerate `validation.contract.yml` from `Routing.fs` (`RefreshSurfaceBaselines`/`TargetMetadataDrift`
  currency) — never hand-edit.
- **Rationale**: Mirrors the existing `package-surface` rule and the documented target-registration
  pattern; keeps governance single-sourced. A mistyped gate is a compile error (the selector is
  compiled F#).
- **Alternatives considered**: *no Routing rule (run target manually)* — rejected: the capability must
  be `Route`-selectable so later stages and CI pick it up; the rule is how the artifact contract is
  enforced.
