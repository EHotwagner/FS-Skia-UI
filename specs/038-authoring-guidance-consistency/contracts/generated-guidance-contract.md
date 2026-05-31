# Contract: Consumer-Facing, Domain-Agnostic Generated Guidance (US4)

**Satisfies:** FR-005, FR-006, FR-007 · SC-004

## Rules

1. **No framework-only references (FR-005).** Generated guidance MUST NOT direct
   authors to API references, paths, or build targets absent from a generated
   consumer project (e.g. `src/.../X.fsi`, `CapabilityCheck`, `PackLocal`).
2. **Consumer-runnable snippet (FR-006).** Each generated skill MUST contain at
   least one consumer-facing, consumer-runnable usage snippet (scene
   construction, host wiring, or evidence production) — not only scope/governance
   text.
3. **Domain-agnostic starter (FR-007).** The generated starter app and tests MUST
   contain zero demo-specific (game-title) identifiers. Forbidden-identifier scan
   list (case-insensitive): `tetris`, `score`, `level`, `next piece`, `board`,
   `piece`. The generic game-starter shape (HUD region, gameplay region,
   primary-interaction counter) is retained so `fs-skia-layout-evidence` stays
   meaningful.

## Targets touched

- `template/base/src/Product/Model.fs`, `View.fs`, `EvidenceCommands.fs`,
  `LayoutEvidence.fs`; `template/base/tests/Product.Tests/Tests.fs`
  (neutralize identifiers, including the `Tetris-style board` / `score` / `level`
  / `next` assertions at `Tests.fs:430-434`).
- `template/fragments/*/skill/SKILL.md` + `README.md` (snippet present; no
  framework-only paths).

## Enforcement

`GeneratedGuidanceCheck` / `TemplateCheck` scan generated output for the
forbidden identifiers, the framework-only paths, and snippet presence.

## Evidence

`readiness/generated-guidance.md`.
