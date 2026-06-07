---
title: Breakout Demo Spec
category: Game specs
categoryindex: 9
---

# Breakout Demo Spec

## Goal

Build a playable Breakout-style arcade demo that exercises paddle input, ball physics, tile-like brick layout, collision response, scoring, levels, lives, and bounded visual evidence.

## Player Experience

The game starts with a paddle near the bottom, a ball ready to launch, and rows of colored bricks above. The player moves the paddle, launches the ball, breaks bricks, catches powerups if included, advances levels, and restarts after losing all lives.

## Core Rules

- The ball bounces off the left, right, and top walls.
- The ball is lost when it passes below the paddle.
- The paddle moves horizontally and is clamped to the playfield.
- The ball bounces off the paddle with angle based on hit position.
- Bricks disappear when their hit points reach zero.
- Some bricks may require multiple hits.
- The level is complete when all breakable bricks are gone.
- Losing a ball decrements lives and resets ball-to-paddle launch state.
- Game over occurs when lives reach zero.

## Controls

- `Left` or `A`: move paddle left while held.
- `Right` or `D`: move paddle right while held.
- `Space`: launch ball from paddle or confirm restart after game over.
- `P`: pause or resume.
- `R`: restart.
- `Esc`: request close when hosted interactively.

## Level Layout

- Use a fixed brick grid, for example 10 columns by 6 rows.
- Each row has a distinct color and point value.
- At least one row should contain stronger two-hit bricks after level 1.
- Later levels increase ball speed slightly and may vary brick patterns.
- Leave enough space around the brick field for ball movement and clear screenshot evidence.

## Scoring

- Bottom brick rows: 10 points.
- Middle brick rows: 20 points.
- Top brick rows: 40 points.
- Two-hit brick final break: full row value; first hit gives no score or half score, chosen consistently.
- Level clear bonus: 500 points times level.

## Optional Powerups

Powerups are optional but allowed if implemented simply:

- Wider paddle for 10 seconds.
- Slower ball for 8 seconds.
- Multi-ball with one extra ball.

If powerups are included, they must be deterministic under seeded mode and visible in the HUD or playfield.

## Visual Requirements

- Render a clear rectangular playfield with dark background.
- Paddle, ball, walls, bricks, and HUD must be visually distinct.
- Bricks should be arranged in a stable grid and not shift during play.
- Show score, lives, level, paused/game-over text, and launch-ready state.
- Ball movement should be visible in evidence frames; avoid a ball color that blends into bricks.

## Game State

Track at minimum:

- Paddle position, width, speed, and active modifiers.
- Ball or balls with position, velocity, radius, and attached/launched state.
- Brick grid with hit points, breakable flag, and score value.
- Score, lives, level, paused flag, game-over flag, elapsed time, and random seed.
- Last collision details for debugging tests.

## Determinism and Evidence

- Accept an optional seed for powerups and level variants.
- Evidence mode should launch the ball, move the paddle, and run long enough to show ball motion.
- Evidence outcome should include frame count, input count, brick count remaining, ball count, score, lives, level, and close reason.
- Screenshot evidence should show paddle, ball, bricks, walls, and HUD.

## Acceptance Criteria

- The ball launches from the paddle with `Space`.
- Paddle movement is continuous while keys are held.
- Ball-wall, ball-paddle, and ball-brick collisions are stable.
- Bricks disappear or decrement hit points on collision.
- Score updates when bricks break.
- Life count decreases when all balls are lost.
- Level advances after all breakable bricks are cleared.
- Pause freezes ball and paddle simulation.
- Interactive mode stays open until explicitly closed.

## Out of Scope

- Pixel-perfect arcade clone behavior.
- Complex spin physics.
- Online scoring.
- Required audio.
