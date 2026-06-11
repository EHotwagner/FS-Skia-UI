---
title: Housekeeping Code-Quality Audit
index: 16
description: Code-smell, duplication, access-qualifier, and custom-equality audit of src/** with prioritized, behavior-preserving remediations.
---

# Housekeeping Code-Quality Audit

Generated 2026-06-11T14:24Z. Scope is the implementation source under `src/**`
(excluding `obj/`, `bin/`). This is a maintainability pass, not a feature change:
every recommendation below is behavior-preserving and most are mechanical. The
codebase is in good shape — signature-file coverage is ~98%, equality discipline
is deliberate, and there is no broad architectural rot. The findings are
localized accumulation: copy-paste helpers in the typed-widget lowering layer,
a few oversized coordination files, and a thin layer of redundant access
qualifiers that the `.fsi` files already enforce.

All file:line citations were spot-verified against the working tree at the time
of writing.

## Executive Summary

| Theme | Severity | Effort | Net effect |
|-------|----------|--------|-----------|
| Duplicated `withKeyOpt` / `onString` lowering helpers (13 copies) | Medium | Low | Single source, ~50 lines removed |
| Duplicated `onChanged` parsers in `Control.fs` (8 copies, 3 shapes) | Medium | Low | 4 helpers replace 8 inline lambdas |
| Oversized coordination files (`SkiaViewer.fs`, `Control.fs`, `Vulkan.fs`) | Medium | High | Smaller compile units, clearer ownership |
| Redundant `private` inside already-signed/internal modules (~16 sites) | Low | Low | Less noise; `.fsi` is the single boundary |
| Stringly-typed attribute lookups in `Control.fs` | Low | Medium | Compiler-checked keys |
| Stringly identifiers → DUs (internal: attr-names, slots, scene-stage, renderer-mode) | Low–Med | Low–Med | Exhaustive matches; most public ids deliberately stay string |
| Custom equality on `AttrValue<'msg>` | Low | Medium | Mostly cosmetic — current design is sound |

The recommended order is duplication first (highest value, lowest risk), then
the redundant-qualifier cleanup, then the file splits as a separate dedicated
pass.

## 1. Duplication

### 1.1 `withKeyOpt` repeated verbatim in 9 widget-lowering modules

Every typed-widget module redefines the identical key-application helper:

```fsharp
let withKeyOpt id control =
    match id with
    | Some key -> Control.withKey key control
    | None -> control
```

Confirmed sites:

- `src/Controls/Widgets/Buttons.fs:27`
- `src/Controls/Widgets/Navigation.fs:31`
- `src/Controls/Widgets/Pickers.fs:33`
- `src/Controls/Widgets/ChartsWidgets.fs:34`
- `src/Controls/Widgets/Input.fs:37`
- `src/Controls/Widgets/Containers.fs:45`
- `src/Controls/Widgets/Display.fs:45`
- `src/Controls/Widgets/Overlay.fs:28`
- `src/Controls/Widgets/Primitives.fs:47` (in `LegacyControls`)

**Remediation:** Add one shared `WidgetLowering` module (compiled before the
widget modules in the Controls fsproj order) exposing `withKeyOpt`, and delete
the nine copies. This is the single highest-value, lowest-risk cleanup in the
report.

### 1.2 `onString` / `onStringList` event helpers repeated

The same string-event adapter is copied across four modules:

```fsharp
let onString (eventKind: string) (map: string -> 'msg) : Attr<'msg> =
    Attr.onWith eventKind (fun event -> event.Payload |> Option.defaultValue "" |> map)
```

- `src/Controls/Widgets/Navigation.fs:36`
- `src/Controls/Widgets/Overlay.fs:33`
- `src/Controls/Widgets/ChartsWidgets.fs:39`
- `src/Controls/Widgets/CollectionsWidgets.fs:46`

`CollectionsWidgets.fs:49` adds an `onStringList` variant of the same shape.

**Remediation:** Fold `onString` / `onStringList` into the same shared
`WidgetLowering` module proposed in 1.1.

### 1.3 `onChanged` parsers duplicated in `Control.fs` (8 copies, 3 shapes)

`src/Controls/Control.fs` defines `onChanged` eight times across the per-kind
builder modules, in three distinct shapes:

- **Bool** (`Option.exists ((=) "true")`): `Control.fs:1587` (CheckBox),
  `Control.fs:1592` (Switch)
- **Float** (`Double.TryParse … |> Option.defaultValue 0.0`): `Control.fs:1597`
  (Slider), `Control.fs:1602` (NumericInput)
- **String** (`Option.defaultValue ""`): `Control.fs:1609` (TextBox),
  `Control.fs:1614` (TextArea), `Control.fs:1620` (RadioGroup), `Control.fs:1664`
  (Tabs)

The float shape also contains an inline 217-character lambda with a nested
`match Double.TryParse` (see §2.3).

**Remediation:** Define `onChangedBool`, `onChangedFloat`, `onChangedString`
once at module scope and have each builder reference them. Extract the float
parser into a named `tryParseFloat : string -> float option` first (§2.3).

### 1.4 Smaller duplications worth folding in opportunistically

- **`intentStyle` enum→string** duplicated at `Primitives.fs:52` and
  `Input.fs:42` (`Primary→"primary"`, etc.). Promote to one
  `Style.intentToString`.
- **Accessibility-metadata builder** nearly identical at `Buttons.fs:35` and
  `Pickers.fs:40` (role/name + `Accessibility.keyboard true ["Enter";"Space"]`).
  Extract a shared `a11y` helper.
- **`Reconcile.fs:70`** builds two `Dictionary` maps with imperative `for`
  loops; `prevAttrs |> List.map (fun a -> a.Name, a) |> Map.ofList` is more
  idiomatic (minor).

## 2. Code Smells

### 2.1 Oversized coordination files

Three files carry multiple unrelated responsibilities in one compilation unit:

| File | Lines | Distinct responsibilities |
|------|------:|---------------------------|
| `src/SkiaViewer/SkiaViewer.fs` | 2865 | window-behavior validation (`609`), state classification (`672`), window-observation diagnosis (`747`), host-environment capability detection (`862`), persistent + legacy window launch (`1225`, `1379`), diagnostic capture/reporting (`544`) |
| `src/Controls/Control.fs` | 1689 | attribute-lookup helpers (`23`), per-kind preview geometry (`290`+), chart rendering (`391`+), layout-node creation (`1070`+), diagnostics aggregation (`1135`+), control builders (`1549`+) |
| `src/SkiaViewer/Host/Vulkan.fs` | 1426 | resource-ledger tracking (`33`), window init + event handling (`292`), swapchain/frame render + readback (`700`+) |

**Remediation:** Treat as a dedicated, separate extraction pass (higher risk,
touches hot paths). Suggested splits — `SkiaViewer` → `WindowValidation` /
`WindowDiagnostics` / `HostEnvironment` / `WindowPresentation`; `Control.fs` →
`ControlAttributes` / `ControlGeometry` / `ControlLayout` / `ControlDiagnostics`;
`Vulkan.fs` → `VulkanResources` / `WindowHost` / `VulkanRenderer`. Keep public
`.fsi` surfaces stable; this is purely internal reorganization. Do this only
after §1 and §3 land so the diff stays reviewable.

### 2.2 Stringly-typed attribute lookups in `Control.fs`

`Control.fs` reads attributes by raw string name ("text", "value",
"styleClasses", "visualState", "slot", "accessibility", "nodes",
"richTextRuns", "orientation") at `Control.fs:29, 54, 66, 83, 136, 269, 283,
1174` and elsewhere, bypassing the `StandardAttributeName` enum defined in the
attributes module. A typo is a silent miss, not a compile error.

**Remediation:** Route lookups through the existing typed attribute-name
constants (or a small `AttributeKey` helper). Medium effort; do it incrementally.

### 2.3 Long inline lambda with nested match

`Control.fs:1597` packs a 217-char lambda with a nested `match Double.TryParse`
inside an `Option` pipeline. Combined with §1.3, extract
`tryParseFloat : string -> float option` and the call site reduces to
`event.Payload |> Option.bind tryParseFloat |> Option.defaultValue 0.0 |> map`.

### 2.4 Mutable-heavy / ref-threaded blocks (lower priority)

- `src/Testing/Testing.fs:901-1050` — the pixel-analysis `validate` accumulates
  ~14 `mutable` locals across nested loops. A fold over an accumulator record
  (counts / bounds / metrics) would make the state flow legible.
- `src/SkiaViewer/SkiaViewer.fs:1225-1228` — `windowOpened`, `framePresented`,
  `closeReason` threaded as `ref` cells across Elmish update closures; an
  explicit threaded state record reads better.
- `src/Input/KeyboardInput.fs:463-581` — `parseYaml` is a ~118-line deeply
  nested `Option.bind`/`match` ladder; extract per-section parsers
  (`parseLayout`, `parseMode`, `parseBinding`, …).

These are honestly judgment calls — flagged for awareness, not urgent.

## 3. Access Qualifiers vs `.fsi` Visibility

Signature-file coverage is excellent: **57 of 58** source modules have a paired
`.fsi`; the lone exception, `src/SkiaViewer/SceneRenderer.fs:17`, correctly uses
`module internal` (no `.fsi`) to keep an exhaustive `SceneNode` match off the
public surface. That one is **load-bearing — keep it.**

The cleanup opportunity is **redundant in-source `private`** where the `.fsi`
(or an enclosing `module internal`) is already the encapsulation boundary:

- **Widget lowering modules** — `module private <Name>Lowering` at
  `Buttons.fs:26`, `Navigation.fs:30`, `Pickers.fs:32`, `ChartsWidgets.fs:33`,
  `CollectionsWidgets.fs:34`, `Containers.fs:44`, `Display.fs:44`,
  `Input.fs:36`, `Overlay.fs:27`, and `Primitives.fs:46` (`LegacyControls`).
  These modules are simply absent from their `.fsi`, which already hides them;
  the `private` keyword adds only visual noise.
- **`let private` inside already-internal modules** —
  `Reconcile.fs:46,69,90` (`attrValueEqual`, `diffAttrs`, `isKeepOp`) and
  `RetainedRender.fs:67,81,94,104` (`childPath`, `clockDuration`,
  `fadeAnimation`, `currentOpacity`). The enclosing `module internal` (declared
  in the `.fsi`) already prevents external access.

**Remediation:** Drop the redundant `private` from these ~16 sites so the `.fsi`
remains the single, obvious visibility boundary. Zero functional change.

**Caveat / nuance:** `private` also restricts visibility from *sibling modules
in the same file*. Each file above contains a single such module, so removal is
safe today — but if a file later grows a second module that should not see these
helpers, the keyword would matter again. Keep the explanatory comments
("file-scoped lowering helpers, hidden by <X>.fsi") when removing the keyword.

**Keep as-is (load-bearing, do not touch):**

- `module internal SceneRenderer` (no `.fsi`, exhaustiveness guard).
- All `InternalsVisibleTo`-backed `internal` test seams: `Reconcile`,
  `RetainedRender`, `ControlInternals` (in `Control.fs`), `ControlRuntime`,
  and the `ControlsElmish` internals — these are deliberate test access points.
- The ~40 `let private` helpers inside *exposed* `ControlInternals` (e.g.
  `Control.fs:340` `palette`, `352` `mkText`, chart geometry `405-502`). These
  are genuine implementation hiding within a module that *is* in the `.fsi`.

## 4. Custom Equality

The question — could `[<CustomEquality>]` interfaces make comparison code more
readable — has a deliberately modest answer: **the codebase is already
well-disciplined here, and the one candidate is borderline.**

- There are **no `sprintf "%A"`-based equality hacks in production source.**
  `%A` appears only in logging/diagnostics. (Any `%A` comparison lives in test
  code, which is lower priority and often legitimate for structural assertions.)
- `obj.ReferenceEquals` / `obj.Equals` are **not scattered** — they are confined
  to one deliberately documented place.

**The single candidate — `AttrValue<'msg>`** (`src/Controls/Types.fs:269-284`).
It cannot derive equality because it carries `EventValue of (ControlEvent ->
'msg)`, `MessageValue of 'msg`, and `UntypedValue of obj`. Comparison is handled
by the total, conservative `attrValueEqual` at `src/Controls/Reconcile.fs:46-65`
(structural cases use `=`; function/opaque cases fall back to reference/`Equals`,
with mismatched cases returning `false`). The function is explicitly documented
as intentionally conservative.

**Assessment:** Moving this into `[<CustomEquality; NoComparison>]` would
colocate the logic with the type and let `=` "just work" at call sites — a
readability win in the abstract. But the type is generic in `'msg`, so a custom
`Equals` override would be `obj`-typed and *less* type-safe than the current
explicit function, and `Control<'msg>` (which embeds `AttrValue`) is never
compared whole anyway — `Reconcile.diff` compares fields individually by design.
**Recommendation: leave as-is.** This is the rare case where the "hack" is
actually the cleaner design; flagged only so future readers don't re-derive the
conclusion.

## 5. Stringly-Typed Identifiers (DU / wrapper candidates)

A dedicated sweep for strings used as *identifiers, tags, kinds, or modes* drawn
from a **closed** set. The headline finding is encouraging: most domain
vocabularies are **already DUs** — `AccessibilityRole`, `ValidationState`,
`VisualState`, `StyleVariant`, `StyleClass`, `StackOrientation`
(`src/Controls/Types.fs`), the entire Layout vocabulary
(`HorizontalAlignment`, `VerticalAlignment`, `LayoutDirection`, `LayoutAlign`),
Scene (`SceneElementKind`, `ShapePlacement`, `LayoutProofLevel`), Input
(`InputSeverity`, `InputDiagnosticCode`, `ModeKind`, `Hand`, `Finger`), Color
(`Role`, `Verdict`, `StepRole`, `RampVariant`), and the KeyboardInput display
enums are all properly typed. No action there.

The remaining string-typed identifiers split sharply by **risk**, because the
high-value ones (`ControlKind`, `ControlId`, attribute names, the SkiaViewer
`*Mode`/diagnostic fields) live on the **public `.fsi` surface** — changing them
is a *consumer-contract* change that `Route` escalates to the maintainer-verify
path and that ripples through `catalog.yml`, `ApiSurfaceGen`, and the
byte-identity tests. The recommendations below are tiered accordingly.

### 5A. Low-risk, internal-only (recommended)

These never cross a public `.fsi`, so they're behavior-preserving internal
refactors with real safety upside:

- **Internal attribute-name lookups → typed keys.** `Control.fs` reads
  attributes by raw string at `Control.fs:29, 54, 66, 83, 136, 269, 283, 1174`
  (also `DataGrid.fs:161, 170, 221`). A `StandardAttributeName` DU *already
  exists* (`Types.fs:80`) but is only used by the catalog `ControlSchema`
  (`Types.fs:106-108`), not the runtime reader path. Expand it to cover the
  closed control-intrinsic names (text/value/styleClasses/visualState/slot/
  orientation/width/height/…) and route the internal `tryLast`/`hasAttr` calls
  through it. This is the typed-up version of §2.2 and the single highest-value
  item here — typos in internal attribute reads become compile errors.
- **Internal slot-name DU.** `slotRegions` (`Control.fs:99-103`) and the slot
  fills (`Containers.fs:132`, `Primitives.fs:94`) pass raw `"leading"`/
  `"trailing"`/`"header"`/`"footer"` strings in `(string * Control<'msg>) list`
  tuples. Feature 095 *deliberately* kept `SlotName` off the public surface, so
  introduce an **internal** `SlotName` DU and keep the carrier internal. Closed
  set, no public change.
- **Internal Scene evidence stage/category.**
  `SceneEvidenceFailure.BlockedStage` and `.DiagnosticCategory`
  (`Scene.fs:701, 703`) are internal strings assigned only `"scene"`/`"renderer"`
  (`Scene.fs:736-744`). A two-case internal DU removes the stringly compare.
- **Internal `RendererMode` comparison DU.** The renderer-mode *dispatch*
  compares the string case-insensitively at `SkiaViewer.fs:2016, 2023, 2047`
  against a closed set (`default`/`skia`/`deterministic-scene`/`unsupported-host`/
  `metadata-hash`/`pixel-readback`). An internal DU + one parse-at-the-edge keeps
  the public field a string (see 5C) while making the internal `match`
  exhaustive.

### 5B. Medium-risk, escalates — judgment calls

- **`ControlId` single-case wrapper.** `type ControlId = string` (`Types.fs:6`).
  A `ControlId of string` wrapper is zero runtime cost and stops string/id
  confusion in signatures, *but* it's threaded through `Control<'msg>.Key`,
  the event-binding path, and the positional-path scheme, and it's public — so it
  escalates and touches many call sites. Worth doing, but as its own scoped
  change, not folded into a cleanup batch.
- **SkiaViewer public diagnostic/mode fields.** `DiagnosticClass`
  (`SkiaViewer.fsi:341`, 3 closed values, compared at `SkiaViewer.fs:947`) and
  `ViewerLaunchOutcome.Mode` (`SkiaViewer.fsi:347`, `interactive-window`/
  `persistent-evidence`) are the closed-set members of the public-surface
  strings. Converting them improves the public API but is a consumer-contract
  break for a modest gain. Recommend only if/when that `.fsi` is being revised
  for another reason.

### 5C. Keep as string (deliberate / open set)

- **`ControlKind` (`Types.fs:7`).** Looks like the biggest prize, but it is a
  *deliberately open* runtime set: `StandardControlKind` (`Types.fs:58`) already
  exists **with a `Custom of string` case** and backs the typed front door +
  schema validation, while the runtime record stays a bare string for
  forward-extensibility (custom controls). Forcing a mandatory DU would be a
  large breaking change across the whole lowering pipeline for little gain over
  the existing typed entry point. **Keep.**
- **Public display/serialization strings** in `SkiaViewer.fsi` — `Status`,
  `InputDispatch`, `EvidenceKind`, and the various `RendererMode` *output*
  fields are written into evidence text; string is the right type at that
  boundary. (Only the internally-*compared* ones in 5A/5B are worth typing.)
- **Consumer metadata** — DataGrid `columnKey`/`rowKey` (`DataGrid.fs:190-191`)
  and similar are application-domain keys, genuinely open. **Keep.**
- **`ControlEvent.Kind` (`Types.fs:237`).** Already typed at the boundary via
  `StandardEventKind` (`Types.fs:71`); the stored string is the canonical
  lowercased internal form. A clarifying comment is the most that's warranted.

## Recommended Sequencing

1. **§1 Duplication** — introduce `WidgetLowering` (`withKeyOpt`, `onString`,
   `onStringList`, `a11y`, `intentToString`) and `onChanged{Bool,Float,String}`
   + `tryParseFloat` in `Control.fs`. Mechanical, high value. Run `Route` — these
   touch `src/Controls/**` only (inner-loop `Dev` unless an `.fsi` changes).
2. **§3 Redundant qualifiers** — drop ~16 noise `private` keywords. Trivial,
   keep the comments.
3. **§5A Internal DUs** — expand `StandardAttributeName` onto the runtime reader
   path (subsumes §2.2), plus the internal slot / scene-stage / renderer-mode
   DUs. Internal-only, stays in the inner-loop tier.
4. **§2.2 File splits** — schedule as a separate, dedicated pass with stable
   `.fsi` surfaces; highest risk, do last.
5. **§5B `ControlId` wrapper** — its own scoped change; it escalates (public).
6. **§2.4 / §4 / §5C** — opportunistic or deliberately-no-op; not required.

Run `./fake.sh build -t Route` before validating each batch and run only the
gates it prints; the typed-widget and `Control.fs` changes stay within
`src/Controls/**` and should route to the light inner-loop tier unless a public
`.fsi` is edited.
