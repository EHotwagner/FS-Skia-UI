# Data Model: Foundations Two-Tier Development Process (Stage 1)

All entities are build-tooling types in `FS.Skia.UI.Build` (`Routing` + `ContractView`
modules) plus the one additive `Targets.Target` case. Nothing here is a runtime/product type.

## Entities

### `DeveloperClass` (DU)
`FrameworkAuthor | ConsumerAgent`. The who-is-changing-what axis (FR-002). Defaults to
`FrameworkAuthor`. Raised by the optional `--developer-class consumer-agent` flag. **Does not**
gate path-based escalation: consumer-contract paths escalate for either class.
- **`ConsumerAgent` floor:** raises the **base tier** to `FocusedAuthority` (`maxBy tierRank` of the
  `FrameworkAuthor` base `InnerLoop` and `FocusedAuthority`); gates remain the computed union
  (`innerLoopGates` ∪ matched-rule gates), so an empty/unmatched diff under `ConsumerAgent` resolves
  to `FocusedAuthority` / `[Dev]`. Path-based escalation still composes on top.

### `Tier` (DU)
`InnerLoop | FocusedAuthority | AgentReady | MaintainerVerify | AutomationFinal | Tier1 | Tier2`.
Authoritative validation levels plus retained legacy aliases.
- **Total order:** `tierRank` → `InnerLoop 0 < FocusedAuthority 1 < AgentReady 2 < MaintainerVerify 3 < AutomationFinal 4`; `Tier2`→rank 0, `Tier1`→rank 2 (R3).
- **Escalation rule (FR-003):** resolved tier = `maxBy tierRank` over the base tier and every matched rule's tier. Never de-escalates.

### `Diff` (record)
`{ ChangedPaths: string list }` — repo-relative paths. The **union** of (a) `git diff --name-only <merge-base(HEAD,master)>...HEAD` and (b) `git status --porcelain --untracked-files=all` (FR-002a). Built at the `Route` edge; the selector is pure over it.
- **Empty diff:** deterministic inner-loop default (edge prints "no changes → inner-loop / Dev"), never a failure (edge case).

### `RoutingRule` (record)
`{ Id; Matches: Diff -> bool; Tier; RequiredGates: Targets.Target list; ExpectedArtifacts: string list; FailureOwner }`. Typed replacement for a YAML `routing_rules` entry.
- `RequiredGates` are `Targets.Target` values → a renamed/mistyped gate is a **compile error** (SC-006/FR-001).
- `Matches` uses repo fnmatch glob semantics (`BuildPaths`).

**Rule table (R5), derived from today's YAML — `template/**` and `.specify/**` broadened to full
consumer-contract coverage (F2); the regenerated `validation.contract.yml` reflects the broadened
rules:**

| Id | Paths (glob) | Tier | Required gates | Expected artifacts |
|----|--------------|------|----------------|--------------------|
| controls-public-surface | `src/Controls/**` | FocusedAuthority | ControlsCatalogCheck, ControlsInteractionCheck, ControlsRenderingCheck, PackageSurfaceCheck, FsiTranscripts, GeneratedProductCheck | typed-controls-front-door.md, package-surface-expectations.md |
| generated-template | `template/**` | FocusedAuthority | TemplateCheck, GeneratedProductCheck | evidence-policy-separation.md |
| evidence-governance | `specs/**/tasks.md`, `…/tasks.deps.yml`, `…/readiness/**`, `.specify/extensions/evidence/**` | AgentReady | EvidenceGraph, EvidenceAudit | validation-contract.md, evidence-graph.md, evidence-audit.md |
| generated-guidance | `.specify/templates/**`, `.specify/presets/**`, `template/fragments/**/README.md`, `…/skill/SKILL.md` | FocusedAuthority | GeneratedGuidanceCheck, TemplateDrift | evidence-policy-separation.md |
| specify-catchall | `.specify/**` | FocusedAuthority | GeneratedGuidanceCheck, TemplateDrift | evidence-policy-separation.md |
| docs-only | `docs/**`, `specs/**/contracts/**`, `specs/**/quickstart.md` | FocusedAuthority | EvidenceGraph | validation-contract.md |
| package-surface | `src/**/*.fsi`, `readiness/surface-baselines/**` | FocusedAuthority | PackageSurfaceCheck, FsiTranscripts | package-surface-expectations.md |
| build-target-contract | `build.fsx`, `scripts/build/**`, `validation.contract.yml` | MaintainerVerify | AgentReady, TargetMetadataDrift, EvidenceGraph, EvidenceAudit, Verify, Ci | target-metadata.md, agent-ready-verdict.md |

**Base (no rule matched) for `FrameworkAuthor`:** `InnerLoop`, gates = `innerLoopGates` = `[Dev]`. A public `src/**/*.fsi` change does **not** add a surface check here — it **escalates** via the `package-surface` rule (F1). **Base (no rule matched) for `ConsumerAgent`:** floor raised to `FocusedAuthority` (see `DeveloperClass`); gates = the computed union, `[Dev]` when no rule matched. **Unmatched path with no inner-loop applicability** → default-deny to the broad fallback `Verify` (`unknown_gate_rejection` preserved, FR-002).

### `GateSet`
The resolved `Selection.Gates`: union of matched rules' `RequiredGates` (plus inner-loop base when applicable), **de-duplicated in `Targets.allTargets` registry order** for deterministic, byte-stable `Route` output (SC-001/SC-002).

### `Selection` (record)
`{ DeveloperClass; Tier; Gates; MatchedRuleIds; ExpectedArtifacts; DogfoodForced }`. The pure output of `select` / `selectForFeature`.

### `DogfoodMarker`
`dogfoodFeatureIds: string list` — typed governance policy in `Routing.fs` (ADR D6), includes `"042"`. `selectForFeature` forces `fullPipelineGates`/`MaintainerVerify` with `DogfoodForced = true` when the active feature id is a member (FR-006, SC-005), regardless of the diff's tier.

### `Route` (Targets.Target case)
New additive union case. `TargetSpec`: TimeoutClass `focused`, Cost `low`, FailureOwner `governance`, `DirectPrerequisites = []` (the selector runs in-process; no build/DLL needed — keeps the entry point instant). Added to `allTargets`/`dispatchTargets`; metadata derives automatically from the total `spec`. No existing target's name/deps/position changes (FR-004/FR-016).

### `ContractView`
`render rules dogfoodFeatureIds : string` — the derived `validation.contract.yml` text (deterministic ordering). `currencyDrift onDiskContract rules dogfoodFeatureIds : string option` — `None` when the on-disk file equals `render` output, else the "regenerate from Routing.fs" diagnostic (FR-007, SC-007).

## State / validation transitions

```
git (edge) ──► Diff ──► select / selectForFeature (PURE) ──► Selection
                                                              │
                          ┌───────────────────────────────────┤
                   (print mode)                          (--enforce mode)
                   renderSelection                  unmetArtifacts present sel
                          │                                    │
                  stdout + readiness                  empty? exit 0 : exit≠0
                                                       (enforceDiagnostic)

Routing.rules ──► ContractView.render ──► validation.contract.yml
       │                                          ▲
       └── currencyDrift(on-disk) ── TargetMetadataDrift (detect)
                                  ── RefreshSurfaceBaselines (regen / write)
```

## Invariants

- **Escalate-only:** a change matching any rule never resolves below that rule's tier (FR-003).
- **Default-deny:** an unmatched, non-inner-loop path routes to `Verify`, never an empty success (FR-002, US2/edge).
- **Compile-checked gates:** every gate is a `Targets.Target`; a renamed target breaks the build (SC-006).
- **Single source:** `validation.contract.yml` is reproducible from `Routing.rules`/`dogfoodFeatureIds`; hand edits fail the currency check (FR-007, SC-007).
- **Purity:** `select`, `selectForFeature`, `unmetArtifacts`, `render`, `currencyDrift` do no I/O; git and `File.Exists` live only at the `Route`/`TargetMetadataDrift`/`RefreshSurfaceBaselines` edges (Principle IV).
