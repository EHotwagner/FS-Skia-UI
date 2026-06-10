# Quickstart: Typed Front-Door Discoverability & Spec-Kit Workflow Followups

The edit→regenerate→verify loop for this governance/docs feature. No runtime/Skia/Vulkan step.

## 0. Route first

```bash
./fake.sh build -t Route
```

Expect escalation to the `maintainer-verify` path (touches `template/**`, `.agents/skills/**`,
governance). Run only the gates it prints; for this change that is the serialized six-target order.

## 1. Make the source edits

| Item | Edit (source of truth only) |
|---|---|
| TYPED-SURFACE-1 | `template/capabilities.yml` — add the 14 `src/Controls/Widgets/*.fsi` to Controls `contracts:`. `build/Governance/CatalogGen.fs` + `.fsi` — add `TypedModule` field + render it in `renderYamlRow` (and `renderFSharpRow` if the F# row carries it). |
| VERIFY-IMPL-1 | `.agents/skills/speckit-implement/SKILL.md` — insert the interactive run-and-use gate after Workflow step 6. |
| EVGRAPH-ECHO-1 | `build/Governance/Evidence/Render.fs` + `.fsi` — add `skillistResolution` helper + resolution section in `taskGraphMd`; thread `SkillRegistry` through `Engine.fs`. |
| CLARIFY-SOURCE-1 | `.agents/skills/speckit-clarify/SKILL.md` — insert the `source-spec.md` pre-check after step 1. |

Do **not** hand-edit any `.claude/**`, `template/base/docs/api-surface/**`, or `catalog.yml` —
those are regenerated in step 2.

## 2. Regenerate the single-source artifacts

```bash
./fake.sh build -t RefreshSurfaceBaselines
```

Regenerates (one byte-idempotent run): `.claude` skill mirror (from `.agents`),
`catalog.yml`/`Catalog.fs` (from `catalogFacts`, now with `TypedModule`), `docs/api-surface/**` +
`template/base/docs/api-surface/Controls/*.fsi` (the 14 typed `.fsi`), and per-package baselines.

Confirm the diff: +14 emitted typed `.fsi`, +`TypedModule` token per catalog row, regenerated
`.claude/skills/{speckit-implement,speckit-clarify}/SKILL.md`. The
`readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` baseline should be **unchanged** (the
Widgets `.fsi` were already in-package).

## 3. Run the serialized six-target order (sequential — shared `.fake` state)

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

> `GeneratedProductCheck` may report a non-authoritative environment failure locally (generated
> Verify can't resolve a feature without a template `feature.json`); judge it per the established
> environment-vs-defect rule.

## 4. Spot-check the new outputs

- `template/base/docs/api-surface/Controls/` lists the typed `.fsi` (e.g. `CollectionsWidgets.fsi`
  with `ListViewProps`/`view`) alongside the legacy set.
- `src/Controls/catalog.yml` rows carry `TypedModule` (e.g. `list-view` → `ListView`).
- `readiness/task-graph.md` has the `Skillist id → SKILL.md path` section (resolved lines + a flagged
  section for any alias/unresolved/ambiguous token).
- `.claude/skills/speckit-implement/SKILL.md` and `.../speckit-clarify/SKILL.md` show the new steps,
  byte-identical to `.agents`.

## Verification ↔ Success Criteria

| Step | Confirms |
|---|---|
| 2 + 4 (api-surface + catalog) | SC-001, SC-002 |
| `.agents`/`.claude` implement guidance | SC-003 |
| step 4 `task-graph.md` | SC-004 |
| `.agents`/`.claude` clarify guidance | SC-005 |
| step 3 six-target order + `SkillSyncCheck` + currency | SC-006 |
</content>
