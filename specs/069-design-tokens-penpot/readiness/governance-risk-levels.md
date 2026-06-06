# Governance risk levels (feature 069)

The change classes for this feature and the gates each requires. Run **only** the
gates `./fake.sh build -t Route` prints; FAKE-backed gates run **sequentially**.

## small (inner-loop)

Generator logic (`build/Governance/DesignTokenGen.fs`), tests, and readiness prose.
- **required evidence**: green `./fake.sh build -t Dev`.
- Gate set: `Dev` only.

## medium

`Theme.fs` re-expression, the generated `src/Controls/DesignTokens.fs`, and the
`Controls.fsproj` `<Compile>` insert.
- **required evidence**: `Dev` + `DesignTokenDrift` + `PackageSurfaceCheck` green.

## broad (escalated / consumer-contract)

Public `src/Controls/**/*.fsi` (the curated `DesignTokens.fsi`), the
`DesignTokenDrift` target, the routing rule, and the new `fs-skia-design-tokens`
skill.
- **broad validation** is required: `Route` escalates to the
  `controls-public-surface` gate set **plus** `DesignTokenDrift` and the
  governance/skill gates.
- **required evidence**: the per-gate PASS lines for every gate `Route` prints
  (authoritative); aggregate `Route`/multi-gate summaries are recorded as
  **non-authoritative**.

## Authoritative command

`./fake.sh build -t Route` (prints the tier + minimal gate list for the working-tree diff).
