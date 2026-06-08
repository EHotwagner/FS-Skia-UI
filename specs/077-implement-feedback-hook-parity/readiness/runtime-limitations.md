# Runtime limitations & behavior-preservation (feature 077)

## Behavior preservation (T015 — SC-006 / FR-005 / FR-009)

The four repaired phase skills (`implement`, `tasks`, `taskstoissues`, `constitution`) gain
a multi-file hook-discovery block that is a **silent no-op when no matching hook is
registered**. This repository registers only `git` and `evidence` hooks in
`.specify/extensions.yml` (no `feedback` extension), so the new blocks:

- produce **no new error**, prompt, or feedback file when the active phase has no extra hook;
- only surface/auto-run hooks that are *already* registered (e.g. the existing `before_*`
  `git.commit` optional hooks and the `before_implement` mandatory `evidence.graph` hook),
  exactly as the five already-compliant siblings (`specify`, `clarify`, `plan`, `analyze`,
  `checklist`) do.

The four edited skills now read identically (modulo `<phase>`/anchor wording) to the five
compliant siblings, so behavior is provably the same: the discovery is parse-tolerant
(absent/invalid files skipped silently) and "no hooks registered ⇒ skip silently".

This `/speckit.implement` run itself exercised the repaired `before_implement` discovery: the
only registered `before_implement` hooks here are the optional `git.commit` (surfaced, not
force-run) and the mandatory `evidence.graph` (the DAG validation reflected in
`evidence-graph.md`). No `feedback` record was produced because no feedback extension is
registered — the intended no-op.

## Governance-text scope

This feature is **governance text + a pure validation rule**. No runtime model, interpreter,
effect, subscription, layout, charts, DataGrid, rendering, screenshot, Vulkan, or Skia
behavior changes — the only product-reachable change is corrected vendored Spec Kit
phase-skill text that ships through the existing copy globs.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux, renders
through **Vulkan**, and depends on a **SkiaSharp preview** native build. Platforms remain
**unsupported macOS/mobile/browser**, and there is **no software-renderer fallback**. This
feature changes none of that — it only makes the phase skills honor their registered hooks.

## Known non-authoritative environment failure

`GeneratedProductCheck` fails locally for environment reasons unrelated to this feature (no
template `feature.json`; `Map.empty` env). See `generated-product-check.md` and
`aggregate-hang-diagnostics.md`. Propagation is proven by `TemplateCheck` instead.
