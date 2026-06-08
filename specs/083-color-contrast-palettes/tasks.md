# Tasks: Accessible Color Contrast & Palettes

**Feature branch**: `083-color-contrast-palettes`
**Spec**: `specs/083-color-contrast-palettes/spec.md`
**Plan**: `specs/083-color-contrast-palettes/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

No `[SEH]` tasks are present. The SC-005 regression scenario injects a
sub-threshold token as a deliberate **test input**, not as a synthetic
substitute for unavailable real evidence (plan: "Synthetic evidence: None
planned"), so it does not qualify for `[SEH]`. implementation-time relabeling is
forbidden.

## Vertical-slice rule (US phases)

A `[US*]` task may be `[X]` only when the change is reachable from a
user-facing entry point that was actually exercised. For this library feature
the entry point is an FSI session against the **packed** `FS.Skia.UI.Color`
surface (or the live `ContrastCheck` gate run), with the transcript / report
captured under `readiness/`. Core/arithmetic changes alone do not satisfy
`[X]` for a `[US*]` task.

**MVU/Principle IV note**: This feature is pure, stateless computation
(luminance, ratio, verdict) plus static palette data and a pure gate core. There
is **no** `Model`/`Msg`/`Effect`, no I/O-bearing workflow, no subscriptions, and
no interpreter. Principle IV's MVU evidence obligation is therefore **not
applicable**; the only filesystem read (the gate loading generated token values)
lives at the existing engine edge and is exercised by the live gate run.

## Success-criterion → assertion mapping

- **SC-002** (WCAG reference values) → `tests/Color.Tests` reference-pair asserts
  (black/white = 21:1, white/white = 1:1, tol 0.01) — T011.
- **SC-003** (matched light/dark ramps, ≥1 AA text/bg pair per family) → ramp
  invariant test — T020.
- **SC-004** (ratio + verdict in one call; per-role thresholds) → verdict tests
  + packed-surface FSI proof — T023, T024.
- **SC-001** (gate passes on shipped themes) → live `ContrastCheck` enforcement — T018.
- **SC-005** (poisoned token fails with pairing/measured/required) → gate
  regression test — T013.
- **SC-006** (single-source consistency, no drift) → `TargetMetadataDrift` /
  `DesignTokenDrift` / `SkillSyncCheck` via the six-target path — T028.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- **[T1]** — Tier 1 (contracted) change; the whole feature is Tier 1, so the
  per-task tier annotation is omitted (it matches the spec tier).

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors its
structured `skillist` value via `[skillist: ...]` (`[skillist: []]` when empty).

## Governance risk level

This is an **escalated / broad** consumer-contract change (new public package +
`.fsi` surface, new build gate, generated token-value changes). It routes through
the serialized maintainer-verify six-target path. Focused validation for the
change is the `ContrastCheck` gate plus the per-package surface diff; broad
validation (the full six-target order) is required because the change touches
`template/**`, governance routing, and a new public surface. FAKE-backed targets
run **sequentially**; non-authoritative aggregate results are recorded in
`readiness/governance-risk-levels.md` and `readiness/aggregate-hang-diagnostics.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the `083-color-contrast-palettes` branch and link spec, plan, research, data-model, contracts, and quickstart as the working set
- [X] T002 [P] [skillist: fs-skia-scene] Scaffold the new packable project `src/Color/Color.fsproj` (`net10.0`, `IsPackable=true`, `PackageId=FS.Skia.UI.Color`, one `ProjectReference` to `src/Scene/Scene.fsproj`) and add it to the solution
- [X] T003 [P] [skillist: []] Create audit-discoverable readiness scaffolds in `readiness/`: `color-contrast-evidence.md` (placeholder), `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md` — each naming its authoritative command, artifact path, failure class, and next action
- [X] T004 [skillist: []] Record feature Tier (Tier 1 contracted), affected layers (new `FS.Skia.UI.Color` package, `FS.Skia.UI.Build` governance, `src/Controls` token values), public-API impact (new `.fsi`; `DesignTokens.fsi` surface unchanged), MVU applicability (N/A — pure/stateless, Principle IV not applicable), and evidence obligations

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-scene] Draft the public `src/Color/Contrast.fsi` and `src/Color/Palettes.fsi` signatures (Role/Verdict/ContrastResult; `Contrast.relativeLuminance/ratio/compositeOver/verdict/check/checkPaint`; `Palettes.StepRole/RampVariant/PaletteStep/PaletteRamp/all/ramp/families`) — no `Model`/`Msg`/`Effect` (pure surface)
- [X] T006 [P] [skillist: fs-skia-design-tokens] Add contrast guidance to the canonical `.agents/skills/fs-skia-design-tokens/SKILL.md` (how to measure contrast, choose ramp values, and interpret/cure `ContrastCheck` failures) and regenerate the `.claude` mirror through the existing sync path (FR-012)
- [X] T007 [P] [skillist: fs-skia-design-tokens] Document the `ContrastCheck` gate contract: the explicit, role-tagged validated-pairing set, the text-vs-graphic threshold selection (`contrastRequiredRatio` vs fixed 3:1), the alpha-compositing and alias-resolution rules, and the `readiness/color-contrast-evidence.md` report format (FR-007, FR-009)
- [X] T008 [skillist: fs-skia-scene] Exercise the draft `.fsi` from FSI per `quickstart.md` (representative `ratio`/`check`/`ramp` calls) and capture the session transcript to `readiness/fsi-session.txt`
- [X] T009 [skillist: []] Add `FS.Skia.UI.Color` to `PerPackageSurface.packagesInScope` and reserve the new per-package baseline path `readiness/per-package-surface/FS.Skia.UI.Color.fsi.txt` (a new package with no baseline is never treated as clean)
- [X] T010 [skillist: fs-skia-evidence-mode] Record unsupported-scope handling and failure diagnostics in `readiness/runtime-limitations.md`: non-solid paints → `Indeterminate` (visible exclusion, never silently passed), and the fail-loud `ContrastCheck` message shape (token names, resolved colors, measured/required ratio, theme, role)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 — Themes are guaranteed legible (US1) [P1]

### Tests First (Principle I, Principle VI)

- [X] T011 [P] [US1] [skillist: fs-skia-scene, fsharp-build-orchestration] Add failing-first Expecto tests in `tests/Color.Tests` for WCAG reference pairs — `ratio` black-on-white ≈ 21.0 and white-on-white = 1.0 within 0.01, plus `relativeLuminance` spot values (SC-002)
- [X] T012 [P] [US1] [skillist: fsharp-build-orchestration] Add a `tests/Governance.Tests` case asserting `ContrastCheck` is in `AgentValidation.knownGates` and is routed for `src/Controls/**` and `src/Color/**` changes (FR-011)
- [X] T013 [P] [US1] [skillist: fs-skia-design-tokens] Add a gate-level regression test (SC-005): a token value dropped below threshold makes `ContrastCheck` fail naming the pairing, measured ratio, and required ratio; restoring an accessible value makes it pass

### Implementation

- [X] T014 [US1] [skillist: fs-skia-scene] Implement `src/Color/Contrast.fs` against `Contrast.fsi`: `relativeLuminance` (sRGB linearization + 0.2126/0.7152/0.0722), `ratio` ((Llight+0.05)/(Ldark+0.05)), `compositeOver` (deterministic source-over for alpha), `verdict` (role→threshold; `Decorative` → `Exempt`), `check`, and `checkPaint` (solid → measured; non-solid → `Indeterminate` with `Ratio = nan`) (FR-001, FR-001a, FR-002, FR-003, FR-004, FR-004a)
- [X] T015 [US1] [skillist: fs-skia-design-tokens, fsharp-parsing] Implement `build/Governance/ContrastGate.fs`/`.fsi`: the explicit documented `ValidatedPairing` set, resolve foreground/background token names to `Color` from the generated Light/Dark tokens (alias-resolved, alpha-composited over `background`), measure, select the threshold (Text→`contrastRequiredRatio`, GraphicOrUi→3.0, Decorative recorded-not-enforced), and emit `PairingOutcome` rows — pure core with the token load at the existing engine edge (FR-007, FR-008, FR-009)
- [X] T016 [US1] [skillist: fsharp-build-orchestration, fsharp-code-generation] Register the gate through the single-source path: add `ContrastCheck` to the `Targets` union, `allTargets`, and the `name`/`directPrerequisites`/`spec` arms; add it to `AgentValidation.knownGates`; append it to the `controls-public-surface` routing rule and add a new `color-contrast` rule for `src/Color/**`; regenerate `validation.contract.yml` from `Routing.fs` (FR-011)
- [X] T017 [US1] [skillist: fs-skia-design-tokens, fsharp-code-generation] Bring the failing shipped Light/Dark token values into conformance — edit only the failing `$value`s in `src/Controls/design-tokens.tokens.json` (drawing replacements from the new ramps), regenerate `DesignTokens.fs` via `RefreshSurfaceBaselines`, and confirm `DesignTokenDrift` currency; leave conforming tokens byte-unchanged (FR-010)
- [X] T018 [US1] [skillist: fs-skia-design-tokens] Run `./fake.sh build -t ContrastCheck` on the shipped themes, write `readiness/color-contrast-evidence.md` with every per-pairing row (both themes, measured vs required, pass/fail), and confirm PASS (SC-001)
- [X] T019 [US1] [skillist: []] Document US1's independent validation path (poison-a-token → gate fails → restore → gate passes) in `readiness/color-contrast-evidence.md`

**Checkpoint**: User Story 1 — themes pass the gate and regression protection is demonstrable.

---

## Phase 4: User Story 2 — Ready-made accessible palettes (US2) [P2]

### Tests First

- [X] T020 [P] [US2] [skillist: fs-skia-scene, fsharp-build-orchestration] Add a failing-first ramp invariant test: every offered family has a matched `Light` and `Dark` ramp, and at least one documented `Text`-step over a documented background-step measures ≥ 4.5:1 under `Contrast.ratio` (SC-003)

### Implementation

- [X] T021 [P] [US2] [skillist: fs-skia-scene] Implement `src/Color/Palettes.fs` against `Palettes.fsi`: Radix-derived, role-labelled ramps as literal `Color` steps with matched light/dark variants and `all`/`ramp`/`families`; record Radix MIT attribution in the package and skill (FR-005, FR-006)
- [X] T022 [US2] [skillist: fs-skia-scene] Exercise the ramps from a packed-library FSI session (select a text step + background step from one family, confirm AA) and append the transcript to `readiness/fsi-session.txt` (US2 vertical slice)

**Checkpoint**: User Story 2 — ramps exist, are matched light/dark, and a documented pair meets AA.

---

## Phase 5: User Story 3 — Validate any color pair programmatically (US3) [P3]

### Tests First

- [X] T023 [P] [US3] [skillist: fsharp-build-orchestration] Add failing-first verdict tests for the per-role thresholds: `Text` → AAA ≥7 / AA ≥4.5 / AA-Large ≥3 / Fail; `GraphicOrUi` → Aa ≥3 / Fail; `Decorative` → `Exempt` for **any** ratio; `checkPaint` non-solid input → `Verdict = Indeterminate` with `Ratio = nan` (the documented `System.Double.NaN` not-applicable sentinel); and `checkPaint` on a **solid** paint → a measured ratio with no render pass (declared-fill capability, FR-001a) (SC-004, FR-003, FR-004a)

### Implementation / Proof

- [X] T024 [US3] [skillist: fs-skia-scene] Exercise the packed `FS.Skia.UI.Color` surface from FSI — obtain a ratio and an AA/AAA verdict in one `Contrast.check` call, replay the reference pairs and role thresholds — and append the consumer transcript to `readiness/fsi-session.txt` (SC-004 vertical slice)

**Checkpoint**: User Story 3 — a consumer gets ratio + verdict in a single call against the packed surface.

---

## Phase 6: Integration & Polish

- [X] T025 [skillist: fs-skia-template-update] Add the `FS.Skia.UI.Color` pin at `$(FsSkiaUiVersion)` to `template/base/Directory.Packages.props` and update the `fs-skia-template-update` expected package set; `TemplateCheck` / `GeneratedProductCheck` revalidate the new pin (FR-013)
- [X] T026 [skillist: []] Refresh the per-package surface baseline `readiness/per-package-surface/FS.Skia.UI.Color.fsi.txt` (Tier 1) from the current `FS.Skia.UI.Color` surface and confirm no surface drift
- [X] T027 [skillist: fs-skia-design-tokens] Verify the `.claude/skills/fs-skia-design-tokens/**` mirror is regenerated and carries the contrast guidance (`SkillSyncCheck` / `GeneratedGuidanceCheck`); record the outcome in `readiness/generated-guidance-validation.md`
- [X] T028 [skillist: fsharp-build-orchestration] Run the escalated serialized order sequentially — `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck` — and record the governance risk level and non-authoritative aggregate results in `readiness/governance-risk-levels.md` and `readiness/aggregate-hang-diagnostics.md`
- [X] T029 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, and no `[S*]` surprises; record `readiness/evidence-graph.md`
- [X] T030 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS or document every `--accept-synthetic` override; record `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
