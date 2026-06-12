# Task Graph — 110-retained-pointer-routing

## ✓ Graph is acyclic and consistent

## Skill Match Assessments

| Task | Candidate | Confidence | Signals | Reviewer disposition | Diagnostic |
|------|-----------|------------|---------|----------------------|------------|
| T001 | (none) | none |  | accepted-empty | T001: skillist trusted as declared; no owns-based capability requirement |
| T002 | (none) | none |  | accepted-empty | T002: skillist trusted as declared; no owns-based capability requirement |
| T003 | (none) | none |  | accepted-empty | T003: skillist trusted as declared; no owns-based capability requirement |
| T004 | (none) | none |  | declared | T004: skillist trusted as declared; no owns-based capability requirement |
| T005 | (none) | none |  | declared | T005: skillist trusted as declared; no owns-based capability requirement |
| T006 | (none) | none |  | declared | T006: skillist trusted as declared; no owns-based capability requirement |
| T007 | (none) | none |  | declared | T007: skillist trusted as declared; no owns-based capability requirement |
| T008 | (none) | none |  | declared | T008: skillist trusted as declared; no owns-based capability requirement |
| T009 | (none) | none |  | accepted-empty | T009: skillist trusted as declared; no owns-based capability requirement |
| T010 | (none) | none |  | declared | T010: skillist trusted as declared; no owns-based capability requirement |
| T011 | (none) | none |  | declared | T011: skillist trusted as declared; no owns-based capability requirement |
| T012 | (none) | none |  | declared | T012: skillist trusted as declared; no owns-based capability requirement |
| T013 | (none) | none |  | declared | T013: skillist trusted as declared; no owns-based capability requirement |
| T014 | (none) | none |  | declared | T014: skillist trusted as declared; no owns-based capability requirement |
| T015 | (none) | none |  | declared | T015: skillist trusted as declared; no owns-based capability requirement |
| T016 | (none) | none |  | accepted-empty | T016: skillist trusted as declared; no owns-based capability requirement |
| T017 | (none) | none |  | declared | T017: skillist trusted as declared; no owns-based capability requirement |
| T018 | (none) | none |  | declared | T018: skillist trusted as declared; no owns-based capability requirement |
| T019 | (none) | none |  | declared | T019: skillist trusted as declared; no owns-based capability requirement |
| T020 | (none) | none |  | declared | T020: skillist trusted as declared; no owns-based capability requirement |
| T021 | (none) | none |  | declared | T021: skillist trusted as declared; no owns-based capability requirement |
| T022 | (none) | none |  | declared | T022: skillist trusted as declared; no owns-based capability requirement |
| T023 | (none) | none |  | declared | T023: skillist trusted as declared; no owns-based capability requirement |
| T024 | (none) | none |  | declared | T024: skillist trusted as declared; no owns-based capability requirement |
| T025 | (none) | none |  | declared | T025: skillist trusted as declared; no owns-based capability requirement |
| T026 | (none) | none |  | declared | T026: skillist trusted as declared; no owns-based capability requirement |
| T027 | (none) | none |  | declared | T027: skillist trusted as declared; no owns-based capability requirement |
| T028 | speckit-evidence-graph | high | owns:graph-validation | accepted | T028: owns graph-validation requires skill speckit-evidence-graph; trigger_group=owns; matched_trigger=owns:graph-validation |
| T029 | speckit-evidence-audit | high | owns:evidence-audit | accepted | T029: owns evidence-audit requires skill speckit-evidence-audit; trigger_group=owns; matched_trigger=owns:evidence-audit |

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 29 |
| [S] synthetic | 0 |
| [S*] auto-synthetic | 0 |
| accepted [SEH] synthetic | 0 |
| unaccepted synthetic | 0 |

## Graph

```mermaid
graph TD
  T001["T001 Scaffold `specs/110-retained-pointer-routing/` and"]:::done
  T002["T002 Create the `specs/110-retained-pointer-routing/rea"]:::done
  T003["T003 Record feature Tier (Tier 1), affected package (`F"]:::done
  T004["T004 Add `FullRenderFallbackCount: int` to the `FrameMe"]:::done
  T005["T005 Add the internal retained-id → authored-control-id"]:::done
  T006["T006 Retain the retained step's `ControlRenderResult` ("]:::done
  T007["T007 Exercise the drafted `FrameMetrics` shape and the "]:::done
  T008["T008 Capture the intended surface + per-package baselin"]:::done
  T009["T009 Record unsupported-scope handling and failure diag"]:::done
  T010["T010 Add a failing-first metrics test through `Perf.run"]:::done
  T011["T011 Add a burst-coalescing test: N move samples in one"]:::done
  T012["T012 Implement the internal retained route: run `Pointe"]:::done
  T013["T013 Wire `runInteractiveApp`'s `processInput` (`Contro"]:::done
  T014["T014 Wire `Perf.runScript`'s `routeInteraction` (`Contr"]:::done
  T015["T015 Narrow `FullRenderCount`/`ViewCalled` so a retaine"]:::done
  T016["T016 Document the US1 independent validation path (run "]:::done
  T017["T017 Add `Feature110RetainedRoutingParityTests` compari"]:::done
  T018["T018 Add the targeted parity cases: an unkeyed same-kin"]:::done
  T019["T019 Make the T005 lookup resolve the exact authored id"]:::done
  T020["T020 Verify focus-outcome parity: a click that also mov"]:::done
  T021["T021 Add a test asserting every normal scripted pointer"]:::done
  T022["T022 Add `Feature110FallbackTests`: a deliberately cons"]:::done
  T023["T023 Implement the counted fallback: when the retained "]:::done
  T024["T024 Regenerate the feature-109 corpus pointer goldens "]:::done
  T025["T025 Run `./fake.sh build -t RefreshSurfaceBaselines` t"]:::done
  T026["T026 Confirm the new field's XML-doc satisfies the doc-"]:::done
  T027["T027 Run the escalated controls-public-surface gates se"]:::done
  T028["T028 Run `./fake.sh build -t EvidenceGraph` — confirm n"]:::done
  T029["T029 Run `./fake.sh build -t EvidenceAudit` — confirm v"]:::done
  T003 -. injected .-> T004
  T003 -. injected .-> T005
  T003 -. injected .-> T006
  T004 --> T007
  T005 --> T007
  T006 --> T007
  T003 -. injected .-> T007
  T004 --> T008
  T003 -. injected .-> T008
  T003 -. injected .-> T009
  T009 -. injected .-> T010
  T009 -. injected .-> T011
  T010 --> T012
  T011 --> T012
  T005 --> T012
  T006 --> T012
  T009 -. injected .-> T012
  T012 --> T013
  T009 -. injected .-> T013
  T012 --> T014
  T009 -. injected .-> T014
  T012 --> T015
  T004 --> T015
  T009 -. injected .-> T015
  T013 --> T016
  T014 --> T016
  T009 -. injected .-> T016
  T016 -. injected .-> T017
  T016 -. injected .-> T018
  T017 --> T019
  T018 --> T019
  T012 --> T019
  T016 -. injected .-> T019
  T019 --> T020
  T016 -. injected .-> T020
  T020 -. injected .-> T021
  T020 -. injected .-> T022
  T021 --> T023
  T022 --> T023
  T012 --> T023
  T020 -. injected .-> T023
  T023 --> T024
  T020 -. injected .-> T024
  T004 --> T025
  T024 -. injected .-> T025
  T025 --> T026
  T024 -. injected .-> T026
  T025 --> T027
  T024 -. injected .-> T027
  T027 --> T028
  T024 -. injected .-> T028
  T028 --> T029
  T024 -. injected .-> T029
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Scaffold `specs/110-retained-pointer-routing/` and confirm spec + plan + research + data-model + contracts + quickstart are linked and current
T002 [X] Create the `specs/110-retained-pointer-routing/readiness/` scaffolds discoverable before implementation — `evidence-audit.md`, `evidence-graph.md`, `skill-loading-evidence.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, and the window-visibility not-applicable set — each naming its authoritative command, artifact path, failure class, and next action
T003 [X] Record feature Tier (Tier 1), affected package (`FS.Skia.UI.Controls.Elmish` + internal `FS.Skia.UI.Controls` retained surface), public-API impact (`FrameMetrics` field), Elmish/MVU applicability (unchanged — N/A with rationale above), and the required evidence obligations (parity oracle, forced fallback, regenerated goldens, baselines, XML-doc)
T004 [X] Add `FullRenderFallbackCount: int` to the `FrameMetrics` record in `ControlsElmish.fsi` with XML-doc, mirror it in the `.fs` definition, and update **every** construction site so the build compiles (`emitFrameMetrics` ~`ControlsElmish.fs:804`, `zero` ~`1076`, move ~`1107`, tick ~`1144`, key ~`1162`, discrete ~`1178`) plus the test serializer `Feature109CorpusTests.fs:153`
T005 [X] Add the internal retained-id → authored-control-id lookup to `RetainedRender` (`RetainedRender.fsi` internal seam + `RetainedRender.fs`), built from the step output and reproducing `Control.nearestAuthored`'s keyed-OR-in-`BoundIds` resolution from retained identity (feature 098 scheme)
T006 [X] Retain the retained step's `ControlRenderResult` (`s.Render`) in a live-loop ref (seeded from `r0.Render` on first frame, `ControlsElmish.fs:763-773`) and carry it alongside the threaded retained value in `Perf.runScript` (`ControlsElmish.fs:1042-1053`) so routing reads `EventBindings`/`BoundIds`/`Bounds` without a fresh render
T007 [X] Exercise the drafted `FrameMetrics` shape and the internal retained seam from FSI (prelude or ad-hoc), capturing the session transcript to `readiness/fsi-session.txt`
T008 [X] Capture the intended surface + per-package baseline shape for the `FrameMetrics` change (the authoritative regen happens in T025) and note it in `readiness/`
T009 [X] Record unsupported-scope handling and failure diagnostics: Phase 3+ of the report is OUT; document that the full-render path is preserved as oracle/fallback and that the fallback degrades to correct dispatch (never silent mis-dispatch)
T010 [X] Add a failing-first metrics test through `Perf.runScript` asserting a routed move and a routed press/click each perform **zero** routing full renders — `FullRenderCount` is not incremented for routing and `ViewCalled` stays false on a pure routing frame (SC-001, SC-002)
T011 [X] Add a burst-coalescing test: N move samples in one frame report `PointerMovesProcessed <= 1` with zero routing full renders, and no discrete press/release/click/scroll is dropped; **also** assert drag/freehand **path fidelity** for a path-consuming consumer is retained through the retained route (the per-sample path the consumer observes is unchanged from the 108/109 baseline) (SC-009, FR-012)
T012 [X] Implement the internal retained route: run `Pointer.update` over the retained frame's **cached** `LayoutResult`, resolve each interaction via `retainedHitTest` → the T005 lookup → the retained frame's `EventBindings`, with the unchanged `MapPointer` fallback for unbound interactions — performing no `host.View` + `Control.renderTree` for routing
T013 [X] Wire `runInteractiveApp`'s `processInput` (`ControlsElmish.fs:816-837`) onto the retained route, keeping the already-retained focus-on-click `resolveFocus` path
T014 [X] Wire `Perf.runScript`'s `routeInteraction` (`ControlsElmish.fs:1058-1066`) onto the retained route over the threaded retained frame instead of re-rendering
T015 [X] Narrow `FullRenderCount`/`ViewCalled` so a retained routing frame increments neither, and thread the frame-local `FullRenderFallbackCount` accumulator through `emitFrameMetrics` and every `Perf.runScript` frame branch (FR-008)
T016 [X] Document the US1 independent validation path (run the move-then-click perf script; assert routing full renders are zero with correct hit + messages) in `readiness/`
T017 [X] Add `Feature110RetainedRoutingParityTests` comparing the retained route against the preserved `routeInteractivePointer` oracle over keyed / unkeyed-same-kind-sibling / composite / nested scenes: dispatched message list, matched control identity, and focus outcome are equal (structural comparison, no value equality) (SC-003)
T018 [X] Add the targeted parity cases: an unkeyed same-kind sibling hit selects the same sibling and fires the same binding, and a composite control whose binding is authored above the hit node dispatches the same authored binding (SC-004, FR-003/FR-005)
T019 [X] Make the T005 lookup resolve the exact authored id `nearestAuthored` would (composite-binding-above-hit climb; distinct retained ids for unkeyed siblings) so the parity tests pass
T020 [X] Verify focus-outcome parity: a click that also moves focus yields the same focused identity via the retained path as the oracle (FR-006 focus clause)
T021 [X] Add a test asserting every normal scripted pointer scenario in the corpus reports `FullRenderFallbackCount = 0` for every frame (SC-005)
T022 [X] Add `Feature110FallbackTests`: a deliberately constructed unroutable case increments `FullRenderFallbackCount` by one and the fallback dispatch still equals the oracle's (SC-006). Real evidence — the fallback runs the preserved oracle (real product code), so this is not synthetic
T023 [X] Implement the counted fallback: when the retained route cannot resolve an event from the retained frame, fall back to the preserved `routeInteractivePointer` oracle and increment `FullRenderFallbackCount` (FR-007/FR-009)
T024 [X] Regenerate the feature-109 corpus pointer goldens (`PERF_CORPUS_REGEN=1`) so routing full-render counts drop to zero, and record the before/after delta in `readiness/`; **also** confirm the at-rest **rendered output + control geometry** byte-identity clause of FR-011/SC-008 — assert no rendered-scene/geometry golden delta against the pre-feature state (the standing Scene-parity golden suite run under `Dev`/T027 is the authority) and record that authority decision in `readiness/byte-identity-authority.md` (SC-007, SC-008, FR-010, FR-011)
T025 [X] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate the surface + per-package baselines for the `FrameMetrics` field, and update any remaining `FrameMetrics` construction/read sites it flags (samples, FSI preludes)
T026 [X] Confirm the new field's XML-doc satisfies the doc-preservation gate and the public `routeInteractivePointer` signature is unchanged (oracle/fallback preserved)
T027 [X] Run the escalated controls-public-surface gates sequentially — `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck` — and record the focused governance risk level + non-authoritative aggregate notes in `readiness/`
T028 [X] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and the echoed `feature-directory`/`tasks=<n>` match this feature
T029 [X] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with no remaining `[S]`/`[S*]` and no diff-scan hits, or document every `--accept-synthetic` override
```

## Injected checkpoint edges (Phase N+1 → Phase N) — FR-007

- T003 → T004  (auto-injected Phase-checkpoint edge)
- T003 → T005  (auto-injected Phase-checkpoint edge)
- T003 → T006  (auto-injected Phase-checkpoint edge)
- T003 → T007  (auto-injected Phase-checkpoint edge)
- T003 → T008  (auto-injected Phase-checkpoint edge)
- T003 → T009  (auto-injected Phase-checkpoint edge)
- T009 → T010  (auto-injected Phase-checkpoint edge)
- T009 → T011  (auto-injected Phase-checkpoint edge)
- T009 → T012  (auto-injected Phase-checkpoint edge)
- T009 → T013  (auto-injected Phase-checkpoint edge)
- T009 → T014  (auto-injected Phase-checkpoint edge)
- T009 → T015  (auto-injected Phase-checkpoint edge)
- T009 → T016  (auto-injected Phase-checkpoint edge)
- T016 → T017  (auto-injected Phase-checkpoint edge)
- T016 → T018  (auto-injected Phase-checkpoint edge)
- T016 → T019  (auto-injected Phase-checkpoint edge)
- T016 → T020  (auto-injected Phase-checkpoint edge)
- T020 → T021  (auto-injected Phase-checkpoint edge)
- T020 → T022  (auto-injected Phase-checkpoint edge)
- T020 → T023  (auto-injected Phase-checkpoint edge)
- T020 → T024  (auto-injected Phase-checkpoint edge)
- T024 → T025  (auto-injected Phase-checkpoint edge)
- T024 → T026  (auto-injected Phase-checkpoint edge)
- T024 → T027  (auto-injected Phase-checkpoint edge)
- T024 → T028  (auto-injected Phase-checkpoint edge)
- T024 → T029  (auto-injected Phase-checkpoint edge)

## Resolved skillist ids — FR-007

Resolved skillist-id set (7): fs-skia-controls-host, fs-skia-evidence-mode, fs-skia-reconciliation, fs-skia-template-update, fs-skia-ui-widgets, speckit-evidence-audit, speckit-evidence-graph

## Skillist id → SKILL.md path

fs-skia-controls-host → .agents/skills/fs-skia-controls-host/SKILL.md
fs-skia-evidence-mode → .agents/skills/fs-skia-evidence-mode/SKILL.md
fs-skia-reconciliation → .agents/skills/fs-skia-reconciliation/SKILL.md
fs-skia-template-update → .agents/skills/fs-skia-template-update/SKILL.md
fs-skia-ui-widgets → src/Controls/skill/SKILL.md
speckit-evidence-audit → .agents/skills/speckit-evidence-audit/SKILL.md
speckit-evidence-graph → .agents/skills/speckit-evidence-graph/SKILL.md

## Skillist id → unresolved / flagged

_(none — every declared skillist id resolves to exactly one installed skill)_

