# Implementation Notes

## Tier 1 Scope

This feature is a Tier 1 package-governance change. It may affect packaged
reference material, generated product guidance, package validation reports,
public `.fsi` signatures when collision safety requires it, and package surface
baselines.

Runtime rendering, screenshots, Vulkan, Skia raster output, generated game demo
work, external documentation hosting, and package publishing are deferred and
out of scope for this feature.

## Broad-Risk Validation

FAKE-backed commands share `.fake` state and must run sequentially. Broad
validation uses this order when the corresponding surfaces change:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t PackLocal`
3. `./fake.sh build -t PackageSurfaceCheck`
4. `./fake.sh build -t FsiTranscripts`
5. `./fake.sh build -t GeneratedGuidanceCheck`
6. `./fake.sh build -t TemplateCheck`
7. `./fake.sh build -t GeneratedProductCheck`
8. `./fake.sh build -t EvidenceGraph`
9. `./fake.sh build -t EvidenceAudit`

Aggregate FAKE results are non-authoritative until the focused readiness files
for API discovery, name-collision safety, generated consumer validation,
feedback classification, package reference material, package surface baseline,
evidence graph, and evidence audit are refreshed.

## Package And Public API Impact

Package contents may gain source-shaped API reference material or package-
adjacent reports generated from curated `.fsi` signatures. Public API contract
changes must start in `.fsi`, be covered by failing-first tests, be implemented
in matching `.fs` files only when required, and be reflected in surface
baselines.

## MVU Non-Applicability

Principle IV runtime MVU workflow rules are not applicable. The feature changes
package reference material, public signature discoverability, generated
guidance, validation reports, and governance evidence. File and package I/O
belongs at script, test, and FAKE target boundaries.

## Synthetic-Evidence Limits

Positive package-consumer discovery and Scene/Controls name-safety evidence
must be real. Synthetic evidence is limited to design-approved malformed
scanner or explicit error-path fixtures, currently T007 with the
`synthetic-error-handling-approved` label.

## Unsupported Runtime Scope

No runtime rendering behavior, layout engine behavior, input dispatch behavior,
window host behavior, or visual screenshot evidence is required or accepted as
the primary proof for this feature.

## Command Evidence Paths

| Command | Primary readiness path | Supporting artifact path | Failure classes | Next action |
|---------|------------------------|--------------------------|-----------------|-------------|
| `./fake.sh build -t Dev` | `readiness/generated-consumer-validation.md` | `readiness/logs/dev.txt` | compile, test, stale restore, unexpected runtime scope | Fix focused compile/test failure before broader reruns. |
| `./fake.sh build -t PackLocal` | `readiness/package-reference-material.md` | `readiness/logs/pack-local.txt` | package generation, missing reference material, stale version metadata | Rebuild package reference generation or package inclusion wiring. |
| `./fake.sh build -t PackageSurfaceCheck` | `readiness/package-surface-baseline.md` | `readiness/logs/package-surface-check.txt` | unrefreshed baseline, unintended `.fsi` contract drift, missing package entry | Reconcile intentional public changes, then refresh baselines. |
| `./fake.sh build -t FsiTranscripts` | `readiness/api-discovery.md` | `readiness/fsi/` | authoring sample compile, missing source-shaped spellings, package-shaped sample gap | Fix public sample/guidance or reference output. |
| `./fake.sh build -t GeneratedGuidanceCheck` | `readiness/generated-consumer-validation.md` | `readiness/logs/generated-guidance-check.txt` | reflection-first guidance, repository-source-copy guidance, open-order dependence | Update template/docs/capability guidance and rerun. |
| `./fake.sh build -t TemplateCheck` | `readiness/generated-consumer-validation.md` | `readiness/template/` | generated file drift, missing template content, stale capability metadata | Update template-owned files or expected generated output. |
| `./fake.sh build -t GeneratedProductCheck` | `readiness/generated-consumer-validation.md` | `readiness/generated-product-verify/` | restore, package reference, copied source, reflection authoring, compile | Fix package/guidance validation before final audit. |
| `./fake.sh build -t EvidenceGraph` | `readiness/evidence-graph.md` | `readiness/task-graph.md`, `readiness/task-graph.json`, `readiness/logs/evidence-graph.txt` | dangling task, cycle, skill metadata mismatch, invalid status | Fix tasks metadata before continuing implementation. |
| `./fake.sh build -t EvidenceAudit` | `readiness/evidence-audit.md` | `readiness/logs/evidence-audit.txt` | synthetic propagation, missing disclosures, diff-scan failure, stale graph | Resolve audit blockers or document approved synthetic error-path rows. |

All FAKE command logs are written under
`specs/035-api-discovery-names/readiness/` and must identify the target,
timestamp, command, result, and any follow-up classification.
