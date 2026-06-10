# Quickstart: Verify retained-tree reconciliation on the render path (091 / E2)

This loop proves the four user stories end to end: a control keeps its identity across an unrelated
re-render (US1), focus + a per-control animation survive it (US2), a localized change repaints only the
changed subtree (US3), and the 067 invariants hold on the wired path (US4). Everything below is capturable
headless/offscreen — **no live Vulkan window required**.

## 0. Consumer's view — unchanged (the FR-008 promise)

An existing MVU consumer writes the **same** pure view it always wrote; nothing about the contract changes.
The retained path is internal:

```fsharp
// view : 'model -> Control<'msg>  — pure, unchanged. Keyed controls get stable identity for free.
let view model =
    Column.create [
        Counter.view model.Counter                       // ticks here ...
        TextBox.create [ withKey "editor"; onChanged Typed ]   // ... must NOT reset focus over there
    ]
```

## 1. US1 — identity survives an unrelated re-render

```fsharp
open FS.Skia.UI.Controls    // RetainedRender is internal; visible to Controls.Tests via InternalsVisibleTo

let r0   = RetainedRender.init theme size (view model)
let next = view { model with Counter = model.Counter + 1 }    // change is unrelated to the keyed "editor"
let s    = RetainedRender.step theme size r0 next

// The keyed "editor" is matched (ChildKeep/Update), NOT Replaced — its RetainedId carries over.
// A control whose Kind actually changed across the two frames is Replaced (no false identity).
```

Confirm via the diff that K is `ChildKeep`/`Update` (not `Replace`) and that a `Kind`-changed control is
`Replace`d. (SC-001)

## 2. US2 — focus + animation survive (reuses the 090 responds-proof primitive)

```fsharp
// render → set focus on "editor" / start its per-control clock → dispatch an UNRELATED model update → re-render
// assert: ControlRuntime.FocusedControl still resolves to "editor"  (focus survives)
//         the "editor" animation clock advanced by Δ (did NOT reset to its start)
// a rebuild-every-frame baseline FAILS this proof.
```

Artifacts: `readiness/survives-proof/{before,after}.png` + `survives-proof.txt`. (SC-002)

## 3. US3 — localized change repaints only the changed subtree

```fsharp
// change one leaf's attribute; step the retained path; record the work reduction.
let s = RetainedRender.step theme size prev (view changedModel)
// s.WorkReduction : RecomputedNodeCount <= ChangedSubtreeBound < BaselineNodeCount (== N)
// s.Render is byte-identical to Control.renderTree theme size (view changedModel)  (golden parity)
```

Artifacts: `readiness/partial-update/work-reduction.txt`, `readiness/retained-parity/{wired,rebuild}.png` +
`retained-parity.txt`. (SC-003 / SC-004)

## 4. US4 — invariants hold on the wired path (≥1,000 cases)

```bash
dotnet test tests/Controls.Tests/Controls.Tests.fsproj   # promoted 067 round-trip/determinism/totality/
                                                          # identity-at-rest, now over RetainedRender.step
```

Plus the duplicate-key diagnostics-surfacing test: a duplicate-keyed sibling list on the live path yields a
`KeyCollision` through the existing `ControlDiagnostic` channel and the path stays total. (SC-005 / SC-006)

## 5. Disposition flip + gates

```bash
# Flip the single source of truth, then regenerate the .claude mirror:
#   .agents/skills/fs-skia-reconciliation/SKILL.md  Disposition: "parked, not wired" -> "wired on the render path"
./fake.sh build -t RefreshSurfaceBaselines       # regenerates .claude mirror (SkillSyncCheck)

# Route prints the authoritative tier + gate list for THIS diff; run only what it prints.
./fake.sh build -t Route
# Expected: escalation to maintainer-verify. Then the serialized six-target order (run sequentially):
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

`readiness/skill-sync-check.md` byte identity confirms the disposition flip currency (SC-007); every printed
gate green confirms SC-008. An existing MVU consumer needed **zero** code changes to get all of the above.
