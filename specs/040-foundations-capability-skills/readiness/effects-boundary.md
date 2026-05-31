# MVU / Effect Boundary — 040 (Principle IV)

The two new gates **plug into the existing `build.fsx` Elmish/MVU engine**; no
new `Model`/`Msg` algebra is introduced.

- **`update`** gains two `StartTarget` arms (`SkillSyncCheck`,
  `SkillExamplesCheck`) that return new effects — no I/O performed in `update`.
- **`BuildEffect`** gains `SkillSyncGate` and `SkillExamplesGate`.
- **The interpreter** (`interpret`) executes them at the edge:
  `runSkillSyncGate` (read bytes → SHA-256 → compare → write report → `FailWith`
  on drift) and `runSkillExamplesGate` (regenerate `Generated/*.fs` → `dotnet
  build` → map diagnostics → write report).
- The **pure core** — `sha256Hex`, `inSync`, `drifted`, `extractBlocks`,
  `renderSkillFile`, `moduleName` — performs no I/O and is unit/property-tested
  in `tests/Governance.Tests` independent of the edge.

In-process-first (per `fsharp-shell-process`): hashing and extraction are
in-process F#; only the real `dotnet build` of the examples project shells out,
which is irreducible.

Per-task Principle IV applicability: the skill-authoring tasks (T010–T015,
T026) are pure content with no workflow/I-O — Principle IV is **Not Applicable**.
The gate tasks (T018–T020, T023–T025) are handled through the boundary above.
