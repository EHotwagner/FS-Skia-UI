# Contract: Generated Product Validation

## Purpose

Define generated product validation obligations affected by the process
reliability follow-up.

## Required Inventory Coverage

Generated product reports must include:

- generated project file lists
- package references
- selected capability skills
- product source files
- product test files
- generated command logs
- public behavior markers claimed by guidance
- framework-source exclusion results

File-list-only evidence is not sufficient when readiness claims that generated
products exercise a public behavior.

## Profile-Specific Rules

### Ordinary Product Profiles

Ordinary generated products must reject:

- copied framework implementation projects
- framework sample/gallery source
- historical specs
- readiness evidence
- framework docs copied as product content
- stale package or capability references

### Sample-Pack Profile

The sample-pack profile may include intended generated sample content under
generated `samples/` paths. It must still reject framework implementation
projects, historical specs, readiness evidence, stale package references, and
sample content copied outside the allowed profile paths.

## Controls Boundary Carryover

Generated consumers must continue to treat Controls as the active path for
controls, charts, graph views, and DataGrid. Legacy Charts may appear only in
intentional migration or deletion guidance.

## Validation Commands

The following commands must produce or validate generated-product evidence:

- `TemplateCheck`
- `GeneratedProductCheck`
- `GeneratedGuidanceCheck`
- `TemplateDrift`
- generated product `Dev`, `Test`, and `Verify` where applicable

Failures must name the generated profile, artifact row, source/test marker,
package reference, file path, or stale guidance rule.
