# Task Generation Readiness

## T003 Assumptions

Story grouping:

- US1 proves generated graphical app default launch through the persistent host.
- US2 prevents bounded evidence from substituting for interactive readiness.
- US3 separates unsupported-host diagnostics from missing product/package capability.
- US4 preserves bounded evidence workflows as explicit helper paths.

Skill confidence review:

- High-confidence SkiaViewer work uses `fs-skia-skiaviewer`.
- Generated MVU host work adds `fs-skia-elmish`.
- Keyboard-capable generated app work adds `fs-skia-keyboard-input`.
- Pure scene construction adds `fs-skia-scene`.
- Generated validation helper work adds `fs-skia-testing`.
- Evidence graph/audit/guidance work uses the corresponding Spec Kit skills.

Valid-empty skill dispositions:

- Documentation, inventory, and readiness filing tasks with `[skillist: []]` are accepted-empty unless implementation discovers a narrower capability owner.
- Empty-skill tasks still require real evidence and task graph refreshes.

Risk-level assumptions:

- The feature is broad Tier 1 because it touches package API, template behavior, governance checks, docs, and readiness gates.
- Focused validation is required per changed surface; broad validation is required before completion.
- Aggregate command failures must be recorded as non-authoritative when focused validation provides the decisive result.

## T010 Graphical Viewer Guidance Expectations

Updated active and preset task templates so future graphical viewer features must generate a distinct persistent graphical launch task reachable from the default executable path.

Bounded smoke, first-frame, frame-count, scene metadata, and unsupported-host diagnostics are explicitly documented as helper evidence that cannot complete interactive graphical readiness.

Verification:

- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated task guidance requires persistent viewer launch evidence separately from bounded helpers"` passed.

## T029 Persistent Launch Guidance

The active and preset task templates now require generated graphical viewer
features to include a distinct persistent graphical launch task reachable from
the default executable path. They also reject default executable paths that only
print metadata, count controls, run bounded smoke, emit scene evidence, or exit
without a persistent launch attempt.

Verification:

- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated task guidance"` passed.
