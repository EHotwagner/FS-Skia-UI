module GeneratedProductContractTests

// Feature 046 (US2, T013 + T027, FR-004/005/006, SC-011): failing-first typed unit
// tests for the versioned generated-product contract. Assert typed RuleOutcome /
// changelog values over literal contracts — no report-text string matching.
open Expecto
open FS.Skia.UI.Build.GeneratedProductContract

let private v (major, minor) = { Major = major; Minor = minor }

/// A literal contract at schema_version 1.0 with one Required, one Deprecated
/// (removal at 2.0), and one Removed rule.
let private sample schemaVersion =
    { SchemaVersion = schemaVersion
      Rules =
        [ { RuleId = "req"; Lifecycle = Required; Description = "a required rule" }
          { RuleId = "dep"; Lifecycle = Deprecated(v (2, 0)); Description = "a deprecated rule" }
          { RuleId = "gone"; Lifecycle = Removed; Description = "a removed rule" } ]
      Changelog =
        [ { Version = v (1, 0); RuleId = "req"; Change = Added; Note = "baseline" }
          { Version = v (1, 0); RuleId = "dep"; Change = DeprecatedRule; Note = "deprecate dep, remove at 2.0" } ] }

[<Tests>]
let generatedProductContractTests =
    testList "GeneratedProductContract" [
        test "a violated Required rule => Fail" {
            Expect.equal (classifyViolation (sample (v (1, 0))) "req") Fail "required rule fails"
        }

        test "a violated Deprecated rule with the window open => Warn naming the removal version" {
            match classifyViolation (sample (v (1, 0))) "dep" with
            | Warn message -> Expect.stringContains message (renderVersion (v (2, 0))) "warning names the removal version"
            | other -> failtestf "expected Warn, got %A" other
        }

        test "a violated Deprecated rule once the window has closed => Fail" {
            // schema_version has reached the removal version (2.0) — the window is closed.
            Expect.equal (classifyViolation (sample (v (2, 0))) "dep") Fail "deprecated rule fails once removal version ships"
        }

        test "a rule promoted Deprecated -> Required => Fail" {
            let promoted =
                { (sample (v (2, 0))) with
                    Rules =
                        [ { RuleId = "dep"; Lifecycle = Required; Description = "promoted to required" } ] }
            Expect.equal (classifyViolation promoted "dep") Fail "promoted rule fails"
        }

        test "a Removed rule is not evaluated => Pass" {
            Expect.equal (classifyViolation (sample (v (2, 0))) "gone") Pass "removed rule not evaluated"
        }

        test "an unknown rule id => Pass (not evaluated)" {
            Expect.equal (classifyViolation (sample (v (1, 0))) "nope") Pass "unknown rule not evaluated"
        }

        test "renderContractHeader exposes the schema_version" {
            let header = renderContractHeader (sample (v (1, 3)))
            Expect.stringContains header "schema_version" "header labels the schema version"
            Expect.stringContains header (renderVersion (v (1, 3))) "header shows the schema version value"
        }

        test "the typed changelog records the deprecation transition" {
            let contract = sample (v (1, 0))
            let depEntry = contract.Changelog |> List.tryFind (fun e -> e.RuleId = "dep")
            Expect.equal (depEntry |> Option.map (fun e -> e.Change)) (Some DeprecatedRule) "changelog records dep deprecation"
        }

        // T027 (FR-006, SC-011): changelog<->schema-version consistency.
        test "the current shipped contract is changelog<->version consistent" {
            Expect.equal (changelogConsistencyFindings current) [] "current contract is consistent"
        }

        test "a breaking changelog entry without a version bump is flagged" {
            // schema_version stays 1.0 but a PromotedToRequired entry claims 1.0 — no bump.
            let broken =
                { SchemaVersion = v (1, 0)
                  Rules = [ { RuleId = "r"; Lifecycle = Required; Description = "r" } ]
                  Changelog =
                    [ { Version = v (1, 0); RuleId = "r"; Change = Added; Note = "added at 1.0" }
                      { Version = v (1, 0); RuleId = "r"; Change = PromotedToRequired; Note = "promoted without a bump" } ] }
            Expect.isNonEmpty (changelogConsistencyFindings broken) "missing-bump breaking change is flagged"
        }

        test "current contract exposes an explicit schema version >= every changelog entry" {
            let maxEntry = current.Changelog |> List.map (fun e -> e.Version) |> List.maxBy (fun ver -> ver.Major * 1000 + ver.Minor)
            Expect.isTrue (compareVersion current.SchemaVersion maxEntry >= 0) "schema version covers the changelog"
        }
    ]
