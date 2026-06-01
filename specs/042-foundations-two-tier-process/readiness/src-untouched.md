# SC-009 — runtime `src/**` untouched; no product surface or package change

    $ git diff --stat -- 'src/**'
      (empty — no runtime source edited)

    $ git diff -- Directory.Packages.props | grep -c "PackageVersion"
      0   (no new PackageVersion outside Directory.Packages.props — FR-012/FR-014)

- **FR-013**: no product public `.fsi`, surface baseline, or sample contract
  changes. The only `.fsi` files added are **build-tooling** (`Routing.fsi`,
  `ContractView.fsi`) under `build/Governance/`, plus the additive `Targets.Route`
  case in `Targets.fsi` — none are product surface. `PackageSurfaceCheck` /
  `FsiTranscripts` show no product baseline diff.
- **FR-014**: `FS.Skia.UI.Build` remains build-tooling only (`IsPackable=false`);
  no package ships into any generated product.
- The change is additive and reversible (FR-016): a new `Route` target + two new
  library module pairs + the generated/derived contract; removing them restores
  the prior "run the serialized order" default with no loss.

Authoritative command: `git diff --stat -- 'src/**'` (+ `PackageSurfaceCheck`).
Failure class: product/governance. Next action: none — runtime is untouched.
