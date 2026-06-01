# Surface-area baselines & scope (T008)

## New build-tooling surface (`build/Governance/Evidence`)

Ten curated `.fsi` modules (governance/internal-tooling surface, **not** product
public contract — no product surface-baseline diff, Invariant 1):

`TaskParser`, `DepsParser`, `SkillRegistry`, `Graph`, `StatusRegion`, `Scans`,
`DiffScan`, `Audit`, `Render`, `Engine`. Each `.fs` carries no access modifiers;
visibility lives in the `.fsi` (Principle II). No `FSharp.Compiler.*` and no new
runtime dependency (`YamlDotNet`/`Fake.Core.Target` already central).

## Failure handling (unsupported scope)

- A `Graph` that fails to compute (cycles, dangling refs, self-deps, unresolved
  skills, mirror mismatch, missing skill-loading evidence) returns
  `GraphResult.Verdict = Error`; the `build.fsx` interpreter `failwith`s on that
  arm, preserving the Python non-zero-exit semantics (spec Edge Cases). Verified:
  feeding a cyclic graph yields a non-empty `detectCycles` result and `verdict=error`.
- The merge-gate audit returns `AuditVerdict.Fail` when `TotalBlockers > 0`; the
  gate `failwith`s. `--accept-synthetic` is not implemented as a verdict override
  (it never changes the verdict; Principle V).

## Deferred scope (out of this feature)

Stages 2.2–2.5 / 5 / 6 / 7; the heavy Spec Kit Bash (`common.sh`, git scripts);
the V3 modular package split. Per FR-011/FR-012 the `--legacy-evidence` selector
and the Python files are removed **in this feature** at parity sign-off (T029) —
not deferred.
