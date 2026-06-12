# Template check — feature 106 (T018, FR-010/FR-011/FR-013, SC-004/SC-005)

`./fake.sh build -t TemplateCheck` — **PASS** (`readiness/template/verdict.md`: "source/package
V3 app, headless-scene, governed, and sample-pack generated projects passed non-visual
validation; pinned-vs-local package-skew sub-check clean").

## Bundled discovery reaches the generated project

- The consumer-visible per-control catalog reference is present in every instantiated project:
  `artifacts/generated-products/106-controls-api-discoverability/{app-source,headless-scene-source,sample-pack-source}/docs/controls-catalog.md`.
- The generated `README.md` carries the **"Authoring controls — discover the API, never
  reflect"** section pointing to: the typed starter (`src/Product/View.fs`), the on-disk
  `docs/api-surface/Controls/*.fsi` bundle, `docs/controls-catalog.md`, the programmatic
  `Catalog.*` discovery API, and the interactive host seam.

## SC-004 walkthrough (named control's complete attribute set, no reflection)

Starting from the generated README → the "discover the API" section → `docs/controls-catalog.md`
and the `Catalog.*` API:

- The `Catalog` table documents `requiredAttributes`/`supportedAttributes`/`supportedEvents`/
  `knownControlKinds`/`markdownSummary`. For e.g. a text box:
  `Catalog.supportedAttributes StandardControlKind.TextBox` returns `value` + the common
  attribute set, and `Catalog.supportedEvents` returns `[ onChanged ]`.
- The static table lists the demonstrated controls' required attributes + events directly
  (TextBox → `value` / `onChanged`, DataGrid → `columns`,`rows` / `onSelected`,…).

So a named control's complete supported-attribute set is obtained from shipped docs / the
discovery API — never by reflecting over the DLL.

## Not-yet-typed control edge case (spec edge case)

Every catalog control carries a `typedModule`, but the reference does **not** imply a control is
unsupported when authored via the legacy builder: `docs/controls-catalog.md` states explicitly
"Controls without a typed module are still fully supported" and the `Catalog` API reports the
full attribute/event contract for every control regardless of construction path.

## SC-005 — no dangling "do not reflect" instruction

Every place the generated template discourages reflection resolves to a concrete, populated
reference (typed starter, `docs/api-surface/Controls/`, `docs/controls-catalog.md`, `Catalog.*`,
the host seam) — see `generated-guidance-validation.md`.

- failure-class: template-defect (none observed)
