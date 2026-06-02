// GeneratedProductContract.fsi — feature 046 (US2, FR-004/005/006, ADR 0003).
// The versioned generated-product structural contract: a typed schema_version,
// per-rule lifecycle (Required | Deprecated | Removed) with a deprecation window,
// and an embedded typed changelog. Build-tooling scope (NOT a tracked product
// surface baseline). Consumed by GeneratedProduct.runScanV3GeneratedProducts so a
// product that violates only a Deprecated rule warns instead of failing.
module FS.Skia.UI.Build.GeneratedProductContract

/// Contract schema version, decoupled from the package marketing Version (ADR 0003).
type ContractSchemaVersion =
    { Major: int
      Minor: int }

/// Lifecycle of a single structural rule (FR-005).
type RuleLifecycle =
    | Required
    | Deprecated of removalVersion: ContractSchemaVersion
    | Removed

/// One structural rule wrapped with its lifecycle.
type StructuralRule =
    { RuleId: string
      Lifecycle: RuleLifecycle
      Description: string }

/// The kind of change recorded in the typed changelog (FR-006).
type ContractChangeKind =
    | Added
    | DeprecatedRule
    | PromotedToRequired
    | RuleRemoved

/// One typed changelog entry — no sidecar file.
type ContractChangelogEntry =
    { Version: ContractSchemaVersion
      RuleId: string
      Change: ContractChangeKind
      Note: string }

/// The whole versioned contract.
type GeneratedProductContract =
    { SchemaVersion: ContractSchemaVersion
      Rules: StructuralRule list
      Changelog: ContractChangelogEntry list }

/// Outcome of evaluating a single violated rule against the contract (FR-005).
type RuleOutcome =
    | Pass
    | Warn of string   // deprecated rule, window open — names the removal version
    | Fail

/// Order comparison for schema versions (Major then Minor).
val compareVersion: ContractSchemaVersion -> ContractSchemaVersion -> int

/// Render `version` as `"{Major}.{Minor}"`.
val renderVersion: ContractSchemaVersion -> string

/// The current contract value (wraps the existing structural-rule ids).
val current: GeneratedProductContract

/// Classify a product that violates *only* `ruleId` against the contract:
/// Required -> Fail; Deprecated with window open -> Warn naming the removal version;
/// Deprecated with window closed -> Fail; Removed / unknown rule -> Pass (not evaluated).
val classifyViolation: contract: GeneratedProductContract -> ruleId: string -> RuleOutcome

/// Render the discoverable contract header (schema_version + changelog summary) for
/// GeneratedProductCheck output (SC-003).
val renderContractHeader: GeneratedProductContract -> string

/// Pure changelog<->schema-version consistency check over a contract (FR-006, SC-011):
/// every breaking entry (PromotedToRequired / RuleRemoved) carries a Version strictly
/// greater than the prior schema version, and SchemaVersion >= the max entry version.
/// Returns the list of consistency violations (empty => consistent).
val changelogConsistencyFindings: GeneratedProductContract -> string list
