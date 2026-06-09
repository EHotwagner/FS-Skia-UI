# Generated-Validation Authority — Feature 088

The aggregate six-target results in `logs/` are recorded **non-authoritatively**. The
authoritative evidence for Feature 088 is:
1. `Dev` — build + full unit/governance suites (562 governance tests PASS).
2. `TargetMetadataDrift` — `validation.contract.yml` currency vs `Routing.fs` (PASS).
3. The Feature 088 governance tests — routable-gate projection set-equality / order, the
   non-degraded contract for every routable gate (SC-003), the split sub-target effect lists,
   the doc-only routing relaxation, and the byte-identical consolidated NuGet config.
A `GeneratedProductCheck` environment-failure (see `runtime-limitations.md`) is
non-authoritative and does not gate the verdict.
