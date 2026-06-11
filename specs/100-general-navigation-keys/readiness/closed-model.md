# Closed model — NavIntent / NavPayload exhaustiveness (feature 100, R5, T019)

evidence-kind=closed-model
status=observed

The navigation model is a **closed, exhaustively-matched** set with **no free-form key-handler
surface** (FR-005/SC-005). Both unions live in `FS.Skia.UI.Controls`:

- `Direction = Previous | Next | First | Last` (`[<RequireQualifiedAccess>]`, distinct from the E4
  `FocusMove`).
- `NavIntent = ValueStep of float | SelectionMove of Direction | GridMove of int * int` (3 cases).
- `NavPayload = SteppedValue of float | MovedSelection of int * string option | MovedCell of int * int`
  (3 cases), mirroring `NavIntent` one-to-one.

## Proof (FsCheck `Check.One`, no `testProperty` in this repo)

- **NavIntent is totally matched** (>=1000 generated values): a total match over every case tags into
  {0,1,2}; never throws. A new case would be a compile error.
- **NavPayload is totally matched** (>=1000 generated values): same.
- **One-to-one correspondence** (>=1000): for every generated `NavIntent`, the payload class of its
  canonical `NavPayload` equals the intent class — each intent class has exactly one payload class.

## Metadata-driven (no per-kind host special-case, FR-006)

Each covered role's navigation outcome is reproduced **purely** from its declared role +
`NavigationKeys` (+ `NavRange`) and the closed model: `Focus.route` is the single role-specific branch
(role → one intent class); the host resolver branches only on the **intent**, never the kind. A value
role with no declared `NavRange`, and any non-navigable role, form **no** intent (`Fallthrough`).

## Source

`tests/Controls.Tests/Feature100NavigationTests.fs` —
`100 US4 closed model + metadata-driven (SC-004/SC-005/FR-010)`.
