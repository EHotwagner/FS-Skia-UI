# SC-006 — no select-tier.fsx, no `dotnet fsi` selector, no FSharp.Compiler

The routing logic is compiled F# in `FS.Skia.UI.Build` (`Routing.fs` /
`ContractView.fs`), reached in-process from `build.fsx`. No throwaway script and
no FSharp Compiler Services dependency are introduced.

    $ find . -name "select-tier.fsx"
      (none)

    $ grep -rIn "FSharp.Compiler" build/Governance/*.fs build/Governance/*.fsi \
        build/Governance/*.fsproj build.fsx tests/Governance.Tests/*.fs
      (none)

    $ grep -rIn "dotnet fsi" build/Governance/Routing.fs build/Governance/ContractView.fs \
        build/Governance/Routing.fsi build/Governance/ContractView.fsi
      (none)

`FS.Skia.UI.Build.fsproj` references only `YamlDotNet` (pre-existing) — no new
package and no `FSharp.Compiler.*`. The pre-existing
`scripts/refresh-surface-baselines.fsx` invoked by `RefreshSurfaceBaselines` is a
surface-baseline script, not a tier selector, so SC-006 is satisfied: the tier
selection logic itself is compiled, build-time-checked F#.
