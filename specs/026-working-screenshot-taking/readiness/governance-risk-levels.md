# Governance Risk Levels

Status: recorded.

Default risk: medium.

Small validation applies only to docs-only or single-package changes with no
public contract, generated command, dependency, or audit impact.

Focused validation must include affected package tests, FSI transcripts,
package surface checks, generated product/guidance checks, and task graph
output.

Broad validation is triggered by any new native capture dependency, template
package pin change, aggregate target change, audit policy change, or other
cross-package behavior change. This implementation added a `SkiaSharp` package
reference to `FS.Skia.UI.Testing` using the repository's existing pinned package
version, so T040 must either run broad validation or record an explicit focused
validation rationale.

When broad verification stalls or times out, record the stage, elapsed duration,
last observed output, focused rerun command, and explicitly mark the aggregate
rerun as non-authoritative.

Required evidence includes screenshot capture evidence, screenshot artifact
inspection, failure diagnostics, generated guidance, package surface baseline,
task graph output, and final evidence audit output.
