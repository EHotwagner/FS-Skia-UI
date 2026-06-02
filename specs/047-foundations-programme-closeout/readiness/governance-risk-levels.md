# Governance risk levels & required validation (T003)

This feature is **documentation / measurement / verification-record** only (Tier 2,
no product `.fsi` / surface-baseline / `PackageVersion` change — SC-006). Each level
below names its **required evidence**; **broad validation is required** whenever a
governance/contributor doc the `Route`-first model depends on (`CLAUDE.md`,
`AGENTS.md`, `docs/reports/build.md`, `docs/reports/speckit.md`, `README.md`) or the
recurring-run schedule definition changes — which it does here, so `Route` escalates
this feature.

- **Small** — routine Markdown edits inside this feature's own `readiness/` notes
  and the deliverable docs (the after-baseline report, the closing ADR, the
  retrospective).
  - Authoritative command: focused review + `git diff` over the edited files.
  - Artifact: the committed Markdown + `git diff`.
  - Failure class: prose error / broken cross-link.
  - Next action: fix in place.

- **Medium** — the after-baseline measurement rows (Section A).
  - Authoritative command: re-run each non-estimate row's recorded reproduction
    command at the pinned SHA and confirm it yields the reported After value (SC-003).
  - Artifact: `readiness/after-baseline-repro.md`.
  - Failure class: a row whose command does not reproduce its After value.
  - Next action: fix the command or the value; never assert an unreproducible number.

- **Broad** — REQUIRED here: this is a governance-doc + `CLAUDE.md` / `AGENTS.md` +
  recurring-run-schedule change that `Route` escalates, and (as the programme-closing
  feature) it is run as a **dogfood** candidate through the full serialized set.
  - Authoritative command: the full serialized FAKE gate order
    (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
    `EvidenceGraph` → `EvidenceAudit`), run **sequentially**, never concurrently.
  - Artifact: `readiness/logs/*.log` + this feature's `EvidenceGraph` /
    `EvidenceAudit` verdicts.
  - Failure class: any gate failure; aggregate FAKE results are **non-authoritative** —
    a race-like / environment-flaky failure (the known `SkiaViewer.Tests` headless
    crash, or an `FsiTranscripts` Class-C exclusion) is rerun in focused isolation and
    that focused result is authoritative.
  - Next action: rerun the affected FAKE-backed command in isolation before product
    debugging.
