# Runtime limitations & unsupported-scope handling (feature 095)

## Supported runtime

FS.Skia.UI targets **.NET 10 desktop** (Windows + Linux), rendering through **Vulkan** via the
**SkiaSharp preview** backend. There is **no software-renderer fallback**, and **macOS/mobile/browser
are unsupported** (`unsupported macOS/mobile/browser`). Feature 095 adds no new dependency and no new
runtime surface — slot lowering is a pure, total structural function evaluated under the same backend.

## E5 non-goal line (FR-008)

The lookless slot mechanism deliberately does **not** introduce, and this feature does not add:

- no `DataContext`
- no binding expression / observable
- no per-item template instantiation
- no `ControlTemplate` type
- no dependency / attached properties
- no CSS-selector styling
- no new top-level `Control` record field, and no second message channel

A slot fill is a **static `Control<'msg>` value** the consumer's own `view : 'model -> Control<'msg>`
already computed; the single `view`/`update` stays the only model→view→message path.

## Totality guarantee (FR-006 / SC-005)

Slot lowering is **pure, total, and deterministic**:

- every declared region has a default (the kind's existing chrome), so an **unfilled** slot falls
  back to that default and contributes **zero geometry** (label/content position invariant);
- lowering **never throws** for any `(kind, fills)` — a kind with no declared regions returns the
  control verbatim (the `>=1000`-input FsCheck totality property exercises this);
- **absent ≠ empty**: a slot name **absent** from the fill list renders the default; a name
  **present** with empty content renders an empty region by the consumer's choice;
- filling a region a kind does **not** declare is a **compile-time error** — there is no field for
  it, so there is no runtime drop path to diagnose (closure enforced by the typed `Props` surface).

## Failure diagnostics

There is no new structured-log surface. The existing `ControlDiagnostic` / `ControlFidelity` /
`ContrastCheck` channels remain authoritative for slotted content as for any control. Because a
slot fill the lowering branch does not place is **unrepresentable** in the typed surface, there is no
silent-drop failure mode.
