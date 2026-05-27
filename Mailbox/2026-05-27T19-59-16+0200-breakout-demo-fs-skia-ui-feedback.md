# Breakout Demo Implementation Feedback For FS.Skia.UI

Date: 2026-05-27 19:59:16 +0200
Source project: `/home/developer/projects/BreakoutDemo1`
Feature: `specs/001-breakout-demo`

## Summary

While replacing the generated BreakoutDemo1 sample with a playable Breakout-style app, the core FS.Skia.UI pieces were usable: pure app state could be modeled in F#, keyboard events could be normalized, dependency-light Scene rendering worked for deterministic visual evidence, and the persistent viewer path launched successfully.

The implementation also exposed several places where FS.Skia.UI could be easier or more complete for generated game-style apps:

- Scene lacks an obvious first-class circle or ellipse primitive for balls and similar simple game entities.
- Screenshot evidence is not available through the packaged viewer surface in a way the generated app can honestly claim as real screenshot proof.
- The separation between Elmish adapter commands and viewer render effects is correct, but easy to misuse without sharper generated-app examples.
- Generated guidance appears to mention `Viewer.runAppWithWindowBehavior`, while the packaged surface available to the generated app uses `Viewer.runApp`.
- Layout evidence works, but F# record inference gets noisy when app-owned geometry types duplicate Scene field names.

## 1. Add First-Class Circle Or Ellipse Scene Primitives

### Observation

Breakout naturally needs a circular ball. During implementation, I initially tried to use a `Circle(...)` scene node. The package surface available to the generated app did not expose that constructor, so the ball had to be rendered as a small rectangle instead.

### Impact

For simple games and demos, circles and ellipses are common primitives:

- Breakout balls
- Asteroids bullets or ship markers
- Pong balls
- Selection handles
- Radial indicators
- Data point markers

Using small rectangles is functional but visually weaker and makes generated examples feel less polished. It also forces each consumer to invent workarounds when a primitive shape would be clearer.

### Recommendation

Add public Scene primitives such as:

```fsharp
Circle of center: Point * radius: float * fill: Color
Ellipse of bounds: Rect * fill: Color
PaintedCircle of center: Point * radius: float * paint: Paint
PaintedEllipse of bounds: Rect * paint: Paint
```

If the internal renderer already supports these shapes, expose them through the Scene API and add package surface tests plus deterministic `SceneEvidence.render` coverage. If not, a path-based or rounded-rectangle approximation could be added behind the primitive while preserving the public model.

### Evidence From BreakoutDemo1

The Breakout ball is currently rendered through `rectNode (ballBounds ball) ballColor`. The implementation would be simpler and more accurate with a public circle primitive.

## 2. Expose A Clear Public Screenshot Evidence API

### Observation

The feature contract required visual evidence. Deterministic pixel readback was available through `SceneEvidence.render`, but screenshot capture through the live viewer host was not exposed enough to make a real screenshot claim. The generated app therefore reports:

```text
status=unsupported
command=--screenshot-evidence
unsupported-host-reason=screenshot capture is not exposed by the current packaged viewer host
```

Pixel readback does succeed and proves composition of paddle, ball, bricks, walls, and HUD, but it is not the same as a live screenshot.

### Impact

Generated apps that need evidence have to choose between:

- claiming only deterministic render proof, or
- launching a persistent viewer without a bounded, inspectable screenshot artifact.

That weakens validation for visual desktop behavior, especially when a spec distinguishes "scene is renderable" from "window is visible and screenshot-relevant content is present."

### Recommendation

Expose a bounded screenshot capture API in `FS.Skia.UI.SkiaViewer`, for example:

```fsharp
Viewer.captureFirstFrame :
    ViewerRunRequest -> ViewerOptions -> GeneratedHost<'model,'msg> -> Result<ViewerScreenshotEvidence, ViewerRunFailure>
```

The evidence should include:

- status
- output path
- image dimensions
- renderer mode
- frames rendered
- whether input dispatch was required or observed
- native/window diagnostics where available
- unsupported-host reason when capture cannot be performed

The important contract point is that unsupported screenshot capture should be explicit and machine-readable, not replaced by text-only metadata that claims screenshot proof.

### Evidence From BreakoutDemo1

The generated app implements both:

- `--screenshot-evidence`, which honestly reports unsupported screenshot capture
- `--pixel-readback-evidence`, which uses deterministic Scene rendering and succeeds

This is good for honesty, but a first-class viewer screenshot path would make the evidence stronger.

## 3. Clarify Elmish Adapter Commands Versus Viewer Render Effects

### Observation

The Breakout app uses a pure `update : Msg -> Model -> Model * AdapterCommand<Msg>`. Close requests emit an adapter host command from the pure boundary, while viewer rendering is produced at the host edge.

During implementation, it was easy to attempt this shape:

```fsharp
next, effects @ [ RenderScene(view next) ]
```

That is incorrect because `RenderScene` belongs to the viewer host effect surface, not `AdapterCommand<Msg>`.

### Impact

This boundary is architecturally correct, but generated app authors can confuse:

- app reducer effects
- control/Elmish adapter commands
- viewer render effects
- interpreter/host-side effects

The type system catches the mistake, but the error is not self-explanatory unless the developer already understands the layering.

### Recommendation

Improve generated-app guidance and examples to show the intended pattern:

```fsharp
let update msg model : Model * AdapterCommand<Msg> =
    // Pure app transition. No rendering or I/O here.
    nextModel, appCommands

let generatedHost =
    { Update =
        fun msg model ->
            let next, _ = update msg model
            next, [ RenderScene(view next) ] }
```

Also consider naming or documentation changes that distinguish:

- `AdapterCommand<'msg>`: app/request effects
- viewer host effects: render/window effects

If both concepts are public and expected in generated apps, a short "where effects live" contract in the generated template would prevent misuse.

### Evidence From BreakoutDemo1

The final implementation keeps `update` pure and emits viewer render effects only inside `generatedHost.Update`.

## 4. Align Generated Guidance With Packaged Viewer Surface

### Observation

Existing generated tests/docs in BreakoutDemo1 referenced `Viewer.runAppWithWindowBehavior`. The package surface available to the generated project uses `Viewer.runApp`. Previous generated source also included string constants and comments around this mismatch.

### Impact

This creates uncertainty about the current supported persistent launch contract:

- Is `runAppWithWindowBehavior` planned but not packaged?
- Is it available in source but missing from the package version?
- Should generated projects use `runApp` until a newer package is installed?

For generated consumer projects, this matters because tests often assert source contracts and evidence wording. A drift between template guidance and package surface can force consumers to keep placeholder strings or synthetic comments just to satisfy governance checks.

### Recommendation

Pick one current generated-app contract and align all of these with it:

- template source
- generated tests
- quickstart/guidance docs
- package surface
- readiness/governance checks

If `runAppWithWindowBehavior` is the intended public API, publish a package version that includes it and update generated apps to call it for real. If `runApp` is the current supported API, remove generated guidance that requires `runAppWithWindowBehavior` until the package supports it.

### Evidence From BreakoutDemo1

The final Breakout app uses:

```fsharp
Viewer.runApp viewerOptions generatedHost
```

The persistent launch worked locally for the user.

## 5. Recommend Reusing Scene Geometry Types For Layout Evidence

### Observation

The implementation initially defined app-owned `Bounds` and `Vector` records with common field names such as `X`, `Y`, `Width`, and `Height`. F# inference then collided with Scene types like `Rect`, `Size`, and other records with overlapping field labels.

The fix was to alias app bounds directly to the Scene rectangle type:

```fsharp
type Bounds = Rect
```

### Impact

The package design is workable, but generated apps can hit confusing inference errors when they define parallel geometry types. This is especially likely in games, where local `Bounds`, `Vector`, `Point`, and `Size` records are natural names.

### Recommendation

In generated game/app guidance, recommend using the Scene package geometry types for layout and evidence:

```fsharp
type Bounds = FS.Skia.UI.Scene.Rect
```

or directly use `Rect` in public evidence helpers. This keeps geometry consistent across:

- scene rendering
- layout evidence
- collision bounds
- containment checks

If desired, add helper constructors for clarity:

```fsharp
Rect.create : x: float -> y: float -> width: float -> height: float -> Rect
Point.create : x: float -> y: float -> Point
Size.create : width: int -> height: int -> Size
```

Those helpers would reduce record-label inference ambiguity and improve readability.

### Evidence From BreakoutDemo1

The final implementation uses `type Bounds = Rect` and shares that representation across collision, layout evidence, and Scene rectangle rendering.

## 6. Generated Evidence Commands Benefit From Standard Helpers

### Observation

The Breakout app needed multiple CLI evidence commands:

- `--breakout-evidence`
- `--layout-evidence`
- `--screenshot-evidence`
- `--pixel-readback-evidence`

Each command needed to write key-value report files, create directories, print the same lines to stdout, and return correct exit codes.

### Impact

Every generated app currently risks hand-rolling slightly different evidence report behavior. That can make governance checks and downstream automation brittle.

### Recommendation

Consider a small generated-product evidence helper, either in the template or a package, for key-value evidence reports:

```fsharp
EvidenceReport.write :
    path: string ->
    fields: (string * string) list ->
    unit
```

or:

```fsharp
EvidenceReport.writeAndPrint :
    path: string ->
    fields: (string * string) list ->
    int
```

Useful built-in conventions:

- create parent directory
- write stable line ordering
- print the same lines to stdout
- normalize booleans and enums
- include `status`, `command`, and `unsupported-host-reason` consistently

### Evidence From BreakoutDemo1

BreakoutDemo1 now has app-local helpers such as `writeLines`, `evidenceLines`, and command-specific report builders. These are small, but the pattern will recur in future generated apps.

## Suggested Priority

1. Align generated viewer guidance with the packaged API (`runApp` vs `runAppWithWindowBehavior`).
2. Add first-class circle/ellipse Scene primitives.
3. Expose a bounded screenshot capture API or formally document screenshot evidence as unsupported with a standard fallback path.
4. Add generated-app docs clarifying Elmish adapter commands versus viewer render effects.
5. Recommend Scene geometry types or provide constructors to reduce F# record inference ambiguity.
6. Standardize key-value evidence report writing for generated apps.

## Closing Notes

The local Breakout implementation validates that the current package set can support a playable, deterministic generated game with real tests and evidence. The improvement areas above are mostly about making that path cleaner, more visually expressive, and less dependent on app-local conventions.
