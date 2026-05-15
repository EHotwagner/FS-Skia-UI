# US1 Real Interpreter Evidence Plan

`TemplateCheck` exercises the operator-facing template workflow through real
process and filesystem effects:

- source install with `dotnet new install .`
- package install with `dotnet new install artifacts/templates/FS.Skia.UI.Template.*.nupkg`
- default profile generation from the source artifact
- minimal profile generation from the source artifact
- default profile generation from the package artifact
- minimal profile generation from the package artifact
- placeholder and excluded-history scans for all rows
- generated Dev execution through `./fake.sh build -t Dev` for all rows

Evidence is written under
`specs/007-v2-template-packaging/readiness/template/`.
