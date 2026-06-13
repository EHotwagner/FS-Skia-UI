# Feature Specification: Backend Paint Replay & Performance Honesty

**Feature Branch**: `120-backend-paint-replay-cache`
**Created**: 2026-06-13
**Status**: Draft
**Input**: User description: "create specs for this fix plus other relevant problems detected."

## Context

This feature closes the loop opened by the controls-performance research report
(`docs/reports/2026-06-12-1422-controls-performance-framework-research.md`, Phase 7
and Phase 9) and resolves the concrete defects a faithfulness audit of features
109–119 surfaced. The audit's central finding: the controls stack reuses F# scene-list
fragments for unchanged subtrees, but **none of that reuse carries into the backend**.
The Skia/OpenGL present path re-clears the whole surface and re-walks the entire scene
into canvas draw calls on **every** present — even fully idle frames — allocating an
`SKPaint`/`SKPath` per primitive each time. The "paint cache" shipped in feature 116 is
**advisory only**: it counts hit/miss but its cache key (`sprintf "%A"` over the subtree
scene) is collision-prone by truncation, and it never skips any paint work.

The report gated real picture/replay caching on "metrics proving paint is the
bottleneck," but the per-phase paint timing that would prove it was never implemented.
This feature therefore proceeds in evidence-led order: make per-phase paint cost
observable, stop doing provably-redundant work, then make the paint cache load-bearing
with a key that is safe to depend on — and clean up the honesty defects the audit found
along the way.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Per-phase paint cost is observable (Priority: P1)

A maintainer profiling a heavy scene (1000+ controls, 10000-row DataGrid) needs to see
how much of each frame is spent translating the scene into draw calls versus presenting
it, so optimization is driven by evidence rather than inspection. Today only a single
whole-frame duration exists; paint and compose/present cost cannot be isolated.

**Independent test**: Run the existing performance corpus scenarios with the live timing
report generator and confirm each frame reports a distinct paint-phase and
present/compose-phase duration, captured into the non-golden baseline report. Verify the
deterministic count goldens are unchanged (timing stays out of goldens).

**Why P1**: This is the report's stated precondition for any cache work and is a
half-day of instrumentation. It must land first so US3's value is measured, not asserted.

### User Story 2 - Unchanged frames do no paint work (Priority: P1)

A user leaves an interactive window idle, or moves the pointer with no visual effect.
Today the backend still clears the full surface, re-walks the entire cached scene, and
re-issues every draw call on every loop iteration. The user gains nothing and the machine
burns CPU/GPU continuously. After this story, a frame whose scene is unchanged (and which
has no active animation, resize, or other dirty cause) performs **no** scene redraw.

**Independent test**: Drive the interactive host with a script that produces a steady
stream of no-op frames after the first render; assert via the paint diagnostic that the
first frame paints and every subsequent unchanged frame reports zero paint work and zero
draw-call re-issue, while the presented pixels remain correct (a forced repaint after the
idle run yields byte-identical pixels to the first frame).

**Why P1**: Largest, cheapest, lowest-risk win. It is "stop doing redundant work," not a
cache, so it carries near-zero correctness risk and does not depend on US3.

### User Story 3 - Stable subtrees replay instead of re-walking, with a key safe to depend on (Priority: P2)

When only a small region of a large scene changes (a hover, a single edited cell in a
10000-row grid), the unchanged subtrees should not be re-walked and re-translated into
draw calls every frame. After this story, reuse-stable subtrees are recorded once and
**replayed** on subsequent frames, skipping their walk and per-node allocation entirely.
Crucially, the cache key that decides "is this replay still valid?" must be
collision-free, so a replay can never present stale pixels.

**Independent test**: For every corpus scene, render with the replay cache enabled and
disabled and assert byte-identical presented pixels (read back and compared). Assert that
a steady-state frame reports replay hits that skip a measurable count of subtree paint
work, that a render-affecting change (theme color, text, geometry, opacity, clip) forces
re-record and a different scene, and that a constructed subtree which stringifies
identically under truncation but differs structurally produces a cache **miss** (the
collision the old key would have missed).

**Why P2**: The headline performance fix, and the one the audit flagged as deferred
(FR-008). It depends on US1 (to measure the win) and on the fingerprint fix being
correct.

### User Story 4 - Audit honesty and correctness defects are resolved (Priority: P3)

Several smaller defects the audit surfaced mislead consumers and maintainers about what
the system does. After this story: the public present-mode documentation matches the
shipped default; the interactive sample showcases the zero-readback present rather than
the legacy readback path; the damage-area metric reports a true region rather than an
over-count; and dead bookkeeping left over from an earlier internalization decision is
removed.

**Independent test**: Confirm the present-mode docstring states the shipped default;
confirm the interactive sample launches in the zero-readback present mode; confirm the
damage-area metric for two overlapping repainted regions is the union area (not the sum)
and never exceeds the frame area; confirm the removed dead reference no longer exists and
all gates still pass.

**Why P3**: Independent, low-risk correctness/honesty cleanups. They can land in any order
relative to US1–US3 but are bundled here because they emerged from the same audit and
touch the same present/metrics surfaces.

### Edge Cases

- **Churny subtree**: A subtree whose content changes every frame must not be recorded-
  then-discarded each frame (a net loss). Emission of a replay boundary is gated on the
  subtree having been reuse-stable on the prior frame.
- **Eviction under pressure**: When the cache exceeds its bound, the least-recently-used
  recorded picture is evicted and its native resources released; a later request for an
  evicted identity re-records (a miss), never a stale hit.
- **Coordinate space**: A replayed subtree must appear at exactly the position it was
  recorded at; a relayout that moves a subtree without changing its content must still
  produce correct placement (position is part of the validity decision or the replay
  transform).
- **Animation frames**: A paint-only animation frame (no model change) must still update,
  so the unchanged-frame skip (US2) must not suppress frames that have an active animation
  clock or other non-idle cause.
- **Resize / theme switch**: A resize or theme change invalidates dependent cached
  pictures; the first frame after such an event re-records as needed and is not skipped.
- **Idle skip and double buffering**: When a frame is skipped, the previously presented
  front buffer must remain valid on screen (no flicker / no stale back-buffer present).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST expose per-frame paint-phase and present/compose-phase
  durations as live (non-golden) diagnostics, alongside the existing whole-frame duration.
- **FR-002**: Per-phase timing MUST remain excluded from deterministic count goldens; the
  deterministic surface MUST stay byte-identical when timing fields are added.
- **FR-003**: The live timing report generator MUST capture the new per-phase durations
  for the existing performance corpus scenarios into the non-gating baseline report.
- **FR-004**: On a frame whose presented scene is unchanged and which has no other active
  dirty cause (animation, resize, theme, model change, or explicit tick requiring work),
  the backend MUST NOT clear and re-walk the scene or re-issue draw calls.
- **FR-005**: When a frame is skipped per FR-004, the previously presented image MUST
  remain correct on screen (no flicker, no stale buffer).
- **FR-006**: Frames with an active animation clock, resize, theme change, model change,
  or explicit work-requiring tick MUST continue to paint (the skip in FR-004 MUST NOT
  suppress them).
- **FR-007**: The system MUST cache recorded backend draw commands for reuse-stable
  subtrees and replay them on subsequent frames in place of re-walking and re-translating
  those subtrees.
- **FR-008**: The cache validity key MUST be a collision-resistant structural fingerprint
  of every render-affecting input of the subtree (geometry, colors, paths, text, font,
  opacity, transform, clip), replacing the current truncation-prone string digest. Two
  subtrees that differ in any render-affecting input MUST produce different fingerprints.
- **FR-009**: A replayed subtree MUST produce pixels byte-identical to walking and
  re-issuing that subtree's draw calls directly. Replay-enabled and replay-disabled renders
  MUST be byte-identical for every corpus scene (proven by pixel readback comparison).
- **FR-010**: Any render-affecting change to a subtree (including theme, text, geometry,
  opacity, clip, or transform) MUST invalidate its cached recording and force a re-record;
  no replay may present pixels that do not reflect the current inputs.
- **FR-011**: A replay-disable oracle (mirroring the existing cache-enabled oracle) MUST
  exist and MUST be exercised by the byte-identity parity proof; correctness MUST NOT
  depend on the cache being enabled.
- **FR-012**: Emission of a replay boundary MUST be gated on the subtree having been
  reuse-stable on the prior frame, so per-frame-churning subtrees are not recorded and
  immediately discarded.
- **FR-013**: The recorded-picture cache MUST be bounded (capacity limit with deterministic
  eviction) and its native/unmanaged resources MUST be released on eviction and on
  replacement; native memory used by the cache MUST be observable.
- **FR-014**: The system MUST report counts of replay hits, misses, records, and the
  count of subtree paint work skipped by replay, so the win is observable and regressions
  are detectable.
- **FR-015**: The damage-area metric MUST report the area of the union of distinct
  repainted regions, not the sum of their areas, and MUST never exceed the frame area.
- **FR-016**: The public present-mode option documentation MUST state the shipped default
  present mode (the zero-readback direct present), correcting the stale text that names the
  superseded readback default.
- **FR-017**: The interactive sample's live window MUST launch in the zero-readback direct
  present mode; evidence/screenshot capture paths that legitimately require readback MUST
  remain on the readback path.
- **FR-018**: Dead bookkeeping left from the runtime-state-touched-count internalization
  (a written-but-never-read reference in the interactive host) MUST be removed without
  changing behavior.
- **FR-019**: All changes MUST preserve at-rest byte-identity of presented output and of
  the deterministic metric goldens; new behavior is additive and gated behind oracles
  where it affects the render path.

> Interacting / conflicting requirements: FR-004 (skip unchanged frames) vs. FR-006
> (animation/resize/theme must paint) — resolved by cause: the skip applies only when
> **no** dirty cause is present; any active cause forces paint. FR-007 (replay for speed)
> vs. FR-009/FR-010 (byte-identity, no stale pixels) — resolved in favor of correctness:
> a fingerprint mismatch or any doubt always re-records; replay is a performance path that
> must be provably indistinguishable from the direct path, never a source of divergence.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This section
> names concrete surfaces by design.

- **Package impact**: Changes span `FS.Skia.UI` (Scene IR — a new additive cache-boundary
  node carrying identity + fingerprint), `FS.Skia.UI.Controls` (RetainedRender — structural
  fingerprint computed/memoized on the fragment, boundary emission gated on prior-frame
  stability), `FS.Skia.UI.Controls.Elmish` (`FrameMetrics` — new non-golden timing fields
  and replay counters), and `FS.Skia.UI.SkiaViewer` (backend `SKPicture` record/replay
  cache, present-path idle skip, present-mode docstring). All packable libraries bump
  version on merge; a follow-up template re-pin (`/fs-skia-template-update`) is expected.
  No package identity changes; no legacy Charts migration involved.
- **Public contract impact**: `SkiaViewer.fsi` present-mode docstring corrected (FR-016).
  New public `FrameMetrics` timing fields and replay counters (FR-001/FR-014) — additive,
  non-golden. Scene IR gains a cache-boundary node (internal if the IR permits, else a
  documented additive public case with transparent fallback). `RetainedRender.fsi`
  fingerprint helpers are internal. Top-level and per-package surface baselines change and
  must be regenerated (`RefreshSurfaceBaselines`).
- **State workflow impact**: The viewer present/effect path changes — the render effect
  gains an unchanged-frame skip and a record/replay step in the scene→canvas interpreter.
  No consumer-visible command/subscription contract changes.
- **Layout/rendering impact**: Core rendering path. Backend paint translation, `SKPicture`
  record/replay, present, and screenshot/readback all touched. Byte-identity of presented
  pixels and of screenshots is the governing invariant and must be proven on real hardware.
- **Evidence obligations**: (1) Cache on/off **pixel readback parity** on every corpus
  scene, on real AMD Mesa/OpenGL hardware (FR-009/FR-011). (2) Idle-frame **zero-redraw**
  proof via the paint diagnostic (FR-004). (3) Forced-staleness proof that a render-
  affecting change re-records (FR-010). (4) Live **before/after timing baseline** on the
  corpus into `docs/reports/_baselines` (FR-003). (5) Updated perf-corpus count goldens for
  the new replay/timing counters. (6) `EvidenceGraph` + `EvidenceAudit` pass, 0 synthetic.
- **Unsupported scope**: No damage-rect GPU clip of the redraw (a separate follow-on that
  depends on FR-015's union; out of scope here). No render-thread/compositor split (report
  "do later," gated on CPU-bound metrics). No Windows/macOS/mobile/browser live evidence —
  proof remains Linux/Mesa OpenGL only; Windows GL portability is asserted by code path,
  not launch-verified, and is called out as a residual risk rather than closed.
- **Build-target impact**: `Dev`; `GeneratedProductCheck` and `TemplateCheck` (package/
  template pins); controls/package public-surface gates (escalated by `.fsi` changes);
  `EvidenceGraph`; `EvidenceAudit`. `DependencyReport` unaffected. Run `Route` first and
  obey its printed gate list.

## Success Criteria *(mandatory)*

- **SC-001**: Every non-idle frame reports a distinct paint-phase and present/compose-phase
  duration in the live diagnostic; deterministic count goldens are byte-identical to before
  the timing fields were added.
- **SC-002**: After the first render, a run of unchanged frames performs zero scene redraws
  (no surface clear, no scene walk, no draw-call re-issue), and a forced repaint afterward
  is byte-identical to the first frame's pixels.
- **SC-003**: For every performance corpus scene, replay-enabled and replay-disabled
  renders produce byte-identical presented pixels (readback comparison).
- **SC-004**: On a small-change frame in the 10000-row DataGrid scene, the count of subtree
  paint work skipped by replay (`ReplaySkippedNodeCount`) is **at least 80%** of total subtree
  paint nodes, and the measured paint-phase duration is **at least 25% lower** than the
  replay-disabled baseline on the same hardware. (Byte-identity — SC-003 — is the gating
  correctness criterion; these thresholds are the directional performance bar, observed on the
  Linux/Mesa reference environment and recorded in the non-golden timing baseline, not a
  deterministic golden.)
- **SC-005**: A render-affecting change to any cached subtree always invalidates its
  recording (no stale-pixel case exists across the corpus and the forced-staleness test);
  a structural difference that the superseded truncating key would have collided on
  produces a cache miss.
- **SC-006**: The recorded-picture cache never exceeds its capacity bound, releases native
  resources on eviction/replacement, and reports its native memory use; memory does not
  grow unbounded across a long scripted run.
- **SC-007**: The damage-area metric equals the union area of distinct repainted regions
  for overlapping-damage cases and never exceeds the frame area.
- **SC-008**: The present-mode option documentation names the shipped zero-readback default;
  the interactive sample launches in that mode and presents with readback disabled (verified
  by the host's own present diagnostic).
- **SC-009**: The dead runtime-state-touched reference is absent from the host, and the full
  routed gate set plus `EvidenceAudit` pass with 0 synthetic evidence.

## Assumptions

- **Subtree selection scope**: Replay boundaries are emitted starting from the subtrees the
  existing advisory cache already identifies as reuse-stable (e.g. childless DataGrid rows),
  generalized via the prior-frame-stability gate (FR-012) to any subtree stable for at least
  one prior frame. A broader heuristic (depth/cost-weighted selection) is out of scope.
- **Fingerprint width**: A 64-bit (or wider, multi-lane) structural hash is assumed
  sufficient to make accidental collision negligible for the cache's purpose; correctness
  is additionally backstopped by the byte-identity oracle (FR-011), so a fingerprint
  collision degrades to a missed optimization, not a wrong pixel.
- **Idle-skip mechanism**: On the double-buffered GL present path, a skipped frame re-uses
  the last presented buffer; if the platform requires an explicit re-present to keep the
  front buffer valid, that re-present is treated as "no scene work," still satisfying FR-004.
- **Hardware**: All pixel-readback and timing evidence is captured on the project's Linux
  AMD/Mesa OpenGL reference environment, consistent with feature 119. Windows GL is in the
  declared support scope but is not launch-verified by this feature.
- **Damage-clip deferral**: Fixing the damage-area metric to a union (FR-015) is included
  because it is a detected metric defect and a prerequisite for future damage-rect GPU
  clipping; the GPU clip itself is explicitly deferred.

## Key Entities

- **Cache-boundary marker (Scene IR)**: An additive Scene node wrapping a subtree and
  carrying a stable cache identity and a structural fingerprint; when replay is disabled the
  backend treats it as transparent (recurses into the wrapped subtree), preserving
  byte-identity.
- **Subtree fingerprint**: A collision-resistant structural hash over a lowered subtree's
  render-affecting inputs, computed on repaint and carried unchanged when the fragment is
  reused (so its cost is proportional to damage, not tree size).
- **Recorded-picture cache (backend)**: A bounded, LRU-evicting store mapping cache identity
  to a recorded draw-command picture plus its fingerprint; owns native resources and their
  release.
- **Frame paint record**: The per-frame timing and replay-counter data (paint/compose
  durations, replay hit/miss/record counts, skipped paint-work count, damage union area,
  native cache bytes) surfaced through the metrics seam.
