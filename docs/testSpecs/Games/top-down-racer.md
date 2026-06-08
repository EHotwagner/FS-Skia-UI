---
title: Top-Down Racer Demo Spec
category: Game specs
categoryindex: 9
---

# Top-Down Racer Demo Spec

## Goal

Build a top-down racing demo that exercises continuous movement, acceleration, steering, collision against a track, checkpoints, lap timing, HUD updates, and persistent interactive rendering.

## Player Experience

The game opens directly onto a visible race track with a small car at the starting line. The player accelerates, brakes, steers around the track, passes checkpoints in order, completes laps, and sees lap times and best time.

## Track

- Use a closed-loop track visible from a top-down camera.
- The track has a drivable road, off-road area, walls or boundaries, start/finish line, and ordered checkpoints.
- Track can be defined with simple rectangles, polygons, or a tile map.
- The full track should be visible without camera scrolling for the base demo.
- Off-road slows the car; hard walls block or bounce the car.

## Controls

- `Up` or `W`: accelerate.
- `Down` or `S`: brake or reverse.
- `Left` or `A`: steer left.
- `Right` or `D`: steer right.
- `Space`: handbrake or quick brake.
- `P`: pause or resume.
- `R`: reset car to start or restart race after finish.
- `Esc`: request close when hosted interactively.

## Driving Model

- Car has position, velocity, heading, acceleration, drag, and turn rate.
- Steering should depend on movement speed, with reduced turning when nearly stopped.
- Reverse is allowed but slower than forward movement.
- Off-road applies higher drag and lower max speed.
- Wall collision prevents leaving the allowed world and should not trap the car permanently.

## Laps and Checkpoints

- A lap counts only after checkpoints are crossed in order.
- Wrong-way crossing should not count progress.
- Default race target is 3 laps.
- Show current lap, checkpoint progress, current lap time, best lap time, and total time.
- Finish state appears after completing the target laps.

## Scoring

No points are required. Success is measured by lap completion and time. Optional medals may be shown based on total time, but they are not required.

## Visual Requirements

- Render road, grass/off-road, walls/barriers, start line, checkpoint markers, car, and HUD.
- Car heading must be visually obvious.
- Checkpoints should be visible but not obscure the road.
- HUD should not cover the track in a way that prevents evidence inspection.
- Track layout must resize cleanly while preserving proportions.

## Game State

Track at minimum:

- Car position, velocity, heading, throttle/brake/steer inputs, surface type, and reset state.
- Track geometry, checkpoint list, next checkpoint index, lap count, and finish state.
- Current lap timer, best lap timer, total timer, paused flag, elapsed time, and random seed if any.
- Collision/debug state for tests.

## Determinism and Evidence

- Evidence mode should inject acceleration and steering inputs, run long enough to move the car away from the start, and exit explicitly.
- Evidence outcome should include frame count, input count, car position, speed, heading, lap, next checkpoint, current time, and close reason.
- Screenshot evidence should show the track, car, start/finish line, checkpoint markers, and HUD.

## Acceptance Criteria

- Car accelerates, brakes, steers, and slows due to drag.
- Car heading changes with steering while moving.
- Off-road slows the car relative to road.
- Wall or boundary collision prevents invalid movement.
- Checkpoints must be crossed in order before a lap counts.
- Lap timing starts and updates during play.
- Finish state appears after target laps.
- Pause freezes physics and timers.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Multiple cars.
- Scrolling camera.
- Procedural track generation.
- Detailed tire simulation.
- Required audio.
