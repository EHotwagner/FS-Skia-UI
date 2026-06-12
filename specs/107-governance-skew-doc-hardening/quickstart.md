# Quickstart: Governance Skew & Doc-Check Hardening

How to exercise and verify the two governance fixes. All commands run from the repo root. FAKE-backed
targets share `.fake` state — run them **sequentially**, never concurrently.

## 1. Run the focused governance/package tests (fastest inner loop)

```bash
# FR-001/FR-002/FR-003 — package-skew false positives gone, real detection retained
dotnet test tests/Governance.Tests

# FR-004/FR-005 — doc-preservation is package-agnostic and still fails on dropped summaries
dotnet test tests/Package.Tests
```

Each requirement ships a red-before/green-after test:
- comment-only `FS.Skia.UI.*` token → **no** referenced symbol (FR-001)
- `open FS.Skia.UI.Controls.Typed` + `…Typed.<Module>.<member>` → resolves clean (FR-002)
- seeded `…UnreleasedBoundsV087` and comment+live-code-same-file → **still a finding** (FR-003)
- placeholder-absent reference fixture → passes; zero-`///` reference fixture → **fails** (FR-004/FR-005)

## 2. Regenerate the additive per-package baseline (FR-002)

```bash
./fake.sh build -t RefreshSurfaceBaselines
git diff readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt
```

Expect an **additive** diff: typed front-door members from `src/Controls/Widgets/*.fsi` (and the
`Typed` namespace segment) appear; nothing is removed. Review and commit the regenerated baseline.

## 3. Run the consumer-contract gate that exercises both fixes

```bash
./fake.sh build -t PackageSurfaceCheck
cat readiness/package-skew.md          # expect: status=clean, findings=0
```

`PackageSurfaceCheck` regenerates `docs/api-surface/*.md`, runs the per-package surface diff against
the regenerated baseline, runs package-skew over the template tree, and runs the package-agnostic
doc-preservation test.

## 4. Full escalated verification + evidence

```bash
./fake.sh build -t Route                 # confirm tier + minimal gate list for this diff
./fake.sh build -t Dev
./fake.sh build -t Verify
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit         # expect verdict PASS, 0 synthetic
```

## Manual smoke (optional): prove the work-around is no longer needed (SC-005)

Temporarily add `open FS.Skia.UI.Controls.Typed` and a comment naming `FS.Skia.UI.Controls.Typed`
to a `template/base/src` file, run `./fake.sh build -t PackageSurfaceCheck`, confirm `package-skew.md`
stays `status=clean`, then revert. (The repository change itself does **not** modify the template —
that adoption is deferred; this only demonstrates it is unblocked.)
