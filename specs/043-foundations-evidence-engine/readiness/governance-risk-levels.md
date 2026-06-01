# Governance Risk Levels — Feature 043 (evidence engine port, Stage 4)

This feature is **build-tooling only** (`build/Governance/Evidence/**` +
`build.fsx` + `template/base/**`); no runtime `src/**` is touched. As a
designated **dogfood** + consumer-contract feature (FR-015) it runs the full
serialized FAKE order for its own validation.

| Risk level | Scope | Authoritative validation |
|------------|-------|--------------------------|
| **small**  | routine framework-internal edits within this feature's own `build/Governance/Evidence/*.fs` library work | focused `./fake.sh build -t Dev` + the `Governance.Tests` suite |
| **medium** | the new build-tooling `.fsi`/`.fs` Evidence modules, the two rewired `build.fsx` gate arms, the published-package flip, the `template/base/**` change | focused `Dev` + the targeted FAKE governance gates the `Route` selector prints |
| **broad**  | required here because this is a **dogfood** + consumer-contract feature | the full serialized FAKE gate order — see below |

## Required evidence and broad validation

The **required evidence** per risk level is named in the table above. **Broad
validation** (the full serialized FAKE order) is required here because this is a
dogfood feature.

The **broad** serialized order (required for this dogfood feature): `Dev` →
`GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
`EvidenceGraph` → `EvidenceAudit`. Aggregate FAKE results are recorded as
**non-authoritative**; any race-like or environment-flaky gate failure (the
documented 039 `FsiTranscripts`/`SkiaViewer.Tests` libdecor-gtk flakes) is rerun
in focused isolation, and the focused rerun is the authoritative result.

Authoritative command: `./fake.sh build -t Route`. Artifact path:
`specs/043-foundations-evidence-engine/readiness/`. Failure class: governance.
Next action: run only the gates `Route` prints; for an escalated change, run them
sequentially in the deterministic order.
