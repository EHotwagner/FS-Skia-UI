# Scaffold map — durable vs replaceable (read before you design)

This generated product ships a **scaffold game model** plus a **durable governance
spine**. When you replace the scaffold with your own game, you rewrite only the
replaceable parts; the durable parts keep compiling and keep their source/evidence
scans green across the swap. Read this map **before** you start designing, so you
know what survives and what you own.

## Replaceable `src/Product/**` (rewrite when you swap the scaffold model)

These call/define the scaffold game model directly — they are yours to replace:

- `src/Product/Model.fs` — the scaffold `Model`/`Msg`/`update` (the game state machine).
- `src/Product/View.fs` — the scaffold `view` (`Model -> SceneNode`).

## Durable `src/Product/**` (keep — model-agnostic host/evidence wiring)

These are model-agnostic plumbing; keep them and re-point them at your own model:

- `src/Product/Program.fs` — host/CLI entry point (`Viewer.runApp`, the
  `--scene-evidence` / evidence command wiring).
- `src/Product/EvidenceCommands.fs` — the deterministic `SceneEvidence.render`
  evidence command (`RendererMode = "deterministic-scene"`).
- `src/Product/WindowOptions.fs` — window-options parsing/diagnostics.
- `src/Product/LayoutEvidence.fs` — layout/gameplay-region bounds evidence.

## The test split: `GovernanceTests.fs` durable, `BehaviorTests.fs` replaceable

`tests/Product.Tests/` compiles `GovernanceTests.fs` **first** and
`BehaviorTests.fs` **after** (see `Product.Tests.fsproj`):

- **`GovernanceTests.fs` — durable, model-agnostic.** Reads the product **source
  text** and asserts structural / evidence / discoverability invariants. It never
  calls the product's `view`/`update`, so it **survives a scaffold-model swap** —
  do not rewrite it.
- **`BehaviorTests.fs` — replaceable scaffold-behavior.** Calls the scaffold
  product's `view`/`update`/host/scene-text directly. When you replace the
  scaffold model with your own, you **rewrite this file**; `GovernanceTests.fs`
  keeps passing.

## Must-survive source-scan strings (keep these tokens present)

`GovernanceTests.fs` (and the framework's generated-guidance scans) assert these
strings remain in the product source across any model swap — keep them present
when you re-point the durable files:

- `--scene-evidence` and `SceneEvidence.render` (the deterministic scene evidence
  command) with `RendererMode = "deterministic-scene"`.
- The visual-evidence honesty vocabulary (decodable image; image dimensions;
  non-trivial content; renderer mode; fallback classification; unsupported reason;
  "metadata-only reports do not satisfy visual proof"; "1x1 fallback images do not
  satisfy visual proof"; benign/blocking/deferred warning; name-collision
  guidance) carried in `GovernanceTests.visualEvidenceGuidance`.

## API surface authority: the shipped `.fsi` / `docs/api-surface/` is ground truth

When you need to know a framework API's real shape, the **authoritative** reference
is the shipped `.fsi` signature files and the generated `docs/api-surface/` tree —
they are the curated public contract the packages actually expose. An
**agent-generated API summary** (e.g. an Explore/grep digest, or a hand-written
"here's what the API looks like" note) is **supporting reference only, never ground
truth**: it can silently mix confirmed signatures with inferred or stale shapes.
Always reconcile any agent-produced API summary against the `.fsi` / `docs/api-surface/`
before you design against it; when they disagree, the `.fsi` wins.

## Pre-design pointer: record-label collision (fs-skia-scene)

**Before** you design your model's records, read the `fs-skia-scene` skill's
**Common pitfalls → record-label collision** note. Scene point/rect literals use
the labels `X`/`Y`/`Width`/`Height`; if your own model declares a record with the
same labels, a bare literal can infer to the wrong record type. Plan your record
label names (or annotate/qualify) up front — it is far cheaper than reworking the
model after the inference errors appear.
