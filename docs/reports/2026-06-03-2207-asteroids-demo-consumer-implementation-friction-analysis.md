---
title: Consumer Implementation Friction Analysis — Asteroids Arcade Demo
category: Design history
categoryindex: 90
---

# Consumer Implementation Friction Analysis — Asteroids Arcade Demo

- **Date:** 2026-06-03T20:07Z
- **Author:** Claude Code (implementation agent)
- **Consumer project:** `AsteroidsDemo3` (generated FS.Skia.UI app), feature `001-asteroids-demo`
- **Framework packages:** `FS.Skia.UI.*` `0.1.62-preview.1`
- **Toolchain:** .NET 10.0.300, F#, Expecto 10.2.2, FAKE-driven `build.fsx`
- **Scope of work analyzed:** full `/speckit-implement` run — transforming the
  generated token-grid scaffold into a playable Asteroids game (33 spec-kit tasks,
  6 source files rewritten, 28 tests, full `Verify` + evidence graph/audit).

This report documents friction encountered as a **consumer** of the FS.Skia.UI
framework, its skills, and its Spec Kit governance during a real feature
implementation. The feature itself completed successfully (all gates green); this
is a retrospective on process gaps that cost time or weakened the safety net, with
root-cause analysis and recommendations for framework/governance/skill maintainers.

Severity legend: **P1** = blocks or silently undermines correctness/governance;
**P2** = significant time cost or sharp edge; **P3** = papercut / docs.

---

## Executive summary

The implementation succeeded, but four findings stand out:

1. **(P1) The merge-gate evidence audit validated the wrong feature.**
   `EvidenceGraph`/`EvidenceAudit` reported `feature-directory=specs/generated-evidence-workflow`
   (`tasks=1`), never `specs/001-asteroids-demo` (33 tasks). The gate returned
   `verdict=PASS` without auditing the feature being shipped or the `[X]` status
   changes made during implementation. A green gate that does not inspect the work
   it gates is a false assurance.

2. **(P1) The documented authoritative API surface does not exist in the generated
   project.** Every capability skill points at `docs/api-surface/<Pkg>/<Pkg>.fsi`
   as the contract source of truth. That directory is absent. API discovery
   required DLL reflection — exactly what the skills say is unnecessary.

3. **(P2) Type/case-name collisions between consumer code and framework types make
   F# inference fragile**, producing cascades of misleading errors.

4. **(P2) Governance and behavioral tests are entangled in a single compilation
   unit**, forcing an all-or-nothing test rewrite with no incremental migration.

The throughline: **contract/skill documentation lags the shipped binaries, and the
evidence gate points at the wrong directory** — so the governance safety net is
weaker than its green checkmarks imply.

---

## Methodology / how the friction was surfaced

Normal implementation flow: read `spec.md`/`plan.md`/`data-model.md`/`contracts/`,
load the declared capability skills, write the six source files, write tests, then
run the canonical sequence `./fake.sh build -t Dev` → `-t Test` → `-t Verify`,
followed by `-t EvidenceGraph` and `-t EvidenceAudit`. Friction points are recorded
below with the concrete artifact (error text, report field, file:symbol) that
exposed them.

---

## Findings

### F1 (P1) — Evidence audit/graph target the wrong feature directory

**Observed.** After marking `specs/001-asteroids-demo/tasks.md` tasks `[X]` and
re-running the gates:

```
# readiness/evidence-graph.md
feature-directory=/home/developer/projects/AsteroidsDemo3/specs/generated-evidence-workflow
status=ok
- verdict=ok
- tasks=1

# readiness/evidence-audit.md
feature-directory=/home/developer/projects/AsteroidsDemo3/specs/generated-evidence-workflow
- verdict=PASS
- real-tasks=1
- unaccepted-synthetic-tasks=0
- total-blockers=0
```

The active feature has **33** tasks in `specs/001-asteroids-demo/`; the audit saw
**1** task in a different directory (`specs/generated-evidence-workflow`).

**Impact.**
- The synthetic-propagation (`[S*]`) computation and diff-scan that constitute the
  merge gate never examined this feature's task graph, dependency topology, or the
  status transitions made during implementation.
- The speckit-implement workflow instruction "re-run `speckit.evidence.graph`
  after every status change … recomputes `[S*]` propagation" is a no-op for the
  feature under development.
- `verdict=PASS` is technically true but provides **no assurance about
  001-asteroids-demo**. A reviewer trusting the green gate would be misled.

**Likely root cause.** The generated `build.fsx` evidence runners resolve a fixed /
default `featureDir` (the framework's own `generated-evidence-workflow`) rather
than discovering the active feature branch (`001-asteroids-demo`) or accepting it
as a parameter. See `build.fsx` `runGeneratedEvidenceGraph` / `runGeneratedEvidenceAudit`
and the `featureDir` they pass to `writeGeneratedEvidenceReport`.

**Recommendation.** Resolve the feature directory from the current git branch (or a
`--feature` argument / `SPECKIT_FEATURE` env var), and **fail** when the resolved
feature has zero or mismatched tasks rather than silently auditing a placeholder.
Echo the audited feature directory prominently so a wrong target is obvious.

---

### F2 (P1) — Documented API surface (`docs/api-surface/*.fsi`) is missing

**Observed.** All five consumed skills state, verbatim, e.g.:

> "The signatures you consume are bundled with this asteroidsdemo3 at
> `docs/api-surface/Scene/Scene.fsi`. Read them to confirm any union case's exact
> field order locally — **no DLL reflection needed.**"

The generated project's `docs/` contains only `effects-boundary.md` and
`product.md`; there is no `docs/api-surface/` tree.

**Impact.** The authoritative contract source the skills mandate does not exist, so
exact shapes (DU case arity/field order, record fields, module helper signatures)
could not be confirmed as instructed. I recovered them by loading the built DLLs
and reflecting with `FSharp.Reflection` (`FSharpType.GetUnionCases` /
`GetRecordFields` / static-method enumeration) — the precise activity the skills
say is unnecessary. This was the largest single time cost of the implementation.

Concrete shapes that had to be reflected because they were undocumented in-project:
`SceneNode` (20 cases incl. `Path of PathSpec * Paint`, `Circle of Point*float*Color`,
`Text of (float*float)*string*Color`), `PathCommand`, `PathFillType` (`Winding`/`EvenOdd`),
`Color`/`Point`/`Rect`/`Size`, `ViewerKey`, `ViewerEffect` (incl. `RenderScene`,
`CloseWindow`), `AdapterEffect<'msg>` (the real type behind `AdapterCommand<'msg>`),
and the `SceneModule`/`PaintModule` helper functions.

**Recommendation.** Either (a) actually emit `docs/api-surface/*.fsi` into generated
projects as the skills promise, or (b) ship the package `.fsi` files inside the
NuGet packages and update the skills to point at the NuGet content / `ref/` assembly.
Until then the skills' "no reflection needed" claim is false for consumers.

---

### F3 (P2) — Structural type collisions break F# inference

**Observed.** The game model defines `Vec2 = { X: float; Y: float }`, structurally
identical to framework `Point` and overlapping `Rect`. F#'s record-label resolution
picks the most-recently-`open`ed type, producing cascades such as:

```
View.fs(75,12): error FS0001: This expression was expected to have type 'Point' but here has type 'Vec2'
LayoutEvidence.fs(27,24): error FS0039: The type 'Vec2' does not define a field 'Width'   # wanted Rect
Model.fs(304,86): error FS0039: The type 'Bullet' does not define a field 'Radius'        # wanted Asteroid
Tests.fs(170,72): error FS0001: This expression was expected to have type 'Vec2' but here has type 'Point'
```

The `Bullet`/`Asteroid` confusion arose because both records have a `Position: Vec2`
field, so an unannotated `processBullets`/`resolveShipHit` parameter inferred the
wrong record. The `Vec2`/`Point` confusion in tests flipped depending on `open`
order of `FS.Skia.UI.Scene` vs the model module.

**Resolution applied.** Explicit parameter annotations (`(asteroids: Asteroid list)`),
a `toPoint : Vec2 -> Point` adapter at Scene-constructor call sites, and deliberate
`open` ordering in tests (`open AsteroidsDemo3.Model` **last** so bare `{X;Y}`
resolves to `Vec2`).

**Impact.** Several build-fix cycles; errors point at the *symptom site*, not the
ambiguous `open`, so they read as unrelated until the pattern is recognized.

**Recommendation.** This is partly inherent to F#, but the framework can reduce it:
prefer distinctive field names or a nominal constructor for `Point`/`Rect`
(e.g. `Scene.point x y`) and document, in the scene skill, that consumer geometry
types collide with `Point`/`Rect` and need conversion. A short "common pitfalls"
note would have pre-empted the whole cycle.

---

### F4 (P2) — Duplicate DU case names across co-opened modules

**Observed.**

```
EvidenceCommands.fs(576,12): error FS0003: This value is not a function and cannot be applied.
It has type 'ViewerRunBlockedStage', which does not accept arguments.
```

`EvidenceCommands.fs` opens `FS.Skia.UI.KeyboardInput` and `FS.Skia.UI.SkiaViewer`.
Both `ViewerKey` and `ViewerRunBlockedStage` define an `Unknown` case; with SkiaViewer
opened later, a bare `Unknown token` bound to `ViewerRunBlockedStage.Unknown` (nullary)
instead of `ViewerKey.Unknown of string`. The error names neither `ViewerKey` nor the
`open` conflict, so it is hard to diagnose.

**Resolution.** Fully qualify: `ViewerKey.Unknown token`.

**Recommendation.** Avoid colliding case names across modules that are designed to be
opened together (`KeyboardInput` + `SkiaViewer` are both standard in the app
profile), or rename the rarely-used `ViewerRunBlockedStage.Unknown` to something
module-specific (`UnknownStage`).

---

### F5 (P2) — Governance tests entangled with scaffold internals in one file

**Observed.** The generated `tests/.../Tests.fs` is a single compilation unit mixing
(a) durable governance source-scans (compile order, evidence-command surface,
`build.fsx` checks) with (b) behavioral tests bound to the **scaffold** model
(`Screen`, `Tally`, `ActiveColumn`, `NameChanged`, `SaveRequested`,
`controlsExampleView`, `adapterProgram`, `dispatchViewerKey`). Replacing the model
removes those members, so the entire test project fails to compile until every
coupled test is rewritten — there is no partial/incremental migration.

**Impact.** The plan said "Tests.fs becomes game-specific" but did not flag the
all-or-nothing constraint. I had to rewrite the whole file, manually salvaging the
~10 governance scans worth keeping and discarding the rest.

**Recommendation.** Split generated tests into two files: `GovernanceTests.fs`
(model-agnostic source/structure scans, intended to survive) and
`BehaviorTests.fs` (scaffold demo behavior, expected to be replaced). This lets a
consumer swap behavior tests without breaking governance coverage, and makes the
"keep these, replace those" boundary explicit.

---

### F6 (P2) — Strict success criteria with no enforcing assertion

**Observed.** SC-001 requires the ship visible on the first frame "100% of the
time." My initial invulnerability indicator hid the ship body on alternating
"blink" phases; the initial state (`InvulnSeconds = 2.0`) landed on a hide phase, so
the very first rendered frame would have shown no ship — a silent SC-001 violation.
No test or FAKE target caught it; only manual reasoning did. Fixed by always drawing
the body and using a blinking *outline ring* as the indicator.

**Impact.** A headline success criterion can be violated while every gate stays
green. The governance encodes the SC as prose, not as an executable check.

**Recommendation.** Where an SC is testable (first-frame content, no-overlap,
determinism), the tasks template should require a corresponding assertion, and the
evidence audit should be able to map SC → test. Otherwise "100%" claims are
aspirational.

---

### F7 (P3) — Mildly conflicting requirements left to implementer judgment

**Observed.** SC-002 ("sustains 5 min without leaking entities") and the soak test's
"bounded entity counts / waves don't accumulate" pull against FR-012 ("each new wave
adds at least one asteroid"). Unbounded per-wave count growth fails the bound over
18000 ticks. I resolved it by capping count (`3 + min wave 6`) while continuing to
escalate asteroid *speed* (FR-012 is satisfied via the "or higher speed" clause).

**Impact.** A reasonable resolution, but it is an unflagged judgment call; a
different implementer might read FR-012 as mandating monotonic count growth and
fail the soak bound.

**Recommendation.** Spec authoring should note the interaction and state the
intended bound explicitly (e.g. "count may cap; difficulty continues via speed").

---

### F8 (P3) — Layout-evidence governance shapes gameplay geometry

**Observed.** The `fs-skia-layout-evidence` contract requires HUD region and
gameplay entity bounds to not overlap. Classic Asteroids wraps entities across the
full screen, which naturally lets asteroids drift behind a HUD. To pass the gate I
reserved a HUD band, confined the playfield below it, **clamped reported gameplay
bounds** into the gameplay region, and overdrew an opaque HUD bar. Outcome is fine
(readable HUD, `ReadableLayout`/`NoLayoutOverlap`), but the governance constraint
nudged a specific design rather than describing a desired property the game already
had.

**Recommendation.** Document the intended pattern ("reserve a HUD band; confine or
clamp gameplay to the gameplay region; overdraw the HUD") in the layout-evidence
skill, so consumers reach it by design instead of by gate-driven trial and error.

---

### F9 (P2) — Skills are templated shells; one example contradicts the contract

**Observed.** The capability skills are string-substituted templates
("asteroidsdemo3" throughout) consisting of a pointer to the (missing) `.fsi`, one
short snippet, and build/test commands. They confirm *shape* (`Viewer.runApp`, pure
`update`, `Scene.group`, the effect boundary) but carry none of the concrete API
detail needed.

More seriously, `fs-skia-keyboard-input` demonstrates a different input model than
the host actually uses:

```fsharp
# skill example:
let bindings = [ { Key = "ArrowLeft"; Command = "move-left" } ... ]
let model, startupEffects = Keyboard.init bindings        # KeyboardEffect reducer
```

whereas the consumed `host-contract.md` and `GeneratedAppHost` use
`MapKey : ViewerKey -> bool -> Msg option`. Following the skill example literally
leads to an unused abstraction. I went by the contract doc + reflected types
instead.

**Recommendation.** Make skill examples match the app-profile host contract, and
either inline the real signatures or fix the `docs/api-surface` pointer (see F2).
A "common pitfalls" section per skill (collisions F3/F4, the geometry pattern F8)
would have prevented most of the time lost here.

---

## What worked well (for balance)

- **Effect boundary is clean and correct.** Pure `Model.update : Msg -> Model ->
  Model * AdapterCommand<Msg>` with all I/O confined to the `Viewer.runApp`
  interpreter made the game trivially unit-testable (28 tests, 96 ms) and the
  determinism guarantee straightforward (LCG threaded through `update`, byte-identical
  metrics across runs).
- **FAKE pipeline was race-free.** `Dev`/`Test`/`Verify`/`EvidenceGraph`/
  `EvidenceAudit` ran sequentially with no `.fake` contention on first attempt.
- **Real visual evidence.** On a host with a Wayland session, `--image-evidence`
  via `Viewer.runAppEvidence` produced a genuine decodable 640×480 RGBA PNG of the
  live scene — no synthetic 1×1 fallback was needed, so SC-009 is honestly met.
- **`runtimeCapability` / `desktopSessionDiagnostic` classification** made the
  unsupported-host-vs-defect distinction easy to wire correctly.

---

## Prioritized recommendations

| # | Priority | Recommendation |
|---|----------|----------------|
| 1 | P1 | Make `EvidenceGraph`/`EvidenceAudit` resolve and audit the **active** feature dir (from branch/arg); fail on zero/mismatched tasks; echo the audited path. (F1) |
| 2 | P1 | Ship the promised `docs/api-surface/*.fsi` in generated projects, or repoint skills at NuGet `.fsi`/ref assemblies. (F2) |
| 3 | P2 | Split generated tests into `GovernanceTests.fs` (survives) and `BehaviorTests.fs` (replaceable). (F5) |
| 4 | P2 | Fix `fs-skia-keyboard-input` example to match `MapKey`; add "common pitfalls" (type/case collisions, HUD/gameplay pattern) to scene/layout/keyboard skills. (F3, F4, F8, F9) |
| 5 | P2 | Where an SC is testable, require a mapped assertion in the tasks template; let the audit verify SC→test coverage. (F6) |
| 6 | P3 | Rename colliding DU cases (`ViewerRunBlockedStage.Unknown`) or distinctive geometry field names. (F3, F4) |
| 7 | P3 | Note interacting requirements (entity bound vs wave escalation) explicitly in spec authoring. (F7) |

---

## Appendix — environment & evidence

- Consumer: `/home/developer/projects/AsteroidsDemo3`, branch `001-asteroids-demo`.
- Gates: `Dev` ✓, `Test` ✓ (28/28), `Verify` ✓ (incl. GeneratedGuidanceCheck +
  TemplateDrift), `EvidenceGraph` `verdict=ok`, `EvidenceAudit` `verdict=PASS`
  (both against the wrong feature dir — see F1).
- `--evidence-run --seed 42 --frames 240 --script …`: `status=ok`,
  `close-reason=evidence-complete`, `deterministic=true`; two runs byte-identical.
- Visual: `readiness/game-image-evidence.png` — PNG 640×480 8-bit RGBA, 2458 bytes.
- API recovered via `FSharp.Reflection` over
  `src/AsteroidsDemo3/bin/Debug/net10.0/FS.Skia.UI.*.dll`.
