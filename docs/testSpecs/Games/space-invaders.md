---
title: Space Invaders Demo Spec
category: Game specs
categoryindex: 9
---

# Space Invaders Demo Spec

## Goal

Build a Space Invaders-style shooter demo that exercises formation movement, enemy waves, projectile collisions, shields, scoring, keyboard input, and clear visual evidence.

## Player Experience

The game opens to a cannon at the bottom, a grid of descending enemies, defensive shields, and a HUD. The player moves horizontally, fires upward, avoids enemy shots, protects shields where possible, clears waves, and loses if enemies reach the bottom or lives run out.

## Core Rules

- Player cannon moves left and right along the bottom.
- Player can have only one active shot at a time, or a small fixed limit such as 2 shots.
- Enemy formation moves horizontally as a group.
- When the formation reaches a side boundary, it steps downward and reverses direction.
- Enemies fire downward periodically.
- Player shots destroy enemies on contact.
- Enemy shots damage shields or the player.
- Shields are destructible cell blocks.
- The wave is clear when all enemies are destroyed.
- The game ends when lives reach zero or enemies reach the player zone.

## Controls

- `Left` or `A`: move player left.
- `Right` or `D`: move player right.
- `Space`: fire.
- `P`: pause or resume.
- `R`: restart after game over.
- `Esc`: request close when hosted interactively.

## Enemies

- Use at least 5 columns by 4 rows of enemies.
- Enemy rows may have different score values and colors/shapes.
- Formation speed increases as fewer enemies remain.
- Later waves can add rows, increase fire rate, or increase movement speed.
- Optional mystery ship may cross the top occasionally for bonus points.

## Scoring

- Bottom enemy row: 10 points.
- Middle enemy rows: 20 points.
- Top enemy row: 40 points.
- Mystery ship: 100 points if included.
- Wave clear bonus: 300 points times wave number.

## Visual Requirements

- Render a dark playfield with clear enemy formation, player cannon, projectiles, shields, and HUD.
- Use simple pixel-art-like shapes or blocky icons for enemies.
- Shields must visibly degrade as they take hits.
- Show score, lives, wave, and paused/game-over state.
- Projectiles must be visually distinct from enemies and shield blocks.

## Game State

Track at minimum:

- Player position, lives, cooldown, and active state.
- Enemy grid/list with position, row type, alive flag, and formation metadata.
- Player projectiles and enemy projectiles.
- Shield block grids and health.
- Score, wave, formation direction, formation step timer, enemy fire timer, paused flag, game-over flag, elapsed time, and random seed.

## Determinism and Evidence

- Accept an optional seed for enemy firing and mystery ship timing.
- Evidence mode should inject movement and fire input, run enough frames to show formation movement, and complete with a bounded close reason.
- Evidence outcome should include frame count, input count, enemies remaining, shield blocks remaining, projectile counts, score, lives, wave, and close reason.
- Screenshot evidence should show the player, enemies, shields, at least one projectile or clear HUD state, and the full playfield.

## Acceptance Criteria

- Player movement is continuous and clamped to the playfield.
- Firing creates a projectile subject to the shot limit/cooldown.
- Player projectiles destroy enemies and update score.
- Enemy formation reverses and descends at side boundaries.
- Enemy shots damage shields and player.
- Shields degrade visibly.
- Clearing all enemies advances the wave.
- Enemy reach or zero lives causes game over.
- Interactive mode stays open until explicitly closed.

## Out of Scope

- Exact original arcade timings.
- Full sprite animation sheets.
- Networked scores.
- Required audio.
