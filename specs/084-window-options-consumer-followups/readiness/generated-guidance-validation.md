# Generated guidance validation (084)

- **Authoritative command**: `./fake.sh build -t GeneratedGuidanceCheck`.
- **Artifact path**: `template/base/docs/product.md`, `template/base/README.md` (Verify-embeds-audit + `-t Test` guidance, FR-012/FR-013); `template/base/docs/scaffold-map.md` (FR-010/FR-011); `template/base/docs/evidence-formats.md` (FR-007).
- **Failure class**: guidance that omits the Verify→audit relationship, misnames the mid-implementation green-test path, or drifts the scaffold-map paths is a generated-guidance defect.
- **Next action**: `GeneratedGuidanceCheck` is green (Status: Ok); the docs name `-t Test` as the mid-implementation path and state that `Verify` embeds `EvidenceGraph`+`EvidenceAudit` and hard-blocks until every task is `[X]`.
