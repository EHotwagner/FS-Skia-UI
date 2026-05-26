# Lunar Lander Demo Spec

## Goal

Build a Lunar Lander-style physics demo that exercises gravity, thrust, rotation, fuel, terrain collision, landing evaluation, scoring, restart flow, and persistent interactive rendering.

## Player Experience

The game opens to a side-view lunar terrain with a lander above the surface, a marked landing pad, and HUD. The player rotates, applies thrust, manages fuel, and attempts to land gently on the pad with acceptable speed and angle.

## World

- Use a 2D side-view playfield with uneven terrain and one clearly marked landing pad.
- The lander starts above the terrain with downward or neutral velocity.
- Gravity constantly accelerates the lander downward.
- Terrain and pad remain visible at all times in the base demo.
- World edges may clamp or wrap horizontally; choose one and show it consistently. Prefer horizontal wrapping for classic feel.

## Controls

- `Left` or `A`: rotate lander counterclockwise.
- `Right` or `D`: rotate lander clockwise.
- `Up`, `W`, or `Space`: main thrust while held.
- `Down` or `S`: optional weaker retro-thrust if implemented.
- `P`: pause or resume.
- `R`: restart after landing or crash.
- `Esc`: request close when hosted interactively.

## Physics

- Lander has position, velocity, rotation, angular velocity if used, mass-like tuning, and fuel.
- Gravity is constant.
- Main thrust applies acceleration in the lander's facing direction and consumes fuel.
- Rotation changes orientation at a fixed rate or through angular velocity.
- When fuel reaches zero, thrust input has no effect.
- Collision with terrain evaluates crash or landing.
- Landing succeeds only on the pad, with vertical speed, horizontal speed, and angle within thresholds.

## Landing Rules

Default safe landing thresholds:

- Vertical speed at contact: no more than 35 units per second.
- Horizontal speed at contact: no more than 20 units per second.
- Angle from upright: no more than 12 degrees.
- Contact point must overlap the landing pad.

Anything outside those thresholds is a crash.

## Scoring

- Successful landing base score: 1000.
- Fuel bonus: remaining fuel rounded down times 5.
- Softness bonus: higher bonus for lower landing speed.
- Crash score: 0 for that attempt.
- Show attempt count.

## Visual Requirements

- Render dark space background, terrain line or filled terrain, landing pad, lander, thrust flame, and HUD.
- Lander orientation must be visually obvious.
- Thrust flame appears only while thrust is active and fuel remains.
- HUD shows fuel, vertical speed, horizontal speed, angle, score, attempt, and state.
- Crash or landed state should be clear without hiding the lander and pad.

## Game State

Track at minimum:

- Lander position, velocity, rotation, thrusting flag, fuel, alive/landed/crashed state.
- Terrain points, pad bounds, gravity, and physics tuning constants.
- Score, attempt count, paused flag, elapsed time, and random seed if terrain can vary.

## Determinism and Evidence

- Accept an optional seed if terrain varies; fixed terrain is also acceptable.
- Evidence mode should inject rotation and thrust inputs, run long enough to show lander motion and thrust, and exit explicitly.
- Evidence outcome should include frame count, input count, lander position, velocity, rotation, fuel, state, score, and close reason.
- Screenshot evidence should show the lander, terrain, landing pad, thrust or HUD evidence of motion, and flight metrics.

## Acceptance Criteria

- Gravity changes vertical velocity over time.
- Thrust consumes fuel and changes velocity in the lander's facing direction.
- Rotation changes lander orientation.
- Fuel depletion disables thrust.
- Terrain collision evaluates crash.
- Pad contact with safe speed and angle evaluates successful landing.
- Restart resets the attempt while preserving deterministic terrain.
- Pause freezes physics and timers.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Orbital mechanics.
- Multiple landing pads.
- Procedural terrain requirement.
- Complex particle effects.
- Required audio.
