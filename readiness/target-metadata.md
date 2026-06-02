# Target metadata currency

`validation.contract.yml` is **generated** from the compiled `Routing.fs` single source of
truth (`build/Governance/Routing.fs`) via `./fake.sh build -t RefreshSurfaceBaselines`
(`ContractView.render Routing.rules Routing.dogfoodFeatureIds`). The
`build-target-contract` routing rule guards `build.fsx`, `scripts/build/**`, and
`validation.contract.yml`; a change to any of them escalates to `maintainer-verify`.

- **Authoritative command**: `./fake.sh build -t TargetMetadataDrift`
- **Verdict**: current — zero drift between `validation.contract.yml` and `Routing.fs`.
- **This change (feature 053)**: the `package-surface` routing rule gained
  `PerPackageSurfaceDiff` in `required_gates`, and `knownGates`
  (`build/Governance/AgentValidation.fs`) gained `"PerPackageSurfaceDiff"`. The contract was
  regenerated from `Routing.fs` so the rule, its rendering, and the allowlist entry land
  together; `TargetMetadataDrift` is green.
- **Failure class**: TargetMetadataDrift. **Next action**: if drift is reported, regenerate
  the contract from `Routing.fs` (`RefreshSurfaceBaselines`) — never hand-edit
  `validation.contract.yml`.
