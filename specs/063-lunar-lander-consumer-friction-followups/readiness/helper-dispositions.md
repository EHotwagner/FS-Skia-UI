# Recurring-helper dispositions (FR-009/010, SC-006)

Each recurring `LunarLander1` helper candidate is dispositioned here — **ship** /
**document** / **document + defer** — with a rationale and a next-recurrence bar, so no
candidate is silently dropped.

| Helper candidate | Disposition | Rationale | Next-recurrence bar |
|---|---|---|---|
| `wrapDeltaX` — shortest wrap-aware delta on a toroidal axis | **SHIP** | Pure, ~4-line, float-only, **no `Scene`/`Layout` dependency**; recurred across Asteroids, Space Invaders, and Lunar Lander — past the 062 "ship on 3rd recurrence" bar. | Already shipped (`FS.Skia.UI.SkillSupport.Wrap.wrapDeltaX`). N/A. |
| Camera-centered projection (world → screen following the player) | **DOCUMENT** (defer ship) | A *closure* over per-game state (player position, view scale, screen center), returns a `Scene.Point` (a soft `Scene` dependency SkillSupport avoids), and varies per game (zoom-centered vs parallax vs fixed) — a `View` concern, not a dependency-light scalar. Documented in `fs-skia-layout-readability`. | Ship only if a **stable, game-agnostic** projection signature (no `Scene` dependency pulled into SkillSupport) recurs across ≥3 demos. |
| `--evidence-run` deterministic-summary | **DOCUMENT + DEFER** | The *discipline* (pure model + per-frame held-input script + `InvariantCulture`/`F3` + `determinism=byte-identical`) recurs, but the **field set varies materially per game**; the stable core (`status`/`command`/`seed`/`frame-count`/`score`/`determinism`) is too thin to justify a shipped record every consumer then appends 5–10 fields to. Documented in `fs-skia-evidence-mode`. | Ship a reusable summary type only if a **stable cross-game field set** (beyond the thin core) emerges. |

## Shipped helper evidence (`wrapDeltaX`)

- Contract `.fsi`: `src/SkillSupport/Wrap.fsi` (`module Wrap`, `val wrapDeltaX`).
- Implementation: `src/SkillSupport/Wrap.fs` (pure scalar arithmetic).
- FSI exercise (pre-impl, Principle I): `readiness/fsi-session.txt` (T006).
- Tests (Expecto, `tests/SkillSupport.Tests/Tests.fs` — "Wrap (FR-010 shortest wrap-aware
  delta)"): shortest-path examples (`100 90 10 = 20`, `100 10 90 = -20`), identity, range
  `(-w/2, w/2]`, symmetry-except-+w/2, determinism — **6/6 pass** (SC-006).
- Surface baseline: `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` adds the
  `Wrap` module; **`PerPackageSurfaceDiff` green** (zero drift).

Nothing is silently dropped: every candidate above has an explicit disposition and a
recorded bar for when to revisit.
