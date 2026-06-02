# ParityGallery / scene-output oracle policy (FR-012, per ADR 0010 / 0011)

## Oracle preserved

The deterministic scene-output parity oracle is **preserved**, not re-derived. It
lives in the Scene-only `tests/Parity.Tests` (`SceneOutput.fs` / `SceneOutputTests.fs`,
referencing just `src/Scene/Scene.fsproj`); the old-vs-new monolith bridge `Tests.fs`
was retired in feature 052. Re-derive byte-identically (authoritative):

```bash
dotnet test tests/Parity.Tests/Parity.Tests.fsproj --filter "FullyQualifiedName~scene-output"
```

The reference-frame screenshot re-capture remains **headless-GPU-infeasible** and is
disclosed as a Principle V infeasibility (corroboration-only — the scene-output oracle
is authoritative). It is **not** synthetic evidence.

## `samples/ParityGallery` keep-vs-retire

Per ADR 0010 (legacy sample policy), `samples/ParityGallery` is **kept** on
`Scene` + `SkiaViewer` (monolith-free; confirmed repointed in feature 052). It is not
retired this stage — it is a live, split-package-only sample. The no-consumer grep
confirms it names no `Lib.fsproj` / `src/Lib` / `"FS.Skia.UI"`.

## Stale governance-list cleanup

Governance scanning lists that named `tests/Parity.Tests` only where they assumed the
**retired old-vs-new bridge** were reviewed. The remaining `tests/Parity.Tests` mention
in `GeneratedProductCheck`'s forbidden-copy list is a *legitimate* forbidden-copy guard
(a generated product must not copy the framework's Parity test project), not a
bridge assumption, so it is retained. No oracle, fixture, or golden is removed.

failure class: ParityOracleDrift. next action: none — oracle preserved; policy recorded.
artifact path: this file. authoritative command: the scene-output test filter above.
