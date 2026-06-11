# Derive precedence — property evidence (feature 096, T022, SC-004)

evidence-kind=derive-precedence
renderer-mode=DeterministicRenderOnly
status=pass

FsCheck properties over `deriveVisualState` + `applyRuntimeVisualState`, each run over **≥1000**
generated `(ControlRuntimeModel, ControlId, consumer-state)` combinations (real generated inputs, not
canned fixtures).

- **Totality + determinism** — `deriveVisualState m id = deriveVisualState m id` for every generated
  `(m, id)`; the projection is defined for every `ControlId` and never throws. (1000 cases passed.)
- **Closed fixed order** — the bridge realizes the closed order
  `Disabled > Validation > Loading > Pressed > Selected > Focused > Hover > Normal`: a consumer-set
  non-`Normal` state (the head states `Disabled`/`Validation`/`Loading` are *only* consumer-set, never
  derived) is preserved in **100%** of cases; a consumer-`Normal` (unset) control takes the derived
  runtime tail `Pressed > Selected > Focused > Hover > Normal`. (1000 cases passed.)
- **Determinism of the bridge** — `applyRuntimeVisualState` produces an identical lowered tree for
  identical inputs. (1000 cases passed.)

The runtime-derivable tail order itself (`Pressed > Selected > Focused > Hover > Normal`) is pinned by
the unit precedence test (T010), which peels each higher state off a single model and observes the
next-ranked one.

result=pass — consumer non-`Normal` preserved 100%; the fixed order holds for every generated combo.
authoritative-test=Feature096RuntimeBridgeTests/Feature 096 bridge properties (FsCheck, SC-004)
