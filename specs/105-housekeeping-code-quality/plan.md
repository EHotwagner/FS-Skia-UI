# Implementation Plan: Housekeeping Code-Quality Remediation

**Branch**: `105-housekeeping-code-quality` | **Date**: 2026-06-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/105-housekeeping-code-quality/spec.md`
**Source**: `docs/reports/2026-06-11-1424-housekeeping-code-quality-audit.md` (§1, §3, §5A)

## Summary

A behavior-preserving maintainability pass over `src/**` that takes the three
low-risk batches the audit recommends first and **defers** the higher-risk /
contract-escalating items to their own scoped passes. Three workstreams, all
internal, all proven byte-/structurally identical to the pre-change output:

1. **US1 (P1) — De-duplicate the lowering layer.** Collapse the 9 verbatim
   `withKeyOpt` copies, the 4 `onString` + 1 `onStringList` copies, and the
   smaller `intentStyle`→string / accessibility-metadata duplications into one
   new shared **`module internal WidgetLowering`** (no `.fsi`, compiled before
   the widget modules). Collapse the 8 inline `onChanged` parsers in `Control.fs`
   into `onChangedBool` / `onChangedFloat` / `onChangedString` over a named
   `tryParseFloat`, killing the twice-duplicated 217-char nested-`Double.TryParse`
   lambda.
2. **US2 (P2) — `.fsi` is the single visibility boundary.** Drop the ~17
   redundant in-source `private` keywords (10 `module private *Lowering`,
   `Reconcile` ×3, `RetainedRender` ×4) the audit certifies redundant, **keeping**
   the explanatory comments and **every** load-bearing qualifier on the keep-list.
3. **US3 (P2) — Internal closed-set identifiers become DUs.** Route the
   closed control-intrinsic attribute reads through an **internal-only** typed
   `AttrKey` (NOT the public `StandardAttributeName`), and introduce internal DUs
   for slot names, the Scene evidence stage/category, and the renderer-mode
   dispatch comparison — each parsing at most once at the edge so every public
   output/serialized field stays a byte-identical string.

US4 (P1) is the banner constraint, not a workstream: **zero** change to
observable output, parity/golden evidence, determinism properties, or the public
`.fsi` surface.

**Tier: 2 (internal change).** Under the default zero-surface choices (shared
module + helpers internal; `AttrKey` internal-only; the new slot / Scene-stage /
renderer-mode DUs internal with string boundaries) there is **no public `.fsi`
delta and no baseline recapture**. The optional FR-012 public-DU expansion is
**not** elected (see D3). Routing may nonetheless escalate to the
`controls-public-surface` maintainer-verify path empirically — features 101/102
observed that *any* `src/Controls/**/*.fs` edit can escalate even with zero
`.fsi` delta — so `Route` is authoritative and the escalated gate set is
expected.

## Technical Context

**Language/Version**: F# / .NET `net10.0`.
**Primary Dependencies**: None new. Touches `FS.Skia.UI.Controls`
(`Widgets/*.fs`, `Control.fs`, `Reconcile.fs`, `RetainedRender.fs`,
`DataGrid.fs`, plus the new `Widgets/WidgetLowering.fs`), `FS.Skia.UI.Scene`
(`Scene.fs`), and `FS.Skia.UI.SkiaViewer` (`SkiaViewer.fs`).
**Testing**: Existing Controls + Controls.Elmish Expecto suites (unchanged,
must stay green); a parity assertion over the consolidated helpers proving the
lowered `Control<'msg>` is structurally identical pre/post; the standard
`EvidenceGraph` / `EvidenceAudit` readiness chain. **No new gate, no new public
property** (spec Assumptions).
**Target Platform**: Windows and Linux (unchanged).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A — no source-of-template, docs, samples, Spec-Kit
  asset, package-policy, or command-surface change reaches
  `.template.config/template.json`. All edits are framework-internal `src/**`
  bodies; lowering output and the template's package pins are untouched. No
  template update or deferral required.
- **Dependency impact**: N/A — no dependency change. `Directory.Packages.props`,
  `docs/dependencies.md`, generated-template inclusion, and `DependencyReport`
  are untouched.
- **Command-surface impact**: No new or changed targets. `Route` selects the
  authoritative gate list for this diff; no gate is added (spec Build-target
  impact). FAKE-backed targets run **sequentially** in the documented order.
  Example order:
  1. `./fake.sh build -t Route` (authoritative gate list for this diff)
  2. `./fake.sh build -t Dev`
  3. the gates `Route` prints — predicted inner-loop `Dev`; **be prepared** for
     the escalated `controls-public-surface` set + `EvidenceGraph` +
     `EvidenceAudit` per the 101/102 precedent that `src/Controls/**/*.fs` edits
     can escalate with zero `.fsi` delta.
- **Generated project impact**: N/A — no default/minimal generated content,
  selected-Controls guidance, local skills, validation logs, placeholder/
  excluded-history scans, or generated `Dev` behavior changes. The edits are
  framework-internal and produce byte-identical lowering/rendering output.
- **Evidence paths**: `specs/105-housekeeping-code-quality/readiness/**`
  (focused-gates log, evidence graph, evidence audit, any escalated
  controls-public-surface output); a `git diff` of `src/**/*.fsi` showing **zero**
  public-surface lines (FR-011/FR-012/SC-007 proof); the parity-assertion test
  output (structural identity of the lowered `Control<'msg>`, SC-006); a grep
  transcript showing one body per consolidated helper (SC-001/SC-002) and the
  ~17 `private` removals with comments retained (SC-003).
- **`.fsi` / contract impact**: **None under the default choices.** The shared
  `WidgetLowering` module is `internal` with **no `.fsi`** (the documented
  `module internal SceneRenderer` precedent — zero baseline capture); the
  `onChanged*`/`tryParseFloat` helpers are `Control.fs`-internal (absent from
  `Control.fsi`); the `AttrKey`, `SlotName`, Scene-stage, and renderer-mode DUs
  are internal with string boundaries. The public `StandardAttributeName` DU is
  **not** expanded (D3). Tier 2; no surface-baseline recapture obligation. If
  `Route` escalates for routing reasons, that is gate selection, not an `.fsi`
  delta — the diff still carries zero `.fsi` lines.
- **MVU/effect boundary**: N/A — no stateful or I/O-bearing behavior changes. The
  `SkiaViewer` Elmish update and the `Controls.Elmish` host loop are read for the
  renderer-mode DU edge only; their `Model`/`Msg`/`Effect`/interpreter contracts
  are unchanged.
- **Synthetic evidence**: None. No mocks/fakes/placeholders/canned data; the
  parity assertion compares **real** lowering output (the actual `Control<'msg>`
  produced by the affected widgets) before vs. after. No `[S]`/`[SEH]` tasks
  anticipated.
- **Test evidence**: Behavior-preserving refactor — the failing-first semantic is
  the **parity** assertion: it is authored to compare the consolidated-helper
  output against a captured pre-change baseline and would go red if any
  consolidation perturbed the lowered control. The existing Controls /
  Controls.Elmish suites are the regression net and must stay green and
  unchanged. No assertion is weakened (Principle VI).
- **Observability**: No new diagnostics. The renderer-mode / Scene-stage DUs keep
  their serialized output strings byte-identical, so existing evidence text and
  unsupported-environment diagnostics are unchanged; the only added safety is
  compile-time exhaustiveness on the internal matches.
- **Deferred scope**: Explicitly out of scope (FR-013) and not touched: §2.1 file
  splits (`SkiaViewer.fs`/`Control.fs`/`Vulkan.fs`), §5B `ControlId` wrapper and
  the SkiaViewer public diagnostic/mode field conversions, §2.4 mutable-heavy
  refactors, §4 `AttrValue<'msg>` custom-equality, and every §5C keep-as-string
  identifier (`ControlKind`, public output fields, consumer metadata keys,
  `ControlEvent.Kind`). The optional FR-012 public-DU expansion is deferred
  (default internal-only key chosen).

**Gate result: PASS.** Tier 2, no `.fsi`/baseline obligation under the default
choices, all governance areas filled, zero synthetic evidence.

> **Principle II note (deliberate, justified).** Principle II forbids
> `private`/`internal`/`public` on top-level bindings; this feature *removes* ~17
> such qualifiers (US2), moving net **toward** the principle. The one retained use
> of a module access modifier — `module internal WidgetLowering` with no `.fsi` —
> is the established repo escape hatch for an assembly-internal, cross-file module
> that must stay off the public surface without a signature file (identical to the
> kept `module internal SceneRenderer`, FR-006). A plain `module WidgetLowering`
> with no `.fsi` would be **public** by default and break FR-012; an `.fsi` would
> move the per-package baseline. `internal`-no-`.fsi` is the only zero-surface
> option and is justified here.

## Project Structure

### Source touched

```
src/Controls/Widgets/WidgetLowering.fs   # NEW — module internal WidgetLowering (no .fsi)
                                         #   withKeyOpt, onString, onStringList, a11y, intentToString
src/Controls/Widgets/Buttons.fs          # drop local withKeyOpt + a11y; module private -> module; ref shared
src/Controls/Widgets/Navigation.fs       # drop withKeyOpt + onString; module private -> module
src/Controls/Widgets/Pickers.fs          # drop withKeyOpt + a11y; module private -> module
src/Controls/Widgets/ChartsWidgets.fs    # drop withKeyOpt + onString; module private -> module
src/Controls/Widgets/Input.fs            # drop withKeyOpt + intentStyle->string; module private -> module
src/Controls/Widgets/Containers.fs       # drop withKeyOpt; module private -> module; SlotName at fill edge
src/Controls/Widgets/Display.fs          # drop withKeyOpt; module private -> module
src/Controls/Widgets/Overlay.fs          # drop withKeyOpt + onString; module private -> module
src/Controls/Widgets/CollectionsWidgets.fs # drop onString/onStringList; module private -> module
src/Controls/Widgets/Primitives.fs       # drop withKeyOpt + intentStyle->string; module private -> module (LegacyControls); SlotName at fill edge
src/Controls/Control.fs                  # onChanged{Bool,Float,String}+tryParseFloat; AttrKey reader; SlotName in slotRegions/lowerSlots
src/Controls/DataGrid.fs                 # route closed-set tryLast/hasAttr reads through AttrKey
src/Controls/Reconcile.fs                # drop 3 redundant `let private` (keep comments)
src/Controls/RetainedRender.fs           # drop 4 redundant `let private` (keep comments)
src/Scene/Scene.fs                       # internal EvidenceStage DU; field stays string at the edge
src/SkiaViewer/SkiaViewer.fs             # internal RendererMode DU; public RendererMode field stays string
src/Controls/Controls.fsproj             # insert WidgetLowering.fs before Widgets/Primitives.fs (line ~92)
```

### NOT edited (read for grounding / explicitly preserved)

```
src/Controls/Types.fs(i)                 # public StandardAttributeName UNCHANGED (D3); AttrValue.SlotFillsValue carrier stays (string * Control) list
src/SkiaViewer/SceneRenderer.fs          # module internal SceneRenderer — KEEP (FR-006)
src/Controls/Control.fs (ControlInternals) # ~40 `let private` inside the EXPOSED ControlInternals — KEEP (FR-006)
<InternalsVisibleTo> test seams          # Reconcile/RetainedRender/ControlInternals/ControlRuntime/ControlsElmish — KEEP
```

## Design decisions (resolved in Phase 0)

- **D1 — Shared module = `module internal WidgetLowering`, no `.fsi`, in a new
  `Widgets/WidgetLowering.fs`.** Inserted in `Controls.fsproj` immediately before
  `Widgets/Primitives.fs` (after `Control.fs`/`Reconcile`/`RetainedRender`, so
  `Control.withKey`, `Attr.onWith`, `Attr<'msg>`, `Control<'msg>`, `ControlEvent`,
  and the style/intent + accessibility types are all in scope). It exposes
  `withKeyOpt`, `onString`, `onStringList`, the `a11y` accessibility-metadata
  builder, and `intentToString`. `internal`-no-`.fsi` is the only choice that is
  both cross-file-visible and zero-public-surface (see the Principle II note).
- **D2 — `onChanged*` + `tryParseFloat` live at `Control.fs` module scope, not in
  `WidgetLowering`.** The eight `onChanged` copies are inside the per-kind builder
  modules *within `Control.fs`* and read `event.Payload` of the local
  `ControlEvent`; keeping the three shape helpers and `tryParseFloat : string ->
  float option` co-located in `Control.fs` (absent from `Control.fsi`) is the
  minimal change and avoids widening `WidgetLowering`'s remit. The float builders
  reduce to `event.Payload |> Option.bind tryParseFloat |> Option.defaultValue
  0.0 |> map`.
- **D3 — Internal-only `AttrKey`; the public `StandardAttributeName` is NOT
  expanded.** This is the explicit resolution of the FR-007 ↔ FR-012 tension. The
  closed control-intrinsic names the reader needs
  (`styleClasses`/`visualState`/`slot`/`orientation`/`width`/`height`/…) are
  **absent** from the 13-case public `StandardAttributeName`; adding them would be
  a public-surface change requiring baseline recapture and escalation. The
  banner constraint (FR-011/FR-012) wins: introduce an internal `AttrKey` DU in
  `Control.fs` with a `name : AttrKey -> string` projection (single-sourcing the
  literal, building on feature 101's `[<Literal>] AttrWidth/Height/Orientation`),
  and a typed `tryKey : AttrKey -> Attr<'msg> list -> _` that the closed-set reads
  use. The string-keyed `tryLast`/`hasAttr` stay for genuinely dynamic/open names.
  **Zero public-surface delta.**
- **D4 — Internal `SlotName` DU; public `SlotFillsValue` carrier stays string.**
  `SlotName` (`Leading|Trailing|Header|Footer`) is internal, used only by
  `slotRegions` and `lowerSlots`/the slot match in `Control.fs`. The widget fills
  in `Containers.fs`/`Primitives.fs` keep yielding into the **unchanged** public
  `AttrValue.SlotFillsValue : (string * Control<'msg>) list` carrier; the string
  is parsed to `SlotName` once at the consumption edge. Preserves feature 095's
  deliberate omission of a public `SlotName` (FR-008).
- **D5 — Internal Scene `EvidenceStage` DU; serialized field stays string.** The
  `BlockedStage`/`DiagnosticCategory` values are only ever `"scene"`/`"renderer"`
  (Scene.fs:736-744). An internal two-case DU (`Scene|Renderer`) drives the
  internal comparison; the public/serialized record fields remain `string`,
  written via a single `stage -> string` projection at construction — byte-
  identical evidence text.
- **D6 — Internal renderer-mode DU; public `RendererMode` field stays string.**
  The case-insensitive dispatch compares at `SkiaViewer.fs:2016/2023/2047` against
  the closed set
  (`default`/`skia`/`deterministic-scene`/`unsupported-host`/`metadata-hash`/`pixel-readback`).
  Parse `request.RendererMode` once at the edge into an internal DU and make the
  `match` exhaustive; every public `RendererMode` output/serialized field stays an
  unchanged string (FR-009, §5C).
- **D7 — Touch exactly the audit-certified redundant qualifiers, keep the comments
  and the keep-list.** Remove `private` from the 10 `module private *Lowering`
  declarations, `Reconcile.fs` (`attrValueEqual`/`diffAttrs`/`isKeepOp`), and
  `RetainedRender.fs` (`childPath`/`clockDuration`/`fadeAnimation`/
  `currentOpacity`). Retain each "file-scoped lowering helpers, hidden by `<X>.fsi`"
  comment (Edge Cases: a future second module in the file must not silently gain
  access). Do **not** touch `module internal SceneRenderer`, the
  `InternalsVisibleTo` seams, or the `let private` helpers inside the exposed
  `ControlInternals` (FR-006). The other `let private` bindings the audit did not
  cite (`Reconcile.applyAttrChanges`, `RetainedRender.fadeOutAnimation`/
  `firstFrameCollisions`) are left as-is to keep the diff exactly the audit's
  ~17-site set.

## Phase 0 — Research

See [research.md](./research.md). All citations re-verified against the working
tree (line numbers had shifted: `onChanged` now at
`Control.fs:1606/1611/1616/1621/1628/1633/1639/1683`; `slotRegions` at
`Control.fs:99`; `StandardAttributeName` at `Types.fs:80`/`Types.fsi:86`;
`RetainedRender` privates at `73/87/100/113/123`). No `NEEDS CLARIFICATION`
remained from the spec; the one open choice (FR-012 public-DU expansion vs.
internal-only key) is resolved as D3 (internal-only, zero-surface).

## Phase 1 — Design & Contracts

- **Entities** — [data-model.md](./data-model.md): the new shared module and the
  four internal DUs, their closed value sets, their string boundaries, and the
  consolidation map (every former helper site → its single new home).
- **Contracts** — [contracts/internal-surface.md](./contracts/internal-surface.md):
  the exact internal-only shapes (`WidgetLowering` members, `AttrKey`, `SlotName`,
  `EvidenceStage`, the renderer-mode DU) with the invariant that **no public
  `.fsi` line changes**; and
  [contracts/parity.md](./contracts/parity.md): the byte-/structural-identity
  contract — the lowered `Control<'msg>` for every affected widget and every
  serialized evidence/output string MUST be identical pre/post.
- **Quickstart** — [quickstart.md](./quickstart.md): how to validate (`Route`
  first; the gates it prints; the zero-`.fsi`-delta diff proof; the parity
  assertion; grep transcripts for SC-001…SC-003).
- **Agent context** — `AGENTS.md` SPECKIT marker updated to point at this plan.

## Re-evaluation (post-design)

No new violations introduced. Still Tier 2; the default choices (D3–D6) hold the
public `.fsi` surface at **zero delta**; US2 moves net toward Principle II; the
sole retained module modifier (`internal WidgetLowering`) is the justified
SceneRenderer precedent; no synthetic evidence; all governance areas filled.
**Constitution Check: PASS.**
