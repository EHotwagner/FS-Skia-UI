# Generated Guidance Validation — Feature 083

The canonical `.agents/skills/**` edits for this feature were regenerated into their derived
`.claude/skills/**` mirrors through the single regeneration entry point, so `SkillSyncCheck` /
`GeneratedGuidanceCheck` cannot trip on drift.

- **Authoritative command**: `./fake.sh build -t RefreshSurfaceBaselines` (regenerates the
  `.claude` tree from `.agents`), enforced by `./fake.sh build -t SkillSyncCheck` /
  `./fake.sh build -t GeneratedGuidanceCheck`.
- **Artifact path**: this file; `readiness/skill-sync-check.md` (gate report).
- **Failure class**: governance (stale generated skill mirror).
- **Next action**: if drift is reported, re-run `RefreshSurfaceBaselines` and commit the `.claude`
  mirror alongside the `.agents` source.

## Edited canonical skills + mirror currency

| Canonical (`.agents`) | Mirror (`.claude`) | Contrast/Color content present in mirror |
|------------------------|---------------------|------------------------------------------|
| `fs-skia-design-tokens/SKILL.md` | `fs-skia-design-tokens/SKILL.md` | yes — "Color contrast & the `ContrastCheck` gate" section (T006/T007) |
| `fs-skia-template-update/SKILL.md` | `fs-skia-template-update/SKILL.md` | yes — `FS.Skia.UI.Color` in the props-pinned list and the step-5 feed loop (T025) |

Outcome: mirrors regenerated and current; `RefreshSurfaceBaselines` exited `Ok`.
