# Agent-Ready Verdict — Feature 088 (Governance Precision Hardening)

**Verdict: agent-ready.** The three independently-shippable tiers landed on one branch in
priority order (US1 → US2 → US3). Authoritative evidence:

- `Dev` — PASS (build + full unit/governance suites; see `logs/dev.txt`). 562 governance tests
  pass, including the 12 new Feature 088 tests.
- `TargetMetadataDrift` — PASS (`validation.contract.yml` current vs `Routing.fs`; the routable-
  gate projection reproduces the prior `knownGates` / `ProductChecksRun` literals).
- `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit` — see `logs/`.

## SC-007 independent-shippability argument (design + ordering property)

SC-007 is verified as a **design/ordering property**, not by checking out and gate-running each
tier in isolation. The tiers touch **disjoint files** relative to Tier 2's contract
regeneration:

- **Tier 1 (US1)** — typed re-keying of `focusedGateContract` (`Front/Helpers.fs`), the
  routable-gate projection + derived `knownGates`/`ProductChecksRun` (`Targets.fs`,
  `AgentValidation.fs`, `Update.fs` call sites). **Byte-identical** contract: non-routable
  targets reproduce the exact former wildcard value; `knownGates` is set-equal and
  `ProductChecksRun` is byte-for-byte/in-order equal to the prior literals (T014).
- **Tier 2 (US2)** — the **only** tier that regenerates `validation.contract.yml`: the two
  additive split sub-targets (`Targets.fs`/`.fsi`), the umbrella composition (`Update.fs`),
  and the doc-only routing relaxation (`Routing.fs`). The intentional contract diff +
  doc-only `Route` relaxation are demonstrated in `route-after-*.txt` and
  `validation-contract-diff.md` (T020/T021).
- **Tier 3 (US3)** — behavior-preserving extraction in `GeneratedProduct.fs` only (shared
  `missingRequiredFiles` validator + consolidated NuGet config renderer). **Byte-identical**
  artifacts/goldens; **no** `.fsi` / `validation.contract.yml` change (T025).

Because Tier 1 and Tier 3 touch files disjoint from Tier 2's contract regeneration, reordering
or dropping any single tier leaves the other two byte-identical. This file **records** that
argument; it does not re-run an isolated single-tier six-target pass.

## Compile-time enforcement (SC-001)

`focusedGateContract` is now an exhaustive, wildcard-free `match` over `Targets.Target`; a
future `Target` case added without classifying it is a **compile error**, not a silent
verification-degraded fall-through. `knownGates` and `ProductChecksRun` derive from the closed
`Targets` DU, so a renamed/retired gate is a compile error that flows to the allowlist and the
verdict automatically.

## Non-authoritative aggregate

`GeneratedProductCheck` / `GeneratedConsumerValidation` consumer runs are recorded
non-authoritatively (see `generated-validation-authority.md`, `runtime-limitations.md`); a
local environment-failure there does not gate this verdict.
