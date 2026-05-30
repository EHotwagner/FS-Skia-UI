# Phase 0 Research: Task Validator Feedback Follow-ups

## Decision: Keep title-trigger matching in the evidence graph validator but make it token-aware

**Rationale**: The current validator owns the enforced `CAPABILITY_EXPECTATIONS` table and already reports high-confidence skill omissions. Updating this script keeps enforcement and diagnostics in one place while preserving existing graph checks for cycles, dangling dependencies, mirrors, unreadable skills, and skill ordering.

**Alternatives considered**:
- Remove title-trigger enforcement entirely. Rejected because existing governance expects high-confidence Spec Kit workflow omissions to block validation.
- Move matching into generated task guidance only. Rejected because guidance without enforcement would not preserve current graph protections.
- Add a new parser dependency. Rejected because Python regex/token helpers are sufficient and no dependency change is needed.

## Decision: Treat filenames and longer words as low-confidence unless a complete trigger phrase is present

**Rationale**: The false positive comes from trigger substrings embedded in mandated readiness filenames such as `skill-loading-evidence-workflow.md`. Matching should recognize complete words, dotted commands, exact filenames that are intentionally enforced, and explicit phrases, not arbitrary substrings within hyphenated filenames or longer words.

**Alternatives considered**:
- Whitelist only the reported filename. Rejected because it fixes one feedback case but leaves the same class of false positive.
- Require authors to use the readiness-notes prefix for all setup tasks. Rejected because setup tasks should be valid when they merely cite required evidence paths.

## Decision: Document the readiness-notes prefix and trigger groups in task-generation guidance

**Rationale**: Authors need to discover blocking title phrases before running `EvidenceGraph`. The visible guidance should name the suppression prefix (`Complete readiness notes`), the actual enforced Spec Kit trigger groups, and safe setup wording examples for readiness filenames.

**Alternatives considered**:
- Leave the behavior discoverable through validator source. Rejected because the user story explicitly requires source-free discovery.
- Document only examples. Rejected because examples drift unless tied to the enforced trigger groups.

## Decision: Use declared `name:` values as authoritative skill ids and improve directory/id mismatch diagnostics

**Rationale**: `discover_skills` already resolves `SKILL.md` by declared `name:` and falls back to the directory name only when no name exists. Diagnostics should make that contract visible when an author declares a directory-like id that points at a readable skill whose declared name differs.

**Alternatives considered**:
- Accept both directory and declared names. Rejected because it creates aliases that weaken the single authoritative registry.
- Rename directories to match ids. Rejected because template fragment directories describe capabilities and may intentionally differ from declared skill ids.

## Decision: Keep FS.Skia.UI capability hints advisory

**Rationale**: The feature asks for help choosing rendering, scene, viewer, input, layout, and evidence skills without adding new hard validation friction. Guidance or non-blocking diagnostics can cover common categories while the validator keeps hard failures limited to existing Spec Kit workflow trigger rules.

**Alternatives considered**:
- Add hard validator rules for every FS.Skia.UI capability. Rejected as out of scope and likely to create more false positives.
- Omit capability guidance. Rejected because task authors currently need to infer capability choices from scattered skill files.

## Decision: Label graph-only output at the script/command surface

**Rationale**: `EvidenceGraph` currently runs graph validation only, while `EvidenceAudit` performs merge-gate audit checks after graph validation. Output and generated command reports should make that distinction obvious in one log scan.

**Alternatives considered**:
- Rename the command. Rejected because command identity is already part of repository and generated-product contracts.
- Add audit checks to `EvidenceGraph`. Rejected because it would collapse the intended graph/audit separation.
