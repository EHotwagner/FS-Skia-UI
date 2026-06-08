---
title: Todo Task Manager Demo Spec
category: Productivity specs
categoryindex: 10
---

# Todo Task Manager Demo Spec

## Goal

Build a complete todo task manager demo that exercises a task list, adding and completing tasks, priority and filter states, reordering, a live summary, and evidence-friendly rendering.

## User Experience

The app opens to a list of tasks with a focused row and an input affordance to add new ones. The user adds tasks, toggles completion, changes priority, filters the view, and reorders items. The layout should be readable, fast to understand, and deterministic under a seed.

## Layout

- A header with the list title and a quick-add field.
- A main list of task rows, each showing a completion box, title, and priority marker.
- A filter strip offering `All`, `Active`, and `Completed`.
- A status strip showing total, active, and completed counts.

## Controls

- `Arrow Up` / `Arrow Down`: move the focused row.
- `Space`: toggle completion of the focused task.
- `N`: focus the quick-add field to enter a new task.
- `Enter`: commit the new task or an edit.
- `F2`: edit the focused task title.
- `1` / `2` / `3`: set the focused task priority to low / medium / high.
- `Shift+Arrow`: reorder the focused task up or down.
- `F`: cycle the active filter.

## Core Behaviors

- Each task has a title, a completion flag, and a priority.
- Adding a task appends it as active and focuses it.
- Toggling completion updates the counts and respects the active filter.
- The filter restricts visible rows without deleting any task.
- Reordering moves a task within the underlying list, not just the filtered view.
- Editing must reject an empty title and restore the previous title on cancel.

## Data Model

- An ordered list of tasks, each with a stable id, title, completion flag, and priority.
- A focus index, an active filter, and an edit buffer.

## Visual Requirements

- Show the header, quick-add field, task rows, filter strip, and status strip.
- Completed tasks must be visually distinct, for example struck through or muted.
- Priority must be shown by a clear marker or color.
- The focused row must be clearly outlined.
- The header and status strip must keep stable dimensions while the list scrolls.

## App State

Track at minimum:

- Tasks with ids, titles, completion flags, and priorities.
- Focus index, active filter, quick-add buffer, edit-in-progress flag, and edit buffer.
- Scroll offset and random seed.

## Determinism and Evidence

- Accept an optional seed for any generated sample tasks.
- Evidence mode should inject a deterministic script that adds a task, completes one, changes a priority, and switches the filter.
- Evidence outcome should include frame count, add count, total, active, and completed counts, active filter, and close reason.
- Screenshot evidence should show the list, a completed task, the filter strip, and the counts.

## Acceptance Criteria

- Adding a task appends it and updates the total.
- Toggling completion updates the active and completed counts.
- The filter shows only matching tasks without deleting any.
- Setting priority updates the marker on the focused task.
- Reordering moves the task within the underlying list.
- Editing rejects an empty title and restores on cancel.
- Header and status strip stay fixed while the list scrolls.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Due dates, reminders, or sub-tasks.
- Multiple lists or projects.
- Persistence or sync.
- External assets or audio.
