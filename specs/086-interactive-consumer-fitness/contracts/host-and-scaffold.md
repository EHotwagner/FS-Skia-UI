# Contract: governed host, neutral scaffold, key warm-up (FR-001..FR-006, FR-015/016)

## Governed pointer host as controls-family default (FR-004/005/006, D6)

No `src/**` `.fsi` change — `ControlsElmish.runInteractiveApp` / `InteractiveAppHost`
(with `MapPointer`) already ship (`src/Controls.Elmish/ControlsElmish.fsi:45-53,140-141`).
The contract change is in the **template + governance**:

- `template/base/src/Product/Program.fs` — the **controls**-family default launch is
  `ControlsElmish.runInteractiveApp options host`; the **game**-family default stays
  `Viewer.runApp viewerOptions generatedHost`.
- `template/base/tests/Product.Tests/GovernanceTests.fs` (`:105`) and
  `BehaviorTests.fs` (`:289`) — generalize the literal
  `Expect.stringContains defaultBranch "Viewer.runApp viewerOptions generatedHost"` to
  assert **the per-family persistent interactive host call** (`runInteractiveApp` for
  controls, `Viewer.runApp ... generatedHost` for game).
- **Family marker**: a template generation profile value (`controls` vs `game`) on the
  existing `//#if (profile == ...)` machinery.

### Laws
- **SC-003**: a synthetic pointer press/release at a control's bounds in the controls
  default app dispatches that control's bound message (headless via
  `ControlsElmish.routeInteractivePointer`), and the governance suite passes with the
  pointer host as default.
- **SC-008**: the game family's governance assertion still passes with the
  keyboard-only host.

## Neutral controls-first scaffold (FR-001/002/003, SCAFFOLD-1/VIEW-1)

- `template/base/src/Product/Model.fs`, `View.fs` — neutral names (no
  `playfield`/`gameplayRegion`/`tally`/`stage`/`upcoming`/`ActiveColumn`/`ActiveRow`/
  `NextToken`/`Initial|Options|Main|Paused|Ended`/"circular entities"); default `view`
  renders `controlsExampleView` via `Control.renderTree`.
- `LayoutEvidence.fs`, `EvidenceCommands.fs`, `WindowOptions.fs`, `Program.fs` — keep
  every **durable governance token** (`--scene-evidence`, `SceneEvidence.render`,
  `RendererMode = "deterministic-scene"`, the visual-evidence honesty vocabulary, the
  `diagnostic-class=*` window facts) but re-frame "game/playfield" → "app/content
  region". Re-point `gameplayRegion*`/`GameplayRegion`/`GameplayBounds` region fields at
  the neutral content region (keeping the evidence token text the scans require).
- `BehaviorTests.fs` (replaceable) — rewritten for the neutral model (asserts real
  controls render, not "grid-style playfield" / "tally/stage/upcoming" / "circular
  entities"). `GovernanceTests.fs` (durable) keeps passing.

### Laws
- **SC-001**: a freshly generated project's product source (model/view/tests) contains
  **zero** game identifiers outside the durable governance tokens (grep), and the
  governance suite still passes.
- **SC-002**: the unmodified default app shows real styled controls (production render
  path), not placeholder rectangles or control-id text.

## Viewer keyboard warm-up (FR-015/016, D7)

- `src/SkiaViewer/SkiaViewer.fs` host input path — bounded pre-ready FIFO buffers key
  events until the pipeline signals ready, then flushes in order; bounded with a
  drop-oldest diagnostic past the cap. Possible additive `SkiaViewer.fsi` readiness
  diagnostic.
- `.agents/skills/fs-skia-viewer-host/SKILL.md` (regenerated to `.claude/`) — documents
  the warm-up window and the buffering mitigation.

### Laws
- **SC-007**: every keystroke issued within the first seconds after focus is delivered
  to `MapKey` (none dropped), and the behavior is documented in `fs-skia-viewer-host`.
