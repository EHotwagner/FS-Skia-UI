# Phase 1 Data Model: Layout Dirty-Set Anti-Drift Guard (R7)

R7 introduces no domain entities, no persisted state, and no public types. The
"data model" here is the small set of **test-local / private** values the
enforcement uses. None appears in any `.fsi`.

## DriftFinding (test-local DU)

The result of comparing the discovered layout-driving names against the
classifier's covered set.

```fsharp
/// A single way the classifier's covered name set disagrees with what the
/// layout lowering actually reads. Lives in the test file (no public surface).
type DriftFinding =
    /// A name the lowering reads (drives `LayoutNode`) that the classifier does
    /// NOT cover — under-coverage; risks reusing STALE cached bounds (FR-002).
    | Uncovered of name: string
    /// A name in the classifier's set that the lowering does NOT read —
    /// over-coverage; wastes a re-measure (FR-003).
    | OverBroad of name: string
```

**Validation / invariants**:
- `layoutDriftReport discovered covered = []` ⟺ `discovered = covered` (the
  two-set exact equality of FR-002+FR-003).
- Findings are deterministic and order-stable (sort by name) so failure messages
  are reproducible.

## layoutDriftReport (pure function under test)

```fsharp
/// Exact set-difference, both directions, as named findings. Pure; total.
val layoutDriftReport: discovered: Set<string> -> covered: Set<string> -> DriftFinding list
//   = [ for n in discovered - covered -> Uncovered n ]      // sorted
//   @ [ for n in covered - discovered -> OverBroad n ]      // sorted
```

```fsharp
/// Human-legible, names each attribute + direction (FR-007). Empty -> "no drift".
val formatDrift: DriftFinding list -> string
//   e.g. "layout dirty-set drift: un-covered layout input: 'padding'
//         (toLayout reads it but the classifier does not dirty on it)"
```

## The probe seam (test-local)

```fsharp
/// A representative control used to observe whether toggling an attribute name
/// changes the lowering. At least: one plain container (orientation-sensitive
/// kind) with a child, and one leaf.
type ProbeFixture = { Label: string; Control: Control<unit> }

/// Distinct attribute names to test (research D2 corpus): the attribute-name
/// vocabulary the controls layer emits (`Attr` builders + attribute-name
/// literals in `src/Controls/Control.fs`) UNION
/// `ControlInternals.layoutAffectingAttrNames` UNION explicit non-layout names
/// (`background`/`foreground`/`text`/a visual-state class).
val probeCorpus: string list

/// True iff attaching an attribute named `name` to `fixture` changes the root
/// `LayoutNode` produced by the REAL `ControlInternals.evaluateLayout`.
val nameDrivesLayout: size: Scene.Size -> fixture: ProbeFixture -> name: string -> bool
//   let withAttr    = { fixture.Control with Attributes = probeAttr name :: fixture.Control.Attributes }
//   let a, _, _ = ControlInternals.evaluateLayout size fixture.Control
//   let b, _, _ = ControlInternals.evaluateLayout size withAttr
//   a <> b        // structural LayoutNode inequality

/// Union over (corpus x fixtures) of names that drive layout — the discovered
/// truth the gate pins `layoutAffectingAttrNames` to.
val discoverLayoutDrivingNames: size: Scene.Size -> Set<string>
```

**Notes**:
- `probeAttr name` attaches a value that is *distinguishable* for geometry names
  (e.g. an explicit `width`/`height` value differing from the fixture default,
  and an `orientation = "horizontal"` on a column-default container) and inert
  for non-geometry names. The probe only needs the *presence* of the name to
  flip the `LayoutNode` for a real layout input.
- For a name the corpus includes that no fixture makes observable, the probe
  reports it as non-driving — acceptable: such a name is genuinely not a
  current layout input, and the documented corpus/fixture discipline (research
  D2) keeps the representative set adequate.

## Shared name-token constants (private, in `src/Controls/Control.fs`)

```fsharp
// US2 single-sourcing — one authoritative token per name; NOT in Control.fsi.
let [<Literal>] private AttrWidth = "width"
let [<Literal>] private AttrHeight = "height"
let [<Literal>] private AttrOrientation = "orientation"
// nodeWidth/nodeHeight/orientationOf and layoutAffectingAttrNames all reference
// these; no string literal of a layout-driving name is duplicated.
```

These change **no** behavior and **no** surface; they only collapse the three
duplicated string literals to one token each.

## What is explicitly NOT modeled

- No `Model`/`Msg`/`Effect` — R7 is not stateful/IO (no Elmish boundary).
- No new public/internal type, member, or `.fsi` entry.
- No intrinsic-size memo type (FR-008 deferred — see research D6).
