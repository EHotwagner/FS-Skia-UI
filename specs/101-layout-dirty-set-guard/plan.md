# Implementation Plan: Layout Dirty-Set Anti-Drift Guard (R7)

**Branch**: `101-layout-dirty-set-guard` | **Date**: 2026-06-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/101-layout-dirty-set-guard/spec.md`

## Summary

R7 converts a correct-but-unguarded invariant in the feature-097 (R2) incremental
layout path into an *enforced* one. Today `toLayout`
(`src/Controls/Control.fs:1209`) derives geometry from exactly three attribute
names — `width`, `height` (via `nodeWidth`/`nodeHeight`/`hasAttr`) and
`orientation` (via `orientationOf`) — and the incremental dirty classifier
`layoutDirtySet` (`src/Controls/RetainedRender.fs:244`) keys on a *separate*
hand-maintained literal `layoutAffectingAttrNames = {width;height;orientation}`
(`src/Controls/Control.fs:1207`). The two agree only by maintenance discipline;
a source comment falsely claims they are single-sourced. If a future feature
makes `toLayout` read a new attribute name, the classifier would keep reusing
stale cached bounds — a silent wrong-layout bug.

**Technical approach.** Keep the runtime literal (the hot classifier needs a
cheap `Set` lookup) but make it *impossible to ship drift* via a deterministic,
in-process **behavioral-probe equality gate** plus light name-token
single-sourcing:

1. A pure **drift report** `layoutDriftReport (discovered) (covered) :
   DriftFinding list` (`DriftFinding = Uncovered of string | OverBroad of
   string`) with a human-legible formatter (FR-007). It is the unit under test
   for both negative directions (FR-002/FR-003), fed simulated sets — no source
   parsing, no `toLayout` mutation.
2. A **behavioral probe** that discovers the *actual* layout-driving names by
   toggling each candidate attribute on representative fixtures and observing
   whether the real `evaluateLayout` root `LayoutNode` changes. The load-bearing
   gate asserts `layoutDriftReport (probe()) layoutAffectingAttrNames = []`
   (FR-001 "gated against" mechanism; SC-001/SC-002).
3. **Category honoring** asserted independently on `layoutDirtySet` directly: an
   `AttrSet` tagged `AttrCategory.Layout` (or an `AttrRemoved` of a prev-node
   Layout-category attr) dirties even when its name is absent from the name set,
   and the name-set equality never demands a category-only attr appear in the
   name list (FR-004; the spec's interacting-requirements resolution).
4. **R2 preservation** is proven by the *existing* feature-097 evidence, which
   R7 leaves byte-identical (FR-005/FR-006, SC-003/SC-004).

The change lands as **ordinary Controls Expecto tests** (no new FAKE/Governance
gate) with **no new public or internal `.fsi` surface**, so it routes
**inner-loop** (`Dev`). The feature's own evidence obligations additionally
produce `EvidenceGraph` + `EvidenceAudit` artifacts.

The optional intrinsic-size memo (FR-008) is **deferred** with the decision
recorded here and in [research.md](./research.md); §10.4 wording reconciliation
is delegated to R8.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: FS.Skia.UI.Controls (internal), FS.Skia.UI.Layout
(read-only, for `LayoutNode` structural equality); Expecto + FsCheck for tests.
No new package dependency.
**Testing**: Expecto in `tests/Controls.Tests` (reaches `ControlInternals` /
`layoutDirtySet` via the existing `InternalsVisibleTo "Controls.Tests"`); the
existing `tests/Layout.Tests/Feature097IncrementalTests.fs` ≥1000-case
incremental-≡-full property is re-run unchanged. Deterministic, in-process
(`Check.One`-style), no wall-clock / external process.
**Target Platform**: Windows and Linux (unchanged; no platform-specific code).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change Classification — Tier 2 (internal change).** No public API surface
change; refactors/hardens an internal invariant with zero behavioral delta. Per
the constitution Tier-2 rule, `.fsi` and baselines remain untouched (the new
helpers stay private to the `.fs`/test code).

**Principle II note (access modifiers in `.fs`).** The US2 name-token constants
are written `let [<Literal>] private AttrWidth = "width"` etc. Principle II says
`.fs` files MUST NOT carry `private`/`internal`/`public` on top-level bindings
because visibility belongs in the `.fsi`. The accepted reading here is that the
prohibition governs bindings that would otherwise leak to a **public** `.fsi`
surface: these constants live inside the **internal** `ControlInternals` module
(reached only via `InternalsVisibleTo`, with no public `.fsi` entry to omit them
from), exactly mirroring the established local convention in the same module
(`let rec private toLayout`, `Control.fs:1209`). `private` is the only mechanism
that hides a helper that has no public-surface declaration to be absent from, so
this is consistent with — not a violation of — Principle II's intent. The T015
per-package internal baseline confirms zero surface delta. Maintainer-confirmed
as the intended interpretation.

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**`, sample, command-surface, or
  Spec Kit asset change; this is a `src/Controls/**` + `tests/Controls.Tests/**`
  internal change. `.template.config/template.json` is untouched.
- **Dependency impact**: N/A — no new package; `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` are unaffected (FsCheck/Expecto
  already referenced by `Controls.Tests`).
- **Command-surface impact**: No new build target. Run `./fake.sh build -t
  Route` first; on its own this change routes **inner-loop** → `Dev` only. The
  enforcement is an ordinary Controls Expecto test (runs under `Dev`), **not** a
  new FAKE/Governance gate, so `AgentValidation.knownGates` and
  `validation.contract.yml` (`TargetMetadataDrift`) need **no** edit and the
  route does **not** escalate on that account. If `Route` unexpectedly escalates
  (e.g. a per-package internal baseline shift), run exactly the gates it prints,
  sequentially. The feature's evidence obligations additionally run
  `EvidenceGraph` then `EvidenceAudit` (FAKE-backed, run sequentially, never
  concurrently). Example order if escalated: `Dev` → `EvidenceGraph` →
  `EvidenceAudit`.
- **Generated project impact**: N/A — no change to default/minimal generated
  contents, selected-Controls guidance, local skills, validation logs, or
  generated `Dev` behavior. R7 is framework-internal.
- **Evidence paths**: feature artifacts under
  `specs/101-layout-dirty-set-guard/` (spec, plan, research, data-model,
  contracts, quickstart, tasks, tasks.deps.yml); Expecto run output for the new
  `tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs`; the re-run
  `tests/Layout.Tests/Feature097IncrementalTests.fs` ≥1000-case property; the
  `WorkReductionRecord` content-only-edit assertions in
  `tests/Controls.Tests/Feature097WiringTests.fs` (cited unchanged);
  `readiness/evidence-graph.*` and `readiness/evidence-audit.md` (with a verdict
  token) from `EvidenceGraph` / `EvidenceAudit`.
- **`.fsi` / contract impact**: **No `.fsi` change.** The drift report, the
  probe, and any shared name-token `[<Literal>]` constants stay **private** to
  `src/Controls/Control.fs` (constants) and to the test file (report + probe),
  consuming only the already-published internal surface
  (`layoutAffectingAttrNames`, `evaluateLayout`, `layoutDirtySet`). No public API
  doc, surface baseline, sample contract, or compatibility note changes. If an
  unintended internal-surface move is detected, recapture via
  `PerPackageSurface.captureCurrent` and note it — but the design's intent is
  zero surface delta (keeps Tier-2 + inner-loop).
- **MVU/effect boundary**: N/A — no stateful or I/O-bearing workflow. The
  classifier and the probe are pure functions of their inputs; no `Model`/`Msg`/
  `Effect`/interpreter is introduced. (Constitution Principle IV does not apply.)
- **Synthetic evidence**: **None — zero `[S]`/`[SEH]`.** The negative-drift tests
  feed literal `Set` values to the *real* pure `layoutDriftReport` (ordinary
  unit testing of a pure function over its natural domain, not a mock/fake/stub
  of any real dependency). The load-bearing positive gate exercises the **real**
  `evaluateLayout` and the **real** literal via the probe. No mock, fake,
  placeholder, canned response, or in-memory substitute for a real dependency
  exists, so Principle V disclosure is not triggered and `EvidenceAudit` must
  report no synthetic work.
- **Test evidence**: Failing-first semantic tests — (a) negative under-coverage:
  `layoutDriftReport {width;height;padding} {width;height}` returns
  `[Uncovered "padding"]` and the formatter names `padding`; (b) negative
  over-coverage: `layoutDriftReport {width} {width;orientation}` returns
  `[OverBroad "orientation"]`; (c) the positive gate: probe-discovered set
  equals `layoutAffectingAttrNames` (passes today, fails the instant `toLayout`
  reads an uncovered corpus name); (d) FR-004 category-honoring units on
  `layoutDirtySet`; (e) FR-005/FR-006 preserved by re-running the existing R2
  property + `WorkReductionRecord` tests unchanged. All run under `Dev`.
- **Observability**: FR-007 — the formatter emits a human-legible message naming
  each drifting attribute and its direction (`un-covered layout input: 'padding'`
  / `over-broad classifier entry: 'orientation'`), pointing the contributor at
  the fix rather than a stale-bounds symptom. The probe's corpus-bounded
  coverage boundary (it discovers only names reachable in the corpus) is
  documented at the test site and in [research.md](./research.md) so the
  guarantee's scope is explicit, not implied; the corpus draws from a
  **concrete, traceable** source — the `Attr` builder vocabulary +
  attribute-name literals in `src/Controls/Control.fs`, unioned with
  `layoutAffectingAttrNames` and explicit non-layout names (research D2) — not a
  hand-curated "representative" list.
- **Deferred scope**: The optional intrinsic-size memo (FR-008) is **deferred**
  — no profiling shows the boundary re-measure is hot; R7 is anti-drift only.
  Decision recorded in [research.md](./research.md) and surfaced in tasks for
  R8's §10.4 wording reconciliation. Also out: R6 visual-state cross-fade, R8
  doc-narrowing reconciliations, collection virtualization, and any expansion of
  the layout-driving attribute set.

**Post-design re-check**: PASS — Phase 1 introduces no public/internal `.fsi`
surface, no new dependency, no MVU boundary, and no synthetic evidence; Tier-2
classification and the inner-loop route hold. No constitution gate is violated.

## Project Structure

```
specs/101-layout-dirty-set-guard/
  spec.md            # the feature spec (input)
  plan.md            # this file
  research.md        # mechanism decision + FR-008 deferral record
  data-model.md      # DriftFinding, the probe seam, fixture/corpus shapes
  contracts/
    layout-drift-guard.md   # the test-facing contracts (pure report + probe + category units)
  quickstart.md      # how to run/extend the guard

src/Controls/
  Control.fs         # (US2) private [<Literal>] name-token constants shared by
                     #   nodeWidth/nodeHeight/orientationOf and layoutAffectingAttrNames
                     #   — name single-sourcing; NO .fsi change, NO behavior change
  RetainedRender.fs  # unchanged behavior; classifier comment corrected to stop
                     #   claiming false single-sourcing (points to the gate)

tests/Controls.Tests/
  Feature101LayoutDriftGuardTests.fs   # NEW: drift report negatives, the probe gate,
                                       #   FR-004 category-honoring units
  Feature097WiringTests.fs             # cited UNCHANGED (WorkReduction / SC-003)

tests/Layout.Tests/
  Feature097IncrementalTests.fs        # cited UNCHANGED (≥1000-case incremental-≡-full)
```

## Complexity Tracking

No constitution-justification-bearing complexity is introduced. The probe uses
ordinary structural equality over `LayoutNode` records and `Set` operations; no
custom operators, SRTP, reflection, non-trivial computation expressions, type
providers, or non-simple active patterns. `DriftFinding` is a plain two-case DU.
The one documented limitation — corpus-bounded discovery — is recorded as
observability, not hidden.
