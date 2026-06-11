# Internal closed-set identifiers → typed keys / DUs (T014–T018) — SC-004 / FR-007–FR-010

US3 routes the internal closed-set identifiers through compile-checked types, with **string
boundaries** so every public/serialized field stays a byte-identical string. SC-004 (a mistyped
internal identifier is a compile error) is demonstrated by the code compiling **only** against
the closed sets — every match below is exhaustive and references the DU cases by name.

## T014 — typed attribute key (`AttrKeys.AttrKey`, FR-007)

New shared `module internal AttrKeys` (`src/Controls/AttrKeys.fs`, **no `.fsi`**, compiled
before `Control.fs`/`DataGrid.fs`). `AttrKey` is the closed set of control-intrinsic names with
a single `nameOf : AttrKey -> string` boundary and `tryKey`/`hasKey` readers (same
last-writer/`List.exists` semantics as `ControlInternals.tryLast`/`hasAttr`).

- `Control.fs` (`ControlInternals`) closed reads routed through the key: `text`, `value`
  (`textFrom`), `styleClasses`, `visualState`, `slot` (read + `slotFill` construction +
  `lowerSlots` filter), `accessibility`, `nodes`, `richTextRuns`.
- `DataGrid.fs` closed reads/writes single-sourced through the key: `rows`, `visibleRange`,
  `columns`, `selectedRows`, `focusedCell`.
- The public `StandardAttributeName` DU is **unchanged** (D3 — internal-only key, zero surface).
  `width`/`height`/`orientation` keep feature 101's `[<Literal>]` single-sourcing (already
  compile-checked). The string-keyed `tryLast`/`hasAttr` stay for dynamic names.
- Keep-as-string (FR-010): DataGrid `columnKey`/`rowKey` are **untouched** (4 sites).

Each `nameOf` arm returns exactly the prior literal, so every read is byte-identical.

## T015 — internal `SlotName` DU (`Control.fs`, FR-008)

`type SlotName = Leading | Trailing | Header | Footer` (internal to `Control.fs`), driving
`slotRegions : SlotName list * SlotName list`; `lowerSlots` projects the region to its string at
the single consumption edge (`fn = slotName n`). **No public `SlotName` surface** — the public
`AttrValue.SlotFillsValue : (string * Control) list` carrier and the `slotFor : name:string`
signature are unchanged (feature 095's deliberate omission preserved).

## T016 — internal `EvidenceStage` DU (`Scene.fs`, FR-009)

`[<RequireQualifiedAccess>] type EvidenceStage = Scene | Renderer` (internal to `Scene.fs`,
RQA to avoid the `Scene` type/module clash), with `SceneEvidence.stageName` the single
projection. The public `SceneEvidenceFailure.BlockedStage`/`DiagnosticCategory` fields stay
`string`, written via `stageName EvidenceStage.Scene` / `EvidenceStage.Renderer` — byte-identical
`"scene"`/`"renderer"` evidence text. Scene.Tests: 28 passed.

## T017 — internal renderer-mode DU (`SkiaViewer.fs`, FR-009)

`[<RequireQualifiedAccess>] type private RendererModeKind = Default | Skia | DeterministicScene
| UnsupportedHost | MetadataHash | PixelReadback`, parsed **once** at the dispatch edge by
`parseRendererMode` (case-insensitive; unrecognized → `Default`, the prior string-comparison
fallthrough). `visualEvidenceArtifacts` is now a single-parse **exhaustive** DU match (the
`Default | Skia | DeterministicScene` arm carries the prior png-or-metadata fallback). The
line-2573 comparison reuses the same parser. Every public `RendererMode` output/serialized field
stays an unchanged string. SkiaViewer.Tests: 62 passed.

## T018 — SC-004 + parity + keep-as-string

- **SC-004**: all four typed surfaces (`AttrKeys.nameOf`, `slotName`, `EvidenceStage` /
  `stageName`, `RendererModeKind` / `parseRendererMode` + dispatch) are **exhaustive** matches
  that compile only against their closed sets — a mistyped internal identifier is a compile error.
  `dotnet build` succeeds with **0 warnings** (no incomplete-match / unused-case warnings).
- **Parity (SC-006/FR-011)**: the Feature 105 parity guard (8) green; Controls (337), Elmish (69),
  Scene (28), SkiaViewer (62) all green and unchanged — every serialized string byte-identical.
- **Keep-as-string (FR-010, SC-008)**: `ControlKind`, public diagnostic/mode output fields,
  consumer metadata keys (`columnKey`/`rowKey`), and `ControlEvent.Kind` are untouched.
