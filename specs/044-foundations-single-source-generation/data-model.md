# Phase 1 Data Model: Single-Source Generation (Stage 2.2–2.5)

Typed entities for the three new pure governance modules in `FS.Skia.UI.Build`. All types are
plain F# records/DUs (Principle III); all functions over them are pure (Principle IV) with the
filesystem I/O kept at the `build.fsx` interpreter edge. Curated `.fsi` surfaces are in
[contracts/](./contracts/).

---

## 1. Skill-tree generation (`SkillTreeGen`) — US1

### `SkillFile`
A single skill document read from a tree.

| Field | Type | Notes |
|---|---|---|
| `Slug` | `string` | skill directory name, e.g. `"fsharp-parsing"` (25 today) |
| `RelPath` | `string` | tree-relative path, e.g. `"fsharp-parsing/SKILL.md"` |
| `Bytes` | `byte[]` | raw file bytes (byte-identity is over these) |

### `GenPlanEntry`
One derived-file write the generator prescribes.

| Field | Type | Notes |
|---|---|---|
| `DerivedRelPath` | `string` | `.claude/skills/`-relative target path |
| `Bytes` | `byte[]` | canonical bytes, reproduced unchanged (FR-003) |

### `SkillTreePlan`
The full derivation plan + the provenance manifest.

| Field | Type | Notes |
|---|---|---|
| `Entries` | `GenPlanEntry list` | one per canonical `SKILL.md`, by enumeration (FR-002) |
| `ManifestRelPath` | `string` | tree-level provenance file path (R5), e.g. `"GENERATED.md"` |
| `ManifestBytes` | `byte[]` | provenance text: canonical source + regeneration command (FR-011) |

### `SkillCurrency`
Result of comparing a committed derived tree against the plan.

| Field | Type | Notes |
|---|---|---|
| `StaleSlugs` | `string list` | derived `SKILL.md` whose bytes differ from canonical |
| `MissingDerived` | `string list` | canonical slugs with no derived counterpart |
| `OrphanDerived` | `string list` | derived files with no canonical source (hand-added drift) |

**Validation rules**
- Enumeration is over the canonical tree, never a hardcoded slug list (the old `expectedSlugs` is
  deleted) — adding a skill needs zero allowlist edits (SC-002).
- A derived `SKILL.md` MUST be byte-identical to canonical (FR-003); any difference is `StaleSlugs`.
- Empty/missing/unreadable canonical input MUST raise a generator error, never an empty plan
  (spec Edge Case, Principle VII).
- Currency is "clean" iff `StaleSlugs`, `MissingDerived`, and `OrphanDerived` are all empty.

---

## 2. Skillist view (`SkillistView`) — US2

The canonical source is `tasks.deps.yml` `skillist:` (already parsed by `Evidence.DepsParser` into
`DepsEntry.Skillist: string list option`); the derived view is the `tasks.md` `[skillist: …]`
annotation (already parsed by `Evidence.TaskParser` into `TaskRecord.SkillistMirror: string list
option`). This module renders the derived view from the canonical source and reports currency.

### `SkillistCurrencyItem`
Per-task currency result, **active feature only**.

| Field | Type | Notes |
|---|---|---|
| `TaskId` | `string` | e.g. `"T007"` |
| `Canonical` | `string list` | deps `skillist:` for the task |
| `DerivedNow` | `string list option` | parsed `tasks.md` annotation (`None` = annotation missing) |
| `ExpectedAnnotation` | `string` | rendered token, e.g. `"[skillist: fsharp-parsing, fsharp-io-globbing]"` |
| `IsStale` | `bool` | `DerivedNow <> Some Canonical` |

**Validation rules**
- Rendering: a non-empty list → `[skillist: a, b, c]` (comma-space separated, order preserved from
  canonical); an empty list → `[skillist: []]`.
- Splice replaces only the bracketed token on the matched task line; all other bytes of the line and
  the file are preserved (R3).
- A missing annotation (`DerivedNow = None`) is reported as invalid, not silently inserted
  (constitution Local Agent Skills: "omitted metadata is invalid").
- Scope is the active feature resolved from `.specify/feature.json`; historical features are never
  re-derived (FR-007, SC-004).

---

## 3. Constitution fragments (`ConstitutionFragments`) — US3

### `PrincipleFragment`
A generated principle-summary unit.

| Field | Type | Notes |
|---|---|---|
| `FragmentId` | `string` | stable id used in the marker, e.g. `"tests-first"`, `"mvu-boundary"` |
| `SourceHeading` | `string` | the `### <Principle>` heading it derives from |
| `RenderedText` | `string` | the deterministic summary spliced into the template |

### `MarkerRegion`
A located `BEGIN/END GENERATED` region inside a template.

| Field | Type | Notes |
|---|---|---|
| `FragmentId` | `string` | matches `BEGIN GENERATED: constitution/<id>` |
| `StartLine` | `int` | line of the BEGIN marker (1-based) |
| `EndLine` | `int` | line of the END marker (1-based) |
| `CurrentInner` | `string` | the text currently between the markers |

### `FragmentCurrency`
Currency result for one template.

| Field | Type | Notes |
|---|---|---|
| `TemplatePath` | `string` | e.g. `".specify/templates/tasks-template.md"` |
| `StaleFragments` | `string list` | fragment ids whose region content differs from the re-derived text |
| `UnknownMarkers` | `string list` | marker ids present in the template with no source fragment |
| `MissingMarkers` | `string list` | source fragments expected in the template but absent |

**Validation rules**
- The splice replaces **only** the inner text of each `BEGIN/END` pair; every byte outside every
  marker pair is preserved (FR-010) — proven by a byte-equality assertion over the non-region spans.
- The fragment set is fixed and enumerated in the module; extraction from a `### Principle` heading
  is deterministic (no free-form paraphrase) so a principle edit changes the fragment reproducibly.
- Currency is "clean" iff `StaleFragments`, `UnknownMarkers`, and `MissingMarkers` are empty for
  every governed template.

### Fixed fragment set (initial — locked by the Phase-1 contract)

| `FragmentId` | Source `### Principle` | Target template region(s) |
|---|---|---|
| `tests-first` | VI. Test Evidence Is Mandatory (+ I.) | `tasks-template.md` "Tests First (Principle I, Principle VI)" |
| `mvu-boundary` | IV. Elmish/MVU Is the Boundary… | `tasks-template.md` Elmish/MVU `[X]`-evidence note |
| `synthetic-disclosure` | V. Synthetic Evidence Requires… | `tasks-template.md` `[S]` legend + Synthetic-Evidence Inventory intro |
| `fsi-visibility` | II. Visibility Lives in `.fsi` | `plan-template.md` `.fsi`/contract-impact decision prompt |

*(The exact line anchors are fixed during implementation against the live templates; the table is
the authoritative fragment inventory. Adding/removing a fragment is a contract change, not an
implementation detail.)*

---

## Relationships

```
.agents/skills/*/SKILL.md ──(SkillTreeGen.plan, enumerate)──▶ .claude/skills/*/SKILL.md (+ manifest)
                          ◀─(SkillTreeGen.currency)── compared at SkillSyncCheck gate

tasks.deps.yml skillist:  ──(SkillistView.render / splice)──▶ tasks.md [skillist: …]
        (canonical)       ◀─(SkillistView.currency, active feature)── compared in Evidence.Audit

constitution.md ### Principle ──(ConstitutionFragments.render / splice)──▶ template BEGIN/END regions
        (canonical)            ◀─(ConstitutionFragments.currency)── compared at TargetMetadataDrift gate
```

All three regeneration writes (skills, constitution) emit from `RefreshSurfaceBaselines`; the
skillist annotation regenerates in the active-feature evidence path. All three currency checks reuse
existing gates (no new gate target). Diagnostics name `./fake.sh build -t RefreshSurfaceBaselines`.
