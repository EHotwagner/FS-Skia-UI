# Implementation Plan: Demonstrative Control Preview Images in Published Docs

**Branch**: `079-doc-preview-examples` | **Date**: 2026-06-08 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/079-doc-preview-examples/spec.md`

## Summary

Feature 078 added a published **Controls** catalog with one render-only preview PNG
per control at `docs/img/controls/<id>.png`. Those previews were rendered through each
control's typed front door with **bare `defaults`**, so most are near-blank ~363-byte
canvases that convey nothing. This feature replaces them with **demonstrative
previews** driven by a **single declared per-control sample source** (representative
fixed content/state), produced through the **same real render-only evidence path**
(`Control.render Theme.light` → `SkiaViewer.captureScreenshotEvidence`
`ViewerRenderTargetPng`). The committed render harness 078 lacked is added so
regeneration is **deterministic, idempotent, and reviewable** (FR-002, FR-008). The
existing `ControlsCatalogDocsCheck` currency gate is **strengthened with a
trivial-content guard** (a real byte-floor property of the committed PNG, verified
against the regenerated assets) so a preview regressing to empty/near-empty content
fails like a missing one. The per-control preview evidence record is regenerated to
the demonstrative renders. Finally, the **Controls** nav category is **repositioned**
(via fsdocs `categoryindex`) to render immediately below **Examples** and above
**Guides**, with no page/file relocation and all cross-links still resolving.

## Technical Context

**Language/Version**: F# / .NET (`net10.0`); build front `FS.Skia.UI.Build` under
`build/Governance/**`; render harness references the published Controls + SkiaViewer
surface with SkiaSharp 4 preview (already pinned).
**Primary Dependencies**: No new dependency. Reuses the already-pinned SkiaSharp via
`SkiaViewer`/`Testing`, the typed `FS.Skia.UI.Controls.Typed` front door, `Scene`
(`Control.render`, `Theme.light`), and the already-pinned `fsdocs` tool. The
governance build that runs the currency gate stays SkiaSharp-free (dependency-light).
**Testing**: Expecto governance tests in `FS.Skia.UI.Build` test project (failing-first
currency + trivial-guard semantics, renderer harness determinism/idempotence over
committed bytes); `dotnet fsdocs build --strict --eval` site build; FAKE-backed
targets run sequentially (shared `.fake` state) in the documented order.
**Target Platform**: Windows and Linux. Preview *rendering* requires a render-capable
host (GPU/Skia); the **docs/site build remains GPU-free** and consumes the committed
PNGs unchanged (FR-009).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Tier**: **Tier 2 (internal change)** with consumer-contract *generation* surface.
No public product `.fsi`/API/behavior changes (Change Classification §Tier 1 does not
apply): the preview machinery and currency gate **read** the existing public control
surface and `CatalogGen.catalogFacts`; they do not redefine it. The new per-control
sample definitions and render harness are **build-tool internal** surface. Because the
change touches governance paths and `docs/**` consumed-contract assets, `Route`
escalates it to the `maintainer-verify` path (serialized six-target order).

**Initial gate evaluation (pre-Phase 0)**: PASS. No principle is violated; the design
keeps `update`/generation pure with I/O at the edge (Principle IV), renders through the
**real** evidence path with explicit honest "unsupported" for non-renderable controls
rather than synthetic placeholders (Principle V), and adds failing-first governance
tests (Principle VI). See the governance decisions below.

**Post-Phase 1 re-evaluation**: PASS — see [research.md](./research.md) (R1–R6) and
[data-model.md](./data-model.md). No new violations introduced by the design; the
trivial-content guard is a real structural property of committed bytes (no synthetic
evidence), and the render harness's sample literals are explicit, fixed, and reviewable.

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json` change. This feature
  touches the repository's *own* published docs (`docs/**`), its governance build
  (`build/Governance/**`), and a new render harness; it ships **nothing** into the
  `dotnet new fs-skia-ui` template and changes no template source, samples, package
  policy, or command surface the template embeds.
- **Dependency impact**: N/A — no new dependency. `Directory.Packages.props`,
  `docs/dependencies.md`, generated template inclusion, and `DependencyReport` coverage
  are unchanged. The render harness reuses already-pinned SkiaSharp via the existing
  `SkiaViewer`/`Testing`/`Controls` packages; the currency gate stays SkiaSharp-free.
- **Command-surface impact**: **Changes required (additive).** (a) A committed
  **render harness** (compiled generator/test that 078 lacked) writes the demonstrative
  PNGs from the per-control sample source; its invocation command is documented in
  [quickstart.md](./quickstart.md). (b) The existing `ControlsCatalogDocsCheck` handler
  (`build/Governance/Engine/Update.fs`) and its pure core (`CatalogDocsGen.fs`) gain the
  **trivial-content guard** (byte-floor + evidence-record consistency); the gate's
  routing, `Targets.fs` registration, and `AgentValidation.knownGates` membership are
  unchanged (no *new* gate). `validation.contract.yml` is regenerated from `Routing.fs`
  if any routed glob changes (not hand-edited; `TargetMetadataDrift` enforces currency).
  `Dev`, `Verify`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`,
  `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`, `GeneratedGuidanceCheck` do not
  change behavior. FAKE-backed commands run **sequentially** (shared `.fake` state) in
  the documented order; non-FAKE reads may parallelize.
- **Generated project impact**: N/A — no change to default/minimal generated contents,
  selected Controls guidance, generated local skills, validation logs,
  placeholder/excluded-history scans, or generated `Dev` behavior. The previews and nav
  live in **this** repo's site, not in generated projects.
- **Evidence paths**: Readiness evidence under
  `specs/079-doc-preview-examples/readiness/`: `controls-preview-evidence.md`
  (regenerated per-control honesty ledger — decodable / dimensions / bytes / content
  classification / renderer mode / **unsupported count**), `controls-catalog-docs.md`
  (`ControlsCatalogDocsCheck` PASS with demonstrative previews; FAIL on a
  blanked/trivial/missing/orphan preview — one negative case per failure class),
  `docs-build.md` (`dotnet fsdocs build --strict --eval` success with all previews
  present, all image links resolving, and the **Examples → Controls → Guides** nav order
  observed), plus the standard governance-suite outputs (`evidence-graph.md`,
  `evidence-audit.md`, `gate-diagnostics.md`, `visual-evidence-honesty.md`).
- **`.fsi` / contract impact**: N/A for **public product** surface — no public `.fsi`
  signatures, documented public types, surface baselines, sample contracts, or
  compatibility notes change. The render harness's per-control sample definitions are an
  **internal build/harness** source (Tier-2 internal), not product public contract; if
  authored as a curated `.fsi`-bearing module under the harness/build it follows
  Principle II locally but does not alter any published baseline.
- **MVU/effect boundary**: N/A as a *runtime* concern — no framework
  `Model`/`Msg`/`Effect`/`update` change. The generator and currency check keep the
  governance engine's pure-core / edge-interpreter shape: pure functions compute the
  rendered sample IR, currency findings, and the trivial-content verdict over in-memory
  values; file reads/writes and `FailWith` happen only at the `Engine/Update.fs` edge.
  The render harness constructs **fixed** control state at the render edge only.
- **Synthetic evidence**: Identified risk — demonstrative PNGs cannot be rendered in the
  GPU-free docs CI. Resolution **avoids** synthetic evidence: previews are rendered
  through the **real** deterministic render-only path on a render-capable host and
  committed as source assets; the gate validates the committed bytes structurally. The
  trivial-content guard is a **real** property of those bytes (not a stub). A control
  that genuinely cannot be honestly rendered keeps its explicit honest **unsupported**
  declaration and commits no image (an honest declaration, not a fabricated/1×1
  placeholder — both rejected). Any control whose demonstrative render cannot be honestly
  produced in this iteration is a **disclosed `[S]`** follow-up with its evidence row, not
  a silent omission (FR-007, SC-005). No `[S]` is expected at merge.
- **Test evidence**: Failing-first governance tests in the `FS.Skia.UI.Build` test
  project: (a) the strengthened currency check FAILs on a **trivial/blanked** preview
  (byte-floor breach), a missing preview, an undecodable preview, an orphan preview, and
  a stale/missing detail region, and PASSes on the regenerated demonstrative tree;
  (b) the render harness is **idempotent** — re-running over the same sample source
  produces byte-identical PNGs (asserted over committed bytes / a hash manifest);
  (c) the per-control sample source is **total** over `catalogFacts` (every supported,
  renderable control has exactly one sample definition or an explicit unsupported entry —
  no silent gap). Plus `dotnet fsdocs build --strict --eval` succeeds with the new
  assets and nav order.
- **Observability**: The `ControlsCatalogDocsCheck` report names each failing condition
  (now including **TrivialPreview**) with an actionable remedy and the re-render command;
  missing required artifacts fail loudly via `RequireFiles`. Preview validation
  distinguishes a genuinely non-renderable control (honest unsupported message) from a
  real `RenderingFailure`, preserving real diagnostics per the evidence-mode
  benign/blocking rules. The per-control evidence record records the rendered-vs-declared
  -unsupported counts explicitly (no silent omission).
- **Deferred scope**: No new controls, no control-behavior/layout/visual redesign, no
  docs-theme redesign, no catalog index/detail-page structural change, no
  API-reference-generator change, no live/animated/interactive/multi-frame/dark-mode
  previews, no release/distribution/platform change. Repositioning the Controls nav
  *ordering* (FR-011) is in scope; restructuring its pages/files is not. Any preview that
  cannot be honestly produced this iteration is a bounded, disclosed `[S]` follow-up.

## Project Structure

```
specs/079-doc-preview-examples/
├── plan.md                       ← this document
├── spec.md                       ← feature specification (requirements)
├── research.md                   ← R1–R6 design decisions (Phase 0)
├── data-model.md                 ← sample-definition + evidence + nav entities (Phase 1)
├── contracts/
│   ├── preview-samples.contract.md   ← P1–P6: sample source, render path, trivial guard, evidence
│   └── nav-ordering.contract.md      ← N1–N3: Examples→Controls→Guides ordering, link resolution
├── quickstart.md                 ← author/re-render/verify loop with FAKE commands
└── readiness/                    ← evidence artifacts (produced during implementation)

Render harness + sample source (compiled, references Controls + SkiaViewer + SkiaSharp):
└── <render-harness>                  ← NEW: per-control sample definitions (single source, FR-002)
                                         + deterministic render loop writing docs/img/controls/<id>.png
                                         (placement decided in research R2 — a render project/test that
                                         already references the typed Controls + SkiaViewer surface)

Governance build (currency gate strengthened — no new gate):
├── build/Governance/CatalogDocsGen.fs(/.fsi)  ← add TrivialPreview finding + byte-floor guard (pure)
├── build/Governance/Engine/Update.fs          ← wire trivial guard + evidence-record consistency (edge)
└── build/Governance/validation.contract.yml   ← regenerated from Routing.fs IFF a routed glob changes

Docs (content + nav reposition — no file relocation):
├── docs/controls/<id>.md  ×52 + catalog.md + spec-kit-workflow.md  ← categoryindex 2 → 8 (reposition)
├── docs/img/controls/<id>.png  ×≤52                                ← regenerated demonstrative renders
├── docs/roadmap.md, docs/development.md, docs/distribution.md,
│   docs/migration/v2-to-v3.md                                      ← categoryindex renumber (research R6)
└── (Examples *.fsx unchanged at 7; Controls slots immediately below)

Tests:
└── FS.Skia.UI.Build test project  ← failing-first currency/trivial-guard + harness-idempotence tests

AGENTS.md  ← SPECKIT plan reference updated 078 → 079
```

## Phase 0 — Research

See [research.md](./research.md). Resolves: (R1) where the per-control sample source
lives and its shape; (R2) where the committed render harness lives and how it is
invoked deterministically; (R3) how the trivial-content guard is detected in the
SkiaSharp-free build (byte-floor threshold pinned against regenerated assets);
(R4) per-control demonstrative sample content (what each control shows) and
overflow/canvas-size policy; (R5) how the evidence record gains a content
classification; (R6) the exact `categoryindex` renumbering yielding
**Examples → Controls → Guides** while resolving the Examples/Roadmap collision.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — Per-control sample definition, preview asset,
  preview evidence record, trivial-content verdict, and nav-ordering entities, with
  totality/determinism invariants.
- [contracts/preview-samples.contract.md](./contracts/preview-samples.contract.md) —
  P1–P6: single declared sample source; real render-only path; decodable/non-1×1/
  non-trivial; strengthened currency gate; deterministic idempotent regeneration;
  honest unsupported + visible counts.
- [contracts/nav-ordering.contract.md](./contracts/nav-ordering.contract.md) —
  N1–N3: built-nav category order, no file relocation, all cross-links resolve.
- [quickstart.md](./quickstart.md) — the author → re-render → verify loop.
- **Agent context update**: `AGENTS.md` SPECKIT plan reference updated to
  `specs/079-doc-preview-examples/plan.md`.
