# Snake Demo Spec

## Goal

Build a complete Snake demo that exercises grid state, timed movement ticks, input buffering, collision rules, growth, scoring, restart behavior, and evidence-friendly rendering.

## Player Experience

The game opens directly to a grid with a snake and food. The player steers the snake to eat food, grow longer, avoid walls and its own body, and continue until collision. The game should be readable, fast to understand, and deterministic under a seed.

## Board

- Use a rectangular grid, for example 24 columns by 18 rows.
- The grid should scale to the window while preserving square cells.
- The snake starts with at least 3 segments near the center.
- Food spawns on an empty cell.
- Optional obstacles may appear only after the first level or through an explicit variant flag.

## Controls

- `Up` or `W`: steer up.
- `Down` or `S`: steer down.
- `Left` or `A`: steer left.
- `Right` or `D`: steer right.
- `P`: pause or resume.
- `R`: restart after game over.
- `Esc`: request close when hosted interactively.

## Core Rules

- The snake moves one cell per tick in its current direction.
- Direction changes take effect on the next tick.
- Direct reversal into the neck is ignored unless the snake length is 1.
- Eating food increases score and grows the snake by at least one segment.
- The game ends when the snake hits a wall or itself.
- Food must never spawn inside the snake.
- Tick speed increases gradually as score grows or levels advance.

## Scoring

- Food: 10 points.
- Every 5 food items increases level by 1.
- Level increases tick speed and may change food value if desired.
- Optional bonus food may appear for a limited time, worth 50 points.

## Visual Requirements

- Show the grid, snake head, snake body, food, score, level, and game state.
- Snake head must be distinguishable from the body.
- Food must be high contrast and fit within one cell.
- Paused and game-over states should show clear text without hiding the whole board.
- The board must keep stable dimensions; HUD updates must not resize the grid.

## Game State

Track at minimum:

- Snake segment list, current direction, pending direction, and growth counter.
- Food position and optional bonus food timer.
- Score, level, tick interval, paused flag, game-over flag, elapsed time, and random seed.
- Last movement tick timestamp or accumulator.

## Determinism and Evidence

- Accept an optional seed for food placement.
- Evidence mode should inject a deterministic direction script that moves the snake and, if practical, eats one food item.
- Evidence outcome should include frame count, movement tick count, input count, score, snake length, food position, game state, and close reason.
- Screenshot evidence should show the full board, snake, food, and HUD.

## Acceptance Criteria

- The snake advances exactly one cell per movement tick.
- Direction input is buffered and applied without allowing illegal reversal.
- Eating food grows the snake and increases score.
- New food appears only on empty cells.
- Wall collision ends the game.
- Self collision ends the game.
- Pause freezes movement ticks while preserving the rendered board.
- Restart resets snake, score, food, and state.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Multiplayer Snake.
- Smooth continuous movement between cells.
- Required obstacles in the base mode.
- External assets or audio.
