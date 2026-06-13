# Unsupported scope & evidence obligations (feature 115, T001)

## Feature directory wiring (confirmed present)

- `specs/115-dependency-updates/spec.md` — present
- `specs/115-dependency-updates/plan.md` — present
- `specs/115-dependency-updates/research.md` — present (per-package adopt/defer decisions)
- `specs/115-dependency-updates/data-model.md` — present (the pin disposition table)
- `specs/115-dependency-updates/quickstart.md` — present (apply + verify runbook)
- `specs/115-dependency-updates/tasks.md` + `tasks.deps.yml` — present

## Tier / classification

- **Tier**: Tier 2 (internal change) for the safe **product** pin bumps (FSharp.Core,
  Microsoft.Extensions.FileSystemGlobbing) — version-only, no behavioral or `.fsi` delta.
- **Route escalation**: the spec-kit asset edit touches `.specify/**` (a consumer-contract /
  governance path), so `Route` escalates beyond the inner loop regardless of the absent
  `.fsi` delta. `Route` is the authority on the exact gate list; this feature runs only what
  it prints. The live `Route` output is recorded in `focused-gates.md`.

## Affected paths

- `Directory.Packages.props` — FSharp.Core, Microsoft.Extensions.FileSystemGlobbing (safe);
  the held bumps (YamlDotNet / Fable.Elmish / Expecto + Microsoft.NET.Test.Sdk +
  YoloDev.Expecto.TestSdk) only if a drop-in check proves them green-with-no-source-change.
- `.specify/init-options.json` — `speckit_version` recorded-version edit (0.8.16 → 0.10.2).
- `template/**` — refreshed only if a generated project becomes inconsistent (US3).
- `docs/reports/dependencies.md` — pin notes refreshed to match.
- `specs/115-dependency-updates/**` — this feature's evidence.
- **No `src/**/*.fs` or `*.fsi` edits.**

## Public-API impact

**None.** Zero `.fsi`, zero public-doc, zero surface-baseline, zero sample-contract change
(FR-003). The surface/golden/generated-product gates are the enforcing assertion.

## Elmish/MVU applicability

**N/A** for the safe bumps — no `Model` / `Msg` / `Effect` / `update` / interpreter edit.
A Fable.Elmish major adoption WOULD touch the runtime under this boundary, which is exactly
why it is held behind a drop-in check (US2) rather than applied blind; if it fails the
drop-in check it is fully reverted (FR-005), so the boundary is never half-changed.

## Out of scope (deferred, recorded with reason in research.md)

- **SkiaSharp** stays on the deliberate `4.147.0-preview.3.1` line (next candidates are
  *older* stable `3.119.x` — a downgrade).
- **FAKE family** (Fake.Core.Target / Fake.IO.FileSystem / Fake.Tools.Git 6.1.4) stays
  `build.fsx.lock`-pinned; bumping it is a separate coordinated build-tooling change.
- **FSharp.Core 11.x line** (`11.0.101-preview5`) — tied to a newer F#/SDK, not drop-in on
  the current `net10.0` toolchain.
- Any held major bump that fails its drop-in check is deferred with the failing gate + symptom.

## Real-evidence obligations (Principle V — zero synthetic)

Every gate runs against the **real** build, **real** packed libraries, and the **real**
generated template — no mocks, fakes, stubs, in-memory substitutes, or canned data. The
`[S]` regime does not apply; `EvidenceAudit` must pass with **zero** synthetic markers. The
behavior-preserving claim is evidenced by the unchanged Expecto/FsCheck suites + golden /
surface gates staying green on the bumped pins. No new tests are authored (no new behavior).

## Interactive-UI run-and-use gate

**N/A** — this feature ships no new interactive surface, host app, window, scene, or
screenshot path. It is a dependency-version + governance-asset maintenance change. The
existing `runInteractiveApp` window-launch contract is untouched. No story is interactive,
so the run-and-use gate does not apply (recorded in the window-visibility not-applicable set).
