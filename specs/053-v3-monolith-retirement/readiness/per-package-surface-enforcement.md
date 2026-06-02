# PerPackageSurfaceDiff enforcement proof (SC-004, real reverted edit)

`PerPackageSurfaceDiff` is now a Route-gated requirement of the `package-surface` rule
(`build/Governance/Routing.fs`), is on the `knownGates` allowlist
(`build/Governance/AgentValidation.fs`), and is rendered into
`validation.contract.yml`'s `package-surface` `required_gates` (TargetMetadataDrift green,
zero drift vs `Routing.fs`).

## 1. Baseline — zero drift

```bash
./fake.sh build -t PerPackageSurfaceDiff
```
→ `Finished (Success)` — nine in-scope packages, zero drift at the pin.

## 2. Real reverted `.fsi` edit fails the gate naming the drifted package

A real one-line public surface addition to one package's `.fsi` (`src/Scene/Scene.fsi`,
`val driftProbe: int`, paired with a `let driftProbe = 0` in `src/Scene/Scene.fs` so the
build is honest), **without** updating the baseline:

```bash
./fake.sh build -t PerPackageSurfaceDiff
```
→ `Finished (Failed)`:
```
PerPackageSurfaceDiff: surface drift detected (drifted: [FS.Skia.UI.Scene]; missing baselines: []).
See readiness/per-package-surface-diff.md and update readiness/per-package-surface/<PackageId>.fsi.txt.
```
The diff names the package and the exact added line:
```diff
    +     val driftProbe: int
```

## 3. Recording the baseline clears it

Appending the captured line to `readiness/per-package-surface/FS.Skia.UI.Scene.fsi.txt` and
re-running:

```bash
./fake.sh build -t PerPackageSurfaceDiff
```
→ `Finished (Success)` — the recorded baseline matches the new surface.

## 4. Revert

```bash
git checkout -- src/Scene/Scene.fsi src/Scene/Scene.fs readiness/per-package-surface/FS.Skia.UI.Scene.fsi.txt
```
`grep -c driftProbe` over the three files → `0`; `git status` clean. The gate is back at
zero drift.

failure class: PackageSurfaceDrift. next action: none — the gate bites on an unrecorded
per-package `.fsi` change and clears on a recorded baseline (SC-004). authoritative command:
`./fake.sh build -t PerPackageSurfaceDiff`. Real evidence — no mock, the edit and baseline
were real files reverted after the proof.
