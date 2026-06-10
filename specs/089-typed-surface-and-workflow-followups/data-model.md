# Data Model: Typed Front-Door Discoverability & Spec-Kit Workflow Followups

This feature is governance/docs — its "entities" are the single-source facts and the
generated-output shapes, not runtime domain types. No `Model`/`Msg`/`Effect` (Principle IV N/A).

---

## E1 — `TypedCatalogFact.TypedModule` (new field on an existing governance type)

**Where:** `build/Governance/CatalogGen.fs:14-22` (type) + `:49-57` (`fact` constructor) + `.fsi`.

**Change:** add one field — the typed front-door module name for the control.

```fsharp
type TypedCatalogFact =
    { Id: string
      DisplayName: string
      Category: string
      Module: string            // existing — legacy/builder module token
      TypedModule: string       // NEW — FS.Skia.UI.Controls.Typed module name (e.g. "ListView")
      Purpose: string
      RequiredAttributes: string list
      Events: string list
      AccessibilityRole: string }
```

**Field rules.**
- `TypedModule` names the `module` under `FS.Skia.UI.Controls.Typed` whose `view`/`Props` realize the
  control (e.g. `list-view` → `"ListView"`, `button` → `"Button"`, `data-grid` → `"DataGrid"`).
- For controls whose typed module equals the legacy `Module`, the two values coincide (no special
  case in code — just equal data).
- `custom-control` → `"CustomControl"` (bridge-typed; no Props schema; `RequiredAttributes` stays `[]`).
- It is a **pointer**, never a copy of field names. The `Props` field list / `view` arity live only in
  the enrolled `.fsi` (E2). Adding/removing the field is a contract change (currency gate + parity test).

**Validation / currency.** `CatalogGen.currency` (`:262-289`) already compares rendered rows in
`catalog.yml` **and** `Catalog.fs` against fresh renders from `catalogFacts`; the new token is rendered
by `renderYamlRow` (`:164-188`) (and `renderFSharpRow` `:136-153` if the F# row carries it), so drift
in the field fails the gate and names the file + id + regen command.

**Cross-check invariant (new test).** Every `TypedModule` value MUST correspond to a real module
present in some enrolled `src/Controls/Widgets/*.fsi` (no dangling pointer). This is the
single-source guard tying E1 to E2.

---

## E2 — Enrolled typed `.fsi` set (api-surface capability rows)

**Where:** `template/capabilities.yml` Controls `contracts:` (currently 14 legacy `.fsi`,
`:113-131`) → add 14 typed rows.

```yaml
# added to Controls contracts:
- src/Controls/Widgets/Primitives.fsi
- src/Controls/Widgets/Buttons.fsi
- src/Controls/Widgets/Containers.fsi
- src/Controls/Widgets/Input.fsi
- src/Controls/Widgets/TextBoxWidget.fsi
- src/Controls/Widgets/TextAreaWidget.fsi
- src/Controls/Widgets/Display.fsi
- src/Controls/Widgets/CollectionsWidgets.fsi
- src/Controls/Widgets/DataGridWidget.fsi
- src/Controls/Widgets/Navigation.fsi
- src/Controls/Widgets/Overlay.fsi
- src/Controls/Widgets/ChartsWidgets.fsi
- src/Controls/Widgets/CustomControlWidget.fsi
- src/Controls/Widgets/Pickers.fsi
```

**Emitted artifact (generated, byte-identical to source):**
`template/base/docs/api-surface/Controls/<file>.fsi` for each row, via `ApiSurfaceGen.plan` +
`regenerateApiSurface`. In a generated project these appear at `docs/api-surface/Controls/`.

**Per-file shape (already true in source, now published):** `namespace FS.Skia.UI.Controls.Typed`;
one or more `*Props<'msg>` records (option-typed fields = optional, others required); `module X` with
`val view: props -> Widget<'msg>` (+ `defaults`). This is the FR-001 richness, unmodified.

**Currency.** `ApiSurfaceGen.currency` (source-bytes vs emitted-bytes vs orphan) inside
`TargetMetadataDrift`.

---

## E3 — EvidenceGraph skillist resolution line (new rendered output)

**Where:** `build/Governance/Evidence/Render.fs` `taskGraphMd`, appended after `:319-330`.

**Shape (one line per distinct skillist id), reusing `Audit.fs:150-162` resolution:**

| Registry result | Emitted line | Class |
|---|---|---|
| `Skills.TryFind` → `Some [path]` | `id → <path>` | resolved |
| `Skills.TryFind` → `Some [p1; p2; …]` | `id → ambiguous: p1, p2` | flagged |
| `None`, `DirectoryAliases.TryFind` → `Some (acceptedId, path)` | `id → directory name for <path> (accepted id: acceptedId)` | flagged |
| `None`, no alias | `id → UNRESOLVED (not registered/readable)` | flagged (FR-009) |

Resolved lines and flagged lines are grouped distinctly (FR-009: "separate from the resolved lines").

**Pure helper (new, unit-tested):**
```fsharp
// Render.fsi
val skillistResolution: registry: SkillRegistry -> ids: string list -> string
```
`taskGraphMd` gains a `registry: SkillRegistry` parameter (already in `EvidenceInputs`).

**Output sink:** `readiness/task-graph.md` (existing), verdict summary in `logs/evidence-graph.txt`.

---

## E4 — Skill-guidance text (documentation entities, not code)

| Skill (`.agents` source → `.claude` mirror) | Insertion point | New content |
|---|---|---|
| `speckit-implement/SKILL.md` | per-task Workflow, after step 6 (`:209-213`), before status-write step 7 | Interactive-UI run-and-use gate: launch+interact via `run`/`verify`; confirm the production render path stated generically (the real user-reachable surface the feature drives — `controlsExampleView` → `Control.renderTree` cited only as an example, never hard-coded); no-op for non-interactive stories; precondition of `[X]` on `[US*]`. |
| `speckit-clarify/SKILL.md` | after step 1 (`:63-69`), before step 2 (`:70-125`) | `source-spec.md` pre-check: if present in `FEATURE_DIR`, consult before forming questions; skip silently if absent. |

Both mirrors are regenerated (not hand-edited) and held byte-identical by `SkillSyncCheck`. Both
skills are excluded from `SkillQualityCheck`'s section rubric, so no mandatory headings constrain the
edit.

---

## Relationships & invariants

- **E1 ⟂ E2 single-source tie:** every `E1.TypedModule` resolves to a module declared in an `E2`
  enrolled `.fsi` — enforced by the E1 cross-check test. Field-level richness lives only in E2; E1 is
  a pointer. Neither restates the other → no drift surface (FR-002).
- **E2 ⟂ legacy surface additive:** the 14 legacy `.fsi` rows remain; typed rows are added, never
  replacing (FR-003).
- **E3 reuses, never re-implements:** the resolution mirrors `Audit.fs` exactly (FR-008), so the echo
  cannot disagree with the validator.
- **E4 currency:** `.agents`↔`.claude` byte-identity (`SkillSyncCheck`); both items present in both
  trees (FR-007, FR-011).
</content>
