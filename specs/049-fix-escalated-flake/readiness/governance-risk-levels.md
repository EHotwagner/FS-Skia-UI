# Governance risk levels & required validation (T003)

This feature is **Tier 2 (internal build-tooling)** — a build-front-end
process-launch change that `Route` escalates to `maintainer-verify`. Each level
below names its **required evidence**; **broad validation** is **required** here
because the change alters how the escalated path spawns test/FSI/nested-`fake.sh`
processes.

- **Small** — routine Markdown edits inside this feature's own `readiness/` notes
  and the decision-table contract mirror (`graphics-env-contract.md`).
  - Authoritative command: focused review + `git diff` over the edited files.
  - Required evidence: the committed Markdown + `git diff`.
  - Failure class: prose error / broken cross-link / drift from `contracts/`.
  - Next action: fix in place.

- **Medium** — the pure-normalization core (`BuildEnvironment.fs`) and its
  unit / FsCheck-property / spawn-contract tests in `tests/Governance.Tests`.
  - Authoritative command: the failing-first → green Expecto run
    (`dotnet test tests/Governance.Tests`).
  - Required evidence: the red-then-green Expecto output for
    `GraphicsEnvironmentTests`.
  - Failure class: a failing assertion or a property counterexample.
  - Next action: fix the function or the test; never weaken an assertion.

- **Broad** — REQUIRED here: a build-front-end process-launch change that `Route`
  escalates to `maintainer-verify`.
  - Authoritative command: the escalated serialized FAKE order
    (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
    `EvidenceGraph` → `EvidenceAudit`), run **sequentially** (shared `.fake`
    state), never concurrently, **once**, with no manual `env -u WAYLAND_DISPLAY`
    prefix.
  - Required evidence: `readiness/logs/*.log` + this feature's `EvidenceGraph` /
    `EvidenceAudit` verdicts + `readiness/aggregate-hang-diagnostics.md`.
  - Failure class: any gate failure. **After this feature the aggregate result is
    authoritative for this graphics-backend flake class** — the deterministic
    dual-display guard (unit-proven) removes the previously-required focused rerun.
    A genuinely race-like failure unrelated to this flake class is still rerun in
    focused isolation.
  - Next action: for the graphics-backend flake class, trust the single-run
    aggregate; for any other race-like failure, rerun the affected FAKE-backed
    command in isolation before product debugging.
