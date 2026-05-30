---
title: Controls Boundary Refactor Process Report
category: Design
categoryindex: 4
index: 7
description: Process issues, tooling failures, mitigations, and follow-up recommendations from the Controls boundary refactor.
---

# Controls Boundary Refactor Process Report

This report captures the problems encountered while implementing
`specs/011-controls-boundary-refactor`, with emphasis on process and tooling
failures. The final feature work reached an implemented state, but the local
aggregate verification path was degraded by memory and process pressure. The
most important distinction is that focused product and governance gates passed,
while the broad `Verify` and `Ci` aggregates failed in the local runner while
starting test infrastructure.

## Final Status

- All 85 Spec Kit tasks were marked complete.
- The final evidence graph reported `85[X]`, zero `[S]`, and zero `[S*]`.
- `EvidenceAudit` passed after T085 in
  `specs/011-controls-boundary-refactor/readiness/logs/t085-evidence-audit-final.txt`.
- Focused gates passed for package surface, FSI transcripts, Controls catalog,
  Controls interaction, Controls rendering, dependency governance, template
  checks, generated product checks, generated guidance, template drift,
  evidence graph, and evidence audit.
- Final local aggregate `Verify` and `Ci` remained environment failures because
  VSTest and CoreCLR startup repeatedly failed under local memory/process
  pressure.

## OOM And Process Pressure

The most disruptive process problem was local process exhaustion during broad
test execution. Repeated `./fake.sh build -t Verify` and
`./fake.sh build -t Ci` attempts reached the `Test` stage and then failed while
starting `Lib.Tests`. The observed diagnostics included:

- `System.OutOfMemoryException` while VSTest was starting socket infrastructure.
- `Failed to create CoreCLR, HRESULT: 0x8007000E`.
- Later local governance/package/template attempts also showed CoreCLR startup
  failures, including `HRESULT: 0x80070008`.
- Many defunct `dotnet` processes accumulated under PID 1, leaving the shell in
  a degraded state even after no live `fake build` or `dotnet test` sessions
  remained.

The failures were not assertion failures in the Controls implementation. They
occurred before useful test execution, while the .NET test platform was trying
to create processes, threads, sockets, or CoreCLR instances. The practical
effect was that the local runner could no longer provide an authoritative broad
aggregate signal without being reset.

Mitigations used during the work:

- Prefer focused gates over broad aggregate targets while the runner was under
  pressure.
- Run high-risk test scopes serially with `dotnet test -m:1` where practical.
- Capture logs, durations, and exit codes for each focused command so evidence
  could be audited even when aggregate gates were unreliable.
- Treat final `Verify` and `Ci` as environment failures, not product evidence,
  and document the caveat in the readiness summary.

Recommended follow-up:

- Run final `Verify` and `Ci` in a fresh CI runner or fresh local container.
- Add a preflight diagnostic for process count, zombie count, memory, and
  `ulimit` values before broad gates.
- Keep focused gates as first-class targets so maintainers can isolate product
  failures from infrastructure exhaustion.
- Consider reducing VSTest socket/thread pressure for local aggregate runs, or
  split broad aggregates into smaller fresh-process stages.

## FAKE Runner And Package Cache Problems

Many successful FAKE target logs still began with:

`Could not load types of compiled script: netstandard, Version=2.0.0.0...`

Target execution usually continued after this message, so the line became
noise that could obscure the real failure later in the log. It should be
treated as a runner/tooling warning unless accompanied by a nonzero target
exit.

Early in the work, local FAKE execution also failed because the package cache
was missing `FSharp.Core/6.0.7` under `/home/developer/.nuget/packages`. This
blocked early `Dev` and generated guidance checks. The immediate mitigation was
to restore the missing package through an ignored scratch project. After that,
the affected focused checks could run far enough to expose real governance and
implementation issues.

Recommended follow-up:

- Make FAKE bootstrap package dependencies explicit and restorable without
  scratch-project intervention.
- Suppress, fix, or classify the repeated `netstandard` script-load warning so
  logs highlight actionable failures.
- Add a lightweight bootstrap target that validates FAKE dependencies before
  running feature gates.

## Target Graph Shape

The original target graph made several focused checks depend on broad build
work. That increased memory pressure and made the feedback loop worse. A
focused check could fail because the broad prerequisite exhausted the runner
before the check itself had a chance to execute.

The clearest example was `ControlsRenderingCheck`. Under FAKE, the target was
coupled to `Build` and used stale `--no-build` assumptions. Direct serial
rendering tests passed, but the target path failed in the aggregate graph.

Mitigations used during the work:

- Narrow target dependencies for `Test`, `PackageSurfaceCheck`,
  `FsiTranscripts`, and the Controls split checks.
- Build or restore the specific project needed by each focused target instead
  of forcing broad prebuilds.
- Update `ControlsCatalogCheck`, `ControlsInteractionCheck`, and
  `ControlsRenderingCheck` to run focused `dotnet test -m:1 --no-restore`
  commands instead of depending on the broad `Build` target.

Recommended follow-up:

- Keep focused verification targets independent from broad aggregates unless a
  broad prerequisite is truly required.
- Add command-contract tests that protect focused target membership and prevent
  accidental recoupling to expensive aggregate targets.
- Make stale `--no-build` or `--no-restore` assumptions visible in target
  diagnostics.

## Governance False Positives

Several governance checks found real process weaknesses in the check logic
rather than product regressions.

### Dependency Report

`DependencyReport` initially reported a Controls dependency leak on `Lib`.
The cause was substring matching: the scanner saw `Lib` inside
`<OutputType>Library</OutputType>`. The check was updated to inspect concrete
`ProjectReference` and `PackageReference` patterns rather than arbitrary text.

Recommended follow-up:

- Prefer structured XML/project parsing for dependency policy.
- Avoid substring scans for names like `Lib`, `Charts`, or `Scene` unless they
  are anchored to a known syntax.

### Template Check

`TemplateCheck` initially failed on the generated `sample-pack` profile because
the copied-content scanner treated intended `sample-pack` files under
`samples/` as forbidden framework sample content. The scanner now allows
`samples/` for the explicit `sample-pack` profile while still rejecting copied
framework sample projects in ordinary generated products.

Recommended follow-up:

- Keep generated product scanners profile-aware.
- Treat "copied framework implementation" and "intended generated sample-pack
  content" as separate rules.

### Generated Product Inventories

Generated file-list inventories initially did not include the product source
markers expected by governance: `RichText.create`, `LineChart.create`,
`GraphView.create`, `DataGrid.create`, and `ControlsElmish.program`.
The inventories were expanded to include product source and tests, not only the
outer generated file list.

Recommended follow-up:

- Keep generated inventory reports tied to the exact product behavior that
  governance expects.
- Include both source and tests when a check claims generated products exercise
  public guidance.

## Stale Boundary Evidence

The final stale scan found active references that contradicted the Controls
ownership boundary:

- Governance memory still mentioned `fs-skia-charts`.
- Architecture documentation still described the old package shape.
- Tracked legacy `src/Charts/*` and `tests/Charts.Tests/*` files remained in
  the active source tree.

These were resolved by updating `.specify/memory/constitution.md`, updating
`docs/reports/architecture.md`, and removing the remaining tracked legacy Charts source
and tests. After that cleanup, `EvidenceAudit` passed.

Recommended follow-up:

- Run stale boundary scans before marking late audit tasks complete.
- Include governance memory and architecture docs in refactor cleanup, not only
  source and tests.
- Require removed packages to have explicit active-tree deletion evidence.

## Evidence Handling

The evidence process worked, but it exposed a useful reporting rule: environment
failures must be separated from product failures. The final aggregate logs were
important, but they were not good evidence of broken Controls behavior because
the failures happened while test infrastructure was starting.

What worked:

- Focused target logs gave concrete pass/fail evidence despite aggregate
  instability.
- The evidence graph and audit prevented synthetic evidence from being counted
  silently.
- The final readiness summary recorded the aggregate caveat rather than hiding
  it.

What should improve:

- Add an explicit "environment failure" verdict category for readiness logs.
- Record process-health diagnostics next to broad aggregate logs.
- Make the recommended re-run environment clear when local evidence is degraded:
  fresh shell, fresh container, or CI runner.

## Recommended Next Steps

1. Re-run `./fake.sh build -t Verify` and `./fake.sh build -t Ci` on a clean
   runner before treating the broad aggregate signal as final.
2. Add a process-health preflight target that reports memory, process count,
   zombie count, thread limits, and relevant `ulimit` values.
3. Keep focused gates independent and serial where they are most useful for
   local development.
4. Replace remaining substring-based governance checks with syntax-aware
   parsing where practical.
5. Keep generated product validation profile-aware, especially for `sample-pack`.
6. Classify runner/bootstrap warnings separately from target failures so logs
   stay actionable.
