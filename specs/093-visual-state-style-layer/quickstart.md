# Quickstart: Declarative Visual-State & Style-Class Layer (E3)

This walks the public surface as a consumer and a maintainer would exercise it through FSI —
the honest audience (Constitution Principle I). It is the shape the `fsi-transcript.md`
readiness artifact captures.

## 1. Style a control by intent (US1)

```fsharp
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed

// A primary action — no color math, no theme branching:
let save = Button.view { Button.defaults with Text = "Save"; Classes = [ Variant StyleVariant.Primary ] }

// A destructive action — same kind, different intent:
let delete = Button.view { Button.defaults with Text = "Delete"; Classes = [ Variant StyleVariant.Danger ] }
```

`save` resolves to accent-derived paint, `delete` to danger-derived paint — both from tokens,
neither computed by the consumer. A free-form class flows through the same path:

```fsharp
let subtle = Button.view { Button.defaults with Text = "More"; Classes = [ Custom "subtle" ] }
```

## 2. Resolve directly and inspect the precedence (US2)

```fsharp
let theme = Theme.light

// Each visual state yields a distinct, token-derived ResolvedStyle:
let normal   = Style.resolve theme [ Variant StyleVariant.Primary ] VisualState.Normal
let disabled = Style.resolve theme [ Variant StyleVariant.Primary ] VisualState.Disabled
// disabled.Fill is the muted/disabled token; normal.Fill is the accent token — state wins.

// Class-vs-state precedence: state overrides the class for an overlapping field,
// the class's other fields remain (FR-003):
let pressedDanger = Style.resolve theme [ Variant StyleVariant.Danger ] VisualState.Pressed
//  Fill  = pressed-state token   (state wins over Danger)
//  Foreground/Font = Danger's    (non-overlapping class fields retained)

// Later class wins over earlier (last-writer-wins in attach order):
let layered = Style.resolve theme [ Variant StyleVariant.Primary; Custom "subtle" ] VisualState.Normal
//  Fill = "subtle" token         (later class overrides Primary)
```

## 3. Confirm purity / determinism (SC-004)

```fsharp
// Identical inputs ⇒ identical output, always:
Style.resolve theme [ Variant StyleVariant.Ghost ] VisualState.Hover
  = Style.resolve theme [ Variant StyleVariant.Ghost ] VisualState.Hover   // true
```

The `Controls.Tests` FsCheck property asserts this plus the fixed precedence over ≥1000
generated `(theme, classes, state)` combinations.

## 4. Verify the migration is behavior-preserving (US3 / SC-003)

```fsharp
// No-class, default state reproduces the prior procedural styling exactly:
let resolved = Style.resolve theme [] VisualState.Normal
// The migrated kind rendered through `resolve [] Normal` is structurally-Scene-equal
// to the captured pre-refactor procedural baseline (readiness/parity/<kind>.<theme>.<state>.scene.txt).
```

Inspection of the migrated kinds' code shows **no** per-kind inline visual-state color branch
remains — paint flows through `Style.resolve`. Unmigrated kinds render unchanged (SC-007).

## 5. Contrast stays the single authority (SC-006)

A `Custom` class whose token is contrast-insufficient against the resolved background still
resolves to a concrete value (no silent drop); the existing `ContrastCheck` gate — not the
resolver — flags it. The resolver adds no second contrast policy (FR-007).

## Validation commands (escalated path — run `Route` first)

```
./fake.sh build -t Route            # confirms controls-public-surface escalation + gate list
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
# plus DesignTokenDrift + ContrastCheck (token authority + sole contrast authority)
# surface baselines recaptured: RefreshSurfaceBaselines / PerPackageSurface.captureCurrent
```

FAKE-backed targets run **sequentially** (shared `.fake` state); non-FAKE reads may parallelize.
