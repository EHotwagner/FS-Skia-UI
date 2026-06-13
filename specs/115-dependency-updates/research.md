# Phase 0 Research: Dependency Updates

**Date**: 2026-06-13 · **Source**: live NuGet flat-container API + GitHub releases API.

All NEEDS CLARIFICATION from the plan's Technical Context are resolved below (there were
none open — this feature's "research" is the version audit + the adopt/defer reasoning for
each package).

## Method

- NuGet latest versions: `https://api.nuget.org/v3-flatcontainer/<id>/index.json`.
- spec-kit: `https://api.github.com/repos/github/spec-kit/releases/latest`.
- .NET SDK: `https://api.github.com/repos/dotnet/sdk/releases/latest` + `dotnet --list-sdks`.
- Disposition rule (from spec): **safe** = patch/minor with no behavioral or `.fsi`
  risk → apply now; **held** = major/SemVer-breaking → adopt only if proven drop-in under
  the routed gates, else defer with reason.

## Decisions

### Safe bumps — apply now (US1)

| Package | Current → Target | Decision | Rationale |
|---|---|---|---|
| spec-kit (`speckit_version`) | 0.8.16 → 0.10.2 | **Apply** | Tooling; the repo's `.specify` extension constraints are all `>=` (`>=0.2.0` … `>=0.7.2`), so 0.10.2 satisfies them. Biggest currency gap. Update `init-options.json`; regenerate any skill/command assets the bump owns. |
| FSharp.Core | 10.1.300 → 10.1.301 | **Apply** | In-line patch on the 10.1.x line matching the `net10.0` / SDK 10.0.301 toolchain. No API change. |
| Microsoft.Extensions.FileSystemGlobbing | 10.0.8 → 10.0.9 | **Apply** | Patch on the 10.0.x servicing line. Build-tooling/adopt-set only; no shipped-product surface. |
| .NET SDK | 10.0.300 → 10.0.301 | **Apply (floats)** | No `global.json` pin exists, so this follows the installed toolchain (10.0.301 already installed). Nothing to edit; recorded for completeness. |

### Held bumps — adopt only if drop-in, else defer (US2)

| Package | Current → Latest | Bump | Default decision | Reason to hold |
|---|---|---|---|---|
| YamlDotNet | 17.1.0 → 18.0.0 | major | **Defer unless drop-in** | Major bump; used for governance YAML parsing — a serializer behavior change could shift gate output. |
| Fable.Elmish | 4.2.0 → 5.0.2 | major | **Defer unless drop-in** | Load-bearing MVU runtime for `Controls.Elmish`; v5 is a breaking line. Highest blast radius. |
| Expecto | 10.2.2 → 11.0.0 | major | **Defer unless drop-in** | Test framework just hit stable 11.0.0; API/runner changes possible across the whole suite. |
| Microsoft.NET.Test.Sdk | 17.11.1 → 18.6.0 | major | **Defer unless drop-in** | Test host major; pairs with the Expecto/YoloDev decision. |
| YoloDev.Expecto.TestSdk | 0.15.3 → 1.0.0 | major | **Defer unless drop-in** | 0.x → 1.0 adapter; coupled to the Expecto + Test.Sdk decision — evaluate the three together. |
| FSharp.Core (11.x line) | 10.1.300 → 11.0.101-preview5 | major | **Defer** | 11.x is tied to a newer F#/SDK and is not drop-in on the current `net10.0` toolchain; out of scope per spec. |

**Adopt-or-defer protocol (per held bump):** apply the single pin → run the full routed
gate set (serialized FAKE order) → if **all gates green with zero source change**, keep it
and move its row to "adopted"; otherwise `git checkout -- Directory.Packages.props` for
that pin and record the failing gate + symptom here. Evaluate Expecto + Test.Sdk + YoloDev
as one cluster (they interlock) — adopt all three or none.

### Out of scope — confirmed deferrals

- **SkiaSharp 4.147.0-preview.3.1** is already the newest available build (the next
  candidates are *older* stable `3.119.x`). Staying on the `4.147 preview` line is the
  existing deliberate choice; this feature does not switch lines. Already current:
  Silk.NET 2.23.0, Yoga.Net 3.2.3, FsCheck 3.3.3, DiffPlex 1.9.0, FSharp.SystemTextJson
  1.4.36, fsdocs-tool 22.1.0, XParsec 1.0.0 — no action.
- **FAKE family (Fake.Core.Target / Fake.IO.FileSystem / Fake.Tools.Git 6.1.4)** is pinned
  to `build.fsx.lock`; bumping it is a separate coordinated build-tooling change, not part
  of this feature.

## Alternatives considered

- **Force every latest version regardless of breakage** — rejected: violates the spec's
  resolution rule (behavior preservation + green gates win over completeness) and FR-005
  (no half-applied breaking bump).
- **Move SkiaSharp to stable `3.119.x`** — rejected: it is a *downgrade* of the major line
  and contradicts the deliberate preview-line choice; no benefit.
- **Skip spec-kit (largest gap) to keep the change source-only** — rejected: spec-kit
  currency is the highest-value safe item and the `>=` constraints already permit it.

## Implementation outcomes (T020 — recorded after the routed gate runs)

### Safe bumps (US1)

- **FSharp.Core 10.1.301** — **applied**. Dev green, zero `.fsi`/golden/generated-product diff.
- **Microsoft.Extensions.FileSystemGlobbing 10.0.9** — **applied**. Dev green; `build/**` adopt-set only.
- **spec-kit `speckit_version` 0.10.2** — **applied**, but **not** the zero-source-change bump the plan
  assumed. It broke one feature-025 governance test (`tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs`)
  that pins the *live* recorded version at `0.8.16`. Per maintainer decision (adopt + track the recorded
  constant), the test's live assertion was updated to `0.10.2` (a test-data update to the new recorded
  value, not a weakened assertion; 025's *historical* readiness records stay `0.8.16`). Only
  `.specify/init-options.json` carries the bumped `speckit_version` — the single field FR-007 governs and
  the only one the test asserts. An earlier iteration also bumped `.specify/integration.json` + the two
  `.specify/integrations/*.manifest.json` `version` fields; that was **reverted** — the `*.manifest.json`
  files are install-provenance records (`version` paired with `installed_at` + a hash set of installed
  files) and no spec-kit `0.10.2` install occurred (the repo generates `.claude` from the canonical
  `.agents` tree rather than vendoring upstream), so bumping them would falsify provenance. Zero
  `.fsi`/public-surface/golden delta; the only `*.fs` change is the `tests/**` constant.
- **.NET SDK** — **unchanged (floats)**. Honesty correction: the installed `net10` SDK is **`10.0.300`**,
  not `10.0.301` as the plan/research assumed. There is no `global.json`, so the SDK floats to whatever is
  installed; nothing was edited and the suite is green on `10.0.300`. The float will adopt `10.0.301`
  wherever that SDK is present.

### Held bumps (US2)

- **YamlDotNet 18.0.0** — **adopted**. Dev + GeneratedGuidanceCheck + TemplateDrift green, zero source
  change. The governance YAML reader (FS.Skia.UI.Build) and KeyboardInput YAML config parse unchanged.
- **Fable.Elmish 5.0.2** — **adopted**. Dev green (Controls.Elmish MVU adapter, Elmish.Tests 141,
  Parity.Tests 21), zero source change. The v5 line is drop-in for this codebase's Cmd/Program usage.
- **Test-stack cluster (Expecto 11.0.0 + Microsoft.NET.Test.Sdk 18.6.0 + YoloDev.Expecto.TestSdk 1.0.0)** —
  **deferred**, whole cluster reverted. `Restore` failed `NU1608`: `YoloDev.Expecto.TestSdk 1.0.0 requires
  Expecto (>= 9.0.0 && < 10.0.0)` but `Expecto 11.0.0` was resolved. YoloDev 1.0.0 caps Expecto **below
  10**, so it is incompatible with both the target Expecto 11 *and* the current 10.2.2 — the cluster is
  internally inconsistent at the published-metadata level and cannot be adopted until a YoloDev release
  supports Expecto 11. Reverted whole (never partial), per FR-004/FR-005.
- **FSharp.Core 11.x** — **deferred**, not attempted. Out of scope (tied to a newer F#/SDK; not drop-in on
  `net10.0`).

### Out-of-scope deferrals (confirmed unchanged)

- **SkiaSharp 4.147.0-preview.3.1** — unchanged (deliberate preview line; next candidates are older stable
  3.119.x).
- **FAKE family (6.1.4)** — unchanged (`build.fsx.lock`-pinned; a separate coordinated build-tooling
  change).

### Incidental finding (pre-existing, unrelated to the version bumps)

- `./fake.sh build -t DependencyReport` was **failing on `main`** independently of feature 115:
  `scripts/dependency-report.fsx` hardcoded Controls.Elmish's expected ProjectReference set as
  `{Controls, KeyboardInput}`, but **Feature 085** intentionally added the documented acyclic
  `Controls.Elmish → SkiaViewer` edge (the fsproj comments it). The script's expected set had drifted. As
  part of T019 (regenerating the dependency-governance output) the expected set was brought current to
  include `../SkiaViewer/SkiaViewer.fsproj`; `DependencyReport` is now green. This is a governance-script
  currency fix, not a dependency-version change, and does not touch any `.fsi`/public surface.
