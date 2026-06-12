# Governance risk levels — feature 107 (governance-skew-doc-hardening)

Feature 107 is a **Tier-2 (internal governance)** change in the single governance home
(`build/Governance/PackageSkew.fs`, `PerPackageSurface.fs/.fsi`) + two governance test files, plus one
additive per-package surface baseline regeneration. No product behavior or product `.fsi` shape change
(spec FR-007). `Route` is authoritative; the per-package-surface rule covers it (no new gate, no
`validation.contract.yml` / `Routing.fs` change).

## small

A comment/doc-only governance edit (e.g. a single helper comment).
- required evidence: the file compiles; the single affected governance unit test stays green; the diff
  shows only the intended lines.

## medium

**This feature's level.** The FR-001 comment-strip + FR-002 capture-broadening rule changes, the
FR-004/FR-005 doc-preservation assertion swap, and the additive Controls + SkiaViewer per-package
baseline regen.
- required evidence: `dotnet test tests/Governance.Tests` (the new `feature107SkewHardeningTests`
  FR-001/002/003 cases + the retained 087 seeded-skew test, green) and `dotnet test tests/Package.Tests`
  (FR-004/FR-005 + the package-agnostic assertion, 35/35); `./fake.sh build -t PackageSurfaceCheck`
  Status Ok with `readiness/package-skew.md` `status=clean findings=0` (SC-001/SC-004) and a green
  per-package diff over the regenerated baselines; the baseline diff is additive (Controls +693,
  SkiaViewer +237, 0 removed — FR-007); `EvidenceGraph` + `EvidenceAudit` PASS with 0 synthetic.

## broad

Required only if a FAKE-backed failure looks race-like or unknown-concurrent. Then rerun the affected
FAKE-backed commands **sequentially** before any product-regression claim.
- broad validation: the serialized order `./fake.sh build -t Dev` → `-t Verify` → `-t EvidenceGraph`
  → `-t EvidenceAudit` executed sequentially (shared `.fake` state, never concurrently); aggregate-suite
  results obtained outside the focused per-suite runs are recorded as a **non-authoritative aggregate**
  (see `aggregate-hang-diagnostics.md`) and the per-suite Expecto outcomes are authoritative.

## Known pre-existing, out-of-scope failure (recorded, not introduced)

The Governance.Tests `template package pins ... posture` test fails at HEAD: template
`FsSkiaUiVersion`=0.1.111-preview.1 vs repo libs=0.1.112-preview.1 — the normal mid-cycle posture after
the feature-106 lib bump (commit 8d5d2fcf) and before the separate "Update template package pins" step.
This feature makes no version change (FR-007) and did not introduce this failure (verified at HEAD); it
is the only Dev failure and is unrelated to the two governance fixes (0 regressions from feature 107).
