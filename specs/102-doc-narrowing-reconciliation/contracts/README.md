# Phase 1 Contracts: Documented-Narrowing Reconciliation (R8)

**No contract change.**

R8 alters **no external interface**: no public `.fsi` signature, no surface-area baseline
(cross-package or per-package), no sample contract, no command/target schema, and no
generated-template surface changes under the plan's recorded default choices
(FR-002 → annotate, FR-005 → document).

Why there is no `.fsi` delta even where source is touched:

- **FR-002 (annotate dead `Selected`)** — a source comment only; the
  `deriveVisualState : ControlRuntimeModel -> ControlId -> VisualState` signature is
  untouched. Note: *even if* the optional FR-002(a) removal were taken, the signature would
  still not move (the dead `elif` carries no unique parameter — `model` stays in use), so no
  baseline recapture would be required. FR-002(a) is **not** taken.
- **FR-004 / FR-005 / FR-007** — source comments/annotations in `Layout.fs`, `Focus.fs`,
  `Control.fs`; no signature, no visibility, no type change.
- **FR-001 / FR-003 / FR-006** — roadmap-report prose only; the report is repo documentation,
  not a packaged surface, so it bumps no package version and moves no baseline.

If a future maintainer elects a higher-touch option that *does* move a public surface (not in
R8's recorded scope), they MUST recapture the affected per-package baseline
(`PerPackageSurface.captureCurrent`) and the cross-package baseline, and route Tier-1
accordingly. This feature does not.
