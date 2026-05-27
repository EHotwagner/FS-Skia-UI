# Contract: Scene Shape Primitives

Scene must expose first-class filled circle and filled ellipse concepts for
generated game, chart, and interaction-marker use.

## Required Contract

- Public `.fsi` signatures include filled circle and filled ellipse creation.
- Circle evidence includes center, radius or derived bounds, fill, and placement.
- Ellipse evidence includes bounds, fill, and placement.
- Deterministic render evidence recognizes circle and ellipse output without
  requiring live desktop screenshot capture.
- Generated examples use these primitives for at least three representative
  circular or elliptical entities.

## Acceptance

- Semantic tests compile through the packed public surface.
- Deterministic evidence verifies bounds, fill, and relative placement in under
  5 seconds for a standard generated scene.
- Rectangle substitutions are no longer required for common circular game
  entities.
