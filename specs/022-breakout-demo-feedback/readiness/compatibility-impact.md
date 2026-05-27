# Compatibility Impact

status=ok

The feature adds public API surface without removing existing documented
members. Existing persistent viewer launch remains
`Viewer.runApp viewerOptions Product.Program.generatedHost`; bounded smoke and
deterministic evidence remain explicit diagnostic commands.

Known compatibility notes:

- Screenshot capture reports `status=unsupported` on hosts without capture
  support and exits successfully for that real unsupported-host fact.
- Deterministic fallback evidence is not relabeled as screenshot proof.
- Generated default app profiles do not gain an unselected Testing package
  dependency.

Validation:

- `./fake.sh build -t TemplateCheck`
- `./fake.sh build -t GeneratedGuidanceCheck`
- `./fake.sh build -t TemplateDrift`
- `./fake.sh build -t PackageSurfaceCheck`
