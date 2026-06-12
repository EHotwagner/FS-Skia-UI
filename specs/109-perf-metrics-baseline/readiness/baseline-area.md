# In-repo baseline area & deterministic-evidence honesty note (feature 109, T003)

The non-golden timing/allocation baselines live under `docs/reports/_baselines/`:

- `2026-06-12-controls-corpus-before.md` — the hover/pointer-move burst with coalescing OFF.
- `2026-06-12-controls-corpus-after.md` — the same burst with coalescing ON (current path).

## Honesty note: counts gate, timing informs

Timing (`TimingMs`) and allocation (`AllocatedBytes`) are environment-dependent, human-facing numbers
and NEVER gate. The gating surface is the deterministic count/boolean goldens under
`readiness/perf-corpus/`. Regression thresholds are defined **counts-first, timing-second** (FR-018):
a regression is a change in the count/boolean golden; timing/allocation only inform. No timing or
allocation field appears in any deterministic golden (SC-009). The before/after pair evidences the
feature-108 coalescing benefit rather than asserting it (FR-019/SC-007).
