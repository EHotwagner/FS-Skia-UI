# Governance Risk Levels

## Small

Small changes are documentation-only readiness report refreshes with no build target, package output, generated product, template manifest, or public `.fsi` change. Required evidence is the affected report generator and focused Expecto tests.

## Medium

Medium changes affect generated guidance, template documentation, scanner behavior, or package-reference decision reports. Required evidence is focused Governance or Package tests, plus `GeneratedGuidanceCheck` and `TemplateDrift` when template guidance changes.

## Broad

Broad changes affect package contents, generated template contents, public contracts, build target behavior, or generated product runtime behavior. Required evidence includes the sequential FAKE order from the plan and current evidence map.

Broad validation is required only when package contents, generated template contents, public `.fsi`, build command behavior, or runtime surfaces change. This feature remains medium: it updates governance reports, scanners, docs, and package-reference decision evidence without runtime or public contract expansion.
