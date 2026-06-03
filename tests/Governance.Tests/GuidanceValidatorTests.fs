module GuidanceValidatorTests

// Feature 045 (US3, T021): the relocated generated-guidance scanner, exercised end-to-end
// over the REAL repository guidance (real evidence — no fixtures/mocks).
// Feature 055 (US1/US2, T006/T009/T010): pure-core decoupling proofs feeding
// evaluateGuidanceCheck real obligations/tokens over realistic reworded/drifted content.
open System
open System.IO
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Guidance
open FS.Skia.UI.Build.Engine.Model
open GovernanceTestSupport

let private model = fst (init repositoryRoot)

// One-file lookup helper: the file under test resolves to `content`, all else missing.
let private lookupOf (file: string) (content: string) : string -> string option =
    fun path -> if path = file then Some content else None

let private obligationById id =
    taskSkillistGuidanceCheck.Obligations |> List.find (fun o -> o.Id = id)

let private tokenByText text =
    taskSkillistGuidanceCheck.Tokens |> List.find (fun t -> t.Token = text)

[<Tests>]
let guidanceValidatorTests =
    testList "Guidance validator (relocated)" [
        // SC-006 regression: the decoupled validators still PASS over the real repo.
        test "runGeneratedGuidanceScan writes a PASS report over the real repository guidance" {
            let outPath = Path.Combine(Path.GetTempPath(), $"guidance-{Guid.NewGuid():N}.md")

            try
                Guidance.runGeneratedGuidanceScan model outPath
                Expect.isTrue (File.Exists outPath) "guidance report written"
                let report = File.ReadAllText outPath
                Expect.stringContains report "# Generated Guidance Check" "report has the expected heading"
                Expect.stringContains report "PASS" "real repository guidance passes the relocated scanner"
            finally
                if File.Exists outPath then File.Delete outPath
        }

        // US1 (SC-001): a reworded-but-concept-preserving edit PASSes the new evaluator,
        // where the pre-055 literal-substring table would have produced a `missing` term.
        test "US1 reworded prose preserving the obligation concept passes where the literal table failed" {
            let structured = obligationById "skillist-structured"
            let file = ".specify/templates/tasks-template.md"

            // Reworded prose: keeps the AnyOf concept anchor "structured skillist" but drops
            // the pre-055 literal phrase "structured `skillist`" (backticked form).
            let reworded =
                "Each task declares a structured skillist listing its capability skill ids in order."

            let check =
                { Tag = "task-skillist-guidance"
                  Tokens = []
                  Obligations = [ { structured with Files = [ file ] } ]
                  Forbidden = [] }

            let findings = evaluateGuidanceCheck (lookupOf file reworded) check
            Expect.equal findings [] "reworded prose preserving the concept anchor passes"

            // The pre-055 literal table pinned the exact phrase, which the rewording removed:
            let pre055LiteralHit = reworded.IndexOf("structured `skillist`", StringComparison.OrdinalIgnoreCase) >= 0
            Expect.isFalse pre055LiteralHit "the pre-055 literal substring would have FAILED on the same edit"
        }

        // US2 (SC-002, FR-003): an obligation concept removed while its sibling ContractToken
        // remains still FAILs, with the exact `file: obligation 'id' (source) not reflected [tag]`
        // diagnostic — proving the obligation fails on prose-concept loss, not merely token loss.
        test "US2 source-of-truth drift fails with an actionable obligation diagnostic" {
            let structured = obligationById "skillist-structured"
            let skillistToken = tokenByText "[skillist: []]"
            let file = ".specify/templates/tasks-template.md"

            // Drift: the "structured skillist" concept is gone, but the machine token remains.
            let drifted =
                "Tasks list their dependencies. Example metadata line: [skillist: []] for no skill."

            let check =
                { Tag = "task-skillist-guidance"
                  Tokens = [ { skillistToken with Files = [ file ] } ]
                  Obligations = [ { structured with Files = [ file ] } ]
                  Forbidden = [] }

            let findings = evaluateGuidanceCheck (lookupOf file drifted) check

            Expect.equal
                findings
                [ $"{file}: obligation 'skillist-structured' (constitution:Local Agent Skills) not reflected [task-skillist-guidance]" ]
                "drift fails on concept loss with the file + obligation id + source named, while the token stays satisfied"
        }

        // US2 twin coverage (spec edge case): drift in one twin file is still caught.
        test "US2 drift in one twin file is caught while the other twin passes" {
            let structured = obligationById "skillist-structured"
            let twinA = ".specify/templates/tasks-template.md"
            let twinB = ".specify/presets/fsharp-opinionated/templates/tasks-template.md"

            let lookup =
                function
                | p when p = twinA -> Some "Each task carries a structured skillist."
                | p when p = twinB -> Some "Each task carries a list of dependencies." // concept dropped
                | _ -> None

            let check =
                { Tag = "task-skillist-guidance"
                  Tokens = []
                  Obligations = [ { structured with Files = [ twinA; twinB ] } ]
                  Forbidden = [] }

            let findings = evaluateGuidanceCheck lookup check

            Expect.equal
                findings
                [ $"{twinB}: obligation 'skillist-structured' (constitution:Local Agent Skills) not reflected [task-skillist-guidance]" ]
                "drift in twin B is caught; twin A (concept present) passes"
        }

        // SC-004 / FR-006: removing any machine-contract token still FAILs; a forbidden/stale
        // term reintroduced still FAILs.
        test "SC-004 removing a machine-contract token still fails" {
            let skillistToken = tokenByText "[skillist: []]"
            let file = ".specify/templates/tasks-template.md"

            let check =
                { Tag = "task-skillist-guidance"
                  Tokens = [ { skillistToken with Files = [ file ] } ]
                  Obligations = []
                  Forbidden = [] }

            // Content with the structured-skillist concept but the literal token removed.
            let findings = evaluateGuidanceCheck (lookupOf file "Each task carries a structured skillist.") check

            Expect.equal
                findings
                [ $"{file}: missing `[skillist: []]` [task-skillist-guidance]" ]
                "a removed machine-contract token still fails verbatim"
        }

        test "FR-006 a reintroduced forbidden/stale term still fails" {
            let staleToken = controlsBoundaryGuidanceCheck.Forbidden |> List.head
            let file = "template/base/README.md"

            let check =
                { Tag = "controls-boundary-guidance"
                  Tokens = []
                  Obligations = []
                  Forbidden = [ { staleToken with Files = [ file ] } ] }

            // Content that reintroduces the removed-Charts stale term verbatim.
            let findings = evaluateGuidanceCheck (lookupOf file $"This still mentions {staleToken.Token} guidance.") check

            Expect.equal
                findings
                [ $"generated controls guidance contains stale term `{staleToken.Token}` [controls-boundary-guidance]" ]
                "a reintroduced forbidden/stale term is still rejected"
        }
    ]
