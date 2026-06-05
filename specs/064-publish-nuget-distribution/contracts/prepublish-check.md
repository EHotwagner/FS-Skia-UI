# Contract: `PrePublishCheck` target + `PrePublish` validator

**Type**: governed FAKE target backed by `build/Governance/PrePublish.fs(i)`. Runs **before** any push and
**aborts** the publish on the first non-empty finding set, **naming the offending package/field**.

## Checks (all must pass; FR-006/010)

| Rule | Asserts | Failure names |
|------|---------|---------------|
| `PinParity` | each `FS.Skia.UI.*` pin in `template/base/Directory.Packages.props` == the version being shipped for that package (`packProjects` `<Version>`). | the package + `Directory.Packages.props` pin. |
| `EnginePinMatch` | `build.fsx`'s resolved engine version == shipped `FS.Skia.UI.Build` version (single-source `<FsSkiaUiVersion>`, FR-004). | `build.fsx` / `FsSkiaUiVersion`. |
| `NoMachineLocalPath` | the **consumer-emitted** `NuGet.config` (from `GeneratedProduct.fs`) has **no** absolute local feed path (FR-003). | `NuGet.config:local`. |
| `RequiredMetadata` | every packable project **and** the template carry non-blank license / repository-url / authors / description / per-package README (FR-010). | the package + the blank field. |

## Composition

Reuses / extends `TemplateCheck` + existing pin-parity validation rather than duplicating them
(`PrePublishCheck` direct-prerequisite = `TemplateCheck`). Recommended (not required) metadata
(tags/icon) produces a warning, not a failure.

## Output

Pass: empty finding list, exit success, publish may proceed. Fail: structured findings (one per
`PrePublishFinding`), abort. Evidence: `readiness/prepublish-check.md` — a **fail+pass** transcript over a
deliberately-skewed set (US4, SC-005): introduce a skew (pin≠shipped version, OR blank required metadata,
OR machine-local path in the emitted config), confirm the named failure, restore, confirm pass.
