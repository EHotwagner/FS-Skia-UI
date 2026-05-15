# Package Shape Review

Evidence:

- Template package: `artifacts/templates/FS.Skia.UI.Template.0.1.0-preview.1.nupkg`
- Package content report: `specs/007-v2-template-packaging/readiness/template/template-package-contents.md`
- Generated scan report: `specs/007-v2-template-packaging/readiness/template/generated-project-scans.md`

Verdict: PASS.

The package content verifier confirmed the required template-owned entries:

- `content/.template.config/template.json`
- `content/build.fsx`
- `content/src/Lib/Lib.fsproj`
- `content/docs/template-profile.md`
- `content/Directory.Packages.props`

The same `TemplateSmoke` matrix passed for source-directory and packaged installs:

| Artifact | Profile | Verdict |
|----------|---------|---------|
| source | default | PASS |
| source | minimal | PASS |
| package | default | PASS |
| package | minimal | PASS |

The package and source rows both completed placeholder scans, excluded-history
scans, optional minimal-profile checks, and generated `./fake.sh build -t Dev`
runs. The generated projects exclude source-only `specs/00*` history and do not
carry `.template.config/` or `.template.package/` metadata after instantiation.
