# Runtime-untouched proof (T021, SC-007 / FR-010 / FR-011)

Stage 0 is **record-and-oracle only**: it changes no runtime code and moves no library
code between packages.

## `src/**` is byte-unchanged
```
$ git diff --stat -- 'src/**'
            (empty)
$ git status --porcelain -- 'src/**' | grep '^??'
            (none)
```
The monolith (`src/Lib`), the eight split packages, the host (`src/SkiaViewer`), and
`src/SkiaViewer/SceneConversion.fs` are all byte-unchanged. The seeded one-package
violation (T018) edited `src/Scene/Scene.fsi` and was reverted with `git checkout --`, so
no runtime `.fsi` is permanently changed.

## No new dependency
```
$ git diff --stat -- Directory.Packages.props
            (empty)
```
No new `PackageVersion`. The `PerPackageSurfaceDiff` capability reuses the already-pinned
**DiffPlex** (a versionless `PackageReference` added only to the build-tooling project
`build/Governance/FS.Skia.UI.Build.fsproj`, which is **not** a runtime package) plus BCL
IO. `docs/reports/dependencies.md` / `DependencyReport` coverage is unchanged.

## No runtime governance-contract change (runtime-coupling finding)
The `PerPackageSurfaceDiff` target is **not** wired as a `Routing.fs` rule in this feature.
A rule would render the new gate into `validation.contract.yml`, whose known-gate allowlist
is validated by the runtime monolith's `src/Lib/AgentValidation.fs` (`knownGates`). Adding
the gate there would be a runtime change, so the rule (and the contract entry) is **deferred**
— keeping `src/**` byte-unchanged. The target remains additive and runnable directly;
`validation.contract.yml` is unchanged (`git diff --stat validation.contract.yml` is empty).

## Aggregate `PackageSurfaceCheck` unchanged (FR-011)
The existing aggregate `PackageSurfaceCheck`, its `readiness/surface-baselines/*.txt`
artifacts, the `package-surface` Routing rule, and `tests/Package.Tests/SurfaceAreaTests.fs`
are untouched. The new `PerPackageSurfaceDiff` capability is **strictly additive** — a
distinctly-named target over a separate artifact tree (`readiness/per-package-surface/`),
with its own `per-package-surface` Routing rule; it neither replaces nor weakens the
aggregate check.

## Change classification
- **Tier 1** for the governance/build surface only (one new curated governance `.fsi`, the
  `PerPackageSurfaceDiff` target, a `Routing.fs` rule, new baseline artifacts).
- **Tier 2-equivalent** for the runtime (no runtime `.fsi`, no package identity/version, no
  rendering behaviour change).
