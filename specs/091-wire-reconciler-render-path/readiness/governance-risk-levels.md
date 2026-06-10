# Governance Risk Levels — Feature 091

Governance risk level for this change is **broad** (consumer-contract public
`.fsi` in two packages + a runtime evidence obligation + a `.agents`/`.claude`
skill-tree edit), so the focused validation is the escalated serialized
six-target order plus the recaptured surface baselines.

- **Small** (framework-internal byte-identical): `Dev` + the failing-first
  Feature 091 tests. No broad rerun. Not this change.
- **Medium** (single governance seam): adds `GeneratedGuidanceCheck` /
  `SkillSyncCheck` currency over the regenerated api-surface + skill mirror.
- **Broad** (consumer-contract + evidence obligation + skill-tree): the escalated
  order, recorded **non-authoritatively** in `logs/` with per-target verdicts.
  `GeneratedProductCheck` may fail locally for environment reasons (see
  `runtime-limitations.md`).

Authoritative gates for this change: `Dev` (build + full unit/governance suites,
incl. the Feature 091 recovery/dispatch/text-seam/responds-proof + governance
tests), `PerPackageSurfaceDiff` / `PackageSurfaceCheck` over the recaptured
`FS.Skia.UI.Controls` + `FS.Skia.UI.Controls.Elmish` baselines, `SkillSyncCheck`,
`EvidenceGraph`, and `EvidenceAudit` — all PASS.

## Required evidence per risk level

- **Small** — **required evidence**: `Dev` + the Feature 091 tests.
- **Medium** — **required evidence**: the above plus the currency gates
  (`GeneratedGuidanceCheck`, `PackageSurfaceCheck`) over the regenerated
  api-surface, the per-package `.fsi.txt` snapshots, and the `.claude` skill mirror.
- **Broad validation** — **required evidence**: the escalated six-target order run
  sequentially, recorded non-authoritatively in `logs/`. **Broad validation** is
  required here because the change touches public `src/Controls/**` and
  `src/Controls.Elmish/**` `.fsi`, the emitted `docs/api-surface` tree, and the
  `.agents`/`.claude` evidence skill. The host change is **additive** (a control
  with no authored binding behaves exactly as before), so effective gate coverage
  is preserved.

## SC-001 scope note (host mechanism, not a per-view audit)

**SC-001's "100% of catalog controls"** is a **host-mechanism guarantee** — the
host dispatches *any* authored binding universally — proven on the representative
sample per FR-005a (one leaf-keyed `onClick`, one container-keyed composite, one
focused text control). It is **not** a per-view audit of all 52 typed
`Widgets/*.fs` views; a per-control "typed view exposes no binding" gap is flagged
to a separate fitness pass, not fixed catalog-wide here.
