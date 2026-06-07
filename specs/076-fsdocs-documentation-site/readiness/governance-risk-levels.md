# Governance risk levels & readiness discovery (feature 076)

This feature is a mixed change: `.fsi` doc comments route to `package-surface`
(contracted) and `docs/**` to `docs-only`. The authoritative tier + minimal gate
list MUST come from `./fake.sh build -t Route` for the actual diff (see
`logs/route.txt` / `validation-contract.md`).

## Risk tiers

- **small** — a single `.fsi` doc-comment edit or one Markdown page. Focused
  gate: `PackageSurfaceCheck` / `PerPackageSurfaceDiff` (surface) or strict
  `dotnet fsdocs build --strict --eval` (content). Authoritative command per
  edit; artifact under `readiness/`; failure class = focused-gate failure; next
  action = fix the single file and rerun the focused gate.
- **medium** — a section (e.g. all `docs/architecture/**` pages, or one
  package's `.fsi` doc comments). Gate: strict fsdocs build + the analysis-section
  governance check. Failure class = content/analysis failure; next action = fix
  the offending page(s) and rerun.
- **broad** — the full site + publish path. Gate: the `Route` set +
  `EvidenceGraph` + `EvidenceAudit`. **broad validation** is required at merge;
  its aggregate results are recorded non-authoritatively under `readiness/` and
  the authoritative per-gate verdict is taken from `Route`.

## Required evidence

The **required evidence** artifacts for this feature are:
`logs/fsdocs-build.txt` (strict build), `surface-baseline-unchanged.md`
(FR-004), `api-coverage.md` (SC-001), `logs/route.txt` + `validation-contract.md`
(docs-only rule), `logs/pages-deploy.txt` (SC-005/SC-006),
`runtime-limitations.md`, and `manual-sc-verification.md`.
