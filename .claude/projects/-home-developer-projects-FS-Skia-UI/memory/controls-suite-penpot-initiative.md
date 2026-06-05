---
name: controls-suite-penpot-initiative
description: Planned multi-feature initiative — typed Elmish controls suite + Penpot Spec-Kit integration
metadata:
  type: project
---

Major planned initiative (analysis written 2026-06-05): a comprehensive
Elmish-native, **typed** controls suite that subsumes the current 47-control
catalog, plus **Penpot** (penpot.app) integration into the Spec Kit workflow.

Key direction decided in analysis: replace the stringly-typed attribute bag
(`Control<'msg>` with `Attr.Name: string` + `UntypedValue of obj`) with **typed
per-control Props records + per-control MVU (Model/Msg/update)**, keeping
`Control<'msg>` as the internal lowered IR (renderer/layout/diagnostics
unchanged). `Fable.Elmish 4.2.0` already pinned; `TextInput`/`DataGrid`/
`Collections` already show the per-control MVU pattern to generalize.

Penpot: tokens-first is the recommended first step — DTCG token JSON → generated
F# theme module, drift-checked; MCP board/component extraction is assistive
spec-drafting only (Penpot official MCP is pre-beta, token plugin API has gaps).

Decomposed into ~7 sequential Spec Kit features (F-α typed core first). Full
report: docs/reports/2026-06-05-1421-controls-suite-and-penpot-integration-analysis.md
