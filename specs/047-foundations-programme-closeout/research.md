# Phase 0 Research — Stage 7 Closeout

Resolves the open questions for the closeout. All five spec Clarifications (session 2026-06-02)
are folded in; no `NEEDS CLARIFICATION` remain.

## R1 — Scaffolding proof scope (FR-001/002, SC-001)

**Question.** FR-001 demands a proof of "zero tracked-tree matches" for the interim-scaffolding
patterns. A naive `git grep` of the token patterns is **not** zero — but every residual match is
non-scaffolding. How do we make the proof zero-by-construction and honest?

**Reconnaissance (authoring-time sweep at the feature branch).**

| Pattern | Proof kind | Tracked-tree result |
|---|---|---|
| root `build.fsx` | `git ls-files build.fsx` | **empty** (deleted in 045) |
| `scripts/build/select-tier.fsx` | `git ls-files` | **empty** (folded into library in 045) |
| `run-audit.sh` | `git ls-files '**/run-audit.sh'` | **empty** (ported in 043) |
| `.specify/**/*.py` | `git ls-files '.specify/**/*.py'` | **empty** (F#-only since 043) |
| `--legacy-evidence` | scoped `git grep` | matches **prose only** in `specs/043/**` + impl-plan |
| `fake-cli` / `dotnet fake` / `FSharp.Compiler.*` | scoped `git grep` | matches prose, enforcement strings, absence-comments |

The flag/runner token matches fall into exactly four **non-scaffolding** classes, confirmed by
inspection:

1. **Frozen feature-history prose** — `specs/043-foundations-evidence-engine/**` (the feature that
   *removed* the Python path) and `docs/reports/2026-05-31-1049-foundations-implementation-plan.md`
   (the programme plan). Rewriting frozen, merged feature history is out of scope and would be
   dishonest; these are records *of* the removal.
2. **The governance library's own enforcement scan-strings** — `build/Governance/Guidance.fs` lines
   88/91 contain the regex `(\./fake\.sh|fake\.cmd|dotnet fake)` because the library *detects*
   `dotnet fake` in docs to enforce the `Route`-first rule. The string is the thing being policed,
   not a live invocation.
3. **Assert-the-absence comments** — `Directory.Packages.props` lines 29–38 are comments stating
   that `FSharp.Compiler.Service` is **NOT** shipped (FR-008/FR-012 of an earlier feature). The
   token appears only to assert its own absence. No `<PackageVersion>` references it (confirmed: no
   match in `*.fsproj`/`*.props` package elements / paket files).
4. **Legitimate FAKE entry-point text** — `./fake.sh`/`fake.cmd`/`dotnet fake` are the *current,
   correct* FAKE entry points; `build/Program.fs` (a comment) and `build/Governance/Preflight.fs`
   (a `dotnet tool restore` diagnostic) mention them as live, valid usage — not the removed
   FSX-runner scaffolding.

**Decision.** The proof has two shapes, both committed in `readiness/scaffolding-proof.md`:

- **File-existence proofs** (the dead artifacts): `git ls-files <path>` → empty output is the proof.
  These are unconditionally zero.
- **Scoped proofs** (the flag/runner): demonstrate no *active scaffolding* — i.e. no
  `--legacy-evidence` flag is parsed in the live command surface, and no `FSharp.Compiler.*`
  dependency or `dotnet fake` FSX-runner invocation exists in build sources / dependency manifests.
  The proof records the *full* token grep, then the scoped grep that excludes the four allowlisted
  classes (frozen `specs/**` history, the `Guidance.fs` enforcement regex, the
  `Directory.Packages.props` absence-comments, the live-FAKE diagnostics), yielding zero. The
  allowlist is **named and justified inline**, so a reviewer sees exactly why each retained match is
  not scaffolding (spec Edge Case: "a stale doc reference … the residual is removed or the reference
  corrected").

**Residual handling (FR-002).** If the scoped sweep surfaces a *genuine* residual (a live flag, a
dead script, a stale operative doc reference such as `branch-vs-master` in `build.md`), it is
removed/corrected and the proof re-run clean. The `branch-vs-master` reference in `build.md:168` is
a known stale-ref candidate for the US3 doc pass, not a scaffolding artifact.

**Alternatives rejected.** (a) Rewriting `specs/043/**` to purge the tokens — dishonest, destroys
the record of the removal, out of scope. (b) A bare unscoped grep asserted as "expected non-zero" —
fails SC-001's "zero matches" letter and hides genuine residuals in the noise. The scoped-proof +
named-allowlist approach satisfies the zero-matches letter *for active scaffolding* while staying
reproducible and honest.

## R2 — Canonical 100% coverage set (FR-003, SC-002)

**Decision (spec Clarification).** The plan's **11-row "Whole-programme definition of done"** table
is the canonical 100%-coverage set. Each row gets a before-value, an after-value, and a met-target
marker *or* a written rationale. The three softer work-item-7.2 metrics that are **not** in that
table — per-feature ceremony time, agent context bytes, warm-build time — are recorded in a clearly
labelled **supplementary "estimate" section** and are **not** counted toward the definition-of-done
total. This resolves the plan-internal mismatch between the 11-row table and 7.2's metric list.

The 11 dimensions (from the implementation plan's table) are: `build.fsx` size; domain-logic
location; evidence-path languages; the three Python/Bash LOC; governance Markdown (rules);
`.claude`/`.agents` duplication; framework-author process; tier selection; framework-owned config;
generated-product contract; runtime architecture.

## R3 — Per-metric reproduction commands (FR-004, SC-003)

**Decision.** Each non-estimate row carries the exact command that yields its after-value, mirroring
the Stage-0 baseline's command discipline (the baseline already records the "before" command for
most rows). Indicative after-commands:

- `build.fsx` size → `git ls-files build.fsx | xargs -r wc -l` → **0** (no tracked root file);
  target was "deleted or ≤200-line shim" → **met** (deleted).
- evidence-path languages → `git ls-files '.specify/**/*.py' '.specify/**/*.sh' | wc -l` → **0** Python/Bash in the evidence path → **{F#}** → **met**.
- the three Python/Bash files → `git ls-files '**/compute-task-graph.py' '**/audit-status-scan.py' '**/run-audit.sh' | wc -l` → **0** → **met** (logic in `build/Governance/Evidence/**`).
- governance Markdown (rules) → the **corrected** measurement: feature 046 established the rule/guidance baseline as **≈6,882 lines**, not the plan's overstated ~23,000 (spec Assumption). The row records the corrected before-figure with an explicit note, the after-figure, and the delta. **This row carries a written rationale** (the baseline correction).
- `.claude`/`.agents` duplication → before 5,854 lines unguarded → after: single-sourced + currency-checked (feature 044) → **met**, with the command counting the combined mirror and citing the currency gate.
- framework-author process → **estimate-backed rationale row**: before ~12–14 h/feature single-tier; after `inner-loop` default (`Dev` only) via `Route`. The hour figure is an estimate (no timing harness — same exemption Stage 0 used), so the *target-met* judgement rests on the mechanism (light tier is now the default), and the hour delta is cross-referenced to the supplementary estimate section.
- tier selection → `./fake.sh build -t Route` exists + `Routing.fs` compiled → **met**.
- framework-owned config → compiled F# (no FCS) → **met** (ADR 0005).
- generated-product contract → versioned with deprecation window (feature 046, ADR 0003) → **met**.
- runtime architecture → **unchanged** (SC-006: `git diff` over `src/**` empty).

Rows whose after-value cannot reach the plan's literal target get a **written rationale** rather
than a padded number (FR-005) — principally the governance-Markdown row (the ~23,000 figure was an
over-estimate) and the framework-author-process row (hours are an estimate). The exact final
commands and values are produced during implementation and pinned with the feature SHA; the
reproducibility re-run is captured in `readiness/after-baseline-repro.md` (SC-003).

## R4 — Recurring-run realization (FR-009, SC-005)

**Decision (spec Clarification).** The repo has no committed schedule file today (only a transient
`.claude/scheduled_tasks.lock`, which is a runtime lock, not a schedule definition). The
recurring-run obligation is satisfied by:

1. A **tracked schedule-definition file** committed under a discoverable repo path (a documented
   routine/cron spec) that names the dogfood set (042, 043) and the full serialized six-target
   pipeline. The concrete path + format is fixed in
   [contracts/recurring-run.md](./contracts/recurring-run.md).
2. A **documented manual full-pipeline fallback command** (the serialized six-target order) so the
   pipeline is runnable by hand with no dependency on a live external CI service.

The mechanism must be **discoverable in the tree** and **runnable**; it does **not** require a live
CI service to exist (spec Unsupported scope, Assumptions). The Claude Code scheduling surface (the
`schedule` skill / routine spec) is the natural local realization; the manual fallback is the
authoritative guarantee.

## R5 — Closing ADR (FR-008)

**Decision.** The next sequential ADR is **`docs/adr/0006-foundations-programme-closeout.md`**,
following the established format of 0001–0005 (`# ADR 000N — Title`, then
`Status`/`Date`/`Decision source`, `## Context`, `## Decision`, `## Alternatives considered`,
`## Consequences / rationale`). It records the programme's outcome, the realized decisions **D1–D6**
(library distribution, build front-end form, evidence-engine F# ownership, generate-don't-sync,
sequencing, compiled-config), and the new steady-state development model (Route-first two-tier
process). It is cross-linked from the after-baseline (SC-005) and links back to the Stage-0 baseline
and the implementation plan.

## Consolidated decisions

| # | Decision | Rationale | Alternatives rejected |
|---|---|---|---|
| R1 | File-existence + scoped proofs with a named allowlist | Zero-by-construction for *active* scaffolding without rewriting frozen history | Rewrite specs/043 history; bare non-zero grep |
| R2 | 11-row table = 100% set; 3 softer metrics = supplementary estimates | Spec Clarification; removes table/7.2 mismatch | Merge all 14 into one total |
| R3 | Per-row command + SHA; rationale on misses | Stage-0 command discipline; honest variance | Padded/omitted numbers |
| R4 | Tracked schedule file + manual fallback, no live CI | Spec Clarification; local-only toolchain | Stand up external CI |
| R5 | ADR 0006, established format, cross-linked | Sequential continuity with 0001–0005 | Inline the ADR into the baseline |
