# Baseline Status

Recorded at: 2026-05-28T08:03:45+02:00

| Command | Result | Evidence |
|---------|--------|----------|
| `./fake.sh build -t Verify` | FAIL baseline: `VerifyPreflight` reports missing readiness artifacts `public-surface.md`, `package-boundary.md`, `generated-product-usage.md`, and `compatibility-impact.md` for this active feature. | `readiness/logs/t002-verify.txt` |
| `./fake.sh build -t GeneratedGuidanceCheck` | PASS | `readiness/logs/t002-generated-guidance-check.txt` |
| `./fake.sh build -t TemplateCheck` | PASS | `readiness/logs/t002-template-check.txt` |

The `Verify` result is preserved as a pre-implementation baseline. Later tasks
must either produce the missing readiness artifacts or continue to report this
focused failure separately from broader aggregate results.
