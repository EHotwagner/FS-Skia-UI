# Governance Notes

- recorded_at: `2026-05-28T15:35:35+02:00`
- tier: Tier 1 contracted governance and generated-consumer behavior change.
- public_surface: No planned `.fsi` public API change. This feature changes generated command behavior, generated template files, docs, tests, and evidence scripts.
- mvu_applicability: Principle IV is not applicable to normal generated runtime changes because interactive app state and gameplay reducers remain out of scope. Evidence command execution is command-edge I/O.
- synthetic_restrictions: Synthetic evidence is limited to design-approved malformed-input and explicit error-path tasks marked `[SEH]`; authoritative success evidence must come from real command output, not success-only placeholders.
- required_readiness_paths: `generated-validation-authority.md`, `skill-loading-evidence-workflow.md`, `audit-diagnostics.md`, `readiness-contract-discovery.md`, `framework-guidance.md`, `evidence-vocabulary.md`, `evidence-graph.md`, and `evidence-audit.md`.
