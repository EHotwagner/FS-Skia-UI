# Phase 0 Research: Governance Skew & Doc-Check Hardening

All unknowns resolved against the live governance code; no NEEDS CLARIFICATION remains.

## D1 — Where the package-skew false positives come from

**Decision:** Two independent defects in `build/Governance/PackageSkew.fs` +
`build/Governance/PerPackageSurface.fs`, fixed independently.

- `PackageSkew.referencedSymbols` (PackageSkew.fs:56-74) runs two regexes over the **raw**
  `sourceText`: `(?m)^\s*open\s+(FS\.Skia\.UI…)` and `\bFS\.Skia\.UI(?:\.ident)+`. Neither strips
  comments, so a `///`/`//`/`(* *)` token like `FS.Skia.UI.Controls.Typed` in prose is extracted
  as a referenced symbol leaf. (FR-001.)
- `PerPackageSurface.captureCurrent` (PerPackageSurface.fs:197-214) captures the package surface
  via `Directory.GetFiles(dir, "*.fsi")` — **non-recursive** — so `src/Controls/Widgets/*.fsi`
  (the typed front door, 14 files) is never captured. `open FS.Skia.UI.Controls.Typed` and
  `FS.Skia.UI.Controls.Typed.<Module>.<member>` therefore resolve to unknown symbols → false skew.
  (FR-002.)

**Rationale:** Confirmed by reading the live baseline
`readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` (no `Typed`/`Widgets` symbols) and the
regexes. Feature 106 only worked around this (per-control module aliases + reworded comment).

**Alternatives considered:** (a) Blanket-exclude any `FS.Skia.UI.Controls.Typed.*` path in the
skew resolver — rejected: the spec edge case requires typed members to be *captured* so a typo'd or
unreleased typed member is still caught, not excluded from checking. (b) A bespoke comment parser in
`PackageSkew` — rejected: `PerPackageSurface` already has a nested-aware `stripBlockComments` and a
`stripLineComment` covering `//`/`///`; reuse them (single home of rules).

## D2 — FR-001 comment stripping: reuse, don't reinvent

**Decision:** Lift `PerPackageSurface`'s `stripBlockComments` + `stripLineComment` into a shared
helper (or expose a single `stripComments`) and apply it to `sourceText` at the top of
`PackageSkew.referencedSymbols` before the two regexes run.

**Rationale:** Both modules live in `build/Governance`; one comment-stripping implementation keeps
the rules single-sourced and already battle-tested (it preserves newlines so line structure / the
`(?m)^\s*open` anchor still works). The live-code edge case (same symbol in a comment **and** live
code) is satisfied automatically: stripping removes only the comment occurrence, the live-code
occurrence survives and still contributes the reference.

**Alternatives considered:** Strip comments in the build-front caller `runPackageSkewCheck`
(Governance.fs:~727) instead — rejected: putting it in the pure core makes `referencedSymbols`
correct and directly unit-testable (the FR-001 red-before test calls `referencedSymbols`).

## D3 — FR-002 capture broadening: recursive enumeration

**Decision:** Change `captureCurrent` to enumerate `*.fsi` with
`SearchOption.AllDirectories` under the package source dir, ordering deterministically by relative
path (so multi-file packages still collapse to one stable surface). Regenerate the Controls
per-package baseline via `RefreshSurfaceBaselines`.

**Rationale:** Verified `src/Controls/Widgets` is the **only** subdirectory under any in-scope
package source dir that contains `.fsi` files; the internal-module subdir convention (feature 105)
keeps internal modules in subdirs **without** `.fsi`, so a recursive `*.fsi` sweep captures only
genuinely-public surface. The change is additive (the baseline grows; nothing is removed) and, in
practice, Controls-only. Broadening makes the typed front door's members *known symbols*, satisfying
both FR-002 (no false finding) and the edge case (typo'd typed member still caught).

**Alternatives considered:** Hard-code a `Widgets`-only second glob for the Controls package —
rejected: more special-casing, and recursion is both simpler and future-proof for any later public
subdir surface. Risk if a future package adds an internal subdir `.fsi`: it would leak into the
public baseline — mitigated by the existing no-`.fsi`-for-internal convention and caught at baseline
review; noted for task-gen.

## D4 — FR-004/FR-005 package-agnostic doc-preservation signal

**Decision:** Replace the `PackageApiReferenceTests` assertion that the **placeholder boilerplate
sentence** ("Public contract type exposed by this FS.Skia.UI package.") is present in Scene/Testing
with a package-agnostic content check applied to **every** tracked package reference: the generated
reference must contain **≥1 `///`-prefixed summary line that is not a placeholder**
(reuse `ControlsDocCoverage.isPlaceholderSummary` / `placeholderRegex` to classify). FR-005's
red-before fixture is a reference body whose embedded signatures carry **zero** `///` lines.

**Rationale:** Verified the generator (`scripts/generate-package-api-reference.fsx:114-156`) embeds
each package's full `.fsi` verbatim in a "## Curated Signatures" block and computes
`xml-summary-count` over that **emitted** body (line 117) — so the embedded `///` lines are a
faithful preservation signal, not a self-report decoupled from output. A content check that "at
least one substantive summary survived" proves preservation for any package regardless of whether it
still carries placeholder boilerplate, so the deferred non-Controls doc cleanup cannot re-break it.
The guarantee is retained: drop the `///` summaries from the source `.fsi` and the embedded body has
zero `///` lines → the check fails.

**Alternatives considered:** (a) Assert `xml-summary-count > 0` only — weaker: a header field is
less direct than inspecting the embedded body, and a non-substantive (placeholder-only) summary would
still satisfy a bare count. Use it as a corroborating secondary assertion, not the primary. (b)
Round-trip a *specific known* summary string per package — rejected: re-introduces package-specific
brittleness, the exact failure mode being removed. (c) Keep Controls' bespoke `typed `Props``
assertion as the universal signal — rejected: package-specific.

## D5 — Real-detection preservation (FR-003) and routing

**Decision:** Retain the existing 086-near-miss tests in
`tests/Governance.Tests/Feature087GovernanceTests.fs` (the seeded
`FS.Skia.UI.Controls.ControlRenderResult.UnreleasedBoundsV087` reference must still be a finding) and
add an explicit edge-case test for "symbol in comment AND live code → still found via live code."
No `Routing.fs` change: editing `build/Governance/**/*.fs` + `tests/**` keeps the existing
`package-surface` / governance routing; `PackageSurfaceCheck` + `Dev`/`Verify` already cover it.
`validation.contract.yml` is generated from `Routing.fs` and needs no edit (no new gate).

**Rationale:** Narrowing false positives (comments) and *adding* genuinely-public symbols to the
captured surface can only make the check pass on more legitimate code — it introduces no path by
which a genuinely-absent symbol resolves, because absent symbols are still absent from the (larger)
surface. The retained seeded test is the guard.
