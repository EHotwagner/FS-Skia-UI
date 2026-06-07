# Readiness notes (feature 076) — T004

## Feature classification

- **Tier**: mixed. `.fsi` doc comments → `package-surface` (contracted);
  `docs/**` → `docs-only` (focused); the FAKE `Docs` target wiring →
  `build-target-contract` (escalated). Authoritative tier + minimal gate list per
  `./fake.sh build -t Route` for the actual diff (T031, `logs/route.txt`).
- **Affected layer**: documentation + doc-build configuration. Source touch is
  limited to `///` XML doc comments on `.fsi` signature files (doc text only).
- **Public-API impact**: **none** — FR-004 doc-only invariant. Adding `///` to
  `.fsi` does not change any signature shape or surface baseline (the surface
  normalizer strips comments before hashing). Verified by `PackageSurfaceCheck` /
  `PerPackageSurfaceDiff` (T030, `surface-baseline-unchanged.md`).
- **MVU/effect boundary**: **N/A** — no stateful or I/O-bearing workflow is
  introduced. The docs *describe* the framework's MVU surfaces; they do not add
  one.
- **Synthetic evidence**: **none expected**. Literate `.fsx` examples are real and
  build-evaluated; closing analyses are authored opinion grounded in real
  `docs/adr/**` + `docs/reports/**`; no embedded fabricated visuals.

## Required evidence obligations

- `logs/fsdocs-build.txt` — `dotnet fsdocs build --strict --eval` (SC-005/SC-009).
- `surface-baseline-unchanged.md` — FR-004 / SC-007.
- `api-coverage.md` — SC-001 (zero empty stubs).
- `logs/route.txt` + `validation-contract.md` — docs-only rule.
- `logs/pages-deploy.txt` — SC-005 / SC-006 (Pages publish).
- `runtime-limitations.md`, `governance-risk-levels.md`, `manual-sc-verification.md`,
  `skill-loading-evidence.md`.
