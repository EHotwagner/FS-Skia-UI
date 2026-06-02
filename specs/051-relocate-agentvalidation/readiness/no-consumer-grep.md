# No-consumer grep — the build→runtime coupling is gone (FR-007 / SC-004)

After the move, **no** `FS.Skia.UI.AgentValidation` namespace consumer remains anywhere in the source
tree, and `Governance.Tests` no longer references the runtime monolith.

## Namespace consumers

```
$ grep -rn "FS.Skia.UI.AgentValidation" --include=*.fs --include=*.fsi --include=*.fsproj --include=*.fsx .
(no matches outside git history)
```

The sole former consumer — `tests/Governance.Tests/AgentValidationFrameworkTests.fs` — now opens
`FS.Skia.UI.Build.AgentValidation`. (Stale `bin/`/`obj/` `FS.Skia.UI.xml` doc artifacts are gitignored
build output, not source, and regenerate on the next pack.)

## Monolith reference

```
$ grep -n "Lib.fsproj" tests/Governance.Tests/Governance.Tests.fsproj
(no matches)
```

`Governance.Tests.fsproj` referenced `..\..\src\Lib\Lib.fsproj` **solely** for `AgentValidation`; that
`ProjectReference` is removed, leaving only `..\..\build\Governance\FS.Skia.UI.Build.fsproj` for this
capability. The suite builds and passes with no link back into `src/Lib`
(`./fake.sh build -t Dev`, `readiness/logs/dev.log`) — proving the parser without the monolith
reference (SC-004).

## Residual path mentions (out of scope)

`grep -rn "src/Lib/AgentValidation"` still matches **prose-only** references in prior-feature
`specs/**` and `docs/**` (historical task logs, baselines, ADR 0009) and one stale comment in
`build/Governance/Routing.fs:214`. `Routing.fs` is deliberately **unchanged** this stage
(currency vs `validation.contract.yml` preserved — SC-007); the comment is Stage-5 cleanup. None of
these are code consumers.
