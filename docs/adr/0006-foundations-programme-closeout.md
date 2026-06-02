# ADR 0006 — Foundations programme closeout (Stage 7)

- **Status**: Accepted
- **Date**: 2026-06-02
- **Decision source**: the foundations implementation plan
  (`docs/reports/2026-05-31-1049-foundations-implementation-plan.md`, Stage 7 + the
  "Whole-programme definition of done" table) and feature
  `047-foundations-programme-closeout`. This ADR **closes** the programme: it records the
  realized outcome of decisions **D1–D6** (made in `0001`–`0005`) and the new
  steady-state development model. It introduces **no new decision** and changes no
  runtime, public `.fsi`, package, or dependency.

## Context

The foundations programme (Stages 0–6, features 039–046) set out to dismantle a
4,688-line `build.fsx` monolith and a tri-language (F# + Bash + Python) governance
engine, replacing them with a compiled, tested F# governance library and a two-tier,
`Route`-selected process. Stages 1–6 shipped and merged; the keystone is built. Stage 7
does **not add capability** — it confirms the programme's promises against the Stage-0
baseline, proves the interim scaffolding is gone, and documents the new normal so it
sticks for the next contributor (human or agent). This ADR is the single closing record.

## Decision

**Declare the foundations programme complete and adopt the two-tier `Route`-first model
as the steady-state development process.** The decisions D1–D6 are realized as follows:

- **D1 — Governance-library placement & distribution.** `FS.Skia.UI.Build` lives under
  `build/Governance/` (not `src/`), project-referenced by the repo build front-end and
  package-referenced by generated consumers. **Realized** (features 041/043): 36 files,
  5,226 lines, unit + property tested; packed and consumed by generated products.
- **D2 — Build front-end form.** A dedicated compiled FAKE exe `build/Build.fsproj`
  (`dotnet run`), registering every target off the typed `Targets` registry — **no FSX
  runner, no `FSharp.Compiler.*`**. **Realized** (feature 045): `build.fsx` deleted in
  full (4,688 → 0, no shim); the spike-confirmed fallback was never needed.
- **D3 — Evidence artifacts (minimal).** No tree cleanup, no history rewrite; only
  future regenerable logs/zips are gitignored. **Realized** (feature 046, one-line
  `.gitignore`); honored here — this closeout commits authored proofs and gitignores its
  regenerable `readiness/logs/`.
- **D4 — Spec Kit fork stance.** Full F# ownership of the evidence engine; the Python
  graph/audit ported to typed, tested, in-process F# with byte-for-byte parity proven
  before deletion. **Realized** (feature 043).
- **D5 — Sequencing.** Stage 0 + the Stage-3.1 spike first, then Stage 1 (two-tier
  process) shipped early as a dogfood feature with the library track in parallel.
  **Realized** across 039 → 042 → (041/043/044) → 045 → 046.
- **D6 — Configuration representation.** Framework-owned config is compiled F#
  values/predicates (`Routing.fs`, `Targets.fs`, `GeneratedProductContract.fs`):
  build-time-checked, no FCS, no runtime YAML parse for governance config; a data format
  is retained only for high-churn agent-authored `tasks.deps.yml`. **Realized**
  (features 041–045).

The **steady-state model**: `./fake.sh build -t Route` is the entry point. It reads the
change's diff and prints the authoritative **tier** and the **minimal gate list** to
run. Routine framework-internal work routes to the light `inner-loop` tier (`Dev`);
consumer-contract paths (`template/**`, `.specify/**`, public `src/**/*.fsi`, the
build-target/governance paths) **escalate** automatically. The full serialized
six-target order is the **escalated `maintainer-verify` path**, reserved for
consumer-contract changes and dogfood features — **not** the unconditional default. The
governance library `FS.Skia.UI.Build` is the **single home of all rules**; a mistyped
gate is a compile error, and generated artifacts (`validation.contract.yml`, the
`.claude` skill mirror) are **generated from a single source, not hand-synced**.

## Alternatives considered

- **Keep the serialized six-target order as the unconditional default (rejected).** It
  imposed full consumer ceremony (~12–14 h estimated) on every framework-internal
  change; the two-tier `Route` model removes that cost while keeping the full pipeline
  for the cases that need it.
- **Leave governance rules as honour-system prose (rejected).** Prose drifts and cannot
  fail a build; the programme moved every enforceable rule into compiled F# gates and
  kept prose only where it doubles as pinned author guidance.
- **Stand up a live external CI service for the recurring run now (deferred).** Out of
  scope; the recurring-run obligation is met by a committed, discoverable schedule
  definition plus a documented manual fallback, with no live-CI dependency.

## Consequences / rationale

- **A new contributor runs `Route` and proceeds** without reading the whole governance
  corpus — the documented exit criterion of Stage 7.
- **Rules cannot silently rot:** they are compiled, tested, and currency-checked;
  duplication is generated; the dogfood pipeline has a discoverable recurring-run
  mechanism.
- **The measured outcome** is recorded in the after-baseline
  [`docs/reports/_baselines/2026-06-02-foundations-after.md`](../reports/_baselines/2026-06-02-foundations-after.md):
  10 of 11 definition-of-done dimensions met-target; the governance-Markdown row carries
  the corrected-baseline rationale (the plan's ~23,000/21:1 was an over-estimate;
  corrected to ≈ 6,882, after 6,876, with rules now enforced by code). The runtime
  architecture is **unchanged** (FR-010).
- **Trade-off:** compiled config requires a recompile rather than a text edit — accepted
  for already-compiled build/governance tooling, and preferable to silent runtime drift
  (per ADR 0005).

## Programme links

- Stage-0 baseline (comparison oracle):
  [`docs/reports/_baselines/2026-05-31-foundations.md`](../reports/_baselines/2026-05-31-foundations.md).
- After-baseline (this programme's measured outcome):
  [`docs/reports/_baselines/2026-06-02-foundations-after.md`](../reports/_baselines/2026-06-02-foundations-after.md).
- Foundations implementation plan:
  [`docs/reports/2026-05-31-1049-foundations-implementation-plan.md`](../reports/2026-05-31-1049-foundations-implementation-plan.md).
- Shaping ADRs: [`0001`](./0001-governance-library-placement-and-distribution.md),
  [`0002`](./0002-build-front-end-form.md),
  [`0003`](./0003-generated-product-contract-versioning.md),
  [`0004`](./0004-spec-kit-fork-stance.md),
  [`0005`](./0005-configuration-representation.md).
