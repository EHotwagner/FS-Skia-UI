# US2 validation — adopt-or-defer each held major bump (T012–T016)

**Story**: Every held (major / SemVer-breaking) bump receives an auditable adopt-or-defer disposition;
no half-applied breaking bump remains (FR-004, FR-005).

## Protocol (per held bump)

Apply the single pin (or the interlocking cluster) → run the routed gate set, FAKE-backed targets
sequentially. The decisive signal is `Dev` (Restore + Build + full Expecto/FsCheck suites) plus the quick
`GeneratedGuidanceCheck` / `TemplateDrift`; `EvidenceGraph` / `EvidenceAudit` are the shared merge gate
(T021/T022). **Adopt** iff all gates are green **with no source change**; otherwise
`git checkout -- Directory.Packages.props` for that pin/cluster and record `deferred(<failing gate +
symptom>)`. Never a partial cluster.

## Dispositions

| Held bump | From → To | Disposition | Evidence / symptom |
|---|---|---|---|
| YamlDotNet (T012) | 17.1.0 → 18.0.0 | **adopted** | `Dev` green (build + Governance.Tests YAML reader + KeyboardInput YAML config), GeneratedGuidanceCheck + TemplateDrift green, **zero** source/golden/.fsi change. `readiness/logs/held-yamldotnet-dev.txt` |
| Fable.Elmish (T013) | 4.2.0 → 5.0.2 | **adopted** | `Dev` green (Controls.Elmish MVU adapter + Elmish.Tests 141 + Parity.Tests 21), GeneratedGuidanceCheck + TemplateDrift green, **zero** source/golden/.fsi change. `readiness/logs/held-fable-elmish-dev.txt` |
| Test-stack cluster (T014): Expecto + Microsoft.NET.Test.Sdk + YoloDev.Expecto.TestSdk | 10.2.2→11.0.0 / 17.11.1→18.6.0 / 0.15.3→1.0.0 | **deferred** — whole cluster reverted | `Restore` failed `NU1608`: `YoloDev.Expecto.TestSdk 1.0.0 requires Expecto (>= 9.0.0 && < 10.0.0)` but `Expecto 11.0.0` was resolved. YoloDev 1.0.0 caps Expecto below 10 — incompatible with both Expecto 11 and the current 10.2.2. The cluster is internally inconsistent at the published-metadata level; not drop-in. `readiness/logs/held-test-cluster-dev.txt` + `readiness/logs/restore.txt` |
| FSharp.Core 11.x (T015) | 10.1.300 → 11.0.101-preview5 | **deferred** — not attempted | Out of scope per spec/research: the 11.x line is tied to a newer F#/SDK and is not drop-in on the current `net10.0` toolchain (SDK 10.0.300 installed). No pin edit made. |

## No half-applied state (T016, FR-005, SC-003)

`git diff Directory.Packages.props` after US2 shows **only** the adopted pins differ from the safe-bump
baseline:

- `FSharp.Core` 10.1.300 → 10.1.301 (US1 safe)
- `Microsoft.Extensions.FileSystemGlobbing` 10.0.8 → 10.0.9 (US1 safe)
- `YamlDotNet` 17.1.0 → 18.0.0 (US2 adopted)
- `Fable.Elmish` 4.2.0 → 5.0.2 (US2 adopted)

`Expecto` (10.2.2), `Microsoft.NET.Test.Sdk` (17.11.1), and `YoloDev.Expecto.TestSdk` (0.15.3) are back at
their current pins — the deferred cluster was reverted **whole**, never partially. No deferred/breaking bump
remains in the tree.

## Final-tree validation note

The adopted tree (safe bumps + YamlDotNet 18 + Fable.Elmish 5, cluster at baseline) is **exactly** the
state exercised green by `held-fable-elmish-dev.txt` (YamlDotNet 18 was already adopted when Fable.Elmish 5
was applied). Reverting the cluster returns to that validated state. The merge-gate
`Dev`/`EvidenceGraph`/`EvidenceAudit` are re-run on this final tree at T021/T022.

Result: **PASS** — every held bump has an auditable adopt-or-defer disposition; the tree carries only
cleanly-drop-in pins (SC-003).
