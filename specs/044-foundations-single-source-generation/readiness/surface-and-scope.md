# Surface Area & Scope — Feature 044 (T006)

## New build-tooling public surfaces (each with a curated `.fsi`, Principle II)

- `FS.Skia.UI.Build.SkillTreeGen` — `SkillFile`/`GenPlanEntry`/`SkillTreePlan`/`SkillCurrency`
  types; `derivedRelPath`, `canonicalRoot`, `derivedRoot`, `renderManifest`, `plan`,
  `currency`, `isCurrent`, `currencyDrift`.
- `FS.Skia.UI.Build.SkillistView` — `SkillistCurrencyItem`; `renderAnnotation`,
  `spliceAnnotation`, `currency`, `currencyDrift`, `staleDiagnostic`.
- `FS.Skia.UI.Build.ConstitutionFragments` — `PrincipleFragment`/`MarkerRegion`/`FragmentCurrency`;
  `fragmentIds`, `extract`, `regions`, `splice`, `currency`, `currencyDrift`.
- `FS.Skia.UI.Build.SkillSync` — reframed to a currency adapter (`planFromCanonical`,
  `currency`, `isCurrent`, `renderReport`, `renderFailureMessage`); the old six-slug
  byte-identity peer surface (`expectedSlugs`, `checkAll`, `checkPair`, `sha256Hex`,
  `inSync`, `drifted`, `SkillPairResult`) is removed.

These are **build-tooling** surfaces (build/Governance under net10.0), NOT tracked runtime
surface baselines. `PackageSurfaceCheck` / `FsiTranscripts` show **no product baseline
diff** (Invariant 1); product `src/**` diff is 0 (`logs/runtime-untouched.md`).

## Retired surface

`FS.Skia.UI.Build.SkillExamples` (module + `.fsi`) and the `SkillExamplesCheck` typed
`Target` case are removed (FR-004, research R6). The exhaustive `Target` match makes any
missed reference a compile error; the metadata registry drops from 38 to 37 rows.

## Failure handling (Principle VII — no partial artifacts)

- `SkillTreeGen.plan` **raises** on an empty canonical set or a file with null/unreadable
  bytes — it never emits a partial derived tree (unit-tested).
- `ConstitutionFragments.extract` **raises** when a required `### Principle` heading is
  missing (unit-tested).
- `SkillistView.spliceAnnotation` **raises** when a task line carries no `[skillist: …]`
  token (omitted metadata is invalid; unit-tested).
- `regenerateConstitutionFragments` (build.fsx edge) **fails loudly** if the constitution or
  a governed template is missing.

## Deferred / out of scope

Stage 5 (MEL-engine relocation / `build.fsx` retirement), Stage 6 (content trimming,
contract `schema_version`, evidence-bloat hygiene), Stage 7. Symlink-based sharing is out
(cross-platform; copy-generation only). No product/runtime/packaging/public-`.fsi` change.
