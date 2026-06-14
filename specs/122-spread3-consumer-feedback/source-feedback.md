# Source feedback snapshot — Spread3 consumer dogfood

**Source repo**: `/home/developer/projects/Spread3` (sibling repo, separate git history)
**Source path**: `specs/001-spreadsheet-editor/feedback/`
**Fetched**: 2026-06-14 (local filesystem; the source is not tracked in FS-Skia-UI)
**Source commit**: `2f26ad7 Document live-window DirectToSwapchain blink investigation in implement feedback`

This snapshot preserves the consumer feedback in-repo so feature 122's specify
phase is reproducible even though the source lives in a different repository.

---

## plan-2026-06-14.md (severity: minor)

### Process friction
The plan phase went smoothly overall. The one real friction point was reconciling the
scaffold's compile-order governance invariant with the need to add a new pure engine file:
`GovernanceTests.fs` asserts an exact relative order over the six scaffold files
(`Model.fs → View.fs → LayoutEvidence.fs → WindowOptions.fs → EvidenceCommands.fs →
Program.fs`), and it was not immediately obvious whether inserting a new `Spreadsheet.fs`
would break that gate. It does not — the test checks relative indices of those six files
and their presence, so inserting before `Model.fs` is safe — but confirming this required
reading the governance test body rather than trusting `docs/scaffold-map.md` alone. A
one-line note in the scaffold map ("you may add new files as long as the six scanned files
keep their relative order") would have saved that lookup. The `docs/scaffold-map.md`
HUD→headers / gameplay→cell-grid worked example was excellent and removed most ambiguity
about which files are durable vs. replaceable.

### Generalizable code
none — the plan phase wrote no F# code. The candidate engine code (recursive-descent
formula parser, dependency-graph cycle detection, topological recompute) is planned but not
yet written; if it generalizes, the relevant skill families are `fsharp-parsing` (a tiny
arithmetic/expression grammar over `+ - * /`, parens, cell refs, `SUM`) and
`fsharp-graph-algorithms` (Kahn topo sort + cycle detection over the cell dependency map).

### Skill gaps
none — the existing skills covered the plan well. A small dedicated "scrollable grid /
virtualized cell layout" skill could help during implement, but it was not blocking.

---

## tasks-2026-06-14.md (severity: minor)

### Process friction
Task generation itself went smoothly — a complete 38-task DAG in one pass, `EvidenceGraph`
passed clean on the first run. The one real friction point: the UI-widgets capability skill
lives in directory `fs-skia-ui-widgets/` but its `SKILL.md` declares `name: spread3-widgets`,
while the tasks-template advisory hints and the visual-demo skill-assignment guidance both
refer to it as `fs-skia-ui-widgets`. The validator correctly requires the `name:` value, so
the directory-vs-name mismatch is an easy way to author a dangling `skillist` id. Surfacing
the resolved `name:` in the advisory-hints list (or aliasing the directory name) would remove
the trap.

### Skill gaps
none for authoring. A small lint that pre-flights `skillist` ids against the resolved `name:`
registry before `EvidenceGraph` (rather than only at graph time) would shorten the
write→validate loop, but it is a convenience, not a missing capability.

---

## implement-2026-06-14.md (severity: major)

### Process friction
1. **`CustomControl` is invisible to `Control.renderTree`.** The plan (research Decision 5)
   and the catalog both present `custom-control` as the "product-owned wrapper for custom
   Skia content", so the first View implementation drew the whole grid as one `CustomControl`
   whose `Render`/`Draw`/`Layout.Content` returned the grid `Scene`. `renderTree` ignored that
   content entirely and painted a `custom-control` placeholder label (and threw `NRE` under
   reflection). That meant the live host (`runInteractiveApp` → `renderTree`) and the
   `view`/screenshot path would diverge — a visual-evidence honesty hazard. The fix was to
   author the grid from primitive controls (`Border`+`TextBlock`+`Stack`) that `renderTree`
   actually paints, and route cell clicks through each cell's bound `Click` event. This is the
   honest, consistent design, but the catalog text strongly implies `CustomControl` content is
   rasterized by `renderTree`, which it is not — costing a full View rewrite to discover.

2. **The window-visibility / generated-validation readiness scanners require `key=value`
   token form, not prose.** `governance-risk-levels`/`runtime-limitations`/`close-reason`/
   `window-options` accept bold-prose tokens, but `interactive-visible-window.md`,
   `window-state-diagnostics.md`, and `generated-validation.md` only matched once the fields
   were embedded as literal `status=...`/`native-handle=observed:...`/`exact-package-match=true`
   lines. The per-file token-matching rule is inconsistent across the readiness contract;
   `evidence-formats.md` lists the *terms* but not that some files demand the `=value` shape.

### Generalizable code
- The pure `Spreadsheet.fs` engine (bijective base-26 `CellAddress`, recursive-descent
  `parseFormula`, resolver-driven `evaluate`, `recomputeWithBounds` with reach-to-self cycle
  detection) is a clean, dependency-free formula-engine pattern reusable by any grid/table demo.
- `View.framedCell`/`rowControl`/`headerRow` is a reusable "fixed-cell grid from primitive
  controls" recipe that `renderTree` paints reliably (where `CustomControl` does not).

### Skill gaps
- A note in `fs-skia-ui-widgets` (or `fs-skia-typed-controls`) clarifying that `CustomControl`
  content is NOT painted by `Control.renderTree` — and that bespoke geometry should be built
  from primitive controls when it must show in the rasterized/screenshot path — would have
  saved the rewrite.
- `T008` called for FsCheck property tests, but the test project ships no FsCheck reference and
  the plan's governance decision is "no dependency change". Resolved by hand-rolled deterministic
  generative loops through the real engine. A skill note on the no-new-dependency property-test
  pattern would help.

### Live-window rendering defect — interleaved black-frame blink (post-implement investigation)

**Severity for this item: major (framework/host-level, not fixable from the generated product).**

**Symptom.** The persistent interactive window (`dotnet run` → `ControlsElmish.runInteractiveApp`)
renders the spreadsheet grid but **blinks**, interleaving fully-black frames with the painted
grid. All *offscreen* evidence is unaffected and correct: `--screenshot-evidence` /
`--image-evidence` produce non-blank, decodable 640×480 PNGs from a live viewer window (712 scene
nodes). The defect is specific to the **persistent `DirectToSwapchain` present path**, not the
scene/content.

**Environment.** Wayland session (`WAYLAND_DISPLAY=wayland-0`), GTK host, `net10.0`. Viewer:
`PresentMode = DirectToSwapchain`, `InitialSize = 1280×800`, default startup =
**windowed-fullscreen** (the `WindowOptions.fs` default; the framework scales a fixed-resolution
scene up to the work area). Key diagnostic signal: **mouse clicks (which mutate the model → force
a paint) visibly change the blink pattern**.

**Hypotheses tested and results (all reverted — none fixed it):**
1. **Unpainted black margins** (the non-size-aware fixed 640×480 `view` scaled up by
   windowed-fullscreen). *Fix tried:* a **size-aware view** laid out to the actual swapchain
   extent, wrapped in a full-extent `Panel`. *Result:* **worse** — pinning the `Panel` to the
   extent stopped the windowed-fullscreen up-scaling (content showed at native ~640×480), and
   laying out the full 26×50 grid for a large extent exploded the control count, wedging the
   window into **"not responding" after a few clicks**. Reverted.
2. **Present pacing.** *Fix tried:* `viewerOptions.FrameRateCap = Some 60`. *Result:* still
   blinking (responsive; clicks still altered the blink). Reverted.
3. **Skip-paint-on-unchanged presenting an undrawn (black) buffer** (feature 120: on
   `DirectToSwapchain` a reference/structurally-unchanged scene does no clear/scene-walk/draw; on
   a multi-buffer swapchain a skipped frame can present an undrawn buffer). *Fix tried:* a
   per-frame scene delta — a `Frame` counter advanced by `Tick` folded into a tiny status-bar
   heartbeat glyph so the rendered scene differs every frame. *Result:* **still blinking.** This
   **rules out skip-paint** as the (sole) cause. Reverted.

**Conclusion.** The interleaved-black is in the framework's **`DirectToSwapchain`
present/buffer-rotation path on Wayland windowed-fullscreen**, independent of whether the app
forces a per-frame paint. It cannot be fixed from the generated product's MVU/view layer:
- The durable `Program.fs` launches `runInteractiveApp viewerOptions interactiveHost` and **does
  not thread the parsed window behavior into the live launch** — `--window-startup normal` only
  feeds the *options report* (`manualWindowOptionResults`), so the documented scaffold-map fix
  ("launch with `--window-startup normal`") is **inert** for the actual window.
- `ViewerOptions` exposes only `Title`/`InitialSize`/`PresentMode`/`FrameRateCap` — **no
  startup-state, buffer-count, or present-sync (vsync/FIFO) knob** — so a consumer cannot select
  a normal (non-windowed-fullscreen) window or change buffering without framework edits.

**Recommendations (framework / skill):**
- The `DirectToSwapchain` present path should **re-present the last good frame** on a
  skipped/undrawn buffer (or draw all swapchain buffers) so static content never interleaves
  black; reproduce on Wayland + windowed-fullscreen.
- Generated `Program.fs` should thread parsed window behavior (e.g. `--window-startup normal`)
  into the **actual** `runInteractiveApp` launch (a `runInteractiveApp…WithWindowBehavior`
  overload), not only the options report — otherwise the scaffold-map remedy is a no-op.
- `ViewerOptions` should expose **buffer-count / present-sync / startup-state** so a consumer can
  avoid the windowed-fullscreen + swapchain blink without patching the framework.
- `fs-skia-viewer-host` documents only the windowed-fullscreen **blur** caveat; it should also
  document this **interleaved-black-frame** symptom on Wayland `DirectToSwapchain` and a real
  remedy.

**Disposition.** All three live-window workarounds were **reverted** as ineffective (or actively
harmful); the committed Spread3 code remains the verified, gate-passing baseline. The blink is a
framework-level defect tracked for triage, not a generated-product regression.
