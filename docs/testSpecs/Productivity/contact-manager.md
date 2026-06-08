---
title: Contact Manager Demo Spec
category: Productivity specs
categoryindex: 10
---

# Contact Manager Demo Spec

## Goal

Build a complete contact manager demo that exercises a searchable contact list, a detail panel, adding and editing contacts, field validation, alphabetical sorting, and evidence-friendly rendering.

## User Experience

The app opens to a list of contacts with a selected entry whose details show alongside. The user searches, selects, adds, and edits contacts across a small set of fields. The layout should be readable, fast to understand, and deterministic under a seed.

## Layout

- A left list of contacts sorted alphabetically with a selection cursor.
- A search field above the list.
- A right detail panel showing the selected contact's fields: name, email, phone, and company.
- A status strip showing the total contact count and the filtered count.

## Controls

- `Arrow Up` / `Arrow Down`: move the selection in the list.
- `/`: focus the search field.
- `Enter`: open the selected contact in the detail panel.
- `Ctrl+N`: create a new contact and focus its name field.
- `F2`: edit the selected contact.
- `Tab`: move between fields while editing.
- `Esc`: cancel an edit.
- `Delete`: remove the selected contact after a confirmation step.

## Core Behaviors

- Each contact has a name, email, phone, and company.
- The list stays sorted alphabetically by name as contacts are added or renamed.
- Search filters the list by a case-insensitive substring across name and company.
- Email must be validated for a basic `local@domain` shape before a save is accepted.
- A blank name must be rejected on save.
- Deleting a contact selects a stable neighboring entry.

## Data Model

- A list of contacts, each with a stable id and the four fields.
- A selection id, a search query, an edit buffer, and per-field validation state.

## Visual Requirements

- Show the search field, contact list, detail panel, and status strip.
- The selected contact in the list must be clearly highlighted.
- Invalid fields during editing must show a clear, stable error indicator.
- The search field and status strip must keep stable dimensions while the list scrolls.

## App State

Track at minimum:

- Contacts with ids and fields.
- Selection id, search query, edit buffer, focused field, and validation state.
- Scroll offset and random seed.

## Determinism and Evidence

- Accept an optional seed for any generated sample contacts.
- Evidence mode should inject a deterministic script that searches, selects a contact, edits a field with a valid value, and attempts one invalid email.
- Evidence outcome should include frame count, edit count, total and filtered counts, selected contact name, and close reason.
- Screenshot evidence should show the list, the search field, and the detail panel.

## Acceptance Criteria

- The list stays alphabetically sorted as contacts change.
- Search filters by a case-insensitive substring and updates the filtered count.
- A malformed email is rejected on save with a stable error.
- A blank name is rejected on save.
- Selecting a contact shows its fields in the detail panel.
- Deleting selects a stable neighbor.
- Search field and status strip stay fixed while the list scrolls.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Groups, tags, or avatars.
- Import or export of contact formats.
- Persistence or sync.
- External assets or audio.
