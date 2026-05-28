# Generated Product Usage

Evidence:

- `readiness/template-version-alignment.md`
- `readiness/template/generated-project-scans.md`
- `readiness/template/verdict.md`
- `./fake.sh build -t TemplateCheck`

Result: generated `app`, `governed`, `headless-scene`, and `sample-pack`
profiles were instantiated and validated through source and packaged template
paths. Focused generated profiles do not add a broad `FS.Skia.UI` dependency.

