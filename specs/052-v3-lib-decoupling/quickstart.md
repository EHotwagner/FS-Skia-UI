# Quickstart: V3 Stage 3–4 Residual — Decouple Remaining Consumers from `src/Lib`

## What this delivers

`src/Lib` reference-free: the rich keyboard-input runtime lives in a new `FS.Skia.UI.Input`
package, the `Parity.Tests` bridge + `Parity` helper are retired, and no sample references the
monolith — so Stage 5 can delete `src/Lib` and unpublish `FS.Skia.UI` without breaking anything.

## Implementation order (behaviour-preserving)

1. **Create the package.** Add `src/Input/Input.fsproj` (PackageId `FS.Skia.UI.Input`; refs
   `Scene` + `SkiaViewer`; net10 conventions via `Directory.Build.props`). Add it to the solution
   and `PackLocal`.
2. **Move the module.** `git mv src/Lib/KeyboardInput.fs(i) src/Input/`; change only the
   `namespace` line (`FS.Skia.UI` → `FS.Skia.UI.Input`). Confirm the body is otherwise unchanged
   (diff the `.fsi` modulo the namespace line).
3. **Capture the new baseline.** `./fake.sh build -t PerPackageSurfaceDiff` — record
   `readiness/per-package-surface/FS.Skia.UI.Input.fsi.txt`; the monolith baseline shrinks.
4. **Migrate tests.** New `tests/Input.Tests` with `KeyboardInputTests.fs` referencing
   `FS.Skia.UI.Input`; triage `Lib.Tests/Tests.fs`; drop `Package.Tests`'s conditional `Lib` ref.
5. **Repoint the sample.** `samples/InteractiveViewer`: drop `Lib.fsproj` + `FS.Skia.UI` pkg; add
   `FS.Skia.UI.Input` (project ref on source path, package ref on `UsePackedPackage` path).
6. **Parity sign-off + retire.** Confirm scene-output byte-identical to the Stage-0 golden; fold
   valuable `Parity.Tests` assertions into `SkiaViewer.Tests`/`Scene.Tests`; remove `Parity.Tests`;
   remove the `Parity` helper from `src/Lib/Library.fs(i)`.
7. **Settle `ParityGallery`** per ADR 0010 (retire with the bridge, recommended; or keep with
   rationale — it is already on `Scene`+`SkiaViewer`).

## Verify (escalated serialized order — FAKE-backed, never concurrent)

```
./fake.sh build -t Route                  # confirms escalation + the gate list to run
./fake.sh build -t Dev                    # full test suite incl. Input.Tests
./fake.sh build -t PerPackageSurfaceDiff  # new + shrunk baselines clean
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit          # PASS, zero synthetic
```

## Done when

- `grep -rn -E "Lib\.fsproj|Include=\"FS\.Skia\.UI\"" samples tests src --include=*.fsproj`
  returns nothing outside `src/Lib` itself.
- `tests/Parity.Tests` removed; the deterministic scene-output check is byte-identical to the
  Stage-0 golden.
- `src/Lib` still present + `FS.Skia.UI` still packable (Stage 5 owns deletion/unpublish).
- Escalated gate set green; `EvidenceAudit` PASS, zero synthetic.
