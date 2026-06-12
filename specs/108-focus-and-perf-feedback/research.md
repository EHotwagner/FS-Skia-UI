# Phase 0 Research — feature 108

Each decision resolves a `NEEDS CLARIFICATION` or a technology/integration choice
raised by the spec. Format: **Decision / Rationale / Alternatives considered**.

## D1 — Focus-ring stamp identity (FR-001/002/005)

**Decision.** Add a pure `Focus.markFocused : ControlId option -> Control<'msg> ->
Control<'msg>` in `FS.Skia.UI.Controls` that walks the lowered control tree and
stamps `VisualState.Focused` on the single control whose identity equals the focused
id, where identity is `Key ?? structural-path` — the **same `Key ?? path` identity
unified in feature 098** (`ControlId = Key ?? structural path`). `None` returns the
tree byte-identical (no stamp). It preserves a consumer-set non-`Normal` state the
same way `applyRuntimeVisualState` does (Focused does not override an explicit
Disabled).

**Rationale.** The feedback's workaround (`View.markFocused`) only stamped **keyed**
controls, so arrow traversal skipped unkeyed scaffolding. Reusing 098's path identity
makes every focusable control — keyed or not — addressable (FR-002), with zero new
identity scheme. The consumer reflects their own `model.FocusedControl` into the tree
by calling `Focus.markFocused model.Focused (view …)` in their `view` (FR-005); this
is the framework-supported generalisation of the proven workaround and keeps the host
free of a consumer-focus field (architecture-preserving).

**Alternatives considered.** (a) Wire the host to read a new `Focused` field on
`InteractiveAppHost` and stamp internally — rejected: couples the host to a consumer
concept and duplicates the `applyRuntimeVisualState` path; the pure consumer-applied
stamp is simpler and testable without the host. (b) Require every focusable control
to be keyed — rejected: that is exactly the friction the feedback reported.

## D2 — Focus order drives reachability (FR-002/004)

**Decision.** Drive reachability from the existing `Focus.order : Control<'msg> ->
TabOrder`, whose `FocusStop` list already enumerates focusable controls by
`AccessibilityRole`/`KeyboardOperation` and skips structural kinds (feature 094's
structural-non-focusability rule). `markFocused` matches against the same identity
`order`/`traverse` produce, so "what traversal can reach" and "what the ring can paint"
are the **same set** by construction. Structural containers / static labels are not
in `order` and receive no ring (FR-004).

**Rationale.** Single source of truth for "is this focusable"; no second classifier to
drift. Disabled focusable controls are skipped in traversal and get no ring (spec edge
case), consistent with `order`.

**Alternatives considered.** A separate focusable-kind predicate in `markFocused` —
rejected (drift risk vs. `Focus.order`).

## D3 — `FrameMetrics` field set and timing exclusion (FR-006/007/008)

**Decision.** Public record on `FS.Skia.UI.Controls.Elmish`:
`{ RemeasuredNodeCount: int; PointerSamplesReceived: int; PointerMovesProcessed: int;
ViewRebuilt: bool; FrameDuration: TimeSpan }`. The four count/bool fields are the
**byte-stable determinism surface** (FR-007); `FrameDuration` is reported but excluded
from golden assertions. `RemeasuredNodeCount` is taken directly from the existing
`WorkReductionRecord.RemeasuredNodeCount`; `ViewRebuilt` is `true` only when the frame
called `host.View` (a model/size change), `false` for a pure pointer/hover/tick frame.

**Rationale.** Reuses metrics the retained path already computes (features 092/097);
adds only the pointer-sample counters and the rebuild flag the host loop already knows.
Excluding timing keeps the golden stable across machines while still exposing it for
real perf observation (Principle VII).

**Alternatives considered.** Including `FrameDuration` in the golden (rejected:
non-deterministic); a free-form `Map<string,int>` (rejected: not a typed contract,
weakens the surface baseline).

## D4 — Deterministic driver: pure `Perf.runScript` vs. generic `EvidenceTour` (FR-009/010)

**Decision.** Two distinct surfaces. (a) `Perf.runScript` on
`FS.Skia.UI.Controls.Elmish`: a **pure, headless** fold of an ordered `FrameInput`
script (`Key`/`Pointer`/`Tick`/`Idle`) over the host's pure update +
`RetainedRender.step`, advancing one frame per step, returning `FrameMetrics list`
(byte-stable counts). This is the framework driver US3's tests assert on — it shares
the exact coalescing + step code path with `runInteractiveApp` (no parallel logic).
(b) `SkillSupport.EvidenceTour.run : ('msg list) -> 'model -> ('msg -> 'model ->
'model) -> 'outcome`: a **generic, consumer-facing** ordered-`Msg` fold combinator
(no framework metrics, no Controls dependency) that generalises the consumer's
hand-rolled "tour", matching the plan- and implement-phase feedback. It lives in
SkillSupport beside the already-shipped seeded `Random` (splitmix64).

**Rationale.** The metrics-bearing driver must live where the metrics and the host
step live (Controls.Elmish); the generic fold is consumer-generic and dependency-free,
so it belongs in SkillSupport. Splitting them avoids forcing a `SkillSupport →
Controls` dependency (see D8) while satisfying both the FR-009 framework driver and the
"reusable combinator" assumption.

**Alternatives considered.** A single combinator in SkillSupport that also reads
framework metrics — rejected: would require `SkillSupport → Controls`, inverting the
layering. Duplicating the host step inside the driver — rejected: two code paths drift
(the whole point of US3 is to lock the *real* path).

## D5 — Pointer-move coalescing policy (FR-011/012/013)

**Decision.** In `runInteractiveApp` and `Perf.runScript`, accumulate raw pointer
samples within a frame and process **at most one move** per frame: keep only the latest
`HoverEnter`/`HoverLeave`/`DragMove` position; for a drag, retain the intermediate path
on the coalesced move so freehand consumers keep fidelity (FR-012). Discrete
interactions — `PressedDown`, `ReleasedUp`, `Click`, `DragBegin`, `DragEnd`,
`DragCancelled`, `Scroll`, secondary — are **never coalesced and never dropped**, and a
click interleaved with moves is processed within one frame of arrival (SC-006). The
coalescing accumulator is a per-frame `mutable` on the host loop (disclosed
`// mutable: hot path / per frame`), reset at each frame boundary. Event-driven tick
(`fun _ -> None`) stays the documented default; animation clocks still advance from the
injected delta even on an idle frame (FR-013).

**Rationale.** Matches the W3C/Chromium "coalesce continuous movement, dispatch once per
frame, preserve the path" precedent the feedback surveyed (P1, highest payoff) and is
host-side (a consumer's `MapPointer` returning `None` cannot stop the host's own
per-sample hit-test/repaint). The mutable accumulator is the constitution's sanctioned
hot-path mutation.

**Alternatives considered.** Coalescing discrete interactions too (rejected: drops
clicks — the exact regression we guard against); time-bucket throttling instead of
frame-aligned (rejected: not deterministic for the golden, not vsync-aligned).

## D6 — Modifier-aware key boundary (FR-016)

**Decision.** Parse modifier prefixes in `KeyboardInput`: strip leading
`Ctrl+`/`Alt+`/`Shift+`/`Meta+` (case-insensitive, any order) from `ViewerKeyEvent.RawKey`
before normalising the base key, and return a `KeyModifiers` value
(`{ Ctrl: bool; Alt: bool; Shift: bool; Meta: bool }`) alongside the normalised
`ViewerKey`. Deliver it to the consumer through an **additive `MapKeyChord : ViewerKey
-> KeyModifiers -> 'msg option`** field on `InteractiveAppHost`, consulted before the
existing shift-only `MapKey` and defaulting to "ignore modifiers / defer to `MapKey`"
so unmodified keys route byte-identically. The host's internal `shift` derivation
(today sniffing `Unknown raw` for `"Shift+"`) is replaced by the parsed
`KeyModifiers.Shift`.

**Rationale.** The modifier is already present in the raw string but `normalize`
collapses `"Ctrl+L"` to `Unknown "Ctrl+L"`, losing it; parsing at the boundary recovers
it without a backend change. An additive field keeps `MapKey` consumers working
(spec allows either "observable modifier" or "distinct chord event"; the additive seam
is the non-breaking choice). All framework construction sites add the field with an
inert default in the same change.

**Alternatives considered.** Changing `MapKey: ViewerKey -> bool` to `ViewerKey ->
KeyModifiers` (rejected: breaks every consumer signature); a backend-level modifier
event (rejected: out-of-scope backend change, and unnecessary — the prefix already
carries it).

## D7 — DataGrid tri-state sort (FR-015)

**Decision.** Make the framework `SortBy column` cycle **asc → desc → none** on the
same column (a third toggle clears to `DataGridSort option = None`), so the consumer's
third-press special-case disappears. A first `SortBy` on a *new* column starts at asc.
No separate `ClearSort` message is required, though the data-model notes it as an
equivalent explicit alternative.

**Rationale.** Removes the product-side special-case the feedback called out (SC-008)
with the smallest surface change — the existing `DataGridSort option` already models
"unsorted" as `None`; only the toggle transition changes.

**Alternatives considered.** Adding a `ClearSort` message and keeping bi-state toggle
(rejected: still forces the consumer to count presses to decide when to send it — the
tri-state cycle is what the spec wants).

## D8 — Theming-helper placement (FR-017/018) — divergence from spec wording

**Decision.** Place `Theming.resolve` (theme mode + accent → role palette) and
`Theming.toTheme` (project a role palette onto the framework `Theme`) in
**`FS.Skia.UI.Controls`** (where `Theme` is defined — `Theme.fsi`/`Theming.fsi`), and
**reuse the already-shipped `FS.Skia.UI.Color` `Contrast.ratio`** for WCAG contrast
rather than adding a second `contrastRatio`. The render-path-vs-reuse-key split is
documented (not a code helper) in `fs-skia-controls-host` / `fs-skia-design-tokens`.

**Rationale.** `toTheme` projects onto the Controls `Theme` type; putting it in
`SkillSupport` (the spec's tentative "skill-support surface" wording) would force a new
`SkillSupport → Controls` dependency, inverting the current layering (SkillSupport is
tooling: YamlDotNet/DiffPlex only). The spec states the *capability* (reusable theming
helpers + contrast) and says "ideally"/"candidate", not a mandated package. WCAG
contrast already ships as `Color.Contrast.ratio` (feature 083), so re-deriving it would
duplicate a verified surface. This is a deliberate, disclosed divergence from the
spec's suggested home, justified by package layering and DRY.

**Alternatives considered.** A `SkillSupport.Theming` module (rejected: layering
inversion); a brand-new `contrastRatio` in Controls (rejected: duplicates
`Color.Contrast.ratio`).

## D9 — Render-proof method (evidence)

**Decision.** Prove the focus ring and at-rest byte-identity with **structural-Scene
equality** over real `Control.renderTree` output (focused vs. unfocused tree), not pixel
hashes. Use `Check.One` for property-style proofs (`Control.map` structural equivalence,
tri-state cycle, coalescing burst). Interactive behaviour uses
`ControlsElmish.respondsProofOf` / `captureRespondsProof`.

**Rationale.** `SceneEvidence.renderPng`/readback are deterministic capability-hash
functions, not pixel encoders (memory `feature-091-reconciler-render-path-wiring`), so a
structural-Scene diff is the honest proof. `Control<'msg>` has no equality (function
fields) → compare `sprintf "%A"` projections (memory `feature-096`/`101`).

**Alternatives considered.** Pixel-PNG golden (rejected: non-encoding capability hash);
live-Vulkan-window requirement (rejected: spec marks it not required).

## D10 — Window-visibility readiness + checklist discoverability (FR-020, evidence)

**Decision.** Author the full window-visibility-class readiness set this feature's
`EvidenceAudit` requires (`interactive-visible-window.md`, `close-reason-separation.md`,
`window-state-diagnostics.md`, `window-options.md`, `real-image-evidence.md`,
`generated-validation.md`) with honest `key=value` tokens, and **ship the enumeration of
those files + tokens as a new discoverable checklist** (`template/base/docs/
interactive-readiness.md` and/or an `fs-skia-evidence-mode` skill section) so a consumer
satisfies them before the first audit run (FR-020). Tokens must be single-line and
single-token where the `kvRe` parser requires it (memory
`evidence-readiness-authoring-gotchas`, `window-feature-audit-evidence-084`).

**Rationale.** The feedback's #1 governance friction was discovering these only by
failing the audit; the checklist closes that. Authoring them is unavoidable for the
interactive feature regardless.

**Alternatives considered.** Pointing only at `docs/evidence-formats.md` (rejected:
that is the status quo that caused the friction).
