# Product

This generated product references selected FS.Skia.UI capabilities instead of
copying the framework repository.

The selected capabilities are controlled by `--profile`:

- `app`: Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Charts
- `headless-scene`: Scene
- `governed`: Scene, Testing
- `sample-pack`: Scene, SkiaViewer, Elmish, Samples

## Quickstart

Run the generated product governance checks:

```bash
./fake.sh build -t Dev
./fake.sh build -t Test
./fake.sh build -t Verify
```

The product references FS.Skia.UI preview packages from the configured NuGet
sources. For local framework development, pack the source repository with
`./fake.sh build -t PackLocal` and add `~/.local/share/nuget-local` as a NuGet
source before restoring or running this generated project.

The product owns its application code, tests, documentation, readiness evidence,
and selected local skills.
