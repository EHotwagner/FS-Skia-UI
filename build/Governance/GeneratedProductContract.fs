module FS.Skia.UI.Build.GeneratedProductContract

type ContractSchemaVersion =
    { Major: int
      Minor: int }

type RuleLifecycle =
    | Required
    | Deprecated of removalVersion: ContractSchemaVersion
    | Removed

type StructuralRule =
    { RuleId: string
      Lifecycle: RuleLifecycle
      Description: string }

type ContractChangeKind =
    | Added
    | DeprecatedRule
    | PromotedToRequired
    | RuleRemoved

type ContractChangelogEntry =
    { Version: ContractSchemaVersion
      RuleId: string
      Change: ContractChangeKind
      Note: string }

type GeneratedProductContract =
    { SchemaVersion: ContractSchemaVersion
      Rules: StructuralRule list
      Changelog: ContractChangelogEntry list }

type RuleOutcome =
    | Pass
    | Warn of string
    | Fail

let compareVersion (a: ContractSchemaVersion) (b: ContractSchemaVersion) =
    if a.Major <> b.Major then compare a.Major b.Major else compare a.Minor b.Minor

let renderVersion (v: ContractSchemaVersion) = $"{v.Major}.{v.Minor}"

// The current contract. Every existing generated-product structural check is wrapped
// as a Required rule at schema_version 1.0 (behaviour-identical — Required still fails).
let current =
    let v1 = { Major = 1; Minor = 0 }

    { SchemaVersion = v1
      Rules =
        [ { RuleId = "product-app-present"; Lifecycle = Required; Description = "exactly one product app project" }
          { RuleId = "product-test-suite-present"; Lifecycle = Required; Description = "exactly one product test suite" }
          { RuleId = "required-files-present"; Lifecycle = Required; Description = "required generated files present" }
          { RuleId = "api-surface-bundled"; Lifecycle = Required; Description = "bundled docs/api-surface stays in lockstep with source" }
          { RuleId = "effects-boundary-doc"; Lifecycle = Required; Description = "self-contained docs/effects-boundary.md" }
          { RuleId = "no-demo-identifiers"; Lifecycle = Required; Description = "generated starter carries no demo identifiers" }
          { RuleId = "consumer-facing-skills"; Lifecycle = Required; Description = "generated capability skills are consumer-facing" }
          { RuleId = "widgets-skill-present"; Lifecycle = Required; Description = "generated app carries the widgets skill" }
          { RuleId = "no-stale-charts"; Lifecycle = Required; Description = "no stale Charts package or skill" }
          { RuleId = "controls-elmish-reference"; Lifecycle = Required; Description = "generated app references Controls.Elmish" }
          { RuleId = "persistent-host-wiring"; Lifecycle = Required; Description = "generated app wires the persistent viewer host" }
          { RuleId = "no-bounded-only-default"; Lifecycle = Required; Description = "no bounded-only/print-only default launch path" }
          { RuleId = "claude-skill-peers"; Lifecycle = Required; Description = ".claude skill peers present for every .agents skill" }
          { RuleId = "no-framework-source"; Lifecycle = Required; Description = "no copied framework implementation/source/specs" } ]
      Changelog =
        [ { Version = v1
            RuleId = "(baseline)"
            Change = Added
            Note = "Structural contract codified with an explicit schema version and deprecation window (feature 046)." } ] }

let private changeKindText =
    function
    | Added -> "added"
    | DeprecatedRule -> "deprecated"
    | PromotedToRequired -> "promoted-to-required"
    | RuleRemoved -> "removed"

let classifyViolation (contract: GeneratedProductContract) (ruleId: string) : RuleOutcome =
    match contract.Rules |> List.tryFind (fun r -> r.RuleId = ruleId) with
    | None -> Pass // unknown rule is not part of the evaluated contract
    | Some rule ->
        match rule.Lifecycle with
        | Required -> Fail
        | Removed -> Pass // a removed rule is no longer evaluated
        | Deprecated removalVersion ->
            if compareVersion contract.SchemaVersion removalVersion < 0 then
                // window open: warn but do not fail, naming the removal version
                Warn $"rule `{ruleId}` is deprecated and will be removed in schema_version {renderVersion removalVersion}"
            else
                // window closed (schema_version has reached the removal version): hard-fail
                Fail

let renderContractHeader (contract: GeneratedProductContract) : string =
    let ruleLine (rule: StructuralRule) =
        let lifecycle =
            match rule.Lifecycle with
            | Required -> "required"
            | Deprecated removalVersion -> $"deprecated (removal in {renderVersion removalVersion})"
            | Removed -> "removed"

        $"- `{rule.RuleId}`: {lifecycle} — {rule.Description}"

    [ $"schema_version: {renderVersion contract.SchemaVersion}"
      ""
      "Generated-product structural rule lifecycle:"
      yield! contract.Rules |> List.map ruleLine
      ""
      "Contract changelog:"
      yield!
          contract.Changelog
          |> List.map (fun e -> $"- {renderVersion e.Version} `{e.RuleId}` {changeKindText e.Change}: {e.Note}") ]
    |> String.concat "\n"

let changelogConsistencyFindings (contract: GeneratedProductContract) : string list =
    let isBreaking change =
        change = PromotedToRequired || change = RuleRemoved

    let entries = contract.Changelog

    let maxEntryVersion =
        entries
        |> List.fold (fun acc e -> if compareVersion e.Version acc > 0 then e.Version else acc) contract.SchemaVersion

    [ if compareVersion contract.SchemaVersion maxEntryVersion < 0 then
          yield
              $"schema_version {renderVersion contract.SchemaVersion} is behind the maximum changelog entry version {renderVersion maxEntryVersion}"

      for i in 0 .. List.length entries - 1 do
          let e = entries.[i]

          if isBreaking e.Change then
              let priorMax =
                  entries.[0 .. i - 1]
                  |> List.fold (fun acc p -> if compareVersion p.Version acc > 0 then p.Version else acc) { Major = 0; Minor = 0 }

              if compareVersion e.Version priorMax <= 0 then
                  yield
                      $"breaking changelog entry for `{e.RuleId}` ({changeKindText e.Change}) at schema_version {renderVersion e.Version} must bump above the prior version {renderVersion priorMax}" ]
