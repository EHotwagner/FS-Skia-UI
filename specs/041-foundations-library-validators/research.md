# Phase 0 Research — Foundations Library Validators (041)

All NEEDS CLARIFICATION items resolved. Each decision records what was chosen, why, and the
alternatives considered.

## R1 — The three reports' golden fixtures do NOT yet exist (parity-oracle provenance)

**Finding (verified, not assumed).** The spec's Assumptions state "the Stage 0 golden fixtures
committed in 039 (`tests/Governance.Tests/fixtures/`) are the authoritative parity oracle for the
three extracted targets." This is **partially inaccurate**: the only committed fixtures are
`tests/Governance.Tests/fixtures/evidence-golden/{036,037,038}/` containing `task-graph.json`,
`task-graph.md`, `audit-counts.txt` — i.e. the **`EvidenceGraph`/`EvidenceAudit`** outputs captured
for the *Stage 4* port. There are **no** `CapabilityCheck` / `TargetMetadata` / `TargetMetadataDrift`
golden fixtures anywhere under `fixtures/`, and `readiness/` does not yet contain these reports in the
working tree.

**Decision.** Capture the three reports from the **pinned pre-extraction baseline** (the current
`build.fsx` logic, before any module moves) and commit them as the parity oracle under
`tests/Governance.Tests/fixtures/reports-golden/`:
`capability-catalog.md`, `target-metadata.json`, `target-metadata-drift.md`. This capture is the
**first implementation task** and must land (or be staged) before any extraction edit, so the diff is
always against a frozen pre-move snapshot — never a moving tree. The byte-diff assertion (FR-008a)
then compares post-extraction output to these committed fixtures.

**Rationale.** The spec's *intent* — golden-diff parity gating the extraction (FR-006, SC-002) — is
unambiguous and the established 040/039 mechanism. Only the fixture *provenance* differs (capture-now
vs already-committed). The reports are byte-deterministic functions of committed inputs
(`template/capabilities.yml`, the target registries, `validation.contract.yml`, docs) modulo R2, so a
capture-now snapshot is exactly as authoritative as a Stage-0 one. Recorded here as an explicit
deviation from the spec Assumption wording (the spec is otherwise correct; this corrects one factual
claim it inherited).

**Alternatives considered.** (a) Treat the missing fixtures as a blocker and re-run Stage 0 — rejected:
wasteful, and Stage 0 is closed/SHA-pinned. (b) Capture into `readiness/` only (not `fixtures/`) —
rejected: FR-008a requires the assertion to run under the `Dev`/test gate, so the oracle must live
where Governance.Tests can read it deterministically (`fixtures/`), matching the evidence-golden
precedent.

## R2 — `TargetMetadata` JSON embeds a non-deterministic timestamp

**Finding.** `build.fsx` `StartTarget "TargetMetadata"` calls
`targetMetadataJson (DateTimeOffset.UtcNow.ToString("O")) [] metadata`, emitting
`"generated_at_utc": "<now>"`. A naive byte-diff against a committed fixture would fail on every run.

**Decision.** The parity comparison **normalizes** the `generated_at_utc` line before byte-comparison,
and the committed fixture stores a fixed sentinel (e.g. the literal captured value, with the parity
test replacing the live `generated_at_utc` value with the fixture's before comparing). The extracted
`TargetMetadata.targetMetadataJson` keeps `generatedAtUtc` as an **explicit parameter** (pure
function — the timestamp is injected by the caller, never read inside the library), so unit tests pass
a fixed timestamp and the build passes `DateTimeOffset.UtcNow`. Byte-identity is asserted over every
line **except** the single `generated_at_utc` value, which is asserted to be *present and
well-formed* rather than equal.

**Rationale.** Preserves Principle IV (no clock I/O inside the pure library function) and keeps the
report byte-for-byte identical in every field that carries meaning. The timestamp is the only
non-deterministic byte in any of the three reports (`capability-catalog.md` and
`target-metadata-drift.md` are fully deterministic).

**Alternatives considered.** Freezing the clock globally for the build — rejected: out of scope and
would alter unrelated targets. Dropping the timestamp from the report — rejected: changes the report
format (violates parity).

## R3 — Deriving metadata from the typed `Target` union (the "single source" mechanism)

**Finding.** Today three stringly-typed registries coexist: `requiredTargets` (string list, ~556–584),
`targetDependencyRows` (`(string * string list) list`, ~596–644), and `targetMetadata model target`
(~837–881) which itself calls `focusedGateContract model target` (the per-target command / log path /
readiness path / stale assumptions / verdict category machinery). `TargetMetadataDrift` exists purely
to detect when these disagree. Additionally `directPrerequisites` derives from `targetDependencyRows`.

**Decision.** Introduce in `Targets.fs` a typed `Target` discriminated union (one case per runnable
target) and a **total** function `spec : Target -> TargetSpec`, where `TargetSpec` carries the target's
direct prerequisites and the per-target attributes used to build metadata (timeout class, cost,
authority, failure owner, command, expected-output shape, stale assumptions). `requiredTargets`,
`targetDependencyRows`, `directPrerequisites`, and `targetMetadata` are all **derived** from
`allTargets |> List.map spec`. Because `spec` is a total match over the closed DU, a runnable target
without a metadata row (or vice-versa) is **unrepresentable** — the compiler enforces exhaustiveness
(SC-003). `build.fsx` converts every `StartTarget "..."` arm to dispatch on `Target` (FR-001); a
renamed/mistyped target becomes a compile error.

**Boundary / coupling note.** `targetMetadata` currently depends on `focusedGateContract`, which is
woven through `BuildModel` paths and `VerificationVerdictCategory`. To keep the extraction pure and
bounded, the library's `TargetSpec` owns the *intrinsic* per-target attributes; the few values that
genuinely depend on `BuildModel` runtime paths (e.g. concrete `LogPath`/`ReadinessPath` strings under
the readiness dir) are passed **in** to the library renderer as typed inputs from `build.fsx`'s
interpreter edge, not recomputed inside the library. This preserves Principle IV (I/O and
path-resolution stay at the edge) while still making the *identity + dependency + metadata-shape*
single-sourced in the DU. The residual derived checks that cannot be made type-impossible (contract
drift, docs drift — `validateTargetMetadataAgainstRepo`) remain pure functions and are covered by
unit tests asserting their typed diagnostics (SC-003 second clause).

**Rationale.** Matches FR-001 / clarification (full `Target` DU now, all dispatch arms) and ADR D6
(compiled-F# config, typed over stringly-typed). Totality is the structural guarantee the spec asks
for; the edge-injection of path strings avoids dragging the entire `BuildModel`/`focusedGateContract`
graph into the library this stage (that is Stage 5's engine relocation, FR-001a).

**Alternatives considered.** (a) Move `focusedGateContract` + `BuildModel` wholesale into the library
now — rejected: that is the Stage-5 MEL-engine relocation, explicitly out of scope; it would also
balloon this feature's blast radius and risk the parity goal. (b) Keep metadata derivation in
`build.fsx` and only move the *drift validator* — rejected: fails FR-001's "derived, not duplicated"
requirement and leaves the drift checkable-but-possible.

## R4 — Capability catalog: YamlDotNet behind the typed model (parser retirement)

**Finding.** `readCapabilityCatalog` (~2241–2308) is a hand-rolled, indentation-fragile line-by-line
state machine over `template/capabilities.yml`, producing `CapabilityRow` (build.fsx type ~37–52). It
relies on `parseScalar`/`trimQuotes`/`parseInlineList` from `BuildPackageResolution`. `YamlDotNet` is
already a managed dependency (present in the Governance.Tests output).

**Decision.** `Capabilities.fs` owns the `CapabilityRow` record and reads `template/capabilities.yml`
through **`YamlDotNet`** (deserialize to an intermediate POCO/`Dictionary` shape, then project to the
typed `CapabilityRow` list) — entirely replacing the bespoke parser, which is deleted from `build.fsx`
(SC-005: grep returns nothing). The YAML file is retained unchanged (FR-003); no new dependency
(FR-012). `validateCapabilityRows` becomes a pure function `CapabilityCatalog -> ValidationFinding
list` reporting **typed** error cases (the existing rule ids: `displayName`, `project`, `contracts`,
`tests`, `skill`, `templateFragment`, `profiles`, `evidence`, `surfaceBaseline`, `dependency`,
`default-app`).

**Parser-divergence guard (spec Edge Case 1).** Before deleting the bespoke parser, a parity check
asserts the `YamlDotNet`-projected model yields the **same `CapabilityRow` list** (and therefore the
same `CapabilityCheck` report) as the old parser over the committed `template/capabilities.yml`. If
any indentation quirk the old parser tolerated diverges, it surfaces in the golden-diff before the old
code is removed. `surfaceBaseline`'s file-existence check (`File.Exists`) stays at the I/O edge or is
passed a resolver, keeping the validator pure.

**Rationale.** ADR D6 + clarification (YAML-behind-typed-model, no file deleted/regenerated, reuse
`YamlDotNet`). A real YAML deserializer removes the indentation fragility the spec calls out, while
the golden-diff guarantees no behavioural change.

**Alternatives considered.** Inline compiled-F# `CapabilityRow list` values (ADR D6's eventual
end-state) — deferred: clarification fixed the source form as YAML-behind-the-model for *this* stage
(the file is consumed by template generation, so it stays data). Keeping the bespoke parser behind the
type — rejected by FR-003 (the parser must be retired).

## R5 — In-process call mechanism and test wiring (confirming the 040 pattern)

**Finding.** `build.fsx` consumes the library by `#load "build/Governance/<Module>.fs"` (lines
284–285 for SkillSync/SkillExamples) and calls `FS.Skia.UI.Build.<Module>.<fn>` from the interpret
edge (e.g. `runSkillSyncGate`, lines 4669+). `tests/Governance.Tests/Governance.Tests.fsproj` already
`ProjectReference`s `build/Governance/FS.Skia.UI.Build.fsproj`, so tests call the compiled library
directly (e.g. `SkillSyncTests.fs`).

**Decision.** Reuse this exact pattern: add the four new modules to the `.fsproj` `<Compile>` (after
the existing modules, in dependency order: `Findings`, `Targets`, `TargetMetadata`, `Capabilities`),
`#load` the four `.fs` into `build.fsx`, and have the three interpret cases call the library. New
Governance.Tests files are added to the test `.fsproj` `<Compile>`. The Stage-5 trigger (edge case 3)
is the only deviation: if `#load`/reference fails at extraction time, surface it explicitly rather
than re-inlining.

**Rationale.** Lowest-risk, already-proven in 040; keeps the feature a pure extraction.

**Alternatives considered.** `#r` the built DLL instead of `#load` the source — rejected: the repo's
established mechanism is `#load`; introducing a DLL-bootstrap ordering dependency is a Stage-5 concern.

## Summary of resolved decisions

| # | Question | Decision |
|---|---|---|
| R1 | Where is the parity oracle? | Capture 3 reports from pinned pre-extraction baseline → commit under `fixtures/reports-golden/` (first task) |
| R2 | `generated_at_utc` non-determinism | Timestamp is a pure function parameter; parity normalizes that one line |
| R3 | How is metadata single-sourced? | Typed `Target` DU + total `spec : Target -> TargetSpec`; registries/metadata derived; path strings injected at edge |
| R4 | Capability source form | `YamlDotNet`-behind-typed-model; bespoke parser deleted; pre-delete parity guard |
| R5 | In-process + test wiring | Reuse 040 pattern: `#load` into `build.fsx`, `ProjectReference` in tests |
