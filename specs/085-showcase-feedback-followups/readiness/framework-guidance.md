# Framework guidance (085) — US5 independent validation path

Each US5 artifact states one fact; the `.claude` mirror is generated from
`.agents` (cite `.agents`, regenerated in T034) and passes
`SkillSyncCheck`/`SkillQualityCheck`.

| Artifact | Fact it states | FR |
|----------|----------------|----|
| `.agents/skills/fs-skia-viewer-host/SKILL.md` (NEW) | host input surface (keyboard `MapKey`; pointer `MapPointer`/`runInteractiveApp`), preview-vs-tree (`Control.render` preview vs `Control.renderTree`), windowed-fullscreen blur caveat + workaround | FR-011 |
| `.agents/skills/fs-skia-typed-controls/SKILL.md` | author whole-catalog consumers via `FS.Skia.UI.Controls.Typed.*`; verify availability from package / `catalog.yml` `module:` field, not `docs/api-surface/`; deterministic typed-surface probe recipe | FR-012 |
| `template/base/docs/scaffold-map.md` | typed front door absent from `docs/api-surface/` (legacy `X.create` only) + how to enumerate the typed surface + windowed-fullscreen blur workaround | FR-013/FR-010 |
| `.specify/templates/spec-template.md` | the Framework Governance Prompts section is exempt from the "no implementation details" rule | FR-014 |
| `.agents/skills/fs-skia-evidence-mode/SKILL.md` (FR-015 note) | evidence token parsing reads `key=value` lines; a markdown table with the same tokens does **not** satisfy the validators. Landed in the **skill**, not `template/base/docs/evidence-formats.md`, because that doc is **generated** (`EvidenceFormatSchema`, do-not-hand-edit) — the task allowed "and/or the `fs-skia-evidence-mode` skill" | FR-015 |
| `.agents/skills/speckit-specify/SKILL.md` | multi-file external-URL snapshot recipe (enumerate a GitHub tree, fetch per file, assemble `source-spec.md` with per-file headers) | FR-016 |

The new skill is named `fs-skia-viewer-host` (not `fs-skia-skiaviewer`) to avoid
colliding with the existing package-owned `fs-skia-skiaviewer` skill under the
shared `SkillSyncCheck` namespace (research D1).
