# Traceability Matrix

Date: 2026-05-16

| Requirement | Tests / validation | Implementation files | Readiness evidence |
|-------------|--------------------|----------------------|--------------------|
| FR-001, FR-003, FR-013 | `CapabilityCheck`, catalog tests | `template/capabilities.yml`, `build.fsx`, governance tests | `capability-catalog.md` |
| FR-002, FR-004, FR-005, FR-016 | Package tests, `DependencyReport`, `PackageSurfaceCheck` | `src/Scene`, `src/SkiaViewer`, `src/Elmish`, `src/KeyboardInput`, `src/Layout`, `src/Charts`, `src/Testing` | `dependency-report.md`, `package-surfaces/` |
| FR-006, FR-009 | Capability resolver and profile tests | `template/profiles/*.yml`, template generation logic | `generated-file-lists/`, `capability-selection.md` |
| FR-007, FR-008, FR-010, FR-014 | `GeneratedProductCheck`, generated product matrix tests | `template/base`, `template/fragments`, `build.fsx` | `generated-file-lists/`, `generated-product-verify/` |
| FR-011, FR-012 | `SkillCheck`, selected-skill copy tests | `src/*/skill/SKILL.md`, project skill, selected skill copy logic | `selected-skills.md` |
| FR-015, FR-017 | Generated product `Dev`, `Test`, `Verify` and command-contract tests | Generated product `build.fsx`, `fake.sh`, `fake.cmd`, Spec Kit assets | `generated-product-verify/` |
| Compatibility scope | Compatibility-impact check, evidence audit | `compatibility-impact.md`, `tasks.md` | `compatibility-impact.md`, `evidence-audit.md` |
