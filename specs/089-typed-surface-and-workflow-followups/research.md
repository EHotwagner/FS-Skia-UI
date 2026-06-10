# Research: Typed Front-Door Discoverability & Spec-Kit Workflow Followups

All four findings are governance/docs-tree work over existing seams. Each decision below resolves a
NEEDS CLARIFICATION from the Technical Context and cites the concrete source seam.

---

## R1 — TYPED-SURFACE-1: how to publish the typed front door (FR-001..FR-004)

**Decision.** Two complementary, single-source moves:

1. **Enroll the 14 `src/Controls/Widgets/*.fsi` into the published api-surface** by adding them to
   the Controls `contracts:` list in `template/capabilities.yml`. `ApiSurfaceGen.plan`
   (`build/Governance/ApiSurfaceGen.fs:44-62`) collects every `.fsi` named in a capability's
   `Contracts`, and `regenerateApiSurface` (`build/Governance/Front/Governance.fs:217-242`) copies
   each **byte-identically** into `template/base/docs/api-surface/Controls/<file>.fsi`. The `.fsi`
   already declares each `*Props` record (field names; optional fields are `option`-typed), the
   `module X` with its `view`/`defaults` signatures, and the event callbacks — so FR-001's richness
   comes straight from the source of truth with **zero hand-authored prose**.

2. **Add a thin `TypedModule` index field to `CatalogGen.TypedCatalogFact`**
   (`build/Governance/CatalogGen.fs:14-22`) and render it into the `catalog.yml` row, giving the
   per-control `id → typed-module` linkage. This is needed because the existing `Module` field is
   the *legacy* module token and does **not** reliably name the typed module: e.g. `list-view`,
   `list-box`, `multi-select-list`, `combo-box`, `tree-view`, `scroll-viewer`, `split-view` all carry
   `Module = "Collections"` (`CatalogGen.fs:76-89`) but their typed Props live in
   `CollectionsWidgets.fsi` as `ListViewProps`/`ListBoxProps`/… under `module ListView`/`module ListBox`/….
   The new field is a **single token per control** (structural metadata, like `Module`/`Events`
   already are) — it does **not** duplicate field names, so FR-002's "single source, no hand-authored
   duplicate prose that could drift" is preserved (the field-level richness stays in the enrolled `.fsi`).

**Rationale.** This honors the spec's own conflict resolution (spec.md:150-153): "the surface is
projected from the typed front door itself … richness comes from the source of truth, never from
hand-maintained duplicate prose." Enrolling the `.fsi` is the richness; the `TypedModule` token is a
pointer, not a copy. Both ride **existing** currency machinery:
- api-surface drift → `ApiSurfaceGen.currency` (`ApiSurfaceGen.fs:64-97`) inside `TargetMetadataDrift`
  (`build/Governance/Engine/Update.fs:834-857`) — fails if an enrolled source `.fsi` is missing,
  stale, or orphaned, directing `RefreshSurfaceBaselines`.
- catalog drift → `CatalogGen.currency` (`CatalogGen.fs:262-289`) over both `catalog.yml` and
  `Catalog.fs`, naming the file + control id + regen command.

**Coverage check (FR-004 whole-catalog).** All 14 Widgets modules have **both** `.fs` and `.fsi`
(verified: 14/14). Enrolling all 14 `.fsi` captures every typed `*Props`/`view` in the catalog,
including the stateful `CollectionModel`/`TextInputModel`-backed controls and the typed-only
breadth-expansion controls (toggle/split button, date/time/color pickers). `custom-control` is
bridge-typed (`Widget.ofControl`, no Props schema) — its `TypedModule` points at
`CustomControlWidget` and its row keeps `RequiredAttributes = []` as today (`CatalogGen.fs:114-116`).

**Alternatives considered.**
- *Extend `TypedCatalogFact` with full Props-field facts (field names, optionality) and regenerate a
  prose doc.* Rejected — re-types the `.fsi` content into the governance source, exactly the
  hand-maintained duplicate the spec's resolution note forbids; guaranteed drift surface.
- *Reuse the existing `Module` field as the typed pointer.* Rejected — ambiguous for the ~7
  Collections-family controls (and any other legacy/typed name mismatch); a consumer following
  `Module = "Collections"` cannot find `ListViewProps`.
- *Emit only the `.fsi`, no index.* Rejected — US1's independent test requires, *for every control*,
  "its typed module" be discoverable; without the id→module index the consumer must guess which of
  the 14 files / many modules holds a given control's Props.

**Open implementation choice (deferred to tasks, both single-source):** whether the id→module index
surfaces in `catalog.yml` only, or *also* gets a generated `docs/api-surface/Controls/` index page via
`CatalogDocsGen`. `catalog.yml` alone satisfies FR-003 ("the published api-surface tree **and/or** the
consumer-visible `catalog.yml`"); a docs index is a nice-to-have, not required.

---

## R2 — VERIFY-IMPL-1: run-and-use discipline in speckit-implement (FR-005..FR-007)

**Decision.** Insert a new interactive-UI gate into the per-task workflow of
`.agents/skills/speckit-implement/SKILL.md`, immediately **after the existing Workflow step 6**
(the "run the verification appropriate for the phase" step, `SKILL.md:209-213`) and **before step 7**
(the `tasks.md` status write, `:214-217`). The new step requires, for any interactive-UI `[US*]` story:
invoke the `run`/`verify` skill discipline (launch the host app + interact via pointer/keyboard), and
confirm the captured evidence exercised the **production render path** (the real user-reachable
surface — `controlsExampleView` → `Control.renderTree`), **not** an author-built parallel scene. A
truthful screenshot of the wrong render path does not satisfy the step. The step is a no-op for
non-interactive stories. Then regenerate `.claude/skills/speckit-implement/SKILL.md` via
`RefreshSurfaceBaselines` (FR-007: present in both source and mirror).

**Rationale.** The skill already has a "Vertical-slice rule" (`SKILL.md:82-105`) and step 6 requires a
"user-reachable exercise", but nothing forces *launching and using the live app* nor *confirming the
render path* — which is exactly how "28 tests + 2 gates + 11 screenshots" shipped a non-interactive
mockup. Placing the new requirement between "verify" and "mark done" makes it a hard precondition of
`[X]` on an interactive `[US*]`. The `run` and `verify` skills are platform skills that launch and
drive the real app — referencing them by name keeps the discipline durable and tool-backed.

**Constraint confirmed.** `speckit-*` skills are **excluded** from `SkillQualityCheck`'s 7-section
rubric (`build/Governance/SkillQuality.fs`, `isSpeckit` guard), so the edit is free-form prose; it is
governed by `SkillSyncCheck` (byte-identity `.agents` ↔ `.claude`) only. No mandatory headings to
preserve.

**Alternatives considered.** Adding the rule to the Vertical-slice section instead of the per-task
loop — rejected; the loop is where `[X]` is written, so the gate belongs adjacent to the status write.

---

## R3 — EVGRAPH-ECHO-1: skillist id→path echo in EvidenceGraph (FR-008, FR-009)

**Decision.** Add a resolution section to the rendered task-graph markdown produced by
`Evidence/Render.fs` `taskGraphMd` (`build/Governance/Evidence/Render.fs:186-345`), appended **after
the existing "Resolved skillist ids" section (`:319-330`)**. For each **distinct** skillist id across
all tasks, emit one line reusing the **exact** resolution `Audit.fs:150-162` already performs:
- `registry.Skills.TryFind id` → `Some [path]` ⇒ `id → <path>` (resolved);
- `Some [p1; p2; …]` ⇒ `id → ambiguous: p1, p2` (flagged);
- `None` → `registry.DirectoryAliases.TryFind id` → `Some (acceptedId, path)` ⇒
  `id → directory-name for <path> (accepted id: acceptedId)` (flagged);
- otherwise ⇒ `id → UNRESOLVED (not registered/readable)` (flagged, FR-009).

Thread the `SkillRegistry` into `taskGraphMd` — it is already present in `EvidenceInputs`
(`Evidence/Engine.fs`) and the registry is built at the call site
(`Front/Governance.fs:804`, `SkillRegistry.build repoRoot`), so no new resolution logic is introduced
(FR-008: "same resolution the validator already performs"). Add a small pure helper
(`skillistResolution: SkillRegistry -> string list -> string`) to `Render.fs`/`.fsi` and unit-test it
directly. Output lands in `readiness/task-graph.md` (and the verdict summary in
`logs/evidence-graph.txt`) where the gate already writes.

**Rationale.** The ambiguity that bit the consumer (`controlsshowcase1-widgets` token that is really
the `name:` of `fs-skia-ui-widgets/`) is precisely the `DirectoryAliases` case — surfacing that
branch in the output replaces the manual `grep '^name:'`. Reusing `Audit.fs`'s resolution guarantees
the echo agrees with the validator's pass/fail semantics.

**Alternatives considered.** Emitting counts only, or a new file — rejected; the spec wants the
per-token resolution *in the same output*, distinct resolved vs unresolved sections.

---

## R4 — CLARIFY-SOURCE-1: source-spec pre-check in speckit-clarify (FR-010, FR-011)

**Decision.** Insert a step into `.agents/skills/speckit-clarify/SKILL.md` **after step 1**
(prerequisites / `FEATURE_DIR`+`FEATURE_SPEC` resolution, `SKILL.md:63-69`) and **before step 2**
(the spec ambiguity/coverage scan, `:70-125`): *if `source-spec.md` exists in `FEATURE_DIR`, read it
and treat anything it already pins as resolved — do not raise a clarification the snapshot already
answers; if absent, skip silently (no-op).* Regenerate the `.claude` mirror via
`RefreshSurfaceBaselines` (FR-011: both source and mirror; graceful no-op when absent).

**Rationale.** Step 1 already computes `FEATURE_DIR`, so the snapshot path is in hand before the
question set is formed; placing the pre-check there means the scan in step 2 starts already aware of
what the source resolves. `source-spec.md` is the established 085 FR-016 snapshot filename, so the
step keys off that exact name. Same `SkillQualityCheck` exclusion as R2 — free-form prose, governed
by `SkillSyncCheck`.

**Alternatives considered.** A new prerequisites-script flag to emit the snapshot path — rejected as
over-engineering; the skill already has `FEATURE_DIR` and a file-existence check is sufficient and
degrades gracefully.

---

## R5 — Routing, regeneration, and baselines (cross-cutting)

**Decision / expectations** (from `Routing.fs` + `Engine/Update.fs`):
- `template/**` (capabilities.yml + emitted api-surface), `.agents/skills/**`, and
  `build.fsx`/governance paths all **escalate**; `Route` will demand the serialized six-target order.
  No **new** routing rule or gate is added, so `validation.contract.yml` content does not change
  (no `TargetMetadataDrift` structural-row churn beyond the regenerated artifacts).
- `RefreshSurfaceBaselines` (`Engine/Update.fs:97-150`) regenerates, in one byte-idempotent run: the
  `.claude` skill tree (from `.agents`), `catalog.yml`/`Catalog.fs` (from `catalogFacts`), the
  `docs/api-surface/**` tree (from `capabilities.yml` contracts), and the per-package baselines.
  Run it once after the source edits, then re-run the six gates.
- Per-package baseline `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`: the Widgets `.fsi`
  are **already** under `src/Controls/`, so `PerPackageSurface.captureCurrent` already includes them —
  recapture is expected to be a **no-op** for that baseline (its content does not change just because
  the file is now also *emitted* into api-surface). The api-surface tree is checked by
  `ApiSurfaceGen.currency`, not by a `.fsi.txt` baseline.

**Rationale.** Confirms the change rides existing currency spines; the only genuinely new generated
output is the 14 emitted typed `.fsi`, the `TypedModule` catalog token, the `EvidenceGraph` resolution
section, and the two regenerated skill mirrors.
</content>
