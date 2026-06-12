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

// Feature 088 (US2, FR-008): doc-path classification for the doc-only routing relaxation.
// A "doc" path is a Markdown file; the SKILL.md skill home is excluded from the doc-only rules
// so it keeps its existing (heavier) skill-quality / generated-guidance routing.
let internalIsDocPath (p: string) =
    p.Replace('\\', '/').EndsWith(".md", StringComparison.OrdinalIgnoreCase)

let internalIsSkillDoc (p: string) =
    p.Replace('\\', '/').EndsWith("skill/SKILL.md", StringComparison.OrdinalIgnoreCase)

// Feature 088 (US2, FR-008): a SOURCE rule whose heavy gates apply only when the diff carries
// at least one non-doc (`*.md`) path under the rule's tree. A pure doc-only change under the
// tree leaves this rule unmatched (the dedicated doc rule handles it); a mixed doc+source diff
// re-escalates because the non-doc path is present, so mixed/source routing is UNCHANGED.
let internalSourceRule id paths tier gates artifacts timeoutClass failureOwner =
    { Id = id
      Paths = paths
      Matches =
        fun diff ->
            diff.ChangedPaths
            |> List.exists (fun p ->
                (not (internalIsDocPath p))
                && (paths |> List.exists (fun pat -> internalMatchesGlob pat p)))
      Tier = tier
      RequiredGates = gates
      ExpectedArtifacts = artifacts
      TimeoutClass = timeoutClass
      FailureOwner = failureOwner }

// Feature 088 (US2, FR-008): a DOC-ONLY rule. It fires only when the change under the broad
// source subtree (`treeGlobs`) is purely documentation: at least one changed path matches the
// doc globs (`docPaths`, a non-skill `*.md`), AND no changed path under `treeGlobs` is a
// non-doc source file. Checking the BROAD subtree (not just `docPaths`) is what keeps a mixed
// `a.md + b.fsi` diff OUT of this rule, so the refined source rule re-escalates it to the full
// set — mixed/source routing is byte-identical to today. `Paths` renders `docPaths`. Only the
// `skill/SKILL.md` skill home is excluded; README and other Markdown get the doc-only path.
let internalDocRule id docPaths treeGlobs tier gates artifacts timeoutClass failureOwner =
    { Id = id
      Paths = docPaths
      Matches =
        fun diff ->
            let underTree p = treeGlobs |> List.exists (fun pat -> internalMatchesGlob pat p)

            let matchesDoc p =
                (docPaths |> List.exists (fun pat -> internalMatchesGlob pat p))
                && not (internalIsSkillDoc p)

            let hasNonDocSource =
                diff.ChangedPaths
                |> List.exists (fun p -> underTree p && not (internalIsDocPath p))

            (diff.ChangedPaths |> List.exists matchesDoc) && not hasNonDocSource
      Tier = tier
      RequiredGates = gates
      ExpectedArtifacts = artifacts
      TimeoutClass = timeoutClass
      FailureOwner = failureOwner }

let rules =
    [ internalSourceRule
          "controls-public-surface"
          [ "src/Controls/**" ]
          FocusedAuthority
          // Feature 066 (US3, FR-006): the typed-catalog generation-currency gate routes
          // with the controls-surface checks so `Route` lists it for `src/Controls/**` and
          // `Route --enforce` blocks a stale generated catalog.
          // Feature 069 (US1, FR-006): the design-token generation-currency gate routes with
          // the controls-surface checks so `Route` lists it for `src/Controls/**` and
          // `Route --enforce` blocks a stale generated DesignTokens.fs.
          // Feature 083 (US1, FR-011): the WCAG color-contrast gate routes with the
          // controls-surface checks so any design-token / theme value edit selects it and
          // `Route --enforce` blocks a sub-threshold shipped token.
          // Feature 106 (US2, FR-007): the documentation-coverage gate routes with the
          // controls-surface checks so any `src/Controls/**/*.fsi` edit selects it and
          // `Route --enforce` blocks a placeholder/empty/duplicate-only summary regression.
          [ Targets.ControlsCatalogCheck
            Targets.ControlsCatalogGenerationCheck
            Targets.DesignTokenDrift
            Targets.ContrastCheck
            Targets.ControlsDocCoverageCheck
            Targets.ControlsInteractionCheck
            Targets.ControlsRenderingCheck
            Targets.PackageSurfaceCheck
            Targets.FsiTranscripts
            Targets.GeneratedProductCheck ]
          [ "readiness/typed-controls-front-door.md"
            "readiness/package-surface-expectations.md" ]
          "focused"
          "product"

      // Feature 083 (US1, FR-011): a change to the new FS.Skia.UI.Color package selects the
      // contrast gate (the package backs the gate's WCAG arithmetic) alongside the
      // per-package surface checks its public `.fsi` requires.
      internalRule
          "color-contrast"
          [ "src/Color/**" ]
          FocusedAuthority
          [ Targets.ContrastCheck
            Targets.PackageSurfaceCheck
            Targets.FsiTranscripts
            Targets.PerPackageSurfaceDiff ]
          [ "readiness/color-contrast-evidence.md" ]
          "focused"
          "product"

      // template/** (F2 broadened) keeps the original template/base/** and
      // template/fragments/** globs the contract's existing consumers scan for.
      // Feature 088 (US2, FR-008): refined to a SOURCE rule — the heavy template/generated
      // checks apply only when a non-doc (`*.md`) path is present under template/**. A pure
      // `template/**/*.md` change routes through `template-docs` instead.
      internalSourceRule
          "generated-template"
          [ "template/base/**"; "template/fragments/**"; "template/**" ]
          FocusedAuthority
          // Feature 060 (FR-003/FR-004): a `template/**` change (incl. the emitted
          // `template/base/docs/api-surface/**` tree and `template/capabilities.yml`) routes
          // the skill-claimed-contract-path gate alongside the template/generated checks.
          [ Targets.TemplateCheck; Targets.GeneratedProductCheck; Targets.SkillContractPathCheck ]
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

      // Feature 088 (US2, FR-008): doc-only relaxation for the published Controls source tree.
      // A pure `src/Controls/**/*.md` change (NOT the SKILL.md home, which routes via
      // skill-quality) validates only the task DAG — the refined `controls-public-surface`
      // source rule no longer fires its heavy controls/generated pipeline for docs alone. The
      // pinned `[ EvidenceGraph ]` keeps a doc-only change DAG-validated; a mixed doc+source
      // diff re-escalates because the source rule sees the non-doc path.
      internalDocRule
          "controls-docs"
          [ "src/Controls/**/*.md" ]
          [ "src/Controls/**" ]
          FocusedAuthority
          [ Targets.EvidenceGraph ]
          [ "readiness/validation-contract.md" ]
          "focused"
          "product"

      // Feature 088 (US2, FR-008): doc-only relaxation for the template tree. A pure
      // `template/**/*.md` change (excluding `skill/SKILL.md` and README, which keep their
      // existing routing via skill-quality / generated-guidance) validates only the task DAG.
      // The refined `generated-template` source rule no longer fires the heavy template set for
      // docs alone; mixed/source template changes are unchanged.
      internalDocRule
          "template-docs"
          [ "template/**/*.md" ]
          [ "template/base/**"; "template/fragments/**"; "template/**" ]
          FocusedAuthority
          [ Targets.EvidenceGraph ]
          [ "readiness/validation-contract.md" ]
          "focused"
          "template"

      // Feature 078 (US1, FR-005, research R5): the published Controls docs section is a
      // single-source projection of CatalogGen.catalogFacts. A change to the section
      // (docs/controls/**, docs/img/controls/**), the docs generator
      // (build/Governance/CatalogDocsGen.fs[i]), or the catalog single source
      // (build/Governance/CatalogGen.fs, src/Controls/catalog.yml) routes the dedicated
      // ControlsCatalogDocsCheck currency/completeness/preview gate alongside the docs-only
      // EvidenceGraph. docs-only alone cannot enforce catalog↔index↔detail↔preview currency.
      internalRule
          "controls-catalog-docs"
          [ "docs/controls/**"
            "docs/img/controls/**"
            "build/Governance/CatalogDocsGen.fs"
            "build/Governance/CatalogDocsGen.fsi"
            "build/Governance/CatalogGen.fs"
            "src/Controls/catalog.yml"
            // Feature 080 (FR-012): the render-capable fidelity gate sources/fixtures. Editing
            // a preview asset, the renderer-fidelity harness, or a fixture requires the decoded-
            // content ControlFidelityCheck, not just the SkiaSharp-free docs currency gate.
            "tests/ControlsPreview.Harness/**" ]
          FocusedAuthority
          [ Targets.ControlsCatalogDocsCheck
            Targets.ControlFidelityCheck ]
          []
          "focused"
          "product"

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

      // Feature 058 (US1, FR-001/FR-005): every FS-authored skill home routes to the
      // SkillQualityCheck rubric gate; canonical `.agents/skills/**` edits also route to
      // SkillSyncCheck so the generated `.claude` mirror cannot drift. The vendored
      // `speckit-*` tree is excluded inside the gate (FR-004), not by path here.
      internalRule
          "skill-quality"
          [ ".agents/skills/**"
            "src/**/skill/SKILL.md"
            "template/product-skills/**"
            "template/fragments/**/skill/SKILL.md"
            "template/base/.agents/skills/**" ]
          FocusedAuthority
          // Feature 060 (FR-004/FR-009): an FS-authored skill edit also routes the
          // skill-claimed-contract-path gate (a product/capability skill may name an
          // api-surface path) and the template-update package-set gate (the
          // `fs-skia-template-update` skill lives under `.agents/skills/**`).
          [ Targets.SkillQualityCheck
            Targets.PhaseHookParityCheck
            Targets.SkillSyncCheck
            Targets.SkillContractPathCheck
            Targets.TemplateUpdateSkillPackageCheck ]
          [ "readiness/skill-quality-check.md" ]
          "focused"
          "governance"

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
          "governance"

      // Feature 064 (FR-007): the distribution change-set — the publish/pre-publish
      // validator sources and the consumer single-source version files
      // (`template/base/build.fsx` + `Directory.Packages.props`) — escalates to the
      // maintainer-verify path. The consumer `NuGet.config`, single-source pin, and package
      // metadata are the *distribution* contract; a malformed release must never reach the
      // push edge, so the pre-publish gate, the template/generated checks, and the metadata
      // currency checks are all required before merge.
      internalRule
          "distribution"
          [ "build/Governance/Publish.fs"
            "build/Governance/Publish.fsi"
            "build/Governance/PrePublish.fs"
            "build/Governance/PrePublish.fsi"
            "template/base/build.fsx"
            "template/base/Directory.Packages.props"
            "template/base/docs/UPGRADING.md" ]
          MaintainerVerify
          [ Targets.PrePublishCheck
            Targets.TemplateCheck
            Targets.GeneratedProductCheck
            Targets.TargetMetadataDrift
            Targets.EvidenceGraph
            Targets.EvidenceAudit ]
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
