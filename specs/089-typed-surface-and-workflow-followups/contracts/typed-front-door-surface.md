# Contract: Published Typed Front-Door Surface (TYPED-SURFACE-1)

The "interface" this feature exposes is the **published consumer-facing surface**: the emitted
api-surface tree + the `catalog.yml` index. This contract pins what a generated project must carry so
a consumer authors typed `Props`/`view` **without reflecting `FS.Skia.UI.Controls.dll`**.

## C1 — api-surface tree (FR-001, FR-003, FR-004)

For **every** of the 52 typed widget modules (declared across the 14 enrolled `.fsi` **files**),
the generated project at `docs/api-surface/Controls/<file>.fsi` (emitted from
`template/base/docs/api-surface/Controls/`) MUST contain the byte-identical source `.fsi`, exposing:

- `namespace FS.Skia.UI.Controls.Typed`
- each `*Props<'msg>` record with its fields — **optional** fields are `option`-typed, **required**
  fields are not;
- each control `module` with `val view: <props> -> Widget<'msg>` (and `defaults`);
- event-callback fields (e.g. `OnClick`, `OnChanged`) on the Props record.

The 14 legacy builder `.fsi` (`Control.fsi`, `Attributes.fsi`, … `Accessibility.fsi`) MUST remain
published (additive). Coverage is whole-catalog: no supported control may require a DLL probe.

**Currency:** `ApiSurfaceGen.currency` (inside `TargetMetadataDrift`) MUST fail if any enrolled source
`.fsi` is missing, the emitted copy differs, or an emitted file has no source — naming the file and
the `./fake.sh build -t RefreshSurfaceBaselines` remedy.

## C2 — catalog id → typed-module index (FR-001, FR-002, FR-004)

`catalog.yml` (and the `template/base` mirror) MUST carry, per supported control row, a `TypedModule`
token naming the `FS.Skia.UI.Controls.Typed` module that realizes it. Combined with C1, a consumer
resolves: **control id → `TypedModule` → the module's `*Props`/`view` in the published `.fsi`**.

- The token is a single value per control (structural metadata), NOT a restatement of Props fields.
- Every `TypedModule` value MUST name a module present in some C1 `.fsi` (no dangling pointer).
- `custom-control` carries `TypedModule = CustomControl` and keeps `RequiredAttributes = []`
  (bridge-typed, no Props schema).

**Currency:** `CatalogGen.currency` MUST fail on `TypedModule` drift in `catalog.yml`/`Catalog.fs`.

## C3 — single source (FR-002)

The surface is **generated, not hand-authored**: C1 is copied byte-for-byte from the typed `.fsi`
(the source of truth); C2's token is rendered from `catalogFacts`. No duplicated Props prose exists
anywhere that could drift from the `.fsi`.

## Acceptance (maps to SC-001, SC-002)

- From a clean checkout, with **no reflection/decompilation**, a consumer can author a correct typed
  `Props` value and `view` call for **100%** of supported controls using only C1+C2.
- For three stateful controls (`CollectionModel`/`TextInputModel`-backed), the published surface alone
  yields a compiling `Props`+`view`.
- A deliberate drift (edit an emitted `.fsi` or a `TypedModule` token) fails the currency gate.
</content>
