# validation.contract.yml — Intentional Tier 2 Diff (Feature 088)

`validation.contract.yml` is **regenerated from `Routing.fs`** (single source). The diff is
intentional and rationale-documented (distinct from the Tier 1/3 byte-identity posture).
`TargetMetadataDrift` PASSES against the regenerated file.

Added rules (24 inserted lines):
- `controls-docs` — `src/Controls/**/*.md` → `[ EvidenceGraph ]` (doc-only relaxation).
- `template-docs` — `template/**/*.md` → `[ EvidenceGraph ]` (doc-only relaxation).

Refined source rules (`controls-public-surface`, `generated-template`) match their heavy gates
only when the diff carries a non-doc path under the tree; the rendered `paths:` view is
unchanged, so the contract's path source-of-truth is stable while the matcher tightened.

The two new split sub-targets (`GeneratedProductStructure`, `GeneratedConsumerValidation`) are
additive `Targets` cases; they carry metadata rows but are not referenced by any routing rule.
