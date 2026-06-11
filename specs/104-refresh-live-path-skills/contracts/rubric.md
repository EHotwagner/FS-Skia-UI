# Contract: SkillQualityCheck 7-section rubric

Every skill **edited or added** by this feature MUST satisfy all seven sections below. The gate
(`build/Governance/SkillQuality.fs`) detects them by heading/substring; the exact triggers are
listed so implementation does not accidentally drop one while editing.

| # | Section | How the gate detects it | Note for this feature |
|---|---|---|---|
| 1 | **Scope / when to use** | a heading containing `scope` or `when to use` | present in all three; keep |
| 2 | **Driven-library API** | a heading with `api`/`contract`/`driven`, OR the literal `.fsi`, OR "no backing library" | all three cite `.fsi` surfaces |
| 3 | **Runnable example** | ≥2 ``` fences (or "no backing library") | reconciliation + Controls already have ≥2; US3 must add ≥2 |
| 4 | **External research links** | ≥2 `http(s)://` URLs | reuse F#/.NET docs + a domain link (React reconciliation / SkiaSharp) |
| 5 | **Persistent-problem mandate** | heading `persistent problem`, OR phrase `official online docs first` | keep the one-line mandate phrase verbatim |
| 6 | **Related** | the literal `[[` (a wiki-link) | cross-links per C3; add the back-links |
| 7 | **Sources** | heading `sources` or `## sources` | keep/extend the Sources line |

**Sync contract (`SkillSync.fs`):** after editing any `.agents/skills/**` file and adding the new
one, the `.claude/skills/**` mirror MUST be byte-identical. Achieve by regeneration
(`RefreshSurfaceBaselines`), never by hand-editing `.claude/**`.

**Currency contract (`TargetMetadataDrift` / `SkillSyncCheck`):** the new `fs-skia-controls-host`
id MUST appear in the regenerated `template/base/docs/skillist-reference.md`.

**Acceptance:** `./fake.sh build -t SkillQualityCheck` reports PASS for all in-scope skills, and
`SkillSyncCheck` reports no `.agents`↔`.claude` drift.
