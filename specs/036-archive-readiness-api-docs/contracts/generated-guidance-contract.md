# Contract: Generated Guidance

Generated and reviewer-facing guidance must tell maintainers and generated
product authors how to treat archived readiness and API references.

## Required Guidance

- Current feature readiness paths are authoritative for current gates.
- Historical feature readiness is audit context only unless a current evidence
  map explicitly marks it as supporting evidence.
- Archived material must not be cited as current package, template,
  generated-product, or audit pass/fail evidence.
- Source-shaped `.fsi` package API reference remains authoritative for agent
  authoring.
- FSharp.Formatting/fsdocs output, if produced, is secondary/hybrid unless the
  generator decision marks it authoritative.
- Package consumers must not use assembly reflection or repository source
  inspection as the authoring substitute.

## In-Scope Files

- `docs/generated-apps.md`
- `docs/template-profile.md`
- `template/base/README.md`
- `template/base/docs/product.md`
- generated guidance scanner fixtures or reports

## Required Evidence

Write guidance validation to:

`specs/036-archive-readiness-api-docs/readiness/generated-guidance-check.md`

## Failure Conditions

- Guidance presents archived readiness as current gate evidence.
- Guidance tells agents to use reflection or repository source inspection for
  package API authoring.
- Guidance promotes fsdocs as authoritative without a passing generator
  decision.
