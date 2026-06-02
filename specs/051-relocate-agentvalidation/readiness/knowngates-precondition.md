# `knownGates` precondition — Stage-0 deferral unblocked (FR-008 / SC-005 / SC-007)

The Stage-0 `PerPackageSurfaceDiff` Route rule (feature 048, T016) was deferred because rendering a
new gate into `validation.contract.yml` would require extending the `knownGates` allowlist, which then
lived in the **runtime monolith** (`src/Lib/AgentValidation.fs`), violating SC-007 (`src/**`
byte-unchanged). This feature relocates `knownGates` into the governance library, removing that
coupling.

## `knownGates` now lives in the governance library

```
$ grep -rn "knownGates" build/Governance/AgentValidation.fs
361:    let knownGates =
481:                if knownGates |> List.contains gate then

$ grep -rn "knownGates" src/Lib        # source only
(none)
```

(The `src/Lib/bin`/`obj` `FS.Skia.UI.xml` hits are gitignored stale build artifacts, not source.)

## Precondition statement

- Adding a gate name to the `knownGates` allowlist now edits only
  `build/Governance/AgentValidation.fs` — a **governance/build path**.
- Rendering that gate into `validation.contract.yml` (generated from `Routing.fs`) would touch only
  `build/Governance/**` and the generated `validation.contract.yml` — **no** `src/**` runtime file.
- Therefore the Stage-0 per-package Route rule the 048 finding deferred is **unblocked**; the rule
  itself, the hard-gate enforcement, and the `src/Lib` deletion remain **Stage 5** (this feature wires
  nothing — SC-005).

## `validation.contract.yml` unchanged this stage (SC-007)

`git status validation.contract.yml` is clean — the contract is **not** edited, so its currency versus
`Routing.fs` (enforced by `TargetMetadataDrift`) is preserved.
