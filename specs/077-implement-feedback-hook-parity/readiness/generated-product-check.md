# Generated product check (feature 077, SC-005)

- **Authoritative command**: `./fake.sh build -t GeneratedProductCheck`.
- **Artifact**: `readiness/generated-product-check.md` (this file).
- **Failure class**: **non-authoritative** local environment failure (the generated `Verify`
  cannot resolve a feature: no template `feature.json`; `Map.empty` env). Pre-existing, not
  introduced by feature 077. See `aggregate-hang-diagnostics.md`.
- **Next action**: rely on `TemplateCheck` / `GeneratedGuidanceCheck` + CI for the
  propagation proof, never on a local `GeneratedProductCheck` "pass".

## Result

**Non-authoritative FAIL (as expected).** `./fake.sh build -t GeneratedProductCheck` failed with
the known local-environment cause (verbatim from
`readiness/generated-product-verify/app-source/verify.log`):

> Cannot resolve the feature to validate: no SPECKIT_FEATURE_DIR override is set and
> `artifacts/generated-products/077-implement-feedback-hook-parity/app-source/.specify/feature.json`
> has no usable "feature_directory" entry.

This is the pre-existing environment limitation (no template `feature.json` / `Map.empty` env),
**not** a feature-077 defect — it reproduces on any feature locally. It is recorded as
non-authoritative in `aggregate-hang-diagnostics.md` and is **never** used to claim a hard pass.
The authoritative propagation proof for SC-005 is `TemplateCheck` (`Status: Ok`, corrected skills
present in generated output — see `template-check.md`) plus `GeneratedGuidanceCheck` and CI.
