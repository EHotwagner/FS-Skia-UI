# US2 narrative verification (078) — T023

**Story**: Controls in the Spec Kit workflow (SC-005).

**Reviewer checklist** against `docs/controls/spec-kit-workflow.md` (built at
`output/controls/spec-kit-workflow.html`):

| Question (a reader must answer from the narrative alone) | Answer in the page | Pass |
|---|---|---|
| At which phase(s) are controls **chosen**? | `specify` (by purpose, from the catalog) and `plan` (commit to concrete controls) — "Where controls are chosen" section | ✅ |
| At which phase are controls **authored**, and how? | `implement`, via the typed Props/MVU front door (`FS.Skia.UI.Controls.Typed`) — "Where controls are authored" section, linking [typed-front-door](../controls-design/typed-front-door.html) | ✅ |
| How are controls **validated**? | The routed gates — `ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`, `ControlsInteractionCheck`/`ControlsRenderingCheck`, `ControlsCatalogDocsCheck` — "Where controls are validated" section | ✅ |
| Can the reader reach the authoring guidance? | Yes — direct link to the typed control front door page (FR-002 authoring path) | ✅ |
| Is the narrative ordered before the catalog in nav? | Yes — `index: 1` vs catalog `index: 2`, both `categoryindex: 2` | ✅ |

**Dead-link check**: the narrative's links to `../controls-design/typed-front-door.html`,
`../controls-design/design-tokens-penpot.html`, and `catalog.html` all resolve in the
strict site build (`docs-build.md`).

**Outcome**: PASS (SC-005) — a reader can name the choose/author/validate phases and
reach the relevant authoring guidance from the narrative alone.
