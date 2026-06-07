---
title: Bomberman Lite Demo Spec
category: Game specs
categoryindex: 9
---

# Bomberman Lite Demo Spec

## Goal

Build a small Bomberman-style arena demo that exercises tile maps, grid movement, bombs, timed explosions, destructible blocks, simple enemies, collision, scoring, and evidence-friendly state transitions.

## Player Experience

The game opens to a top-down tile arena. The player moves one tile at a time or smoothly within the grid, places bombs, breaks soft blocks, avoids explosions and enemies, collects simple powerups, and clears the level by defeating enemies or reaching an exit.

## Arena

- Use an odd-sized grid, for example 15 columns by 13 rows.
- Outer walls are indestructible.
- Interior fixed walls appear in a regular pattern.
- Soft blocks fill some empty cells and can be destroyed.
- Player starts in a safe corner with at least two adjacent empty cells.
- Enemies start away from the player.
- Optional exit appears under a soft block or is visible from the start.

## Controls

- `Up` or `W`: move up.
- `Down` or `S`: move down.
- `Left` or `A`: move left.
- `Right` or `D`: move right.
- `Space`: place bomb.
- `P`: pause or resume.
- `R`: restart after win or game over.
- `Esc`: request close when hosted interactively.

## Core Rules

- Player cannot move through walls, soft blocks, bombs, or enemies.
- Bombs are placed on the player's current tile.
- Bombs explode after a fixed fuse, such as 2 seconds.
- Explosion extends in four cardinal directions up to the bomb range.
- Indestructible walls block explosions.
- Soft blocks are destroyed by explosions and stop the blast.
- Player dies when hit by an explosion or enemy.
- Enemies die when hit by an explosion.
- Level is won when all enemies are defeated, or when the player reaches an exit after defeating enemies.

## Powerups

Include at least one optional deterministic powerup type:

- Extra bomb capacity.
- Increased blast range.
- Temporary speed increase.

Powerups may appear when soft blocks are destroyed and must be seeded in evidence mode.

## Scoring

- Soft block destroyed: 10 points.
- Enemy defeated: 100 points.
- Level clear: 500 points.
- Powerup collected: 50 points.

## Visual Requirements

- Render a stable grid with clearly different wall, soft block, floor, bomb, explosion, player, enemy, powerup, and exit visuals.
- Explosion cells must be visible as cross-shaped blast segments.
- Show HUD with score, lives or health, bomb capacity, blast range, enemies remaining, and state.
- The full arena should be visible at common desktop sizes.

## Game State

Track at minimum:

- Tile map with wall, soft block, floor, exit, and powerup cells.
- Player grid/world position, alive flag, bomb capacity, active bomb count, blast range, and movement intent.
- Bomb list with tile, fuse timer, owner, and range.
- Explosion list with affected cells and remaining lifetime.
- Enemy list with position, movement direction, alive flag, and movement timer.
- Score, level, paused flag, win/game-over state, elapsed time, and random seed.

## Determinism and Evidence

- Accept an optional seed for soft block pattern, enemy choices, and powerup drops.
- Evidence mode should move the player, place a bomb, show fuse/explosion progression, and exit explicitly.
- Evidence outcome should include frame count, input count, bombs placed, soft blocks remaining, enemies remaining, score, player state, and close reason.
- Screenshot evidence should show the arena, player, at least one bomb or explosion if timing allows, blocks, enemies, and HUD.

## Acceptance Criteria

- Player movement obeys tile collision.
- Bomb placement respects capacity and occupied-tile rules.
- Bomb fuse triggers a cross-shaped explosion.
- Explosions destroy soft blocks and enemies but stop at blocking tiles.
- Player death and level win states work.
- Powerups, if present, modify player capabilities visibly or in HUD.
- Pause freezes timers, enemies, bombs, and explosions.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Multiplayer.
- Chain-reaction-heavy advanced rules beyond simple bomb triggering.
- Complex enemy AI.
- Required audio.
