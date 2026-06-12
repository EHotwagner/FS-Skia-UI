# Contract: Controls authoring doc-comment standard

The rule the 186-summary rewrite applies and the gate enforces. A public Controls
`.fsi` member's `///` summary is **substantive** when it would let a consumer use the
member without reflection or experimentation.

## Required content by member kind

- **Attribute builder** (`Attr.text`, `Attr.width`, `Button.onClick`, …): state
  (1) what the attribute does, (2) the meaning/units/range of its value, (3) which
  control kind(s) accept it (or "common to all controls" for shared attrs). Name the
  default-lowering when relevant (e.g. an omitted optional binds nothing).
  - *Good*: `/// Sets the button's click message. Bound only when provided; an absent
    handler lowers to no event binding. Accepted by Button, IconButton, SplitButton.`
  - *Bad*: `/// Public contract function exposed by this FS.Skia.UI package.`
- **Per-control entry** (`Button.create`, `Stack.create`, …): what the control is, its
  required attribute(s), and its primary event(s); cross-reference the typed `Props`
  module as the recommended path.
- **Typed `Props` field / `view` / `defaults`** (`Widgets/*.fsi`): already exemplary —
  keep the existing semantic, default-behavior-naming style as the positive model.
- **`Catalog` discovery function**: what it returns and how to use it to enumerate a
  control's contract (e.g. `requiredAttributes kind` → the attributes that control must
  have; pair with `supportedAttributes`).
- **Public type** (`ControlDefinition`, `ButtonProps`, …): what it represents and the
  authoring role of its key fields.

## Prohibited

- The placeholder sentence `Public contract function exposed by this FS.Skia.UI package.`
- Empty/whitespace-only summaries on public members.
- A single generic sentence copy-pasted across many members with no member-specific
  token (duplicate-only — the anti-evasion rule).

## Where it lives

`///` comments go on the **`.fsi`** members (Constitution II — visibility and the
public contract live in `.fsi`). They ship in `FS.Skia.UI.Controls.xml` (IntelliSense +
offline agents) and are copied verbatim into the consumer's
`docs/api-surface/Controls/*.fsi` bundle.

## Scope

Applies to every public member of `src/Controls/**/*.fsi`. Internal modules (no `.fsi`
public surface, e.g. the internal lowering helpers) are out of scope.
