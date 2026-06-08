---
title: Platformer Mini-Demo Spec
category: Game specs
categoryindex: 9
---

# Platformer Mini-Demo Spec

## Goal

Build a compact 2D platformer demo that exercises gravity, jumping, collision with tile platforms, moving platforms, collectibles, hazards, camera or fixed-level framing, and evidence-friendly game states.

## Player Experience

The game opens to a side-view level with a player character standing on solid ground. The player runs, jumps, lands on platforms, collects items, avoids hazards, reaches an exit, and can restart after winning or dying.

## Level

- Use a single short level with a clear start, middle challenge area, and exit.
- Include ground, at least three static platforms, at least one moving platform, collectibles, and one hazard.
- The level may fit on one screen or use a simple side-scrolling camera.
- If scrolling is implemented, keep the player visible with stable camera bounds.
- The exit should be visible or clearly reachable by moving right.

## Controls

- `Left` or `A`: move left.
- `Right` or `D`: move right.
- `Space`, `Up`, or `W`: jump.
- `P`: pause or resume.
- `R`: restart after win or death.
- `Esc`: request close when hosted interactively.

## Movement and Physics

- Player has position, velocity, grounded flag, facing direction, and alive/win state.
- Gravity accelerates the player downward.
- Horizontal movement accelerates or changes velocity while input is held.
- Jump is allowed only while grounded, with optional short coyote time.
- Variable jump height is allowed if jump release cuts upward velocity.
- Player collides with solid tiles and platforms.
- Moving platforms carry the player when standing on them.
- Falling below the level or touching a hazard causes death.

## Collectibles and Goal

- Place at least 5 collectibles.
- Collectibles disappear when touched and increase score.
- Reaching the exit wins the level.
- The player can win without collecting every item, but HUD should show collected count.

## Scoring

- Collectible: 10 points.
- Level clear: 100 points.
- Optional time bonus may be awarded on win.

## Visual Requirements

- Render background, solid tiles, moving platforms, hazards, collectibles, player, exit, and HUD.
- Player facing direction and grounded/jumping state should be visually readable.
- Hazards must be visually distinct from collectibles and platforms.
- HUD shows score, collectibles, state, and optionally elapsed time.
- Avoid text overlays that hide the player or exit in evidence screenshots.

## Game State

Track at minimum:

- Player position, velocity, grounded state, facing, coyote/jump timers, alive/win flags.
- Tile or platform collision map.
- Moving platform positions, paths, velocities, and timers.
- Collectible list with collected flags.
- Hazard zones, exit bounds, score, paused flag, elapsed time, and random seed if any.

## Determinism and Evidence

- Evidence mode should inject run and jump inputs, move the player across visible terrain, and exit explicitly.
- Evidence outcome should include frame count, input count, player position, velocity, grounded flag, collected count, score, state, and close reason.
- Screenshot evidence should show player, platforms, at least one collectible, a hazard or exit, and HUD.

## Acceptance Criteria

- Player runs left and right with collision against solid surfaces.
- Jumping works only from valid grounded/coyote states.
- Gravity and landing produce stable grounded state.
- Moving platform carries the player.
- Collectibles increment score and disappear.
- Hazards or falling cause death.
- Reaching the exit wins the level.
- Pause freezes physics, platform timers, and hazards.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Multiple levels.
- Combat.
- Complex animation system.
- Slopes unless already easy in the renderer.
- Required audio.
