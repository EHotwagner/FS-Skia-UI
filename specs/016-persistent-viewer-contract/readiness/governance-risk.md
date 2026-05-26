# Governance Risk Readiness

## T015 Risk Record

Risk level: broad Tier 1.

Focused validation required:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj`
- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj`
- `./fake.sh build -t GeneratedProductCheck`
- `./fake.sh build -t GeneratedGuidanceCheck`
- `./fake.sh build -t EvidenceGraph`
- `./fake.sh build -t EvidenceAudit`
- `./fake.sh build -t PackageSurfaceCheck`

Broad validation required before completion:

- `./fake.sh build -t Verify`

Non-authoritative aggregate-result handling:

- If a broad aggregate times out or fails for unrelated environment reasons, record the aggregate result as non-authoritative and include focused rerun evidence for each changed surface.
- Focused gate failures are authoritative for their surface and must be fixed or explicitly disclosed.

Changed surfaces:

- SkiaViewer public API and tests
- generated app template startup
- generated product validation
- generated guidance checks
- evidence graph/audit expectations
- docs and readiness artifacts
