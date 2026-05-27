# Generated Validation Evidence

exact-package-match=true
generated-tests-exist=true
generated-tests-ran=true
authoritative=true
failure-class=none

## Status

T017 documented the independent US1 layout-readability validation path.

## Commands

- Generated product test: `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj --no-restore --logger "console;verbosity=minimal"`
- Generated product executable evidence:
  - `dotnet run --project template/base/src/Product/Product.fsproj --no-build -- --layout-evidence specs/020-asteroids-integration-feedback/readiness/generated-layout-1280x720.txt 1280 720`
  - `dotnet run --project template/base/src/Product/Product.fsproj --no-build -- --layout-evidence specs/020-asteroids-integration-feedback/readiness/generated-layout-640x480.txt 640 480`

## Evidence

US1 validation is independent of deterministic render hashes. The generated
product exposes `--layout-evidence`, writes a report for the requested output
size, and fails the command if generated layout validation rejects the report.

Evidence files:

- `readiness/generated-layout-1280x720.txt`
- `readiness/generated-layout-640x480.txt`

Public generated docs now describe the command in `docs/generated-apps.md`;
template product docs describe the reserved HUD region, gameplay region,
1280x720 default size, 640x480 constrained size, and evidence command in
`template/base/docs/product.md`.

Integration checks:

- `GeneratedProductCheck`: passed in 95 seconds after switching unattended
  persistent launch validation to the explicit `--launch-evidence` path.
- `GeneratedGuidanceCheck`: passed in 1 second.
- `TemplateCheck`: passed in 24 seconds.
- Combined measured duration: 120 seconds, under the 5 minute requirement.

Generated product contract evidence:

- `PackLocal`: completed before `GeneratedProductCheck`.
- `GeneratedProductCheck`: produced `readiness/generated-product-validation.md`.
- `exact-package-match`: `true`; requested and resolved package versions match for generated consumer validation.
- `generated-tests-ran`: `true`; generated product tests ran through the generated consumer `Verify` path.
- `authoritative`: `true`; this record uses the packed generated product validation path, not a bounded-only substitute.
- `failure-class`: `none`.
- `supported-host-persistent-launch.txt`: normalized from generated launch
  evidence so `EvidenceAudit` reports `0` persistent-launch blockers.
