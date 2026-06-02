# Quickstart — V3 Stage 5 Closeout (feature 053)

How to validate this feature end-to-end. FAKE-backed commands share `.fake` state —
run them **sequentially**, never concurrently.

## 0. Route (authoritative gate list)

```
./fake.sh build -t Route            # confirms escalation; --enforce names missing evidence
```
This change escalates (touches governance `Routing.fs`, public-`.fsi` routing, the
pack flow, and dependency docs). Run exactly the gates `Route` prints.

## 1. No-consumer proof (SC-001)

```
grep -rn --include='*.fsproj' --include='*.fs' --include='*.fsx' \
  --include='*.sln' --include='*.props' \
  -E 'Lib\.fsproj|src/Lib|"FS\.Skia\.UI"' \
  src samples tests template build *.sln Directory.Packages.props
```
Expect **zero** hits (programme history under `docs/`/`specs/` is excluded). Confirm
`src/Lib` is gone and absent from `FS-Skia-UI.sln`.

## 2. Per-package surface enforcement (SC-004)

```
./fake.sh build -t PerPackageSurfaceDiff          # green at zero drift
# prove the gate bites:
#   edit one line in e.g. src/Scene/<a public .fsi>, do NOT update the baseline
./fake.sh build -t PerPackageSurfaceDiff          # FAILS, naming the drifted package
#   record it:
#   regenerate readiness/per-package-surface/FS.Skia.UI.Scene.fsi.txt
./fake.sh build -t PerPackageSurfaceDiff          # PASS
#   revert both the .fsi edit and the baseline
./fake.sh build -t TargetMetadataDrift            # contract current vs Routing.fs
```
Confirm `validation.contract.yml`'s `package-surface` rule lists
`PerPackageSurfaceDiff` in `required_gates`.

## 3. Generated-project cleanliness (SC-005 / SC-006)

```
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck          # cleanliness assertions included
```
A generated default `app` restores/builds/runs referencing split packages only, with
no `samples/` / framework docs / `specs/` / framework README copy. Planting any of
those makes the gate fail naming the artifact.

## 4. Closeout docs

- `docs/migration/v2-to-v3.md` — surface map + ref-move steps + `SceneConversion`
  removal note + rich keyboard-input → `FS.Skia.UI.Input` mapping (FR-009).
- `docs/adr/0012-monolith-retirement-closeout.md` — Accepted; links ADRs 0007–0011
  (FR-011).
- `docs/reports/_baselines/2026-06-02-v3-after.md` — `src/Lib` LOC→0, duplicate-type
  count→0, transitive-pull→none, each with its reproduction command (FR-010, SC-007).

## 5. Full escalated gate sequence (SC-009)

```
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit          # PASS, zero synthetic
```
`Dev` is green with zero `Lib` references; `EvidenceAudit` returns PASS with no
`[S]`/`[S*]` and no diff-scan hit.

## Notes / gotchas (carried from prior stages)

- **Path-string sweep, not just symbol grep** (Stage-2 lesson): a deleted file is
  referenced by *path* in packable-fsi enumerations and routing-test inputs — grep the
  **path** as well as the symbol. See `research.md` R1 for the ~14 call sites.
- **The rule + `knownGates` entry + regenerated contract must land together** or
  `TargetMetadataDrift` / the contract validator fails.
- **Headless screenshot re-capture is infeasible** — the deterministic scene-output
  oracle is authoritative (Principle V infeasibility disclosure, not synthetic).
- **`EvidenceAudit` readiness-contract enforces verbatim vocabulary** in the
  per-feature readiness notes — avoid markdown line-wraps that split a required phrase.
