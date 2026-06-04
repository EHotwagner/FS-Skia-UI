# Target Metadata — 063-lunar-lander-consumer-friction-followups

## Feature classification (T003)

- **Tier**: **Tier 1**, driven by **FR-010** (new public `FS.Skia.UI.SkillSupport.Wrap`
  `.fsi` surface + new per-package surface baseline line
  `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt`) and **FR-003** (a new
  `SymbolCrossCheck` governance target + `knownGates` entry → `TargetMetadataDrift`). The
  renderer fix (FR-001/002) is Tier-2 internal — the shared `SceneRenderer` module is
  **non-public**, so SkiaViewer's surface baseline does not change — but it is
  consumer-observable evidence output, so it ships with regenerated image evidence.
- **Affected layers**: `src/SkiaViewer/**` (new non-public shared `SceneRenderer.paintNode`
  + `drawScene`/`drawScreenshotScene` delegation), `src/SkillSupport/**` (new `Wrap`
  module), `build/Governance/**` (new `SymbolCrossCheck` target + `Render.fs`
  readiness-diagnostic relabel), Spec Kit phase skills
  (`.agents/skills/speckit-{implement,plan,specify,analyze}/SKILL.md`,
  `fs-skia-{scene,layout-readability,evidence-mode}` → regenerated `.claude/**`), generated
  docs (`template/base/docs/scaffold-map.md`).
- **Public-API impact**: **FR-010 only**. No SkiaViewer `.fsi` change (shared painter is
  non-public). No framework/consumer DU case renamed.
- **Elmish/MVU applicability (Principle IV)**: **N/A** — no framework
  `Model`/`Msg`/`Effect`/`init`/`update`/interpreter added or changed. The renderer is a
  pure draw walk; `wrapDeltaX` is a pure helper for a consumer's `update`.
- **Required evidence obligations**: `target-metadata.md`, `agent-ready-verdict.md`,
  `skill-loading-evidence.md`, `aggregate-hang-diagnostics.md` (escalated tier);
  `renderer-image-evidence.md` (FR-001/002); `symbol-cross-check.md` (FR-003); the `Wrap`
  unit-test output + updated per-package baseline (FR-010); `evidence-path-token-scan.md`
  (FR-008 disposition).

## Route output (T004)

`./fake.sh build -t Route` is **authoritative on the actual diff** — re-run after each
change-set. Captured runs:

### Spec/readiness/docs-only diff (start of implementation)

```
developer-class=framework-author
tier=agent-ready
gates=Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=evidence-governance, specify-catchall, docs-only
```

### Full implementation diff (re-run in Phase 8, all change-sets landed)

```
developer-class=framework-author
tier=agent-ready
gates=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck,
      GeneratedProductCheck, GeneratedGuidanceCheck, SkillSyncCheck, SkillQualityCheck,
      SkillContractPathCheck, TemplateUpdateSkillPackageCheck, TemplateDrift, EvidenceGraph,
      EvidenceAudit
matched-rules=generated-template, evidence-governance, specify-catchall, docs-only,
              package-surface, skill-quality
```

Escalated exactly as planned: `package-surface` (FR-010 `.fsi` surface),
`generated-template` + `skill-quality` (the `.agents/**` skill edits + scaffold-map doc),
`evidence-governance` (the renderer/governance change-sets). `TargetMetadataDrift` is pulled
by the new `SymbolCrossCheck` target. Every printed gate was run individually (see table).

## Per-target verdicts (Phase 8, serialized order)

Stamped gate-by-gate (each FAKE target run individually and sequentially; the authoritative
merge verdict is `EvidenceAudit verdict=PASS`):

| Gate | Verdict | Notes |
|---|---|---|
| `RefreshSurfaceBaselines` | **PASS** | regenerated `.claude` from `.agents` + the SkillSupport surface baseline (Wrap added). |
| `SkillSyncCheck` | **PASS** | `.claude` tree in sync with `.agents`. |
| `SkillQualityCheck` | **PASS** | edited skills meet the quality rubric. |
| `TargetMetadataDrift` | **PASS** | registry currency green after adding the `SymbolCrossCheck` target (38 rows). |
| `PerPackageSurfaceDiff` | **PASS** | zero drift; `FS.Skia.UI.SkillSupport` baseline adds the `Wrap` module. |
| `SymbolCrossCheck` (new) | **PASS** | prints + writes `readiness/symbol-cross-check.md`; seeded-drift verification confirmed. |
| `Dev` | **PASS** | full build + all test projects green (`Test` Success): SkiaViewer 51, SkillSupport (incl. 6 Wrap), Governance 449, renderer 3. |
| `GeneratedGuidanceCheck` | **PASS** | implement/plan/specify/analyze guidance tokens intact after the skill edits. |
| `TemplateCheck` | **PASS** | generated projects ship the faithful image-evidence renderer, the regenerated phase skills + `evidence-formats`/`scaffold-map` pointers, and the `wrapDeltaX` helper (TemplatePack/InstallSource/InstallPackage/Instantiate/Smoke all green). |
| `GeneratedProductCheck` | **EXPECTED-FAIL (non-regression)** | the generated product's `Dev` + `GeneratedGuidanceCheck` + `TemplateDrift` all **completed** (the fixes compile and pass in the generated project); the run fails only at the evidence-graph step because a **feature-less scaffold has no `feature_directory`** (no `/speckit.specify` was run). This is the documented 059-onward non-regression, not a defect in this feature's changes. The aggregate is **non-authoritative**; the authoritative verdict is `EvidenceAudit verdict=PASS`. |
| `PackageSurfaceCheck` | **PASS** | aggregate package surface stable. |
| `FsiTranscripts` | **PASS** | FSI transcripts regenerated. |
| `SkillContractPathCheck` | **PASS** | every skill-claimed `docs/api-surface/...fsi` path resolves. |
| `TemplateUpdateSkillPackageCheck` | **PASS** | template-update skill package set matches the packable `.fsproj` set. |
| `TemplateDrift` | **PASS** | no template drift. |
| `EvidenceGraph` | **PASS** | no cycles, no dangling refs, `[S*]=0`; effective DAG written to `task-graph.md`. |
| `EvidenceAudit` | **PASS** | `verdict=PASS`, `real-tasks=39`, `total-blockers=0`, `diff-scan-hits=0`, `auto-synthetic-tasks=0`. The authoritative merge verdict. |

**Every escalated Route gate was run individually and passed** (only `GeneratedProductCheck`
is the documented expected-fail non-regression). Merge verdict: `EvidenceAudit verdict=PASS`.

> **Note (gotcha recorded):** the renderer before/after capture was initially named
> `real-image-evidence.md`, which is a **reserved window-visibility evidence marker** (used
> by feature 019-fix-window-visibility). Naming it that falsely triggered the
> `windowVisibility` readiness scan (requiring `interactive-visible-window.md` /
> `window-state-diagnostics.md` for a headless, raster-only feature). Renamed to
> `renderer-image-evidence.md` — this feature launches no window, so window-visibility
> evidence would be fabricated.

Aggregate `Verify`/`Ci` umbrella results are **non-authoritative** — see
`aggregate-hang-diagnostics.md` (regenerated by the `Dev` target). The authoritative
verdict is the per-target gate above.
