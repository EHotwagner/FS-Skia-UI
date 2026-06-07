# API doc coverage (SC-001 / FR-003) — feature 076

Supported-member doc-coverage summary across the 10 published packages. The
"supported" set is defined by the per-package surface baselines; coverage is the
fraction of supported public members carrying a non-empty `<summary>` sourced from
`///` on the member's `.fsi` declaration.

- **Authoritative command**: `dotnet fsdocs build --strict --eval`
  (`FsDocsWarnOnMissingDocs=true` → a missing summary on a supported member is a
  build-failing warning).
- **Artifact path**: this file + `readiness/logs/fsdocs-build.txt`.
- **Failure class**: undocumented-supported-member (empty stub) → SC-001 fail.
- **Next action**: add `///` to the named `.fsi` member and rerun the strict build.

## Version-behaviour note (fsdocs 22.1.0)

The plan (R5) assumed `--strict` + `FsDocsWarnOnMissingDocs` would **fail** the
build on an undocumented supported member. Empirically, fsdocs 22.1.0 emits
missing-doc diagnostics as **non-fatal warnings** (the strict build exits 0). It
also only warns at the **parameter** level (`missing docs for parameter`), never on
a missing member `<summary>`. So SC-001 ("every supported member has a non-empty
summary") is verified by **source coverage** (every `val`/`member`/`abstract`/`type`
in the `.fsi` carries a `///`), not by a build-failure gate. The
`DocsArchitectureAnalysisTests` governance test (SC-002) and the source-coverage
audit below are the enforceable signals.

## Coverage table (member-level `///` on `.fsi`, post-T011)

`val` / `member` / `abstract` declarations per package and their `///` coverage.
Counts from the `.fsi` signature files (the emission source for signatured modules).

| Package | Members | Documented | Undocumented |
|---|---|---|---|
| Scene | 82 | 82 | 0 |
| SkiaViewer | 55 | 55 | 0 |
| Elmish | 3 | 3 | 0 |
| Input | 18 | 18 | 0 |
| KeyboardInput | 6 | 6 | 0 |
| Layout | 33 | 33 | 0 |
| Controls | 334 | 334 | 0 |
| Controls.Elmish | 13 | 13 | 0 |
| Testing | 24 | 24 | 0 |
| SkillSupport | 18 | 18 | 0 |
| **Total** | **586** | **586** | **0** |

Public `type` declarations: 1 gap found and closed (`PointerDiagnostic` in
`src/Controls/Pointer.fsi`). The 20 `DesignTokens.Light`/`Dark` token vals were the
only `val` gaps (Controls) — closed in T011.

**STATUS: complete** — 0 undocumented authored supported members. SC-001 met.

## Known non-authorable / tool-limitation warnings (NOT SC-001 stubs)

The strict build still prints these warnings; they are **not** authored empty
stubs and cannot be closed with a `///`:

- `FD0001: no documentation for 'P:….IsError' / '.IsFailed' / '.IsX'` —
  compiler-**synthesized** union-case discriminator properties; no source member
  exists to annotate.
- `Could not read comments from entity '….CommandId': the entity System.String was
  not registered` — type **abbreviations** of primitives (`type CommandId =
  string`); an fsdocs cross-reference limitation, not a missing summary.
- `missing docs for parameter 'x'` / `a parameter was missing a name` — `<param>`
  completeness; many are unnamed curried/tupled parameters that cannot carry a
  `<param>` without a **signature change** (out of scope, FR-004). Summaries (the
  SC-001 bar) are present.
