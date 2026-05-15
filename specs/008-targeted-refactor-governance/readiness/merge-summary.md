# Merge Summary

## Scope

- Internal native startup ownership helpers were added without expanding the
  package-visible public surface.
- `VulkanHost.run` startup is staged with clearer failure propagation.
- Build, generated-guidance, and template-drift governance checks now use
  semantic section/path-class validation.
- Recoverable Yoga execution failure emits an observable fallback diagnostic
  through existing `LayoutDiagnostic` fields.
- Public record invariants are inventoried with deferred additive helper API
  recommendations.

## Verification

| Command | Result | Log |
|---------|--------|-----|
| `dotnet build FS-Skia-UI.sln -maxcpucount:1 --no-restore` | PASS | `readiness/logs/build.txt` |
| `dotnet test tests/Lib.Tests/Lib.Tests.fsproj --no-build` | PASS | `readiness/logs/lib-tests.txt` |
| `dotnet test tests/Layout.Tests/Layout.Tests.fsproj --no-build` | PASS | `readiness/logs/layout-tests.txt` |
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-restore` | PASS | `readiness/logs/governance-tests.txt` |
| `dotnet test tests/Package.Tests/Package.Tests.fsproj --no-build` | PASS | `readiness/logs/package-tests.txt` |
| `./fake.sh build -t PackageSurfaceCheck` | PASS | `readiness/logs/package-surface-check.txt` |
| `./fake.sh build -t GeneratedGuidanceCheck` | PASS | `readiness/generated-guidance.md` |
| `./fake.sh build -t TemplateDrift` | PASS | `readiness/logs/template-drift.txt` |
| `./fake.sh build -t Verify` | PASS | `readiness/logs/verify.txt` |
| `./fake.sh build -t Ci` | PASS | `readiness/logs/ci.txt` |
| `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/008-targeted-refactor-governance --graph-only` | PASS | `readiness/logs/evidence-graph.txt` |
| `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/008-targeted-refactor-governance` | PASS | `readiness/logs/evidence-audit.txt` |

## Public Surface Verdict

`src/Lib/Library.fsi` remains stable. Package surface checks confirm the new
`VulkanResources` and `VulkanStartup` helpers are not package-visible exports.

## Native Cleanup Verdict

Named startup stages and ownership rules are documented in
`native-startup-cleanup.md`. Synthetic failure injection is disclosed and paired
with real Vulkan startup and first-frame smoke evidence in `native-smoke.txt`.

## Governance Verdict

Generated guidance and template drift diagnostics now report missing sections,
prompt placement, path classes, alignment classes, active feature evidence, and
accepted deferral schema problems.

## Deferred Work

`API-REC-001` recommends future additive validation-first helper APIs for public
records that can currently be freely constructed into invalid states.
