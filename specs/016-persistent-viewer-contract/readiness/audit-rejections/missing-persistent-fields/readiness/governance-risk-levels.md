# Governance Risk Levels

Selected risk level: broad.

Risk scale:

- small: isolated source or documentation changes with narrow validation
- medium: one package or generated-product surface with focused validation
- broad: cross-package, template, governance, documentation, or readiness-gate changes

Required evidence:

- SkiaViewer semantic tests and package surface checks
- generated product source, package, and template validation
- generated guidance checks
- dependency governance
- evidence graph and evidence audit
- supported-host persistent launch artifact

Broad validation is required before final readiness because the feature changes
package API, generated template behavior, generated product validation,
documentation, and evidence audit behavior.
