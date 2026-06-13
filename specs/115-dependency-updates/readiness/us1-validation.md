# US1 validation — bring safe pins current (T010 / T011)

**Story**: The repository's safe (patch/minor) dependency pins are brought current with all routed
gates green and zero contract delta.

## Applied bumps

| Pin | Location | From | To | Outcome |
|---|---|---|---|---|
| FSharp.Core | `Directory.Packages.props` | 10.1.300 | 10.1.301 | **applied** |
| Microsoft.Extensions.FileSystemGlobbing | `Directory.Packages.props` | 10.0.8 | 10.0.9 | **applied** |
| spec-kit (`speckit_version`) | `.specify/init-options.json` | 0.8.16 | 0.10.2 | **applied** |
| .NET SDK | installed toolchain (no `global.json`) | 10.0.300 | (floats) | see note below |

## .NET SDK float — honesty correction (FR-001)

The plan/research assumed the floating .NET SDK was already at `10.0.301`. The actually-installed
`net10` SDK on this machine is **`10.0.300`** (`dotnet --list-sdks` → `6.0.428`, `10.0.300`). There is
**no `global.json`** pin, so the SDK floats to whatever is installed — currently `10.0.300`. Nothing was
edited (there is no pin to change, and `10.0.301` is not present on this toolchain). The build + full test
suite are green on the installed `10.0.300`; the "bump to 10.0.301" is a no-op-floats item recorded for
completeness, and the float will pick up `10.0.301` automatically wherever that SDK is installed. This is
recorded truthfully rather than asserting `10.0.301` is in use (it is not, here).

## spec-kit bump footprint — plan correction (FR-001 / FR-007)

The plan classified the spec-kit bump as a **zero-source-change** "safe" bump. In reality it broke one
governance test, `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` — the feature-025 assertion
"`.specify/init-options.json` contains `\"speckit_version\": \"0.8.16\"`". Per the maintainer decision
(adopt + track the recorded constant), the bump was **kept** and the recorded version is now tracked to
its new value:

- `.specify/init-options.json` — `speckit_version` `0.8.16 → 0.10.2`. This is the **only** file FR-007
  governs ("`speckit_version` equals the version in use") and the only one the governance test asserts as
  the live recorded version.
- `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` — the live assertion now expects `0.10.2` (a
  test-data update to the new recorded version, **not** a weakened assertion; the test still pins one
  concrete version). Feature 025's *historical* readiness records (`version-selection.md`,
  `template-version-alignment.md`) correctly stay at `0.8.16` and are unchanged.

**Provenance correction (manifests left at 0.8.16):** an earlier iteration also bumped the `version` field
in `.specify/integration.json` and the two `.specify/integrations/*.manifest.json` files to `0.10.2`. That
was reverted. The two `*.manifest.json` files are **install-provenance** records — their `version` is
paired with an `installed_at` timestamp and a SHA-256 hash set of the files that version installed; no
spec-kit `0.10.2` install actually occurred (the repo doesn't vendor upstream — `.claude` is generated
from the canonical `.agents` tree), so bumping their `version` would falsify provenance. They correctly
record `0.8.16` (the version that installed those hashed assets). FR-007 is satisfied by
`init-options.json` alone; nothing asserts the manifest versions, and `GeneratedGuidanceCheck` /
`TemplateDrift` / `SkillSyncCheck` are green with the manifests at `0.8.16`.

There is **zero `.fsi`, public-surface, golden, or generated-product diff** (FR-003); the only `*.fs`
change is the governance test constant above (a `tests/**` file, not `src/**`).

## Routed gates (Route-authoritative, run sequentially)

`./fake.sh build -t Route` escalated to `Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph,
EvidenceAudit` (matched rules: evidence-governance, specify-catchall, docs-only — the `.specify/**` path).
Run only those, FAKE-backed targets sequentially:

| Gate | Result | Log |
|---|---|---|
| Dev | **Ok** (Restore+Build+SampleContractSmoke+Test green; Test 1m17s) | `readiness/logs/dev-after-safe.txt` |
| GeneratedGuidanceCheck | **Ok** | `readiness/logs/generated-guidance-check.txt` |
| TemplateDrift | **Ok** | `readiness/logs/template-drift.txt` |
| EvidenceGraph | merge gate — T021 | — |
| EvidenceAudit | merge gate — T022 | — |

## Zero-delta confirmation (SC-002, FR-002/FR-003)

- `git diff --name-only` shows **no** `*.fsi`, golden, surface-baseline, or generated-product file changed.
- The only `*.fs` change is `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` (the recorded-version
  constant), per the maintainer-approved spec-kit footprint above.
- An incidental `specs/011-.../sample-smoke/keyboard-input-gallery-state-display.txt` `elapsed-ms` timing
  churn from the `SampleContractSmoke` run was reverted (non-feature artifact).

Result: **PASS** — safe pins current, all routed gates green, zero contract delta (SC-001, SC-002, SC-005).
