# Generated Guidance Check

Status: focused validation pending sequential FAKE run.

Reviewed guidance surfaces:

- `docs/generated-apps.md`
- `docs/template-profile.md`
- `template/base/README.md`
- `template/base/docs/product.md`

Required guidance now present:

- current feature readiness paths are authoritative for current gates
- historical feature readiness is audit context only unless a current evidence map explicitly marks it as supporting evidence
- Archived material must not be cited as current package, template, generated-product, or audit pass/fail evidence
- source-shaped `.fsi` package API reference remains authoritative for agent authoring
- FSharp.Formatting/fsdocs output is secondary or hybrid unless the decision record marks it authoritative
- Package consumers must not use assembly reflection or repository source inspection as an authoring substitute

replacement instruction: when active guidance cites a historical readiness path as current evidence, replace the citation with `specs/036-archive-readiness-api-docs/readiness/current-evidence-map.md` or the current readiness path named by that map.
