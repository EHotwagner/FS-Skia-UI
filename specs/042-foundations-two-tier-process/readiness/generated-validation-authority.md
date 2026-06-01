# Generated Validation Authority — Feature 042

`validation.contract.yml` is now a **generated/derived view** of the compiled
single source of truth `build/Governance/Routing.fs` (FR-007), not a
hand-maintained second source.

- **Single source**: tiers, the framework-author/consumer-agent axis, the routing
  rules, and the dogfood feature ids live in `Routing.fs` as typed values.
- **Emitter**: `ContractView.render Routing.rules Routing.dogfoodFeatureIds`
  produces the canonical `validation.contract.yml` text deterministically.
- **Regeneration**: folded into `./fake.sh build -t RefreshSurfaceBaselines`.
- **Currency check**: folded into `./fake.sh build -t TargetMetadataDrift`, which
  calls the pure `ContractView.currencyDrift` over the on-disk file. A hand-edit
  is rejected with a "regenerate from Routing.fs" diagnostic; a freshly derived
  file passes. See `contract-currency.md`.

Authoritative command: `./fake.sh build -t TargetMetadataDrift` (currency) /
`./fake.sh build -t RefreshSurfaceBaselines` (regen). Failure class: governance.
Next action: regenerate the contract from `Routing.fs` — never hand-edit it.
