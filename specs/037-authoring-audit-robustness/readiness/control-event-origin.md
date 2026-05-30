# US3 Evidence — ControlEventOrigin Qualified Access

Covers FR-007, FR-008, FR-010, SC-004. The single public-contract change in this
feature.

## Contract change

`[<RequireQualifiedAccess>]` added to `ControlEventOrigin` in
`src/Controls/Types.fsi` and `src/Controls/Types.fs`, aligning it with its
sibling DUs. The six repo usages of unqualified cases were qualified:

- `tests/Controls.Tests/InteractionTests.fs` — `ControlEventOrigin.Pointer/Text/Keyboard`
- `samples/DemoReel/Program.fs` — `ControlEventOrigin.Keyboard/Text`
- `samples/ControlsGallery/Program.fs` — `ControlEventOrigin.Pointer`

## SC-004 — mixed Scene/Controls open order

Fixture: `readiness/fsi/mixed-scene-controls-text-collision.fsx` (opens
`FS.Skia.UI.Scene` then `FS.Skia.UI.Controls`, Controls last; constructs an
unqualified `Text` node + a shared `FS.Skia.UI.Scene.Rect` bounds literal).

| Phase | Result | Log |
|---|---|---|
| Pre-fix (failing-first) | `error FS0003: This value is not a function and cannot be applied. It has type 'ControlEventOrigin'` (the opaque error) | `readiness/fsi/mixed-scene-controls-text-collision.prefix.log` |
| Post-fix | resolves to the Scene `Text` constructor (`scene text node resolved: Text ((0.0, 0.0), "Hello, scene", …)`) | `readiness/fsi/mixed-scene-controls-text-collision.postfix.log` |

## Surface baseline (FR-010 / T020)

`scripts/refresh-surface-baselines.fsx` regenerated the baselines from the
post-fix assemblies. `RequireQualifiedAccess` is an attribute, not a
surface-name change, so `readiness/surface-baselines/FS.Skia.UI.Controls.txt`
and `FS.Skia.UI.txt` are **unchanged** — the `FS.Skia.UI.Controls.ControlEventOrigin`
and `…ControlEventOrigin+Tags` entries remain. `./fake.sh build -t
PackageSurfaceCheck` → `Status: Ok`.

## FR-008 — shared structurally-typed types

`docs/generated-apps.md` documents reusing the shared `FS.Skia.UI.Scene.Rect`
type (rather than a look-alike record) to avoid record-field inference hijack,
at the mixed Scene/Controls authoring point of use.

## Reversal (FR-010)

`specs/035-api-discovery-names/readiness/name-collision-safety.md` records the
reversal of spec 035's `consumer-guidance` decision for `ControlEventOrigin`
only, with rationale and a before/after consumer migration snippet. No other
spec 035 collision decision changes.

## Build evidence

- `dotnet build src/Controls/Controls.fsproj -c Debug` → Build succeeded.
- `dotnet test tests/Controls.Tests/Controls.Tests.fsproj` → 36 passed.
- `dotnet build samples/DemoReel` and `samples/ControlsGallery` → Build succeeded
  (qualified usages compile).
- `./fake.sh build -t PackageSurfaceCheck` → Ok.

> Note: the full `./fake.sh build -t Dev` aggregate is currently blocked by
> pre-existing branch test failures unrelated to spec 037 (README-rewrite commits
> left `Governance.Tests` README/canonical-command-surface assertions failing,
> and `.claude/commands/speckit-implement.md` is missing). The US3-specific build,
> tests, surface check, and FSI vertical slice above are all green.
