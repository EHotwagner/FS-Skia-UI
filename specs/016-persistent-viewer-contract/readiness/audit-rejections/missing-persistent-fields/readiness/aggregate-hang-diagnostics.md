# Aggregate Hang Diagnostics

Verdict: non-authoritative aggregate result.

Stage: `Verify` aggregate run, inside `Test`, last observed command:
`dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj -m:1`.

Elapsed duration: more than 6 minutes with no additional console output before
manual termination.

Focused rerun evidence:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` passed.
- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "persistent host wiring|generated task guidance"` passed.
- `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj --filter "bounded smoke command|deterministic scene evidence|default executable path"` passed.
- `./fake.sh build -t GeneratedGuidanceCheck` passed.
- `./fake.sh build -t GeneratedProductCheck` passed.
- `./fake.sh build -t EvidenceGraph` passed.

The aggregate `Verify` result is non-authoritative. Final readiness remains
blocked by the explicit `EvidenceAudit` synthetic/readiness diagnostics rather
than by this interrupted aggregate run.
