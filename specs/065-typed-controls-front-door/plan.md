# Implementation Plan: Typed Controls Front Door

**Branch**: `065-typed-controls-front-door` | **Date**: 2026-06-05 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/065-typed-controls-front-door/spec.md`

## Summary

Add an additive, compiler-checked authoring surface to `FS.Skia.UI.Controls`: a
sealed public `Widget<'msg>` wrapper plus, for a six-control reference slice
(`TextBlock`, `Button`, `CheckBox`, `TextBox`, `Stack`, `DataGrid`), a per-control
immutable typed `Props` record with `defaults` and `view` (and `init`/`update`
for the two stateful controls). Every typed `view` lowers to a `Control<'msg>`
that is **structurally equal** to the legacy `Control.create`/`Attr` output it
replaces, proven by a per-control parity test. The legacy string-keyed API is
frozen and untouched, so the public surface change is additive-only. No new
dependency (in particular no `Fable.Elmish`) is added. The change is confined to
`src/Controls/**`, which the `controls-public-surface` routing rule already
escalates, and whose required evidence artifacts the rule already names.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (matches `Controls.fsproj`)
**Primary Dependencies**: existing only — `Scene`, `Layout`, `KeyboardInput`. **No new dependency** (FR-011); explicitly not `Fable.Elmish`.
**Testing**: Expecto (`tests/Controls.Tests/`, `tests/Elmish.Tests/`), FSI transcripts, FAKE escalated six-target order, `Route`-printed gates
**Target Platform**: Windows and Linux (library; no platform narrowing)
**Change Tier**: Tier 1 (contracted change — adds public `.fsi` surface)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Initial evaluation: PASS.** The feature is additive-only, reuses the existing
IR and stateful models, adds no dependency, and introduces no new stateful
workflow or I/O. The two stateful typed controls satisfy Principle IV by
delegating to the existing pure `TextInput`/`DataGrid` `init`/`update`. Lowering
is real and parity-tested, so Principle V (synthetic disclosure) is not engaged.

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json` change. This
  feature adds source files and tests under `src/Controls/**` and
  `tests/**` only; it ships no new Spec Kit asset, sample template, package
  policy, or command-surface change that the generated-project template must
  mirror. (The `samples/ControlsGallery` panel extended in T019 is a repo sample,
  not a template fragment.)
- **Dependency impact**: N/A — no dependency added (FR-011). `Directory.Packages.props`,
  `docs/dependencies.md`, generated template inclusion, and `DependencyReport`
  are unchanged. A dependency-governance guard test (T014) asserts
  `Controls.fsproj` still references no `Fable.Elmish` (SC-004).
- **Command-surface impact**: No `build.fsx`/`Routing.fs`/wrapper change. The
  path `src/Controls/**` already matches the `controls-public-surface` rule, so
  no routing edit is required. Validation runs the `Route`-printed gates
  (`ControlsCatalogCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`,
  `PackageSurfaceCheck`, `FsiTranscripts`, `GeneratedProductCheck`) plus the
  escalated serialized six-target order. FAKE-backed targets run **sequentially**
  in the deterministic order below (never concurrently — shared `.fake` state):
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A — no change to default/minimal generated
  contents, selected Controls guidance, local skills, validation logs, or
  generated `Dev` behavior. `GeneratedProductCheck` is run (it is a printed gate)
  and is expected to pass unchanged because the typed surface is additive and the
  generated product does not consume it in this feature.
- **Evidence paths**: All under `specs/065-typed-controls-front-door/readiness/`.
  Routing-required (Route --enforce): `typed-controls-front-door.md`,
  `package-surface-expectations.md`. Supporting: `typed-lowering-parity.md`
  (six-control parity matrix), `controls-rendering.md` (typed-panel viewport
  render evidence). FSI transcripts land under the same `readiness/` tree per the
  `FsiTranscripts` gate.
- **`.fsi` / contract impact**: Yes — Tier 1. New public `.fsi`: `Widget.fsi`
  (sealed `Widget<'msg>` + `Widget` module) and the typed control modules under a
  distinct `FS.Skia.UI.Controls.Typed` namespace (`Widgets/Primitives.fsi`,
  `Widgets/TextBoxWidget.fsi`, `Widgets/DataGridWidget.fsi`). Existing `.fsi`
  files are unchanged (additive-only). The package public-surface baseline is
  regenerated and reviewed in the diff (T020); `PackageSurfaceCheck` gates it.
  Compatibility note: legacy API frozen, migration bridge is `Widget.ofControl`.
- **MVU/effect boundary**: `TextBox` reuses `TextInputModel`/`TextInputMsg`/
  `TextInputEffect`; `DataGrid` reuses `DataGridModel`/`DataGridMsg`/
  `DataGridEffect` (FR-006). Typed `init`/`update` **delegate** to the existing
  pure `TextInput.update`/`DataGrid.update` — no parallel state types, no I/O in
  `update`. Effect equality vs. the existing controls is asserted (T011); the
  edge interpreter (`TextInput.interpretEffect`) is unchanged and reused.
- **Synthetic evidence**: None. Lowering is real and verified by structural
  parity tests against the legacy builders (FR-004, SC-002). No `[S]`/`[S*]`
  disclosure is required; the evidence file states this explicitly. If any task
  is forced to ship placeholder lowering, it must carry `[S]` per Principle V —
  not expected.
- **Test evidence**: Failing-first contract tests (T003) committed red before
  production code. Per-control structural-parity tests (T007/T016), interaction/MVU
  delegation tests (T011), accessibility + rendering tests at ≥2 viewports (T017),
  dependency-governance guard, and an Elmish-boundary test proving a
  `Widget.toControl`-terminated `view` runs through `AdapterProgram` unchanged.
- **Observability**: No new diagnostics path. Typed views lower to the same IR,
  so existing `Control.diagnostics` and `ControlDiagnostic` reporting are reused
  byte-for-byte. No new actionable-failure or unsupported-environment message is
  introduced.
- **Deferred scope**: Out of scope and sequenced as later features — design
  tokens / Penpot (069), typed catalog generation (066), keyed reconciliation
  (067), `Controls.Elmish` command-model convergence (068), migrating the other
  41 controls (070), legacy-API deprecation (later decision, Q1). No release
  validation, repo split, or distribution automation in this feature.

## Project Structure

New compile units land in `src/Controls/` so they ship in `FS.Skia.UI.Controls`
(no project moves). `<Compile>` order in `Controls.fsproj` (order is significant
in F#):

```
src/Controls/
  Control.fsi / Control.fs            (existing)
  Widget.fsi / Widget.fs              <- NEW, inserted after Control
  ... Catalog, TextInput, ControlRuntime, Collections, Charts, RichText, DataGrid (existing) ...
  Widgets/Primitives.fsi / .fs        <- NEW (TextBlock, Button/ButtonIntent, CheckBox, Stack/StackOrientation)
  Widgets/TextBoxWidget.fsi / .fs     <- NEW (typed TextBox façade over TextInput)
  Widgets/DataGridWidget.fsi / .fs    <- NEW (typed DataGrid façade over DataGrid model)

tests/Controls.Tests/
  TypedControlContractTests.fs        (extend: assert Widget + six typed modules)
  TypedLoweringTests.fs               <- NEW (structural parity, the keystone test)
  InteractionTests.fs                 (extend: typed OnClick/OnChanged + MVU delegation)
  RenderingTests.fs, AccessibilityTests.fs, PublicSurfaceTests.fs (extend)
tests/Elmish.Tests/                   (extend: Widget.toControl through AdapterProgram; dependency guard)

samples/ControlsGallery/Program.fs    (extend: typed-authoring panel)

specs/065-typed-controls-front-door/
  readiness/typed-controls-front-door.md          (routing-required)
  readiness/package-surface-expectations.md       (routing-required)
  readiness/typed-lowering-parity.md              (supporting)
  readiness/controls-rendering.md                 (supporting)
```

**Namespace decision** (resolves spec Assumption / report Q2): typed modules live
under `FS.Skia.UI.Controls.Typed` so the six typed modules keep clean names
(`Button`, `TextBox`, `Stack`, …) without shadowing the legacy `module Button`/
`module TextBox` in `Control.fsi`. `Widget`/`Widget<'msg>` live in the base
`FS.Skia.UI.Controls` namespace (the lowering seam used by render + adapter).

## Phase 0: Research

See [research.md](./research.md). All five open decisions from the source report
(Q1–Q5) are resolved there with rationale and were baked into the spec's
Assumptions; no `NEEDS CLARIFICATION` remains.

## Phase 1: Design & Contracts

- Data model: [data-model.md](./data-model.md) — `Widget<'msg>`, the six `Props`
  records + variant enums, and the reused stateful models.
- Contracts: [contracts/](./contracts/) — the new `.fsi` surface sketches
  (`Widget.fsi`, `Typed.Primitives.fsi`, `Typed.TextBoxWidget.fsi`,
  `Typed.DataGridWidget.fsi`) and the lowering-parity contract.
- Quickstart: [quickstart.md](./quickstart.md) — author the six controls through
  the typed surface and finish with `Widget.toControl`.

**Post-design Constitution re-check: PASS.** The contracts keep every `Props`
field strongly typed (no `obj`, no string-named events — FR-005), expose only the
sealed `Widget` type + module functions on the public surface (Principle II, the
internal `{ Lowered }` record stays in `.fs`), and route stateful `init`/`update`
through the existing pure models (Principle IV). No new violation surfaced.

## Phase 2 (planning complete)

Tasks are deferred to `/speckit-tasks`; the dependency-ordered breakdown (which
mirrors §9 of the source report) is realized in [tasks.md](./tasks.md) as:
failing contract tests (T003) → the `Widget` seam (T004 `.fsi` / T005 `.fs`) →
the six controls across their US phases (T007–T013) → the keystone parity matrix
(T016) → a11y/render (T017) → Elmish-boundary + gallery (T018/T019) → public
surface baseline (T020) → the `Route` escalation and gate run (T021–T024).
