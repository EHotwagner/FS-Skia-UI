# Phase 1 Data Model: Documented-Narrowing Reconciliation (R8)

**N/A — no data model.**

R8 is a documentation/honesty pass. It introduces **no new entity, field, relationship,
validation rule, or state transition**. Every change is either prose in the roadmap report
or a descriptive source comment/annotation; no type is added, removed, or modified, and no
runtime state is touched.

For completeness, the *existing* types the reconciliations describe (read for context only,
**not** changed by R8):

- `VisualState` — closed DU; `deriveVisualState` returns its 5-level runtime tail
  (`Pressed > Selected > Focused > Hover > Normal`). FR-001/FR-002 reconcile descriptions of
  how it is derived/arbitrated; the type and the functions are unchanged.
- `ControlRuntimeModel.Selection : ControlSelection option` — populated by consumer
  `SetSelection`, never by the live `ControlsElmish` host. FR-002 annotates this fact; the
  field is unchanged.
- `LayoutNode` / cached `Bounds` keyed by `LayoutNodeId` — the shipped R2 cache FR-003's
  §10.4 wording is reconciled to. Unchanged.
- `AccessibilityRole` — closed DU with **no** `Segmented` case (FR-006); `navIntentFor`'s
  `Progress | Chart | Graph` value branch (FR-005). Both annotated, neither changed.

No state machine, no transitions, no persistence. Nothing to model.
