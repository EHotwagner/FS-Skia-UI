# Traceability Matrix

| Requirement | Tests | Implementation / Artifact | Verification |
|-------------|-------|---------------------------|--------------|
| FR-001 public surface stability | `tests/Package.Tests/SurfaceAreaTests.fs` | `src/Lib/Library.fsi`, `readiness/surface-baselines/*.txt` | `PackageSurfaceCheck` |
| FR-002 runtime responsibility separation | `tests/Governance.Tests/RuntimeOrganizationTests.fs` | `runtime-responsibility-map.md`, helper `.fsi` pairs | focused governance tests |
| FR-004..FR-006 native ownership/startup | `tests/Lib.Tests/NativeStartupCleanupTests.fs` | `src/Lib/VulkanResources.*`, `src/Lib/VulkanStartup.*`, `VulkanHost.run` | focused Lib tests, native smoke evidence |
| FR-007 build organization | `tests/Governance.Tests/CommandContractTests.fs` | `build.fsx`, `build-organization.md` | `BuildWorkflowCheck`, `Dev`, `Verify`, `Ci` |
| FR-008 generated guidance | `tests/Governance.Tests/GeneratedGuidanceTests.fs` | `build.fsx`, Spec Kit templates | `GeneratedGuidanceCheck` |
| FR-009 template drift | `tests/Governance.Tests/TemplateDriftTests.fs` | `scripts/template-drift.fsx` | `TemplateDrift` |
| FR-010..FR-012 Yoga fallback | `tests/Layout.Tests/YogaFallbackDiagnosticsTests.fs` | `src/Layout/Layout.fs`, `follow-ups.md` | focused Layout tests |
| FR-013..FR-014 public record invariants | `tests/Governance.Tests/PublicRecordInvariantTests.fs` | `record-invariants.md`, `follow-ups.md` | focused Governance tests |
| FR-015 evidence completeness | evidence audit | readiness directory | `EvidenceGraph`, `EvidenceAudit` |
