# build.fsx Line Delta (T018 / FR-011 / SC-001)

| Measure | Lines |
|---|---|
| build.fsx working baseline (2026-05-31 foundations baseline) | 4,767 |
| build.fsx Stage-0 baseline | 4,688 |
| build.fsx after relocation | **0 (file deleted)** |
| Residual shim | none — no `#r`-the-DLL fallback was needed |

`build.fsx` was deleted in full (`git rm build.fsx`). The entire front-end relocated into
compiled, curated-`.fsi` modules under `build/Governance/` (Engine/Model, Engine/Update,
Engine/Interpret, GeneratedProduct, Guidance, Preflight) plus helper modules under
`build/Governance/Front/` (the former `scripts/build/*.fsx`). No concrete blocker required the
documented `≤200-line` shim, so the residual count is 0.

Captured: 2026-06-01T14:44:26Z
