# Space Invaders Demo Persistent Launch And Evidence Report

Date: 2026-05-27 15:57:22 Europe/Vienna

Generated app repo: `/home/developer/projects/SpaceInvadersDemo1`

Framework repo mailbox: `/home/developer/projects/FS-Skia-UI/Mailbox`

## Summary

While implementing the generated `SpaceInvadersDemo1` Space Invaders feature,
the app itself reached a mostly functional state: pure gameplay reducer,
keyboard mapping, scene rendering, deterministic evidence CLI, layout evidence,
and the generated `Test`/`Verify` targets all passed.

The remaining merge gate failure was not gameplay correctness. It was evidence:
`speckit.evidence.audit` requires a successful supported-host persistent-launch
artifact, but the generated app/tooling did not produce an accepted
machine-readable proof of a visible persistent GUI window.

I initially summarized this incorrectly as "the current environment only
supports headless." That statement was too strong and not supported by the later
facts. The user confirmed that the persistent app window did appear. The correct
finding is:

> The local environment can show the persistent app, but the current automated
> evidence path failed to observe and record that success in the format required
> by EvidenceAudit.

## Current Status In The Generated App

Implemented in `SpaceInvadersDemo1`:

- App-owned deterministic Space Invaders game model and pure reducer.
- `GameSession`, `Model`, `Msg`, close reasons, enemies, shields, projectiles,
  and evidence outcome data.
- Keyboard mapping for left/right/fire/pause/restart/close.
- FS.Skia.UI.Scene rendering for dark playfield, cannon, enemies, shields,
  projectiles, score, lives, wave, paused, and game-over state.
- CLI evidence commands:
  - `--space-invaders-evidence`
  - `--layout-evidence`
- Expecto tests covering foundation plus US1-US4 gameplay and evidence.

Passed commands:

```bash
./fake.sh build -t Test
./fake.sh build -t Verify
dotnet run --project src/SpaceInvadersDemo1/SpaceInvadersDemo1.fsproj -- --space-invaders-evidence specs/001-space-invaders-demo/readiness/gameplay-evidence.txt --seed 12345 --frames 600
dotnet run --project src/SpaceInvadersDemo1/SpaceInvadersDemo1.fsproj -- --layout-evidence specs/001-space-invaders-demo/readiness/layout-evidence.txt 1280 720
```

EvidenceGraph result:

```text
status: 34[X], 1[ ], 1[F]
synthetic tasks: 0
auto-synthetic tasks: 0
diff-scan blocking hits: 0
```

Remaining failed task:

- `T035`: EvidenceAudit failed because supported persistent launch evidence was
  missing.

Remaining pending task:

- `T036`: readiness notes cannot honestly complete while `T035` is failed.

## Persistent Launch Evidence Problem

### What EvidenceAudit Requires

The audit script scans readiness files and expects a supported-host persistent
launch artifact, preferably named like:

```text
specs/001-space-invaders-demo/readiness/supported-host-persistent-launch.txt
```

Required fields include:

```text
status
mode
command
window-opened
input-dispatch
exit-path
blocked-stage
classification
category
message
```

For a passing supported artifact, the audit expects values equivalent to:

```text
status=ok
mode=interactive-window
window-opened=true
input-dispatch=true|verified|false|not-verified
exit-path=true
```

### Initial Audit Failure

EvidenceAudit output:

```text
[BLOCK] .../readiness/supported-host-persistent-launch.txt
(missing supported-host persistent launch evidence)
```

After adding readiness contract documents, the persistent-launch evidence was
the only remaining blocking audit hit.

### Default Persistent Launch Was Not Tried Initially

Before making the first "headless" claim, I had run bounded evidence,
layout evidence, `--window-diagnostics`, and EvidenceAudit. I had not run the
plain default persistent GUI command.

That was a process error. The default launch should have been attempted before
classifying the environment.

### Later Default Launch Attempt

Command:

```bash
timeout 12s dotnet run --project src/SpaceInvadersDemo1/SpaceInvadersDemo1.fsproj
```

Captured output:

```text
Gtk-Message: Failed to load module "colorreload-gtk-module"
Gtk-Message: Failed to load module "window-decorations-gtk-module"

process-exit-code=124
```

Interpretation:

- Exit code `124` came from `timeout`.
- The process stayed alive until killed.
- That is consistent with a persistent app window being open.
- It does not itself prove visibility, renderability, or input dispatch in the
  machine-readable format required by the audit.

The user then confirmed that the window worked visually.

### Automated Window Observation Failed

A later attempt launched the app in the background and tried to observe it with
`wmctrl` and `xdotool`:

```bash
dotnet run --project src/SpaceInvadersDemo1/SpaceInvadersDemo1.fsproj &
wmctrl -l
xdotool search --name "Space Invaders Demo"
```

Generated artifact:

```text
status=failed
mode=interactive-window
command=dotnet run --project src/SpaceInvadersDemo1/SpaceInvadersDemo1.fsproj
window-opened=false
window-visible=observed:false
input-dispatch=not-verified
exit-path=true
blocked-stage=window-observation
classification=window-visibility
category=runtime
message=persistent process launched but no matching window observed
process-exit-code=143
window-ids=
window-list-match=
```

This conflicts with the user's visual observation. The likely causes are:

- The viewer window title/class exposed to the compositor is not
  `Space Invaders Demo`.
- The app is visible but not exposed through the X11 tools used by the
  validation script, possibly due to Wayland/XWayland/compositor behavior.
- The window was not visible long enough under that particular backgrounded
  validation attempt.
- The audit requires post-return fields, but a truly persistent app does not
  naturally return until the user closes it.

## Why The "Headless Only" Claim Was Wrong

Environment variables showed a real desktop/session context:

```text
DISPLAY=:1
WAYLAND_DISPLAY=wayland-0
XDG_RUNTIME_DIR=/run/user/1000
DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/1000/bus
```

The app's diagnostic command reported:

```text
Desktop session prerequisites are present.
```

But it also reported:

```text
visible=observed:false
client-size=0x0
renderable-surface=observed:false
input-devices=unavailable
```

Those diagnostics are useful, but they are not equivalent to a failed default
persistent launch. The later user observation proved the persistent path can
work locally. Therefore the accurate conclusion is evidence-capture failure,
not environment incapability.

## Framework/Tooling Problems Exposed

### 1. Persistent Viewer Success Is Hard To Prove Automatically

The persistent app is designed to keep running until user close. That is correct
for interactive gameplay, but it makes readiness automation difficult:

- If the app stays open, `Viewer.runApp` does not return promptly.
- If the process is killed by `timeout`, the app cannot print a clean
  `status=ok ... exit-path=true` line.
- External observation through `wmctrl`/`xdotool` may miss the window even when
  a human can see it.

Potential improvement:

- Add a persistent-launch evidence mode to FS.Skia.UI.SkiaViewer that:
  - opens the real persistent viewer,
  - waits until first frame is presented,
  - records native/window facts,
  - optionally verifies input dispatch,
  - writes the audit-required fields,
  - then closes via a controlled test/evidence path.

This should be distinct from the normal default launch, so normal interactive
play remains persistent.

### 2. Window Title/Class Observation Is Not Reliable Enough

The generated app sets:

```fsharp
Title = "Space Invaders Demo"
```

Yet `wmctrl`/`xdotool` did not find a matching title while the user reported the
app worked.

Potential improvement:

- Expose viewer-native window handle, title, visibility, client-size, and
  renderable-surface facts through the viewer's own diagnostics rather than
  relying on shell tools.
- Include stable app/window identity in diagnostics.
- If cross-backend title visibility is inconsistent, document that and avoid
  making audit depend on external title search.

### 3. EvidenceAudit's Persistent Launch Gate Is Too Rigid For Manual Success

The audit only accepts a structured artifact. A human-visible working window is
not enough unless it is translated into the exact fields.

Potential improvement:

- Keep the strict gate, but provide a first-class generated command that writes
  the accepted artifact.
- Alternatively, add a documented manual-observation artifact format with
  explicit reviewer fields, if fully automated visibility is unavailable.

### 4. Diagnostic Command Can Be Misread As Launch Failure

`--window-diagnostics` reported window-visibility failure facts. Those facts
were then easy to overgeneralize into "the environment only supports headless."

Potential improvement:

- Separate:
  - desktop/session prerequisite diagnostics,
  - synthetic diagnostic examples,
  - actual attempted persistent launch observation.
- Ensure diagnostics output states whether it is describing a real launch
  attempt or generic/probe facts.

### 5. Benign GTK Module Warnings Add Noise

The default launch emitted:

```text
Failed to load module "colorreload-gtk-module"
Failed to load module "window-decorations-gtk-module"
```

The app still worked according to the user. These appear to be non-fatal host
environment warnings.

Potential improvement:

- Classify these warnings as benign when the viewer otherwise opens.
- Keep them in logs, but prevent them from being treated as unsupported-host
  evidence by themselves.

## App Implementation Problems Encountered

### 1. F# Record Inference Collisions

Several app records shared field names such as `Status`, `Score`, `Lives`,
`Seed`, `CloseReason`, and `InputCount`. F# inferred the wrong record type in
multiple update paths.

Observed symptoms included compile errors where `InputFlowDiagnostic` or
`EvidenceOutcome` was inferred instead of `GameSession`.

Fix applied:

- Added type annotations to reducer/helper functions.
- Renamed diagnostic `Status` to `StatusText`.
- Added conversion helpers between app `PlayRect` and FS.Skia.UI `Rect`.

Potential framework/generator improvement:

- Generated app examples should avoid repeating ambiguous record labels across
  many same-module records, or add explicit annotations in reducer helpers.

### 2. Package `update` Name Collision

Opening `FS.Skia.UI.KeyboardInput` in tests caused unqualified `update` calls to
resolve to the package input reducer instead of `SpaceInvadersDemo1.Program.update`.

Observed error types referenced:

```text
InputMsg
InputRuntime
```

Fix applied:

- Qualified app reducer calls as `SpaceInvadersDemo1.Program.update`.
- Qualified `init` similarly.

Potential framework/generator improvement:

- Generated tests should qualify app `Program.update` whenever capability
  namespaces are opened.
- Guidance should warn that common MVU names collide across capability modules.

### 3. Adapter Commands And Viewer Render Effects Are Different Types

The app `update` returns `AdapterCommand<Msg>`, but `generatedHost.Update` must
return viewer render effects. Returning adapter commands directly caused a type
mismatch.

Fix applied:

- `Program.update` remains pure and returns adapter commands.
- `generatedHost.Update` ignores app adapter commands and emits
  `RenderScene(view next)` for the viewer edge.

Potential framework/generator improvement:

- Generated templates should show this boundary explicitly for apps that combine
  Elmish-style reducer contracts with SkiaViewer host rendering.

### 4. Shield Tests Initially Asserted Exhaustion Instead Of Degradation

The shield model uses cells with `Health = 2`, so one hit degrades a shield cell
without removing the block. Initial tests checked block count and failed.

Fix applied:

- Tests now assert total shield health decreases.
- Scene still renders a different color for degraded cells.

Potential framework/generator improvement:

- Test guidance should distinguish "visible degradation" from "removed block."

### 5. Layout Evidence Can Prove Structure, Not Screenshot Proof

The generated `--layout-evidence` command records:

- full playfield coverage,
- HUD bounds,
- gameplay bounds,
- player/enemy/shield presence,
- overlap status,
- unsupported screenshot/pixel-readback diagnostics.

It does not claim screenshot proof.

This is honest, but EvidenceAudit still needs persistent GUI evidence for the
feature. The two evidence types should remain separate.

Potential improvement:

- Keep deterministic layout evidence as a structural proof.
- Add a separate viewer-owned screenshot/pixel-readback/persistent-window proof
  command that can satisfy visual/runtime gates.

## Spec Kit / Evidence Workflow Problems Encountered

### 1. Skill Loading Timestamps Are Strict

When `loaded_at` and `work_started_at` were equal, graph validation failed:

```text
skill fs-skia-project loaded after work started
```

The script requires `loaded_at < work_started_at`, not `<=`.

Potential improvement:

- Error text could say "loaded_at must be strictly before work_started_at."
- The skill evidence template should show timestamps with at least one-second
  separation.

### 2. EvidenceAudit Requires Readiness Contract Files Not Listed As Tasks

The audit required these files:

- `governance-risk-levels.md`
- `aggregate-hang-diagnostics.md`
- `runtime-limitations.md`

They were not explicit implementation tasks, so the first audit failed with
readiness contract hits.

Potential improvement:

- Task generation should include these readiness contract files when the audit
  gate requires them.
- Or EvidenceAudit should point back to the missing task-template obligation.

### 3. Marking EvidenceAudit As Failed Blocks Final Readiness

Once `T035` honestly became `[F]`, `T036` remained blocked by dependency. This
is correct behavior, but it leaves the feature in a state where app code is
working and tests are passing while merge readiness is blocked only by
persistent-launch evidence.

Potential improvement:

- Add a more granular persistent-launch task before EvidenceAudit so this
  failure is isolated and easier to remediate.

## Recommended Next Actions

1. Add a framework-supported persistent launch evidence command that opens the
   real viewer, records first-frame/window/input facts, writes the audit fields,
   and closes under controlled evidence mode.

2. Update generated app templates/tests to qualify `Program.update` and
   `Program.init` whenever capability namespaces are opened.

3. Make SkiaViewer diagnostics distinguish generic environment probes from real
   persistent launch attempts.

4. Add stable viewer-native window identity and visibility diagnostics instead
   of depending on `wmctrl`/`xdotool` title matching.

5. Treat common GTK module warnings as benign unless paired with a concrete
   launch/render failure.

6. Update Spec Kit task generation or audit documentation so required readiness
   contract files are visible before the final audit.

## Final Interpretation

The Space Invaders generated app is not blocked by gameplay implementation or
test failures. It is blocked by an evidence contract gap between a working
persistent GUI app and the audit's required machine-readable persistent-launch
artifact.

The local desktop environment should not be classified as headless-only based on
the current evidence. The correct classification is:

```text
visible persistent launch observed by user;
automated supported-host persistent launch artifact not yet captured;
EvidenceAudit remains failed until that artifact exists.
```
