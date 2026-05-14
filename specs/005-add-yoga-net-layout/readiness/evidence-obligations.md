# Yoga.Net Layout Evidence Obligations

## Tier 1 Scope

This feature changes the public `FS.Skia.UI.Layout` contract and package dependency graph. Evidence must include public `.fsi` signatures, semantic tests through the package surface, surface-area baseline refresh, FSI transcript, sample smoke transcript, and final restore/build/test logs.

## Dependency Pinning

`src/Layout/Layout.fsproj` pins `Yoga.Net` to `3.2.3`. Yoga.Net types must remain implementation details and must not appear in public `.fsi` signatures.

## MVU Applicability

The automatic layout evaluator is pure library behavior. Stateful host/sample workflows for resize, widget updates, and content-measurement invalidation are represented by an explicit sample workflow contract and verified through transition/effect tests plus readiness transcript evidence.

## Unsupported V1 Scope

Automatic layout v1 supports row, column, wrap, flex sizing, custom measurement, hidden/collapsed visibility, render placement, hit testing, pixel snapping, diagnostics, and bounded fallback geometry. Absolute and overlay automatic-layout intent remain unsupported; manual scene composition, stack, dock, graph, absolute, and overlay usage remain available outside the automatic evaluator.
