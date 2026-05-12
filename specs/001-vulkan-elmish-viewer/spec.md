# Feature Specification: Vulkan Elmish Viewer

**Feature Branch**: `001-vulkan-elmish-viewer`  
**Created**: 2026-05-12  
**Status**: Draft  
**Input**: User description: "Create a SkiaSharp 4 preview viewer using Elmish with Vulkan-only rendering and no fallback renderer."

## Clarifications

### Session 2026-05-12

- Q: Which operating systems must the first Vulkan-only version support? → A: Windows and Linux desktop only
- Q: Should the first version expose only the Elmish model? → A: Elmish-only
- Q: What should the first version deliver? → A: Packable library plus runnable sample applications

## Change Classification

**Tier**: Tier 1 (contracted change)
**Public API Impact**: Adds a new packable library surface for an Elmish-first Vulkan viewer and sample applications.
**Verification Approach**: Validate `.fsi` public surface, semantic tests through the packed library or prelude, smoke-test sample applications on supported desktop environments, and fail-fast diagnostics for unsupported Vulkan environments.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Run Vulkan-Only Viewer (Priority: P1)

An application developer can start the viewer on a machine with Vulkan support and render a scene without choosing among multiple rendering backends.

**Why this priority**: This is the minimum useful product: the viewer must render successfully through the single supported backend.

**Independent Test**: Start the viewer with a simple scene on a Vulkan-capable system and confirm that a visible window renders frames without requiring backend selection or fallback configuration.

**Acceptance Scenarios**:

1. **Given** a Vulkan-capable system, **When** the developer starts the viewer with a valid scene, **Then** the viewer opens and displays the scene using the required renderer.
2. **Given** a running viewer, **When** scene content changes, **Then** the next visible frame reflects the updated content without restarting the viewer.
3. **Given** a developer reviews viewer configuration, **When** rendering options are inspected, **Then** there is no option to request or silently use a non-Vulkan fallback.

---

### User Story 2 - Fail Clearly Without Vulkan (Priority: P2)

An application developer receives an immediate, understandable failure when the viewer is started on a system that cannot provide Vulkan rendering.

**Why this priority**: A Vulkan-only product must make unsupported environments obvious so failures are diagnosable rather than silently degrading.

**Independent Test**: Start the viewer on a system or test environment where Vulkan initialization is unavailable and confirm that startup fails before rendering begins with a clear diagnostic.

**Acceptance Scenarios**:

1. **Given** a system without usable Vulkan support, **When** the developer starts the viewer, **Then** startup fails before showing a partially functional viewer.
2. **Given** Vulkan initialization fails, **When** the failure is reported, **Then** the message identifies Vulkan availability or initialization as the cause and does not mention fallback rendering.

---

### User Story 3 - Drive UI With Elmish Flow (Priority: P3)

An application developer can model viewer state, messages, updates, and subscriptions in an Elmish-style flow so interaction logic remains predictable as scenes change over time.

**Why this priority**: The requested update changes how applications integrate with the viewer, but it depends on the core viewer being able to start and render first.

**Independent Test**: Build a small interactive sample with model, message, update, and view functions; send input events; and confirm that state changes produce corresponding rendered scene changes.

**Acceptance Scenarios**:

1. **Given** an Elmish-style sample application, **When** the user presses a key or pointer input is received, **Then** the application updates its model and renders the corresponding scene state.
2. **Given** a subscription produces periodic messages, **When** the viewer is running, **Then** the rendered scene updates over time without direct mutable scene pushes from the sample application.

---

### User Story 4 - Provide Complete Elmish Viewer Examples (Priority: P4)

An application developer can learn the viewer through complete Elmish examples that cover declarative scenes, layout compositions, charts, screenshots, and input-driven visuals.

**Why this priority**: The first version should teach the supported programming model directly without requiring knowledge of any earlier viewer.

**Independent Test**: Run representative examples for basic scenes, layout, charting, input handling, and screenshot capture and confirm each scenario still completes with expected visual output.

**Acceptance Scenarios**:

1. **Given** a declarative scene with shapes, text, and effects, **When** it is rendered, **Then** the visible output matches the scene description within normal graphics tolerances.
2. **Given** chart and layout examples, **When** they are rendered, **Then** the viewer displays composed visual elements without requiring separate rendering paths.
3. **Given** a rendered frame, **When** screenshot capture is requested, **Then** the viewer writes an image representing the current visible frame.

### Edge Cases

- Vulkan is installed but initialization fails because the device, driver, surface, or required capability is unavailable.
- The viewer is started on a headless or remote environment without a compatible presentation surface.
- Scene updates arrive before the first frame is ready.
- Input events arrive while the viewer is shutting down.
- A frame-level rendering error occurs after the viewer has already displayed prior frames.
- Screenshot capture is requested before the first successful frame.
- Application code attempts to configure a non-Vulkan backend or alternate integration model.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The viewer MUST provide one supported rendering path for normal operation: Vulkan.
- **FR-002**: The viewer MUST NOT expose, select, or silently use a non-Vulkan fallback renderer.
- **FR-003**: The viewer MUST fail startup clearly when Vulkan rendering cannot be initialized.
- **FR-004**: Startup failure diagnostics MUST identify the unavailable rendering capability and include enough detail for a developer to distinguish unsupported hardware, driver, or surface issues.
- **FR-005**: Developers MUST be able to render declarative scenes containing geometry, text, images, effects, and composable visual elements.
- **FR-006**: Developers MUST be able to update rendered content while the viewer is running without recreating the window for each scene change.
- **FR-007**: Developers MUST be able to handle keyboard, pointer, resize, close, and lifecycle events through the viewer integration surface.
- **FR-008**: The viewer MUST expose only an Elmish-style application flow with model initialization, message dispatch, update handling, view generation, and subscriptions.
- **FR-009**: Elmish-style samples MUST demonstrate input-driven state changes and subscription-driven scene updates.
- **FR-010**: Core viewer capabilities MUST be available through the Elmish model where they are compatible with Vulkan-only rendering: scene composition, layout composition, chart rendering, screenshot capture, lifecycle disposal, and frame-level error isolation.
- **FR-011**: The viewer MUST reject attempts to configure a non-Vulkan backend or non-Elmish integration model with clear diagnostics.
- **FR-012**: The viewer MUST document supported environment requirements for Vulkan-only operation.
- **FR-013**: The first version MUST support Windows and Linux desktop environments only.
- **FR-014**: The package and examples MUST make clear that the graphics stack uses SkiaSharp 4 preview and that preview dependency behavior may change before stable release.
- **FR-015**: The viewer MUST include representative Elmish examples that can be used as smoke tests for simple scenes, interactive flow, charts, layout, and screenshot capture.
- **FR-016**: The first version MUST be delivered as a packable library with runnable sample applications.

### Key Entities

- **Viewer Configuration**: Startup options such as title, size, target frame behavior, clear color, and diagnostics settings; it excludes backend fallback selection.
- **Scene**: A declarative description of visual content to render, including primitives, text, images, effects, layout, charts, and composition.
- **Viewer Event**: Input and lifecycle information emitted by the running viewer, including keyboard, pointer, resize, close, and error events.
- **Elmish Program**: The developer-facing application structure made up of model, messages, update logic, view generation, and subscriptions.
- **Render Diagnostic**: Structured startup or runtime information that explains renderer availability, initialization failures, and frame-level errors.
- **Screenshot Request**: A request to capture the current rendered frame into an image output.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: On a Vulkan-capable developer workstation, a simple sample scene opens and displays its first frame within 2 seconds in at least 95% of smoke-test runs.
- **SC-002**: On an environment without usable Vulkan support, startup fails before rendering begins and reports a renderer-specific diagnostic in 100% of smoke-test runs.
- **SC-003**: No smoke-test run records use of a fallback renderer.
- **SC-004**: Interactive sample users can trigger a visible state change from keyboard or pointer input in under 1 second.
- **SC-005**: Subscription-driven sample updates visibly advance for at least 60 seconds without requiring manual scene pushes or window recreation.
- **SC-006**: Representative Elmish examples cover at least five viewer capabilities: simple scene rendering, input handling, state update, layout or chart composition, and screenshot capture.
- **SC-007**: Product documentation identifies the Vulkan-only and Elmish-only requirements in one discoverable location.
- **SC-008**: A consumer can package the library and run at least two sample applications from a clean checkout using documented commands.

## Assumptions

- Target users are application developers who want an Elmish-first F# viewer library.
- The update is allowed to make breaking changes where needed to remove fallback rendering and adopt the Elmish-style integration.
- Windows and Linux desktop environments with Vulkan support are in scope; macOS, mobile, browser, and headless production targets are out of scope for the first version.
- The current SkiaSharp 4 preview is acceptable even though preview dependencies may change before stable release.
- Capability scope means delivering the listed viewer behavior categories through the Elmish-first model.
- Declarative scene rendering, layout, charting, screenshots, reactive input, lifecycle handling, and frame-level error isolation are in scope only through the Elmish-first viewer model.
- The first version is a reusable library product with sample applications, not only a standalone viewer executable.
