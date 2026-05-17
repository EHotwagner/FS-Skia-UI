# Traceability Matrix

Status: setup mapping, to be filled with final evidence paths as tasks complete.

| Obligation | Primary tasks | Implementation files | Tests or commands | Readiness artifacts |
|------------|---------------|----------------------|-------------------|---------------------|
| FR-001, FR-002, FR-002a, FR-021, FR-022 | T008, T012, T014, T016, T023, T026, T028, T029, T032 | `src/Controls/*.fsi`, `src/Controls/*.fs`, planned `src/Controls.Elmish/*` | `PackageSurfaceCheck`, `FsiTranscripts`, Controls and adapter tests | `public-surface.md`, `elmish-adapter.md`, `rich-rendering.md` |
| FR-003, FR-024, FR-025, FR-026 | T011, T020, T024, T030, T035 | planned `src/Controls/ControlRuntime.*`, Controls diagnostics | Controls runtime tests, FSI transcripts | `control-runtime.md` |
| FR-016 through FR-020a | T010, T015, T025, T031, T033 | `src/KeyboardInput/KeyboardInput.*`, Controls integration | `KeyboardInputCheck`, KeyboardInput tests, FSI transcripts | `keyboardinput-package.md`, `keyboard-input-elmish.md` |
| FR-005, FR-006, FR-007, FR-011 | T009, T017, T037-T049 | `src/Controls/Charts.*`, planned `src/Controls/DataGrid.*`, `src/Controls/catalog.yml` | `ControlsCatalogCheck`, `ControlsRenderingCheck`, Package tests | `control-catalog.md`, `chart-datagrid-controls.md`, `compatibility-impact.md` |
| FR-008, FR-009, FR-014, FR-023 | T013, T050-T059 | `template/capabilities.yml`, `template/fragments/*`, generated guidance producers | `TemplateCheck`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateDrift` | `generated-product-usage.md`, `template-drift.md` |
| FR-010, FR-013, FR-015, FR-027 | T060-T072 | `docs/*`, `build.fsx`, project files, package baselines | `DependencyReport`, governance tests, package tests, docs checks | `package-boundary.md`, `dependency-report.md`, `compatibility-impact.md` |
| SC and audit obligations | T073-T085 | Build scripts and readiness outputs | `Dev`, `Verify`, `Ci`, `EvidenceGraph`, `EvidenceAudit` | `evidence-graph.md`, `evidence-audit.md`, final readiness summary |
