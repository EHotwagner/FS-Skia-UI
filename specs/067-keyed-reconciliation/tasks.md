# Tasks: Internal Keyed Reconciliation

**Feature branch**: `067-keyed-reconciliation`
**Spec**: `specs/067-keyed-reconciliation/spec.md`
**Plan**: `specs/067-keyed-reconciliation/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. **None is planned for this feature** —
the plan (Principle V) commits to fully-real evidence: `diff`/`apply` are real
and the round-trip property runs real generated trees. The classification must
be assigned during design, planning, clarification, or task generation;
implementation-time relabeling is forbidden.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the changed behavior was
actually exercised. Because feature 067 is **internal-only** (no public `.fsi`,
unreachable from a packed library or generated product — FR-002), the
"user-reachable surface" for these stories is the **in-assembly Expecto/FsCheck
test** that reaches `module internal Reconcile` via
`[<assembly: InternalsVisibleTo("Controls.Tests")>]`. A `[US*]` task is `[X]`
only when its failing-first test was committed red and then greened by real
`diff`/`apply` code — not by the algorithm compiling in isolation. If the test
is missing or stubbed, mark `[ ]` (work continues), never `[X]`.

Principle IV (MVU boundary) is **N/A** for this feature: the reconciler is a
pure, stateless diff over immutable IR — no `Model`/`Msg`/`Effect`/`update`.

## Success-criterion → assertion mapping

Each mechanically-testable success criterion is pinned to a concrete assertion so a
headline SC cannot be silently violated while gates stay green:

- **SC-001** (reorder ⇒ zero replaces) → T009 reorder test asserts zero `Replace` ops.
- **SC-002** (round-trip) → T020 FsCheck `apply prev (diff prev next).Patch ≡ next`, ≥1000 cases.
- **SC-003** (single-attr targeted update) → T012 asserts exactly one `AttrSet`, no other node touched.
- **SC-004** (determinism) → T020 determinism property: repeated `diff` is structurally identical.
- **SC-005** (zero public-surface delta) → T008 `PackageSurfaceCheck` baseline unchanged.
- **SC-006** (Route gates pass) → T022 runs the printed `controls-public-surface` gate set.
- **SC-007** (totality) → T018 edge tests + T020 generator confirm `diff` never throws.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T2]** — the whole feature is Tier 2 (internal change); the annotation is
  omitted per-task because every task matches the feature classification.

Every task has a matching entry in `tasks.deps.yml`; every line mirrors its
structured `skillist` as `[skillist: ...]`. FAKE-backed targets run sequentially
in deterministic order (`.fake` state is not concurrency-safe).

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold `specs/067-keyed-reconciliation/` and link spec, plan, data-model, and `contracts/reconcile.fsi`
- [X] T002 [P] [skillist: []] Create readiness placeholders discoverable before implementation: `readiness/typed-controls-front-door.md`, `readiness/package-surface-expectations.md`, `readiness/keyed-reconciliation.md` (FAKE-emitted gate logs land alongside them later)
- [X] T003 [P] [skillist: []] Record feature classification — Tier 2 internal, affected layer `src/Controls/**` (internal module), zero public-API impact (FR-002), MVU N/A (pure diff), and the three evidence obligations

---

## Phase 2: Foundation

- [X] T004 [P] [skillist: []] Expose the assembly-internal `module Reconcile` to `Controls.Tests` via an SDK `<InternalsVisibleTo Include="Controls.Tests" />` MSBuild item in `Controls.fsproj` (the SDK generates the assembly attribute at build time — a source `AssemblyInfo.fs` would lack the `.fsi` pair the surface-area gate requires, so the MSBuild item is used instead)
- [X] T005 [P] [skillist: fs-skia-ui-widgets] Draft `src/Controls/Reconcile.fsi` as `module internal Reconcile` — `FieldChange<'a>`, `AttrChange<'msg>`, `NodePatch<'msg>`, `UpdatePatch<'msg>`, `ChildOp<'msg>`, `ReconcileResult<'msg>`, and the `diff`/`apply` signatures, matching `contracts/reconcile.fsi`
- [X] T006 [skillist: fs-skia-ui-widgets] Add `src/Controls/Reconcile.fs` with total stub bodies (e.g. `diff` returns `Replace next`, `apply` returns `prev`) and insert `Reconcile.fsi`/`Reconcile.fs` after `Control.fs` in `Controls.fsproj`; confirm the `Controls.fsproj` reference set is unchanged — `Scene`, `Layout`, `KeyboardInput` only, **no `Fable.Elmish`** and no renderer dependency (FR-013)
- [X] T007 [P] [skillist: fsharp-build-orchestration] Edit `tests/Controls.Tests/Controls.Tests.fsproj` — add the `FsCheck` `<PackageReference>` (pinned 3.3.3, test-only) and register `ReconcileTests.fs` before `Program.fs`
- [X] T008 [skillist: fsharp-build-orchestration] Confirm `./fake.sh build -t Dev` builds the wire-up green and verify `PackageSurfaceCheck` shows a byte-for-byte unchanged public-surface baseline (FR-002 / SC-005)

**Checkpoint**: Internal module + test harness compile; public surface unchanged — story implementation may begin.

---

## Phase 3: User Story 1 — Keyed children survive reordering (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T009 [P] [US1] [skillist: fsharp-build-orchestration] Add failing-first reorder tests in `ReconcileTests.fs`: keyed `[a; b; c]` → `[c; a; b]` produces **zero** `Replace` ops, child ops are `ChildKeep`/`ChildMove` keyed to a/b/c, and a moved-but-unchanged node carries `NodePatch.Keep` (SC-001, US1 AC#1–2)

### Implementation

- [X] T010 [US1] [skillist: fsharp-graph-algorithms, fs-skia-ui-widgets] Implement keyed sibling matching in `diff` — build prev/next key buckets (keys-first), emit `ChildKeep`/`ChildMove` with next-relative indices, recurse into matched children — to green US1
- [X] T011 [US1] [skillist: []] Record US1's independent validation path (the in-assembly reorder test) in `readiness/keyed-reconciliation.md`

**Checkpoint**: Keyed reorder preserves identity with zero replacements.

---

## Phase 4: User Story 2 — Minimal patch for in-place changes (US2, P2)

### Tests First

- [X] T012 [P] [US2] [skillist: fsharp-build-orchestration] Add failing-first targeted-update tests: two same-key/same-kind nodes differing in one attribute yield exactly one `AttrSet` and touch no other node (SC-003); a content-only difference records exactly one `ContentChange`

### Implementation

- [X] T013 [US2] [skillist: fs-skia-ui-widgets] Implement `UpdatePatch` computation — attribute diff by `Name` sorted for determinism (FR-007), `ContentChange`/`AccessibilityChange` as `FieldChange` (FR-004), recurse via child ops (FR-005), and canonicalize an all-empty `Update` to `Keep` (identical-trees no-op) — to green US2

**Checkpoint**: A single field change yields a single targeted patch.

---

## Phase 5: User Story 3 — Insertion and removal detection (US3, P2)

### Tests First

- [X] T014 [P] [US3] [skillist: fsharp-build-orchestration] Add failing-first insert/remove tests: `[a; b]` → `[a; b; c]` yields exactly one `ChildInsert` for `c`; `[a; b; c]` → `[a; c]` yields exactly one `ChildRemove` for `b`, others kept

### Implementation

- [X] T015 [US3] [skillist: fsharp-graph-algorithms] Implement `ChildInsert` (next-only children) and `ChildRemove` (prev-only children, keyed by `ControlId option` + index) emission to green US3

**Checkpoint**: Added/removed children surface as explicit insert/remove ops.

---

## Phase 6: User Story 4 — Deterministic unkeyed fallback (US4, P3)

### Tests First

- [X] T016 [P] [US4] [skillist: fsharp-build-orchestration] Add failing-first fallback tests: two unkeyed sibling lists reconcile byte-for-byte identically on repeated runs; a mixed keyed/unkeyed list matches keyed nodes by key first, then the residual unkeyed nodes positionally (FR-010, US4 AC#1–2)

### Implementation

- [X] T017 [US4] [skillist: fsharp-graph-algorithms] Implement the positional fallback and the keys-first-then-residual-positional matching rule (FR-010) to green US4

**Checkpoint**: Unkeyed and mixed sibling lists diff deterministically by one documented rule.

---

## Phase 7: Integration & Polish

- [X] T018 [P] [skillist: fsharp-build-orchestration] Add edge-case tests: root `Kind` change → whole-subtree `Replace` (FR-006); duplicate keys in one sibling list → first-occurrence wins **and** a `KeyCollision` `Warning` diagnostic on `ReconcileResult.Diagnostics` (FR-011); empty→non-empty all-inserts, non-empty→empty all-removes, both-empty `Keep`; identical trees → `Keep` (SC-007 totality)
- [X] T019 [skillist: fs-skia-ui-widgets] Implement `apply` plus edge handling in `diff` — `Replace` on `Kind` mismatch, the first-occurrence `KeyCollision` diagnostic, empty-tree/identical canonicalization, and totality (never throws) — to green the edge tests
- [X] T020 [skillist: fsharp-graph-algorithms] Author the `Control<int>` FsCheck generator (bounded depth, keyed/unkeyed mix, duplicate-key cases) and the properties: round-trip `apply prev (diff prev next).Patch ≡ next` over ≥1000 cases (FR-008 / SC-002) and determinism `diff prev next = diff prev next` (SC-004); green
- [X] T021 [skillist: []] Author `readiness/keyed-reconciliation.md` (algorithm, keys-first matching rule, duplicate-key first-occurrence diagnostic, round-trip + determinism property results) and finalize `readiness/typed-controls-front-door.md` and `readiness/package-surface-expectations.md` recording the **zero** public-surface delta (SC-005)
- [X] T022 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Route` over the branch diff, confirm it prints the `controls-public-surface` escalation, then run the printed gates (`ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`, `PackageSurfaceCheck`, `FsiTranscripts`, `GeneratedProductCheck`) to green — SC-006. The `ControlsRenderingCheck` / `ControlsInteractionCheck` gates are the enforcer that render, layout, diagnostics, accessibility, and interaction behavior are unchanged (FR-012)
- [X] T023 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed feature directory/task count match, no cycles, no dangling refs, and no `[S*]` surprises
- [X] T024 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (no synthetic propagation; no `--accept-synthetic` override expected)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none planned — plan commits to fully-real `diff`/`apply` evidence)_ | | | | | | | | |
