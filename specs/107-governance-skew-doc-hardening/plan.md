# Implementation Plan: Governance Skew & Doc-Check Hardening

**Branch**: `107-governance-skew-doc-hardening` | **Date**: 2026-06-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/107-governance-skew-doc-hardening/spec.md`

## Summary

Fix two recurring, structural governance foot-guns surfaced by the feature-106
retrospective, both in the single governance home (`build/Governance/**` + its tests):

1. **Package-skew false positives.** The static package-skew check
   (`build/Governance/PackageSkew.fs`) scans raw template/sample source — comments
   included — and validates references against a per-package surface that **omits the
   typed front door** (`src/Controls/Widgets/*.fsi` / `FS.Skia.UI.Controls.Typed`). So
   a comment that names a framework namespace, or a live `open FS.Skia.UI.Controls.Typed`,
   produces a false skew finding. **Fix:** (a) strip comments from source before extracting
   referenced symbols (reusing the comment-strippers already in `PerPackageSurface`), and
   (b) broaden `PerPackageSurface.captureCurrent` to recurse into the package source dir so
   the genuinely-public `Widgets/*.fsi` typed surface becomes captured known symbols; then
   regenerate the `FS.Skia.UI.Controls` per-package baseline.

2. **Doc-preservation check depends on boilerplate.** `PackageApiReferenceTests`
   proves the reference generator preserves `///` summaries by asserting the **placeholder
   boilerplate sentence** is present in Scene/Testing — which the deferred non-Controls
   doc cleanup will remove, re-breaking the check the same way feature 106 had to special-case
   Controls. **Fix:** replace the boilerplate-presence assertion with a **package-agnostic**
   signal — every tracked package's generated reference embeds its full `.fsi` and must carry
   at least one **substantive (non-placeholder) `///` summary line** — and keep a red-before
   fixture proving the check still fails if the generator drops summaries.

**Approach:** pure-core changes in `build/Governance/**`, red-before/green-after governance
tests under `tests/Governance.Tests` and `tests/Package.Tests`, one additive per-package
baseline regeneration via `RefreshSurfaceBaselines`. No product `.fsi` shape change, no new
gate, no `validation.contract.yml` change.

## Technical Context

**Language/Version**: F# / .NET (`net10.0`) — compiled governance in `FS.Skia.UI.Build`
**Primary Dependencies**: None new. Expecto + FsCheck (existing test stack); `System.Text.RegularExpressions` (already used by `PackageSkew`).
**Testing**: Expecto unit/governance tests; FAKE `PackageSurfaceCheck` target exercises the regenerated baseline + the doc-preservation test; `RefreshSurfaceBaselines` regenerates the per-package baseline; `EvidenceGraph` + `EvidenceAudit` verdicts.
**Target Platform**: Windows and Linux (governance text analysis is platform-neutral).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Tier**: Tier 2 (internal governance change). No product behavior, public product `.fsi`
signature, or generated runtime output changes (spec FR-007). The one baseline touched is the
**captured per-package surface** for `FS.Skia.UI.Controls`, regenerated additively to include
the already-public typed front door — a surface-capture completeness fix, not a product
contract change. Principle II/VI honored: changes ride compiled `.fsi`-backed governance modules
with failing-first tests.

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**` content, sample, or `.template.config/template.json`
  change. The check that *scans* `template/base/src` + `template/base/tests` changes, but the
  scanned template tree is untouched. (After this fix the template *could* drop feature 106's
  per-control module-alias work-around in favor of a natural `open FS.Skia.UI.Controls.Typed` —
  that simplification is **deferred / out of scope** here, SC-005 only proves it is no longer blocked.)
- **Dependency impact**: N/A — no new dependency; `Directory.Packages.props`, `docs/dependencies.md`,
  and `DependencyReport` are unaffected.
- **Command-surface impact**: No new target and no `validation.contract.yml` change (no new gate —
  spec Framework Governance Prompts). Existing targets exercise the change, run sequentially
  (FAKE `.fake` state is not concurrency-safe):
  1. `./fake.sh build -t Dev` (runs the new Governance.Tests + Package.Tests)
  2. `./fake.sh build -t RefreshSurfaceBaselines` (regenerates the Controls per-package baseline — FR-002)
  3. `./fake.sh build -t PackageSurfaceCheck` (per-package surface + package-skew + doc-preservation)
  4. `./fake.sh build -t Verify`
  `TargetMetadataDrift`/`SkillSyncCheck` need no action (no `Routing.fs` gate edit, no skill-tree edit).
- **Generated project impact**: N/A — generated default/minimal contents, selected Controls
  guidance, local skills, validation logs, and placeholder/excluded-history scans are unchanged.
  The package-skew check that *runs against* the generated/template tree is hardened, but the
  generated output it inspects does not change.
- **Evidence paths**: log + artifacts under `readiness/`:
  - `readiness/package-skew.md` — regenerated skew report (expected `status=clean`, `findings=0`).
  - `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` — regenerated, additive (now includes `Widgets/*.fsi`).
  - `readiness/logs/package-surface-check.txt` — `PackageSurfaceCheck` log.
  - `docs/api-surface/FS.Skia.UI.*.md` — regenerated references (unchanged content; read by the doc-preservation test).
  - `readiness/evidence-graph.md`, `readiness/evidence-audit.md` — `EvidenceGraph`/`EvidenceAudit` verdicts.
- **`.fsi` / contract impact**: No product `.fsi` signature shape change (FR-007). Governance
  module signatures may change: `PerPackageSurface.fsi` (capture broadening — likely no public
  signature change if recursion is internal to `captureCurrent`), and possibly a small new
  predicate exposed for the doc-preservation test (e.g. a "substantive summary" helper). The
  captured `FS.Skia.UI.Controls.fsi.txt` baseline is regenerated additively and reviewed.
- **MVU/effect boundary**: N/A — both fixes are pure text analyses (regex extraction over
  comment-stripped source; substring/line predicates over generated reference text). No `Model`,
  `Msg`, `Effect`, `Cmd<Msg>`, `init`, `update`, subscription, or interpreter is involved.
- **Synthetic evidence**: None expected `[S]`. The red-before fixtures for FR-005 (a reference
  body with no `///` summaries) and FR-003 (a seeded unreleased symbol — the retained 086
  near-miss) are **negative-path / error-path** literals that prove a real check still fires;
  they exercise the production predicate against a malformed/absent input, not a stubbed product
  capability. If any such case needs disclosure it is `[SEH]` (`synthetic-error-handling-approved`),
  decided at task generation — not `[S]`/`[S*]`. Default expectation: 0 synthetic, EvidenceAudit PASS.
- **Test evidence**: failing-first governance tests, one per requirement —
  - FR-001: `referencedSymbols` over a file whose only `FS.Skia.UI.*` token is in a `//`/`///`/`(* *)`
    comment yields **no** referenced symbol (red before comment-stripping).
  - FR-002: `captureCurrent` for `FS.Skia.UI.Controls` includes typed front-door members; and
    `referencedSymbols` for `open FS.Skia.UI.Controls.Typed` + `FS.Skia.UI.Controls.Typed.<Module>.<member>`
    resolves clean against the regenerated baseline.
  - FR-003: the retained seeded-unreleased-symbol test (`...UnreleasedBoundsV087`) and the
    comment+live-code-same-file edge case still produce a finding (real detection intact).
  - FR-004: with the placeholder sentence absent from a package reference fixture, the
    package-agnostic doc-preservation check passes (≥1 substantive `///` summary present).
  - FR-005: with a reference fixture carrying **zero** `///` summaries, the check FAILS (guarantee retained).
  Plus target-level evidence: `PackageSurfaceCheck` green over the regenerated baseline.
- **Observability**: `renderFindings` already emits an actionable `readiness/package-skew.md`
  (per-finding `symbol/file/pinned/local`); preserved. The doc-preservation test failure message
  must name the offending package and that no substantive summary was found (actionable, not a
  bare boolean). No silent narrowing: the skew report still lists every real finding.
- **Deferred scope**: (1) the feature-106 third finding — a planning artifact under-counting the
  doc surface (186 vs 356) — is an authoring/process-discipline issue, **no code fix, out of scope**;
  (2) the non-Controls boilerplate documentation cleanup itself is a separate future feature (this
  feature only removes the landmine that would block it); (3) the template's adoption of the natural
  `open FS.Skia.UI.Controls.Typed` (dropping 106's aliases) is deferred (SC-005 proves only that it
  is unblocked). No skew/reference-generation architecture redesign beyond these two narrow fixes.

### Constitution re-check (post-design)

No principle is implicated beyond II (visibility in `.fsi`) and VI (failing-first tests), both
satisfied: governance signatures live in their `.fsi`, each requirement ships a red-before/green-after
test. Tier 2 holds — no product contract shape change; the single additive baseline regeneration is
the captured-surface completeness fix the spec authorizes (FR-002/FR-007). **PASS.**

## Project Structure

```
build/Governance/
  PackageSkew.fs                # FR-001: strip comments before referencedSymbols extraction
  PackageSkew.fsi               # (unchanged shape unless a stripper is surfaced)
  PerPackageSurface.fs          # FR-002: captureCurrent recurses src/<dir>/**/*.fsi (picks up Widgets/*.fsi)
  PerPackageSurface.fsi         # (likely unchanged; stripper helpers already private)
  ControlsDocCoverage.fs(.fsi)  # source of isPlaceholderSummary / placeholderRegex (reuse for FR-004 "substantive")

scripts/
  generate-package-api-reference.fsx   # generator (unchanged) — embeds full .fsi + emits xml-summary-count

readiness/per-package-surface/
  FS.Skia.UI.Controls.fsi.txt   # FR-002: regenerated additively (now includes Widgets/*.fsi)

docs/api-surface/
  FS.Skia.UI.*.md               # regenerated references (unchanged content), read by the doc-preservation test

tests/Governance.Tests/
  Feature087GovernanceTests.fs  # retained 086-near-miss real-detection tests (FR-003) + new FR-001/FR-002 cases
                                # (a new Feature107*.fs file may host the new cases — decided at task gen)
tests/Package.Tests/
  PackageApiReferenceTests.fs   # FR-004/FR-005: replace boilerplate assertion with package-agnostic signal
```

**Key design decisions** (detail in [research.md](./research.md)):

- **FR-001 (comment stripping):** reuse the existing, proven strippers in
  `PerPackageSurface` (`stripBlockComments` nested-aware + `stripLineComment` covering
  `//`/`///`) rather than writing a second comment parser. Lift them to a shared helper so
  `PackageSkew.referencedSymbols` strips comments before its two regexes run — the live-code edge
  case (same symbol in a comment *and* live code) still resolves via the live occurrence.
- **FR-002 (capture broadening):** make `PerPackageSurface.captureCurrent` enumerate
  `*.fsi` recursively (`SearchOption.AllDirectories`) under the package source dir. Today
  `src/Controls/Widgets` is the **only** subdirectory with `.fsi` files (verified), so the change
  is additive and Controls-only in practice; the deterministic filename ordering must remain stable
  (sort by relative path). This makes the typed front door's public members captured known symbols,
  so a typo'd/unreleased *typed* member is still caught (edge case), not blanket-excluded.
- **FR-004/FR-005 (doc-preservation signal):** package-agnostic preservation = each tracked
  package's generated reference contains ≥1 `///`-prefixed line whose summary is **not** a
  placeholder (reuse `ControlsDocCoverage.isPlaceholderSummary`). `xml-summary-count > 0` is a
  weaker corroborating signal; the content check is the assertion. The FR-005 red-before fixture is
  a reference body with zero `///` lines.
```
