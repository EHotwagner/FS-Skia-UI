# Phase 0 Research: Accessible Color Contrast & Palettes

All NEEDS CLARIFICATION from Technical Context are resolved below. The spec's
Clarifications session (2026-06-08) already settled the three open product
questions (fill-color level, role/threshold model, non-solid-paint handling);
this file records the *technical* decisions for implementation.

## R1 — WCAG 2.x relative luminance & contrast ratio

- **Decision**: Implement the canonical WCAG 2.x formulas directly over the
  Scene `Color` byte channels. For each channel `c ∈ {R,G,B}`: `cs = c/255`;
  linearize `cl = cs/12.92` if `cs ≤ 0.03928`, else `cl = ((cs+0.055)/1.055)^2.4`.
  Relative luminance `L = 0.2126·Rl + 0.7152·Gl + 0.0722·Bl`. Contrast ratio
  `(Llighter + 0.05) / (Ldarker + 0.05)`, range 1:1 … 21:1.
- **Rationale**: These are the exact constants the spec names (FR-001/FR-002)
  and the reference the user selected; reproducing them yields the published
  reference values (black-on-white 21:1, white-on-white 1:1) within SC-002's
  0.01 tolerance with `float` (double) arithmetic. No library needed — pure
  managed math keeps `FS.Skia.UI.Color` dependency-light (Scene only).
- **Alternatives considered**: SkiaSharp color helpers (rejected — pulls a native
  dependency into a pure-math package and adds nothing); a third-party WCAG NuGet
  (rejected — minimize dependencies per the constitution; the formula is ~15
  lines). `float32` (rejected — `float` is plainer and comfortably inside
  tolerance).

## R2 — Element-role threshold model

- **Decision**: Three roles. `Text` → AAA ≥ 7:1, AA ≥ 4.5:1, AA-Large ≥ 3:1
  (large = ≥18pt or ≥14pt bold; pairings treated as body unless designated
  large). `GraphicOrUi` → ≥ 3:1 (WCAG 1.4.11; meaningful symbols, icons, vector
  shapes, strokes, control outlines, focus rings). `Decorative` → exempt (no
  threshold; recorded, never enforced). The verdict function takes the role and
  applies the matching threshold.
- **Rationale**: Directly encodes the clarified role taxonomy (FR-003). Modeling
  `Decorative` as a first-class role (rather than omission) lets the gate
  *record* decorative pairings without ever failing on them (FR-009 edge), which
  is the evidence-honesty stance.
- **Alternatives considered**: Two roles (text + non-text) — rejected because the
  spec explicitly wants decorative recorded-but-exempt and large-text handled.

## R3 — Verdict taxonomy & Indeterminate

- **Decision**: `Verdict = Aaa | Aa | AaLarge | Fail | Indeterminate`.
  `Indeterminate` is returned for non-solid paints (gradients, shaders, image
  fills) — neither pass nor fail (FR-004a). For a `Text` role the mapping is
  threshold-ordered (≥7 → Aaa, ≥4.5 → Aa, ≥3 → AaLarge, else Fail); for
  `GraphicOrUi` the binary 3:1 maps to `Aa`/`Fail` (3:1 is the single
  conformance bar there); `Decorative` always yields a pass-equivalent that the
  gate records but does not enforce.
- **Rationale**: Makes excluded inputs *visible* rather than silently certified —
  the project's evidence-honesty principle (Principle V/VII spirit).
- **Alternatives considered**: Throwing on non-solid paint (rejected — a thrown
  exception is not a *visible record*; the gate would crash rather than report).

## R4 — Alpha compositing rule

- **Decision**: For a color with `Alpha < 255`, composite over a supplied opaque
  background using straight source-over: `out = src·α + bg·(1-α)` per channel
  with `α = srcAlpha/255`, producing an opaque color before luminance. The gate
  composites any non-opaque token over its *theme background* token before
  measuring (FR-004). The rule is documented at the API and in the skill.
- **Rationale**: Contrast is defined only for opaque colors; source-over is the
  standard, deterministic compositing model and matches how Skia would draw the
  token over the background. Determinism keeps the gate and goldens meaningful.
- **Alternatives considered**: Premultiplied compositing (rejected — identical
  result for opaque bg but less obvious); refusing alpha tokens (rejected — theme
  tokens legitimately carry alpha).

## R5 — Palette source (Radix Colors)

- **Decision**: Ship ramps derived from **Radix Colors** — matched, role-
  structured 12-step light/dark scales per hue family. Encode the steps as
  literal `Color` values in `Palettes.fs` with documented per-step roles
  (app background, subtle background, component backgrounds, borders/focus,
  solid, text). Offer a curated subset of families sufficient to fix the shipped
  themes and give consumers a vetted source (FR-005).
- **Rationale**: Radix provides exactly the matched light/dark, role-labelled
  structure the spec wants, under a permissive (MIT) license. Because Radix's
  own guarantees are stated in APCA, the **WCAG gate** — not the source palette —
  is the authority that certifies WCAG conformance of any chosen value (spec
  Assumptions). Ramps are reusable *catalog data* in the library, **not** a
  second source of truth for shipped themes (FR-006): the DTCG source stays the
  sole authority feeding the generated token surface.
- **Alternatives considered**: Tailwind palette (rejected — not structured by
  semantic role, no built-in dark-mode pairing guarantee); Open Color (rejected —
  fewer role semantics); generating ramps algorithmically (rejected — out of
  scope; deterministic literal data is simpler and auditable).
- **Attribution**: Record Radix license attribution in the package/skill per the
  constitution's reuse-attribution rule.

## R6 — Validated-pairing set (the gate's contract)

- **Decision**: An explicit, documented list of `(foreground token, background
  token, role)` tuples — NOT the token cartesian product (FR-009). Seed set
  derived from how the themes are actually used: `foreground`-on-`background`
  (Text), `accent`-on-`background` (Text/GraphicOrUi as used), `danger`-on-
  `background` (Text), `muted`-on-`background` (Text), plus control-outline /
  focus pairings as `GraphicOrUi`. Each tuple is tagged with its role so the
  correct threshold applies; the text target is read from each theme's
  `contrastRequiredRatio` token (not hardcoded), graphic/UI uses fixed 3:1.
- **Rationale**: Avoids false failures on token pairs never drawn together
  (FR-009 edge) while still preventing the original "colors don't contrast
  enough" defect on the pairings that matter. Reading `contrastRequiredRatio`
  keeps the gate honest to each theme's own promise (Assumptions).
- **Alternatives considered**: Full cartesian product (rejected — flags
  semantically meaningless pairs); a fixed 4.5 constant (rejected — the spec
  wants the token-declared ratio honored).

## R7 — Gate integration & generation single-source

- **Decision**: Add `ContrastCheck` to the `Targets` union →
  `allTargets`/`name`/`directPrerequisites`/`spec` arms →
  `AgentValidation.knownGates` → two routing rules (append to
  `controls-public-surface`; new `color-contrast` rule for `src/Color/**`). The
  validator core (`ContrastGate.fs`) is pure over the parsed Light/Dark token
  values; the only filesystem read (the generated token values) is at the
  existing `Engine/Interpret.fs` edge. Regenerate `validation.contract.yml` from
  `Routing.fs` via the existing path; `TargetMetadataDrift` enforces currency.
- **Rationale**: The Targets union makes a mistyped gate a compile error;
  governance artifacts are generated, never hand-synced (CLAUDE.md). Routing on
  `src/Controls/**` ensures the gate is selected for design-token/theme changes
  (FR-011).
- **Alternatives considered**: A standalone script gate outside `FS.Skia.UI.Build`
  (rejected — would bypass `knownGates`/contract generation and could drift).

## R8 — Packable library mechanics

- **Decision**: `src/Color/Color.fsproj` mirrors `src/Scene/Scene.fsproj`:
  `IsPackable=true`, `PackageId=FS.Skia.UI.Color`, `Version` at the shared
  framework version, one `ProjectReference` to Scene. Add `FS.Skia.UI.Color` to
  `PerPackageSurface.packagesInScope` and commit
  `readiness/per-package-surface/FS.Skia.UI.Color.fsi.txt`. Add the template pin
  at `$(FsSkiaUiVersion)`. Add `src/Color/skill/SKILL.md` capability skill.
- **Rationale**: Follows the established packable-library and per-package-surface
  conventions exactly (FR-013); a new package with no baseline is never treated
  as clean (Principle VII), so the baseline must land with the surface.
- **Alternatives considered**: Folding contrast/palettes into `FS.Skia.UI.Scene`
  (rejected — the spec mandates a *new* package identity `FS.Skia.UI.Color`).
