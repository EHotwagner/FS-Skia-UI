# SC-007 — unmigrated kinds are unchanged (no render-output delta)

The migration is **additive and partial**: only `Button` and `CheckBox` route
through the resolver. Every other kind is byte-for-byte unchanged.

## Evidence (real, in-repo)

- **Regression test** — `tests/Controls.Tests/Feature093ParityTests.fs`: an
  unmigrated kind (`switch`) renders **identically** whether or not a
  `styleClasses` attribute is attached — it ignores the class, proving no
  behavior change leaked into the unmigrated path.
- **Full suite** — `dotnet test tests/Controls.Tests/Controls.Tests.fsproj` →
  **213/213 green**, including the 080/085/086 preview-parity guards, the 091/092
  retained byte-identity invariants, the typed-lowering/migration parity suites,
  and `PublicSurfaceTests`. None regressed under the new types/attrs.
- **Consumer contract unchanged** — `view : 'model -> Control<'msg>` is
  untouched; a consumer who attaches no class (`Classes = []`, no `visualState`)
  gets byte-identical lowering and render.

## Result

PASS — unmigrated kinds show no render-output delta; the migration is additive.
