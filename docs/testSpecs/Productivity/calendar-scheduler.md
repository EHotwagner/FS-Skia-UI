---
title: Calendar Scheduler Demo Spec
category: Productivity specs
categoryindex: 10
---

# Calendar Scheduler Demo Spec

## Goal

Build a complete calendar scheduler demo that exercises a month grid, day selection, event creation and listing per day, month navigation, an event detail panel, and evidence-friendly rendering.

## User Experience

The app opens to the current month as a grid of day cells, with a selected day and its events listed alongside. The user moves the selection, navigates months, and adds events to a day. The layout should be readable, fast to understand, and deterministic under a fixed reference date and seed.

## Layout

- A month header showing the month and year with previous and next controls.
- A 7-column weekday grid with up to 6 week rows of day cells.
- A day detail panel listing the selected day's events.
- A status strip showing the selected date and the count of events that day.

## Controls

- `Arrow keys`: move the selected day within the grid.
- `Page Up` / `Page Down`: go to the previous / next month.
- `Enter` / `N`: create an event on the selected day and begin editing its title.
- `F2`: edit the focused event.
- `Esc`: cancel an edit.
- `Delete`: remove the focused event.

## Core Behaviors

- The month grid aligns the first day under its correct weekday.
- Days outside the current month are shown muted or blank but never overlap valid days.
- Each event has a title and belongs to exactly one day.
- A day cell shows a marker or count when it has events.
- Month navigation preserves the selected day-of-month where valid, otherwise clamps to the last valid day.
- The reference "today" date must be injectable for determinism rather than read from the wall clock.

## Data Model

- A current month anchor and a selected date.
- A map from date to an ordered list of events, each with a stable id and title.
- A focused event reference and an edit buffer.

## Visual Requirements

- Show the month header, weekday labels, the day grid, the detail panel, and the status strip.
- The selected day must be clearly outlined; today's date should be distinguishable.
- Days with events must show a visible marker or count.
- The header and weekday labels must keep stable dimensions while the detail panel scrolls.

## App State

Track at minimum:

- Current month anchor, selected date, and injected reference date.
- Events keyed by date with stable ids.
- Focused event reference, edit buffer, and random seed.

## Determinism and Evidence

- Accept an injected reference date and an optional seed for sample events.
- Evidence mode should inject a deterministic script that selects a day, adds an event, and navigates one month forward and back.
- Evidence outcome should include frame count, month-change count, selected date, event count for that day, and close reason.
- Screenshot evidence should show the month grid, the selected day, and the day detail panel.

## Acceptance Criteria

- The first day of the month aligns under the correct weekday.
- Arrow navigation moves the selection within the visible month.
- Month navigation clamps the selected day to a valid date.
- Adding an event lists it under the selected day and updates the count.
- Days with events show a marker or count.
- The reference date is injected, not read from the wall clock.
- Header and weekday labels stay fixed while the detail panel scrolls.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Week or day timeline views.
- Recurring events, reminders, or time-of-day scheduling.
- Time zones or persistence.
- External assets or audio.
