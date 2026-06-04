# Quickstart & Verification: Feature 062

End-to-end verification of the five workstreams. **Run `./fake.sh build -t Route`
first** and run only the gates it prints for the actual diff; FAKE-backed commands
share `.fake` state and run **sequentially** in deterministic order.

## 0. Route (authoritative gate list)

```bash
./fake.sh build -t Route          # re-run after each change-set
```
Spec-only baseline = `focused-authority` / `Dev, GeneratedGuidanceCheck,
TemplateDrift`. Expect escalation as template/skill/governance/`SkillSupport`
change-sets land (TemplateCheck, GeneratedProductCheck, SkillSyncCheck,
TargetMetadataDrift, SkillQualityCheck, EvidenceGraph, EvidenceAudit, and —
FR-010 only — PackageSurfaceCheck/PerPackageSurfaceDiff).

## 1. Hook policy (FR-001/002) — verified in a generated project

```bash
dotnet new fs-skia-ui -n SI062Probe --feedback true --allow-scripts yes </dev/null
```
- Inspect `.specify/extensions/feedback/feedback.yml` in the generated project:
  every `after_<phase>` entry is `optional: false`.
- Complete a phase and confirm `specs/<feature>/feedback/<phase>-<date>.md` is
  written with **no** manual trigger (SC-001).
- With an optional hook (git commit) registered, confirm the phase skill prints
  the **single effective-hooks notice** and applies the precedence rule with no
  clarifying round-trip.
- Repo-side: `TemplateCheck`/`GeneratedGuidanceCheck` assert no `optional: true`
  remains under feedback hooks.

## 2. Self-describing diagnostics (FR-004/005/007)

```bash
./fake.sh build -t Dev                               # FR-004: output + dev-verdict.txt say "does not compile; use Test/Verify"
./fake.sh build -t EvidenceGraph                     # FR-007: effective DAG w/ injected Phase N+1→N edges labeled + skillist set
```
- FR-005: in `SI062Probe`, trigger each evidence-format failure class
  (readiness-contract, skill-loading-evidence, window-visibility/diagnostic-class,
  SEH) and reach a passing `EvidenceAudit` using only the diagnostics and/or the
  generated `docs/evidence-formats.md` — **no `strings -el`, no sibling copy**.
  Log to `readiness/readiness-recoverability.md` (SC-002).

## 3. Authoring references (FR-003/006)

- `docs/scaffold-map.md` present in the generated project: names durable vs
  replaceable `src/**/*.fs`, the GovernanceTests-durable / BehaviorTests-replaceable
  split, must-survive scan strings, and the pre-design `fs-skia-scene`
  record-label pointer (SC-003).
- `docs/skillist-reference.md` generated from the live registry: valid `skillist`
  ids (id-vs-`name:` resolved) + closed `owns:` table; currency-checked (SC-004).

## 4. Symbol cross-check + pitfalls (FR-008/009)

- Seed a deliberate `Msg`-case drift (in `data-model.md`/`tasks.md`, not `plan.md`)
  and run analyze pass G — the set-difference is reported mechanically (SC-005).
- `fs-skia-skiaviewer` "Common pitfalls" covers `Result.Ok`/`Result.Error`
  shadowing (SC-005).

## 5. Shipped helpers (FR-010/012) — Tier 1

```fsharp
// FSI against the packed SkillSupport surface
open FS.Skia.UI.SkillSupport
let s0 = Random.seedRng 42UL
let v1, s1 = Random.nextRng s0
let v1', _ = Random.nextRng (Random.seedRng 42UL)
// replay equality: v1 = v1'
let layout = Hud.reserveHudBand 600.0 48.0 Hud.Top
// layout.HudBand.Size = 48.0; layout.Gameplay.Size = 552.0
```
```bash
./fake.sh build -t PackageSurfaceCheck               # new FS.Skia.UI.SkillSupport.txt baseline matches .fsi
dotnet test                                          # RNG determinism/replay, nextBelow bounds, reserveHudBand clamp
```
- `readiness/surface-baselines/FS.Skia.UI.SkillSupport.txt` exists and matches the
  `.fsi`; skill references added (SC-006).

## 6. Merge-readiness (SC-007) — serialized escalated order

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit                     # verdict=PASS for specs/062-...
```
Plus `SkillSyncCheck` / `TargetMetadataDrift` / `SkillQualityCheck` green after
every `.agents` edit + `RefreshSurfaceBaselines`, and the surface-baseline gates
for FR-010. All evidence under
`specs/062-space-invaders-consumer-friction-followups/readiness/` is real (no
`[S]`/`[S*]`, no unresolved diff-scan hits).
