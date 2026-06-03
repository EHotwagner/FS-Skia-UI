# FR-001 Duplication Catalogue

The authority the rest of feature 057 is checked against. Every structural
duplication instance in the governed corpus, across the four classes, traced to
the validator that requires it today, with home files, proposed canonical
source, hybrid-by-consumer resolution (`DeleteScanCanonical` vs
`GenerateAndCheck`), and the currency gate that will guard each generated copy.
Genuine identical-content duplication (eligible for single-sourcing) is
distinguished from legitimate per-file variation (FR-011).

Source of truth for tokens/obligations: `build/Governance/Guidance.fs`
(`taskSkillistGuidanceCheck`, `controlsBoundaryGuidanceCheck`,
`validateSkillIdResolution`). Echo lines located by
`grep -rn "phrases for scans"`. Constitution variation points located by
`diff` of the three constitution files.

## Resolution legend

- **DeleteScanCanonical** — the duplicate exists only to feed an in-repo scanner
  (or is redundant with natural prose already in the same file); delete the copy
  and let the scanner read the canonical natural-prose block directly.
- **GenerateAndCheck** — the home file is a shipped/template-owned or
  `.agents`/`.claude` file an agent reads directly and cannot reach the source;
  the block stays present but is generated (`BEGIN/END GENERATED: gov/<id>`
  splice, or constitution render, or `.claude` skill-tree regen) and
  currency-checked.

---

## Class 1 — Per-file contract-token carriage (`evaluateGuidanceCheck` tokens)

| Id | Token(s) | Home files (N) | Requiring validator | Resolution | Canonical source | Currency gate |
| --- | --- | --- | --- | --- | --- | --- |
| `seh-token` | `[SEH]`, `synthetic-error-handling-approved` | 9: tasks-template ×2, speckit-tasks SKILL, speckit.tasks.md, speckit-implement SKILL, speckit.implement.md, constitution.md, constitution-template.md ×2 | `task-skillist-guidance` (`sehTokenFiles`) | **GenerateAndCheck** | `GovernedBlock gov/seh-token` | `TargetMetadataDrift` (constitution copies via constitution render) |
| `controls-pkg` | `FS.Skia.UI.Controls` | 6: controls/README, controls/skill SKILL, elmish/README, base/README, product.md, src/Controls/skill SKILL | `controls-boundary-guidance` | **GenerateAndCheck** | `GovernedBlock gov/controls-tokens` | `TargetMetadataDrift` |
| `controls-msg` | `Control<'msg>` | 4: controls/README, controls/skill SKILL, elmish/README, src/Controls/skill SKILL | `controls-boundary-guidance` | **GenerateAndCheck** | `GovernedBlock gov/controls-tokens` | `TargetMetadataDrift` |
| `controls-datagrid` | `DataGrid` | 7: controls/README, controls/skill SKILL, base/README, product.md, src/Controls/skill SKILL, spec-template ×2 | `controls-boundary-guidance` | **GenerateAndCheck** | `GovernedBlock gov/controls-tokens` | `TargetMetadataDrift` |
| `controls-elmish` | `FS.Skia.UI.Controls.Elmish` | 5: controls/skill SKILL, elmish/README, base/README, product.md, src/Controls/skill SKILL | `controls-boundary-guidance` | **GenerateAndCheck** | `GovernedBlock gov/controls-tokens` | `TargetMetadataDrift` |
| `skillist-empty` | `[skillist: []]` | 2: tasks-template ×2 | `task-skillist-guidance` | **GenerateAndCheck** | `GovernedBlock gov/skillist-tokens` | `TargetMetadataDrift` |
| `implement-fields` | `loaded_at`, `work_started_at`, `readiness/skill-loading-evidence.md` | 2: speckit-implement SKILL, speckit.implement.md | `task-skillist-guidance` (`implementFiles`) | **GenerateAndCheck** | `GovernedBlock gov/implement-fields` | `TargetMetadataDrift` |

**FR-011 exclusions (legitimate single occurrence — NOT duplication):**
`ControlsElmish.program` (1 file: elmish/README); `skillist:` / `deps:`
(1 file: tasks-deps-template.yml). A token in exactly one file is not duplicated
and is left in place.

---

### FR-011 reclassification (implementation finding — supersedes the Class 1 `GenerateAndCheck` column)

On migration each Class-1 contract token was inspected at every home file. The
`Guidance.fs` `ContractToken` requires only that the token appear as a
**substring** of each home file — it does **not** require identical surrounding
prose. In every case the token is embedded in genuinely **file-specific prose**
that legitimately differs per file, e.g. `[SEH]` reads
"is a **design/task-generation** classification…" in `speckit-implement`,
"Approved synthetic error-handling work uses `[SEH]` plus…" in
`tasks-template.md`, and "is an annotation, not a completion status…" in
`speckit-tasks`. Splicing an identical `gov/<token>` block into those files would
either (a) duplicate the token on top of the existing prose (adding lines, against
SC-002) or (b) destroy the meaningful per-file prose. Both are the exact case
**FR-011** excludes ("content that legitimately differs per file"). The same holds
for the Class-2 obligation concept phrases: each home file states the concept in
its own words and the obligation only requires the concept be present per its
`AnyOf`/`AllOf` mode — there is no identical block to collapse.

**Decision (user-confirmed 2026-06-03):** Class 1 and Class 2 are **FR-011
legitimate per-file variation**, not genuine identical-content duplication. They
are **left in place**, governed exactly as today by `evaluateGuidanceCheck` over
`Guidance.fs` (`GeneratedGuidanceCheck`). The one genuine identical-content
duplication these classes touched was the constitution copies of `[SEH]` and the
constitution skill-gate obligation prose — those collapse via the **Class 4**
constitution render (already single-sourced), so the `seh-token` rows for
`constitution.md` / `constitution-template.md` ×2 are covered there, not by a
separate `gov/seh-token` block.

The genuine identical-content duplication that *was* collapsed is **Class 3** (the
cross-file-identical visual-proof/owner phrase lines + the redundant in-file scan
echoes) and **Class 4** (the constitution triple). The `gov/seh-token`,
`gov/controls-*`, `gov/skillist-*`, `gov/persistent-launch`, `gov/seh-discipline`,
`gov/tasks-skill-gate`, `gov/implement-*` blocks proposed below are therefore
**not introduced** (they would carry per-file-varied prose, violating FR-011).

---

## Class 2 — Per-file obligation anchors (`GuidanceObligation` concepts)

The obligation concept phrases that must be present (per `Mode`) in every home
file. Each obligation already names every twin in its `Files` list, so drift in
one twin is caught today; single-sourcing makes the concept prose generated so
the copies cannot diverge.

| Id | Mode | Home files (N) | Resolution | Canonical source | Currency gate |
| --- | --- | --- | --- | --- | --- |
| `controls-skia-rendered` | AnyOf | 3: controls/README, controls/skill SKILL, src/Controls/skill SKILL | **GenerateAndCheck** | `GovernedBlock gov/controls-skia-rendered` | `TargetMetadataDrift` |
| `controls-no-charts-shim` | AllOf | 2: controls/skill SKILL, src/Controls/skill SKILL | **GenerateAndCheck** | `GovernedBlock gov/controls-no-charts-shim` | `TargetMetadataDrift` |
| `skillist-structured` | AnyOf | 4: tasks-template ×2, implement files ×2 | **GenerateAndCheck** | `GovernedBlock gov/skillist-prose` | `TargetMetadataDrift` |
| `skillist-minimal-ordered` | AnyOf | 4: same | **GenerateAndCheck** | `GovernedBlock gov/skillist-prose` | `TargetMetadataDrift` |
| `graph-before-after` | AnyOf | 4: same | **GenerateAndCheck** | `GovernedBlock gov/skillist-prose` | `TargetMetadataDrift` |
| `skillist-confidence-fields` | AllOf | 4: tasks-template ×2, tasks SKILL, speckit.tasks.md | **GenerateAndCheck** | `GovernedBlock gov/skillist-confidence` | `TargetMetadataDrift` |
| `skill-breadth` | AnyOf | 4: same | **GenerateAndCheck** | `GovernedBlock gov/skillist-confidence` | `TargetMetadataDrift` |
| `aggregate-non-authoritative` | AnyOf | 4: same | **GenerateAndCheck** | `GovernedBlock gov/skillist-confidence` | `TargetMetadataDrift` |
| `persistent-launch` | AnyOf | 2: tasks-template ×2 | **GenerateAndCheck** | `GovernedBlock gov/persistent-launch` | `TargetMetadataDrift` |
| `seh-discipline` | AnyOf | 6: tasks-template ×2, tasks SKILL, speckit.tasks.md, implement SKILL, speckit.implement.md | **GenerateAndCheck** | `GovernedBlock gov/seh-discipline` | `TargetMetadataDrift` |
| `tasks-skill-gate` | AllOf | 2: tasks SKILL, speckit.tasks.md | **GenerateAndCheck** | `GovernedBlock gov/tasks-skill-gate` | `TargetMetadataDrift` |
| `implement-skill-loading` | AllOf | 2: implement SKILL, speckit.implement.md | **GenerateAndCheck** | `GovernedBlock gov/implement-skill-loading` | `TargetMetadataDrift` |
| `constitution-skill-gates` | AllOf | 3: constitution.md + template ×2 | **GenerateAndCheck** | constitution render (placeholder source) | `TargetMetadataDrift` |
| `tasks-post-gen-timing` | AnyOf | 2: tasks-template ×2 | **GenerateAndCheck** | `GovernedBlock gov/skillist-prose` | `TargetMetadataDrift` |

**FR-011 exclusion:** `deps-skillist-doc` (1 file: tasks-deps-template.yml).

---

## Class 3 — In-file scanner echoes (`Exact … phrases for scans:`)

Each echo line and whether the pinned phrases also live in natural prose in the
**same** file (which decides `DeleteScanCanonical` vs keep-as-sole-carrier).
Phrase pins enforced by `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs`
(`expectFileContains`, substring-anywhere) and `validateSkillIdResolution`
(`advertisedSkillIdRegex` reads `-> <id>` lines).

| Id | Echo phrase | Home files | In-file prose duplicate? | Resolution |
| --- | --- | --- | --- | --- |
| `skill-phrases-echo` | `Exact skill phrases for scans:` + `… -> fs-skia-…` map | tasks-template ×2, speckit.tasks.md | **Yes** — same `-> id` map appears in the surrounding natural-prose paragraph (e.g. tasks-template.md:207) | **DeleteScanCanonical** (delete the echo; the `expectFileContains` pins survive in the prose line) |
| `skill-phrases-echo` (advertising) | same | speckit-tasks SKILL.md (+ `.claude` peer) | the `-> id` map is the **sole** carrier read by `advertisedSkillIdRegex` | **GenerateAndCheck** — the `-> id` advertising block stays; `.agents` canonical, `.claude` peer via `SkillSyncCheck` |
| `readiness-phrases-echo` | `Exact readiness phrases for scans: authoritative command.` | tasks-template ×2, speckit.tasks.md | **Yes** — "authoritative command" appears in the readiness-scaffold prose | **DeleteScanCanonical** |
| `visual-proof-echo` | `Exact visual proof rejection phrases for scans: …` | fs-skia-layout-evidence SKILL (+ `.claude` peer), product.md | **No** — the three rejection phrases are the **sole** carrier (only the *acceptance* cues appear in prose) | **GenerateAndCheck** — keep the line as canonical carrier in `.agents` SKILL (`.claude` peer via `SkillSyncCheck`); the `product.md` copy becomes `GovernedBlock gov/visual-proof-phrases` |
| `owner-phrases-echo` | `Exact owner phrases for scans: …` | fs-skia-layout-evidence SKILL (+ `.claude` peer), product.md | **No** — the eleven owner phrases are the **sole** verbatim carrier | **GenerateAndCheck** — same as above; `product.md` copy becomes `GovernedBlock gov/owner-phrases` |

**Key FR-011 finding:** the `visual-proof`/`owner` phrases are NOT in-file
redundant echoes — within `fs-skia-layout-evidence/SKILL.md` the echo line is the
only verbatim carrier of the rejection/owner phrases, so it is kept as canonical
prose. The genuine duplication is **cross-file** (the same line copied into
`template/base/docs/product.md`), which collapses via `GenerateAndCheck`. By
contrast, the `skill-phrases`/`readiness-phrases` echoes ARE in-file redundant
(the map/phrase already lives in the surrounding prose), so the echo is deleted.

---

## Class 4 — Constitution / template / fragment triple-maintenance

Canonical source after the collapse: the **placeholder-bearing**
`constitution-template.md` form (FR-007 / spec Clarification Q2). Render modes:
`constitution.md` = **Substituted**; the two `constitution-template.md` twins =
**Verbatim** (and byte-identical to each other — verified today). The 12 known
variation points between the concrete file and the template form:

| Id | Variation point | Twin (canonical, placeholder-bearing) | `constitution.md` (substituted) |
| --- | --- | --- | --- |
| `c-title` | document title | `# [PROJECT_NAME] Constitution` (+ `REQUIRED` comment) | `# FS-Skia-UI Constitution` |
| `c-principles-locked` | `## Core Principles` LOCKED banner | HTML comment present | stripped |
| `principle-v-body` | Principle V synthetic list wording | `` `NotImplementedException` placeholders, `failwith "TODO"` `` | `unfinished-code placeholder exceptions, TODO-style failing placeholders` |
| `principle-vi-body` | Principle VI skip mechanism | `` `[<Skip>]` attribute `` | `the relevant test-framework skip attribute` |
| `c-classification-locked` | Change Classification LOCKED banner | present | stripped |
| `c-constraints-tailorable` | Engineering Constraints TAILORABLE banner | present | stripped |
| `c-pack-output` | pack output path | `[PACK_OUTPUT_PATH]` | `~/.local/share/nuget-local/` |
| `c-logging` | logging library | `[LOGGING_LIBRARY]` | `not yet selected; see ADR when chosen` |
| `c-project-constraints` | project constraints | `[PROJECT_CONSTRAINTS]` | the SkiaSharp/Elmish/net10.0 paragraph |
| `local-agent-skills-intro` | Local Agent Skills intro paragraph | generic "Local skills under … are repository governance artifacts" | concrete "Capability skills are package-owned … prefer capability skills" |
| `local-agent-skills-list` | enumerated skill list | directive "enumerate the current … inventory here" | the 9-bullet `fs-skia-*` capability list |
| `c-skills-locked` / `c-gates-locked` | Local-Skills + Workflow LOCKED banners | present | stripped |
| `c-version` | version line | `[CONSTITUTION_VERSION]` / `[RATIFICATION_DATE]` / `[LAST_AMENDED_DATE]` | `1.3.0` / `2026-05-12` / `2026-05-27` |

The remaining ~300 lines of principle prose (Principles I–VII bodies, Change
Classification, Engineering Constraints, Workflow & Quality Gates, Governance)
are identical across all three files today and become the single canonical body.
The existing `ConstitutionFragments` first-sentence splice into the plan/tasks
templates is preserved and generalized to own these full bodies.

---

## Summary of canonical sources introduced (as-built)

Per the FR-011 reclassification above, only **genuine identical-content
duplication** was collapsed:

- `build/Governance/GovernedBlocks.fs` — `gov/visual-proof-phrases`,
  `gov/owner-phrases` (Class 3 cross-file-identical phrase lines).
- Placeholder-bearing constitution principle source (canonical
  `.specify/templates/constitution-template.md`) → renders
  `.specify/memory/constitution.md` (Substituted) + the preset twin (Verbatim)
  (Class 4).
- `.agents/skills/**` stay canonical for their `.claude/skills/**` peers
  (`SkillSyncCheck`, unchanged).

The Class-1 token blocks and Class-2 obligation blocks proposed in the original
draft (`gov/seh-token`, `gov/controls-*`, `gov/skillist-*`, `gov/persistent-launch`,
`gov/seh-discipline`, `gov/tasks-skill-gate`, `gov/implement-*`) are **not
introduced** — they are FR-011 legitimate per-file variation and remain governed
by `evaluateGuidanceCheck` over `Guidance.fs` as before.

Every generated copy that *is* introduced maps to a non-empty currency gate
(enumerated in `silent-drift-audit.md`, SC-005). `Guidance.fs` keeps the rule
*set* unchanged (FR-004); only the carriage of the genuinely-duplicated Class 3/4
content changes.
