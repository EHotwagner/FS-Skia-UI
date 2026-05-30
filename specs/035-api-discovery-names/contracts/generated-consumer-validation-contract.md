# Contract: Generated Consumer Validation

## Purpose

Validation proves that a clean package consumer can author with FS.Skia.UI
packages using package-provided or package-adjacent reference material.

## Scenario Requirements

- Restore packages from the local feed produced by `PackLocal`.
- Avoid project references to repository implementation projects.
- Avoid copying repository `src/` files into the consumer.
- Avoid reflection as the source of authoring information.
- Compile Scene primitives, `Paint` helpers, geometry records, viewer host
  records, keyboard key cases, and Controls-adjacent code.
- Compile a mixed Scene/Controls sample that uses explicit qualification for
  collision-prone names.

## Required Evidence

- package feed path
- package ids and versions
- consumer project path
- source files under test
- commands run
- logs
- reflection/source-inspection scan result
- pass/fail verdict and diagnostics

## Acceptance

`GeneratedProductCheck` or a feature-specific package consumer validation step
writes `specs/035-api-discovery-names/readiness/generated-consumer-validation.md`
with a passing verdict and actionable diagnostics on failure.
