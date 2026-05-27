# Asteroids Demo FS.Skia.UI Integration Analysis

Date: 2026-05-27 11:20:35 Europe/Vienna

Source implementation: `/home/developer/projects/AsteroidsDemo2`

## Summary

The AsteroidsDemo2 implementation replaced the generated board/control sample
with a playable Asteroids-style FS.Skia.UI app. The demo ran successfully in the
persistent viewer on the local host, but one user-visible issue remained: HUD
score/status text overlapped with other UI/gameplay content. The implementation
also exposed several FS.Skia.UI API-shape and evidence limitations that are worth
tracking from the framework side.

## Observed FS.Skia.UI-Related Friction

### 1. Scene Type Naming Was Easy To Misidentify

The app signature initially used a generic `Node` type name for scene values.
The actual package surface exposes `SceneNode`. This caused compile failures
when adding an explicit `.fsi` for the app-owned public surface.

Impact:

- App authors can guess the wrong scene type name when moving from inline sample
  code to an explicit signature file.
- The failure is straightforward once seen, but the discoverability is weak
  because package source/signature files are not visible from the consuming repo.

Potential improvement:

- Ensure generated samples or docs consistently name `SceneNode` anywhere a
  public scene-returning function is shown.
- Consider adding a tiny consumer-facing API reference snippet for common
  signatures such as `val view: Model -> SceneNode`.

### 2. Viewer Host Type Name Was Not Obvious

The app host shape used by this generated consumer is
`GeneratedAppHost<Model, Msg>`. I initially expected a `ViewerHost<Model, Msg>`
name, which caused a signature mismatch.

Impact:

- The runtime wiring worked once the right type was used.
- The naming makes it harder for generated app consumers to expose host values in
  `.fsi` files without inspecting compiled package metadata.

Potential improvement:

- Document the intended public host type for generated apps.
- If `GeneratedAppHost` is the stable consumer contract, make that explicit in
  generated guidance and examples.

### 3. KeyboardInput `update` Collided With App `update`

Opening `FS.Skia.UI.KeyboardInput` in the test module caused unqualified calls
to `update` to bind to the keyboard package update function instead of
`AsteroidsDemo2.Program.update`.

Impact:

- This produced confusing type errors involving `InputMsg` and `InputRuntime`
  while testing the app's own MVU update surface.
- The issue is avoidable by qualifying app update calls, but generated examples
  should be careful around common names.

Potential improvement:

- Prefer examples that qualify package reducer calls when package modules expose
  common MVU names like `update`.
- Consider guidance for generated apps: use `Program.update` or local aliases in
  tests when opening capability namespaces.

### 4. Scene Evidence Did Not Catch Layout Overlap

`SceneEvidence.render` produced metadata/hash evidence cleanly for screenshot and
pixel-readback commands. That was useful for proving that a scene graph can be
rendered deterministically, but it did not catch the HUD/text overlap seen during
interactive use.

Impact:

- Existing semantic tests asserted that HUD text existed, not that the HUD had a
  reserved layout region or non-overlapping bounds.
- Metadata/hash evidence is not enough to validate readable UI layout.
- The app can pass deterministic evidence while still having visible text overlap
  in the real viewer.

Potential improvement:

- Provide or document text measurement/bounds helpers suitable for tests.
- Add a scene inspection convention for layout-sensitive generated samples:
  expected HUD band, gameplay bounds, and text bounding boxes.
- Consider a lightweight visual-readback or layout assertion helper that reports
  approximate node bounds for `Text`, `Rectangle`, and line geometry.

### 5. Persistent Viewer Path Worked But Emitted GTK Module Warnings

The default interactive launch stayed alive as expected. Console output included
non-fatal GTK module warnings:

```text
Failed to load module "colorreload-gtk-module"
Failed to load module "window-decorations-gtk-module"
```

Impact:

- These warnings did not prevent the app from running.
- They can still create noise in readiness logs and may distract from real
  startup diagnostics.

Potential improvement:

- If these warnings are common and harmless, document them as non-fatal host
  environment noise.
- Keep unsupported-host diagnostics distinct from benign platform module
  warnings.

## Specific UI Overlap Finding

The AsteroidsDemo2 HUD currently uses fixed text positions in the same scene
coordinate space as gameplay. The HUD row starts near the top of the playfield,
and the active game entities can render into that same area. The score/lives/wave
text can visually collide with status text or gameplay elements depending on
window sizing and live state.

Recommended app-side fix:

- Reserve a dedicated HUD band at the top or a right-side panel.
- Offset the active gameplay region below or beside that band.
- Wrap ship, asteroid, and bullet coordinates inside the gameplay region rather
  than the full scene.
- Add semantic tests that assert HUD text y positions stay outside gameplay
  bounds.
- Add a small-window test/evidence mode that verifies HUD readability under the
  resize contract.

Framework-side opportunity:

- FS.Skia.UI could make this easier by exposing a simple layout/bounds utility
  for scene graph nodes, especially text.
- Generated game/sample templates should avoid drawing HUD and gameplay entities
  into the same coordinate region unless the overlap is explicitly intended.

## Evidence From The AsteroidsDemo2 Run

Commands completed during implementation:

```bash
./fake.sh build -t Test
./fake.sh build -t Verify
dotnet run --project src/AsteroidsDemo2/AsteroidsDemo2.fsproj -- --asteroids-evidence readiness/asteroids-game-evidence.txt --seed 12345
dotnet run --project src/AsteroidsDemo2/AsteroidsDemo2.fsproj -- --screenshot-evidence readiness/game-screenshot-evidence.txt
dotnet run --project src/AsteroidsDemo2/AsteroidsDemo2.fsproj -- --pixel-readback-evidence readiness/game-pixel-readback-evidence.txt
dotnet run --project src/AsteroidsDemo2/AsteroidsDemo2.fsproj
```

The user confirmed the demo works, with the remaining issue being overlapping
score/text UI.
