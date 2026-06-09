---
title: Controls Gallery — Overlays & Feedback Page Spec
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery — Overlays & Feedback Page Spec

Inherits the shell, palette, pointer contract, keyboard contract, and
determinism rules from [`00-controls-gallery-overview.md`](00-controls-gallery-overview.md).

## Goal

Demonstrate the overlay and feedback controls — tooltip, modal dialog, layered
overlay, transient toast, determinate progress, indeterminate spinner, and
validation message — triggered and dismissed by pointer, with correct modality and
focus handling.

## Controls Demonstrated

| Control | Module | Required | Events | Demonstrates |
|---------|--------|----------|--------|--------------|
| tooltip | Tooltip | `text` | — | Hover/focus auxiliary explanation |
| dialog | Dialog | `children` | `onSelected` | Modal content region |
| overlay | Overlay | `child` | — | Layered content above the page |
| toast | Toast | `text` | — | Transient status message |
| progress-bar | ProgressBar | `value` | — | Determinate progress |
| spinner | Spinner | — | — | Indeterminate progress |
| validation-message | ValidationMessage | `text` | — | Model-tied validation text |

## User Experience

The page presents trigger buttons that raise each overlay: a modal dialog that
traps focus until dismissed, a non-modal overlay layer, and a toast that appears
and auto-dismisses. Live progress and spinner indicators animate, and a small form
shows validation messages reacting to input. Tooltips appear on hover throughout.

## Layout

- A heading `Overlays & Feedback` and description.
- A row of trigger `Button`s: `Open Dialog`, `Show Overlay`, `Show Toast`.
- A `ProgressBar` whose value is driven by a slider and by a "simulate" action,
  plus a `Spinner` running while a simulated task is "busy".
- A tiny validation form (one `TextBox` + `ValidationMessage`) demonstrating
  success/error text.
- Every trigger and indicator carries a `Tooltip`.

## Mouse & Pointer Interactions

- `HoverEnter` on any element with a tooltip shows it after the hover; `HoverLeave`
  hides it.
- `Click` `Open Dialog` raises a modal `Dialog`; the scrim dims the page; pointer
  input to controls behind the scrim is blocked; `Click` a dialog button fires
  `onSelected` (`Confirm`/`Cancel`) and closes it.
- `Click` `Show Overlay` raises a non-modal `Overlay` layer that the page can still
  interact around; clicking its close affordance dismisses it.
- `Click` `Show Toast` raises a `Toast` that auto-dismisses after a fixed duration;
  clicking it dismisses it early.
- `Click`/drag the progress slider sets the `ProgressBar` value; `Click` `Simulate`
  animates progress 0→100 and runs the `Spinner` until complete.
- Typing in the validation form updates the `ValidationMessage` live.

## Keyboard

- `Esc` dismisses the topmost overlay (dialog, overlay, toast) in that order.
- While a modal dialog is open, `Tab` is trapped within the dialog; focus returns
  to the trigger on close.
- `Enter` activates the dialog's default (`Confirm`) button.

## Core Behaviors

- The dialog is modal: it traps focus, blocks pointer input behind its scrim, and
  must be dismissed before page interaction resumes.
- The overlay is non-modal: it layers above content without trapping focus.
- The toast is transient and self-dismissing, capped to one or a small stack.
- The progress bar is determinate (0–100); the spinner is indeterminate and runs
  only while busy.
- Validation messages reflect the bound field's success/error state and never
  shift the layout when toggling (reserved space).

## Data Model

- Open flags and content for the dialog and overlay; the dialog result.
- Toast queue with per-toast remaining duration.
- Progress value and a "busy" flag driving the spinner.
- Validation form value and its validation state.
- Tooltip target and visible flag.

## Visual / Palette Requirements

- The dialog scrim dims the page; the dialog surface uses surface with an elevation
  shadow and accent default button.
- The toast uses surface-raised with a role-colored accent edge.
- Progress fill uses accent; the spinner uses accent on a muted track.
- Validation success uses the success role, error uses the danger role.

## App State

Track: dialog/overlay open flags and result; toast queue and timers; progress
value and busy flag; validation value and state; tooltip target; focus-return
target.

## Determinism and Evidence

- Toast and progress durations are fixed and seed-independent; evidence mode uses
  deterministic, frame-counted timing rather than wall-clock.
- Evidence mode hovers to show a tooltip, opens and confirms the dialog, shows and
  dismisses the overlay, raises a toast, runs the progress simulation to 100, and
  triggers then clears a validation error.
- Evidence outcome: dialog result, overlay shown/dismissed, toast count, final
  progress value, busy transitions, validation transitions, and close reason.
- Screenshot evidence shows the modal dialog over the dimmed page, a visible toast,
  and the progress bar mid-fill with the spinner running.

## Acceptance Criteria

- Tooltips appear on hover and dismiss on leave.
- The dialog traps focus, blocks input behind its scrim, returns a result via
  `onSelected`, and restores focus on close.
- The overlay layers without trapping focus and dismisses on demand.
- The toast appears, auto-dismisses, and can be dismissed early by click.
- The progress bar reflects its value; the spinner runs only while busy.
- Validation messages reflect state without shifting layout.
- `Esc` dismisses overlays topmost-first.

## Out of Scope

- Stacked nested modal dialogs.
- Draggable or resizable dialogs.
- Rich toast actions beyond dismiss.
