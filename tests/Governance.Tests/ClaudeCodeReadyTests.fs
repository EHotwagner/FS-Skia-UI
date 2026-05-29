module ClaudeCodeReadyTests

open System
open System.IO
open System.Text.Json
open Expecto
open GovernanceTestSupport

let private repositorySkillIds () =
    Directory.GetDirectories(fullPath ".agents/skills", "*", SearchOption.TopDirectoryOnly)
    |> Array.map Path.GetFileName
    |> Array.sort
    |> Array.toList

let private expectValidClaudeSettings relativePath =
    use document = readJson relativePath
    let root = document.RootElement
    Expect.equal root.ValueKind JsonValueKind.Object $"{relativePath} is a JSON object"
    let mutable permissions = Unchecked.defaultof<JsonElement>
    Expect.isTrue (root.TryGetProperty("permissions", &permissions)) $"{relativePath} declares project permissions"

    let text = read relativePath
    Expect.isFalse (text.Contains("settings.local.json", StringComparison.OrdinalIgnoreCase)) $"{relativePath} does not require settings.local.json"
    Expect.isFalse (text.Contains(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), StringComparison.OrdinalIgnoreCase)) $"{relativePath} does not contain user home paths"

    if text.Contains("\"hooks\"", StringComparison.Ordinal) then
        Expect.stringContains text "$CLAUDE_PROJECT_DIR" $"{relativePath} hook command is project-local"

[<Tests>]
let claudeCodeReadyTests =
    testList "Claude Code readiness" [
        test "repository CLAUDE imports AGENTS and preserves active plan guidance without duplicating Codex body" {
            expectPathsExist
                "repository Claude artifacts"
                [ "CLAUDE.md"
                  ".claude/settings.json"
                  ".claude/hooks/validate-speckit-project.sh" ]

            let claude = read "CLAUDE.md"
            Expect.stringContains claude "@AGENTS.md" "CLAUDE imports AGENTS"
            Expect.stringContains claude ".claude/skills/" "CLAUDE points to project skills"
            Expect.isFalse (claude.Contains("specs/032-sokoban-feedback-followups/plan.md")) "CLAUDE does not duplicate the active AGENTS plan line"
            expectFileContains "AGENTS.md" [ "specs/032-sokoban-feedback-followups/plan.md" ]
        }

        test "repository Claude skills exist for every Codex workflow with identical content" {
            repositorySkillIds ()
            |> List.iter (fun skillId ->
                let codexPath = $".agents/skills/{skillId}/SKILL.md"
                let claudePath = $".claude/skills/{skillId}/SKILL.md"
                Expect.isTrue (fileExists claudePath) $"Claude skill exists for {skillId}"
                Expect.equal (read claudePath) (read codexPath) $"Claude skill content matches Codex for {skillId}")
        }

        test "repository Claude settings are project-shareable and hook scripts are local" {
            expectValidClaudeSettings ".claude/settings.json"
            expectFileContains ".claude/settings.json" [ "$CLAUDE_PROJECT_DIR/.claude/hooks/validate-speckit-project.sh" ]
            expectFileContains ".claude/hooks/validate-speckit-project.sh" [ "AGENTS.md"; "feature_directory"; "plan.md" ]
        }

        test "generated template source and package mappings include Claude project artifacts and skill peers" {
            expectPathsExist
                "template Claude artifacts"
                [ "template/base/CLAUDE.md"
                  "template/base/.claude/settings.json"
                  "template/base/.claude/hooks/validate-speckit-project.sh"
                  "template/base/.claude/skills/fs-skia-project/SKILL.md"
                  ".template.config/generated/CLAUDE.md"
                  ".template.config/generated/.claude/settings.json" ]

            expectValidClaudeSettings "template/base/.claude/settings.json"

            expectFileContains
                ".template.config/template.json"
                [ "\"target\": \".claude/skills/\""
                  "\"target\": \".claude/skills/fs-skia-scene/\""
                  "\"target\": \".claude/skills/fs-skia-skiaviewer/\""
                  "\"target\": \".claude/skills/fs-skia-elmish/\""
                  "\"target\": \".claude/skills/fs-skia-keyboard-input/\""
                  "\"target\": \".claude/skills/fs-skia-ui-widgets/\""
                  "\"target\": \".claude/skills/fs-skia-testing/\""
                  "\"target\": \".claude/skills/fs-skia-samples/\"" ]

            expectFileContains
                "build.fsx"
                [ "content/template/base/CLAUDE.md"
                  "content/template/base/.claude/settings.json"
                  "content/.claude/skills/speckit-specify/SKILL.md"
                  "missing Claude skill peers"
                  "Selected Claude skills:" ]
        }

        test "template drift fixtures report Codex Claude mismatch fields and repair actions" {
            let outputPath = fullPath "specs/030-claude-code-ready/readiness/fixtures/codex-claude-drift.md"
            let exitCode, stdout, stderr = runProcess "dotnet" $"fsi scripts/template-drift.fsx --fixture codex-claude-codex-drift {outputPath}"
            let output = stdout + stderr
            Expect.notEqual exitCode 0 "Codex drift fixture fails"

            [ "scope=repository"
              "sourceId=speckit-plan"
              "workflowId=speckit-plan"
              "expectedPath=.agents/skills/speckit-plan/SKILL.md"
              "actualPath=.claude/skills/speckit-plan/SKILL.md"
              "differenceSummary=Codex skill changed without Claude peer update"
              "repairAction=regenerate Codex and Claude skills from the shared source" ]
            |> List.iter (fun needle -> Expect.stringContains output needle $"drift output includes {needle}")
        }

        test "Claude-side drift fixture fails with symmetric diagnostics" {
            let outputPath = fullPath "specs/030-claude-code-ready/readiness/fixtures/claude-codex-drift.md"
            let exitCode, stdout, stderr = runProcess "dotnet" $"fsi scripts/template-drift.fsx --fixture codex-claude-claude-drift {outputPath}"
            let output = stdout + stderr
            Expect.notEqual exitCode 0 "Claude drift fixture fails"
            Expect.stringContains output "differenceSummary=Claude skill changed without Codex peer update" "diagnostic names Claude-only drift"
            Expect.stringContains output "repairAction=regenerate Codex and Claude skills from the shared source" "diagnostic names repair action"
        }

        test "Claude research readiness cites official sources and maps them to artifacts" {
            expectFileContains
                "specs/030-claude-code-ready/readiness/claude-code-research.md"
                [ "Retrieved: 2026-05-29"
                  "docs.anthropic.com"
                  "CLAUDE.md"
                  ".claude/settings.json"
                  ".claude/hooks"
                  ".claude/commands"
                  ".claude/skills" ]
        }
    ]
