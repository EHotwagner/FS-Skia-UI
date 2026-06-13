# Feature Specification: Backend and Host Mode Review

**Feature Branch**: `118-backend-host-review`  
**Created**: 2026-06-13  
**Status**: Draft  
**Input**: User description: "@docs/reports/2026-06-12-1422-controls-performance-framework-research.md do next part."

> **Source.** This is **Phase 9 (Backend and Host Mode Review)** — the **final**
> rung — of the staged controls-performance plan in
> `docs/reports/2026-06-12-1422-controls-performance-framework-research.md`.
> Phases 0–8 shipped as features 109–117 (Phase 8 = layout hot-path = feature
> 117, merged 2026-06-13). Phase 9 ensures the Skia/Vulkan viewer backend does
> not defeat the retained-controls performance won by the earlier rungs. There is
> **no successor phase**; this closes the roadmap.
>
> **What the audit found.** The live interactive present path
> (`SkiaViewer/Host/Vulkan.fs` `renderFrame`) renders every frame into an
> **offscreen** Skia/Vulkan surface, then performs a full GPU→CPU pixel readback
> (`SKSurface.ReadPixels`), then re-uploads those CPU pixels to the acquired
> swapchain image through a **per-frame** staging buffer and command pool, and
> waits on a **per-frame `vkQueueWaitIdle`** (a full pipeline stall) before
> `vkQueuePresentKHR`. Crucially the **live present path and the
> evidence/screenshot path share the same `renderSceneToPixels` readback
> routine**, so ordinary live frames pay the evidence-mode readback cost. This
> directly violates the report's Phase 9 acceptance criterion *"live mode has no
> accidental readback in ordinary frames."* No direct-to-swapchain rendering
> (`GRBackendRenderTarget` / `GRVkImageInfo`) exists anywhere in the codebase
> today; the offscreen-render-plus-readback path is the only present path.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - The live present path stops reading back every frame (Priority: P1)

A consumer runs an interactive FS.Skia.UI app in the windowed viewer. Today,
even when the scene is unchanged, every presented frame does a full GPU→CPU
readback of the rendered pixels and a CPU→GPU re-upload through a freshly created
staging buffer and command pool, gated by a per-frame full-queue stall
(`vkQueueWaitIdle`). That round-trip and stall are pure backend overhead that the
retained-controls work upstream cannot eliminate, and they defeat any pipelined
or threaded presentation the swapchain would otherwise allow.

This story adds an **opt-in** present mode, selectable through the public
`ViewerOptions`, that renders the Skia scene **directly onto the acquired Vulkan
swapchain image** (via a backend render target wrapping that image) and presents
it without a GPU→CPU readback, without a per-frame staging buffer / command-pool
allocation, and without the per-frame `vkQueueWaitIdle` stall on ordinary frames.
The **default** present mode is unchanged: it remains exactly today's
offscreen-render-plus-readback path, so existing behavior, screenshots, window
diagnostics, and visual output stay byte-identical. The direct path is therefore
a safe, opt-in performance path, not a behavior change to existing consumers.

**Why this priority**: This is the one item in the report's Phase 9 that removes
real, measurable per-frame backend cost and is the load-bearing acceptance
criterion ("no accidental readback in ordinary frames"). It is the headline of
this rung, and the user explicitly chose to land the fix (behind a flag) rather
than only document it.

**Independent Test**: Launch the windowed viewer twice over the same scene — once
in the default (readback) mode and once in the opt-in direct mode. Confirm both
present visually equivalent frames (compare via on-demand screenshots of each
path), and confirm via the live backend diagnostic (US2) that the direct-mode run
performs **zero** GPU→CPU readbacks on ordinary frames while the default-mode run
reports readback per frame. The default-mode run's visual output and window
diagnostics remain byte-identical to the pre-feature baseline.

---

### User Story 2 - The live backend reports whether it read back (Priority: P2)

A maintainer profiling the live viewer needs to know, for a given run, which
present mode is active and whether ordinary frames are performing a GPU→CPU
readback — the exact symptom the report's Phase 9 targets. The deterministic
`FrameMetrics` produced by the **headless** `Perf.runScript` driver cannot answer
this: `Perf.runScript` has no window and no Vulkan backend, so a backend
present/readback fact can never appear in a golden and a `FrameMetrics` field for
it would be permanently zero and misleading.

This story emits a **live-only, non-golden** backend diagnostic through the
existing `ViewerDiagnosticEvent` channel (`Category = Swapchain` or `Frame`,
delivered to the consumer's `ViewerDiagnosticsOptions.Sink`) reporting the active
present mode and whether ordinary frames read back (optionally with present /
readback timing). It is explicitly excluded from `Perf.runScript` goldens, and
**no `FrameMetrics` field is added**, preserving the report's deterministic-counts
vs. live-timing separation.

**Why this priority**: Observability makes US1's win provable on a live run and
makes a regression (a reintroduced readback) visible, but it is reporting-only and
does not itself remove work, so it ranks below the present-path fix.

**Independent Test**: Run the windowed viewer in each present mode with a
diagnostics sink attached. Assert the sink receives an event naming the active
present mode, and that the direct-mode run reports no per-frame readback while the
default-mode run reports readback. Assert `Perf.runScript` goldens are unchanged
(the diagnostic never reaches the deterministic metric path).

---

### User Story 3 - Hosting modes and backend limits are documented honestly (Priority: P3)

A consumer or maintainer needs to understand the distinct hosting/rendering modes
and their performance tradeoffs, and must not mistake deterministic evidence runs
for live-performance proof. Today the present-path readback, the per-frame stall,
and the shared live/evidence readback routine are undocumented, and there is no
single place stating that the deterministic render-only evidence mode is a
correctness/proof tool, not a frame-rate proxy.

This story produces an audit + hosting-mode tradeoff document that enumerates the
modes (`runInteractiveApp`, `runApp`, `runInteractiveViewer`, the bounded
evidence runs `runBounded`/`runForFrames`/`runUntilFirstFrame`, and the headless
`Perf.runScript`), records the present-path audit findings (the readback
round-trip and `vkQueueWaitIdle` stall, the shared live/evidence readback
routine, the absence of any direct-to-swapchain path before this feature), and
states explicitly that evidence/readback runs are deterministic proof and **not**
a live performance proxy.

**Why this priority**: This formalizes the report's Phase 9 documentation
acceptance criteria. It is reporting-only and depends on the audit US1 performs,
so it ranks last, consistent with prior rungs where observability/documentation
ranks below the mechanism.

**Independent Test**: The hosting-mode tradeoff document and the present-path
audit artifact exist under the feature readiness area, enumerate every host mode,
record the readback / stall findings with the concrete `Vulkan.fs` call sites, and
contain the explicit "evidence mode is not live performance proof" statement.

### Edge Cases

- **Direct-path initialization failure**: if the backend cannot create a direct
  swapchain render target (unsupported swapchain format/color type, Skia/Vulkan
  interop failure, or a driver that refuses the wrap), the viewer MUST fall back
  to the default readback present path and emit a `Warning` diagnostic, never
  crash and never present a corrupt/garbage frame.
- **Swapchain recreation (resize / minimize / device-lost recovery)**: the direct
  path's per-swapchain-image render targets MUST be recreated when the swapchain
  is recreated; resize behavior MUST stay correct under both present modes. The
  default readback path already handles resize and is unaffected.
- **Multiple swapchain images**: the direct path must wrap/track a backend render
  target per swapchain image index, not assume a single image.
- **Screenshots / evidence under direct mode**: an explicit screenshot/evidence
  capture still needs CPU pixels, so capture MUST continue to use the offscreen
  render-plus-readback routine **on demand** (only when a capture is requested),
  decoupled from per-frame present. Opting into direct present MUST NOT disable
  visual evidence.
- **Headless / `Perf.runScript`**: there is no window and no backend, so present
  mode is irrelevant there; `Perf.runScript` output and `FrameMetrics` are
  unchanged, and the live backend diagnostic never appears in that path.
- **Unsupported environments (no Vulkan / software-only / CI headless)**: when the
  viewer cannot start at all, the present-mode option is moot; existing
  unsupported-environment diagnostics and window-state classification are
  unchanged.
- **Color-type / sample-count match**: the direct render target MUST match the
  swapchain image's format and sample count (the current offscreen surface uses
  sample count 1); a mismatch is an init failure handled by the fallback above.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The framework MUST add an **opt-in** present-mode selector to the
  **public** `ViewerOptions` (`src/SkiaViewer`), whose **default value selects the
  existing offscreen-render-plus-readback present path**. With the option at its
  default, the live present path, screenshots, window diagnostics, and visual
  output MUST be byte-identical to the pre-feature baseline.
- **FR-002**: The framework MUST provide a **direct-to-swapchain** live present
  path — rendering the Skia scene onto the acquired Vulkan swapchain image through
  a backend render target wrapping that image — selectable via the FR-001 option.
  When active, ordinary live frames MUST perform **no** GPU→CPU pixel readback,
  **no** per-frame staging-buffer / command-pool allocation, and **no** per-frame
  `vkQueueWaitIdle` full-pipeline stall.
  > **BLOCKED-BY-DEPENDENCY (implementation finding, 2026-06-13).** This is
  > **infeasible on SkiaSharp**: the managed binding cannot create an `SKSurface`
  > from a Vulkan swapchain image (`SKSurface.Create` returns null even with a valid
  > `GRBackendRenderTarget` — [mono/SkiaSharp #1502](https://github.com/mono/SkiaSharp/issues/1502),
  > open since 2020; the Vulkan image-layout interop is unbound — #2191). Reproduced
  > live on a real AMD/RADV GPU; confirmed on the newest SkiaSharp `4.147.0-preview.3.1`.
  > The `DirectToSwapchain` *seam* ships and degrades safely to `OffscreenReadback`
  > (FR-005). The readback-free goal is **not achieved** this rung; its concrete
  > resolution is an **OpenGL present backend**
  > (`readiness/audit/opengl-backend-resolution.md`), captured as the next roadmap phase.
- **FR-003**: The direct present path MUST produce the **same presented visual
  output** as the default readback path for the same scene (the scene is rendered
  identically; only the present mechanism differs). Equivalence is proven by
  on-demand screenshot comparison and live visual evidence, **not** by
  deterministic `Perf.runScript` goldens (the headless driver has no backend).
- **FR-004**: Evidence/screenshot capture MUST remain available under **both**
  present modes. Capture MUST use the offscreen render-plus-readback routine
  **on demand** (only when a capture is requested), decoupled from the per-frame
  present path. This satisfies the report's Phase 9 task to **separate live
  rendering from evidence readback** — the offscreen+readback routine becomes the
  evidence path, and the direct path becomes the live-only present path.
- **FR-005**: The direct present path MUST **degrade safely**: on any failure to
  create or use a direct swapchain render target, the viewer MUST fall back to the
  default readback present path, emit a `Warning` diagnostic, and continue
  presenting correctly. A direct-path failure MUST NOT crash the viewer or present
  a corrupt frame.
- **FR-006**: Swapchain recreation (resize and recovery) MUST be handled under the
  direct path: per-swapchain-image render targets are recreated on swapchain
  recreation, and resize remains correct under both present modes.
- **FR-007**: The framework MUST emit a **live-only, non-golden** backend
  diagnostic through the existing `ViewerDiagnosticEvent` channel
  (`Category = Swapchain` or `Frame`, via `ViewerDiagnosticsOptions.Sink`)
  reporting the active present mode and whether ordinary frames perform a GPU→CPU
  readback (present / readback timing MAY be included). This diagnostic MUST be
  excluded from `Perf.runScript` goldens.
- **FR-008**: **No `FrameMetrics` field is added** for backend present/readback
  facts. `FrameMetrics` is produced by the headless `Perf.runScript` driver, which
  has no window or backend; a backend field would be permanently zero/absent in
  goldens and would mislead. The deterministic-counts (golden) vs. live-timing
  (diagnostic) separation the report mandates MUST be preserved.
- **FR-009**: The framework MUST produce an audit + hosting-mode tradeoff document
  that (a) enumerates the host modes (`runInteractiveApp`, `runApp`,
  `runInteractiveViewer`, the bounded evidence runs, and headless `Perf.runScript`)
  with their performance tradeoffs, (b) records the present-path audit findings
  (the per-frame readback round-trip, the per-frame `vkQueueWaitIdle` stall, the
  shared live/evidence `renderSceneToPixels` routine, and the prior absence of any
  direct-to-swapchain path) with the concrete `Vulkan.fs` call sites, and (c)
  states explicitly that deterministic evidence/readback runs are correctness
  proof and **not** a live performance proxy.
- **FR-010**: This rung MUST NOT introduce a render-thread/compositor split, a
  layer / scene-submission diffing mechanism, scene-graph caching, or any
  GPU/layer cache (deferred per the report's Phase 9 task 5 and "do not do yet"
  guidance). The direct present path is a single-threaded present-mechanism change
  only.
- **FR-011**: No timing-based pass/fail gate MUST be introduced. Backend timing is
  a human/diagnostic signal only; deterministic gating stays on counts and
  booleans, and live timing stays out of goldens.

> Interacting / conflicting requirements: **default byte-identity (FR-001) vs. the
> new present path (FR-002)** — resolve by keeping the default mode equal to today's
> readback path and making the direct path strictly opt-in; the direct path's proof
> is live smoke + on-demand screenshot equivalence (FR-003), not goldens. **No
> live-frame readback (FR-002) vs. evidence still needs CPU pixels (FR-004)** —
> resolve by performing readback **only on demand** for an explicit
> screenshot/evidence capture, never on the ordinary per-frame present; the live
> present path and the evidence readback path are thereby separated. **Direct-path
> performance (FR-002) vs. safety on unsupported backends (FR-005)** — safety wins:
> a direct-path failure always falls back to the proven readback path with a
> warning rather than risking a crash or corrupt frame.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This
> section is *expected* to name concrete packages, `.fsi` signatures, build
> targets, native APIs, and evidence paths.

- **Package impact**: No package identity or dependency change. The primary
  affected packable library is **`FS.Skia.UI.SkiaViewer`** (`src/SkiaViewer` —
  `SkiaViewer.fsi` public surface, `Host/Vulkan.fs` present path, `Host/Viewer.fs`
  configuration defaults, `Host/Diagnostics.fsi` `ViewerConfiguration`). Version
  bump on merge per the standard packable-library bump (continuing the
  `0.1.x-preview.1` series). No Charts package migration.
- **Public contract impact**: `SkiaViewer.fsi`'s public `ViewerOptions` record
  (currently `{ Title; InitialSize }`) gains a new present-mode field, and a new
  present-mode type (a closed, `[<RequireQualifiedAccess>]` DU) is added — a
  **public surface addition** that **escalates Route** to the SkiaViewer
  public-surface gate set (run exactly what `./fake.sh build -t Route` prints).
  Because `ViewerOptions` is a record, adding a field is a **breaking record-shape
  change**: **every** `ViewerOptions` construction site must add the new field
  (samples, FSI preludes, `RefreshSurfaceBaselines`-tracked sites, and the
  `dotnet new fs-skia-ui` template / generated product), so this likely also
  escalates **`TemplateCheck`** and **`GeneratedProductCheck`**. The default field
  value MUST be the readback path so all existing construction sites keep today's
  behavior. The internal `ViewerConfiguration` (`Host/Diagnostics.fsi`) gains a
  matching field threaded into `renderFrame`. XML-doc gate: `///` before each new
  public field/type, attribute-before-doc-before-type ordering preserved.
- **State workflow impact**: No change to stateful workflow, I/O, commands,
  effects, subscriptions, or interpreter behavior. Present mode is a render-backend
  selection with no observable effect ordering; the MVU/host update loop is
  unchanged.
- **Layout/rendering impact**: The live **present mechanism** changes for the
  opt-in mode only; the default mode is byte-identical. The **scene is rendered
  identically** under both modes, so layout, charts, DataGrid geometry,
  screenshots/evidence pixels, and visual output are unchanged. Unsupported-
  environment diagnostics and window-state classification are unchanged. No
  damage-rect (116), virtualization (114), or layout (117) behavior changes.
- **Evidence obligations**: `specs/118-backend-host-review/readiness/**` — the
  present-path audit artifact and hosting-mode tradeoff document (FR-009), a live
  smoke / visual-evidence artifact proving the direct path presents output
  equivalent to the readback path (FR-003) and that ordinary direct-mode frames
  perform zero readback per the live diagnostic (FR-007), a default-mode
  byte-identity proof (visual evidence + window diagnostics unchanged, FR-001), the
  safe-fallback evidence (FR-005), the `evidence-audit.md` verdict, and
  `generated-validation.md` (`package-resolution=resolved`, `package-mismatch=false`).
  Note: `Perf.runScript` metric goldens are **unchanged** (no `FrameMetrics` field
  added) — the absence of golden churn is itself an evidence point (FR-008).
- **Unsupported scope**: Out of scope and explicitly deferred — render-thread /
  compositor split, layer / scene-submission diffing, scene-graph caching,
  GPU/layer caches, and any timing-based pass/fail gate (report Phase 9 task 5 and
  "do not do yet"). This feature closes the controls-performance roadmap; there is
  no successor phase.
- **Build-target impact**: Run only what `./fake.sh build -t Route` prints
  (expected: the escalated SkiaViewer public-surface set because `ViewerOptions`
  `.fsi` changes, plus `TemplateCheck` / `GeneratedProductCheck` because
  `ViewerOptions` is constructed by the template and generated product). No change
  to the *definitions* of `Dev`, `TemplateCheck`, `GeneratedProductCheck`,
  `GeneratedGuidanceCheck`, `EvidenceGraph`, or `EvidenceAudit`; they run as the
  routed gate set requires.

## Success Criteria *(mandatory)*

- **SC-001**: With the present-mode option at its default, the live viewer's
  present path, screenshots, window diagnostics, and visual output are
  byte-identical to the pre-feature baseline (the default path is unchanged).
- **SC-002**: With the opt-in direct present mode active, ordinary live frames
  perform no GPU→CPU pixel readback, no per-frame staging-buffer/command-pool
  allocation, and no per-frame `vkQueueWaitIdle` stall; the live backend diagnostic
  reports the active mode and zero readback on ordinary frames.
  > **BLOCKED-BY-DEPENDENCY** — not achievable on SkiaSharp (see FR-002 note;
  > mono/SkiaSharp #1502/#2191). The `DirectToSwapchain` seam degrades safely to
  > `OffscreenReadback`; the live diagnostic instead reports the fallback to readback
  > (the honest live signal). Resolution: the OpenGL present backend
  > (`readiness/audit/opengl-backend-resolution.md`).
- **SC-003**: With the opt-in mode active, the presented visual output matches the
  default readback-mode output for the same scene (verified via on-demand
  screenshots of both paths).
- **SC-004**: Screenshot / evidence capture succeeds under both present modes, with
  readback performed only on demand for an explicit capture (decoupled from
  per-frame present).
- **SC-005**: When the direct path cannot initialize, the viewer falls back to the
  readback path, emits a `Warning` diagnostic, and continues presenting correctly
  with no crash and no corrupt frame.
- **SC-006**: Resize and swapchain recreation work correctly under both present
  modes.
- **SC-007**: `Perf.runScript` metric goldens are unchanged — no `FrameMetrics`
  field is added, and the live backend diagnostic never enters the deterministic
  metric path.
- **SC-008**: The present-path audit and hosting-mode tradeoff documentation exist,
  enumerate every host mode with its tradeoffs, record the readback / stall
  findings with concrete `Vulkan.fs` call sites, and state explicitly that
  evidence/readback runs are deterministic proof and not a live performance proxy.
- **SC-009**: The feature merges with the routed gate set (SkiaViewer
  public-surface plus the template/generated-product gates Route prints) and the
  evidence audit passing with zero synthetic tasks.
