# Phase 1 Data Model: Governance Gate Hardening

These are the engine value types that change in `FS.Skia.UI.Build`
(`build/Governance/**`). All are pure F# values over `tasks.md` /
`tasks.deps.yml` / `readiness/` / surface baselines; none introduce host I/O or
a new effect class. Shapes are illustrative (final field names settle in
implementation against the existing modules).

## 1. `AuditVerdict` — three-state (FR-007, `Evidence/Audit.fs`)

Replaces the binary `Pass | Fail` returned by `Audit.verdict` (currently
`Audit.fs:402`).

```fsharp
type AuditVerdict =
    | Pass                              // no synthetic, no blocking hits
    | PassWithAcceptedDeferrals         // only synthetic deferrals, each justified; zero blocking hits
    | Fail                              // any unaccepted synthetic, or any blocking hit
```

- **Invariant (FR-011)**: `PassWithAcceptedDeferrals` requires
  `unacceptedSynthetic = 0` **and** every blocking-hit count = 0
  (diff-scan, readiness-contract, persistent-launch, persistent-gui-runtime,
  window-visibility, audit-status, invalid-seh). It can never be reached while an
  unaccepted synthetic or any blocking hit exists.
- **Derivation**: from the existing `sehSummary` (`Audit.fs:365`) counts plus the
  new accepted-deferral set: `Fail` if any unaccepted-synthetic or blocking > 0;
  else `PassWithAcceptedDeferrals` if `acceptedDeferrals` non-empty; else `Pass`.

## 2. `AcceptedDeferral` — durable structured record (FR-008)

Recorded in `readiness/synthetic-evidence.json` (not solely a logged CLI flag).

```fsharp
type AcceptedDeferral =
    { TaskId: string                    // the deferred [S]/[S*] task
      Justification: string             // written rationale (required, non-empty)
      RealEvidencePath: string          // where real evidence will land once the capability exists
      AwaitedHostCapability: string }   // the host capability the artifact awaits
```

- Surfaced in `seh-audit-summary.json` with **separate** `acceptedSyntheticCount`
  vs `unacceptedSyntheticCount` (FR-008). `--accept-synthetic` populates these
  records (with justification); the verdict reads them.

## 3. Skill-loading evidence: `LoadProvenance` (FR-010, `Evidence/EvidenceFormatSchema.fs`)

Adds a `provenance` field/column to the existing 8-column skill-loading-evidence
row (`Audit.fs:228` `validateSkillLoadingEvidence`).

```fsharp
type LoadProvenance =
    | Captured       // observed during the run (recorded at the load action, before code changes)
    | Asserted       // manually hand-authored timestamp
```

- Row gains a 9th column `provenance`. Existing rules (`loaded_at <
  work_started_at`, ISO-8601, one row per task/skill) are unchanged.
- **Gap surfacing**: a declared-but-unloaded skill (a task's `skillist` entry with
  no matching loaded row) is reported **when the declaring task is implemented**,
  not only on the `[X]` flip. The schema (single source) is mirrored into
  `docs/evidence-formats.md`.

## 4. Per-step generated-product classification (FR-002, `Front/Governance.fs`)

```fsharp
type StepClassification = ProductDefect | Environment

type GeneratedProductStepResult =
    { Step: string                      // "Build" | "Test" | "Verify" (and scan steps)
      Passed: bool
      Classification: StepClassification // only meaningful when Passed = false
      PackageSet: PackageSet }          // see §5 — which package set this step used
```

- Overall target verdict = fail iff **any** step has `Passed=false` **and**
  `Classification=ProductDefect`. Environment-classified failures are reported but
  non-authoritative, and **cannot** suppress a product-defect in the same run
  (each step classified independently).

## 5. `PackageSet` — explicit package-source tag (FR-004)

```fsharp
type PackageSet =
    | LocalPacked    // TemplateCheck — locally-packed/unreleased .nupkg
    | Pinned         // GeneratedProductCheck — pinned/published version from Directory.Packages.props
```

- Every generated-product report states its `PackageSet` so an operator can tell
  which package set produced a given pass/fail and cannot mistake one for the
  other.

## 6. `PackageSkewFinding` — static pinned-vs-local skew (FR-003)

```fsharp
type PackageSkewFinding =
    { Symbol: string                    // referenced public API symbol, e.g. "ControlRenderResult.Bounds"
      File: string                      // generated source/test file referencing it
      PinnedVersion: string             // from template Directory.Packages.props
      LocalVersion: string }            // the local-packed bump target
```

- Produced by comparing **referenced symbols in generated source/tests** ∩
  **(local-packed surface − pinned surface)**. Computed statically from existing
  surface baselines — **no network restore**. A non-empty finding set blocks
  before merge (FR-003); the real tree produces an empty set.

## 7. Propagation input split (FR-009, `Evidence/Graph.fs`)

No new type — a **behavioral** change to `Graph.propagate`:

- **Today**: `allDeps t = t.ExplicitDeps @ t.PhaseDeps` (`Graph.fs:33`), and
  `propagate` filters taint over `allDeps` (`Graph.fs:128`).
- **Change**: taint propagation filters over `t.ExplicitDeps` **only**. Toposort,
  cycle detection, and ordering (`Graph.fs:58,85`) keep using `allDeps`.
- **Effect**: a phase-checkpoint edge (`PhaseDeps`, injected `TaskParser.fs:344`)
  no longer marks a downstream task `[S*]`; only a real declared data dependency
  (`ExplicitDeps`, incl. `owns`/`consumes`) propagates taint.

## Relationships

- `AuditVerdict` reads `sehSummary` counts **and** the `AcceptedDeferral` set.
- `AcceptedDeferral` records are the durable form of what `--accept-synthetic`
  asserts; counts feed `seh-audit-summary.json`.
- `GeneratedProductStepResult.PackageSet` ties FR-002's per-step classification to
  FR-004's package-source reporting.
- `PackageSkewFinding` is computed from the same surface baselines that
  `RefreshSurfaceBaselines` (FR-005/006) now regenerates completely and
  idempotently — the skew check trusts those baselines, so their currency matters.
