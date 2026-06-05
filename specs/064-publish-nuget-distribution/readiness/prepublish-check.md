# Pre-publish consistency check — fail+pass (US4, SC-005)

`PrePublishCheck` over the real release set: a deliberately-skewed set **fails** naming the
offending package/field and **aborts**, and the restored set **passes**. The four rules are
`PinParity`, `EnginePinMatch`, `NoMachineLocalPath`, `RequiredMetadata` (see
[contracts/prepublish-check.md](../contracts/prepublish-check.md)).

## Skew introduced

Set the single-source `<FsSkiaUiVersion>` in `template/base/Directory.Packages.props` to a
wrong value (`0.1.66-preview.1`) while the shipped libraries are `0.1.67-preview.1`. Because
the property is the single source, this skews **every** library pin and the build-engine pin
at once.

## FAIL transcript (publish aborts, naming each offender)

```
# Pre-publish consistency check

Status: FAIL — 11 finding(s); the publish is aborted.

| Rule | Package | Field | Detail |
|------|---------|-------|--------|
| PinParity | `FS.Skia.UI.Build` | Directory.Packages.props pin | pin resolves to 0.1.66-preview.1 but the shipped version is 0.1.67-preview.1 |
| PinParity | `FS.Skia.UI.SkillSupport` | Directory.Packages.props pin | pin resolves to 0.1.66-preview.1 but the shipped version is 0.1.67-preview.1 |
| PinParity | `FS.Skia.UI.Scene` | Directory.Packages.props pin | ... |
| PinParity | `FS.Skia.UI.SkiaViewer` | Directory.Packages.props pin | ... |
| PinParity | `FS.Skia.UI.Elmish` | Directory.Packages.props pin | ... |
| PinParity | `FS.Skia.UI.KeyboardInput` | Directory.Packages.props pin | ... |
| PinParity | `FS.Skia.UI.Layout` | Directory.Packages.props pin | ... |
| PinParity | `FS.Skia.UI.Controls` | Directory.Packages.props pin | ... |
| PinParity | `FS.Skia.UI.Controls.Elmish` | Directory.Packages.props pin | ... |
| PinParity | `FS.Skia.UI.Testing` | Directory.Packages.props pin | ... |
| EnginePinMatch | `FS.Skia.UI.Build` | build.fsx / FsSkiaUiVersion | build.fsx resolves engine version 0.1.66-preview.1 but the shipped FS.Skia.UI.Build version is 0.1.67-preview.1 |

System.Exception: PrePublishCheck FAIL (11 finding(s)); the publish is aborted: ...
```

The target throws (non-zero exit) — `Publish` depends on `PrePublishCheck`, so the publish
**cannot proceed** while it fails.

## PASS transcript (consistency restored)

Restore `<FsSkiaUiVersion>` to `0.1.67-preview.1`:

```
# Pre-publish consistency check

Status: PASS — no findings; the release set is internally consistent and the publish may proceed.
```

## Other skew classes (same shape)

- **`NoMachineLocalPath`** — if the consumer-emitted `NuGet.config` carried a machine-local
  path (`key="local"` / `nuget-local` / an absolute path), the check names
  `template / NuGet.config:local`. (Verified by the `T027 NoMachineLocalPath` unit test;
  the shipped public-feed config produces no finding.)
- **`RequiredMetadata`** — a blank license / repository URL / authors / description, or a
  packable project with **no `README.md`**, names `<package> / <field>`. (Verified by the
  `T027 RequiredMetadata` unit test; all 11 libs + the template now carry a README + metadata,
  so the live check produces no finding.)

## Verdict

`PrePublishCheck` fails naming the offending package/field for the skew, aborts the publish,
and passes once consistency is restored (SC-005). A malformed/inconsistent release can never
reach the push edge.
