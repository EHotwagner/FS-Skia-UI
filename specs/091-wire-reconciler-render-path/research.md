# Phase 0 Research: Wiring the Parked Keyed Reconciler onto the Render Path (091 / E2)

This feature is **wiring + invariant-preservation**, not a new algorithm. The diff
(`module internal Reconcile`, feature 067) is pure/total/deterministic and round-trip property-tested.
Research here resolves *how* to put it on the live render path while keeping every invariant and the pure
MVU consumer surface. All findings are grounded against current source.

## Seam-by-seam findings (grounded against source)

### The diff already exists, internal, total, and round-trippable
`src/Controls/Reconcile.fsi` (`module internal Reconcile`): `diff : prev -> next -> ReconcileResult<'msg>`
and `apply : prev -> NodePatch<'msg> -> Control<'msg>`. `ReconcileResult` carries `Patch` +
`Diagnostics : ControlDiagnostic list`; duplicate sibling keys become a `KeyCollision` diagnostic, never a
throw (`Reconcile.fs:100`, `Types.fsi:136`). Children match key-first then positional; a `Kind` mismatch on
a matched pair is a whole-subtree `Replace`. `apply prev (diff prev next).Patch` is structurally equal to
`next` up to attribute ordering (canonicalized by `Name`). Tests reach it via
`[<assembly: InternalsVisibleTo("Controls.Tests")>]` (`Controls.fsproj`).

### The render path rebuilds the whole tree each frame
`Control.render` (`Control.fs:996`) is single-control preview (empty `Bounds`). `Control.renderTree`
(`Control.fs:1014`) runs real recursive Yoga layout at the output `Size` and paints every node at computed
bounds, returning `ControlRenderResult` (`Types.fsi:285`: `Scene`, `Layout`, `Bounds : (ControlId * Rect)
list`, `Diagnostics`, `EventBindings`, `NodeCount`). Per-node id is **structural**: `let id = c.Key |>
Option.defaultValue path` (`Control.fs:1052`). It rebuilds from the `Control<'msg>` tree on every call;
`Reconcile.diff`/`apply` are never called from it.

### The host loops rebuild on every message
- `runInteractiveApp` (`ControlsElmish.fs:331`) holds per-frame mutable refs already:
  `pointerState`, `focusedText : ControlId option ref`, `textModels : Map<ControlId, TextInputModel> ref`,
  `latest : (Size * 'model) option ref`. It calls `Control.renderTree host.Theme size (host.View size
  model)` fresh on each interaction (`:349/376/391`). **There is no retained tree in the adapter** — the
  tree is rebuilt; only the keyed UI-state maps persist.
- `SkiaViewer.dispatchHostMsg` (`SkiaViewer.fs:2364`): `let next, effects = host.Update msg currentModel;
  currentModel <- next; currentScene <- host.View currentModel; interpretEffects effects` — an
  unconditional full re-render. Size-aware variant at `:2437` (`host.View currentSize currentModel`).

### Why identity is lost today (the real defect this fixes)
Because the id is path-derived when a control is unkeyed, an unrelated model change that **shifts a
control's position** changes its `ControlId`. The host's per-control state is keyed by that id
(`focusedText`, `textModels`, and `ControlRuntime.FocusedControl` at `ControlRuntime.fsi:42`), so the
lookup misses after the shift and focus/text-state "resets" — the ControlsShowcase2 symptom. The
reconciler's match is exactly the missing piece: it identifies the next-frame node that **is the same** as a
prev-frame node across a positional shift, giving identity-bearing state a stable hook.

### Animation is Scene-level today
Feature 073 `src/Scene/Animation.fsi`: `Tween<'a>`, `Animation`, `AnimationState<'a>` (`Current`/`Start`/
`Target`/`Elapsed`/`Duration`/`Easing`/`Interp`), `applyAt : TimeSpan -> Animation -> Scene -> SceneNode`
with an identity-at-rest rule (settled animation = static render). The clock is an explicit `TimeSpan`
sampled per frame from the host `Tick`. A per-control animation clock is just an `AnimationState`/`Elapsed`
keyed by the retained identity — no new public animation API is needed to prove FR-003 survival.

### The 067 property suite to promote
`tests/Controls.Tests/ReconcileTests.fs`: round-trip (`:394`, ≥1,000 cases via `Gen067.pair`), determinism
(`:403`), plus identity-at-rest / minimal-patch / mixed-list / insert-remove / duplicate-key cases.
Promoting these to the wired path means re-running the same generators but asserting against the **wired
render output** (retained-apply ≡ full rebuild of `next`), not just `diff`/`apply` in isolation.

### The 090 responds-proof primitive to reuse
090 added a capturable before/after render-diff proof (`readiness/responds-proof/{before,after}.png` +
verdict) that an inert app fails. 091 reuses the **same mechanism** for the survives-proof: render → set
focus / start a per-control animation → dispatch an **unrelated** model update → re-render → assert focus is
unchanged and the animation clock advanced (did not reset), with a rebuild-every-frame baseline failing.

## Open design decisions (resolved)

### D1 — Retained-structure shape & home
**Decision.** A new `module internal RetainedRender` in `src/Controls/RetainedRender.fs(i)` defines a
framework-internal structure pairing each `Control<'msg>` node with (a) the previous lowered `Control<'msg>`
(the diff's `prev`), (b) its cached render fragment (`LayoutNode` + Scene fragment + computed `Rect`), and
(c) the node's **stable identity**. It lives in the host loop's existing mutable-ref state — a
`retained : RetainedRender<'msg> option ref` alongside `pointerState`/`textModels` in `runInteractiveApp`,
and the `currentScene`/`currentModel` refs in `dispatchHostMsg`. **Rationale:** the refs are already the
interpreter-edge home for per-frame state (constitution III — mutation at the edge); a separate internal
module keeps the structure off the public surface (`module internal`, InternalsVisibleTo for tests) and out
of `Control.fs`'s already-large render body. **Alternatives rejected:** threading the retained tree through
the consumer model (leaks framework internals into `'model`, breaks FR-008 "no rewrite"); a global mutable
cache (breaks determinism across hosts/processes, SC-005).

### D2 — Identity re-keying of focus/text state
**Decision.** The diff's match becomes the identity source: for a `ChildKeep`/`Update` (same control across
frames) the retained identity is carried over; for `Replace`/fresh `ChildInsert` a new identity is minted.
Re-key `focusedText`, `textModels`, and the per-control animation clock to this **stable retained
identity** rather than the raw path-derived `ControlId`, so they survive a positional shift (FR-003). A
`Replace` (e.g. `Kind` changed) drops the old identity → its state is **not** spuriously retained (SC-001
negative case). **Rationale:** this is precisely the defect mechanism above; the diff already computes the
match, so re-keying is a lookup remap, not new logic. **Alternatives rejected:** requiring explicit `Key`
on every control (pushes work onto consumers, contradicts "key-first **then positional**"); persisting state
by raw path id (the current broken behavior).

### D3 — Partial-render reuse mechanism & golden-parity guarantee
**Decision.** Drive re-render from the patch: `Keep`/`ChildKeep` subtrees **reuse** the retained fragment
(no re-measure/re-paint); `Update` recomputes the node's own measure/paint and recurses into its
`Children` ops; `Replace`/`ChildInsert` build fresh via the existing `renderTree` path; `ChildRemove`/
`ChildMove` reorder cached fragments. The wired output MUST be **byte-for-byte equal to a full rebuild of
`next`** — enforced by a golden-diff parity test on every test scene and the round-trip property
(`retained-apply ≡ rebuild(next)`). **Correctness-wins fallback (spec):** if any parity/round-trip check
would fail, the path produces the full-rebuild-equivalent result instead of the faster-but-divergent one;
efficiency is measured (work-reduction record) but never at the cost of FR-005/FR-006. **Rationale:** layout
is deterministic in the node + its inputs, so an unchanged subtree's fragment is provably reusable; the
golden gate makes any divergence a hard failure rather than a silent visual bug. **Alternatives rejected:**
diffing the painted Scene directly (loses the `Control`-level identity the focus/animation hooks need);
caching by structural path id (invalidates on every positional shift — the bug).

### D4 — Per-control animation clock for the survives-proof
**Decision.** Attach an `AnimationState`/`Elapsed` (`TimeSpan`) to the retained identity in the host loop,
advanced by the existing `host.Tick` delta, sampled via the Scene-level `Animation.applyAt`. Its **only**
job here is to prove FR-003 survival: across an unrelated re-render the clock for a kept control continues
(does not reset). **Rationale:** reuses the shipped 073 Scene animation entirely; no new public animation
API. **Alternatives rejected:** building the full animation↔identity retargeting now (explicitly sequenced
after E2, roadmap §8 / spec Out-of-Scope); a new control-level animation type (E-scope creep, baseline
churn).

### D5 — Public seam vs internal, and default-vs-flag
**Decision.** Default to **zero public-surface delta**: `Reconcile` and `RetainedRender` stay `module
internal`; the wired retained path **replaces** the internal render path (not a per-call opt-in), keeping
the pure MVU `view` contract unchanged (roadmap §6.4, spec Assumptions). The only public-facing changes are
**honest `.fsi` doc comments** noting the behavioral change (no signature change). A small additive public
seam is introduced **only if** implementation experience warrants it: (a) a `ViewerDiagnosticsOptions`
work-metric field to let a consumer observe the per-frame reduction from outside, or (b) a safety flag to
force full-rebuild. If either lands it is additive and per-package + `docs/api-surface` baselines are
recaptured then. **Rationale:** 067 SC-005 / the `module internal SceneRenderer` precedent — wiring does not
require promotion; minimal surface = minimal baseline risk. **Alternatives rejected:** promoting `Reconcile`
public "to document it" (the skill forbids this); gating behind a default-off flag (most consumers would
never get the unlock; contradicts FR-008 "no rewrite to benefit").

## Cross-cutting resolutions (from the spec's conflicting-requirements notes)
- **Partial-update efficiency (FR-004) vs output fidelity (FR-005) vs determinism (FR-006):** correctness
  wins (D3 fallback). Output is byte-equal to a full rebuild of `next`; efficiency is measured but never
  divergent.
- **Internal mutation (FR-002) vs determinism/identity-at-rest (FR-006):** mutation is confined to the
  internal `RetainedRender` structure at the interpreter edge; the consumer surface stays pure MVU; the
  gates assert observable round-trip/determinism/no-spurious-re-render, not the absence of internal
  mutation (the reason 067 was written pure/total).

## Persistent-problem protocol
Per `[[fs-skia-reconciliation]]`: if the diff/round-trip will not reconcile after reasonable in-repo
attempts, consult **official online docs first** (F#/.NET docs; React's reconciliation/keys notes are the
canonical keyed-VDOM prior art), then community sources; record findings in
`specs/091-wire-reconciler-render-path/feedback/` and durable lessons in the skill's Sources line. Offline,
record "research blocked — <why>" rather than hard-failing the phase.

**Output**: all NEEDS CLARIFICATION resolved (D1–D5); no unresolved unknowns remain for Phase 1.
