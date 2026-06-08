---
title: Markdown Notes Demo Spec
category: Productivity specs
categoryindex: 10
---

# Markdown Notes Demo Spec

## Goal

Build a complete markdown notes demo that exercises a note list, a text editor pane, a rendered preview, switching between notes, basic markdown formatting, and evidence-friendly rendering.

## User Experience

The app opens to a list of notes on the left and an editor with a live preview on the right. The user selects a note, edits its body, and sees the rendered preview update. The layout should be readable, fast to understand, and deterministic under a seed.

## Layout

- A left sidebar listing note titles with a selection cursor.
- A central editor pane showing the raw markdown of the active note.
- A right preview pane showing the rendered result, or a toggle between edit and preview.
- A status strip showing the active note title, word count, and modified state.

## Controls

- `Arrow Up` / `Arrow Down`: move the note selection in the sidebar.
- `Enter`: open the selected note for editing.
- `Tab`: toggle focus between the sidebar and the editor.
- `Ctrl+N`: create a new note and focus its title.
- `Ctrl+P`: toggle the preview pane.
- `Esc`: return focus to the sidebar.

## Core Behaviors

- Each note has a title and a markdown body.
- The preview renders at least headings (`#`), bold, italic, unordered list items, and paragraphs.
- Editing updates the live word count and marks the note modified.
- Creating a note adds it to the list and selects it.
- The first line or an explicit title field provides the sidebar title.
- An empty note body must render an empty but stable preview, not an error.

## Data Model

- An ordered list of notes, each with a stable id, title, and body.
- An active note id, an editor buffer, and a per-note modified flag.

## Visual Requirements

- Show the sidebar list, the editor, the preview (when enabled), and the status strip.
- The selected note in the sidebar must be clearly highlighted.
- Rendered headings, emphasis, and lists must be visually distinct from body text.
- The sidebar and status strip must keep stable dimensions while the editor and preview scroll.

## App State

Track at minimum:

- Notes with ids, titles, and bodies.
- Active note id, editor buffer, focus target, and preview-visible flag.
- Per-note modified flags, scroll offsets, and random seed.

## Determinism and Evidence

- Accept an optional seed for any generated sample notes.
- Evidence mode should inject a deterministic script that creates a note, types a heading and a list, and toggles the preview.
- Evidence outcome should include frame count, edit count, note count, active note title, word count, and close reason.
- Screenshot evidence should show the sidebar, editor, and a rendered preview.

## Acceptance Criteria

- Selecting a note loads its body into the editor.
- Editing updates the word count and modified state.
- The preview renders headings, emphasis, and list items.
- Creating a note adds and selects it.
- Toggling the preview shows and hides the rendered pane.
- An empty body renders a stable empty preview.
- Sidebar and status strip stay fixed while panes scroll.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Full CommonMark coverage, tables, or embedded HTML.
- Images, links resolution, or external content.
- Persistence to disk or sync.
- External assets or audio.
