module FS.Skia.UI.Build.ContractView

open System
open FS.Skia.UI.Build

// Stable tier ordering for the rendered view (independent of the Tier DU declaration).
let private tierOrder =
    [ Routing.InnerLoop
      Routing.FocusedAuthority
      Routing.AgentReady
      Routing.MaintainerVerify
      Routing.AutomationFinal
      Routing.Tier1
      Routing.Tier2 ]

let private tierLabel tier =
    match tier with
    | Routing.InnerLoop -> "inner-loop"
    | Routing.FocusedAuthority -> "focused-authority"
    | Routing.AgentReady -> "agent-ready"
    | Routing.MaintainerVerify -> "maintainer-verify"
    | Routing.AutomationFinal -> "automation-final"
    | Routing.Tier1 -> "tier1"
    | Routing.Tier2 -> "tier2"

let private tierAuthority tier =
    match tier with
    | Routing.InnerLoop
    | Routing.Tier2 -> "non-authoritative"
    | Routing.FocusedAuthority
    | Routing.AgentReady
    | Routing.Tier1 -> "focused-authoritative"
    | Routing.MaintainerVerify
    | Routing.AutomationFinal -> "broad-authoritative"

let private tierDefaultGates tier : Targets.Target list =
    match tier with
    | Routing.InnerLoop
    | Routing.Tier2 -> [ Targets.Dev ]
    | Routing.FocusedAuthority -> [ Targets.EvidenceGraph ]
    | Routing.AgentReady
    | Routing.Tier1 -> [ Targets.AgentReady; Targets.EvidenceGraph; Targets.EvidenceAudit ]
    | Routing.MaintainerVerify -> [ Targets.Verify ]
    | Routing.AutomationFinal -> [ Targets.Ci ]

let private listItems (indent: string) (values: string list) =
    values |> List.map (fun value -> sprintf "%s- %s" indent value)

let private tierBlock tier =
    [ sprintf "  %s:" (tierLabel tier)
      sprintf "    authority: %s" (tierAuthority tier)
      "    default_gates:" ]
    @ listItems "      " (tierDefaultGates tier |> List.map Targets.name)

let private quotedItems (indent: string) (values: string list) =
    values |> List.map (fun value -> sprintf "%s- \"%s\"" indent value)

let private ruleBlock (rule: Routing.RoutingRule) =
    [ sprintf "  - id: %s" rule.Id
      sprintf "    tier: %s" (tierLabel rule.Tier)
      "    paths:" ]
    @ quotedItems "      " rule.Paths
    @ [ "    feature_concerns:" ]
    @ listItems "      " [ rule.Id ]
    @ [ "    required_gates:" ]
    @ listItems "      " (rule.RequiredGates |> List.map Targets.name)
    @ [ "    expected_artifacts:" ]
    @ listItems "      " rule.ExpectedArtifacts
    @ [ sprintf "    timeout_class: %s" rule.TimeoutClass
        sprintf "    failure_owner: %s" rule.FailureOwner ]

let render (rules: Routing.RoutingRule list) (dogfoodFeatureIds: string list) =
    let header =
        [ "schema_version: 1"
          ""
          "# GENERATED from build/Governance/Routing.fs (feature 042). Do not hand-edit;"
          "# regenerate via ./fake.sh build -t RefreshSurfaceBaselines. The compiled"
          "# Routing module is the single source of truth for tiers and routing rules."
          ""
          "defaults:"
          "  broad_fallback_command: \"./fake.sh build -t Verify\""
          "  unknown_gate_rejection:"
          "    owner: governance"
          "    diagnostic: \"unknown gate is rejected with no success verdict\""
          "  final_gates:"
          "    - EvidenceGraph"
          "    - EvidenceAudit"
          ""
          "tiers:" ]

    let tierLines = tierOrder |> List.collect tierBlock
    let ruleLines = rules |> List.collect ruleBlock

    let dogfoodLines =
        "dogfood_feature_ids:"
        :: listItems "  " (dogfoodFeatureIds |> List.map (fun featureId -> sprintf "\"%s\"" featureId))

    let lines =
        header
        @ tierLines
        @ [ ""; "routing_rules:" ]
        @ ruleLines
        @ [ "" ]
        @ dogfoodLines

    String.concat "\n" lines + "\n"

let private normalize (text: string) =
    text.Replace("\r\n", "\n").TrimEnd() + "\n"

let currencyDrift (onDiskContract: string) (rules: Routing.RoutingRule list) (dogfoodFeatureIds: string list) : string option =
    let expected = render rules dogfoodFeatureIds

    if normalize onDiskContract = normalize expected then
        None
    else
        Some "validation.contract.yml is stale — regenerate from Routing.fs via ./fake.sh build -t RefreshSurfaceBaselines"
