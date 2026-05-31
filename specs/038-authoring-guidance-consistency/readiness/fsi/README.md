# FSI consumer fixtures (US3, US6)

Both fixtures are compiled with `dotnet fsi <file>` against the freshly built
per-package DLLs under `src/*/bin/Debug/net10.0/`.

## US3 — name-collision hardening (FR-008, SC-003)

`us3-name-collision-consumer.fsx`: a consumer declares its own `AppState` with a
`Normal` case (plus its own `update`/`init`) and then `open`s the viewer
namespace — the realistic authoring order.

| State | Library | Result | Transcript |
|---|---|---|---|
| before | `ViewerWindowStartupState` **without** RQA | **FAIL** — `error FS0001: This expression was expected to have type 'AppState' but here has type 'ViewerWindowStartupState'` (the framework's bare `Normal` shadows the consumer's) | `us3-name-collision-before-FAIL.txt` |
| after | `ViewerWindowStartupState` **with** `[<RequireQualifiedAccess>]` | **PASS** — `us3 fixture compiled: Normal -> Busy` (consumer's `Normal` resolves) | `us3-name-collision-after-PASS.txt` |

`update`/`init` are plain consumer let-bindings and compile in both states,
proving the framework's `Viewer.update`/`ElmishAdapter.update`/`Keyboard.update`
are module-qualified and never shadowed (see `../collision-name-enumeration.md`).

## US6 — consistent scene constructors (FR-010, SC-006)

`us6-scene-constructors.fsx`: constructs `Rectangle`/`Text` via the existing
positional helpers and DU cases AND via the new self-describing
`Scene.filledRectangle` (Rect-based) / `Scene.textAt` (Point-based) forms.

| State | Scene surface | Result | Transcript |
|---|---|---|---|
| before | no self-describing forms | **FAIL** — `error FS0039: The value, constructor, namespace or type 'filledRectangle' is not defined.` | `us6-scene-constructors-before-FAIL.txt` |
| after | `filledRectangle` + `textAt` added (additive) | **PASS** — every positional and self-describing form compiles | `us6-scene-constructors-after-PASS.txt` |

No existing constructor was removed: `Scene.rectangle`, `Scene.text`, and the
positional `Rectangle`/`Text` DU cases all still compile in the after state.
