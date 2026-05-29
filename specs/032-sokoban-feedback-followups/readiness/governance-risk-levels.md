# Governance Risk Levels

- Feature: `032-sokoban-feedback-followups`
- Branch: `032-sokoban-feedback-followups`
- Tier: medium, because the change touches Testing public helpers, generated guidance, Spec Kit task guidance, and generated validation.
- Small risk level: documentation-only or single-test updates can use focused package or governance tests.
- Medium risk level: public helper plus guidance changes require focused package tests, governance tests, and generated guidance validation.
- Broad risk level: template generation, generated product behavior, or cross-package contracts require the full serialized FAKE order.
- Required evidence: package tests, guidance scans, generated product checks, graph output, and audit output are recorded under this readiness directory.
- Broad validation: run `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, and `EvidenceAudit` sequentially before final readiness.
- Public API: `FS.Skia.UI.Testing.DefaultTextGlyphEvidence` was added in `src/Testing/Testing.fsi`; Scene and SkiaViewer public surfaces were reviewed and reused.
- MVU/effect applicability: persistent close remains covered by existing SkiaViewer `Viewer.init` / `Viewer.update` pure close effects and generated host boundary guidance. Default text glyph validation is a pure screenshot artifact check, so no MVU shell is needed.
- Synthetic-evidence policy: no task is marked `[S]`; negative classifier tests use generated PNG artifacts to exercise validation branches and do not replace the real supported-host readiness path.
- Runtime limitations: supported-host default text readability depends on a decodable PNG screenshot from the viewer capture path; unsupported hosts must record `unsupported-host-reason` without claiming readability.
- Aggregate hang diagnostics: FAKE-backed commands are serialized. Any race-like failure must be rerun sequentially before product debugging.
- Serialized FAKE order: `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`.
