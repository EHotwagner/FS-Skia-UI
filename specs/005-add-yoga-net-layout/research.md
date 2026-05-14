# Research: Yoga.Net Layout for UI Elements and Widgets

## Decision: Use Yoga.Net `3.2.3` as the pinned automatic-layout dependency

**Rationale**: NuGet lists `Yoga.Net` versions `3.2.1`, `3.2.2`, and `3.2.3`; the `3.2.3` nuspec targets `net8.0`, `net9.0`, and `net10.0`, has no package dependencies, uses MIT licensing, and points at `https://github.com/chenrensong/Yoga.Net`. The project README describes it as a C# port of Meta Yoga with Flexbox support, measure callbacks, caching, deterministic layout, and AOT/NativeAOT compatibility. Pinning the latest feed version satisfies the constitution requirement for explicit dependency versioning.

**Alternatives considered**: Use the older `Facebook.Yoga` package; rejected because it is an old prerelease package surface and does not align with the requested Yoga.Net dependency. Hand-roll flex layout; rejected because the feature explicitly requests Yoga.Net and a hand-rolled engine would increase correctness and maintenance risk.

## Decision: Keep Yoga.Net behind F# layout contracts in `src/Layout`

**Rationale**: Existing layout records, stack/dock helpers, graph layout, tests, and sample consumers live in `FS.Skia.UI.Layout`. Adding automatic layout there gives a single package-level layout story while allowing implementation details to remain omitted from `.fsi`.

**Alternatives considered**: Add a new project; rejected because it would split layout concepts and package baselines without a clear ownership benefit. Expose Yoga.Net directly; rejected because public API would inherit C# C-style node lifecycle and disposal concerns.

## Decision: Scope v1 to flex-style row, column, and wrap semantics

**Rationale**: The spec explicitly defers absolute and overlay positioning outside the automatic layout tree. Flex direction, wrapping, alignment, justification, padding, margin, gaps, min/max/fixed sizes, grow, and shrink cover the stated application and widget scenarios while keeping the public contract bounded.

**Alternatives considered**: Expose Yoga.Net CSS Grid in v1; rejected as outside the clarified scope and likely to expand tests and docs beyond the requested migration path.

## Decision: Model custom measurement as callback data on leaf layout nodes

**Rationale**: Text and custom-drawn content need preferred-size callbacks during layout. The public F# callback receives available logical size and per-axis measure modes, then returns a preferred size plus diagnostics. The Yoga.Net adapter maps this to Yoga measure callbacks internally.

**Alternatives considered**: Require all content to provide fixed sizes; rejected because it fails content-driven widgets and the explicit clarification. Let rendering measure text independently; rejected because it would duplicate layout decisions and break deterministic bounds.

## Decision: Return structured `LayoutResult` data with bounds and diagnostics

**Rationale**: Tests and applications need computed bounds without screenshot inspection. Recoverable layout failures must produce bounded fallback geometry and actionable diagnostics instead of runtime termination or silent clamping.

**Alternatives considered**: Throw exceptions for invalid layout input; rejected for recoverable cases because the spec requires safe renderable output. Return bounds only; rejected because diagnostics are required for invalid values, conflicts, unmeasurable content, and fallback behavior.

## Decision: Use logical coordinates as canonical layout values and snap only at render/hit-test boundaries

**Rationale**: Logical coordinates keep layout deterministic and independent of platform scale factor. A shared snapping function used by rendering and hit testing keeps visual bounds and interaction regions aligned.

**Alternatives considered**: Store snapped pixels in computed bounds; rejected because scale-factor changes would mutate canonical layout output and could accumulate rounding drift in nested trees.

## Decision: Track invalidation at node/subtree granularity

**Rationale**: Parent size, visibility, layout intent, child structure, and content measurement changes should invalidate only affected subtrees. The evaluator can re-read stable computed results for unaffected siblings and ancestors where constraints are unchanged, satisfying the stability and performance success criteria.

**Alternatives considered**: Recompute the whole tree for every change; rejected because the spec requires affected-subtree invalidation and a 200-node interactive resize goal. Cache only Yoga.Net internals; rejected because callers still need deterministic public invalidation semantics.

## Decision: Verify through public FSI, semantic tests, samples, baselines, and performance evidence

**Rationale**: The constitution requires public `.fsi` design before implementation, semantic tests against public APIs, test evidence, and surface-area baseline updates for Tier 1 changes. Layout behavior is deterministic and suitable for direct assertions on structured bounds and diagnostics.

**Alternatives considered**: Rely on screenshots; rejected because the spec requires structured computed bounds and screenshot-only evidence would miss invalidation and diagnostics.
