# US4 — consumer capability guidance for the whole E1–E5 surface (SC-008 / SC-009)

**Edited skills:**
- `src/Controls/skill/SKILL.md` (`fs-skia-ui-widgets`) — the package-owned consumer skill.
- `template/fragments/controls/skill/SKILL.md` (`fs-skia-generated-controls-guidance`) — the
  template-fragment skill a `dotnet new fs-skia-ui` project selecting Controls receives.

**Renderer mode:** N/A (documentation/governance deliverable, no interactive surface).
**Failure class:** product-defect (missing/dishonest capability guidance).

## Result: PASS

### Both skills name + show a runnable example for every rung E1–E5 (SC-008)

Each skill gained a `## Capability surface — E1–E5` section with a runnable consumer example for:

- **E1** live event dispatch (`OnClick` lowers to a per-`ControlId` binding; `Control.dispatch`);
- **E2** retained identity (keep a control keyed so focus/text survive a sibling-shifting re-render —
  identity is a property of the keyed tree, **not** a binding);
- **E3** style class / variant + visual state (`Classes = [ Variant StyleVariant.Danger ]`, fixed
  precedence, no CSS selectors);
- **E4** focus / keyboard traversal (`Focus.order` / `Focus.traverse` / `Focus.route`);
- **E5** lookless slot composition (`Button.Leading`/`.Trailing`, `Panel.Header`/`.Footer`).

### Honesty (FR-010)

The E5 example states plainly that **a slot lowers to `Control<'msg>`, not a data-bound template**,
and that retained identity is a property of the keyed tree, not a binding. Because E1–E5 are **all
shipped by this feature**, the guidance carries **no** Principle V synthetic-evidence disclosure —
every rung documented is a real, landed capability.

### Governance checks green

- `SkillSyncCheck` — PASS (the `.claude` peer was regenerated from the canonical `.agents` source via
  `RefreshSurfaceBaselines`; never hand-edited).
- `SkillQualityCheck` — PASS (rubric headings + the one-line "official online docs first" mandate
  preserved).
- `GeneratedGuidanceCheck` — PASS (the template-fragment guidance reaches a generated project).

### Generated project receives the guidance (SC-009)

A generated project selecting the Controls capability receives the updated
`template/fragments/controls/skill/SKILL.md` E1–E5 runnable examples — validated by
`GeneratedGuidanceCheck` / `GeneratedProductCheck` / `TemplateDrift` and recorded in
[generated-validation.md](./generated-validation.md).
