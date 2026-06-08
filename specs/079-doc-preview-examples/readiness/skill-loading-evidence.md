# Skill-Loading Evidence — 079-doc-preview-examples

Authoritative command: resolution cross-checked by `./fake.sh build -t EvidenceGraph`
(skillist resolution) and enforced when a task flips to `[X]` by `EvidenceAudit`.
Artifact path: this file. Failure class: `unresolved-skill` / `late-skill-load`.
Next action: load the named `SKILL.md` from its `.agents`/`src` home before the
task's code work begins and record the row here.

All declared skills were resolved and **loaded (read) before any implementation
work began**: `loaded_at = 2026-06-08T11:50:47Z` (resolution + read), strictly
before `work_started_at = 2026-06-08T11:53:29Z` (first code/artifact change).
Skills are cited at their canonical `.agents/skills/<id>/SKILL.md` or
`src/*/skill/SKILL.md` source homes (not the generated `.claude` peers).

| Task | Skill | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|-------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T004 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewSamples.fs | none |
| T004 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewSamples.fs | none |
| T005 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewRender.fs | none |
| T005 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewRender.fs | none |
| T005 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewRender.fs | none |
| T006 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | build/Governance/CatalogDocsGen.fs | none |
| T007 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | specs/079-doc-preview-examples/readiness/visual-evidence-honesty.md | none |
| T008 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewHarnessTests.fs | none |
| T009 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewHarnessTests.fs | none |
| T010 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewSamples.fs | none |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewSamples.fs | none |
| T010 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewSamples.fs | none |
| T011 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | specs/079-doc-preview-examples/readiness/real-image-evidence.md | none |
| T011 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | specs/079-doc-preview-examples/readiness/real-image-evidence.md | none |
| T011 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | specs/079-doc-preview-examples/readiness/real-image-evidence.md | none |
| T013 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | specs/079-doc-preview-examples/readiness/real-image-evidence.md | none |
| T014 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewSamples.fs | none |
| T014 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/ControlsPreview.Harness/PreviewSamples.fs | none |
| T016 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/Governance.Tests/CatalogDocsGenTests.fs | none |
| T016 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | tests/Governance.Tests/CatalogDocsGenTests.fs | none |
| T017 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | build/Governance/Engine/Update.fs | none |
| T017 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | build/Governance/Engine/Update.fs | none |
| T019 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | build/Governance/validation.contract.yml | none |
| T020 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | specs/079-doc-preview-examples/readiness/docs-build.md | none |
| T021 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | specs/079-doc-preview-examples/readiness/docs-build.md | none |
| T022 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | specs/079-doc-preview-examples/readiness/governance-risk-levels.md | none |
| T023 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | specs/079-doc-preview-examples/readiness/evidence-graph.md | none |
| T024 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-08T11:50:47Z | 2026-06-08T11:53:29Z | specs/079-doc-preview-examples/readiness/evidence-audit.md | none |
