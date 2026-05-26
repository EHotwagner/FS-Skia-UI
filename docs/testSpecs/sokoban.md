# Sokoban Demo Spec

## Goal

Build a Sokoban puzzle demo that exercises grid rendering, deterministic input, push mechanics, undo, level loading, win detection, restart, and evidence-friendly board states.

## Player Experience

The game opens to a small warehouse puzzle with walls, floor, crates, goals, and a player. The player moves on the grid, pushes crates onto goals, uses undo when needed, restarts the level, and advances to the next level after solving.

## Board

- Use a tile grid with walls, floors, goals, crates, crates-on-goals, player, and player-on-goal.
- Include at least three built-in levels.
- The first level should be simple enough to solve in under 20 moves.
- Board dimensions may vary by level but should fit common windows.
- Empty outside space should not be treated as walkable floor.

## Controls

- `Up` or `W`: move up.
- `Down` or `S`: move down.
- `Left` or `A`: move left.
- `Right` or `D`: move right.
- `U` or `Backspace`: undo one move.
- `R`: restart current level.
- `N`: next level after solved, or cycle for testing.
- `P`: pause or resume if the shared runtime expects pause.
- `Esc`: request close when hosted interactively.

## Core Rules

- Player moves one cell per valid input.
- Player cannot move into walls.
- Player can push one crate if the cell beyond it is empty floor or goal.
- Player cannot pull crates.
- Player cannot push two crates at once.
- A crate on a goal counts as satisfied.
- Level is solved when every goal has a crate.
- Move count increments for every successful player movement.
- Push count increments only when a crate moves.

## Undo

- Undo restores the previous board, player position, move count, push count, and solved state.
- Undo stack should store enough history for the current level from the start.
- Invalid moves should not create undo entries.

## Scoring

No arcade score is required. Show:

- Level number.
- Move count.
- Push count.
- Best moves for the current session if easy to track.

## Visual Requirements

- Render a stable grid with distinct walls, floors, goals, crates, and player.
- Crates on goals must be visually distinct from crates off goals.
- The player on a goal must still communicate both player and goal.
- HUD shows level, moves, pushes, solved state, and controls state if paused.
- Board should be centered or otherwise clearly framed.

## Game State

Track at minimum:

- Current level index.
- Static map with walls and goals.
- Player position.
- Crate positions.
- Move count, push count, solved flag, undo stack, paused flag, and elapsed time.

## Determinism and Evidence

- No randomness is required.
- Evidence mode should inject a short deterministic move script that performs at least one valid move and one crate push.
- Evidence outcome should include frame count, input count, level index, move count, push count, crate positions, solved flag, undo depth, and close reason.
- Screenshot evidence should show the board, crates, goals, player, and HUD.

## Acceptance Criteria

- Valid movement updates player position exactly one cell.
- Wall movement is blocked.
- Single-crate push works when the destination is free.
- Push into wall or another crate is blocked.
- Solved state triggers when all goals are covered by crates.
- Undo restores previous valid states.
- Restart restores the original level.
- Next-level flow works after solving.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Level editor.
- External level-pack parser.
- Animated walking requirements.
- Online scoring.
- Required audio.
