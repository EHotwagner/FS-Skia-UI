# Contract: Behavior / output parity (US4)

The banner constraint. Every observable artifact is byte-/structurally identical
before and after the change.

## P1 — Lowered `Control<'msg>` parity (SC-006)

For every widget whose lowering used a consolidated helper (`withKeyOpt`,
`onString`, `onStringList`, `a11y`, `intentToString`) or a consolidated
`onChanged` shape, the lowered `Control<'msg>` MUST be structurally identical to
the pre-change output.

- `Control<'msg>` has **no** structural equality (it embeds `AttrValue<'msg>`
  whose `EventValue`/`MessageValue`/`UntypedValue` cases carry functions/`obj`).
  Compare via `sprintf "%A"` of the lowered control — the established pattern
  (features 096/097/101).
- The assertion is authored failing-first against a captured pre-change baseline:
  it would go red if any consolidation perturbed attribute order, event-kind
  strings, key application, or slot lowering.

## P2 — Serialized string boundaries unchanged (FR-009)

- Scene evidence `BlockedStage`/`DiagnosticCategory` text: still `"scene"` /
  `"renderer"`, byte-identical.
- `RendererMode` output/serialized fields: byte-identical strings; only the
  internal dispatch `match` is typed.
- `SlotFillsValue` carrier payloads: still `(string, Control)` pairs with the
  same `"leading"/"trailing"/"header"/"footer"` strings on the public side.

## P3 — Existing suites green and unchanged (SC-005)

- Controls + Controls.Elmish Expecto suites pass with **no test edits** forced by
  behavior change. (A test may reference a renamed-away local helper only if the
  helper was test-visible; none of the consolidated helpers are — they are
  file-private/internal.)
- Parity/golden evidence: **no row moves**.
- Determinism properties (reconcile totality/identity-at-rest, layout byte-
  identity): unperturbed.

## P4 — Evidence chain (spec Evidence obligations)

- The routed gate set green (`Route` authoritative).
- `EvidenceGraph` + `EvidenceAudit` produce a verdict token with **0 synthetic**
  tasks and no diff-scan blockers.
- `git diff -- 'src/**/*.fsi'` empty (zero public-surface delta) — unless the
  optional FR-012 expansion is elected, which this plan does **not** elect.
