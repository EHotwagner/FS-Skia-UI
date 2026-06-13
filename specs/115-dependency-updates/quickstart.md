# Quickstart: Applying & Verifying the Dependency Updates

Runbook for implementing feature 115. No `src/**` source changes — pins + governance
assets only.

## 1. Apply the safe bumps (US1)

In `Directory.Packages.props`:

- `FSharp.Core` `10.1.300` → `10.1.301`
- `Microsoft.Extensions.FileSystemGlobbing` `10.0.8` → `10.0.9`

In `.specify/init-options.json`:

- `speckit_version` `0.8.16` → `0.10.2`, then regenerate any spec-kit-owned skill/command
  assets the bump produces (use the spec-kit upgrade path; do **not** hand-edit generated
  trees — let `RefreshSurfaceBaselines` / `SkillSyncCheck` validate currency).

.NET SDK `10.0.301` is already installed and there is no `global.json` to edit — nothing
to change, recorded for completeness.

## 2. Route, then run only the gates it prints

```sh
./fake.sh build -t Route            # authoritative tier + minimal gate list for THIS diff
./fake.sh build -t Route --enforce  # additionally fail if an escalated change lacks evidence
```

Because `.specify/**` is a consumer-contract path, expect escalation. Run the gates
`Route` prints, FAKE-backed targets **sequentially** in the deterministic order:

```sh
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

**Pass criteria (FR-002/FR-003):** all printed gates green, **zero** surface-baseline,
golden, and generated-product diff. If a safe bump produces any source/golden diff, treat
it as not-actually-safe and reclassify it as held.

## 3. Evaluate each held bump (US2) — adopt-or-revert

For YamlDotNet, Fable.Elmish, and the {Expecto + Microsoft.NET.Test.Sdk +
YoloDev.Expecto.TestSdk} **cluster** (adopt the cluster together or not at all):

```sh
# bump the single pin (or the cluster) in Directory.Packages.props, then:
./fake.sh build -t Route
# run the full printed gate set, sequentially, as above
```

- **All gates green, no source change** → keep it; mark the row `adopted` in
  `research.md` / `data-model.md`.
- **Any gate red, or a source change would be required** → revert and record:

  ```sh
  git checkout -- Directory.Packages.props   # or restore just the affected pin
  ```

  Mark the row `deferred(<failing gate + symptom>)`.

FSharp.Core 11.x is **not** attempted (out of scope — tied to a newer F#/SDK).

## 4. Template consistency (US3)

If the safe bumps touch anything a generated project consumes, refresh template pins via
the `fs-skia-template-update` skill, then confirm a fresh project restores and builds:

```sh
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
```

## 5. Refresh dependency notes & evidence

- Update `docs/reports/dependencies.md` pin notes to match (and let `DependencyReport`
  regenerate its output).
- Record final per-package outcomes in `research.md` / `data-model.md`.
- Confirm `EvidenceAudit` verdict = PASS with **zero** synthetic markers (none are used).

## Revert protocol summary

No partially-applied breaking bump may remain (FR-005). Any held bump that is not cleanly
drop-in is fully reverted to its current pin before the feature is declared merge-ready.
