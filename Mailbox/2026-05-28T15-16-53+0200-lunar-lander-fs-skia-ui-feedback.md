# Lunar Lander Demo FS.Skia.UI Process Feedback

Timestamp: 2026-05-28T15:16:53+02:00

Source implementation: `/home/developer/projects/LunarDemo1`

## Context

The generated `LunarDemo1` app was changed from the placeholder Tetris-style sample to a deterministic 2D lunar lander demo. The implementation stayed inside the generated consumer app boundary and used the existing FS.Skia.UI packages:

- `FS.Skia.UI.Scene`
- `FS.Skia.UI.SkiaViewer`
- `FS.Skia.UI.Elmish`
- `FS.Skia.UI.KeyboardInput`
- `FS.Skia.UI.Layout`
- `FS.Skia.UI.Controls`
- `FS.Skia.UI.Controls.Elmish`

The final app worked interactively. The default executable path launched the persistent viewer, and evidence commands produced real artifacts, including live screenshot proof in this environment.

## What Worked Well

- The app-owned `Program.view`, `Program.update`, `Program.generatedHost`, and `Program.dispatchViewerKey` shape was workable for tests and generated guidance.
- The pure reducer plus host interpreter split was straightforward once the placeholder sample was removed.
- `Viewer.runApp` successfully supported persistent interactive launch for the finished demo.
- `Viewer.captureScreenshotEvidence` produced a live nonblank render-target screenshot with useful proof fields:
  - `capture-source=LiveViewerWindow`
  - `proves-screenshot=True`
  - `pixel-content-validation=PixelContentNonBlank`
- The existing explicit evidence command pattern made it easy to keep bounded proof separate from normal interactive play.

## Process Friction

### FAKE Evidence Targets Are Too Shallow

`./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit` only wrote completion logs in the generated app. The real graph and audit validation came from:

```bash
python .specify/extensions/evidence/scripts/python/compute-task-graph.py specs/001-lunar-lander-demo
.specify/extensions/evidence/scripts/bash/run-audit.sh specs/001-lunar-lander-demo
```

This creates a false sense of completion. A generated project can say `EvidenceGraph completed` while the real graph still fails.

Recommendation: wire generated `EvidenceGraph` and `EvidenceAudit` targets to the real extension scripts, or rename the generated stubs so they cannot be mistaken for authoritative validation.

### Skill-Loading Evidence Contract Is Brittle

The graph validator requires one `skill-loading-evidence.md` row per `(task id, skill id)`. A human-readable batch row such as `T002-T055 where declared` fails validation.

It also rejects `loaded_at >= work_started_at`, so equal timestamps or after-the-fact records fail even when the agent did load the skill before editing. The rule is sound, but the workflow depends on very precise bookkeeping.

Recommendation:

- Make the required row granularity explicit in `/speckit.implement`.
- Provide a script to generate the per-task/per-skill rows from `tasks.deps.yml`.
- Record skill load events automatically when a skill is opened, instead of relying on manual Markdown rows.

### Audit Readiness Diagnostics Hide Required Terms

`run-audit.sh` reported several readiness files as "incomplete" without naming the missing required phrases. The actual phrase requirements are encoded in the script, for example:

- `governance-risk-levels.md`: `small`, `medium`, `broad`, `required evidence`, `broad validation`
- `aggregate-hang-diagnostics.md`: `verdict`, `stage`, `elapsed duration`, `last observed command`, `focused rerun`, `non-authoritative aggregate`
- `runtime-limitations.md`: `.NET 10 desktop`, `Vulkan`, `SkiaSharp preview`, `unsupported macOS/mobile/browser`, `no software-renderer fallback`

Recommendation: when a readiness contract file is incomplete, print the missing terms. The current failure is actionable only after reading `run-audit.sh`.

### Generated Readiness Contract Is Not Discoverable Enough

The missing readiness files were not obvious from the task list alone:

- `governance-risk-levels.md`
- `aggregate-hang-diagnostics.md`
- `runtime-limitations.md`

Recommendation: task generation should include explicit tasks for every readiness file enforced by the audit script, or the audit contract should be rendered into the feature readiness directory before implementation starts.

## Framework/API Friction

### Name Collisions Around Viewer Types

Opening `FS.Skia.UI.SkiaViewer` introduced a collision with the app message case `CloseRequested`. Calls such as:

```fsharp
Program.update CloseRequested model
```

resolved toward a SkiaViewer type until qualified as:

```fsharp
Program.update LunarDemo1.Model.CloseRequested model
```

Recommendation: generated examples should show explicit qualification for app `Msg` cases that may collide with viewer lifecycle names, especially `CloseRequested`.

### Scene Point and App Vector Types Need Explicit Conversion

The app model naturally introduced:

```fsharp
type Vector2 = { X: float; Y: float }
```

Scene primitives require `FS.Skia.UI.Scene.Point`, so rendering needed explicit conversion:

```fsharp
let toPoint (vector: Vector2) : Point =
    { X = vector.X; Y = vector.Y }
```

This is fine, but the pattern is easy to miss because the record fields are structurally similar.

Recommendation: generated game samples should include a small `toPoint` conversion helper or guidance for domain vector types versus scene point types.

### Scene Evidence Proves Rendering, Not Semantic Contents

`SceneEvidence.render` provides useful deterministic metadata and hashes, but it does not directly prove semantic scene contents such as "lander", "terrain", "landing pad", or "HUD metrics." The implementation had to prove those through tests over scene nodes and custom evidence fields.

Recommendation: consider a lightweight semantic annotation/tag mechanism in scene nodes or evidence metadata. Generated apps could then report stable semantic facts without relying on source checks or fragile text/node counting.

### Screenshot/Fallback Wording Needs Strong Defaults

The screenshot path worked in this environment, but the generated evidence surface must remain careful:

- live screenshot proof is real only when `proves-screenshot=True`;
- deterministic scene metadata is useful but not screenshot proof;
- pixel-readback fallback should not be described as desktop visibility.

Recommendation: template evidence wording should make these distinctions default and hard to accidentally weaken. In particular, fallback reports should include explicit fields like `fallback-reason`, `deterministic-fallback-kind`, and `proves-screenshot=false`.

## Suggested Improvements

1. Make generated FAKE `EvidenceGraph` and `EvidenceAudit` targets invoke the real Spec Kit extension scripts.
2. Add a generated helper for skill-loading evidence rows, derived from `tasks.deps.yml`.
3. Improve audit diagnostics by printing missing readiness-contract terms.
4. Generate readiness-contract placeholder files with the required sections and terms.
5. Add framework guidance for app `Vector2` to `Scene.Point` conversion.
6. Add examples that qualify app `Msg` cases when SkiaViewer namespaces are open.
7. Consider semantic scene annotations for evidence-friendly generated games.
8. Keep screenshot, pixel-readback, and deterministic scene evidence vocabulary strict in templates.

## Bottom Line

The FS.Skia.UI runtime and rendering path handled the lunar lander demo well. Most friction came from generated project governance and evidence workflow, not from the core rendering/viewer APIs. The highest-value improvement is to make the generated validation commands authoritative and to automate the evidence bookkeeping that the audit already expects.
