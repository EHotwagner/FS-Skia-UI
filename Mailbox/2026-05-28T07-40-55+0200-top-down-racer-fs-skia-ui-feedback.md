# Top-Down Racer FS.Skia.UI Feedback

Timestamp: 2026-05-28T07:40:55+02:00  
Consumer repo: `/home/developer/projects/TDRacerDemo1`  
Feature: `specs/001-top-down-racer`

## Summary

The top-down racer implementation did not hit a blocking FS.Skia.UI framework defect. The core paths used by the generated app worked:

- `Viewer.runApp` for the default persistent interactive host.
- `Viewer.runBounded` for first-frame launch evidence.
- `Viewer.runAppEvidence` for image evidence.
- `Viewer.captureScreenshotEvidence` for explicit screenshot proof or unsupported-host reporting.
- `SceneEvidence.render` for deterministic pixel-readback/hash evidence.
- Scene primitives for rectangles, lines, circles, grouped scenes, text, and resize-preserved world-to-screen mapping.
- Elmish-style generated host wiring through `Init`, `Update`, `View`, `MapKey`, and `Tick`.

The issues below are integration friction or host-environment limitations, not observed correctness failures in the framework.

## Observed Friction

### Scene Type Name Collisions

The consumer app initially defined domain records named `Rect` and `Vec2`. `FS.Skia.UI.Scene` also exposes scene layout types such as `Rect` and `Point`, and scene constructors like `Line` require `FS.Skia.UI.Scene.Point`.

Symptoms:

- F# inferred the app's domain `Rect` where a scene/layout `Rect` was required.
- Record literals for points were ambiguous or inferred as the wrong local type.
- `LayoutEvidence` signatures became noisy until types were annotated.

Resolution in the consumer app:

- Renamed the domain rectangle to `WorldRect`.
- Added a small scene-point helper with explicit `FS.Skia.UI.Scene.Point` return type.
- Added explicit annotations around layout evidence helpers.

Framework feedback:

- The framework API is usable as-is, but generated samples should avoid naming app-domain records `Rect`, `Point`, or `Size` when opening `FS.Skia.UI.Scene`.
- Template guidance could recommend domain-specific names such as `WorldRect`, `WorldPoint`, or `TrackBounds`.

### Screenshot Evidence Unsupported On This Host

`Viewer.captureScreenshotEvidence` returned an explicit unsupported result:

```text
status=unsupported
evidence-kind=screenshot
unsupported-host-reason=screenshot capture is unavailable for this viewer host
fallback=deterministic-scene-evidence
```

This is good behavior. It lets the consumer report that live screenshot proof is unavailable without falsely substituting deterministic render evidence as desktop visibility proof.

Framework feedback:

- Keep this distinction. The unsupported result shape was actionable and audit-friendly.
- If possible later, include one more field that states whether the window opened but capture failed, versus capture being unavailable before launch. That would help separate renderer/capture capability from desktop session capability.

### GTK Host Module Warnings During Interactive Launch

During attached interactive launch, GTK printed:

```text
Gtk-Message: Failed to load module "colorreload-gtk-module"
Gtk-Message: Failed to load module "window-decorations-gtk-module"
```

The app still reached first-frame evidence successfully. These appear to be host decoration/module warnings, not app or framework failures.

Framework feedback:

- Existing diagnostics already made the important distinction: launch evidence passed while decoration warnings were benign.
- If the viewer owns warning classification, these two GTK module messages are good candidates for benign-host-warning classification in generated readiness docs.

### Detached Launch Behavior

An initial `nohup dotnet run --project ... &` attempt exited immediately with no log output. A later `setsid dotnet run --project ... > ... 2>&1 < /dev/null &` stayed running.

Interpretation:

- This looks like process/session handling around GUI startup rather than a framework rendering bug.
- Attached terminal launch and bounded launch evidence both worked.

Framework feedback:

- Generated guidance for "run this GUI app in the background" should prefer a known working pattern such as `setsid ... < /dev/null` on Linux, or avoid promising that simple `nohup` will work.

## What Worked Well

- The generated host boundary was straightforward: pure app `update` emitted app commands, while `interpretAtHostBoundary` mapped model state to `RenderScene`.
- `MapKey` and `Tick` integration were simple enough for gameplay controls and deterministic evidence injection.
- Scene evidence was stable enough to support audit-friendly pixel/hash evidence.
- Screenshot unsupported reporting avoided a false-positive visual proof claim.
- The framework's first-frame evidence path gave a clean sanity check before attempting persistent interactive launch.

## Suggested Follow-Ups

1. Add generated sample guidance warning against app-domain `Rect`/`Point` names when `FS.Skia.UI.Scene` is opened.
2. Consider adding a screenshot-result capability detail that distinguishes "viewer could not open" from "viewer opened but screenshot capture is unavailable."
3. Classify the GTK module warnings above as benign host warnings if that warning classifier lives in FS.Skia.UI.
4. Document the Linux detached GUI launch pattern that worked in this repo:

```bash
setsid dotnet run --project src/TDRacerDemo1/TDRacerDemo1.fsproj > specs/001-top-down-racer/readiness/logs/game-run.txt 2>&1 < /dev/null &
```

## Evidence From Consumer Repo

- Focused tests passed: `dotnet test tests/TDRacerDemo1.Tests/TDRacerDemo1.Tests.fsproj -m:1`
- Full verify passed: `dotnet fsi build.fsx --target Verify`
- Evidence audit passed: `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/001-top-down-racer`
- Racer run evidence completed three laps through the interpreter boundary:

```text
status=ok
command=--racer-evidence
lap=3
total-laps=3
moved-from-start=True
interpreter-boundary=interpretAtHostBoundary
```
