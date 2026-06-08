# Feature Tier & Evidence Obligations — Feature 083

- **Tier**: Tier 1 (contracted change). New public package + `.fsi` surface, new build gate, new
  internal dependency edge, new template pin → escalates to the serialized maintainer-verify
  six-target path.
- **Affected layers**:
  - NEW packable library `FS.Skia.UI.Color` (`src/Color/**`) — pure WCAG contrast + Radix ramps.
  - `FS.Skia.UI.Build` governance — `ContrastCheck` target + gate (`ContrastGate.fs[i]`), routing
    (`controls-public-surface` + new `color-contrast`), `knownGates`, `PerPackageSurface` scope.
  - `src/Controls` design-token **values** (DTCG source + regenerated `DesignTokens.fs`).
  - `template/base/Directory.Packages.props` (new pin) + `fs-skia-template-update` skill set.
- **Public-API impact**: NEW `src/Color/*.fsi` surface + new per-package baseline. `DesignTokens.fsi`
  surface is UNCHANGED (no new token name; `contrastRequiredRatio` already exists) — only generated
  *values* change. Additive, no compatibility break.
- **MVU applicability**: N/A. Pure, stateless computation (luminance, ratio, verdict) + static
  palette data + a pure gate core. No `Model`/`Msg`/`Effect`, no subscriptions, no interpreter.
  The only filesystem read (the gate loading generated token values) lives at the existing
  `Engine/Interpret.fs` edge and is exercised by the live gate run (Principle IV satisfied by
  non-applicability + edge isolation).
- **Evidence obligations**:
  - `tests/Color.Tests` (reference pairs SC-002, ramp invariant SC-003, verdict thresholds SC-004).
  - `tests/Governance.Tests/Feature083GovernanceTests.fs` (routing/known-gates FR-011, gate
    regression SC-005).
  - Live `ContrastCheck` gate (SC-001) → `readiness/color-contrast-evidence.md`.
  - FSI transcript against the built surface → `readiness/fsi-session.txt`.
  - New per-package baseline `readiness/per-package-surface/FS.Skia.UI.Color.fsi.txt`.
  - Regenerated `DesignTokens.fs`, `validation.contract.yml`, `.claude` skill mirror.
  - The escalated six-target evidence set (Dev, GeneratedGuidanceCheck, TemplateCheck,
    GeneratedProductCheck, EvidenceGraph, EvidenceAudit).
- **Synthetic evidence**: none planned. WCAG reference constants and the SC-005 poisoned-token
  test input are reference/test inputs, not synthetic substitutes for unavailable evidence.
