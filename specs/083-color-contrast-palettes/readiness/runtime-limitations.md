# Runtime Limitations & Failure Diagnostics — Feature 083

How `FS.Skia.UI.Color` and the `ContrastCheck` gate handle unsupported scope, and the shape of
a fail-loud gate message. Unsupported inputs are made **visible**, never silently passed
(Principle V/VII; [[fs-skia-evidence-mode]] honesty stance).

## Unsupported scope — non-solid paints

- **Behavior**: `Contrast.checkPaint` returns `Verdict = Indeterminate` with `Ratio = nan`
  (`System.Double.NaN`, the documented not-applicable sentinel) for any non-solid paint —
  gradient (`LinearGradient`/`RadialGradient`/`SweepGradient`) or a paint with no resolvable
  solid fill color.
- **Why visible, not silent**: `Indeterminate` is neither pass nor fail. It is recorded as an
  excluded input rather than being certified as conformant. A solid paint (`SolidColor` shader,
  or no shader with a concrete `Fill`) measures a real ratio with no render pass (declared-fill
  capability, FR-001a).
- **Authoritative check**: `tests/Color.Tests` (`checkPaint` non-solid → Indeterminate/nan;
  solid → measured).

## Decorative role — recorded, never enforced

- `Contrast.verdict Decorative _ = Exempt` for any ratio. The gate records decorative pairings
  but never fails on them (FR-009 edge). The measured ratio is still carried for the report.

## Inherited product runtime limitations (unchanged by this feature)

`FS.Skia.UI.Color` is **pure managed arithmetic** (WCAG luminance/ratio) plus static palette
data — it has no native, render, or host dependency and depends only on `FS.Skia.UI.Scene`. It
therefore changes none of the shipped product runtime limitations: the product still targets
**.NET 10 desktop** on Windows and Linux, renders through **Vulkan**, and depends on a
**SkiaSharp preview** native build; platforms remain **unsupported macOS/mobile/browser**, and
there is **no software-renderer fallback**. The contrast gate runs in the framework repo's
build tooling, not in the product runtime.

## Out of scope (deferred)

- APCA / WCAG 3 scoring, rendered-pixel sampling, gradient-stop worst-case analysis,
  color-blindness simulation, palette generation from brand seeds. These are explicitly out of
  scope (spec Unsupported scope) and are not silently approximated.

## Fail-loud `ContrastCheck` message shape (FR-008)

On failure the gate names, per failing row: **both token names**, **both resolved colors**,
**measured ratio**, **required ratio**, **theme**, and **role**. Example diagnostic line:

```
dark theme: danger on background (Text) measures 2.74:1 but requires 4.50:1 — foreground #b91c1cff over background #111827ff
```

- **Authoritative command**: `./fake.sh build -t ContrastCheck`.
- **Artifact path**: `readiness/color-contrast-evidence.md` (per-pairing rows, both themes).
- **Failure class**: `sub-threshold-shipped-token` (product).
- **Next action**: edit only the failing `$value` in `src/Controls/design-tokens.tokens.json`
  (drawing a replacement from a ramp), `RefreshSurfaceBaselines`, re-run `ContrastCheck`.
