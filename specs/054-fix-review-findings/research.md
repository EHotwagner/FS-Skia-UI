# Phase 0 Research: Fix Implementation-Completeness Review Findings

All "unknowns" here are empirical facts about the current tree, resolved by direct
inspection (grep/build), not open design questions. Each finding below is verified.

## R1 — Template engine-pin drift (§4.1 / FR-001..FR-004)

**Decision**: Treat `template/base/Directory.Packages.props` `PackageVersion` as the source of
truth; make the `#r` literal in `template/base/build.fsx` a derived value the pin-bump flow
keeps current.

**Evidence (current drift confirmed)**:

- `template/base/build.fsx:1` → `#r "nuget: FS.Skia.UI.Build, 0.1.45-preview.1"`
- `template/base/Directory.Packages.props:9` → `<PackageVersion Include="FS.Skia.UI.Build" Version="0.1.56-preview.1" />`
- They disagree (`0.1.45` vs `0.1.56`) — a generated app's governance build would restore an
  engine **11 patch versions stale**.

**Root cause**: the recurring pin-bump flow is the `fs-skia-template-update` skill. Its step 3
runs `sed -i 's/Version="<old>"/Version="<new>"/g' template/base/Directory.Packages.props` —
which **only** touches the props file. The `#r "nuget: FS.Skia.UI.Build, <ver>"` literal in
`build.fsx` uses a different syntax (`, <ver>` not `Version="<ver>"`) so the `sed` never matches
it. Nothing in the flow keeps the `#r` literal current → guaranteed drift on every bump.

**Why the gate missed it**: `tests/Governance.Tests/GeneratedProjectValidationTests.fs:292`
asserts only `Expect.stringContains build "#r \"nuget: FS.Skia.UI.Build"` — a **prefix**
substring that ignores the version entirely.

**Rationale**: props is already the single value the flow drives (one version for all nine repo
packages, per the skill). Deriving the `#r` literal from it removes the second source of truth.

**Alternatives considered**:
- Make `build.fsx` read the version from props at runtime — rejected: `build.fsx` is a generated
  consumer script that must be self-contained and offline-restorable; an `#r` literal cannot be
  computed at `#r` time anyway.
- Drop the `#r` version pin (float) — rejected: violates the project's explicit-pinning
  discipline and reintroduces non-determinism.

## R2 — FS3261 nullness warnings (§4.2 / FR-005, FR-009)

**Decision**: Resolve **every** FS3261 site (clean build) with safe null handling, then remove
the project-local `WarningsNotAsErrors;FS3261` escape hatch so regressions fail the build.
(User-confirmed scope: all 8 files; project-local promotion.)

**Evidence (clean `--no-incremental` build of `FS.Skia.UI.Build.fsproj`)** — 34 distinct sites
across 8 files (the review §4.2 saw only the 2 files an *incremental* build recompiled):

| File | Sites |
|---|---|
| `build/Governance/GeneratedProduct.fs` | 22 lines (468, 471, 488, 495, 750, 751, 844, 847, …) |
| `build/Governance/Front/Governance.fs` | 20 lines (185–187, …) |
| `build/Governance/Engine/Model.fs` | 14 lines (20, 72, 78, 187, 197, 201) |
| `build/Governance/Guidance.fs` | 8 lines (522, 543) |
| `build/Governance/Front/BuildProcess.fs` | 8 lines (35, 78, 80) |
| `build/Governance/Preflight.fs` | 6 lines (145, 396, 404) |
| `build/Governance/PerPackageSurface.fs` | 6 lines (204, 263) |
| `build/Governance/Front/BuildProcessHealth.fs` | 6 lines (72, 115, 144) |

(Site counts are raw warning emissions; some lines emit two flavours of FS3261. The deduped
distinct `file(line,col)` count is **34**.)

**Two failure shapes** observed:
1. **Nullable `string`** from BCL APIs (`Environment.GetEnvironmentVariable`, regex
   `Groups[n].Value`, `Path.GetDirectoryName`, etc.) flowing into non-nullable `string`.
2. **Nullable reference values** — e.g. `Front/Governance.fs:185` `use proc =
   System.Diagnostics.Process.Start startInfo` returns `Process | null`, then `.StandardOutput`
   is dereferenced. `Engine/Model.fs:72` is a **signature mismatch**: impl infers
   `featureId: string | null` but the `.fsi` declares `string`.

**Decision per shape**:
- Nullable BCL `string` → pattern-match (`match … with null -> … | s -> …`) or `nonNull` /
  `Option.ofObj` with an explicit default. Never force-unwrap.
- `Process.Start` result → `match Process.Start startInfo with null -> Error … | proc -> …`
  (fail-fast per Constitution VII; do not silently swallow a null process).
- `Engine/Model.fs:72` signature mismatch → align the **inferred** nullness to the `.fsi`
  (the `.fsi` is the contract — make the impl's value provably non-null), so **no `.fsi`
  change** is needed (preserves "no surface-baseline change", FR-008/Framework-Governance).

**Rationale**: behaviour-preserving (FR-006) — pattern-matching the null case to the value's
existing effective behaviour changes no observable output; `Governance.Tests` stay green.
Removing the escape hatch (FR-009) is the only enforcement that uses the compiler itself, so it
cannot silently regress the way a grep test could be skipped.

**Alternatives considered**:
- `#nowarn "3261"` per file or blanket suppression — rejected by spec (FR-005): hides real
  nullness bugs and defeats `Nullable=enable`.
- Keep the escape hatch + add a grep-the-build-log test — rejected (user chose the stronger
  compiler-enforced gate); a test is skippable and lives outside the compile.
- Promote FS3261 globally in `Directory.Build.props` — out of scope (spec); other projects
  (e.g. runtime `src/**`) are not in this feature's blast radius.

## R3 — Stray pack-flow scratch file (§4.3 / FR-007, FR-008)

**Decision**: Treat `specs/053-v3-monolith-retirement/readiness/package/local-packages.md` as
**pack-flow scratch** (per spec Assumption); remove it and `.gitignore` the scratch location so
it cannot reappear as untracked.

**Evidence**:
- `git status --porcelain` shows `?? specs/053-v3-monolith-retirement/readiness/package/`.
- It is a local-NuGet inventory emitted by the version-bump/pack flow, not authored 053 evidence
  (053 is merged + closed; this file post-dates the merge, dated Jun 2 22:20).
- Because it sits under an `evidence-governance` readiness path, it escalates `Route` to
  `agent-ready` (review §4.3 confirms this is *why* the live `Route` did not show `inner-loop`).

**Existing `.gitignore` precedent** (so the new rule matches house style):
- `specs/*/readiness/logs/**` — regenerable logs ignored.
- `specs/*/readiness/**/readiness*.zip` — archives ignored.
- `specs/*/readiness/generated-consumer-validation/nuget-packages/` — generated nuget ignored.

**Decision**: add `specs/*/readiness/package/` (or `.../package/local-packages.md`) to
`.gitignore` under the existing Feature-046 evidence-hygiene block, mirroring those precedents,
then `git rm`/delete the stray file. Authored `.md` evidence elsewhere stays tracked (the rule is
scoped to the `package/` scratch subdir only — no broad `*.md` sweep, consistent with the
[[readiness-dir-hazard]] note).

**Alternatives considered**:
- Commit the file as genuine 053 evidence — rejected: 053 is closed/merged; this is regenerable
  pack inventory, and committing post-close evidence churns a landed feature's readiness set.
- Ignore the whole `readiness/package/` tree forever vs. just `local-packages.md` — choose the
  directory glob to also catch sibling scratch the same flow may emit, matching the `logs/**`
  precedent.

## R4 — Verification approach (cross-cutting)

**Decision**: Tier 2 (internal change). No `.fsi`/surface-baseline edits. `Route` will classify
this change after the stray file is resolved.

- **Pin parity (SC-001/002/003)**: a single reproducible grep/compare command; strengthen the
  existing `GeneratedProjectValidationTests.fs` test to assert exact version equality between the
  `#r` literal and the props `PackageVersion`; demonstrate the gate catching a deliberate
  mismatch and passing after revert; extend the `fs-skia-template-update` flow to bump both.
- **FS3261 (SC-004/005)**: before/after clean-build logs (34 → 0); escape hatch removed; full
  `Governance.Tests` green.
- **Clean tree (SC-006/007)**: `git status --porcelain` empty; a routine framework-internal diff
  routes to `inner-loop` via `Route`.

**Route / FAKE discipline**: per CLAUDE.md, run `./fake.sh build -t Route` first and run only the
gates it prints. This change touches `template/**`, governance `build/**`, a public-ish test, and
`.gitignore`, so `Route` will **escalate** (consumer-contract + governance paths) to the
maintainer-verify path. FAKE-backed targets run **sequentially** ([[fake-build-constraints]]).
