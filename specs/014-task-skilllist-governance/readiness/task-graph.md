# Task Graph — 014-task-skilllist-governance

## ✓ Graph is acyclic and consistent

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 33 |
| [S] synthetic | 0 |
| [S*] auto-synthetic | 0 |

## Graph

```mermaid
graph TD
  T001["T001 Review the spec, plan, contract, existing task tem"]:::done
  T002["T002 Create readiness directory scaffolding for validat"]:::done
  T003["T003 Record Tier 1 governance scope, affected workflow "]:::done
  T004["T004 Capture the canonical capability skill inventory u"]:::done
  T005["T005 Document that no `.fsi` contract or runtime public"]:::done
  T006["T006 Finalize the structured task metadata shape with `"]:::done
  T007["T007 Add failing-first readiness fixtures for missing s"]:::done
  T008["T008 Add failing-first readiness fixtures for mirror mi"]:::done
  T009["T009 Add implementation-loading fixtures for valid skil"]:::done
  T010["T010 Define the diagnostics contract for task id, faili"]:::done
  T011["T011 Add validation coverage that rejects task lists mi"]:::done
  T012["T012 Add validation coverage that rejects mirror mismat"]:::done
  T013["T013 Add generated-guidance coverage requiring task tem"]:::done
  T014["T014 Extend task graph parsing to read object-form `tas"]:::done
  T015["T015 Implement readiness validation for required `skill"]:::done
  T016["T016 Update root and preset task templates to include t"]:::done
  T017["T017 Update `/speckit.tasks` command and skill guidance"]:::done
  T018["T018 Record readiness evidence for invalid fixtures, a "]:::done
  T019["T019 Add generated-guidance coverage that `/speckit.imp"]:::done
  T020["T020 Add blocking-path fixtures for missing, unreadable"]:::done
  T021["T021 Update root and preset implementation skill guidan"]:::done
  T022["T022 Update implementation command guidance to record p"]:::done
  T023["T023 Record readiness evidence that valid non-empty `sk"]:::done
  T024["T024 Add generated-guidance coverage that the constitut"]:::done
  T025["T025 Update `.specify/memory/constitution.md` with the "]:::done
  T026["T026 Update root and preset constitution templates so g"]:::done
  T027["T027 Verify contributors can find the task-generation a"]:::done
  T028["T028 Update `.specify/integrations/codex.manifest.json`"]:::done
  T029["T029 Run `./fake.sh build -t EvidenceGraph` and capture"]:::done
  T030["T030 Run `./fake.sh build -t EvidenceAudit` and capture"]:::done
  T031["T031 Run `./fake.sh build -t GeneratedGuidanceCheck` an"]:::done
  T032["T032 Run `./fake.sh build -t Dev` and capture the final"]:::done
  T033["T033 Complete readiness notes, including fixture invent"]:::done
  T004 --> T005
  T004 --> T006
  T006 --> T007
  T004 --> T007
  T006 --> T008
  T004 --> T008
  T006 --> T009
  T004 --> T009
  T006 --> T010
  T004 --> T010
  T007 --> T011
  T010 --> T011
  T008 --> T012
  T010 --> T012
  T010 --> T013
  T011 --> T014
  T012 --> T014
  T010 --> T014
  T014 --> T015
  T010 --> T015
  T013 --> T016
  T010 --> T016
  T013 --> T017
  T010 --> T017
  T015 --> T018
  T016 --> T018
  T017 --> T018
  T010 --> T018
  T018 --> T019
  T009 --> T020
  T018 --> T020
  T019 --> T021
  T020 --> T021
  T018 --> T021
  T021 --> T022
  T018 --> T022
  T020 --> T023
  T022 --> T023
  T018 --> T023
  T023 --> T024
  T024 --> T025
  T023 --> T025
  T024 --> T026
  T023 --> T026
  T025 --> T027
  T026 --> T027
  T023 --> T027
  T016 --> T028
  T017 --> T028
  T021 --> T028
  T022 --> T028
  T025 --> T028
  T026 --> T028
  T027 --> T028
  T018 --> T029
  T023 --> T029
  T027 --> T029
  T029 --> T030
  T027 --> T030
  T028 --> T031
  T027 --> T031
  T029 --> T032
  T030 --> T032
  T031 --> T032
  T027 --> T032
  T029 --> T033
  T030 --> T033
  T031 --> T033
  T032 --> T033
  T027 --> T033
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Review the spec, plan, contract, existing task templates, and active skill inventory for this feature
T002 [X] Create readiness directory scaffolding for validation logs and task skilllist fixtures
T003 [X] Record Tier 1 governance scope, affected workflow surfaces, no public API impact, no package impact, and Principle IV non-applicability
T004 [X] Capture the canonical capability skill inventory used for post-task skill evaluation
T005 [X] Document that no `.fsi` contract or runtime public API surface is introduced by this feature
T006 [X] Finalize the structured task metadata shape with `deps` and `skillist` fields plus the `tasks.md` mirror format
T007 [X] Add failing-first readiness fixtures for missing structured `skillist`, non-list `skillist`, missing task mirrors, and existing bare-list metadata that must be migrated or regenerated
T008 [X] Add failing-first readiness fixtures for mirror mismatch, omitted obviously applicable capability skills, excess non-minimal skills, and invalid multi-skill dependency order
T009 [X] Add implementation-loading fixtures for valid skill loads, missing skills, unreadable skills, and ambiguous skill ids
T010 [X] Define the diagnostics contract for task id, failing field, unresolved skill id, and readiness-blocking verdicts
T011 [X] Add validation coverage that rejects task lists missing structured `skillist` fields or `tasks.md` mirrors
T012 [X] Add validation coverage that rejects mirror mismatches and omitted obvious capability skills
T013 [X] Add generated-guidance coverage requiring task templates and task-generation guidance to emit `skillist` values
T014 [X] Extend task graph parsing to read object-form `tasks.deps.yml` entries with `deps` and `skillist`
T015 [X] Implement readiness validation for required `skillist`, list typing, mirror presence, mirror equality, declared skill resolution, obvious capability omissions, non-minimal skill sets, multi-skill dependency order, and existing task-list migration blockers
T016 [X] Update root and preset task templates to include the `skillist` mirror on every task line and structured metadata for every task
T017 [X] Update `/speckit.tasks` command and skill guidance to require post-generation skill evaluation and minimal ordered skill selection
T018 [X] Record readiness evidence for invalid fixtures, a valid task list containing explicit empty and non-empty `skillist` values, and under-30-second missing-`skillist` rejection timing
T019 [X] Add generated-guidance coverage that `/speckit.implement` must load each task's declared skills before implementation
T020 [X] Add blocking-path fixtures for missing, unreadable, ambiguous declared task skills, and existing task lists that must be migrated or regenerated before implementation
T021 [X] Update root and preset implementation skill guidance to read `skillist`, resolve each skill, load skills in order, and stop on failures
T022 [X] Update implementation command guidance to record per-task skill-load evidence before code changes begin
T023 [X] Record readiness evidence that valid non-empty `skillist` entries are loaded and invalid entries block implementation
T024 [X] Add generated-guidance coverage that the constitution and constitution template require post-task skill evaluation and implementation-time loading
T025 [X] Update `.specify/memory/constitution.md` with the mandatory `skillist` governance gate
T026 [X] Update root and preset constitution templates so generated products inherit the mandatory `skillist` rule
T027 [X] Verify contributors can find the task-generation and implementation-loading obligations from the constitution and generated guidance
T028 [X] Update `.specify/integrations/codex.manifest.json` if generated skill hashes or governed integration state changed
T029 [X] Run `./fake.sh build -t EvidenceGraph` and capture `readiness/logs/evidence-graph.txt`
T030 [X] Run `./fake.sh build -t EvidenceAudit` and capture `readiness/logs/evidence-audit.txt`
T031 [X] Run `./fake.sh build -t GeneratedGuidanceCheck` and capture `readiness/logs/generated-guidance-check.txt`
T032 [X] Run `./fake.sh build -t Dev` and capture the final governed verification result
T033 [X] Complete readiness notes, including fixture inventory, validator diagnostics, implementation-load evidence, and any synthetic-evidence disclosures
```

