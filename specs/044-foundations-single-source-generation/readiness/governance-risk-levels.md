# Governance Risk Levels — Feature 044 (single-source generation, Stage 2.2–2.5)

This feature is **build-tooling / governance only** (`build/Governance/**` +
`build.fsx` + `.specify/**` + the skill trees); no runtime `src/**` is touched. It is a
`.specify/**` + skill-tree + governance-path change, so `Route` **escalates** it to the
full serialized FAKE gate set (FR-013/FR-015).

| Risk level | Scope | Authoritative validation |
|------------|-------|--------------------------|
| **small**  | routine framework-internal edits within this feature's own `build/Governance/*.fs` library work | focused `./fake.sh build -t Dev` + the `Governance.Tests` suite |
| **medium** | the three new build-tooling `.fsi`/`.fs` modules (`SkillTreeGen`, `SkillistView`, `ConstitutionFragments`), the reframed `build.fsx` gate arms, the `SkillExamplesCheck` retirement, the template marker regions | focused `Dev` + the targeted FAKE governance gates the `Route` selector prints |
| **broad**  | required here because this is a `.specify/**` + skill-tree + governance-path change that `Route` escalates | the full serialized FAKE gate order — see below |

## Required evidence and broad validation

The **required evidence** per risk level is named in the table above. **Broad
validation** (the full serialized FAKE order) is required here because `Route` escalates
this change.

The **broad** serialized order: `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` →
`GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`. Aggregate FAKE results are
recorded as **non-authoritative**; any race-like or environment-flaky gate failure (the
documented 039 `FsiTranscripts`/`SkiaViewer.Tests` libdecor-gtk flakes) is rerun in
focused isolation, and the focused rerun is the authoritative result.

Authoritative command: `./fake.sh build -t Route`. Artifact path:
`specs/044-foundations-single-source-generation/readiness/`. Failure class: governance.
Next action: run only the gates `Route` prints; for this escalated change, run them
sequentially in the deterministic order.
