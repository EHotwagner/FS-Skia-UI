# Responds-vs-renders — US1 selection move (feature 100, R5, T011)

evidence-kind=responds-vs-renders
status=observed
seam=runInteractiveApp (real RetainedRender.init/step + ControlsElmish.routeFocusedKey)
mode=render-only

## What was driven

A focused **radio-group** authored the documented way — `RadioGroup.items [ "A"; "B"; "C" ]`,
`RadioGroup.selected "B"`, `RadioGroup.onChanged RadioChanged`, **no custom key handler** — was routed
through the **real** `routeFocusedKey` host seam (the same path `runInteractiveApp` wires), resolving
the focused control by its stable `RetainedId` with no hand-seeded identity map (mirrors the landed
Feature094 routing tests).

| Pressed key | Current selection | Dispatched message | Closed Nav payload |
|-------------|-------------------|--------------------|--------------------|
| ArrowDown | "B" (index 1) | `RadioChanged "C"` | `MovedSelection (2, Some "C")` |
| ArrowUp | "B" (index 1) | `RadioChanged "A"` | `MovedSelection (0, Some "A")` |

The dual-set is observed directly on a `"selected"`-binding capture: the dispatched `ControlEvent`
carries `Payload = Some "C"` **and** `Nav = Some (MovedSelection (2, Some "C"))` with
`Origin = Keyboard` (research R-2). The radio-group binds `onChanged`, so the resolver's
selected-then-changed fallback is what makes it operable.

## Responds, not just renders

A **pre-R5 / un-wired** build dispatches **nothing** on a focused radio-group arrow (the slider-only
`Navigate` arm filtered to a hardcoded float and never read the selection model), so it cannot produce
this artifact — the move is a genuine input→dispatch response, not a static render. Boundary cases are
verified no-ops with no spurious dispatch: last item + Next and first item + Previous dispatch nothing;
an empty group and an unresolvable current index dispatch nothing.

## Source

`tests/Elmish.Tests/Feature100NavigationTests.fs` — `100 US1 selection move at the host seam (SC-001)`.
At-rest rendered output is unchanged (navigation produces a `'msg`, no layout/render change); see
[real-image-evidence.md](./real-image-evidence.md).
