# Feature 043 classification (T001)

- **Tier**: Tier 1 (contracted) — adds new published `FS.Skia.UI.Build.Evidence`
  governance modules, each with a curated `.fsi` (Principle II). Designated
  **dogfood** + consumer-contract feature (FR-015) → `Route` escalates to the full
  serialized gate set.
- **Affected layer**: build-tooling only — `build/Governance/Evidence/**`,
  `build.fsx`, and (planned) `template/base/**`. No runtime `src/**`.
- **Public-API impact**: **no product `.fsi` change**; new curated build-tooling
  `.fsi` per Evidence module. No product surface-baseline diff (Invariant 1).
- **Elmish/MVU applicability**: the engine core is **pure**; it plugs into the
  existing `build.fsx` `update`/effect-interpreter boundary via two new pure
  effect cases (`EvidenceGraphCheck` / `EvidenceAuditCheck`). `update` stays pure;
  all I/O (file reads, `git`, writes) lives in `interpret` (Principle IV).
- **Real-evidence obligations**: 036/037/038 byte-parity for `task-graph.json`,
  `task-graph.md`, `audit-counts.txt` and the five captured scan outputs; typed
  cycle/topo/propagation/status-region tests; no-`python3`/no-`FSharp.Compiler.*`
  greps; the packed-engine consumer pass; the serialized FAKE logs.
- **Synthetic evidence**: none planned. This feature's own audit returns
  `verdict=PASS` with 0 `[S]`/`[S*]`/late-seh/diff-scan blocking (verified
  in-process).
