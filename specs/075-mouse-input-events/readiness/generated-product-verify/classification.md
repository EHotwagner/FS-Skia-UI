# GeneratedProductCheck — environment-failure classification (075)

`./fake.sh build -t GeneratedProductCheck` scaffolds a product into
`artifacts/generated-products/075-mouse-input-events/app-source/` and runs its
`Verify`. **Verdict: environment-failure, not a product defect.**

## Observed

- The generated product's **`Dev` completed** — `Product.dll` plus `Product.Tests`
  **build and test cleanly** against the local package pins (the log records
  "Dev completed for generated product" before the failing sub-step).
- The generated product's **evidence-graph sub-step aborts** with:
  > Cannot resolve the feature to validate: no `SPECKIT_FEATURE_DIR` override is
  > set and `…/075-mouse-input-events/app-source/.specify/feature.json` has no
  > usable "feature_directory" entry.

The generated scaffold ships an empty `.specify/feature.json`, so it cannot
self-resolve a feature in a headless run. Evidence: `app-source/verify.log`.

## Classification

This is a **pre-existing, sandbox-wide environmental condition**, identical to the
failure recorded on prior **merged** features (e.g.
`064-publish-nuget-distribution`, `065-typed-controls-front-door`) and documented
in project memory (`generated-product-check-env-failure`). This feature is an
additive pointer-coordination surface; the generated product does **not** consume
the new pointer front door, so the generated `Product` compiles and its tests pass
— only the generated evidence-graph step, which needs `SPECKIT_FEATURE_DIR` / a
populated generated `feature.json`, fails.

The authoritative merge gate remains **`EvidenceAudit verdict=PASS`** (recorded
clean for this feature: 0 blockers, 35 real tasks). FAKE-backed gates run
sequentially; this non-authoritative aggregate failure does not reflect a product
defect.
