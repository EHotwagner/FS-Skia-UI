# Research: Package API Discovery And Name Safety

## Decision: Generate Source-Shaped API Reference From `.fsi`

Use curated public signature files as the source of truth for packaged API
reference. The generated reference must preserve F# authoring names, module
paths, union cases, record fields, constructor labels, function parameter
names, return shapes, and XML documentation summaries.

**Rationale**: The constitution already makes `.fsi` the public contract.
Compiled reflection names can differ from F# source spelling and do not give
package consumers the authoring shape agents need.

**Alternatives considered**: Assembly reflection was rejected because it is the
reported failure mode. Repository source inspection was rejected because clean
package consumers should not need source checkout. Handwritten-only reference
was rejected because it will drift from `.fsi` signatures.

## Decision: Package Or Package-Adjacent Reference Is Acceptable

The reference may be included inside each `.nupkg` or emitted beside package
artifacts by the packaging workflow, but `PackLocal`/package validation must
make the location explicit and stable for consumers.

**Rationale**: The requirement is consumer discoverability from installed
packages or packaged documentation, not external hosting. A stable generated
file path with package id/version evidence is enough for local generated
consumers and agents.

**Alternatives considered**: External docs hosting was rejected as out of
scope. Embedding XML documentation alone was rejected because XML docs do not
necessarily expose examples, collision guidance, or source-shaped F# samples.

## Decision: Collision Safety Uses An Inventory And Decision Record

Build an inventory of names shared by Scene, Controls, Layout, SkiaViewer, and
related packages, then record a decision for each collision-prone group:
contract-level qualification, explicit module/namespace guidance, sample
rewrite, or accepted non-issue.

**Rationale**: Some Controls types already use `[<RequireQualifiedAccess>]`;
other overlaps such as `Text`, `Image`, `Point`, `Rect`, `Size`, `Value`,
`Children`, event origins, and helper names may be better handled by explicit
qualification in examples. The decision record keeps compatibility and guidance
tradeoffs visible.

**Alternatives considered**: Blanket renaming was rejected as excessive. Adding
qualification attributes everywhere was rejected until compatibility impact is
known. Relying on namespace open order was rejected because it is unstable.

## Decision: Generated Guidance Must Lead With Discovery Sources

Generated product guidance must tell agents to consult the packaged
source-shaped API reference, compact API map, capability docs, and package
surface evidence before reflection or repository source reads.

**Rationale**: The failure mode is agent behavior during generated product
authoring. Guidance needs to change the first action, not only document a
fallback after an error.

**Alternatives considered**: Adding only repository docs was rejected because
package consumers may not have the repository. Adding only tests was rejected
because tests do not help an author choose correct names.

## Decision: Validate With A Clean Package Consumer

Validation must create or use a generated consumer that references local
packages from `~/.local/share/nuget-local/`, compiles Scene primitives,
`Paint` helpers, viewer/geometry records, and Controls-adjacent examples, and
does not copy repository `src/` files or use reflection as the authoring input.

**Rationale**: The consumer scenario is the only evidence that proves package
discoverability and name safety from the consumer side.

**Alternatives considered**: In-repo project references were rejected because
they bypass package contents. Unit-only tests were rejected because they do not
prove package consumption.

## Decision: Feedback Classification Is A Maintainer Contract

Readiness evidence must classify reflection and name-collision findings as
package documentation discoverability, public contract ergonomics, generated
template workflow, or consumer authoring guidance, with next action and
evidence path.

**Rationale**: The same symptom can require different owners. A classification
record prevents runtime issues, documentation gaps, and authoring mistakes from
being mixed into one backlog item.

**Alternatives considered**: Free-form notes were rejected because they are
hard to audit and slow to triage.
