---
title: Asteroids Demo Spec
category: Game specs
categoryindex: 9
---

# Asteroids Demo Spec

## Goal

Build a playable 2D Asteroids-style arcade demo that proves the GUI runtime can keep a game loop alive, process continuous keyboard input, render moving vector-like shapes, handle collision-heavy state changes, and expose deterministic evidence for tests.

## Player Experience

The first screen is the live game, not a menu. A triangular ship starts near the center of a dark playfield with several large asteroids drifting around it. The player rotates, thrusts, shoots, avoids collisions, and clears waves by splitting asteroids into smaller pieces. The game should feel responsive and readable at desktop and small-window sizes.

## Core Rules

- The playfield wraps on all four edges.
- The ship wraps at screen edges.
- Asteroids wrap at screen edges.
- Bullets wrap or expire after a fixed lifetime; prefer expiration after 1.2 seconds to avoid clutter.
- Large asteroids split into two medium asteroids when shot.
- Medium asteroids split into two small asteroids when shot.
- Small asteroids disappear when shot.
- The player loses one life when the ship collides with an asteroid.
- After losing a life, the ship respawns at the center only when the center area is safe.
- The wave is complete when no asteroids remain.
- Each new wave adds at least one asteroid or increases asteroid speed slightly.
- The game ends when lives reach zero.

## Controls

- `Left` or `A`: rotate ship counterclockwise.
- `Right` or `D`: rotate ship clockwise.
- `Up` or `W`: thrust forward while held.
- `Space`: fire bullet, with a short cooldown.
- `P`: pause or resume.
- `R`: restart after game over.
- `Esc`: request close when hosted interactively.

## Game State

Track at minimum:

- Ship position, velocity, rotation, thrusting flag, invulnerability timer, and alive/respawning state.
- Asteroid list with position, velocity, radius/category, rotation, and stable id.
- Bullet list with position, velocity, remaining lifetime, and stable id.
- Score, lives, wave number, pause state, game-over state, elapsed time, and random seed.
- Last input snapshot for deterministic tests.

## Scoring

- Large asteroid: 20 points.
- Medium asteroid: 50 points.
- Small asteroid: 100 points.
- Wave clear bonus: 250 points times wave number.
- No score is awarded during game over or pause.

## Visual Requirements

- Use a dark background with high-contrast ship, bullets, asteroids, and text.
- The ship must visibly rotate and show a thrust flame only while thrust is active.
- Asteroids should be irregular polygon outlines or filled rocky shapes, not identical circles.
- Bullets should be small bright dots or short strokes.
- Show HUD text for score, lives, wave, and paused/game-over status.
- Show a subtle respawn/invulnerability indicator, such as blinking ship outline.
- Avoid assets that require external downloads.

## Runtime Requirements

- The default run mode opens an interactive persistent window and stays open until the user or host closes it.
- Simulation advances by delta time rather than fixed per-frame position increments.
- Pause stops simulation but still renders the paused frame and accepts resume/restart input.
- The game must remain stable when the window is resized.
- The demo must start without network access.

## Determinism and Evidence

- Accept an optional seed for asteroid spawning.
- Provide a deterministic evidence mode that runs for a bounded duration, injects a short input script, renders at least one frame, and then exits with an outcome.
- Evidence mode should report frame count, input events observed, screenshot or pixel-readback availability, score, wave, entity counts, and close reason.
- A screenshot should show the playfield, ship, at least one asteroid, and HUD.

## Acceptance Criteria

- Starting the demo immediately shows a ship, asteroids, and HUD.
- Holding thrust changes ship velocity over time.
- Rotating changes ship heading independently of velocity.
- Firing creates bullets with cooldown and lifetime.
- Bullet-asteroid collision splits or removes asteroids according to size.
- Ship-asteroid collision removes a life and respawns safely.
- Clearing a wave creates the next wave.
- Pause freezes asteroid and bullet motion.
- Game over prevents normal play until restart.
- Evidence mode exits by explicit evidence completion, not by accidental first-frame close.

## Out of Scope

- Online leaderboards.
- Multiplayer.
- Complex particle systems.
- Full physics engine.
- Asset pipeline or audio requirements.
