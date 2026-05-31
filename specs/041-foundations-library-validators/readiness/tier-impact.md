# Change tier & impact (T003)

- **Change tier:** Tier 2 — internal refactor/extraction with no product behavioural or
  public-API change. Output parity is the explicit correctness contract.
- **Affected layer:** build-tooling only — `build/Governance/*` (four new compiled module
  pairs) and `build.fsx` (in-process wiring + typed `Targets.Target` dispatch). No `src/**`.
- **Public-API impact:** none on the product. New **build-tooling** `.fsi` are required by
  Principle II (`Findings`, `Targets`, `TargetMetadata`, `Capabilities`); no product `.fsi`,
  no shipped package, no surface baseline moves (FR-011).
- **Elmish/MVU applicability:** plugs into the *existing* `build.fsx` `update`/effect
  boundary — no new `Model`/`Msg`/`Effect`. `BuildMsg.StartTarget` changed payload from
  `string` to the typed `Targets.Target`; the extracted validators are pure over typed
  inputs and all I/O stays at the interpreter edge (Principle IV). The MEL engine relocation
  is Stage-5, out of scope (FR-001a).
- **Real-evidence obligations (this feature ships zero synthetic evidence):**
  - golden-diff parity = **0 bytes** for the three reports (SC-002/005) — `report-parity.md`;
  - **≥6 typed-finding cases** (3 catalog + 5 drift = 8) (SC-004) — `governance-tests.md`;
  - `git diff src/**` **empty** (SC-007) — `src-untouched.md`;
  - no new `PackageVersion` outside `Directory.Packages.props` (FR-010/FR-012);
  - build.fsx line-count delta recorded — `build-fsx-line-delta.md` (note the SC-001
    ≥800-line target is **not** met: realized shrink **385**; disclosed, not padded).
