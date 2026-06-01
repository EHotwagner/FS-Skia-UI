module SequentialRunnerGuidanceTests

open System
open System.Text.RegularExpressions
open Expecto
open GovernanceTestSupport

let private requiredSerializedRunnerTerms =
    [ "FAKE-backed"
      ".fake"
      "sequential"
      "not safe to run concurrently" ]

let private containsAny (content: string) (needles: string list) =
    needles
    |> List.exists (fun needle -> content.Contains(needle, StringComparison.OrdinalIgnoreCase))

let private buildRunnerCommandPattern =
    Regex(@"(\./fake\.sh|fake\.cmd|dotnet fake)\b", RegexOptions.IgnoreCase)

let private numberedBuildRunnerCommandPattern =
    Regex(@"(?m)^\s*(\d+\.|-)\s+(`)?(\./fake\.sh|fake\.cmd|dotnet fake)\b", RegexOptions.IgnoreCase)

let private hasRunnerCommand (content: string) =
    buildRunnerCommandPattern.IsMatch content

let private hasDeterministicOrder (content: string) =
    numberedBuildRunnerCommandPattern.Matches(content).Count > 1

let private validateSerializedRunnerGuidance (path: string) (content: string) =
    if hasRunnerCommand content then
        [ yield!
              requiredSerializedRunnerTerms
              |> List.choose (fun term ->
                  if content.Contains(term, StringComparison.OrdinalIgnoreCase) then
                      None
                  else
                      Some $"{path}: missing `{term}` for sequential FAKE guidance")
          if content.Contains("parallel", StringComparison.OrdinalIgnoreCase)
             && not (containsAny content [ "non-FAKE"; "do not invoke FAKE"; "does not invoke FAKE" ]) then
              $"{path}: FAKE guidance mentions parallelism without the non-FAKE exception"
          if buildRunnerCommandPattern.Matches(content).Count > 1 && not (hasDeterministicOrder content) then
              $"{path}: multiple FAKE-backed commands require deterministic sequential ordering" ]
    else
        []

let private repositoryGuidancePaths =
    [ "README.md"
      "docs/reports/build.md"
      "docs/reports/testing.md"
      "docs/reports/evidence.md" ]

let private agentGuidancePaths =
    [ "AGENTS.md"
      "CLAUDE.md"
      ".agents/skills/speckit-implement/SKILL.md"
      ".agents/skills/speckit-evidence-graph/SKILL.md"
      ".agents/skills/speckit-evidence-audit/SKILL.md"
      ".claude/skills/speckit-implement/SKILL.md"
      ".claude/skills/speckit-evidence-graph/SKILL.md"
      ".claude/skills/speckit-evidence-audit/SKILL.md" ]
// Note: the redundant `.claude/commands/speckit-*.md` slash-command wrappers were
// intentionally removed in commit ba327b0 ("Remove redundant speckit slash-command
// wrappers"); the same-named `.claude/skills/speckit-*` skills above are the
// canonical surface and remain validated here.

let private generatedGuidancePaths =
    [ "template/base/README.md"
      "template/base/docs/product.md"
      "template/base/.agents/skills/fs-skia-project/SKILL.md"
      "template/base/.claude/skills/fs-skia-project/SKILL.md"
      ".specify/templates/tasks-template.md"
      ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
      ".specify/templates/plan-template.md"
      ".specify/presets/fsharp-opinionated/templates/plan-template.md" ]

[<Tests>]
let serializedRunnerGuidanceTests =
    testList "Sequential FAKE guidance" [
        test "repository guidance lists FAKE-backed commands sequentially with shared state warning" {
            repositoryGuidancePaths
            |> List.collect (fun path -> validateSerializedRunnerGuidance path (read path))
            |> fun findings -> Expect.equal findings [] (String.concat Environment.NewLine findings)
        }

        test "agent guidance distinguishes safe non-FAKE parallelism from serialized FAKE targets" {
            agentGuidancePaths
            |> List.collect (fun path -> validateSerializedRunnerGuidance path (read path))
            |> fun findings -> Expect.equal findings [] (String.concat Environment.NewLine findings)
        }

        test "generated guidance source carries sequential FAKE rules" {
            generatedGuidancePaths
            |> List.collect (fun path -> validateSerializedRunnerGuidance path (read path))
            |> fun findings -> Expect.equal findings [] (String.concat Environment.NewLine findings)
        }

        test "readiness evidence contract requires command-order and race triage fields" {
            expectFileContains
                "specs/031-serialize-fake-runs/contracts/readiness-evidence.md"
                [ "Order number"
                  "Command"
                  "Working directory"
                  "Start/end timestamp"
                  "Exit code"
                  "Failed command"
                  "another FAKE-backed command"
                  "Suspected `.fake` race status"
                  "Required rerun order"
                  "Follow-up classification" ]
        }

        // Feature 042 / US4 (FR-008, SC-008): the agent guidance points at `Route`
        // first and reframes the serialized six-target order as the escalated path.
        test "agent guidance instructs Route-first and reframes the six-target order as escalated" {
            [ "AGENTS.md"; "CLAUDE.md" ]
            |> List.iter (fun path ->
                let content = read path

                Expect.isTrue
                    (content.Contains("Route", StringComparison.Ordinal))
                    $"{path}: names the Route entry point"

                Expect.isTrue
                    (content.Contains("run only the gates it prints", StringComparison.OrdinalIgnoreCase))
                    $"{path}: instructs running only the gates Route prints"

                Expect.isTrue
                    (content.Contains("maintainer-verify", StringComparison.OrdinalIgnoreCase)
                     && content.Contains("escalated", StringComparison.OrdinalIgnoreCase))
                    $"{path}: reframes the serialized order as the escalated/maintainer-verify path"

                Expect.isFalse
                    (content.Contains("unconditional default", StringComparison.OrdinalIgnoreCase)
                     && not (content.Contains("no longer the unconditional default", StringComparison.OrdinalIgnoreCase)))
                    $"{path}: does not present the six-target order as the unconditional default")
        }

        test "scanner Synthetic rejects unsafe FAKE guidance snippets" {
            // SYNTHETIC: malformed guidance snippets are design-approved negative scanner fixtures; real evidence is the repository guidance scan.
            let unsafeSnippets =
                [ "missing-dot-fake", "Run `./fake.sh build -t Dev` and `./fake.sh build -t Verify` sequentially."
                  "missing-order", "FAKE-backed commands use shared `.fake` state and are not safe to run concurrently. Run `./fake.sh build -t Dev` and then run `./fake.sh build -t Verify`."
                  "unsafe-parallel", "FAKE-backed commands use shared `.fake` state. Run `./fake.sh build -t Dev` and `./fake.sh build -t Verify` in parallel." ]

            unsafeSnippets
            |> List.iter (fun (name, snippet) ->
                let findings = validateSerializedRunnerGuidance $"fixture:{name}" snippet
                Expect.isGreaterThan findings.Length 0 $"Synthetic unsafe fixture {name} is rejected")
        }
    ]
