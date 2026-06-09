# Quickstart: Interactive Non-Game Consumer Fitness

**Feature**: `086-interactive-consumer-fitness`

## What this delivers

A non-game consumer can generate a project, get a **neutral, controls-first**
scaffold, and have **mouse clicks work** in the governed default app — with real
**side-by-side layout** from a control tree, **per-control bounds** for Scene-host
hit-testing, **Scene translate + sized-text** primitives, and **no dropped
keystrokes** at window focus.

## Validate (escalated `maintainer-verify` — run sequentially; FAKE is not concurrency-safe)

```bash
# 1. Confirm routing escalates this change and names required evidence.
./fake.sh build -t Route --enforce

# 2. Serialized six-target order (one at a time, deterministic).
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck   # known local env-failure (non-authoritative); record output
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit

# 3. Regenerate the skill tree (fs-skia-viewer-host warm-up note) + surface baselines.
./fake.sh build -t RefreshSurfaceBaselines
# Recapture per-package + cross-package .fsi baselines for Scene + Controls.
```

## Headless checks you can run without a window

```bash
# Neutral-scaffold grep proof (SC-001): expect NO matches.
grep -rEn "playfield|gameplayRegion|tally|stage|upcoming|ActiveColumn|ActiveRow|NextToken|Initial\|Options\|Main\|Paused\|Ended" \
  template/base/src/Product/Model.fs template/base/src/Product/View.fs \
  template/base/tests/Product.Tests/BehaviorTests.fs

# Pointer dispatch (SC-003), side-by-side layout (SC-004), per-control bounds (SC-005),
# translate/sized-text (SC-006): Expecto semantic tests via Dev, plus an FSI transcript
# exercising Control.renderTree -> Bounds/hitTest and ControlsElmish.routeInteractivePointer.
```

## Live-window evidence (needs a COMPILED host — fsi cannot open a Vulkan window)

- **SC-002** real-controls screenshot: launch the controls-family default app and
  capture the live window showing actual styled controls via the production
  `controlsExampleView → Control.renderTree` path (NOT placeholder geometry).
- **SC-007** warm-up: a compiled self-closing host issues a known keystroke sequence
  within the first seconds after focus; assert all delivered to `MapKey`.
- Render-target PNGs of the production render path **do** work headless and satisfy the
  "real controls" honesty vocabulary without a window.

## Key entry points

| Concern | Symbol |
|---------|--------|
| Side-by-side layout | `Stack.orientation Horizontal` → `Control.renderTree` |
| Per-control bounds | `ControlRenderResult.Bounds` + `Control.hitTest` |
| Pointer host (default) | `ControlsElmish.runInteractiveApp` / `InteractiveAppHost.MapPointer` |
| Headless pointer route | `ControlsElmish.routeInteractivePointer` |
| Scene offset | `Scene.translate dx dy scene` |
| Sized chrome text | `Scene.sizedText pos text size color` |
