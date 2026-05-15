# Traceability Matrix

| Requirement | Planned tests | Implementation files | Docs | Readiness artifacts |
|-------------|---------------|----------------------|------|---------------------|
| FR-001 template profile | `TemplateProfileTests` | `.template.config/template.json` | `docs/template-profile.md` | `template-source-inventory.md` |
| FR-002 source/package validation | `TemplateWorkflowTests` | `build.fsx`, `.template.package/FS.Skia.UI.Template.fsproj` | `docs/build.md`, `docs/testing.md` | `template/*.log`, `template/verdict.md` |
| FR-003 generated contents | `GeneratedProjectValidationTests` | `build.fsx`, template modifiers | `docs/template-profile.md` | placeholder and excluded-history scans |
| FR-004 placeholders | `GeneratedProjectValidationTests` | `build.fsx` | `docs/testing.md` | `template/*placeholder*` |
| FR-005 generated Dev | `TemplateWorkflowTests` | `build.fsx`, command wrappers | `docs/build.md` | generated `Dev` logs |
| FR-006 central versions | `DependencyGovernanceTests` | `Directory.Packages.props`, project files | `docs/dependencies.md` | `readiness/dependencies.md` |
| FR-007 dependency metadata | `DependencyGovernanceTests` | `scripts/dependency-report.fsx` | `docs/dependencies.md` | `readiness/dependencies.md` |
| FR-008 spec guidance | `GeneratedGuidanceTests` | `.specify/templates/spec-template.md` and preset override | `docs/speckit.md` | `generated-guidance.md` |
| FR-009 plan guidance | `GeneratedGuidanceTests` | `.specify/templates/plan-template.md` and preset override | `docs/speckit.md` | `generated-guidance.md` |
| FR-010 drift verification | `TemplateDriftTests` | `scripts/template-drift.fsx`, `build.fsx` | `docs/template-profile.md` | `template-drift.md` |
| FR-011 readiness outputs | `ArtifactPathTests` | `build.fsx` | `docs/evidence.md` | feature readiness tree |
| FR-012 V1 workflows preserved | `CommandContractTests` | `build.fsx`, wrappers | README, `docs/build.md` | `Dev`, `Verify`, and `Ci` logs |
| FR-013 deferred roadmap | `GeneratedGuidanceTests` | templates and docs | `docs/template-profile.md`, `docs/speckit.md` | final review |
| FR-014 minimal profile | `TemplateProfileTests` | `.template.config/template.json` | `docs/template-profile.md` | minimal profile review |
| FR-015 deferral fields | `TemplateDriftTests` | `readiness/template-deferrals.yml`, `scripts/template-drift.fsx` | `docs/template-profile.md` | `template-drift.md` |
