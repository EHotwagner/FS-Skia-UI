# Skillist Currency: no historical regression (SC-004)

The reframed skillist comparison keeps the feature-043 **active-feature scope** unchanged:
the evidence engine resolves the active feature from `.specify/feature.json`
(`specs/044-foundations-single-source-generation`) and reads **only** that feature's
`tasks.md` + `tasks.deps.yml`. The ~43 historical feature directories are **never
re-derived**, so the reframe cannot introduce a new failure for any feature whose
representations already agree.

## Proof — active-feature-scoped, baseline green

```
$ cat .specify/feature.json
{ "feature_directory": "specs/044-foundations-single-source-generation" }

$ ./fake.sh build -t EvidenceGraph    # exit 0 — PASS
$ ./fake.sh build -t EvidenceAudit    # exit 0 — PASS (active feature 044 only)
```

The only behavior change for features whose `tasks.md` view already matches their
`tasks.deps.yml` canonical is the **diagnostic wording** (symmetric peer complaint →
asymmetric currency message), which is emitted **only on drift**. Because the engine does
not read historical feature directories at all, re-deriving across the existing feature
set yields **zero new failures**.

This is structural, not incidental: the active-feature resolution is the same code path the
043 engine already used; feature 044 changed the *message* and *framing* of the on-drift
diagnostic, not the *set of features compared*.

**Verdict: PASS** — zero historical regression; the currency check is active-feature scoped
by construction (SC-004).
