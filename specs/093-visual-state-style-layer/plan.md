# Implementation Plan: Declarative Visual-State & Style-Class Layer

**Branch**: `093-visual-state-style-layer` | **Date**: 2026-06-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/093-visual-state-style-layer/spec.md`

## Summary

E3 of the controls architecture-evolution roadmap. Replace the procedural, per-kind
visual styling scattered across `module internal ControlInternals` (Control.fs) with **one
pure, total, deterministic state→style resolver** fed by `(design tokens + theme + attached
style classes + current VisualState)`, and add a **consumer-facing style-class/variant
surface** — a typed, closed semantic-variant union (primary/danger/ghost/…) plus a free-form
user-class escape hatch — carried as an ordered list on a `Control`. Resolution is a fixed,
closed, last-writer-wins fold with precedence `token/theme base < attached classes
(earlier < later) < current VisualState`; there is no selector matching, specificity algebra,
or cross-control cascade (those are permanent roadmap non-goals). A **representative set** of
controls (spanning the rich-geometry and box+label families) is migrated off procedural
styling onto the resolver and proven **byte-identical / structurally-`Scene`-equal** to the
prior procedural output for the default (no-class) case across every `VisualState`. A
catalog-wide migration of all 52 controls is explicitly out of scope.

**Technical approach**: A new `module Style` (`Style.fsi`/`Style.fs`, inserted after
`Theme.fs` and before `Attributes`/`Control` in `Controls.fsproj`) declares a `ResolvedStyle`
record and the pure `resolve` fold. New public types land on `Types.fsi` (the typed
`StyleVariant` union, the `StyleClass = Variant | Custom` carrier, and an `AttrValue` case
carried under the existing `AttrCategory.Style` that attaches an ordered class list to a `Control`). The
migrated controls' paint in `ControlInternals` is refactored to call `Style.resolve` and read
back `ResolvedStyle` fields instead of branching inline on `theme`/boolean attributes. Token
references stay sourced from the DTCG single source (`design-tokens.tokens.json` → generated
`DesignTokens`); any variant-specific token is added there and regenerated, never inlined. The
state-driven appearance attaches to E2's existing retained identity (features 067/091/092) —
E3 reads that identity and the `VisualState`/animation clock `ControlRuntime` /
`RetainedRender.StateByIdentity` already track; it does not re-derive them. The typed front
door (`FS.Skia.UI.Controls.Typed`) gains an attach-class affordance on the migrated controls'
`Props`.

This is a **Tier 1** change: it moves public surface (new types on `Types.fsi`, a new public
`Style.fsi`, typed-`Props` deltas), so controls-public-surface + per-package + cross-package
surface baselines MUST be recaptured.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: existing `FS.Skia.UI.Controls` deps only (SkiaSharp 4 preview via
Scene; Layout/Yoga via `ControlInternals.layoutNode`). No new package dependency.
**Testing**: Expecto + FsCheck (property tests for purity/determinism/precedence over ≥1000
generated combinations, SC-004), deterministic structural-`Scene`-equality parity tests
(SC-003), FSI transcript exercising the public resolver + attach-class front door, FAKE
targets per `Route`.
**Target Platform**: Windows and Linux (deterministic render-only evidence; no live window
required — parity is structural `Scene` / resolved-style equality, not pixel PNGs).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: N/A to `.template.config/template.json` content — no new capability,
  sample, command, or package-policy surface ships into the `dotnet new fs-skia-ui` template.
  The template's package **pins** are refreshed on merge per the standard version-bump flow
  (all packable libraries bumped, `FsSkiaUiVersion` pin updated), not by this plan's edits.
- **Dependency impact**: N/A — no new dependency. `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` coverage are unchanged; the resolver and
  style surface use only existing Scene/Layout/Controls types.
- **Command-surface impact**: No new gate or build target. `build.fsx`/`Routing.fs` are
  unchanged — the change routes through the **existing** controls-public-surface escalation
  rule (a public `src/Controls/*.fsi` edit). FAKE-backed targets run sequentially in the
  deterministic escalated order: `./fake.sh build -t Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` →
  `EvidenceAudit`, plus `DesignTokenDrift` and `ContrastCheck` (token authority + sole
  contrast authority). Surface baselines recaptured via `RefreshSurfaceBaselines` and
  `PerPackageSurface.captureCurrent`.
- **Generated project impact**: None. No change to default/minimal generated contents,
  selected Controls guidance, local skills, validation logs, or generated `Dev` behavior. The
  styling layer is additive: a generated project that attaches no class renders identically.
- **Evidence paths**: All under `specs/093-visual-state-style-layer/readiness/`:
  - `us1-variant-resolution.md` — a semantic variant resolves to its token-derived style and
    two variants on one kind differ token-appropriately (US1 / SC-001).
  - `us2-visualstate-and-precedence.md` — each `VisualState` resolves to a distinct
    token-derived style; the fixed class-vs-state precedence holds (US2 / SC-002).
  - `us3-parity-baseline.md` + `parity/<kind>.<theme>.<state>.scene.txt` captured baselines —
    migrated kinds' resolver output is structurally-`Scene`-equal to the prior procedural
    output for the (kind, theme, state, no-class) inputs, and no per-kind color branch remains
    (US3 / SC-003).
  - `sc004-determinism-property.md` — purity/determinism + precedence property results over
    ≥1000 generated combinations (SC-004).
  - `sc005-retained-identity.md` — state-driven look survives a sibling-shifting re-render via
    the live retained path, not a hand-seeded map (SC-005).
  - `sc006-contrast-authority.md` — `ContrastCheck` is the sole contrast authority; a
    deliberately insufficient class is flagged by the gate, not silently dropped (SC-006).
  - `sc007-unmigrated-unchanged.md` — unmigrated kinds show no render-output delta (SC-007).
  - `fsi-transcript.md` — FSI exercise of the public resolver + attach-class front door
    through the packed library surface (Principle I).
  - `surface-baselines.md` — recaptured controls-public-surface / per-package / cross-package
    baseline diffs.
- **`.fsi` / contract impact**: **Tier 1, surface moves.** New public types on
  `src/Controls/Types.fsi` (`StyleVariant`, `StyleClass`, the `AttrValue` carrier case); new
  public `src/Controls/Style.fsi` (`ResolvedStyle`, `Style.resolve`, the attach-class
  `Attr` builder, defaults); `src/Controls/Attributes.fsi` gains the `styleClasses` builder;
  the migrated controls' typed `Props` (`Widgets/Buttons.fsi`, plus the `Widgets/Primitives.fsi` `CheckBox` rich-family
  widget `.fsi`) gain an attach-class affordance. `DesignTokens.fsi` changes **only** if a
  variant needs a new token (additive). `Theme` and the existing `DesignTokens` value surface
  stay value-identical for the migrated default case. controls-public-surface + per-package +
  cross-package baselines recaptured. Compatibility: purely additive to consumers — the
  `view : 'model -> Control<'msg>` contract is unchanged; a consumer attaching no class sees
  no behavior change.
- **MVU/effect boundary**: N/A — style resolution is a **pure, total function** of
  `(tokens + theme + classes + state)`; it introduces no `Model`/`Msg`/`Effect`/`Cmd`,
  no interpreter, and no I/O. It **reads** the `VisualState` that `ControlRuntime` already
  tracks and the retained identity / animation clock `RetainedRender.StateByIdentity` already
  owns (features 067/091/092); it does not own, mutate, or re-derive any of that state. No new
  Elmish ceremony is warranted (Principle IV — pure function, not a stateful/I/O workflow).
- **Synthetic evidence**: None planned. All parity, determinism, and precedence evidence is
  real — structural `Scene` / `ResolvedStyle` equality from the actual resolver and the actual
  procedural baseline, and FsCheck-generated real inputs. SC-005 is proven through the **live**
  retained render path, not a hand-seeded `StateByIdentity` map (the 092 gap this explicitly
  avoids repeating). No mocks/stubs/fakes are anticipated; if any `[S]` appears it triggers the
  full Principle V disclosure regime.
- **Test evidence**: Failing-first semantic tests in `Controls.Tests`: (1) a parity test
  asserting structural-`Scene` equality vs a captured procedural baseline for each migrated
  (kind, theme, state) — fails before the resolver matches the baseline; (2) an FsCheck
  property asserting purity/determinism + fixed precedence over ≥1000 inputs; (3) a
  precedence test asserting state-over-class and later-class-over-earlier last-writer-wins; (4)
  a variant-distinctness test (two variants differ token-appropriately); (5) an
  unmigrated-unchanged regression test. Governance: surface-baseline tests
  (controls-public-surface / per-package / cross-package), `DesignTokenDrift`, `ContrastCheck`.
- **Observability**: The resolver is pure with no failure mode of its own — total over the
  closed `StyleVariant` set and the eight `VisualState` cases (no partial match, no exception
  path). A `Custom` class that names no known token resolves deterministically to the base
  value (documented, not silent — covered by the edge-case test and surfaced via the existing
  contrast diagnostic when contrast-insufficient). No new structured-log surface; the existing
  `ControlDiagnostic` / `ContrastCheck` channels remain authoritative.
- **Deferred scope**: Out of scope and bounded as follow-ups — catalog-wide migration of all
  52 controls off procedural styling (only a representative set here); CSS-selector matching,
  specificity/cascade, attached/dependency properties (permanent non-goals); lookless
  `ControlTemplate`/slot composition (E5, demand-driven); focus/keyboard traversal delivery
  (E4); a theme-switching UI; a live windowed pixel-PNG capture path; data
  binding/observables (permanent non-goal).

**Initial Constitution Check: PASS** — Tier 1 with `.fsi` updates + baseline recapture
planned; no MVU boundary required (pure function); no synthetic evidence; idiomatic-simplicity
respected (the resolver is a plain fold over records and a closed union — no SRTP, reflection,
type providers, custom operators, or non-trivial computation expressions).

## Project Structure

```
src/Controls/
  Types.fsi / Types.fs           # + StyleVariant union, StyleClass carrier, AttrValue case (Tier 1)
  DesignTokens.fsi / .fs         # token VALUES (generated); +token only if a variant needs one
  design-tokens.tokens.json      # DTCG single source — edited only if a variant adds a token
  Theme.fsi / Theme.fs           # unchanged (value-identical)
  Style.fsi / Style.fs           # NEW — ResolvedStyle record + pure `resolve` fold (Tier 1)
  Attributes.fsi / Attributes.fs # + `styleClasses` attribute builder
  Control.fsi / Control.fs       # ControlInternals: migrated kinds route paint through Style.resolve
  Widgets/Buttons.fsi / .fs      # typed Props gains attach-class affordance (box+label migrant)
  Widgets/Primitives.fsi / .fs   # CheckBox typed Props gains attach-class affordance (rich-geometry migrant)

src/Controls/Controls.fsproj     # insert Style.fsi/Style.fs after Theme.fs, before Attributes.fsi

test/Controls.Tests/             # parity, property (FsCheck), precedence, variant, regression tests

specs/093-visual-state-style-layer/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/style-resolver.md    # the resolver + style-class public contract
  contracts/attach-class-surface.md
  checklists/requirements.md
  readiness/                      # evidence artifacts (paths above)
```

**Insertion point**: `Style.fsi`/`Style.fs` go after `Theme.fs` (line 42 of `Controls.fsproj`)
and before `Attributes.fsi` (line 47) so the resolver is in scope for `Attributes`/`Control`
and depends only on already-compiled `Types`, `DesignTokens`, and `Theme`.

## Complexity Tracking

No constitution deviations requiring justification. The resolver is a plain
record-producing fold over a closed union; no Principle III escape (SRTP, reflection, type
providers, custom operators, non-trivial CEs, multi-case active patterns) is used.
