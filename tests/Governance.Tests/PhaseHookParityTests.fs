module PhaseHookParityTests

(* Feature 077 / FR-006 — failing-first (Principle I/VI) tests for the phase-hook parity
   guard extracted into FS.Skia.UI.Build.PhaseHookParity. The positive corpus assertions
   (T012/T016) are RED until the four deficient phase skills are repaired; the negative
   assertion (T006) is the synthetic-error-handling proof that the guard bites. *)

open System.IO
open Expecto
open FS.Skia.UI.Build
open GovernanceTestSupport

// Build a ParsedPhaseSkill from a real SKILL.md under the repo (None if absent).
let private parseReal (treeRoot: string) (phase: string) =
    let rel = sprintf "%s/skills/speckit-%s/SKILL.md" treeRoot phase
    let full = fullPath rel

    if File.Exists full then
        Some
            { PhaseHookParity.Phase = phase
              PhaseHookParity.RelPath = rel
              PhaseHookParity.Body = File.ReadAllText full }
    else
        None

let private corpusFor (treeRoot: string) =
    PhaseHookParity.roster |> List.choose (parseReal treeRoot)

[<Tests>]
let phaseHookParityTests =
    testList
        "Feature 077 PhaseHookParity"
        [

          // T006 [SEH] synthetic-error-handling-approved — the negative guard proof. A
          // block-stripped SKILL.md body (modern markers removed) must yield a phase-hook
          // parity finding naming each missing marker. SYNTHETIC: in-memory block-stripped body
          // (accepted-seh); keeping a permanently broken real SKILL.md in the tree is infeasible.
          test "checkCorpus_Synthetic_flags a block-stripped implement skill body" {
              let stripped =
                  { PhaseHookParity.Phase = "implement"
                    PhaseHookParity.RelPath = ".agents/skills/speckit-implement/SKILL.md"
                    PhaseHookParity.Body = "# Implement\n\nNo hook discovery block here at all.\n" }

              let implementFindings =
                  PhaseHookParity.checkCorpus [ stripped ]
                  |> List.filter (fun f -> f.Path.Contains "speckit-implement")

              Expect.isNonEmpty implementFindings "a block-stripped body yields phase-hook parity findings"

              Expect.isTrue
                  (implementFindings |> List.forall (fun f -> f.ArtifactClass = "phase-hook-parity"))
                  "every emitted finding is a phase-hook-parity finding"

              Expect.isTrue
                  (implementFindings
                   |> List.exists (fun f -> f.Message.Contains "## Effective hooks for implement notice"))
                  "the missing consolidated-notice marker is named"
          }

          // T012 [US1] — the real speckit-implement skill (canonical + generated mirror) carries
          // the modern block. RED until T013 repairs `.agents/skills/speckit-implement/SKILL.md`.
          test "the real speckit-implement skill carries the modern hook block (.agents + .claude)" {
              for treeRoot in [ ".agents"; ".claude" ] do
                  match parseReal treeRoot "implement" with
                  | None -> failtestf "%s/skills/speckit-implement/SKILL.md is missing" treeRoot
                  | Some skill ->
                      let findings =
                          PhaseHookParity.checkCorpus [ skill ]
                          |> List.filter (fun f -> f.Path.Contains "speckit-implement")

                      Expect.isEmpty
                          findings
                          (sprintf
                              "speckit-implement (%s) passes all three markers; missing: %A"
                              treeRoot
                              (findings |> List.map (fun f -> f.Message)))
          }

          // T016 [US2] (SC-002/SC-003) — every one of the nine roster phase skills, and each
          // `.claude` mirror, passes all three markers. RED until T017–T019 repair the rest.
          test "every roster phase skill carries the modern hook block (.agents + .claude)" {
              for treeRoot in [ ".agents"; ".claude" ] do
                  let corpus = corpusFor treeRoot

                  Expect.equal
                      (List.length corpus)
                      (List.length PhaseHookParity.roster)
                      (sprintf "all nine roster %s phase skills are present" treeRoot)

                  let findings = PhaseHookParity.checkCorpus corpus

                  Expect.isEmpty
                      findings
                      (sprintf
                          "all nine %s phase skills pass; violations: %A"
                          treeRoot
                          (findings |> List.map (fun f -> f.Message)))
          } ]
