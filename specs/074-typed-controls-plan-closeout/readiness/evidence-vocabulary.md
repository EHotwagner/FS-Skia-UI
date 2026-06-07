# Evidence Vocabulary — Typed-Controls Plan Closeout (074)

Status legend used in `tasks.md` (five-state):

- `[ ]` — pending.
- `[X]` — done with **real** evidence. For this feature, "real" = the authoring artifact exists,
  was regenerated where applicable, the governance gates pass, and the per-story independent
  reading test passes.
- `[S]` — done with synthetic evidence only (mock/stub/fake/placeholder). **Not used** by this
  feature.
- `[S*]` — computed (not written): a task whose dependency is `[S]`/`[S*]` and would otherwise be
  `[X]`. **None** here, since there is no `[S]`.
- `[F]` — failed. **None.**
- `[-]` — skipped (with rationale). **None.**

Other terms:

- **non-authoritative aggregate** — a multi-target / generated-product run whose failure is an
  environment artifact, not a product regression (here: `GeneratedProductCheck`). The
  authoritative verdict is each focused gate's own result.
- **skill currency** — the `.agents` canonical source and its generated `.claude` peer are
  byte-in-sync (`SkillSyncCheck` zero drift).
- **disposition** — the recorded status of the `Reconcile` module: `module internal`,
  property-tested, deliberately unwired, parked.

This feature introduces **no** synthetic, `[SEH]`, window-visibility, or accepted-synthetic
vocabulary — none of those classes is engaged.
