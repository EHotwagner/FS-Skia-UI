# Generated-validation authority — feature 087 (placeholder)

- **Authoritative command**: `./fake.sh build -t GeneratedProductCheck` (Pinned
  package set) and `./fake.sh build -t TemplateCheck` (LocalPacked package set).
- **Artifact path**: `readiness/generated-product-check-green.txt`,
  `readiness/generated-product-defect-classification.txt`,
  `readiness/package-skew-clean.txt`, `readiness/package-skew-seeded.txt`.
- **Failure class**: per-step `ProductDefect` vs `Environment` (FR-002); each
  report names its `PackageSet` (FR-004). An `Environment` step is
  non-authoritative and never suppresses a `ProductDefect` in the same run.
- **Next action**: filled by T011/T015. A generated `Verify` step that cannot
  resolve a feature context is fixed by FR-001 (T009), not hand-classified away.
