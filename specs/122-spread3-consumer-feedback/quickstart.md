# Quickstart: Verifying Feature 122

## Build / route
```sh
./fake.sh build -t Route            # confirm the escalated gate list for the diff
./fake.sh build -t Dev              # compile + inner-loop
```

## FR-001/002 — present path (no black buffer, idle preserved)
- Pure decision golden:
  ```sh
  dotnet test tests/SkiaViewer.Tests   # planPresent: PaintAndPresent → RepresentLastGood×2 → SkipPresent…
  ```
  Expect, for `bufferFillDepth = 3` and a (change, then static…) sequence:
  `[PaintAndPresent; RepresentLastGood; RepresentLastGood; SkipPresent; SkipPresent; …]`.
- Host present-action log: a static scene yields a populated buffer on every present (no
  `RepresentLastGood`/skip ever presents an undrawn buffer) and steady-state reaches `SkipPresent`
  (idle preserved).
- Offscreen byte-identical: existing screenshot/readback goldens unchanged.
- **Wayland windowed-fullscreen visual blink**: `[-]` — not reproducible in headless/Mesa CI (no
  Wayland windowed-fullscreen compositor); disclosed, not synthetic.

## FR-003/005 — window-behavior threading
```sh
# additive overload exists and default matches runInteractiveApp:
dotnet test tests/Controls.Elmish.Tests   # parity: WithWindowBehavior(default) == runInteractiveApp
```
- In a generated app: `dotnet run -- --window-startup normal` launches a **normal** (non
  windowed-fullscreen) controls window (the flag now reaches the live launch, not just the report).

## FR-006/007 — CustomControl
```sh
dotnet test tests/Controls.Tests   # validate/create with null Id / null effect → no NRE
```
- `docs/controls-catalog.md` custom-control entry states `renderTree` paints a labeled placeholder
  and points to primitive controls.

## FR-008/009/010/011 — docs/governance/skills
- `template/base/docs/evidence-formats.md` shows `key=value` token shapes for
  `interactive-visible-window.md` + `generated-validation.md`.
- `template/base/docs/scaffold-map.md` has the additive-files note.
- `.specify/templates/tasks-template.md` widgets hint flags the resolved `name:` trap.
- `.agents/skills/fs-skia-viewer-host/SKILL.md` documents the black-frame symptom + remedy.

## Routed gate set (sequential — FAKE is not concurrency-safe)
```sh
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit      # verdict=PASS, 0 synthetic
```
Plus the controls/package-surface gates + `SkillSyncCheck` that `Route` prints.
