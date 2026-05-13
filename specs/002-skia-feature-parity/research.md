# Research: Skia Feature Parity

## Decision: Pin the parity baseline to one upstream commit

**Decision**: Use `EHotwagner/SkiaViewer` commit `7aac43dd12903f93004d0c2bf7c6254318a366dc` as the parity baseline.

**Rationale**: A hard parity gate needs a stable target. A moving branch would make implementation and acceptance tests drift.

**Alternatives considered**:

- Review date only: rejected because the source could change while the feature is in progress.
- Latest release only: rejected because the requested repository state includes behavior beyond a package version label and the exact observed source is more reproducible.

## Decision: Split reusable capabilities into three packages

**Decision**: Keep `FS.Skia.UI` as the core viewer package and add `FS.Skia.UI.Charts` and `FS.Skia.UI.Layout` as independently referenceable packages.

**Rationale**: The clarified requirement demands independent consumer package boundaries. This also prevents applications that only need the viewer from carrying chart/layout concepts and lets public `.fsi` contracts stay focused.

**Alternatives considered**:

- Single all-in package: rejected because it weakens dependency boundaries and does not satisfy the clarified packaging requirement.
- Samples-only advanced capabilities: rejected because parity requires reusable consumer capabilities.

## Decision: Treat charts, DataGrid, layout, and graph as pure view-layer components

**Decision**: Component packages expose pure configuration/data records and builder functions that return core scene elements. Application state remains in the Elmish `Model`; model-to-view projection helpers prepare chart/layout props; `view` composes the returned elements.

**Rationale**: This mirrors MVVM's model-to-viewmodel projection without introducing a second state system. It keeps `update` pure and makes sorting, selection, scroll, zoom, hover, and filter state testable through Elmish messages.

**Alternatives considered**:

- Stateful chart widgets: rejected because hidden component state conflicts with Elmish/MVU boundaries and complicates semantic tests.
- Model-owned scene fragments: rejected because storing rendered scene output in the model blurs domain state and view projection.

## Decision: Implement chart and layout logic without new heavy runtime dependencies

**Decision**: Use local pure F# chart scaling, DataGrid virtualization math, stack/dock layout, DAG layering, and undirected force-layout algorithms unless a later benchmark proves a dependency is necessary.

**Rationale**: The constitution requires dependency minimization. The baseline scale targets are reachable with straightforward algorithms, and local pure algorithms are easier to validate through FSI and semantic tests.

**Alternatives considered**:

- Add a graph layout dependency: deferred. If planning or benchmarking proves the local approach cannot satisfy the 100-node/2-second target, a dependency proposal must document version pinning, maintenance owner, and compatibility impact.
- Port upstream source directly: rejected unless license attribution and source reuse are explicitly approved. The constitution treats upstream as behavioral/reference material.

## Decision: Expand the core scene DSL to cover baseline Skia features

**Decision**: Add typed scene, paint, shader, filter, path, clipping, text, picture, region, color-space, and transform declarations to the core package.

**Rationale**: Baseline parity requires broad expressiveness through immutable declarations that flow through `view`. A typed DSL gives `.fsi` contracts, FSI usage, and semantic tests a stable shape.

**Alternatives considered**:

- Expose raw canvas callbacks for missing features: rejected because public integration must remain Elmish-only and declarative.
- Expose loosely typed escape hatches: rejected for parity gate work because they make automated evidence and compatibility harder.

## Decision: Automated-first evidence is the completion standard

**Decision**: Require semantic tests, surface baselines, screenshot comparisons, package tests, sample smoke tests, and a generated parity evidence report. Manual visual review is allowed only for non-deterministic graphics differences.

**Rationale**: The spec has a hard parity gate. The evidence model must be repeatable enough to prevent subjective completion claims.

**Alternatives considered**:

- Manual review as primary evidence: rejected because it cannot reliably enforce a hard parity gate.
- Screenshot comparison for every visual feature: rejected because device/driver differences make some graphics effects non-deterministic.

## Decision: Store parity evidence as checked or generated readiness data

**Decision**: Produce `readiness/parity-evidence.json` from tests/scripts with one record per baseline capability, including status, evidence type, command, and notes.

**Rationale**: A machine-readable evidence report lets task generation and merge checks reason about completion and makes gaps visible.

**Alternatives considered**:

- Markdown-only parity matrix: useful for docs, but insufficient as a gate.
- Test names only: rejected because test names cannot capture adapted/excluded status or manual visual notes clearly.

## Decision: Data visualization scale follows the baseline

**Decision**: Chart checks cover empty, small, and 100,000-point datasets; DataGrid checks cover empty, small, and 10,000-row datasets.

**Rationale**: The clarified spec requires baseline scale. Planning around these concrete numbers prevents under-testing.

**Alternatives considered**:

- Moderate scale targets: rejected because they weaken the hard parity requirement.
- Sample-only data: rejected because it is not measurable enough.

## Decision: Diagnostics distinguish renderer, capability, frame, screenshot, and shutdown failures

**Decision**: Keep structured diagnostics as public data with stage, severity, message, cause, and optional capability metadata.

**Rationale**: Vulkan-only operation must fail clearly and recover safely. Diagnostics are required for unsupported environments and frame-level recovery.

**Alternatives considered**:

- Exception-only reporting: rejected because it is harder to route through Elmish update flow and smoke tests.
- String-only diagnostics: rejected because tests need structured stages and severity.

## Decision: Screenshot verification uses Skia-readable image files and tolerance metadata

**Decision**: Screenshot tests create PNG and JPEG files, load them back, verify dimensions/format, and compare deterministic scenes with documented tolerances.

**Rationale**: Screenshots are user-visible artifacts and must work from the public viewer handle/effect path.

**Alternatives considered**:

- File-exists-only verification: rejected because it misses invalid image output.
- Pixel-perfect for all scenes: rejected because JPEG and some GPU effects require tolerance.
