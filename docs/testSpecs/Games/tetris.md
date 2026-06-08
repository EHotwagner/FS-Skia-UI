---
title: Tetris Demo Spec
category: Game specs
categoryindex: 9
---

# Tetris Demo Spec

## Goal

Build a complete single-player falling-block puzzle demo that exercises grid rendering, keyboard input, deterministic step timing, collision checks, line clearing, scoring, and a persistent interactive runtime.

## Player Experience

The game opens directly into a Tetris-style board with an active tetromino falling inside a 10 by 20 visible grid. A side panel shows next piece, held piece, score, lines, level, and game state. The player moves, rotates, drops, holds, clears lines, and restarts after topping out.

## Board and Pieces

- The visible board is 10 columns by 20 rows.
- The spawn area may include hidden rows above the visible board.
- Use the seven standard tetrominoes: I, O, T, S, Z, J, and L.
- Each tetromino has a distinct color.
- A piece locks when it can no longer move down after a short lock delay.
- Filled rows clear and rows above fall down.
- The game ends when a newly spawned piece overlaps occupied cells.

## Controls

- `Left` or `A`: move piece left.
- `Right` or `D`: move piece right.
- `Down` or `S`: soft drop while held.
- `Up` or `X`: rotate clockwise.
- `Z`: rotate counterclockwise.
- `Space`: hard drop.
- `C` or `Shift`: hold piece.
- `P`: pause or resume.
- `R`: restart after game over.
- `Esc`: request close when hosted interactively.

## Rotation and Movement

- Implement wall-kick behavior sufficient to rotate near walls and floor.
- O piece rotation should not visually drift.
- Horizontal movement supports repeated movement while key is held, with an initial delay and repeat rate.
- Soft drop accelerates the fall and awards small points per manually dropped row.
- Hard drop instantly places the piece at its landing row and awards points per dropped row.
- Hold is allowed once per spawned piece.

## Scoring

- Soft drop: 1 point per row.
- Hard drop: 2 points per row.
- Single line clear: 100 times level.
- Double: 300 times level.
- Triple: 500 times level.
- Tetris: 800 times level.
- Level increases every 10 cleared lines.
- Fall speed increases by level, with a reasonable lower bound so the game remains playable.

## Visual Requirements

- Render the board as a stable grid with fixed-size cells.
- Locked cells and the active piece must be visually distinct.
- Show a ghost piece landing preview.
- Show next piece and held piece in compact preview boxes.
- Show score, lines, level, and state labels.
- Paused and game-over overlays must not obscure all board evidence; the board should remain visible behind or beside the state text.
- The board must remain readable after resizing.

## Game State

Track at minimum:

- Board cells.
- Active piece type, rotation, grid position, and lock timer.
- Next queue, held piece, hold-used flag, and random seed.
- Score, lines, level, paused flag, game-over flag, elapsed time, and frame count.
- Input repeat timers for left/right/soft drop.

## Determinism and Evidence

- Accept an optional seed for piece order.
- Evidence mode should inject a deterministic input script that moves and drops at least one piece.
- Evidence outcome should include frame count, input count, board occupancy count, active piece type, next piece type, score, lines, level, and close reason.
- Screenshot evidence should clearly show the board, active piece, ghost piece, side panel, and HUD values.

## Acceptance Criteria

- A seeded run produces repeatable first pieces.
- Pieces collide correctly with walls, floor, and locked cells.
- Rotations work near walls using wall kicks.
- A full row clears and increases score and line count.
- Hard drop locks the piece at the predicted ghost position.
- Hold swaps the active piece only once per spawned piece.
- The game tops out when spawn is blocked.
- Pause freezes falling and input-driven movement except resume/restart.
- Interactive mode remains open until closed by user or host.

## Out of Scope

- Multiplayer garbage lines.
- Perfect official guideline scoring beyond the values above.
- Audio.
- Networked leaderboards.
