# Feature Specification: Live Host Pacing, Surface Honesty & Viewer Ergonomics

**Feature Branch**: `121-live-host-idle-parking`
**Created**: 2026-06-14
**Status**: Draft
**Input**: User description: "get the feedback from the sibling repo controlshowcase4"

## Provenance

This feature is sourced from downstream feedback captured while building the
**ControlsShowcase4** gallery against `FS.Skia.UI` `0.1.127-preview.1` (features
111–120). The authoritative feedback lives at
`/home/developer/projects/ControlsShowcase4/specs/001-controls-gallery/feedback/`
(`implement-2026-06-13.md`, severity **major**, plus three minor phase files).

### Reconciliation to shipped truth (why this spec is narrower than the raw report)

Before specifying work, the live path was traced end-to-end
(`runInteractiveViewerWithWindowBehavior` → `runPresentedPersistentWindow` →
`src/SkiaViewer/Host/OpenGl.fs` `runEventLoop`). **Three load-bearing claims in the
downstream report do not hold against 0.1.127**, so this spec reconciles them as
*already shipped* rather than re-building them (re-implementing shipped behavior
would produce synthetic evidence that `EvidenceAudit` is designed to block):

- **"Feature 120's unchanged-frames-do-no-paint is not yet on the live path."**
  Already shipped. `src/SkiaViewer/Host/OpenGl.fs` `renderFrame`/`shouldPresent`
  (~481–504): on `DirectToSwapchain` (the live default since feature 119), an
  unchanged scene at an unchanged framebuffer size performs **no clear, no scene
  walk, no draw calls** — it re-presents the front buffer.
- **"The tree is re-stamped/re-stepped/painted every frame regardless."** Not true
  for the live loop. In `src/SkiaViewer/SkiaViewer.fs`, `currentScene` is recomputed
  **only** on dispatch/key/pointer/resize (~2515–2551); a pure tick with
  `host.Tick = None` reuses the same scene **reference** → `shouldPresent = false` →
  paint skipped. `host.View` is **not** called per frame on the live path (unlike the
  deterministic `Perf.runScript` path the report appears to have read).
- **"`InteractiveAppHost` gives the pure `update` no close/quit effect."** Not true.
  `InteractiveAppHost.Update : 'msg -> 'model -> 'model * ViewerEffect list`, and
  `CloseWindow` is a public `ViewerEffect`; returning `[CloseWindow]` from `update`
  propagates to `AppRequestedClose` + `Shutdown` (`SkiaViewer.fs` ~1264–1273). A
  consumer can already wire "press Q to quit" today.

### The genuine remaining gaps (the scope of this feature)

1. **The native event loop free-runs.** `runEventLoop`
   (`src/SkiaViewer/Host/OpenGl.fs` ~858–885) is a manual
   `while { DoEvents(); DoUpdate(); DoRender(); Thread.Sleep(1) }` poll. Paint is
   skipped (feature 120), but `DoRender()` is still invoked every ~1 ms regardless of
   the frame target, and `TargetFrameRate` only gates `DoUpdate`. On a vsync'd
   compositor this would pace on present; **headless it free-runs** — the
   environment-bound residual the report observed.
2. **No consumer-side pacing lever.** `ViewerOptions` exposes only
   `{ Title; InitialSize; PresentMode }`; the loop's `TargetFrameRate = Some 60` is
   hard-coded with no way for product code to bound it.
3. **Per-tick allocation when idle.** `wrappedTick`
   (`src/Controls.Elmish/ControlsElmish.fs` ~1221–1233) rebuilds the per-identity
   animation-clock map (`Map.map` over `StateByIdentity`) **every** tick even when no
   clock is active — avoidable garbage on an otherwise-idle frame.
4. **Interactive surface not discoverable.** `PointerInteraction` /
   `ViewerPointerPhaseKind` / `PointerButton` have no entry under
   `docs/api-surface/`, forcing the downstream author to reflect over the compiled
   assemblies to recover the DU shapes.
5. **No present-mode / environment-limitation guidance.** The viewer-host skill does
   not state `DirectToSwapchain` (live) vs `OffscreenReadback` (evidence), warn that
   the generated default launch must not reuse the evidence `viewerOptions`, or record
   the no-compositor free-run as an environment limitation rather than a defect.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A consumer can pace the live loop (Priority: P1)

A consumer can bound the live interactive loop's render/update rate from viewer
configuration instead of the hard-coded 60, so a host without a blocking compositor
does not spin `DoRender` ~1000×/s. Combined with the already-shipped paint-skip
(feature 120), a capped, at-rest loop wakes at most N times/sec and does effectively
no work per wake.

**Why this priority**: This is the actionable core of the major-severity finding once
the already-shipped paint-skip is accounted for. It gives the consumer the lever the
report explicitly asked for and is the only change that measurably reduces the
free-running CPU on a no-compositor host. Independently shippable and observable.

**Independent Test**: Construct viewer options carrying an explicit frame-rate bound
and assert (a) the live loop's configured target equals that bound, not the hard-coded
60, and (b) the render loop's per-second `DoRender` cadence is bounded by it (asserted
on the pure pacing decision extracted from the loop, since the persistent window is not
drivable headless). Omitting the bound preserves today's exact behavior.

**Acceptance Scenarios**:

1. **Given** viewer options with an explicit frame-rate bound, **When** the live host
   runs, **Then** the native loop's frame target equals that bound, and **both** the
   update and the render cadence are gated by it (not just update).
2. **Given** viewer options without the bound (existing consumers), **When** they
   upgrade, **Then** the loop behaves exactly as today (default 60, byte-identical
   output), with no recompile required for existing call sites.
3. **Given** an invalid bound (zero or negative), **When** options are validated,
   **Then** validation fails with a clear startup diagnostic, consistent with how
   `InitialSize <= 0` is already rejected.

---

### User Story 2 - An idle live frame allocates nothing for animation (Priority: P2)

When the live host ticks and no per-identity animation clock is active, advancing the
clocks must not allocate a fresh clock map — an idle tick does no animation work and
produces no garbage, while a tick with an active clock still advances exactly as today.

**Why this priority**: A concrete, low-risk per-frame cost on the otherwise-idle path,
internal-only (no public surface), independently verifiable, and directly reduces the
residual work the report observed. Secondary to giving the consumer a pacing lever.

**Independent Test**: Drive the clock-advance seam with a state map that has no active
clock and assert the returned state map is reference-equal to the input (no
reallocation); drive it with an active clock and assert the clock advanced by the
delta exactly as before.

**Acceptance Scenarios**:

1. **Given** a retained state with no active animation clock, **When** a tick advances
   the clocks, **Then** the state map is returned unchanged (reference-equal, no
   allocation) and behavior is identical to today.
2. **Given** a retained state with at least one active clock, **When** a tick advances
   the clocks, **Then** every active clock advances by the delta exactly as before
   (no behavior change for live animation / cross-fade, features 099/103).

---

### User Story 3 - The interactive surface is honestly documented (Priority: P3)

A consumer authoring against the live host can discover the pointer/host signatures
and the correct present-mode for each launch context from the shipped API surface and
skills — without reflecting over compiled assemblies — and the docs state plainly what
is already shipped (live paint-skip; quit via `CloseWindow`) and what is an environment
limitation (free-run on a no-compositor host).

**Why this priority**: Pure honesty/discoverability; lowest risk; independent of US1/US2.
It is what would have saved the downstream author the reflection detour and the blank
off-screen window, and it reconciles the framework's documented story to shipped truth.

**Independent Test**: Confirm `PointerInteraction`, `ViewerPointerPhaseKind`, and
`PointerButton` appear under `docs/api-surface/`, that a governance/doc check fails if
they drift, and that the viewer-host skill enumerates present-mode selection plus the
environment-limit caveat.

**Acceptance Scenarios**:

1. **Given** a consumer implementing pointer narration, **When** they consult
   `docs/api-surface/`, **Then** the `PointerInteraction`, `ViewerPointerPhaseKind`,
   and `PointerButton` signatures (and the `MapPointer`/`MapKeyChord` folding contract)
   are present without reflection.
2. **Given** a consumer choosing a present mode, **When** they consult the viewer-host
   skill, **Then** it states `DirectToSwapchain` (persistent interactive launch) vs
   `OffscreenReadback` (evidence/screenshot capture) and warns that a generated default
   launch must **not** reuse the evidence `viewerOptions`.
3. **Given** the no-compositor free-run, **When** a consumer reads the viewer-host
   skill, **Then** it states the desktop-session prerequisites (compositor + vsync),
   the new frame-cap lever (US1), and that a headless host free-runs — an environment
   limitation, not a product defect — and records that live paint-skip and quit-via-
   `CloseWindow` are already shipped.

---

### Edge Cases

- **Frame-cap of zero/negative**: rejected at validation with a clear startup
  diagnostic, like `InitialSize <= 0`.
- **Frame-cap larger than the display refresh**: honored as a target; the loop never
  renders faster than the cap but is not obligated to reach it (the `Thread.Sleep(1)`
  yield and the paint-skip still apply).
- **Active clock settling on this tick**: the settling tick still advances and renders;
  the no-alloc short-circuit applies only when *no* clock is active going in.
- **Mixed state map (some clocks active, some settled)**: any active clock makes the
  tick non-idle; the map is rebuilt exactly as today (no behavior change), the
  short-circuit applies only to the all-inactive case.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Viewer configuration MUST let a consumer bound the live loop's frame rate
  instead of the hard-coded `TargetFrameRate = Some 60`. The field MUST be additive and
  defaulted so existing call sites that do not set it compile and behave exactly as
  today (default 60).
- **FR-002**: The consumer frame-cap MUST gate **both** the update and the render
  cadence of the native event loop, so the cap actually bounds `DoRender` (not only
  `DoUpdate` as today); a capped, at-rest loop must not spin `DoRender` faster than the
  cap.
- **FR-003**: An invalid frame-cap (zero or negative) MUST be rejected by options
  validation with a clear startup diagnostic, consistent with the existing
  positive-size validation.
- **FR-004**: The live per-tick animation-clock advance MUST NOT allocate when no
  per-identity clock is active: the advance MUST return the input state unchanged
  (reference-equal) on an all-inactive tick, and MUST advance active clocks by the
  delta exactly as today otherwise (no behavior change for features 099/103).
- **FR-005**: The published API surface (`docs/api-surface/`) MUST include the
  interactive pointer/host signatures the report had to reflect to recover —
  `PointerInteraction`, `ViewerPointerPhaseKind`, `PointerButton`, and the
  `InteractiveAppHost` `MapPointer` / `MapKeyChord` folding contract — and a
  governance/doc check MUST fail if they drift.
- **FR-006**: The viewer-host guidance (skill + docs) MUST document present-mode
  selection (`DirectToSwapchain` live vs `OffscreenReadback` evidence), warn that a
  generated default launch must **not** reuse the evidence `viewerOptions`, state the
  desktop-session prerequisites (compositor + vsync) and the new frame-cap lever, and
  record the no-compositor free-run as an environment limitation — not a product defect.
- **FR-007**: The guidance MUST reconcile the framework's story to shipped truth by
  stating that (a) live unchanged-frame paint-skip is **already shipped** (feature 120,
  `DirectToSwapchain`) and (b) a graceful in-app quit is **already available** by
  returning `[CloseWindow]` from the host `update` — so consumers neither expect a new
  scheduler nor reach for `kill`.
- **FR-008**: All changes MUST preserve byte-identical visual output at rest and
  identical live animation/input behavior; the only intended deltas are the new
  configuration field, the render-cadence gating, and the no-alloc idle tick. Behavior
  that cannot be exercised in the headless/no-compositor CI (the persistent window) MUST
  be proven on its extracted pure decision seams + reasoning and recorded honestly in
  `readiness/runtime-limitations.md` — no false "interactive pass".

> Interacting / conflicting requirements:
> - **Frame-cap vs paint-skip (FR-002 vs already-shipped feature 120).** The cap bounds
>   how often the loop *wakes to render*; the paint-skip bounds how much a wake *does*.
>   They compose: a capped at-rest loop wakes ≤ N×/s and does no draw work per wake. The
>   cap does not replace the paint-skip and vice versa.
> - **No-alloc idle tick vs animation liveness (FR-004 vs features 099/103).** The
>   short-circuit fires only when *no* clock is active; the instant any clock is active
>   the map advances exactly as today, so a cross-fade never freezes. "No active clock"
>   is the whole-map predicate, evaluated before deciding to reallocate.
> - **Default-preserving config vs public `.fsi` change (FR-001).** Adding a field to
>   `ViewerOptions` changes a public record; resolution: the field is added with a
>   defaulting construction path so existing positional/record call sites keep working
>   and at-rest output is byte-identical (FR-008).

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity/split change. Package **contents** change in
  `FS.Skia.UI.SkiaViewer` (`ViewerOptions` field + `runEventLoop` cadence gating) and
  `FS.Skia.UI.Controls.Elmish` (`wrappedTick` no-alloc). Post-merge version bump of all
  packable libraries per normal policy; `dotnet new fs-skia-ui` template re-pin is the
  chained `/fs-skia-template-update` follow-up.
- **Public contract impact**: **Yes.** `src/SkiaViewer/SkiaViewer.fsi` `ViewerOptions`
  gains an additive, defaulted frame-cap field. New `docs/api-surface/` entries for
  `PointerInteraction` / `ViewerPointerPhaseKind` / `PointerButton`. Surface baselines
  (`RefreshSurfaceBaselines`) regenerate. No `Controls.Elmish` `.fsi` shape change
  (the `wrappedTick` fix is internal).
- **State workflow impact**: The native event loop's render-cadence gating changes how
  often `DoRender` fires (paced, not per-poll); no change to effects/subscriptions, the
  MVU contract, or the already-wired `CloseWindow` quit path.
- **Layout/rendering impact**: Rendering **output** is byte-identical at rest (FR-008);
  rendering **cadence** is bounded by the new cap. No scene/chart/DataGrid/Skia/OpenGL
  pixel change. Present-mode guidance is documented, not changed.
- **Evidence obligations**: Unit evidence for the pure pacing decision (cap gates
  render cadence) and the no-alloc idle tick (reference-equality); a doc/api-surface
  drift check for the published pointer types; honest `readiness/runtime-limitations.md`
  note that the persistent window remains undrivable headless (free-run is environment-
  bound). Standard `specs/121-live-host-idle-parking/` evidence (`evidence-audit.md`
  with verdict token; `generated-validation.md`).
- **Unsupported scope**: No new platforms. No Spec Kit tooling (the minor-severity
  `speckit-spec-lint` / `speckit-source-snapshot` / `catalog-coverage` asks are out of
  scope — Spec Kit concerns, not framework). No new MVU/host contract field for quit
  (already available via `CloseWindow`). No attempt to make the headless CI window
  responsive — recorded, not fixed. The downstream scaffold's own `Program.fs`
  default-launch fix lives in ControlsShowcase4, not here.
- **Build-target impact**: Run `Route` first and run only the gates it prints; the
  public `ViewerOptions` `.fsi` change escalates to the viewer/controls public-surface
  set. Expected: `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`, plus
  `RefreshSurfaceBaselines` regeneration. No new governance gate is introduced.

## Success Criteria *(mandatory)*

- **SC-001**: A consumer can set an explicit frame-cap and observe the live loop's
  frame target equal it (not the hard-coded 60), with both update and render cadence
  bounded by it — proven on the extracted pacing decision.
- **SC-002**: Existing consumers that do not set the frame-cap observe byte-identical
  at-rest output and unchanged behavior; no existing call site needs editing to compile.
- **SC-003**: An at-rest tick with no active animation clock allocates nothing for the
  clock advance (returns the input state reference-equal), while an active clock still
  advances by the delta exactly as before.
- **SC-004**: A consumer can author pointer narration and choose a present mode using
  only `docs/api-surface/` and the viewer-host skill — no reflection over compiled
  assemblies — and the live-paint-skip / quit-via-`CloseWindow` / environment-limit
  facts are discoverable before first run.
- **SC-005**: An invalid frame-cap is rejected with a clear startup diagnostic rather
  than launching a misconfigured window.

## Key Entities

- **Viewer pacing configuration**: the additive, defaulted `ViewerOptions` frame-cap
  field, flowing to the native `ViewerConfiguration.TargetFrameRate` and gating the
  event loop's update **and** render cadence.
- **Animation-clock advance seam**: the live per-tick clock-map advance (`wrappedTick`)
  whose all-inactive case must be allocation-free (reference-equal passthrough).
- **Pointer interaction surface**: the `PointerInteraction` / `ViewerPointerPhaseKind`
  / `PointerButton` types and the `MapPointer`/`MapKeyChord` folding contract, to be
  published in `docs/api-surface/`.

## Assumptions

- The frame-cap field is additive and defaulted (default 60), so adding it to the
  public `ViewerOptions` record does not break existing consumers and keeps at-rest
  output byte-identical.
- The persistent interactive window cannot be driven in the headless/no-compositor CI
  (per `readiness/runtime-limitations.md`); US1's loop changes are therefore asserted on
  the extracted pure pacing decision plus reasoning, with the live free-run recorded as
  an environment limitation rather than claimed as an interactive pass.
- FR-005 (live paint-skip) and FR-007 (quit via `CloseWindow`) are **already shipped**;
  this feature documents and reconciles them, and does not re-implement them — doing so
  would create synthetic/duplicate-behavior evidence.
- Spec Kit tooling asks from the minor-severity feedback files (spec-lint,
  source-snapshot, catalog-coverage authoring) are acknowledged but out of scope — they
  are Spec Kit command/skill concerns, not `FS.Skia.UI` framework changes.
</content>
