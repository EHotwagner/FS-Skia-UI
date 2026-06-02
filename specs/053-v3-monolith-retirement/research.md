# Phase 0 Research — V3 Stage 5 Closeout

All Technical-Context unknowns are resolved from on-disk inspection at the
`053-v3-monolith-retirement` branch tip (2026-06-02). No NEEDS CLARIFICATION
remains.

## R1 — What still references `src/Lib` / the `FS.Skia.UI` monolith?

- **Decision:** The only `ProjectReference` build-consumer is
  `tests/Package.Tests/Package.Tests.fsproj:20` (conditional
  `..\..\src\Lib\Lib.fsproj`). Beyond that, the monolith survives only as
  **path-string references** in governance/test code and the pack flow.
- **Rationale:** Ground-truth grep (`*.fsproj`/`*.fs`/`*.sln`/`*.props`) across
  `src samples tests template build`. The Stage-2 lesson recurs: a deleted file is
  referenced by *path string*, invisible to a namespace/symbol grep. Enumerated
  call sites to clear:
  - `tests/Package.Tests/Tests.fs:69,88,153` — `packProjects`/`PackLocal` monolith asserts.
  - `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs:29` — packable enumeration row `("src/Lib/Lib.fsproj","FS.Skia.UI",["src/Lib/Library.fsi"])`.
  - `tests/Governance.Tests/DependencyGovernanceTests.fs:176,177` — `src/Lib/Lib.fsproj`, `../Lib/Lib.fsproj`.
  - `tests/Governance.Tests/RuntimeOrganizationTests.fs:41` — `src/Lib/Library.fs`.
  - `tests/Governance.Tests/PublicRecordInvariantTests.fs:9` — `src/Lib/Library.fsi`.
  - `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs:51` — `"src/Lib"`.
  - `tests/Governance.Tests/AgentValidationFrameworkTests.fs:491` — stale `Paths=["src/Lib/AgentValidation.fsi"]` rule input.
  - `tests/Governance.Tests/RoutingTests.fs:36,109,119` — `src/Lib/Foo.fsi` generic `.fsi`-change inputs.
  - `tests/Controls.Tests/DiagnosticsTests.fs:14,20,25,32` — `src/Lib/Lib.fsproj`/`src/Lib/KeyboardInput.fs` diagnostic-string examples.
  - `build/Governance/Front/Helpers.fs:36` — `packProjects` monolith entry.
  - `build/Governance/Routing.fs:214` — stale `knownGates` comment (FR-013).
  - `build/Governance/PerPackageSurface.fs:29` — stale monolith-exclusion comment.
  - `build/Governance/GeneratedProduct.fs:179,960,1875,1876` — forbidden-content patterns naming `src/Lib`/`..\Lib\Lib.fsproj`.
  - `FS-Skia-UI.sln:4` — `Lib` project entry.
- **Alternatives considered:** Symbol/namespace grep only — **rejected** (Stage-2
  showed it misses path strings; the `Dev` test failure surfaced only at build time).
  A repo-wide path grep is mandatory and becomes the no-consumer proof (SC-001).
- **Triage rule:** *enumerations* of the monolith → drop; *generic `.fsi`-example*
  inputs (RoutingTests `src/Lib/Foo.fsi`) → repoint to a live package path
  (`src/Scene/Foo.fsi`); *negative/illustrative diagnostic strings* (DiagnosticsTests,
  GeneratedProduct forbidden-content) → keep if they survive deletion, repoint if they
  must reference a still-living path.

## R2 — Is `src/Lib` safe to delete (residue)?

- **Decision:** Yes. `src/Lib` holds only `Library.fs` (142 LOC: `ParityStatus`/
  `EvidenceType`/`ParityEvidenceItem`/`ParityReport` + the `Parity` module),
  `Library.fsi` (61 LOC), and `InternalsVisibleTo.fs`. **No `VulkanStartup`/
  `VulkanResources`/`KeyboardInput`/`AgentValidation` residue remains** — those moved
  in Stages 1–2 and 052. `Lib.fsproj` still `ProjectReference`s `Scene` + `SkiaViewer`
  but nothing depends on `Lib`'s output except `Package.Tests`.
- **Rationale:** Direct read of `src/Lib/*`. The spec's hedged "any
  `VulkanStartup`/`VulkanResources` residue" resolves to **none present**; FR-003's
  deletion list is just `Library.fs(i)` + `InternalsVisibleTo.fs` + `Lib.fsproj`.
- **Alternatives considered:** Keeping `Lib` as an empty husk — **rejected** by the
  programme definition of done (the package must be deleted and unpublished).

## R3 — How is the `PerPackageSurfaceDiff` Route-gating wired (FR-007)?

- **Decision:** `Targets.PerPackageSurfaceDiff` **already exists** as a `Targets` DU
  case (`build/Governance/Targets.fs:12`, `.fsi:24`). FR-007 is satisfied by adding it
  to the **existing** `package-surface` routing rule's `RequiredGates`
  (`build/Governance/Routing.fs:201`, currently
  `[PackageSurfaceCheck; FsiTranscripts]`, matching `["src/**/*.fsi";
  "readiness/surface-baselines/**"]`), and adding `"PerPackageSurfaceDiff"` to the
  `knownGates` allowlist (`build/Governance/AgentValidation.fs`, the 16-entry list).
- **Rationale:** Stage 2 relocated `knownGates` into `FS.Skia.UI.Build`, so this is a
  **governance/build edit with no `src/**` runtime change** — the Stage-0 §0.3 deferral
  is now clean to pick up. `validation.contract.yml` regenerates from `Routing.fs`;
  `TargetMetadataDrift` enforces currency. The rule, its rendering, and the allowlist
  entry must land **together** or the contract currency check fails.
- **Alternatives considered:** A *separate* new rule for `PerPackageSurfaceDiff` —
  **rejected**; the existing `package-surface` rule already matches the same
  `src/**/*.fsi` trigger, so extending its `RequiredGates` is minimal and avoids a
  second overlapping rule. Adding a new `Targets` case — **unnecessary** (it exists).

## R4 — Where does the generated-project cleanliness gate live (FR-008)?

- **Decision:** Extend `GeneratedProductCheck` (`build/Governance/GeneratedProduct.fs`)
  rather than adding a standalone `GeneratedProjectCheck` target — the file already
  owns the forbidden-content posture (forbidden `content/specs/00N-` prefixes at
  ~lines 52–64; `src/Lib` forbidden-path patterns at 179/960/1875). The cleanliness
  assertion (no `samples/`, no framework docs set, no historical `specs/`, no framework
  README copy; references packages not copied framework projects) extends that existing
  validator.
- **Rationale:** Reuses the existing generated-product validation harness and keeps the
  gate name on the `knownGates` allowlist (`GeneratedProductCheck` already listed). A
  new target would need a new `Targets` case + `knownGates` entry + `validation.contract.yml`
  routing — more surface for the same assertion.
- **Alternatives considered:** A distinct `GeneratedProjectCheck` target (the spec's
  alternative phrasing) — viable but heavier; deferred unless the extension proves
  awkward. The spec explicitly permits either ("`GeneratedProjectCheck`, or an extension
  of `GeneratedProductCheck`").

## R5 — Do any CPM or template pins name the monolith (FR-005)?

- **Decision:** **No.** Root `Directory.Packages.props` names no `FS.Skia.UI` monolith
  pin (only externals + the build engine + split-package adopt-set). `template/base/
  Directory.Packages.props` pins `FS.Skia.UI.Build` + the nine split packages,
  conditioned by profile — **no monolith pin**. FR-005 is therefore a **verify-only**
  obligation (grep proof), not an edit.
- **Rationale:** Direct read of both files. The monolith was never a CPM/template pin;
  it was only ever in `packProjects`/`PackLocal` and the dependency docs.

## R6 — After-baseline structure (FR-010)

- **Decision:** Mirror `docs/reports/_baselines/2026-06-02-v3-before.md`: pin SHA,
  `src/Lib` LOC, leak/transitive-pull proof, duplicate-type inventory, consumer
  inventory — each with its reproduction command. The after-values: `src/Lib` LOC → 0
  (deleted), monolith transitive-pull → none, duplicate-type count → 0, package count
  = 9 split + build engine, per-package baselines present (9), generated-`app`
  cleanliness asserted (the new gate).
- **Rationale:** FR-010 requires a mirror of the Stage-0 before-baseline with
  reproduction commands; the before-file's section structure is the template.

## R7 — `ParityGallery` / Scene-only parity oracle residue (FR-012)

- **Decision:** `tests/Parity.Tests` is already Scene-only (references just
  `src/Scene/Scene.fsproj`; the old-vs-new bridge `Tests.fs` retired in 052). The
  scene-output oracle (`SceneOutput.fs`/`SceneOutputTests.fs`) is **preserved** per ADR
  0010. `samples/ParityGallery` is settled per ADR 0010 (kept on `Scene`+`SkiaViewer`,
  monolith-free — confirmed in 052). Governance scanning lists that name
  `tests/Parity.Tests` are cleaned where they assume the retired bridge.
- **Rationale:** 052 already did the heavy lifting; this stage records the decision in
  ADR 0012 and cleans any stale list entries. No oracle is removed.
