# Quickstart: General Navigation-Key Delivery (R5 / feature 100)

What a consumer sees after R5, and how a maintainer validates it.

## Consumer view (no API change)

A consumer authors a focusable selection control the documented way — items + a selection
binding, **no custom key handler**:

```fsharp
RadioGroup.create
    [ RadioGroup.items [ "Small"; "Medium"; "Large" ]
      RadioGroup.selected model.Size
      RadioGroup.onChanged (fun key -> SetSize key) ]
```

**Before R5:** focus the radio-group, press ↓/↑ → nothing happens (the Navigate arm only
matched sliders and dispatched a hardcoded `0.1` float).

**After R5:** focus it (Tab or click), press ↓ → selection moves to the next item, the
`onChanged`/`onSelected` binding fires with the moved item, and (via R1) the newly selected
item shows its `Selected`/`Focused` visual. Sliders now step by their **declared** step, not
a hardcoded `0.1`. A focused button (no navigation metadata) is a no-op on arrows but still
activates on Space/Enter.

## The intent model (mental model)

```
focused key ──▶ Focus.route(role, keyboard, navRange, key)
                     │
                     ├─ Navigate (ValueStep delta)        range roles  → step value, clamp [min,max]
                     ├─ Navigate (SelectionMove dir)      selection    → move index, dispatch "selected"/"changed"
                     ├─ Navigate (GridMove (dr,dc))       grid         → 2-D move, clamp to grid
                     ├─ Activate                          unchanged (Space/Enter)
                     ├─ Traverse move                     unchanged (Tab)
                     └─ Fallthrough                       no-op
```

One role maps to exactly one intent class. Adding a role = classifying it into the existing
closed set, never opening a new key-handler surface.

## Maintainer validation

Run `Route` first and run only the gates it prints (this change edits public `.fsi` in
`Focus`/`Types`/`Accessibility`, so it escalates to the controls-public-surface route):

```sh
./fake.sh build -t Route            # authoritative tier + minimal gate list for the diff
./fake.sh build -t Route --enforce  # additionally fails on a missing required evidence artifact
```

Then the escalated serialized order (run FAKE targets sequentially):

```sh
./fake.sh build -t Dev                     # nav unit/property/integration tests
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit           # merge gate — must PASS, no [S]/[S*], no diff-scan hits
```

## Evidence to expect under `specs/100-general-navigation-keys/evidence/`

- **responds-vs-renders** — arrow → selection-move on a focused radio-group/tab on the live
  host (a pre-R5 build dispatches nothing and fails it).
- **declared-step** — a non-default-step slider steps by its declared step; a default-step
  slider stays byte-identical (non-regressive numeric golden).
- **role-coverage** — value (slider) + linear selection (radio-group/tab) + grid, each
  validated by `Accessibility.validate`.
- **closed-model** — property/exhaustiveness proof that `NavIntent` and `NavPayload` are
  closed, totally-matched sets.

## Key files

| Concern | File |
|---------|------|
| `NavIntent` / `Direction` / `route` | `src/Controls/Focus.fsi` + `.fs` |
| `NavRange` / `NavPayload` / `ControlEvent` / `AccessibilityMetadata` | `src/Controls/Types.fsi` + `.fs` |
| per-role keyboard + range metadata | `src/Controls/Accessibility.fs` |
| host `Navigate` arm → per-intent resolver | `src/Controls.Elmish/ControlsElmish.fs:455-478` |
| pure route tests | `tests/Controls.Tests/Feature100*` |
| host routing tests | `tests/Elmish.Tests/Feature100*` |
