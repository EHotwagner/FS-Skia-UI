# Governance Risk Levels — Feature 042 (two-tier development process)

This feature is **build-tooling only** (`build/Governance` + `build.fsx`); no
runtime `src/**` is touched. As a designated **dogfood** feature (FR-015) it runs
the full serialized FAKE order for its own validation even though the capability
it ships would route routine framework work light.

| Risk level | Scope | Authoritative validation |
|------------|-------|--------------------------|
| **small**  | routine framework-internal `src/**/*.fs`-style edits within this feature's own library work | focused `./fake.sh build -t Dev` + the `Governance.Tests` suite |
| **medium** | the new build-tooling `.fsi`/`.fs` (`Routing`, `ContractView`), the `Route` target case, the generated `validation.contract.yml` | focused `Dev` + targeted FAKE governance gates (`TargetMetadataDrift`) |
| **broad**  | required here because this is a **dogfood** feature | the full serialized FAKE gate order — see below |

## Required evidence and broad validation

The **required evidence** per risk level is named in the table above. **Broad
validation** (the full serialized FAKE order) is required here because this is a
dogfood feature.

The **broad** serialized order (required for this dogfood feature): `Dev` →
`GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
`EvidenceGraph` → `EvidenceAudit`. Aggregate FAKE results are recorded as
**non-authoritative**; any race-like or environment-flaky gate failure (the
documented 039 `FsiTranscripts`/`SkiaViewer.Tests` flakes) is rerun in focused
isolation, and the focused rerun is the authoritative result.

Authoritative command: `./fake.sh build -t Route` (selects the tier required for a
given change). Artifact path: `specs/042-foundations-two-tier-process/readiness/`.
Failure class: governance. Next action: run only the gates `Route` prints; for an
escalated change, run them sequentially in the deterministic order.
