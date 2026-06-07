---
title: Tower Defense Prototype Spec
category: Game specs
categoryindex: 9
---

# Tower Defense Prototype Spec

## Goal

Build a small tower-defense prototype that exercises path rendering, wave spawning, enemy movement, tower placement, targeting, projectile updates, economy, health, and deterministic evidence.

## Player Experience

The game opens to a top-down map with a visible enemy path, buildable tiles, starting money, base health, wave information, and tower controls. The player places towers, starts or watches waves, earns money by defeating enemies, and survives as long as possible.

## Map

- Use a rectangular map with a fixed enemy path from spawn to base.
- Path cells are not buildable.
- Buildable cells are clearly marked or implied by open terrain.
- The path should have at least three turns so targeting is visually meaningful.
- The full map should be visible in the base demo.

## Controls

- Mouse click or keyboard-driven placement must be supported; if mouse is not available in the target runtime, keyboard placement is acceptable.
- `1`: select basic tower.
- `2`: select slow tower if implemented.
- `Enter` or mouse click: place selected tower on highlighted cell.
- Arrow keys or `WASD`: move placement cursor when using keyboard placement.
- `Space`: start next wave or toggle fast-forward if a wave is active.
- `P`: pause or resume.
- `R`: restart after defeat.
- `Esc`: request close when hosted interactively.

## Towers

Include at least one tower type:

- Basic tower: medium range, medium fire rate, direct damage projectile.

Optional second type:

- Slow tower: lower damage, applies temporary slow.

Tower placement rules:

- Towers cost money.
- Towers can be placed only on buildable empty cells.
- Towers target enemies in range.
- Targeting priority may be first-on-path by default.

## Enemies and Waves

- Enemies follow the path from spawn to base.
- Reaching the base reduces health and removes the enemy.
- Defeating an enemy awards money and score.
- Waves contain multiple enemies with spawn intervals.
- Later waves increase enemy count, health, or speed.
- The game is defeated when base health reaches zero.

## Economy and Scoring

- Starting money: enough to place at least two basic towers.
- Basic tower cost: 50.
- Enemy defeat reward: 10.
- Score: 10 points per defeated enemy plus wave clear bonus.
- Wave clear bonus: 25 times wave number.

## Visual Requirements

- Render map terrain, path, spawn, base, placement cursor, towers, enemies, projectiles, and HUD.
- Tower range should be visible while selecting or hovering a tower.
- Show money, base health, wave number, enemies remaining, selected tower, and state.
- Projectiles and enemies must be high contrast and inspectable in screenshots.

## Game State

Track at minimum:

- Map cells and path waypoints.
- Placement cursor, selected tower type, and placement validity.
- Tower list with position, range, cooldown, type, and target id.
- Enemy list with path progress, position, health, speed, status effects, and alive/reached-base state.
- Projectile list with position, target id or direction, damage, speed, and active state.
- Money, score, base health, wave number, wave spawn timer, paused flag, defeated flag, elapsed time, and random seed.

## Determinism and Evidence

- Accept an optional seed for any target tie-breaking or wave variants.
- Evidence mode should place at least one tower, spawn enemies, fire at least one projectile if timing allows, and exit explicitly.
- Evidence outcome should include frame count, input count, towers placed, enemies spawned, enemies alive, projectiles active, money, base health, wave, score, and close reason.
- Screenshot evidence should show path, tower, enemies, HUD, and preferably projectile or range indicator.

## Acceptance Criteria

- Placement cursor or mouse placement validates buildable cells.
- Placing a tower deducts money and occupies the cell.
- Enemies follow the path and damage the base when they arrive.
- Towers acquire targets in range and fire on cooldown.
- Projectiles damage or destroy enemies.
- Defeated enemies award money and score.
- Waves progress and increase challenge.
- Defeat occurs at zero base health.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Large tech tree.
- Tower upgrades unless trivial.
- Procedural maps.
- Complex pathfinding; fixed waypoint following is enough.
- Required audio.
