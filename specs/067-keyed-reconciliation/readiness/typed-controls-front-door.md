# Typed Controls Front Door — readiness (067)

Routing-required artifact for the `controls-public-surface` rule
(`src/Controls/**`). Feature 067 adds the **internal keyed reconciler** inside the
existing `FS.Skia.UI.Controls` package; it records here that the change lands as
internal code with **zero** public-surface delta.

## Change classification

- **Feature tier**: Tier 2 (internal change — adds no public API surface).
- **Route tier**: `FocusedAuthority` (matched rule: `controls-public-surface`,
  because the diff touches `src/Controls/**`).
- **Affected layer**: `src/Controls/**` only — two new compile units
  `Reconcile.fsi`/`Reconcile.fs` (inserted after `Control.fs`) plus an
  `<InternalsVisibleTo Include="Controls.Tests" />` MSBuild item in
  `Controls.fsproj` (the SDK generates the assembly attribute at build time; a
  source `AssemblyInfo.fs` is avoided because it would lack the `.fsi` pair the
  surface-area gate requires). No project moves; the `Controls.fsproj` reference
  set is unchanged (`Scene`, `Layout`, `KeyboardInput` only — **no
  `Fable.Elmish`**, no renderer dependency, FR-013).
- **Public-API impact**: **none.** `Reconcile.fsi` declares `module internal
  Reconcile` (assembly-internal accessibility, matching the `module internal
  SceneRenderer` precedent) and is deliberately **not** added to the Controls
  capability `contracts:` list, so `ApiSurfaceGen`/`PackageSurfaceCheck` emit no
  new entry and the baseline stays byte-for-byte identical (FR-002, SC-005). No
  existing `.fsi` file changed.
- **Governance risk level**: **small** (internal-only framework code confined to
  `src/Controls/**`, not wired into the render path).

## Internal reach mechanism (research R1/R2)

- `module internal` makes `diff`/`apply` and the patch algebra genuinely
  unreachable from package consumers — "internal only" is *enforced*, not merely
  documented.
- The Expecto/FsCheck property tests reach the module from the separate
  `Controls.Tests` assembly via a single
  `[<assembly: InternalsVisibleTo("Controls.Tests")>]` in `src/Controls/AssemblyInfo.fs`.
  This is an assembly-level attribute, not a top-level binding access modifier, so
  Principle II is unaffected.

## Elmish/MVU applicability (Principle IV)

**N/A — pure stateless diff.** The reconciler owns no `Model`/`Msg`/`Effect`,
performs no I/O, and emits no commands; it is a pure, total function over the
immutable `Control<'msg>` IR. The generic `'msg` is opaque payload threaded
through unchanged — the diff never dispatches or interprets it.

## Reconciler is real — no synthetic evidence (Principle V)

`diff` and `apply` are real; the round-trip property runs real generated trees
through both. No mock, stub, placeholder, or canned response is used. No
`[S]`/`[S*]`/`[SEH]` disclosure is required; the Synthetic-Evidence Inventory in
`tasks.md` is empty.

## Evidence obligations (Route-printed gates)

| Gate | Evidence |
| --- | --- |
| `ControlsCatalogCheck` | catalog unchanged (no catalog edit; internal module only) |
| `ControlsCatalogGenerationCheck` | generated catalog current (no `src/Controls/Catalog*` change) |
| `ControlsInteractionCheck` | interaction behavior unchanged (FR-012); reconciler not wired to the render path |
| `ControlsRenderingCheck` | render/layout/a11y unchanged (FR-012) |
| `PackageSurfaceCheck` | zero-delta baseline, [package-surface-expectations.md](./package-surface-expectations.md) |
| `FsiTranscripts` | no public symbol to load; internal reach proven by in-assembly Expecto/FsCheck tests |
| `GeneratedProductCheck` | unchanged — the reconciler is assembly-internal and unreachable from generated products |
| `EvidenceGraph` / `EvidenceAudit` | [evidence-graph.md](./evidence-graph.md), [evidence-audit.md](./evidence-audit.md) |

## No-behavioral-diff result (FR-012)

The full existing `Controls.Tests` suite passes with the reconciler added (77
passed, 0 failed, 0 errored — 65 pre-existing tests plus 12 new Reconcile
tests/properties). No existing control source, renderer, layout, diagnostics, or
accessibility path was edited; the addition is purely additive internal code.

## Dependency impact (FR-013)

No product dependency added. `Controls.fsproj` adds no `PackageReference`. The
only new reference is a **test-only** `FsCheck` (pinned 3.3.3 in
`Directory.Packages.props`) on `Controls.Tests.fsproj`, which is not a product
dependency — `DependencyReport` is unaffected.
