module FS.Skia.UI.Build.Routing

open System
open System.Text
open System.Text.RegularExpressions
open FS.Skia.UI.Build

type DeveloperClass =
    | FrameworkAuthor
    | ConsumerAgent

type Tier =
    | InnerLoop
    | FocusedAuthority
    | AgentReady
    | MaintainerVerify
    | AutomationFinal
    | Tier1
    | Tier2

let tierRank tier =
    match tier with
    | InnerLoop -> 0
    | Tier2 -> 0
    | FocusedAuthority -> 1
    | AgentReady -> 2
    | Tier1 -> 2
    | MaintainerVerify -> 3
    | AutomationFinal -> 4

type Diff = { ChangedPaths: string list }

type RoutingRule =
    { Id: string
      Paths: string list
      Matches: Diff -> bool
      Tier: Tier
      RequiredGates: Targets.Target list
      ExpectedArtifacts: string list
      TimeoutClass: string
      FailureOwner: string }

type Selection =
    { DeveloperClass: DeveloperClass
      Tier: Tier
      Gates: Targets.Target list
      MatchedRuleIds: string list
      ExpectedArtifacts: string list
      DogfoodForced: bool }

// fnmatch-style glob → anchored regex. `**` matches any characters including `/`;
// `*` and `?` match within a single path segment. Repo paths are normalised to `/`.
let internalGlobToRegex (pattern: string) =
    let sb = StringBuilder()
    sb.Append('^') |> ignore
    let n = pattern.Length
    let mutable i = 0

    while i < n do
        let c = pattern.[i]

        if c = '*' then
            if i + 1 < n && pattern.[i + 1] = '*' then
                sb.Append(".*") |> ignore
                i <- i + 2
                // absorb a following '/', so `a/**/b` also matches `a/b`
                if i < n && pattern.[i] = '/' then
                    i <- i + 1
            else
                sb.Append("[^/]*") |> ignore
                i <- i + 1
        elif c = '?' then
            sb.Append("[^/]") |> ignore
            i <- i + 1
        elif c = '/' then
            sb.Append('/') |> ignore
            i <- i + 1
        else
            sb.Append(Regex.Escape(string c)) |> ignore
            i <- i + 1

    sb.Append('$') |> ignore
    sb.ToString()

let internalMatchesGlob (pattern: string) (pathStr: string) =
    let normalized = pathStr.Replace('\\', '/')
    Regex.IsMatch(normalized, internalGlobToRegex pattern)

let internalRuleMatcher (patterns: string list) : Diff -> bool =
    fun diff ->
        diff.ChangedPaths
        |> List.exists (fun p -> patterns |> List.exists (fun pat -> internalMatchesGlob pat p))

// A change is inner-loop applicable when every changed path is framework source under
// `src/**` (and the empty diff trivially qualifies). Such a change with no escalation
// rule match routes to the base tier; any other unmatched path default-denies (FR-002).
let internalInnerLoopApplicable (diff: Diff) =
    diff.ChangedPaths
    |> List.forall (fun p -> internalMatchesGlob "src/**" p)

let internalTierLabel tier =
    match tier with
    | InnerLoop -> "inner-loop"
    | FocusedAuthority -> "focused-authority"
    | AgentReady -> "agent-ready"
    | MaintainerVerify -> "maintainer-verify"
    | AutomationFinal -> "automation-final"
    | Tier1 -> "tier1"
    | Tier2 -> "tier2"

let internalDeveloperClassLabel developerClass =
    match developerClass with
    | FrameworkAuthor -> "framework-author"
    | ConsumerAgent -> "consumer-agent"

// A rule constructor that derives the `Matches` glob predicate from the same `Paths`
// list the generated contract renders, so the typed predicate and the rendered
// `paths:` view cannot drift (FR-007).
let internalRule id paths tier gates artifacts timeoutClass failureOwner =
    { Id = id
      Paths = paths
      Matches = internalRuleMatcher paths
      Tier = tier
      RequiredGates = gates
      ExpectedArtifacts = artifacts
      TimeoutClass = timeoutClass
      FailureOwner = failureOwner }

let rules =
    [ internalRule
          "controls-public-surface"
          [ "src/Controls/**" ]
          FocusedAuthority
          [ Targets.ControlsCatalogCheck
            Targets.ControlsInteractionCheck
            Targets.ControlsRenderingCheck
            Targets.PackageSurfaceCheck
            Targets.FsiTranscripts
            Targets.GeneratedProductCheck ]
          [ "readiness/typed-controls-front-door.md"
            "readiness/package-surface-expectations.md" ]
          "focused"
          "product"

      // template/** (F2 broadened) keeps the original template/base/** and
      // template/fragments/** globs the contract's existing consumers scan for.
      internalRule
          "generated-template"
          [ "template/base/**"; "template/fragments/**"; "template/**" ]
          FocusedAuthority
          [ Targets.TemplateCheck; Targets.GeneratedProductCheck ]
          [ "readiness/evidence-policy-separation.md" ]
          "focused"
          "template"

      internalRule
          "evidence-governance"
          [ "specs/**/tasks.md"
            "specs/**/tasks.deps.yml"
            "specs/**/readiness/**"
            ".specify/extensions/evidence/**" ]
          AgentReady
          [ Targets.EvidenceGraph; Targets.EvidenceAudit ]
          [ "readiness/validation-contract.md"
            "readiness/evidence-graph.md"
            "readiness/evidence-audit.md" ]
          "focused"
          "governance"

      internalRule
          "generated-guidance"
          [ ".specify/templates/**"
            ".specify/presets/**"
            "template/fragments/**/README.md"
            "template/fragments/**/skill/SKILL.md" ]
          FocusedAuthority
          [ Targets.GeneratedGuidanceCheck; Targets.TemplateDrift ]
          [ "readiness/evidence-policy-separation.md" ]
          "focused"
          "governance"

      // .specify/** catch-all (F2 broadened) so any generated-guidance source escalates.
      internalRule
          "specify-catchall"
          [ ".specify/**" ]
          FocusedAuthority
          [ Targets.GeneratedGuidanceCheck; Targets.TemplateDrift ]
          [ "readiness/evidence-policy-separation.md" ]
          "focused"
          "governance"

      internalRule
          "docs-only"
          [ "docs/**"; "specs/**/contracts/**"; "specs/**/quickstart.md" ]
          FocusedAuthority
          [ Targets.EvidenceGraph ]
          [ "readiness/validation-contract.md" ]
          "focused"
          "governance"

      // Feature 053 (V3 Stage 5, FR-007): `PerPackageSurfaceDiff` is now a Route-gated
      // requirement of the package-surface rule, so a public `src/**/*.fsi` change
      // selects the per-package DiffPlex surface check alongside the aggregate
      // `PackageSurfaceCheck` and the FSI transcripts. The Stage-0 (feature 048) deferral
      // is cleared: the contract validator's `knownGates` allowlist moved out of the
      // retired runtime monolith into `FS.Skia.UI.Build` (`AgentValidation.fs`) in Stage 2,
      // so wiring the gate is a governance/build edit with no runtime change. The rule, its
      // `validation.contract.yml` rendering, and the allowlist entry land together
      // (TargetMetadataDrift enforces currency).
      internalRule
          "package-surface"
          [ "src/**/*.fsi"; "readiness/surface-baselines/**" ]
          FocusedAuthority
          [ Targets.PackageSurfaceCheck; Targets.FsiTranscripts; Targets.PerPackageSurfaceDiff ]
          [ "readiness/package-surface-expectations.md" ]
          "focused"
          "product"

      internalRule
          "build-target-contract"
          [ "build.fsx"; "scripts/build/**"; "validation.contract.yml" ]
          MaintainerVerify
          [ Targets.AgentReady
            Targets.TargetMetadataDrift
            Targets.EvidenceGraph
            Targets.EvidenceAudit
            Targets.Verify
            Targets.Ci ]
          [ "readiness/target-metadata.md"
            "readiness/agent-ready-verdict.md" ]
          "broad"
          "governance" ]

let innerLoopGates = [ Targets.Dev ]

let fullPipelineGates =
    [ Targets.Dev
      Targets.GeneratedGuidanceCheck
      Targets.TemplateCheck
      Targets.GeneratedProductCheck
      Targets.EvidenceGraph
      Targets.EvidenceAudit ]

let dogfoodFeatureIds = [ "042" ]

// The active feature id at the `Route` edge is the full directory slug
// (e.g. "042-foundations-two-tier-process"); `dogfoodFeatureIds` carries the short
// numeric ids ("042"). Match either the full id or its leading slug segment so the
// dogfood policy is keyed on the feature number regardless of the directory name.
let isDogfood (featureId: string) =
    let leadingSegment =
        match featureId.IndexOf('-') with
        | -1 -> featureId
        | index -> featureId.Substring(0, index)

    dogfoodFeatureIds
    |> List.exists (fun id -> id = featureId || id = leadingSegment)

// De-duplicate a gate set into Targets.allTargets registry order so `Route` output is
// deterministic and byte-stable regardless of which rules contributed which gate.
let internalDedupInRegistryOrder (gates: Targets.Target list) =
    Targets.allTargets |> List.filter (fun t -> gates |> List.contains t)

let select developerClass diff =
    let baseTier =
        match developerClass with
        | FrameworkAuthor -> InnerLoop
        | ConsumerAgent -> FocusedAuthority // ConsumerAgent floor

    let matched = rules |> List.filter (fun rule -> rule.Matches diff)

    if List.isEmpty matched then
        if internalInnerLoopApplicable diff then
            { DeveloperClass = developerClass
              Tier = baseTier
              Gates = internalDedupInRegistryOrder innerLoopGates
              MatchedRuleIds = []
              ExpectedArtifacts = []
              DogfoodForced = false }
        else
            // default-deny: an unmatched, non-inner-loop path routes to the broad fallback,
            // never an empty success (FR-002, US2 edge).
            let fallbackTier = [ baseTier; MaintainerVerify ] |> List.maxBy tierRank

            { DeveloperClass = developerClass
              Tier = fallbackTier
              Gates = [ Targets.Verify ]
              MatchedRuleIds = []
              ExpectedArtifacts = []
              DogfoodForced = false }
    else
        let tier =
            baseTier :: (matched |> List.map (fun rule -> rule.Tier))
            |> List.maxBy tierRank

        let gates =
            innerLoopGates @ (matched |> List.collect (fun rule -> rule.RequiredGates))
            |> internalDedupInRegistryOrder

        let artifacts =
            matched
            |> List.collect (fun rule -> rule.ExpectedArtifacts)
            |> List.distinct

        { DeveloperClass = developerClass
          Tier = tier
          Gates = gates
          MatchedRuleIds = matched |> List.map (fun rule -> rule.Id)
          ExpectedArtifacts = artifacts
          DogfoodForced = false }

let selectForFeature developerClass featureId diff =
    let baseSelection = select developerClass diff

    if isDogfood featureId then
        { baseSelection with
            Tier = [ baseSelection.Tier; MaintainerVerify ] |> List.maxBy tierRank
            Gates = fullPipelineGates
            DogfoodForced = true }
    else
        baseSelection

let unmetArtifacts (present: Set<string>) (selection: Selection) =
    selection.ExpectedArtifacts
    |> List.filter (fun artifact -> present.Contains artifact |> not)

let enforceDiagnostic (selection: Selection) (missing: string list) =
    let tier = internalTierLabel selection.Tier
    let artifacts = String.concat ", " missing

    sprintf
        "Route --enforce: tier '%s' requires evidence artifacts that are not present: %s"
        tier
        artifacts

let renderSelection (selection: Selection) =
    let gates =
        if List.isEmpty selection.Gates then
            "(none)"
        else
            selection.Gates |> List.map Targets.name |> String.concat ", "

    let matchedRules =
        if List.isEmpty selection.MatchedRuleIds then
            "(none)"
        else
            String.concat ", " selection.MatchedRuleIds

    [ sprintf "developer-class=%s" (internalDeveloperClassLabel selection.DeveloperClass)
      sprintf "tier=%s" (internalTierLabel selection.Tier)
      sprintf "gates=%s" gates
      sprintf "dogfood-forced=%b" selection.DogfoodForced
      sprintf "matched-rules=%s" matchedRules ]
    |> String.concat Environment.NewLine
