# GeneratedProductCheck (T043) — non-authoritative local env-failure

command: `./fake.sh build -t GeneratedProductCheck`

## Authoritative outcome: PASS (build + tests)

The generated `app` (controls family) product was generated, restored, **built**, and its
**29 tests passed** against the **pinned** `FS.Skia.UI.* 0.1.91-preview.1` packages:

```
Passed!  - Failed: 0, Passed: 29, Skipped: 0, Total: 29 - Product.Tests.dll (net10.0)
exit-code=0  (app/source generated Test)
```

This includes the rewritten neutral BehaviorTests and the SC-003 pointer-dispatch test
(which uses only 085-available APIs — `Control.renderTree` + `Layout.evaluate` on
`rendered.Layout` — NOT the 086-only `ControlRenderResult.Bounds`, since the generated
product builds against the published 085 packages, not the local 086 src).

## Real regression caught and fixed by this gate

The first run FAILED to compile: the generated BehaviorTests used `rendered.Bounds`
(an 086 framework field absent from the pinned 085 `FS.Skia.UI.Controls`). Fixed by
resolving the control box via `FS.Skia.UI.Layout.Layout.evaluate available rendered.Layout`
(085 API). The generated product now compiles and its tests pass.

## Non-authoritative failure (known, documented)

The overall target then fails at the generated `Verify` step with:

```
Cannot resolve the feature to validate: no SPECKIT_FEATURE_DIR override is set and
.../app-source/.specify/feature.json has no usable "feature_directory" entry.
```

This is the documented local env-failure: the template intentionally **excludes**
`.specify/feature.json` from generation (`.template.config/template.json` sources exclude),
so a freshly generated product has no feature to validate without `SPECKIT_FEATURE_DIR`.
It is an **environment/template-structure** limitation, **not a product defect** and **not**
a regression from this feature. Verdict for T043: authoritative build+tests PASS; the
Verify feature-resolution failure is recorded as non-authoritative.
