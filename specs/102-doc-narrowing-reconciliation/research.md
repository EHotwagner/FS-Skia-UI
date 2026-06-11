# Phase 0 Research: Documented-Narrowing Reconciliation (R8)

**NEEDS CLARIFICATION resolved**: none existed — the spec pins all six sites and both
reconciliation-choice defaults. This document confirms each site against the working tree
(2026-06-11) and records the two decisions the spec defers to the plan (SC-006).

## The six reconciliations (decision / rationale / alternatives)

### FR-001 — R1 `deriveVisualState` order (roadmap §10.3 wording)

- **Decision**: Re-word roadmap §10.3 to describe the **two-function split** —
  `deriveVisualState` realizes only the **5-level runtime tail**
  (`Pressed > Selected > Focused > Hover > Normal`); the head semantic states and the
  consumer-out-ranks-derived arbitration live in `applyRuntimeVisualState`.
- **Rationale**: The `.fsi` (`ControlRuntime.fsi:88`, `:95`) already documents the split;
  §10.3 (roadmap `:686`, `:704`) describes a single full 8-level arbiter. Aligning prose to
  the already-honest `.fsi` closes the gap with no source change.
- **Verified**: `ControlRuntime.fs:203` `deriveVisualState` has exactly the 5 branches;
  `ControlRuntime.fs:229` `applyRuntimeVisualState` performs the consumer-wins arbitration
  and Normal-emits-nothing rest behavior.
- **Alternatives**: fold the full order behind one documented entry point and describe that
  (allowed by FR-001) — rejected: it would imply a source refactor R8 explicitly avoids.

### FR-002 — R1 dead derived `Selected` (source/surface)  ⟵ **RECORDED DECISION**

- **Decision**: **Annotate (FR-002b), do not remove.** Add a descriptive comment at the
  `Selected` branch (`ControlRuntime.fs:206-207`) stating it is forward-looking — the live
  host (`ControlsElmish`) never populates `model.Selection`, so only a *consumer-set*
  `Selected` fires today.
- **Rationale**: (1) Lowest risk and the spec's stated default (zero surface delta).
  (2) The public `deriveVisualState` is exercised by tests that may seed a `Selection`;
  annotation guarantees no test moves and no `EvidenceAudit` surprise. (3) Critically,
  **removal would also not change the `.fsi`** — the dead `elif` uses `model.Selection` but
  `model` stays in use by the other four branches, so no parameter is dropped; the spec's
  "removal changes a public signature" edge case does **not** apply. Removal therefore buys
  no surface-honesty win that annotation does not, while risking a test move. Annotate wins.
- **Alternatives**: FR-002(a) remove the branch — rejected for the test-move risk above
  despite being signature-safe; if a future maintainer wants the dead code gone, it remains
  a safe, baseline-free `.fs`-only edit.

### FR-003 — R2 cache wording (roadmap §10.4)

- **Decision**: Re-word roadmap §10.4 (`:754`, `:768`) to name the **shipped** cache — a
  computed **`Bounds`** cache keyed by structural **`LayoutNodeId`** — and remove the claim
  of "measured intrinsic size … keyed by retained identity". Cross-reference feature 101's
  recorded intrinsic-size-memo deferral (FR-008 of feature 101).
- **Rationale**: R2 (feature 097) shipped a bounds cache, not an intrinsic-size memo, and
  not keyed by `RetainedId`. The roadmap's own §11.5 audit row (`:1038`) already states this;
  §10.4 prose lags it.
- **Alternatives**: none — landing the memo is feature 101's deferral, out of R8 scope.

### FR-004 — R2 Yoga rationale (source comment)

- **Decision**: Extend the `src/Layout/Layout.fs:7-12` comment so it records the maintainer's
  **blast-radius approval rationale** ("blast-radius nil, Controls integer geometry
  unaffected") alongside the existing **INV-1 correctness** motive.
- **Rationale**: The comment justifies *why disabling point-scale rounding is correct* but
  not *why it was deemed safe to ship*; the approval rationale is the missing half.
- **Verified**: `Layout.fs:9-12` carries the INV-1 motive; no approval rationale present.
- **Alternatives**: none.

### FR-005 — R5 value-role surface (source/surface)  ⟵ **RECORDED DECISION**

- **Decision**: **Document (annotate), do not drop and do not enable routing.** Add a note in
  the `navIntentFor` value branch (`Focus.fs:127-129`, `Progress | Chart | Graph`) stating
  these roles are **classed-but-not-routed-by-default** because `Accessibility.defaultFor`
  gives them `Navigation = None` / non-focusable, so they never route on arrow keys today.
- **Rationale**: FR-008's no-behavior-change banner wins over the "give default `NavRange`s"
  alternative. Documenting keeps arrow-key routing for `Chart`/`Graph`/`Progress` exactly as
  today (not routed) → SC-004. Dropping the roles from the branch is the higher-touch option
  and risks a subtle classification change; an annotation is sufficient and lower-risk.
- **Alternatives**: (a) drop the roles from the value branch with a note — rejected as
  higher-touch with no behavior benefit; (b) **enable** routing by giving real default
  `NavRange`s — rejected: that moves a parity row, a behavior change explicitly **out of R8
  default scope** (would be a separate evidence-carrying feature).

### FR-006 — "segmented" role mention (roadmap + source)

- **Decision**: Correct every "segmented" selection-role mention (roadmap `:938`, `:1041`)
  to reflect that **no `Segmented` `AccessibilityRole` exists** — name the selection roles
  that do exist (radio-group, tab, menu, list) or drop the "segmented" item with a note.
- **Rationale**: The roadmap lists "segmented" among selection roles, but the
  `AccessibilityRole` enum has no such case; the prose implies a capability that is absent.
- **Verified**: no `Segmented` token in the `AccessibilityRole` definition (Accessibility
  surface); "segmented" appears only in roadmap prose.
- **Alternatives**: none.

### FR-007 — R3 preview-path annotation (source)

- **Decision**: Annotate the residual `control.Key |> Option.defaultValue control.Kind` at
  `src/Controls/Control.fs:1131` (inside `layoutNode`) as the **legacy 080 single-control
  preview/layout id**, distinct from the R3-unified `Key ?? path` dispatch/recovery id
  (feature 098), so a future reader does not mistake it for the divergence R3 removed.
- **Rationale**: R3 (feature 098) unified the dispatch/recovery id onto `Key ?? path`; the
  preview render path legitimately still uses `Key ?? Kind`. Without a note, a reader may
  "fix" a deliberate, scoped narrowing.
- **Verified**: `Control.fs:1131` is `layoutNode`'s id; the file already carries a contrasting
  comment at `:195` and `:1352` describing the R3 `Key ?? path` migration, so the annotation
  has a clear anchor. (The other `Option.defaultValue control.Kind` sites at `:1062`/`:1083`
  are *label-text* fallbacks (`control.Content |> …`), a different semantic — out of FR-007
  scope.)
- **Alternatives**: none.

## Cross-cutting findings

- **Routing**: any `src/Controls/**/*.fs` edit (even a comment) is expected to escalate
  `Route` to the **controls-public-surface** gate set (feature 101 precedent), despite no
  `.fsi` change. Run `./fake.sh build -t Route` first and run only what it prints,
  sequentially. The spec's "inner-loop `Dev`" prediction is treated as unconfirmed.
- **No `.fsi`/baseline move** under the recorded defaults (annotate, document) — confirmed by
  the signature finding for FR-002. No `PerPackageSurface.captureCurrent` recapture needed.
- **FR-010 hazard**: comments MUST be purely descriptive and MUST avoid bare gate-significant
  tokens / literal evidence filenames (e.g. `real-image-evidence.md`, status tokens) that
  could trip the window-visibility or diff-scan audits — a known gotcha from features 080/095.
- **Evidence**: `EvidenceAudit` must report **0 synthetic** and a verdict token in
  `readiness/evidence-audit.md`; existing R1/R2/R4/R5 suites stay green and unchanged.
