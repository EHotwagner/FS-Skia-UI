# Feature Specification: Spread3 Consumer Feedback Remediation

**Feature Branch**: `122-spread3-consumer-feedback`
**Created**: 2026-06-14
**Status**: Draft
**Input**: User description: "get the feedback from the sibling repo spread3"

## Context

The `Spread3` sibling repo — a generated `dotnet new fs-skia-ui` spreadsheet-editor
demo that consumes this framework as packages — recorded plan/tasks/implement
feedback while being built. That feedback (snapshotted in-repo at
[`source-feedback.md`](./source-feedback.md)) names concrete framework-level
friction a consumer hit but could not fix from the generated product. This feature
remediates the genuine framework, template, governance-readiness, and skill gaps the
feedback surfaced.

> **Dogfood-verify discipline (precedent: feature 121).** Consumer feedback can name
> defects that a *later* framework feature already shipped a fix for. Before building,
> the plan phase MUST re-verify each claim against the current tree (features 118–121
> changed the host/present/idle path substantially) and build only the gaps that still
> exist, recording which claims were already-shipped vs. genuine. Requirements below are
> stated as consumer-observable outcomes, not as commitments to a specific mechanism.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Live interactive window presents static content without black-frame blink (Priority: P1)

A consumer runs their generated app's persistent interactive window
(`dotnet run` → `ControlsElmish.runInteractiveApp`) on a Wayland session with the
default `DirectToSwapchain` present path and the default windowed-fullscreen startup.
With static (un-mutated) model content on screen, the window shows the painted content
steadily — it does **not** interleave fully-black frames with the painted frames.

**Why this priority**: This is a visual-honesty defect on the default present path of
the default backend: what the live window shows diverges from what offscreen/screenshot
evidence shows (offscreen is correct; the live window blinks). It directly undermines
the framework's visual-evidence honesty contract and is unfixable from the consumer's
MVU/view layer. The Spread3 reporter tried three workarounds (size-aware view, frame-rate
cap, per-frame scene delta) and all failed or made things worse.

**Independent Test**: Reproduce on a Wayland + windowed-fullscreen + `DirectToSwapchain`
host with a static scene and confirm via a deterministic present-path probe (or live
harness) that consecutive presents of an unchanged scene never present an undrawn/black
buffer; the last-good frame is what the user sees on every present.

**Acceptance Scenarios**:

1. **Given** a static (reference-unchanged) scene under `DirectToSwapchain` on a
   multi-buffer swapchain, **When** the host presents repeatedly without a fresh paint,
   **Then** every presented buffer carries the last good frame (no undrawn/black buffer
   is ever presented).
2. **Given** the reporter's three reverted workarounds (size-aware view, `FrameRateCap =
   Some 60`, per-frame heartbeat delta), **When** the framework fix is in place, **Then**
   the blink is resolved **without** requiring any of those consumer-side workarounds.

---

### User Story 2 - Consumers can select window startup-state / present-sync / buffering without patching the framework (Priority: P1)

A consumer who needs to avoid the windowed-fullscreen + swapchain interaction (or who
simply wants a normal non-fullscreen window, a specific present-sync mode, or a known
buffer count) can express that through the public `ViewerOptions`/host surface, and the
generated `Program.fs` actually applies the consumer's parsed window behavior to the live
window — not only to the options/diagnostics report.

**Why this priority**: Today `ViewerOptions` exposes only
`Title`/`InitialSize`/`PresentMode`/`FrameRateCap`, and the generated `Program.fs` threads
`--window-startup normal` into the *options report* but launches `runInteractiveApp` with
the default windowed-fullscreen behavior — so the documented scaffold-map remedy is inert.
A consumer cannot escape the failing configuration from the product side. This is the
"give the consumer a real knob" companion to US1.

**Independent Test**: With a generated app, pass the window-behavior flag the scaffold-map
documents (e.g. `--window-startup normal`) and confirm the **actual** launched window
reflects it (normal, non-windowed-fullscreen); and confirm the new public viewer
surface lets a host select startup-state / present-sync / buffer count.

**Acceptance Scenarios**:

1. **Given** a generated app launched with `--window-startup normal`, **When** it starts,
   **Then** the live `runInteractiveApp` window is a normal window (the flag is applied to
   the launch, not just reported in `manualWindowOptionResults`).
2. **Given** the public host/viewer surface, **When** a consumer sets a non-default window
   startup-state, present-sync mode, or buffer count, **Then** the live window honors it,
   with byte-identical behavior to today when the consumer leaves the new knobs at their
   defaults.

---

### User Story 3 - `CustomControl` render behavior is honest in docs, catalog, and skills (Priority: P2)

A consumer authoring bespoke geometry consults the catalog/skill for `custom-control` and
learns — before writing a full View — whether `Control.renderTree` actually rasterizes
`CustomControl` content, and what to do when geometry must appear in the
rasterized/screenshot path.

**Why this priority**: The catalog text presents `custom-control` as the "product-owned
wrapper for custom Skia content," strongly implying `renderTree` paints its
`Render`/`Draw`/`Layout.Content`. It does not — `renderTree` paints a `custom-control`
placeholder label (and threw an NRE under reflection), so the live host and the
view/screenshot path diverge. The reporter authored the whole grid as one `CustomControl`,
discovered the divergence only at runtime, and paid a full View rewrite to primitive
controls. This is a documentation/honesty defect with a clear, low-risk fix; the
behavioral question (paint it vs. document it) is decided in plan.

**Independent Test**: A consumer reading the `custom-control` catalog entry and the
relevant skill can correctly predict whether `renderTree` paints `CustomControl` content,
and is pointed to the primitive-control recipe for must-rasterize geometry — verified by a
governance/doc check, not only prose.

**Acceptance Scenarios**:

1. **Given** the `custom-control` catalog entry and the `fs-skia-ui-widgets` /
   `fs-skia-typed-controls` skill, **When** a consumer reads them, **Then** they state
   plainly that `CustomControl` content is **not** painted by `Control.renderTree` and that
   must-show geometry should be built from primitive controls
   (`Border`+`TextBlock`+`Stack`), OR — if plan chooses the behavioral fix —
   `renderTree` paints `CustomControl` content and the docs say so.
2. **Given** a `CustomControl` reached via the reflection/screenshot path, **When**
   `renderTree` processes it, **Then** it does not throw (no NRE).

---

### User Story 4 - Readiness-evidence token form is consistent (or its per-file shape is documented) (Priority: P2)

A consumer (or agent) authoring readiness/window-visibility evidence can satisfy every
scanner using one documented token convention, instead of discovering by trial that some
files accept bold-prose tokens while others silently require a literal `key=value` shape.

**Why this priority**: `governance-risk-levels` / `runtime-limitations` / `close-reason` /
`window-options` accept bold-prose tokens, but `interactive-visible-window.md`,
`window-state-diagnostics.md`, and `generated-validation.md` only matched once the fields
were literal `status=…` / `native-handle=observed:…` / `exact-package-match=true` lines.
`evidence-formats.md` lists the *terms* but not the per-file `=value` requirement. This
costs avoidable author churn and is a recurring trap noted across prior features.

**Independent Test**: Following `evidence-formats.md` alone, an author produces evidence
that passes every readiness scanner on the first attempt; the per-file token-shape
expectation is either uniform or explicitly documented.

**Acceptance Scenarios**:

1. **Given** the readiness scanners, **When** an author follows the documented token
   convention, **Then** prose and `key=value` forms are accepted uniformly, OR
   `evidence-formats.md` explicitly names which files require the `=value` shape and with
   what exact keys.

---

### User Story 5 - Scaffold and skill authoring traps are removed (Priority: P3)

A consumer extending a generated product can (a) learn from `docs/scaffold-map.md` that new
files may be added as long as the six scanned scaffold files keep their relative compile
order, and (b) author `skillist` ids without falling into the directory-name-vs-`name:`
mismatch (the UI-widgets skill lives in `fs-skia-ui-widgets/` but declares
`name: spread3-widgets`).

**Why this priority**: Both are documentation/guidance papercuts that cost a governance-test
read or a dangling `skillist` id, not blockers. Bundled here because they are cheap, share
the feedback source, and reduce repeat friction.

**Independent Test**: An author adding a new scaffold file, and an author writing a
`skillist` id, each succeed using the documented guidance without reading governance-test
internals.

**Acceptance Scenarios**:

1. **Given** `docs/scaffold-map.md`, **When** a consumer needs to add a new source file,
   **Then** the map states that additional files are allowed provided the six scanned files
   retain their relative order.
2. **Given** the tasks-template advisory hints / skill-assignment guidance, **When** an
   author references the UI-widgets skill, **Then** the resolved `name:` is surfaced (or the
   directory/name are aligned) so the authored `skillist` id resolves.

---

### Edge Cases

- **US1**: Single-buffer vs. multi-buffer swapchains; the fix must not regress the
  feature-120 idle skip-paint optimization (skip the *paint*, still present the last good
  frame — never present an undrawn buffer).
- **US1/US2**: Non-Wayland (X11/headless) hosts and the OpenGL vs. Vulkan backend must not
  regress; offscreen/screenshot evidence (already correct) must stay byte-identical.
- **US2**: Default values for any new `ViewerOptions`/window knobs MUST reproduce today's
  behavior exactly (byte-identical) so existing generated apps are unaffected.
- **US3**: A `CustomControl` with null/empty `Render`/`Draw`/`Layout.Content` must not throw
  on any render path (live, reflection, screenshot).
- **US4**: Existing already-passing readiness evidence across prior features must keep
  passing (no scanner tightening that breaks historical evidence).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The `DirectToSwapchain` present path MUST never present an undrawn/black
  buffer for a static (reference-/structurally-unchanged) scene; on a skipped paint it MUST
  present the last good frame (or otherwise keep every swapchain buffer populated), so a
  static live window does not interleave black frames. Reproduction MUST cover Wayland +
  windowed-fullscreen.
- **FR-002**: The fix in FR-001 MUST preserve the feature-120 idle optimization — a skipped
  paint stays a skipped *paint* (no clear/scene-walk/draw), it does not become a forced full
  repaint — and MUST be byte-identical for offscreen/screenshot evidence.
- **FR-003**: The public host/viewer surface MUST let a consumer select a non-default window
  startup-state (e.g. normal, non-windowed-fullscreen). Leaving it unset MUST reproduce
  today's default behavior byte-identically.
- **FR-004**: The public host/viewer surface MUST let a consumer select present-sync
  behavior (e.g. vsync/FIFO) and/or swapchain buffer count, sufficient to avoid the
  windowed-fullscreen + swapchain blink without framework edits. Unset values MUST reproduce
  today's defaults. *(Plan verifies which of present-sync / buffer-count / startup-state are
  individually necessary versus already covered.)*
- **FR-005**: The generated `Program.fs` scaffold MUST thread the consumer's parsed window
  behavior (e.g. `--window-startup normal`) into the **actual** `runInteractiveApp` launch,
  not only into the options/diagnostics report — so the documented scaffold-map remedy is
  effective rather than inert. This implies a public host entry point that accepts the
  parsed window behavior.
- **FR-006**: `Control.renderTree` MUST NOT throw (no NRE) when it encounters a
  `CustomControl`, including via the reflection/screenshot path and with null/empty content.
- **FR-007**: The catalog entry for `custom-control` and the `fs-skia-ui-widgets` /
  `fs-skia-typed-controls` skills MUST honestly state `renderTree`'s actual `CustomControl`
  behavior — either (a) `renderTree` does **not** rasterize `CustomControl` content and
  must-show geometry should be built from primitive controls, or (b) plan elects to make
  `renderTree` paint `CustomControl` content and the docs reflect that. The shipped docs and
  shipped behavior MUST agree.
- **FR-008**: Readiness-evidence token matching MUST be made consistent OR
  `evidence-formats.md` MUST explicitly document which readiness files
  (`interactive-visible-window.md`, `window-state-diagnostics.md`, `generated-validation.md`)
  require the literal `key=value` shape and with which exact keys. Historical passing
  evidence MUST continue to pass.
- **FR-009**: `docs/scaffold-map.md` MUST state that new source files may be added provided
  the six scanned scaffold files (`Model.fs → View.fs → LayoutEvidence.fs → WindowOptions.fs
  → EvidenceCommands.fs → Program.fs`) keep their relative compile order.
- **FR-010**: The UI-widgets skill directory/`name:` mismatch MUST be removed as an authoring
  trap — either by surfacing the resolved `name:` in the tasks-template advisory hints /
  skill-assignment guidance, or by aligning the directory name with the declared `name:`.
- **FR-011**: The `fs-skia-viewer-host` skill MUST document the interleaved-black-frame
  symptom on Wayland `DirectToSwapchain` windowed-fullscreen and the real remedy/knobs from
  FR-001/FR-003/FR-004 (today it documents only the windowed-fullscreen blur caveat). It MUST
  NOT recommend the reverted "size-aware view" workaround, which the reporter found
  ineffective and an O(cells) ANR trap.
- **FR-012** *(optional / plan-decided)*: A skill note SHOULD capture the
  no-new-dependency property-test pattern (deterministic generative loops through the real
  engine, disclosed in the test header) used when a test project ships no FsCheck reference.

> Interacting / conflicting requirements: FR-001 (never present a black buffer) vs. FR-002
> (preserve the idle skip-paint) — resolution: the skip applies to the *paint*, never to
> *presenting a populated buffer*; the present path re-presents the last good frame, so idle
> CPU/GPU savings are retained while the visible buffer is always non-black. FR-003/FR-004
> (new knobs) vs. byte-identical defaults — resolution: every new knob defaults to today's
> exact behavior; only an explicit non-default value changes anything.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Library package *contents* change (host/present path, `ViewerOptions`,
  a window-behavior-aware `runInteractiveApp` entry point). Expect a version bump and a
  re-pack of the affected libs (per the established post-merge flow). The `dotnet new
  fs-skia-ui` template changes (generated `Program.fs` window-behavior threading; possibly
  scaffold docs) and must be re-pinned to the new package versions. No package *identity*
  change.
- **Public contract impact**: `.fsi` signatures change — additive `ViewerOptions` fields
  (startup-state / present-sync / buffer-count) and a new window-behavior-aware host entry
  point (e.g. a `runInteractiveApp…WithWindowBehavior` overload). Surface baselines and the
  package-surface gates will move; XML-doc gates apply to new public members. If FR-007
  chooses behavioral CustomControl painting, `Control`/render surface may change; the
  doc-only path keeps the surface stable.
- **State workflow impact**: Host launch/loop and present scheduling change (present
  last-good-frame on skip; thread window behavior into launch). No change to the MVU
  update/command/effect model itself.
- **Layout/rendering impact**: Rendering/present path changes (swapchain buffer rotation,
  black-frame avoidance); potentially `Control.renderTree` `CustomControl` handling. Offscreen
  screenshots MUST stay byte-identical; live present visibility is the behavior under change.
  Wayland + windowed-fullscreen + `DirectToSwapchain` is the primary reproduction
  environment.
- **Evidence obligations**: Real evidence required — a `DirectToSwapchain` present-path probe
  (deterministic) showing last-good-frame on skip and no black buffer; a live Wayland
  windowed-fullscreen repro/repaired observation where feasible; offscreen byte-identical
  goldens; window-visibility / generated-validation readiness evidence in the corrected token
  form; `TemplateCheck` / `GeneratedProductCheck` green after the template re-pin; full
  escalated gate set + `EvidenceAudit` PASS with 0 synthetic.
- **Unsupported scope**: Not in scope — new chart/graph/DataGrid features; macOS/iOS/Android/
  browser support; the generalizable formula-engine / primitive-grid recipes from the
  feedback (candidate `FS.Skia.UI.SkillSupport` triage, tracked separately, not built here);
  the convenience `skillist` pre-flight lint (FR-010 covers the trap via guidance/alias, not
  a new gate unless plan finds it cheap).
- **Build-target impact**: `Route` will escalate (template/**, public `src/**/*.fsi`,
  governance/readiness, `.specify/**`, skill tree all in play) to the `maintainer-verify`
  path. Expect `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit`, plus the controls/package-surface gates and
  `SkillSyncCheck` (skill-tree edits). Run `./fake.sh build -t Route` first and run only the
  gates it prints.

## Success Criteria *(mandatory)*

- **SC-001**: On a Wayland + windowed-fullscreen + `DirectToSwapchain` host showing static
  content, 0 black/undrawn frames are presented over a sustained observation window — the
  user perceives steady content with no blink (was: visible interleaved black-frame blink).
- **SC-002**: A consumer can launch a generated app as a normal (non-windowed-fullscreen)
  window — and/or select present-sync/buffer-count — using only the public surface and the
  documented flag, with no framework source edits.
- **SC-003**: `--window-startup normal` (and equivalent parsed window behavior) is applied to
  the actual live window, verifiable by observing the launched window's state — not merely
  echoed in the options report.
- **SC-004**: Leaving every new knob at its default produces byte-identical behavior and
  byte-identical offscreen/screenshot evidence versus the pre-feature baseline.
- **SC-005**: A consumer reading the `custom-control` catalog entry + skill can correctly
  predict `renderTree`'s `CustomControl` behavior before authoring; `renderTree` never throws
  on a `CustomControl` (reflection/screenshot path included).
- **SC-006**: Following `evidence-formats.md` alone, an author produces readiness/
  window-visibility evidence that passes every scanner on the first attempt.
- **SC-007**: An author adds a new scaffold file (six-file order preserved) and authors a
  UI-widgets `skillist` id, each using only the documented guidance — no governance-test read
  required, no dangling skill id.
- **SC-008**: The full escalated gate set + `EvidenceAudit` pass with verdict PASS and 0
  synthetic tasks; `TemplateCheck` / `GeneratedProductCheck` green after the template re-pin.

## Assumptions

- The feedback's framework claims reflect the tree as of Spread3's build (2026-06-14). Per the
  dogfood-verify discipline, plan re-verifies each against features 118–121; any claim already
  remediated is recorded as already-shipped and dropped from scope rather than re-built.
- The default backend is OpenGL (feature 119) with default `PresentMode = DirectToSwapchain`;
  the blink reproduces on that default path. The fix targets that path without regressing the
  Vulkan path or headless/X11 hosts.
- For FR-007, the **doc/honesty** fix (state that `renderTree` does not paint `CustomControl`
  content + point to the primitive-control recipe) is the assumed default because it is
  low-risk and surface-stable; plan may instead elect the behavioral fix if cheap and
  consistent. Either way, shipped docs and shipped behavior must agree.
- New `ViewerOptions`/host knobs are additive with defaults equal to today's behavior; existing
  generated apps recompile and run unchanged.
- The generalizable formula-engine and primitive-grid recipes (feedback "Generalizable code")
  are candidate `SkillSupport` triage, not framework requirements; they are out of scope here.

## Dependencies

- Builds on the host/present/idle work of features 118 (backend/host review), 119 (OpenGL
  present backend), 120 (paint replay + skip-present), and 121 (idle parking + `FrameRateCap`).
- Template re-pin (per `fs-skia-template-update`) follows the post-merge library version bump.
