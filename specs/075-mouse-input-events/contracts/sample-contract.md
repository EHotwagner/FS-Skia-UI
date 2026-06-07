# Sample Contract: PointerInteractionGallery

The runnable sample (`samples/PointerInteractionGallery`, also shipped as a
`template/fragments/samples/` fragment for generated projects) is part of the
consumer contract: it is the demonstrable proof of SC-007 (no consumer-written
coordinate math / hit-testing) and the source of the readiness smoke log.

## What it must demonstrate

| User story | Demonstrated behavior |
|------------|----------------------|
| US1 (P1) hover | two side-by-side buttons; the one under the pointer renders a hover affordance; moving between them shows leave-A-then-enter-B; moving to empty space clears hover |
| US2 (P2) click | clicking a button dispatches its activation message exactly once; press-on-A / release-off-A dispatches nothing and clears the pressed look |
| US3 (P3) drag | a draggable element (e.g. slider thumb) follows the pointer past threshold; a tiny press+release is a click, not a drag |
| US4 (P3) secondary | right-clicking a control emits a distinct secondary outcome (sample shows a context indicator — NOT a framework-rendered menu) |
| US5 (P3) wheel | wheel over a scrollable region scrolls it by the signed delta; wheel over empty space does nothing |

## Wiring constraints (contractual)

- The sample's application code references **only** `ControlId`-level interaction
  messages routed from `PointerInteraction`; it performs no point-in-rect math and
  calls no `hitTestComputed` directly (SC-007).
- The only host-coupled code is the `ViewerEvent -> PointerSample` translation
  (per quickstart §2), kept small and isolated.
- Pointer state lives in the sample `Model`; `update` is pure; effects are lowered
  via `ControlsElmish.interpretPointerOutcome` + `AdapterCmd.toCmd`.
- Determinism: the sample's interaction logic must be reproducible via
  `Pointer.replay` for a scripted sequence (the deterministic test, not the GUI,
  is the authoritative evidence; the screenshot follows evidence-mode render-only
  honesty rules).

## Evidence produced

- `specs/075-mouse-input-events/readiness/sample-smoke/PointerInteractionGallery.txt`
- Screenshot (render-only evidence mode) if captured — classified per
  `fs-skia-evidence-mode`; GPU/Vulkan smoke failures distinguished from
  window-system/presentation setup, not assumed to be defects.
