# Generated-Validation Authority (T003)

- **Authoritative command**: `./fake.sh build -t GeneratedProductCheck` (+ `GeneratedGuidanceCheck`).
- **Artifact path**: `readiness/generated-file-lists/`, `readiness/generated-product-validation.md`,
  `readiness/generated-guidance.md`.
- **Failure class**: generated-consumer structural drift.
- **Next action**: the generated-product structural checks are relocated **behaviour-identically**
  (no `schema_version`/deprecation-window change — Stage 6.4, out of scope); the consumer package
  contract is unchanged. Re-run the gate; any diff is a relocation defect, fixed in the library.
Captured: 2026-06-01T14:44:47Z
