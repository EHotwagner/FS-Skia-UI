# Runtime untouched + dependency invariants (SC-007 / FR-010 / FR-011 / FR-012)

- `git diff --stat -- src` → **empty** (no runtime source changed; SC-007).
- No `<PackageVersion>` added or changed outside `Directory.Packages.props` (FR-010/FR-012).
  `YamlDotNet 17.1.0` was already pinned there; the build front-end references it via the
  paket header `nuget YamlDotNet 17.1.0` (the FSX resolver, distinct from MSBuild CPM — same
  mechanism already used for `FSharp.Core`/`Fake.Core.Target`).
- No product `.fsi` changed; only new **build-tooling** `.fsi` were added under
  `build/Governance/` (FR-011). Surface baselines under `readiness/surface-baselines/` are
  untouched (`git diff --stat -- readiness/surface-baselines` empty), so PackageSurfaceCheck /
  FsiTranscripts cannot show a baseline diff (SC-006) — their inputs are provably unchanged.

Authoritative commands: `git diff --stat -- src`, `git diff -- '*.fsproj' Directory.Build.props | grep PackageVersion`.
Failure class: `governance / runtime-boundary`. Next action: revert any `src/**` leak.
