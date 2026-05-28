# Governance Risk Levels

Evidence:

- `specs/025-upgrade-skia-speckit/plan.md`
- `specs/025-upgrade-skia-speckit/readiness/scope.md`
- `specs/025-upgrade-skia-speckit/readiness/logs/t037-verify.log`

| Risk level | Applies to this feature | Validation |
|------------|-------------------------|------------|
| Small | readiness-only wording and documentation clarifications | focused governance tests |
| Medium | generated template metadata, Spec Kit metadata, package guidance, compatibility inventory | `GeneratedGuidanceCheck`, `TemplateCheck`, `TemplateDrift`, governance tests |
| Broad | central SkiaSharp package version movement, template package metadata, package surface status, native/viewer validation | `PackageSurfaceCheck`, `PackLocal`, `SampleContractSmoke`, `Verify` |

This feature is Tier 1 because it changes governed dependencies and generated
governance/template metadata.

required evidence: version selection, dependency report, template alignment,
compatibility inventory, package surface baseline, sample smoke, Verify, and
EvidenceAudit.

broad validation: `./fake.sh build -t Verify` plus focused reruns for any
native-host transient failure.
