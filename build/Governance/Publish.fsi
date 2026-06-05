// Publish.fsi — pure publish-plan helpers (feature 064, FR-001/FR-002; Principle II/IV).
//
// Build-tooling only. Every function here is PURE over its inputs: env reading, the
// anonymous feed read, and `dotnet nuget push` all live at the Interpret edge. The
// interpreter resolves the shipped (id, version) set and a `feedHasVersion` predicate, then
// calls `buildPlan`; dry-run renders `renderPlan` and pushes nothing.
module FS.Skia.UI.Build.Publish

open FS.Skia.UI.Build.Engine.Model

/// The nuget.org service-index URL the feed defaults to (FR-001).
val defaultFeedUrl: string

/// True when the feed URL is a local directory path (not an http(s) URL) — selects the
/// directory-listing read over the flat-container HTTP read (research R2).
val isLocalFeedUrl: feedUrl: string -> bool

/// Derive the anonymous-read base from the push feed URL: the nuget.org flat-container base
/// for an http(s) service index, or the directory path itself for a local feed (R2).
val deriveReadUrl: feedUrl: string -> isLocalFeed: bool -> string

/// Build a PublishConfig from an environment lookup (pure given the lookup) — FSSKIA_PUBLISH_FEED,
/// FSSKIA_PUBLISH_API_KEY, FSSKIA_PUBLISH_DRYRUN.
val configFromEnv: lookup: (string -> string option) -> PublishConfig

/// Validation (data-model PublishConfig "Validation"): a non-dry-run with `ApiKeyPresent = false`
/// returns an error message naming the missing key; otherwise None. Dry-run never needs a key.
val validateConfig: config: PublishConfig -> string option

/// Build the per-package plan: Skip when the feed already has the version (idempotency, FR-002),
/// else Push. `feedHasVersion id version` is the anonymous-read result supplied by the edge.
val buildPlan: packages: (string * string) list -> feedHasVersion: (string -> string -> bool) -> PublishPlanRow list

/// The flat-container `index.json` published-version set (`{"versions":[...]}`); 404/empty ⇒ empty.
val parseFlatContainerVersions: json: string -> Set<string>

/// Render the dry-run / plan as a markdown report (12-row table + the config summary).
val renderPlan: config: PublishConfig -> rows: PublishPlanRow list -> string
