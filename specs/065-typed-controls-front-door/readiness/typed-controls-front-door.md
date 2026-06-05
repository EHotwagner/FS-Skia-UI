# Typed Controls Front Door — readiness (065)

Routing-required artifact for the `controls-public-surface` rule
(`src/Controls/**`). Records the change classification and the evidence
obligations the escalated tier names.

## Change classification

- **Feature tier**: Tier 1 (contracted change — adds public `.fsi` surface).
- **Route tier**: `agent-ready` (matched rules: `controls-public-surface`,
  `evidence-governance`, `specify-catchall`, `docs-only`, `package-surface`).
- **Affected layer**: `src/Controls/**` only — new compile units
  `Widget.fsi`/`Widget.fs` and `Widgets/{Primitives,TextBoxWidget,DataGridWidget}.{fsi,fs}`,
  shipped inside the existing `FS.Skia.UI.Controls` package (no project moves).
- **Public-API impact**: **additive `.fsi` only**. New sealed `Widget<'msg>` +
  `module Widget` in the base `FS.Skia.UI.Controls` namespace, and six typed
  modules (`TextBlock`, `Button`, `CheckBox`, `Stack`, `TextBox`, `DataGrid`)
  under a distinct `FS.Skia.UI.Controls.Typed` namespace. No existing `.fsi`
  file changed — the legacy string-keyed API is frozen and untouched (FR-007).
- **Governance risk level**: **medium** (additive public `.fsi` surface confined
  to `src/Controls/**`). See [governance-risk-levels.md](./governance-risk-levels.md).

## Elmish/MVU applicability (Principle IV)

The two stateful typed controls satisfy Principle IV by **delegation**, not by
introducing parallel state:

- `TextBox` reuses `TextInputModel` / `TextInputMsg` / `TextInputEffect`; typed
  `init`/`update` delegate to `TextInput.init`/`TextInput.update` (asserted equal
  in `TypedLoweringTests.fs`).
- `DataGrid` reuses `DataGridModel` / `DataGridMsg` / `DataGridEffect`; typed
  `init`/`update` delegate to `DataGrid.init`/`DataGrid.update` (asserted equal).
- `update` stays pure (no filesystem, network, clock, RNG, or mutable global);
  the edge interpreter (`TextInput.interpretEffect`) is unchanged and reused.
- A `Widget.toControl`-terminated `view` runs through the existing
  `AdapterProgram` with **no adapter edit** (FR-009), proven by
  `tests/Elmish.Tests/TypedControlsAdapterTests.fs`.

The four pure controls (`TextBlock`, `Button`, `CheckBox`, `Stack`) involve no
stateful workflow or I/O, so they take the ordinary spec → FSI → semantic-test →
implementation path; Principle IV is not engaged for them.

## Lowering is real — no synthetic evidence (Principle V)

Every typed `view` calls the **same legacy string-keyed builders** it replaces
(`TextBlock.create`/`Button.create`/…), so the lowered `Control<'msg>` is
structurally equal to the legacy authoring call **by construction**, modulo
attribute order. This is proven, not stubbed:

- per-control + keystone six-control parity matrix in
  `TypedLoweringTests.fs` (FR-004, SC-002) — see
  [typed-lowering-parity.md](./typed-lowering-parity.md);
- event-binding parity (`OnClick`/`OnChanged` dispatch the same `'msg`;
  `None` lowers to **no** binding) in `InteractionTests.fs` (FR-008);
- render + accessibility parity at ≥2 viewports in `RenderingTests.fs`
  (same IR, same render path, equal deterministic hash) — see
  [controls-rendering.md](./controls-rendering.md).

No `[S]`/`[S*]`/`[SEH]` disclosure is required; the Synthetic-Evidence Inventory
in `tasks.md` is empty.

## Evidence obligations (Route-printed gates)

| Gate | Evidence |
| --- | --- |
| `ControlsCatalogCheck` | existing catalog unchanged (additive surface) |
| `ControlsInteractionCheck` | `InteractionTests.fs` typed dispatch parity |
| `ControlsRenderingCheck` | `RenderingTests.fs` viewport parity, [controls-rendering.md](./controls-rendering.md) |
| `PackageSurfaceCheck` / `PerPackageSurfaceDiff` | regenerated additive baseline, [package-surface-expectations.md](./package-surface-expectations.md) |
| `FsiTranscripts` | [fsi-session.txt](./fsi-session.txt) |
| `GeneratedProductCheck` | unchanged — generated product does not consume the typed surface in this feature |
| `EvidenceGraph` / `EvidenceAudit` | [evidence-graph.md](./evidence-graph.md), [evidence-audit.md](./evidence-audit.md) |

## No-behavioral-diff result (SC-003, T015)

The full existing `Controls.Tests` suite passes unchanged with the typed surface
added (56 passed, 0 failed), and `Elmish.Tests` passes (6 passed, 0 failed). The
existing `samples/ControlsGallery` compiles and its `--contract-smoke` run prints
`status=ok` with the additive typed-authoring panel composed in — no edit to any
existing control source was required. The legacy string-keyed API and existing
consumers are provably unaffected.

## Dependency impact (FR-011, SC-004)

No dependency added. `Controls.fsproj` references no `Fable.Elmish` and adds no
`PackageReference`; only the three existing project references
(`Scene`, `Layout`, `KeyboardInput`) remain. Asserted by the
dependency-governance guard in `tests/Elmish.Tests/TypedControlsAdapterTests.fs`.
