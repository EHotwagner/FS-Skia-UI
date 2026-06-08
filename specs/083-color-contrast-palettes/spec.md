# Feature Specification: Accessible Color Contrast & Palettes

**Feature Branch**: `083-color-contrast-palettes`
**Created**: 2026-06-08
**Status**: Draft
**Input**: User description: "Some colors used do not contrast enough. Provide ready-made WCAG-2.x-accessible palettes and a programmatic way to validate which colors can be used together and still be legible — designed as an F# library producing fs-skia-ui types, with a new packable `FS.Skia.UI.Color` library (WCAG contrast logic + Radix-derived palette ramps), themes still authored through the DTCG single source, a new `ContrastCheck` build gate, and guidance folded into the `fs-skia-design-tokens` skill."

## Overview

The framework ships Light and Dark themes whose color tokens are not all
guaranteed to be mutually legible — some foreground/background pairings fall
below the contrast their own `contrastRequiredRatio` token promises. This
feature gives the framework (1) a reusable, consumer-facing way to *measure*
whether any two colors are legible together by the WCAG 2.x standard, (2)
ready-made accessible color ramps to *choose* legible values from, and (3) an
automatic gate that *enforces* the promise every theme already makes — so a
theme can never ship with an illegible pairing again.

## Clarifications

### Session 2026-06-08

- Q: At what level does the contrast capability operate? → A: On declared fill
  colors — the semantic `Color`/Paint fill values of any Skia-renderable element
  (text glyph, icon, symbol, vector shape, stroke) measured against the color it
  is drawn over. Deterministic; no render/raster pass. "Fit to Skia" means the
  API speaks Skia's paint/color types and applies to *any* drawable's fill, not
  text alone. Rendered-pixel sampling is out of scope (possible later follow-up).
- Q: How should the role/threshold model classify Skia graphics & symbols? → A:
  Three roles. **Text** → 4.5:1 body, 3:1 large (>=18pt or >=14pt bold), 7:1
  AAA. **Graphic-or-UI** (meaningful symbols, icons, vector shapes, control
  outlines, focus rings, and other graphical objects required to understand
  content) → 3:1 per WCAG 1.4.11. **Decorative** → exempt (no contrast
  requirement). The verdict API takes the role and applies the matching
  threshold.
- Q: How should non-solid Skia paints (gradients, shaders, image fills) be
  handled? → A: Solid fills only in v1. Non-solid paints are reported as
  `Indeterminate`/unsupported — neither pass nor fail — so they are visibly
  excluded rather than falsely certified, consistent with the project's
  evidence-honesty principle. Worst-case gradient stop analysis is a noted
  follow-up, out of scope here.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Themes are guaranteed legible (Priority: P1)

As a framework maintainer editing theme colors through the design-token single
source, I want every foreground/background pairing in each theme to be
automatically checked against the contrast ratio that theme declares, so that I
cannot accidentally ship an illegible pairing and the original "colors don't
contrast enough" defect cannot recur.

**Why this priority**: This is the direct fix for the reported problem and the
only part that *prevents regression*. It delivers value even if no consumer ever
calls the new library directly.

**Independent Test**: Introduce a deliberately low-contrast value into a theme
token, run the contrast gate, and confirm it fails naming the offending pairing,
the measured ratio, and the required ratio; restore an accessible value and
confirm it passes.

**Acceptance Scenarios**:

1. **Given** a theme whose foreground-on-background pairing meets the theme's
   declared required ratio, **When** the contrast gate runs, **Then** it reports
   that pairing as passing with the measured ratio.
2. **Given** a theme token edited to a value that drops a pairing below the
   declared required ratio, **When** the contrast gate runs, **Then** it fails,
   names the pairing (e.g. `danger` on `background`), reports measured vs.
   required ratio, and the failure is actionable.
3. **Given** both Light and Dark themes, **When** the gate runs, **Then** both
   themes are validated independently against their own declared ratios.

---

### User Story 2 - Ready-made accessible palettes to choose from (Priority: P2)

As a maintainer or downstream app developer building a theme, I want
ready-made, accessibility-tuned color ramps — available as native fs-skia-ui
color values for both light and dark surfaces — so I can pick legible token
values without hand-tuning hex codes.

**Why this priority**: Turns "fix the failing colors" into "have a vetted source
to fix them from," and gives downstream consumers reusable value. Depends on
nothing from later stories.

**Independent Test**: From the new library, select a text-role step and a
background-role step from the same ramp family, run them through the contrast
function, and confirm the pair meets the AA body-text threshold; confirm a
matched light and dark ramp exist for the same family.

**Acceptance Scenarios**:

1. **Given** the palette library, **When** a developer requests a color ramp,
   **Then** they receive an ordered set of steps with documented roles
   (backgrounds, component backgrounds, borders/focus, solid, text) for both a
   light and a dark variant of the same family.
2. **Given** a ramp's documented text step and a documented background step from
   the same family, **When** their contrast is measured, **Then** it meets the
   AA body-text threshold (>= 4.5:1).

---

### User Story 3 - Validate any color pair programmatically (Priority: P3)

As a developer or agent, I want to compute the WCAG 2.x contrast ratio and
pass/fail verdict for any two fs-skia-ui colors in a single call, so I can check
custom color choices (including end-user-supplied colors at runtime) against
AA/AAA thresholds.

**Why this priority**: The general-purpose capability underneath stories 1 and
2; valuable on its own for consumer apps but not required to fix the shipped
themes.

**Independent Test**: Call the contrast function on known reference pairs (e.g.
pure black on pure white) and confirm the returned ratio and verdict match the
WCAG reference value.

**Acceptance Scenarios**:

1. **Given** two colors, **When** their contrast is computed, **Then** the
   returned ratio matches the WCAG 2.x reference value for that pair within a
   negligible tolerance.
2. **Given** a contrast ratio, **When** a verdict is requested for a text role,
   **Then** it reports AAA (>= 7:1), AA (>= 4.5:1), AA-Large (>= 3:1), or Fail
   consistent with WCAG 2.x thresholds.
3. **Given** a non-text/UI-component role, **When** a verdict is requested,
   **Then** the applicable threshold is 3:1 (WCAG 1.4.11).

---

### Edge Cases

- **Alpha / translucency**: Tokens may carry an alpha channel. Contrast is
  defined for opaque colors; the gate composites any non-opaque token over its
  theme background before measuring, and this compositing rule is stated so
  results are deterministic.
- **Token aliasing**: A theme token may alias another (e.g. Dark `danger`
  aliasing Light `danger`); the gate measures the *resolved* color.
- **Identical colors**: A pairing of equal colors yields the minimum ratio
  (1:1) and fails any text/component threshold.
- **Linearization boundary**: Channel values at the sRGB linearization
  threshold are handled by the WCAG-specified piecewise formula.
- **Pairings not intended to overlap**: Only semantically meaningful pairings
  are validated; the validated pairing set is explicit, not the full cartesian
  product of tokens, so unrelated tokens are not falsely flagged.
- **Non-solid paints**: A gradient, shader, or image fill has no single color;
  it is reported `Indeterminate` (not pass/fail) so it is visibly excluded rather
  than wrongly certified.
- **Decorative graphics**: Elements not required to understand content carry the
  Decorative role and are exempt from any threshold; they are recorded but not
  enforced, so they cannot cause a false failure.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST provide a function that computes the WCAG 2.x
  relative luminance of an fs-skia-ui color using the standard sRGB
  linearization and the 0.2126/0.7152/0.0722 weighting.
- **FR-001a**: The capability MUST operate on *declared fill colors* — the
  semantic color/paint values of any Skia-renderable element (text glyph, icon,
  symbol, vector shape, stroke) — not on rasterized pixels. It MUST NOT require a
  render pass, and its inputs MUST be the same color/paint types Skia draws with,
  so contrast can be checked for any drawable's fill against the color behind it.
- **FR-002**: The system MUST provide a function that computes the WCAG 2.x
  contrast ratio between two fs-skia-ui colors as `(L1 + 0.05) / (L2 + 0.05)`
  with the lighter luminance as `L1`, yielding values in the range 1:1 to 21:1.
- **FR-003**: The system MUST provide a verdict mapping from a contrast ratio
  and an element *role* to a WCAG conformance result. The role taxonomy is:
  **Text** (AAA >= 7:1, AA >= 4.5:1, AA-Large >= 3:1, with large text defined as
  >= 18pt or >= 14pt bold), **Graphic-or-UI** (>= 3:1 per WCAG 1.4.11 — covers
  meaningful symbols, icons, vector shapes, strokes, control outlines, and focus
  rings), and **Decorative** (exempt — no contrast requirement). The verdict
  applies the threshold matching the supplied role.
- **FR-004**: Contrast computation MUST operate on opaque colors; when a color
  carries alpha < full, the system MUST composite it over a specified
  background color before measuring, using a documented, deterministic
  compositing rule.
- **FR-004a**: The capability MUST support solid color fills. Non-solid paints
  (gradients, shaders, image fills) MUST be reported as `Indeterminate`
  (unsupported) — neither pass nor fail — rather than reduced to a single color
  or silently passed, so excluded inputs are visible. Worst-case gradient-stop
  analysis is out of scope (noted follow-up).
- **FR-005**: The system MUST provide ready-made accessible color ramps derived
  from an established open palette system, offered as fs-skia-ui color values,
  with matched light and dark variants of each ramp family and documented
  per-step roles (background, component background, border/focus, solid, text).
- **FR-006**: The palette ramps MUST be exposed as reusable data within the new
  library; they MUST NOT become a second source of truth for the framework's
  shipped themes — shipped theme values continue to be authored through the
  design-token single source and generated into the typed token surface.
- **FR-007**: A new build gate (`ContrastCheck`) MUST load the generated Light
  and Dark theme tokens and validate an explicit set of semantic
  foreground/background pairings against each theme's declared required contrast
  ratio (for text pairings) and the 3:1 threshold (for non-text/UI pairings).
- **FR-008**: When any validated pairing falls below its required threshold, the
  `ContrastCheck` gate MUST fail and report, per failing pairing: the two token
  names, the resolved colors, the measured ratio, the required ratio, and the
  theme.
- **FR-009**: The set of validated pairings MUST be explicit and documented (not
  the full token cartesian product), each tagged with its element role (Text,
  Graphic-or-UI, or Decorative) so the correct threshold is applied to each;
  Decorative pairings are recorded but not enforced.
- **FR-010**: The shipped Light and Dark theme token values MUST be brought into
  conformance so the `ContrastCheck` gate passes; pairings that already conform
  are left unchanged, and only failing values are adjusted to accessible values
  drawn from the ready-made ramps.
- **FR-011**: The `ContrastCheck` gate MUST be registered in the governance
  routing so it is selected for design-token / theme changes, listed in the
  known-gates registry, and reflected in the generated validation contract — via
  the single-source generation path, not hand-editing generated artifacts.
- **FR-012**: Guidance for measuring contrast, choosing palette values, and
  interpreting/curing `ContrastCheck` failures MUST be added to the existing
  `fs-skia-design-tokens` skill (canonical `.agents` source), with the `.claude`
  mirror regenerated through the existing sync path so the two cannot drift.
- **FR-013**: The new library MUST be a packable public package with a complete
  public signature surface and a per-package surface baseline, and the template's
  package pins MUST include the new package at the shared framework version.

> Interacting / conflicting requirements: FR-005/FR-006 (ship palette data in
> the library) vs. the single-source-of-truth rule (themes come from the DTCG
> source). Resolution: the library's ramps are a reusable *catalog* consumers and
> maintainers select from; the DTCG source remains the sole authority for the
> framework's *shipped* themes. The two never both feed the generated token
> surface. FR-010 (fix failing colors) vs. minimizing visual churn: only
> pairings the gate reports as failing are changed; conforming tokens keep their
> current values.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Adds a NEW package identity `FS.Skia.UI.Color` (contents:
  contrast functions + accessible palette ramp data), depending on
  `FS.Skia.UI.Scene` for the `Color` type. The template package pins gain the
  new package at the shared `$(FsSkiaUiVersion)`; on merge every packable project
  bumps version per the merge flow. No legacy Charts migration involved.
- **Public contract impact**: NEW public `.fsi` surface for `FS.Skia.UI.Color`
  plus a new per-package surface baseline. The existing `DesignTokens.fsi` token
  *surface* (token names/types) is unchanged; only generated token *values* in
  `DesignTokens.fs` change where colors are brought into conformance. No other
  public API changes.
- **State workflow impact**: None. No stateful workflow, I/O, commands, effects,
  subscriptions, or interpreter behavior changes.
- **Layout/rendering impact**: Theme color *values* change where pairings were
  failing, so rendered control colors (and any color-bearing preview
  screenshots) shift toward more legible values. No layout, chart, DataGrid,
  rendering-engine, Skia, or Vulkan behavior changes.
- **Evidence obligations**: `ContrastCheck` readiness report (both themes,
  per-pairing measured vs. required); regenerated `DesignTokens.fs`; new
  per-package surface baseline for `FS.Skia.UI.Color`; regenerated validation
  contract; regenerated `.claude` skill mirror; the escalated maintainer-verify
  six-target evidence set.
- **Unsupported scope**: APCA / WCAG 3 contrast model; automatic palette
  generation from arbitrary brand colors; color-blindness simulation; a
  theme-authoring UI; any non-color token (size/density/radius/font); changing
  the meaning or value of the `contrastRequiredRatio` token itself.
- **Build-target impact**: NEW `ContrastCheck` gate target added to the target
  enumeration, routing rules, known-gates registry, and the generated
  `validation.contract.yml`. `TemplateCheck` is affected by the new package pin;
  `GeneratedGuidanceCheck` / skill-sync by the skill edit; `DesignTokenDrift`
  remains the authority for token currency (unchanged). The escalated change
  routes through the serialized six-target maintainer-verify path.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of the explicitly-validated foreground/background pairings in
  both the Light and Dark shipped themes meet their required contrast threshold
  (the `ContrastCheck` gate passes on the shipped themes).
- **SC-002**: The contrast computation reproduces WCAG 2.x reference values for
  known pairs — e.g. black-on-white returns 21:1 and white-on-white returns 1:1
  — within a tolerance of 0.01.
- **SC-003**: Ready-made ramps provide a matched light and dark variant for each
  offered family, each with documented background, border/focus, solid, and text
  roles, and at least one documented text/background step pair per family meets
  >= 4.5:1.
- **SC-004**: A developer can obtain a contrast ratio and an AA/AAA verdict for
  any two colors in a single function call.
- **SC-005**: Introducing a sub-threshold value into any validated theme pairing
  causes the gate to fail with a message identifying the pairing, the measured
  ratio, and the required ratio (regression protection is demonstrable).
- **SC-006**: The new package, its surface baseline, the routing/known-gates
  registration, the validation contract, and the `.claude` skill mirror are all
  consistent and generated through their single-source paths (no drift detected
  by the currency/sync gates).

## Key Entities

- **Color**: An fs-skia-ui RGBA color value (the existing core `Color` type);
  the unit both contrast measurement and palette ramps operate on.
- **Contrast ratio**: A scalar in 1:1–21:1 derived from two colors' relative
  luminance per WCAG 2.x.
- **Conformance verdict**: A classification (AAA / AA / AA-Large / Fail /
  Indeterminate) derived from a contrast ratio and an element role.
- **Element role**: The kind of Skia-renderable element a color is used for —
  Text, Graphic-or-UI (meaningful symbol/icon/shape/stroke/outline/focus ring),
  or Decorative (exempt) — which selects the applicable threshold.
- **Palette ramp**: An ordered, role-labelled sequence of colors for a single
  hue family, in matched light and dark variants.
- **Validated pairing**: A named (foreground token, background token, role) tuple
  the gate checks against a threshold.
- **Required ratio**: The per-theme contrast target carried by the existing
  `contrastRequiredRatio` token.

## Assumptions

- The required contrast target for *text* pairings is the value already carried
  by each theme's `contrastRequiredRatio` token; the gate reads it rather than
  hardcoding a constant. Non-text/UI pairings use the fixed WCAG 1.4.11 3:1
  threshold.
- "Legible together" is defined by the WCAG 2.x relative-luminance contrast
  model (the standard the user selected), not APCA/WCAG 3.
- The ready-made ramps are derived from Radix Colors (the open system that
  provides matched, role-structured light and dark scales). Because Radix's own
  guarantees are stated in APCA, the WCAG gate — not the source palette — is what
  certifies WCAG conformance of the chosen values.
- Large-text vs. body-text distinction follows WCAG (>= 18pt, or >= 14pt bold);
  the validated theme pairings are treated as body text unless a pairing is
  explicitly designated otherwise.
- Non-opaque tokens are composited over their theme background before measuring;
  disabled/inactive UI states are exempt from the non-text contrast requirement
  per WCAG.
- This change is a consumer-contract change and escalates to the serialized
  maintainer-verify six-target path.

## Dependencies

- The existing design-token single source and its generator (DTCG →
  `DesignTokens.fs`) and the `DesignTokenDrift` currency gate.
- The core `Color` type and helpers in the Scene library.
- The governance routing, known-gates registry, validation-contract generation,
  and `.agents` → `.claude` skill sync paths.
- The template package-pin mechanism and the shared framework version property.

## Out of Scope

- APCA / WCAG 3 contrast scoring (possible later dual-threshold follow-up).
- Rendered-pixel / rasterized-output sampling (validation operates on declared
  fill colors only; pixel sampling is a possible later follow-up).
- Contrast analysis of non-solid paints — gradient-stop worst-case, shader, and
  image-fill evaluation (reported `Indeterminate` for now).
- Generating palettes from arbitrary brand/seed colors.
- Color-blindness / deuteranopia simulation and non-contrast accessibility
  concerns.
- Non-color design tokens (size, density, radius, typography).
- A runtime or design-time theme-authoring UI.
