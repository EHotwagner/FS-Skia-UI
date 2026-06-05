# Distribution doc sweep (US5, T035, SC-006)

Swept in-repo docs for any that still present distribution as **deferred** or
**local-feed-only as the consumer story**. Disposition:

| Doc | Before | Action |
|-----|--------|--------|
| `README.md` (§Get started) | "From a clone… pack to a local feed… add `~/.local/share/nuget-local`" as the install path | **Updated** → `dotnet new install FS.Skia.UI.Template` from public nuget.org; local-feed scoped to framework developers; links `docs/distribution.md`. |
| `template/base/README.md` (generated project) | "references FS.Skia.UI preview packages from the configured NuGet sources… for local framework development add `~/.local/share/nuget-local`" | **Updated** → public nuget.org by default + single-source pin + `docs/UPGRADING.md`; local feed scoped to framework developers. |
| `docs/adr/0001-…distribution.md` | "Distribution… exercised only in Stage 4/5… does not pack or publish" | **Superseded** by a feature-064 addendum recording the implemented publish path. |
| `docs/distribution.md` | _did not exist_ | **Authored** — consumer install, feeds/channel, single-edit upgrade, maintainer release+publish flow, publish-target reference. |
| `docs/reports/speckit.md`, `testing.md`, `V2Analysis.md` | mention "deferred distribution automation" | **Left as-is** — these are dated V2-era analysis reports describing the state *at the time of writing*, not the current consumer story. They are archival; rewriting historical reports would falsify the record. The current consumer narrative (README, generated README, ADR 0001, distribution.md) no longer presents distribution as deferred or local-feed-only. |

## Verdict

No doc still presents distribution as deferred or local-feed-only **as the consumer story**
(SC-006). The consumer install commands, the public feed, the preview/stable channel, and the
maintainer publish sequence are documented in `docs/distribution.md` and the ADR 0001 addendum.
