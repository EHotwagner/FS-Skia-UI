# Compatibility Impact Evidence

- Status: Codex workflow compatibility preserved while adding Claude Code project-local artifacts.
- Codex artifacts: existing `AGENTS.md`, `.agents/skills/**`, generated Codex instructions, and template Codex outputs remain governed.
- Claude artifacts: repository and generated project `CLAUDE.md`, `.claude/settings.json`, `.claude/skills/**`, and supported hook paths are generated or validated from the shared source model.
- Runtime behavior: no FS.Skia.UI renderer, layout, input, controls, sample, or application runtime behavior is intentionally changed.
- Unsupported scope: user-local Claude preferences, managed enterprise deployment, release publishing, browser/mobile behavior, and product UI changes remain out of scope.
- Validation path: `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceAudit`, and `Verify` check compatibility guidance, drift diagnostics, audit pattern coverage, and broad readiness.

