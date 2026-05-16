# Generated Product Usage

## Verdict

PASS for generated product usage evidence.

## Evidence

- `./fake.sh build -t GeneratedProductCheck` passed.
- `./fake.sh build -t TemplateCheck` passed.
- Source and package generated app profiles reference `FS.Skia.UI.Controls`.
- Generated app products include `.agents/skills/fs-skia-ui-widgets/SKILL.md`.
- Generated app products do not receive `fs-skia-charts` or generated
  `fs-skia-layout` widget guidance skills.
- Generated products exclude framework samples, galleries, historical specs,
  readiness evidence, framework docs, framework README copy, and implementation
  projects.

## Product-Owned Example

`template/base/src/Product/Program.fs` contains `controlsExampleView`, a
product-owned view composed from `Stack`, `TextBlock`, `TextBox`, and `Button`.
`template/base/tests/Product.Tests/Tests.fs` verifies the generated product test
suite can count the view through `FS.Skia.UI.Controls.Control.count`.

## Command Logs

`readiness/generated-product-verify/*/{dev,test,verify}.log` all end with
`exit-code=0` for the app source/package, headless-scene source, governed
source, and sample-pack source rows.
