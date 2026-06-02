# Governance risk levels & required validation (T003)

Each level below names its **required evidence**; **broad validation** is required
whenever a code-enforced rule's prose is deleted or the generated-product contract changes.

- **Small** — routine framework-internal edits inside this feature's own
  `build/Governance/*.fs` / `tests/Governance.Tests/*.fs` work.
  - Authoritative command: `./fake.sh build -t Dev` + the `Governance.Tests` suite.
  - Artifact: `readiness/unit-tests.md`, `readiness/logs/dev.log`.
  - Failure class: unit-test failure / compile error.
  - Next action: fix in place; never weaken an assertion.

- **Medium** — the new `Guidance.fs` Constitution-Check validator surface, the new
  `GeneratedProductContract.fs(/.fsi)` module, and the `GeneratedProduct.fs` consult-point.
  - Authoritative command: focused `Dev` plus the targeted gates `Route` prints
    (`GeneratedGuidanceCheck`, `GeneratedProductCheck`) plus the typed unit tests.
  - Artifact: `readiness/logs/generated-guidance-check.log`,
    `readiness/logs/generated-product-check.log`, `readiness/unit-tests.md`.
  - Failure class: gate finding (named area / structural rule) or typed-test failure.
  - Next action: address the named finding; re-run the focused gate.

- **Broad** — REQUIRED here: this is a governance-path + generated-product-contract +
  `.agents/skills/**` change that `Route` escalates, and a code-enforced rule's prose is
  deleted. Broad validation is mandatory whenever prose is trimmed or the generated-product
  contract changes.
  - Authoritative command: the full serialized FAKE gate order
    (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
    `EvidenceGraph` → `EvidenceAudit`), run **sequentially**, never concurrently.
  - Artifact: `readiness/logs/serialized-gates.md` + the per-gate logs.
  - Failure class: any gate failure; aggregate FAKE results are **non-authoritative** —
    a race-like / environment-flaky failure (the known `SkiaViewer.Tests` headless crash)
    is rerun in focused isolation and that focused result is authoritative.
  - Next action: rerun the affected FAKE-backed command in isolation before product debugging.
