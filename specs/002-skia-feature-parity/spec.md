# Feature Specification: Skia Feature Parity

**Feature Branch**: `002-skia-feature-parity`  
**Created**: 2026-05-12  
**Status**: Draft  
**Input**: User description: "improve the skia feature coverage to at least reach parity with https://github.com/EHotwagner/SkiaViewer"

## Source Baseline

Parity is measured against the publicly visible behavior of `EHotwagner/SkiaViewer` commit `7aac43dd12903f93004d0c2bf7c6254318a366dc`, reviewed on 2026-05-12. The baseline includes declarative scene construction, broad drawing and paint coverage, screenshots, input/lifecycle handling, charting, data grid rendering, layout containers, graph visualization, demo/sample coverage, performance evidence, thread-safe shutdown, and frame-level error recovery. This feature keeps this project's existing product constraints: Vulkan-only rendering, Elmish-only application flow, Windows and Linux desktop support, and a packable library with runnable samples.

## Clarifications

### Session 2026-05-12

- Q: Should completion require hard parity for every non-conflicting baseline capability, or allow documented gaps? → A: Hard parity gate: 100% of non-conflicting baseline capabilities must be supported or adapted before completion.
- Q: What evidence standard should prove parity for the hard gate? → A: Automated-first evidence: deterministic semantic, smoke, packaging, and screenshot checks are required; manual visual review is allowed only for non-deterministic graphics differences.
- Q: How should reusable parity capabilities be packaged for consumers? → A: Separate capability packages: core viewer, charts/data grid, and layout/graph are independently referenceable.
- Q: How should the parity baseline be fixed for planning and completion? → A: Pin exact revision: planning must record the exact baseline commit or release used for parity.
- Q: What data visualization scale should parity tests target? → A: Baseline scale: charts handle up to 100,000 data points and DataGrid handles 10,000 rows.

## Change Classification

**Tier**: Tier 1 (contracted change)  
**Public API Impact**: Expands the public scene, viewer, charting, layout, graph, diagnostics, and sample surfaces needed to reach feature parity through the Elmish-first viewer model.  
**Verification Approach**: Validate public contract coverage, semantic behavior, visual output, sample smoke runs, packaging, documentation, and parity evidence against the baseline capability checklist. Evidence is automated-first: deterministic semantic, smoke, packaging, and screenshot checks are required, while manual visual review is allowed only for non-deterministic graphics differences.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Render Baseline Drawing Coverage (Priority: P1)

An application developer can express and render the drawing, styling, effect, image, path, text, and transformation capabilities available in the baseline viewer without leaving the declarative Elmish model.

**Why this priority**: Feature parity starts with the core visual surface. Without comparable drawing coverage, higher-level charts, layouts, and examples cannot be ported faithfully.

**Independent Test**: Render a parity gallery containing representative geometry, text, image, path, shader, filter, blend, clipping, picture, region, color-space, and perspective examples and confirm each item appears as expected.

**Acceptance Scenarios**:

1. **Given** a parity gallery scene with primitive shapes, paths, text, and images, **When** the sample runs, **Then** each visual element renders in the expected position, order, and style.
2. **Given** a scene using gradients, noise, runtime effects, blend modes, color filters, mask filters, image filters, clipping, and path effects, **When** it is rendered, **Then** each effect is visibly applied without falling back to a different rendering path.
3. **Given** a scene using reusable pictures, region-based clipping, complex paths, point sets, vertex geometry, and 3D-style perspective transforms, **When** it is rendered, **Then** the output matches the declared composition within normal graphics tolerances.

---

### User Story 2 - Build Rich Data Visuals (Priority: P1)

An application developer can compose the same charting and tabular data visualization categories available in the baseline viewer as normal scene elements.

**Why this priority**: The baseline viewer presents charting and DataGrid support as first-class capabilities, so parity requires them to work as part of ordinary visual composition.

**Independent Test**: Render a data visualization gallery containing line, bar, pie or donut, scatter, area, histogram, candlestick, radar, and tabular views with realistic datasets and verify each category is readable and composable.

**Acceptance Scenarios**:

1. **Given** representative numeric, categorical, proportional, frequency, financial, and multi-axis datasets, **When** the gallery is rendered, **Then** every supported chart category displays readable axes, labels, legends where relevant, and correctly scaled data.
2. **Given** tabular data with text, numeric, and boolean values, **When** the grid is rendered and interacted with, **Then** headers, rows, sorting, and scrolling behave predictably.
3. **Given** charts and tabular views embedded inside a larger scene, **When** the scene is resized or updated, **Then** the data visuals stay within their allocated area and preserve readability.

---

### User Story 3 - Compose Layouts and Graphs (Priority: P2)

An application developer can arrange scene elements with reusable layout containers and visualize directed or undirected graphs with automatic layout, styling, validation, and composition.

**Why this priority**: Layout and graph support are major baseline differentiators and enable larger application screens without manual coordinate management.

**Independent Test**: Render nested stack and dock layouts containing ordinary elements, charts, grids, and graph views; verify layout results, graph readability, and validation behavior.

**Acceptance Scenarios**:

1. **Given** a nested layout containing multiple scene elements, **When** it is rendered at several window sizes, **Then** children are arranged without overlap and respond to available space changes.
2. **Given** a directed graph with valid acyclic relationships, **When** it is rendered, **Then** nodes and directional edges are laid out clearly without requiring manual positions.
3. **Given** an invalid graph declared as acyclic but containing a cycle, **When** it is rendered or validated, **Then** the developer receives a clear validation result instead of a misleading visualization.
4. **Given** an undirected weighted graph, **When** it is rendered, **Then** all connected and disconnected components remain visible and edge weights are visually distinguishable.

---

### User Story 4 - Operate Reliably Through Elmish Viewer Flow (Priority: P2)

An application developer can run rich visuals through the Elmish viewer with predictable input delivery, lifecycle handling, screenshots, diagnostics, and recovery from frame-level rendering failures.

**Why this priority**: Parity is not only visual coverage; the baseline includes operational hardening that consumers rely on when embedding the viewer in applications.

**Independent Test**: Run an interactive sample that handles input, updates state, captures screenshots, shuts down from different threads or lifecycle events, and continues after recoverable frame errors.

**Acceptance Scenarios**:

1. **Given** a running viewer, **When** keyboard, pointer, scroll, resize, close, and frame tick events occur, **Then** they are delivered to the Elmish update flow with enough information to update the model.
2. **Given** a screenshot request after at least one successful frame, **When** the request completes, **Then** a valid image file representing the current visible frame is available to the caller.
3. **Given** shutdown is requested from outside the rendering flow, **When** the request is processed, **Then** the viewer releases resources and exits within the documented timeout.
4. **Given** a recoverable frame-level rendering error occurs, **When** the next valid frame is available, **Then** the viewer reports the error and continues rendering instead of crashing the application.

---

### User Story 5 - Demonstrate and Prove Parity (Priority: P3)

An application developer evaluating the library can run documented samples and review evidence showing that this viewer reaches or exceeds the baseline viewer's feature coverage while honoring this project's constraints.

**Why this priority**: Parity claims need visible examples and repeatable evidence so consumers and maintainers can compare capabilities confidently.

**Independent Test**: From a clean checkout, run the documented parity samples, smoke tests, and parity checklist and confirm every baseline capability is either supported, intentionally adapted to the Elmish/Vulkan-only model, or documented as out of scope with rationale.

**Acceptance Scenarios**:

1. **Given** a clean checkout, **When** the developer follows the parity quickstart, **Then** they can run galleries for drawing coverage, effects, charts, layouts, graphs, data grid, screenshots, input, and a demo reel.
2. **Given** maintainers review parity status, **When** they inspect the evidence report, **Then** each baseline capability has a Supported, Adapted, IntentionallyExcluded, or NotYetSupported status and a link to an automated or manual verification path.
3. **Given** a baseline behavior conflicts with Vulkan-only or Elmish-only constraints, **When** parity is assessed, **Then** the behavior is represented by an equivalent supported workflow rather than reintroducing fallback rendering or a non-Elmish integration model.

### Edge Cases

- A baseline feature depends on a non-Vulkan fallback renderer; the project must provide equivalent supported behavior or document the behavior as intentionally excluded by product constraint.
- A runtime visual effect is unsupported or behaves differently on a particular Vulkan-capable device.
- Requested fonts, images, shaders, or data values are missing, invalid, too large, empty, or contain not-a-number/infinite values.
- Layouts are given zero, negative, very small, very large, or rapidly changing bounds.
- Graph inputs contain cycles, disconnected components, duplicate identifiers, missing endpoints, self-loops, or very dense edge sets.
- DataGrid inputs contain more rows than can be displayed at once, mixed value types, repeated sort requests, or column content wider than the available area.
- Screenshot capture is requested before the first successful frame, during shutdown, or when file output fails.
- Input events arrive while a frame is failing, while the viewer is resizing, or while shutdown is in progress.
- Samples are run in headless, remote, or driver-limited environments where the required rendering capability is unavailable.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The viewer MUST provide declarative scene elements for rectangles, ellipses, lines, text, images, paths, groups, points, vertices, arcs, reusable pictures, text runs, and composable nested scenes.
- **FR-002**: Scene elements MUST support fill, stroke, opacity, antialiasing, stroke cap, stroke join, stroke miter, blend mode, shader, color filter, mask filter, image filter, path effect, font, transform, and clipping options where meaningful for the element.
- **FR-003**: The viewer MUST support all standard compositing blend modes represented in the baseline viewer.
- **FR-004**: The viewer MUST support linear, radial, sweep, two-point conical, noise, solid color, image, composed, and runtime shader effects where the required rendering environment supports them.
- **FR-005**: The viewer MUST support blend-mode, matrix, composed, high-contrast, lighting, and luma color filters.
- **FR-006**: The viewer MUST support blur mask filters and the baseline image filter categories, including blur, drop shadow, dilate, erode, offset, color-filter wrapping, composition, merge, displacement map, and matrix convolution.
- **FR-007**: The viewer MUST support dash, corner, trim, one-dimensional, composed, and summed path effects.
- **FR-008**: The viewer MUST support rectangular, path-based, and region-based clipping with documented clipping operations and antialias behavior where applicable.
- **FR-009**: The viewer MUST support path commands, path fill types, boolean path operations, path measurement, segment extraction, path direction, and common path construction helpers.
- **FR-010**: The viewer MUST support font family, font style attributes, text measurement, positioned text runs, and a documented fallback when requested fonts are unavailable.
- **FR-011**: The viewer MUST support reusable drawing recordings, drawing playback, color-space selection or conversion behavior, and perspective-style transformations where supported by the rendering environment.
- **FR-012**: Developers MUST be able to express all parity drawing features through immutable, composable declarations that flow through the Elmish model.
- **FR-013**: Developers MUST be able to render line, bar, pie or donut, scatter, area, histogram, candlestick, and radar chart categories.
- **FR-014**: Chart elements MUST support automatic scaling, labels, legends where relevant, empty-data behavior, invalid-value handling, resizing, composition with other scene elements, and datasets up to 100,000 data points.
- **FR-015**: Developers MUST be able to render a tabular data grid with headers, text/numeric/boolean cells, vertical scrolling, fixed headers, sorting, width management, and datasets up to 10,000 rows.
- **FR-016**: Developers MUST be able to arrange visual elements with horizontal stacks, vertical stacks, and dock-style layouts, including nesting and resize response.
- **FR-017**: Developers MUST be able to render directed acyclic and undirected graphs with automatic layout, node and edge styling, labels, edge weights, disconnected components, and validation diagnostics.
- **FR-018**: Core viewer, charts/data grid, and layout/graph capabilities MUST be available as independently referenceable consumer packages.
- **FR-019**: The viewer MUST preserve Vulkan-only operation and MUST NOT introduce a fallback renderer to satisfy parity.
- **FR-020**: The viewer MUST preserve Elmish-only public integration and MUST NOT introduce a separate imperative rendering or stream-only integration model to satisfy parity.
- **FR-021**: The viewer MUST deliver keyboard, pointer, scroll, resize, close, lifecycle, frame tick, diagnostic, and recoverable frame-error events through the Elmish application flow.
- **FR-022**: Developers MUST be able to capture the current rendered frame as PNG and JPEG output, with clear success or failure results.
- **FR-023**: The viewer MUST fail startup clearly when required rendering capabilities are unavailable and report diagnostics that distinguish unsupported hardware, driver, surface, or effect capability issues where possible.
- **FR-024**: The viewer MUST recover from documented recoverable frame-level errors by reporting the error, preserving lifecycle safety, and rendering the next valid frame.
- **FR-025**: The viewer MUST support thread-safe lifecycle disposal and document the expected shutdown timeout.
- **FR-026**: The feature MUST provide runnable samples or galleries for drawing primitives, effects, shaders, screenshots, input handling, charts, data grid, layouts, graphs, performance-oriented scenes, and a combined demo reel.
- **FR-027**: The feature MUST provide a parity evidence report mapping each baseline capability to Supported, Adapted, IntentionallyExcluded, or NotYetSupported status; completion requires every non-conflicting baseline capability to be Supported or Adapted with automated-first evidence.
- **FR-028**: Public documentation MUST explain how baseline features map into this project's Vulkan-only and Elmish-only model.
- **FR-029**: The packages and samples MUST remain usable by consumers from a clean checkout without requiring private tooling or undocumented manual setup.

### Key Entities

- **Parity Baseline**: The externally referenced viewer capability set used for comparison, including feature categories, sample coverage, and operational behavior.
- **Capability Area**: A grouped set of related viewer behavior, such as drawing coverage, effects, charts, layout, graphs, data grid, screenshots, input, lifecycle, diagnostics, or samples.
- **Capability Package**: An independently referenceable consumer package for one capability boundary: core viewer, charts/data grid, or layout/graph.
- **Parity Evidence Item**: A record showing a baseline capability, this project's status for that capability, verification method, result, and any adaptation notes.
- **Scene Element**: A declarative visual item that can be composed into a frame.
- **Visual Style**: The paint, shader, filter, blend, stroke, font, clip, and transform data applied to one or more scene elements.
- **Data Visualization Element**: A chart or tabular element that renders structured data as part of a scene.
- **Layout Container**: A declarative arrangement element that allocates space for child scene elements.
- **Graph Visualization**: A rendered directed or undirected graph with nodes, edges, labels, styling, automatic layout, and validation results.
- **Viewer Diagnostic**: Structured information about startup, rendering capability, lifecycle, frame recovery, or unsupported scenarios.
- **Parity Sample**: A runnable example that demonstrates one or more capability areas and can be used as smoke-test evidence.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of baseline capability areas from the pinned baseline revision are represented in the parity evidence report with Supported, Adapted, IntentionallyExcluded, or NotYetSupported status.
- **SC-002**: 100% of baseline capabilities that do not conflict with Vulkan-only or Elmish-only constraints are marked Supported or Adapted with passing verification evidence.
- **SC-003**: The drawing parity gallery verifies at least 60 distinct visual capabilities across primitives, paint styling, shaders, filters, path operations, text, images, clipping, reusable recordings, regions, color handling, and transforms.
- **SC-004**: Automated checks cover all required chart categories with empty, small, and 100,000-point datasets, and cover the data grid category with empty, small, and 10,000-row datasets.
- **SC-005**: Layout samples demonstrate nested layouts with at least 10 child elements and resize to three different window sizes without overlap or clipped required content.
- **SC-006**: Graph samples render a 100-node directed acyclic graph within 2 seconds and a 50-node undirected weighted graph with all components visible.
- **SC-007**: Input-driven samples show visible response to keyboard or pointer interaction within 1 second in at least 95% of smoke-test runs on a supported workstation.
- **SC-008**: Screenshot verification produces valid PNG and JPEG files that match the current visible frame within normal graphics tolerances.
- **SC-009**: A recoverable frame-error scenario is reported and followed by a successful valid frame in 100% of recovery tests.
- **SC-010**: Unsupported rendering environments fail before presenting a partially functional viewer and report a clear diagnostic in 100% of startup tests.
- **SC-011**: A clean-checkout evaluator can run at least eight parity samples covering drawing, effects, charts, data grid, layout, graphs, screenshots, input, and demo reel behavior using documented commands.
- **SC-012**: Consumer-facing documentation includes a parity matrix and explains all adapted or excluded baseline behaviors in one discoverable location.
- **SC-013**: The simple viewer reaches first visible frame within 2 seconds in at least 95% of smoke-test runs on a supported Vulkan workstation.

## Assumptions

- The referenced baseline is pinned to public `EHotwagner/SkiaViewer` commit `7aac43dd12903f93004d0c2bf7c6254318a366dc`; future upstream changes are outside this feature unless explicitly added later.
- Parity means observable capability parity, not source-level compatibility or matching the baseline package/module structure.
- Baseline support for fallback rendering is intentionally not copied because this project remains Vulkan-only.
- Baseline stream-style integration is represented through equivalent Elmish workflows because this project remains Elmish-only.
- Windows and Linux desktop are the target environments for parity verification.
- Visual verification may combine automated semantic checks, screenshot comparisons, smoke tests, and documented manual inspection only where deterministic image comparison is impractical.
- Some advanced visual effects may depend on device or driver support; unsupported cases must be diagnosed clearly rather than silently ignored.
