# Feature Specification: Governance Skew & Doc-Check Hardening

**Feature Branch**: `107-governance-skew-doc-hardening`
**Created**: 2026-06-12
**Status**: Draft
**Input**: User description: "create specs to fix what can be reasonable fixed."

> **Origin.** A retrospective on feature 106 (controls-api-discoverability) surfaced two
> governance foot-guns that are not one-off test churn but **recurring, structural** problems
> the next feature will hit again. This feature fixes the two that are reasonably fixable in the
> governance home; the third finding (a planning artifact under-counting the doc surface) is an
> authoring-discipline issue with no code fix and is recorded as out of scope.
>
> 1. **The package-skew check cannot validate the recommended authoring path.** The
>    static package-skew check (feature 087) extracts "referenced FS.Skia.UI symbols" from
>    generated template/sample source and fails the build on any symbol absent from the captured
>    per-package surface. Two defects make it produce **false skew findings** on legitimate code:
>    (a) it scans **raw source including comments**, so prose or a doc-comment that merely names a
>    framework namespace (e.g. `FS.Skia.UI.Controls.Typed`) is treated as a live reference; and
>    (b) the captured per-package surface it checks against **omits the typed front door**
>    entirely — the `Widgets/*.fsi` files and the `FS.Skia.UI.Controls.Typed` sub-namespace are
>    not captured — so `open FS.Skia.UI.Controls.Typed` (the path feature 106 now recommends and
>    the generated starter uses) resolves to an unknown symbol. Feature 106 only **worked around**
>    this (explicit module aliases + a reworded comment); the next consumer or template that opens
>    the typed namespace, or documents it in prose, hits it again.
> 2. **The "XML summaries are preserved" check depends on boilerplate existing.** The package
>    API-reference governance check proves the reference generator preserves `///` summaries by
>    asserting the *placeholder boilerplate sentence* is present in each package's reference.
>    Feature 106 removed that boilerplate from the Controls surface, so the check had to be
>    special-cased. The same check still asserts the boilerplate is present in the **other**
>    packages (Scene, Testing); the deferred non-Controls documentation cleanup (explicitly noted
>    in feature 106) will remove it there too and **re-break the check the same way**.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - The recommended typed authoring path passes governance (Priority: P1)

A consumer (or the generated template/sample) authors controls through the typed front door —
opening the `FS.Skia.UI.Controls.Typed` namespace and referencing its modules — and naturally
mentions framework namespaces in comments/doc-comments. The static package-skew governance check
passes with **zero false findings**, so authors are not forced into awkward work-arounds
(per-control module aliases, avoiding the namespace in prose) just to satisfy a check.

**Independent test**: A source file that (a) `open`s `FS.Skia.UI.Controls.Typed`, (b) references
typed modules, and (c) names `FS.Skia.UI.…` paths inside comments produces **no** package-skew
findings — while a sibling file that references a genuinely absent/unreleased FS.Skia.UI symbol
**still** produces a finding (real detection intact).

### User Story 2 - Documenting more of the surface never breaks the doc-preservation check (Priority: P2)

A maintainer documents additional packages' public surface (e.g. the deferred non-Controls
boilerplate cleanup), removing placeholder summaries. The package API-reference
"summaries-are-preserved" governance check keeps passing, because it verifies preservation by a
**package-agnostic** signal rather than by the continued presence of the placeholder sentence.

**Independent test**: With the placeholder boilerplate removed from *every* tracked package's
reference (simulated by a fixture or by documenting one more package), the API-reference
preservation check still passes; and it still fails if the generator genuinely drops `///`
summaries.

### Edge Cases

- A real skew — a generated reference to an FS.Skia.UI symbol not present in the captured surface
  (the 086 near-miss class) — MUST still be caught. The fix must narrow false positives, not
  blanket-pass.
- A reference that appears in BOTH a comment and live code in the same file MUST still be checked
  via its live-code occurrence (comment-stripping must not blind the check to real references).
- The typed front door's own public members MUST become part of the captured surface the check
  validates against (so a typo'd or unreleased typed member is still caught), not merely
  excluded from checking.
- The doc-preservation check MUST remain meaningful — it must still fail if reference generation
  silently drops summaries — i.e. the fix replaces a brittle sample, it does not delete the
  guarantee.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The static package-skew check MUST NOT treat an `FS.Skia.UI.…` token that appears
  only inside a source comment (single-line `//`/`///` or block `(* … *)`) as a referenced
  symbol; only references in live code contribute skew signals.
- **FR-002**: A reference to a real sub-namespace of a tracked package — specifically the typed
  front door `FS.Skia.UI.Controls.Typed` and its members — MUST resolve cleanly (no skew
  finding). Resolution: the captured per-package surface MUST include the typed front door
  (`Widgets/*.fsi`) so its namespace segments and public members are known symbols, and/or the
  skew resolver MUST recognize a path whose prefix is a declared package sub-namespace. Either
  way, `open FS.Skia.UI.Controls.Typed` and `FS.Skia.UI.Controls.Typed.<Module>.<member>` are
  not false findings.
- **FR-003**: Real skew detection MUST be preserved: a generated reference to an FS.Skia.UI
  symbol that is genuinely absent from the captured surface still produces a finding, and the
  comment/sub-namespace narrowing introduces no path by which an unreleased symbol slips through.
- **FR-004**: The package API-reference "XML summaries are preserved" governance check MUST
  verify summary preservation by a package-agnostic signal (e.g. each package's reference carries
  at least one substantive, non-placeholder summary, and/or the generator demonstrably
  round-trips a known summary) rather than by asserting the placeholder boilerplate sentence is
  present in any package — so documenting any package's surface does not break the check.
- **FR-005**: The doc-preservation check MUST still FAIL if the reference generator drops `///`
  summaries (the guarantee is retained, only its brittle sample is replaced), proven by a
  red-before fixture.
- **FR-006**: Both fixes MUST live in the single governance home (`build/Governance/**` and its
  tests under `tests/Governance.Tests`/`tests/Package.Tests`) and ship with red-before/green-after
  governance tests demonstrating each false positive is gone and each real detection is retained.
- **FR-007**: No product behavior, public product `.fsi` signature, or generated runtime output
  changes. If the per-package surface capture is broadened to include the typed front door
  (FR-002), the regenerated baseline is an additive, reviewed surface-capture change, not a
  product contract change.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This section names
> concrete packages, modules, build targets, and evidence paths by design.

- **Package impact**: No package identity or version change from this feature itself (the merge
  flow bumps libraries as usual). Package **contents** are unaffected. The captured **per-package
  surface baseline** for `FS.Skia.UI.Controls` may grow to include the typed front door
  (`Widgets/*.fsi`) if FR-002 is resolved by broadening the capture — an additive baseline
  regeneration, reviewed, not a runtime change.
- **Public contract impact**: No product `.fsi` signature shape change. The governance modules
  (`build/Governance/PackageSkew.fs`, the package API-reference test) change; `validation.contract.yml`
  is unaffected (no new gate). If the per-package capture is broadened, the captured baseline
  (`readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`) is regenerated.
- **State workflow impact**: None. The package-skew analysis and the reference check are pure
  text analyses; no stateful workflow, I/O, command, effect, subscription, or interpreter change.
- **Layout/rendering impact**: None. No Vulkan/Skia/screenshot/visual-output change.
- **Evidence obligations**: Red-before/green-after governance tests for FR-001/FR-002/FR-003
  (skew false-positive vs real-detection) and FR-004/FR-005 (doc-preservation package-agnostic +
  still-fails-on-drop); `PackageSurfaceCheck` green (incl. any regenerated per-package baseline);
  `EvidenceGraph` + `EvidenceAudit` verdict PASS with 0 synthetic.
- **Unsupported scope**: The feature-106 retrospective's third finding — a planning artifact
  under-counting the doc surface (186 vs 356) — is an authoring/process-discipline issue with no
  code fix and is **out of scope**. This feature does not document the deferred non-Controls
  boilerplate itself (it only ensures the doc-preservation check won't break when that work
  later lands). No new governance gate is added. No redesign of the skew or reference-generation
  architecture beyond the two narrow fixes.
- **Build-target impact**: `PackageSurfaceCheck` exercises the changes (and any regenerated
  baseline); `Dev`/`Verify` run the new governance tests; `RefreshSurfaceBaselines` regenerates
  the per-package baseline if the capture is broadened (FR-002). No `TemplateCheck`,
  `GeneratedProductCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, or new
  gate change is required.

## Success Criteria *(mandatory)*

- **SC-001**: A template/sample source file that opens `FS.Skia.UI.Controls.Typed`, references
  its modules, and names framework namespaces in comments produces **0** package-skew findings —
  verified by a governance test that fails before the fix and passes after.
- **SC-002**: With the placeholder boilerplate absent from every tracked package's reference, the
  API-reference "summaries-preserved" check passes; and it still fails when summaries are dropped
  — both verified by fixtures.
- **SC-003**: A genuinely absent/unreleased FS.Skia.UI symbol reference is still detected as skew
  after the fix (0 loss of real coverage), verified by the retained 086-near-miss test class.
- **SC-004**: 0 governance/package test regressions; `PackageSurfaceCheck` green; `EvidenceGraph`
  + `EvidenceAudit` verdict PASS with 0 synthetic.
- **SC-005**: After this feature, the generated template could author the typed front door via a
  natural `open FS.Skia.UI.Controls.Typed` (rather than the per-control aliases feature 106 was
  forced into) without a package-skew failure — demonstrating the work-around is no longer
  required.

## Key Entities

- **Referenced symbol (skew input)**: an `FS.Skia.UI.*`-rooted token extracted from generated
  template/sample source; must now be sourced from **live code only** and resolved against a
  surface that **includes the typed front door**.
- **Captured per-package surface**: the set of public symbols (per `readiness/per-package-surface/
  *.fsi.txt`) the skew check validates against; the gap is that it omits `Widgets/*.fsi` /
  `FS.Skia.UI.Controls.Typed`.
- **Doc-preservation signal**: the package-agnostic evidence that the API-reference generator
  carries `///` summaries through — to replace the placeholder-sentence sample.

## Assumptions

- The intent of "fix what can be reasonably fixed" is the two recurring, code-fixable governance
  foot-guns from the feature-106 retrospective (the skew check's comment-scanning + missing typed
  surface, and the doc-preservation check's dependence on boilerplate), not a broad rewrite of the
  governance tooling.
- Broadening the per-package surface capture to include the typed front door is acceptable and
  desirable (the typed `Widgets/*.fsi` are genuinely public and already appear in the
  `docs/api-surface/` bundle from feature 089), so making the per-package baseline consistent with
  the bundle is an improvement, not scope creep.
- The deferred non-Controls documentation pass is a separate future feature; this feature only
  removes the landmine that would block it.
