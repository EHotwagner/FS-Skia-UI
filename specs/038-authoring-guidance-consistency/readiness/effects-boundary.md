# Canonical Effects-Boundary Page (US5, FR-009, SC-005)

One canonical page, `template/base/docs/effects-boundary.md`, is bundled into
every generated project at `docs/effects-boundary.md`. It is self-contained — an
author follows it without reading `docs/reports/*` or framework source.

## Required contents (asserted by GeneratedProductCheck)

`scanV3GeneratedRow` requires `docs/effects-boundary.md` to be present and to
contain all of: `application commands`, `viewer effects`, `host boundary`,
`MVU edge`, `Viewer.runApp`, `generatedHost`.

The page names both effect categories:

1. **Application commands at the MVU edge** — the pure reducer
   `Product.Program.update : Msg -> Model -> Model * ViewerEffect list` returns
   requested effects as plain values; it performs no I/O.
2. **Viewer effects at the host boundary** — the `Viewer.runApp` interpreter
   executes the requested `ViewerEffect` values (`OpenWindow`,
   `ApplyWindowOptions`, `RenderScene`, `CaptureScreenshot`, `EmitDiagnostic`, …)
   against the real desktop host.

…the boundary between them, and the canonical `update`→host wiring:
`Viewer.runApp viewerOptions generatedHost`, with `generatedHost`'s
`Init`/`Update`/`View`/`MapKey`/`Tick`/`Diagnostics` callbacks (matching the real
`GeneratedAppHost` surface).

## Single source of truth

`docs/reports/generated-apps.md` was repointed to this page (a blockquote at the
top names the bundled `docs/effects-boundary.md` as the canonical source and says
to follow it from a generated project).

## Reachability (SC-005)

Generated and present at
`artifacts/generated-products/038-authoring-guidance-consistency/app-source/docs/effects-boundary.md`,
and the wiring it documents matches how the generated `Product.Program` actually
wires effects (`Viewer.runApp viewerOptions generatedHost`). Validated green by
`./fake.sh build -t GeneratedProductCheck` — see `logs/generated-product-check.txt`.
