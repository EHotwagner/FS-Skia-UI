# Contract: Spec-Kit Skill Guidance (VERIFY-IMPL-1, CLARIFY-SOURCE-1)

The interface is the **durable skill guidance** present in BOTH the canonical `.agents` source and the
regenerated `.claude` mirror, held byte-identical by `SkillSyncCheck`. Both skills are excluded from
`SkillQualityCheck`'s section rubric, so the edits are free-form prose with no mandatory headings.

## C1 — speckit-implement run-and-use gate (FR-005, FR-006, FR-007)

`speckit-implement/SKILL.md` MUST, in its per-task workflow, require for any **interactive-UI** user
story — before the story is marked done (`[X]` on `[US*]`):

1. an explicit **run-and-use** step — launch the host app and interact with it (pointer/keyboard) via
   the `run`/`verify` skill discipline (FR-005); tests + gates + offscreen captures alone are
   insufficient;
2. confirmation that the captured **evidence exercised the production render path** — the real
   user-reachable surface (`controlsExampleView` → `Control.renderTree`), not a bespoke author-built
   parallel scene (FR-006); a truthful screenshot of the **wrong** render path does NOT count;
3. no-op for non-interactive stories.

The guidance MUST be durable (applies to every future interactive-UI feature) and present in both the
`.agents` source and the `.claude` mirror (FR-007).

**Acceptance (SC-003):** inspecting the skill confirms an interactive `[US*]` cannot be marked done
without the recorded run-and-use step on the production path; a bespoke-placeholder-scene build is
rejected by this discipline rather than accepted.

## C2 — speckit-clarify source-spec pre-check (FR-010, FR-011)

`speckit-clarify/SKILL.md` MUST include a step: when a `source-spec.md` snapshot exists in the feature
directory (`FEATURE_DIR`), **consult it before forming clarification questions** and do not ask what
the snapshot already resolves; when absent, the step is a silent **no-op** (FR-011 graceful
degradation). Present in both `.agents` source and `.claude` mirror.

**Acceptance (SC-005):** running `/speckit-clarify` on a feature whose directory contains a
`source-spec.md` produces no question already answered by that snapshot; on a feature without one it
behaves exactly as today.

## C3 — currency (FR-007, FR-011, SC-006)

After editing the `.agents` sources, `RefreshSurfaceBaselines` regenerates the `.claude` mirrors;
`SkillSyncCheck` MUST pass (byte-identical), confirming both items are present in both trees.
</content>
