# Governance risk levels & required validation (T003)

This feature is **Tier 1 for the governance/build surface only** (it adds one new
curated governance `.fsi`, the `PerPackageSurfaceDiff` target, a `Routing.fs` rule,
and new baseline artifacts) and **Tier 2-equivalent for the runtime** (no runtime
`.fsi`, package identity/version, or rendering behaviour change — FR-010/FR-011/SC-007).
Because it touches governance/build paths, `Route` **escalates** it; as a V3-programme
dogfood feature it runs the full serialized gate set **plus** `PerPackageSurfaceDiff`.
Each level below names its **required evidence**; **broad validation is required**
whenever a governance/build target, `Routing.fs` rule, or curated governance `.fsi`
is added — which it is here.

- **Small** — routine Markdown inside this feature's own `readiness/`, the ADRs
  (`docs/adr/0007`–`0011`), and the baseline report prose.
  - Authoritative command: focused review + `git diff` over the edited files.
  - Required evidence: the committed Markdown + `git diff` (authoritative for the level).
  - Failure class: prose error / broken cross-link.
  - Next action: fix in place.

- **Medium** — the new `PerPackageSurfaceDiff` capability and its eight captured
  per-package baselines.
  - Authoritative command: the focused `./fake.sh build -t PerPackageSurfaceDiff`
    target run plus the `tests/Governance.Tests` pure + interpreter tests.
  - Required evidence: the zero-drift run (SC-004, `readiness/per-package-surface-diff.md`)
    and the one-package seeded drift (SC-005, `readiness/seeded-violation.md`) — these
    are the authoritative signals for the level.
  - Failure class: drift at the pin, or a seeded edit that drifts the wrong package count.
  - Next action: re-capture the baseline or fix the diff core; never weaken an assertion.

- **Broad** — REQUIRED here, because `Route` escalates this governance/build-path change.
  - Authoritative command: the full serialized FAKE gate order
    (`Dev` → `PerPackageSurfaceDiff` → `GeneratedGuidanceCheck` → `TemplateCheck` →
    `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`), run **sequentially**,
    never concurrently.
  - Required evidence: `readiness/logs/*.log` + this feature's `EvidenceGraph` /
    `EvidenceAudit` verdicts.
  - Failure class: any gate failure; aggregate FAKE results are **non-authoritative** —
    a race-like / environment-flaky failure (the known `SkiaViewer.Tests` headless
    libdecor-gtk crash) is rerun in focused isolation and that focused result is
    authoritative.
  - Next action: rerun the affected FAKE-backed command in isolation before product
    debugging.
