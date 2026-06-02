# Tier, layer, and evidence obligations (T001)

- **Tier**: Tier 2 (internal build-tooling / governance). `Route` **escalates** it to the
  full serialized six-target set because it touches governance paths
  (`build/Governance/**`, `.agents/skills/**`, `.specify/**`-adjacent) and a
  generated-product-contract surface. The **US2** story is **[T1]** (it changes a
  *consumer* contract: the generated-product structural contract gains `schema_version` +
  a deprecation window). Run as a **dogfood** feature.
- **Affected layers**: `build/Governance/Guidance.fs(/.fsi)`, new
  `build/Governance/GeneratedProductContract.fs(/.fsi)`,
  `build/Governance/GeneratedProduct.fs` (consult-point), `tests/Governance.Tests/**`,
  `.agents/skills/**` (+ regenerated `.claude/skills/**`), `.gitignore`.
- **Public-API impact**: **No** product `.fsi` / surface-baseline / `PackageVersion`-
  outside-CPM change (SC-009). The consumer generated-product contract gains a
  `schema_version` + deprecation window (US2/T1).
- **Elmish/MVU applicability**: product runtime untouched; the new validators are **pure
  functions** returning typed results with file I/O confined to the existing
  `interpret`/`Front` edge (Principle IV). Product MVU **not applicable**.
- **Real-evidence obligations** (zero synthetic): typed unit tests (`unit-tests.md`),
  seeded-violation proofs (`seeded-violations/`), live gate fail→fix→pass
  (`seeded-violations/constitution-check.md`), generation-currency green
  (`generated-guidance-check.log`), `git check-ignore` proof (`gitignore-check.md`),
  prose-delta measurement (`prose-delta.md`), serialized escalated FAKE logs
  (`logs/serialized-gates.md`).
