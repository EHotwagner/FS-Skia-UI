# Contract: Governance Validation

## Purpose

Generated products receive full product governance by default while avoiding
framework-source maintenance checks.

## Generated Product Governance Must Include

- product spec templates and constitution
- readiness workflow
- evidence graph validation
- evidence audit
- generated guidance checks scoped to product artifacts
- template drift checks scoped to generated product ownership
- selected local skills
- `Dev`, `Test`, and `Verify` command surface

## Generated Product Governance Must Exclude

- framework sample-gallery validation
- framework parity suite
- framework package-surface maintenance checks
- framework template packaging checks
- framework generated template source/package matrix unless the product opts
  into framework-source development

## Framework Validation Must Include

- capability catalog validation
- selected skill validation
- generated product content validation
- source and package template generation
- generated product `Dev`, `Test`, and `Verify` logs
- package surface baselines
- dependency report
- generated guidance report
- template drift report
- evidence graph and audit
- compatibility-impact readiness record stating V2 migration is out of scope

## Validation Contract

Framework `Verify` must fail when:

- generated products lack full product governance
- generated products run framework-source maintenance checks in consumer mode
- generated products miss evidence or drift command surfaces
- framework readiness evidence for this feature is missing
- compatibility impact is not recorded
- V2 migration implementation tasks appear in this feature scope
