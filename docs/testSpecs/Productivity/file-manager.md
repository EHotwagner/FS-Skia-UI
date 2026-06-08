---
title: File Manager Demo Spec
category: Productivity specs
categoryindex: 10
---

# File Manager Demo Spec

## Goal

Build a complete file manager demo that exercises a tree or breadcrumb navigation model, a sortable file listing, selection, rename and delete operations over an in-memory virtual filesystem, and evidence-friendly rendering.

## User Experience

The app opens to the root of a virtual filesystem showing folders and files. The user navigates into folders, selects items, sorts the listing, and performs basic operations. The layout should be readable, fast to understand, and deterministic under a seed.

## Layout

- A breadcrumb bar showing the current path from root.
- A main listing pane with columns for name, type, size, and modified order.
- An optional left sidebar showing top-level folders for quick navigation.
- A status strip showing item count, selected count, and total selected size.

## Controls

- `Arrow Up` / `Arrow Down`: move the selection cursor.
- `Enter`: open the selected folder or focus the selected file.
- `Backspace`: navigate to the parent folder.
- `F2`: rename the selected item.
- `Delete`: delete the selected item after a confirmation step.
- `Ctrl+A`: select all items in the current folder.
- `S`: cycle the sort key between name, size, and type.

## Core Behaviors

- The filesystem is a virtual in-memory tree of folders and files; no real disk access occurs.
- Navigating into a folder shows only its direct children.
- The listing can be sorted ascending by the active sort key, with folders grouped before files.
- Rename must reject empty names and duplicate names within the same folder.
- Delete removes the item and its subtree and updates counts.
- Navigation must never escape above the virtual root.

## Data Model

- A tree of nodes, each a folder (with children) or a file (with a size).
- A current path, a selection set, and the active sort key and direction.

## Visual Requirements

- Show the breadcrumb, the listing columns, the sidebar if present, and the status strip.
- The selection cursor and any multi-selection must be clearly distinguished.
- Folders and files must be visually separable, for example by an icon glyph or label.
- The breadcrumb and status strip must keep stable dimensions while the listing scrolls.

## App State

Track at minimum:

- The virtual filesystem tree and the current path.
- Selection set, cursor index, sort key, and sort direction.
- Pending operation state (rename buffer, delete confirmation) and random seed.

## Determinism and Evidence

- Accept an optional seed for any generated sample tree.
- Evidence mode should inject a deterministic script that enters a folder, changes the sort key, and selects an item.
- Evidence outcome should include frame count, navigation count, current path, item count, selected count, and close reason.
- Screenshot evidence should show the breadcrumb, listing, and status strip.

## Acceptance Criteria

- Entering a folder shows only its direct children.
- Backspace returns to the parent and never escapes the root.
- Sorting reorders the listing with folders grouped before files.
- Rename rejects empty or duplicate names.
- Delete removes the item and its subtree and updates counts.
- The selection cursor stays within the current listing bounds.
- Breadcrumb and status strip stay fixed while the listing scrolls.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Real disk or network filesystem access.
- Copy, move, or drag-and-drop between folders.
- File previews or content viewers.
- External assets or audio.
