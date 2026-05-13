# Merge Summary

## Verification Commands

- `dotnet build FS-Skia-UI.sln`
- `dotnet test FS-Skia-UI.sln`
- `dotnet fsi scripts/prelude.fsx`
- `dotnet fsi scripts/charts-prelude.fsx`
- `dotnet fsi scripts/layout-prelude.fsx`
- `dotnet fsi scripts/parity-evidence.fsx`
- `dotnet pack src/Lib/Lib.fsproj --output specs/002-skia-feature-parity/readiness/package/nuget`
- `dotnet pack src/Charts/Charts.fsproj --output specs/002-skia-feature-parity/readiness/package/nuget`
- `dotnet pack src/Layout/Layout.fsproj --output specs/002-skia-feature-parity/readiness/package/nuget`

## Evidence Paths

- Consolidated test log: `readiness/logs/t075-dotnet-test.txt`
- Surface baseline verification: `readiness/logs/t074-surface-baseline-tests.txt`
- Parity report: `readiness/parity-evidence.json`
- Package output: `readiness/package/nuget/`
- Sample contract logs: `readiness/smoke/t072-*-contract.txt`
- Vulkan smoke logs: `readiness/smoke/*-vulkan.txt`
- Screenshot evidence: `readiness/screenshots/`

## Synthetic Evidence

- T014: native Vulkan startup-stage failure fixtures are simulated to avoid mutating GPU/driver state.
- T077: Linux Vulkan smoke evidence is present; Windows Vulkan smoke evidence must be captured on a Windows workstation.

## Platform Caveats

The current workspace is Linux. Merge without caveat requires adding Windows smoke logs under `readiness/smoke/windows/` or accepting the T077 synthetic limitation.
