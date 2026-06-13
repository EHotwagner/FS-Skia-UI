# US3 validation — template stays consistent (T017 / T018)

**Story**: A freshly generated `dotnet new fs-skia-ui` project restores and builds against the current
pins.

**Skill loaded before work**: `fs-skia-template-update` (`.agents/skills/fs-skia-template-update/SKILL.md`)
— see `skill-loading-evidence.md` (loaded 08:46:46Z, work started 08:47:20Z).

## T017 — template pin refresh: NOT needed (FR-006)

The generated template's `template/base/Directory.Packages.props` directly pins only:

- the repo's own `FS.Skia.UI.*` packages, all via the single `<FsSkiaUiVersion>` property
  (`0.1.121-preview.1`) — bumped at **merge time** by `speckit-merge`, not by this feature; and
- the test cluster `Expecto` (10.2.2), `Microsoft.NET.Test.Sdk` (17.11.1), `YoloDev.Expecto.TestSdk`
  (0.15.3).

None of feature 115's applied/adopted pins is a direct template pin:

- **FSharp.Core** — SDK/transitive; not template-pinned.
- **Microsoft.Extensions.FileSystemGlobbing** — build-tooling adopt-set (`build/**` only); not shipped in
  any generated product, not template-pinned.
- **YamlDotNet 18** / **Fable.Elmish 5** — transitive through the published `FS.Skia.UI.*` packages; not
  directly template-pinned. They reach generated projects only after the `FS.Skia.UI.*` packages are
  repacked at the post-merge version bump.
- The **deferred** test cluster (reverted to 10.2.2 / 17.11.1 / 0.15.3) **matches** the template's direct
  test pins exactly — so no drift is introduced.

The `fs-skia-template-update` skill is explicit: leave `Expecto` / `Microsoft.NET.Test.Sdk` /
`YoloDev.Expecto.TestSdk` unchanged unless asked, and the `<FsSkiaUiVersion>` bump is the merge-time
action. Therefore **no `template/**` edit is made by this feature** (FR-006). `TemplateDrift` (a routed
gate, T010) confirmed template currency = **Ok**.

## T018 — generated project restores + builds (SC-004)

Run the authoritative template/product validators (these pack the current source — including adopted
YamlDotNet 18 + Fable.Elmish 5 — to the local feed, generate the profiles, and restore+build+test them):

| Gate | Result | Log |
|---|---|---|
| TemplateCheck | **Ok** | `readiness/logs/template-check.txt` |
| GeneratedProductCheck | **Ok** | `readiness/logs/generated-product-check.txt` |

A freshly generated `dotnet new fs-skia-ui` project restores and builds against the updated pins, with the
adopted transitive YamlDotNet 18 / Fable.Elmish 5 flowing through the packed `FS.Skia.UI.*` packages. No
`NU1###` resolution error, no compile error.

Result: **PASS** — generated project restores and builds against the current pins; no template pin refresh
was required (SC-004, FR-006).
