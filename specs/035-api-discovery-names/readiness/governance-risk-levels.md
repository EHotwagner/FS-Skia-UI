# Governance Risk Levels

Status: pass.

Risk classification:

- small: focused documentation wording or readiness-only evidence changes with
  no package, template, or public surface impact.
- medium: generated guidance, template docs, package-adjacent reference
  material, or focused package validation changes.
- broad: Tier 1 package governance changes that affect package-discovery
  workflows, generated product authoring, package surface checks, or clean
  package-consumer validation.

This feature is broad because it changes package-reference generation,
generated product guidance, package validation evidence, and user-reachable
consumer authoring samples.

Required evidence:

- `./fake.sh build -t Dev`
- `./fake.sh build -t PackLocal`
- `./fake.sh build -t PackageSurfaceCheck`
- `./fake.sh build -t FsiTranscripts`
- `./fake.sh build -t GeneratedGuidanceCheck`
- `./fake.sh build -t TemplateCheck`
- `./fake.sh build -t GeneratedProductCheck`
- `./fake.sh build -t EvidenceGraph`
- `./fake.sh build -t EvidenceAudit`

Broad validation:

Broad validation must run FAKE-backed commands sequentially because repository
`.fake` state is shared. The commands above were run in the documented order
through `GeneratedProductCheck`; `EvidenceAudit` is rerun after this readiness
contract fix.
