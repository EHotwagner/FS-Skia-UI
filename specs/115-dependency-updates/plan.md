# Implementation Plan: Dependency Updates ("update all if possible")

**Branch**: `115-dependency-updates` | **Date**: 2026-06-13 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/115-dependency-updates/spec.md`

## Summary

Bring the repository's dependency pins as current as is **provably safe**, and give
every remaining (major) bump an auditable adopt-or-defer decision. The 2026-06-13 audit
found four safe bumps and a set of held major bumps:

- **Safe now (US1):** spec-kit `0.8.16 → 0.10.2` (`.specify/init-options.json`
  `speckit_version` + any regenerated skill/command assets), FSharp.Core
  `10.1.300 → 10.1.301`, Microsoft.Extensions.FileSystemGlobbing `10.0.8 → 10.0.9`
  (both in `Directory.Packages.props`), and the .NET SDK `10.0.300 → 10.0.301`
  (floats — no `global.json` pin to edit).
- **Held for drop-in evaluation (US2):** YamlDotNet 18, Fable.Elmish 5, Expecto 11,
  Microsoft.NET.Test.Sdk 18, YoloDev.Expecto.TestSdk 1.0.0, and the FSharp.Core 11.x
  line — each adopted **only** if it proves byte-clean under the routed gates with no
  source change, otherwise reverted and recorded.
- **Out of scope:** SkiaSharp stays on the deliberate `4.147.0-preview` line; the FAKE
  family stays `build.fsx.lock`-pinned at 6.1.4.

**Technical approach.** This is a **dependency-version + governance-asset** change, not a
source-behavior change. The product-affecting edits (`FSharp.Core`,
`Microsoft.Extensions.FileSystemGlobbing`) are version-only and asserted byte-identical;
the spec-kit bump touches `.specify/**` (and possibly regenerated `.claude`/`.agents`
skill/command assets), which is a **consumer-contract / governance path** that makes
`Route` escalate beyond the inner loop. No `.fsi` signature changes. Each held bump is a
throwaway experiment: apply → run the full routed gate set → keep iff green-with-no-source-
change, else `git checkout` the pin back and record the reason in `research.md`. No
partially-applied breaking bump may remain in the tree (FR-005).

## Technical Context

**Language/Version**: F# / .NET `net10.0` (SDK `10.0.301`)
**Primary Dependencies**: No **new** dependencies — version bumps of existing pins only.
Edits `Directory.Packages.props` (FSharp.Core, Microsoft.Extensions.FileSystemGlobbing;
and, per held-bump outcomes, possibly YamlDotNet / Fable.Elmish / Expecto /
Microsoft.NET.Test.Sdk / YoloDev.Expecto.TestSdk) and `.specify/init-options.json`
(`speckit_version`). Template pins (`template/**`) refreshed only if needed for
consistency (US3). No `src/**/*.fs` or `*.fsi` edits.
**Testing**: The existing Expecto + FsCheck suites and FAKE gates are the evidence — the
suites must stay green unchanged across every bump. No new tests are authored (there is no
new behavior to prove); the safe-bump proof is "zero diff in surface/golden/generated-
product output + all routed gates green", and each held bump's proof is the full escalated
gate run.
**Target Platform**: Windows and Linux. No platform-specific, Vulkan, Skia, or visual-
output change (SkiaSharp/Silk/Yoga pins unchanged).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification — Tier 2 (internal change) for the safe product bumps; governance/
consumer-contract escalation for the spec-kit asset bump.** The FSharp.Core and
FileSystemGlobbing bumps are version-only with no behavioral or `.fsi` delta (Tier 2). The
spec-kit bump edits `.specify/**` governance assets, a consumer-contract path that
`Route` escalates regardless of the absent `.fsi` delta. The constitution's Tier 1
trigger "introduces new dependencies" does **not** fire — these are existing pins moving
version, not new dependencies. `Route` is the authority on the exact gate list; the plan
runs only what it prints.

**Principle compliance.**
- *I (Spec→FSI→Tests→Impl)*: no API is designed — there is no new public surface. The
  "test" is the standing gate suite staying green; the failing-first equivalent is that a
  bad bump turns a currently-green gate red, which is the signal to revert.
- *II (Visibility in `.fsi`)*: **zero `.fsi` change** is a hard requirement (FR-003);
  surface-baseline gates must report no delta. No access modifiers touched.
- *III (Idiomatic simplicity)*: no code is written; the change is version strings in
  config plus a regenerated governance asset. Nothing to justify under the complexity
  list.
- *IV (Elmish/MVU boundary)*: unchanged for the safe bumps (no `Model`/`Msg`/`Effect`/
  `update`/interpreter edits). A Fable.Elmish major bump *would* touch the runtime under
  this boundary — which is exactly why it is held behind a drop-in check rather than
  applied blind.
- *V (Synthetic disclosure)*: **none.** Every gate runs against the real build, real
  packed libraries, and real generated template — no mocks, fakes, or canned data. The
  `[S]` regime does not apply; `EvidenceAudit` must pass with zero synthetic markers.
- *VI (Test evidence)*: the behavior-preserving claim is evidenced by the unchanged test
  suites passing on the bumped pins and zero golden/surface diff. No assertion is weakened.
- *VII (Observability)*: not applicable to a version bump — no new diagnostics or failure
  paths. (N/A — version-only change introduces no operational events.)

### Repository Governance Decisions

- **Template ownership**: `template/**` pins are refreshed **only if** the safe bumps make
  the generated project inconsistent (US3 / FR-006); the `fs-skia-template-update` skill
  governs that refresh. `.template.config/template.json` is **not** otherwise edited.
  N/A for `src`/`docs`/`samples`/`tests` — no such files change.
- **Dependency impact**: **Yes** — `Directory.Packages.props` changes (FSharp.Core,
  Microsoft.Extensions.FileSystemGlobbing now; held bumps only if adopted). `docs/`
  dependency notes (`docs/reports/dependencies.md` / `docs/dependencies.md`) and the
  `DependencyReport` target must reflect the new pins. The spec-kit pin in
  `.specify/init-options.json` is updated to match the version in use (FR-007).
- **Command-surface impact**: No `build.fsx` / wrapper / target *definition* changes.
  `DependencyReport` output content updates with the pins. Gates run via `Route` in the
  serialized, non-concurrent FAKE order (`Dev` → `GeneratedGuidanceCheck` →
  `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`) since the
  spec-kit/`.specify` change escalates; FAKE-backed targets run sequentially.
- **Generated project impact**: Only if a safe bump changes generated-project restore/
  build (it must not, by FR-006). `GeneratedProductCheck` / `TemplateCheck` confirm a
  freshly generated `dotnet new fs-skia-ui` project still restores and builds against the
  updated pins (SC-004).
- **Evidence paths**: `specs/115-dependency-updates/research.md` (per-package adopt/defer
  decisions + rationale), `specs/115-dependency-updates/data-model.md` (the disposition
  table + before/after pins), `specs/115-dependency-updates/quickstart.md` (apply +
  verify runbook), the routed gate logs under `readiness/`, the `DependencyReport` output,
  and the `EvidenceAudit` verdict. No screenshots/FSI transcripts required (no behavior
  change, no new surface).
- **`.fsi` / contract impact**: **None.** No signature, public-doc, surface-baseline, or
  sample-contract change. Asserted, gate-confirmed (FR-003).
- **MVU/effect boundary**: N/A — no stateful or I/O-bearing source change. (A held
  Fable.Elmish adoption would re-open this and require its own evidence before merge; it
  is deferred unless proven drop-in.)
- **Synthetic evidence**: None. No mocks/fakes/placeholders/in-memory substitutes; all
  evidence is the real build/test/generated-project run. No `[S]` tasks planned.
- **Test evidence**: The existing Expecto/FsCheck suites and golden/surface gates are the
  failing-first guard — a bump that breaks one is reverted. No new tests authored (no new
  behavior). Packed-library and generated-project smoke covered by
  `GeneratedProductCheck` / `TemplateCheck`.
- **Observability**: N/A — version-only change adds no diagnostics, log paths, or
  unsupported-environment messages.
- **Deferred scope**: SkiaSharp `4.147 preview` line change, FAKE-family bump (lock-
  pinned), FSharp.Core 11.x adoption, and any major bump that fails its drop-in check are
  deferred follow-ups, recorded in `research.md` with the reason.

**Initial Constitution Check: PASS** (no violations; no complexity to justify).

## Project Structure

Files this feature may touch (no `src/**` source):

```
Directory.Packages.props                     # FSharp.Core, FileSystemGlobbing (+ adopted held bumps)
.specify/init-options.json                   # speckit_version 0.8.16 -> 0.10.2
.specify/** , .claude/** , .agents/**        # regenerated spec-kit skill/command assets (if the bump regenerates them)
template/**                                  # pins refreshed only if needed for consistency (US3)
docs/reports/dependencies.md                 # pin notes refreshed to match
specs/115-dependency-updates/                # this feature's evidence (plan/research/data-model/quickstart/tasks)
```

No `contracts/` directory: this feature exposes **no external interface** (it is internal
dependency maintenance), so per the planning rules the contracts step is skipped.

## Phase 0 — Research

See [research.md](./research.md): resolves the per-package disposition (safe vs. held),
records the adopt/defer outcome and rationale for each held major bump, and confirms the
out-of-scope decisions (SkiaSharp line, FAKE lock, FSharp.Core 11.x).

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md): the dependency entity / disposition table (current →
  target → classification → outcome) — the only "data" this feature has.
- Contracts: **skipped** (no external interface).
- [quickstart.md](./quickstart.md): the apply-and-verify runbook (which pins to edit, how
  to run `Route`, the serialized FAKE gate order, and the revert protocol for a failing
  held bump).
- Agent context: `AGENTS.md` plan reference updated to point at this plan.

## Phase 2 — Task planning approach

`/speckit-tasks` will produce story-grouped tasks: **US1** (apply + verify the four safe
bumps, one task per pin + a routed-gate verification task), **US2** (one apply/evaluate/
record task per held major bump, each ending in adopt-or-revert), **US3** (template
consistency + generated-project verify). Every task carries `skillist` metadata
(`fs-skia-template-update` for template refresh; `[]` for pure pin edits). The graph is
linear-ish: safe bumps gate the template check; held bumps are independent of each other.
