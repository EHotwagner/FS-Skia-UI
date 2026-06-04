# Data Model: Feature 063

This feature is largely governance/diagnostics/skill content plus one rendering
refactor and one shipped helper. The "entities" are therefore the public/internal
types that change shape, the diagnostic records that change labeling, and the new
shipped surface. Pure-content edits (skill bodies, doc notes) carry no entity.

---

## 1. `SceneRenderer.paintNode` (D1/D2/D3 — internal, `src/SkiaViewer/`)

The single shared scene painter both renderers delegate to.

- **Signature**: `paintNode: SKCanvas -> SceneNode -> unit` (non-public; no `.fsi`
  export, so SkiaViewer's per-package surface baseline is unchanged — D11).
- **Coverage invariant (tested + compiler-enforced)**: the `match` over `SceneNode`
  is **exhaustive — no wildcard**. Every case modeled in `src/Scene/Scene.fs`
  (`Empty`, `Group`, `Rectangle`, `PaintedRectangle`, `Circle`, `FilledEllipse`,
  `Ellipse`, `Line`, `Path`, `Points`, `Vertices`, `Arc`, `Text`, `TextRun`,
  `Image`, `ClipNode`, `RegionNode`, `ColorSpaceNode`, `PerspectiveNode`,
  `PictureNode`, `Chart`) draws to pixels. Adding a future `SceneNode` case breaks
  the build until handled.
- **`Text`/`TextRun`**: render real glyphs via the moved `drawTextWithFallback`
  (`SKTypeface.Default` + vector fallback), never a placeholder rect.
- **Relationships**: replaces the bodies of `drawScene` (`Vulkan.fs:1005-1160`) and
  `drawScreenshotScene` (`SkiaViewer.fs:1771-1808`); the moved helpers (`skColor`,
  `configurePaint`, `toSkPath`, `drawTextWithFallback`, support fns) live with it.

**State transitions**: none (pure draw walk over an immutable `SceneNode`).

---

## 2. `ScreenshotPixelContentValidation` (D3 — unchanged this round)

Retained as-is (`SkiaViewer.fs:254-258`): `PixelContentNonBlank` /
`PixelContentBlank` / `PixelContentUnreadable` / `PixelContentNotValidated`. After
D1 a non-blank result is honest (real primitive pixels, not a placeholder), so **no
new case is added** — explicitly recorded so a reviewer sees the omission is
deliberate.

---

## 3. `SymbolCrossCheck` target wiring (D4 — `build/Governance/**`)

No change to the analyzer types (`SymbolKind`, `Artifact`,
`Symbol = { Kind; Name; PresentIn }`) or to `diff`/`render` — they already exist and
`render` already emits `## Symbol consistency (analyze pass G)`. New entities are the
**target and its effect**:

- `Targets.Target` gains a `SymbolCrossCheck` case (+ `allTargets`, `name`,
  `directPrerequisites`).
- `Engine/Model.fs` `BuildEffect` gains `SymbolCrossCheckAnalyze`.
- `ValidationContract.knownGates` gains `"SymbolCrossCheck"`.
- **Output artifact**: `readiness/symbol-cross-check.md` (the rendered markdown),
  required via `RequireFiles`.
- **Inputs**: `plan.md` / `data-model.md` / `tasks.md` resolved from the feature dir
  (`BuildModel`), not CLI args.

**Validation rule**: it is a **command/diagnostic, not a hard gate** — never added
to a routing rule's `RequiredGates`; intentionally design-only symbols are reported
for human judgment, never hard-failed.

---

## 4. Readiness-contract diagnostic labels (D5 — `Evidence/Render.fs`)

The printed readiness-contract failure record changes **labels only**:

| Field (source) | Before label | After label |
|---|---|---|
| `Required = Some terms` (`Scans.fs:101`, full enforced set) | `required-tokens:` | `full-required-set:` |
| `MissingTerms` (`Scans.fs:104-106`, absent subset) | `missing:` | `absent-from-file:` |

No data shape change; the two fields already exist. The single-source enforcement
(`terms`) is untouched.

---

## 5. `FS.Skia.UI.SkillSupport.Wrap` (D10 — new shipped surface, Tier-1)

```fsharp
namespace FS.Skia.UI.SkillSupport
module Wrap =
    val wrapDeltaX: worldWidth: float -> fromX: float -> toX: float -> float
```

- **Purity/determinism invariant (tested)**: pure scalar arithmetic; no state, no
  I/O, no `Scene`/`Layout` dependency (keeps SkillSupport dependency-light).
- **Range invariant (tested)**: result ∈ `(-worldWidth/2, worldWidth/2]` for
  `worldWidth > 0`; `wrapDeltaX w a b` is the shortest signed distance from `a` to
  `b` modulo `w`.
- **Surface relationship**: the only entity adding public `.fsi`, so the
  per-package baseline `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt`
  gains the `Wrap` module and `SkillSupport.fsproj` gains `Wrap.fsi`/`Wrap.fs`
  Compile entries — the lone Tier-1 escalation (FR-011, Principle II).

---

## 6. Documented-not-shipped (D9 — no entity)

The `--evidence-run` deterministic-summary discipline (pure model + held-input
script + `InvariantCulture`/`F3` + `determinism=byte-identical` marker) and the
camera-centered projection are **documented**, not typed surface. Recorded here so
the disposition is explicit: common summary core =
`status`/`command`/`seed`/`frame-count`/`score`/`determinism`; per-game fields vary,
so no shipped record. Next recurrence bar: a *stable* cross-game field set.
