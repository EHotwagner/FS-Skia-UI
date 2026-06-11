# Contract: Currency claims per skill (claim → verified source anchor)

This is the authoritative checklist of claims each refreshed/new skill **MUST** make, each bound
to a `.fsi` anchor verified present on `main` at authoring time (FR-009). Implementation asserts
each "Must state" item appears, and that no "Must NOT state" item survives. Anchors are read-only
grounding — **none of these files is edited** (FR-008).

---

## C1 — `fs-skia-reconciliation` (US1 / FR-001, FR-002)

**Must state (added/updated):**

| # | Claim | Anchor |
|---|---|---|
| 1 | The wired path's status is current **through feature 103**, not frozen at 091. | `RetainedRender.fsi:1-7,40-46` |
| 2 | `RetainedRender.step` threads a previous-frame `LayoutResult` **bounds cache**; unchanged subtrees reuse bounds without re-measuring. | `RetainedRender.fsi:81-85` |
| 3 | `WorkReductionRecord.RemeasuredNodeCount` reports the post-propagation re-measure dirty set (097/101). | `RetainedRender.fsi:102-106` |
| 4 | A per-identity `AnimationClock {Anim;Elapsed;Target;From}` is advanced by an **injected** host delta (no wall-clock) and **sampled on paint**; settled/absent ⇒ byte-identical at rest. | `RetainedRender.fsi:55-59,67-69,134-143` |
| 5 | The paint cross-fade (103) is a **two-snapshot composite**: prior `From` fading out under the next own-scene fading in (`sampleOnPaint`), not a `Color` tween. | `RetainedRender.fsi:44-54,155-163` |
| 6 | Runtime visual state is **stamped pre-reconcile** by `ControlRuntime.applyRuntimeVisualState` (R1/096); `updateClockForState` decides start/retarget/advance/drop. | `RetainedRender.fsi:146-153`, `ControlRuntime.fsi:95` |

**Must NOT state (removed):**

- Any phrasing that frames E3 style / E4 focus / virtualization or features 096–103 as **future**
  "further work [that] builds atop the wired path" — replace with shipped-truth (those landed as
  093/094 and 096–103). (Current stale text: `fs-skia-reconciliation/SKILL.md:33-35`.)

**Invariants preserved:** the 067 diff contract, operation set, and totality/determinism/
identity-at-rest/round-trip invariants are unchanged and stay in the skill; `Reconcile` and
`RetainedRender` remain `module internal` (zero public-surface delta).

---

## C2 — `src/Controls/skill/SKILL.md` E3 + E4 (US2 / FR-003, FR-004)

**E3 — visual state. Must state:**

| # | Claim | Anchor |
|---|---|---|
| 1 | Runtime visual state has a public entry point: `deriveVisualState model controlId : VisualState` (096) — the closed precedence tail the resolver consumes. | `ControlRuntime.fsi:88` |
| 2 | `applyRuntimeVisualState` (internal) stamps the derived state onto the tree pre-reconcile; consumers read state via `deriveVisualState`. | `ControlRuntime.fsi:95` |

**E4 — focus / traversal. Must state:**

| # | Claim | Anchor |
|---|---|---|
| 3 | `Focus.route` takes `role`, `keyboard`, **`navRange`**, `key`, `isTab`, `shift` and returns `KeyRouting`. | `Focus.fsi:83-90` |
| 4 | A focused navigation key is classified into a closed **`NavIntent`** = `ValueStep of delta` \| `SelectionMove of Direction` \| `GridMove of rowDelta*colDelta`, carried by `KeyRouting.Navigate`. | `Focus.fsi:41-44,50-53` |

**Must NOT state (corrected):**

- E4 describing `Focus.route` as only "classifies a delivered key against the focused control"
  with the pre-100 two-line example. (Current stale text: prose `src/Controls/skill/SKILL.md:124-127`,
  example `:129-132`; the `### E4` heading at `:122` is preserved.)
- Any E3/E4 code example referencing a signature that no longer exists.

**Constraint:** edits stay within the existing E3/E4 headings; the skill must still pass all 7
rubric sections (it already does — do not remove Sources/Related/mandate/examples).

---

## C3 — `fs-skia-controls-host` (US3 / FR-005) — NEW

**Scope:** the maintainer-facing `Controls.Elmish` interactive-host seam.

**Must cover:**

| # | Claim | Anchor |
|---|---|---|
| 1 | `runInteractiveApp` is the live host entry; the host record carries `Init/Update/View/MapKey/MapPointer/Tick/Theme`. | `ControlsElmish.fsi:53,256` |
| 2 | The host holds the `RetainedRender` structure in interpreter-edge ref state and produces each frame via `RetainedRender.step` (carrying `StateByIdentity`, `Layout`, `Theme`). | `RetainedRender.fsi:71-85`, `ControlsElmish.fsi:149` |
| 3 | `host.Tick` advances each identity's `AnimationClock` by the injected delta before render; sample-on-paint composites the cross-fade. | `RetainedRender.fsi:48-49,134-143`, `ControlsElmish.fsi` Tick path |
| 4 | Visual state is assembled from pointer/focus and stamped via `applyRuntimeVisualState` pre-reconcile each frame. | `ControlRuntime.fsi:95`, `RetainedRender.fsi:146` |
| 5 | Key delivery goes through internal `routeFocusedKey` (E1 text seam → `Focus.route` activation/navigation/Tab → fallthrough to `host.MapKey`). | `ControlsElmish.fsi:197-241` |
| 6 | Pointer hit-testing resolves to a stable identity via `retainedHitTest` (per-node boxes, no unkeyed-sibling collision). | `RetainedRender.fsi:186-193` |

**Rubric:** must satisfy all 7 `SkillQualityCheck` sections (Scope, Driven-library API,
Runnable example, ≥2 research URLs, persistent-problem mandate phrase "official online docs
first", `[[related]]` links, Sources).

**Cross-links (FR-005):** `[[fs-skia-reconciliation]]` (the diff/retained structure it drives),
`[[fs-skia-viewer-host]]` (the consumer-facing counterpart), `[[fs-skia-ui-widgets]]`/Controls
skill (the controls it hosts). The reconciliation and viewer-host skills add a back-link to
`[[fs-skia-controls-host]]`.
