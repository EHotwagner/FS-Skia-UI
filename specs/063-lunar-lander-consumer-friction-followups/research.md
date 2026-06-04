# Research & Design Decisions: Feature 063

Resolves every design choice the spec (FR-001…FR-011) deferred to `/speckit-plan`.
Each decision records what was chosen, why, and the alternatives rejected. Grounded
in the **post-062** source (template `0.1.85`, libs `0.1.66-preview.1`) — the same
packages `LunarLander1` was generated from — and verified against the actual
renderer, governance, and SkillSupport code.

---

## D1 — Single shared scene painter (FR-001, primary framework fix)

**Decision.** Eliminate the second, stunted renderer. Extract one shared
**`SceneRenderer.paintNode (canvas: SKCanvas) (node: SceneNode)`** walker (a new
non-public module compiled before both consumers in `src/SkiaViewer/`), move the
existing paint helpers (`skColor`, `configurePaint`, `toSkPath`,
`drawTextWithFallback`, and their support functions, currently private inside the
`VulkanHost` module at `src/SkiaViewer/Host/Vulkan.fs:769-1003`) into it, and have
**both** the interactive `drawScene` (`Vulkan.fs:1005-1160`) and the screenshot
`drawScreenshotScene` (`SkiaViewer.fs:1771-1808`) delegate to it. The `match` is
**exhaustive — no wildcard `_` case** — so the placeholder-rect substitution at
`SkiaViewer.fs:1804-1806` is deleted outright.

**Rationale.** The defect is two divergent renderers; the durable fix is *one*
renderer. Both draw onto an `SKCanvas` (the screenshot path uses a raster
`SKBitmap` canvas, `SkiaViewer.fs:1841-1844`) and every primitive
(`DrawLine`/`DrawPath`/`DrawArc`/`DrawPoints`/`DrawVertices`/`DrawText`) works
identically on raster and GPU surfaces — so there is **no technical reason** any
primitive cannot render in evidence mode. An exhaustive shared match also makes the
F# compiler the ongoing guard: if a future `SceneNode` case is added, the build
breaks until both paths handle it — they can never silently diverge again. This is
strictly stronger than patching `drawScreenshotScene`'s match to add `Line`/`Path`.

**Alternatives rejected.** (a) *Add the missing cases to `drawScreenshotScene`'s
own match, keep two renderers* — fixes today's symptom but leaves the divergence
mechanism (and the wildcard) in place to rot again. (b) *Make the `VulkanHost`
helpers `public` and call them cross-module* — exposes GPU-host internals as public
surface for no reason; a dedicated shared module is cleaner and stays non-public.
(c) *Track unrendered nodes via a `RendererDiagnostics` accumulator* — unnecessary
once the match is exhaustive: there is no unrendered-but-claimed-visible set left to
track (see D3).

---

## D2 — Real-glyph text in evidence mode (FR-001, resolves the `Text` edge case)

**Decision.** Evidence-mode `Text`/`TextRun` render as **real glyphs** via the
shared `drawTextWithFallback`, not the placeholder rectangle the screenshot path
draws today (`SkiaViewer.fs:1796-1799`). `drawTextWithFallback`
(`Vulkan.fs:987-1003`) uses `SKFont(SKTypeface.Default, …)` with a hand-drawn
vector-glyph fallback (`drawVectorText`, `Vulkan.fs:966-985`) — both work on the
raster evidence canvas with no GPU or window-system dependency.

**Rationale.** The spec's interacting-requirements note left `Text` as a planning
fork (real glyphs vs. classified placeholder). Since the shared painter already
owns a working raster text path, real glyphs are the faithful outcome and cost
nothing extra once D1 lands — `Text` is just one more case routed through the shared
walker. SC-001's before/after capture then shows real HUD text, not a box.

**Alternatives rejected.** *Keep `Text` as an explicitly-classified non-glyph
placeholder* — only justified if real rasterization were infeasible; it is not, so a
placeholder would be a self-imposed fidelity loss and would keep the visual-proof
honesty vocabulary carrying a permanent `Text`-is-fake caveat.

---

## D3 — False-confidence fixed at the root; document the unified renderer (FR-002)

**Decision.** The false-"scene is visible" confidence is fixed **structurally** by
D1, not by new tracking machinery. Today a scene of only `Line` nodes drew a single
40×40 teal placeholder, whose non-zero alpha made `pngDimensionsAndNonBlank`
(`SkiaViewer.fs:1810-1834`) report `PixelContentNonBlank` — "proves scene
rendering" on an effectively invisible scene. After D1 there is **no placeholder**:
those `Line` nodes render as actual pixels (genuine non-blank) or, if a scene is
truly empty, the existing `PixelContentBlank` path reports honestly. The remaining
work is **documentation**: the `fs-skia-scene` skill (and `docs/scaffold-map.md`'s
evidence note) states that the **interactive and evidence renderers are now the same
shared painter**, so `Scene.describe` / node-count assertions are understood as
*structural* checks and the image is the *visual* proof — they are no longer
conflated. The existing blank/non-blank/unreadable classification
(`ScreenshotPixelContentValidation`) is retained as-is.

**Rationale.** Once the wildcard is gone the exhaustive match guarantees every
node the framework models is drawn, so the "passes node count yet invisible" trap
cannot recur for a modeled primitive — the honest fix is removing the placeholder,
not adding a flag to describe it. Documenting the unified renderer closes the
consumer's "node-count tests gave false confidence" by setting the right
expectation.

**Alternatives rejected.** *Add a `PixelContentUnrenderedNodes` classification +
per-node diagnostics* — solves a problem D1 deletes; there is no residual unrendered
set on a raster canvas. Carrying it would be dead complexity (Principle III).

---

## D4 — `SymbolCrossCheck` FAKE target that derives from the feature dir (FR-003)

**Decision.** Add a real **`SymbolCrossCheck` FAKE target** (`./fake.sh build -t
SymbolCrossCheck`) that reads `plan.md`, `data-model.md`, and `tasks.md` **from the
resolved feature directory** (the `DependencyReport` pattern — paths from
`BuildModel`, not CLI `<files…>` args), runs the existing analyzer, and prints +
writes its markdown. `build/Governance/SymbolCrossCheck.fs` already exposes
`diff: plan -> dataModel -> tasks -> Symbol list` and
`render: Symbol list -> string`, and `render` already emits the exact
`## Symbol consistency (analyze pass G)` header — so **no new analyzer or renderer
is written**, only the target wiring. The `speckit-analyze` pass-G instruction is
updated to run `./fake.sh build -t SymbolCrossCheck` instead of "do not eyeball it"
with no invocation path.

**Wiring (minimal, avoids the "unknown gate" trap).**
- `build/Governance/Targets.fs` — add `SymbolCrossCheck` to the `Target` DU,
  `allTargets`, `name`, and `directPrerequisites`.
- `build/Governance/AgentValidation.fs` — add `"SymbolCrossCheck"` to
  `ValidationContract.knownGates` (the separate allowlist; omitting it fails
  `Governance.Tests` with an unknown-gate diagnostic — the documented 058/062 trap).
- `build/Governance/Engine/Model.fs` — add a `SymbolCrossCheckAnalyze` `BuildEffect`.
- `build/Governance/Engine/Update.fs` — `StartTarget Targets.SymbolCrossCheck` emits
  the effect + a `RequireFiles` on the readiness output.
- `build/Governance/Engine/Interpret.fs` — interpret the effect: resolve the feature
  dir, read the three artifacts, `SymbolCrossCheck.render (SymbolCrossCheck.diff …)`,
  print and write `readiness/symbol-cross-check.md`.
- `build/Governance/Front/Helpers.fs` — add the `focusedGateContract` case.
- Regenerate `validation.contract.yml` via `./fake.sh build -t
  RefreshSurfaceBaselines` so `TargetMetadataDrift` stays green.

**Rationale.** `SymbolCrossCheck` is a build-time governance analyzer over spec
artifacts, so a FAKE target (not a generated-product evidence command) is its
idiomatic home. Deriving paths from the feature dir is *better* than `<files…>`
args: FAKE targets have no per-target arg payload today, and a read-only analyze
pass wants "run it in this feature's context" with zero arguments. It is delivered
as a **command/diagnostic, not a hard merge gate** (the spec's design-only-symbol
edge case must remain human judgment, not a false-fail).

**Alternatives rejected.** (a) *An `--symbol-crosscheck <files>` evidence command in
the generated product* — evidence commands prove generated-product runtime behavior;
this is governance over spec text and belongs in `FS.Skia.UI.Build`. (b) *Env-var
file list* (`SYM_CROSSCHECK_FILES=…`) — clumsier than feature-dir derivation. (c)
*A hard gate* — false-fails on intentional design-only symbols.

---

## D5 — Evidence-format discoverability + missing-vs-required labeling (FR-004/005)

**Decision — two small, surgical changes.**
1. **`speckit-implement` skill body** (`.agents/skills/speckit-implement/SKILL.md`,
   canonical; regenerated to `.claude/**`): add a pre-implementation pointer to read
   the generated `docs/evidence-formats.md` **before** writing readiness/evidence
   files (FR-004), and enrich the skill-loading-evidence step to state it is read
   from the **feature** readiness dir `specs/<feature>/readiness/` (not repo-root),
   requires one row per (task, declared-skill) with `.agents/skills/<id>/SKILL.md`
   paths and `loaded_at < work_started_at`, and is **enforced only once tasks flip to
   `[X]`** (so it surfaces late) (FR-005).
2. **Diagnostic labeling** (`build/Governance/Evidence/Render.fs:471-480`): relabel
   the readiness-contract print so the **full required set** and the **absent
   subset** are visibly distinct — `full-required-set:` and `absent-from-file:` —
   instead of `required-tokens:` + `missing:` printing the same list when a file is
   wholly absent (the "read as 'all missing'" confusion). The single-sourcing is
   unchanged: `Required = Some terms` (full set) and `MissingTerms` (absent subset)
   already exist in `Scans.fs:95-106`; only the labels in the renderer change.

**Rationale.** `docs/evidence-formats.md` already ships into generated projects
(062 FR-005) — the residual is purely *discoverability before authoring* and
*diagnostic legibility*. Both are one-line/one-label edits, no new artifact, no new
gate. Directly improves observability (Principle VII).

**Alternatives rejected.** *Change `Scans.fs` to stop populating the full
`Required` set* — would lose the "here is the complete required shape" recovery the
062 FR-004 work deliberately added; the fix is *labeling*, not removing data.

---

## D6 — `scaffold-map.md` discoverability + `.fsi`-authoritative pointer (FR-006)

**Decision.** Add a pre-planning pointer in the `speckit-plan` skill
(`.agents/skills/speckit-plan/SKILL.md`) telling an author working on a generated
product to read `docs/scaffold-map.md` **before** designing the game model; and add
an "API surface authority" note to `template/base/docs/scaffold-map.md` stating that
the shipped `.fsi` surfaces / `docs/api-surface/` are the **authoritative** API
reference and agent-generated API summaries (e.g. Explore output) are supporting
reference only, never ground truth.

**Rationale.** 062 FR-003 already shipped the durable-vs-replaceable content into
`scaffold-map.md` (verified present in LunarLander1's `docs/`); the consumer simply
never found it and reconstructed it by hand, and separately trusted an Explore
summary that mixed confirmed APIs with inferred shapes. Both gaps are
discoverability/authority pointers — no new content, no gate.

**Alternatives rejected.** *A new skill* — heavier than a two-line pointer to an
existing page.

---

## D7 — External-URL source-spec snapshot (FR-007)

**Decision.** Extend the `speckit-specify` skill (`.agents/skills/speckit-specify/
SKILL.md`, step 3 "Create the spec feature directory") so that **when the feature
input is an external URL**, after fetching the source it is **snapshotted into the
feature directory** (e.g. `specs/<feature>/source-spec.md`, with the source URL
recorded in a header) and the spec references the in-repo snapshot rather than the
live URL. For local-file or inline input the step is an explicit no-op (no redundant
copy). Delivered as skill guidance/process — no gate.

**Rationale.** LunarLander1's source spec lived only at an external GitHub URL,
making the specify phase network-dependent and non-reproducible offline. A snapshot
captures provenance in-repo at the moment of authoring, which is cheap and matches
the repo's "source of truth lives in-repo" posture.

**Alternatives rejected.** (a) *A compiled gate that fetches/validates URLs* —
over-built; specify is an authoring phase, snapshotting is a one-line process step.
(b) *Always copy regardless of input kind* — the no-op-for-local guard avoids
fabricating redundant files (spec edge case).

---

## D8 — Evidence-path token: consumer-authoring-only, no code change (FR-008)

**Decision.** **Close FR-008 with no code change**, recording the finding: a
template-wide check confirms **no generated artifact template seeds a divergent
`evidence/` token**. `.specify/templates/spec-template.md` references neither path;
`.specify/templates/tasks-template.md` uses `readiness/` consistently;
`template/base/docs/**` seeds no `specs/<feature>/evidence/` path. The drift the
consumer saw at analyze time was a **consumer-authoring slip** they self-reconciled
to `readiness/` (the merged spec uses `readiness/`).

**Rationale.** The spec scoped FR-008 as "confirm whether a template seeds the
divergent token, and if so unify it; if purely consumer-authoring, record and
close." The investigation confirms the latter, so the honest disposition is to
record it — not invent a canonical-token mechanism for a non-existent template
defect. (If a future template is found seeding `evidence/`, it is unified then.)

**Alternatives rejected.** *Introduce a canonical evidence-path token + currency
check anyway* — solves a problem no template has; pure over-engineering.

---

## D9 — `--evidence-run` summary: document the discipline, defer the helper (FR-009)

**Decision.** **Document, defer ship.** The deterministic-summary *discipline* —
pure model + per-frame held-input script + `InvariantCulture`/`F3` float formatting
+ a `determinism=byte-identical` marker — is documented in the
`fs-skia-evidence-mode` skill with the LunarLander1 / AsteroidsDemo3 functions as
canonical examples, and the deferral is recorded with rationale and the next
recurrence bar. No reusable summary *function* is shipped this round.

**Rationale.** A four-game comparison (LunarLander1 `EvidenceCommands.fs:603-672`,
AsteroidsDemo3 `EvidenceCommands.fs:670-685`, plus Breakout/SpaceInvaders) shows the
**field set varies materially per game**: LunarLander emits physics state
(`final-position`/`final-velocity`/`final-rotation`/`final-fuel`, `F3`), Asteroids
emits entity counts (`wave`/`lives`/`asteroid-count`/`bullet-count`) and
availability probes. The only stable core is `status`/`command`/`seed`/
`frame-count`/`score`/`determinism` — too thin to be worth a shipped type that every
consumer then appends 5–10 game-specific fields to. The reusable value is the
*discipline*, not a function; shipping a forced-generic summary would obscure which
fields are mandatory. This is the honest read of "ship on recurrence" — the *loop
primitives* recur, the *summary shape* does not.

**Alternatives rejected.** (a) *Ship a generic `EvidenceRunSummary` record + writer*
— forces a lowest-common-denominator shape and a polymorphic field bag; net friction,
not less. (b) *Defer silently* — the spec forbids a silent drop; documenting +
recording the bar keeps it findable.

---

## D10 — Helper candidates: ship `wrapDeltaX`, document camera projection (FR-010)

**Decision — SHIP `wrapDeltaX`** into `FS.Skia.UI.SkillSupport` as a new
**`Wrap`** module (the only Tier-1 escalation this round): a pure, 4-line,
float-only shortest-wrap-aware delta on a toroidal axis. Matches the 062
arcade-helper family style (dependency-light `float` API, no `Scene`/`Layout`
reference). Adds curated `.fsi` and updates the per-package surface baseline
`readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` in the same
change-set (FR-011, Principle II). A skill reference is added to
`fs-skia-layout-readability` alongside the existing `reserveHudBand` note.

Proposed surface:
```fsharp
namespace FS.Skia.UI.SkillSupport
module Wrap =
    /// Shortest wrap-aware delta from `fromX` to `toX` on a toroidal axis of
    /// width `worldWidth`. Pure scalar arithmetic; result in (-worldWidth/2, worldWidth/2].
    val wrapDeltaX: worldWidth: float -> fromX: float -> toX: float -> float
```

**Decision — DOCUMENT the camera-centered projection (defer ship).** Record it as a
canonical example in `fs-skia-layout-readability` (referencing
`LunarLander1/src/LunarLander1/View.fs:60-61`) rather than shipping it.

**Rationale.** `wrapDeltaX` is pure, tiny, dependency-light, and has now recurred
across toroidal demos (Asteroids, SpaceInvaders, LunarLander) — exactly the 062
"ship on 3rd recurrence" bar. The camera projection, by contrast, is a *closure*
over per-game state (lander position, view scale, screen center), returns a
`Scene.Point` (a soft `Scene` dependency SkillSupport deliberately avoids), and
varies per game (zoom-centered here, parallax/fixed elsewhere) — it is a `View`
concern, not a shippable scalar helper.

**Alternatives rejected.** (a) *Ship the camera projection too* — pulls a `Scene`
dependency into SkillSupport and over-fits one game's camera model. (b) *Document
`wrapDeltaX` a fourth time* — it is past the recurrence bar; documenting again
repeats the friction the spec is closing.

---

## D11 — Guidance vs. gate; surface containment; low-cost checks (FR-002/003/011)

**Decision.**
- The renderer fix (D1-D3) is **behavior + tests**, not a new gate; its evidence is
  the before/after image capture (SC-001) and the now-exhaustive shared painter.
- The `SymbolCrossCheck` target (D4) is a **command/diagnostic**, not a hard merge
  gate; it is wired into `knownGates`/`validation.contract.yml` so
  `TargetMetadataDrift` stays green, but it is not added to any rule's
  `RequiredGates`.
- **Surface containment:** the shared `SceneRenderer` module is **non-public**
  (no public `.fsi` export), so the SkiaViewer per-package surface baseline does
  **not** change — only `FS.Skia.UI.SkillSupport` gains surface (D10 `Wrap`). This
  keeps the renderer refactor Tier-2 by itself; FR-010 is the lone Tier-1 trigger.
- All skill edits are made in canonical `.agents/skills/**` and regenerated
  (`RefreshSurfaceBaselines`), keeping `SkillSyncCheck` / `TargetMetadataDrift` /
  `SkillQualityCheck` green (FR-011).

**Rationale.** Matches the house posture: deliver diagnostics/guidance unless a
low-cost executable check prevents real regression, and never expand public surface
incidentally. The only deliberate surface change is the shipped `wrapDeltaX`.

---

## Resolved unknowns summary

| Spec deferral | Resolution |
|---|---|
| FR-001 how to fix the renderer | D1 — one shared exhaustive `SceneRenderer.paintNode`; delete the placeholder wildcard |
| FR-001/002 evidence-mode `Text` (glyphs vs placeholder) | D2 — real glyphs via shared `drawTextWithFallback` |
| FR-002 stop false "visible" confidence | D3 — fixed at root by exhaustive match; document the unified renderer in `fs-skia-scene` |
| FR-003 FAKE target vs evidence command; `<files>` vs derived | D4 — FAKE target deriving plan/data-model/tasks from the feature dir; command not gate |
| FR-004/005 discoverability + diagnostic clarity | D5 — `speckit-implement` pointer + skill-loading note; `Render.fs` relabels full-required vs absent |
| FR-006 mechanism | D6 — `speckit-plan` pointer + `scaffold-map.md` `.fsi`-authoritative note |
| FR-007 mechanism | D7 — snapshot URL source into the feature dir in `speckit-specify`; no-op for local input |
| FR-008 template-seeded vs consumer-authoring | D8 — consumer-authoring only; no template seeds `evidence/`; record + close, no code change |
| FR-009 ship vs document the summary pattern | D9 — document the discipline (field shapes vary per game), defer the helper with recorded bar |
| FR-010 per-helper ship/document | D10 — ship `wrapDeltaX` (SkillSupport `Wrap`, new baseline line); document camera projection |
| FR-011 surface/gate wiring | D11 — non-public `SceneRenderer`; `SymbolCrossCheck` in knownGates/contract; `.agents`→`.claude` regen |
