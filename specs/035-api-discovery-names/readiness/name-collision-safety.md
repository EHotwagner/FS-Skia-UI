# Name Collision Safety Evidence

Status: pass.

Red evidence:

- T006 added collision inventory tests in
  `tests/Package.Tests/NameCollisionSafetyTests.fs`.
- Focused command:
  `dotnet test tests/Package.Tests/Package.Tests.fsproj -m:1 --logger "console;verbosity=minimal"`.
- Expected red log:
  `specs/035-api-discovery-names/readiness/logs/t006-collision-inventory-red.txt`.
- Red failure class: missing structured collision decision records and explicit
  Scene/Controls qualification guidance in this readiness report.

| Name | Scene owner | Controls owner | Symbol kind | Risk | Decision | Guidance | Validation scenario |
|------|-------------|----------------|-------------|------|----------|----------|---------------------|
| `Text` | `FS.Skia.UI.Scene.TextRun.Text` and `SceneElementKind.TextElement` | `FS.Skia.UI.Controls.KnownAttribute.Text`, `TextBlock.text`, `Button.text` | record field / union case / builder helper | risk: open-order-sensitive | decision: consumer-guidance | Use `FS.Skia.UI.Scene.TextRun` for scene text and `FS.Skia.UI.Controls.TextBlock.create` or `FS.Skia.UI.Controls.Button.text` for controls text. | validation: mixed-scene-controls-open-scene-first |
| `Width` | `FS.Skia.UI.Scene.Rect.Width`, `Size.Width`, `Stroke.Width` | layout/control sizing attributes | record field / attribute helper | risk: open-order-sensitive | decision: consumer-guidance | Use `FS.Skia.UI.Scene.Rect` for geometry records and Controls sizing helpers through the `FS.Skia.UI.Controls` module that owns the control. | validation: mixed-scene-controls-open-controls-first |
| `Height` | `FS.Skia.UI.Scene.Rect.Height`, `Size.Height`, `TextMetrics.Height` | layout/control sizing attributes | record field / attribute helper | risk: open-order-sensitive | decision: consumer-guidance | Use `FS.Skia.UI.Scene.Rect` or `FS.Skia.UI.Scene.Size` for scene geometry and qualify Controls layout helpers. | validation: mixed-scene-controls-open-controls-first |
| `Color` | `FS.Skia.UI.Scene.Color`, `Paint.Fill`, `Shader.SolidColor` | Controls accessibility and theme color values | type / attribute value | risk: open-order-sensitive | decision: consumer-guidance | Use `FS.Skia.UI.Scene.Paint` and `FS.Skia.UI.Scene.Color` for scene paint values; keep Controls theme/accessibility values qualified by their Controls owner. | validation: mixed-scene-controls-open-scene-first |
| `Changed` | scene input/update messages in consumer code | `FS.Skia.UI.Controls.KnownEvent.Changed`, `TextBox.onChanged` | event case / builder helper | risk: open-order-sensitive | decision: contract-qualified | Controls event discriminators remain `[<RequireQualifiedAccess>]`; generated samples use `FS.Skia.UI.Controls.TextBox.onChanged`. | validation: mixed-scene-controls-open-controls-first |
| `children` | scene/group construction concepts | `FS.Skia.UI.Controls.Stack.children` | builder helper | risk: open-order-sensitive | decision: consumer-guidance | Use `FS.Skia.UI.Controls.Stack.children` for Controls layout and explicit Scene constructors for scene groups. | validation: mixed-scene-controls-open-scene-first |
| `create` | product-local scene builder helpers | `FS.Skia.UI.Controls.TextBlock.create`, `DataGrid.create`, `LineChart.create` | builder helper | risk: open-order-sensitive | decision: consumer-guidance | Generated examples use fully qualified Controls builders such as `FS.Skia.UI.Controls.TextBlock.create`. | validation: mixed-scene-controls-open-controls-first |

decision: contract-qualified
decision: consumer-guidance
compatibility: no-contract-change
compatibility: signature-change-reviewed

Qualification samples:

- `FS.Skia.UI.Scene.Rect`
- `FS.Skia.UI.Scene.Paint`
- `FS.Skia.UI.Scene.TextRun`
- `FS.Skia.UI.Controls.TextBlock.create`
- `FS.Skia.UI.Controls.TextBox.onChanged`
- `FS.Skia.UI.Controls.Stack.children`

Guidance paths:

- `template/base/docs/product.md`
- `template/base/README.md`
- `template/fragments/controls/README.md`
- `docs/generated-apps.md`
- `specs/035-api-discovery-names/readiness/fsi/controls-adjacent-authoring.fsx`
- `specs/035-api-discovery-names/readiness/fsi/mixed-scene-controls-open-scene-first.fsx`
- `specs/035-api-discovery-names/readiness/fsi/mixed-scene-controls-open-controls-first.fsx`

Surface baseline paths reviewed:

- `readiness/surface-baselines/FS.Skia.UI.Scene.txt`
- `readiness/surface-baselines/FS.Skia.UI.Controls.txt`

Contract decision:

- No new public `.fsi` changes were selected. Existing Controls discriminated
  unions for known controls, events, standard event kinds, and standard
  attribute names already carry `[<RequireQualifiedAccess>]` in
  `src/Controls/Types.fsi`.
- The remaining overlap risk is handled through explicit consumer guidance and
  fully qualified generated examples.
- Corresponding `.fs` body changes were not required because no new public
  contract member or attribute was added.

Compile scenario result:

- `dotnet fsi specs/035-api-discovery-names/readiness/fsi/mixed-scene-controls-open-scene-first.fsx`
  passed and wrote
  `specs/035-api-discovery-names/readiness/fsi/mixed-scene-controls-open-scene-first.log`.
- `dotnet fsi specs/035-api-discovery-names/readiness/fsi/mixed-scene-controls-open-controls-first.fsx`
  passed and wrote
  `specs/035-api-discovery-names/readiness/fsi/mixed-scene-controls-open-controls-first.log`.
- `dotnet test tests/Package.Tests/Package.Tests.fsproj --no-restore --logger "console;verbosity=minimal"`
  passed: 33 tests.

Next action: rerun `GeneratedGuidanceCheck`, `TemplateCheck`, and
`GeneratedProductCheck` during integration after US3 guidance-classification
work is complete.
