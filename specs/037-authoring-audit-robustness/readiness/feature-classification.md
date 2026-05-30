# Feature Classification & Evidence Obligations (T002)

## Tier

**Tier 1 (contracted change).** The single public-contract change is isolated to
`ControlEventOrigin` in `FS.Skia.UI.Controls` (US3): it gains
`[<RequireQualifiedAccess>]`. All other work (US1 audit feature resolution, US2
audit parsing robustness, US4 generated FSI load script) is governance-tooling
and template-generation hardening with no runtime contract impact.

## Affected layers

- **Controls `.fsi` / `.fs`** — `src/Controls/Types.fsi` and `src/Controls/Types.fs`
  (`ControlEventOrigin` qualified access). Surface baselines
  `readiness/surface-baselines/FS.Skia.UI.Controls.txt` and `FS.Skia.UI.txt`.
- **Governance tooling** — `build.fsx` (`activeFeatureId` fail-loud),
  `.specify/extensions/evidence/scripts/python/compute-task-graph.py`
  (task-count echo), `.specify/extensions/evidence/scripts/bash/run-audit.sh`
  (structured-region status parsing), `.specify/scripts/bash/common.sh`
  (`get_feature_paths` resolution parity), the `speckit-evidence-graph` /
  `speckit-evidence-audit` SKILL peers.
- **Template / generation** — `template/base/` (generated `.fsx` load script
  source + guidance), `.template.config/template.json` (generated content),
  `GenerateV3Products` in `build.fsx`.

## Public-contract impact

Yes, scoped to US3 only — `[<RequireQualifiedAccess>]` on `ControlEventOrigin`.
Surface baselines refreshed; spec 035 "guidance over attributes" decision
reversed for this one type and recorded per FR-010. No package identity/version
change. No other public surface changes.

## Elmish/MVU applicability

**Not applicable.** No stateful or I/O-bearing runtime workflow changes (spec:
"State workflow impact: None"). The governance tooling is a batch CLI, not an
Elmish program; the generated FSI load script is static generated text.

## Evidence obligations (from the plan's Evidence Plan)

| Obligation | Evidence |
|---|---|
| US1 correct resolution + true task count | `feature-resolution.md`, `logs/evidence-audit.txt` (resolved id + real task count) |
| US1 unresolved hard-fail | transcript of a run with no resolvable feature → non-zero exit + warning |
| US2 no false block | `audit-fixtures/prose-negation-clean.md` audited → PASS |
| US2 sustained true block | `audit-fixtures/genuine-violation.md` audited → BLOCK |
| US3 mixed-open compile | `fsi/` compile of previously-failing open order |
| US3 surface delta | refreshed `readiness/surface-baselines/FS.Skia.UI.Controls.txt` (+ merged) |
| US4 FSI load | `fsi-load-script.md` + real FSI load transcript for a generated app |
