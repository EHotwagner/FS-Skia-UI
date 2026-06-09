# TargetMetadata / TargetMetadataDrift — Feature 088

`./fake.sh build -t TargetMetadataDrift` → PASS (TargetMetadata + TargetMetadataDrift both
Success). The regenerated `validation.contract.yml` is current vs `Routing.fs`; the routable-
gate projection reproduces the prior `knownGates` / `ProductChecksRun` literals. The metadata
registry grew 40 → 42 rows (the two additive split sub-targets), an intentional Tier 2 change.
