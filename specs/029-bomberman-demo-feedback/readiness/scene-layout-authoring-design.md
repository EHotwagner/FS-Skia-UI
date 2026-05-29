# Scene Layout Authoring Design

Task: T010
Captured: 2026-05-29T11:48:32+02:00

## Ambiguity Categories

- Coordinates: distinguish `Scene.Point`, layout coordinates, text positions, vertex positions, and window positions.
- Dimensions: distinguish `Scene.Size`, `Layout.LayoutSize`, `LayoutBounds`, and screenshot image dimensions.
- Diagnostics: distinguish viewer diagnostics, layout diagnostics, evidence diagnostics, and validation diagnostics.
- State: distinguish app `Model`, viewer lifecycle state, layout workflow model, and evidence workflow state.
- Positions: distinguish text positions, vertex positions, window startup positions, and layout bounds positions.

## Accepted Patterns

- Use module qualification in nearby examples when record fields overlap.
- Use type annotations on record literals when both Scene and Layout modules are open.
- Prefer helper constructors only if they remove repeated ambiguity without hiding package boundaries.
- Keep Scene/Layout examples pure and dependency-light.
