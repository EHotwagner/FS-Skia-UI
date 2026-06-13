# Feature Specification: OpenGL Present Backend (Direct GPU Rendering)

**Feature Branch**: `119-opengl-present-backend`  
**Created**: 2026-06-13  
**Status**: Draft  
**Input**: User description: "change renderer to gl to enable direct gpu rendering per information gathered last feature."

## Context

Feature 118 (backend/host-mode review) established — and reproduced live on real hardware —
that a **readback-free direct present is infeasible on the Vulkan backend** because
SkiaSharp's managed binding cannot wrap a Vulkan swapchain image as an `SKSurface`
([mono/SkiaSharp #1502](https://github.com/mono/SkiaSharp/issues/1502)) nor hand off the
image layout it needs to present ([#2191](https://github.com/mono/SkiaSharp/issues/2191)).
Feature 118 therefore shipped the public `ViewerPresentMode` seam (`OffscreenReadback` |
`DirectToSwapchain`), made `DirectToSwapchain` degrade safely back to readback, and recorded
FR-002/SC-002 (the readback-free goal) as **blocked-by-dependency**. Its deliverable
`opengl-backend-resolution.md` named the fix: **host on OpenGL instead of Vulkan**, where
SkiaSharp's GL interop is complete (`GRGlFramebufferInfo` + `SKSurface.Create` over the
window's default framebuffer succeed), giving genuine zero-readback direct present.

This feature implements that resolution: replace the Vulkan host backend with an OpenGL host
backend so that live presentation draws the Skia scene straight onto the window framebuffer
and the windowing toolkit's buffer swap presents it — no GPU→CPU readback, no staging buffer,
no per-frame command pool, no full-queue stall. It unblocks FR-002/SC-002 from feature 118.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Direct, readback-free live presentation (Priority: P1)

A developer runs an interactive FS.Skia.UI application on a supported desktop (Windows or
Linux with GPU passthrough). The live window renders the Skia scene each frame and presents it
directly via the GPU with no per-frame GPU→CPU→GPU round-trip, so the steady-state present path
spends no time on readback.

**Why this priority**: This is the entire purpose of the feature and the deliverable that
feature 118 deferred. Without it, the `DirectToSwapchain` mode remains a stub that always
degrades to readback.

**Independent Test**: Launch the persistent interactive host on a GPU-passthrough machine,
confirm the window renders the expected scene continuously and pointer/keyboard interaction
still works, and confirm via diagnostics/instrumentation that the present path performs no
per-frame readback in the direct-present mode. Visual output is unchanged from the prior backend
(same scene renderer).

### User Story 2 - Unchanged visual output and interaction (Priority: P1)

A developer who already uses the interactive host, on-demand screenshots/evidence capture, and
pointer/keyboard input observes that all of these continue to behave identically after the
backend swap. The scene renderer is canvas-based and backend-agnostic, so the rendered pixels,
the focus/pointer routing, the animation clock, and the screenshot/evidence routine produce the
same results as before.

**Why this priority**: A backend swap that silently changes visual output or breaks input would
be a regression masquerading as an improvement; behavioral parity is a hard requirement, not a
nice-to-have.

**Independent Test**: Run the existing controls/charts/datagrid sample-smoke and evidence
captures under the new backend and confirm the rendered output and interaction match the
established baselines (allowing for documented, intentional differences only).

### User Story 3 - Safe behavior where GPU/GL is unavailable (Priority: P2)

A developer (or CI agent) on a machine without GL/GPU passthrough, or with a broken GL driver,
gets a clear, honest diagnostic that distinguishes a missing/unavailable presentation
environment from an actual implementation defect, consistent with the project's existing
unsupported-environment handling — rather than a confusing crash or a silent black window.

**Why this priority**: The constitution requires that presentation failures distinguish
environment limitations from implementation defects; CI and headless contexts must remain
honest. It is P2 because it governs the failure path, not the primary success path.

**Independent Test**: Run the host in an environment without working GL and confirm it emits the
appropriate classified diagnostic (benign environment-limitation vs. blocking defect) and does
not falsely report success.

### User Story 4 - Consumers migrate off the removed Vulkan host surface (Priority: P2)

A consumer who referenced the public Vulkan host contract (`VulkanResources`, `VulkanStartup`,
`VulkanHost`) is given clear migration guidance for the replacement GL host surface, since this
is a breaking public-surface change shipped in a new package version.

**Why this priority**: The change is breaking by necessity; consumers need a documented path
forward. P2 because the primary consumer entry points (`runInteractiveApp` /
`runInteractiveViewer` / `ViewerOptions`) are intended to remain source-stable.

**Independent Test**: Confirm the changelog/migration notes name every removed/renamed public
member and its replacement, and that the high-level consumer entry points compile unchanged
against the new package.

### Edge Cases

- **Window resize**: the framebuffer-backed surface must re-wrap/recreate correctly on resize so
  presentation tracks the new window size without leaking GPU resources.
- **Context loss / driver reset**: define expected behavior (recover or fail honestly) so a lost
  GL context does not wedge the host loop.
- **Headless / CI**: the offscreen evidence-capture path must still function (or degrade with an
  honest diagnostic) when there is no on-screen window.
- **`ViewerPresentMode` interaction**: with the GL backend, define how `OffscreenReadback` and
  `DirectToSwapchain` map onto GL — whether direct present becomes the default, and what the
  readback mode means when the native present is already direct.
- **High-DPI / fractional scaling** (notably Wayland): presented pixels must match the scene's
  intended size.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The interactive host MUST present each live frame by drawing the Skia scene onto a
  GPU-backed surface bound to the window's framebuffer and presenting it via the windowing
  toolkit's buffer swap, with **no per-frame GPU→CPU readback** in the direct-present path.
  *(Unblocks feature 118 FR-002/SC-002.)*
- **FR-002**: Visual output MUST match the prior backend for the same scene, since the scene
  renderer is canvas-based and backend-agnostic. Any intentional deviation MUST be documented.
- **FR-003**: Pointer and keyboard input routing, focus handling, the animation clock, and the
  retained render path MUST continue to behave identically across the backend swap.
- **FR-004**: The on-demand screenshot / evidence-capture routine MUST continue to produce the
  same results and remain available independent of the live present path (preserving feature
  118's capture/present decoupling).
- **FR-005**: When a working GL context cannot be created (no GPU passthrough, missing/broken
  driver, headless), the host MUST emit an honest classified diagnostic that distinguishes an
  environment limitation from an implementation defect, consistent with the existing
  unsupported-environment handling — never a false success and never an unclassified crash.
- **FR-006**: The host MUST handle window resize by correctly re-binding/recreating the
  framebuffer-backed surface to the new size without leaking GPU resources.
- **FR-007**: The `ViewerPresentMode` seam MUST be reconciled with the GL backend: define and
  document how `DirectToSwapchain` (now genuinely readback-free) and `OffscreenReadback` map to
  GL, including the default mode, so the public contract stays coherent rather than carrying a
  Vulkan-only meaning.
- **FR-008**: The Vulkan host backend dependency MUST be replaced by an OpenGL host backend in the
  dependency manifest, dependency report, and dependency docs; the removed Vulkan packages MUST no
  longer be referenced by the host.
- **FR-009**: The breaking public-surface change MUST be accompanied by migration guidance that
  names every removed/renamed public member (`VulkanResources`, `VulkanStartup`, `VulkanHost`, and
  any Vulkan-named viewer/diagnostic surface) and its GL replacement, and the new package version
  MUST reflect the breaking change.
- **FR-010**: Governance artifacts and tokens that hard-code "Vulkan" as a required/expected term
  (e.g. the generated `runtime-limitations.md` token rule, smoke clauses, ADRs, skills,
  architecture/reports docs) MUST be updated so the backend swap does not leave governance
  asserting a backend that no longer exists.
- **FR-011**: The project constitution MUST be amended to reflect the OpenGL backend (replacing the
  Vulkan-backend mandate and the "Vulkan smoke" clause) via the constitution workflow, since the
  rendering backend is a constitution-level decision.

> Interacting / conflicting requirements: FR-002 (visual parity) vs. FR-008/FR-011 (backend
> replacement) — resolution: the **scene renderer stays unchanged** and only the **present/host
> backend** is swapped, so parity is preserved by construction; any pixel difference is treated as
> a defect unless explicitly documented as an intentional GL deviation. FR-007 (keep the public
> `ViewerPresentMode` DU) vs. FR-009 (breaking surface change) — resolution: prefer **retaining**
> the `ViewerPresentMode` contract and re-mapping its meaning onto GL over removing it, so
> consumers of the present-mode seam are not broken even though the lower-level Vulkan host modules
> are.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This section is
> *expected* to name concrete packages, `.fsi` signatures, build targets, Vulkan/Skia/OpenGL, and
> evidence paths — that is its purpose.

- **Package impact**: Package **versions** change (a new bumped preview across the 12 packable
  libs) and package **contents** change for the host package: the public Vulkan host surface in
  `FS.Skia.UI.SkiaViewer.Host` (`Host/Vulkan.fsi`: `VulkanResources`, `VulkanStartup`,
  `VulkanHost`) is replaced by a GL host surface. No new package identity. The `dotnet new
  fs-skia-ui` template pin is a **separate follow-up track** (`/fs-skia-template-update`), not in
  this feature's merge scope.
- **Public contract impact**: **Breaking** `.fsi` change. `Host/Vulkan.fsi` public modules are
  removed/renamed/replaced; `SkiaViewer/PresentMode.fsi` (`ViewerPresentMode`) is retained but its
  documented semantics are re-mapped to GL. High-level consumer entry points
  (`runInteractiveApp` / `runInteractiveViewer` / `ViewerOptions`) MUST remain source-stable.
  Top-level and per-package surface baselines regenerate (`RefreshSurfaceBaselines`).
- **State workflow impact**: No change to Elmish update/command/effect/subscription semantics or
  the interpreter. The host loop's frame scheduling, injected-`Tick` animation-clock advance, and
  retained render step are preserved; only the present/swap mechanism beneath them changes.
- **Layout/rendering impact**: The rendering **present path** changes (Vulkan swapchain present →
  GL framebuffer + buffer-swap direct present); layout, charts, DataGrid, the `SKCanvas`-based
  scene renderer, and screenshot pixels are **unchanged**. Unsupported-environment diagnostics
  change from Vulkan-specific to GL-specific classification.
- **Evidence obligations**: Real evidence required under
  `specs/119-opengl-present-backend/readiness/`: live-host launch proof on a GPU-passthrough
  machine (`supported-host-persistent-launch.txt`); a present-path proof that direct mode does
  zero per-frame readback; sample-smoke captures matching baselines; the unsupported-environment
  classified-diagnostic capture; `evidence-audit.md` (verdict=PASS, 0 synthetic);
  `generated-validation.md` (package-resolution=resolved). Design source of record:
  `specs/118-backend-host-review/readiness/audit/opengl-backend-resolution.md`.
- **Unsupported scope**: macOS, mobile, and browser remain out of scope (GL deprecation on macOS
  is moot). No new platform support, no distribution/release changes, no visual redesign. This
  feature does **not** add new scene capabilities — it is a backend swap. WebGL/ANGLE/multiple
  simultaneous backends are out of scope (single GL host).
- **Build-target impact**: `Dev` (host build/tests), `DependencyReport` (package swap),
  `TemplateCheck` / `GeneratedProductCheck` (consumer compile against new surface — an expected
  pin-lag failure pre-merge, resolved by the template follow-up), `GeneratedGuidanceCheck` and the
  governance gates (token churn from FR-010), `PackageSurfaceCheck` / `PerPackageSurfaceDiff`
  (breaking surface), `EvidenceGraph`, and `EvidenceAudit` must all be exercised. Run
  `./fake.sh build -t Route` to confirm the authoritative escalated gate list.

## Success Criteria *(mandatory)*

- **SC-001**: In the direct-present mode on a GPU-passthrough desktop, the steady-state present
  path performs **zero per-frame GPU→CPU readback** (measurably eliminated relative to the prior
  readback path), unblocking feature 118's SC-002.
- **SC-002**: For every existing sample-smoke and evidence scene, the rendered output under the GL
  backend matches the established visual baseline (no unintended pixel differences).
- **SC-003**: Pointer, keyboard, focus, and animation interactions in the interactive host behave
  identically to the prior backend across the existing interaction tests/samples.
- **SC-004**: On a machine without working GL/GPU passthrough, the host emits a correctly
  classified diagnostic (environment-limitation vs. defect) 100% of the time and never reports a
  false success.
- **SC-005**: High-level consumer entry points (`runInteractiveApp` / `runInteractiveViewer` /
  `ViewerOptions`) compile unchanged against the new package version; every removed/renamed public
  member is named in the migration guidance with its replacement.
- **SC-006**: No governance gate asserts a backend that no longer exists — all Vulkan-required
  tokens, smoke clauses, and the constitution are updated to the GL backend, and the full routed
  gate set (including `EvidenceAudit`) passes with 0 synthetic evidence.

## Assumptions

- Supported targets remain Windows and Linux desktop with GPU passthrough; the dev environment
  provides GL (Mesa/vendor drivers), consistent with the constitution's GPU-passthrough
  expectation.
- The windowing toolkit already in use (Silk.NET) supports creating a GL context and performing
  the buffer swap; no new windowing dependency is required beyond swapping the Silk.NET Vulkan
  packages for the Silk.NET OpenGL package.
- SkiaSharp's GL interop (`GRGlInterface`, `GRGlFramebufferInfo`,
  `GRBackendRenderTarget.GetGlFramebufferInfo`, `SKSurface.Create` over FBO 0) is available in the
  pinned SkiaSharp version, as documented in feature 118's resolution.
- The single-threaded render loop suits GL's thread-affine context; no change to the threading
  model is assumed.
- The `ViewerPresentMode` public DU is retained (semantics re-mapped) rather than removed, to keep
  the present-mode seam stable for consumers.
- The template re-pin and any downstream `dotnet new` validation are handled as a separate
  follow-up track after merge, matching the established repo workflow.

## Dependencies

- **Blocks/unblocks**: Directly unblocks feature 118 FR-002/SC-002 (readback-free direct present).
- **Design source of record**:
  `specs/118-backend-host-review/readiness/audit/opengl-backend-resolution.md` and
  `present-path-audit.md`.
- **External**: SkiaSharp GL interop; Silk.NET.OpenGL (added); Silk.NET.Vulkan +
  .Extensions.KHR (removed). `Directory.Packages.props`, `DependencyReport`, and
  `docs/dependencies` change.
- **Governance**: Requires a constitution amendment (`/speckit-constitution`) and governance-token
  updates with this change (FR-010, FR-011).

## Key Entities

- **GL host backend**: the replacement for the Vulkan host — owns the GL context, the
  framebuffer-bound `SKSurface`, the per-frame draw + buffer-swap present, and resize/context-loss
  handling.
- **`ViewerPresentMode`**: retained public DU; its `DirectToSwapchain` / `OffscreenReadback` cases
  are re-interpreted for GL (direct framebuffer present vs. offscreen render + readback for
  evidence/capture).
- **Unsupported-environment diagnostic**: classified benign vs. blocking host warning for the
  missing/broken-GL case.
- **Public host surface**: the removed/replaced `VulkanResources` / `VulkanStartup` / `VulkanHost`
  contract and its GL successor (subject to migration guidance).
