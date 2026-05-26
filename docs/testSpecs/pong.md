# Pong Demo Spec

## Goal

Build a Pong demo that exercises simple real-time physics, paddle control, AI opponent behavior, scoring, serve states, pause/restart flow, and persistent interactive launch behavior.

## Player Experience

The game opens directly to a two-paddle court. The player controls the left paddle, an AI controls the right paddle, and a ball moves between them. The first side to reach the target score wins, then the player can restart.

## Core Rules

- Ball bounces off the top and bottom walls.
- Ball bounces off paddles.
- Missing the ball awards one point to the opposing side.
- After a score, the ball resets to center and waits briefly before serving.
- First side to 10 points wins by default.
- Paddle movement is clamped to the court.
- Ball speed increases slightly after paddle hits up to a maximum.

## Controls

- `Up` or `W`: move player paddle up.
- `Down` or `S`: move player paddle down.
- `Space`: start serve if waiting, or restart after match end.
- `P`: pause or resume.
- `R`: restart match.
- `Esc`: request close when hosted interactively.

## AI Opponent

- AI tracks the ball with limited speed, not perfect teleporting.
- AI may include a small reaction delay or target error so it can miss.
- AI difficulty can increase slowly as rallies get longer.
- AI must be deterministic when seeded.

## Scoring

- Score increments by 1 when the opponent misses.
- Rally length may be displayed but does not affect score.
- Winning state appears when either side reaches the target score.

## Visual Requirements

- Show a simple court, center line, paddles, ball, player score, AI score, and state text.
- The ball must be high contrast and visible in screenshot evidence.
- The winner message should not obscure the scores.
- Layout must preserve playfield proportions after resizing.

## Game State

Track at minimum:

- Player paddle position and velocity intent.
- AI paddle position, target, and reaction state.
- Ball position, velocity, speed multiplier, and serve/waiting state.
- Player score, AI score, rally count, target score, paused flag, match-over flag, elapsed time, and random seed.

## Determinism and Evidence

- Accept an optional seed for serve direction and AI imperfection.
- Evidence mode should inject paddle movement and serve input, run enough frames to show ball motion, and exit explicitly.
- Evidence outcome should include frame count, input count, ball position, paddle positions, scores, rally count, game state, and close reason.
- Screenshot evidence should show both paddles, ball, court markings, and scores.

## Acceptance Criteria

- Player paddle moves while input is held.
- AI paddle follows the ball with limited speed.
- Ball bounces correctly off walls and paddles.
- Paddle hit location affects outgoing ball angle.
- Missing the ball increments the correct score and resets serve.
- Match ends at target score and can restart.
- Pause freezes ball and paddle simulation.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Multiplayer networking.
- Tournament brackets.
- Complex spin model.
- Required audio.
