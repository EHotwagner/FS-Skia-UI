# Quickstart: Layout Dirty-Set Anti-Drift Guard (R7)

## What this guard does

It makes it **impossible to silently ship** a layout-input drift in the
incremental layout path. If a future change makes the layout lowering
(`toLayout`) read a new attribute name without teaching the incremental dirty
classifier (`layoutAffectingAttrNames` / `layoutDirtySet`) about it, a Controls
test fails fast and **names** the offending attribute — instead of the app
quietly reusing stale cached bounds and mis-rendering a frame.

## Run it

```sh
# Authoritative tier + gate list for the working-tree diff (run FIRST):
./fake.sh build -t Route          # expected: inner-loop -> Dev only

# Inner-loop validation (runs the new guard tests + existing R2 evidence):
./fake.sh build -t Dev

# Feature evidence obligations (FAKE-backed; run sequentially, never concurrent):
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

The new tests live in `tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs`.
The preserved R2 evidence is `tests/Layout.Tests/Feature097IncrementalTests.fs`
(≥1000-case incremental-≡-full) and the `WorkReductionRecord` assertions in
`tests/Controls.Tests/Feature097WiringTests.fs`.

## How the guard works (one paragraph)

A pure `layoutDriftReport discovered covered` returns named `DriftFinding`s
(`Uncovered` / `OverBroad`) for any disagreement between two name sets. A
**behavioral probe** discovers the *actual* layout-driving names by toggling each
candidate attribute on representative fixtures and checking whether the real
`ControlInternals.evaluateLayout` root `LayoutNode` changes. The load-bearing
test asserts `layoutDriftReport (probe()) layoutAffectingAttrNames = []`. The
negative tests feed simulated sets to prove both failure directions and the
human-legible message. A separate set of units asserts the independent
`AttrCategory.Layout` honoring on `layoutDirtySet`.

## If the guard fails

The failure message names the drift, e.g.:

```
layout dirty-set drift: un-covered layout input: 'padding'
  (toLayout reads 'padding' to derive geometry, but the incremental classifier
   does not dirty on it — it would reuse stale bounds). Fix: add 'padding' to
   ControlInternals.layoutAffectingAttrNames (or tag the attribute
   AttrCategory.Layout).
```

- **`un-covered`** → the lowering reads a name the classifier ignores. Add the
  name to `layoutAffectingAttrNames` (shares the same private token constants),
  **or** tag the attribute `AttrCategory.Layout` (the classifier honors that
  channel independently).
- **`over-broad`** → the classifier lists a name the lowering no longer reads.
  Remove it from `layoutAffectingAttrNames` (it was wasting a re-measure).

## Extending the guard

- Adding a genuinely layout-driving attribute? Update the **one** authoritative
  token + the `layoutAffectingAttrNames` set; the probe and the live classifier
  pick it up, and the gate confirms agreement.
- Adding a new control attribute to the vocabulary? It auto-enrolls in the probe
  corpus when the representative gallery emits it (research D2). If you add a
  layout input that no gallery control yet carries, add a fixture that exercises
  it so the probe can observe it.

## Scope notes

- **Zero behavior change**: rendering output and re-measure counts are identical
  to pre-R7 (FR-005/006). R7 only adds tests + a private name-token refactor +
  a corrected comment.
- **Intrinsic-size memo (FR-008): deferred** — see `research.md` D6; §10.4
  wording reconciliation is R8's job.
