---
title: Pomodoro Timer Demo Spec
category: Productivity specs
categoryindex: 10
---

# Pomodoro Timer Demo Spec

## Goal

Build a complete pomodoro timer demo that exercises a countdown driven by timed ticks, work and break phases, a session counter, start/pause/reset controls, a task label, and evidence-friendly rendering.

## User Experience

The app opens to a large countdown showing a full work interval, ready to start. The user starts the timer, watches it count down, and the app cycles between work and break phases while counting completed sessions. The layout should be readable, fast to understand, and deterministic under an injected clock and seed.

## Layout

- A large countdown display showing remaining minutes and seconds.
- A phase indicator showing `Work`, `Short Break`, or `Long Break`.
- A session-progress row showing completed work sessions in the current cycle.
- A task label field and a status strip showing the run state.

## Controls

- `Space`: start or pause the timer.
- `R`: reset the current phase to its full duration.
- `S`: skip to the next phase.
- `T`: edit the task label.
- `Enter`: commit the task label.
- `Esc`: cancel a label edit.

## Core Behaviors

- A work phase has a fixed duration, for example 25 minutes; short and long breaks are shorter.
- The countdown decreases by one second per timer tick while running.
- Reaching zero advances to the next phase: work to break, break to work.
- Every fourth completed work session triggers a long break instead of a short break.
- Completed work sessions increment a counter shown in the progress row.
- The clock source must be injectable so ticks are deterministic rather than wall-clock bound.
- Pause freezes the countdown while preserving the remaining time and phase.

## Data Model

- A current phase, remaining seconds, run state, and per-phase durations.
- A completed-work-session count and a within-cycle index for long-break timing.
- A task label and an edit buffer.

## Visual Requirements

- Show the countdown, phase indicator, session progress, task label, and status strip.
- The countdown must be large and high contrast and use a stable `MM:SS` format.
- The active phase must be clearly indicated and visually distinct between work and break.
- The countdown digits must not change the layout size as they update.

## App State

Track at minimum:

- Phase, remaining seconds, run state, and accumulated tick time.
- Completed-session count, within-cycle index, and per-phase durations.
- Task label, edit buffer, injected clock reference, and random seed.

## Determinism and Evidence

- Accept an injected clock and an optional seed.
- Evidence mode should inject a deterministic tick script that starts the timer, advances enough ticks to complete a phase, and transitions to the next phase.
- Evidence outcome should include frame count, tick count, completed sessions, current phase, remaining seconds, and close reason.
- Screenshot evidence should show the countdown, the phase indicator, and the session progress.

## Acceptance Criteria

- The countdown decreases one second per tick while running.
- Reaching zero advances to the correct next phase.
- Every fourth work session triggers a long break.
- The session counter increments only on completed work phases.
- Pause freezes the countdown and preserves remaining time and phase.
- Reset restores the current phase to its full duration.
- The clock is injected, not read from the wall clock.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Configurable durations through a settings screen.
- Notifications, sound, or background running.
- History logging or persistence.
- External assets or audio.
