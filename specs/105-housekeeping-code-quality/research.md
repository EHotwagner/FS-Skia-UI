# Phase 0 Research: Housekeeping Code-Quality Remediation

All audit citations were re-verified against the working tree on branch
`105-housekeeping-code-quality`; line numbers had drifted from the report and are
corrected here.

## R1 — The duplicated lowering helpers (US1 / FR-001..FR-004)

**Decision**: One new `module internal WidgetLowering` (no `.fsi`) hosts
`withKeyOpt`, `onString`, `onStringList`, `a11y`, `intentToString`; the per-file
copies are deleted and references rewired. `onChanged{Bool,Float,String}` +
`tryParseFloat` live at `Control.fs` module scope.

**Verification** (working tree):

| Helper | Confirmed sites |
|--------|-----------------|
| `withKeyOpt` (×9) | `Buttons.fs:27`, `Navigation.fs:31`, `Pickers.fs:33`, `ChartsWidgets.fs:34`, `Input.fs:37`, `Containers.fs:45`, `Display.fs:45`, `Overlay.fs:28`, `Primitives.fs:47` |
| `onString` (×4) | `Navigation.fs:36`, `Overlay.fs:33`, `ChartsWidgets.fs:39`, `CollectionsWidgets.fs:46` |
| `onStringList` (×1) | `CollectionsWidgets.fs:49` |
| `onChanged` (×8, 3 shapes) | bool `1606`,`1611`; float `1616`,`1621`; string `1628`,`1633`,`1639`,`1683` (all `Control.fs`) |

The float `onChanged` carries the identical 217-char nested-`Double.TryParse`
lambda at `1616` **and** `1621` — `tryParseFloat` removes both.

**Rationale**: `internal`-no-`.fsi` is the only cross-file-visible, zero-public-
surface home (R3 below). Compile order is the one ordering constraint:
`Controls.fsproj` has `Control.fs` at line 55 and the first widget module
(`Widgets/Primitives.fs`) at line 92, so `Widgets/WidgetLowering.fs` is inserted
before line 92.

**Alternatives considered**: (a) put helpers in the existing `ControlInternals`
(exposed in `.fsi`) — rejected, would add public surface; (b) a public
`WidgetLowering` with `.fsi` — rejected, public-surface delta + baseline
recapture for an internal helper.

## R2 — Redundant access qualifiers (US2 / FR-005, FR-006)

**Decision**: Remove `private` from the audit's certified-redundant ~17 sites,
keep comments, leave the keep-list untouched.

**Verification**:

- `module private *Lowering` (×10): `Buttons.fs:26`, `Navigation.fs:30`,
  `Pickers.fs:32`, `ChartsWidgets.fs:33`, `CollectionsWidgets.fs:34`,
  `Containers.fs:44`, `Display.fs:44`, `Input.fs:36`, `Overlay.fs:27`,
  `Primitives.fs:46` (`LegacyControls`). Each is absent from its `.fsi` already.
- `let private` in `Reconcile.fs`: `attrValueEqual:46`, `diffAttrs:69`,
  `isKeepOp:90` (the enclosing `module internal Reconcile` is the boundary).
  `applyAttrChanges:229` exists but was **not** cited → left as-is.
- `let private` in `RetainedRender.fs`: `childPath:73`, `clockDuration:87`,
  `fadeAnimation:100`, `currentOpacity:123`. `fadeOutAnimation:113` and
  `firstFrameCollisions:203` exist but were **not** cited → left as-is.

**Keep-list (FR-006), confirmed present, untouched**: `module internal
SceneRenderer` (no `.fsi`); the `InternalsVisibleTo` seams on `Reconcile`,
`RetainedRender`, `ControlInternals`, `ControlRuntime`, `ControlsElmish`; the
~40 `let private` inside the **exposed** `ControlInternals`.

**Rationale**: `private` on a sole-module-in-file is pure noise the `.fsi`
already enforces; the comment is retained so a future second module in the same
file does not silently gain access (spec Edge Cases).

## R3 — Internal stringly-typed identifiers → DUs (US3 / FR-007..FR-009)

### R3a — Attribute reader (FR-007 ↔ FR-012, the resolved tension)

**Decision**: **Internal-only `AttrKey` DU; do NOT expand public
`StandardAttributeName`.**

**Verification**: The public DU (`Types.fs:80`, `Types.fsi:86`) has exactly 13
cases (`Text|Value|Children|Series|Values|Columns|Rows|Items|Nodes|VisibleRange|
SelectedRows|FocusedCell|Custom`). The runtime reader in `Control.fs` reads
`"text"`,`"value"`,`"styleClasses"`,`"visualState"`,`"slot"`,`"accessibility"`,
`"nodes"`,`"richTextRuns"`,`"orientation"` (and `width`/`height` via the
`[<Literal>]` constants feature 101 added at `Control.fs:334`) plus `DataGrid.fs`
reads `"rows"/"visibleRange"/"columns"/"selectedRows"/"focusedCell"`. Of the
control-intrinsic reader names, `styleClasses`/`visualState`/`slot`/
`accessibility`/`richTextRuns`/`orientation`/`width`/`height` are **absent** from
the public DU — routing through it requires *adding public cases*.

**Rationale**: FR-011/FR-012 (zero behavior + zero public-surface change) is the
banner constraint. An internal `AttrKey` with `name : AttrKey -> string` gives
the same compile-time-typo safety with zero public delta. The public DU stays
exactly as shipped; `DataGrid`'s intrinsic reads route through `AttrKey` too
(their values happen to coincide with public-DU cases, but the internal key is
independent).

**Alternative (recorded, rejected as default)**: expand public
`StandardAttributeName` — permitted by FR-012 only as a deliberate, baseline-
recaptured, escalated choice. Not elected: cost (recapture + escalation) exceeds
benefit for an internal reader-path typing.

### R3b — Slot names (FR-008)

**Decision**: Internal `SlotName = Leading|Trailing|Header|Footer`; the public
`AttrValue.SlotFillsValue : (string * Control<'msg>) list` carrier is unchanged.
**Verification**: fills yielded as `"header"`/`"footer"` (`Containers.fs:132/135`)
and `"leading"`/`"trailing"` (`Primitives.fs:94/97`); consumed via
`slotRegions` (`Control.fs:99`) and the `slot` filter (`Control.fs:124`). Parse
string→`SlotName` once at the `slotRegions`/`lowerSlots` edge. Preserves feature
095's deliberate no-public-`SlotName`.

### R3c — Scene evidence stage/category (FR-009)

**Decision**: Internal `EvidenceStage = Scene|Renderer`; record fields
`BlockedStage`/`DiagnosticCategory` stay `string`.
**Verification**: `Scene.fs:701/703` declare the string fields; only `"scene"`
(`736/738`) and `"renderer"` (`742/744`) are ever assigned. Project `stage ->
string` once at construction; evidence text is byte-identical.

### R3d — Renderer-mode dispatch (FR-009, §5C boundary)

**Decision**: Internal renderer-mode DU parsed once at the edge; public
`RendererMode : string` field unchanged.
**Verification**: case-insensitive `String.Equals(request.RendererMode, …)`
dispatch at `SkiaViewer.fs:2016` (`unsupported-host`), `2023` (`metadata-hash`),
`2047` (`pixel-readback`); the closed set also includes
`default`/`skia`/`deterministic-scene`. `RendererMode` is a public record field
at `196/203/225/273/294/316` and is written into evidence (`782/1872/1894`) — it
stays a string; only the internal dispatch `match` becomes exhaustive.

## R4 — Behavior preservation (US4 / FR-011..FR-014)

**Decision**: Prove byte-/structural identity via (1) the unchanged Controls +
Controls.Elmish suites staying green, and (2) a parity assertion comparing the
lowered `Control<'msg>` for each affected widget against a pre-change capture.
`Control<'msg>` has no structural equality (it embeds `AttrValue<'msg>` with a
function case), so the parity check compares `sprintf "%A"` renderings — the
established pattern (features 096/097/101). No new gate, no new public property
(spec Assumptions). Comments added/retained are purely descriptive and contain no
gate-token literals (FR-014, spec Edge Cases).
