# US3 Penpot/design-tokens verification (078) — T025

**Story**: Penpot drives control theming (SC-006).

**Reviewer checklist** against the **Penpot & design tokens** `##` subsection of
`docs/controls/spec-kit-workflow.md` (built at
`output/controls/spec-kit-workflow.html`):

| Question (a reader must answer from the subsection alone) | Answer in the page | Pass |
|---|---|---|
| What drives control theming? | Design tokens — the 10 `Theme` primitives (foreground, background, accent, danger, muted, font family, font size, density, corner radius, contrast-required ratio) × light/dark | ✅ |
| What is the token→theme path? | DTCG source → `RefreshSurfaceBaselines` regenerates `DesignTokens.fs` → `DesignTokenDrift` gate → controls render against the active `Theme` (4 numbered steps) | ✅ |
| Where is the design-token **single source**? | `src/Controls/design-tokens.tokens.json` — named explicitly as "the one edit point" | ✅ |
| How does Penpot fit in? | A Penpot workflow exports design decisions as DTCG tokens into that single source (live Penpot/MCP sync disclosed as future work, not yet wired) | ✅ |
| Can the reader reach the deep dive? | Yes — link to [Design tokens & Penpot](../controls-design/design-tokens-penpot.html) | ✅ |

**Dead-link check**: the subsection's link to
`../controls-design/design-tokens-penpot.html` resolves in the strict site build
(`docs-build.md`).

**Outcome**: PASS (SC-006) — a reader can describe the design-token/Penpot →
control-theming path and locate the design-token single source from the subsection.
