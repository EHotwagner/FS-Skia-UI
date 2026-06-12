# Phase 1 Data Model: Controls Authoring API Discoverability

This feature is governance + documentation, not a stateful product workflow. The
"entities" are the analysis inputs/outputs of the new gate and the authoring-surface
concepts the docs describe. No `Model`/`Msg`/`Effect` product surface is introduced
(Constitution IV: N/A — no stateful/I/O product workflow).

## Entity: Controls public surface member (gate input)

The unit the gate enumerates and judges.

| Field | Type | Meaning |
|-------|------|---------|
| `File` | path | the `src/Controls/**/*.fsi` file the member is declared in |
| `Line` | int | declaration line (for actionable report) |
| `Kind` | `Val \| Type \| Member` | declaration kind |
| `Identifier` | string | the member/type name (e.g. `text`, `ButtonProps`, `requiredAttributes`) |
| `Summary` | string option | the associated leading `///` block, joined; `None` if absent |

**Derivation**: enumerate `.fsi` files (Globbing) → scan lines → attach the contiguous
leading `///` block to the next declaration.

## Entity: DocFinding (gate output)

One per violating member. The gate FAILS iff the finding list is non-empty.

| Field | Type | Meaning |
|-------|------|---------|
| `File` | path | offending file |
| `Line` | int | offending declaration |
| `Identifier` | string | offending member |
| `Reason` | `Placeholder \| Empty \| DuplicateOnly` | which rule (research D2) fired |
| `Detail` | string | human-actionable note (e.g. the placeholder text, or "summary shared by 12 members, no member-specific token") |

**Validation rules** (from research D2):
- `Placeholder`: whitespace-normalized summary contains
  `Public contract function exposed by this FS.Skia.UI package.`
- `Empty`: `Summary = None` or whitespace-only.
- `DuplicateOnly`: identical summary across ≥ 8 members in a file **and** no
  member-specific token (no backticked identifier, no parameter/value description).

**Report**: rendered to `readiness/doc-coverage.md` — enumerated surface (file,
member count) + every finding, or an explicit "0 findings over N members across M
files" pass line (observability: no silent pass).

## Entity: Authoring-surface concept (documentation target, not code)

What a substantive summary must convey (the doc standard the rewrite applies and the
gate's intent — see `contracts/doc-comment-standard.md`).

| Concept | A good summary states |
|---------|-----------------------|
| Attribute builder (`Attr.text`, `width`, `onClick`, …) | what the attribute does, the meaning/units of its value, and which control kinds accept it |
| Per-control entry (`Button.create`, `Stack.create`, …) | what the control is, its required attributes, key events |
| Typed `Props` field | what the field controls and its default-lowering behavior (e.g. `OnClick = None` → no binding) — already exemplary in `Widgets/*.fsi` |
| `Catalog` discovery function | what it returns and how a consumer uses it to enumerate a control's contract |

## Entity: Catalog fact (US3 discovery surface — existing, made visible)

Already modeled in code as `Catalog.ControlDefinition` and `catalog.yml`. No schema
change; this feature surfaces it, it does not redefine it.

| Field (existing) | Use to a consumer |
|------------------|-------------------|
| `Id` / `DisplayName` / `Category` | identify the control |
| `Module` / `typedModule` | the legacy builder and typed `Props` module to author with |
| `RequiredAttributes` / `CommonAttributes` | the attribute contract (answers "what does it take") |
| `Events` | bindable events |
| `VisualStates` | states the style layer resolves |

**Consumer access paths** (research D7): programmatic via documented `Catalog.*` +
`Catalog.markdownSummary()`; static via the bundled `template/base/docs/` catalog
reference, both linked from the generated README.

## Entity: Generated starter view (US1 demonstrated default)

`template/base/src/Product/View.fs` — rewritten from legacy `Module.create [ ... ]`
attr lists to `FS.Skia.UI.Controls.Typed` `{ Module.defaults with Field = ... } |>
Module.view`. Obligation: lowers to the same controls (parity), compiles, renders.

## State transitions

None. The gate is a pure pass/fail analysis; the documentation and starter are static
artifacts. No runtime state machine.
