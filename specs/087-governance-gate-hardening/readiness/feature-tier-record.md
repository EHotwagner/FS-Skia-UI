# Feature tier record (T003) — feature 087 Governance Gate Hardening

- **Tier**: 1 — governance-contract level. The change escalates to the
  `maintainer-verify` / broad-validation path because it touches governance paths
  (`build/Governance/**`), the generated `template/base/docs/evidence-formats.md`,
  and regenerates `validation.contract.yml` from `Routing.fs`.
- **Affected layer**: `build/Governance/**` only — `FS.Skia.UI.Build` engine
  (`Engine/Model.fs`, `Engine/Update.fs`, `Evidence/{Graph,TaskParser,Audit,
  EvidenceFormatSchema,Render}.fs`, `Front/Governance.fs`, `PerPackageSurface.fs`,
  `Routing.fs`) plus `tests/Governance.Tests/`. **No `src/**/*.fsi` change.**
- **Public-API impact**: none. No `src/**` public surface changes; the changed
  *contracts* are governance-internal and generated from single sources
  (`validation.contract.yml` from `Routing.fs`; the audit verdict schema; the
  skill-loading-evidence row; `docs/evidence-formats.md`), currency-checked by
  `TargetMetadataDrift` / `SkillSyncCheck`.
- **Elmish/MVU applicability (Principle IV)**: satisfied by keeping the verdict
  (`Audit.verdict`), `[S*]` propagation (`Graph.propagate`), and skew comparison
  **pure** functions over `tasks.md` / `tasks.deps.yml` / `readiness/` / surface
  baselines. Only FR-001 feature-context provisioning and the green-run I/O live at
  the existing interpreter edge (`Front/Governance.fs`). No new public
  `Model`/`Msg`/`Effect` surface, no new effect algebra.
- **Synthetic evidence**: none expected. Seeded defect/skew/deferral/violation
  inputs are *real* inputs to pure governance functions (exercised through
  `Governance.Tests` + FAKE runs), not synthetic substitutes for a missing
  capability. If a seeded error-path input proves infeasible to produce really, it
  is disclosed per Principle V at task time, not relabeled at implementation time.
- **Required evidence obligations** (from plan.md, all under `readiness/`):
  - `generated-product-check-green.txt` — clean-tree `GeneratedProductCheck` green (FR-001/002, SC-001).
  - `generated-product-defect-classification.txt` — seeded defect + env obstacle → product-defect verdict (FR-002, SC-002).
  - `package-skew-seeded.txt` / `package-skew-clean.txt` — skew check fails on seeded unpinned-API ref, passes on real tree, no restore (FR-003/004, SC-003/004).
  - `refresh-surface-baselines-idempotent.txt` — `RefreshSurfaceBaselines` twice, `git status` clean (FR-005/006, SC-005/006).
  - `audit-three-verdicts.txt` + `seh-audit-summary.json` — three seeded inputs → three verdicts (FR-007/008, SC-007).
  - `synthetic-propagation-no-phase-edge.txt` — leaf `[S]` → zero phase-edge-only `[S*]` (FR-009, SC-008).
  - `skill-loading-evidence-provenance.md` + at-implementation gap report (FR-010, SC-009).
  - `true-positive-gates-still-block.txt` — diff-scan / additive-surface / window-visibility / persistent-launch / synthetic-honesty still block (FR-011, SC-010).
- **Validation path**: escalated serialized six-target order (`Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
  `EvidenceGraph` → `EvidenceAudit`), FAKE-backed targets run sequentially.
