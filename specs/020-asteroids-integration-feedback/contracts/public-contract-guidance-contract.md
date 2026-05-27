# Contract: Public Scene, Host, And Update Guidance

Generated app guidance must make the intended public names unambiguous for
consumers writing signatures, tests, or documentation.

## Required Names

- Scene-returning function: `Product.Program.view`.
- Generated host value: `Product.Program.generatedHost`.
- App reducer: `Product.Program.update`.

## Guidance Rules

- Use qualified app-owned names in docs and tests when framework namespaces are
  opened.
- Do not imply that similarly named framework helpers are substitutes for the
  generated app reducer or host.
- Generated signatures should expose the scene-returning function as returning
  `FS.Skia.UI.Scene.Scene`.
- Generated host guidance should name the intended host value used by
  `Viewer.runApp` or the current public viewer launch API.

## Validation

`GeneratedGuidanceCheck` must fail when docs or generated examples:

- Omit the intended scene, host, or update names.
- Use inconsistent names across public docs and generated examples.
- Show ambiguous unqualified update calls in examples that also open framework
  namespaces with common reducer names.
