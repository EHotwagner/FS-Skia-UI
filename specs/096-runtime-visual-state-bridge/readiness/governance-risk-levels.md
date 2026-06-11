# Governance risk levels — feature 096 (runtime visual-state bridge, T004)

This Tier-1 change moves public surface (`src/Controls/ControlRuntime.fsi` gains one additive
`val deriveVisualState`), so `./fake.sh build -t Route` escalates it to the **controls-public-surface**
rule. Run `Route` first and run exactly the gates it prints.

## small

The pure `deriveVisualState` precedence, the `applyRuntimeVisualState` tree walk, and the four widened
geometry functions (`slider`/`text-box`/`radio-group`/`switch`).
- required evidence: targeted `Controls.Tests` — precedence (T010), byte-identity-at-rest (T012),
  widened-kind restyle/parity (T015/T023), the FsCheck precedence property (T022).
- gate: `./fake.sh build -t Dev`.

## medium

The `renderRetained` host wiring + the live focus-survival / responds path.
- required evidence: `Elmish.Tests` live-retained suites — focus-survives-reshuffle (T018) and the
  responds-proof (T020), driven through `RetainedRender.init`/`step` + the real bridge.
- gate: `./fake.sh build -t Dev` (Elmish.Tests) + the captured `responds-proof.md`.

## broad

The public `ControlRuntime.fsi` surface move (controls-public-surface escalation).
- required evidence: recaptured surface baselines (controls-public-surface + per-package +
  cross-package), and the contrast result.
- broad validation: the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck →
  GeneratedProductCheck → EvidenceGraph → EvidenceAudit` path **plus `ContrastCheck`**. FAKE-backed
  targets run **sequentially** (shared `.fake` state); aggregate results are recorded as
  **non-authoritative** unless re-confirmed sequentially (see `aggregate-hang-diagnostics.md`).

authoritative-gate-list=Dev, ContrastCheck, GeneratedGuidanceCheck, TemplateCheck/TemplateDrift, GeneratedProductCheck, EvidenceGraph, EvidenceAudit
