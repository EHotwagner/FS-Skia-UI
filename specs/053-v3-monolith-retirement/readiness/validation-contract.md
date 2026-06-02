# Validation contract (FR-007)

`validation.contract.yml` is regenerated from `build/Governance/Routing.fs` via
`./fake.sh build -t RefreshSurfaceBaselines`. This feature added `PerPackageSurfaceDiff` to
the `package-surface` rule's `required_gates` and `"PerPackageSurfaceDiff"` to the
`knownGates` allowlist (`build/Governance/AgentValidation.fs`).

- `package-surface` rule `required_gates`: `PackageSurfaceCheck`, `FsiTranscripts`,
  `PerPackageSurfaceDiff`.
- authoritative command: `./fake.sh build -t TargetMetadataDrift` → Ok (zero drift vs
  `Routing.fs`).
- `./fake.sh build -t Route` prints `PerPackageSurfaceDiff` on a `src/**/*.fsi` change.

failure class: ContractDrift. next action: regenerate from `Routing.fs`, never hand-edit.
